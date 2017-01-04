namespace Assets.Scripts.GameLogic
{
    using System;

    public class GameContextBase
    {
        protected SLevelContext LevelContext;
        protected int RewardCount;

        public virtual GameInfoBase CreateGameInfo()
        {
            return null;
        }

        public virtual void PrepareStartup()
        {
        }

        public SLevelContext levelContext
        {
            get
            {
                return this.LevelContext;
            }
        }

        public int rewardCount
        {
            get
            {
                return this.RewardCount;
            }
        }
    }
}

