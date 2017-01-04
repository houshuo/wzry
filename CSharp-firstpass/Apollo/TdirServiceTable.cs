namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TdirServiceTable
    {
        public uint updateTime;
        public uint bitMap;
        public uint userAttr;
        public int zoneID;
        public uint appLen;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x200)]
        public byte[] appBuff;
    }
}

