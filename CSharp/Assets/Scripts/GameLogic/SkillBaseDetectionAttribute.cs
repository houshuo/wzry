namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;
    using System.Runtime.CompilerServices;

    public class SkillBaseDetectionAttribute : Attribute
    {
        public SkillBaseDetectionAttribute(SkillUseRule _rule)
        {
            this.UseRule = _rule;
        }

        public SkillUseRule UseRule { get; set; }
    }
}

