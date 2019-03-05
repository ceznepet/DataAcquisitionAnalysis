using csmatio.io;
using csmatio.types;
using DatabaseModule.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseModule.MongoDB
{
    public class MongoLoader
    {
        private IMongoDatabase Database { get; set; }
        private IMongoCollection<BsonDocument> Collection { get; set; }
        private bool Profinet { get; set; }
        private string Folder { get; set; }
        private string FileName { get; set; }

        public MongoLoader(string database, string document, string profinet, string folder, string fileName)
        {
            Profinet = int.Parse(profinet) == 1 ? true : false;
            Folder = folder;
            var client = new MongoClient();
            Database = client.GetDatabase(database);
            Collection = Database.GetCollection<BsonDocument>(document);
            FileName = Folder + "/" + fileName + DateTime.Now.ToString("yy-MM-dd-hh-mm-ss");
        }

        public async Task ReadData()
        {
            using (IAsyncCursor<BsonDocument> cursor = await Collection.FindAsync(new BsonDocument()))
            {
                var measuredData = new List<double[]>();
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;

                    foreach (BsonDocument document in batch)
                    {
                        measuredData.Add(BsonDocToList(document));
                    }
                }
                SaveToFile(measuredData);
            }


        }

        private double[] BsonDocToList(BsonDocument document)
        {
            document.Remove("_id");
            if (Profinet)
            {
                //TODO: case for loading data from profinet
                return null;
            }
            else
            {
                var measuredData = JsonConvert.DeserializeObject<TcpRobot>(document.ToJson());
                return measuredData.FilePreparation().ToArray();
            }
        }
        private void SaveToFile(List<double[]> measuredData)
        {
            ToMatFile(measuredData);
            ToCsvFile(measuredData);
        }

        private void ToMatFile(List<double[]> measuredData)
        {
            var mMatrix = new MLDouble("Measurement", measuredData.ToArray());
            var mList = new List<MLArray>();
            mList.Add(mMatrix);
            var mFileWrite = new MatFileWriter(FileName + ".mat", mList, false);
        }

        private void ToCsvFile(List<double[]> measuredData)
        {
            var csv = new StringBuilder();
            foreach (var data in measuredData)
            {
                var newLine = string.Join(", ", data.Select(element => element.ToString()).ToArray());
                csv.AppendLine(newLine);
            }
            File.WriteAllTextAsync(FileName + ".csv", csv.ToString()).Wait();
        }
    }
}
