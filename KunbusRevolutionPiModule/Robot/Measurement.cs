using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KunbusRevolutionPiModule.Robot
{
    public class Measurement
    {        
        public string RobotTime { get; set; }
        public string SaveTime { get; set; }
        public List<MeasurementVariable> Variables = new List<MeasurementVariable>();
    }
}
