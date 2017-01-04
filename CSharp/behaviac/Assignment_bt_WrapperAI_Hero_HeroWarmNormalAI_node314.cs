namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node314 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int attackRange = ((ObjAgent) pAgent).GetAttackRange();
            pAgent.SetVariable<int>("p_attackRange", attackRange, 0x61fc90a6);
            return status;
        }
    }
}

