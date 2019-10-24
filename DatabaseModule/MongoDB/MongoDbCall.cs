using System;

namespace DatabaseModule.MongoDB
{
    public class MongoDbCall
    {
        public static void LoadDataAndSave(string databaseLocation, string database, string document, bool profinet, string folder,
                                           string fileName, bool sorted, bool byProduct, bool toMatFile)
        {
            var mongoLoader = new MongoToFile(databaseLocation, database, document, profinet, folder, fileName, sorted, byProduct, toMatFile);
                mongoLoader.ReadData().Wait();
            Console.WriteLine("Data from your database are saved into .mat file.");
        }

        public static MongoSaver GetSaverToMongoDb(string databaseLocation, string database, string document)
        {
            return new MongoSaver(databaseLocation, database, document);
        }
    }
}
