namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [MessageHandlerClass]
    public class ActivitySys : Singleton<ActivitySys>
    {
        private DictionaryView<uint, Activity> _actvDict;
        private CampaignForm _campaignForm;
        private int _checkTimer;
        private bool _firstLoad = true;
        private int _refreshTimer;
        private bool _rewardHasSpecial;
        private ListView<RewardList> _rewardListQueue = new ListView<RewardList>();
        private int _rewardQueueIndex = -1;
        private int _rewardShowIndex = -1;
        private RewardList _rewardShowList;
        [CompilerGenerated]
        private static Func<Activity, bool> <>f__am$cacheD;
        [CompilerGenerated]
        private static Func<Activity, bool> <>f__am$cacheE;
        public const int CHECK_TIMER_CYCLE = 0x3e8;
        public const long GAME_OVER_TIME = 0xf45c2700L;
        private bool m_bShareTask;
        public static string SpriteRootDir = "UGUI/Sprite/Dynamic/Activity/";
        public const string TODAY_LOGIN_SHOW_ACTIVITY_CNT = "TODAY_LOGIN_SHOW_ACTIVITY_CNT";

        public event StateChangeDelegate OnStateChange;

        internal void _NotifyStateChanged()
        {
            if (this.OnStateChange != null)
            {
                this.OnStateChange();
            }
        }

        public bool CheckReadyForDot(RES_WEAL_ENTRANCE_TYPE entry)
        {
            if (this._actvDict != null)
            {
                DictionaryView<uint, Activity>.Enumerator enumerator = this._actvDict.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, Activity> current = enumerator.Current;
                    if (current.Value.Entrance == entry)
                    {
                        KeyValuePair<uint, Activity> pair2 = enumerator.Current;
                        if (pair2.Value.ReadyForDot)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void Clear()
        {
            this.m_bShareTask = false;
            this.OnCloseCampaignForm(null);
            if (this._actvDict != null)
            {
                DictionaryView<uint, Activity>.Enumerator enumerator = this._actvDict.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, Activity> current = enumerator.Current;
                    current.Value.Clear();
                }
                this._actvDict = null;
            }
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._checkTimer);
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._refreshTimer);
        }

        private void CreateStatic()
        {
            if (this._actvDict == null)
            {
                this._actvDict = new DictionaryView<uint, Activity>();
            }
            DictionaryView<uint, ResWealCheckIn>.Enumerator enumerator = GameDataMgr.wealCheckInDict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, ResWealCheckIn> current = enumerator.Current;
                this.TryAdd(new CheckInActivity(this, current.Value));
            }
            DictionaryView<uint, ResWealFixedTime>.Enumerator enumerator2 = GameDataMgr.wealFixtimeDict.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                KeyValuePair<uint, ResWealFixedTime> pair2 = enumerator2.Current;
                this.TryAdd(new FixTimeActivity(this, pair2.Value));
            }
            DictionaryView<uint, ResWealMultiple>.Enumerator enumerator3 = GameDataMgr.wealMultipleDict.GetEnumerator();
            while (enumerator3.MoveNext())
            {
                KeyValuePair<uint, ResWealMultiple> pair3 = enumerator3.Current;
                this.TryAdd(new MultiGainActivity(this, pair3.Value));
            }
            DictionaryView<uint, ResCltWealExchange>.Enumerator enumerator4 = GameDataMgr.wealExchangeDict.GetEnumerator();
            while (enumerator4.MoveNext())
            {
                KeyValuePair<uint, ResCltWealExchange> pair4 = enumerator4.Current;
                this.TryAdd(new ExchangeActivity(this, pair4.Value));
            }
            DictionaryView<uint, ResWealPointExchange>.Enumerator enumerator5 = GameDataMgr.wealPointExchangeDict.GetEnumerator();
            while (enumerator5.MoveNext())
            {
                KeyValuePair<uint, ResWealPointExchange> pair5 = enumerator5.Current;
                this.TryAdd(new PointsExchangeActivity(this, pair5.Value));
            }
            DictionaryView<uint, ResWealCondition>.Enumerator enumerator6 = GameDataMgr.wealConditionDict.GetEnumerator();
            while (enumerator6.MoveNext())
            {
                KeyValuePair<uint, ResWealCondition> pair6 = enumerator6.Current;
                this.TryAdd(new ExeTaskActivity(this, pair6.Value));
            }
            DictionaryView<uint, ResWealText>.Enumerator enumerator7 = GameDataMgr.wealNoticeDict.GetEnumerator();
            while (enumerator7.MoveNext())
            {
                KeyValuePair<uint, ResWealText> pair7 = enumerator7.Current;
                this.TryAdd(new NoticeActivity(this, pair7.Value));
            }
        }

        public Activity GetActivity(COM_WEAL_TYPE type, uint id)
        {
            if (this._actvDict != null)
            {
                uint key = Activity.GenKey(type, id);
                if (this._actvDict.ContainsKey(key))
                {
                    return this._actvDict[key];
                }
            }
            return null;
        }

        public ListView<Activity> GetActivityList(Func<Activity, bool> filter)
        {
            ListView<Activity> view = new ListView<Activity>();
            if (this._actvDict != null)
            {
                DictionaryView<uint, Activity>.Enumerator enumerator = this._actvDict.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, Activity> current = enumerator.Current;
                    if (filter(current.Value))
                    {
                        KeyValuePair<uint, Activity> pair2 = enumerator.Current;
                        view.Add(pair2.Value);
                    }
                }
            }
            return view;
        }

        public uint GetReveivableRedDot(RES_WEAL_ENTRANCE_TYPE entry)
        {
            uint num = 0;
            if (this._actvDict != null)
            {
                DictionaryView<uint, Activity>.Enumerator enumerator = this._actvDict.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, Activity> current = enumerator.Current;
                    if (current.Value.Entrance == entry)
                    {
                        KeyValuePair<uint, Activity> pair2 = enumerator.Current;
                        if (pair2.Value.ReadyForGet)
                        {
                            num++;
                        }
                    }
                }
            }
            return num;
        }

        public override void Init()
        {
            this.m_bShareTask = false;
            this._actvDict = null;
            this._checkTimer = 0;
            this._refreshTimer = 0;
            this._campaignForm = new CampaignForm(this);
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenCampaignForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseCampaignForm));
            Singleton<EventRouter>.instance.AddEventHandler("IDIPNOTICE_UNREAD_NUM_UPDATE", new System.Action(this.OnIDIPNoticeUpdate));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.GLOBAL_REFRESH_TIME, new System.Action(ActivitySys.OnResetAllExchangeCount));
        }

        private void LoadDynamicData(CSPkg msg)
        {
            this.LoadInfo(ref msg.stPkgData.stWealDataNtf.stWealList);
            this.LoadStatistic(ref msg.stPkgData.stWealDataNtf.stWealConData);
            this.LoadPointsData(msg.stPkgData.stWealDataNtf.stWealPointData);
            if (this._actvDict != null)
            {
                uint[] numArray = new uint[this._actvDict.Count];
                int num = 0;
                DictionaryView<uint, Activity>.Enumerator enumerator = this._actvDict.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, Activity> current = enumerator.Current;
                    Activity activity = current.Value;
                    activity.CheckTimeState();
                    if (((activity.timeState == Activity.TimeState.InHiding) || (activity.timeState == Activity.TimeState.Close)) || (activity.Completed && ((activity.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_FOREVER) || (activity.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_LIMIT))))
                    {
                        numArray[num++] = activity.Key;
                    }
                }
                for (int i = 0; i < num; i++)
                {
                    this.RemoveActivity(numArray[i]);
                }
                this._NotifyStateChanged();
                this.StartTimer();
            }
        }

        public void LoadInfo(ref COMDT_WEAL_LIST actvList)
        {
            if (this._actvDict != null)
            {
                for (ushort i = 0; i < actvList.wCnt; i = (ushort) (i + 1))
                {
                    COMDT_WEAL_DETAIL comdt_weal_detail = actvList.astWealDetail[i];
                    Activity activity = this.GetActivity((COM_WEAL_TYPE) comdt_weal_detail.bWealType, comdt_weal_detail.dwWealID);
                    if (activity != null)
                    {
                        activity.UpdateInfo(ref comdt_weal_detail.stWealDetail);
                    }
                }
            }
        }

        public void LoadPointsData(COMDT_WEAL_POINT_DATA pointsInfo)
        {
            if (pointsInfo != null)
            {
                for (ushort i = 0; i < pointsInfo.bWealCnt; i = (ushort) (i + 1))
                {
                    COMDT_WEAL_POINT_DETAIL info = pointsInfo.astWealList[i];
                    this.UpdateOnePointData(info);
                }
            }
        }

        public void LoadStatistic(ref COMDT_WEAL_CON_DATA dat)
        {
            for (ushort i = 0; i < dat.wWealNum; i = (ushort) (i + 1))
            {
                COMDT_WEAL_CON_DATA_DETAIL conData = dat.astWealDetail[i];
                ExeTaskActivity activity = this.GetActivity(COM_WEAL_TYPE.COM_WEAL_CONDITION, conData.dwWealID) as ExeTaskActivity;
                if (activity != null)
                {
                    activity.LoadInfo(conData);
                }
            }
        }

        public static bool NeedShowWhenLogin()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                string key = string.Format("{0}_{1}", "TODAY_LOGIN_SHOW_ACTIVITY_CNT", masterRoleInfo.playerUllUID);
                if (masterRoleInfo.bFirstLoginToday)
                {
                    PlayerPrefs.SetInt(key, 0);
                }
                uint @int = (uint) PlayerPrefs.GetInt(key, 0);
                if ((GameDataMgr.svr2CltCfgDict != null) && GameDataMgr.svr2CltCfgDict.ContainsKey(0))
                {
                    uint dwConfValue = 0;
                    ResGlobalInfo info2 = new ResGlobalInfo();
                    if (GameDataMgr.svr2CltCfgDict.TryGetValue(0, out info2))
                    {
                        dwConfValue = info2.dwConfValue;
                    }
                    if (@int >= dwConfValue)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        [MessageHandler(0x9cc)]
        public static void OnActivityDataNtf(CSPkg msg)
        {
            Singleton<ActivitySys>.GetInstance().CreateStatic();
            Singleton<ActivitySys>.GetInstance().LoadDynamicData(msg);
            if ((!Singleton<BattleLogic>.instance.isRuning && !MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding) && ((Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel >= 5) && !CSysDynamicBlock.bLobbyEntryBlocked))
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.SevenCheck_LoginOpen);
            }
        }

        [MessageHandler(0x9c8)]
        public static void OnActivityDrawRsp(CSPkg msg)
        {
            Singleton<ActivitySys>.GetInstance().ShowRewards(msg.stPkgData.stDrawWealRsp);
        }

        [MessageHandler(0x9c9)]
        public static void OnActivityInfoNtf(CSPkg msg)
        {
            Singleton<ActivitySys>.GetInstance().LoadInfo(ref msg.stPkgData.stWealDetailNtf.stWealList);
        }

        [MessageHandler(0x9ca)]
        public static void OnActivityStatisticNtf(CSPkg msg)
        {
            Singleton<ActivitySys>.GetInstance().LoadStatistic(ref msg.stPkgData.stWealConDataNtf.stWealConData);
        }

        internal void OnCampaignFormOpened()
        {
            MonoSingleton<IDIPSys>.GetInstance().UpdateGlobalPoint();
        }

        private void OnCloseCampaignForm(CUIEvent uiEvent)
        {
            if (this._campaignForm != null)
            {
                this._campaignForm.Close();
                CUICommonSystem.CloseUseableTips();
            }
        }

        private void OnIDIPNoticeUpdate()
        {
            if (this._campaignForm != null)
            {
                GameObject iDIPRedObj = this._campaignForm.GetIDIPRedObj();
                if (iDIPRedObj != null)
                {
                    if (MonoSingleton<IDIPSys>.GetInstance().HaveUpdateList)
                    {
                        CUICommonSystem.AddRedDot(iDIPRedObj, enRedDotPos.enTopRight, 0);
                    }
                    else if (iDIPRedObj != null)
                    {
                        CUICommonSystem.DelRedDot(iDIPRedObj);
                    }
                }
            }
        }

        private void OnOpenCampaignForm(CUIEvent uiEvent)
        {
            if ((!CSysDynamicBlock.bLobbyEntryBlocked && !CSysDynamicBlock.bOperationBlock) && ((this._campaignForm != null) && (this._actvDict != null)))
            {
                this.RequestRefresh(0);
                this._campaignForm.Open();
            }
        }

        [MessageHandler(0x9d0)]
        public static void OnPointsInfoNtf(CSPkg msg)
        {
            if (msg.stPkgData.stWealPointDataNtf != null)
            {
                Singleton<ActivitySys>.GetInstance().UpdateOnePointData(msg.stPkgData.stWealPointDataNtf.stWealData);
            }
        }

        public static void OnResetAllExchangeCount()
        {
            if (<>f__am$cacheD == null)
            {
                <>f__am$cacheD = actv => actv.Type == COM_WEAL_TYPE.COM_WEAL_EXCHANGE;
            }
            ListView<Activity> activityList = Singleton<ActivitySys>.GetInstance().GetActivityList(<>f__am$cacheD);
            for (int i = 0; i < activityList.Count; i++)
            {
                ExchangeActivity activity = activityList[i] as ExchangeActivity;
                if (activity != null)
                {
                    activity.ResetExchangeCount();
                    activity.UpdateView();
                }
            }
            if (<>f__am$cacheE == null)
            {
                <>f__am$cacheE = actv => actv.Type == COM_WEAL_TYPE.COM_WEAL_PTEXCHANGE;
            }
            ListView<Activity> view2 = Singleton<ActivitySys>.GetInstance().GetActivityList(<>f__am$cacheE);
            for (int j = 0; j < view2.Count; j++)
            {
                PointsExchangeActivity activity2 = view2[j] as PointsExchangeActivity;
                if (activity2 != null)
                {
                    activity2.ResetExchangeCount();
                    activity2.UpdateView();
                }
            }
        }

        [MessageHandler(0x9cf)]
        public static void OnResExchange(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stWealExchangeRes.dwWealID != 0)
            {
                ListView<CUseable> useableListFromItemList = CUseableManager.GetUseableListFromItemList(msg.stPkgData.stWealExchangeRes.stExchangeRes);
                if (useableListFromItemList.Count > 0)
                {
                    CUseableManager.ShowUseableItem(useableListFromItemList[0]);
                }
                if (msg.stPkgData.stWealExchangeRes.bWealType == 4)
                {
                    ExchangeActivity activity = (ExchangeActivity) Singleton<ActivitySys>.GetInstance().GetActivity(COM_WEAL_TYPE.COM_WEAL_EXCHANGE, msg.stPkgData.stWealExchangeRes.dwWealID);
                    if (activity != null)
                    {
                        activity.IncreaseExchangeCount(msg.stPkgData.stWealExchangeRes.bWealIdx);
                        activity.UpdateView();
                    }
                }
                else if (msg.stPkgData.stWealExchangeRes.bWealType == 5)
                {
                    PointsExchangeActivity activity2 = (PointsExchangeActivity) Singleton<ActivitySys>.GetInstance().GetActivity(COM_WEAL_TYPE.COM_WEAL_PTEXCHANGE, msg.stPkgData.stWealExchangeRes.dwWealID);
                    if (activity2 != null)
                    {
                        activity2.IncreaseExchangeCount(msg.stPkgData.stWealExchangeRes.bWealIdx);
                        activity2.UpdateView();
                    }
                }
            }
        }

        private void OnTimerCheck(int timeSeq)
        {
            if ((this._actvDict != null) && !Singleton<BattleLogic>.GetInstance().isRuning)
            {
                DictionaryView<uint, Activity>.Enumerator enumerator = this._actvDict.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, Activity> current = enumerator.Current;
                    current.Value.CheckTimeState();
                }
                this._campaignForm.Update();
            }
        }

        public void PandroaUpdateBtn()
        {
            if (this._campaignForm != null)
            {
                this._campaignForm.PandroaUpdateBtn();
            }
        }

        private void RemoveActivity(uint key)
        {
            if ((this._actvDict != null) && this._actvDict.ContainsKey(key))
            {
                this._actvDict[key].Clear();
                this._actvDict.Remove(key);
            }
        }

        public void RequestRefresh(int seq = 0)
        {
            if (seq > 0)
            {
                Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._refreshTimer);
            }
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x9cb);
            msg.stPkgData.stWealDataReq.bReserved = 1;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void ShowNextReward(CUIEvent firstIfNull)
        {
            if ((firstIfNull != null) || ((this._rewardShowList == null) && (this._rewardQueueIndex <= -1)))
            {
                if (this._rewardShowList == null)
                {
                    this._rewardQueueIndex++;
                    if (this._rewardQueueIndex >= this._rewardListQueue.Count)
                    {
                        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_NewHeroOrSkinFormClose, new CUIEventManager.OnUIEventHandler(this.ShowNextReward));
                        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Get_AWARD_CLOSE_FORM, new CUIEventManager.OnUIEventHandler(this.ShowNextReward));
                        this._rewardListQueue.Clear();
                        this._rewardQueueIndex = -1;
                        this._rewardShowIndex = -1;
                        return;
                    }
                    this._rewardShowList = this._rewardListQueue[this._rewardQueueIndex];
                    this._rewardShowIndex = -1;
                    this._rewardHasSpecial = false;
                }
                while ((((++this._rewardShowIndex < this._rewardShowList.usabList.Count) && (this._rewardShowList.usabList[this._rewardShowIndex].MapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO)) && (this._rewardShowList.usabList[this._rewardShowIndex].MapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN)) && ((this._rewardShowList.usabList[this._rewardShowIndex].MapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM) || ((this._rewardShowList.usabList[this._rewardShowIndex].ExtraFromType != 1) && (this._rewardShowList.usabList[this._rewardShowIndex].ExtraFromType != 2))))
                {
                }
                if (this._rewardShowIndex < this._rewardShowList.usabList.Count)
                {
                    CUseable useable = this._rewardShowList.usabList[this._rewardShowIndex];
                    if (useable.MapRewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM)
                    {
                        if (useable.ExtraFromType == 1)
                        {
                            CUICommonSystem.ShowNewHeroOrSkin((uint) useable.ExtraFromData, 0, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority1, (uint) useable.m_stackCount, 0);
                        }
                        else if (useable.ExtraFromType == 2)
                        {
                            int extraFromData = useable.ExtraFromData;
                            CUICommonSystem.ShowNewHeroOrSkin(0, (uint) extraFromData, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority1, (uint) useable.m_stackCount, 0);
                        }
                    }
                    else if (useable is CHeroSkin)
                    {
                        CHeroSkin skin = useable as CHeroSkin;
                        CUICommonSystem.ShowNewHeroOrSkin(skin.m_heroId, skin.m_skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority1, 0, 0);
                    }
                    else
                    {
                        CUICommonSystem.ShowNewHeroOrSkin(useable.m_baseID, 0, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority1, 0, 0);
                    }
                    this._rewardHasSpecial = true;
                }
                else if ((this._rewardShowList.usabList.Count > 1) || !this._rewardHasSpecial)
                {
                    bool flag = (this._rewardShowList.flags & 2) > 0;
                    Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(this._rewardShowList.usabList), Singleton<CTextManager>.GetInstance().GetText(!flag ? "gotAward" : "gotExtraAward"), true, enUIEventID.None, false, false, "Form_Award");
                    this._rewardShowList = null;
                }
                else
                {
                    this._rewardShowList = null;
                    this.ShowNextReward(new CUIEvent());
                }
            }
        }

        private void ShowRewards(SCPKG_DRAWWEAL_RSP rspPkg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Activity activity = Singleton<ActivitySys>.GetInstance().GetActivity((COM_WEAL_TYPE) rspPkg.bWealType, rspPkg.dwWealID);
            if (activity != null)
            {
                if (rspPkg.iResult == 0)
                {
                    activity.SetPhaseMarked(rspPkg.dwPeriodID);
                    if (rspPkg.stReward.bNum > 0)
                    {
                        ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(rspPkg.stReward);
                        for (int i = 0; i < useableListFromReward.Count; i++)
                        {
                            useableListFromReward[i].m_stackMulti = (int) (rspPkg.stMultipleInfo.dwWealRatio / 0x2710);
                        }
                        this._rewardListQueue.Add(new RewardList(useableListFromReward, rspPkg.iExtraCode));
                        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_NewHeroOrSkinFormClose, new CUIEventManager.OnUIEventHandler(this.ShowNextReward));
                        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Get_AWARD_CLOSE_FORM, new CUIEventManager.OnUIEventHandler(this.ShowNextReward));
                        this.ShowNextReward(null);
                    }
                }
                else
                {
                    string str = "[";
                    if ((rspPkg.iResult == 8) || (rspPkg.iResult == 9))
                    {
                        str = str + Singleton<CTextManager>.GetInstance().GetText("payError");
                    }
                    else
                    {
                        str = str + rspPkg.iResult;
                    }
                    str = str + "]";
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("drawRewardFailed") + str, false);
                }
            }
        }

        public void StartTimer()
        {
            if (this._checkTimer == 0)
            {
                this._checkTimer = Singleton<CTimerManager>.GetInstance().AddTimer(0x3e8, -1, new CTimer.OnTimeUpHandler(this.OnTimerCheck), false);
            }
            if (this._refreshTimer == 0)
            {
                DateTime time = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
                DateTime time2 = time.AddDays(1.0);
                DateTime time3 = new DateTime(time2.Year, time2.Month, time2.Day, 0, UnityEngine.Random.Range(5, 60), UnityEngine.Random.Range(0, 60));
                TimeSpan span = (TimeSpan) (time3 - time);
                this._refreshTimer = Singleton<CTimerManager>.GetInstance().AddTimer((int) (span.TotalSeconds * 1000.0), 1, new CTimer.OnTimeUpHandler(this.RequestRefresh));
            }
        }

        private void TryAdd(Activity actv)
        {
            if (!this._actvDict.ContainsKey(actv.Key))
            {
                actv.CheckTimeState();
                if ((actv.timeState == Activity.TimeState.ForeShow) || (actv.timeState == Activity.TimeState.Going))
                {
                    actv.Start();
                    this._actvDict.Add(actv.Key, actv);
                }
            }
        }

        public override void UnInit()
        {
            Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._checkTimer);
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenCampaignForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseCampaignForm));
            Singleton<EventRouter>.instance.RemoveEventHandler("IDIPNOTICE_UNREAD_NUM_UPDATE", new System.Action(this.OnIDIPNoticeUpdate));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.GLOBAL_REFRESH_TIME, new System.Action(ActivitySys.OnResetAllExchangeCount));
        }

        public static void UpdateLoginShowCnt()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                string key = string.Format("{0}_{1}", "TODAY_LOGIN_SHOW_ACTIVITY_CNT", masterRoleInfo.playerUllUID);
                uint @int = (uint) PlayerPrefs.GetInt(key, 0);
                PlayerPrefs.SetInt(key, (int) (++@int));
                PlayerPrefs.Save();
            }
        }

        public void UpdateOnePointData(COMDT_WEAL_POINT_DETAIL info)
        {
            PointsExchangeActivity activity = this.GetActivity(COM_WEAL_TYPE.COM_WEAL_PTEXCHANGE, info.dwWealID) as PointsExchangeActivity;
            if (activity != null)
            {
                activity.UpdatePointsInfo(info.dwPointCnt, info.dwWealValue);
            }
        }

        public bool IsShareTask
        {
            get
            {
                return this.m_bShareTask;
            }
            set
            {
                this.m_bShareTask = value;
            }
        }

        private class RewardList
        {
            public int flags;
            public ListView<CUseable> usabList;

            public RewardList(ListView<CUseable> usabList, int flags)
            {
                this.usabList = usabList;
                this.flags = flags;
            }
        }

        public delegate void StateChangeDelegate();
    }
}

