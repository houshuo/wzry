namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;
    using System.Runtime.CompilerServices;

    public class SkillBaseSelectTargetAttribute : Attribute
    {
        public SkillBaseSelectTargetAttribute(SkillTargetRule _rule)
        {
            this.TargetRule = _rule;
        }

        public SkillTargetRule TargetRule { get; set; }
    }
}

