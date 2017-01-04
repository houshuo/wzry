namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Newbie")]
    internal class PvPTalentGuideTick : TickEvent
    {
        public int GuideType;

        public override BaseEvent Clone()
        {
            PvPTalentGuideTick tick = ClassObjPool<PvPTalentGuideTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            PvPTalentGuideTick tick = src as PvPTalentGuideTick;
            this.GuideType = tick.GuideType;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            if (this.GuideType == 1)
            {
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.newbieTalent, new uint[0]);
            }
            else if (this.GuideType == 2)
            {
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.newbiePvPTalent, new uint[0]);
            }
            else if (this.GuideType == 3)
            {
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.newbieTalentSecondTime, new uint[0]);
            }
            else
            {
                object[] inParameters = new object[] { this.GuideType };
                DebugHelper.Assert(false, "Invalid Talent GuideType -- {0}", inParameters);
            }
        }
    }
}

