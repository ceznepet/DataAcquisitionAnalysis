using System;
using System.Collections.Generic;
using Accord;
using Accord.Math;
using Accord.Statistics;

namespace DataAcquisitionAnalysis.Processing
{
    public class StatisticalMoments
    {
        public List<double> Values { get; set; }
        public double FirstMoment { get; set; }
        public double SecondMoment { get; set; }
        public double ThirdMoment { get; set; }

        public StatisticalMoments()
        {
            Values = new List<double>();
        }

        public void ComputeMoments()
        {
            FirstMoment = Values.ToArray().Mean();
            SecondMoment = Values.ToArray().Variance();
            ThirdMoment = Values.ToArray().Skewness();
        }
    }
}
