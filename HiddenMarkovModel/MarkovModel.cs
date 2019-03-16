using System.Collections.Generic;
using System.Linq;
using System.Data;
using HiddenMarkovModel.Loaders;
using DatabaseModule.Extensions;
using HiddenMarkovModel.Models;

namespace HiddenMarkovModel
{
    public class MarkovModel
    {
        private string FilePath { get; }
        private DataTable MeasuredDataT { get; set; }
        private HashSet<int> ProgramNumbers { get; }
        private Dictionary<int, List<double[]>> Dictionary = new Dictionary<int, List<double[]>>();
        private Learning Teacher { get; set; }

        public MarkovModel(string filePath)
        {
            FilePath = filePath;
            MeasuredDataT = Loader.LoadCSV(filePath);
            ProgramNumbers = new HashSet<int>();
            ToDictionary();
            SortList();
        }

        private void ToDictionary()
        {
            var list = MeasuredDataT.Rows.Cast<DataRow>().ToList();
            foreach (var row in list)
            {
                var data = row.ItemArray.Cast<string>().ToArray();
                var lis = row.ItemArray.Cast<string>().Skip(2).ToArray();
                var programNumber = int.Parse(data[1]);
                if (!ProgramNumbers.Contains(programNumber))
                {
                    Dictionary.Add(programNumber, new List<double[]>());
                    ProgramNumbers.Add(programNumber);
                }

                Dictionary[programNumber].Add(lis.ToDoubleArray(24));
            }
        }

        private void SortList()
        {
            var sortedDict = from entry in Dictionary orderby entry.Key ascending select entry;            
            Learning.StartTeaching(sortedDict, 24);

        }
    }
}
