namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Condition_bt_WrapperAI_Hero_HeroGuideFollowNew_node125 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            int baronDeadTimes = ((ObjAgent) pAgent).GetBaronDeadTimes();
            int num2 = 0;
            return ((baronDeadTimes <= num2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

