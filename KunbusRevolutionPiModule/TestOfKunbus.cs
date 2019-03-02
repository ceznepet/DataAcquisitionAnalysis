using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using KunbusRevolutionPiModule.KunbusPNS;
using KunbusRevolutionPiModule.Wrapper;

namespace KunbusRevolutionPiModule
{
    public class TestOfKunbus
    {
        private readonly ProfinetIOConfig config;
        private readonly bool deviceActive = true;
        private readonly Thread samplerThread;

        public TestOfKunbus()
        {
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

                int dataRead = 4;
                uint offset = 0;
                IntPtr dataPtr = new IntPtr(0);
                var outData = new byte[dataRead];

                Console.WriteLine();

                var profinetIocStatus =
                    KunbusRevolutionPiWrapper.piControlRead(offset, (uint)dataRead, outData);
                //Console.WriteLine(PnSimaticnetErrorNumber());
                
                Console.WriteLine("Status of connection is: {0}", profinetIocStatus);
                // if endianing is reverse, reorder the array
                if (config.BigEndian ^ BitConverter.IsLittleEndian) Array.Reverse(outData);

                var pokus = BitConverter.ToString(outData);

                if (profinetIocStatus != (int) KunbusProfinetIOStatus.HWCONFIG_ERR)
                {
                    foreach (var bit in outData)
                    {
                        Console.Write("{0}, ", bit);
                    }
                    Console.WriteLine();
                    Console.WriteLine(pokus);
                    Console.ReadKey();
                }
                else
                    Console.WriteLine("Hups...");
            }

        }
    }
}
