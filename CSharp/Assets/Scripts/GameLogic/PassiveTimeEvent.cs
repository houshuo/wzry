namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [PassiveEvent(PassiveEventType.TimePassiveEvent)]
    public class PassiveTimeEvent : PassiveEvent
    {
        private bool bActive;
        private int curTriggerCount;
        private int maxTriggerCount;
        protected int startTime;
        private int totalTime;

        public override void Init(PoolObjHandle<ActorRoot> _actor, PassiveSkill _skill)
        {
            base.Init(_actor, _skill);
            this.startTime = 0;
            this.curTriggerCount = 0;
            this.totalTime = base.localParams[0];
            this.maxTriggerCount = base.localParams[1];
            this.bActive = true;
            if (this.maxTriggerCount <= 0)
            {
                this.maxTriggerCount = 0x7fffffff;
            }
        }

        public override void UpdateLogic(int _delta)
        {
            base.UpdateLogic(_delta);
            if (this.bActive)
            {
                this.startTime += _delta;
                if (base.Fit())
                {
                    if ((this.startTime >= this.totalTime) && (this.curTriggerCount < this.maxTriggerCount))
                    {
                        base.Trigger();
                        this.startTime -= this.totalTime;
                        base.Reset();
                        this.curTriggerCount++;
                        if (this.curTriggerCount == this.maxTriggerCount)
                        {
                            this.bActive = false;
                        }
                    }
                }
                else
                {
                    this.startTime = 0;
                    base.Reset();
                }
            }
        }
    }
}

