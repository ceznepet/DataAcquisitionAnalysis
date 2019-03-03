using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("kunbus", HelpText = "Start Kunbus modeule for data acquisition from Profinet.")]
    public class KunbusOptions
    {
        [Option('b', "byte", HelpText = "Number of bytes to read", Default = 4)]
        public int NumberOfBytes { get; set; }

        [Option('e', "endian", HelpText = "BigEndian? 1 - true, 0 - false", Default = "1")]
        public string BigEndian { get; set; }
        //[Option('d', "database", HelpText = "Name of database.")]
        //public string Database { get; set; }
    }
}
