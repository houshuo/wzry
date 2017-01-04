namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node222 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint captain = ((ObjAgent) pAgent).GetCaptain();
            pAgent.SetVariable<uint>("p_captainID", captain, 0x7e66728f);
            return status;
        }
    }
}

