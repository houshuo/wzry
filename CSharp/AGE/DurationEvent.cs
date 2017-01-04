namespace AGE
{
    using System;

    public abstract class DurationEvent : BaseEvent
    {
        public int length;

        protected DurationEvent()
        {
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            DurationEvent event2 = src as DurationEvent;
            this.length = event2.length;
        }

        public virtual void Enter(AGE.Action _action, Track _track)
        {
        }

        public virtual void EnterBlend(AGE.Action _action, Track _track, BaseEvent _prevEvent, int _blendTime)
        {
            this.Enter(_action, _track);
        }

        public virtual void Leave(AGE.Action _action, Track _track)
        {
        }

        public virtual void LeaveBlend(AGE.Action _action, Track _track, BaseEvent _nextEvent, int _blendTime)
        {
            this.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.length = 0;
        }

        public virtual void Process(AGE.Action _action, Track _track, int _localTime)
        {
        }

        public virtual void ProcessBlend(AGE.Action _action, Track _track, int _localTime, DurationEvent _prevEvent, int _prevLocalTime, float _blendWeight)
        {
            if (_prevEvent != null)
            {
                _prevEvent.Process(_action, _track, _prevLocalTime);
            }
            this.Process(_action, _track, _localTime);
        }

        public virtual bool bScaleLength
        {
            get
            {
                return true;
            }
        }

        public int End
        {
            get
            {
                return (base.time + this.length);
            }
            set
            {
                if (value > base.time)
                {
                    this.length = value - base.time;
                }
                else
                {
                    this.length = 0;
                    base.time = value;
                }
            }
        }

        public float lengthSec
        {
            get
            {
                return ActionUtility.MsToSec(this.length);
            }
        }

        public int Start
        {
            get
            {
                return base.time;
            }
            set
            {
                int end = this.End;
                base.time = value;
                if (value < end)
                {
                    this.length = end - base.time;
                }
                else
                {
                    this.length = 0;
                }
            }
        }
    }
}

