namespace Assets.Scripts.GameSystem
{
    using System;

    public class GuildRankInfo
    {
        public uint seasonStartTime;
        public uint totalRankPoint;
        public uint weekRankPoint;

        public GuildRankInfo()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.totalRankPoint = 0;
            this.seasonStartTime = 0;
            this.weekRankPoint = 0;
        }
    }
}

