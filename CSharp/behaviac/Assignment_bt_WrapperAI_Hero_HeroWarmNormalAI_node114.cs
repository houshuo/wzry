namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    internal class Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node114 : Assignment
    {
        private int opr_p0 = 0x1388;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            Vector3 nearestShenfuInRange = ((ObjAgent) pAgent).GetNearestShenfuInRange(this.opr_p0);
            pAgent.SetVariable<Vector3>("p_attackPathCurTargetPos", nearestShenfuInRange, 0x12a69858);
            return status;
        }
    }
}

