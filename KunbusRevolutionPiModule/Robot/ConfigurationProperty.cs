using System;
using System.Collections.Generic;
using System.Text;
using KunbusRevolutionPiModule.Kunbus;

namespace KunbusRevolutionPiModule.Robot
{
    public class ConfigurationProperty
    {
        public string Variable;
        public uint BytOffset;
        public uint Length;
        public KunbusIOData IoData;
        public ConfigurationProperty(string variable, uint bytOffset, uint length)
        {
            Variable = variable;
            BytOffset = bytOffset;
            Length = length;
            IoData = new KunbusIOData(BytOffset, Variable);
            IoData.Length = Length;
        }
    }
}
