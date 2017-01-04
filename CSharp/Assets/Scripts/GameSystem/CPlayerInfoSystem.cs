namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CPlayerInfoSystem : Singleton<CPlayerInfoSystem>
    {
        private bool _isShowGuildAppointViceChairmanBtn;
        private bool _isShowGuildFireMemberBtn;
        private bool _isShowGuildTransferPositionBtn;
        private DetailPlayerInfoSource _lastDetailSource;
        public const ushort CREDIT_RULE_ID = 11;
        public bool isShowPlayerInfo = true;
        private Tab m_CurTab;
        private CUIFormScript m_Form;
        private bool m_IsFormOpen;
        private CPlayerProfile m_PlayerProfile = new CPlayerProfile();
        public const ushort PlAYER_INFO_RULE_ID = 3;
        public string sPlayerInfoFormPath = "UGUI/Form/System/Player/Form_Player_Info.prefab";

        [MessageHandler(0x4aa)]
        public static void ChangePersonSgin(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stSignatureRsp.dwResult != 0)
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox(Utility.ProtErrCodeToStr(0x4aa, (int) msg.stPkgData.stSignatureRsp.dwResult), false);
            }
            else
            {
                if (Singleton<CPlayerInfoSystem>.GetInstance().CurTab == Tab.Base_Info)
                {
                    Singleton<CPlayerInfoSystem>.GetInstance().UpdateBaseInfo();
                }
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    masterRoleInfo.PersonSign = Singleton<CPlayerInfoSystem>.instance.m_PlayerProfile.m_personSign;
                }
            }
        }

        private void DeepLinkClick(CUIEvent uiEvent)
        {
            if ((ApolloConfig.platform == ApolloPlatform.Wechat) && MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.bLoadSucc)
            {
                Debug.Log(string.Concat(new object[] { "deeplink ", MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.linkType, " ", MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.linkUrl }));
                Singleton<ApolloHelper>.GetInstance().OpenWeiXinDeeplink(MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.linkType, MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.linkUrl);
            }
        }

        private void DisplayCustomButton()
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                GameObject widget = this.m_Form.GetWidget(1);
                if ((widget != null) && widget.activeSelf)
                {
                    GameObject obj3 = Utility.FindChild(widget, "pnlContainer/pnlHead/btnRename");
                    GameObject obj4 = Utility.FindChild(widget, "pnlContainer/pnlHead/btnShare");
                    switch (this._lastDetailSource)
                    {
                        case DetailPlayerInfoSource.DefaultOthers:
                            obj3.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            this.SetBaseInfoScrollable(false);
                            this.SetAllGuildBtnActive(widget, false);
                            this.SetAllFriendBtnActive(widget, false);
                            break;

                        case DetailPlayerInfoSource.Self:
                            obj3.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            this.SetBaseInfoScrollable(false);
                            this.SetAllGuildBtnActive(widget, false);
                            this.SetAllFriendBtnActive(widget, false);
                            break;

                        case DetailPlayerInfoSource.Guild:
                            obj3.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            this.SetBaseInfoScrollable(false);
                            this.SetSingleGuildBtnActive(widget);
                            this.SetAllFriendBtnActive(widget, false);
                            break;

                        case DetailPlayerInfoSource.Friend:
                            obj3.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            this.SetBaseInfoScrollable(false);
                            this.SetAllGuildBtnActive(widget, false);
                            this.SetAllFriendBtnActive(widget, true);
                            break;
                    }
                }
            }
        }

        public CPlayerProfile GetProfile()
        {
            return this.m_PlayerProfile;
        }

        public void ImpResDetailInfo(CSPkg msg)
        {
            if (msg.stPkgData.stGetAcntDetailInfoRsp.iErrCode != 0)
            {
                Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format("Error Code {0}", msg.stPkgData.stGetAcntDetailInfoRsp.iErrCode), false);
            }
            else
            {
                this.m_PlayerProfile.ConvertServerDetailData(msg.stPkgData.stGetAcntDetailInfoRsp.stAcntDetail.stOfSucc);
                this.OpenForm();
            }
        }

        public override void Init()
        {
            base.Init();
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.AddUIEventListener(enUIEventID.Player_Info_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfo_OpenForm));
            instance.AddUIEventListener(enUIEventID.Player_Info_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfo_CloseForm));
            instance.AddUIEventListener(enUIEventID.Player_Info_Tab_Change, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoTabChange));
            instance.AddUIEventListener(enUIEventID.Player_Info_Open_Pvp_Info, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoOpenPvpInfo));
            instance.AddUIEventListener(enUIEventID.Player_Info_Open_Base_Info, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoOpenBaseInfo));
            instance.AddUIEventListener(enUIEventID.Player_Info_Quit_Game, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoQuitGame));
            instance.AddUIEventListener(enUIEventID.Player_Info_Quit_Game_Confirm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoQuitGameConfirm));
            instance.AddUIEventListener(enUIEventID.Player_Info_Most_Used_Hero_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoMostUsedHeroItemEnable));
            instance.AddUIEventListener(enUIEventID.Player_Info_Most_Used_Hero_Item_Click, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoMostUsedHeroItemClick));
            instance.AddUIEventListener(enUIEventID.Player_Info_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoShowRule));
            instance.AddUIEventListener(enUIEventID.Player_Info_License_ListItem_Enable, new CUIEventManager.OnUIEventHandler(this.OnLicenseListItemEnable));
            instance.AddUIEventListener(enUIEventID.Player_Info_Common_Hero_Enable, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroItemEnable));
            instance.AddUIEventListener(enUIEventID.Player_Info_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
            this.m_IsFormOpen = false;
            this.m_CurTab = Tab.Base_Info;
            this.m_Form = null;
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_QQ_VIP, new CUIEventManager.OnUIEventHandler(this.OpenByQQVIP));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.DeepLink_OnClick, new CUIEventManager.OnUIEventHandler(this.DeepLinkClick));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NOBE_STATE_CHANGE, new System.Action(this.UpdateNobeHeadIdx));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.HEAD_IMAGE_FLAG_CHANGE, new System.Action(this.UpdateHeadFlag));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NAMECHANGE_PLAYER_NAME_CHANGE, new System.Action(this.OnPlayerNameChange));
        }

        private void InitTab()
        {
            if ((this.m_Form != null) && this.m_IsFormOpen)
            {
                GameObject widget = this.m_Form.GetWidget(1);
                if ((widget != null) && widget.activeSelf)
                {
                    widget.CustomSetActive(false);
                }
                GameObject obj3 = this.m_Form.GetWidget(2);
                if ((obj3 != null) && obj3.activeSelf)
                {
                    obj3.CustomSetActive(false);
                }
                Tab[] values = (Tab[]) Enum.GetValues(typeof(Tab));
                string[] strArray = new string[values.Length];
                for (byte i = 0; i < values.Length; i = (byte) (i + 1))
                {
                    switch (values[i])
                    {
                        case Tab.Base_Info:
                            strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Base_Info");
                            break;

                        case Tab.Pvp_Info:
                            strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Pvp_Info");
                            break;

                        case Tab.Honor_Info:
                            strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Honor_Info");
                            break;

                        case Tab.Common_Hero:
                            strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Common_Hero_Info");
                            break;

                        case Tab.PvpHistory_Info:
                            strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_PvpHistory_Info");
                            break;

                        case Tab.PvpCreditScore_Info:
                            strArray[i] = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Credit_Info");
                            break;
                    }
                }
                CUIListScript component = this.m_Form.GetWidget(0).GetComponent<CUIListScript>();
                if (component != null)
                {
                    component.SetElementAmount(strArray.Length);
                    for (int j = 0; j < component.m_elementAmount; j++)
                    {
                        component.GetElemenet(j).gameObject.transform.Find("Text").GetComponent<Text>().text = strArray[j];
                    }
                    component.m_alwaysDispatchSelectedChangeEvent = true;
                    component.SelectElement((int) this.CurTab, true);
                }
                Singleton<CMiShuSystem>.GetInstance().ShowNewFlag(component.GetElemenet(2).gameObject, enNewFlagKey.New_HonorInfo_V1);
                Singleton<CMiShuSystem>.GetInstance().ShowNewFlag(component.GetElemenet(4).gameObject, enNewFlagKey.New_PvpHistoryInfo_V1);
            }
        }

        private bool isSelf(ulong playerUllUID)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            return ((masterRoleInfo != null) && (masterRoleInfo.playerUllUID == playerUllUID));
        }

        public void LoadSubModule()
        {
            DebugHelper.Assert(this.m_Form != null, "Player Form Is Null");
            if (this.m_Form != null)
            {
                bool flag = false;
                GameObject widget = this.m_Form.GetWidget(10);
                GameObject obj3 = this.m_Form.GetWidget(9);
                if ((widget != null) && (obj3 != null))
                {
                    switch (this.m_CurTab)
                    {
                        case Tab.Honor_Info:
                            flag = Singleton<CPlayerHonorController>.GetInstance().Loaded(this.m_Form);
                            if (!flag)
                            {
                                widget.CustomSetActive(true);
                                Singleton<CPlayerHonorController>.GetInstance().Load(this.m_Form);
                                obj3.CustomSetActive(false);
                            }
                            break;

                        case Tab.PvpHistory_Info:
                            flag = Singleton<CPlayerPvpHistoryController>.GetInstance().Loaded(this.m_Form);
                            if (!flag)
                            {
                                widget.CustomSetActive(true);
                                Singleton<CPlayerPvpHistoryController>.GetInstance().Load(this.m_Form);
                                obj3.CustomSetActive(false);
                            }
                            break;
                    }
                }
                if (!flag)
                {
                    GameObject obj4 = this.m_Form.GetWidget(11);
                    if (obj4 != null)
                    {
                        CUITimerScript component = obj4.GetComponent<CUITimerScript>();
                        if (component != null)
                        {
                            component.ReStartTimer();
                        }
                    }
                }
                else
                {
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Player_Info_Update_Sub_Module);
                }
            }
        }

        private void OnCommonHeroItemEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject p = Utility.FindChild(uiEvent.m_srcWidget, "heroItem");
            ListView<COMDT_MOST_USED_HERO_INFO> view = this.m_PlayerProfile.MostUsedHeroList();
            if ((view != null) && (srcWidgetIndexInBelongedList < view.Count))
            {
                COMDT_MOST_USED_HERO_INFO comdt_most_used_hero_info = view[srcWidgetIndexInBelongedList];
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    IHeroData data = CHeroDataFactory.CreateHeroData(comdt_most_used_hero_info.dwHeroID);
                    GameObject proficiencyIcon = Utility.FindChild(p, "heroProficiencyImg");
                    GameObject proficiencyBg = Utility.FindChild(p, "heroProficiencyBgImg");
                    CUICommonSystem.SetHeroProficiencyIconImage(uiEvent.m_srcFormScript, proficiencyIcon, (int) comdt_most_used_hero_info.dwProficiencyLv);
                    CUICommonSystem.SetHeroProficiencyBgImage(uiEvent.m_srcFormScript, proficiencyBg, (int) comdt_most_used_hero_info.dwProficiencyLv, false);
                    if (!this.isSelf(this.m_PlayerProfile.m_uuid))
                    {
                        CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, p, CSkinInfo.GetHeroSkinPic(comdt_most_used_hero_info.dwHeroID, 0), enHeroHeadType.enBust, false);
                    }
                    else
                    {
                        CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, p, masterRoleInfo.GetHeroSkinPic(comdt_most_used_hero_info.dwHeroID), enHeroHeadType.enBust, false);
                    }
                    GameObject root = Utility.FindChild(p, "profession");
                    CUICommonSystem.SetHeroJob(uiEvent.m_srcFormScript, root, (enHeroJobType) data.heroType);
                    Utility.GetComponetInChild<Text>(p, "heroNameText").text = data.heroName;
                    string[] args = new string[] { (comdt_most_used_hero_info.dwGameWinNum + comdt_most_used_hero_info.dwGameLoseNum).ToString() };
                    Utility.GetComponetInChild<Text>(p, "TotalCount").text = Singleton<CTextManager>.instance.GetText("Player_Info_PVP_Total_Count", args);
                    string[] textArray2 = new string[] { CPlayerProfile.Round(CPlayerProfile.Divide(comdt_most_used_hero_info.dwGameWinNum, comdt_most_used_hero_info.dwGameWinNum + comdt_most_used_hero_info.dwGameLoseNum) * 100f) };
                    Utility.GetComponetInChild<Text>(p, "WinRate").text = Singleton<CTextManager>.instance.GetText("Player_Info_PVP_Win_Rate", textArray2);
                    ulong num2 = 0L;
                    ulong num3 = 0L;
                    ulong num4 = 0L;
                    uint num5 = 0;
                    COMDT_HERO_STATISTIC_DETAIL stStatisticDetail = comdt_most_used_hero_info.stStatisticDetail;
                    uint dwNum = stStatisticDetail.dwNum;
                    for (int i = 0; i < dwNum; i++)
                    {
                        COMDT_HERO_STATISTIC_INFO comdt_hero_statistic_info = stStatisticDetail.astTypeDetail[i];
                        if (((comdt_hero_statistic_info.bGameType == 4) || (comdt_hero_statistic_info.bGameType == 5)) && (comdt_hero_statistic_info.bMapAcntNum == 10))
                        {
                            num2 += comdt_hero_statistic_info.ullKDAPct;
                            num3 += comdt_hero_statistic_info.ullTotalHurtHero;
                            num4 += comdt_hero_statistic_info.ullTotalBeHurt;
                            num5 = (num5 + comdt_hero_statistic_info.dwWinNum) + comdt_hero_statistic_info.dwLoseNum;
                        }
                    }
                    num5 = (num5 != 0) ? num5 : 1;
                    string[] textArray3 = new string[1];
                    textArray3[0] = ((num2 / ((ulong) num5)) / ((ulong) 100L)).ToString("0.0");
                    Utility.GetComponetInChild<Text>(p, "AverKDA").text = Singleton<CTextManager>.instance.GetText("Player_Info_PVP_AverKDA", textArray3);
                    string[] textArray4 = new string[1];
                    textArray4[0] = (num3 / ((ulong) num5)).ToString("d");
                    Utility.GetComponetInChild<Text>(p, "AverHurt").text = Singleton<CTextManager>.instance.GetText("Player_Info_PVP_AverHurt", textArray4);
                    string[] textArray5 = new string[1];
                    textArray5[0] = (num4 / ((ulong) num5)).ToString("d");
                    Utility.GetComponetInChild<Text>(p, "AverTakenHurt").text = Singleton<CTextManager>.instance.GetText("Player_Info_PVP_AverTakenHurt", textArray5);
                }
            }
        }

        private void OnLicenseListItemEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo != null) && (masterRoleInfo.m_licenseInfo != null))
            {
                CLicenseItem licenseItemByIndex = masterRoleInfo.m_licenseInfo.GetLicenseItemByIndex(srcWidgetIndexInBelongedList);
                if (((srcWidget != null) && (licenseItemByIndex != null)) && (licenseItemByIndex.m_resLicenseInfo != null))
                {
                    srcWidget.transform.Find("licenseIcon").GetComponent<Image>().SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Task_Dir, licenseItemByIndex.m_resLicenseInfo.szIconPath), this.m_Form, true, false, false);
                    srcWidget.transform.Find("licenseNameText").GetComponent<Text>().text = licenseItemByIndex.m_resLicenseInfo.szLicenseName;
                    Transform transform3 = srcWidget.transform.Find("licenseStateText");
                    if (licenseItemByIndex.m_getSecond > 0)
                    {
                        DateTime time = Utility.ToUtcTime2Local((long) licenseItemByIndex.m_getSecond);
                        transform3.GetComponent<Text>().text = string.Format("<color=#00d519>{0}/{1}/{2}</color>", time.Year, time.Month, time.Day);
                    }
                    else
                    {
                        transform3.GetComponent<Text>().text = "<color=#fecb2f>未获得</color>";
                    }
                    srcWidget.transform.Find("licenseDescText").GetComponent<Text>().text = licenseItemByIndex.m_resLicenseInfo.szDesc;
                }
            }
        }

        private void OnPersonSignEndEdit(string personSign)
        {
            if (string.Compare(personSign, this.m_PlayerProfile.m_personSign) != 0)
            {
                this.m_PlayerProfile.m_personSign = personSign;
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x4a9);
                StringHelper.StringToUTF8Bytes(personSign, ref msg.stPkgData.stSignatureReq.szSignatureInfo);
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        private void OnPlayerInfo_CloseForm(CUIEvent uiEvent)
        {
            if (this.m_IsFormOpen)
            {
                this.m_IsFormOpen = false;
                Singleton<CUIManager>.GetInstance().CloseForm(this.sPlayerInfoFormPath);
                this.m_Form = null;
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.PlayerInfoSystem_Form_Close);
            }
        }

        private void OnPlayerInfo_OpenForm(CUIEvent uiEvent)
        {
            this.ShowPlayerDetailInfo(0L, 0, DetailPlayerInfoSource.Self);
            CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_HeroHeadBtn);
        }

        private void OnPlayerInfoMostUsedHeroItemClick(CUIEvent uiEvent)
        {
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Player_Info_CloseForm, uiEvent.m_eventParams);
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.HeroInfo_OpenForm, uiEvent.m_eventParams);
        }

        private void OnPlayerInfoMostUsedHeroItemEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            if (srcWidget != null)
            {
                GameObject gameObject = srcWidget.transform.Find("heroItem").gameObject;
                ListView<COMDT_MOST_USED_HERO_INFO> view = this.m_PlayerProfile.MostUsedHeroList();
                if (srcWidgetIndexInBelongedList < view.Count)
                {
                    COMDT_MOST_USED_HERO_INFO heroInfo = view[srcWidgetIndexInBelongedList];
                    this.SetHeroItemData(uiEvent.m_srcFormScript, gameObject, heroInfo);
                    Text componetInChild = Utility.GetComponetInChild<Text>(srcWidget, "usedCnt");
                    if (componetInChild != null)
                    {
                        componetInChild.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Games_Cnt_Label"), heroInfo.dwGameWinNum + heroInfo.dwGameLoseNum);
                    }
                }
            }
        }

        private void OnPlayerInfoOpenBaseInfo(CUIEvent uiEvent)
        {
            this.OpenBaseInfo();
        }

        private void OnPlayerInfoOpenPvpInfo(CUIEvent uiEvent)
        {
            this.OpenPvpInfo();
        }

        private void OnPlayerInfoQuitGame(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Common_QuitGameTips"), enUIEventID.Player_Info_Quit_Game_Confirm, enUIEventID.None, false);
        }

        private void OnPlayerInfoQuitGameConfirm(CUIEvent uiEvent)
        {
            SGameApplication.Quit();
        }

        private void OnPlayerInfoShowRule(CUIEvent uiEvent)
        {
            ResRuleText dataByKey = null;
            ushort num = 0;
            if (this.m_CurTab == Tab.PvpCreditScore_Info)
            {
                num = 11;
            }
            else
            {
                num = 3;
            }
            dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((uint) num);
            if (dataByKey != null)
            {
                string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
                string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
                Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
            }
        }

        private void OnPlayerInfoTabChange(CUIEvent uiEvent)
        {
            if ((this.m_Form != null) && this.m_IsFormOpen)
            {
                CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
                if (component != null)
                {
                    int selectedIndex = component.GetSelectedIndex();
                    this.CurTab = (Tab) selectedIndex;
                    GameObject widget = this.m_Form.GetWidget(1);
                    GameObject obj3 = this.m_Form.GetWidget(2);
                    GameObject obj4 = this.m_Form.GetWidget(5);
                    GameObject obj5 = this.m_Form.GetWidget(4);
                    GameObject obj6 = this.m_Form.GetWidget(7);
                    GameObject p = this.m_Form.GetWidget(8);
                    GameObject obj8 = this.m_Form.GetWidget(10);
                    GameObject obj9 = this.m_Form.GetWidget(9);
                    GameObject obj10 = this.m_Form.GetWidget(12);
                    if (obj10 != null)
                    {
                        this.SetTitle(this.m_CurTab, obj10.transform);
                    }
                    Transform transform = this.m_Form.transform.Find("pnlBg/pnlBody/pnlHonorInfo");
                    GameObject gameObject = null;
                    if (transform != null)
                    {
                        gameObject = transform.gameObject;
                    }
                    Transform transform2 = this.m_Form.transform.Find("pnlBg/pnlBody/pnlPvPHistory");
                    GameObject obj12 = null;
                    if (transform2 != null)
                    {
                        obj12 = transform2.gameObject;
                    }
                    switch (this.m_CurTab)
                    {
                        case Tab.Base_Info:
                            obj9.CustomSetActive(true);
                            obj8.CustomSetActive(false);
                            widget.CustomSetActive(true);
                            obj3.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj5.CustomSetActive(false);
                            obj6.CustomSetActive(false);
                            gameObject.CustomSetActive(false);
                            obj12.CustomSetActive(false);
                            if (p != null)
                            {
                                Utility.GetComponetInChild<Text>(p, "Text").text = Singleton<CTextManager>.instance.GetText("PlayerInfo_Common_Rule");
                            }
                            this.UpdateBaseInfo();
                            this.ProcessQQVIP(this.m_Form, true);
                            this.ProcessNobeHeadIDx(this.m_Form, true);
                            break;

                        case Tab.Pvp_Info:
                            obj9.CustomSetActive(true);
                            obj8.CustomSetActive(false);
                            widget.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj5.CustomSetActive(false);
                            p.CustomSetActive(true);
                            obj3.CustomSetActive(true);
                            obj6.CustomSetActive(false);
                            gameObject.CustomSetActive(false);
                            obj12.CustomSetActive(false);
                            this.UpdatePvpInfo();
                            this.ProcessQQVIP(this.m_Form, false);
                            this.ProcessNobeHeadIDx(this.m_Form, false);
                            if (p != null)
                            {
                                Utility.GetComponetInChild<Text>(p, "Text").text = Singleton<CTextManager>.instance.GetText("PlayerInfo_Common_Rule");
                            }
                            break;

                        case Tab.Honor_Info:
                            widget.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj3.CustomSetActive(false);
                            p.CustomSetActive(true);
                            obj5.CustomSetActive(false);
                            obj6.CustomSetActive(false);
                            gameObject.CustomSetActive(false);
                            obj12.CustomSetActive(false);
                            this.LoadSubModule();
                            MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onClickGloryPoints, new uint[0]);
                            Singleton<CMiShuSystem>.GetInstance().HideNewFlag(component.GetElemenet(2).gameObject, enNewFlagKey.New_HonorInfo_V1);
                            if (p != null)
                            {
                                Utility.GetComponetInChild<Text>(p, "Text").text = Singleton<CTextManager>.instance.GetText("PlayerInfo_Common_Rule");
                            }
                            break;

                        case Tab.Common_Hero:
                            obj9.CustomSetActive(true);
                            obj8.CustomSetActive(false);
                            widget.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj5.CustomSetActive(false);
                            obj3.CustomSetActive(false);
                            gameObject.CustomSetActive(false);
                            p.CustomSetActive(true);
                            obj6.CustomSetActive(true);
                            obj12.CustomSetActive(false);
                            this.UpdateCommonHeroList();
                            if (p != null)
                            {
                                Utility.GetComponetInChild<Text>(p, "Text").text = Singleton<CTextManager>.instance.GetText("PlayerInfo_Common_Rule");
                            }
                            break;

                        case Tab.PvpHistory_Info:
                            widget.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj3.CustomSetActive(false);
                            p.CustomSetActive(true);
                            obj5.CustomSetActive(false);
                            obj6.CustomSetActive(false);
                            gameObject.CustomSetActive(false);
                            obj12.CustomSetActive(false);
                            this.LoadSubModule();
                            Singleton<CMiShuSystem>.GetInstance().HideNewFlag(component.GetElemenet(4).gameObject, enNewFlagKey.New_PvpHistoryInfo_V1);
                            if (p != null)
                            {
                                Utility.GetComponetInChild<Text>(p, "Text").text = Singleton<CTextManager>.instance.GetText("PlayerInfo_Common_Rule");
                            }
                            break;

                        case Tab.PvpCreditScore_Info:
                            obj9.CustomSetActive(true);
                            obj8.CustomSetActive(false);
                            widget.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj3.CustomSetActive(false);
                            p.CustomSetActive(true);
                            obj5.CustomSetActive(true);
                            obj6.CustomSetActive(false);
                            gameObject.CustomSetActive(false);
                            obj12.CustomSetActive(false);
                            this.UpdatePlayerInfo();
                            this.UpdateCreditScore();
                            if (p != null)
                            {
                                Utility.GetComponetInChild<Text>(p, "Text").text = Singleton<CTextManager>.instance.GetText("PlayerInfo_Credit_Rule");
                            }
                            break;
                    }
                    Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.PlayerInfoSystem_Tab_Change);
                }
            }
        }

        private void OnPlayerNameChange()
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                GameObject widget = this.m_Form.GetWidget(1);
                if (widget != null)
                {
                    Text componetInChild = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlHead/NameGroup/txtName");
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if ((componetInChild != null) && (masterRoleInfo != null))
                    {
                        componetInChild.text = masterRoleInfo.Name;
                    }
                }
            }
        }

        private void OnUpdateSubModule(CUIEvent uiEvent)
        {
            DebugHelper.Assert(this.m_Form != null, "Player Form Is Null");
            if (this.m_Form != null)
            {
                GameObject widget = this.m_Form.GetWidget(10);
                this.m_Form.GetWidget(9).CustomSetActive(true);
                widget.CustomSetActive(false);
                switch (this.m_CurTab)
                {
                    case Tab.Honor_Info:
                        Singleton<CPlayerHonorController>.GetInstance().Draw(this.m_Form);
                        break;

                    case Tab.PvpHistory_Info:
                        Singleton<CPlayerPvpHistoryController>.GetInstance().Draw(this.m_Form);
                        break;
                }
            }
        }

        public void OpenBaseInfo()
        {
            this.ShowPlayerDetailInfo(0L, 0, DetailPlayerInfoSource.Self);
        }

        private void OpenByQQVIP(CUIEvent uiEvent)
        {
            if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    if (masterRoleInfo.HasVip(0x10))
                    {
                        Singleton<ApolloHelper>.GetInstance().PayQQVip("CJCLUBT", Singleton<CTextManager>.GetInstance().GetText("QQ_Vip_XuFei_Super_Vip"), 1);
                    }
                    else if (masterRoleInfo.HasVip(1))
                    {
                        Singleton<ApolloHelper>.GetInstance().PayQQVip("LTMCLUB", Singleton<CTextManager>.GetInstance().GetText("QQ_Vip_XuFei_Vip"), 1);
                    }
                    else
                    {
                        Singleton<ApolloHelper>.GetInstance().PayQQVip("LTMCLUB", Singleton<CTextManager>.GetInstance().GetText("QQ_Vip_Buy_Vip"), 1);
                    }
                }
            }
        }

        public void OpenForm()
        {
            if (!this.m_IsFormOpen)
            {
                this.m_IsFormOpen = true;
                this.m_Form = Singleton<CUIManager>.GetInstance().OpenForm(this.sPlayerInfoFormPath, true, true);
                this.CurTab = Tab.Base_Info;
                this.InitTab();
            }
        }

        public void OpenPvpInfo()
        {
            this.ShowPlayerDetailInfo(0L, 0, DetailPlayerInfoSource.Self);
        }

        public void ProcessCommonQQVip(GameObject parent)
        {
            if (parent != null)
            {
                GameObject gameObject = parent.transform.FindChild("QQSVipIcon").gameObject;
                parent.transform.FindChild("QQVipIcon").gameObject.CustomSetActive(false);
                gameObject.CustomSetActive(false);
            }
        }

        private void ProcessNobeHeadIDx(CUIFormScript form, bool bshow)
        {
            GameObject obj2 = Utility.FindChild(form.gameObject, "pnlBg/pnlBody/pnlBaseInfo/pnlContainer/pnlHead/changeNobeheadicon");
            if (!this.isSelf(this.m_PlayerProfile.m_uuid))
            {
                obj2.CustomSetActive(false);
            }
            else
            {
                if (CSysDynamicBlock.bVipBlock)
                {
                    bshow = false;
                }
                if (bshow)
                {
                    obj2.CustomSetActive(true);
                }
                else
                {
                    obj2.CustomSetActive(false);
                }
            }
        }

        private void ProcessQQVIP(CUIFormScript form, bool bShow)
        {
            if (form != null)
            {
                GameObject obj2 = Utility.FindChild(form.gameObject, "pnlBg/pnlBody/pnlBaseInfo/pnlContainer/BtnGroup/QQVIPBtn");
                GameObject obj3 = Utility.FindChild(form.gameObject, "pnlBg/pnlBody/pnlBaseInfo/pnlContainer/pnlHead/NameGroup/QQVipIcon");
                if (!this.isSelf(this.m_PlayerProfile.m_uuid))
                {
                    obj2.CustomSetActive(false);
                    MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(obj3.GetComponent<Image>(), (int) this.m_PlayerProfile.qqVipMask);
                }
                else
                {
                    GameObject obj4 = Utility.FindChild(form.gameObject, "pnlBg/pnlBody/pnlBaseInfo/pnlContainer/BtnGroup/QQVIPBtn/Text");
                    if (!bShow)
                    {
                        obj2.CustomSetActive(false);
                        obj3.CustomSetActive(false);
                        obj4.CustomSetActive(false);
                    }
                    else
                    {
                        if (ApolloConfig.platform == ApolloPlatform.QQ)
                        {
                            obj2.CustomSetActive(true);
                        }
                        else
                        {
                            obj2.CustomSetActive(false);
                        }
                        obj4.CustomSetActive(true);
                        obj3.CustomSetActive(false);
                        if (CSysDynamicBlock.bLobbyEntryBlocked)
                        {
                            obj2.CustomSetActive(false);
                            obj3.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                        }
                        else
                        {
                            if (ApolloConfig.platform == ApolloPlatform.QQ)
                            {
                                obj2.CustomSetActive(true);
                            }
                            else
                            {
                                obj2.CustomSetActive(false);
                            }
                            obj4.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("QQ_Vip_Buy_Vip");
                            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
                            {
                                MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(obj3.GetComponent<Image>());
                            }
                        }
                    }
                }
            }
        }

        private void RefreshLicenseInfoPanel(CUIFormScript form)
        {
            if (null != form)
            {
                GameObject widget = form.GetWidget(6);
                if (null != widget)
                {
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if ((masterRoleInfo != null) && (masterRoleInfo.m_licenseInfo != null))
                    {
                        CUIListScript component = widget.GetComponent<CUIListScript>();
                        if (component != null)
                        {
                            component.SetElementAmount(masterRoleInfo.m_licenseInfo.m_licenseList.Count);
                        }
                    }
                }
            }
        }

        public void ReqOtherPlayerDetailInfo(ulong ullUid, int iLogicWorldId, bool showInfo = true, bool isShowAlert = true)
        {
            if (ullUid > 0L)
            {
                this.isShowPlayerInfo = showInfo;
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa2e);
                msg.stPkgData.stGetAcntDetailInfoReq.ullUid = ullUid;
                msg.stPkgData.stGetAcntDetailInfoReq.iLogicWorldId = iLogicWorldId;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, isShowAlert);
            }
        }

        [MessageHandler(0xa2f)]
        public static void ResPlyaerDetailInfo(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (msg.stPkgData.stGetAcntDetailInfoRsp.iErrCode == 0)
            {
                if (Singleton<CPlayerInfoSystem>.GetInstance().isShowPlayerInfo)
                {
                    Singleton<CPlayerInfoSystem>.instance.ImpResDetailInfo(msg);
                }
                else
                {
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>(EventID.PlayerInfoSystem_Info_Received, msg);
                }
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0xa2f, 0xa3), false, 1.5f, null, new object[0]);
            }
        }

        private void SetAllFriendBtnActive(GameObject root, bool isActive)
        {
            GameObject obj2 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnSettings");
            GameObject obj3 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnQuit");
            obj2.CustomSetActive(isActive);
            obj3.CustomSetActive(isActive);
        }

        private void SetAllGuildBtnActive(GameObject root, bool isActive)
        {
            GameObject obj2 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnAddFriend");
            GameObject obj3 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnAppointViceChairman");
            GameObject obj4 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnTransferPosition");
            GameObject obj5 = Utility.FindChild(root, "pnlContainer/btnFireMember");
            obj2.CustomSetActive(isActive);
            obj3.CustomSetActive(isActive);
            obj4.CustomSetActive(isActive);
            obj5.CustomSetActive(isActive);
        }

        private void SetBaseInfoScrollable(bool scrollable = false)
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                GameObject widget = this.m_Form.GetWidget(1);
                if ((widget != null) && widget.activeSelf)
                {
                    GameObject obj3 = Utility.FindChild(widget, "pnlContainer/pnlInfo/scrollRect");
                    if (obj3 != null)
                    {
                        RectTransform component = obj3.GetComponent<RectTransform>();
                        ScrollRect rect = obj3.GetComponent<ScrollRect>();
                        if (component != null)
                        {
                            if (scrollable)
                            {
                                component.offsetMin = new Vector2(component.offsetMin.x, 90f);
                            }
                            else
                            {
                                component.offsetMin = new Vector2(component.offsetMin.x, 0f);
                            }
                        }
                        if (rect != null)
                        {
                            rect.verticalNormalizedPosition = 1f;
                        }
                    }
                }
            }
        }

        public void SetHeroItemData(CUIFormScript formScript, GameObject listItem, COMDT_MOST_USED_HERO_INFO heroInfo)
        {
            if ((listItem != null) && (heroInfo != null))
            {
                IHeroData data = CHeroDataFactory.CreateHeroData(heroInfo.dwHeroID);
                Transform transform = listItem.transform;
                ResHeroProficiency heroProficiency = CHeroInfo.GetHeroProficiency(data.heroType, (int) heroInfo.dwProficiencyLv);
                if (heroProficiency != null)
                {
                    listItem.GetComponent<Image>().SetSprite(string.Format("{0}{1}", "UGUI/Sprite/Dynamic/Quality/", StringHelper.UTF8BytesToString(ref heroProficiency.szImagePath)), formScript, true, false, false);
                }
                string heroSkinPic = CSkinInfo.GetHeroSkinPic(heroInfo.dwHeroID, 0);
                CUICommonSystem.SetHeroItemImage(formScript, listItem, heroSkinPic, enHeroHeadType.enIcon, false);
                GameObject gameObject = transform.Find("profession").gameObject;
                CUICommonSystem.SetHeroJob(formScript, gameObject, (enHeroJobType) data.heroType);
                transform.Find("heroNameText").GetComponent<Text>().text = data.heroName;
                CUIEventScript component = listItem.GetComponent<CUIEventScript>();
                stUIEventParams eventParams = new stUIEventParams();
                eventParams.openHeroFormPar.heroId = data.cfgID;
                eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.HeroListClick;
                component.SetUIEvent(enUIEventType.Click, enUIEventID.Player_Info_Most_Used_Hero_Item_Click, eventParams);
            }
        }

        private void SetSingleGuildBtnActive(GameObject root)
        {
            GameObject obj2 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnAddFriend");
            GameObject obj3 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnAppointViceChairman");
            GameObject obj4 = Utility.FindChild(root, "pnlContainer/BtnGroup/btnTransferPosition");
            GameObject obj5 = Utility.FindChild(root, "pnlContainer/btnFireMember");
            obj2.CustomSetActive(true);
            obj3.CustomSetActive(this._isShowGuildAppointViceChairmanBtn);
            obj4.CustomSetActive(this._isShowGuildTransferPositionBtn);
            obj5.CustomSetActive(this._isShowGuildFireMemberBtn);
        }

        private void SetTitle(Tab tab, Transform titleTransform)
        {
            if (titleTransform != null)
            {
                Text component = titleTransform.GetComponent<Text>();
                if (component != null)
                {
                    switch (tab)
                    {
                        case Tab.Base_Info:
                            component.text = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Base_Info");
                            break;

                        case Tab.Pvp_Info:
                            component.text = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Pvp_Info");
                            break;

                        case Tab.Honor_Info:
                            component.text = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Honor_Info");
                            break;

                        case Tab.Common_Hero:
                            component.text = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Common_Hero_Info");
                            break;

                        case Tab.PvpHistory_Info:
                            component.text = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_PvpHistory_Info");
                            break;

                        case Tab.PvpCreditScore_Info:
                            component.text = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Tab_Credit_Info");
                            break;

                        default:
                            component.text = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Title");
                            break;
                    }
                }
            }
        }

        public void ShowPlayerDetailInfo(ulong ullUid, int iLogicWorldId, DetailPlayerInfoSource sourceType = 0)
        {
            this._lastDetailSource = sourceType;
            if (this._lastDetailSource == DetailPlayerInfoSource.Self)
            {
                this.m_PlayerProfile.ConvertRoleInfoData(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo());
                this.OpenForm();
            }
            else if (ullUid > 0L)
            {
                this.ReqOtherPlayerDetailInfo(ullUid, iLogicWorldId, true, true);
            }
        }

        public void ShowPlayerDetailInfo(ulong ullUid, int iLogicWorldId, bool isShowGuildAppointViceChairmanBtn, bool isShowGuildTransferPositionBtn, bool isShowGuildFireMemberBtn)
        {
            this._isShowGuildAppointViceChairmanBtn = isShowGuildAppointViceChairmanBtn;
            this._isShowGuildTransferPositionBtn = isShowGuildTransferPositionBtn;
            this._isShowGuildFireMemberBtn = isShowGuildFireMemberBtn;
            this.ShowPlayerDetailInfo(ullUid, iLogicWorldId, DetailPlayerInfoSource.Guild);
        }

        public override void UnInit()
        {
            base.UnInit();
            CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
            instance.RemoveUIEventListener(enUIEventID.Player_Info_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfo_OpenForm));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfo_CloseForm));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Open_Pvp_Info, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoOpenPvpInfo));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Open_Base_Info, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoOpenBaseInfo));
            instance.RemoveUIEventListener(enUIEventID.BuyPick_QQ_VIP, new CUIEventManager.OnUIEventHandler(this.OpenByQQVIP));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Quit_Game, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoQuitGame));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Quit_Game_Confirm, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoQuitGameConfirm));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Most_Used_Hero_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoMostUsedHeroItemEnable));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Most_Used_Hero_Item_Click, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoMostUsedHeroItemClick));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnPlayerInfoShowRule));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_License_ListItem_Enable, new CUIEventManager.OnUIEventHandler(this.OnLicenseListItemEnable));
            instance.RemoveUIEventListener(enUIEventID.Player_Info_Update_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnUpdateSubModule));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.NOBE_STATE_CHANGE, new System.Action(this.UpdateNobeHeadIdx));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.HEAD_IMAGE_FLAG_CHANGE, new System.Action(this.UpdateHeadFlag));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.DeepLink_OnClick, new CUIEventManager.OnUIEventHandler(this.DeepLinkClick));
        }

        private void UpdateBaseInfo()
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                GameObject widget = this.m_Form.GetWidget(1);
                if (widget != null)
                {
                    this.DisplayCustomButton();
                    if (!CSysDynamicBlock.bSocialBlocked)
                    {
                        GameObject obj3 = Utility.FindChild(widget, "pnlContainer/pnlHead/pnlImg/HttpImage");
                        if ((obj3 != null) && !string.IsNullOrEmpty(this.m_PlayerProfile.HeadUrl()))
                        {
                            CUIHttpImageScript script = obj3.GetComponent<CUIHttpImageScript>();
                            script.SetImageUrl(this.m_PlayerProfile.HeadUrl());
                            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(script.GetComponent<Image>(), (int) this.m_PlayerProfile.m_vipInfo.stGameVipClient.dwCurLevel, false);
                            GameObject obj4 = Utility.FindChild(widget, "pnlContainer/pnlHead/pnlImg/HttpImage/NobeImag");
                            if (obj4 != null)
                            {
                                MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(obj4.GetComponent<Image>(), (int) this.m_PlayerProfile.m_vipInfo.stGameVipClient.dwHeadIconId);
                            }
                        }
                    }
                    this.UpdateHeadFlag();
                    COM_PRIVILEGE_TYPE privilegeType = this.m_PlayerProfile.PrivilegeType();
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/pnlHead/NameGroup/WXGameCenterIcon"), privilegeType, ApolloPlatform.Wechat, true, false);
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/WXGameCenter/WXGameCenterBtn"), privilegeType, ApolloPlatform.Wechat, true, false);
                    COM_PRIVILEGE_TYPE com_privilege_type2 = (privilegeType != COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_WXGAMECENTER_LOGIN) ? COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_WXGAMECENTER_LOGIN : COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/WXGameCenter/WXGameCenterGrey"), com_privilege_type2, ApolloPlatform.Wechat, true, false);
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/pnlHead/NameGroup/QQGameCenterIcon"), privilegeType, ApolloPlatform.QQ, true, false);
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/QQGameCenter/QQGameCenterBtn"), privilegeType, ApolloPlatform.QQ, true, false);
                    COM_PRIVILEGE_TYPE com_privilege_type3 = (privilegeType != COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_QQGAMECENTER_LOGIN) ? COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_QQGAMECENTER_LOGIN : COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/QQGameCenter/QQGameCenterGrey"), com_privilege_type3, ApolloPlatform.QQ, true, false);
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/pnlHead/NameGroup/GuestGameCenterIcon"), privilegeType, ApolloPlatform.Guest, true, false);
                    MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(Utility.FindChild(widget, "pnlContainer/GuestGameCenter/GuestGameCenterBtn"), privilegeType, ApolloPlatform.Guest, true, false);
                    Text componetInChild = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlHead/Level");
                    if (componetInChild != null)
                    {
                        componetInChild.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ranking_PlayerLevel"), this.m_PlayerProfile.PvpLevel());
                    }
                    Text text2 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlHead/DianZan/GeiLiDuiYouValue");
                    if (text2 != null)
                    {
                        text2.text = this.m_PlayerProfile._geiLiDuiYou.ToString();
                    }
                    text2 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlHead/DianZan/KeJingDuiShouValue");
                    if (text2 != null)
                    {
                        text2.text = this.m_PlayerProfile._keJingDuiShou.ToString();
                    }
                    Image component = Utility.FindChild(widget, "pnlContainer/pnlHead/NameGroup/Gender").GetComponent<Image>();
                    component.gameObject.CustomSetActive(this.m_PlayerProfile.Gender() != COM_SNSGENDER.COM_SNSGENDER_NONE);
                    if (this.m_PlayerProfile.Gender() == COM_SNSGENDER.COM_SNSGENDER_MALE)
                    {
                        CUIUtility.SetImageSprite(component, string.Format("{0}icon/Ico_boy.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false);
                    }
                    else if (this.m_PlayerProfile.Gender() == COM_SNSGENDER.COM_SNSGENDER_FEMALE)
                    {
                        CUIUtility.SetImageSprite(component, string.Format("{0}icon/Ico_girl.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false);
                    }
                    Text text3 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlHead/NameGroup/txtName");
                    if (text3 != null)
                    {
                        text3.text = this.m_PlayerProfile.Name();
                    }
                    GameObject obj5 = Utility.FindChild(widget, "pnlContainer/pnlHead/Status/Rank");
                    if (this.m_PlayerProfile.GetRankGrade() == 0)
                    {
                        if (obj5 != null)
                        {
                            obj5.CustomSetActive(false);
                        }
                    }
                    else
                    {
                        obj5.CustomSetActive(true);
                        Image image2 = null;
                        Image image3 = null;
                        if (obj5 != null)
                        {
                            image2 = Utility.GetComponetInChild<Image>(obj5, "ImgRank");
                            image3 = Utility.GetComponetInChild<Image>(obj5, "ImgRank/ImgSubRank");
                        }
                        if (image2 != null)
                        {
                            string rankSmallIconPath = CLadderView.GetRankSmallIconPath(this.m_PlayerProfile.GetRankGrade(), this.m_PlayerProfile.GetRankClass());
                            image2.SetSprite(rankSmallIconPath, this.m_Form, true, false, false);
                        }
                        if (image3 != null)
                        {
                            string subRankSmallIconPath = CLadderView.GetSubRankSmallIconPath(this.m_PlayerProfile.GetRankGrade(), this.m_PlayerProfile.GetRankClass());
                            image3.SetSprite(subRankSmallIconPath, this.m_Form, true, false, false);
                        }
                    }
                    GameObject obj6 = Utility.FindChild(widget, "pnlContainer/pnlHead/Status/HisRank");
                    if (this.m_PlayerProfile.GetHistoryHighestRankGrade() == 0)
                    {
                        if (obj6 != null)
                        {
                            obj6.CustomSetActive(false);
                        }
                    }
                    else
                    {
                        obj6.CustomSetActive(true);
                        Image image4 = null;
                        Image image5 = null;
                        if (obj6 != null)
                        {
                            image4 = Utility.GetComponetInChild<Image>(obj6, "ImgRank");
                            image5 = Utility.GetComponetInChild<Image>(obj6, "ImgRank/ImgSubRank");
                        }
                        if (image4 != null)
                        {
                            string prefabPath = CLadderView.GetRankSmallIconPath(this.m_PlayerProfile.GetHistoryHighestRankGrade(), this.m_PlayerProfile.GetHistoryHighestRankClass());
                            image4.SetSprite(prefabPath, this.m_Form, true, false, false);
                        }
                        if (image5 != null)
                        {
                            string str4 = CLadderView.GetSubRankSmallIconPath(this.m_PlayerProfile.GetHistoryHighestRankGrade(), this.m_PlayerProfile.GetHistoryHighestRankClass());
                            image5.SetSprite(str4, this.m_Form, true, false, false);
                        }
                    }
                    string str5 = string.Empty;
                    string str6 = string.Empty;
                    if (this.m_PlayerProfile._haveExtraCoin)
                    {
                        GameObject obj7 = Utility.FindChild(widget, "pnlContainer/pnlHead/Status/ExtraCoin");
                        obj7.CustomSetActive(true);
                        if (this.m_PlayerProfile._coinExpireHours > 0)
                        {
                            str5 = string.Format(Singleton<CTextManager>.GetInstance().GetText("DoubleCoinExpireTimeTips"), this.m_PlayerProfile._coinExpireHours / 0x18, this.m_PlayerProfile._coinExpireHours % 0x18);
                        }
                        if (this.m_PlayerProfile._coinWinCount > 0)
                        {
                            str6 = string.Format(Singleton<CTextManager>.GetInstance().GetText("DoubleCoinCountWinTips"), this.m_PlayerProfile._coinWinCount);
                        }
                        if (string.IsNullOrEmpty(str5))
                        {
                            CUICommonSystem.SetCommonTipsEvent(this.m_Form, obj7, string.Format("{0}", str6), enUseableTipsPos.enBottom);
                        }
                        else if (string.IsNullOrEmpty(str6))
                        {
                            CUICommonSystem.SetCommonTipsEvent(this.m_Form, obj7, string.Format("{0}", str5), enUseableTipsPos.enBottom);
                        }
                        else
                        {
                            CUICommonSystem.SetCommonTipsEvent(this.m_Form, obj7, string.Format("{0}\n{1}", str5, str6), enUseableTipsPos.enBottom);
                        }
                    }
                    else
                    {
                        Utility.FindChild(widget, "pnlContainer/pnlHead/Status/ExtraCoin").CustomSetActive(false);
                    }
                    if (this.m_PlayerProfile._haveExtraExp)
                    {
                        GameObject obj8 = Utility.FindChild(widget, "pnlContainer/pnlHead/Status/ExtraExp");
                        obj8.CustomSetActive(true);
                        if (this.m_PlayerProfile._expExpireHours > 0)
                        {
                            str5 = string.Format(Singleton<CTextManager>.GetInstance().GetText("DoubleExpExpireTimeTips"), this.m_PlayerProfile._expExpireHours / 0x18, this.m_PlayerProfile._expExpireHours % 0x18);
                        }
                        if (this.m_PlayerProfile._expWinCount > 0)
                        {
                            str6 = string.Format(Singleton<CTextManager>.GetInstance().GetText("DoubleExpCountWinTips"), this.m_PlayerProfile._expWinCount);
                        }
                        if (string.IsNullOrEmpty(str5))
                        {
                            CUICommonSystem.SetCommonTipsEvent(this.m_Form, obj8, string.Format("{0}", str6), enUseableTipsPos.enBottom);
                        }
                        else if (string.IsNullOrEmpty(str6))
                        {
                            CUICommonSystem.SetCommonTipsEvent(this.m_Form, obj8, string.Format("{0}", str5), enUseableTipsPos.enBottom);
                        }
                        else
                        {
                            CUICommonSystem.SetCommonTipsEvent(this.m_Form, obj8, string.Format("{0}\n{1}", str5, str6), enUseableTipsPos.enBottom);
                        }
                    }
                    else
                    {
                        Utility.FindChild(widget, "pnlContainer/pnlHead/Status/ExtraExp").CustomSetActive(false);
                    }
                    uint num = (uint) (((((this.m_PlayerProfile.Pvp5V5TotalGameCnt() + this.m_PlayerProfile.Pvp3V3TotalGameCnt()) + this.m_PlayerProfile.Pvp2V2TotalGameCnt()) + this.m_PlayerProfile.Pvp1V1TotalGameCnt()) + this.m_PlayerProfile.RankTotalGameCnt()) + this.m_PlayerProfile.EntertainmentTotalGameCnt());
                    uint a = (uint) (((((this.m_PlayerProfile.Pvp5V5WinGameCnt() + this.m_PlayerProfile.Pvp3V3WinGameCnt()) + this.m_PlayerProfile.Pvp2V2WinGameCnt()) + this.m_PlayerProfile.Pvp1V1WinGameCnt()) + this.m_PlayerProfile.RankWinGameCnt()) + this.m_PlayerProfile.EntertainmentWinGameCnt());
                    Text text4 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/scrollRect/content/Left/PvpInfo/Cnt");
                    if (text4 != null)
                    {
                        text4.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Total_Fighting_Cnt"), num);
                    }
                    Text text5 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/scrollRect/content/Left/PvpInfo/WinsBg/imgWins/txtWins");
                    if (text5 != null)
                    {
                        text5.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ranking_WinRate"), CPlayerProfile.Round(CPlayerProfile.Divide(a, num) * 100f));
                    }
                    Image image = Utility.GetComponetInChild<Image>(widget, "pnlContainer/pnlInfo/scrollRect/content/Left/PvpInfo/WinsBg/imgWins");
                    if (image != null)
                    {
                        image.CustomFillAmount(CPlayerProfile.Divide(a, num));
                    }
                    Text text6 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/scrollRect/content/Left/OwnInfo/HeroCnt");
                    if (text6 != null)
                    {
                        text6.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Total_Hero_Cnt"), this.m_PlayerProfile.HeroCnt());
                    }
                    Text text7 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/scrollRect/content/Left/OwnInfo/SkinCnt");
                    if (text7 != null)
                    {
                        text7.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Total_Skin_Cnt"), this.m_PlayerProfile.SkinCnt());
                    }
                    Text text8 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/scrollRect/content/Left/GuildInfo/Name");
                    Text text9 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/scrollRect/content/Left/GuildInfo/Position");
                    Text text10 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/scrollRect/content/Left/GuildInfo/None");
                    if (!CGuildSystem.IsInNormalGuild(this.m_PlayerProfile.GuildState) || string.IsNullOrEmpty(this.m_PlayerProfile.GuildName))
                    {
                        if (text8 != null)
                        {
                            text8.text = string.Empty;
                        }
                        if (text9 != null)
                        {
                            text9.text = string.Empty;
                        }
                        if (text10 != null)
                        {
                            text10.text = Singleton<CTextManager>.GetInstance().GetText("PlayerInfo_Guild");
                        }
                    }
                    else
                    {
                        if (text8 != null)
                        {
                            text8.text = this.m_PlayerProfile.GuildName;
                        }
                        if (text9 != null)
                        {
                            text9.text = Singleton<CTextManager>.GetInstance().GetText("Player_Info_Guild_Position") + CGuildHelper.GetPositionName(this.m_PlayerProfile.GuildState);
                        }
                        if (text10 != null)
                        {
                            text10.text = string.Empty;
                        }
                    }
                    bool bActive = this.isSelf(this.m_PlayerProfile.m_uuid);
                    this.m_Form.GetWidget(3).CustomSetActive(bActive);
                    GameObject obj10 = this.m_Form.GetWidget(13);
                    InputField field = obj10.GetComponent<InputField>();
                    obj10.CustomSetActive(true);
                    if (field != null)
                    {
                        if (string.IsNullOrEmpty(this.m_PlayerProfile.m_personSign))
                        {
                            obj10.CustomSetActive(bActive);
                            field.text = string.Empty;
                        }
                        else
                        {
                            field.text = this.m_PlayerProfile.m_personSign;
                        }
                        if (bActive)
                        {
                            field.interactable = true;
                            field.onEndEdit.RemoveAllListeners();
                            field.onEndEdit.AddListener(new UnityAction<string>(this.OnPersonSignEndEdit));
                        }
                        else
                        {
                            field.interactable = false;
                        }
                    }
                    GameObject obj11 = Utility.FindChild(widget, "pnlContainer/BtnGroup/JFQBtn");
                    if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
                    {
                        if (!CSysDynamicBlock.bJifenHallBlock)
                        {
                            obj11.CustomSetActive(bActive);
                        }
                        else
                        {
                            obj11.CustomSetActive(false);
                        }
                    }
                    else
                    {
                        obj11.CustomSetActive(false);
                    }
                    GameObject obj12 = Utility.FindChild(widget, "pnlContainer/BtnGroup/BuLuoBtn");
                    if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
                    {
                        obj12.CustomSetActive(bActive);
                    }
                    else
                    {
                        obj12.CustomSetActive(false);
                    }
                    if (MonoSingleton<BannerImageSys>.GetInstance().IsWaifaBlockChannel())
                    {
                        obj12.CustomSetActive(false);
                    }
                    GameObject obj13 = Utility.FindChild(widget, "pnlContainer/BtnGroup/DeepLinkBtn");
                    if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
                    {
                        obj13.CustomSetActive(false);
                    }
                    else
                    {
                        long currentUTCTime = CRoleInfo.GetCurrentUTCTime();
                        if (MonoSingleton<BannerImageSys>.GetInstance().DeepLinkInfo.isTimeValid(currentUTCTime))
                        {
                            obj13.CustomSetActive(bActive);
                        }
                        else
                        {
                            obj13.CustomSetActive(false);
                        }
                    }
                    if (MonoSingleton<BannerImageSys>.GetInstance().IsWaifaBlockChannel())
                    {
                        obj13.CustomSetActive(false);
                    }
                    if (CSysDynamicBlock.bLobbyEntryBlocked)
                    {
                        Transform transform = widget.transform.Find("pnlContainer/pnlHead/changeNobeheadicon");
                        if (transform != null)
                        {
                            transform.gameObject.CustomSetActive(false);
                        }
                        Transform transform2 = widget.transform.Find("pnlContainer/BtnGroup/BuLuoBtn");
                        if (transform2 != null)
                        {
                            transform2.gameObject.CustomSetActive(false);
                        }
                        Transform transform3 = widget.transform.Find("pnlContainer/BtnGroup/QQVIPBtn");
                        if (transform3 != null)
                        {
                            transform3.gameObject.CustomSetActive(false);
                        }
                        Transform transform4 = widget.transform.Find("pnlContainer/BtnGroup/JFQBtn");
                        if (transform4 != null)
                        {
                            transform4.gameObject.CustomSetActive(false);
                        }
                        Transform transform5 = widget.transform.Find("pnlContainer/BtnGroup/XYJLBBtn");
                        if (transform5 != null)
                        {
                            transform5.gameObject.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        private void UpdateCommonHeroList()
        {
            GameObject widget = this.m_Form.GetWidget(7);
            if (widget != null)
            {
                CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "List");
                if (componetInChild != null)
                {
                    int count = this.m_PlayerProfile.MostUsedHeroList().Count;
                    uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xb2).dwConfValue;
                    if (count > dwConfValue)
                    {
                        count = (int) dwConfValue;
                    }
                    componetInChild.SetElementAmount(count);
                }
            }
        }

        private void UpdateCreditScore()
        {
            CUIFormScript form = Singleton<CUIManager>.instance.GetForm(this.sPlayerInfoFormPath);
            if (form != null)
            {
                GameObject widget = form.GetWidget(4);
                if (widget != null)
                {
                    uint creditScore = this.m_PlayerProfile.creditScore;
                    ResCreditLevelInfo creditLevelInfoByScore = CRoleInfo.GetCreditLevelInfoByScore((int) creditScore);
                    if (creditLevelInfoByScore != null)
                    {
                        ResCreditLevelInfo creditLevelInfo = CRoleInfo.GetCreditLevelInfo(creditLevelInfoByScore.bCreditLevel + 1);
                        ResCreditLevelInfo info3 = CRoleInfo.GetCreditLevelInfo(creditLevelInfoByScore.bCreditLevel - 1);
                        Text componetInChild = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlCreditScore/CreditValue/ScoreValue");
                        GameObject obj3 = Utility.FindChild(widget, "pnlContainer/pnlCreditScore/CreditValue/CreditLevel/LevelValue");
                        GameObject p = Utility.FindChild(widget, "pnlContainer/pnlCreditAward/SelfAward");
                        GameObject obj5 = Utility.FindChild(widget, "pnlContainer/pnlCreditAward/MoreAward");
                        int num2 = 0;
                        int num3 = 0;
                        if (componetInChild != null)
                        {
                            componetInChild.text = creditScore.ToString();
                        }
                        if (obj3 != null)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                obj3.transform.GetChild(i).gameObject.CustomSetActive(creditLevelInfoByScore.bCreditLevel > i);
                            }
                        }
                        if (p != null)
                        {
                            Text text2 = Utility.GetComponetInChild<Text>(p, "TitleTxt");
                            if (creditLevelInfoByScore.bCreditLevelResult == 0)
                            {
                                text2.text = Singleton<CTextManager>.instance.GetText("Credit_Punish_Title");
                            }
                            else
                            {
                                text2.text = Singleton<CTextManager>.instance.GetText("Credit_Reward_Title");
                            }
                            if (creditLevelInfo == null)
                            {
                                if (info3 != null)
                                {
                                    int num5 = 0;
                                    for (int j = 0; j < 3; j++)
                                    {
                                        GameObject obj6 = Utility.FindChild(p, string.Format("pnlAward/itemCell_{0}", num5));
                                        if ((j < info3.astCreditRewardDetail.Length) && !string.IsNullOrEmpty(info3.astCreditRewardDetail[j].szCreditRewardItemDesc))
                                        {
                                            num2++;
                                            num5++;
                                            obj6.CustomSetActive(true);
                                            Text text3 = Utility.GetComponetInChild<Text>(obj6, "ItemName");
                                            Image image = Utility.GetComponetInChild<Image>(obj6, "imgIcon");
                                            text3.text = info3.astCreditRewardDetail[j].szCreditRewardItemDesc;
                                            image.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, info3.astCreditRewardDetail[j].szCreditRewardItemIcon), form, true, false, false);
                                        }
                                    }
                                    for (int k = 0; k < 3; k++)
                                    {
                                        GameObject obj7 = Utility.FindChild(p, string.Format("pnlAward/itemCell_{0}", num5));
                                        if ((k < creditLevelInfoByScore.astCreditRewardDetail.Length) && !string.IsNullOrEmpty(creditLevelInfoByScore.astCreditRewardDetail[k].szCreditRewardItemDesc))
                                        {
                                            num2++;
                                            num5++;
                                            obj7.CustomSetActive(true);
                                            Text text4 = Utility.GetComponetInChild<Text>(obj7, "ItemName");
                                            Image image2 = Utility.GetComponetInChild<Image>(obj7, "imgIcon");
                                            text4.text = creditLevelInfoByScore.astCreditRewardDetail[k].szCreditRewardItemDesc;
                                            image2.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, creditLevelInfoByScore.astCreditRewardDetail[k].szCreditRewardItemIcon), form, true, false, false);
                                        }
                                    }
                                    for (int m = num5; m < 6; m++)
                                    {
                                        Utility.FindChild(p, string.Format("pnlAward/itemCell_{0}", m)).CustomSetActive(false);
                                    }
                                    p.CustomSetActive(num2 > 0);
                                }
                                else
                                {
                                    DebugHelper.Assert(false, " nextCreditLevelInfo == null and preCreditLevelInfo == null , this is not avaible!!");
                                }
                            }
                            else
                            {
                                for (int n = 0; n < 6; n++)
                                {
                                    GameObject obj9 = Utility.FindChild(p, string.Format("pnlAward/itemCell_{0}", n));
                                    if ((n < creditLevelInfoByScore.astCreditRewardDetail.Length) && !string.IsNullOrEmpty(creditLevelInfoByScore.astCreditRewardDetail[n].szCreditRewardItemDesc))
                                    {
                                        num2++;
                                        obj9.CustomSetActive(true);
                                        Text text5 = Utility.GetComponetInChild<Text>(obj9, "ItemName");
                                        Image image3 = Utility.GetComponetInChild<Image>(obj9, "imgIcon");
                                        text5.text = creditLevelInfoByScore.astCreditRewardDetail[n].szCreditRewardItemDesc;
                                        image3.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, creditLevelInfoByScore.astCreditRewardDetail[n].szCreditRewardItemIcon), form, true, false, false);
                                    }
                                    else
                                    {
                                        obj9.CustomSetActive(false);
                                    }
                                }
                                p.CustomSetActive(num2 > 0);
                            }
                        }
                        if (obj5 != null)
                        {
                            if (creditLevelInfo == null)
                            {
                                obj5.CustomSetActive(false);
                            }
                            else
                            {
                                obj5.CustomSetActive(true);
                                Utility.GetComponetInChild<Text>(obj5, "TitleTxt").text = Singleton<CTextManager>.instance.GetText("Credit_NextReward_Title");
                                for (int num10 = 0; num10 < 3; num10++)
                                {
                                    GameObject obj10 = Utility.FindChild(obj5, string.Format("pnlAward/itemCell_{0}", num10));
                                    if (obj10 == null)
                                    {
                                        break;
                                    }
                                    if (!string.IsNullOrEmpty(creditLevelInfo.astCreditRewardDetail[num10].szCreditRewardItemDesc))
                                    {
                                        num3++;
                                        obj10.CustomSetActive(true);
                                        Text text7 = Utility.GetComponetInChild<Text>(obj10, "ItemName");
                                        Image image4 = Utility.GetComponetInChild<Image>(obj10, "imgIcon");
                                        text7.text = creditLevelInfo.astCreditRewardDetail[num10].szCreditRewardItemDesc;
                                        image4.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, creditLevelInfo.astCreditRewardDetail[num10].szCreditRewardItemIcon), form, true, false, false);
                                    }
                                    else
                                    {
                                        obj10.CustomSetActive(false);
                                    }
                                }
                                string[] args = new string[] { (creditLevelInfo.dwCreditThresholdLow - creditScore).ToString() };
                                Utility.GetComponetInChild<Text>(obj5, "DifferenceValue").text = Singleton<CTextManager>.instance.GetText("Credit_Next_Level", args);
                                obj5.CustomSetActive(num3 > 0);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateHeadFlag()
        {
            if ((this.m_IsFormOpen && (this.m_Form != null)) && (this.m_Form.GetWidget(1) != null))
            {
                GameObject target = Utility.FindChild(this.m_Form.gameObject, "pnlBg/pnlBody/pnlBaseInfo/pnlContainer/pnlHead/changeNobeheadicon");
                if (target != null)
                {
                    if (Singleton<HeadIconSys>.instance.UnReadFlagNum > 0)
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

        private void UpdateNobeHeadIdx()
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                GameObject widget = this.m_Form.GetWidget(1);
                if (widget != null)
                {
                    GameObject obj3 = Utility.FindChild(widget, "pnlContainer/pnlHead/pnlImg/HttpImage/NobeImag");
                    if (obj3 != null)
                    {
                        MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(obj3.GetComponent<Image>(), (int) MonoSingleton<NobeSys>.GetInstance().m_vipInfo.stGameVipClient.dwHeadIconId);
                    }
                }
            }
        }

        private void UpdatePlayerInfo()
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                GameObject widget = this.m_Form.GetWidget(4);
                if (widget != null)
                {
                    this.DisplayCustomButton();
                    if (!CSysDynamicBlock.bSocialBlocked)
                    {
                        GameObject obj3 = Utility.FindChild(widget, "pnlContainer/pnlHead/pnlImg/HttpImage");
                        if ((obj3 != null) && !string.IsNullOrEmpty(this.m_PlayerProfile.HeadUrl()))
                        {
                            CUIHttpImageScript component = obj3.GetComponent<CUIHttpImageScript>();
                            component.SetImageUrl(this.m_PlayerProfile.HeadUrl());
                            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.GetComponent<Image>(), (int) this.m_PlayerProfile.m_vipInfo.stGameVipClient.dwCurLevel, false);
                            GameObject obj4 = Utility.FindChild(widget, "pnlContainer/pnlHead/pnlImg/HttpImage/NobeImag");
                            if (obj4 != null)
                            {
                                MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(obj4.GetComponent<Image>(), (int) this.m_PlayerProfile.m_vipInfo.stGameVipClient.dwHeadIconId);
                            }
                        }
                    }
                    Text componetInChild = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlHead/Level");
                    if (componetInChild != null)
                    {
                        componetInChild.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ranking_PlayerLevel"), this.m_PlayerProfile.PvpLevel());
                    }
                    Text text2 = Utility.GetComponetInChild<Text>(widget, "pnlContainer/pnlInfo/CreditRule/text");
                    if (text2 != null)
                    {
                        ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((uint) 11);
                        if (dataByKey != null)
                        {
                            text2.text = string.Format("{0}", dataByKey.szContent);
                        }
                    }
                }
            }
        }

        private void UpdatePvpInfo()
        {
            if (this.m_IsFormOpen && (this.m_Form != null))
            {
                GameObject widget = this.m_Form.GetWidget(2);
                if (widget != null)
                {
                    if (!CSysDynamicBlock.bSocialBlocked)
                    {
                        GameObject obj3 = Utility.FindChild(widget, "pnlHead/mvp/HttpImage");
                        if ((obj3 != null) && !string.IsNullOrEmpty(this.m_PlayerProfile.HeadUrl()))
                        {
                            obj3.GetComponent<CUIHttpImageScript>().SetImageUrl(this.m_PlayerProfile.HeadUrl());
                        }
                        GameObject obj4 = Utility.FindChild(widget, "pnlHead/loseSoul/HttpImage");
                        if ((obj4 != null) && !string.IsNullOrEmpty(this.m_PlayerProfile.HeadUrl()))
                        {
                            obj4.GetComponent<CUIHttpImageScript>().SetImageUrl(this.m_PlayerProfile.HeadUrl());
                        }
                    }
                    Text componetInChild = Utility.GetComponetInChild<Text>(widget, "pnlLeft/pnlHead/mvp/cnt");
                    if (componetInChild != null)
                    {
                        componetInChild.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Cnt_Unit"), this.m_PlayerProfile.MVPCnt());
                    }
                    Text text2 = Utility.GetComponetInChild<Text>(widget, "pnlLeft/pnlHead/loseSoul/cnt");
                    if (text2 != null)
                    {
                        text2.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Cnt_Unit"), this.m_PlayerProfile.LoseSoulCnt());
                    }
                    Text text3 = Utility.GetComponetInChild<Text>(widget, "pnlLeft/pnlHead/godLike/cnt");
                    if (text3 != null)
                    {
                        text3.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Cnt_Unit"), this.m_PlayerProfile.HolyShit());
                    }
                    Text text4 = Utility.GetComponetInChild<Text>(widget, "pnlLeft/pnlHead/tripleKill/cnt");
                    if (text4 != null)
                    {
                        text4.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Cnt_Unit"), this.m_PlayerProfile.TripleKill());
                    }
                    Text text5 = Utility.GetComponetInChild<Text>(widget, "pnlLeft/pnlHead/QuataryKill/cnt");
                    if (text5 != null)
                    {
                        text5.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Cnt_Unit"), this.m_PlayerProfile.QuataryKill());
                    }
                    Text text6 = Utility.GetComponetInChild<Text>(widget, "pnlLeft/pnlHead/PentaKill/cnt");
                    if (text6 != null)
                    {
                        text6.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Cnt_Unit"), this.m_PlayerProfile.PentaKill());
                    }
                    GameObject obj5 = Utility.FindChild(widget, "pnlInfo/Left/qualifyInfo");
                    if (this.m_PlayerProfile.GetRankGrade() != 0)
                    {
                        obj5.CustomSetActive(true);
                        Text text7 = Utility.GetComponetInChild<Text>(widget, "pnlInfo/Left/qualifyInfo/cnt");
                        if (text7 != null)
                        {
                            text7.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Fighting_Cnt"), this.m_PlayerProfile.RankTotalGameCnt());
                        }
                        Image image = Utility.GetComponetInChild<Image>(widget, "pnlInfo/Left/qualifyInfo/WinsBg/imgWins");
                        if (image != null)
                        {
                            image.CustomFillAmount(this.m_PlayerProfile.RankWins());
                        }
                        Text text8 = Utility.GetComponetInChild<Text>(widget, "pnlInfo/Left/qualifyInfo/WinsBg/imgWins/txtWins");
                        if (text8 != null)
                        {
                            text8.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ranking_WinRate"), CPlayerProfile.Round(this.m_PlayerProfile.RankWins() * 100f));
                        }
                    }
                    else
                    {
                        obj5.CustomSetActive(false);
                    }
                    Text text9 = Utility.GetComponetInChild<Text>(widget, "pnlInfo/Left/5V5Info/cnt");
                    if (text9 != null)
                    {
                        text9.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Fighting_Cnt"), this.m_PlayerProfile.Pvp5V5TotalGameCnt());
                    }
                    Image image2 = Utility.GetComponetInChild<Image>(widget, "pnlInfo/Left/5V5Info/WinsBg/imgWins");
                    if (image2 != null)
                    {
                        image2.CustomFillAmount(this.m_PlayerProfile.Pvp5V5Wins());
                    }
                    Text text10 = Utility.GetComponetInChild<Text>(widget, "pnlInfo/Left/5V5Info/WinsBg/imgWins/txtWins");
                    if (text10 != null)
                    {
                        text10.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ranking_WinRate"), CPlayerProfile.Round(this.m_PlayerProfile.Pvp5V5Wins() * 100f));
                    }
                    Text text11 = Utility.GetComponetInChild<Text>(widget, "pnlInfo/Left/3V3Info/cnt");
                    if (text11 != null)
                    {
                        text11.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Fighting_Cnt"), this.m_PlayerProfile.Pvp3V3TotalGameCnt());
                    }
                    Image image3 = Utility.GetComponetInChild<Image>(widget, "pnlInfo/Left/3V3Info/WinsBg/imgWins");
                    if (image3 != null)
                    {
                        image3.CustomFillAmount(this.m_PlayerProfile.Pvp3V3Wins());
                    }
                    Text text12 = Utility.GetComponetInChild<Text>(widget, "pnlInfo/Left/3V3Info/WinsBg/imgWins/txtWins");
                    if (text12 != null)
                    {
                        text12.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ranking_WinRate"), CPlayerProfile.Round(this.m_PlayerProfile.Pvp3V3Wins() * 100f));
                    }
                    Text text13 = Utility.GetComponetInChild<Text>(widget, "pnlInfo/Left/1V1Info/cnt");
                    if (text13 != null)
                    {
                        text13.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Fighting_Cnt"), this.m_PlayerProfile.Pvp1V1TotalGameCnt());
                    }
                    Image image4 = Utility.GetComponetInChild<Image>(widget, "pnlInfo/Left/1V1Info/WinsBg/imgWins");
                    if (image4 != null)
                    {
                        image4.CustomFillAmount(this.m_PlayerProfile.Pvp1V1Wins());
                    }
                    Text text14 = Utility.GetComponetInChild<Text>(widget, "pnlInfo/Left/1V1Info/WinsBg/imgWins/txtWins");
                    if (text14 != null)
                    {
                        text14.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ranking_WinRate"), CPlayerProfile.Round(this.m_PlayerProfile.Pvp1V1Wins() * 100f));
                    }
                    Text text15 = Utility.GetComponetInChild<Text>(widget, "pnlInfo/Left/entertainmentInfo/cnt");
                    if (text15 != null)
                    {
                        text15.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Player_Info_Fighting_Cnt"), this.m_PlayerProfile.EntertainmentTotalGameCnt());
                    }
                    Image image5 = Utility.GetComponetInChild<Image>(widget, "pnlInfo/Left/entertainmentInfo/WinsBg/imgWins");
                    if (image5 != null)
                    {
                        image5.CustomFillAmount(this.m_PlayerProfile.EntertainmentWins());
                    }
                    Text text16 = Utility.GetComponetInChild<Text>(widget, "pnlInfo/Left/entertainmentInfo/WinsBg/imgWins/txtWins");
                    if (text16 != null)
                    {
                        text16.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ranking_WinRate"), CPlayerProfile.Round(this.m_PlayerProfile.EntertainmentWins() * 100f));
                    }
                }
            }
        }

        public Tab CurTab
        {
            get
            {
                return this.m_CurTab;
            }
            set
            {
                this.m_CurTab = value;
            }
        }

        public enum DetailPlayerInfoSource
        {
            DefaultOthers,
            Self,
            Guild,
            Friend
        }

        public enum enPlayerFormWidget
        {
            Tab,
            Base_Info_Tab,
            Pvp_Info_Tab,
            Change_Name_Button,
            CreditScore_Tab,
            License_Info_Tab,
            License_List,
            Common_Hero_info,
            Rule_Btn,
            Body,
            Juhua,
            Update_Sub_Module_Timer,
            Title,
            PersonSign,
            PvpHistoryInfo
        }

        public enum Tab
        {
            Base_Info,
            Pvp_Info,
            Honor_Info,
            Common_Hero,
            PvpHistory_Info,
            PvpCreditScore_Info
        }
    }
}

