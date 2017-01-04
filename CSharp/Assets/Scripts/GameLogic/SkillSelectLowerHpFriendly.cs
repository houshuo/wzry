namespace Assets.Scripts.GameLogic
{
    using ResData;

    [SkillBaseSelectTarget(SkillTargetRule.LowerHpFriendly)]
    public class SkillSelectLowerHpFriendly : SkillBaseSelectTarget
    {
        public override ActorRoot SelectTarget(SkillSlot UseSlot)
        {
            return Singleton<TargetSearcher>.GetInstance().GetLowestHpTarget(UseSlot.Actor.handle, UseSlot.SkillObj.cfgData.iMaxSearchDistance, TargetPriority.TargetPriority_Hero, UseSlot.SkillObj.cfgData.dwSkillTargetFilter, false, true);
        }

        public override VInt3 SelectTargetDir(SkillSlot UseSlot)
        {
            ActorRoot root = Singleton<TargetSearcher>.GetInstance().GetLowestHpTarget(UseSlot.Actor.handle, UseSlot.SkillObj.cfgData.iMaxSearchDistance, TargetPriority.TargetPriority_Hero, UseSlot.SkillObj.cfgData.dwSkillTargetFilter, false, true);
            if (root != null)
            {
                VInt3 num = root.location - UseSlot.Actor.handle.location;
                num.y = 0;
                return num.NormalizeTo(0x3e8);
            }
            return UseSlot.Actor.handle.forward;
        }
    }
}

