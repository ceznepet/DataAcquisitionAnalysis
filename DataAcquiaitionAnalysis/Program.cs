using CommandLine;
using DataAcquisitionAnalysis.Options;
using DatabaseModule.MongoDB;
using System;
using TcpCommunication.TcpClient;

namespace DataAcquisitionAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<TcpSocketSaveOptions, LoadMongoDataOptions>(args)
                .MapResult((TcpSocketSaveOptions options) => PacketSaver(options),
                    (LoadMongoDataOptions options) => LoadDataFromMongoDb(options),
                    errs => 1);
        }

        public static int PacketSaver(TcpSocketSaveOptions options)
        {
            var client = new SocketClient(int.Parse(options.Port), options.IP);
            client.StartClient();

            return 0;
        }

        public static int LoadDataFromMongoDb(LoadMongoDataOptions options)
        {
            MongoDbCall.LoadDataAndSave(options.Database, options.Document, options.Profinet, options.Folder);
            return 0;
        }
    }
}
