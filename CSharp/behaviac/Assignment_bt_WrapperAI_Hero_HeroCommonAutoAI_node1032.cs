namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node1032 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int num = 0x5b;
            pAgent.SetVariable<int>("p_idleShowLast", num, 0x5710b0cf);
            return status;
        }
    }
}

