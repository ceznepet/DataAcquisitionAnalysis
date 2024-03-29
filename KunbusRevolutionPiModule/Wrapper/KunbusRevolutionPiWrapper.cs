﻿using System.Runtime.InteropServices;
using KunbusRevolutionPiModule.KunbusPNS;

namespace KunbusRevolutionPiModule.Wrapper
{
    internal class KunbusRevolutionPiWrapper
    {
        [DllImport("Resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void piControlOpen();

        [DllImport("Resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void piControlClose();

        [DllImport("Resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlReset();

        [DllImport("Resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlRead(uint Offset, uint Length, byte[] pData);

        [DllImport("Resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlWrite(uint Offset, uint Length, byte[] pData);

        [DllImport("Resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlGetDeviceInfo(ref KunbusPNSDeviceInfo pDev);

        [DllImport("Resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlGetDeviceInfoList(ref KunbusPNSDeviceInfo pDev);

        [DllImport("Resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlGetBitValue(ref KunbusPNSPIValue pSpiValue);

        [DllImport("Resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlSetBitValue(ref KunbusPNSPIValue pSpiValue);

        [DllImport("Resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlGetVariableInfo(ref KunbusPNSPIVariable pSpiVariable);

        [DllImport("Resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlFindVariable([MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("Resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlResetCounter(int address, int bitfield);

        [DllImport("Resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlWaitForEvent();

        [DllImport("Resources/libkunbuspn.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int piControlUpdateFirmware(uint addr_p);
    }
}