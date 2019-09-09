using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class Operation
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public double[][] Data { get; set; }

        public Operation(double[][] data, string name, string fileName)
        {
            Name = name;
            Data = data;
            FileName = fileName;
        }
    }
}
