namespace Assets.Scripts.GameLogic
{
    using CSProtocol;
    using System;

    public class NONHERO_STATISTIC_INFO
    {
        public COM_PLAYERCAMP ActorCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
        public ActorTypeDef ActorType = ActorTypeDef.Actor_Type_Monster;
        public uint uiAttackDistanceMax = 0;
        public uint uiAttackDistanceMin = uint.MaxValue;
        public uint uiBeHurtMax = 0;
        public uint uiBeHurtMin = uint.MaxValue;
        public uint uiFirstBeAttackTime = 0;
        public uint uiHpMax = 0;
        public uint uiHpMin = uint.MaxValue;
        public uint uiHurtMax = 0;
        public uint uiHurtMin = uint.MaxValue;
        public uint uiTotalAttackNum = 0;
        public uint uiTotalBeAttackedNum = 0;
        public uint uiTotalBeHurtCount = 0;
        public uint uiTotalDeadNum = 0;
        public uint uiTotalHurtCount = 0;
        public uint uiTotalSpawnNum = 0;
    }
}

