namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;

    public class TriggerActionBattleEquipLimit : TriggerActionBase
    {
        public TriggerActionBattleEquipLimit(TriggerActionWrapper inWrapper) : base(inWrapper)
        {
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger, object prm)
        {
            if ((src != 0) && (src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
            {
                src.handle.EquipComponent.m_isInEquipBoughtArea = true;
            }
            return null;
        }

        public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            if ((src != 0) && (src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
            {
                src.handle.EquipComponent.m_isInEquipBoughtArea = false;
                src.handle.EquipComponent.m_hasLeftEquipBoughtArea = true;
            }
        }
    }
}

