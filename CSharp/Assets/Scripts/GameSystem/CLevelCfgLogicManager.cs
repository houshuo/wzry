namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using CSProtocol;
    using ResData;
    using System;

    public class CLevelCfgLogicManager
    {
        public static SLevelContext CreatePveLevelContext(SCPKG_STARTSINGLEGAMERSP InMessage)
        {
            SLevelContext context = new SLevelContext();
            context.SetGameType((COM_GAME_TYPE) InMessage.bGameType);
            ResLevelCfgInfo pveMapInfo = GetPveMapInfo(InMessage.bGameType, InMessage.iLevelId);
            if (InMessage.bGameType == 2)
            {
                context.InitPveData(pveMapInfo, 1);
                if (pveMapInfo.bGuideLevelSubType == 0)
                {
                    context.m_isMobaType = true;
                    return context;
                }
                if (pveMapInfo.bGuideLevelSubType == 1)
                {
                    context.m_isMobaType = false;
                }
                return context;
            }
            if (InMessage.bGameType == 0)
            {
                context.InitPveData(pveMapInfo, Singleton<CAdventureSys>.instance.currentDifficulty);
                return context;
            }
            if (InMessage.bGameType == 7)
            {
                context.InitPveData(pveMapInfo, 1);
                return context;
            }
            if (InMessage.bGameType == 8)
            {
                context.InitPveData(pveMapInfo, 1);
                return context;
            }
            if ((InMessage.bGameType != 3) && (InMessage.bGameType == 1))
            {
                byte bMapType = InMessage.stGameParam.stSingleGameRspOfCombat.bMapType;
                uint dwMapId = InMessage.stGameParam.stSingleGameRspOfCombat.dwMapId;
                context = CreatePvpLevelContext(bMapType, dwMapId, (COM_GAME_TYPE) InMessage.bGameType, 1);
                context.m_isWarmBattle = Convert.ToBoolean(InMessage.stGameParam.stSingleGameRspOfCombat.bIsWarmBattle);
                context.SetWarmHeroAiDiff(InMessage.stGameParam.stSingleGameRspOfCombat.bAILevel);
            }
            return context;
        }

        public static SLevelContext CreatePvpLevelContext(byte mapType, uint mapID, COM_GAME_TYPE GameType, int difficult)
        {
            SLevelContext context = new SLevelContext();
            context.SetGameType(GameType);
            context.m_mapType = mapType;
            context.m_levelDifficulty = difficult;
            context.m_horizonEnableMethod = Horizon.EnableMethod.EnableAll;
            ResDT_LevelCommonInfo pvpMapCommonInfo = GetPvpMapCommonInfo(mapType, mapID);
            context.InitPvpData(pvpMapCommonInfo, mapID);
            if (mapType != 1)
            {
                if (mapType == 3)
                {
                    return context;
                }
                if (mapType == 4)
                {
                    ResEntertainmentLevelInfo dataByKey = GameDataMgr.entertainLevelDatabin.GetDataByKey(mapID);
                    context.m_entertainmentSubMapType = dataByKey.bEntertainmentSubType;
                    return context;
                }
                if (mapType == 5)
                {
                    ResRewardMatchLevelInfo info3 = GameDataMgr.uinionBattleLevelDatabin.GetDataByKey(mapID);
                    context.m_SecondName = info3.szMatchName;
                    return context;
                }
                if (mapType == 2)
                {
                }
            }
            return context;
        }

        public static ResLevelCfgInfo GetPveMapInfo(byte gameType, int levelID)
        {
            ResLevelCfgInfo dataByKey = null;
            if (gameType == 2)
            {
                dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) levelID);
            }
            else if (gameType == 0)
            {
                dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long) levelID);
            }
            else if (gameType == 7)
            {
                dataByKey = GameDataMgr.burnMap.GetDataByKey((long) Singleton<BurnExpeditionController>.GetInstance().model.Get_LevelID(Singleton<BurnExpeditionController>.GetInstance().model.curSelect_LevelIndex));
            }
            else if (gameType == 8)
            {
                dataByKey = GameDataMgr.arenaLevelDatabin.GetDataByKey((long) levelID);
            }
            else if (gameType == 3)
            {
            }
            if (dataByKey != null)
            {
                return dataByKey;
            }
            if (gameType != 1)
            {
            }
            return new ResLevelCfgInfo();
        }

        public static ResDT_LevelCommonInfo GetPvpMapCommonInfo(byte mapType, uint mapId)
        {
            ResDT_LevelCommonInfo stLevelCommonInfo = new ResDT_LevelCommonInfo();
            if (mapType == 1)
            {
                ResAcntBattleLevelInfo dataByKey = GameDataMgr.pvpLevelDatabin.GetDataByKey(mapId);
                DebugHelper.Assert(dataByKey != null);
                stLevelCommonInfo = dataByKey.stLevelCommonInfo;
            }
            else if (mapType == 3)
            {
                ResRankLevelInfo info3 = GameDataMgr.rankLevelDatabin.GetDataByKey(mapId);
                DebugHelper.Assert(info3 != null);
                stLevelCommonInfo = info3.stLevelCommonInfo;
            }
            else if (mapType == 4)
            {
                ResEntertainmentLevelInfo info4 = GameDataMgr.entertainLevelDatabin.GetDataByKey(mapId);
                DebugHelper.Assert(info4 != null);
                stLevelCommonInfo = info4.stLevelCommonInfo;
            }
            else if (mapType == 5)
            {
                ResRewardMatchLevelInfo info5 = GameDataMgr.uinionBattleLevelDatabin.GetDataByKey(mapId);
                DebugHelper.Assert(info5 != null);
                stLevelCommonInfo = info5.stLevelCommonInfo;
            }
            else if (mapType == 2)
            {
                ResCounterPartLevelInfo info6 = GameDataMgr.cpLevelDatabin.GetDataByKey(mapId);
                DebugHelper.Assert(info6 != null);
                stLevelCommonInfo = info6.stLevelCommonInfo;
            }
            if (stLevelCommonInfo == null)
            {
                stLevelCommonInfo = new ResDT_LevelCommonInfo();
            }
            return stLevelCommonInfo;
        }
    }
}

