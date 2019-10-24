﻿using Accord.Math;
using Accord.Statistics.Analysis;
using Common.Extensions;
using Common.Loaders;
using NLog;
using System.Linq;
using MarkovModule.Models;
using DatabaseModule.MongoDB;

namespace MarkovModule
{
    public class MarkovModel
    {
        private static readonly Logger Logger = LogManager.GetLogger("Markov main");

        public DiscreteModel DiscreteModel { get; set; }

        public static void LearnFromMatFile(string dataFolder, int states, int take, float trainSplit, 
                                            string modelFolder)
        {
            const int skip = 0;
            Logger.Info("Start loading data...");
            var train = MatLoaders.LoadProgramsAsTimeSeries(dataFolder, true, take);
            var operations = train.ToList();
            var length = operations.Count();
            var split = (int)(length * trainSplit);
            Logger.Info("Loading is succesfully done...");
            Learning.StartTeaching(operations.Take(split), operations.Skip(split), skip, take, states, modelFolder);
        }

        public static void LearnFromDB(string databaseLocation, string databaseName, string collection, int states,
                                       int take, float trainSplit, string modelFolder)
        {
            const int skip = 0;
            Logger.Info("Start loading data...");
            var train = MongoLoader.GetData(databaseLocation, databaseName, collection);
            var operations = train.ToList();
            var length = operations.Count();
            var split = (int)(length * trainSplit);
            Logger.Info("Loading is succesfully done...");
            Learning.StartTeaching(operations.Take(split), operations.Skip(split), skip, take, states, modelFolder);
        }

        public static void ClassifieFromFile(string modelPath, string dataFolder, int take)
        {
            Logger.Info("Loading data.");
            var test = MatLoaders.LoadProgramsAsTimeSeries(dataFolder, true, take).ToList();
            var classifier = LoadModel.LoadMarkovClassifier(modelPath);

            var testData = test.ToSequence().Take(100).ToArray();
            var testOutputs = test.GetLabels().Take(100).ToArray();
            Logger.Info("Load done.");

            //var path = DiscreteModel.GetFake();
            //CsvSavers.SaveMarkovPath(@"C:\Users\cezyc\Desktop\path.csv", path);

            var trainer = new DiscreteModel(LoadModel.LoadMarkovModel(modelPath), classifier); //LoadModel.LoadMarkovModel(modelPath), classifier || 22, testOutputs.Take(200).ToArray()

            var decisions = testData.Select(element => trainer.Decide(element));
            var count = 0;
            var enumerable = decisions.ToList();
            foreach (var decision in enumerable)
            {
                var check = decision.State;
                if (check == testOutputs[count])
                {
                    count++;
                    continue;
                }
                Logger.Info("Position: {}", count);
                Logger.Info("Predict: {} \t Actual: {}", decision.State, testOutputs[count]);
                Logger.Info("Predict probability of sequence {} is {}", decision.Sequence, decision.LogLikelihoodDifferences);
                Logger.Info("Classifier probability: {}", decision.ClassifierProbability);
                count++;
            }

            var testPredict2 = enumerable.Select(item => item.State).ToArray();
            var confusionMatrix2 = new GeneralConfusionMatrix(testPredict2, testOutputs);
            var trainAccTest2 = confusionMatrix2.Accuracy;

            Logger.Info("Check of performance: {0}", trainAccTest2);
        }        


        public MarkovModel(string modelPath)
        {
            var classifier = LoadModel.LoadMarkovClassifier(modelPath);
            DiscreteModel = new DiscreteModel(LoadModel.LoadMarkovModel(modelPath), classifier);
        }
    }
}
