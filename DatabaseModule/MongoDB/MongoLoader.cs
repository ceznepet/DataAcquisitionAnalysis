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
        private bool Sorted { get; set; }
        private string Folder { get; set; }
        private string FileName { get; set; }
        private StringBuilder LocalStringBuilder { get; set; }
        private SortMeasurementProfinet SortedMeasurementProfinet { get; }
        private SortMeasurementEthernet SortedMeasurementEthernet { get; }
        private List<double[]> MeasuredData { get; set; }

        public MongoLoader(string databaseLocation, string database, string document, string profinet, string folder, string fileName, bool sorted)
        {
            Profinet = int.Parse(profinet) == 1;
            Sorted = sorted;
            Folder = folder;
            SortedMeasurementProfinet = new SortMeasurementProfinet();
            SortedMeasurementEthernet = new SortMeasurementEthernet();
            MeasuredData = new List<double[]>();

            var client = new MongoClient(MongoUrl.Create(databaseLocation));
            Database = client.GetDatabase(database);
            Collection = Database.GetCollection<BsonDocument>(document);

            var source = int.Parse(profinet) == 1 ? "_profinet_" : "_ethernet_";
            FileName = Folder + "/" + fileName + source + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-FFF");
        }

        public async Task ReadData()
        {
            using (var cursor = await Collection.FindAsync(new BsonDocument()))
            {
                var measuredData = new List<double[]>();
                LocalStringBuilder = new StringBuilder();
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;

                    foreach (var document in batch)
                    {
                        BsonDocToList(document);
                    }
                }
                //ToMatFile(measuredData);
                File.WriteAllTextAsync(FileName + ".csv", LocalStringBuilder.ToString()).Wait();
                Save();
            }


        }

        private void BsonDocToList(BsonDocument document)
        {
            document.Remove("_id");
            if (Profinet)
            {
                var measurement = JsonConvert.DeserializeObject<MeasuredVariables>(document.ToJson());
                if (Sorted)
                {
                    SortedMeasurementProfinet.AddToList(measurement);
                }
                else
                {
                    MeasuredData.Add(measurement.GetMeasuredValues().ToArray());
                    ToCsvFile(measurement);
                }
            }
            var measuredData = JsonConvert.DeserializeObject<TcpRobot>(document.ToJson());
            if (Sorted)
            {
                SortedMeasurementEthernet.AddToList(measuredData);
            }
            else
            {
                measuredData.ToList();

                ToCsvFile(measuredData);
                MeasuredData.Add(measuredData.FilePreparation().ToArray());
            }           
        }

        private void Save()
        {
            if (Profinet)
            {
                SaveProfinet();
            }
            else
            {
                SaveEthernet();
            }
        }

        private void SaveProfinet()
        {
            SortedMeasurementProfinet.SortList();
            var sortedProfinet = SortedMeasurementProfinet.Dictionary;

            foreach (var key in sortedProfinet.Keys)
            {
                PrintOneProgramProfinet(key, sortedProfinet[key]);
            }
        }

        private void SaveEthernet()
        {
            SortedMeasurementEthernet.SortList();

            var sortedEthernet = SortedMeasurementEthernet.Dictionary;

            foreach (var key in sortedEthernet.Keys)
            {
                PrintOneProgramEthernet(key, sortedEthernet[key]);
            }
        }

        private void PrintOneProgramProfinet(int programNumber, List<MeasuredVariables> measuredVariables)
        {
            var rows = new List<double[]>();
            foreach (var measurement in measuredVariables)
            {
                rows.Add(measurement.GetMeasuredValues().ToArray());
                ToCsvFile(measurement);
            }
            //ToMatFile(rows, programNumber);
        }

        private void PrintOneProgramEthernet(int programNumber, List<TcpRobot> measuredVariables)
        {
            var rows = new List<double[]>();
            foreach (var measurement in measuredVariables)
            {
                rows.Add(measurement.FilePreparation().ToArray());
                ToCsvFile(measurement);
            }
            //ToMatFile(rows, programNumber);
        }

        //private void ToMatFile(List<double[]> measuredData, string name)
        //{
        //    var mMatrix = new MLDouble(name, measuredData.ToArray());
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

        private void ToCsvFile(MeasuredVariables measuredData)
        {
            var dateTime = measuredData.SaveTime;
            var prNumber = (measuredData.ProgramNumber).ToString();
            var begin = string.Join(", ", dateTime, prNumber);
            var newLine = string.Join(", ", measuredData.GetMeasuredValues().ToArray().Select(element => element.ToString(CultureInfo.InvariantCulture)).ToArray());
            LocalStringBuilder.AppendLine(begin + ", " + newLine);
        }
    }
}
