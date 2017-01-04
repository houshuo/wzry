namespace Assets.Scripts.GameSystem
{
    using System;

    public class RankpointRankInfo
    {
        public uint guildHeadId;
        public ulong guildId;
        public byte guildLevel;
        public string guildName;
        public byte memberNum;
        public uint rankNo;
        public uint rankScore;
        public uint star;

        public RankpointRankInfo()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.guildId = 0L;
            this.rankNo = 0;
            this.rankScore = 0;
            this.guildHeadId = 0;
            this.guildName = null;
            this.guildLevel = 0;
            this.memberNum = 0;
            this.star = 0;
        }
    }
}

