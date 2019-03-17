using System;
using System.Collections.Generic;
using System.Text;

namespace HiddenMarkovModel.Models.Data
{
    public class TimeSeries
    {
        public string Name { get; set; }
        public double[] Data { get; set; }

        public TimeSeries(double[] data, string name)
        {
            Name = name;
            Data = data;
        }
    }
}
