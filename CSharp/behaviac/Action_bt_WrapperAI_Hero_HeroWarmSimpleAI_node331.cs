namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Hero_HeroWarmSimpleAI_node331 : behaviac.Action
    {
        private int method_p0 = 0x251c;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            return ((ObjAgent) pAgent).HasEnemyBuildingAndEnemyBuildingWillAttackSelf(this.method_p0);
        }
    }
}

