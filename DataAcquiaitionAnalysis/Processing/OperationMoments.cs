using System;
using System.Collections.Generic;
using Common.Savers;

namespace DataAcquisitionAnalysis.Processing
{
    public class OperationMoments
    {
        public List<StatisticalMoments> StatisticalMoments { get; set; }
        public int OperationNumber { get; set; }

        public OperationMoments(int operationNumber, int size)
        {
            OperationNumber = operationNumber;

            StatisticalMoments = new List<StatisticalMoments>(size);
            for (var i = 0; i < size; i++)
            {
                StatisticalMoments.Add(new StatisticalMoments());
            }
        }

        public void AddData(double[] values)
        {
            var i = 2;
            foreach (var variable in StatisticalMoments)
            {
                variable.Values.Add(values[i]);
                i++;
            }
        }

        public void ComputeMoments()
        {
            foreach (var variable in StatisticalMoments)
            {
                variable.ComputeMoments();
            }
        }

        public void PrintMoments(string fileName)
        {
            var printArray = new List<double>(StatisticalMoments.Count * 3);
            foreach (var variable in StatisticalMoments)
            {
                printArray.Add(variable.FirstMoment);
                printArray.Add(variable.SecondMoment);
                printArray.Add(variable.ThirdMoment);
            }
            CsvSavers.ToCsvFile(printArray.ToArray(), OperationNumber, fileName);           
        }

    }
}