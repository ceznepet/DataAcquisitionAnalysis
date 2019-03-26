using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("markov", HelpText = "Learning model for HMM")]
    public class MarkovOptions
    {
        [Option('f', "train", HelpText = "Path to the train folder.", Required = true)]
        public string TrainFolderPath { get; set; }

        [Option('t', "test", HelpText = "Path to the test folder.", Required = true)]
        public string TestFolderPath { get; set; }

        [Option('s', "states", HelpText = "Number of states", Required = true)]
        public int States { get; set; }
    }
}
