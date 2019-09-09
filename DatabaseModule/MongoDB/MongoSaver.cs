using DatabaseModule.Extensions;
using DatabaseModule.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog;
using System;
using System.Threading;
using Newtonsoft.Json;

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
            if (document == null)
            {
                _logger.Info("Document is empty!");
                return;
            }
            Collection.InsertOne(document);
            _logger.Trace("Saving of the packet is done.");
        }

        public void SaveIOData(dynamic measurement)
        {
            if (measurement == null)
            {
                _logger.Info("Document is empty!");
                return;
            }
            var document = BsonDocument.Parse(JsonConvert.SerializeObject(measurement));
            
            Collection.InsertOne(document);
            _logger.Info("Saving of the I/O is done.");            
        }
    }
}
