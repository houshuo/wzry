namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TdirStaticInfo
    {
        public int cltAttr;
        public int cltAttr1;
        [MarshalAs(UnmanagedType.LPStr)]
        public string appAttr;
        [MarshalAs(UnmanagedType.LPStr)]
        public string curVersion;
        public int windowAttr;
        public int appID;
        public int cltFlag;
        public uint bitmapMask;
        [MarshalAs(UnmanagedType.LPStr)]
        public string virConnUrl;
    }
}

