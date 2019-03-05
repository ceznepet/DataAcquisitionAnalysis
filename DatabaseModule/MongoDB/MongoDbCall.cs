using System;

namespace DatabaseModule.MongoDB
{
    public class MongoDbCall
    {
        public static void LoadDataAndSave(string database, string document, string profinet, string folder, string fileName)
        {
            var mongoLoader = new MongoLoader(database, document, profinet, folder, fileName);
            mongoLoader.ReadData().Wait();
            Console.WriteLine("Data from your database are saved into .mat file.");
        }

        public static MongoSaver GetSaverToMongoDb(string databaseLocation, string database, string document)
        {
            return new MongoSaver(databaseLocation, database, document);
        }
    }
}
