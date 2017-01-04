namespace behaviac
{
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroSimpleAI_node970 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int variable = (int) pAgent.GetVariable((uint) 0x61fc90a6);
            int num2 = 0x1388;
            return ((variable <= num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

