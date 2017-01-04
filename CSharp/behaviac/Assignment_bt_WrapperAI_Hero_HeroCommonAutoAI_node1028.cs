namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node1028 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint num = 0;
            pAgent.SetVariable<uint>("p_idleShowFrame", num, 0x7206a9b3);
            return status;
        }
    }
}

