using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("online", HelpText = "Online analysis using the Revolution Pi by Kunbus.")]
    public class OnlineClassificationOptions
    {
        [Option('p', "path", HelpText = "Path to the directory with Markov models. Both Discrete and Classifier.",Required = true)]
        public string FolderPath { get; set; }

        [Option('f', "features", HelpText = "Path to the file with selected features.", Default = @"Configuration/features.json")]
        public string Features { get; set; }

        [Option('j', "json", HelpText = "Configuration file for I/O measurement set up.", Default = @"Configuration/measureVariable.json")]
        public string ConfigurationFile { get; set; }

        [Option('e', "endian", HelpText = "BigEndian? 1 - true, 0 - false", Default = "1")]
        public string BigEndian { get; set; }

        [Option('r', "period", HelpText = "Period of reading the I/O", Default = 12)]
        public int Period { get; set; }

    }
}
