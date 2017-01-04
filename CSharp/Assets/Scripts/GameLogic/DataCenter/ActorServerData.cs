namespace Assets.Scripts.GameLogic.DataCenter
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct ActorServerData
    {
        public ActorMeta TheActorMeta;
        public uint Level;
        public uint Star;
        public uint Exp;
        public uint SkinId;
        public uint[] SymbolID;
        public ushort[] m_customRecommendEquips;
        public QualityInfo TheQualityInfo;
        public ProficiencyInfo TheProficiencyInfo;
        public BurnInfo TheBurnInfo;
        public ExtraInfo TheExtraInfo;
        [StructLayout(LayoutKind.Sequential)]
        public struct BurnInfo
        {
            public uint HeroRemainingHp;
            public bool IsDead;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ExtraInfo
        {
            public int BornPointIndex;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ProficiencyInfo
        {
            public uint Proficiency;
            public uint Level;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct QualityInfo
        {
            public uint Quality;
            public uint SubQuality;
        }
    }
}

