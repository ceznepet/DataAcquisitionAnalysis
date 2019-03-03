using System;
using CommandLine;
using DataAcquisitionAnalysis.Options;
using DatabaseModule.MongoDB;
using KunbusRevolutionPiModule;
using TcpCommunication.TcpClient;

namespace DataAcquisitionAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<TcpSocketSaveOptions, LoadMongoDataOptions, KunbusOptions>(args)
                .MapResult((TcpSocketSaveOptions options) => PacketSaver(options),
                    (LoadMongoDataOptions options) => LoadDataFromMongoDb(options),
                    (KunbusOptions options) => KunbusModule(options),
                    errs => 1);
        }

        public static int PacketSaver(TcpSocketSaveOptions options)
        {
            //var client = new SocketClient(options.Port, options.Ip, options.Database, options.Document);
            //client.StartClient();
            var pokus = new TcpClientSocket(options.Ip, options.Port, options.Database, options.Document);
            pokus.ConnectAndReceive();
            return 0;
        }

        public static int LoadDataFromMongoDb(LoadMongoDataOptions options)
        {
            MongoDbCall.LoadDataAndSave(options.Database, options.Document, options.Profinet, options.Folder);
            return 0;
        }

        public static int KunbusModule(KunbusOptions options)
        {
            var endian = options.BigEndian == "1" ? true : false;
            Console.WriteLine("Kunbus Start");
            var kunbus = new TestOfKunbus((uint) options.NumberOfBytes, endian);
            return 0;
        }
    }
}
