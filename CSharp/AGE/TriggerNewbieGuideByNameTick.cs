namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Newbie")]
    public class TriggerNewbieGuideByNameTick : TickEvent
    {
        public eNewbieGuideName newbieGuideType;
        public int parm0;

        public override BaseEvent Clone()
        {
            TriggerNewbieGuideByNameTick tick = ClassObjPool<TriggerNewbieGuideByNameTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            TriggerNewbieGuideByNameTick tick = src as TriggerNewbieGuideByNameTick;
            this.newbieGuideType = tick.newbieGuideType;
            this.parm0 = tick.parm0;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            base.Process(_action, _track);
            switch (this.newbieGuideType)
            {
                case eNewbieGuideName.MapSignGuide:
                    MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onMiniMapSignGuide, new uint[0]);
                    return;

                case eNewbieGuideName.JungleBuyEquipGuide:
                    MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onJungleEquipGuide, new uint[0]);
                    return;

                case eNewbieGuideName.SelectCommonAttackType:
                {
                    uint[] param = new uint[] { this.parm0 };
                    MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onSetLockOrFreeTargetType, param);
                    return;
                }
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }

        public enum eNewbieGuideName
        {
            MapSignGuide,
            JungleBuyEquipGuide,
            SelectCommonAttackType
        }
    }
}

