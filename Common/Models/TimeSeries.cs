using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
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
