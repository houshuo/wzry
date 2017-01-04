namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using ResData;
    using System;

    public class SLevelContext
    {
        public bool canAutoAI;
        public uint m_addLoseCondStarId;
        public uint m_addWinCondStarId;
        public string m_ambientSoundEvent;
        public int m_baseReviveTime = 0x3a98;
        public uint[] m_battleTaskOfCamps = new uint[3];
        public bool m_bEnableFow;
        public bool m_bEnableOrnamentSlot;
        public bool m_bEnableShopHorizonTab;
        public float m_bigMapFowScale;
        public int m_bigMapHeight;
        public string m_bigMapPath;
        public int m_bigMapWidth;
        public int m_birthLevelConfig;
        public int m_chapterNo;
        public uint m_cooldownReduceUpperLimit;
        public uint m_dynamicPropertyConfig;
        public int m_entertainmentSubMapType = -1;
        public ResDT_ExpCompensateInfo[] m_expCompensateInfo;
        public int m_extraPassiveSkillId;
        public int m_extraSkill2Id;
        public int m_extraSkillId;
        public int m_failureDialogId;
        public string m_gameMatchName = string.Empty;
        private COM_GAME_TYPE m_gameType;
        public int m_headPtsUpperLimit;
        public RES_LEVEL_HEROAITYPE m_heroAiType;
        public Horizon.EnableMethod m_horizonEnableMethod;
        public bool m_isBattleEquipLimit;
        public bool m_isCameraFlip;
        public bool m_isCanRightJoyStickCameraDrag;
        public bool m_isMobaType;
        public byte m_isOpenExpCompensate;
        public bool m_isShowHonor;
        public bool m_isShowTrainingHelper;
        public bool m_isWarmBattle;
        public string m_levelArtistFileName = string.Empty;
        public string m_levelDesignFileName = string.Empty;
        public int m_levelDifficulty;
        public string m_levelName = string.Empty;
        public byte m_levelNo;
        public int m_loseCondition;
        public ResDT_MapBuff[] m_mapBuffs;
        public float m_mapFowScale;
        public int m_mapHeight;
        public int m_mapID;
        public int m_mapType = -1;
        public int m_mapWidth;
        public string m_miniMapPath;
        public string m_musicBankResName;
        public string m_musicEndEvent;
        public string m_musicStartEvent;
        public ushort m_originalGoldCoinInBattle;
        public int m_ornamentSkillId;
        public int m_ornamentSwitchCD;
        public int m_passDialogId;
        public int m_preDialogId;
        private ResLevelCfgInfo m_pveLevelInfo;
        public RES_LEVEL_TYPE m_pveLevelType;
        private ResDT_LevelCommonInfo m_pvpLevelCommonInfo;
        public int m_pvpPlayerNum;
        public ResDT_PveReviveInfo[] m_reviveInfo = new ResDT_PveReviveInfo[0];
        public byte m_reviveTimeMax;
        public string m_SecondName = string.Empty;
        public enSelectType m_selectHeroType;
        public int m_soldierActivateCountDelay1;
        public int m_soldierActivateCountDelay2;
        public int m_soldierActivateDelay;
        public uint m_soulAllocId;
        public byte m_soulGrow;
        public uint m_soulID;
        public ResDT_IntParamArrayNode[] m_starDetail = new ResDT_IntParamArrayNode[0];
        public uint m_timeDuration;
        public ResBattleDynamicDifficulty m_warmHeroAiDiffInfo;

        public COM_GAME_TYPE GetGameType()
        {
            return this.m_gameType;
        }

        public enSelectType GetSelectHeroType()
        {
            return this.m_selectHeroType;
        }

        public void InitPveData(ResLevelCfgInfo levelCfg, int difficult)
        {
            this.m_isMobaType = false;
            this.m_pveLevelInfo = levelCfg;
            this.m_selectHeroType = enSelectType.enMutile;
            this.m_levelName = this.m_pveLevelInfo.szName;
            this.m_levelDesignFileName = this.m_pveLevelInfo.szDesignFileName;
            if ((this.m_pveLevelInfo.szArtistFileName != null) && (this.m_pveLevelInfo.szArtistFileName.Length > 0))
            {
                this.m_levelArtistFileName = this.m_pveLevelInfo.szArtistFileName;
            }
            this.m_mapWidth = this.m_pveLevelInfo.iMapWidth;
            this.m_mapHeight = this.m_pveLevelInfo.iMapHeight;
            this.m_bigMapWidth = this.m_pveLevelInfo.iBigMapWidth;
            this.m_bigMapHeight = this.m_pveLevelInfo.iBigMapHeight;
            this.m_miniMapPath = this.m_pveLevelInfo.szThumbnailPath;
            this.m_bigMapPath = this.m_pveLevelInfo.szBigMapPath;
            this.m_mapID = this.m_pveLevelInfo.iCfgID;
            this.m_chapterNo = this.m_pveLevelInfo.iChapterId;
            this.m_levelNo = this.m_pveLevelInfo.bLevelNo;
            this.m_levelDifficulty = difficult;
            this.m_pveLevelType = (RES_LEVEL_TYPE) this.m_pveLevelInfo.iLevelType;
            this.m_horizonEnableMethod = (Horizon.EnableMethod) this.m_pveLevelInfo.bEnableHorizon;
            this.m_isMobaType = this.m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_PVP;
            if (this.m_isMobaType)
            {
                this.m_horizonEnableMethod = Horizon.EnableMethod.EnableAll;
            }
            this.m_passDialogId = this.m_pveLevelInfo.iPassDialogId;
            this.m_preDialogId = this.m_pveLevelInfo.iPreDialogId;
            this.m_failureDialogId = this.m_pveLevelInfo.iFailureDialogId;
            this.m_heroAiType = (RES_LEVEL_HEROAITYPE) this.m_pveLevelInfo.iHeroAIType;
            this.m_soulGrow = this.m_pveLevelInfo.bSoulGrow;
            this.m_baseReviveTime = (int) this.m_pveLevelInfo.dwReviveTime;
            this.m_dynamicPropertyConfig = this.m_pveLevelInfo.dwDynamicPropertyCfg;
            this.m_miniMapPath = this.m_pveLevelInfo.szThumbnailPath;
            this.m_bigMapPath = this.m_pveLevelInfo.szBigMapPath;
            this.m_mapWidth = this.m_pveLevelInfo.iMapWidth;
            this.m_mapHeight = this.m_pveLevelInfo.iMapHeight;
            this.m_mapFowScale = 1f;
            this.m_bigMapFowScale = 1f;
            this.m_musicStartEvent = this.m_pveLevelInfo.szMusicStartEvent;
            this.m_musicEndEvent = this.m_pveLevelInfo.szMusicEndEvent;
            this.m_ambientSoundEvent = this.m_pveLevelInfo.szAmbientSoundEvent;
            this.m_musicBankResName = this.m_pveLevelInfo.szBankResourceName;
            this.canAutoAI = this.m_pveLevelInfo.bIsOpenAutoAI != 0;
            this.m_mapBuffs = this.m_pveLevelInfo.astMapBuffs;
            this.m_isShowTrainingHelper = this.m_pveLevelInfo.bShowTrainingHelper > 0;
            this.m_soulID = this.m_pveLevelInfo.dwSoulID;
            this.m_soulAllocId = this.m_pveLevelInfo.dwSoulAllocId;
            this.m_starDetail = this.m_pveLevelInfo.astStarDetail;
            this.m_reviveInfo = this.m_pveLevelInfo.astReviveInfo;
            this.m_reviveTimeMax = this.m_pveLevelInfo.bReviveTimeMax;
            this.m_loseCondition = this.m_pveLevelInfo.iLoseCondition;
            this.m_extraSkillId = this.m_pveLevelInfo.iExtraSkillId;
            this.m_extraSkill2Id = this.m_pveLevelInfo.iExtraSkill2Id;
            this.m_extraPassiveSkillId = this.m_pveLevelInfo.iExtraPassiveSkillId;
            this.m_isCanRightJoyStickCameraDrag = this.m_pveLevelInfo.bSupportCameraDrag > 0;
            this.m_isCameraFlip = false;
            this.m_pvpPlayerNum = this.m_pveLevelInfo.bMaxAcntNum;
        }

        public void InitPvpData(ResDT_LevelCommonInfo levelCommonInfo, uint mapID)
        {
            this.m_isMobaType = true;
            this.m_pvpLevelCommonInfo = levelCommonInfo;
            this.m_mapID = (int) mapID;
            this.m_selectHeroType = (enSelectType) this.m_pvpLevelCommonInfo.stPickRuleInfo.bPickType;
            this.m_levelName = this.m_pvpLevelCommonInfo.szName;
            this.m_levelDesignFileName = this.m_pvpLevelCommonInfo.szDesignFileName;
            if (this.m_pvpLevelCommonInfo.szArtistFileName != null)
            {
                this.m_levelArtistFileName = this.m_pvpLevelCommonInfo.szArtistFileName;
            }
            this.m_mapWidth = this.m_pvpLevelCommonInfo.iMapWidth;
            this.m_mapHeight = this.m_pvpLevelCommonInfo.iMapHeight;
            this.m_bigMapWidth = this.m_pvpLevelCommonInfo.iBigMapWidth;
            this.m_bigMapHeight = this.m_pvpLevelCommonInfo.iBigMapHeight;
            this.m_miniMapPath = this.m_pvpLevelCommonInfo.szThumbnailPath;
            this.m_bigMapPath = this.m_pvpLevelCommonInfo.szBigMapPath;
            this.m_mapFowScale = this.m_pvpLevelCommonInfo.fMapFowScale;
            this.m_bigMapFowScale = this.m_pvpLevelCommonInfo.fBigMapFowScale;
            this.m_heroAiType = (RES_LEVEL_HEROAITYPE) this.m_pvpLevelCommonInfo.iHeroAIType;
            this.m_pvpPlayerNum = this.m_pvpLevelCommonInfo.bMaxAcntNum;
            this.m_isBattleEquipLimit = this.m_pvpLevelCommonInfo.bBattleEquipLimit > 0;
            this.m_headPtsUpperLimit = this.m_pvpLevelCommonInfo.bHeadPtsUpperLimit;
            this.m_birthLevelConfig = this.m_pvpLevelCommonInfo.bBirthLevelConfig;
            this.m_isShowHonor = this.m_pvpLevelCommonInfo.bShowHonor > 0;
            this.m_cooldownReduceUpperLimit = this.m_pvpLevelCommonInfo.dwCooldownReduceUpperLimit;
            this.m_dynamicPropertyConfig = this.m_pvpLevelCommonInfo.dwDynamicPropertyCfg;
            this.m_originalGoldCoinInBattle = this.m_pvpLevelCommonInfo.wOriginalGoldCoinInBattle;
            this.m_battleTaskOfCamps[1] = this.m_pvpLevelCommonInfo.dwBattleTaskOfCamp1;
            this.m_battleTaskOfCamps[2] = this.m_pvpLevelCommonInfo.dwBattleTaskOfCamp2;
            this.m_musicStartEvent = this.m_pvpLevelCommonInfo.szMusicStartEvent;
            this.m_musicEndEvent = this.m_pvpLevelCommonInfo.szMusicEndEvent;
            this.m_musicBankResName = this.m_pvpLevelCommonInfo.szBankResourceName;
            this.m_ambientSoundEvent = this.m_pvpLevelCommonInfo.szAmbientSoundEvent;
            this.m_isOpenExpCompensate = this.m_pvpLevelCommonInfo.bIsOpenExpCompensate;
            this.m_expCompensateInfo = this.m_pvpLevelCommonInfo.astExpCompensateDetail;
            this.m_soldierActivateDelay = this.m_pvpLevelCommonInfo.iSoldierActivateDelay;
            this.m_soldierActivateCountDelay1 = this.m_pvpLevelCommonInfo.iSoldierActivateCountDelay1;
            this.m_soldierActivateCountDelay2 = this.m_pvpLevelCommonInfo.iSoldierActivateCountDelay2;
            this.m_timeDuration = this.m_pvpLevelCommonInfo.dwTimeDuration;
            this.m_addWinCondStarId = this.m_pvpLevelCommonInfo.dwAddWinCondStarId;
            this.m_addLoseCondStarId = this.m_pvpLevelCommonInfo.dwAddLoseCondStarId;
            this.m_soulID = this.m_pvpLevelCommonInfo.dwSoulID;
            this.m_soulAllocId = this.m_pvpLevelCommonInfo.dwSoulAllocId;
            this.m_extraSkillId = this.m_pvpLevelCommonInfo.iExtraSkillId;
            this.m_extraSkill2Id = this.m_pvpLevelCommonInfo.iExtraSkill2Id;
            this.m_extraPassiveSkillId = this.m_pvpLevelCommonInfo.iExtraPassiveSkillId;
            this.m_ornamentSkillId = this.m_pvpLevelCommonInfo.iOrnamentSkillId;
            this.m_ornamentSwitchCD = this.m_pvpLevelCommonInfo.iOrnamentSwitchCD;
            this.m_bEnableFow = this.m_pvpLevelCommonInfo.bIsEnableFow > 0;
            this.m_bEnableOrnamentSlot = this.m_pvpLevelCommonInfo.bIsEnableOrnamentSlot > 0;
            this.m_bEnableShopHorizonTab = this.m_pvpLevelCommonInfo.bIsEnableShopHorizonTab > 0;
            this.m_isCanRightJoyStickCameraDrag = this.m_pvpLevelCommonInfo.bSupportCameraDrag > 0;
            this.m_gameMatchName = this.m_pvpLevelCommonInfo.szGameMatchName;
            this.m_isCameraFlip = this.m_pvpLevelCommonInfo.bCameraFlip > 0;
        }

        public bool IsFireHolePlayMode()
        {
            return ((this.m_mapType == 4) && (this.m_entertainmentSubMapType == 2));
        }

        public bool IsGameTypeActivity()
        {
            return (this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ACTIVITY);
        }

        public bool IsGameTypeAdventure()
        {
            return (this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ADVENTURE);
        }

        public bool IsGameTypeArena()
        {
            return (this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ARENA);
        }

        public bool IsGameTypeBurning()
        {
            return (this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_BURNING);
        }

        public bool IsGameTypeComBat()
        {
            return (this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_COMBAT);
        }

        public bool IsGameTypeEntertainment()
        {
            return (this.m_gameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_ENTERTAINMENT);
        }

        public bool IsGameTypeGuide()
        {
            return (this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_GUIDE);
        }

        public bool IsGameTypeLadder()
        {
            return (this.m_gameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_LADDER);
        }

        public bool IsGameTypePvpMatch()
        {
            return (this.m_gameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_PVP_MATCH);
        }

        public bool IsGameTypePvpRoom()
        {
            return (this.m_gameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_PVP_ROOM);
        }

        public bool IsGameTypeRewardMatch()
        {
            return (this.m_gameType == COM_GAME_TYPE.COM_MULTI_GAME_OF_REWARDMATCH);
        }

        public bool IsLuanDouPlayMode()
        {
            return ((this.m_mapType == 4) && (this.m_entertainmentSubMapType == 1));
        }

        public bool IsMobaMode()
        {
            return this.m_isMobaType;
        }

        public bool IsMobaModeWithOutGuide()
        {
            return (this.m_isMobaType && !this.IsGameTypeGuide());
        }

        public bool IsMultilModeWithoutWarmBattle()
        {
            if ((((this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ADVENTURE) || (this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_COMBAT)) || ((this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_GUIDE) || (this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ACTIVITY))) || ((this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_BURNING) || (this.m_gameType == COM_GAME_TYPE.COM_SINGLE_GAME_OF_ARENA)))
            {
                return false;
            }
            return true;
        }

        public bool IsMultilModeWithWarmBattle()
        {
            return (this.m_isWarmBattle || this.IsMultilModeWithoutWarmBattle());
        }

        public virtual bool IsSoulGrow()
        {
            return (this.m_isMobaType || (this.m_soulGrow > 0));
        }

        public void SetGameType(COM_GAME_TYPE gType)
        {
            this.m_gameType = gType;
        }

        public static void SetMasterPvpDetailWhenGameSettle(COMDT_GAME_INFO gameInfo)
        {
            byte bMaxAcntNum;
            byte bGameType = gameInfo.bGameType;
            byte bMapType = gameInfo.bMapType;
            uint iLevelID = (uint) gameInfo.iLevelID;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "masterRoleInfo is null");
            if (masterRoleInfo == null)
            {
                return;
            }
            switch (((COM_GAME_TYPE) bGameType))
            {
                case COM_GAME_TYPE.COM_SINGLE_GAME_OF_COMBAT:
                    Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stVsMachineInfo, gameInfo.bGameResult);
                    return;

                case COM_GAME_TYPE.COM_SINGLE_GAME_OF_GUIDE:
                case COM_GAME_TYPE.COM_SINGLE_GAME_OF_ACTIVITY:
                case COM_GAME_TYPE.COM_MULTI_GAME_OF_PVP_ROOM:
                case COM_GAME_TYPE.COM_SINGLE_GAME_OF_BURNING:
                case COM_GAME_TYPE.COM_SINGLE_GAME_OF_ARENA:
                    return;

                case COM_GAME_TYPE.COM_MULTI_GAME_OF_LADDER:
                    Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stLadderInfo, gameInfo.bGameResult);
                    Singleton<CRoleInfoManager>.instance.CalculateKDA(gameInfo);
                    return;

                case COM_GAME_TYPE.COM_MULTI_GAME_OF_PVP_MATCH:
                case COM_GAME_TYPE.COM_MULTI_GAME_OF_REWARDMATCH:
                    if (gameInfo.bIsPKAI != 2)
                    {
                        bMaxAcntNum = CLevelCfgLogicManager.GetPvpMapCommonInfo(bMapType, iLevelID).bMaxAcntNum;
                        switch (bMaxAcntNum)
                        {
                            case 2:
                                Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stOneVsOneInfo, gameInfo.bGameResult);
                                goto Label_0183;

                            case 4:
                                Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stTwoVsTwoInfo, gameInfo.bGameResult);
                                goto Label_0183;

                            case 6:
                                Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stThreeVsThreeInfo, gameInfo.bGameResult);
                                goto Label_0183;
                        }
                        break;
                    }
                    Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stVsMachineInfo, gameInfo.bGameResult);
                    return;

                case COM_GAME_TYPE.COM_MULTI_GAME_OF_ENTERTAINMENT:
                    if (gameInfo.bIsPKAI == 1)
                    {
                        Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stEntertainmentInfo, gameInfo.bGameResult);
                        Singleton<CRoleInfoManager>.instance.CalculateKDA(gameInfo);
                    }
                    return;

                default:
                    return;
            }
            if (bMaxAcntNum == 10)
            {
                Singleton<CRoleInfoManager>.instance.CalculateWins(masterRoleInfo.pvpDetail.stFiveVsFiveInfo, gameInfo.bGameResult);
            }
        Label_0183:
            Singleton<CRoleInfoManager>.instance.CalculateKDA(gameInfo);
        }

        public void SetWarmHeroAiDiff(byte aiLevel)
        {
            this.m_warmHeroAiDiffInfo = GameDataMgr.battleDynamicDifficultyDB.GetDataByKey((uint) aiLevel);
        }
    }
}

