using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models.Setup
{
    public class MarkovSetup
    {
        public bool DoClassification { get; set; }

        public string TrainFolderPath { get; set; }

        public string TestFolderPath { get; set; }

        public string DataFolderPath { get; set; }

        public string ModelFolder { get; set; }

        public int States { get; set; }

        public int TakeVector { get; set; }

        public ICollection<int> SignificantFeatures { get; set; }
    }
}
