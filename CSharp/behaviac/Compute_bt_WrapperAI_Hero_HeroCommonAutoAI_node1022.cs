namespace behaviac
{
    using System;

    internal class Compute_bt_WrapperAI_Hero_HeroCommonAutoAI_node1022 : Compute
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint variable = (uint) pAgent.GetVariable((uint) 0x7206a9b3);
            uint num2 = 1;
            uint num3 = variable + num2;
            pAgent.SetVariable<uint>("p_idleShowFrame", num3, 0x7206a9b3);
            return status;
        }
    }
}

