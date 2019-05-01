using Accord.Math;
using Accord.Statistics;
using Accord.Statistics.Analysis;
using Common.Extensions;
using Common.Loaders;
using Common.Savers;
using NLog;
using System.Collections.Generic;
using System.Linq;
using MarkovModule.Models;

namespace MarkovModule
{
    public class MarkovModel
    {
        private static readonly Logger Logger = LogManager.GetLogger("Markov main");

        public MarkovModel(string trainFolder, string testFolder, int states, int take)
        {
            const int skip = 0;
            const bool product = false;
            Logger.Info("Start loading data...");
            var train = MatLoaders.LoadProgramsAsTimeSeries(trainFolder, true, take);
            var operations = train.ToList();
            var length = operations.Count();
            Logger.Info("Loading is succesfully done...");
            Learning.StartTeaching(operations.Take(length / 2), operations.Skip(length / 2), skip, take, states, testFolder);
        }

        public MarkovModel(string modelPath, string dataFolder, int take)
        {
            Logger.Info("Loading data.");
            var test = MatLoaders.LoadProgramsAsTimeSeries(dataFolder, true, take).ToList();
            var classifier = LoadModel.LoadMarkovClassifier(modelPath);

            var testData = test.ToSequence().Take(100).ToArray();
            var testOutputs = test.GetLabels().Take(100).ToArray();
            Logger.Info("Load done.");

            //testData = testData.Apply(Accord.Statistics.Tools.ZScores);

            var trainer = new DiscreteModel(LoadModel.LoadMarkovModel(modelPath), classifier); //LoadModel.LoadMarkovModel(modelPath), classifier || 22, testOutputs.Take(200).ToArray()

            //trainer.Decide(testData[400], @"C:\Users\cezyc\OneDrive\Plocha\log_likelihood.csv");

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
            var meanOfThreshold = CreateList(enumerable);
            //CsvSavers.SaveClassificationOuput(meanOfThreshold, "Op,Mean", @"C:\Users\cezyc\OneDrive\Plocha\means.csv");

        }


        private Dictionary<int, double> CreateList(List<Decision> decisions)
        {
            var checkOperation = new HashSet<int>();
            var dictonary = new Dictionary<int, List<double>>();

            foreach(var decision in decisions)
            {
                if (!checkOperation.Contains(decision.State))
                {
                    checkOperation.Add(decision.State);
                    dictonary.Add(decision.State, new List<double>());
                }

                dictonary[decision.State].Add(decision.LogLikelihoodDifferences);
            }

            return ComputeMean(dictonary);

        }

        private Dictionary<int, double>  ComputeMean(Dictionary<int, List<double>> dictionary)
        {
            var sorted = new SortedDictionary<int, List<double>>(dictionary);
            var retList = new Dictionary<int, double>();

            foreach(var key in sorted.Keys)
            {
                retList.Add(key, sorted[key].ToArray().Mean());
            }

            return retList;
        }
    }
}
