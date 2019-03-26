using Common.Loaders;
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
            var test = MatLoaders.LoadProgramsAsTimeSeries(testFolder,  product);
            Logger.Info("Loading is succesfully done...");
            Learning.StartTeaching(train, test, skip, take, states);
        }

    }
}
