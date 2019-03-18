using System.Collections.Generic;
using System.Linq;
using System.Data;
using HMModel.Loaders;
using DatabaseModule.Extensions;
using HMModel.Models;
using NLog;

namespace HMModel
{
    public class MarkovModel
    {
        private static readonly Logger Logger = LogManager.GetLogger("Markov main");

        public MarkovModel(string trainFolder, string testFolder)
        {
            var take = 6;
            Logger.Info("Start loading data...");
            var train = Loader.LoadPrograms(trainFolder, take);
            var test = Loader.LoadPrograms(testFolder, take);
            Logger.Info("Loading is succesfully done...");
            Learning.StartTeaching(train, test, take);
        }

    }
}
