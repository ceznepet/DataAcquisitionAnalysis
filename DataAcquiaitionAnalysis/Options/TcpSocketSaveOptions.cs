using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("socket", HelpText = "Use given IP and Port for connection to server.")]
    public class TcpSocketSaveOptions
    {
        [Option('i', "ip", HelpText = "IP address", Required = true)]
        public string IP { get; set; }

        [Option('p', "port", HelpText = "Number of port", Required = true)]
        public string Port { get; set; }
    }
}
