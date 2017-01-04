namespace AGE
{
    using System;

    public abstract class DurationCondition : DurationEvent
    {
        protected DurationCondition()
        {
        }

        public virtual bool Check(AGE.Action _action, Track _track)
        {
            return true;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            bool flag = this.Check(_action, _track);
            _action.SetCondition(_track, flag);
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            bool flag = this.Check(_action, _track);
            _action.SetCondition(_track, flag);
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            bool flag = this.Check(_action, _track);
            _action.SetCondition(_track, flag);
        }
    }
}

