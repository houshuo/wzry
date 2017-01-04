namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node377 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint variable = (uint) pAgent.GetVariable((uint) 0x3e931a5c);
            pAgent.SetVariable<uint>("p_baseID", variable, 0xeddd618f);
            return status;
        }
    }
}

