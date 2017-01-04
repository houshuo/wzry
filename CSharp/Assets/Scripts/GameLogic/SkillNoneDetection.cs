namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    [SkillBaseDetection(SkillUseRule.AnyUse)]
    public class SkillNoneDetection : SkillBaseDetection
    {
        public override bool Detection(SkillSlot slot)
        {
            return true;
        }
    }
}

