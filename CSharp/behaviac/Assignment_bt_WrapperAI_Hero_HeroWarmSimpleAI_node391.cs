namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    internal class Assignment_bt_WrapperAI_Hero_HeroWarmSimpleAI_node391 : Assignment
    {
        private int opr_p1 = 0xfa0;
        private int opr_p2 = 0x7d0;
        private int opr_p3 = 0x1388;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            Vector3 variable = (Vector3) pAgent.GetVariable((uint) 0x12a69858);
            Vector3 vector2 = ((ObjAgent) pAgent).GetRandomPointByGivenPointAndMinRange(variable, this.opr_p1, this.opr_p2, this.opr_p3);
            pAgent.SetVariable<Vector3>("p_attackPathCurTargetPos", vector2, 0x12a69858);
            return status;
        }
    }
}

