namespace KunbusRevolutionPiModule.Kunbus
{
    public struct ProfinetIOConfig
    {
        /// <summary>
        /// Sampling period in milliseconds.
        /// </summary>
        public int Period;

        /// <summary>
        /// Indicator of endianness of the <strong>target</strong> device (e.g. PLC).
        /// </summary>
        public bool BigEndian;
    }
}
