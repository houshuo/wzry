namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using System;

    public class TriggerActionShowToggleAuto : TriggerActionBase
    {
        public TriggerActionShowToggleAuto(TriggerActionWrapper inWrapper) : base(inWrapper)
        {
        }

        private void EnableToggleAuto(bool bInEnable)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
            if (form != null)
            {
                form.transform.FindChild("PanelBtn/ToggleAutoBtn").gameObject.CustomSetActive(bInEnable);
            }
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger, object prm)
        {
            this.EnableToggleAuto(base.bEnable);
            return null;
        }

        public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            if (base.bStopWhenLeaving)
            {
                this.EnableToggleAuto(!base.bEnable);
            }
        }
    }
}

