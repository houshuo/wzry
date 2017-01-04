namespace Apollo
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SubTdirTreeNode
    {
        public int nodeID;
        public int parentID;
        public int flag;
        [MarshalAs(UnmanagedType.LPStr)]
        public string name;
        public int status;
        public int nodeType;
        public int svrFlag;
        public TdirStaticInfo staticInfo;
        public TdirDynamicInfo dynamicInfo;
    }
}

