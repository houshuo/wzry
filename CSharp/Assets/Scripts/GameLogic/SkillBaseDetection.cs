namespace Assets.Scripts.GameLogic
{
    using System;

    public abstract class SkillBaseDetection
    {
        protected SkillBaseDetection()
        {
        }

        public virtual bool Detection(SkillSlot slot)
        {
            return true;
        }
    }
}

