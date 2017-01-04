namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    internal class Action_bt_WrapperAI_Hero_HeroSimpleAI_node452 : behaviac.Action
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            Vector3 variable = (Vector3) pAgent.GetVariable((uint) 0xbf2a6712);
            ((ObjAgent) pAgent).LookAtDirection(variable);
            return EBTStatus.BT_SUCCESS;
        }
    }
}

