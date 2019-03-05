using System.Collections.Generic;

namespace KunbusRevolutionPiModule.Robot
{
    public class MeasurementVariable
    {
        public string VariableName { get; }
        public string NameInRobot { get; }
        public List<KunbusIOData> Joints = new List<KunbusIOData>();             

        public MeasurementVariable(string variableName, string nameInRobot)
        {
            VariableName = variableName;
            NameInRobot = nameInRobot;
        }
    }
}
