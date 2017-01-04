namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stMatchOpenInfo
    {
        public enMatchOpenState matchState;
        public string descStr;
        public uint leftSec;
        public int leftDay;
    }
}

