using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using HiddenMarkovModel.Loaders;

namespace HiddenMarkovModel
{
    public class MarkovModel
    {
        private string FilePath { get; }
        private DataTable MeasuredDataT { get; }

        public MarkovModel(string filePath)
        {
            FilePath = filePath;
            MeasuredDataT = Loader.LoadCSV(filePath);
        }
    }
}
