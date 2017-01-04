namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterBaozou_node4 : Assignment
    {
        private uint opr_p0 = 0x3e8;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int randomInt = ((BTBaseAgent) pAgent).GetRandomInt(this.opr_p0);
            pAgent.SetVariable<int>("p_randomIndex", randomInt, 0xf3ab8ea7);
            return status;
        }
    }
}

