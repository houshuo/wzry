namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TdirDynamicInfo
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string appAttr;
        [MarshalAs(UnmanagedType.LPStr)]
        public string connectUrl;
        [MarshalAs(UnmanagedType.LPStr)]
        public string pingUrl;
    }
}

