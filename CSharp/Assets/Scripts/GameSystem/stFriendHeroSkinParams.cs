namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stFriendHeroSkinParams
    {
        public bool bSkin;
        public uint heroId;
        public uint skinId;
        public uint price;
    }
}

