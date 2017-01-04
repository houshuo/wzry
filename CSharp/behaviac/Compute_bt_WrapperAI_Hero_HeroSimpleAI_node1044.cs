namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Compute_bt_WrapperAI_Hero_HeroSimpleAI_node1044 : Compute
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int campIndex = ((ObjAgent) pAgent).GetCampIndex();
            int num2 = 4;
            int num3 = campIndex * num2;
            pAgent.SetVariable<int>("p_waitToPlayBornAge", num3, 0x13cef34);
            return status;
        }
    }
}

