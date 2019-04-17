using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("markov", HelpText = "Learning model for HMM")]
    public class MarkovOptions
    {
        [Option('l', "load", HelpText = "Path to the train folder.", Required = true)]
        public string Load { get; set; }

        [Option('f', "train", HelpText = "Path to the train folder.")]
        public string TrainFolderPath { get; set; }

        [Option('t', "test", HelpText = "Path to the test folder.")]
        public string TestFolderPath { get; set; }

        [Option('s', "states", HelpText = "Number of states")]
        public int States { get; set; }

        [Option('v', "vector", HelpText = "Lenght of sequence vector", Default = 13)]
        public int DataSet { get; set; }
    }
}
