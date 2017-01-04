namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    public static class SelectTargetHelper
    {
        public static ActorRoot GetTarget(SkillSlot UseSlot)
        {
            int srchR = 0;
            bool flag = Singleton<SkillSelectControl>.GetInstance().IsLowerHpMode();
            if (UseSlot.SkillObj.AppointType == SkillRangeAppointType.Target)
            {
                srchR = UseSlot.SkillObj.cfgData.iMaxSearchDistance;
            }
            else
            {
                srchR = (int) UseSlot.SkillObj.cfgData.iMaxAttackDistance;
            }
            if (flag)
            {
                return Singleton<TargetSearcher>.GetInstance().GetLowestHpTarget(UseSlot.Actor.handle, srchR, TargetPriority.TargetPriority_Hero, UseSlot.SkillObj.cfgData.dwSkillTargetFilter, true, true);
            }
            return Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(UseSlot.Actor.handle, srchR, TargetPriority.TargetPriority_Hero, UseSlot.SkillObj.cfgData.dwSkillTargetFilter, true);
        }
    }
}

