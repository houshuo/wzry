namespace com.tencent.pandora
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct MethodArgs
    {
        public int index;
        public ExtractValue extractValue;
        public bool isParamsArray;
        public Type paramsArrayType;
    }
}

