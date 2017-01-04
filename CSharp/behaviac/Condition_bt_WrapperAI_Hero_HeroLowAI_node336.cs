namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    internal class Condition_bt_WrapperAI_Hero_HeroLowAI_node336 : Condition
    {
        private int opl_p1 = 600;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            Vector3 variable = (Vector3) pAgent.GetVariable((uint) 0x12a69858);
            bool flag = ((ObjAgent) pAgent).IsDistanceToPosLessThanRange(variable, this.opl_p1);
            bool flag2 = true;
            return ((flag != flag2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

