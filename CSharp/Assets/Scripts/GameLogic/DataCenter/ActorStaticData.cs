namespace Assets.Scripts.GameLogic.DataCenter
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct ActorStaticData
    {
        public ActorMeta TheActorMeta;
        public BaseAttribute TheBaseAttribute;
        public HeroOnlyInfo TheHeroOnlyInfo;
        public MonsterOnlyInfo TheMonsterOnlyInfo;
        public OrganOnlyInfo TheOrganOnlyInfo;
        public ResInfo TheResInfo;
        public GameActorDataProviderType ProviderType;
        [StructLayout(LayoutKind.Sequential)]
        public struct BaseAttribute
        {
            public int Sight;
            public int MoveSpeed;
            public int BaseAtkSpeed;
            public int PerLvAtkSpeed;
            public int BaseHp;
            public int PerLvHp;
            public int BaseHpRecover;
            public int PerLvHpRecover;
            public uint EpType;
            public int BaseEp;
            public int EpGrowth;
            public int BaseEpRecover;
            public int PerLvEpRecover;
            public int BaseAd;
            public int PerLvAd;
            public int BaseAp;
            public int PerLvAp;
            public int BaseDef;
            public int PerLvDef;
            public int BaseRes;
            public int PerLvRes;
            public int CriticalChance;
            public int CriticalDamage;
            public int SoulExpGained;
            public int GoldCoinInBattleGained;
            public int GoldCoinInBattleGainedFloatRange;
            public uint DynamicProperty;
            public uint ClashMark;
            public int RandomPassiveSkillRule;
            public int PassiveSkillID1;
            public int PassiveSkillID2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HeroOnlyInfo
        {
            public int HeroCapability;
            public int HeroAttackType;
            public int HeroDamageType;
            public int InitialStar;
            public int RecommendStandPos;
            public int AttackDistanceType;
            public string HeroNamePinYin;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MonsterOnlyInfo
        {
            public int Reserved;
            public int MonsterBaseLevel;
            public byte SoldierType;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OrganOnlyInfo
        {
            public int OrganType;
            public bool ShowInMinimap;
            public int PhyArmorHurtRate;
            public int AttackRouteID;
            public int DeadEnemySoldier;
            public int NoEnemyAddPhyDef;
            public int NoEnemyAddMgcDef;
            public int HorizonRadius;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ResInfo
        {
            public string Name;
            public string ResPath;
        }
    }
}

