namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterZhaoHuanWu_node364 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint master = ((ObjAgent) pAgent).GetMaster();
            pAgent.SetVariable<uint>("p_captainID", master, 0x7e66728f);
            return status;
        }
    }
}

