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

        [Option('s', "state", HelpText = "Number of state", Required = true)]
        public int State { get; set; }

    }
}
