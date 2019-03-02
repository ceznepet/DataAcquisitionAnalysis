using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using KunbusRevolutionPiModule.KunbusPNS;
using KunbusRevolutionPiModule.Wrapper;

namespace KunbusRevolutionPiModule
{
    public class TestOfKunbus
    {
        private readonly ProfinetIOConfig config;
        private readonly bool deviceActive = false;
        private readonly Thread samplerThread;

        public TestOfKunbus()
        {
            config = new ProfinetIOConfig();
            config.Period = 4;
            config.BigEndian = true;

            KunbusRevolutionPiWrapper.piControlOpen();
            samplerThread = new Thread(GatherData);
            samplerThread.Start();
        }

        private void GatherData()
        {
            while (true)
            {
                Thread.Sleep(config.Period);

                if (!deviceActive) continue;

                var outData = new byte[16];

                var profinetIocStatus =
                    KunbusRevolutionPiWrapper.piControlRead(0, 16, outData);
                //Console.WriteLine(PnSimaticnetErrorNumber());

                // if endianing is reverse, reorder the array
                if (config.BigEndian ^ BitConverter.IsLittleEndian) Array.Reverse(outData);

                if (profinetIocStatus != (uint)KunbusProfinetIOStatus.OK)
                    Console.WriteLine(outData);
                else
                    Console.WriteLine("Hups...");
            }
        }
    }
}
