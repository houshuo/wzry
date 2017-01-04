namespace Assets.Scripts.GameLogic.DataCenter
{
    using Assets.Scripts.GameLogic;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct ActorMeta
    {
        public int ConfigId;
        public uint PlayerId;
        public ActorTypeDef ActorType;
        public COM_PLAYERCAMP ActorCamp;
        public CrypticInt32 EnCId;
        public uint SkinID;
        public byte Difficuty;
        public bool Invincible;
        public bool NotMovable;
    }
}

