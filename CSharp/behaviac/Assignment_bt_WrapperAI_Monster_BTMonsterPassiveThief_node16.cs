namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterPassiveThief_node16 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint variable = (uint) pAgent.GetVariable((uint) 0x4349179f);
            Vector3 randomFarPoint = ((ObjAgent) pAgent).GetRandomFarPoint(variable);
            pAgent.SetVariable<Vector3>("p_randomPos", randomFarPoint, 0xcb5cf5ca);
            return status;
        }
    }
}

