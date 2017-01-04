namespace Assets.Scripts.GameSystem
{
    using System;

    public class GuildSelfRecommendInfo
    {
        public uint time;
        public ulong uid;

        public GuildSelfRecommendInfo()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.uid = 0L;
            this.time = 0;
        }
    }
}

