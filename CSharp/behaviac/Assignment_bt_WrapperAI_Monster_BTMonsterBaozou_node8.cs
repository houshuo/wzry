namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterBaozou_node8 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0xf3ab8ea7);
            Vector3 polygonEdgePoint = ((ObjAgent) pAgent).GetPolygonEdgePoint(variable);
            pAgent.SetVariable<Vector3>("p_targetPos", polygonEdgePoint, 0x10d56de1);
            return status;
        }
    }
}

