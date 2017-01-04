namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;

    [PassiveEvent(PassiveEventType.EffectPassiveEvent)]
    public class PassiveEffectEvent : PassiveEvent
    {
        private bool bTriggerFlag;

        public override void Init(PoolObjHandle<ActorRoot> _actor, PassiveSkill _skill)
        {
            this.bTriggerFlag = false;
            base.Init(_actor, _skill);
        }

        private void RemoveSkillEffect()
        {
            PoolObjHandle<ActorRoot> triggerActor;
            if (base.triggerActor != 0)
            {
                triggerActor = base.triggerActor;
            }
            else
            {
                triggerActor = base.sourceActor;
            }
            if (triggerActor != 0)
            {
                if (base.localParams[0] != 0)
                {
                    triggerActor.handle.BuffHolderComp.RemoveBuff(base.localParams[0]);
                }
                else if (base.localParams[1] != 0)
                {
                    triggerActor.handle.BuffHolderComp.RemoveBuff(base.localParams[1]);
                }
            }
        }

        public override void UpdateLogic(int _delta)
        {
            base.UpdateLogic(_delta);
            if (base.Fit() && !this.sourceActor.handle.ActorControl.IsDeadState)
            {
                base.Trigger();
                this.bTriggerFlag = true;
            }
            else if (this.bTriggerFlag || this.sourceActor.handle.ActorControl.IsDeadState)
            {
                this.bTriggerFlag = false;
                this.RemoveSkillEffect();
            }
        }
    }
}

