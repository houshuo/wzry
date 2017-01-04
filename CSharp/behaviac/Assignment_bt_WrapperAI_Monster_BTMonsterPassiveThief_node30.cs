namespace behaviac
{
    using System;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterPassiveThief_node30 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint num = 0;
            pAgent.SetVariable<uint>("p_targetID", num, 0x4349179f);
            return status;
        }
    }
}

