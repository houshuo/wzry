namespace Assets.Scripts.GameLogic.DataCenter
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct ActorServerRuneData
    {
        public ActorMeta TheActorMeta;
        public ActorRunelSlot RuneSlot;
        public uint RuneId;
    }
}

