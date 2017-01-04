namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterRob_node133 : Assignment
    {
        private TargetPriority opr_p1 = TargetPriority.TargetPriority_Monster;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0x921d0d6a);
            uint nearestEnemyWithTargetPriority = ((ObjAgent) pAgent).GetNearestEnemyWithTargetPriority(variable, this.opr_p1);
            pAgent.SetVariable<uint>("p_targetID", nearestEnemyWithTargetPriority, 0x4349179f);
            return status;
        }
    }
}

