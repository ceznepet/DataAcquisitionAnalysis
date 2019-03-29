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

    }
}
