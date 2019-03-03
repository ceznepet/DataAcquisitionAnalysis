using System;
using System.Collections.Generic;
using System.Text;

namespace KunbusRevolutionPiModule.Conversion
{
    public static class ByteExtension
    {
        public static dynamic OutputConversion(this byte[] input, dynamic convertTo)
        {
            dynamic output;
            if (convertTo is ulong)
            {
                output = BitConverter.ToUInt64(input, 0);
            }
            else if (convertTo is uint)
            {
                output = BitConverter.ToUInt32(input, 0);
            }
            else if (convertTo is ushort)
            {
                output = BitConverter.ToUInt16(input, 0);
            }
            else if (convertTo is long)
            {
                output = BitConverter.ToInt64(input, 0);
            }
            else if (convertTo is int)
            {
                output = BitConverter.ToInt32(input, 0);
            }
            else if (convertTo is short)
            {
                output = BitConverter.ToInt16(input, 0);
            }
            else if (convertTo is byte)
            {
                output = input[0];
            }
            else if(convertTo is float)
            {
                output = BitConverter.ToSingle(input, 0);
            }
            else if (convertTo is double)
            {
                output = BitConverter.ToDouble(input, 0);
            }
            else
            {
                throw new InvalidCastException("Unknown format to convert to");
            }

            return output;
        }
    }
}
