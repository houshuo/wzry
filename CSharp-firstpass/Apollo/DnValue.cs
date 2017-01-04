namespace Apollo
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct DnValue
    {
        public long errCode;
        [MarshalAs(UnmanagedType.LPStr)]
        public string errString;
        [MarshalAs(UnmanagedType.LPStr)]
        public string domainName;
        [MarshalAs(UnmanagedType.LPStr)]
        public List<string> IPList;
    }
}

