using Common.Loaders;
using HMModel.Models;
using NLog;

namespace HMModel
{
    public class MarkovModel
    {
        private static readonly Logger Logger = LogManager.GetLogger("Markov main");

        public MarkovModel(string trainFolder, string testFolder, int state)
        {
            const int take = 18;
            const int skip = 0;
            const bool product = false;
            Logger.Info("Start loading data...");
            var train = MatLoaders.LoadProgramsAsTimeSeries(trainFolder, true);
            var operations = train.ToList();
            var length = operations.Count();
            Logger.Info("Loading is succesfully done...");
            for(var i  = 0; i < 3; i++)
            {
                Learning.StartTeaching(operations.Take(length / 2), operations.Skip(length / 2), skip, take, states + i, testFolder);
            }
        }

        public MarkovModel(string modelPath, string dataFolder)
        {
            Logger.Info("Loading data.");
            var test = MatLoaders.LoadProgramsAsTimeSeries(dataFolder, true).ToList();
            var classifier = LoadModel.LoadMarkovClassifier(modelPath);

            var testData = test.ToSequence();
            var testOutputs = test.GetLabels();

            testData = testData.Apply(Accord.Statistics.Tools.ZScores);

            //testData = Accord.Statistics.Tools.ZScores(testData);

            var testPredict = classifier.Decide(testData);
            Logger.Info("Dicision done.");
            var confusionMatrix = new GeneralConfusionMatrix(testPredict, testOutputs);
            var trainAccTest = confusionMatrix.Accuracy;

            Logger.Info("Check of performance: {0}", trainAccTest);

            if (trainAccTest > 0.99)
            {
                Learning.StartTeaching(operations.Take(length / 2), operations.Skip(length / 2), skip, take, states + i, testFolder);
            }
        }

    }
}
