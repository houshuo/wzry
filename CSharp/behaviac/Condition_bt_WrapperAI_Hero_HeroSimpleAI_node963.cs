namespace behaviac
{
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroSimpleAI_node963 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int variable = (int) pAgent.GetVariable((uint) 0xb38a2a58);
            int num2 = 2;
            return ((variable > num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

