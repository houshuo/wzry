namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;

    public class KillDetailInfo
    {
        public bool bAllDead;
        public bool bPlayerSelf_KillOrKilled;
        public bool bSelfCamp;
        public KillDetailInfoType HeroContiKillType;
        public KillDetailInfoType HeroMultiKillType;
        public PoolObjHandle<ActorRoot> Killer;
        public KillDetailInfoType Type;
        public PoolObjHandle<ActorRoot> Victim;
    }
}

