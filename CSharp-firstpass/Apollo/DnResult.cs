namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct DnResult
    {
        public int errCode;
        [MarshalAs(UnmanagedType.LPStr)]
        public string errString;
        [MarshalAs(UnmanagedType.LPStr)]
        public string domainName;
        [MarshalAs(UnmanagedType.LPStr)]
        public string value;
    }
}

