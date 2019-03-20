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
            ComputeMoments(data, ParsingLength.TakeTwelve);
        }

        private void ComputeMoments(Dictionary<int, List<double[]>> data, ParsingLength size)
        {
            var momemts = new List<OperationMoments>();
            foreach (var product in data)
            {
                var programNumber = (int)product.Value[0][1];
                var currentOperation = new OperationMoments(programNumber, (int)size);
                foreach (var value in product.Value)
                {
                    if (programNumber != (int)value[1])
                    {
                        currentOperation.ComputeMoments();
                        momemts.Add(currentOperation);
                        programNumber = (int) value[1];
                        currentOperation = new OperationMoments(programNumber, (int) size);
                    }
                    currentOperation.AddData(value);

                }
            }
        }

    }
}
