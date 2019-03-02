using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("kunbus", HelpText = "Start Kunbus modeule for data acquisition from Profinet.")]
    public class KunbusOptions
    {
        [Option('b', "byte", HelpText = "Number of bytes to read", Default = 4)]
        public uint NumberOfBytes { get; set; }
        //[Option('d', "database", HelpText = "Name of database.")]
        //public string Database { get; set; }
    }
}
