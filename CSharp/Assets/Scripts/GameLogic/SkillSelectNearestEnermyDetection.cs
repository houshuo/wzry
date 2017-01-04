namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    [SkillBaseDetection(SkillUseRule.EnermyUse)]
    public class SkillSelectNearestEnermyDetection : SkillBaseDetection
    {
        public override bool Detection(SkillSlot slot)
        {
            int srchR = 0;
            if (slot.SkillObj.AppointType == SkillRangeAppointType.Target)
            {
                srchR = slot.SkillObj.cfgData.iMaxSearchDistance;
            }
            else
            {
                srchR = (int) slot.SkillObj.cfgData.iMaxAttackDistance;
            }
            return (Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(slot.Actor.handle, srchR, slot.SkillObj.cfgData.dwSkillTargetFilter, true) != null);
        }
    }
}

