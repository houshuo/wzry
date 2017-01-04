namespace Assets.Scripts.GameSystem
{
    using ResData;
    using System;

    [Serializable]
    public class GuildBuildingInfo
    {
        public byte level;
        public RES_GUILD_BUILDING_TYPE type;

        public GuildBuildingInfo()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.type = (RES_GUILD_BUILDING_TYPE) 0;
            this.level = 0;
        }
    }
}

