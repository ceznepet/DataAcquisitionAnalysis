using System;
using System.Runtime.InteropServices;

namespace KunbusRevolutionPiModule.Kunbus
{
    public struct KunbusProfinetCardDeviceInfo
    {
        /**
         * Address of module in current configuration
         */
        byte i8uAddress;

        /**
         * serial number of module
         */
        UInt32 i32uSerialnumber;

        /**
         * Type identifier of module
         */
        UInt16 i16uModuleType;

        /**
         * hardware revision
         */
        UInt16 i16uHW_Revision;

        /**
         * major software version
         */
        UInt16 i16uSW_Major;

        /**
         * minor software version
         */
        UInt16 i16uSW_Minor;

        /**
         * svn revision of software
         */
        UInt32 i32uSVN_Revision;

        /**
         * length in bytes of all input values together
         */
        UInt16 i16uInputLength;

        /**
         * length in bytes of all output values together
         */
        UInt16 i16uOutputLength;

        /**
         * length in bytes of all config values together
         */
        UInt16 i16uConfigLength;

        /**
         * offset in process image
         */
        UInt16 i16uBaseOffset;

        /**
         * offset in process image of first input byte
         */
        UInt16 i16uInputOffset;

        /**
         * offset in process image of first output byte
         */
        UInt16 i16uOutputOffset;

        /**
         * offset in process image of first config byte
         */
        UInt16 i16uConfigOffset;

        /**
         * index of entry
         */
        UInt16 i16uFirstEntry;

        /**
         * number of entries in process image
         */
        UInt16 i16uEntries;

        /**
         * fieldbus state of piGate Module
         */
        byte i8uModuleState;

        /**
         * == 0 means that the module is not present and no data is available
         */
        byte i8uActive;

        /**
         * space for future extensions without changing the size of the struct
         */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
        byte[] i8uReserve;         
    }
}