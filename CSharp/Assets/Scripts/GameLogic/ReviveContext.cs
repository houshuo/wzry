namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ReviveContext
    {
        public bool bEnable;
        public int ReviveTime;
        public int ReviveLife;
        public int ReviveEnergy;
        public bool AutoReset;
        public bool bBaseRevive;
        public bool bCDReset;
        public void Reset()
        {
            this.ReviveTime = 0;
            this.ReviveLife = 0x2710;
            this.ReviveEnergy = 0x2710;
            this.AutoReset = false;
            this.bBaseRevive = true;
            this.bCDReset = false;
            this.bEnable = false;
        }
    }
}

