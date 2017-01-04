namespace Assets.Scripts.GameLogic.DataCenter
{
    using Assets.Scripts.Framework;
    using ResData;
    using System;

    internal class ActorStaticLobbyDataProvider : ActorStaticDataProviderBase
    {
        protected override bool BuildHeroData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
        {
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint) actorData.TheActorMeta.ConfigId);
            if (dataByKey == null)
            {
                base.ErrorMissingHeroConfig((uint) actorData.TheActorMeta.ConfigId);
                return false;
            }
            actorData.TheBaseAttribute.EpType = dataByKey.dwEnergyType;
            actorData.TheBaseAttribute.BaseEp = (int) dataByKey.iEnergy;
            actorData.TheBaseAttribute.EpGrowth = (int) dataByKey.iEnergyGrowth;
            actorData.TheBaseAttribute.BaseEpRecover = (int) dataByKey.iEnergyRec;
            actorData.TheBaseAttribute.PerLvEpRecover = (int) dataByKey.iEnergyRecGrowth;
            actorData.TheBaseAttribute.BaseHp = (int) dataByKey.iBaseHP;
            actorData.TheBaseAttribute.PerLvHp = (int) dataByKey.iHpGrowth;
            actorData.TheBaseAttribute.BaseAd = (int) dataByKey.iBaseATT;
            actorData.TheBaseAttribute.PerLvAd = (int) dataByKey.iAtkGrowth;
            actorData.TheBaseAttribute.BaseAp = (int) dataByKey.iBaseINT;
            actorData.TheBaseAttribute.PerLvAp = (int) dataByKey.iSpellGrowth;
            actorData.TheBaseAttribute.BaseAtkSpeed = (int) dataByKey.iBaseAtkSpd;
            actorData.TheBaseAttribute.PerLvAtkSpeed = (int) dataByKey.iAtkSpdAddLvlup;
            actorData.TheBaseAttribute.BaseDef = (int) dataByKey.iBaseDEF;
            actorData.TheBaseAttribute.PerLvDef = (int) dataByKey.iDefGrowth;
            actorData.TheBaseAttribute.BaseRes = (int) dataByKey.iBaseRES;
            actorData.TheBaseAttribute.PerLvRes = (int) dataByKey.iResistGrowth;
            actorData.TheBaseAttribute.BaseHpRecover = (int) dataByKey.iBaseHPAdd;
            actorData.TheBaseAttribute.PerLvHpRecover = (int) dataByKey.iHPAddLvlup;
            actorData.TheBaseAttribute.CriticalChance = (int) dataByKey.iCritRate;
            actorData.TheBaseAttribute.CriticalDamage = (int) dataByKey.iCritEft;
            actorData.TheBaseAttribute.Sight = dataByKey.iSightR;
            actorData.TheBaseAttribute.MoveSpeed = (int) dataByKey.iBaseSpeed;
            actorData.TheBaseAttribute.SoulExpGained = 0;
            actorData.TheBaseAttribute.GoldCoinInBattleGained = 0;
            actorData.TheBaseAttribute.GoldCoinInBattleGainedFloatRange = 0;
            actorData.TheBaseAttribute.ClashMark = 0;
            actorData.TheBaseAttribute.RandomPassiveSkillRule = 0;
            actorData.TheBaseAttribute.PassiveSkillID1 = dataByKey.iPassiveID1;
            actorData.TheBaseAttribute.PassiveSkillID2 = dataByKey.iPassiveID2;
            actorData.TheHeroOnlyInfo.HeroCapability = dataByKey.bMainJob;
            actorData.TheHeroOnlyInfo.HeroDamageType = dataByKey.bDamageType;
            actorData.TheHeroOnlyInfo.HeroAttackType = dataByKey.bAttackType;
            actorData.TheHeroOnlyInfo.InitialStar = dataByKey.iInitialStar;
            actorData.TheHeroOnlyInfo.RecommendStandPos = dataByKey.iRecommendPosition;
            actorData.TheHeroOnlyInfo.AttackDistanceType = dataByKey.bAttackDistanceType;
            actorData.TheHeroOnlyInfo.HeroNamePinYin = dataByKey.szNamePinYin;
            actorData.TheResInfo.Name = StringHelper.UTF8BytesToString(ref dataByKey.szName);
            actorData.TheResInfo.ResPath = StringHelper.UTF8BytesToString(ref dataByKey.szCharacterInfo);
            actorData.ProviderType = GameActorDataProviderType.StaticLobbyDataProvider;
            return true;
        }

        protected override bool BuildMonsterData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
        {
            ResMonsterCfgInfo dataCfgInfo = MonsterDataHelper.GetDataCfgInfo(actorData.TheActorMeta.ConfigId, actorData.TheActorMeta.Difficuty);
            if (dataCfgInfo == null)
            {
                base.ErrorMissingMonsterConfig((uint) actorData.TheActorMeta.ConfigId);
                return false;
            }
            actorData.TheBaseAttribute.EpType = 1;
            actorData.TheBaseAttribute.BaseHp = dataCfgInfo.iBaseHP;
            actorData.TheBaseAttribute.PerLvHp = 0;
            actorData.TheBaseAttribute.BaseAd = dataCfgInfo.iBaseATT;
            actorData.TheBaseAttribute.PerLvAd = 0;
            actorData.TheBaseAttribute.BaseAp = dataCfgInfo.iBaseINT;
            actorData.TheBaseAttribute.PerLvAp = 0;
            actorData.TheBaseAttribute.BaseAtkSpeed = 0;
            actorData.TheBaseAttribute.PerLvAtkSpeed = 0;
            actorData.TheBaseAttribute.BaseDef = dataCfgInfo.iBaseDEF;
            actorData.TheBaseAttribute.PerLvDef = 0;
            actorData.TheBaseAttribute.BaseRes = dataCfgInfo.iBaseRES;
            actorData.TheBaseAttribute.PerLvRes = 0;
            actorData.TheBaseAttribute.BaseHpRecover = dataCfgInfo.iBaseHPAdd;
            actorData.TheBaseAttribute.PerLvHpRecover = 0;
            actorData.TheBaseAttribute.CriticalChance = 0;
            actorData.TheBaseAttribute.CriticalDamage = 0;
            actorData.TheBaseAttribute.Sight = dataCfgInfo.iSightR;
            actorData.TheBaseAttribute.MoveSpeed = dataCfgInfo.iBaseSpeed;
            actorData.TheBaseAttribute.SoulExpGained = dataCfgInfo.iSoulExp;
            actorData.TheBaseAttribute.GoldCoinInBattleGained = dataCfgInfo.wStartingGoldCoinInBattle;
            actorData.TheBaseAttribute.GoldCoinInBattleGainedFloatRange = dataCfgInfo.bGoldCoinInBattleRange;
            actorData.TheBaseAttribute.DynamicProperty = dataCfgInfo.dwDynamicPropertyCfg;
            actorData.TheBaseAttribute.ClashMark = dataCfgInfo.dwClashMark;
            actorData.TheResInfo.Name = StringHelper.UTF8BytesToString(ref dataCfgInfo.szName);
            actorData.TheResInfo.ResPath = StringHelper.UTF8BytesToString(ref dataCfgInfo.szCharacterInfo);
            actorData.ProviderType = GameActorDataProviderType.StaticLobbyDataProvider;
            return true;
        }

        protected override bool BuildOrganData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
        {
            ResOrganCfgInfo dataCfgInfoByCurLevelDiff = OrganDataHelper.GetDataCfgInfoByCurLevelDiff(actorData.TheActorMeta.ConfigId);
            if (dataCfgInfoByCurLevelDiff == null)
            {
                base.ErrorMissingOrganConfig((uint) actorData.TheActorMeta.ConfigId);
                return false;
            }
            actorData.TheBaseAttribute.EpType = 1;
            actorData.TheBaseAttribute.BaseHp = dataCfgInfoByCurLevelDiff.iBaseHP;
            actorData.TheBaseAttribute.PerLvHp = dataCfgInfoByCurLevelDiff.iHPLvlup;
            actorData.TheBaseAttribute.BaseAd = dataCfgInfoByCurLevelDiff.iBaseATT;
            actorData.TheBaseAttribute.PerLvAd = dataCfgInfoByCurLevelDiff.iATTLvlup;
            actorData.TheBaseAttribute.BaseAp = dataCfgInfoByCurLevelDiff.iBaseINT;
            actorData.TheBaseAttribute.PerLvAp = dataCfgInfoByCurLevelDiff.iINTLvlup;
            actorData.TheBaseAttribute.BaseAtkSpeed = 0;
            actorData.TheBaseAttribute.PerLvAtkSpeed = dataCfgInfoByCurLevelDiff.iAtkSpdAddLvlup;
            actorData.TheBaseAttribute.BaseDef = dataCfgInfoByCurLevelDiff.iBaseDEF;
            actorData.TheBaseAttribute.PerLvDef = dataCfgInfoByCurLevelDiff.iDEFLvlup;
            actorData.TheBaseAttribute.BaseRes = dataCfgInfoByCurLevelDiff.iBaseRES;
            actorData.TheBaseAttribute.PerLvRes = dataCfgInfoByCurLevelDiff.iRESLvlup;
            actorData.TheBaseAttribute.BaseHpRecover = dataCfgInfoByCurLevelDiff.iBaseHPAdd;
            actorData.TheBaseAttribute.PerLvHpRecover = dataCfgInfoByCurLevelDiff.iHPAddLvlup;
            actorData.TheBaseAttribute.CriticalChance = 0;
            actorData.TheBaseAttribute.CriticalDamage = 0;
            actorData.TheBaseAttribute.Sight = dataCfgInfoByCurLevelDiff.iSightR;
            actorData.TheBaseAttribute.MoveSpeed = dataCfgInfoByCurLevelDiff.iBaseSpeed;
            actorData.TheBaseAttribute.SoulExpGained = dataCfgInfoByCurLevelDiff.iSoulExp;
            actorData.TheBaseAttribute.GoldCoinInBattleGained = dataCfgInfoByCurLevelDiff.wGoldCoinInBattle;
            actorData.TheBaseAttribute.GoldCoinInBattleGainedFloatRange = 0;
            actorData.TheBaseAttribute.DynamicProperty = dataCfgInfoByCurLevelDiff.dwDynamicPropertyCfg;
            actorData.TheBaseAttribute.ClashMark = dataCfgInfoByCurLevelDiff.dwClashMark;
            actorData.TheResInfo.Name = StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szName);
            actorData.TheResInfo.ResPath = StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szCharacterInfo);
            actorData.TheOrganOnlyInfo.PhyArmorHurtRate = dataCfgInfoByCurLevelDiff.iPhyArmorHurtRate;
            actorData.TheOrganOnlyInfo.AttackRouteID = dataCfgInfoByCurLevelDiff.iAktRouteID;
            actorData.TheOrganOnlyInfo.DeadEnemySoldier = dataCfgInfoByCurLevelDiff.iDeadEnemySoldier;
            actorData.TheOrganOnlyInfo.NoEnemyAddPhyDef = dataCfgInfoByCurLevelDiff.iNoEnemyAddPhyDef;
            actorData.TheOrganOnlyInfo.NoEnemyAddMgcDef = dataCfgInfoByCurLevelDiff.iNoEnemyAddMgcDef;
            actorData.TheOrganOnlyInfo.HorizonRadius = dataCfgInfoByCurLevelDiff.iHorizonRadius;
            actorData.ProviderType = GameActorDataProviderType.StaticLobbyDataProvider;
            return true;
        }
    }
}

