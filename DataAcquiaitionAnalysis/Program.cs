﻿using CommandLine;
using Common.Logging;
using Common.Models.Setup;
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
                    (MarkovOptions options) => MarkovModule(options),
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
            var setup_file = JsonConvert.DeserializeObject<MonogoSetup>(File.ReadAllText(options.Setup));
            Logger.Info("Loading of data from DB started.");
            MongoDbCall.LoadDataAndSave(setup_file.DatabaseLocation, setup_file.DatabaseName, setup_file.DatabaseCollection,
                                        setup_file.Profinet, setup_file.SaveFolderLocation, setup_file.Filename, setup_file.SortData,
                                        setup_file.SortByProduct, setup_file.ToMatFile);
            return 0;
        }

        public static int KunbusModule(KunbusOptions options)
        {
            Logger.Info("Kunbus started.");
            var setup_file = JsonConvert.DeserializeObject<KunbusSetup>(File.ReadAllText(options.Setup));
            var kunbus = new KunbusIOModule(setup_file.BigEndian, setup_file.ConfigurationFile, setup_file.DatabaseLocation, 
                                            setup_file.DatabaseName, setup_file.DatabaseCollection, setup_file.ReadingPerios);
            return 0;
        }

        public static int MarkovModule(MarkovOptions options)
        {

            var setup_file = JsonConvert.DeserializeObject<MarkovSetup>(File.ReadAllText(options.Setup));
            if (setup_file.DoClassification)
            {
                Logger.Info("Loading the model form path: {}.", setup_file.DataFolder);
                MarkovModel.ClassifieFromFile(setup_file.ModelFolder, setup_file.DataFolder, setup_file.TakeVector);
                return 0;
            }
            if (setup_file.LearnFromDB)
            {
                Logger.Info("Start of learning the markov model from MongoDB.");
                MarkovModel.LearnFromDB(setup_file.DatabaseCollection, setup_file.DatabaseName, setup_file.DatabaseCollection,
                                        setup_file.States, setup_file.TakeVector, setup_file.TrainDatasetSize, setup_file.ModelFolder);
            }
            else
            {
                Logger.Info("Start of learning the markov model from Mat file.");
                MarkovModel.LearnFromMatFile(setup_file.DataFolder, setup_file.States,
                                             setup_file.TakeVector, setup_file.TrainDatasetSize, setup_file.ModelFolder);
            }
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
