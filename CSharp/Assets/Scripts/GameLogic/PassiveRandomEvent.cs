namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using ResData;
    using System;

    [PassiveEvent(PassiveEventType.RandomPassiveEvent)]
    public class PassiveRandomEvent : PassiveEvent
    {
        private int randomRate;

        public override void Init(PoolObjHandle<ActorRoot> _actor, PassiveSkill _skill)
        {
            base.Init(_actor, _skill);
            this.randomRate = base.localParams[0];
        }

        public override void UpdateLogic(int _delta)
        {
            base.UpdateLogic(_delta);
            if (base.Fit())
            {
                if (FrameRandom.Random(0x2710) < this.randomRate)
                {
                    base.Trigger();
                }
                base.Reset();
            }
            else
            {
                base.Reset();
            }
        }
    }
}

