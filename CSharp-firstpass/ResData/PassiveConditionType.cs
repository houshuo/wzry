namespace ResData
{
    using System;

    public enum PassiveConditionType
    {
        ActorCritCondition = 10,
        ActorReviveCondition = 9,
        ActorUpgradeCondition = 11,
        AssistPassiveCondition = 7,
        BeKilledPassiveCondition = 3,
        DamagePassiveCondition = 2,
        EnterBattlePassiveCondition = 15,
        ExitBattlePassiveCondition = 6,
        FightStartCondition = 8,
        HpPassiveCondition = 5,
        ImmuneDeadHurtCondition = 13,
        KilledPassiveCondition = 4,
        LimitMoveCondition = 14,
        NoDamagePassiveCondition = 1,
        RemoveBuffPassiveCondition = 0x10,
        UseSkillCondition = 12
    }
}

