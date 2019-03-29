using Accord.Math;
using Accord.Statistics.Analysis;
using Common.Extensions;
using Common.Loaders;
using HMModel.Models;
using NLog;
using System.Linq;

namespace HMModel
{
    public class MarkovModel
    {
        private static readonly Logger Logger = LogManager.GetLogger("Markov main");

        public MarkovModel(string trainFolder, string testFolder, int states)
        {
            const int take = 10;
            const int skip = 0;
            const bool product = false;
            Logger.Info("Start loading data...");
            var train = MatLoaders.LoadProgramsAsTimeSeries(trainFolder, true);
            var operations = train.ToList();
            var length = operations.Count();
            Logger.Info("Loading is succesfully done...");
            for (var i = 0; i < 3; i++)
            {
                Learning.StartTeaching(operations.Take(length / 2), operations.Skip(length / 2), skip, take, states + i, testFolder);
            }
        }

        public MarkovModel(string modelPath, string dataFolder)
        {
            Logger.Info("Loading data.");
            var test = MatLoaders.LoadProgramsAsTimeSeries(dataFolder, true).ToList();
            var classifier = LoadModel.LoadMarkovClassifier(modelPath);

            var testData = test.ToSequence().Take(100).ToArray();
            var testOutputs = test.GetLabels().Take(100).ToArray();
            Logger.Info("Load done.");

            testData = testData.Apply(Accord.Statistics.Tools.ZScores);

            //testData = Accord.Statistics.Tools.ZScores(testData);
            var testPredict = classifier.Decide(testData);

            Logger.Info("Dicision done.");
            var confusionMatrix = new GeneralConfusionMatrix(testPredict, testOutputs);
            var trainAccTest = confusionMatrix.Accuracy;

            Logger.Info("Check of performance: {0}", trainAccTest);

            if (trainAccTest >= 0.5)
            {
                var trainer = new DiscreteModel(LoadModel.LoadMarkovModel(modelPath), classifier); //LoadModel.LoadMarkovModel(modelPath) || 22, testOutputs.Take(200).ToArray()
                var decisions = testData.Select(element => trainer.Decide(element));
                var count = 0;
                var enumerable = decisions.ToList();
                foreach (var decision in enumerable)
                {
                    if (decision.Probability > 0.8 || decision.State == 0)
                    {
                        count++;
                        continue;
                    }
                    Logger.Info("Position: {}", count);
                    Logger.Info("Predict: {} \t Actual: {}", decision.State, testOutputs[count]);
                    Logger.Info("Predict probability: {}", decision.Probability);
                    Logger.Info("Classifier probability: {}", decision.ClassifierProbability);
                    count++;
                }

                var testPredict2 = enumerable.Select(item => item.State == 0 ? 22 : item.State).ToArray();
                var confusionMatrix2 = new GeneralConfusionMatrix(testPredict2, testOutputs);
                var trainAccTest2 = confusionMatrix2.Accuracy;

                Logger.Info("Check of performance: {0}", trainAccTest2);
            }

        }
    }
}
