namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct STriggerCondActor
    {
        public ActorTypeDef[] ActorType;
        public int[] ConfigID;
        public COM_PLAYERCAMP[] CmpType;
        public bool FilterMatch(ref PoolObjHandle<ActorRoot> inActor)
        {
            if (inActor == 0)
            {
                return false;
            }
            if (((this.ActorType != null) && (this.ActorType.Length > 0)) && !LinqS.Contains<ActorTypeDef>(this.ActorType, inActor.handle.TheActorMeta.ActorType))
            {
                return false;
            }
            if (((this.ConfigID != null) && (this.ConfigID.Length > 0)) && !LinqS.Contains<int>(this.ConfigID, inActor.handle.TheActorMeta.ConfigId))
            {
                return false;
            }
            if (((this.CmpType != null) && (this.CmpType.Length > 0)) && !LinqS.Contains<COM_PLAYERCAMP>(this.CmpType, inActor.handle.TheActorMeta.ActorCamp))
            {
                return false;
            }
            return true;
        }
    }
}

