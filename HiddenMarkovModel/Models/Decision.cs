using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Transactions;

namespace HMModel.Models
{
    public class Decision
    {
        public double[][] Probability { get; set; }
        public int State { get; set; }


        public Decision(double[][] probability, int state)
        {
            Probability = probability;
            State = state;
        }
    }
}
