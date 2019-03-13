using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("mongo", HelpText = "Load and save data into file from MongoDB.")]
    public class LoadMongoDataOptions
    {
        [Option('l', "local", HelpText = "Network location of MongoDB.", Default = "mongodb://10.35.91.210:27017")]
        public string DatabaseLocation { get; set; }

        [Option('d', "database", HelpText = "Name of database.", Default = "Measurement")]
        public string Database { get; set; }

        [Option('c', "document", HelpText = "Name of document.", Required = true)]
        public string Document { get; set; }

        [Option('p', "profinet", HelpText = "Is used profinet communication? 0 - no, 1 - yes.", Default = "0")]
        public string Profinet { get; set; }

        [Option('f', "folder", HelpText = "Folder for saving the data from DB.", Required = true)]
        public string Folder { get; set; }

        [Option('n', "filderName", HelpText = "Name of the output file, time is automaticly included.",
            Default = "measurement")]
        public string FilderName { get; set; }

        [Option('s', "sorted", HelpText = "Sorted output data. Yes or No", Default = "Yes")]
        public string Sorted { get; set; }
    }
}
