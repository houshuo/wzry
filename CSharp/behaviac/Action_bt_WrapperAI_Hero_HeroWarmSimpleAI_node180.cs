namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Hero_HeroWarmSimpleAI_node180 : behaviac.Action
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int variable = (int) pAgent.GetVariable((uint) 0x2cb8695e);
            return ((ObjAgent) pAgent).HasEnemyInRange(variable);
        }
    }
}

