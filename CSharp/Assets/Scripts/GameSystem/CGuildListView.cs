namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CGuildListView : Singleton<CGuildListView>
    {
        public const string GuildListFormPrefabPath = "UGUI/Form/System/Guild/Form_Guild_List.prefab";
        private const int GuildRuleTextIndex = 8;
        public const string IconFormPrefabPath = "UGUI/Form/System/Guild/Form_Guild_Icon.prefab";
        private GameObject m_curPanelGo;
        private Tab m_curTab = Tab.None;
        private CUIFormScript m_form;
        private CGuildModel m_Model = Singleton<CGuildModel>.GetInstance();

        public CGuildListView()
        {
            this.Init();
        }

        public void CloseForm()
        {
            Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Guild/Form_Guild_List.prefab");
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_List_View_Change_Tab, new CUIEventManager.OnUIEventHandler(this.On_Tab_Change));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Select_In_Guild_List, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Select));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Prepare_Guild_Select, new CUIEventManager.OnUIEventHandler(this.On_Guild_Prepare_Guild_Select));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Join, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Join));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Join, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Join));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Join_Confirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Join_Confirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Join_Cancel, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Join_Cancel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Search_In_Guild_List_Panel, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Search_In_Guild_List_Panel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Search_In_Prepare_Guild_List_Panel, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Search_In_Prepare_Guild_List_Panel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_Help, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_Help));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Create, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Create));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Create_Confirm, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Create_Confirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Create_Cancel, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Create_Cancel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Create_Modify_Icon, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Create_Modify_Icon));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Create_Icon_Selected, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Create_Icon_Selected));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_PrepareGuild_Timeout, new CUIEventManager.OnUIEventHandler(this.On_Guild_PrepareGuild_Timeout));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Only_Friend_Slider_Value_Changed, new CUIEventManager.OnUIEventHandler(this.On_Guild_Only_Friend_Slider_Value_Changed));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Guild_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.On_Guild_Guild_List_Element_Enabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Request_More_Guild_List, new CUIEventManager.OnUIEventHandler(this.On_Guild_Request_More_Guild_List));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Prepare_Guild_List_Element_Enabled, new CUIEventManager.OnUIEventHandler(this.On_Guild_Prepare_Guild_List_Element_Enabled));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Requesst_More_Prepare_Guild_List, new CUIEventManager.OnUIEventHandler(this.On_Guild_Requesst_More_Prepare_Guild_List));
        }

        private void InitPanel()
        {
            this.m_form.GetWidget(5).gameObject.CustomSetActive(false);
            this.m_form.GetWidget(6).gameObject.CustomSetActive(false);
            this.m_form.GetWidget(7).gameObject.CustomSetActive(false);
            switch (this.CurTab)
            {
                case Tab.Guild:
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<int, int>("Request_Guild_List", 1, 20);
                    break;

                case Tab.PrepareGuild:
                    switch (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState)
                    {
                        case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_CREATE:
                        case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_JOIN:
                            Singleton<EventRouter>.GetInstance().BroadCastEvent("Request_PrepareGuild_Info");
                            return;
                    }
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<int>("Request_PrepareGuild_List", 0);
                    break;

                case Tab.CreateGuild:
                    this.RefreshCreateGuildPanel();
                    break;
            }
        }

        public void InitTabList()
        {
            string[] strArray = new string[] { Singleton<CTextManager>.GetInstance().GetText("Guild_Join_Guild"), Singleton<CTextManager>.GetInstance().GetText("Guild_Prepare_Guild"), Singleton<CTextManager>.GetInstance().GetText("Guild_Create_Guild") };
            CUIListScript component = this.m_form.GetWidget(0x21).GetComponent<CUIListScript>();
            component.SetElementAmount(strArray.Length);
            for (int i = 0; i < component.m_elementAmount; i++)
            {
                component.GetElemenet(i).gameObject.transform.Find("Text").GetComponent<Text>().text = strArray[i];
            }
        }

        public bool IsShow()
        {
            return (Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_List.prefab") != null);
        }

        private bool IsSliderOnlyFriendSelected()
        {
            return (((int) this.m_form.GetWidget(0x20).GetComponent<Slider>().value) == 0);
        }

        private void On_Guild_Guild_Help(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenInfoForm(8);
        }

        private void On_Guild_Guild_Join(CUIEvent uiEvent)
        {
            int selectedIndex = this.m_form.GetWidget(30).GetComponent<CUIListScript>().GetSelectedIndex();
            GuildInfo guildInfoByIndex = this.m_Model.GetGuildInfoByIndex(selectedIndex);
            if (guildInfoByIndex != null)
            {
                uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 10).dwConfValue;
                if ((dwConfValue != 0) && (dwConfValue > Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().PvpLevel))
                {
                    string[] args = new string[] { dwConfValue.ToString() };
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Guild_Join_Level_Limit", args), false, 1.5f, null, new object[0]);
                }
                else if (this.m_Model.GetAppliedGuildInfoByUid(guildInfoByIndex.stBriefInfo.uulUid).stBriefInfo.uulUid != 0)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Guild_Current_Guild_Has_Invited_Tip", true, 1.5f, null, new object[0]);
                }
                else if (!CGuildHelper.IsInLastQuitGuildCd())
                {
                    if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.bApplyJoinGuildNum > 0)
                    {
                        uint num3 = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 8).dwConfValue;
                        if ((Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.bApplyJoinGuildNum + 1) > num3)
                        {
                            Singleton<CUIManager>.GetInstance().OpenTips("Guild_Today_Apply_Reach_Limit_Tip", true, 1.5f, null, new object[0]);
                            return;
                        }
                    }
                    if (this.m_Model.IsInGuildStep())
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips("Guild_In_Guild_Step_Tip", true, 1.5f, null, new object[0]);
                    }
                    else
                    {
                        Singleton<EventRouter>.GetInstance().BroadCastEvent<GuildInfo>("Request_Apply_Guild_Join", guildInfoByIndex);
                    }
                }
            }
        }

        private void On_Guild_Guild_List_Element_Enabled(CUIEvent uiEvent)
        {
            GuildInfo guildInfoByIndex = this.m_Model.GetGuildInfoByIndex(uiEvent.m_srcWidgetIndexInBelongedList);
            if (guildInfoByIndex != null)
            {
                this.SetGuildListItem(uiEvent.m_srcWidgetScript as CUIListElementScript, guildInfoByIndex);
            }
        }

        private void On_Guild_Guild_Search_In_Guild_List_Panel(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                string text = srcFormScript.GetWidget(9).GetComponent<InputField>().text;
                if (!string.IsNullOrEmpty(text))
                {
                    Singleton<CGuildSystem>.GetInstance().SearchGuild(0L, text, 0, false);
                }
            }
        }

        private void On_Guild_Guild_Search_In_Prepare_Guild_List_Panel(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                string text = srcFormScript.GetWidget(0x16).GetComponent<InputField>().text;
                if (!string.IsNullOrEmpty(text))
                {
                    Singleton<CGuildSystem>.GetInstance().SearchGuild(0L, text, 0, true);
                }
            }
        }

        private void On_Guild_Guild_Select(CUIEvent uiEvent)
        {
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            GuildInfo guildInfoByIndex = this.m_Model.GetGuildInfoByIndex(selectedIndex);
            if (guildInfoByIndex != null)
            {
                this.m_form.GetWidget(0x22).GetComponent<CUIHttpImageScript>().SetImageUrl(CGuildHelper.GetHeadUrl(guildInfoByIndex.stChairman.stBriefInfo.szHeadUrl));
                MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(this.m_form.GetWidget(0x29).GetComponent<Image>(), CGuildHelper.GetNobeLevel(guildInfoByIndex.stChairman.stBriefInfo.uulUid, guildInfoByIndex.stChairman.stBriefInfo.stVip.level), false);
                MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(this.m_form.GetWidget(40).GetComponent<Image>(), CGuildHelper.GetNobeHeadIconId(guildInfoByIndex.stChairman.stBriefInfo.uulUid, guildInfoByIndex.stChairman.stBriefInfo.stVip.headIconId));
                this.m_form.GetWidget(10).GetComponent<Text>().text = guildInfoByIndex.stBriefInfo.sBulletin;
                this.m_form.GetWidget(11).GetComponent<Text>().text = guildInfoByIndex.stChairman.stBriefInfo.sName;
                string[] args = new string[] { guildInfoByIndex.stChairman.stBriefInfo.dwLevel.ToString() };
                this.m_form.GetWidget(0x27).GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Common_Level_Format", args);
            }
        }

        private void On_Guild_Only_Friend_Slider_Value_Changed(CUIEvent uiEvent)
        {
            int sliderValue = (int) uiEvent.m_eventParams.sliderValue;
            this.m_form.GetWidget(0x1d).GetComponent<Text>().text = (sliderValue != 0) ? Singleton<CTextManager>.GetInstance().GetText("Common_No") : Singleton<CTextManager>.GetInstance().GetText("Common_Yes");
        }

        private void On_Guild_Prepare_Guild_List_Element_Enabled(CUIEvent uiEvent)
        {
            PrepareGuildInfo prepareGuildInfoByIndex = this.m_Model.GetPrepareGuildInfoByIndex(uiEvent.m_srcWidgetIndexInBelongedList);
            if (prepareGuildInfoByIndex != null)
            {
                this.SetPrepareGuildListItem(uiEvent.m_srcWidgetScript as CUIListElementScript, prepareGuildInfoByIndex);
            }
        }

        private void On_Guild_Prepare_Guild_Select(CUIEvent uiEvent)
        {
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            PrepareGuildInfo prepareGuildInfoByIndex = this.m_Model.GetPrepareGuildInfoByIndex(selectedIndex);
            DebugHelper.Assert(prepareGuildInfoByIndex != null);
            if (prepareGuildInfoByIndex != null)
            {
                this.m_form.GetWidget(0x23).GetComponent<CUIHttpImageScript>().SetImageUrl(CGuildHelper.GetHeadUrl(prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.szHeadUrl));
                Image component = this.m_form.GetWidget(0x2d).GetComponent<Image>();
                MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component, CGuildHelper.GetNobeLevel(prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.uulUid, prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.stVip.level), false);
                Image image = this.m_form.GetWidget(0x2c).GetComponent<Image>();
                MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image, CGuildHelper.GetNobeHeadIconId(prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.uulUid, prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.stVip.headIconId));
                this.m_form.GetWidget(14).GetComponent<Text>().text = prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.sName;
                string[] args = new string[] { prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.dwLevel.ToString() };
                this.m_form.GetWidget(15).GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Common_Level_Format", args);
                this.m_form.GetWidget(0x10).GetComponent<Text>().text = prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.dwGameEntity.ToString();
                this.m_form.GetWidget(0x15).GetComponent<Text>().text = prepareGuildInfoByIndex.stBriefInfo.sBulletin;
            }
        }

        private void On_Guild_PrepareGuild_Create(CUIEvent uiEvent)
        {
            Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "3";
            Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.time;
            Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_channel = "3";
            Singleton<BeaconHelper>.GetInstance().m_curBuyPropInfo.buy_prop_id_time = Time.time;
            int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.dwLastCreateGuildTime != 0)
            {
                uint num2 = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 3).dwConfValue;
                if (currentUTCTime < (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.dwLastCreateGuildTime + num2))
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Guild_Last_Create_Time_Tip", true, 1.5f, null, new object[0]);
                    return;
                }
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 5).dwConfValue;
            if (masterRoleInfo.DianQuan < dwConfValue)
            {
                CUICommonSystem.OpenDianQuanNotEnoughTip();
            }
            else
            {
                uint num4 = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 10).dwConfValue;
                if ((num4 != 0) && (num4 > Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().PvpLevel))
                {
                    string[] args = new string[] { num4.ToString() };
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Guild_Join_Level_Limit", args), false, 1.5f, null, new object[0]);
                }
                else
                {
                    string str = CUIUtility.RemoveEmoji(this.m_form.GetWidget(0).GetComponent<Text>().text).Trim();
                    if (string.IsNullOrEmpty(str))
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips("Guild_Input_Guild_Name_Empty", true, 1.5f, null, new object[0]);
                    }
                    else if (!Utility.IsValidText(str))
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips("Guild_Input_Guild_Name_Invalid", true, 1.5f, null, new object[0]);
                    }
                    else if (string.IsNullOrEmpty(CUIUtility.RemoveEmoji(this.m_form.GetWidget(1).GetComponent<Text>().text).Trim()))
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips("Guild_Input_Guild_Bulletin_Empty", true, 1.5f, null, new object[0]);
                    }
                    else
                    {
                        uint num5 = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 2).dwConfValue;
                        uint num6 = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 1).dwConfValue;
                        TimeSpan span = new TimeSpan(0, 0, 0, (int) num5);
                        string[] textArray2 = new string[] { span.TotalHours.ToString(), num6.ToString() };
                        string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Create_Tip", textArray2);
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_PrepareGuild_Create_Confirm, enUIEventID.Guild_PrepareGuild_Create_Cancel, false);
                    }
                }
            }
        }

        private void On_Guild_PrepareGuild_Create_Cancel(CUIEvent uiEvent)
        {
        }

        private void On_Guild_PrepareGuild_Create_Confirm(CUIEvent uiEvent)
        {
            if (this.m_Model.IsInGuildStep())
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_In_Guild_Step_Tip_2", true, 1.5f, null, new object[0]);
            }
            else
            {
                Text component = this.m_form.GetWidget(0).GetComponent<Text>();
                Text text2 = this.m_form.GetWidget(1).GetComponent<Text>();
                Text text3 = this.m_form.GetWidget(8).GetComponent<Text>();
                stPrepareGuildCreateInfo info = new stPrepareGuildCreateInfo {
                    sName = component.text.Trim(),
                    sBulletin = text2.text.Trim(),
                    dwHeadId = Convert.ToUInt32(text3.text),
                    isOnlyFriend = false
                };
                Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
                Singleton<EventRouter>.GetInstance().BroadCastEvent<stPrepareGuildCreateInfo>("PrepareGuild_Create", info);
            }
        }

        private void On_Guild_PrepareGuild_Create_Icon_Selected(CUIEvent uiEvent)
        {
            CUIListScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListScript;
            if (srcWidgetScript != null)
            {
                int selectedIndex = srcWidgetScript.GetSelectedIndex();
                if (selectedIndex == -1)
                {
                    selectedIndex = 0;
                }
                CUIListElementScript elemenet = srcWidgetScript.GetElemenet(selectedIndex);
                if (elemenet != null)
                {
                    Text component = elemenet.transform.Find("imgIcon/txtIconIdData").GetComponent<Text>();
                    this.m_form.GetWidget(8).GetComponent<Text>().text = component.text;
                    this.m_form.GetWidget(4).GetComponent<Image>().SetSprite(elemenet.transform.Find("imgIcon").GetComponent<Image>());
                    CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
                    if (srcFormScript != null)
                    {
                        srcFormScript.Close();
                    }
                }
            }
        }

        private void On_Guild_PrepareGuild_Create_Modify_Icon(CUIEvent uiEvent)
        {
            this.OpenIconForm();
        }

        private void On_Guild_PrepareGuild_Join(CUIEvent uiEvent)
        {
            PrepareGuildInfo prepareGuildInfoByIndex = this.m_Model.GetPrepareGuildInfoByIndex(this.m_form.GetWidget(0x1b).GetComponent<CUIListScript>().GetSelectedIndex());
            if (((prepareGuildInfoByIndex != null) && prepareGuildInfoByIndex.stBriefInfo.IsOnlyFriend) && (Singleton<CFriendContoller>.GetInstance().model.getFriendByName(prepareGuildInfoByIndex.stBriefInfo.stCreatePlayer.sName, CFriendModel.FriendType.GameFriend) == null))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Only_Friend_Can_Join_Tip", true, 1.5f, null, new object[0]);
            }
            else
            {
                uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 10).dwConfValue;
                if ((dwConfValue != 0) && (dwConfValue > Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().PvpLevel))
                {
                    string[] args = new string[] { dwConfValue.ToString() };
                    Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Guild_Join_Level_Limit", args), false, 1.5f, null, new object[0]);
                }
                else
                {
                    if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.dwLastQuitGuildTime != 0)
                    {
                        int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
                        uint num3 = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 6).dwConfValue;
                        int seconds = ((int) (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.dwLastQuitGuildTime + num3)) - currentUTCTime;
                        TimeSpan span = new TimeSpan(0, 0, 0, seconds);
                        if (seconds > 0)
                        {
                            string[] textArray2 = new string[] { ((int) span.TotalMinutes).ToString(), span.Seconds.ToString() };
                            Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Guild_Cannot_Apply_Tip", textArray2), false, 1.5f, null, new object[0]);
                            return;
                        }
                    }
                    string text = Singleton<CTextManager>.GetInstance().GetText("Guild_Response_Tip");
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Guild_PrepareGuild_Join_Confirm, enUIEventID.Guild_PrepareGuild_Join_Cancel, false);
                }
            }
        }

        private void On_Guild_PrepareGuild_Join_Cancel(CUIEvent uiEvent)
        {
        }

        private void On_Guild_PrepareGuild_Join_Confirm(CUIEvent uiEvent)
        {
            if (!this.m_Model.IsInGuildStep())
            {
                PrepareGuildInfo prepareGuildInfoByIndex = this.m_Model.GetPrepareGuildInfoByIndex(this.m_form.GetWidget(0x1b).GetComponent<CUIListScript>().GetSelectedIndex());
                Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
                Singleton<EventRouter>.GetInstance().BroadCastEvent<PrepareGuildInfo>("PrepareGuild_Join", prepareGuildInfoByIndex);
            }
        }

        private void On_Guild_PrepareGuild_Timeout(CUIEvent uiEvent)
        {
            switch (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState)
            {
                case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_CREATE:
                case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_JOIN:
                    Singleton<EventRouter>.GetInstance().BroadCastEvent("Request_PrepareGuild_Info");
                    break;

                default:
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<int>("Request_PrepareGuild_List", 0);
                    break;
            }
        }

        private void On_Guild_Requesst_More_Prepare_Guild_List(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if ((srcFormScript != null) && (srcFormScript.GetWidget(0x1b).GetComponent<CUIListScript>().GetElementAmount() > 0))
            {
                int num2 = int.Parse(srcFormScript.GetWidget(0x26).GetComponent<Text>().text) + 1;
                Singleton<EventRouter>.GetInstance().BroadCastEvent<int>("Request_PrepareGuild_List", num2);
            }
        }

        private void On_Guild_Request_More_Guild_List(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                int elementAmount = srcFormScript.GetWidget(30).GetComponent<CUIListScript>().GetElementAmount();
                if (elementAmount > 0)
                {
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<int, int>("Request_Guild_List", elementAmount + 1, 20);
                }
            }
        }

        private void On_Tab_Change(CUIEvent uiEvent)
        {
            CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            if (component != null)
            {
                int selectedIndex = component.GetSelectedIndex();
                this.CurTab = (Tab) selectedIndex;
                this.InitPanel();
            }
        }

        public void OpenForm(Tab selectTab = 3, bool isDispatchTabChangeEvent = true)
        {
            if (!this.IsShow())
            {
                this.m_form = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_List.prefab", false, true);
                this.InitTabList();
                this.SelectTabElement(selectTab, isDispatchTabChangeEvent);
            }
        }

        private void OpenIconForm()
        {
            <OpenIconForm>c__AnonStorey70 storey = new <OpenIconForm>c__AnonStorey70 {
                <>f__this = this
            };
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Guild/Form_Guild_Icon.prefab", false, true);
            storey.listScript = script.GetWidget(0).GetComponent<CUIListScript>();
            int count = GameDataMgr.guildIconDatabin.count;
            storey.listScript.SetElementAmount(count);
            storey.i = 0;
            GameDataMgr.guildIconDatabin.Accept(new Action<ResGuildIcon>(storey.<>m__5A));
        }

        public void RefreshCreateGuildPanel()
        {
            if (this.IsShow())
            {
                this.m_form.GetWidget(7).CustomSetActive(true);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                Text component = this.m_form.GetWidget(3).GetComponent<Text>();
                uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 5).dwConfValue;
                if (masterRoleInfo.DianQuan < dwConfValue)
                {
                    component.text = "<color=#a52a2aff>" + dwConfValue.ToString() + "</color>/" + masterRoleInfo.DianQuan.ToString();
                }
                else
                {
                    component.text = "<color=#00ff00>" + dwConfValue.ToString() + "</color>/" + masterRoleInfo.DianQuan.ToString();
                }
                Image image = this.m_form.GetWidget(4).GetComponent<Image>();
                Text text2 = this.m_form.GetWidget(8).GetComponent<Text>();
                ResGuildIcon anyData = GameDataMgr.guildIconDatabin.GetAnyData();
                if (anyData != null)
                {
                    string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + anyData.dwIcon;
                    image.SetSprite(prefabPath, this.m_form, true, false, false);
                    text2.text = anyData.dwIcon.ToString();
                }
            }
        }

        public void RefreshGuildListPanel(bool isHideListExtraContent = false)
        {
            if (this.IsShow())
            {
                CUIListScript component;
                this.m_form.GetWidget(5).CustomSetActive(true);
                switch (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState)
                {
                    case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL:
                    case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_CREATE:
                    case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_JOIN:
                    case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_WAIT_RESULT:
                    {
                        int guildInfoCount = this.m_Model.GetGuildInfoCount();
                        component = this.m_form.GetWidget(30).GetComponent<CUIListScript>();
                        component.SetElementAmount(guildInfoCount);
                        GameObject widget = this.m_form.GetWidget(0x24);
                        GameObject obj4 = this.m_form.GetWidget(0x25);
                        GameObject obj5 = this.m_form.GetWidget(0x1f);
                        if (guildInfoCount <= 0)
                        {
                            widget.CustomSetActive(false);
                            obj4.CustomSetActive(false);
                            obj5.CustomSetActive(false);
                            break;
                        }
                        component.SelectElement(0, true);
                        widget.CustomSetActive(true);
                        obj4.CustomSetActive(true);
                        obj5.CustomSetActive(true);
                        break;
                    }
                    case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN:
                    case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_VICE_CHAIRMAN:
                    case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_ELDER:
                    case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_MEMBER:
                        return;

                    default:
                        return;
                }
                if (isHideListExtraContent)
                {
                    component.HideExtraContent();
                }
            }
        }

        public void RefreshPrepareGuildPanel(bool isForceShowPrepareGuildList = false, byte pageId = 0, bool isHideListExtraContent = false)
        {
            if (this.IsShow())
            {
                this.m_form.GetWidget(6).CustomSetActive(true);
                if (isForceShowPrepareGuildList)
                {
                    this.RefreshPrepareGuildPanelPrepareGuildList(pageId, isHideListExtraContent);
                }
                else
                {
                    switch (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState)
                    {
                        case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL:
                            this.RefreshPrepareGuildPanelPrepareGuildList(pageId, isHideListExtraContent);
                            break;

                        case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_CREATE:
                        case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_JOIN:
                            this.RefreshPrepareGuildPanelMemberList();
                            break;
                    }
                }
            }
        }

        private void RefreshPrepareGuildPanelMemberList()
        {
            GameObject widget = this.m_form.GetWidget(12);
            GameObject obj3 = this.m_form.GetWidget(13);
            GameObject obj4 = this.m_form.GetWidget(0x17);
            GameObject obj5 = this.m_form.GetWidget(0x19);
            GameObject obj6 = this.m_form.GetWidget(0x1a);
            GameObject obj7 = this.m_form.GetWidget(0x18);
            obj3.CustomSetActive(true);
            obj4.CustomSetActive(true);
            obj7.CustomSetActive(true);
            widget.CustomSetActive(false);
            obj5.CustomSetActive(false);
            obj6.CustomSetActive(false);
            CUIListScript component = this.m_form.GetWidget(0x1c).GetComponent<CUIListScript>();
            ListView<GuildMemInfo> memList = this.m_Model.CurrentPrepareGuildInfo.m_MemList;
            int amount = this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.bMemCnt - 1;
            component.SetElementAmount(amount);
            if (amount > 0)
            {
                component.SelectElement(0, true);
            }
            int index = 0;
            for (int i = 0; i < this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.bMemCnt; i++)
            {
                if (memList[i].stBriefInfo.uulUid != this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.uulUid)
                {
                    CUIListElementScript elemenet = component.GetElemenet(index);
                    if (elemenet != null)
                    {
                        this.SetPrepareGuildMemListItem(elemenet, memList[i]);
                    }
                    index++;
                }
            }
            CUIHttpImageScript script3 = this.m_form.GetWidget(0x23).GetComponent<CUIHttpImageScript>();
            Image image = this.m_form.GetWidget(0x2d).GetComponent<Image>();
            Image image2 = this.m_form.GetWidget(0x2c).GetComponent<Image>();
            Text text = this.m_form.GetWidget(14).GetComponent<Text>();
            Text text2 = this.m_form.GetWidget(15).GetComponent<Text>();
            Text text3 = this.m_form.GetWidget(0x10).GetComponent<Text>();
            script3.SetImageUrl(CGuildHelper.GetHeadUrl(this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.szHeadUrl));
            text.text = this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.sName;
            string[] args = new string[] { this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.dwLevel.ToString() };
            text2.text = Singleton<CTextManager>.GetInstance().GetText("Common_Level_Format", args);
            text3.text = this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.dwAbility.ToString();
            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, CGuildHelper.GetNobeLevel(this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.uulUid, this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.stVip.level), false);
            MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, CGuildHelper.GetNobeHeadIconId(this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.uulUid, this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.stCreatePlayer.stVip.headIconId));
            Text text4 = this.m_form.GetWidget(0x12).GetComponent<Text>();
            Text text5 = this.m_form.GetWidget(0x13).GetComponent<Text>();
            Image image3 = this.m_form.GetWidget(0x11).GetComponent<Image>();
            uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 1).dwConfValue;
            dwConfValue = (dwConfValue >= 0) ? dwConfValue : 0;
            CUITimerScript script4 = this.m_form.GetWidget(20).GetComponent<CUITimerScript>();
            uint num5 = this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.dwRequestTime + GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 2).dwConfValue;
            int seconds = ((int) num5) - CRoleInfo.GetCurrentUTCTime();
            if (seconds < 0)
            {
                seconds = 0;
            }
            TimeSpan span = new TimeSpan(0, 0, 0, seconds);
            script4.SetTotalTime((float) span.TotalSeconds);
            script4.StartTimer();
            text4.text = this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.sName;
            string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + this.m_Model.CurrentPrepareGuildInfo.stBriefInfo.dwHeadId;
            image3.SetSprite(prefabPath, this.m_form, true, false, false);
            text5.text = amount.ToString() + "/" + dwConfValue.ToString();
        }

        private void RefreshPrepareGuildPanelPrepareGuildList(byte pageId, bool isHideListExtraContent)
        {
            GameObject widget = this.m_form.GetWidget(12);
            GameObject obj3 = this.m_form.GetWidget(13);
            GameObject obj4 = this.m_form.GetWidget(0x17);
            GameObject obj5 = this.m_form.GetWidget(0x19);
            GameObject obj6 = this.m_form.GetWidget(0x1a);
            GameObject obj7 = this.m_form.GetWidget(0x18);
            widget.CustomSetActive(true);
            obj3.CustomSetActive(false);
            obj7.CustomSetActive(false);
            CUIListScript component = this.m_form.GetWidget(0x1b).GetComponent<CUIListScript>();
            int prepareGuildInfoCount = this.m_Model.GetPrepareGuildInfoCount();
            component.SetElementAmount(prepareGuildInfoCount);
            if (prepareGuildInfoCount > 0)
            {
                component.SelectElement(0, true);
                obj4.CustomSetActive(true);
                obj5.CustomSetActive(true);
                obj6.CustomSetActive(true);
            }
            else
            {
                obj4.CustomSetActive(false);
                obj5.CustomSetActive(false);
                obj6.CustomSetActive(false);
            }
            if (isHideListExtraContent)
            {
                component.HideExtraContent();
            }
            this.m_form.GetWidget(0x26).GetComponent<Text>().text = pageId.ToString();
        }

        public void SelectTabElement(Tab defaultTab = 3, bool isDisableTabChangeEvent = true)
        {
            if (defaultTab == Tab.None)
            {
                switch (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState)
                {
                    case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_CREATE:
                    case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_PREPARE_JOIN:
                        this.CurTab = Tab.PrepareGuild;
                        goto Label_0053;
                }
                this.CurTab = Tab.Guild;
            }
            else
            {
                this.CurTab = defaultTab;
            }
        Label_0053:
            this.m_form.GetWidget(0x21).GetComponent<CUIListScript>().SelectElement((int) this.CurTab, isDisableTabChangeEvent);
        }

        private void SetGuildIcon(CUIListElementScript listElementScript, ResGuildIcon info)
        {
            string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + info.dwIcon;
            listElementScript.transform.Find("imgIcon").GetComponent<Image>().SetSprite(prefabPath, listElementScript.m_belongedFormScript, true, false, false);
            listElementScript.transform.Find("imgIcon/txtIconIdData").GetComponent<Text>().text = info.dwIcon.ToString();
        }

        private void SetGuildListItem(CUIListElementScript listElementScript, GuildInfo info)
        {
            Transform transform = listElementScript.transform;
            Image component = transform.Find("imgIcon").GetComponent<Image>();
            Text text = transform.Find("txtName").GetComponent<Text>();
            Text text2 = transform.Find("txtBattle").GetComponent<Text>();
            Text text3 = transform.Find("txtLevel").GetComponent<Text>();
            Text text4 = transform.Find("txtMemCnt").GetComponent<Text>();
            GameObject gameObject = transform.Find("imgApplied").gameObject;
            Text text5 = transform.Find("txtChairmanName").GetComponent<Text>();
            string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + info.stBriefInfo.dwHeadId;
            component.SetSprite(prefabPath, this.m_form, true, false, false);
            text.text = info.stBriefInfo.sName;
            text2.text = info.stBriefInfo.Ability.ToString();
            text3.text = info.stBriefInfo.bLevel.ToString();
            text4.text = info.stBriefInfo.bMemCnt + "/" + CGuildHelper.GetMaxGuildMemberCountByLevel(info.stBriefInfo.bLevel);
            text5.text = info.stChairman.stBriefInfo.sName;
            if (this.m_Model.GetAppliedGuildInfoByUid(info.stBriefInfo.uulUid).stBriefInfo.uulUid != 0)
            {
                gameObject.CustomSetActive(true);
            }
            else
            {
                gameObject.CustomSetActive(false);
            }
        }

        private void SetPrepareGuildListItem(CUIListElementScript listElementScript, PrepareGuildInfo info)
        {
            Transform transform = listElementScript.transform;
            Image component = transform.Find("imgIcon").GetComponent<Image>();
            Text text = transform.Find("txtName").GetComponent<Text>();
            Text text2 = transform.Find("txtCreator").GetComponent<Text>();
            Text text3 = transform.Find("txtMemCnt").GetComponent<Text>();
            CUITimerScript script = transform.Find("timeoutTimer").GetComponent<CUITimerScript>();
            GameObject gameObject = transform.Find("imgOnlyFriend").gameObject;
            string prefabPath = CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + info.stBriefInfo.dwHeadId;
            component.SetSprite(prefabPath, this.m_form, true, false, false);
            text.text = info.stBriefInfo.sName;
            text2.text = info.stBriefInfo.stCreatePlayer.sName;
            text3.text = (info.stBriefInfo.bMemCnt - 1) + "/" + GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 1).dwConfValue;
            uint num = info.stBriefInfo.dwRequestTime + GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 2).dwConfValue;
            int seconds = ((int) num) - CRoleInfo.GetCurrentUTCTime();
            if (seconds < 0)
            {
                seconds = 0;
            }
            TimeSpan span = new TimeSpan(0, 0, 0, seconds);
            script.SetTotalTime((float) span.TotalSeconds);
            script.StartTimer();
            gameObject.CustomSetActive(info.stBriefInfo.IsOnlyFriend);
        }

        private void SetPrepareGuildMemListItem(CUIListElementScript listElementScript, GuildMemInfo info)
        {
            Transform transform = listElementScript.transform;
            CUIHttpImageScript component = transform.Find("imgHead").GetComponent<CUIHttpImageScript>();
            Image image = component.transform.Find("NobeIcon").GetComponent<Image>();
            Image image2 = component.transform.Find("NobeImag").GetComponent<Image>();
            Text text = transform.Find("txtName").GetComponent<Text>();
            Text text2 = transform.Find("txtLevel").GetComponent<Text>();
            Text text3 = transform.Find("txtBattle").GetComponent<Text>();
            component.SetImageUrl(CGuildHelper.GetHeadUrl(info.stBriefInfo.szHeadUrl));
            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(image, CGuildHelper.GetNobeLevel(info.stBriefInfo.uulUid, info.stBriefInfo.stVip.level), false);
            MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(image2, CGuildHelper.GetNobeHeadIconId(info.stBriefInfo.uulUid, info.stBriefInfo.stVip.headIconId));
            text.text = info.stBriefInfo.sName;
            text2.text = info.stBriefInfo.dwLevel.ToString();
            text3.text = info.stBriefInfo.dwAbility.ToString();
        }

        public override void UnInit()
        {
        }

        public Tab CurTab
        {
            get
            {
                return this.m_curTab;
            }
            set
            {
                this.m_curTab = value;
                Singleton<EventRouter>.GetInstance().BroadCastEvent<Tab>("Guild_List_Tab_Change", this.m_curTab);
            }
        }

        [CompilerGenerated]
        private sealed class <OpenIconForm>c__AnonStorey70
        {
            internal CGuildListView <>f__this;
            internal int i;
            internal CUIListScript listScript;

            internal void <>m__5A(ResGuildIcon x)
            {
                CUIListElementScript elemenet = this.listScript.GetElemenet(this.i);
                this.<>f__this.SetGuildIcon(elemenet, x);
                this.i++;
            }
        }

        public enum enGuildIconFormWidget
        {
            Icon_List
        }

        public enum enGuildListFormWidget
        {
            GuildCreatePanel_GuildNameText,
            GuildCreatePanel_GuildBulletinText,
            GuildCreatePanel_CostCoinText,
            GuildCreatePanel_CostDianQuanText,
            GuildCreatePanel_GuildIconImage,
            GuildListPanel,
            GuildPrepareListPanel,
            GuildCreatePanel,
            GuildCreatePanel_GuildIconIdText,
            GuildListPanel_SearchGuildInput,
            GuildListPanel_GuildBulletinText,
            GuildListPanel_GuildChairmanNameText,
            GuildPrepareListPanel_GuildListPanel,
            GuildPrepareListPanel_MemberListPanel,
            GuildPrepareListPanel_CreatorNameText,
            GuildPrepareListPanel_CreatorLevelText,
            GuildPrepareListPanel_CreatorBattleText,
            GuildPrepareListPanel_GuildIconImage,
            GuildPrepareListPanel_GuildNameText,
            GuildPrepareListPanel_GuildMemberCountText,
            GuildPrepareListPanel_GuildTimeoutTimer,
            GuildPrepareListPanel_BulletinText,
            GuildPrepareListPanel_SearchGuildInput,
            GuildPrepareListPanel_GuildCreatorPanel,
            GuildPrepareListPanel_GuildInfoPanel,
            GuildPrepareListPanel_GuildBulletinPanel,
            GuildPrepareListPanel_GuildOperationPanel,
            GuildPrepareListPanel_GuildList,
            GuildPrepareListPanel_GuildMemberList,
            GuildCreatePanel_IsOnlyFriendSliderHandleText,
            GuildListPanel_GuildList,
            GuildListPanel_GuildOperationPanel,
            GuildCreatePanel_IsOnlyFriendSlider,
            PanelTab,
            GuildListPanel_ChairmanHeadIconImage,
            GuildPrepareListPanel_CreatorHeadIconImage,
            GuildListPanel_GuildChairmanPanel,
            GuildListPanel_GuildBulletinPanel,
            GuildPrepareListPanel_PageIdDataText,
            GuildListPanel_GuildChairmanLevelText,
            GuildListPanel_ChairmanNobeBgImage,
            GuildListPanel_ChairmanNobeIconImage,
            GuildPrepareListPanel_MemberNobeBgImage,
            GuildPrepareListPanel_MemberNobeIconImage,
            GuildPrepareListPanel_CreatorNobeBgImage,
            GuildPrepareListPanel_CreatorNobeIconImage
        }

        public enum Tab
        {
            Guild,
            PrepareGuild,
            CreateGuild,
            None
        }
    }
}

