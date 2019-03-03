using System;
using System.Runtime.InteropServices;
using System.Threading;
using KunbusRevolutionPiModule.Conversion;
using KunbusRevolutionPiModule.KunbusPNS;
using KunbusRevolutionPiModule.Wrapper;

namespace KunbusRevolutionPiModule
{
    public class TestOfKunbus
    {
        private readonly ProfinetIOConfig _config;
        private readonly bool deviceActive = true;
        private readonly Thread _samplerThread;
        private uint NumberOfBytes { get; set; }

        public TestOfKunbus(uint numberOfBytes, bool endian)
        {
            NumberOfBytes = numberOfBytes;
            _config = new ProfinetIOConfig {Period = 500, BigEndian = endian};

            KunbusRevolutionPiWrapper.piControlOpen();
            _samplerThread = new Thread(GatherData);
            _samplerThread.Start();
        }

        ~TestOfKunbus()
        {
            KunbusRevolutionPiWrapper.piControlClose();
            _samplerThread.Interrupt();
        }

        private void GatherData()
        {
            while (true)
            {
                Thread.Sleep(_config.Period);

                if (!deviceActive) continue;

                uint offset = 0;
                var outData = new byte[NumberOfBytes];


                var readBytes =
                    KunbusRevolutionPiWrapper.piControlRead(offset, (uint) NumberOfBytes, outData);

                if (_config.BigEndian ^ BitConverter.IsLittleEndian)
                {
                    Array.Reverse(outData);
                }

                if (readBytes == NumberOfBytes)
                {
                    Console.WriteLine("Act_pos A3: {0}", BitConverter.ToSingle(outData, 0));
                }
                else
                {
                    Console.WriteLine("Hups...");
                }
            }
        }
    }
}