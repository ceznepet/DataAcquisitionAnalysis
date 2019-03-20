﻿using CommandLine;

namespace DataAcquisitionAnalysis.Options
{
    [Verb("data", HelpText = "Processing of data from files.")]
    public class DataProcessingOptions
    {
        [Option('f', "folder", HelpText = "Root folder with files.", Required = true)]
        public string Folder { get; set; }

        [Option('s', "save", HelpText = "Save file name and location.", Required = true)]
        public string SaveFile { get; set; }
    }
}
