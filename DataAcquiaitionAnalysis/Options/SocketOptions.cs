using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("socket", HelpText = "Use given IP and Port for connection to server.")]
    public class SocketOptions
    {
        [Option('l', "local", HelpText = "Network location of MongoDB.", Default = "mongodb://10.35.91.210:27017")]
        public string DatabaseLocation { get; set; }

        [Option('i', "ip", HelpText = "IP address of TCP/UDP server", Default = "10.35.91.61")]
        public string Ip { get; set; }

        [Option('s', "protocol", HelpText = "Protocol type (0 - TCP/IP, 1 - UDP/IP)", Default = 0)]
        public int Protocol { get; set; }

        [Option('p', "port", HelpText = "Port number", Default = 54600)]
        public int Port { get; set; }

        [Option('d', "database", HelpText = "Name of the database in MongoDB on localhost", Default = "KunbusIoVariables")]
        public string Database { get; set; }

        [Option('o', "document", HelpText = "Name of the document in database", Default = "TestbedTest_EthernetKRL")]
        public string Document { get; set; }
    }
}
