namespace behaviac
{
    using Assets.Scripts.GameLogic;
    using System;

    internal class Assignment_bt_WrapperAI_Monster_BTMonsterZhaoHuanWu_node336 : Assignment
    {
        protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
        {
            EBTStatus status = EBTStatus.BT_SUCCESS;
            int variable = (int) pAgent.GetVariable((uint) 0x921d0d6a);
            uint nearestEnemyWithoutNotInBattleJungleMonster = ((ObjAgent) pAgent).GetNearestEnemyWithoutNotInBattleJungleMonster(variable);
            pAgent.SetVariable<uint>("p_targetID", nearestEnemyWithoutNotInBattleJungleMonster, 0x4349179f);
            return status;
        }
    }
}

