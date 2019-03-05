using System;
using System.Runtime.InteropServices;

namespace KunbusRevolutionPiModule.KunbusPNS
{
    public class KunbusPNSPIVariable
    {
        /**
         * Variable name
         */
        [MarshalAs(UnmanagedType.LPStr, SizeConst = 32)]
        string strVarName;

        /**
         * Address of the byte in the process image
         */
        UInt16 i16uAddress;

        /**
         * 0-7 bit position, >= 8 whole byte
         */
        byte i8uBit;

        /**
         * length of the variable in bits. Possible values are 1, 8, 16 and 32
         */
        UInt16 i16uLength;

    }
}