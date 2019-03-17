using System.Collections.Generic;
using System.Linq;
using System.Data;
using HiddenMarkovModel.Loaders;
using DatabaseModule.Extensions;
using HiddenMarkovModel.Models;
using NLog;

namespace HiddenMarkovModel
{
    public class MarkovModel
    {
        private static readonly Logger Logger = LogManager.GetLogger("Markov main");

        public MarkovModel(string trainFolder, string testFolder)
        {

            Logger.Info("Start loading data...");
            var train = Loader.LoadPrograms(trainFolder);
            var test = Loader.LoadPrograms(testFolder);
            Logger.Info("Loading is succesfully done...");
            Learning.StartTeaching(train, test, 6);
        }

    }
}
