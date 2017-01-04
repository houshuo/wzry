namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    internal class Condition_bt_WrapperAI_Hero_HeroWarmNormalAI_node169 : Condition
    {
        private int opl_p1 = 0x61a8;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            Vector3 variable = (Vector3) pAgent.GetVariable((uint) 0x9bc0c9a2);
            bool flag = ((ObjAgent) pAgent).IsDistanceToPosMoreThanRange(variable, this.opl_p1);
            bool flag2 = true;
            return ((flag != flag2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

