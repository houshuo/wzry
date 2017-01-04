namespace Assets.Scripts.GameLogic
{
    using ResData;

    [SkillBaseSelectTarget(SkillTargetRule.NearestEnermy)]
    public class SkillSelectNearestTarget : SkillBaseSelectTarget
    {
        public override ActorRoot SelectTarget(SkillSlot UseSlot)
        {
            return SelectTargetHelper.GetTarget(UseSlot);
        }

        public override VInt3 SelectTargetDir(SkillSlot UseSlot)
        {
            ActorRoot target = SelectTargetHelper.GetTarget(UseSlot);
            if (target != null)
            {
                VInt3 num = target.location - UseSlot.Actor.handle.location;
                num.y = 0;
                return num.NormalizeTo(0x3e8);
            }
            return UseSlot.Actor.handle.forward;
        }
    }
}

