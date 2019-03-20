using System.Collections.Generic;

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

    }
}