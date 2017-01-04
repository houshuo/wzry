namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;
    using UnityEngine;

    internal class Condition_bt_WrapperAI_Monster_BTMonsterBossInitiative_node130 : Condition
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            Vector3 variable = (Vector3) pAgent.GetVariable((uint) 0xde2f3d28);
            int range = (int) pAgent.GetVariable((uint) 0x332d755);
            bool flag = ((ObjAgent) pAgent).IsDistanceToPosLessThanRange(variable, range);
            bool flag2 = false;
            return ((flag != flag2) ? EBTStatus.BT_FAILURE : EBTStatus.BT_SUCCESS);
        }
    }
}

