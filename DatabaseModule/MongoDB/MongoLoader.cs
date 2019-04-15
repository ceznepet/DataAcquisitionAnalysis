using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using Common.Savers;
using DatabaseModule.Extensions;
using DatabaseModule.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using NLog;

namespace DatabaseModule.MongoDB
{
    public class MongoLoader
    {
        private static readonly Logger Logger = LogManager.GetLogger("File saving");

        public MongoLoader(string databaseLocation, string database, string document, string profinet, string folder,
            string fileName, bool sorted, bool byProduct)
        {
            Profinet = int.Parse(profinet) == 1;
            Sorted = sorted;
            Folder = folder;
            ByProduct = byProduct;
            SortedMeasurementProfinet = new SortMeasurementProfinet();
            SortedMeasurementEthernet = new SortMeasurementEthernet();
            MeasuredData = new List<double[]>();

            var client = new MongoClient(MongoUrl.Create(databaseLocation));
            Database = client.GetDatabase(database);
            Collection = Database.GetCollection<BsonDocument>(document);

            var source = int.Parse(profinet) == 1 ? "_profinet" : "_ethernet";
            FileName = Folder + "/" + fileName + source;
        }

        private IMongoDatabase Database { get; }
        private IMongoCollection<BsonDocument> Collection { get; }
        private bool Profinet { get; }
        private bool Sorted { get; }
        private bool ByProduct { get; }
        private string Folder { get; }
        private string FileName { get; }
        private SortMeasurementProfinet SortedMeasurementProfinet { get; }
        private SortMeasurementEthernet SortedMeasurementEthernet { get; }
        private List<double[]> MeasuredData { get; }

        public async Task ReadData()
        {
            using (var cursor = await Collection.FindAsync(new BsonDocument()))
            {
                //LocalStringBuilder = new StringBuilder();
                while (cursor.MoveNext())
                {
                    var batch = cursor.Current;

                    foreach (var document in batch) BsonDocToList(document);
                }

                Logger.Info("Data are loaded from databese.");
                Save();
                //File.WriteAllText(FileName + ".csv", LocalStringBuilder.ToString());
                Logger.Info("All data are saved into files");
            }
        }

        private void BsonDocToList(BsonDocument document)
        {
            document.Remove("_id");
            if (Profinet)
            {
                var measurement = JsonConvert.DeserializeObject<MeasuredVariables>(document.ToJson());
                if (measurement.Variables.Count == 0) return;
                if (!Sorted)
                {
                    var time = measurement.SaveTime.TimeInSecond();
                    var prNumber = (double) measurement.ProgramNumber;
                    var data = measurement.GetMeasuredValues().ToList();
                    data.Insert(0, time);
                    data.Insert(1, prNumber);
                    MeasuredData.Add(data.ToArray());
                }

                SortedMeasurementProfinet.AddToList(measurement);
            }
            else
            {
                var measuredData = JsonConvert.DeserializeObject<TcpRobot>(document.ToJson());
                measuredData.ToList();
                if (Sorted)
                {
                    SortedMeasurementEthernet.AddToList(measuredData);
                }
                else
                {
                    MeasuredData.Add(measuredData.FilePreparation().ToArray());
                    CsvSavers.ToCsvFile(measuredData, FileName);
                }
            }
        }

        private void Save()
        {
            if (Sorted)
            {
                if (Profinet)
                    SaveProfinet();
                else
                    SaveEthernet();
            }
            else
            {
                SortedMeasurementProfinet.SortMeasurement();
                foreach (var mesurement in SortedMeasurementProfinet.Measurements)
                    CsvSavers.ToCsvFile(mesurement, FileName);
                MeasuredData.Sort((x, y) => x[0].CompareTo(y[0]));
                MatSavers.ToMatFile(MeasuredData, "0", FileName);
            }
        }

        private void SaveProfinet()
        {
            if (ByProduct)
                SortedMeasurementProfinet.SortByProduct();
            else
                SortedMeasurementProfinet.SortList();
            var sortedProfinet = SortedMeasurementProfinet.Dictionary;

            foreach (var key in sortedProfinet.Keys)
                PrintOneProgramProfinet(key, sortedProfinet.First(data => data.Key == key).Value);
        }

        private void SaveEthernet()
        {
            SortedMeasurementEthernet.SortList();

            var sortedEthernet = SortedMeasurementEthernet.Dictionary;

            foreach (var key in sortedEthernet.Keys)
                PrintOneProgramEthernet(key, sortedEthernet.First(data => data.Key == key).Value);
        }

        private void PrintOneProgramProfinet(int programNumber, List<MeasuredVariables> measuredVariables)
        {
            var rows = new List<double[]>();
            foreach (var measurement in measuredVariables)
            {
                var time = measurement.SaveTime.TimeInSecond();
                var prNumber = (double) measurement.ProgramNumber;
                var data = measurement.GetMeasuredValues().ToList();
                data.Insert(0, time);
                data.Insert(1, prNumber);
                rows.Add(data.ToArray());
                CsvSavers.ToCsvFile(measurement, FileName);
            }

            MatSavers.ToMatFile(rows, programNumber.ToString(), FileName);
        }

        private void PrintOneProgramEthernet(int programNumber, List<TcpRobot> measuredVariables)
        {
            var rows = new List<double[]>();
            foreach (var measurement in measuredVariables)
            {
                var row = measurement.FilePreparation().ToArray();
                measurement.Called = true;
                rows.Add(row);
                CsvSavers.ToCsvFile(measurement, FileName);
            }

            MatSavers.ToMatFile(rows, programNumber.ToString(), FileName);
        }
    }
}