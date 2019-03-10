using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseModule.Models.Profinet
{
    public class Measurement
    {
        public string RobotTime { get; set; }
        public string SaveTime { get; set; }
        public int ProgramNumber { get; set; }
        public List<MeasurementVariable> Variables = new List<MeasurementVariable>();

        public IEnumerable<double> GetMeasuredValues()
        {
            foreach (var variable in Variables)
            {
                foreach (var value in variable.GetValues())
                {
                    yield return value;
                }
            }            
        }
    }
}
