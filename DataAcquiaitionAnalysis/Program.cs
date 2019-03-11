using System;
using System.IO;
using CommandLine;
using DataAcquisitionAnalysis.Options;
using DatabaseModule.MongoDB;
using KunbusRevolutionPiModule;
using KunbusRevolutionPiModule.Robot;
using Newtonsoft.Json;
using TcpCommunication.TcpClientDAA;
using Common.Logging;
using NLog;

namespace DataAcquisitionAnalysis
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetLogger("Main");
        static void Main(string[] args)
        {
            LoggerSetUp.SetUpLogger();
            Parser.Default
                .ParseArguments<TcpSocketSaveOptions, LoadMongoDataOptions, KunbusOptions>(args)
                .MapResult((TcpSocketSaveOptions options) => PacketSaver(options),
                    (LoadMongoDataOptions options) => LoadDataFromMongoDb(options),
                    (KunbusOptions options) => KunbusModule(options),
                    errs => 1);
        }

        public static int PacketSaver(TcpSocketSaveOptions options)
        {
            Logger.Info("Tcp Socket client started.");
            var client = new TcpClientSocket(options.Ip, options.Port, options.DatabaseLocation, 
                                             options.Database, options.Document);
            client.ConnectAndReceive();
            return 0;
        }

        public static int LoadDataFromMongoDb(LoadMongoDataOptions options)
        {
            Logger.Info("Loading of data from DB started.");
            MongoDbCall.LoadDataAndSave(options.DatabaseLocation, options.Database, options.Document,
                                        options.Profinet, options.Folder, options.FilderName);
            return 0;
        }

        public static int KunbusModule(KunbusOptions options)
        {
            Logger.Info("Kunbus started.");
            var endian = options.BigEndian == "1";
            var kunbus = new KunbusIOModule(endian, options.ConfigurationFile,
                                          options.DatabaseLocation, options.Database, options.Document);
            return 0;
        }
    }
}
