namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Compute_bt_WrapperAI_FireTrapRand_node0 : Compute
    {
        private uint opr2_p0 = 20;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int num = 5;
            int randomInt = ((BTBaseAgent) pAgent).GetRandomInt(this.opr2_p0);
            int num3 = num + randomInt;
            pAgent.SetVariable<int>("p_waitFrame", num3, 0xd5f60189);
            return status;
        }
    }
}

