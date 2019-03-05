using DatabaseModule.Extensions;
using DatabaseModule.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog;
using System;

namespace DatabaseModule.MongoDB
{
    public class MongoSaver
    {
        private IMongoDatabase Database { get; set; }
        private IMongoCollection<BsonDocument> Collection { get; set; }
        private static readonly Logger _logger = LogManager.GetLogger("Mongo Saver");

        public MongoSaver(string databaseLocation, string database, string document)
        {
            try
            {
                var client = new MongoClient(MongoUrl.Create(databaseLocation));
                Database = client.GetDatabase(database);

                Collection = Database.GetCollection<BsonDocument>(document);
            }
            catch (MongoConfigurationException exception)
            {
                _logger.Error("Mongo Configuration exception: {0}", exception);
            }
        }

        public void SavePacket(string xml)
        {
            var packet = xml.XmlDeserialize<EthernetXmlSerialization>();
            var document = BsonDocument.Parse(packet.ToJson());
            //var document = BsonDocument.Parse(JsonConvert.SerializeXmlNode(xml, Newtonsoft.Json.Formatting.Indented));

            Collection.InsertOneAsync(document);
            _logger.Trace("Saving of the packet is done.");
        }

        public void SaveIOData(dynamic measurement)
        {
            //TODO: Test if this is correct serialization
            var document = measurement.ToBsonDocument();
            Collection.InsertOneAsync(document);
            _logger.Trace("Saving of the I/O is done.");
        }
    }
}
