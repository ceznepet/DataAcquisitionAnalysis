using System;
using System.Collections.Generic;
using System.Text;

namespace DataAcquisitionAnalysis.Processing
{
    public class OperationMoments
    {
        public  List<StatisticalMoments> StatisticalMomentses { get; set; }

        public StatisticalMoments VelocityA1 { get; set; }
        public StatisticalMoments VelocityA2 { get; set; }
        public StatisticalMoments VelocityA3 { get; set; }
        public StatisticalMoments VelocityA4 { get; set; }
        public StatisticalMoments VelocityA5 { get; set; }
        public StatisticalMoments VelocityA6 { get; set; }
        public StatisticalMoments CurrentA1 { get; set; }
        public StatisticalMoments CurrentA2 { get; set; }
        public StatisticalMoments CurrentA3 { get; set; }
        public StatisticalMoments CurrentA4 { get; set; }
        public StatisticalMoments CurrentA5 { get; set; }
        public StatisticalMoments CurrentA6 { get; set; }


        public OperationMoments()
        {
            StatisticalMomentses = new List<StatisticalMoments>();
        }

    }
}
