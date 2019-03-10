using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseModule.Models.Profinet
{
    public class MeasurementVariable
    {
        public string VariableName { get; }
        public string NameInRobot { get; }
        public List<DbJoint> Joints = new List<DbJoint>();

        public MeasurementVariable(string variableName, string nameInRobot)
        {
            VariableName = variableName;
            NameInRobot = nameInRobot;
        }

        public IEnumerable<double> GetValues()
        {
            foreach (var joint in Joints)
            {
                yield return joint.Value;
            }
        }
    }
}
