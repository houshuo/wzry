namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Compute_bt_WrapperAI_Hero_HeroWarmSimpleAI_node295 : Compute
    {
        private uint opr2_p0 = 10;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int num = 2;
            int randomInt = ((BTBaseAgent) pAgent).GetRandomInt(this.opr2_p0);
            int num3 = num + randomInt;
            pAgent.SetVariable<int>("p_waitRandomFrames", num3, 0x56cb5020);
            return status;
        }
    }
}

