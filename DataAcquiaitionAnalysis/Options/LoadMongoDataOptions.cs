using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("mongo", HelpText = "Load and save data into file from MongoDB.")]
    public class LoadMongoDataOptions
    {
        [Option('s', "setup", HelpText = "Setup file destination.", Required = true)]
        public string Setup { get; set; }
    }
}
