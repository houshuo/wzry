namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Hero_HeroWarmSimpleAI_node98 : behaviac.Action
    {
        private int method_p1 = 0x1b58;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int variable = (int) pAgent.GetVariable((uint) 0x921d0d6a);
            return ((ObjAgent) pAgent).IsAroundTeamThanStrongThanEnemise(variable, this.method_p1);
        }
    }
}

