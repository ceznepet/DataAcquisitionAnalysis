namespace Common.Models.Measurement
{
    public class VariableComponent
    {
        public uint Length = 4;
        public uint BytOffset;
        public string Name;
        public float Value;

        public VariableComponent(uint bytOffset, string name, uint lenght)
        {
            BytOffset = bytOffset;
            Name = name;
            Length = lenght;
        }
    }
}
