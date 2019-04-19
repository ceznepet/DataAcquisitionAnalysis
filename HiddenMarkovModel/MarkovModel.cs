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

        public MarkovModel(string trainFolder, string testFolder, int states, int take)
        {
            const int skip = 0;
            Logger.Info("Start loading data...");
            var train = MatLoaders.LoadProgramsAsTimeSeries(trainFolder, true, take);
            var operations = train.ToList();
            var length = operations.Count();
            Logger.Info("Loading is succesfully done...");
            Learning.StartTeaching(operations, operations.Skip(length / 2), skip, take,  states, testFolder);
        }

        public MarkovModel(string modelPath, string dataFolder, int take)
        {
            Logger.Info("Loading data.");
            var test = MatLoaders.LoadProgramsAsTimeSeries(dataFolder, true, take).ToList();
            var classifier = LoadModel.LoadMarkovClassifier(modelPath);

            var testData = test.ToSequence();
            var testOutputs = test.GetLabels();

            testData = testData.Apply(Accord.Statistics.Tools.ZScores);


            var testPredict = classifier.Decide(testData);
            Logger.Info("Dicision done.");
            var confusionMatrix = new GeneralConfusionMatrix(testPredict, testOutputs);
            var trainAccTest = confusionMatrix.Accuracy;

            Logger.Info("Check of performance: {0}", trainAccTest);

            if (trainAccTest > 0.99)
            {
                var trainer = new DiscreteModel(LoadModel.LoadMarkovModel(modelPath)); //LoadModel.LoadMarkovModel(modelPath)
                var decisions = trainer.Decide(testPredict).ToArray();

                var count = 0;
                foreach (var decision in decisions)
                {
                    if (decision.State == testOutputs[count] || decision.State == 0)
                    {
                        count++;
                        continue;
                    }
                    Logger.Info("Position: {}", count);
                    Logger.Info("Predict: {} \t Actual: {}", decision.State, testOutputs[count]);
                    Logger.Info("Predict probability: {}", decision.Probability[0]);
                    count++;
                }

                var testPredict2 = decisions.Select(item => item.State == 0 ? 22 : item.State).ToArray();
                var confusionMatrix2 = new GeneralConfusionMatrix(testPredict2, testOutputs);
                var trainAccTest2 = confusionMatrix2.Accuracy;

                Logger.Info("Check of performance: {0}", trainAccTest2);
            }

            }
    }
}
