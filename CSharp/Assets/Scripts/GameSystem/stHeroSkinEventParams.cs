namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stHeroSkinEventParams
    {
        public uint heroId;
        public uint skinId;
        public bool isCanCharge;
    }
}

