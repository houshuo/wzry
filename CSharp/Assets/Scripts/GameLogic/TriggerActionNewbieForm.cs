namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;

    public class TriggerActionNewbieForm : TriggerActionBase
    {
        public TriggerActionNewbieForm(TriggerActionWrapper inWrapper) : base(inWrapper)
        {
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger, object prm)
        {
            if ((base.EnterUniqueId > 0) && (base.EnterUniqueId < 0x1f))
            {
                Singleton<CBattleGuideManager>.GetInstance().OpenFormShared((CBattleGuideManager.EBattleGuideFormType) base.EnterUniqueId, 0, true);
            }
            return null;
        }
    }
}

