using System;
using System.Collections.Generic;
using System.Text;

namespace KunbusRevolutionPiModule.Conversion
{
    public static class ByteExtension
    {
        public static object OutputConversion(this byte[] input, dynamic convertTo)
        {
            dynamic output;
            if (convertTo == typeof(UInt64))
            {
                output = BitConverter.ToUInt64(input, 0);
            }
            else if (convertTo == typeof(UInt32))
            {
                output = BitConverter.ToUInt32(input, 0);
            }
            else if (convertTo == typeof(UInt16))
            {
                output = BitConverter.ToUInt16(input, 0);
            }
            else if (convertTo == typeof(Int64))
            {
                output = BitConverter.ToInt64(input, 0);
            }
            else if (convertTo == typeof(Int32))
            {
                output = BitConverter.ToInt32(input, 0);
            }
            else if (convertTo == typeof(Int16))
            {
                output = BitConverter.ToInt16(input, 0);
            }
            else if (convertTo == typeof(byte))
            {
                output = input[0];
            }
            else
            {
                throw new InvalidCastException("Unknown format to convert to");
            }

            return output;
        }
    }
}
