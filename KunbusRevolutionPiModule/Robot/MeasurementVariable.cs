using System;
using System.Collections.Generic;
using System.Text;

namespace KunbusRevolutionPiModule.Robot
{
    public class MeasurementVariable
    {
        public string VariableName { get; }
        public string NameInRobot { get; }
        public List<Joint> Joints = new List<Joint>();

        public MeasurementVariable(string variableName, string nameInRobot)
        {
            VariableName = variableName;
            NameInRobot = nameInRobot;
        }
    }
}
