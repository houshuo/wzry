namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Drama")]
    public class TimeScalerDuration : DurationEvent
    {
        public float TimeScale = 1f;

        public override BaseEvent Clone()
        {
            TimeScalerDuration duration = ClassObjPool<TimeScalerDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            TimeScalerDuration duration = src as TimeScalerDuration;
            this.TimeScale = duration.TimeScale;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            DebugHelper.Assert(!Singleton<FrameSynchr>.instance.bActive, "frame synchr active forbid set timeScale");
            Time.timeScale = this.TimeScale;
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            Time.timeScale = 1f;
            DebugHelper.Assert(!Singleton<FrameSynchr>.instance.bActive, "frame synchr active forbid set timeScale");
            base.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.TimeScale = 1f;
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

