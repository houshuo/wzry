namespace behaviac
{
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroCommonAutoAI_node1031 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int variable = (int) pAgent.GetVariable((uint) 0x5710b0cf);
            int num2 = 0;
            return ((variable != num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

