using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KunbusRevolutionPiModule.Robot
{
    public class Measurement
    {        
        public string Time { get; set; }
        public List<MeasurementVariable> Variables = new List<MeasurementVariable>();
    }
}
