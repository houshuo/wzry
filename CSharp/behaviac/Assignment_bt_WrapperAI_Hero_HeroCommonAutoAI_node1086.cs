namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroCommonAutoAI_node1086 : Assignment
    {
        private int opr_p0 = 0x2ee0;
        private TargetPriority opr_p1 = TargetPriority.TargetPriority_Hero;

        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            uint variable = (uint) pAgent.GetVariable((uint) 0xb81a7cc);
            uint num2 = ((ObjAgent) pAgent).GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor(this.opr_p0, this.opr_p1, variable);
            pAgent.SetVariable<uint>("p_targetID", num2, 0x4349179f);
            return status;
        }
    }
}

