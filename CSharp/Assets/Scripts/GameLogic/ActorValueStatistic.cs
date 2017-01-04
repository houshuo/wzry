namespace Assets.Scripts.GameLogic
{
    using System;

    public class ActorValueStatistic
    {
        public int iActorATT;
        public int iActorINT;
        public int iActorLvl;
        public int iActorMaxHp;
        public int iActorMinHp = -1;
        public int iCritStrikeEff;
        public int iCritStrikeRate;
        public int iCritStrikeValue;
        public int iDEFStrike;
        public int iFinalHurt;
        public int iHurtOutputRate;
        public int iMagicHemophagia;
        public int iMagicHemophagiaRate;
        public int iMoveSpeedMax;
        public int iPhysicsHemophagia;
        public int iPhysicsHemophagiaRate;
        public int iReduceCritStrikeRate;
        public int iReduceCritStrikeValue;
        public int iRESStrike;
        public int iSoulExpMax;
        public uint uiAddSoulExpIntervalMax;
        public uint uiTeamSoulExpTotal;
        public ulong ulLastAddSoulExpTime = Singleton<FrameSynchr>.instance.LogicFrameTick;
    }
}

