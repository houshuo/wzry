namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("Debug")]
    public class BreakPoint : TickEvent
    {
        public bool enabled = true;
        public string info = string.Empty;

        public override BaseEvent Clone()
        {
            BreakPoint point = ClassObjPool<BreakPoint>.Get();
            point.CopyData(this);
            return point;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            BreakPoint point = src as BreakPoint;
            this.enabled = point.enabled;
            this.info = point.info;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.enabled = true;
            this.info = string.Empty;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
        }
    }
}

