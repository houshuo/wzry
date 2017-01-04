namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    internal class Action_bt_WrapperAI_Monster_BTMonsterRunToInitiative_node257 : behaviac.Action
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            Vector3 variable = (Vector3) pAgent.GetVariable((uint) 0xde2f3d28);
            ((ObjAgent) pAgent).RealMovePosition(variable);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

