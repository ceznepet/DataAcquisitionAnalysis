using DatabaseModule.Extensions;
using DatabaseModule.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace DatabaseModule.MongoDB
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

        public void SavePacket(string xml)
        {
            var packet = xml.XmlDeserialize<EthernetXmlSerialization>();
            var document =  BsonDocument.Parse(packet.ToJson());
            //var document = BsonDocument.Parse(JsonConvert.SerializeXmlNode(xml, Newtonsoft.Json.Formatting.Indented));
            Console.WriteLine("Done");
            Collection.InsertOneAsync(document);
        }
    }
}
