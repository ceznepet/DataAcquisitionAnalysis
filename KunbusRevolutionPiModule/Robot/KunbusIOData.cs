namespace KunbusRevolutionPiModule.Robot
{
    public class KunbusIOData
    {
        public readonly uint _length = 4;
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
