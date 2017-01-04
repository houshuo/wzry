namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node491 : Assignment
    {
        private uint opr_p0 = 300;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int randomInt = ((BTBaseAgent) pAgent).GetRandomInt(this.opr_p0);
            pAgent.SetVariable<int>("p_waitRandomFrames", randomInt, 0x56cb5020);
            return status;
        }
    }
}

