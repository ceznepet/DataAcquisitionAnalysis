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
using DataAcquisitionAnalysis.Processing;
using HMModel;
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
                .ParseArguments<TcpSocketSaveOptions, LoadMongoDataOptions, KunbusOptions, MarkovOptions, DataProcessingOptions>(args)
                .MapResult((TcpSocketSaveOptions options) => PacketSaver(options),
                    (LoadMongoDataOptions options) => LoadDataFromMongoDb(options),
                    (KunbusOptions options) => KunbusModule(options),
                    (MarkovOptions options) => MarkovModel(options),
                    (DataProcessingOptions options) => DataProcessing(options),
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
            if (options.Load == "1")
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
    }
}
