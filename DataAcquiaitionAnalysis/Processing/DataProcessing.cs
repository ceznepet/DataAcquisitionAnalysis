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

        private string Folder { get; }
        private string SaveFile { get; }
        private List<OperationMoments> Moments { get; }

        public DataProcessing(string folder, string saveFile)
        {
            Folder = folder;
            SaveFile = saveFile + @"\moments";
            Moments = new List<OperationMoments>();
            var data = MatLoaders.LoadPrograms(Folder, (int)ParsingLength.TakeTwelve,
                                          (int)ParsingLength.WithOperation, true);
            ComputeMoments(data, ParsingLength.TakeTwelve);
            PrintMoments();
        }

        private void ComputeMoments(Dictionary<int, List<double[]>> data, ParsingLength size)
        {
            foreach (var product in data)
            {
                var programNumber = (int)product.Value[0][1];
                var currentOperation = new OperationMoments(programNumber, (int)size);
                foreach (var value in product.Value)
                {
                    if (programNumber != (int)value[1])
                    {
                        currentOperation.ComputeMoments();
                        Moments.Add(currentOperation);
                        programNumber = (int) value[1];
                        currentOperation = new OperationMoments(programNumber, (int) size);
                    }
                    currentOperation.AddData(value);

                }
                currentOperation.ComputeMoments();
                Moments.Add(currentOperation);
            }
        }


        private void PrintMoments()
        {
            foreach (var row in Moments)
            {
                row.PrintMoments(SaveFile);
            }
        }

    }
}
