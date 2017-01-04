namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct DestroyStat
    {
        public int CampEnemyNum;
        public int CampSelfNum;
    }
}

