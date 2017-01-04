namespace Assets.Scripts.GameLogic.DataCenter
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct ActorPerStarLvData
    {
        public ActorMeta TheActorMeta;
        public ActorStarLv StarLv;
        public int PerLvHp;
        public int PerLvAd;
        public int PerLvAp;
        public int PerLvDef;
        public int PerLvRes;
    }
}

