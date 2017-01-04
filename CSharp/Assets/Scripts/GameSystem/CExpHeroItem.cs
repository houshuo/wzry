namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Runtime.InteropServices;

    public class CExpHeroItem : CHeroItem
    {
        public uint expDays;

        public CExpHeroItem(ulong objID, uint baseID, uint expDays, int stackCount = 0, int addTime = 0) : base(objID, baseID, stackCount, addTime)
        {
            this.expDays = expDays;
        }
    }
}

