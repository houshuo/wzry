namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct stInBattleLevelBits
    {
        public byte bReportFinished;
        public List<uint> finishedDetail;
    }
}

