namespace Assets.Scripts.GameSystem
{
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stItemGetInfoParams
    {
        public byte getType;
        public ResLevelCfgInfo levelInfo;
        public bool isCanDo;
        public string errorStr;
    }
}

