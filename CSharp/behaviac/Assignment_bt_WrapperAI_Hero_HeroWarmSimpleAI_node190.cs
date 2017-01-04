namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroWarmSimpleAI_node190 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint num = 0;
            pAgent.SetVariable<uint>("p_abandonTargetID", num, 0xb81a7cc);
            return status;
        }
    }
}

