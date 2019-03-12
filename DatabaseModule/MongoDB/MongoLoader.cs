//using csmatio.io;
//using csmatio.types;
using DatabaseModule.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace DatabaseModule.MongoDB
{
    public class MongoLoader
    {
        private IMongoDatabase Database { get; set; }
        private IMongoCollection<BsonDocument> Collection { get; set; }
        private bool Profinet { get; set; }
        private string Folder { get; set; }
        private string FileName { get; set; }
        private StringBuilder LocalStringBuilder { get; set; }
        private SortMeasurementProfinet SortedMeasurementProfinet { get; }
        private SortMeasurementEthernet SortedMeasurementEthernet { get; }

        public MongoLoader(string databaseLocation, string database, string document, string profinet, string folder, string fileName)
        {
            Profinet = int.Parse(profinet) == 1;
            Folder = folder;
            SortedMeasurementProfinet = new SortMeasurementProfinet();
            SortedMeasurementEthernet = new SortMeasurementEthernet();

            var client = new MongoClient(MongoUrl.Create(databaseLocation));
            Database = client.GetDatabase(database);
            Collection = Database.GetCollection<BsonDocument>(document);

            var source = int.Parse(profinet) == 1 ? "_profinet_" : "_ethernet_";
            FileName = Folder + "/" + fileName + source + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-FFF");
        }

        public async Task ReadData()
        {
            using (IAsyncCursor<BsonDocument> cursor = await Collection.FindAsync(new BsonDocument()))
            {
                var measuredData = new List<double[]>();
                LocalStringBuilder = new StringBuilder();
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;

                    foreach (BsonDocument document in batch)
                    {
                        measuredData.Add(BsonDocToList(document));
                    }
                }
                SaveToFile(measuredData);
                File.WriteAllTextAsync(FileName + ".csv", LocalStringBuilder.ToString()).Wait();
            }


        }

        private double[] BsonDocToList(BsonDocument document)
        {
            document.Remove("_id");
            if (Profinet)
            {
                var measurement = JsonConvert.DeserializeObject<MeasuredVaribles>(document.ToJson());
                SortedMeasurementProfinet.AddToList(measurement);
                var values = measurement.GetMeasuredValues().ToArray();
                ToCsvFile(measurement);
                return values;
            }
            var measuredData = JsonConvert.DeserializeObject<TcpRobot>(document.ToJson());
            SortedMeasurementEthernet.AddToList(measuredData);
            measuredData.ToList();
            ToCsvFile(measuredData);
            return measuredData.FilePreparation().ToArray();
        }

        private void Save()
        {
            if (Profinet)
            {

            }
            else
            {

            }
        }

        private void SaveToFile(List<double[]> measuredData)
        {
            //ToMatFile(measuredData);
        }

        //private void ToMatFile(List<double[]> measuredData)
        //{
        //    var mMatrix = new MLDouble("Measurement", measuredData.ToArray());
        //    var mList = new List<MLArray>();
        //    mList.Add(mMatrix);
        //    var mFileWrite = new MatFileWriter(FileName + ".mat", mList, false);
        //}

        private void ToCsvFile(TcpRobot measuredData)
        {
            var dateTime = measuredData.Time.GetDate().ToString("yyyy-MM-dd-HH-mm-ss-FFF");
            var prNumber = ((int)measuredData.ProgramNumber.Value).ToString();
            var begin = string.Join(", ", dateTime, prNumber);
            var newLine = string.Join(", ", measuredData.FilePreparation().ToArray().Select(element => element.ToString(CultureInfo.InvariantCulture)).ToArray());
            measuredData.Called = true;
            LocalStringBuilder.AppendLine(begin + ", " + newLine);
        }

        private void ToCsvFile(MeasuredVaribles measuredData)
        {
            var dateTime = measuredData.SaveTime;
            var prNumber = (measuredData.ProgramNumber).ToString();
            var begin = string.Join(", ", dateTime, prNumber);
            var newLine = string.Join(", ", measuredData.GetMeasuredValues().ToArray().Select(element => element.ToString(CultureInfo.InvariantCulture)).ToArray());
            LocalStringBuilder.AppendLine(begin + ", " + newLine);
        }
    }
}
