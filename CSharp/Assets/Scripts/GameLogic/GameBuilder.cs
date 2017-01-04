namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class GameBuilder : Singleton<GameBuilder>
    {
        private List<KeyValuePair<string, string>> m_eventsLoadingTime = new List<KeyValuePair<string, string>>();
        private float m_fLoadingTime;
        public int m_iMapId;
        public COM_GAME_TYPE m_kGameType = COM_GAME_TYPE.COM_GAME_TYPE_MAX;

        public void EndGame()
        {
            if (Singleton<BattleLogic>.instance.isRuning)
            {
                try
                {
                    DebugHelper.CustomLog("Prepare GameBuilder EndGame");
                }
                catch (Exception)
                {
                }
                Singleton<LobbyLogic>.GetInstance().StopGameEndTimer();
                Singleton<LobbyLogic>.GetInstance().StopSettleMsgTimer();
                Singleton<LobbyLogic>.GetInstance().StopSettlePanelTimer();
                MonoSingleton<GameLoader>.instance.AdvanceStopLoad();
                Singleton<WatchController>.GetInstance().Stop();
                Singleton<CBattleGuideManager>.GetInstance().resetPause();
                if (this.gameInfo != null)
                {
                    this.gameInfo.EndGame();
                }
                string openID = Singleton<ApolloHelper>.GetInstance().GetOpenID();
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("GameBuild.EndGame", null, true);
                List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()),
                    new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
                    new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
                    new KeyValuePair<string, string>("openid", openID),
                    new KeyValuePair<string, string>("GameType", this.m_kGameType.ToString()),
                    new KeyValuePair<string, string>("MapID", this.m_iMapId.ToString()),
                    new KeyValuePair<string, string>("LoadingTime", this.m_fLoadingTime.ToString())
                };
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_LoadingBattle", events, true);
                List<KeyValuePair<string, string>> list2 = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()),
                    new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
                    new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
                    new KeyValuePair<string, string>("openid", openID),
                    new KeyValuePair<string, string>("totaltime", Singleton<CHeroSelectBaseSystem>.instance.m_fOpenHeroSelectForm.ToString()),
                    new KeyValuePair<string, string>("gameType", this.m_kGameType.ToString()),
                    new KeyValuePair<string, string>("role_list", string.Empty),
                    new KeyValuePair<string, string>("errorCode", string.Empty),
                    new KeyValuePair<string, string>("error_msg", string.Empty)
                };
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Login_EnterGame", list2, true);
                float num = Singleton<FrameSynchr>.GetInstance().LogicFrameTick * 0.001f;
                List<KeyValuePair<string, string>> list3 = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()),
                    new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
                    new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
                    new KeyValuePair<string, string>("openid", openID),
                    new KeyValuePair<string, string>("GameType", this.m_kGameType.ToString()),
                    new KeyValuePair<string, string>("MapID", this.m_iMapId.ToString()),
                    new KeyValuePair<string, string>("Max_FPS", Singleton<CBattleSystem>.GetInstance().m_MaxBattleFPS.ToString()),
                    new KeyValuePair<string, string>("Min_FPS", Singleton<CBattleSystem>.GetInstance().m_MinBattleFPS.ToString())
                };
                float num2 = -1f;
                if (Singleton<CBattleSystem>.GetInstance().m_BattleFPSCount > 0f)
                {
                    num2 = Singleton<CBattleSystem>.GetInstance().m_AveBattleFPS / Singleton<CBattleSystem>.GetInstance().m_BattleFPSCount;
                }
                list3.Add(new KeyValuePair<string, string>("Avg_FPS", num2.ToString()));
                list3.Add(new KeyValuePair<string, string>("Ab_FPS_time", Singleton<BattleLogic>.GetInstance().m_Ab_FPS_time.ToString()));
                list3.Add(new KeyValuePair<string, string>("Abnormal_FPS", Singleton<BattleLogic>.GetInstance().m_Abnormal_FPS_Count.ToString()));
                list3.Add(new KeyValuePair<string, string>("Ab_4FPS_time", Singleton<BattleLogic>.GetInstance().m_Ab_4FPS_time.ToString()));
                list3.Add(new KeyValuePair<string, string>("Abnormal_4FPS", Singleton<BattleLogic>.GetInstance().m_Abnormal_4FPS_Count.ToString()));
                list3.Add(new KeyValuePair<string, string>("Min_Ping", Singleton<FrameSynchr>.instance.m_MinPing.ToString()));
                list3.Add(new KeyValuePair<string, string>("Max_Ping", Singleton<FrameSynchr>.instance.m_MaxPing.ToString()));
                list3.Add(new KeyValuePair<string, string>("Avg_Ping", Singleton<FrameSynchr>.instance.m_AvePing.ToString()));
                list3.Add(new KeyValuePair<string, string>("Abnormal_Ping", Singleton<FrameSynchr>.instance.m_Abnormal_PingCount.ToString()));
                list3.Add(new KeyValuePair<string, string>("Ping300", Singleton<FrameSynchr>.instance.m_ping300Count.ToString()));
                list3.Add(new KeyValuePair<string, string>("Ping150to300", Singleton<FrameSynchr>.instance.m_ping150to300.ToString()));
                list3.Add(new KeyValuePair<string, string>("Ping150", Singleton<FrameSynchr>.instance.m_ping150.ToString()));
                list3.Add(new KeyValuePair<string, string>("LostpingCount", Singleton<FrameSynchr>.instance.m_pingLost.ToString()));
                list3.Add(new KeyValuePair<string, string>("Battle_Time", num.ToString()));
                list3.Add(new KeyValuePair<string, string>("BattleSvr_Reconnect", Singleton<NetworkModule>.GetInstance().m_GameReconnetCount.ToString()));
                list3.Add(new KeyValuePair<string, string>("GameSvr_Reconnect", Singleton<NetworkModule>.GetInstance().m_lobbyReconnetCount.ToString()));
                list3.Add(new KeyValuePair<string, string>("music", GameSettings.EnableMusic.ToString()));
                list3.Add(new KeyValuePair<string, string>("quality", GameSettings.RenderQuality.ToString()));
                list3.Add(new KeyValuePair<string, string>("status", "1"));
                int num3 = 0;
                if (MonoSingleton<VoiceSys>.GetInstance().UseSpeak && MonoSingleton<VoiceSys>.GetInstance().UseMic)
                {
                    num3 = 2;
                }
                else if (MonoSingleton<VoiceSys>.GetInstance().UseSpeak)
                {
                    num3 = 1;
                }
                list3.Add(new KeyValuePair<string, string>("Mic", num3.ToString()));
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_PVPBattle_Summary", list3, true);
                List<KeyValuePair<string, string>> list4 = new List<KeyValuePair<string, string>>();
                float num4 = Singleton<BattleLogic>.GetInstance().m_fAveFPS / ((float) Singleton<BattleLogic>.GetInstance().m_fpsCount);
                list4.Add(new KeyValuePair<string, string>("AveFPS", num4.ToString()));
                list4.Add(new KeyValuePair<string, string>("<10FPSCount", Singleton<BattleLogic>.GetInstance().m_fpsCunt10.ToString()));
                list4.Add(new KeyValuePair<string, string>("<18FPSCount", Singleton<BattleLogic>.GetInstance().m_fpsCunt18.ToString()));
                list4.Add(new KeyValuePair<string, string>("Quality_Mode", GameSettings.ModelLOD.ToString()));
                list4.Add(new KeyValuePair<string, string>("Quality_Particle", GameSettings.ParticleLOD.ToString()));
                if (Singleton<BattleLogic>.GetInstance().GetCurLvelContext() != null)
                {
                    list4.Add(new KeyValuePair<string, string>("MapId", Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_mapID.ToString()));
                }
                if (this.m_eventsLoadingTime != null)
                {
                    for (int i = 0; i < this.m_eventsLoadingTime.Count; i++)
                    {
                        KeyValuePair<string, string> item = this.m_eventsLoadingTime[i];
                        list4.Add(item);
                    }
                }
                Singleton<BeaconHelper>.GetInstance().EventBase(ref list4);
                if (num4 >= 25f)
                {
                    Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Event_FPS_ave >= 25_" + Singleton<BattleLogic>.GetInstance().GetLevelTypeDescription(), list4, true);
                }
                else if ((num4 >= 20f) && (num4 < 25f))
                {
                    Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Event_FPS_ave >= 20_" + Singleton<BattleLogic>.GetInstance().GetLevelTypeDescription(), list4, true);
                }
                else if ((num4 >= 18f) && (num4 < 20f))
                {
                    Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Event_FPS_ave >= 18_" + Singleton<BattleLogic>.GetInstance().GetLevelTypeDescription(), list4, true);
                }
                else if ((num4 >= 15f) && (num4 < 18f))
                {
                    Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Event_FPS_ave >= 15_" + Singleton<BattleLogic>.GetInstance().GetLevelTypeDescription(), list4, true);
                }
                else if ((num4 >= 10f) && (num4 < 15f))
                {
                    Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Event_FPS_ave >= 10_" + Singleton<BattleLogic>.GetInstance().GetLevelTypeDescription(), list4, true);
                }
                else
                {
                    Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Event_FPS_ave < 10_" + Singleton<BattleLogic>.GetInstance().GetLevelTypeDescription(), list4, true);
                }
                float num6 = ((float) Singleton<BattleLogic>.GetInstance().m_fpsCunt10) / ((float) Singleton<BattleLogic>.GetInstance().m_fpsCount);
                int num7 = Mathf.CeilToInt((num6 * 100f) / 10f) * 10;
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Event_FPS_<=10_Percent:" + num7.ToString() + "%", list4, true);
                float num8 = ((float) (Singleton<BattleLogic>.GetInstance().m_fpsCunt18 + Singleton<BattleLogic>.GetInstance().m_fpsCunt10)) / ((float) Singleton<BattleLogic>.GetInstance().m_fpsCount);
                int num9 = Mathf.CeilToInt((num8 * 100f) / 10f) * 10;
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Event_FPS<=18_Percent:" + num9.ToString() + "%", list4, true);
                this.m_eventsLoadingTime.Clear();
                Singleton<FrameSynchr>.instance.ReportPingToBeacon();
                try
                {
                    CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1388);
                    msg.stPkgData.stCltPerformance.iMapID = this.m_iMapId;
                    msg.stPkgData.stCltPerformance.iPlayerCnt = Singleton<GamePlayerCenter>.instance.GetAllPlayers().Count;
                    msg.stPkgData.stCltPerformance.chModelLOD = (sbyte) GameSettings.ModelLOD;
                    msg.stPkgData.stCltPerformance.chParticleLOD = (sbyte) GameSettings.ParticleLOD;
                    msg.stPkgData.stCltPerformance.chCameraHeight = (sbyte) GameSettings.CameraHeight;
                    msg.stPkgData.stCltPerformance.chEnableOutline = !GameSettings.EnableOutline ? ((sbyte) 0) : ((sbyte) 1);
                    msg.stPkgData.stCltPerformance.iFps10PercentNum = num7;
                    msg.stPkgData.stCltPerformance.iFps18PercentNum = num9;
                    msg.stPkgData.stCltPerformance.iAveFps = (int) Singleton<CBattleSystem>.GetInstance().m_AveBattleFPS;
                    msg.stPkgData.stCltPerformance.iPingAverage = Singleton<FrameSynchr>.instance.m_PingAverage;
                    msg.stPkgData.stCltPerformance.iPingVariance = Singleton<FrameSynchr>.instance.m_PingVariance;
                    Utility.StringToByteArray(SystemInfo.deviceModel, ref msg.stPkgData.stCltPerformance.szDeviceModel);
                    Utility.StringToByteArray(SystemInfo.graphicsDeviceName, ref msg.stPkgData.stCltPerformance.szGPUName);
                    msg.stPkgData.stCltPerformance.iCpuCoreNum = SystemInfo.processorCount;
                    msg.stPkgData.stCltPerformance.iSysMemorySize = SystemInfo.systemMemorySize;
                    msg.stPkgData.stCltPerformance.iAvailMemory = DeviceCheckSys.GetAvailMemory();
                    Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                }
                catch (Exception exception)
                {
                    Debug.Log(exception.Message);
                }
                MonoSingleton<DialogueProcessor>.GetInstance().Uninit();
                Singleton<TipProcessor>.GetInstance().Uninit();
                Singleton<LobbyLogic>.instance.inMultiRoom = false;
                Singleton<LobbyLogic>.instance.inMultiGame = false;
                Singleton<BattleLogic>.GetInstance().isRuning = false;
                Singleton<BattleLogic>.GetInstance().isFighting = false;
                Singleton<BattleLogic>.GetInstance().isGameOver = false;
                Singleton<BattleLogic>.GetInstance().isWaitMultiStart = false;
                Singleton<NetworkModule>.GetInstance().CloseGameServerConnect(true);
                Singleton<ShenFuSystem>.instance.ClearAll();
                MonoSingleton<ActionManager>.GetInstance().ForceStop();
                Singleton<GameObjMgr>.GetInstance().ClearActor();
                Singleton<SceneManagement>.GetInstance().Clear();
                MonoSingleton<SceneMgr>.GetInstance().ClearAll();
                Singleton<GamePlayerCenter>.GetInstance().ClearAllPlayers();
                Singleton<ActorDataCenter>.instance.ClearHeroServerData();
                Singleton<FrameSynchr>.GetInstance().ResetSynchr();
                Singleton<GameReplayModule>.GetInstance().OnGameEnd();
                Singleton<BattleLogic>.GetInstance().ResetBattleSystem();
                ActionManager.Instance.frameMode = false;
                if (!Singleton<GameStateCtrl>.instance.isLobbyState)
                {
                    DebugHelper.CustomLog("GotoLobbyState by EndGame");
                    Singleton<GameStateCtrl>.GetInstance().GotoState("LobbyState");
                }
                Singleton<BattleSkillHudControl>.DestroyInstance();
                this.m_kGameType = COM_GAME_TYPE.COM_GAME_TYPE_MAX;
                this.m_iMapId = 0;
                Singleton<BattleStatistic>.instance.PostEndGame();
                try
                {
                    FrameTracer.Destroy();
                    DebugHelper.CustomLog("Finish GameBuilder EndGame");
                }
                catch (Exception)
                {
                }
            }
        }

        private void OnGameLoadComplete()
        {
            if (!Singleton<BattleLogic>.instance.isRuning)
            {
                DebugHelper.Assert(false, "都没有在游戏局内，何来的游戏加载完成");
            }
            else
            {
                this.gameInfo.PostBeginPlay();
                this.m_fLoadingTime = Time.time - this.m_fLoadingTime;
                if (MonoSingleton<Reconnection>.GetInstance().g_fBeginReconnectTime > 0f)
                {
                    List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>>();
                    float num = Time.time - MonoSingleton<Reconnection>.GetInstance().g_fBeginReconnectTime;
                    events.Add(new KeyValuePair<string, string>("ReconnectTime", num.ToString()));
                    MonoSingleton<Reconnection>.GetInstance().g_fBeginReconnectTime = -1f;
                    Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Reconnet_IntoGame", events, true);
                }
            }
        }

        private void onGameLoadProgress(float progress)
        {
            if (this.gameInfo != null)
            {
                this.gameInfo.OnLoadingProgress(progress);
            }
        }

        public void RestoreGame()
        {
        }

        public GameInfoBase StartGame(GameContextBase InGameContext)
        {
            DebugHelper.Assert(InGameContext != null);
            if (InGameContext == null)
            {
                return null;
            }
            if (Singleton<BattleLogic>.instance.isRuning)
            {
                return null;
            }
            GameSettings.DecideDynamicParticleLOD();
            Singleton<CHeroSelectBaseSystem>.instance.m_fOpenHeroSelectForm = Time.time - Singleton<CHeroSelectBaseSystem>.instance.m_fOpenHeroSelectForm;
            this.m_fLoadingTime = Time.time;
            this.m_eventsLoadingTime.Clear();
            ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
            DebugHelper.Assert(accountInfo != null, "account info is null");
            this.m_iMapId = InGameContext.levelContext.m_mapID;
            this.m_kGameType = InGameContext.levelContext.GetGameType();
            this.m_eventsLoadingTime.Add(new KeyValuePair<string, string>("OpenID", (accountInfo == null) ? "0" : accountInfo.OpenId));
            this.m_eventsLoadingTime.Add(new KeyValuePair<string, string>("LevelID", InGameContext.levelContext.m_mapID.ToString()));
            this.m_eventsLoadingTime.Add(new KeyValuePair<string, string>("isPVPLevel", InGameContext.levelContext.IsMobaMode().ToString()));
            this.m_eventsLoadingTime.Add(new KeyValuePair<string, string>("isPVPMode", InGameContext.levelContext.IsMobaMode().ToString()));
            this.m_eventsLoadingTime.Add(new KeyValuePair<string, string>("bLevelNo", InGameContext.levelContext.m_levelNo.ToString()));
            Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("GameBuilder.StartGame", this.m_eventsLoadingTime, true);
            if (InGameContext.levelContext.IsGameTypeBurning())
            {
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Burning Game Started", null, true);
            }
            Singleton<BattleLogic>.GetInstance().isRuning = true;
            Singleton<BattleLogic>.GetInstance().isFighting = false;
            Singleton<BattleLogic>.GetInstance().isGameOver = false;
            Singleton<BattleLogic>.GetInstance().isWaitMultiStart = false;
            ActionManager.Instance.frameMode = true;
            MonoSingleton<ActionManager>.GetInstance().ForceStop();
            Singleton<GameObjMgr>.GetInstance().ClearActor();
            Singleton<SceneManagement>.GetInstance().Clear();
            MonoSingleton<SceneMgr>.GetInstance().ClearAll();
            MonoSingleton<GameLoader>.GetInstance().ResetLoader();
            InGameContext.PrepareStartup();
            if (!MonoSingleton<GameFramework>.instance.EditorPreviewMode)
            {
                DebugHelper.Assert(InGameContext.levelContext != null);
                DebugHelper.Assert(!string.IsNullOrEmpty(InGameContext.levelContext.m_levelDesignFileName));
                if (string.IsNullOrEmpty(InGameContext.levelContext.m_levelArtistFileName))
                {
                    MonoSingleton<GameLoader>.instance.AddLevel(InGameContext.levelContext.m_levelDesignFileName);
                }
                else
                {
                    MonoSingleton<GameLoader>.instance.AddDesignSerializedLevel(InGameContext.levelContext.m_levelDesignFileName);
                    MonoSingleton<GameLoader>.instance.AddArtistSerializedLevel(InGameContext.levelContext.m_levelArtistFileName);
                }
                MonoSingleton<GameLoader>.instance.AddSoundBank("Effect_Common");
                MonoSingleton<GameLoader>.instance.AddSoundBank("System_Voice");
            }
            GameInfoBase base2 = InGameContext.CreateGameInfo();
            DebugHelper.Assert(base2 != null, "can't create game logic object!");
            this.gameInfo = base2;
            base2.PreBeginPlay();
            Singleton<BattleLogic>.instance.m_GameInfo = this.gameInfo;
            Singleton<TreasureChestMgr>.instance.Reset(InGameContext.levelContext, InGameContext.rewardCount);
            try
            {
                object[] inParameters = new object[] { InGameContext.levelContext.IsMobaMode(), InGameContext.levelContext.m_mapID, InGameContext.levelContext.GetGameType(), InGameContext.levelContext.m_levelName, InGameContext.levelContext.IsMobaMode(), InGameContext.levelContext.GetSelectHeroType(), InGameContext.levelContext.m_pveLevelType };
                DebugHelper.CustomLog("GameBuilder Start Game: ispvplevel={0} ispvpmode={4} levelid={1} leveltype={6} levelname={3} Gametype={2} pick={5}", inParameters);
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                if (hostPlayer != null)
                {
                    object[] objArray2 = new object[] { hostPlayer.PlayerId, hostPlayer.Name };
                    DebugHelper.CustomLog("HostPlayer player id={1} name={2} ", objArray2);
                }
            }
            catch (Exception)
            {
            }
            if (!MonoSingleton<GameFramework>.instance.EditorPreviewMode)
            {
                MonoSingleton<GameLoader>.GetInstance().Load(new GameLoader.LoadProgressDelegate(this.onGameLoadProgress), new GameLoader.LoadCompleteDelegate(this.OnGameLoadComplete));
                MonoSingleton<VoiceSys>.GetInstance().HeroSelectTobattle();
                Singleton<GameStateCtrl>.GetInstance().GotoState("LoadingState");
            }
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            return base2;
        }

        public void StoreGame()
        {
        }

        public GameInfoBase gameInfo { get; private set; }
    }
}

