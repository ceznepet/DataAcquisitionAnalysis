using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Transactions;

namespace MarkovModule.Models
{
    public class Decision
    {
        public double Probability { get; set; }
        public double ClassifierProbability { get; set; }
        public double[] Sequence { get; set; }
        public int State { get; set; }

        public double LogLikelihoodDifferences { get; set; }


        public Decision(double classifier, double probability, int state, double[] sequence)
        {
            ClassifierProbability = classifier;
            Probability = probability;
            State = state;
            Sequence = sequence;
        }

        public Decision(double classifier, double logDifferences, int state)
        {
            ClassifierProbability = classifier;
            LogLikelihoodDifferences = logDifferences;
            State = state;
        }

    }
}
