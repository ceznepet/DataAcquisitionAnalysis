﻿namespace KunbusRevolutionPiModule.Kunbus
{
    public class KunbusIOData
    {
        public uint Length = 4;
        public uint BytOffset;
        public string Name;
        public float Value;

        public KunbusIOData(uint bytOffset, string name)
        {
            BytOffset = bytOffset;
            Name = name;
        }
    }
}
