namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Newbie")]
    internal class NewbieDragonBuffGuideTick : TickEvent
    {
        public override BaseEvent Clone()
        {
            NewbieDragonBuffGuideTick tick = ClassObjPool<NewbieDragonBuffGuideTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.newbiePvPDragonBuff, new uint[0]);
        }
    }
}

