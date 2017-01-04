namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterZhaoHuanWu_node395 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            bool flag = false;
            pAgent.SetVariable<bool>("p_forceToApproachHero", flag, 0x88b8dd9a);
            return status;
        }
    }
}

