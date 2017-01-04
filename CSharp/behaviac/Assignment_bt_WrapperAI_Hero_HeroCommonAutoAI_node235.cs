namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using UnityEngine;

    internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node235 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            Vector3 curRouteLastForward = ((ObjAgent) pAgent).GetCurRouteLastForward();
            pAgent.SetVariable<Vector3>("p_direction", curRouteLastForward, 0xbf2a6712);
            return status;
        }
    }
}

