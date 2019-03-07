using System.Collections.Generic;

namespace KunbusRevolutionPiModule.Robot
{
    public class MongoVariables
    {
        public string RobotTime { get; set; }
        public string SaveTime { get; set; }
        public int ProgramNumber { get; set; }
        public List<MeasurementVariable> Variables = new List<MeasurementVariable>();
    }
}
