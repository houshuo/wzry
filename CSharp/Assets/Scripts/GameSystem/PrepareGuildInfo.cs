namespace Assets.Scripts.GameSystem
{
    using System;

    public class PrepareGuildInfo
    {
        public ListView<GuildMemInfo> m_MemList = new ListView<GuildMemInfo>();
        public stPrepareGuildBriefInfo stBriefInfo;

        public void Reset()
        {
            this.stBriefInfo.Reset();
            this.m_MemList.Clear();
        }
    }
}

