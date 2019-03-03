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
                    KunbusRevolutionPiWrapper.piControlRead(offset, (uint)NumberOfBytes, outData);

                Console.WriteLine("Status of connection is: {0}", profinetIocStatus);

                if (_config.BigEndian ^ BitConverter.IsLittleEndian) Array.Reverse(outData);

                var pokus = outData.OutputConversion(new int()) / 1000;

                if (profinetIocStatus > 0)
                {
                    foreach (var bit in outData) Console.Write("{0}, ", bit);
                    Console.WriteLine();
                    Console.WriteLine("From convertor: {0}",pokus);
                    Console.WriteLine("From ByteArray: {0}",FromByteArray(outData, 0) / 1000);
                }
                else
                {
                    Console.WriteLine("Hups...");
                }
            }
        }

        public static float FromByteArray(byte[] arr, int ix = 0)
        {
            var uitos = new UInt32ToSingle
            {
                Byte0 = arr[ix],
                Byte1 = arr[ix + 1],
                Byte2 = arr[ix + 2],
                Byte3 = arr[ix + 3],
            };

            return uitos.Single;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct UInt32ToSingle
        {
            [FieldOffset(0)]
            public uint UInt32;

            [FieldOffset(0)]
            public float Single;

            [FieldOffset(0)]
            public byte Byte0;

            [FieldOffset(1)]
            public byte Byte1;

            [FieldOffset(2)]
            public byte Byte2;

            [FieldOffset(3)]
            public byte Byte3;
        }
    }
}