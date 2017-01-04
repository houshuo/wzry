namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Action_bt_WrapperAI_Monster_BTMonsterRob_node233 : behaviac.Action
    {
        private int method_p0 = 0x1388;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            ((ObjAgent) pAgent).NotifySelfCampSelfBeAttacked(this.method_p0);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

