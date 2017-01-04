namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CLobbySystem : Singleton<CLobbySystem>
    {
        private static bool _autoPoped = false;
        private GameObject _rankingBtn;
        private GameObject addSkill_btn;
        public static bool AutoPopAllow = true;
        private GameObject bag_btn;
        private GameObject hero_btn;
        public static string LOBBY_FORM_PATH = "UGUI/Form/System/Lobby/Form_Lobby.prefab";
        public static string LOBBY_FUN_UNLOCK_PATH = "UGUI/Form/System/Lobby/Form_FunUnLock.prefab";
        private bool m_bInLobby;
        private DictionaryView<int, GameObject> m_Btns;
        private Text m_CreditScore;
        private Text m_lblDiamond;
        private Text m_lblDianquan;
        private Text m_lblGlodCoin;
        private CUIFormScript m_LobbyForm;
        private Text m_ping;
        private Text m_PlayerExp;
        private Text m_PlayerName;
        private Text m_PlayerVipLevel;
        private Image m_PvpExpImg;
        private Text m_PvpExpTxt;
        private Text m_PvpLevel;
        private GameObject m_QQbuluBtn;
        private CUIFormScript m_RankingBtnForm;
        private RankingSystem.RankingSubView m_rankingType = RankingSystem.RankingSubView.Friend;
        private GameObject m_SysEntry;
        private CUIFormScript m_SysEntryForm;
        private GameObject m_textMianliu;
        private GameObject m_wifiIcon;
        private GameObject m_wifiInfo;
        private int myRankingNo = -1;
        public bool NeedRelogin;
        public static string Pve_BtnRes_PATH = (CUIUtility.s_Sprite_System_Lobby_Dir + "PveBtnDynamic.prefab");
        public static string Pvp_BtnRes_PATH = (CUIUtility.s_Sprite_System_Lobby_Dir + "PvpBtnDynamic.prefab");
        private ListView<COMDT_FRIEND_INFO> rankFriendList;
        public static string RANKING_BTN_FORM_PATH = "UGUI/Form/System/Lobby/Form_Lobby_RankingBtn.prefab";
        public static uint s_CoinShowMaxValue = 0xf1b30;
        public static uint s_CoinShowStepValue = 0x2710;
        public static string[] s_netStateName = new string[] { "Net_1", "Net_2", "Net_3" };
        public static string s_noNetStateName = "NoNet";
        public static Color[] s_WifiStateColor = new Color[] { Color.red, Color.yellow, Color.green };
        public static string[] s_wifiStateName = new string[] { "Wifi_1", "Wifi_2", "Wifi_3" };
        private GameObject social_btn;
        private GameObject symbol_btn;
        public static string SYSENTRY_FORM_PATH = "UGUI/Form/System/Lobby/Form_Lobby_SysTray.prefab";
        private GameObject task_btn;

        public void AddRedDot(enSysEntryID sysEntryId, enRedDotPos redDotPos = 2, int count = 0)
        {
            if (this.m_Btns != null)
            {
                GameObject obj2;
                this.m_Btns.TryGetValue((int) sysEntryId, out obj2);
                CUICommonSystem.AddRedDot(obj2, redDotPos, count);
            }
        }

        public void AddRedDotEx(enSysEntryID sysEntryId, enRedDotPos redDotPos = 2, int alertNum = 0)
        {
            if (this.m_Btns != null)
            {
                GameObject obj2;
                this.m_Btns.TryGetValue((int) sysEntryId, out obj2);
                CUICommonSystem.AddRedDot(obj2, redDotPos, alertNum);
            }
        }

        private void AutoPopup1_IDIP()
        {
            if (MonoSingleton<IDIPSys>.GetInstance().RedPotState)
            {
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IDIP_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnIDIPClose));
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.IDIP_OpenForm);
            }
            else
            {
                this.AutoPopup2_Activity();
            }
        }

        private void AutoPopup2_Activity()
        {
            if (Singleton<ActivitySys>.GetInstance().CheckReadyForDot(RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_ACTIVITY))
            {
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnActivityClose));
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Activity_OpenForm);
            }
        }

        public void Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE type)
        {
            bool bShow = Singleton<CFunctionUnlockSys>.instance.TipsHasShow(type);
            if (!bShow && !Singleton<CFunctionUnlockSys>.instance.IsTypeHasCondition(type))
            {
                bShow = true;
            }
            this.SetEnable(type, bShow);
        }

        private bool checkIsHaveRedDot()
        {
            DictionaryView<int, GameObject>.Enumerator enumerator = this.m_Btns.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, GameObject> current = enumerator.Current;
                if (CUICommonSystem.IsHaveRedDot(current.Value))
                {
                    return true;
                }
            }
            return false;
        }

        public void CheckMianLiu()
        {
            if (this.m_textMianliu != null)
            {
                if (MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai() && MonoSingleton<CTongCaiSys>.instance.IsLianTongIp())
                {
                    this.m_textMianliu.CustomSetActive(true);
                }
                else
                {
                    this.m_textMianliu.CustomSetActive(false);
                }
            }
        }

        private void CheckNewbieIntro(int timerSeq)
        {
            if (!this.PopupNewbieIntro() && !_autoPoped)
            {
                _autoPoped = true;
            }
        }

        public void CheckWifi()
        {
            if (((this.m_wifiIcon != null) && (this.m_wifiInfo != null)) && (this.m_ping != null))
            {
                int lobbyPing = (int) Singleton<NetworkModule>.GetInstance().lobbyPing;
                lobbyPing = (lobbyPing <= 100) ? lobbyPing : ((((lobbyPing - 100) * 7) / 10) + 100);
                lobbyPing = Mathf.Clamp(lobbyPing, 0, 460);
                uint index = 0;
                if (lobbyPing < 100)
                {
                    index = 2;
                }
                else if (lobbyPing < 200)
                {
                    index = 1;
                }
                else
                {
                    index = 0;
                }
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    CUICommonSystem.PlayAnimator(this.m_wifiIcon, s_noNetStateName);
                }
                else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                {
                    CUICommonSystem.PlayAnimator(this.m_wifiIcon, s_wifiStateName[index]);
                }
                else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                {
                    CUICommonSystem.PlayAnimator(this.m_wifiIcon, s_netStateName[index]);
                }
                if ((this.m_wifiInfo != null) && this.m_wifiInfo.activeInHierarchy)
                {
                    this.m_ping.text = lobbyPing + "ms";
                }
            }
        }

        public void Clear()
        {
            this.m_rankingType = RankingSystem.RankingSubView.Friend;
        }

        public void DelRedDot(enSysEntryID sysEntryId)
        {
            if (this.m_Btns != null)
            {
                GameObject obj2;
                this.m_Btns.TryGetValue((int) sysEntryId, out obj2);
                CUICommonSystem.DelRedDot(obj2);
            }
        }

        public void FullShow()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(SYSENTRY_FORM_PATH);
            if (form != null)
            {
                Transform transform = form.transform;
                transform.Find("PlayerBtn/MailBtn").gameObject.CustomSetActive(true);
                transform.Find("PlayerBtn/SettingBtn").gameObject.CustomSetActive(true);
                transform.Find("PlayerBtn/FriendBtn").gameObject.CustomSetActive(true);
            }
        }

        public string GetCoinString(uint coinValue)
        {
            string str = coinValue.ToString();
            if (coinValue > s_CoinShowMaxValue)
            {
                int num = (int) (coinValue / s_CoinShowStepValue);
                str = string.Format("{0}万", num);
            }
            return str;
        }

        private void HideMoreBtn(CUIEvent uiEvent)
        {
            if (this.m_LobbyForm != null)
            {
                Transform transform = this.m_LobbyForm.transform.Find("Popup/MoreCon");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(false);
                }
                Transform transform2 = this.m_LobbyForm.transform.Find("Popup/MoreBtn");
                if (transform2 != null)
                {
                    transform2.gameObject.CustomSetActive(true);
                }
            }
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OpenLobbyForm, new CUIEventManager.OnUIEventHandler(this.onOpenLobby));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.WEB_OpenURL, new CUIEventManager.OnUIEventHandler(this.onOpenWeb));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_WifiCheckTimer, new CUIEventManager.OnUIEventHandler(this.onCommon_WifiCheckTimer));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_ShowOrHideWifiInfo, new CUIEventManager.OnUIEventHandler(this.onCommon_ShowOrHideWifiInfo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_ConfirmErrExit, new CUIEventManager.OnUIEventHandler(this.onErrorExit));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_LobbyFormShow, new CUIEventManager.OnUIEventHandler(this.Lobby_LobbyFormShow));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_LobbyFormHide, new CUIEventManager.OnUIEventHandler(this.Lobby_LobbyFormHide));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_Close, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_PrepareFight_Sub, new CUIEventManager.OnUIEventHandler(this.OnPrepareFight_Sub));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_PrepareFight_Origin, new CUIEventManager.OnUIEventHandler(this.OnPrepareFight_Origin));
            Singleton<EventRouter>.instance.AddEventHandler("MasterAttributesChanged", new System.Action(this.UpdatePlayerData));
            Singleton<EventRouter>.instance.AddEventHandler("TaskUpdated", new System.Action(this.OnTaskUpdate));
            Singleton<EventRouter>.instance.AddEventHandler("Friend_LobbyIconRedDot_Refresh", new System.Action(this.OnFriendSysIconUpdate));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Entry_Add_RedDotCheck, new System.Action(this.OnCheckAddMallEntryRedDot));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Entry_Del_RedDotCheck, new System.Action(this.OnCheckDelMallEntryRedDot));
            Singleton<EventRouter>.instance.AddEventHandler("MailUnReadNumUpdate", new System.Action(this.OnMailUnReadUpdate));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.ACHIEVE_STATE_UPDATE, new System.Action(this.OnAchieveStateUpdate));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.SymbolEquipSuc, new System.Action(this.OnCheckSymbolEquipAlert));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.BAG_ITEMS_UPDATE, new System.Action(this.OnCheckSymbolEquipAlert));
            Singleton<EventRouter>.instance.AddEventHandler("MasterPvpLevelChanged", new System.Action(this.OnCheckSymbolEquipAlert));
            Singleton<EventRouter>.instance.AddEventHandler<bool>("Guild_Sign_State_Changed", new Action<bool>(this.OnGuildSignStateChanged));
            Singleton<ActivitySys>.GetInstance().OnStateChange += new ActivitySys.StateChangeDelegate(this.ValidateActivitySpot);
            Singleton<EventRouter>.instance.AddEventHandler("IDIPNOTICE_UNREAD_NUM_UPDATE", new System.Action(this.ValidateActivitySpot));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.IDIP_QQVIP_OpenWealForm, new CUIEventManager.OnUIEventHandler(this.OpenQQVIPWealForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.WEB_OpenHome, new CUIEventManager.OnUIEventHandler(this.OpenWebHome));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.WEB_IntegralHall, new CUIEventManager.OnUIEventHandler(this.OpenIntegralHall));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OPEN_QQ_Buluo, new CUIEventManager.OnUIEventHandler(this.OpenQQBuluo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_ShowMoreBtn, new CUIEventManager.OnUIEventHandler(this.ShowMoreBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_HideMoreBtn, new CUIEventManager.OnUIEventHandler(this.HideMoreBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_UnlockAnimation_End, new CUIEventManager.OnUIEventHandler(this.On_Lobby_UnlockAnimation_End));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_MysteryShopClose, new CUIEventManager.OnUIEventHandler(this.On_Lobby_MysteryShopClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.GameCenter_OpenWXRight, new CUIEventManager.OnUIEventHandler(this.OpenWXGameCenterRightForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.GameCenter_OpenQQRight, new CUIEventManager.OnUIEventHandler(this.OpenQQGameCenterRightForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_RankingListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnRankingListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.GameCenter_OpenGuestRight, new CUIEventManager.OnUIEventHandler(this.OpenGuestGameCenterRightForm));
            Singleton<EventRouter>.GetInstance().AddEventHandler("CheckNewbieIntro", new System.Action(this.OnCheckNewbieIntro));
            Singleton<EventRouter>.GetInstance().AddEventHandler("VipInfoHadSet", new System.Action(this.UpdateQQVIPState));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NOBE_STATE_CHANGE, new System.Action(this.UpdateNobeIcon));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NOBE_STATE_HEAD_CHANGE, new System.Action(this.UpdateNobeHeadIdx));
            Singleton<EventRouter>.GetInstance().AddEventHandler("MasterPvpLevelChanged", new System.Action(this.OnPlayerLvlChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Rank_Friend_List", new System.Action(this.RefreshRankList));
            Singleton<EventRouter>.GetInstance().AddEventHandler<RankingSystem.RankingSubView>("Rank_List", new Action<RankingSystem.RankingSubView>(this.RefreshRankList));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NAMECHANGE_PLAYER_NAME_CHANGE, new System.Action(this.OnPlayerNameChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.HEAD_IMAGE_FLAG_CHANGE, new System.Action(this.UpdatePlayerData));
        }

        private void InitForm(CUIFormScript form)
        {
            Transform transform = form.transform;
            this.m_PlayerName = transform.Find("PlayerHead/NameGroup/PlayerName").GetComponent<Text>();
            this.m_PvpLevel = transform.Find("PlayerHead/pvpLevel").GetComponent<Text>();
            this.m_PlayerExp = transform.Find("PlayerHead/PlayerExp").GetComponent<Text>();
            this.m_PlayerVipLevel = transform.Find("PlayerHead/imgVipBg/txtVipLevel").GetComponent<Text>();
            this.m_PvpExpImg = transform.Find("PlayerHead/pvpExp/expBg/imgExp").GetComponent<Image>();
            this.m_PvpExpTxt = transform.Find("PlayerHead/pvpExp/expBg/txtExp").GetComponent<Text>();
            this.m_CreditScore = transform.Find("PlayerHead/CreditTxt").GetComponent<Text>();
            this.hero_btn = transform.Find("LobbyBottom/SysEntry/HeroBtn").gameObject;
            this.symbol_btn = transform.Find("LobbyBottom/SysEntry/SymbolBtn").gameObject;
            this.bag_btn = transform.Find("LobbyBottom/SysEntry/BagBtn").gameObject;
            this.task_btn = transform.Find("LobbyBottom/Newbie").gameObject;
            this.social_btn = transform.Find("LobbyBottom/SysEntry/SocialBtn").gameObject;
            this.addSkill_btn = transform.Find("LobbyBottom/SysEntry/AddedSkillBtn").gameObject;
            this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_HERO);
            this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL);
            this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_BAG);
            this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_TASK);
            this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_UNION);
            this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_FRIEND);
            this.Check_Enable(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL);
            if (CSysDynamicBlock.bLobbyEntryBlocked)
            {
                Transform transform2 = transform.Find("Popup");
                if (transform2 != null)
                {
                    transform2.gameObject.CustomSetActive(false);
                }
                Transform transform3 = transform.Find("BtnCon/CompetitionBtn");
                if (transform3 != null)
                {
                    transform3.gameObject.CustomSetActive(false);
                }
                if (this.task_btn != null)
                {
                    this.task_btn.CustomSetActive(false);
                }
                Transform transform4 = transform.Find("DiamondPayBtn");
                if (transform4 != null)
                {
                    transform4.gameObject.CustomSetActive(false);
                }
            }
            Button component = transform.Find("BtnCon/LadderBtn").GetComponent<Button>();
            if (component != null)
            {
                component.interactable = Singleton<CLadderSystem>.GetInstance().IsLevelQualified();
                Transform transform5 = component.transform.Find("Lock");
                if (transform5 != null)
                {
                    transform5.gameObject.CustomSetActive(!component.interactable);
                }
            }
            Button button2 = transform.FindChild("BtnCon/UnionBtn").GetComponent<Button>();
            if (button2 != null)
            {
                bool bActive = Singleton<CUnionBattleEntrySystem>.GetInstance().IsUnionFuncLocked();
                button2.interactable = !bActive;
                button2.transform.FindChild("Lock").gameObject.CustomSetActive(bActive);
            }
            GameObject gameObject = transform.Find("PlayerHead/pvpExp").gameObject;
            if (gameObject != null)
            {
                CUIEventScript script = gameObject.GetComponent<CUIEventScript>();
                if (script == null)
                {
                    script = gameObject.AddComponent<CUIEventScript>();
                    script.Initialize(form);
                }
                CUseable useable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enExp, 0);
                stUIEventParams eventParams = new stUIEventParams {
                    iconUseable = useable,
                    tag = 3
                };
                script.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
                script.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
                script.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
                script.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
            }
            RefreshDianQuanPayButton(false);
            Transform transform6 = transform.Find("BtnCon/PvpBtn");
            Transform transform7 = transform.Find("BtnCon/PveBtn");
            if (transform6 != null)
            {
                CUICommonSystem.LoadUIPrefab(Pvp_BtnRes_PATH, "PvpBtnDynamic", transform6.gameObject, form);
            }
            if (transform7 != null)
            {
                CUICommonSystem.LoadUIPrefab(Pve_BtnRes_PATH, "PveBtnDynamic", transform7.gameObject, form);
            }
            GameObject widget = form.GetWidget(7);
            if (CSysDynamicBlock.bLobbyEntryBlocked)
            {
                widget.CustomSetActive(false);
            }
            else
            {
                Text text = form.GetWidget(8).GetComponent<Text>();
                if (GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xcc).dwConfValue > 0)
                {
                    widget.CustomSetActive(IsPlatChannelOpen);
                    text.text = Singleton<CTextManager>.GetInstance().GetText("CrossPlat_Plat_Channel_Open_Lobby_Msg");
                }
                else
                {
                    widget.CustomSetActive(!IsPlatChannelOpen);
                    text.text = Singleton<CTextManager>.GetInstance().GetText("CrossPlat_Plat_Channel_Not_Open_Lobby_Msg");
                }
            }
        }

        private void InitOther(CUIFormScript m_FormScript)
        {
            Singleton<CTimerManager>.GetInstance().AddTimer(50, 1, new CTimer.OnTimeUpHandler(this.CheckNewbieIntro));
            this.ProcessQQVIP(m_FormScript);
            this.UpdateGameCenterState(m_FormScript);
            MonoSingleton<NobeSys>.GetInstance().ShowDelayNobeTipsInfo();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo != null) && (masterRoleInfo.m_licenseInfo != null))
            {
                masterRoleInfo.m_licenseInfo.ReviewLicenseList();
            }
        }

        private void InitRankingBtnForm()
        {
            if (this.m_RankingBtnForm == null)
            {
                DebugHelper.Assert(false, "m_RankingBtnForm cannot be null!!!");
            }
            else
            {
                this._rankingBtn = this.m_RankingBtnForm.GetWidget(0);
                if ((this._rankingBtn != null) && CSysDynamicBlock.bSocialBlocked)
                {
                    this._rankingBtn.CustomSetActive(false);
                }
                this.RefreshRankList();
            }
        }

        private void InitSysEntryForm(CUIFormScript form)
        {
            Transform transform = form.gameObject.transform;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            this.m_lblGlodCoin = transform.FindChild("PlayerBtn/GoldCoin/Text").GetComponent<Text>();
            this.m_lblDianquan = transform.FindChild("PlayerBtn/Dianquan/Text").GetComponent<Text>();
            this.m_lblDiamond = transform.FindChild("PlayerBtn/Diamond/Text").GetComponent<Text>();
            this.m_wifiIcon = form.GetWidget(0);
            this.m_wifiInfo = form.GetWidget(1);
            this.m_ping = form.GetWidget(2).GetComponent<Text>();
            this.m_textMianliu = form.GetWidget(9);
            this.m_lblGlodCoin.text = this.GetCoinString(masterRoleInfo.GoldCoin);
            this.m_lblDianquan.text = this.GetCoinString((uint) masterRoleInfo.DianQuan);
            this.m_lblDiamond.text = this.GetCoinString(masterRoleInfo.Diamond);
            GameObject gameObject = transform.Find("PlayerBtn/GoldCoin").gameObject;
            if (gameObject != null)
            {
                CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
                if (component == null)
                {
                    component = gameObject.AddComponent<CUIEventScript>();
                    component.Initialize(form);
                }
                CUseable useable = CUseableManager.CreateCoinUseable(RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN, (int) masterRoleInfo.GoldCoin);
                stUIEventParams eventParams = new stUIEventParams {
                    iconUseable = useable,
                    tag = 3
                };
                component.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
                component.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
                component.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
                component.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
            }
            GameObject obj3 = transform.Find("PlayerBtn/Diamond").gameObject;
            if (obj3 != null)
            {
                CUIEventScript script2 = obj3.GetComponent<CUIEventScript>();
                if (script2 == null)
                {
                    script2 = obj3.AddComponent<CUIEventScript>();
                    script2.Initialize(form);
                }
                CUseable useable2 = CUseableManager.CreateCoinUseable(RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND, (int) masterRoleInfo.Diamond);
                stUIEventParams params2 = new stUIEventParams {
                    iconUseable = useable2,
                    tag = 3
                };
                script2.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, params2);
                script2.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, params2);
                script2.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, params2);
                script2.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, params2);
            }
            if (!ApolloConfig.payEnabled)
            {
                Transform transform2 = transform.Find("PlayerBtn/Dianquan/Button");
                if (transform2 != null)
                {
                    transform2.gameObject.CustomSetActive(false);
                }
            }
            this.m_SysEntry = this.m_LobbyForm.gameObject.transform.Find("LobbyBottom/SysEntry").gameObject;
            this.m_Btns = new DictionaryView<int, GameObject>();
            this.m_Btns.Add(0, this.m_SysEntry.transform.Find("HeroBtn").gameObject);
            this.m_Btns.Add(1, this.m_SysEntry.transform.Find("SymbolBtn").gameObject);
            this.m_Btns.Add(2, this.m_SysEntry.transform.Find("AchievementBtn").gameObject);
            this.m_Btns.Add(3, this.m_SysEntry.transform.Find("BagBtn").gameObject);
            this.m_Btns.Add(5, this.m_SysEntry.transform.Find("SocialBtn").gameObject);
            this.m_Btns.Add(6, form.transform.Find("PlayerBtn/FriendBtn").gameObject);
            this.m_Btns.Add(7, this.m_SysEntry.transform.Find("AddedSkillBtn").gameObject);
            this.m_Btns.Add(8, form.transform.Find("PlayerBtn/MailBtn").gameObject);
            this.m_Btns.Add(9, Utility.FindChild(this.m_LobbyForm.gameObject, "Popup/ActBtn"));
            this.m_Btns.Add(10, Utility.FindChild(this.m_LobbyForm.gameObject, "Popup/BoardBtn"));
            this.m_Btns.Add(4, this.m_LobbyForm.gameObject.transform.Find("LobbyBottom/Newbie/RedDotPanel").gameObject);
        }

        public bool IsInLobbyForm()
        {
            return this.m_bInLobby;
        }

        private void Lobby_LobbyFormHide(CUIEvent uiEvent)
        {
            if (this.m_bInLobby)
            {
                this.MiniShow();
            }
        }

        private void Lobby_LobbyFormShow(CUIEvent uiEvent)
        {
            if (this.m_bInLobby)
            {
                this.FullShow();
                CUICommonSystem.CloseCommonTips();
                CUICommonSystem.CloseUseableTips();
                Singleton<CMiShuSystem>.instance.CheckActPlayModeTipsForLobby();
                Singleton<CChatController>.instance.ShowPanel(true, false);
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Lobby_PrepareFight_Sub);
                if (!string.IsNullOrEmpty(Singleton<CFriendContoller>.instance.IntimacyUpInfo))
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(string.Format(UT.GetText("Intimacy_UpInfo"), Singleton<CFriendContoller>.instance.IntimacyUpInfo, Singleton<CFriendContoller>.instance.IntimacyUpValue), false, 1.5f, null, new object[0]);
                    Singleton<CFriendContoller>.instance.IntimacyUpInfo = string.Empty;
                    Singleton<CFriendContoller>.instance.IntimacyUpValue = 0;
                }
            }
        }

        public void MiniShow()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(SYSENTRY_FORM_PATH);
            if (form != null)
            {
                Transform transform = form.transform;
                transform.Find("PlayerBtn/MailBtn").gameObject.CustomSetActive(false);
                transform.Find("PlayerBtn/SettingBtn").gameObject.CustomSetActive(false);
                transform.Find("PlayerBtn/FriendBtn").gameObject.CustomSetActive(false);
            }
        }

        private void On_Lobby_MysteryShopClose(CUIEvent uiEvent)
        {
            GameObject obj2 = Utility.FindChild(uiEvent.m_srcFormScript.gameObject, "Popup/BoardBtn/MysteryShop");
            Debug.LogWarning(string.Format("mystery shop icon on close:{0}", obj2));
            obj2.CustomSetActive(false);
        }

        private void On_Lobby_UnlockAnimation_End(CUIEvent uievent)
        {
            Singleton<CUIManager>.instance.CloseForm(LOBBY_FUN_UNLOCK_PATH);
            Singleton<CSoundManager>.instance.PostEvent("UI_hall_system_back", null);
            this.SetEnable((RES_SPECIALFUNCUNLOCK_TYPE) uievent.m_eventParams.tag, true);
        }

        private void OnAchieveStateUpdate()
        {
            CAchieveInfo achieveInfo = CAchieveInfo.GetAchieveInfo();
            if (achieveInfo != null)
            {
                if (achieveInfo.IsHaveFinishButNotGetRewardAchievement(0))
                {
                    this.AddRedDot(enSysEntryID.AchievementBtn, enRedDotPos.enTopRight, 0);
                }
                else
                {
                    this.DelRedDot(enSysEntryID.AchievementBtn);
                }
            }
        }

        private void OnActivityClose(CUIEvent uiEvt)
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnActivityClose));
        }

        private void OnCheckAddMallEntryRedDot()
        {
            if ((((CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_HeroTab) || CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_HeroSkinTab)) || (CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_SymbolTab) || CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_SaleTab))) || ((CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_LotteryTab) || CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_RecommendTab)) || (CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_MysteryTab) && CUIRedDotSystem.IsShowRedDotByLogic(enRedID.Mall_MysteryTab)))) || (CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_BoutiqueTab) || CUIRedDotSystem.IsShowRedDotByLogic(enRedID.Mall_SymbolTab)))
            {
                this.AddRedDot(enSysEntryID.MallBtn, enRedDotPos.enTopRight, 0);
            }
        }

        private void OnCheckDelMallEntryRedDot()
        {
            if ((((!CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_HeroTab) && !CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_HeroSkinTab)) && (!CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_SymbolTab) && !CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_SaleTab))) && ((!CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_LotteryTab) && !CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_RecommendTab)) && (!CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_MysteryTab) || !CUIRedDotSystem.IsShowRedDotByLogic(enRedID.Mall_MysteryTab)))) && (!CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Mall_BoutiqueTab) && !CUIRedDotSystem.IsShowRedDotByLogic(enRedID.Mall_SymbolTab)))
            {
                this.DelRedDot(enSysEntryID.MallBtn);
            }
        }

        private void OnCheckNewbieIntro()
        {
            Singleton<CTimerManager>.GetInstance().AddTimer(100, 1, seq => this.PopupNewbieIntro());
        }

        public void OnCheckSymbolEquipAlert()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                int num;
                uint num2;
                if (masterRoleInfo.m_symbolInfo.CheckAnyWearSymbol(out num, out num2))
                {
                    this.AddRedDot(enSysEntryID.SymbolBtn, enRedDotPos.enTopRight, 0);
                }
                else
                {
                    this.DelRedDot(enSysEntryID.SymbolBtn);
                }
            }
        }

        private void OnCheckUpdateClientVersion()
        {
            if (Singleton<LobbyLogic>.instance.NeedUpdateClient)
            {
                Singleton<LobbyLogic>.instance.NeedUpdateClient = false;
                Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("VersionIsLow"), enUIEventID.None, false);
            }
        }

        protected void OnCloseForm(CUIEvent uiEvt)
        {
            this.m_bInLobby = false;
            this.m_LobbyForm = null;
            this.m_SysEntryForm = null;
            Singleton<CUIManager>.GetInstance().CloseForm(SYSENTRY_FORM_PATH);
            Singleton<CUIManager>.GetInstance().CloseForm(RANKING_BTN_FORM_PATH);
            this.UnInitWidget();
        }

        private void onCommon_ShowOrHideWifiInfo(CUIEvent uiEvent)
        {
            if (this.m_bInLobby)
            {
                this.ShowOrHideWifiInfo();
            }
        }

        private void onCommon_WifiCheckTimer(CUIEvent uiEvent)
        {
            if (this.m_bInLobby)
            {
                this.CheckWifi();
            }
        }

        private void onErrorExit(CUIEvent uiEvent)
        {
            SGameApplication.Quit();
        }

        private void OnFriendSysIconUpdate()
        {
            if (Singleton<CFriendContoller>.GetInstance().model.GetDataCount(CFriendModel.FriendType.RequestFriend) > 0)
            {
                this.AddRedDot(enSysEntryID.FriendBtn, enRedDotPos.enTopRight, 0);
            }
            else
            {
                this.DelRedDot(enSysEntryID.FriendBtn);
            }
        }

        private void OnGuildSignStateChanged(bool isSigned)
        {
            if (isSigned)
            {
                this.DelRedDot(enSysEntryID.GuildBtn);
            }
            else
            {
                this.AddRedDot(enSysEntryID.GuildBtn, enRedDotPos.enTopRight, 0);
            }
        }

        private void OnIDIPClose(CUIEvent uiEvt)
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IDIP_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnIDIPClose));
            this.AutoPopup2_Activity();
        }

        private void OnMailUnReadUpdate()
        {
            int unReadMailCount = Singleton<CMailSys>.instance.GetUnReadMailCount(true);
            if (this.m_LobbyForm != null)
            {
                if (unReadMailCount > 0)
                {
                    this.AddRedDot(enSysEntryID.MailBtn, enRedDotPos.enTopRight, 0);
                }
                else
                {
                    this.DelRedDot(enSysEntryID.MailBtn);
                }
            }
        }

        private void onOpenLobby(CUIEvent uiEvent)
        {
            this.m_LobbyForm = Singleton<CUIManager>.GetInstance().OpenForm(LOBBY_FORM_PATH, false, true);
            this.m_SysEntryForm = Singleton<CUIManager>.GetInstance().OpenForm(SYSENTRY_FORM_PATH, false, true);
            this.m_RankingBtnForm = Singleton<CUIManager>.GetInstance().OpenForm(RANKING_BTN_FORM_PATH, false, true);
            this.m_bInLobby = true;
            this.InitForm(this.m_LobbyForm);
            this.InitRankingBtnForm();
            this.InitOther(this.m_LobbyForm);
            this.InitSysEntryForm(this.m_SysEntryForm);
            this.UpdatePlayerData();
            this.OnFriendSysIconUpdate();
            this.OnTaskUpdate();
            this.ValidateActivitySpot();
            this.OnMailUnReadUpdate();
            this.OnCheckSymbolEquipAlert();
            this.OnCheckUpdateClientVersion();
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.Mall_Entry_Add_RedDotCheck);
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.Mall_Set_Free_Draw_Timer);
            Singleton<CMiShuSystem>.instance.CheckMiShuTalk(true);
            Singleton<CMiShuSystem>.instance.OnCheckFirstWin(null);
            Singleton<CMiShuSystem>.instance.CheckActPlayModeTipsForLobby();
            Singleton<CMiShuSystem>.instance.ShowNewFlagForBeizhanEntry();
            Singleton<CMiShuSystem>.instance.ShowNewFlagForMishuEntry();
            Singleton<CMiShuSystem>.instance.SetNewFlagForUnionBattleEntry(true);
            Singleton<CMiShuSystem>.instance.SetNewFlagForMessageBtnEntry(true);
            Singleton<CMiShuSystem>.instance.SetNewFlagForOBBtn(true);
            if (Singleton<CLoginSystem>.GetInstance().m_fLoginBeginTime > 0f)
            {
                List<KeyValuePair<string, string>> events = new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()),
                    new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()),
                    new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()),
                    new KeyValuePair<string, string>("openid", "NULL")
                };
                float num = Time.time - Singleton<CLoginSystem>.GetInstance().m_fLoginBeginTime;
                events.Add(new KeyValuePair<string, string>("totaltime", num.ToString()));
                events.Add(new KeyValuePair<string, string>("errorCode", "0"));
                events.Add(new KeyValuePair<string, string>("error_msg", "0"));
                Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Login_InLobby", events, true);
                Singleton<CLoginSystem>.GetInstance().m_fLoginBeginTime = 0f;
            }
            if (this.NeedRelogin)
            {
                this.NeedRelogin = false;
                LobbyMsgHandler.PopupRelogin();
            }
        }

        private void onOpenWeb(CUIEvent uiEvent)
        {
            string strUrl = "http://www.qq.com";
            CUICommonSystem.OpenUrl(strUrl, true);
        }

        private void OnPlayerLvlChange()
        {
            if (this.m_LobbyForm != null)
            {
                Transform transform = this.m_LobbyForm.transform;
                Button component = transform.Find("BtnCon/LadderBtn").GetComponent<Button>();
                if (component != null)
                {
                    component.interactable = Singleton<CLadderSystem>.GetInstance().IsLevelQualified();
                    Transform transform2 = component.transform.Find("Lock");
                    if (transform2 != null)
                    {
                        transform2.gameObject.CustomSetActive(!component.interactable);
                    }
                }
                Button button2 = transform.FindChild("BtnCon/UnionBtn").GetComponent<Button>();
                if (button2 != null)
                {
                    bool bActive = Singleton<CUnionBattleEntrySystem>.GetInstance().IsUnionFuncLocked();
                    button2.interactable = !bActive;
                    button2.transform.FindChild("Lock").gameObject.CustomSetActive(bActive);
                }
            }
        }

        private void OnPlayerNameChange()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((this.m_PlayerName != null) && (masterRoleInfo != null))
            {
                this.m_PlayerName.text = masterRoleInfo.Name;
            }
        }

        protected void OnPrepareFight_Origin(CUIEvent uiEvt)
        {
            Transform transform = this.m_LobbyForm.transform.Find("LobbyBottom/SysEntry/ChatBtn_sub");
            Transform transform2 = this.m_LobbyForm.transform.Find("LobbyBottom/SysEntry/ChatBtn");
            if (transform != null)
            {
                transform.gameObject.CustomSetActive(true);
            }
            if (transform2 != null)
            {
                transform2.gameObject.CustomSetActive(false);
            }
            Singleton<CMiShuSystem>.instance.HideNewFlagForBeizhanEntry();
        }

        protected void OnPrepareFight_Sub(CUIEvent uiEvt)
        {
            Transform transform = this.m_LobbyForm.transform.Find("LobbyBottom/SysEntry/ChatBtn_sub");
            Transform transform2 = this.m_LobbyForm.transform.Find("LobbyBottom/SysEntry/ChatBtn");
            if (transform != null)
            {
                transform.gameObject.CustomSetActive(false);
            }
            if (transform2 != null)
            {
                transform2.gameObject.CustomSetActive(true);
            }
        }

        private void OnRankingListElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            if (this.m_rankingType == RankingSystem.RankingSubView.Friend)
            {
                this.OnUpdateRankingFriendElement(srcWidget, srcWidgetIndexInBelongedList);
            }
            else if (this.m_rankingType == RankingSystem.RankingSubView.All)
            {
                this.OnUpdateRankingAllElement(srcWidget, srcWidgetIndexInBelongedList);
            }
        }

        private void OnTaskUpdate()
        {
            CTaskModel model = Singleton<CTaskSys>.instance.model;
            model.task_Data.Sort(RES_TASK_TYPE.RES_TASKTYPE_HEROWAKE);
            model.task_Data.Sort(RES_TASK_TYPE.RES_TASKTYPE_USUAL);
            model.task_Data.Sort(RES_TASK_TYPE.RES_TASKTYPE_MAIN);
            int count = Singleton<CTaskSys>.instance.model.GetMainTask_RedDotCount() + Singleton<CTaskSys>.instance.model.task_Data.GetTask_Count(RES_TASK_TYPE.RES_TASKTYPE_USUAL, CTask.State.Have_Done);
            if (count > 0)
            {
                this.AddRedDot(enSysEntryID.TaskBtn, enRedDotPos.enTopRight, count);
            }
            else
            {
                this.DelRedDot(enSysEntryID.TaskBtn);
            }
        }

        private void OnUpdateRankingAllElement(GameObject go, int index)
        {
            CSDT_RANKING_LIST_SUCC rankList = Singleton<RankingSystem>.instance.GetRankList(RankingSystem.RankingType.Ladder);
            if (rankList != null)
            {
                string serverUrl = string.Empty;
                GameObject headIcon = null;
                GameObject gameObject = null;
                GameObject obj4 = null;
                headIcon = go.transform.Find("HeadIcon").gameObject;
                gameObject = go.transform.Find("HeadbgNo1").gameObject;
                obj4 = go.transform.Find("123No").gameObject;
                serverUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref rankList.astItemDetail[index].stExtraInfo.stDetailInfo.stLadderPoint.szHeadUrl);
                gameObject.CustomSetActive(index == 0);
                RankingView.SetUrlHeadIcon(headIcon, serverUrl);
                obj4.transform.GetChild(0).gameObject.CustomSetActive(0 == index);
                obj4.transform.GetChild(1).gameObject.CustomSetActive(1 == index);
                obj4.transform.GetChild(2).gameObject.CustomSetActive(2 == index);
                int dwHeadIconId = (int) rankList.astItemDetail[index].stExtraInfo.stDetailInfo.stLadderPoint.stGameVip.dwHeadIconId;
                Image component = go.transform.Find("NobeImag").GetComponent<Image>();
                if (component != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component, dwHeadIconId);
                }
                GameObject qQVipIcon = go.transform.Find("QQVipIcon").gameObject;
                this.SetQQVip(qQVipIcon, false, (int) rankList.astItemDetail[index].stExtraInfo.stDetailInfo.stLadderPoint.dwVipLevel);
            }
        }

        private void OnUpdateRankingFriendElement(GameObject go, int index)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            string serverUrl = string.Empty;
            GameObject headIcon = null;
            GameObject gameObject = null;
            GameObject obj4 = null;
            int num = (index <= this.myRankingNo) ? 0 : -1;
            Transform transform = go.transform;
            headIcon = transform.Find("HeadIcon").gameObject;
            gameObject = transform.transform.Find("HeadbgNo1").gameObject;
            obj4 = transform.transform.Find("123No").gameObject;
            int headIdx = 0;
            if (index == this.myRankingNo)
            {
                if (masterRoleInfo != null)
                {
                    serverUrl = masterRoleInfo.HeadUrl;
                    headIdx = (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId;
                    GameObject qQVipIcon = transform.transform.Find("QQVipIcon").gameObject;
                    this.SetQQVip(qQVipIcon, true, 0);
                }
            }
            else if ((index + num) < this.rankFriendList.Count)
            {
                serverUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref this.rankFriendList[index + num].szHeadUrl);
                headIdx = (int) this.rankFriendList[index + num].stGameVip.dwHeadIconId;
                GameObject obj6 = transform.transform.Find("QQVipIcon").gameObject;
                this.SetQQVip(obj6, false, (int) this.rankFriendList[index + num].dwQQVIPMask);
            }
            gameObject.CustomSetActive(index == 0);
            obj4.transform.GetChild(0).gameObject.CustomSetActive(0 == index);
            obj4.transform.GetChild(1).gameObject.CustomSetActive(1 == index);
            obj4.transform.GetChild(2).gameObject.CustomSetActive(2 == index);
            Image component = transform.transform.Find("NobeImag").GetComponent<Image>();
            if (component != null)
            {
                MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component, headIdx);
            }
            RankingView.SetUrlHeadIcon(headIcon, serverUrl);
        }

        private void OpenGuestGameCenterRightForm(CUIEvent uiEvent)
        {
            this.ShowPlatformRight();
        }

        private void OpenIntegralHall(CUIEvent uiEvent)
        {
            string str = "http://jfq.qq.com/comm/index_android.html";
            CUICommonSystem.OpenUrl(string.Format("{0}?partition={1}", str, MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID), true);
        }

        private void OpenQQBuluo(CUIEvent uievent)
        {
            if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
            {
                string strUrl = "http://xiaoqu.qq.com/cgi-bin/bar/qqgame/handle_ticket?redirect_url=http%3A%2F%2Fxiaoqu.qq.com%2Fmobile%2Fbarindex.html%3F%26_bid%3D%26_wv%3D1027%23bid%3D227061";
                CUICommonSystem.OpenUrl(strUrl, true);
            }
        }

        private void OpenQQGameCenterRightForm(CUIEvent uiEvent)
        {
            this.ShowPlatformRight();
        }

        private void OpenQQVIPWealForm(CUIEvent uiEvent)
        {
            string formPath = string.Format("{0}{1}", "UGUI/Form/System/", "IDIPNotice/Form_QQVipPrivilege.prefab");
            CUIFormScript formScript = Singleton<CUIManager>.GetInstance().OpenForm(formPath, false, true);
            if (formScript != null)
            {
                Singleton<QQVipWidget>.instance.SetData(formScript.gameObject, formScript);
            }
        }

        private void OpenWebHome(CUIEvent uiEvent)
        {
            ulong playerUllUID = 0L;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                playerUllUID = masterRoleInfo.playerUllUID;
            }
            string str = "1";
            if (ApolloConfig.platform == ApolloPlatform.QQ)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    str = "1";
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    str = "2";
                }
            }
            else if (ApolloConfig.platform == ApolloPlatform.Wechat)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    str = "3";
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    str = "4";
                }
            }
            CUICommonSystem.OpenUrl(string.Concat(new object[] { "http://yxzj.qq.com/ingame/all/index.shtml?partition=", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID, "&roleid=", playerUllUID, "&area=", str }), true);
        }

        private void OpenWXGameCenterRightForm(CUIEvent uiEvent)
        {
            this.ShowPlatformRight();
        }

        public void Play_UnLock_Animation(RES_SPECIALFUNCUNLOCK_TYPE type)
        {
            string str = string.Empty;
            RES_SPECIALFUNCUNLOCK_TYPE res_specialfuncunlock_type = type;
            switch (res_specialfuncunlock_type)
            {
                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_UNION:
                    str = "SocialBtn";
                    break;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_TASK:
                    str = "TaskBtn";
                    break;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_HERO:
                    str = "HeroBtn";
                    break;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_BAG:
                    str = "BagBtn";
                    break;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_FRIEND:
                    str = "FriendBtn";
                    break;

                case RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL:
                    str = "AddedSkillBtn";
                    break;

                default:
                    if (res_specialfuncunlock_type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL)
                    {
                        str = "SymbolBtn";
                    }
                    break;
            }
            if (!string.IsNullOrEmpty(str))
            {
                stUIEventParams eventParams = new stUIEventParams {
                    tag = (int) type
                };
                CUIAnimatorScript component = Singleton<CUIManager>.instance.OpenForm(LOBBY_FUN_UNLOCK_PATH, false, true).GetComponent<CUIAnimatorScript>();
                component.SetUIEvent(enAnimatorEventType.AnimatorEnd, enUIEventID.Lobby_UnlockAnimation_End, eventParams);
                component.PlayAnimator(str);
                Singleton<CSoundManager>.instance.PostEvent("UI_hall_system_unlock", null);
            }
        }

        private bool PopupNewbieIntro()
        {
            if (CSysDynamicBlock.bNewbieBlocked)
            {
                return true;
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "Master Role info is NULL!");
            if (((masterRoleInfo != null) && !masterRoleInfo.IsNewbieAchieveSet(0x54)) && (Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Newbie/Form_NewbieSettle.prefab") == null))
            {
                masterRoleInfo.SetNewbieAchieve(0x54, true, true);
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Wealfare_CloseForm);
                return true;
            }
            return false;
        }

        private void ProcessQQVIP(CUIFormScript form)
        {
            if (null != form)
            {
                Transform transform = form.transform.Find("Popup/QQVIpBtn");
                GameObject gameObject = null;
                if (transform != null)
                {
                    gameObject = transform.gameObject;
                }
                GameObject obj3 = Utility.FindChild(form.gameObject, "PlayerHead/NameGroup/QQVipIcon");
                if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
                {
                    if (CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        gameObject.CustomSetActive(false);
                        obj3.CustomSetActive(false);
                    }
                    else
                    {
                        gameObject.CustomSetActive(true);
                        if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
                        {
                            MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(obj3.GetComponent<Image>());
                        }
                    }
                }
                else
                {
                    gameObject.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                }
            }
        }

        public static void RefreshDianQuanPayButton(bool notifyFromSvr = false)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(LOBBY_FORM_PATH);
            if (form != null)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                GameObject gameObject = form.transform.Find("DiamondPayBtn").gameObject;
                CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
                CTextManager instance = Singleton<CTextManager>.GetInstance();
                if (!masterRoleInfo.IsGuidedStateSet(0x16))
                {
                    CUICommonSystem.SetButtonName(gameObject, instance.GetText("Pay_Btn_FirstPay"));
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_OpenFirstPayPanel);
                    CUICommonSystem.DelRedDot(gameObject);
                }
                else if (!masterRoleInfo.IsGuidedStateSet(0x17))
                {
                    CUICommonSystem.SetButtonName(gameObject, instance.GetText("Pay_Btn_FirstPay"));
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_OpenFirstPayPanel);
                    CUICommonSystem.AddRedDot(gameObject, enRedDotPos.enTopRight, 0);
                }
                else if (!masterRoleInfo.IsGuidedStateSet(0x18))
                {
                    CUICommonSystem.SetButtonName(gameObject, instance.GetText("Pay_Btn_Renewal"));
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_OpenRenewalPanel);
                    CUICommonSystem.DelRedDot(gameObject);
                }
                else if (!masterRoleInfo.IsGuidedStateSet(0x19))
                {
                    CUICommonSystem.SetButtonName(gameObject, instance.GetText("Pay_Btn_Renewal"));
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_OpenRenewalPanel);
                    CUICommonSystem.AddRedDot(gameObject, enRedDotPos.enTopRight, 0);
                }
                else if (masterRoleInfo.IsClientBitsSet(0))
                {
                    CUICommonSystem.SetButtonName(gameObject, instance.GetText("GotoTehuiShopName"));
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_TehuiShop);
                }
                else if (notifyFromSvr)
                {
                    masterRoleInfo.SetClientBits(0, true, false);
                    RefreshDianQuanPayButton(false);
                }
                else
                {
                    gameObject.CustomSetActive(false);
                }
            }
        }

        private void RefreshRankList()
        {
            if (this._rankingBtn != null)
            {
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(this._rankingBtn, "RankingList");
                int amount = 0;
                if (this.m_rankingType == RankingSystem.RankingSubView.Friend)
                {
                    this.myRankingNo = Singleton<RankingSystem>.instance.GetMyFriendRankNo();
                    if (this.myRankingNo != -1)
                    {
                        this.rankFriendList = Singleton<CFriendContoller>.instance.model.GetSortedRankingFriendList(COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_LADDER_POINT);
                        if (this.rankFriendList != null)
                        {
                            amount = this.rankFriendList.Count + 1;
                            componetInChild.SetElementAmount(amount);
                            CUIListElementScript elemenet = null;
                            GameObject go = null;
                            for (int i = 0; i < amount; i++)
                            {
                                elemenet = componetInChild.GetElemenet(i);
                                if (elemenet != null)
                                {
                                    go = elemenet.gameObject;
                                    if (go != null)
                                    {
                                        this.OnUpdateRankingFriendElement(go, i);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (this.m_rankingType == RankingSystem.RankingSubView.All)
                {
                    CSDT_RANKING_LIST_SUCC rankList = Singleton<RankingSystem>.instance.GetRankList(RankingSystem.RankingType.Ladder);
                    if (rankList != null)
                    {
                        amount = (int) rankList.dwItemNum;
                        componetInChild.SetElementAmount(amount);
                        CUIListElementScript script3 = null;
                        GameObject gameObject = null;
                        for (int j = 0; j < amount; j++)
                        {
                            script3 = componetInChild.GetElemenet(j);
                            if (script3 != null)
                            {
                                gameObject = script3.gameObject;
                                if (gameObject != null)
                                {
                                    this.OnUpdateRankingAllElement(gameObject, j);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RefreshRankList(RankingSystem.RankingSubView rankingType)
        {
            this.m_rankingType = rankingType;
            this.RefreshRankList();
        }

        private void SetEnable(RES_SPECIALFUNCUNLOCK_TYPE type, bool bShow)
        {
            GameObject obj2 = null;
            if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_HERO)
            {
                obj2 = this.hero_btn;
            }
            else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL)
            {
                obj2 = this.symbol_btn;
            }
            else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_BAG)
            {
                obj2 = this.bag_btn;
            }
            else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_TASK)
            {
                obj2 = this.task_btn;
            }
            else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_UNION)
            {
                obj2 = this.social_btn;
            }
            else if (type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL)
            {
                obj2 = this.addSkill_btn;
            }
            else
            {
                obj2 = null;
            }
            if (obj2 != null)
            {
                obj2.CustomSetActive(bShow);
            }
        }

        private void SetQQVip(GameObject QQVipIcon, bool bSelf, int mask = 0)
        {
            if (QQVipIcon != null)
            {
                if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
                {
                    if (CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        QQVipIcon.CustomSetActive(false);
                    }
                    else if (bSelf)
                    {
                        if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
                        {
                            MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(QQVipIcon.GetComponent<Image>());
                        }
                    }
                    else
                    {
                        MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(QQVipIcon.GetComponent<Image>(), mask);
                    }
                }
                else
                {
                    QQVipIcon.CustomSetActive(false);
                }
            }
        }

        public void SetTopBarPriority(enFormPriority prioRity)
        {
            if (this.m_SysEntryForm != null)
            {
                CUIFormScript component = this.m_SysEntryForm.GetComponent<CUIFormScript>();
                if (component != null)
                {
                    component.SetPriority(prioRity);
                }
            }
        }

        public void ShowHideRankingBtn(bool show)
        {
            if (this._rankingBtn != null)
            {
                if (CSysDynamicBlock.bSocialBlocked)
                {
                    this._rankingBtn.CustomSetActive(false);
                }
                else
                {
                    this._rankingBtn.CustomSetActive(show);
                }
            }
        }

        private void ShowMoreBtn(CUIEvent uiEvent)
        {
            if (this.m_LobbyForm != null)
            {
                Transform transform = this.m_LobbyForm.transform.Find("Popup/MoreCon");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(true);
                }
                Transform transform2 = this.m_LobbyForm.transform.Find("Popup/MoreBtn");
                if (transform2 != null)
                {
                    transform2.gameObject.CustomSetActive(false);
                }
            }
        }

        public void ShowOrHideWifiInfo()
        {
            if (this.m_wifiInfo != null)
            {
                this.m_wifiInfo.CustomSetActive(!this.m_wifiInfo.activeInHierarchy);
            }
            this.CheckWifi();
        }

        private void ShowPlatformRight()
        {
            if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Guest)
            {
                string formPath = string.Format("{0}{1}", "UGUI/Form/System/", "GameCenter/Form_GuestGameCenter.prefab");
                Singleton<CUIManager>.GetInstance().OpenForm(formPath, false, true);
            }
            else if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
            {
                string str2 = string.Format("{0}{1}", "UGUI/Form/System/", "GameCenter/Form_QQGameCenter.prefab");
                Singleton<CUIManager>.GetInstance().OpenForm(str2, false, true);
            }
            else if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat)
            {
                string str3 = string.Format("{0}{1}", "UGUI/Form/System/", "GameCenter/Form_WXGameCenter.prefab");
                Singleton<CUIManager>.GetInstance().OpenForm(str3, false, true);
            }
        }

        private void StartAutoPopupChain(int timerSeq)
        {
            AutoPopAllow &= !MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding;
            if (AutoPopAllow)
            {
                this.AutoPopup1_IDIP();
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OpenLobbyForm, new CUIEventManager.OnUIEventHandler(this.onOpenLobby));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.WEB_OpenURL, new CUIEventManager.OnUIEventHandler(this.onOpenWeb));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_WifiCheckTimer, new CUIEventManager.OnUIEventHandler(this.onCommon_WifiCheckTimer));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_ShowOrHideWifiInfo, new CUIEventManager.OnUIEventHandler(this.onCommon_ShowOrHideWifiInfo));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_LobbyFormShow, new CUIEventManager.OnUIEventHandler(this.Lobby_LobbyFormShow));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_LobbyFormHide, new CUIEventManager.OnUIEventHandler(this.Lobby_LobbyFormHide));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_ConfirmErrExit, new CUIEventManager.OnUIEventHandler(this.onErrorExit));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_Close, new CUIEventManager.OnUIEventHandler(this.OnCloseForm));
            Singleton<EventRouter>.instance.RemoveEventHandler("MasterAttributesChanged", new System.Action(this.UpdatePlayerData));
            Singleton<EventRouter>.instance.RemoveEventHandler("TaskUpdated", new System.Action(this.OnTaskUpdate));
            Singleton<EventRouter>.instance.RemoveEventHandler("Friend_LobbyIconRedDot_Refresh", new System.Action(this.OnFriendSysIconUpdate));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Entry_Add_RedDotCheck, new System.Action(this.OnCheckAddMallEntryRedDot));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.Mall_Entry_Del_RedDotCheck, new System.Action(this.OnCheckDelMallEntryRedDot));
            Singleton<EventRouter>.instance.RemoveEventHandler("MailUnReadNumUpdate", new System.Action(this.OnMailUnReadUpdate));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.ACHIEVE_STATE_UPDATE, new System.Action(this.OnAchieveStateUpdate));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.SymbolEquipSuc, new System.Action(this.OnCheckSymbolEquipAlert));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BAG_ITEMS_UPDATE, new System.Action(this.OnCheckSymbolEquipAlert));
            Singleton<EventRouter>.instance.RemoveEventHandler("MasterPvpLevelChanged", new System.Action(this.OnCheckSymbolEquipAlert));
            Singleton<ActivitySys>.GetInstance().OnStateChange -= new ActivitySys.StateChangeDelegate(this.ValidateActivitySpot);
            Singleton<EventRouter>.instance.RemoveEventHandler("IDIPNOTICE_UNREAD_NUM_UPDATE", new System.Action(this.ValidateActivitySpot));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.IDIP_QQVIP_OpenWealForm, new CUIEventManager.OnUIEventHandler(this.OpenQQVIPWealForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.WEB_OpenHome, new CUIEventManager.OnUIEventHandler(this.OpenWebHome));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.WEB_IntegralHall, new CUIEventManager.OnUIEventHandler(this.OpenIntegralHall));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OPEN_QQ_Buluo, new CUIEventManager.OnUIEventHandler(this.OpenQQBuluo));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_ShowMoreBtn, new CUIEventManager.OnUIEventHandler(this.ShowMoreBtn));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_HideMoreBtn, new CUIEventManager.OnUIEventHandler(this.HideMoreBtn));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_UnlockAnimation_End, new CUIEventManager.OnUIEventHandler(this.On_Lobby_UnlockAnimation_End));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_MysteryShopClose, new CUIEventManager.OnUIEventHandler(this.On_Lobby_MysteryShopClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.GameCenter_OpenWXRight, new CUIEventManager.OnUIEventHandler(this.OpenWXGameCenterRightForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_RankingListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnRankingListElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.GameCenter_OpenGuestRight, new CUIEventManager.OnUIEventHandler(this.OpenGuestGameCenterRightForm));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.NOBE_STATE_CHANGE, new System.Action(this.UpdateNobeIcon));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.NOBE_STATE_HEAD_CHANGE, new System.Action(this.UpdateNobeHeadIdx));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("MasterPvpLevelChanged", new System.Action(this.OnPlayerLvlChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("Rank_Friend_List", new System.Action(this.RefreshRankList));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<RankingSystem.RankingSubView>("Rank_List", new Action<RankingSystem.RankingSubView>(this.RefreshRankList));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.HEAD_IMAGE_FLAG_CHANGE, new System.Action(this.UpdatePlayerData));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("CheckNewbieIntro", new System.Action(this.OnCheckNewbieIntro));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("VipInfoHadSet", new System.Action(this.UpdateQQVIPState));
        }

        private void UnInitWidget()
        {
            this._rankingBtn = null;
        }

        private void UpdateGameCenterState(CUIFormScript form)
        {
            if (null != form)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                COM_PRIVILEGE_TYPE privilegeType = (masterRoleInfo != null) ? masterRoleInfo.m_privilegeType : COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
                GameObject obj2 = Utility.FindChild(form.gameObject, "WXGameCenterBtn");
                GameObject obj3 = Utility.FindChild(form.gameObject, "PlayerHead/NameGroup/WXGameCenterIcon");
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj2, privilegeType, ApolloPlatform.Wechat, false, CSysDynamicBlock.bLobbyEntryBlocked);
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj3, privilegeType, ApolloPlatform.Wechat, true, false);
                GameObject obj4 = Utility.FindChild(form.gameObject, "QQGameCenterBtn");
                GameObject obj5 = Utility.FindChild(form.gameObject, "PlayerHead/NameGroup/QQGameCenterIcon");
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj4, privilegeType, ApolloPlatform.QQ, false, CSysDynamicBlock.bLobbyEntryBlocked);
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj5, privilegeType, ApolloPlatform.QQ, true, false);
                GameObject obj6 = Utility.FindChild(form.gameObject, "GuestGameCenterBtn");
                GameObject obj7 = Utility.FindChild(form.gameObject, "PlayerHead/NameGroup/GuestGameCenterIcon");
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj6, privilegeType, ApolloPlatform.Guest, false, CSysDynamicBlock.bLobbyEntryBlocked);
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(obj7, privilegeType, ApolloPlatform.Guest, true, false);
            }
        }

        private void UpdateNobeHeadIdx()
        {
            int dwHeadIconId = (int) MonoSingleton<NobeSys>.GetInstance().m_vipInfo.stGameVipClient.dwHeadIconId;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (((this.m_LobbyForm != null) && this.m_LobbyForm.gameObject.activeSelf) && (masterRoleInfo != null))
            {
                Image component = this.m_LobbyForm.GetWidget(3).GetComponent<Image>();
                if (component != null)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component, dwHeadIconId);
                }
                this.RefreshRankList();
            }
        }

        private void UpdateNobeIcon()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (((this.m_LobbyForm != null) && this.m_LobbyForm.gameObject.activeSelf) && (masterRoleInfo != null))
            {
                GameObject widget = this.m_LobbyForm.GetWidget(2);
                if (widget != null)
                {
                    CUIHttpImageScript component = widget.GetComponent<CUIHttpImageScript>();
                    MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.GetComponent<Image>(), (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwCurLevel, false);
                    Image image = this.m_LobbyForm.GetWidget(3).GetComponent<Image>();
                    MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image, (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId);
                }
            }
        }

        private void UpdatePlayerData()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if ((masterRoleInfo != null) && (this.m_LobbyForm != null))
            {
                if (this.m_PlayerName != null)
                {
                    this.m_PlayerName.text = masterRoleInfo.Name;
                }
                if (this.m_PlayerExp != null)
                {
                    this.m_PlayerExp.text = masterRoleInfo.Level.ToString();
                }
                if (this.m_PvpExpImg != null)
                {
                    this.m_PvpExpImg.CustomFillAmount(CPlayerProfile.Divide(masterRoleInfo.PvpExp, masterRoleInfo.PvpNeedExp));
                    this.m_PvpExpTxt.text = masterRoleInfo.PvpExp + "/" + masterRoleInfo.PvpNeedExp;
                }
                if (this.m_CreditScore != null)
                {
                    this.m_CreditScore.text = masterRoleInfo.creditScore.ToString();
                }
                if (this.m_PvpLevel != null)
                {
                    string text = Singleton<CTextManager>.GetInstance().GetText("ranking_PlayerLevel");
                    if ((!string.IsNullOrEmpty(text) && (this.m_PvpLevel.text != null)) && (masterRoleInfo != null))
                    {
                        this.m_PvpLevel.text = string.Format(text, masterRoleInfo.PvpLevel);
                    }
                }
                if (!CSysDynamicBlock.bSocialBlocked)
                {
                    if ((this.m_PlayerVipLevel != null) && (masterRoleInfo != null))
                    {
                        this.m_PlayerVipLevel.text = string.Format("VIP{0}", masterRoleInfo.m_payLevel);
                    }
                    if (((this.m_LobbyForm != null) && this.m_LobbyForm.gameObject.activeSelf) && (masterRoleInfo != null))
                    {
                        GameObject widget = this.m_LobbyForm.GetWidget(2);
                        if ((widget != null) && !string.IsNullOrEmpty(masterRoleInfo.HeadUrl))
                        {
                            CUIHttpImageScript component = widget.GetComponent<CUIHttpImageScript>();
                            component.SetImageUrl(masterRoleInfo.HeadUrl);
                            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.GetComponent<Image>(), (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwCurLevel, false);
                            Image image = this.m_LobbyForm.GetWidget(3).GetComponent<Image>();
                            MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image, (int) masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId);
                            bool flag = Singleton<HeadIconSys>.instance.UnReadFlagNum > 0;
                            GameObject target = Utility.FindChild(widget, "RedDot");
                            if (target != null)
                            {
                                if (flag)
                                {
                                    CUICommonSystem.AddRedDot(target, enRedDotPos.enTopRight, 0);
                                }
                                else
                                {
                                    CUICommonSystem.DelRedDot(target);
                                }
                            }
                        }
                    }
                }
                else if ((this.m_LobbyForm != null) && this.m_LobbyForm.gameObject.activeSelf)
                {
                    GameObject obj4 = this.m_LobbyForm.GetWidget(2);
                    if (obj4 != null)
                    {
                        CUIHttpImageScript script2 = obj4.GetComponent<CUIHttpImageScript>();
                        if (script2 != null)
                        {
                            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(script2.GetComponent<Image>(), 0, false);
                        }
                    }
                }
                if (this.m_lblGlodCoin != null)
                {
                    this.m_lblGlodCoin.text = this.GetCoinString(masterRoleInfo.GoldCoin);
                }
                if (this.m_lblDianquan != null)
                {
                    this.m_lblDianquan.text = this.GetCoinString((uint) masterRoleInfo.DianQuan);
                }
                if (this.m_lblDiamond != null)
                {
                    this.m_lblDiamond.text = this.GetCoinString(masterRoleInfo.Diamond);
                }
            }
        }

        private void UpdateQQVIPState()
        {
            if (this.m_bInLobby && (this.m_LobbyForm != null))
            {
                Transform transform = this.m_LobbyForm.transform;
                Transform transform2 = transform.Find("Popup/QQVIpBtn");
                GameObject gameObject = null;
                if (transform2 != null)
                {
                    gameObject = transform2.gameObject;
                }
                Transform transform3 = transform.Find("PlayerHead/QQVipIcon");
                Transform transform4 = transform.Find("PlayerHead/QQSVipIcon");
                GameObject obj3 = null;
                GameObject obj4 = null;
                if (transform3 != null)
                {
                    obj3 = transform3.gameObject;
                }
                if (transform4 != null)
                {
                    obj4 = transform4.gameObject;
                }
                if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
                {
                    if (CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        gameObject.CustomSetActive(false);
                        obj3.CustomSetActive(false);
                        obj4.CustomSetActive(false);
                    }
                    else
                    {
                        gameObject.CustomSetActive(true);
                        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                        if (masterRoleInfo != null)
                        {
                            if (masterRoleInfo.HasVip(0x10))
                            {
                                obj3.CustomSetActive(false);
                                obj4.CustomSetActive(true);
                            }
                            else if (masterRoleInfo.HasVip(1))
                            {
                                obj3.CustomSetActive(true);
                                obj4.CustomSetActive(false);
                            }
                            else
                            {
                                obj3.CustomSetActive(false);
                                obj4.CustomSetActive(false);
                            }
                        }
                    }
                }
                else
                {
                    gameObject.CustomSetActive(false);
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                }
            }
        }

        public void ValidateActivitySpot()
        {
            if (this.m_bInLobby)
            {
                if (Singleton<ActivitySys>.GetInstance().CheckReadyForDot(RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_ACTIVITY))
                {
                    uint reveivableRedDot = Singleton<ActivitySys>.GetInstance().GetReveivableRedDot(RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_ACTIVITY);
                    this.AddRedDotEx(enSysEntryID.ActivityBtn, enRedDotPos.enTopRight, (int) reveivableRedDot);
                }
                else if (MonoSingleton<IDIPSys>.GetInstance().HaveUpdateList)
                {
                    this.AddRedDotEx(enSysEntryID.ActivityBtn, enRedDotPos.enTopRight, 0);
                }
                else if (MonoSingleton<PandroaSys>.GetInstance().ShowRedPoint)
                {
                    this.AddRedDotEx(enSysEntryID.ActivityBtn, enRedDotPos.enTopRight, 0);
                }
                else
                {
                    this.DelRedDot(enSysEntryID.ActivityBtn);
                }
            }
        }

        public static bool IsPlatChannelOpen
        {
            [CompilerGenerated]
            get
            {
                return <IsPlatChannelOpen>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <IsPlatChannelOpen>k__BackingField = value;
            }
        }

        public enum enSysEntryFormWidget
        {
            WifiIcon,
            WifiInfo,
            WifiPing,
            GlodCoin,
            Dianquan,
            MailBtn,
            SettingBtn,
            Wifi_Bg,
            FriendBtn,
            MianLiuTxt
        }

        public enum LobbyFormWidget
        {
            HeadImgBack = 3,
            LoudSpeakerRolling = 5,
            LoudSpeakerRollingBg = 6,
            None = -1,
            PlatChannel = 7,
            PlatChannelText = 8,
            RankingBtn = 1,
            Reserve = 0,
            Rolling = 4,
            SnsHead = 2
        }

        public enum LobbyRankingBtnFormWidget
        {
            RankingBtnPanel
        }
    }
}

