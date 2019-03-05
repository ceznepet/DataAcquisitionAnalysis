using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("mongo", HelpText = "Load and save data into file from MongoDB.")]
    public class LoadMongoDataOptions
    {
        [Option('d', "database", HelpText = "Name of database.", Required = true)]
        public string Database { get; set; }

        [Option('c', "document", HelpText = "Name of document.", Required = true)]
        public string Document { get; set; }

        [Option('p', "profinet", HelpText = "Is used profinet communication? 0 - no, 1 - yes.", Required = true)]
        public string Profinet { get; set; }

        [Option('f', "folder", HelpText = "Folder for saving the data from DB.", Required = true)]
        public string Folder { get; set; }

        [Option('n', "folderName", HelpText = "Name of the output file, time is automaticly included.", Default = "measurement")]
        public string FolderName { get; set; }
    }
}
