using Common.Models;
using System.Collections.Generic;

namespace KunbusRevolutionPiModule.Robot
{
    public class KunbusIoVariables : MeasuredVaribles
    {        
        public List<VariableComponent> ProfinetProperty = new List<VariableComponent>();
        public List<VariableComponent> Time = new List<VariableComponent>();
    }
}
