using System.Linq;
using Accord.Math;
using Accord.Statistics.Analysis;
using Common.Loaders;
using Common.Extensions;
using HMModel.Models;
using NLog;

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
            var train = MatLoaders.LoadProgramsAsTimeSeries(trainFolder, product);
            var test = MatLoaders.LoadProgramsAsTimeSeries(testFolder, product);
            Logger.Info("Loading is succesfully done...");
            Learning.StartTeaching(train, test, skip, take, states);
        }

        public MarkovModel(string modelPath, string dataFolder)
        {
            var test = MatLoaders.LoadProgramsAsTimeSeries(dataFolder, true).ToList();
            var classifier = LoadModel.LoadMarkovClassifier(modelPath);

            var testData = test.ToSequence();
            var testOutputs = test.GetLabels();

            testData = testData.Apply(Accord.Statistics.Tools.ZScores);

            //testData = Accord.Statistics.Tools.ZScores(testData);

            var testPredict = classifier.Decide(testData);

            var confusionMatrix = new GeneralConfusionMatrix(testPredict, testOutputs);
            var trainAccTest = confusionMatrix.Accuracy;

            Logger.Info("Check of performance: {0}", trainAccTest);

            if (trainAccTest > 0.996)
            {
                var trainer = new TrainPredictor(testPredict, 22);
                trainer.CreateTransitionMatrix();
            }

        }
    }
}
