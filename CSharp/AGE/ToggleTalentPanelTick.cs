namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using System;

    [EventCategory("MMGame/Newbie")]
    internal class ToggleTalentPanelTick : TickEvent
    {
        public bool bShow;

        public override BaseEvent Clone()
        {
            ToggleTalentPanelTick tick = ClassObjPool<ToggleTalentPanelTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            ToggleTalentPanelTick tick = src as ToggleTalentPanelTick;
            this.bShow = tick.bShow;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
            if (form != null)
            {
                Utility.FindChild(form.gameObject, "PanelTalent").CustomSetActive(this.bShow);
            }
        }
    }
}

