using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("kunbus", HelpText = "Start Kunbus modeule for data acquisition from Profinet.")]
    public class KunbusOptions
    {
        [Option('f', "file", HelpText = "Configuration file for I/O measurement set up.", Default = @"measureVariable.json")]
        public string ConfigurationFile { get; set; }

        [Option('b', "byte", HelpText = "Number of bytes to read", Default = 4)]
        public int NumberOfBytes { get; set; }

        [Option('e', "endian", HelpText = "BigEndian? 1 - true, 0 - false", Default = "1")]
        public string BigEndian { get; set; }

        [Option('d', "database", HelpText = "Name of the database in MongoDB on localhost", Default = "Measurement")]
        public string Database { get; set; }

        [Option('o', "document", HelpText = "Name of the document in database", Default = "TestbedTest")]
        public string Document { get; set; }

        [Option('l', "local", HelpText = "Network location of MongoDB.", Default = "mongodb://192.268.0.55:27017")]
        public string DatabaseLocation { get; set; }
    }
}
