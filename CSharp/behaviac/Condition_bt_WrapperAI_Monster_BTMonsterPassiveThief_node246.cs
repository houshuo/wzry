namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    internal class Condition_bt_WrapperAI_Monster_BTMonsterPassiveThief_node246 : Condition
    {
        private int opl_p1 = 600;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            Vector3 variable = (Vector3) pAgent.GetVariable((uint) 0xcb5cf5ca);
            bool flag = ((ObjAgent) pAgent).IsDistanceToPosLessThanRange(variable, this.opl_p1);
            bool flag2 = true;
            return ((flag != flag2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

