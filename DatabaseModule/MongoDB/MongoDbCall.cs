using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseModule.MongoDB
{
    public class MongoDbCall
    {
        public static void LoadDataAndSave(string database, string document, string profinet, string folder)
        {
            var mongoLoader = new MongoLoader(database, document, profinet, folder);
            mongoLoader.ReadData().Wait();
            Console.WriteLine("Data from your database are saved into .mat file.");
        }

        public static MongoSaver GetSaverToMongoDb(string database, string document)
        {
            return new MongoSaver(database, document);
        }
    }
}
