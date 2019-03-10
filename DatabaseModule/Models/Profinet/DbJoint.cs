using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseModule.Models.Profinet
{
    public class DbJoint
    {
        public uint Length = 4;
        public uint BytOffset;
        public string Name;
        public float Value;

        public DbJoint(uint bytOffset, string name)
        {
            BytOffset = bytOffset;
            Name = name;
        }
    }
}
