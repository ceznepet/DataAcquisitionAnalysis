namespace Common.Models
{
    public class VariableComponent
    {
        public uint Length = 4;
        public uint BytOffset;
        public string Name;
        public float Value;

        public VariableComponent(uint bytOffset, string name)
        {
            BytOffset = bytOffset;
            Name = name;
        }
    }
}
