namespace behaviac
{
    using System;
    using UnityEngine;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterRunToInitiative_node62 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            Vector3 variable = (Vector3) pAgent.GetVariable((uint) 0x12a69858);
            pAgent.SetVariable<Vector3>("p_orignalPos", variable, 0xde2f3d28);
            return status;
        }
    }
}

