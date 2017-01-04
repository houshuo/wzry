namespace behaviac
{
    using System;
    using UnityEngine;

    internal class Assignment_bt_WrapperAI_Hero_HeroWarmNormalAI_node212 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            Vector3 variable = (Vector3) pAgent.GetVariable((uint) 0x9bc0c9a2);
            pAgent.SetVariable<Vector3>("p_attackPathCurTargetPos", variable, 0x12a69858);
            return status;
        }
    }
}

