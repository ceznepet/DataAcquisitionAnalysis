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

                Console.WriteLine();

                var profinetIocStatus =
                    KunbusRevolutionPiWrapper.piControlRead(offset, (uint) NumberOfBytes, outData);

                Console.WriteLine("Status of connection is: {0}", profinetIocStatus);

                if (_config.BigEndian ^ BitConverter.IsLittleEndian) Array.Reverse(outData);

                var pokus = outData.OutputConversion(new int()) / 1000;

                if (profinetIocStatus > 0)
                {
                    foreach (var bit in outData) Console.Write("{0}, ", bit);
                    Console.WriteLine();
                    Console.WriteLine("From convertor: {0}", pokus);
                }
                else
                {
                    Console.WriteLine("Hups...");
                }
            }
        }
    }
}