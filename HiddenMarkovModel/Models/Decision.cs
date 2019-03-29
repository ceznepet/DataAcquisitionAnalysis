using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Transactions;

namespace HMModel.Models
{
    public class Decision
    {
        public double Probability { get; set; }
        public double ClassifierProbability { get; set; }
        public int State { get; set; }


        public Decision(double classifier, double probability, int state)
        {
            ClassifierProbability = classifier;
            Probability = probability;
            State = state;
        }
    }
}
