namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Hero_HeroLowAI_node590 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0x921d0d6a);
            uint withOutActor = (uint) pAgent.GetVariable((uint) 0xb81a7cc);
            uint nearestEnemyWithoutNotInBattleJungleMonsterWithoutActor = ((ObjAgent) pAgent).GetNearestEnemyWithoutNotInBattleJungleMonsterWithoutActor(variable, withOutActor);
            pAgent.SetVariable<uint>("p_targetID", nearestEnemyWithoutNotInBattleJungleMonsterWithoutActor, 0x4349179f);
            return status;
        }
    }
}

