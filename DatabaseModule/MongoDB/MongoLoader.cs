using csmatio.io;
using csmatio.types;
using DatabaseModule.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
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

        public MongoLoader(string database, string document, string profinet, string folder)
        {
            Profinet = int.Parse(profinet) == 1 ? true : false;
            Folder = folder;
            var client = new MongoClient();
            Database = client.GetDatabase(database);

            Collection = Database.GetCollection<BsonDocument>(document);
        }

        public async Task ReadData()
        {
            using (IAsyncCursor<BsonDocument> cursor = await Collection.FindAsync(new BsonDocument()))
            {
                var dataList = new List<double[]>();
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;

                    foreach (BsonDocument document in batch)
                    {
                        dataList.Add(Save2File(document));
                    }
                }

                ToMatFile(dataList);
                ToCsvFile(dataList);
            }


        }

        private double[] Save2File(BsonDocument document)
        {
            document.Remove("_id");
            if (Profinet)
            {
                //TODO: case for loading data from profinet
                return null;
            }
            else
            {
                var measuredData = JsonConvert.DeserializeObject<Robot>(document.ToJson());
                return measuredData.FilePreparation().ToArray();
            }
        }

        public void ToMatFile(List<double[]> measuredData)
        {
            var data = measuredData.ToArray();
            var mMatrix = new MLDouble("Measurement", data);
            var mList = new List<MLArray>();
            mList.Add(mMatrix);
            var mFileWrite = new MatFileWriter(Folder + "/pokus.mat", mList, false);
        }

        public void ToCsvFile(List<double[]> measuredData)
        {
            var csvFile = Folder + "/pokus.csv";
            var csv = new StringBuilder();
            foreach (var data in measuredData)
            {
                var newLine = string.Join(", ", data.Select(element => element.ToString()).ToArray());
                csv.AppendLine(newLine);
            }
            File.WriteAllTextAsync(csvFile, csv.ToString()).Wait();
        }
    }
}
