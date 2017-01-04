namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node86 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int searchRange = ((ObjAgent) pAgent).GetSearchRange();
            pAgent.SetVariable<int>("p_srchRange", searchRange, 0x921d0d6a);
            return status;
        }
    }
}

