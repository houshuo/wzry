namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterRob_node38 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int num = 0;
            pAgent.SetVariable<int>("p_attackOneTime", num, 0x1fd679ba);
            return status;
        }
    }
}

