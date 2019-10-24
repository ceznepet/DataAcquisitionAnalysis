using CommandLine;
using Common.Logging;
using Common.Models;
using DataAcquisitionAnalysis.Options;
using DataAcquisitionAnalysis.Processing;
using DatabaseModule.MongoDB;
using KunbusRevolutionPiModule;
using MarkovModule;
using Newtonsoft.Json;
using NLog;
using SocketModule.SocketClient;
using System.IO;

namespace DataAcquisitionAnalysis
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetLogger("Main");
        static void Main(string[] args)
        {
            LoggerSetUp.SetUpLogger();
            Parser.Default
                .ParseArguments<SocketOptions, LoadMongoDataOptions, KunbusOptions, MarkovOptions,
                                DataProcessingOptions, OnlineClassificationOptions>(args)
                .MapResult((SocketOptions options) => PacketSaver(options),
                    (LoadMongoDataOptions options) => LoadDataFromMongoDb(options),
                    (KunbusOptions options) => KunbusModule(options),
                    (MarkovOptions options) => MarkovModel(options),
                    (DataProcessingOptions options) => DataProcessing(options),
                    (OnlineClassificationOptions options) => OnlineAnalysis(options),
                    errs => 1);
        }

        public static int PacketSaver(SocketOptions options)
        {
            Logger.Info("Socket client started.");
            var client = new SocketClient(options.Ip, options.Port, options.Protocol, options.DatabaseLocation,
                                             options.Database, options.Document);
            client.ConnectAndReceive();
            return 0;
        }

        public static int LoadDataFromMongoDb(LoadMongoDataOptions options)
        {
            var sorted = options.Sorted == "Yes";
            var byProduct = options.ByProduct == "Yes";
            Logger.Info("Loading of data from DB started.");
            MongoDbCall.LoadDataAndSave(options.DatabaseLocation, options.Database, options.Document,
                                        options.Profinet, options.Folder, options.FilderName, sorted,
                                        byProduct);
            return 0;
        }

        public static int KunbusModule(KunbusOptions options)
        {
            Logger.Info("Kunbus started.");
            var setup_file = JsonConvert.DeserializeObject<KunbusSetup>(File.ReadAllText(options.Setup));
            var endian = setup_file.BigEndian;
            var kunbus = new KunbusIOModule(endian, setup_file.ConfigurationFile, setup_file.DatabaseLocation, 
                                            setup_file.DatabaseName, setup_file.DatabaseDocument,
                                            setup_file.ReadingPerios);
            return 0;
        }

        public static int MarkovModel(MarkovOptions options)
        {

            var setup_file = JsonConvert.DeserializeObject<MarkovSetup>(File.ReadAllText(options.Setup));
            if (setup_file.DoClassification)
            {
                Logger.Info("Loading the model form path: {}.", setup_file.TrainFolderPath);
                var predictor = new MarkovModel(setup_file.TrainFolderPath, setup_file.TestFolderPath, setup_file.TakeVector);
                return 0;
            }
            Logger.Info("Start of learning the markov model.");
            var model = new MarkovModel(setup_file.TrainFolderPath, setup_file.TestFolderPath, setup_file.States, setup_file.TakeVector);
            return 0;
        }

        public static int OnlineAnalysis(OnlineClassificationOptions options)
        {
            Logger.Info("Online Analysis started.");
            var endian = options.BigEndian == "1";
            var online = new KunbusIOModule(options.ConfigurationFile, options.FolderPath, endian, options.Period, options.Features);
            return 0;
        }
    }
}
