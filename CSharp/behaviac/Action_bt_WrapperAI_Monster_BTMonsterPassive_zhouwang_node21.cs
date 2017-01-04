namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    internal class Action_bt_WrapperAI_Monster_BTMonsterPassive_zhouwang_node21 : behaviac.Action
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            Vector3 variable = (Vector3) pAgent.GetVariable((uint) 0xcd833580);
            ((ObjAgent) pAgent).LookAtDirection(variable);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

