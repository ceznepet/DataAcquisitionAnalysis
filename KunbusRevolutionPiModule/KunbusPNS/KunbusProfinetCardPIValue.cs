using System;

namespace KunbusRevolutionPiModule.KunbusPNS
{
    public struct KunbusProfinetCardPIValue
    {
        /**
         * Address of the byte in the process image
         */
        UInt16 i16uAddress;

        /**
         * 0-7 bit position, >= 8 whole byte
         */
        byte i8uBit;

        /**
         * Value: 0/1 for bit access, whole byte otherwise
         */
        byte i8uValue;
    }
}