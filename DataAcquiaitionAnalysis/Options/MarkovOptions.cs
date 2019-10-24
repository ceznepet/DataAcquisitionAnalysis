using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("markov", HelpText = "Learning model for HMM")]
    public class MarkovOptions
    {
        [Option('s', "setup", HelpText = "Setup file destination.", Required = true)]
        public string Setup { get; set; }
    }
}
