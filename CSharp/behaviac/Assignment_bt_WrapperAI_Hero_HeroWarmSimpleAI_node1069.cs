namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroWarmSimpleAI_node1069 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int num = 5;
            pAgent.SetVariable<int>("p_useSkillWeightActually", num, 0xf25eb1eb);
            return status;
        }
    }
}

