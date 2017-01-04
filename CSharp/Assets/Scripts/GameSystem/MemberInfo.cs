namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;

    public class MemberInfo
    {
        public COM_PLAYERCAMP camp;
        public uint[] canUseHero;
        public COMDT_CHOICEHERO[] ChoiceHero;
        public uint dwMemberHeadId;
        public uint dwMemberLevel;
        public uint dwMemberPvpLevel;
        public uint dwObjId;
        public uint dwPosOfCamp;
        public int iFromGameEntity;
        public int iLogicWorldID;
        public bool isGM;
        public bool isPrepare;
        public string MemberHeadUrl;
        public string MemberName;
        public uint RoomMemberType;
        public uint swapSeq;
        public byte swapStatus;
        public ulong swapUid;
        public ulong ullUid;
        public COMDT_FAKEACNT_DETAIL WarmNpc;
    }
}

