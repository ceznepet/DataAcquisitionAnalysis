using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("kunbus", HelpText = "Start Kunbus modeule for data acquisition from Profinet.")]
    public class KunbusOptions
    {
        [Option('s', "setup", HelpText = "Setup file destination.", Required = true)]
        public string Setup { get; set; }
    }
}
