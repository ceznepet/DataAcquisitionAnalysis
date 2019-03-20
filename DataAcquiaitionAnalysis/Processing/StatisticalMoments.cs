using System;
using System.Collections.Generic;
using System.Text;

namespace DataAcquisitionAnalysis.Processing
{
    public class StatisticalMoments
    {
        private string OperationName { get; set; }
        public List<double> Moments { get; set; }

        public StatisticalMoments(string operationName)
        {
            OperationName = operationName;
            Moments = new List<double>();
        }
    }
}
