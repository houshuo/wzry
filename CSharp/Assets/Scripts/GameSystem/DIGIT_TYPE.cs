namespace Assets.Scripts.GameSystem
{
    using System;

    public enum DIGIT_TYPE
    {
        Invalid,
        PhysicalAttackNormal,
        PhysicalAttackCrit,
        MagicAttackNormal,
        MagicAttackCrit,
        RealAttackNormal,
        RealAttackCrit,
        SufferPhysicalDamage,
        SufferMagicDamage,
        SufferRealDamage,
        ReviveHp,
        ReceiveSpirit,
        ReceiveGoldCoinInBattle,
        ReceiveLastHitGoldCoinInBattle
    }
}

