using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("markov", HelpText = "Learning model for HMM")]
    public class MarkovOptions
    {
        [Option('f', "file", HelpText = "Path to the path.", Required = true)]
        public string FilePath { get; set; }

    }
}
