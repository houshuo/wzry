namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("ActionControl")]
    public class Unstoppable : DurationEvent
    {
        public override BaseEvent Clone()
        {
            Unstoppable unstoppable = ClassObjPool<Unstoppable>.Get();
            unstoppable.CopyData(this);
            return unstoppable;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            _action.unstoppable = true;
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            _action.unstoppable = false;
        }

        public override void OnUse()
        {
            base.OnUse();
        }
    }
}

