using System.Collections.Generic;

namespace Common.Models
{
    public class Variable
    {
        public string VariableName { get; }
        public string NameInRobot { get; }
        public List<VariableComponent> Joints = new List<VariableComponent>();

        public Variable(string variableName, string nameInRobot)
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
