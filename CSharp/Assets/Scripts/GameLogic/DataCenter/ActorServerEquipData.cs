namespace Assets.Scripts.GameLogic.DataCenter
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct ActorServerEquipData
    {
        public ActorMeta TheActorMeta;
        public ActorEquiplSlot EquipSlot;
        public uint EquipCfgId;
        public ulong EquipUid;
        public EquipDetailInfo TheDetailInfo;
        [StructLayout(LayoutKind.Sequential)]
        public struct EquipDetailInfo
        {
            public int StackCount;
        }
    }
}

