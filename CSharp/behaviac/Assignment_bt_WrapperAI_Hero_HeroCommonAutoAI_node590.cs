namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node590 : Assignment
    {
        private TargetPriority opr_p1 = TargetPriority.TargetPriority_Hero;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0x921d0d6a);
            uint withOutActor = (uint) pAgent.GetVariable((uint) 0xb81a7cc);
            uint num3 = ((ObjAgent) pAgent).GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor(variable, this.opr_p1, withOutActor);
            pAgent.SetVariable<uint>("p_targetID", num3, 0x4349179f);
            return status;
        }
    }
}

