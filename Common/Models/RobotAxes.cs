using System.Collections.Generic;

namespace Common.Models
{
    public class RobotAxes
    {
        public double A1 { get; set; }
        public double A2 { get; set; }
        public double A3 { get; set; }
        public double A4 { get; set; }
        public double A5 { get; set; }
        public double A6 { get; set; }

        public List<double> Axis = new List<double>();

        public void CreateList()
        {
            Axis.Add(A1);
            Axis.Add(A2);
            Axis.Add(A3);
            Axis.Add(A4);
            Axis.Add(A5);
            Axis.Add(A6);
        }
    }
}
