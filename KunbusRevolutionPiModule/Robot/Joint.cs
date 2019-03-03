namespace KunbusRevolutionPiModule.Robot
{
    public class Joint
    {
        public readonly int _length = 4;
        public int BytOffset { get; }
        public string JointName { get; }
        public byte[] Outputs { get; private set; }

        public Joint(int bytOffset, string jointName)
        {
            BytOffset = bytOffset;
            JointName = jointName;
        }

        public void SetOutputs(byte[] outputs)
        {
            Outputs = outputs;
        }
    }
}