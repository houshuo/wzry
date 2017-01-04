namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node995 : Assignment
    {
        private int opr_p1 = 0xbb8;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            Vector3 variable = (Vector3) pAgent.GetVariable((uint) 0x12a69858);
            Vector3 randomPointAroundGivenPoint = ((ObjAgent) pAgent).GetRandomPointAroundGivenPoint(variable, this.opr_p1);
            pAgent.SetVariable<Vector3>("p_attackPathCurTargetPos", randomPointAroundGivenPoint, 0x12a69858);
            return status;
        }
    }
}

