namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stOpenHeroFormParams
    {
        public uint heroId;
        public uint skinId;
        public enHeroFormOpenSrc openSrc;
    }
}

