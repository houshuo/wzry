namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class GuildInfo
    {
        public byte bBuildingCount;
        public uint dwActive;
        public uint dwCoinPool;
        public uint dwGuildMoney;
        public uint groupGuildId;
        public string groupKey;
        public string groupOpenId;
        public List<GuildBuildingInfo> listBuildingInfo = new List<GuildBuildingInfo>();
        [NonSerialized]
        public ListView<GuildMemInfo> listMemInfo = new ListView<GuildMemInfo>();
        public List<GuildSelfRecommendInfo> listSelfRecommendInfo = new List<GuildSelfRecommendInfo>();
        public GuildRankInfo RankInfo = new GuildRankInfo();
        public uint star;
        public stGuildBriefInfo stBriefInfo = new stGuildBriefInfo();
        public GuildMemInfo stChairman = new GuildMemInfo();
        public ulong uulCreatedTime;

        public void Reset()
        {
            this.uulCreatedTime = 0L;
            this.dwActive = 0;
            this.dwCoinPool = 0;
            this.dwGuildMoney = 0;
            this.stChairman.Reset();
            this.stBriefInfo.Reset();
            this.listMemInfo.Clear();
            this.listBuildingInfo.Clear();
            this.listSelfRecommendInfo.Clear();
            this.RankInfo.Reset();
            this.star = 0;
            this.groupKey = null;
            this.groupOpenId = null;
        }
    }
}

