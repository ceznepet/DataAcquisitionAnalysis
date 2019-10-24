using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models.Setup
{
    public class MarkovSetup
    {
        public bool DoClassification { get; set; }
        public bool LearnFromDB { get; set; }
        public string DataFolder { get; set; }
        public string DataFolderPath { get; set; }
        public string ModelFolder { get; set; }
        public int States { get; set; }
        public int TakeVector { get; set; }
        public string DatabaseLocation { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseCollection { get; set; }
        public float TrainDatasetSize { get; set; }
        public ICollection<int> SignificantFeatures { get; set; }
    }
}
