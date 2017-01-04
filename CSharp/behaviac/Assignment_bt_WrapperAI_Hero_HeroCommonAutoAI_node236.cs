namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node236 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint nearestMemberNotCaptain = ((ObjAgent) pAgent).GetNearestMemberNotCaptain();
            pAgent.SetVariable<uint>("p_nearstMember", nearestMemberNotCaptain, 0xa01cd192);
            return status;
        }
    }
}

