namespace behaviac
{
    using System;

    internal class Compute_bt_WrapperAI_Monster_BTMonsterInitiative_node39 : Compute
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0x1fd679ba);
            int num2 = 1;
            int num3 = variable + num2;
            pAgent.SetVariable<int>("p_attackOneTime", num3, 0x1fd679ba);
            return status;
        }
    }
}

