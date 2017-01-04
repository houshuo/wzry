namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct BuffLimiteHurt
    {
        public bool bValid;
        public int hurtRate;
    }
}

