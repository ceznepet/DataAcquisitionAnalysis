using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("markov", HelpText = "Learning model for HMM")]
    public class MarkovOptions
    {
        [Option('t', "train", HelpText = "Train of Classify", Required = true)]
        public string DoClassification { get; set; }

        [Option('p', "path", HelpText = "Path to the train folder.")]
        public string TrainFolderPath { get; set; }

        [Option('f', "test", HelpText = "Path to the test folder.")]
        public string TestFolderPath { get; set; }

        [Option('s', "states", HelpText = "Number of states")]
        public int States { get; set; }

        [Option('v', "vector", HelpText = "Lenght of sequence vector", Default = 13)]
        public int DataSet { get; set; }
    }
}
