using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("kunbus", HelpText = "Start Kunbus modeule for data acquisition from Profinet.")]
    public class KunbusOptions
    {
        //[Option('d', "database", HelpText = "Name of database.")]
        //public string Database { get; set; }
    }
}
