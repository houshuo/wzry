namespace Assets.Scripts.GameLogic.DataCenter
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using ResData;
    using System;

    internal class ActorStaticBattleDataProvider : ActorStaticDataProviderBase
    {
        protected override bool BuildHeroData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
        {
            IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticLobbyDataProvider);
            actorData.ProviderType = GameActorDataProviderType.StaticBattleDataProvider;
            return actorDataProvider.GetActorStaticData(ref actorMeta, ref actorData);
        }

        protected override bool BuildMonsterData(ref ActorMeta actorMeta, ref ActorStaticData actorData)
        {
            ResMonsterCfgInfo monsterCfg = this.ConsiderDifficultyToChooseMonsterCfg((uint) actorData.TheActorMeta.ConfigId, actorData.TheActorMeta.Difficuty);
            if (monsterCfg == null)
            {
                monsterCfg = MonsterDataHelper.GetDataCfgInfo(actorData.TheActorMeta.ConfigId, actorData.TheActorMeta.Difficuty);
                if (monsterCfg == null)
                {
                    base.ErrorMissingMonsterConfig((uint) actorData.TheActorMeta.ConfigId);
                    return false;
                }
            }
            DynamicAttributeInfo info2 = this.ConsiderMonsterDynamicInfo(monsterCfg);
            actorData.TheBaseAttribute.EpType = 1;
            actorData.TheBaseAttribute.BaseHp = (info2 == null) ? monsterCfg.iBaseHP : info2.iBaseHP;
            actorData.TheBaseAttribute.PerLvHp = 0;
            actorData.TheBaseAttribute.BaseAd = (info2 == null) ? monsterCfg.iBaseATT : info2.iAD;
            actorData.TheBaseAttribute.PerLvAd = 0;
            actorData.TheBaseAttribute.BaseAp = (info2 == null) ? monsterCfg.iBaseINT : info2.iAP;
            actorData.TheBaseAttribute.PerLvAp = 0;
            actorData.TheBaseAttribute.BaseAtkSpeed = 0;
            actorData.TheBaseAttribute.PerLvAtkSpeed = 0;
            actorData.TheBaseAttribute.BaseDef = (info2 == null) ? monsterCfg.iBaseDEF : info2.iDef;
            actorData.TheBaseAttribute.PerLvDef = 0;
            actorData.TheBaseAttribute.BaseRes = (info2 == null) ? monsterCfg.iBaseRES : info2.iRes;
            actorData.TheBaseAttribute.PerLvRes = 0;
            actorData.TheBaseAttribute.BaseHpRecover = monsterCfg.iBaseHPAdd;
            actorData.TheBaseAttribute.PerLvHpRecover = 0;
            actorData.TheBaseAttribute.CriticalChance = 0;
            actorData.TheBaseAttribute.CriticalDamage = 0;
            actorData.TheBaseAttribute.Sight = monsterCfg.iSightR;
            actorData.TheBaseAttribute.MoveSpeed = monsterCfg.iBaseSpeed;
            actorData.TheBaseAttribute.SoulExpGained = monsterCfg.iSoulExp;
            actorData.TheBaseAttribute.GoldCoinInBattleGained = monsterCfg.wStartingGoldCoinInBattle;
            actorData.TheBaseAttribute.GoldCoinInBattleGainedFloatRange = monsterCfg.bGoldCoinInBattleRange;
            actorData.TheBaseAttribute.DynamicProperty = monsterCfg.dwDynamicPropertyCfg;
            actorData.TheBaseAttribute.ClashMark = monsterCfg.dwClashMark;
            actorData.TheBaseAttribute.RandomPassiveSkillRule = monsterCfg.bRandomPassiveSkillRule;
            actorData.TheBaseAttribute.PassiveSkillID1 = 0;
            actorData.TheBaseAttribute.PassiveSkillID2 = 0;
            actorData.TheMonsterOnlyInfo.MonsterBaseLevel = monsterCfg.iLevel;
            actorData.TheMonsterOnlyInfo.SoldierType = monsterCfg.bSoldierType;
            actorData.TheResInfo.Name = StringHelper.UTF8BytesToString(ref monsterCfg.szName);
            actorData.TheResInfo.ResPath = StringHelper.UTF8BytesToString(ref monsterCfg.szCharacterInfo);
            actorData.ProviderType = GameActorDataProviderType.StaticBattleDataProvider;
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
            DynamicAttributeInfo info2 = this.ConsiderOrganDynamicInfo(dataCfgInfoByCurLevelDiff);
            actorData.TheBaseAttribute.EpType = 1;
            actorData.TheBaseAttribute.BaseHp = (info2 == null) ? dataCfgInfoByCurLevelDiff.iBaseHP : info2.iBaseHP;
            actorData.TheBaseAttribute.PerLvHp = dataCfgInfoByCurLevelDiff.iHPLvlup;
            actorData.TheBaseAttribute.BaseAd = (info2 == null) ? dataCfgInfoByCurLevelDiff.iBaseATT : info2.iAD;
            actorData.TheBaseAttribute.PerLvAd = dataCfgInfoByCurLevelDiff.iATTLvlup;
            actorData.TheBaseAttribute.BaseAp = (info2 == null) ? dataCfgInfoByCurLevelDiff.iBaseINT : info2.iAP;
            actorData.TheBaseAttribute.PerLvAp = dataCfgInfoByCurLevelDiff.iINTLvlup;
            actorData.TheBaseAttribute.BaseAtkSpeed = 0;
            actorData.TheBaseAttribute.PerLvAtkSpeed = dataCfgInfoByCurLevelDiff.iAtkSpdAddLvlup;
            actorData.TheBaseAttribute.BaseDef = (info2 == null) ? dataCfgInfoByCurLevelDiff.iBaseDEF : info2.iDef;
            actorData.TheBaseAttribute.PerLvDef = dataCfgInfoByCurLevelDiff.iDEFLvlup;
            actorData.TheBaseAttribute.BaseRes = (info2 == null) ? dataCfgInfoByCurLevelDiff.iBaseRES : info2.iRes;
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
            actorData.TheBaseAttribute.RandomPassiveSkillRule = 0;
            actorData.TheBaseAttribute.PassiveSkillID1 = 0;
            actorData.TheBaseAttribute.PassiveSkillID2 = 0;
            actorData.TheResInfo.Name = StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szName);
            actorData.TheResInfo.ResPath = StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szCharacterInfo);
            actorData.TheOrganOnlyInfo.OrganType = dataCfgInfoByCurLevelDiff.bOrganType;
            actorData.TheOrganOnlyInfo.ShowInMinimap = dataCfgInfoByCurLevelDiff.bShowInMinimap != 0;
            actorData.TheOrganOnlyInfo.PhyArmorHurtRate = dataCfgInfoByCurLevelDiff.iPhyArmorHurtRate;
            actorData.TheOrganOnlyInfo.AttackRouteID = dataCfgInfoByCurLevelDiff.iAktRouteID;
            actorData.TheOrganOnlyInfo.DeadEnemySoldier = dataCfgInfoByCurLevelDiff.iDeadEnemySoldier;
            actorData.TheOrganOnlyInfo.NoEnemyAddPhyDef = dataCfgInfoByCurLevelDiff.iNoEnemyAddPhyDef;
            actorData.TheOrganOnlyInfo.NoEnemyAddMgcDef = dataCfgInfoByCurLevelDiff.iNoEnemyAddMgcDef;
            actorData.TheOrganOnlyInfo.HorizonRadius = dataCfgInfoByCurLevelDiff.iHorizonRadius;
            actorData.ProviderType = GameActorDataProviderType.StaticBattleDataProvider;
            return true;
        }

        internal ResMonsterCfgInfo ConsiderDifficultyToChooseMonsterCfg(uint monsterCfgId, byte diff)
        {
            return MonsterDataHelper.GetDataCfgInfo((int) monsterCfgId, diff);
        }

        internal DynamicAttributeInfo ConsiderMonsterDynamicInfo(ResMonsterCfgInfo monsterCfg)
        {
            int dwAIPlayerLevel = 0;
            if (monsterCfg.iDynamicInfoType == 1)
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                if ((curLvelContext != null) && !curLvelContext.IsMobaMode())
                {
                    ResLevelCfgInfo info2 = GameDataMgr.levelDatabin.GetDataByKey((long) curLvelContext.m_mapID);
                    if (info2 == null)
                    {
                        base.ErrorMissingLevelConfig((uint) curLvelContext.m_mapID);
                        return null;
                    }
                    if (curLvelContext.m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_PVP)
                    {
                        dwAIPlayerLevel = (int) info2.dwAIPlayerLevel;
                    }
                }
            }
            if (dwAIPlayerLevel <= 0)
            {
                return null;
            }
            ResMonsterOrganLevelDynamicInfo dataByKey = GameDataMgr.monsterOrganLvDynamicInfobin.GetDataByKey((long) dwAIPlayerLevel);
            if (dataByKey == null)
            {
                return null;
            }
            switch (monsterCfg.bSoldierType)
            {
                case 1:
                    return dataByKey.stMelee;

                case 2:
                    return dataByKey.stRemote;

                case 3:
                    return dataByKey.stSuper;

                case 4:
                    return dataByKey.stAnYingDaJiang;

                case 5:
                    return dataByKey.stHeiAnXianFeng;

                case 6:
                    return dataByKey.stBaoZouJiangShi;

                case 7:
                case 8:
                    return dataByKey.stBaoJun;
            }
            return null;
        }

        internal DynamicAttributeInfo ConsiderOrganDynamicInfo(ResOrganCfgInfo organCfg)
        {
            int dwAIPlayerLevel = 0;
            if (organCfg.iDynamicInfoType == 1)
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                if ((curLvelContext != null) && !curLvelContext.IsMobaMode())
                {
                    ResLevelCfgInfo info2 = GameDataMgr.levelDatabin.GetDataByKey((long) curLvelContext.m_mapID);
                    if (info2 == null)
                    {
                        base.ErrorMissingLevelConfig((uint) curLvelContext.m_mapID);
                        return null;
                    }
                    if (curLvelContext.m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_PVP)
                    {
                        dwAIPlayerLevel = (int) info2.dwAIPlayerLevel;
                    }
                }
            }
            if (dwAIPlayerLevel <= 0)
            {
                return null;
            }
            ResMonsterOrganLevelDynamicInfo dataByKey = GameDataMgr.monsterOrganLvDynamicInfobin.GetDataByKey((long) dwAIPlayerLevel);
            if (dataByKey == null)
            {
                return null;
            }
            switch (organCfg.bOrganType)
            {
                case 1:
                    return dataByKey.stTurret;

                case 2:
                    return dataByKey.stBase;

                case 3:
                    return dataByKey.stBarrack;
            }
            return null;
        }
    }
}

