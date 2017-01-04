namespace AGE
{
    using System;

    public abstract class TickEvent : BaseEvent
    {
        protected TickEvent()
        {
        }

        public virtual void PostProcess(AGE.Action _action, Track _track, int _localTime)
        {
        }

        public virtual void Process(AGE.Action _action, Track _track)
        {
        }

        public virtual void ProcessBlend(AGE.Action _action, Track _track, TickEvent _prevEvent, float _blendWeight)
        {
        }
    }
}

