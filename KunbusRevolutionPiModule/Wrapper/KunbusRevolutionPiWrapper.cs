using System.Runtime.InteropServices;
using KunbusRevolutionPiModule.KunbusPNS;

namespace KunbusRevolutionPiModule.Wrapper
{
    internal class KunbusRevolutionPiWrapper
    {
        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void piControlOpen();

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void piControlClose();

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlReset();

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlRead(uint Offset, uint Length, byte[] pData);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlWrite(uint Offset, uint Length, byte[] pData);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlGetDeviceInfo(ref KunbusPNSDeviceInfo pDev);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlGetDeviceInfoList(ref KunbusPNSDeviceInfo pDev);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlGetBitValue(ref KunbusPNSPIValue pSpiValue);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlSetBitValue(ref KunbusPNSPIValue pSpiValue);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlGetVariableInfo(ref KunbusPNSPIVariable pSpiVariable);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlFindVariable([MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlResetCounter(int address, int bitfield);

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlWaitForEvent();

        [DllImport("resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlUpdateFirmware(uint addr_p);
    }
}