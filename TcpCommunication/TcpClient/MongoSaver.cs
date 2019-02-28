using System;
using System.Threading.Tasks;
using System.Xml;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace TcpCommunication.TcpClient
{
    public class MongoSaver
    {
        private IMongoDatabase Database { get; set; }
        private IMongoCollection<BsonDocument> Collection { get; set; }

        public MongoSaver(string database, string document)
        {
            var client = new MongoClient();
            Database = client.GetDatabase(database);

            Collection = Database.GetCollection<BsonDocument>(document);

        }

        public async Task SavePacket(XmlDocument xml)
        {
            var document = BsonDocument.Parse(JsonConvert.SerializeXmlNode(xml, Newtonsoft.Json.Formatting.Indented));
            Console.WriteLine("Done");
            await Collection.InsertOneAsync(document);
        }
    }
}
