using Common.Loaders;
using HMModel.Models;
using NLog;

namespace HMModel
{
    public class MarkovModel
    {
        private static readonly Logger Logger = LogManager.GetLogger("Markov main");

        public MarkovModel(string trainFolder, string testFolder)
        {
            const int take = 6;
            const int skip = 0;
            const bool product = false;
            Logger.Info("Start loading data...");
            var train = MatLoaders.LoadPrograms(trainFolder, take, skip, product);
            var test = MatLoaders.LoadPrograms(testFolder, take, skip, product);
            Logger.Info("Loading is succesfully done...");
            Learning.StartTeaching(train, test, take);
        }

    }
}
