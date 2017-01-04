namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Newbie")]
    public class TriggerNewbieGuideTick : TickEvent
    {
        public bool bCurrentGuideOver;
        public bool bWeakGuide;
        public int NewbieTriggerType;

        public override BaseEvent Clone()
        {
            TriggerNewbieGuideTick tick = ClassObjPool<TriggerNewbieGuideTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            TriggerNewbieGuideTick tick = src as TriggerNewbieGuideTick;
            this.bWeakGuide = tick.bWeakGuide;
            this.bCurrentGuideOver = tick.bCurrentGuideOver;
            this.NewbieTriggerType = tick.NewbieTriggerType;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            base.Process(_action, _track);
            if (this.NewbieTriggerType > 0)
            {
                if (this.bCurrentGuideOver)
                {
                    if (this.bWeakGuide)
                    {
                        MonoSingleton<NewbieGuideManager>.GetInstance().ForceCompleteWeakGuide();
                    }
                    else
                    {
                        MonoSingleton<NewbieGuideManager>.GetInstance().ForceCompleteNewbieGuide();
                    }
                }
                else if (this.bWeakGuide)
                {
                    MonoSingleton<NewbieGuideManager>.GetInstance().CheckWeakGuideTrigger((NewbieGuideWeakGuideType) this.NewbieTriggerType, new uint[0]);
                }
                else
                {
                    MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime((NewbieGuideTriggerTimeType) this.NewbieTriggerType, new uint[0]);
                }
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

