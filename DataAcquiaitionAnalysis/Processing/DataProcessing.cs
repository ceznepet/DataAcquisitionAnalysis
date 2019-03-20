using System;
using System.Collections.Generic;
using System.Text;
using Common.Loaders;

namespace DataAcquisitionAnalysis.Processing
{
    public enum ParsingLength
    {
        Axis = 6,
        WithOperation = 8,
        TakeTwelve = 12
    }

    public class DataProcessing
    {

        private string Folder { get; set; }


        public DataProcessing(string folder)
        {
            Folder = folder;
            var data = MatLoaders.LoadPrograms(folder, (int)ParsingLength.TakeTwelve,
                                          (int)ParsingLength.WithOperation, true);
            ComputeMoments(data);
        }

        private void ComputeMoments(Dictionary<int, List<double[]>> data)
        {
            foreach (var product in data)
            {
                var operation = (int)product.Value[0][2];
                foreach (var value in product.Value)
                {
                    if (operation != (int)value[2])
                    {

                    }
                }
            }
        }

    }
}
