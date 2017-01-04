namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Hero_HeroSimpleAI_node260 : behaviac.Action
    {
        private bool method_p0 = false;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            ((ObjAgent) pAgent).SetIsAttackByEnemyHero(this.method_p0);
            return EBTStatus.BT_FAILURE;
        }
    }
}

