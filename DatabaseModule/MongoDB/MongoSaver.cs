using DatabaseModule.Extensions;
using DatabaseModule.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog;
using System;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseModule.MongoDB
{
    public class MongoSaver
    {
        private IMongoDatabase Database { get; set; }
        private IMongoCollection<BsonDocument> Collection { get; set; }
        private static readonly Logger _logger = LogManager.GetLogger("Mongo Saver");

        private List<BsonDocument> BatchData = new List<BsonDocument>();
        private int DocumentCount = 0;

        public MongoSaver(string databaseLocation, string database, string collection)
        {
            try
            {
                var client = new MongoClient(MongoUrl.Create(databaseLocation));
                Database = client.GetDatabase(database);

                Collection = Database.GetCollection<BsonDocument>(collection);
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

        public void SaveBatchData(dynamic measurement)
        {
            if (measurement == null)
            {
                _logger.Info("Document is empty!");
                return;
            }
            BsonDocument document = BsonDocument.Parse(JsonConvert.SerializeObject(measurement));
            BatchData.Add(document);
            DocumentCount++;
            if (DocumentCount == 500)
            {
                _logger.Info("Batch of data saved.");
                DocumentCount = 0;
                var batch = new List<BsonDocument>(BatchData);
                var thread = new Thread(() => SendBatchData(batch));
                thread.Start();
                BatchData.Clear();
            }
        }

        private void SendBatchData(List<BsonDocument> batchData)
        {
            Collection.InsertMany(batchData);
        }

    }
}
