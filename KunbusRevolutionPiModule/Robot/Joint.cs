namespace KunbusRevolutionPiModule.Robot
{
    public class Joint
    {
        public readonly uint _length = 4;
        public uint BytOffset { get; }
        public string JointName { get; }
        public float Outputs { get; set; }

        public Joint(uint bytOffset, string jointName)
        {
            BytOffset = bytOffset;
            JointName = jointName;
        }
    }
}