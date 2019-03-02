using System;
using System.Threading;
using KunbusRevolutionPiModule.Conversion;
using KunbusRevolutionPiModule.KunbusPNS;
using KunbusRevolutionPiModule.Wrapper;

namespace KunbusRevolutionPiModule
{
    public class TestOfKunbus
    {
        private readonly ProfinetIOConfig config;
        private readonly bool deviceActive = true;
        private readonly Thread samplerThread;
        private uint NumberOfBytes { get; set; }

        public TestOfKunbus(uint numberOfBytes)
        {
            NumberOfBytes = numberOfBytes;
            config = new ProfinetIOConfig();
            config.Period = 4;
            config.BigEndian = true;

            KunbusRevolutionPiWrapper.piControlOpen();
            Console.WriteLine("Thread started!!");
            samplerThread = new Thread(GatherData);
            samplerThread.Start();
        }

        ~TestOfKunbus()
        {
            KunbusRevolutionPiWrapper.piControlClose();
            samplerThread.Interrupt();
        }

        private void GatherData()
        {
            while (true)
            {
                Thread.Sleep(config.Period);

                if (!deviceActive) continue;

                uint offset = 0;
                var outData = new byte[NumberOfBytes];

                Console.WriteLine();

                var profinetIocStatus =
                    KunbusRevolutionPiWrapper.piControlRead(offset, (uint)NumberOfBytes, outData);

                Console.WriteLine("Status of connection is: {0}", profinetIocStatus);

                if (config.BigEndian ^ BitConverter.IsLittleEndian) Array.Reverse(outData);

                var pokus = outData.OutputConversion(new Int64());

                if (profinetIocStatus > 0)
                {
                    foreach (var bit in outData) Console.Write("{0}, ", bit);
                    Console.WriteLine();
                    Console.WriteLine(pokus);
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Hups...");
                }
            }
        }
    }
}