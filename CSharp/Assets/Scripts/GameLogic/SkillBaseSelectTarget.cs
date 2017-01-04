namespace Assets.Scripts.GameLogic
{
    using System;

    public abstract class SkillBaseSelectTarget
    {
        protected SkillBaseSelectTarget()
        {
        }

        public virtual ActorRoot SelectTarget(SkillSlot UseSlot)
        {
            return null;
        }

        public virtual VInt3 SelectTargetDir(SkillSlot UseSlot)
        {
            return UseSlot.Actor.handle.forward;
        }
    }
}

