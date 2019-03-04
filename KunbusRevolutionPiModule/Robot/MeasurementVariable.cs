using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace KunbusRevolutionPiModule.Robot
{
    public class MeasurementVariable
    {
        public string VariableName { get; }
        public string NameInRobot { get; }
        public List<KunbusIOData> Joints = new List<KunbusIOData>();
        [BsonId]
        public DateTime Time { get; set; }        

        public MeasurementVariable(string variableName, string nameInRobot)
        {
            VariableName = variableName;
            NameInRobot = nameInRobot;
        }
    }
}
