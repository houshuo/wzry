namespace Assets.Scripts.Framework
{
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.Sound;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;

    [AutoSingleton(false)]
    public class GameFramework : MonoSingleton<GameFramework>
    {
        private Vector3 _DebugPreCamPos;
        private float _DebugPreCamTime;
        private float accum;
        private float accumTime;
        public static string AppVersion;
        public static int c_renderFPS = 30;
        public static float c_targetFrameTime = (1000f / ((float) c_renderFPS));
        public bool CreateReplayFile = true;
        public bool EditorPreviewMode;
        private int frames;
        private float frequency = 0.1f;
        public double lastRealTime;
        private float lastUpdateTime;
        private bool lockFPS_SGame = true;
        public static float m_fFps = 0f;
        private bool m_isAllSystemPrepared;
        private bool m_isBaseSystemPrepared;
        public string tongCaiKey;
        public string tongCaiKey1;

        protected void DestoryBattleSys()
        {
            Singleton<SkillIndicateSystem>.DestroyInstance();
            Singleton<SkillFuncDelegator>.DestroyInstance();
            Singleton<SkillSelectControl>.DestroyInstance();
            MonoSingleton<SceneMgr>.DestroyInstance();
            Singleton<SkillDetectionControl>.DestroyInstance();
            Singleton<GameObjMgr>.DestroyInstance();
            Singleton<SceneManagement>.DestroyInstance();
            Singleton<GameBuilder>.DestroyInstance();
            Singleton<TipProcessor>.DestroyInstance();
            Singleton<PassiveCreater<PassiveCondition, PassiveConditionAttribute>>.DestroyInstance();
            Singleton<PassiveCreater<PassiveEvent, PassiveEventAttribute>>.DestroyInstance();
            Singleton<DropItemMgr>.DestroyInstance();
            Singleton<CBattleSystem>.DestroyInstance();
            Singleton<BattleStatistic>.DestroyInstance();
            Singleton<StarSystem>.DestroyInstance();
            MonoSingleton<CameraSystem>.DestroyInstance();
            Singleton<ActorDataCenter>.DestroyInstance();
            Singleton<GamePlayerCenter>.DestroyInstance();
            Singleton<CTrainingHelper>.DestroyInstance();
            Singleton<CSurrenderSystem>.DestroyInstance();
        }

        protected void DestoryMiscSys()
        {
            Singleton<CheatCommandRegister>.DestroyInstance();
            Singleton<CheatCommandsRepository>.DestroyInstance();
            if (MonoSingleton<ConsoleWindow>.HasInstance())
            {
                MonoSingleton<ConsoleWindow>.DestroyInstance();
            }
        }

        protected void DestoryPeripherySys()
        {
            Singleton<CEquipSystem>.DestroyInstance();
            Singleton<CUIParticleSystem>.DestroyInstance();
            MonoSingleton<NewbieGuideManager>.DestroyInstance();
            Singleton<HeroChooseLogic>.DestroyInstance();
            Singleton<CTaskSys>.DestroyInstance();
            Singleton<CGuildSystem>.DestroyInstance();
            Singleton<CAchievementSystem>.DestroyInstance();
            Singleton<CExperienceCardSystem>.DestroyInstance();
            Singleton<CPurchaseSys>.DestroyInstance();
            Singleton<CMailSys>.DestroyInstance();
            Singleton<CUILoadingSystem>.DestroyInstance();
            Singleton<CRoleRegisterSys>.DestroyInstance();
            Singleton<NewbieGuideDataManager>.DestroyInstance();
            Singleton<PVESettleSys>.DestroyInstance();
            Singleton<CSettleSystem>.DestroyInstance();
            Singleton<TreasureChestMgr>.DestroyInstance();
            Singleton<CSymbolSystem>.DestroyInstance();
            Singleton<CHeroSelectNormalSystem>.DestroyInstance();
            Singleton<CSettingsSys>.DestroyInstance();
            Singleton<CLobbySystem>.DestroyInstance();
            Singleton<CQualifyingSystem>.DestroyInstance();
            Singleton<CPvPHeroShop>.DestroyInstance();
            Singleton<CShopSystem>.DestroyInstance();
            Singleton<BurnExpeditionController>.DestroyInstance();
            Singleton<CChatController>.DestroyInstance();
            Singleton<CFriendContoller>.DestroyInstance();
            Singleton<CHeroOverviewSystem>.DestroyInstance();
            Singleton<CHeroInfoSystem2>.DestroyInstance();
            Singleton<CHeroSkinBuyManager>.DestroyInstance();
            Singleton<CHeroChooseSys>.DestroyInstance();
            Singleton<CLadderSystem>.DestroyInstance();
            Singleton<CInviteSystem>.DestroyInstance();
            Singleton<CRoomSystem>.DestroyInstance();
            Singleton<CMatchingSystem>.DestroyInstance();
            Singleton<CAdventureSys>.DestroyInstance();
            Singleton<CMallSystem>.DestroyInstance();
            Singleton<CBagSystem>.DestroyInstance();
            Singleton<CRoleInfoManager>.DestroyInstance();
            Singleton<CUICommonSystem>.DestroyInstance();
            Singleton<SingleGameSettleMgr>.DestroyInstance();
            Singleton<CHeroAnimaSystem>.DestroyInstance();
            Singleton<CPaySystem>.DestroyInstance();
            MonoSingleton<IDIPSys>.DestroyInstance();
            MonoSingleton<PandroaSys>.DestroyInstance();
            Singleton<HeadIconSys>.DestroyInstance();
            MonoSingleton<BannerImageSys>.DestroyInstance();
            MonoSingleton<ShareSys>.DestroyInstance();
            MonoSingleton<NoticeSys>.DestroyInstance();
            Singleton<CAddSkillSys>.DestroyInstance();
            MonoSingleton<NobeSys>.DestroyInstance();
            MonoSingleton<VoiceSys>.DestroyInstance();
            Singleton<CPlayerInfoSystem>.DestroyInstance();
            Singleton<CMiniPlayerInfoSystem>.DestroyInstance();
            Singleton<CLoudSpeakerSys>.DestroyInstance();
            Singleton<COBSystem>.DestroyInstance();
        }

        protected void DestroyBaseSys()
        {
            Singleton<GameStateCtrl>.DestroyInstance();
            Singleton<CCheatSystem>.DestroyInstance();
            Singleton<CTextManager>.DestroyInstance();
            Singleton<CUIEventManager>.DestroyInstance();
            Singleton<CUIManager>.DestroyInstance();
            Singleton<CSoundManager>.DestroyInstance();
            Singleton<CGameObjectPool>.DestroyInstance();
            Singleton<CResourceManager>.DestroyInstance();
            Singleton<CTimerManager>.DestroyInstance();
            Singleton<EventRouter>.DestroyInstance();
            MonoSingleton<TssdkSys>.DestroyInstance();
        }

        protected void DestroyCoreSys()
        {
            Singleton<BattleLogic>.DestroyInstance();
            Singleton<LobbyLogic>.DestroyInstance();
            Singleton<GameLogic>.DestroyInstance();
            Singleton<GameInput>.DestroyInstance();
            Singleton<InputModule>.DestroyInstance();
            MonoSingleton<ActionManager>.DestroyInstance();
            MonoSingleton<GameLoader>.DestroyInstance();
            Singleton<ResourceLoader>.DestroyInstance();
            Singleton<GameEventSys>.DestroyInstance();
            Singleton<GameDataMgr>.DestroyInstance();
        }

        protected override void Init()
        {
            Screen.sleepTimeout = -1;
            AppVersion = CVersion.GetAppVersion();
            this.setTargetFrameRate();
        }

        protected void InitBaseSys()
        {
            Singleton<CTimerManager>.CreateInstance();
            Singleton<CResourceManager>.CreateInstance();
            Singleton<ResourceLoader>.GetInstance();
            Singleton<CGameObjectPool>.CreateInstance();
            Singleton<CSoundManager>.CreateInstance();
            Singleton<CUIEventManager>.CreateInstance();
            Singleton<CUIManager>.CreateInstance();
            MonoSingleton<CVersionUpdateSystem>.GetInstance();
            Singleton<CCheatSystem>.CreateInstance();
            Singleton<GameStateCtrl>.CreateInstance();
            OutlineFilter.EnableSurfaceShaderOutline(false);
            DynamicShadow.InitDefaultGlobalVariables();
        }

        protected void InitBattleSys()
        {
            BTConfig.SetBTConfig();
            Singleton<GamePlayerCenter>.CreateInstance();
            Singleton<ActorDataCenter>.CreateInstance();
            Singleton<ShenFuSystem>.CreateInstance();
            MonoSingleton<CameraSystem>.GetInstance();
            Singleton<StarSystem>.CreateInstance();
            Singleton<BattleStatistic>.CreateInstance();
            Singleton<CBattleSystem>.CreateInstance();
            Singleton<DropItemMgr>.CreateInstance();
            Singleton<PassiveCreater<PassiveEvent, PassiveEventAttribute>>.CreateInstance();
            Singleton<PassiveCreater<PassiveCondition, PassiveConditionAttribute>>.CreateInstance();
            Singleton<TipProcessor>.CreateInstance();
            Singleton<GameBuilder>.CreateInstance();
            Singleton<GameObjMgr>.CreateInstance();
            Singleton<SceneManagement>.CreateInstance();
            Singleton<SkillDetectionControl>.CreateInstance();
            MonoSingleton<SceneMgr>.GetInstance();
            Singleton<SkillSelectControl>.CreateInstance();
            Singleton<SkillFuncDelegator>.CreateInstance();
            Singleton<SkillIndicateSystem>.CreateInstance();
            Singleton<CBattleGuideManager>.CreateInstance();
            Singleton<CTrainingHelper>.CreateInstance();
            Singleton<CSurrenderSystem>.CreateInstance();
        }

        protected void InitCoreSys()
        {
            Singleton<GameEventSys>.CreateInstance();
            MonoSingleton<GameLoader>.GetInstance();
            MonoSingleton<ActionManager>.GetInstance();
            Singleton<InputModule>.CreateInstance();
            Singleton<GameInput>.GetInstance();
            Singleton<GameLogic>.CreateInstance();
            Singleton<LobbyLogic>.CreateInstance();
            Singleton<BattleLogic>.CreateInstance();
            Singleton<GameReplayModule>.CreateInstance();
            Singleton<CUICommonSystem>.CreateInstance();
        }

        protected void InitMiscSys()
        {
        }

        protected void InitPeripherySys()
        {
            GameSettings.Init();
            Singleton<CRoleInfoManager>.CreateInstance();
            Singleton<CLoginSystem>.CreateInstance();
            Singleton<CBagSystem>.CreateInstance();
            Singleton<RankingSystem>.CreateInstance();
            Singleton<SevenDayCheckSystem>.CreateInstance();
            Singleton<SettlementSystem>.CreateInstance();
            Singleton<QQVipWidget>.CreateInstance();
            Singleton<CMallSystem>.CreateInstance();
            Singleton<CAdventureSys>.CreateInstance();
            Singleton<CMatchingSystem>.CreateInstance();
            Singleton<CRoomSystem>.CreateInstance();
            Singleton<CInviteSystem>.CreateInstance();
            Singleton<CLadderSystem>.CreateInstance();
            Singleton<CHeroChooseSys>.CreateInstance();
            Singleton<CHeroOverviewSystem>.CreateInstance();
            Singleton<CHeroInfoSystem2>.CreateInstance();
            Singleton<CHeroSkinBuyManager>.CreateInstance();
            Singleton<CFriendContoller>.CreateInstance();
            Singleton<CChatController>.CreateInstance();
            Singleton<BurnExpeditionController>.CreateInstance();
            Singleton<CShopSystem>.CreateInstance();
            Singleton<CPvPHeroShop>.CreateInstance();
            Singleton<CQualifyingSystem>.CreateInstance();
            Singleton<CLobbySystem>.CreateInstance();
            Singleton<CSettingsSys>.CreateInstance();
            Singleton<CHeroSelectBaseSystem>.CreateInstance();
            Singleton<CHeroSelectNormalSystem>.CreateInstance();
            Singleton<CSymbolSystem>.CreateInstance();
            Singleton<TreasureChestMgr>.CreateInstance();
            Singleton<CSettleSystem>.CreateInstance();
            Singleton<PVESettleSys>.CreateInstance();
            Singleton<NewbieGuideDataManager>.CreateInstance();
            Singleton<CRoleRegisterSys>.CreateInstance();
            Singleton<CUILoadingSystem>.CreateInstance();
            Singleton<CMailSys>.CreateInstance();
            Singleton<CPurchaseSys>.CreateInstance();
            Singleton<CGuildSystem>.GetInstance();
            Singleton<CAchievementSystem>.GetInstance();
            Singleton<CExperienceCardSystem>.GetInstance();
            Singleton<CTaskSys>.CreateInstance();
            Singleton<HeroChooseLogic>.CreateInstance();
            MonoSingleton<NewbieGuideManager>.GetInstance();
            Singleton<SingleGameSettleMgr>.CreateInstance();
            Singleton<CPlayerInfoSystem>.CreateInstance();
            Singleton<CMiniPlayerInfoSystem>.CreateInstance();
            Singleton<CArenaSystem>.CreateInstance();
            Singleton<CNewbieAchieveSys>.CreateInstance();
            Singleton<CHeroAnimaSystem>.CreateInstance();
            Singleton<CUIParticleSystem>.CreateInstance();
            Singleton<CPaySystem>.CreateInstance();
            MonoSingleton<IDIPSys>.GetInstance();
            MonoSingleton<PandroaSys>.GetInstance();
            Singleton<HeadIconSys>.GetInstance();
            MonoSingleton<BannerImageSys>.GetInstance();
            MonoSingleton<ShareSys>.GetInstance();
            MonoSingleton<NoticeSys>.GetInstance();
            Singleton<CAddSkillSys>.GetInstance();
            MonoSingleton<NobeSys>.GetInstance();
            MonoSingleton<VoiceSys>.GetInstance();
            Singleton<CMiShuSystem>.GetInstance();
            Singleton<CUnionBattleEntrySystem>.GetInstance();
            Singleton<CUnionBattleRankSystem>.GetInstance();
            Singleton<CEquipSystem>.CreateInstance();
            Singleton<CUnionBattleEntrySystem>.CreateInstance();
            Singleton<CLoudSpeakerSys>.CreateInstance();
            Singleton<COBSystem>.CreateInstance();
            Singleton<CReplayKitSys>.CreateInstance();
            Singleton<CRecordUseSDK>.CreateInstance();
        }

        private void LateUpdate()
        {
            try
            {
                if (this.m_isAllSystemPrepared)
                {
                    Singleton<GameLogic>.GetInstance().LateUpdate();
                    Singleton<CBattleSystem>.GetInstance().LateUpdate();
                }
                try
                {
                    Singleton<CUIManager>.GetInstance().LateUpdate();
                }
                catch (Exception exception)
                {
                    object[] inParameters = new object[] { exception.Message, exception.StackTrace };
                    DebugHelper.Assert(false, "Exception Occur when CUIManager.LateUpdate, Message:{0}, Stack:{1}", inParameters);
                }
                if (this.m_isAllSystemPrepared)
                {
                    Singleton<LobbyLogic>.GetInstance().LateUpdate();
                }
            }
            catch (Exception exception2)
            {
                Singleton<CCheatSystem>.GetInstance().RecordErrorLog(string.Format("Exception Occur when GameFramework.LateUpdate, Message:{0}", exception2.Message));
                object[] objArray2 = new object[] { exception2.Message, exception2.StackTrace, Singleton<GameStateCtrl>.instance.currentStateName };
                DebugHelper.Assert(false, "Exception Occur when GameFramework.LateUpdate in state {2}, Message:{0}, Stack:{1}", objArray2);
                throw exception2;
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            object[] inParameters = new object[] { pauseStatus };
            DebugHelper.CustomLog("OnApplicationPause {0}", inParameters);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Singleton<BugLocateLogSys>.DestroyInstance();
        }

        private void OnNativeMessage(string InMessage)
        {
        }

        [DebuggerHidden]
        private IEnumerator PrepareBaseSystem(DelegateOnBaseSystemPrepareComplete delegateOnBaseSystemPrepareComplete)
        {
            return new <PrepareBaseSystem>c__Iterator4 { delegateOnBaseSystemPrepareComplete = delegateOnBaseSystemPrepareComplete, <$>delegateOnBaseSystemPrepareComplete = delegateOnBaseSystemPrepareComplete, <>f__this = this };
        }

        [DebuggerHidden]
        public IEnumerator PrepareGameSystem()
        {
            return new <PrepareGameSystem>c__Iterator5 { <>f__this = this };
        }

        public void PrepareSoundSystem(bool applySoundSettings = false)
        {
            Singleton<CSoundManager>.GetInstance().Prepare();
            Singleton<CSoundManager>.GetInstance().LoadBank("Music_Login", CSoundManager.BankType.Global);
            Singleton<CSoundManager>.GetInstance().LoadBank("Music", CSoundManager.BankType.Global);
            Singleton<CSoundManager>.GetInstance().LoadBank("Ambience", CSoundManager.BankType.Global);
            Singleton<CUIManager>.GetInstance().LoadSoundBank();
            Singleton<CSoundManager>.GetInstance().LoadBank("Audio_Control", CSoundManager.BankType.Global);
            if (applySoundSettings)
            {
                GameSettings.ApplySettings_Music();
                GameSettings.ApplySettings_Sound();
            }
        }

        public void setTargetFrameRate()
        {
            if (this.lockFPS_SGame)
            {
                Application.targetFrameRate = unityTargetFrameRate;
                base.StartCoroutine("SGame_WaitForTargetFrameRate");
            }
            else
            {
                base.StopCoroutine("SGame_WaitForTargetFrameRate");
                Application.targetFrameRate = c_renderFPS;
            }
        }

        [DebuggerHidden]
        private IEnumerator SGame_WaitForTargetFrameRate()
        {
            return new <SGame_WaitForTargetFrameRate>c__Iterator6 { <>f__this = this };
        }

        public virtual void Start()
        {
            Application.runInBackground = true;
            try
            {
                object[] inParameters = new object[] { CVersion.GetAppVersion(), CVersion.GetRevisonNumber().Trim(new char[] { ' ', '\n', '\r', '\t' }), Application.unityVersion };
                DebugHelper.CustomLog("GameFramework Start, Version:{0}.R({1}), Unity:{2}", inParameters);
            }
            catch (Exception)
            {
            }
            Singleton<BugLocateLogSys>.CreateInstance();
            Singleton<ApolloHelper>.GetInstance().EnableBugly();
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.orientation = ScreenOrientation.AutoRotation;
            GameSettings.RefreshResolution();
            AndroidJavaClass class2 = new AndroidJavaClass(ApolloConfig.GetGameUtilityString());
            if (class2 != null)
            {
                object[] args = new object[] { true };
                class2.CallStatic("EnableInput", args);
                class2.Dispose();
            }
            UnityEngine.Debug.Log("android EnableInput");
            CVersionUpdateSystem.SetIIPSServerTypeFromFile();
            this.InitBaseSys();
            Singleton<GameStateCtrl>.GetInstance().Initialize();
            Singleton<GameStateCtrl>.GetInstance().GotoState("LaunchState");
        }

        public void StartPrepareBaseSystem(DelegateOnBaseSystemPrepareComplete delegateOnBaseSystemPrepareComplete)
        {
            base.StartCoroutine(this.PrepareBaseSystem(delegateOnBaseSystemPrepareComplete));
        }

        private void Update()
        {
            if (Singleton<BattleLogic>.HasInstance() && Singleton<BattleLogic>.GetInstance().isFighting)
            {
                FPSStatistic.Update();
            }
            try
            {
                if (this.m_isBaseSystemPrepared)
                {
                    Singleton<CGameObjectPool>.GetInstance().Update();
                    Singleton<CResourceManager>.GetInstance().CustomUpdate();
                }
                if (this.m_isAllSystemPrepared)
                {
                    if (Singleton<GameReplayModule>.GetInstance().isReplay)
                    {
                        Singleton<GameReplayModule>.GetInstance().UpdateFrame();
                    }
                    else
                    {
                        Singleton<InputModule>.GetInstance().UpdateFrame();
                        Singleton<GameInput>.GetInstance().UpdateFrame();
                        Singleton<NetworkModule>.GetInstance().UpdateFrame();
                    }
                    Singleton<WatchController>.GetInstance().UpdateFrame();
                    Singleton<FrameWindow>.GetInstance().UpdateFrame();
                    Singleton<FrameSynchr>.GetInstance().UpdateFrame();
                }
            }
            catch (Exception exception)
            {
                Singleton<CCheatSystem>.GetInstance().RecordErrorLog(string.Format("Exception Occur when GameFramework.FixedUpdate, Message:{0}", exception.Message));
                object[] inParameters = new object[] { exception.Message, Singleton<GameStateCtrl>.instance.currentStateName };
                DebugHelper.Assert(false, "Exception Occur when GameFramework.FixedUpdate in state {1}, Message:{0}", inParameters);
                throw exception;
            }
            this.UpdateElse();
        }

        private void UpdateElse()
        {
            try
            {
                if (this.m_isAllSystemPrepared)
                {
                    Singleton<CBattleSystem>.GetInstance().Update();
                    Singleton<NewbieWeakGuideControl>.GetInstance().Update();
                    Singleton<CChatController>.GetInstance().Update();
                    Singleton<CFriendContoller>.GetInstance().Update();
                }
                if (Singleton<BattleLogic>.HasInstance())
                {
                    Singleton<BattleLogic>.GetInstance().Update();
                }
                Singleton<CTimerManager>.GetInstance().Update();
                Singleton<CUIManager>.GetInstance().Update();
                MonoSingleton<TssdkSys>.GetInstance().Update();
                if (!this.lockFPS_SGame)
                {
                    float num = Time.realtimeSinceStartup - this.lastUpdateTime;
                    this.accumTime += num;
                    this.accum += 1f / num;
                    this.lastUpdateTime = Time.realtimeSinceStartup;
                    this.frames++;
                    if (this.accumTime >= this.frequency)
                    {
                        m_fFps = this.accum / ((float) this.frames);
                        this.accumTime = 0f;
                        this.accum = 0f;
                        this.frames = 0;
                    }
                }
            }
            catch (Exception exception)
            {
                Singleton<CCheatSystem>.GetInstance().RecordErrorLog(string.Format("Exception Occur when GameFramework.UpdateElse, Message:{0}", exception.Message));
                object[] inParameters = new object[] { exception.Message, Singleton<GameStateCtrl>.instance.currentStateName };
                DebugHelper.Assert(false, "Exception Occur when GameFramework.UpdateElse in State: {1}, Message:{0}", inParameters);
                throw exception;
            }
        }

        public bool LockFPS_SGame
        {
            get
            {
                return this.lockFPS_SGame;
            }
            set
            {
                if (value != this.lockFPS_SGame)
                {
                    this.lockFPS_SGame = value;
                    this.setTargetFrameRate();
                }
            }
        }

        public static int unityTargetFrameRate
        {
            get
            {
                int num = c_renderFPS;
                return 60;
            }
        }

        [CompilerGenerated]
        private sealed class <PrepareBaseSystem>c__Iterator4 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameFramework.DelegateOnBaseSystemPrepareComplete <$>delegateOnBaseSystemPrepareComplete;
            internal GameFramework <>f__this;
            internal GameFramework.DelegateOnBaseSystemPrepareComplete delegateOnBaseSystemPrepareComplete;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        Singleton<EventRouter>.CreateInstance();
                        Singleton<CTextManager>.CreateInstance();
                        Singleton<CTextManager>.GetInstance().LoadLocalText();
                        this.$current = null;
                        this.$PC = 1;
                        goto Label_00B1;

                    case 1:
                        Singleton<NetworkModule>.CreateInstance();
                        MonoSingleton<TssdkSys>.GetInstance();
                        this.$current = null;
                        this.$PC = 2;
                        goto Label_00B1;

                    case 2:
                        Singleton<FrameSynchr>.CreateInstance();
                        this.$current = null;
                        this.$PC = 3;
                        goto Label_00B1;

                    case 3:
                        this.<>f__this.m_isBaseSystemPrepared = true;
                        if (this.delegateOnBaseSystemPrepareComplete != null)
                        {
                            this.delegateOnBaseSystemPrepareComplete();
                        }
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_00B1:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <PrepareGameSystem>c__Iterator5 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameFramework <>f__this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        Singleton<GameDataMgr>.CreateInstance();
                        this.$current = this.<>f__this.StartCoroutine(Singleton<GameDataMgr>.GetInstance().LoadDataBin());
                        this.$PC = 1;
                        goto Label_01A9;

                    case 1:
                        Singleton<CTextManager>.GetInstance().LoadText(GameDataMgr.s_textDatabin);
                        Singleton<CFriendContoller>.instance.model.LoadPreconfigVerifyContentList();
                        Singleton<CFriendContoller>.instance.model.LoadIntimacyConfig();
                        Singleton<CChatController>.CreateInstance();
                        Singleton<CChatController>.instance.model.Load_HeroSelect_ChatTemplate();
                        Singleton<CTaskSys>.instance.model.Load_Share_task();
                        Singleton<CTaskSys>.instance.model.Load_Task_Tab_String();
                        Singleton<InBattleMsgMgr>.instance.ParseCfgData();
                        Singleton<CTaskSys>.instance.model.ParseLevelRewardData();
                        KillNotify.LoadConfig();
                        this.<>f__this.PrepareSoundSystem(false);
                        this.$current = null;
                        this.$PC = 2;
                        goto Label_01A9;

                    case 2:
                        this.<>f__this.InitCoreSys();
                        this.$current = null;
                        this.$PC = 3;
                        goto Label_01A9;

                    case 3:
                        if (this.<>f__this.EditorPreviewMode)
                        {
                            break;
                        }
                        this.<>f__this.InitPeripherySys();
                        this.$current = null;
                        this.$PC = 4;
                        goto Label_01A9;

                    case 4:
                        break;

                    case 5:
                        this.<>f__this.InitMiscSys();
                        this.$current = null;
                        this.$PC = 6;
                        goto Label_01A9;

                    case 6:
                        Singleton<GameReplayModule>.GetInstance().Reset();
                        NetworkAccelerator.Init();
                        this.<>f__this.m_isAllSystemPrepared = true;
                        this.$PC = -1;
                        goto Label_01A7;

                    default:
                        goto Label_01A7;
                }
                this.<>f__this.InitBattleSys();
                this.$current = null;
                this.$PC = 5;
                goto Label_01A9;
            Label_01A7:
                return false;
            Label_01A9:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <SGame_WaitForTargetFrameRate>c__Iterator6 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GameFramework <>f__this;
            internal double <dt>__1;
            internal float <fdelta>__4;
            internal double <sleepTime>__2;
            internal int <sleepTimeInt>__3;
            internal double <t>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        break;

                    case 1:
                        this.<t>__0 = Time.realtimeSinceStartup;
                        this.<dt>__1 = (this.<t>__0 - this.<>f__this.lastRealTime) * 1000.0;
                        this.<sleepTime>__2 = GameFramework.c_targetFrameTime - this.<dt>__1;
                        this.<sleepTimeInt>__3 = Mathf.Clamp((int) this.<sleepTime>__2, 0, GameFramework.c_renderFPS);
                        if (this.<sleepTimeInt>__3 > 0)
                        {
                            Thread.Sleep(this.<sleepTimeInt>__3);
                        }
                        this.<>f__this.lastRealTime = Time.realtimeSinceStartup;
                        this.<fdelta>__4 = Time.realtimeSinceStartup - this.<>f__this.lastUpdateTime;
                        this.<>f__this.accumTime += this.<fdelta>__4;
                        this.<>f__this.accum += 1f / this.<fdelta>__4;
                        this.<>f__this.lastUpdateTime = Time.realtimeSinceStartup;
                        this.<>f__this.frames++;
                        if (this.<>f__this.accumTime >= this.<>f__this.frequency)
                        {
                            GameFramework.m_fFps = this.<>f__this.accum / ((float) this.<>f__this.frames);
                            this.<>f__this.accumTime = 0f;
                            this.<>f__this.accum = 0f;
                            this.<>f__this.frames = 0;
                        }
                        break;

                    default:
                        goto Label_0199;
                }
                this.$current = new WaitForEndOfFrame();
                this.$PC = 1;
                return true;
                this.$PC = -1;
            Label_0199:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        public delegate void DelegateOnBaseSystemPrepareComplete();
    }
}

