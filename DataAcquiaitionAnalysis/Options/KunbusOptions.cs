using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("kunbus", HelpText = "Start Kunbus modeule for data acquisition from Profinet.")]
    public class KunbusOptions
    {
        [Option('f', "file", HelpText = "Configuration file for I/O measurement set up.", Default = @"measureVariable.json")]
        public string ConfigurationFile { get; set; }

        [Option('e', "endian", HelpText = "BigEndian? 1 - true, 0 - false", Default = "1")]
        public string BigEndian { get; set; }

        [Option('d', "database", HelpText = "Name of the database in MongoDB on localhost", Default = "Measurement")]
        public string Database { get; set; }

        [Option('o', "document", HelpText = "Name of the document in database", Default = "TestbedTest_Profinet")]
        public string Document { get; set; }

        [Option('l', "local", HelpText = "Network location of MongoDB.", Default = "mongodb://10.35.91.210:27017")]
        public string DatabaseLocation { get; set; }
    }
}
