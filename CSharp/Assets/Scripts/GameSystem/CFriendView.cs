namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class CFriendView
    {
        private Tab _tab;
        private GameObject addFriendBtnGameObject;
        public AddFriendView addFriendView = new AddFriendView();
        private Text btnText;
        private CUIFormScript friendform;
        public CUIListScript friendListCom;
        public GameObject friendListNode;
        public Text ifnoText;
        private GameObject info_node;
        private GameObject lbs_node;
        private Button lbsRefreshBtn;
        private Toggle localtionToggle;
        private GameObject m_QQboxBtn;
        private Toggle nanToggle;
        private Toggle nvToggle;
        private GameObject rule_btn_node;
        private GameObject sns_add_switch;
        private GameObject sns_invite_btn;
        private GameObject sns_share_switch;
        private CUIListScript tablistScript;
        public Verfication verficationView = new Verfication();

        private COMDT_FRIEND_INFO _get_current_info(Tab type, int index)
        {
            COMDT_FRIEND_INFO infoAtIndex = null;
            if (type == Tab.Friend_SNS)
            {
                infoAtIndex = Singleton<CFriendContoller>.GetInstance().model.GetInfoAtIndex(CFriendModel.FriendType.SNS, index);
            }
            if (type == Tab.Friend_Request)
            {
                infoAtIndex = Singleton<CFriendContoller>.GetInstance().model.GetInfoAtIndex(CFriendModel.FriendType.RequestFriend, index);
            }
            if (type == Tab.Friend)
            {
                infoAtIndex = Singleton<CFriendContoller>.GetInstance().model.GetInfoAtIndex(CFriendModel.FriendType.GameFriend, index);
            }
            return infoAtIndex;
        }

        private void _refresh_black_list(CUIListScript listScript, List<CFriendModel.stBlackName> blackList)
        {
            if (listScript != null)
            {
                int count = blackList.Count;
                listScript.SetElementAmount(count);
                for (int i = 0; i < count; i++)
                {
                    CUIListElementScript elemenet = listScript.GetElemenet(i);
                    if ((elemenet != null) && listScript.IsElementInScrollArea(i))
                    {
                        FriendShower component = elemenet.GetComponent<FriendShower>();
                        CFriendModel.stBlackName info = blackList[i];
                        if (component != null)
                        {
                            UT.ShowBlackListData(ref info, component);
                        }
                    }
                }
            }
        }

        private void _refresh_LBS_list(CUIListScript listScript, ListView<CSDT_LBS_USER_INFO> LBSList)
        {
            if (listScript != null)
            {
                if (LBSList == null)
                {
                    listScript.SetElementAmount(0);
                }
                else
                {
                    int count = LBSList.Count;
                    listScript.SetElementAmount(count);
                }
            }
        }

        private void _refresh_list(CUIListScript listScript, ListView<COMDT_FRIEND_INFO> data_list, FriendShower.ItemType type, bool bShowNickName, CFriendModel.FriendType friend)
        {
            if (listScript != null)
            {
                int count = data_list.Count;
                listScript.SetElementAmount(count);
                for (int i = 0; i < count; i++)
                {
                    CUIListElementScript elemenet = listScript.GetElemenet(i);
                    if ((elemenet != null) && listScript.IsElementInScrollArea(i))
                    {
                        FriendShower component = elemenet.GetComponent<FriendShower>();
                        COMDT_FRIEND_INFO info = data_list[i];
                        if ((component != null) && (info != null))
                        {
                            UT.ShowFriendData(info, component, type, bShowNickName, friend);
                            if (component.sendHeartButton != null)
                            {
                                if (friend == CFriendModel.FriendType.GameFriend)
                                {
                                    component.sendHeartBtn_eventScript.m_onClickEventID = enUIEventID.Friend_SendCoin;
                                }
                                else if (friend == CFriendModel.FriendType.SNS)
                                {
                                    component.sendHeartBtn_eventScript.m_onClickEventID = enUIEventID.Friend_SNS_SendCoin;
                                }
                                component.sendHeartBtn_eventScript.m_onClickEventParams.commonUInt64Param1 = info.stUin.ullUid;
                                component.sendHeartBtn_eventScript.m_onClickEventParams.commonUInt64Param2 = info.stUin.dwLogicWorldId;
                            }
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            if (this.tablistScript != null)
            {
                CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(0).gameObject);
                CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(1).gameObject);
                if (!CSysDynamicBlock.bSocialBlocked)
                {
                    CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(2).gameObject);
                }
            }
            this.friendListNode = null;
            this.friendListCom = null;
            this.addFriendBtnGameObject = null;
            this.info_node = null;
            this.btnText = null;
            this.ifnoText = null;
            this.friendform = null;
            this.sns_invite_btn = null;
            this.lbs_node = null;
            this.m_QQboxBtn = null;
            this.sns_share_switch = null;
            this.sns_add_switch = null;
            this.rule_btn_node = null;
            this.localtionToggle = this.nanToggle = (Toggle) (this.nvToggle = null);
            this.lbsRefreshBtn = null;
            this.tablistScript = null;
            if (this.addFriendView != null)
            {
                this.addFriendView.Clear();
            }
        }

        public void CloseForm()
        {
            this.Clear();
        }

        public Tab GetSelectedTab()
        {
            if (this.tablistScript != null)
            {
                return (Tab) this.tablistScript.GetSelectedIndex();
            }
            return Tab.None;
        }

        public bool IsActive()
        {
            return (this.friendform != null);
        }

        public void On_Friend_Invite_SNS_Friend(CUIEvent uiEvent)
        {
            if (MonoSingleton<ShareSys>.instance.IsInstallPlatform())
            {
                string text = UT.GetText("Friend_Invite_SNS_Title");
                string desc = UT.GetText("Friend_Invite_SNS_Desc");
                Singleton<ApolloHelper>.GetInstance().ShareToFriend(text, desc);
            }
        }

        public void On_List_ElementEnable(CUIEvent uievent)
        {
            int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
            FriendShower component = uievent.m_srcWidget.GetComponent<FriendShower>();
            if (component != null)
            {
                if (this.CurTab == Tab.Friend_SNS)
                {
                    COMDT_FRIEND_INFO info = this._get_current_info(this.CurTab, srcWidgetIndexInBelongedList);
                    UT.ShowFriendData(info, component, FriendShower.ItemType.Normal, true, CFriendModel.FriendType.SNS);
                    component.sendHeartBtn_eventScript.m_onClickEventID = enUIEventID.Friend_SNS_SendCoin;
                    component.sendHeartBtn_eventScript.m_onClickEventParams.commonUInt64Param1 = info.stUin.ullUid;
                    component.sendHeartBtn_eventScript.m_onClickEventParams.commonUInt64Param2 = info.stUin.dwLogicWorldId;
                }
                else if (this.CurTab == Tab.Friend)
                {
                    COMDT_FRIEND_INFO comdt_friend_info2 = this._get_current_info(this.CurTab, srcWidgetIndexInBelongedList);
                    UT.ShowFriendData(comdt_friend_info2, component, FriendShower.ItemType.Normal, false, CFriendModel.FriendType.GameFriend);
                    component.sendHeartBtn_eventScript.m_onClickEventID = enUIEventID.Friend_SendCoin;
                    component.sendHeartBtn_eventScript.m_onClickEventParams.commonUInt64Param1 = comdt_friend_info2.stUin.ullUid;
                    component.sendHeartBtn_eventScript.m_onClickEventParams.commonUInt64Param2 = comdt_friend_info2.stUin.dwLogicWorldId;
                }
                else if (this.CurTab == Tab.Friend_Request)
                {
                    UT.ShowFriendData(this._get_current_info(this.CurTab, srcWidgetIndexInBelongedList), component, FriendShower.ItemType.Request, false, CFriendModel.FriendType.RequestFriend);
                }
                else if (this.CurTab == Tab.Friend_BlackList)
                {
                    List<CFriendModel.stBlackName> blackList = Singleton<CFriendContoller>.instance.model.GetBlackList();
                    if (srcWidgetIndexInBelongedList < blackList.Count)
                    {
                        CFriendModel.stBlackName name = blackList[srcWidgetIndexInBelongedList];
                        if (component != null)
                        {
                            UT.ShowBlackListData(ref name, component);
                        }
                    }
                }
                else if (this.CurTab == Tab.Friend_LBS)
                {
                    ListView<CSDT_LBS_USER_INFO> currentLBSList = Singleton<CFriendContoller>.instance.model.GetCurrentLBSList();
                    if (srcWidgetIndexInBelongedList < currentLBSList.Count)
                    {
                        CSDT_LBS_USER_INFO csdt_lbs_user_info = currentLBSList[srcWidgetIndexInBelongedList];
                        if (component != null)
                        {
                            UT.ShowLBSUserData(csdt_lbs_user_info, component);
                        }
                    }
                }
            }
        }

        private void On_SearchFriend(CUIEvent uiEvent)
        {
            this.addFriendView.On_SearchFriend(uiEvent);
        }

        public void On_Tab_Change(int index)
        {
            if (CSysDynamicBlock.bFriendBlocked)
            {
                if (index == 0)
                {
                    index = 1;
                }
                else if (index == 1)
                {
                    index = 2;
                }
            }
            this.CurTab = (Tab) index;
            if ((this.CurTab == Tab.Friend_LBS) && (this.tablistScript != null))
            {
                Singleton<CMiShuSystem>.GetInstance().HideNewFlag(this.tablistScript.GetElemenet((int) this.CurTab).gameObject, enNewFlagKey.New_PeopleNearby_V1);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if ((masterRoleInfo != null) && !masterRoleInfo.IsClientBitsSet(4))
                {
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Friend_LBSFristTimeOpen_Tip"), enUIEventID.Friend_LBS_NoShare, enUIEventID.None, false);
                    masterRoleInfo.SetClientBits(4, true, true);
                }
            }
        }

        public void OpenForm(CUIEvent uiEvent)
        {
            this.friendform = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.FriendFormPath, false, true);
            GameObject gameObject = this.friendform.gameObject;
            this.friendListNode = gameObject.transform.Find("node/Image/FriendList").gameObject;
            this.friendListNode.CustomSetActive(true);
            this.friendListCom = this.friendListNode.GetComponent<CUIListScript>();
            this.addFriendBtnGameObject = Utility.FindChild(gameObject, "node/Buttons/Add");
            this.info_node = gameObject.transform.Find("node/Image/info_node").gameObject;
            this.info_node.CustomSetActive(false);
            this.ifnoText = Utility.GetComponetInChild<Text>(gameObject, "node/Image/info_node/Text");
            this.ifnoText.text = Singleton<CTextManager>.instance.GetText("Friend_NoFriend_Tip");
            string text = Singleton<CTextManager>.instance.GetText("FriendAdd_Tab_QQ");
            if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat)
            {
                text = Singleton<CTextManager>.instance.GetText("FriendAdd_Tab_Weixin");
            }
            this.tablistScript = gameObject.transform.Find("TopCommon/Panel_Menu/List").gameObject.GetComponent<CUIListScript>();
            string[] strArray = new string[] { text, UT.GetText("Friend_Title_List"), UT.GetText("Friend_Title_Requests"), "黑名单", "附近的人" };
            string[] strArray2 = new string[] { UT.GetText("Friend_Title_List"), UT.GetText("Friend_Title_Requests") };
            string[] strArray3 = !CSysDynamicBlock.bSocialBlocked ? strArray : strArray2;
            Tab tab = !CSysDynamicBlock.bSocialBlocked ? Tab.Friend_SNS : Tab.Friend;
            this.tablistScript.SetElementAmount(strArray3.Length);
            for (int i = 0; i < this.tablistScript.m_elementAmount; i++)
            {
                this.tablistScript.GetElemenet(i).gameObject.transform.Find("Text").GetComponent<Text>().text = strArray3[i];
            }
            this.btnText = Utility.GetComponetInChild<Text>(gameObject, "node/Buttons/Invite/Text");
            this.sns_invite_btn = gameObject.transform.Find("node/Buttons/Invite").gameObject;
            string str2 = Singleton<CTextManager>.instance.GetText("FriendAdd_Invite_Btn_QQ");
            if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat)
            {
                str2 = Singleton<CTextManager>.instance.GetText("FriendAdd_Invite_Btn_Weixin");
            }
            this.btnText.text = str2;
            this.sns_invite_btn.CustomSetActive(false);
            this.rule_btn_node = gameObject.transform.Find("btnRule").gameObject;
            this.lbs_node = gameObject.transform.Find("node/LBSNode").gameObject;
            this.m_QQboxBtn = Utility.FindChild(gameObject, "node/Buttons/QQBoxBtn");
            this.sns_share_switch = Utility.FindChild(gameObject, "node/SnsNtfNode/SnsToggle");
            this.sns_add_switch = Utility.FindChild(gameObject, "node/SnsNtfNode/AddToggle");
            this.sns_share_switch.CustomSetActive(false);
            this.localtionToggle = Utility.FindChild(gameObject, "node/LBSNode/location").GetComponent<Toggle>();
            this.nanToggle = Utility.FindChild(gameObject, "node/LBSNode/nan").GetComponent<Toggle>();
            this.nvToggle = Utility.FindChild(gameObject, "node/LBSNode/nv").GetComponent<Toggle>();
            this.lbsRefreshBtn = Utility.FindChild(gameObject, "node/LBSNode/Add").GetComponent<Button>();
            if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
            {
                Utility.GetComponetInChild<Text>(this.sns_share_switch, "Label").text = Singleton<CTextManager>.instance.GetText("Friend_SNS_NTF_Switch_Tips_1");
            }
            else
            {
                Utility.GetComponetInChild<Text>(this.sns_share_switch, "Label").text = Singleton<CTextManager>.instance.GetText("Friend_SNS_NTF_Switch_Tips_2");
            }
            this.sns_add_switch.CustomSetActive(false);
            this.tablistScript.m_alwaysDispatchSelectedChangeEvent = true;
            this.tablistScript.SelectElement(0, true);
            this.tablistScript.m_alwaysDispatchSelectedChangeEvent = false;
            this._tab = Tab.None;
            this.Refresh_Tab();
            this.CurTab = tab;
            CUIListElementScript elemenet = this.tablistScript.GetElemenet(4);
            if (elemenet != null)
            {
                Singleton<CMiShuSystem>.GetInstance().ShowNewFlag(elemenet.gameObject, enNewFlagKey.New_PeopleNearby_V1);
            }
        }

        public void OpenSearchForm()
        {
            this.addFriendView.Init();
        }

        public void Refresh()
        {
            this.Refresh_Tab();
            this.Refresh_SnsSwitch();
            this.Refresh_List(this.CurTab);
            if ((this.addFriendView != null) && this.addFriendView.bShow)
            {
                this.addFriendView.Refresh();
            }
        }

        public void Refresh_List(Tab type)
        {
            if (this.friendListCom != null)
            {
                if (type == Tab.Friend_SNS)
                {
                    ListView<COMDT_FRIEND_INFO> view = Singleton<CFriendContoller>.instance.model.GetList(CFriendModel.FriendType.SNS);
                    this._refresh_list(this.friendListCom, view, FriendShower.ItemType.Normal, true, CFriendModel.FriendType.SNS);
                    this.friendListCom.gameObject.CustomSetActive(true);
                    if ((this.info_node != null) && (view != null))
                    {
                        this.info_node.CustomSetActive(view.Count == 0);
                    }
                }
                else if (type == Tab.Friend_Request)
                {
                    ListView<COMDT_FRIEND_INFO> view2 = Singleton<CFriendContoller>.instance.model.GetList(CFriendModel.FriendType.RequestFriend);
                    this._refresh_list(this.friendListCom, view2, FriendShower.ItemType.Request, false, CFriendModel.FriendType.RequestFriend);
                    this.friendListCom.gameObject.CustomSetActive(true);
                    if ((this.info_node != null) && (view2 != null))
                    {
                        this.info_node.CustomSetActive(view2.Count == 0);
                    }
                }
                else if (type == Tab.Friend)
                {
                    ListView<COMDT_FRIEND_INFO> view3 = Singleton<CFriendContoller>.instance.model.GetList(CFriendModel.FriendType.GameFriend);
                    this._refresh_list(this.friendListCom, view3, FriendShower.ItemType.Normal, false, CFriendModel.FriendType.GameFriend);
                    this.friendListCom.gameObject.CustomSetActive(true);
                    if ((this.info_node != null) && (view3 != null))
                    {
                        this.info_node.CustomSetActive(view3.Count == 0);
                    }
                }
                else if (type == Tab.Friend_BlackList)
                {
                    List<CFriendModel.stBlackName> blackList = Singleton<CFriendContoller>.instance.model.GetBlackList();
                    this._refresh_black_list(this.friendListCom, blackList);
                    this.friendListCom.gameObject.CustomSetActive(true);
                    if ((this.info_node != null) && (blackList != null))
                    {
                        this.info_node.CustomSetActive(blackList.Count == 0);
                    }
                }
                else if (type == Tab.Friend_LBS)
                {
                    ListView<CSDT_LBS_USER_INFO> currentLBSList = Singleton<CFriendContoller>.instance.model.GetCurrentLBSList();
                    this._refresh_LBS_list(this.friendListCom, currentLBSList);
                    this.friendListCom.gameObject.CustomSetActive(Singleton<CFriendContoller>.instance.model.EnableShareLocation);
                    if (this.info_node != null)
                    {
                        if (currentLBSList == null)
                        {
                            this.info_node.CustomSetActive(true);
                        }
                        else
                        {
                            this.info_node.CustomSetActive(currentLBSList.Count == 0);
                        }
                    }
                }
            }
        }

        public void Refresh_SnsSwitch()
        {
            if ((this.sns_share_switch != null) && (this.sns_add_switch != null))
            {
                this.sns_share_switch.CustomSetActive(false);
                this.sns_add_switch.CustomSetActive(false);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                if ((masterRoleInfo != null) && !CSysDynamicBlock.bSocialBlocked)
                {
                    if (this.CurTab == Tab.Friend_SNS)
                    {
                        this.sns_share_switch.CustomSetActive(true);
                        bool flag = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_DONOTE_AND_REC);
                        this.sns_share_switch.GetComponent<Toggle>().isOn = flag;
                    }
                    else if (this.CurTab == Tab.Friend_Request)
                    {
                        this.sns_add_switch.CustomSetActive(true);
                        bool flag2 = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_ADDFRIEND);
                        this.sns_add_switch.GetComponent<Toggle>().isOn = flag2;
                    }
                }
            }
        }

        public void Refresh_Tab()
        {
            int dataCount = Singleton<CFriendContoller>.GetInstance().model.GetDataCount(CFriendModel.FriendType.RequestFriend);
            int index = !CSysDynamicBlock.bSocialBlocked ? 2 : 1;
            if (dataCount > 0)
            {
                CUICommonSystem.AddRedDot(this.tablistScript.GetElemenet(index).gameObject, enRedDotPos.enTopRight, dataCount);
            }
            else
            {
                CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(index).gameObject);
            }
        }

        public void Show_Search_Result(COMDT_FRIEND_INFO info)
        {
            if ((this.addFriendView != null) && this.addFriendView.bShow)
            {
                this.addFriendView.Record_SearchFriend(info);
                this.addFriendView.Show_Search_Result(info);
            }
        }

        public void SyncGenderToggleState()
        {
            if (this.nanToggle != null)
            {
                this.nanToggle.isOn = (Singleton<CFriendContoller>.instance.model.fileter & 1) != 0;
            }
            if (this.nvToggle != null)
            {
                this.nvToggle.isOn = (Singleton<CFriendContoller>.instance.model.fileter & 2) != 0;
            }
        }

        public void SyncLBSShareBtnState()
        {
            bool enableShareLocation = Singleton<CFriendContoller>.instance.model.EnableShareLocation;
            if (this.lbsRefreshBtn != null)
            {
                CUICommonSystem.SetButtonEnable(this.lbsRefreshBtn, enableShareLocation, enableShareLocation, true);
            }
            if (this.localtionToggle != null)
            {
                this.localtionToggle.isOn = enableShareLocation;
            }
        }

        public void Update()
        {
            if (this.verficationView != null)
            {
                this.verficationView.Update();
            }
            if ((this.lbsRefreshBtn != null) && (Singleton<CFriendContoller>.instance.startCooldownTimestamp > 0L))
            {
                uint num = (uint) (Singleton<FrameSynchr>.GetInstance().LogicFrameTick - Singleton<CFriendContoller>.instance.startCooldownTimestamp);
                Transform transform = this.lbsRefreshBtn.transform.Find("Text");
                if (transform != null)
                {
                    Text component = transform.GetComponent<Text>();
                    if (component != null)
                    {
                        int num2 = 0x2710 - ((int) num);
                        if (num2 > 0)
                        {
                            CUICommonSystem.SetButtonEnableWithShader(this.lbsRefreshBtn, false, true);
                            this.lbsRefreshBtn.enabled = false;
                            component.text = string.Format(Singleton<CTextManager>.instance.GetText("LBS_Refresh_CDInfo"), (num2 / 0x3e8).ToString());
                        }
                        else
                        {
                            Singleton<CFriendContoller>.instance.startCooldownTimestamp = 0L;
                            component.text = Singleton<CTextManager>.instance.GetText("LBS_Refresh_CDInfoNormal");
                            CUICommonSystem.SetButtonEnableWithShader(this.lbsRefreshBtn, true, true);
                        }
                    }
                }
            }
        }

        public Tab CurTab
        {
            get
            {
                return this._tab;
            }
            set
            {
                if (this._tab != value)
                {
                    string searchLBSZero;
                    this._tab = value;
                    this.Refresh_List(this.CurTab);
                    this.Refresh_SnsSwitch();
                    if (this.sns_invite_btn != null)
                    {
                        this.sns_invite_btn.CustomSetActive(false);
                    }
                    if (this.lbs_node != null)
                    {
                        this.lbs_node.CustomSetActive(false);
                    }
                    if (this.addFriendBtnGameObject != null)
                    {
                        this.addFriendBtnGameObject.CustomSetActive(true);
                    }
                    if (((this._tab == Tab.Friend) || (this._tab == Tab.Friend_Request)) && (this.addFriendBtnGameObject != null))
                    {
                        this.addFriendBtnGameObject.CustomSetActive(!CSysDynamicBlock.bFriendBlocked);
                    }
                    if (this.m_QQboxBtn != null)
                    {
                        bool bActive = false;
                        long currentUTCTime = CRoleInfo.GetCurrentUTCTime();
                        if (MonoSingleton<BannerImageSys>.GetInstance().QQBOXInfo.isTimeValid(currentUTCTime))
                        {
                            bActive = true;
                        }
                        if ((ApolloConfig.platform == ApolloPlatform.QQ) || (ApolloConfig.platform == ApolloPlatform.WTLogin))
                        {
                            if (this._tab == Tab.Friend_SNS)
                            {
                                this.m_QQboxBtn.CustomSetActive(bActive);
                            }
                            else
                            {
                                this.m_QQboxBtn.CustomSetActive(false);
                            }
                        }
                        else
                        {
                            this.m_QQboxBtn.CustomSetActive(false);
                        }
                        if (CSysDynamicBlock.bLobbyEntryBlocked && (this.m_QQboxBtn != null))
                        {
                            this.m_QQboxBtn.CustomSetActive(false);
                        }
                    }
                    CFriendModel model = Singleton<CFriendContoller>.GetInstance().model;
                    switch (this._tab)
                    {
                        case Tab.Friend_SNS:
                        {
                            if (this.ifnoText != null)
                            {
                                this.ifnoText.text = Singleton<CTextManager>.instance.GetText("Friend_NoFriend_Tip");
                            }
                            int dataCount = model.GetDataCount(CFriendModel.FriendType.SNS);
                            if (this.info_node != null)
                            {
                                this.info_node.CustomSetActive(dataCount == 0);
                            }
                            if (this.sns_invite_btn != null)
                            {
                                this.sns_invite_btn.CustomSetActive(!CSysDynamicBlock.bSocialBlocked);
                            }
                            if (this.rule_btn_node != null)
                            {
                                this.rule_btn_node.CustomSetActive(false);
                            }
                            return;
                        }
                        case Tab.Friend:
                        {
                            if (this.ifnoText != null)
                            {
                                this.ifnoText.text = Singleton<CTextManager>.instance.GetText("Friend_NoFriend_Tip");
                            }
                            int num2 = model.GetDataCount(CFriendModel.FriendType.GameFriend);
                            if (this.info_node != null)
                            {
                                this.info_node.CustomSetActive(num2 == 0);
                            }
                            if (this.rule_btn_node != null)
                            {
                                this.rule_btn_node.CustomSetActive(true);
                            }
                            return;
                        }
                        case Tab.Friend_Request:
                        {
                            if (this.ifnoText != null)
                            {
                                this.ifnoText.text = Singleton<CTextManager>.instance.GetText("Friend_NoRequest_Tip");
                            }
                            int num4 = model.GetDataCount(CFriendModel.FriendType.RequestFriend);
                            if (this.info_node != null)
                            {
                                this.info_node.CustomSetActive(num4 == 0);
                            }
                            if (this.rule_btn_node != null)
                            {
                                this.rule_btn_node.CustomSetActive(false);
                            }
                            return;
                        }
                        case Tab.Friend_BlackList:
                        {
                            if (this.ifnoText != null)
                            {
                                this.ifnoText.text = Singleton<CTextManager>.instance.GetText("Friend_NoBlackList_Tip");
                            }
                            int count = model.GetBlackList().Count;
                            if (this.info_node != null)
                            {
                                this.info_node.CustomSetActive(count == 0);
                            }
                            if (this.rule_btn_node != null)
                            {
                                this.rule_btn_node.CustomSetActive(false);
                            }
                            return;
                        }
                        case Tab.Friend_LBS:
                            searchLBSZero = string.Empty;
                            if (string.IsNullOrEmpty(model.searchLBSZero))
                            {
                                searchLBSZero = Singleton<CTextManager>.instance.GetText("Friend_NoLBSList_Tip");
                                break;
                            }
                            searchLBSZero = model.searchLBSZero;
                            break;

                        default:
                            if (this.info_node != null)
                            {
                                this.info_node.CustomSetActive(false);
                            }
                            if (this.rule_btn_node != null)
                            {
                                this.rule_btn_node.CustomSetActive(false);
                            }
                            return;
                    }
                    if (this.ifnoText != null)
                    {
                        this.ifnoText.text = searchLBSZero;
                    }
                    if (this.rule_btn_node != null)
                    {
                        this.rule_btn_node.CustomSetActive(false);
                    }
                    if (this.sns_invite_btn != null)
                    {
                        this.sns_invite_btn.CustomSetActive(false);
                    }
                    if (this.lbs_node != null)
                    {
                        this.lbs_node.CustomSetActive(true);
                    }
                    if (this.addFriendBtnGameObject != null)
                    {
                        this.addFriendBtnGameObject.CustomSetActive(false);
                    }
                    if (this.m_QQboxBtn != null)
                    {
                        this.m_QQboxBtn.CustomSetActive(false);
                    }
                    this.SyncGenderToggleState();
                    this.SyncLBSShareBtnState();
                    int num6 = 0;
                    ListView<CSDT_LBS_USER_INFO> currentLBSList = Singleton<CFriendContoller>.instance.model.GetCurrentLBSList();
                    if (currentLBSList != null)
                    {
                        num6 = currentLBSList.Count;
                    }
                    if (this.info_node != null)
                    {
                        this.info_node.CustomSetActive(num6 == 0);
                    }
                }
            }
        }

        public class AddFriendView
        {
            public bool bShow;
            public GameObject buttons_node;
            private GameObject info_text;
            private Text input;
            public CUIListScript recommandFriendListCom;
            private static readonly Vector2 recommandFriendListPos1 = new Vector2(40f, 190f);
            private static readonly Vector2 recommandFriendListPos2 = new Vector2(40f, 340f);
            private static readonly Vector2 recommandFriendListSize1 = new Vector2(-80f, 180f);
            private static readonly Vector2 recommandFriendListSize2 = new Vector2(-80f, 320f);
            public COMDT_FRIEND_INFO search_info_Game;
            private FriendShower searchFriendShower;

            public void Clear()
            {
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_SerchFriend, new CUIEventManager.OnUIEventHandler(this.On_SearchFriend));
                Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_Close_AddForm, new CUIEventManager.OnUIEventHandler(this.On_Friend_Close_AddForm));
                this.input = null;
                this.searchFriendShower = null;
                this.recommandFriendListCom = null;
                this.search_info_Game = null;
                this.buttons_node = null;
                this.bShow = false;
                Singleton<CFriendContoller>.GetInstance().search_info = null;
            }

            public void Clear_SearchFriend()
            {
                this.search_info_Game = null;
            }

            public void Init()
            {
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SerchFriend, new CUIEventManager.OnUIEventHandler(this.On_SearchFriend));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Close_AddForm, new CUIEventManager.OnUIEventHandler(this.On_Friend_Close_AddForm));
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.AddFriendFormPath, false, true);
                this.input = script.transform.FindChild("GameObject/SearchFriend/InputField/Text").GetComponent<Text>();
                this.searchFriendShower = script.transform.FindChild("GameObject/SearchFriend/Result/Friend").GetComponent<FriendShower>();
                this.searchFriendShower.gameObject.CustomSetActive(false);
                this.recommandFriendListCom = Utility.GetComponetInChild<CUIListScript>(script.gameObject, "GameObject/RecommandList");
                this.buttons_node = script.transform.FindChild("GameObject/Buttons").gameObject;
                this.info_text = script.transform.Find("GameObject/SearchFriend/Result/info").gameObject;
                if (this.info_text != null)
                {
                    this.info_text.CustomSetActive(false);
                }
                FriendSysNetCore.Send_Request_RecommandFriend_List();
                this.Refresh();
                this.bShow = true;
            }

            public void On_Friend_Close_AddForm(CUIEvent uiEvent)
            {
                this.Clear();
            }

            public void On_SearchFriend(CUIEvent uiEvent)
            {
                this.Clear_SearchFriend();
                this.searchFriendShower.gameObject.CustomSetActive(false);
                if (string.IsNullOrEmpty(this.input.text))
                {
                    Singleton<CUIManager>.GetInstance().OpenMessageBox(UT.GetText("Friend_Input_Tips"), false);
                }
                else
                {
                    FriendSysNetCore.Send_Serch_Player(this.input.text);
                }
                this.Refresh_Friend_Recommand_List_Pos();
            }

            public void Record_SearchFriend(COMDT_FRIEND_INFO info)
            {
                this.search_info_Game = info;
            }

            public void Refresh()
            {
                this.buttons_node.CustomSetActive(false);
                this.Show_Search_Game();
                this.Show_Search_Result(null);
            }

            public void Refresh_Friend_Recommand_List()
            {
                if (this.recommandFriendListCom != null)
                {
                    Singleton<CFriendContoller>.GetInstance().model.FilterRecommendFriends();
                    int dataCount = Singleton<CFriendContoller>.GetInstance().model.GetDataCount(CFriendModel.FriendType.Recommend);
                    this.recommandFriendListCom.SetElementAmount(dataCount);
                    COMDT_FRIEND_INFO info = null;
                    for (int i = 0; i < dataCount; i++)
                    {
                        info = Singleton<CFriendContoller>.GetInstance().model.GetInfoAtIndex(CFriendModel.FriendType.Recommend, i);
                        if (info != null)
                        {
                            this.Refresh_Recomand_Friend(i, info);
                        }
                    }
                }
            }

            public void Refresh_Friend_Recommand_List_Pos()
            {
                if ((this.recommandFriendListCom != null) && (this.searchFriendShower != null))
                {
                    RectTransform transform = this.recommandFriendListCom.transform as RectTransform;
                    if (this.searchFriendShower.gameObject.activeSelf)
                    {
                        transform.anchoredPosition = recommandFriendListPos1;
                        transform.sizeDelta = recommandFriendListSize1;
                    }
                    else
                    {
                        transform.anchoredPosition = recommandFriendListPos2;
                        transform.sizeDelta = recommandFriendListSize2;
                    }
                }
            }

            public void Refresh_Recomand_Friend(int index, COMDT_FRIEND_INFO info)
            {
                CUIListElementScript elemenet = this.recommandFriendListCom.GetElemenet(index);
                if (elemenet != null)
                {
                    FriendShower component = elemenet.GetComponent<FriendShower>();
                    if (component != null)
                    {
                        UT.ShowFriendData(info, component, FriendShower.ItemType.Add, false, CFriendModel.FriendType.Recommend);
                    }
                }
            }

            private void Show_Search_Game()
            {
                this.Refresh_Friend_Recommand_List();
                this.Refresh_Friend_Recommand_List_Pos();
            }

            public void Show_Search_Result(COMDT_FRIEND_INFO info)
            {
                COMDT_FRIEND_INFO comdt_friend_info = null;
                comdt_friend_info = this.search_info_Game;
                if (comdt_friend_info == null)
                {
                    if (this.searchFriendShower != null)
                    {
                        this.searchFriendShower.gameObject.CustomSetActive(false);
                    }
                }
                else if (this.searchFriendShower != null)
                {
                    this.searchFriendShower.gameObject.CustomSetActive(true);
                    UT.ShowFriendData(comdt_friend_info, this.searchFriendShower, FriendShower.ItemType.Add, false, CFriendModel.FriendType.RequestFriend);
                }
            }
        }

        public enum Tab
        {
            Friend_SNS,
            Friend,
            Friend_Request,
            Friend_BlackList,
            Friend_LBS,
            None
        }

        public class Verfication
        {
            private InputField _inputName;
            private uint dwLogicWorldId;
            private bool m_bAddSearchFirend;
            private ulong ullUid;
            private static int Verfication_ChatMaxLength = 15;

            public Verfication()
            {
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Verify_Close, new CUIEventManager.OnUIEventHandler(this.On_Friend_Verify_Close));
                Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Verify_Send, new CUIEventManager.OnUIEventHandler(this.On_Friend_Verify_Send));
            }

            private void On_Friend_Verify_Close(CUIEvent uievent)
            {
                this.ullUid = 0L;
                this.dwLogicWorldId = 0;
                this.m_bAddSearchFirend = false;
                this._inputName = null;
            }

            private void On_Friend_Verify_Send(CUIEvent uievent)
            {
                if (uievent == null)
                {
                    Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
                }
                else
                {
                    CUIFormScript srcFormScript = uievent.m_srcFormScript;
                    if (srcFormScript == null)
                    {
                        Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
                    }
                    else
                    {
                        InputField component = srcFormScript.GetWidget(0).GetComponent<InputField>();
                        if (component == null)
                        {
                            Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
                        }
                        else
                        {
                            string veriyText = CUIUtility.RemoveEmoji(component.text).Trim();
                            if (this.ullUid == 0)
                            {
                                Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
                            }
                            else
                            {
                                if (this.m_bAddSearchFirend)
                                {
                                    FriendSysNetCore.Send_Request_BeFriend(this.ullUid, this.dwLogicWorldId, veriyText);
                                }
                                else
                                {
                                    FriendSysNetCore.Send_Request_BeFriend(this.ullUid, this.dwLogicWorldId, veriyText);
                                    Singleton<CFriendContoller>.instance.model.Remove(CFriendModel.FriendType.Recommend, this.ullUid, this.dwLogicWorldId);
                                }
                                this.ullUid = 0L;
                                this.dwLogicWorldId = 0;
                                this.m_bAddSearchFirend = false;
                                this._inputName = null;
                                Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
                            }
                        }
                    }
                }
            }

            public void Open(ulong ullUid, uint dwLogicWorldId, bool bAddSearchFirend)
            {
                CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.VerifyFriendFormPath, true, true);
                if (script != null)
                {
                    GameObject gameObject = script.GetWidget(0).gameObject;
                    if (gameObject != null)
                    {
                        this._inputName = gameObject.GetComponent<InputField>();
                        if (this._inputName != null)
                        {
                            if (this._inputName.placeholder != null)
                            {
                                this._inputName.placeholder.gameObject.CustomSetActive(false);
                            }
                            string randomVerifyContent = Singleton<CFriendContoller>.instance.model.GetRandomVerifyContent();
                            if (!string.IsNullOrEmpty(randomVerifyContent))
                            {
                                this._inputName.text = randomVerifyContent;
                            }
                            this.ullUid = ullUid;
                            this.dwLogicWorldId = dwLogicWorldId;
                            this.m_bAddSearchFirend = bAddSearchFirend;
                        }
                    }
                }
            }

            public void Update()
            {
                if (this._inputName != null)
                {
                    string text = this._inputName.text;
                    if ((text != null) && (text.Length > Verfication_ChatMaxLength))
                    {
                        this._inputName.DeactivateInputField();
                        this._inputName.text = text.Substring(0, Verfication_ChatMaxLength);
                        Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.instance.GetText("chat_input_max"), Verfication_ChatMaxLength), false);
                    }
                }
            }

            public enum eVerficationFormWidget
            {
                DescText = 1,
                NameInputField = 0,
                None = -1
            }
        }
    }
}

