using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace DatabaseModule.Extensions
{

    public static class StringExtensions
    {
        /// <summary>
        /// Deserializes the XML data contained by the specified System.String
        /// </summary>
        /// <typeparam name="T">The type of System.Object to be deserialized</typeparam>
        /// <param name="s">The System.String containing XML data</param>
        /// <returns>The System.Object being deserialized.</returns>
        public static T XmlDeserialize<T>(this string s)
        {
            var locker = new object();
            var stringReader = new StringReader(s);
            var reader = new XmlTextReader(stringReader);
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                lock (locker)
                {
                    var item = (T)xmlSerializer.Deserialize(reader);
                    reader.Close();
                    return item;
                }
            }
            catch
            {
                return default(T);
            }
            finally
            {
                reader.Close();
            }
        }

        public static double[] ToDoubleArray(this string[] array, int elements)
        {
            return array.Select(element => double.Parse(element, CultureInfo.InvariantCulture)).Take(elements).ToArray();
        }

        public static double TimeInSecond(this string time)
        {
            return DateTime.ParseExact(time, "yyyy-MM-dd-HH-mm-ss-FFF", CultureInfo.InvariantCulture).ToUniversalTime().Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            ).TotalMilliseconds;
        }
    }
}
