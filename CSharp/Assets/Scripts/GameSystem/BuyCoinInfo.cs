namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct BuyCoinInfo
    {
        public int m_CostDiamond;
        public int m_GainCoin;
        public int m_CritTime;
    }
}

