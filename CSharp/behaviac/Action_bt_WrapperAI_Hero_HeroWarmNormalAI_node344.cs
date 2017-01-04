namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Hero_HeroWarmNormalAI_node344 : behaviac.Action
    {
        private int method_p0 = 0x1f40;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            return ((ObjAgent) pAgent).HasEnemyBuildingAndEnemyBuildingWillAttackSelf(this.method_p0);
        }
    }
}

