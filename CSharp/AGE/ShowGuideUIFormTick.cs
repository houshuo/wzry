namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Newbie")]
    internal class ShowGuideUIFormTick : TickEvent
    {
        public int delayShowInteractable;
        public CBattleGuideManager.EBattleGuideFormType FormType;

        public override BaseEvent Clone()
        {
            ShowGuideUIFormTick tick = ClassObjPool<ShowGuideUIFormTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            ShowGuideUIFormTick tick = src as ShowGuideUIFormTick;
            this.FormType = tick.FormType;
            this.delayShowInteractable = tick.delayShowInteractable;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            base.Process(_action, _track);
            if (this.FormType > CBattleGuideManager.EBattleGuideFormType.Invalid)
            {
                Singleton<CBattleGuideManager>.GetInstance().OpenFormShared(this.FormType, this.delayShowInteractable, true);
            }
        }
    }
}

