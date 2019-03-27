using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Common.Loaders;
using Common.Savers;

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
            SaveFile = Path.Combine(saveFile, "moments");
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
                        programNumber = (int)value[1];
                        currentOperation = new OperationMoments(programNumber, (int)size);
                    }
                    currentOperation.AddData(value);

                }
                currentOperation.ComputeMoments();
                Moments.Add(currentOperation);
            }
        }


        private void PrintMoments()
        {
            CsvSavers.ToCsvFile("timestamp,opnum,vel_A1_m1,vel_A1_m2,vel_A1_m3,vel_A2_m1,vel_A2_m2,vel_A2_m3,vel_A3_m1,vel_A3_m2,vel_A3_m3,vel_A4_m1,vel_A4_m2,vel_A4_m3,vel_A5_m1,vel_A5_m2,vel_A5_m3,vel_A6_m1,vel_A6_m2,vel_A6_m3,cur_A1_m1,cur_A1_m2,cur_A1_m3,cur_A2_m1,cur_A2_m2,cur_A2_m3,cur_A3_m1,cur_A3_m2,cur_A3_m3,cur_A4_m1,cur_A4_m2,cur_A4_m3,cur_A5_m1,cur_A5_m2,cur_A5_m3,cur_A6_m1,cur_A6_m2,cur_A6_m3", SaveFile);
            foreach (var row in Moments)
            {
                row.PrintMoments(SaveFile);
            }
        }

    }
}
