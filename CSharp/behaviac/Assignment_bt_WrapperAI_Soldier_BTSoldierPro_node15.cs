namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using UnityEngine;

    internal class Assignment_bt_WrapperAI_Soldier_BTSoldierPro_node15 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            Vector3 routeCurWaypointPos = ((ObjAgent) pAgent).GetRouteCurWaypointPos();
            pAgent.SetVariable<Vector3>("p_attackPathCurTargetPos", routeCurWaypointPos, 0x12a69858);
            return status;
        }
    }
}

