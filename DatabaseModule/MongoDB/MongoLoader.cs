using Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using NLog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseModule.MongoDB
{
    public class MongoLoader
    {
        private IMongoDatabase DatabaseName { get; }
        private IMongoCollection<BsonDocument> Collection { get; }

        public List<Operation> MongoData { get; private set; }

        private static readonly Logger Logger = LogManager.GetLogger("Mongo Loader");

        public MongoLoader(string databaseLocation, string databaseName, string collection)
        {
            var client = new MongoClient(MongoUrl.Create(databaseLocation));
            DatabaseName = client.GetDatabase(databaseName);
            Collection = DatabaseName.GetCollection<BsonDocument>(collection);
        }

        public static List<Operation> GetData(string databaseLocation, string databaseName, string document)
        {
            var loader = new MongoLoader(databaseLocation, databaseName, document);
            loader.ReadData().Wait();
            return loader.MongoData;
        }

        public async Task ReadData()
        {
            using (var cursor = await Collection.FindAsync(new BsonDocument()))
            {
                while (cursor.MoveNext())
                {
                    var batch = cursor.Current;

                    foreach (var document in batch)
                    {
                        BsonDocToList(document);
                    }
                }
            }
        }

        private void BsonDocToList(BsonDocument document)
        {
            document.Remove("_id");
            var measurement = JsonConvert.DeserializeObject<Operation>(document.ToJson());
            MongoData.Add(measurement);
        }
    }
}
