namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct QueryValue
    {
        public long errCode;
        [MarshalAs(UnmanagedType.LPStr)]
        public string errString;
        public ListView<DnValue> value;
    }
}

