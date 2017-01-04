namespace Assets.Scripts.GameLogic
{
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct DeadRecord
    {
        public COM_PLAYERCAMP camp;
        public ActorTypeDef actorType;
        public int cfgId;
        public int deadTime;
        public COM_PLAYERCAMP killerCamp;
        public uint AttackPlayerID;
        public byte actorSubType;
        public byte actorSubSoliderType;
        public int iOrder;
        public int fightTime;
        public DeadRecord(COM_PLAYERCAMP camp, ActorTypeDef actorType, int cfgId, int deadTime, COM_PLAYERCAMP killerCamp, uint plyaerID)
        {
            this.camp = camp;
            this.actorType = actorType;
            this.cfgId = cfgId;
            this.deadTime = deadTime;
            this.killerCamp = killerCamp;
            this.AttackPlayerID = plyaerID;
            this.actorSubType = 0;
            this.actorSubSoliderType = 0;
            this.iOrder = 0;
            this.fightTime = 0;
        }
    }
}

