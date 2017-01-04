namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using UnityEngine;

    internal class Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node221 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            Vector3 routeCurWaypointPosPre = ((ObjAgent) pAgent).GetRouteCurWaypointPosPre();
            pAgent.SetVariable<Vector3>("p_attackPathCurTargetPos", routeCurWaypointPosPre, 0x12a69858);
            return status;
        }
    }
}

