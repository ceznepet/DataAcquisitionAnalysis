using CommandLine;
using Common.Logging;
using DataAcquisitionAnalysis.Options;
using DataAcquisitionAnalysis.Processing;
using DatabaseModule.MongoDB;
using KunbusRevolutionPiModule;
using MarkovModule;
using NLog;
using SocketModule.SocketClient;

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
            var endian = options.BigEndian == "1";
            var kunbus = new KunbusIOModule(endian, options.ConfigurationFile,
                                          options.DatabaseLocation, options.Database, options.Document,
                                          options.Period);
            return 0;
        }

        public static int MarkovModel(MarkovOptions options)
        {
            if (options.DoClassification == "1")
            {
                Logger.Info("Loading the model form path: {}.", options.TrainFolderPath);
                var predictor = new MarkovModel(options.TrainFolderPath, options.TestFolderPath, options.DataSet);
                return 0;
            }
            Logger.Info("Start of learning the markov model.");
            var model = new MarkovModel(options.TrainFolderPath, options.TestFolderPath, options.States, options.DataSet);
            return 0;
        }

        public static int DataProcessing(DataProcessingOptions options)
        {
            Logger.Info("Start of learning the markov model.");
            var processing = new DataProcessing(options.Folder, options.SaveFile);
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
