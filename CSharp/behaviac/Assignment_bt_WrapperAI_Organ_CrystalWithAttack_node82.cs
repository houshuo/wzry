namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Organ_CrystalWithAttack_node82 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int attackRange = ((ObjAgent) pAgent).GetAttackRange();
            pAgent.SetVariable<int>("p_srchRange", attackRange, 0x921d0d6a);
            return status;
        }
    }
}

