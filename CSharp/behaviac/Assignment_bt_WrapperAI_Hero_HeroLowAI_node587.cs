namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroLowAI_node587 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int num = 0;
            pAgent.SetVariable<int>("p_pursueFrame", num, 0xb8ff8feb);
            return status;
        }
    }
}

