namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node610 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint leader = ((ObjAgent) pAgent).GetLeader();
            pAgent.SetVariable<uint>("p_captainID", leader, 0x7e66728f);
            return status;
        }
    }
}

