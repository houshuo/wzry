namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.GameLogic;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct KillInfo
    {
        public string KillerImgSrc;
        public string VictimImgSrc;
        public KillDetailInfoType MsgType;
        public bool bSrcAllies;
        public bool bPlayerSelf_KillOrKilled;
        public ActorTypeDef actorType;
        public byte bPriority;
        public KillInfo(string inKillerImg, string inVictimImg, KillDetailInfoType type, bool bSrcAllies, bool bPlayerSelf_KillOrKilled, ActorTypeDef actorType)
        {
            this.KillerImgSrc = inKillerImg;
            this.VictimImgSrc = inVictimImg;
            this.MsgType = type;
            this.bSrcAllies = bSrcAllies;
            this.bPlayerSelf_KillOrKilled = bPlayerSelf_KillOrKilled;
            this.actorType = actorType;
            KillNotify.knPriority.TryGetValue(type, out this.bPriority);
        }
    }
}

