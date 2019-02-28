using System;
using System.Collections.Generic;
using System.Text;

namespace KunbusRevolutionPiModule
{
    class KunbusRevolutionPiWrapper
    {
        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void piControlOpen();

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void piControlClose();

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 piControlReset();

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 piControlRead(UInt32 Offset, UInt32 Length, byte[] pData);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 piControlWrite(UInt32 Offset, UInt32 Length, byte[] pData);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 piControlGetDeviceInfo(ref KunbusPNSDeviceInfo pDev);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 piControlGetDeviceInfoList(ref KunbusPNSDeviceInfo pDev);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 piControlGetBitValue(ref KunbusPNSPIValue pSpiValue);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 piControlSetBitValue(ref KunbusPNSPIValue pSpiValue);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 piControlGetVariableInfo(ref KunbusPNSPIVariable pSpiVariable);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 piControlFindVariable([MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 piControlResetCounter(Int32 address, Int32 bitfield);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 piControlWaitForEvent();

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 piControlUpdateFirmware(UInt32 addr_p);
    }
}
