namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroSimpleAI_node962 : Assignment
    {
        private int opr_p0 = 0x1b58;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int ourCampActorsCount = ((ObjAgent) pAgent).GetOurCampActorsCount(this.opr_p0);
            pAgent.SetVariable<int>("p_friendCount", ourCampActorsCount, 0xb38a2a58);
            return status;
        }
    }
}

