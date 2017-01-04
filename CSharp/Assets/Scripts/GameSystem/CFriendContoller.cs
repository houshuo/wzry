namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class CFriendContoller : Singleton<CFriendContoller>
    {
        public static string AddFriendFormPath = "UGUI/Form/System/Friend/AddFriend.prefab";
        private GameObject com;
        public static string FriendFormPath = "UGUI/Form/System/Friend/FriendForm.prefab";
        public string IntimacyUpInfo = string.Empty;
        public int IntimacyUpValue;
        public CFriendModel model = new CFriendModel();
        public COMDT_FRIEND_INFO search_info;
        public static string VerifyFriendFormPath = "UGUI/Form/System/Friend/Form_FriendVerification.prefab";
        public CFriendView view = new CFriendView();

        private void Add_And_Refresh(CFriendModel.FriendType type, COMDT_FRIEND_INFO data)
        {
            this.model.Add(type, data, false);
            if (type == CFriendModel.FriendType.GameFriend)
            {
                Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
            }
            if (this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        public void ClearAll()
        {
            this.model.ClearAll();
            this.IntimacyUpInfo = string.Empty;
            this.IntimacyUpValue = 0;
            this.search_info = null;
            this.com = null;
            this.SetFilter(3);
        }

        public bool FilterSameFriend(COMDT_FRIEND_INFO info, ListView<COMDT_FRIEND_INFO> friendList)
        {
            if (friendList != null)
            {
                for (int i = 0; i < friendList.Count; i++)
                {
                    if (friendList[i].stUin.ullUid == info.stUin.ullUid)
                    {
                        if (friendList[i].dwLastLoginTime < info.dwLastLoginTime)
                        {
                            friendList[i] = info;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private ulong GetFriendUid(CUIEvent uiEvent)
        {
            FriendShower component = uiEvent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
            if (component == null)
            {
                return 0L;
            }
            CFriendModel.FriendType type = (this.view.GetSelectedTab() != CFriendView.Tab.Friend_SNS) ? CFriendModel.FriendType.GameFriend : CFriendModel.FriendType.SNS;
            COMDT_FRIEND_INFO comdt_friend_info = this.model.GetInfo(type, component.ullUid, component.dwLogicWorldID);
            if (comdt_friend_info == null)
            {
                return 0L;
            }
            return comdt_friend_info.stUin.ullUid;
        }

        private void Handle_CoinSend_Data(CSDT_FRIEND_INFO info)
        {
            this.Update_Send_Coin_Data(info.stFriendInfo.stUin, info.ullDonateAPSec, COM_FRIEND_TYPE.COM_FRIEND_TYPE_GAME);
        }

        private void Handle_CoinSend_Data(CSDT_SNS_FRIEND_INFO info)
        {
            this.Update_Send_Coin_Data(info.stSnsFrindInfo.stUin, (ulong) info.dwDonateTime, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
        }

        private void Handle_Invite_Data(COMDT_ACNT_UNIQ uin)
        {
            this.model.SnsReCallData.Add(uin, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
        }

        public override void Init()
        {
            base.Init();
            this.InitEvent();
        }

        private void InitEvent()
        {
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_List", new Action<CSPkg>(this.On_FriendSys_Friend_List));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Request_List", new Action<CSPkg>(this.On_FriendSys_Friend_Request_List));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Recommand_List", new Action<CSPkg>(this.On_FriendSys_Friend_Recommand_List));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Search", new Action<CSPkg>(this.On_FriendSys_Friend_Search));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_RequestBeFriend", new Action<CSPkg>(this.On_FriendSys_Friend_RequestBeFriend));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Confrim", new Action<CSPkg>(this.On_FriendSys_Friend_Confrim));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Deny", new Action<CSPkg>(this.On_FriendSys_Friend_Deny));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Delete", new Action<CSPkg>(this.On_FriendSys_Friend_Delete));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_ADD_NTF", new Action<CSPkg>(this.On_FriendSys_Friend_ADD_NTF));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Delete_NTF", new Action<CSPkg>(this.On_FriendSys_Friend_Delete_NTF));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Request_NTF", new Action<CSPkg>(this.On_FriendSys_Friend_Request_NTF));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Login_NTF", new Action<CSPkg>(this.On_Friend_Login_NTF));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_GAME_STATE_NTF", new Action<CSPkg>(this.On_Friend_GAME_STATE_NTF));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_SNS_STATE_NTF", new Action<CSPkg>(this.On_Friend_SNS_STATE_NTF));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_SNS_CHG_PROFILE", new Action<CSPkg>(this.On_Friend_SNS_CHG_PROFILE));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_SNS_NICNAME_NTF", new Action<CSPkg>(this.On_Friend_SNS_NICKNAME_NTF));
            Singleton<EventRouter>.GetInstance().AddEventHandler<float, float>("Friend_LBS_Location_Calced", new Action<float, float>(this.On_Friend_LBS_Location_Calced));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_RecommandFriend_Refresh", new System.Action(this.On_Friend_RecommandFriend_Refresh));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_FriendList_Refresh", new System.Action(this.On_Friend_FriendList_Refresh));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_SNSFriendList_Refresh", new System.Action(this.On_Friend_SNSFriendList_Refresh));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_LBS_User_Refresh", new System.Action(this.On_Friend_LBS_User_Refresh));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Invite_Success", new System.Action(this.On_GuildSys_Guild_Invite_Success));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Recommend_Success", new System.Action(this.On_GuildSys_Guild_Recommend_Success));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NEWDAY_NTF, new System.Action(this.OnNewDayNtf));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_OpenForm, new CUIEventManager.OnUIEventHandler(this.On_OpenForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_CloseForm, new CUIEventManager.OnUIEventHandler(this.On_CloseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Tab_Change, new CUIEventManager.OnUIEventHandler(this.On_TabChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_OpenAddFriendForm, new CUIEventManager.OnUIEventHandler(this.On_OpenAddFriendForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_RequestBeFriend, new CUIEventManager.OnUIEventHandler(this.On_AddFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Accept_RequestFriend, new CUIEventManager.OnUIEventHandler(this.On_Accept_RequestFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Refuse_RequestFriend, new CUIEventManager.OnUIEventHandler(this.On_Refuse_RequestFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_DelFriend, new CUIEventManager.OnUIEventHandler(this.On_DelFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Tab_Friend, new CUIEventManager.OnUIEventHandler(this.On_Friend_Tab_Friend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Tab_FriendRequest, new CUIEventManager.OnUIEventHandler(this.On_Friend_Tab_FriendRequest));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_DelFriend_OK, new CUIEventManager.OnUIEventHandler(this.On_DelFriend_OK));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_DelFriend_Cancle, new CUIEventManager.OnUIEventHandler(this.On_Friend_DelFriend_Cancle));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SendCoin, new CUIEventManager.OnUIEventHandler(this.On_Friend_SendCoin));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SNS_SendCoin, new CUIEventManager.OnUIEventHandler(this.On_SNSFriend_SendCoin));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_InviteGuild, new CUIEventManager.OnUIEventHandler(this.On_Friend_InviteGuild));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_RecommendGuild, new CUIEventManager.OnUIEventHandler(this.On_Friend_RecommendGuild));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_CheckInfo, new CUIEventManager.OnUIEventHandler(this.On_Friend_CheckInfo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_List_ElementEnable, new CUIEventManager.OnUIEventHandler(this.On_Friend_List_ElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Invite_SNS_Friend, new CUIEventManager.OnUIEventHandler(this.On_Friend_Invite_SNS_Friend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Share_SendCoin, new CUIEventManager.OnUIEventHandler(this.On_Friend_Share_SendCoin));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SNS_ReCall, new CUIEventManager.OnUIEventHandler(this.On_Friend_SNS_ReCall));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_OB_Click, new CUIEventManager.OnUIEventHandler(this.On_Friend_OB_Click));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.QQBOX_OnClick, new CUIEventManager.OnUIEventHandler(this.QQBox_OnClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SNS_Share_Switch_Click, new CUIEventManager.OnUIEventHandler(this.OnShareToggle));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SNS_Add_Switch_Click, new CUIEventManager.OnUIEventHandler(this.OnAddToggle));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Chat_Button, new CUIEventManager.OnUIEventHandler(this.OnFriend_Chat_Button));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Show_Rule, new CUIEventManager.OnUIEventHandler(this.OnFriend_Show_Rule));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Gift, new CUIEventManager.OnUIEventHandler(this.OnFriend_Gift_Button));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_CancleBlock, new CUIEventManager.OnUIEventHandler(this.OnCancleBlockBtn));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_CancleBlock_Ok, new CUIEventManager.OnUIEventHandler(this.OnCancleBlockBtnOK));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_LBS_NoShare, new CUIEventManager.OnUIEventHandler(this.OnFriend_LBS_NoShare));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_LBS_Nan, new CUIEventManager.OnUIEventHandler(this.OnFriend_LBS_Nan));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_LBS_Nv, new CUIEventManager.OnUIEventHandler(this.OnFriend_LBS_Nv));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_LBS_Refresh, new CUIEventManager.OnUIEventHandler(this.OnFriend_LBS_Refresh));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_LBS_CheckInfo, new CUIEventManager.OnUIEventHandler(this.OnFriend_LBS_CheckInfo));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Room_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnFriend_Room_AddFriend));
        }

        private void On_Accept_RequestFriend(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
            if (component != null)
            {
                COMDT_FRIEND_INFO comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.RequestFriend, component.ullUid, component.dwLogicWorldID);
                if (comdt_friend_info != null)
                {
                    FriendSysNetCore.Send_Confrim_BeFriend(comdt_friend_info.stUin);
                }
            }
        }

        private void On_AddFriend(CUIEvent evt)
        {
            COMDT_FRIEND_INFO stLbsUserInfo = Singleton<CFriendContoller>.GetInstance().search_info;
            if ((evt.m_srcWidgetBelongedListScript == null) && (stLbsUserInfo != null))
            {
                this.Open_Friend_Verify(stLbsUserInfo.stUin.ullUid, stLbsUserInfo.stUin.dwLogicWorldId, true);
            }
            else if (((evt.m_srcWidget != null) && (evt.m_srcWidget.transform.parent != null)) && ((evt.m_srcWidget.transform.parent.parent != null) && (evt.m_srcWidget.transform.parent.parent.parent != null)))
            {
                FriendShower component = evt.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
                if (component != null)
                {
                    stLbsUserInfo = this.model.GetInfo(CFriendModel.FriendType.Recommend, component.ullUid, component.dwLogicWorldID);
                    if (stLbsUserInfo == null)
                    {
                        stLbsUserInfo = this.model.GetLBSUserInfo(component.ullUid, component.dwLogicWorldID, CFriendModel.LBSGenderType.Both).stLbsUserInfo;
                    }
                    if (stLbsUserInfo != null)
                    {
                        this.Open_Friend_Verify(stLbsUserInfo.stUin.ullUid, stLbsUserInfo.stUin.dwLogicWorldId, false);
                    }
                }
            }
        }

        private void On_CloseForm(CUIEvent uiEvent)
        {
            this.view.CloseForm();
        }

        private void On_DelFriend(CUIEvent evt)
        {
            this.com = evt.m_srcWidget;
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(UT.GetText("Friend_Tips_DelFriend"), enUIEventID.Friend_DelFriend_OK, enUIEventID.Friend_DelFriend_Cancle, false);
        }

        private void On_DelFriend_OK(CUIEvent evt)
        {
            FriendShower component = this.com.transform.parent.parent.parent.GetComponent<FriendShower>();
            if (component != null)
            {
                COMDT_FRIEND_INFO comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.GameFriend, component.ullUid, component.dwLogicWorldID);
                if (comdt_friend_info != null)
                {
                    FriendSysNetCore.Send_Del_Friend(comdt_friend_info.stUin);
                }
            }
        }

        private void On_Friend_CheckInfo(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
            if (component != null)
            {
                Singleton<CPlayerInfoSystem>.instance.ShowPlayerDetailInfo(component.ullUid, (int) component.dwLogicWorldID, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers);
            }
        }

        private void On_Friend_DelFriend_Cancle(CUIEvent evt)
        {
        }

        private void On_Friend_FriendList_Refresh()
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        private void On_Friend_GAME_STATE_NTF(CSPkg msg)
        {
            SCPKG_CMD_NTF_FRIEND_GAME_STATE stNtfFriendGameState = msg.stPkgData.stNtfFriendGameState;
            this.model.SetFriendGameState(stNtfFriendGameState.stAcntUniq.ullUid, stNtfFriendGameState.stAcntUniq.dwLogicWorldId, (COM_ACNT_GAME_STATE) stNtfFriendGameState.bGameState, stNtfFriendGameState.dwGameStartTime, string.Empty, false);
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.Friend_Game_State_Change);
        }

        private void On_Friend_Invite_SNS_Friend(CUIEvent uievent)
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.On_Friend_Invite_SNS_Friend(uievent);
            }
        }

        private void On_Friend_InviteGuild(CUIEvent uiEvent)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong>("Guild_Invite", this.GetFriendUid(uiEvent));
        }

        private void On_Friend_LBS_Location_Calced(float n, float e)
        {
        }

        private void On_Friend_LBS_User_Refresh()
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        private void On_Friend_List_ElementEnable(CUIEvent uievent)
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.On_List_ElementEnable(uievent);
            }
        }

        private void On_Friend_Login_NTF(CSPkg msg)
        {
            SCPKG_CMD_NTF_FRIEND_LOGIN_STATUS stNtfFriendLoginStatus = msg.stPkgData.stNtfFriendLoginStatus;
            CFriendModel.FriendType type = (stNtfFriendLoginStatus.bFriendType != 1) ? CFriendModel.FriendType.SNS : CFriendModel.FriendType.GameFriend;
            COMDT_FRIEND_INFO info = Singleton<CFriendContoller>.GetInstance().model.GetInfo(type, stNtfFriendLoginStatus.stUin);
            if (info != null)
            {
                info.bIsOnline = stNtfFriendLoginStatus.bLoginStatus;
                info.dwLastLoginTime = (uint) CRoleInfo.GetCurrentUTCTime();
                Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
                if ((this.view != null) && this.view.IsActive())
                {
                    this.view.Refresh();
                }
            }
        }

        private void On_Friend_OB_Click(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
            if (component != null)
            {
                COMDT_FRIEND_INFO comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.SNS, component.ullUid, component.dwLogicWorldID);
                if (comdt_friend_info == null)
                {
                    comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.GameFriend, component.ullUid, component.dwLogicWorldID);
                }
                if (comdt_friend_info != null)
                {
                    Singleton<COBSystem>.instance.OBFriend(comdt_friend_info.stUin);
                }
            }
        }

        private void On_Friend_RecommandFriend_Refresh()
        {
        }

        private void On_Friend_RecommendGuild(CUIEvent uiEvent)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<ulong>("Guild_Recommend", this.GetFriendUid(uiEvent));
        }

        private void On_Friend_SendCoin(CUIEvent uievent)
        {
            ulong ullUid = uievent.m_eventParams.commonUInt64Param1;
            uint dwLogicWorldID = (uint) uievent.m_eventParams.commonUInt64Param2;
            COMDT_FRIEND_INFO comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.GameFriend, ullUid, dwLogicWorldID);
            if (comdt_friend_info != null)
            {
                FriendSysNetCore.Send_FriendCoin(comdt_friend_info.stUin, COM_FRIEND_TYPE.COM_FRIEND_TYPE_GAME);
            }
        }

        private void On_Friend_Share_SendCoin(CUIEvent uievent)
        {
            try
            {
                if (MonoSingleton<ShareSys>.instance.IsInstallPlatform())
                {
                    string openId = uievent.m_eventParams.snsFriendEventParams.openId;
                    Singleton<ApolloHelper>.instance.ShareSendHeart(openId, Singleton<CTextManager>.instance.GetText("Common_Sns_Tips_1"), Singleton<CTextManager>.instance.GetText("Common_Sns_Tips_2"), ShareSys.SNS_SHARE_SEND_HEART);
                    Singleton<CUIManager>.instance.OpenTips("Common_Sns_Tips_7", true, 1.5f, null, new object[0]);
                }
            }
            catch (Exception exception)
            {
                object[] inParameters = new object[] { exception.Message };
                DebugHelper.Assert(false, "Exception in On_Friend_Share_SendCoin, {0}", inParameters);
            }
        }

        private void On_Friend_SNS_CHG_PROFILE(CSPkg msg)
        {
            SCPKG_CHG_SNS_FRIEND_PROFILE stSnsFriendChgProfile = msg.stPkgData.stSnsFriendChgProfile;
            this.model.Add(CFriendModel.FriendType.SNS, stSnsFriendChgProfile.stSnsFrindInfo, true);
            this.model.SetFriendGameState(stSnsFriendChgProfile.stSnsFrindInfo.stUin.ullUid, stSnsFriendChgProfile.stSnsFrindInfo.stUin.dwLogicWorldId, (COM_ACNT_GAME_STATE) stSnsFriendChgProfile.bGameState, 0, string.Empty, true);
            this.model.SortSNSFriend();
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.Friend_Game_State_Change);
        }

        private void On_Friend_SNS_NICKNAME_NTF(CSPkg msg)
        {
            SCPKG_NTF_SNS_NICKNAME stNtfSnsNickName = msg.stPkgData.stNtfSnsNickName;
            uint num = Math.Min(stNtfSnsNickName.dwSnsFriendNum, 100);
            for (int i = 0; i < num; i++)
            {
                if (!CFriendModel.RemarkNames.ContainsKey(stNtfSnsNickName.astSnsNameList[i].ullUid))
                {
                    CFriendModel.RemarkNames.Add(stNtfSnsNickName.astSnsNameList[i].ullUid, CUIUtility.RemoveEmoji(UT.Bytes2String(stNtfSnsNickName.astSnsNameList[i].szNickName)));
                }
            }
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        private void On_Friend_SNS_ReCall(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
            if (component != null)
            {
                COMDT_FRIEND_INFO comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.SNS, component.ullUid, component.dwLogicWorldID);
                if (comdt_friend_info != null)
                {
                    FriendSysNetCore.ReCallSnsFriend(comdt_friend_info.stUin, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
                    if (!CFriendModel.IsOnSnsSwitch(comdt_friend_info.dwRefuseFriendBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_DONOTE_AND_REC))
                    {
                        this.ShareTo_SNSFriend_ReCall(Utility.UTF8Convert(comdt_friend_info.szOpenId));
                    }
                }
            }
        }

        private void On_Friend_SNS_STATE_NTF(CSPkg msg)
        {
            SCPKG_NTF_SNS_FRIEND stNtfSnsFriend = msg.stPkgData.stNtfSnsFriend;
            uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
            uint num2 = 0x15180 * GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x9e).dwConfValue;
            ListView<COMDT_FRIEND_INFO> list = this.model.GetList(CFriendModel.FriendType.SNS);
            for (int i = 0; i < stNtfSnsFriend.dwSnsFriendNum; i++)
            {
                CSDT_SNS_FRIEND_INFO info = stNtfSnsFriend.astSnsFriendList[i];
                if ((info != null) && !this.FilterSameFriend(info.stSnsFrindInfo, list))
                {
                    this.model.Add(CFriendModel.FriendType.SNS, info.stSnsFrindInfo, false);
                    this.model.SetFriendGameState(info.stSnsFrindInfo.stUin.ullUid, info.stSnsFrindInfo.stUin.dwLogicWorldId, (COM_ACNT_GAME_STATE) info.bGameState, info.dwGameStartTime, UT.Bytes2String(info.szNickName), false);
                    this.Handle_CoinSend_Data(info);
                }
            }
            this.model.SortSNSFriend();
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.Friend_Game_State_Change);
        }

        private void On_Friend_SNSFriendList_Refresh()
        {
            if (((this.view != null) && (this.view.addFriendView != null)) && this.view.addFriendView.bShow)
            {
                this.view.addFriendView.Refresh();
            }
        }

        private void On_Friend_Tab_Friend(CUIEvent uievent)
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.On_Tab_Change(0);
            }
        }

        private void On_Friend_Tab_FriendRequest(CUIEvent uievent)
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.On_Tab_Change(1);
            }
        }

        private void On_FriendSys_Friend_ADD_NTF(CSPkg msg)
        {
            SCPKG_CMD_NTF_FRIEND_ADD stNtfFriendAdd = msg.stPkgData.stNtfFriendAdd;
            if (this.model.IsBlack(stNtfFriendAdd.stUserInfo.stUin.ullUid, stNtfFriendAdd.stUserInfo.stUin.dwLogicWorldId))
            {
                this.model.RemoveFriendBlack(stNtfFriendAdd.stUserInfo.stUin.ullUid, stNtfFriendAdd.stUserInfo.stUin.dwLogicWorldId);
            }
            this.model.AddFriendIntimacy(stNtfFriendAdd.stUserInfo.stUin.ullUid, stNtfFriendAdd.stUserInfo.stUin.dwLogicWorldId, stNtfFriendAdd.stIntimacData.dwLastIntimacyTime, stNtfFriendAdd.stIntimacData.wIntimacyValue);
            this.Add_And_Refresh(CFriendModel.FriendType.GameFriend, stNtfFriendAdd.stUserInfo);
        }

        private void On_FriendSys_Friend_Confrim(CSPkg msg)
        {
            SCPKG_CMD_ADD_FRIEND_CONFIRM stFriendAddConfirmRsp = msg.stPkgData.stFriendAddConfirmRsp;
            COMDT_FRIEND_INFO stUserInfo = stFriendAddConfirmRsp.stUserInfo;
            if (stFriendAddConfirmRsp.dwResult == 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips(string.Format(UT.GetText("Friend_Tips_BeFriend_Ok"), UT.Bytes2String(stUserInfo.szUserName)), false, 1.5f, null, new object[0]);
                Singleton<CFriendContoller>.GetInstance().model.Remove(CFriendModel.FriendType.RequestFriend, stUserInfo.stUin);
                Singleton<CFriendContoller>.GetInstance().model.Add(CFriendModel.FriendType.GameFriend, stUserInfo, false);
                if (this.model.IsBlack(stUserInfo.stUin.ullUid, stUserInfo.stUin.dwLogicWorldId))
                {
                    this.model.RemoveFriendBlack(stUserInfo.stUin.ullUid, stUserInfo.stUin.dwLogicWorldId);
                }
                this.model.AddFriendIntimacy(stUserInfo.stUin.ullUid, stUserInfo.stUin.dwLogicWorldId, stFriendAddConfirmRsp.stIntimacData.dwLastIntimacyTime, stFriendAddConfirmRsp.stIntimacData.wIntimacyValue);
                Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
                CFriendView view = Singleton<CFriendContoller>.GetInstance().view;
                if ((view != null) && view.IsActive())
                {
                    view.Refresh();
                }
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stFriendAddConfirmRsp.dwResult), false, 1.5f, null, new object[0]);
                this.Remove_And_Refresh(CFriendModel.FriendType.RequestFriend, stUserInfo.stUin);
            }
        }

        private void On_FriendSys_Friend_Delete(CSPkg msg)
        {
            SCPKG_CMD_DEL_FRIEND stFriendDelRsp = msg.stPkgData.stFriendDelRsp;
            if (stFriendDelRsp.dwResult == 0)
            {
                this.Remove_And_Refresh(CFriendModel.FriendType.GameFriend, stFriendDelRsp.stUin);
            }
        }

        private void On_FriendSys_Friend_Delete_NTF(CSPkg msg)
        {
            SCPKG_CMD_NTF_FRIEND_DEL stNtfFriendDel = msg.stPkgData.stNtfFriendDel;
            this.Remove_And_Refresh(CFriendModel.FriendType.GameFriend, stNtfFriendDel.stUin);
        }

        private void On_FriendSys_Friend_Deny(CSPkg msg)
        {
            SCPKG_CMD_ADD_FRIEND_DENY stFriendAddDenyRsp = msg.stPkgData.stFriendAddDenyRsp;
            if (stFriendAddDenyRsp.dwResult == 0)
            {
                this.Remove_And_Refresh(CFriendModel.FriendType.RequestFriend, stFriendAddDenyRsp.stUin);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stFriendAddDenyRsp.dwResult), false, 1.5f, null, new object[0]);
                this.Remove_And_Refresh(CFriendModel.FriendType.RequestFriend, stFriendAddDenyRsp.stUin);
            }
        }

        private void On_FriendSys_Friend_List(CSPkg msg)
        {
            SCPKG_CMD_LIST_FRIEND stFriendListRsp = msg.stPkgData.stFriendListRsp;
            if (stFriendListRsp != null)
            {
                int num = Mathf.Min(stFriendListRsp.astFrindList.Length, (int) stFriendListRsp.dwFriendNum);
                for (int i = 0; i < num; i++)
                {
                    CSDT_FRIEND_INFO info = stFriendListRsp.astFrindList[i];
                    this.model.Add(CFriendModel.FriendType.GameFriend, info.stFriendInfo, false);
                    this.Handle_CoinSend_Data(info);
                    this.model.SetFriendGameState(info.stFriendInfo.stUin.ullUid, info.stFriendInfo.stUin.dwLogicWorldId, (COM_ACNT_GAME_STATE) info.bGameState, info.dwGameStartTime, string.Empty, false);
                    this.model.AddFriendIntimacy(info.stFriendInfo.stUin.ullUid, info.stFriendInfo.stUin.dwLogicWorldId, info.stIntimacyData.dwLastIntimacyTime, info.stIntimacyData.wIntimacyValue);
                }
                Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
                if ((this.view != null) && this.view.IsActive())
                {
                    this.view.Refresh();
                }
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Rank_Friend_List");
            }
        }

        private void On_FriendSys_Friend_Recommand_List(CSPkg msg)
        {
            SCPKG_CMD_LIST_FREC stFRecRsp = msg.stPkgData.stFRecRsp;
            this.model.Clear(CFriendModel.FriendType.Recommend);
            if (stFRecRsp.dwResult == 0)
            {
                for (int i = 0; i < stFRecRsp.dwAcntNum; i++)
                {
                    COMDT_FRIEND_INFO data = stFRecRsp.astAcnts[i];
                    if (this.model.getFriendByUid(data.stUin.ullUid, CFriendModel.FriendType.GameFriend) == null)
                    {
                        this.model.Add(CFriendModel.FriendType.Recommend, data, false);
                    }
                }
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stFRecRsp.dwResult), false, 1.5f, null, new object[0]);
            }
            if (this.view != null)
            {
                this.view.addFriendView.Refresh_Friend_Recommand_List();
            }
        }

        private void On_FriendSys_Friend_Request_List(CSPkg msg)
        {
            SCPKG_CMD_LIST_FRIENDREQ stFriendReqListRsp = msg.stPkgData.stFriendReqListRsp;
            if (stFriendReqListRsp != null)
            {
                int num = Mathf.Min(stFriendReqListRsp.astVerificationList.Length, (int) stFriendReqListRsp.dwFriendReqNum);
                for (int i = 0; i < num; i++)
                {
                    CSDT_VERIFICATION_INFO csdt_verification_info = stFriendReqListRsp.astVerificationList[i];
                    this.model.Add(CFriendModel.FriendType.RequestFriend, csdt_verification_info.stFriendInfo, false);
                    this.model.AddFriendVerifyContent(csdt_verification_info.stFriendInfo.stUin.ullUid, csdt_verification_info.stFriendInfo.stUin.dwLogicWorldId, StringHelper.BytesToString(csdt_verification_info.szVerificationInfo));
                }
                if ((this.view != null) && this.view.IsActive())
                {
                    this.view.Refresh();
                }
            }
        }

        private void On_FriendSys_Friend_Request_NTF(CSPkg msg)
        {
            SCPKG_CMD_NTF_FRIEND_REQUEST stNtfFriendRequest = msg.stPkgData.stNtfFriendRequest;
            if (stNtfFriendRequest != null)
            {
                this.model.AddFriendVerifyContent(stNtfFriendRequest.stUserInfo.stUin.ullUid, stNtfFriendRequest.stUserInfo.stUin.dwLogicWorldId, StringHelper.BytesToString(stNtfFriendRequest.szVerificationInfo));
                this.Add_And_Refresh(CFriendModel.FriendType.RequestFriend, stNtfFriendRequest.stUserInfo);
            }
        }

        private void On_FriendSys_Friend_RequestBeFriend(CSPkg msg)
        {
            SCPKG_CMD_ADD_FRIEND stFriendAddRsp = msg.stPkgData.stFriendAddRsp;
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<EventRouter>.instance.BroadCastEvent("Friend_SNSFriendList_Refresh");
            UT.ShowFriendNetResult(stFriendAddRsp.dwResult, UT.FriendResultType.RequestBeFriend);
        }

        private void On_FriendSys_Friend_Search(CSPkg msg)
        {
            SCPKG_CMD_SEARCH_PLAYER stFriendSearchPlayerRsp = msg.stPkgData.stFriendSearchPlayerRsp;
            if (stFriendSearchPlayerRsp.dwResult == 0)
            {
                this.search_info = stFriendSearchPlayerRsp.stUserInfo;
                if (this.view != null)
                {
                    this.view.Show_Search_Result(stFriendSearchPlayerRsp.stUserInfo);
                }
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stFriendSearchPlayerRsp.dwResult), false, 1.5f, null, new object[0]);
            }
            this.view.addFriendView.Refresh_Friend_Recommand_List_Pos();
        }

        private void On_GuildSys_Guild_Invite_Success()
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        private void On_GuildSys_Guild_Recommend_Success()
        {
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        private void On_OpenAddFriendForm(CUIEvent uiEvent)
        {
            this.view.OpenSearchForm();
        }

        private void On_OpenForm(CUIEvent uiEvent)
        {
            this.view.OpenForm(uiEvent);
            CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_FrientBtn);
            Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(5, null);
        }

        private void On_Refuse_RequestFriend(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
            if (component != null)
            {
                COMDT_FRIEND_INFO comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.RequestFriend, component.ullUid, component.dwLogicWorldID);
                if (comdt_friend_info != null)
                {
                    FriendSysNetCore.Send_DENY_BeFriend(comdt_friend_info.stUin);
                }
            }
        }

        public void On_SCID_CMD_BLACKLIST(CSPkg msg)
        {
            SCPKG_CMD_BLACKLIST stBlackListProfile = msg.stPkgData.stBlackListProfile;
            for (int i = 0; i < stBlackListProfile.wBlackListNum; i++)
            {
                COMDT_FRIEND_INFO info = stBlackListProfile.astBlackList[i];
                if (info != null)
                {
                    this.model.AddFriendBlack(info);
                }
            }
        }

        private void On_SNSFriend_SendCoin(CUIEvent uievent)
        {
            ulong ullUid = uievent.m_eventParams.commonUInt64Param1;
            uint dwLogicWorldID = (uint) uievent.m_eventParams.commonUInt64Param2;
            COMDT_FRIEND_INFO comdt_friend_info = this.model.GetInfo(CFriendModel.FriendType.SNS, ullUid, dwLogicWorldID);
            if (comdt_friend_info != null)
            {
                FriendSysNetCore.Send_FriendCoin(comdt_friend_info.stUin, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
            }
        }

        private void On_TabChange(CUIEvent uievent)
        {
            if ((this.view != null) && this.view.IsActive())
            {
                int selectedIndex = uievent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
                this.view.On_Tab_Change(selectedIndex);
            }
        }

        private void OnAddToggle(CUIEvent uievent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                bool flag = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_ADDFRIEND);
                int num = 2;
                masterRoleInfo.snsSwitchBits ^= (uint) num;
                FriendSysNetCore.Send_Request_Sns_Switch(COM_REFUSE_TYPE.COM_REFUSE_TYPE_ADDFRIEND, flag ? 0 : 1);
            }
        }

        private void OnCancleBlockBtn(CUIEvent uiEvent)
        {
            this.com = uiEvent.m_srcWidget.transform.parent.parent.parent.gameObject;
            if (this.com != null)
            {
                FriendShower component = this.com.GetComponent<FriendShower>();
                if (component != null)
                {
                    string blackName = this.model.GetBlackName(component.ullUid, component.dwLogicWorldID);
                    string[] args = new string[] { blackName };
                    string strContent = string.Format(Singleton<CTextManager>.instance.GetText("Black_CancleBlockTip", args), new object[0]);
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.Friend_CancleBlock_Ok, enUIEventID.Friend_CancleBlock_Cancle, false);
                }
            }
        }

        private void OnCancleBlockBtnOK(CUIEvent evt)
        {
            FriendShower component = this.com.GetComponent<FriendShower>();
            if (component != null)
            {
                FriendSysNetCore.Send_Cancle_Block(component.ullUid, component.dwLogicWorldID);
            }
        }

        public void OnChangeIntimacy(CSPkg msg)
        {
            if (msg.stPkgData.stNtfChgIntimacy.dwResult == 0)
            {
                ushort num;
                CFriendModel.EIntimacyType type;
                bool flag;
                SCPKG_CMD_NTF_CHG_INTIMACY stNtfChgIntimacy = msg.stPkgData.stNtfChgIntimacy;
                if (Singleton<CFriendContoller>.instance.model.GetFriendIntimacy(stNtfChgIntimacy.stUin.ullUid, stNtfChgIntimacy.stUin.dwLogicWorldId, out num, out type, out flag))
                {
                    int num2 = stNtfChgIntimacy.stIntimacData.wIntimacyValue - num;
                    if (num2 > 0)
                    {
                        COMDT_FRIEND_INFO gameOrSnsFriend = this.model.GetGameOrSnsFriend(stNtfChgIntimacy.stUin.ullUid, stNtfChgIntimacy.stUin.dwLogicWorldId);
                        if (gameOrSnsFriend != null)
                        {
                            if (!Singleton<BattleLogic>.instance.isRuning)
                            {
                                this.IntimacyUpInfo = string.Format(UT.GetText("Intimacy_UpInfo"), UT.Bytes2String(gameOrSnsFriend.szUserName), num2);
                                Singleton<CUIManager>.GetInstance().OpenTips(this.IntimacyUpInfo, false, 1.5f, null, new object[0]);
                                this.IntimacyUpInfo = string.Empty;
                                this.IntimacyUpValue = 0;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(this.IntimacyUpInfo))
                                {
                                    this.IntimacyUpInfo = string.Format(" {0}", UT.Bytes2String(gameOrSnsFriend.szUserName));
                                }
                                else
                                {
                                    this.IntimacyUpInfo = this.IntimacyUpInfo + string.Format(", {0}", UT.Bytes2String(gameOrSnsFriend.szUserName));
                                }
                                this.IntimacyUpValue = num2;
                            }
                        }
                    }
                }
                this.model.AddFriendIntimacy(stNtfChgIntimacy.stUin.ullUid, stNtfChgIntimacy.stUin.dwLogicWorldId, stNtfChgIntimacy.stIntimacData.dwLastIntimacyTime, stNtfChgIntimacy.stIntimacData.wIntimacyValue);
                Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
                if ((this.view != null) && this.view.IsActive())
                {
                    this.view.Refresh();
                }
            }
            else if (msg.stPkgData.stNtfChgIntimacy.dwResult == 170)
            {
                this.IntimacyUpInfo = "跟朋友的亲密度已满";
                if (!Singleton<BattleLogic>.instance.isRuning)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips(this.IntimacyUpInfo, false, 1.5f, null, new object[0]);
                    this.IntimacyUpInfo = string.Empty;
                }
                SCPKG_CMD_NTF_CHG_INTIMACY scpkg_cmd_ntf_chg_intimacy2 = msg.stPkgData.stNtfChgIntimacy;
                this.model.AddFriendIntimacy(scpkg_cmd_ntf_chg_intimacy2.stUin.ullUid, scpkg_cmd_ntf_chg_intimacy2.stUin.dwLogicWorldId, scpkg_cmd_ntf_chg_intimacy2.stIntimacData.dwLastIntimacyTime, scpkg_cmd_ntf_chg_intimacy2.stIntimacData.wIntimacyValue);
                Singleton<CFriendContoller>.GetInstance().model.SortGameFriend();
                if ((this.view != null) && this.view.IsActive())
                {
                    this.view.Refresh();
                }
            }
        }

        private void OnFriend_Chat_Button(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
            if (component != null)
            {
                COMDT_FRIEND_INFO info = this.model.GetInfo(CFriendModel.FriendType.GameFriend, component.ullUid, component.dwLogicWorldID);
                if (info == null)
                {
                    info = this.model.GetInfo(CFriendModel.FriendType.SNS, component.ullUid, component.dwLogicWorldID);
                    if (info == null)
                    {
                        return;
                    }
                }
                Singleton<CChatController>.instance.Jump2FriendChat(info);
            }
        }

        private void OnFriend_Gift_Button(CUIEvent uievent)
        {
            FriendShower component = uievent.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
            if (component != null)
            {
                if (!Utility.IsSamePlatform(component.dwLogicWorldID))
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("CS_PRESENTHEROSKIN_INVALID_PLAT", true, 1.5f, null, new object[0]);
                }
                else
                {
                    bool isSns = false;
                    if (this.model.GetInfo(CFriendModel.FriendType.GameFriend, component.ullUid, component.dwLogicWorldID) == null)
                    {
                        isSns = true;
                        if (this.model.GetInfo(CFriendModel.FriendType.SNS, component.ullUid, component.dwLogicWorldID) == null)
                        {
                            return;
                        }
                    }
                    Singleton<CMallSystem>.GetInstance().OpenGiftCenter(component.ullUid, component.dwLogicWorldID, isSns);
                }
            }
        }

        private void OnFriend_LBS_CheckInfo(CUIEvent evt)
        {
            FriendShower component = evt.m_srcWidget.transform.parent.parent.parent.GetComponent<FriendShower>();
            if (component != null)
            {
                Singleton<CPlayerInfoSystem>.instance.ShowPlayerDetailInfo(component.ullUid, (int) component.dwLogicWorldID, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers);
            }
        }

        private void OnFriend_LBS_Nan(CUIEvent evt)
        {
            CFriendModel model = Singleton<CFriendContoller>.instance.model;
            if (model.fileter != 1)
            {
                this.SetFilter(model.NegFlag(model.fileter, 0));
            }
            else
            {
                this.SetFilter(model.fileter);
            }
        }

        private void OnFriend_LBS_NoShare(CUIEvent evt)
        {
            Singleton<CFriendContoller>.instance.model.EnableShareLocation = !Singleton<CFriendContoller>.instance.model.EnableShareLocation;
            bool enableShareLocation = Singleton<CFriendContoller>.instance.model.EnableShareLocation;
            FriendSysNetCore.Send_Request_Sns_Switch(COM_REFUSE_TYPE.COM_REFUSE_TYPE_LBSSHARE, !enableShareLocation ? 0 : 1);
            if (this.view != null)
            {
                this.view.SyncLBSShareBtnState();
                this.view.Refresh_List(this.view.CurTab);
            }
            if (!enableShareLocation)
            {
                FriendSysNetCore.Send_Clear_Location();
            }
            else if (!MonoSingleton<GPSSys>.instance.bGetGPSData)
            {
                MonoSingleton<GPSSys>.instance.StartGPS();
                Singleton<CUIManager>.instance.OpenTips("正在定位，请稍后...", false, 1.5f, null, new object[0]);
            }
            else
            {
                FriendSysNetCore.Send_Report_Clt_Location(MonoSingleton<GPSSys>.instance.iLongitude, MonoSingleton<GPSSys>.instance.iLatitude);
            }
        }

        private void OnFriend_LBS_Nv(CUIEvent evt)
        {
            CFriendModel model = Singleton<CFriendContoller>.instance.model;
            if (model.fileter != 2)
            {
                this.SetFilter(model.NegFlag(model.fileter, 1));
            }
            else
            {
                this.SetFilter(model.fileter);
            }
        }

        private void OnFriend_LBS_Refresh(CUIEvent evt)
        {
            int iLongitude = MonoSingleton<GPSSys>.instance.iLongitude;
            int iLatitude = MonoSingleton<GPSSys>.instance.iLatitude;
            bool isShowAlert = evt.m_eventParams.tag == 0;
            if (!CSysDynamicBlock.bFriendBlocked && ((iLongitude == 0) || (iLatitude == 0)))
            {
                string text = Singleton<CTextManager>.instance.GetText("LBS_Location_Error");
                Singleton<CUIManager>.instance.OpenTips(text, false, 1.5f, null, new object[0]);
                Singleton<CFriendContoller>.instance.model.searchLBSZero = text;
                if ((this.view != null) && (this.view.ifnoText != null))
                {
                    this.view.ifnoText.text = text;
                }
            }
            FriendSysNetCore.Send_Search_LBS_Req(this.model.GetLBSGenterFilter(), iLongitude, iLatitude, isShowAlert);
            this.startCooldownTimestamp = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
        }

        private void OnFriend_Room_AddFriend(CUIEvent evt)
        {
            if ((evt.m_eventParams.commonUInt64Param1 != 0) && (evt.m_eventParams.commonUInt32Param1 != 0))
            {
                this.Open_Friend_Verify(evt.m_eventParams.commonUInt64Param1, evt.m_eventParams.commonUInt32Param1, false);
            }
        }

        private void OnFriend_Show_Rule(CUIEvent uievent)
        {
            ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((uint) 12);
            if (dataByKey != null)
            {
                string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
                string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
                Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
            }
        }

        private void OnNewDayNtf()
        {
            this.model.SnsReCallData.Clear();
            this.model.HeartData.Clear();
            if ((this.view != null) && this.view.IsActive())
            {
                this.view.Refresh();
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.getFriendCoinCnt = 0;
                Singleton<CMailSys>.instance.UpdateUnReadNum();
            }
        }

        public void OnReCallFriendNtf(CSPkg msg)
        {
            uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
            uint num2 = 0x15180 * GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x9e).dwConfValue;
            for (int i = 0; i < msg.stPkgData.stNtfRecallFirend.wRecallNum; i++)
            {
                COMDT_RECALL_FRIEND comdt_recall_friend = msg.stPkgData.stNtfRecallFirend.astRecallAcntList[i];
                if (comdt_recall_friend != null)
                {
                    this.Handle_Invite_Data(comdt_recall_friend.stAcntUniq);
                }
            }
        }

        public void OnSCPKG_CMD_BLACKFORFRIENDREQ(CSPkg msg)
        {
        }

        public void OnSCPKG_CMD_CANCEL_DEFRIEND(CSPkg msg)
        {
            SCPKG_CMD_CANCEL_DEFRIEND stCancelDeFriendRsp = msg.stPkgData.stCancelDeFriendRsp;
            if (stCancelDeFriendRsp.dwResult == 0)
            {
                this.model.RemoveFriendBlack(stCancelDeFriendRsp.stUin.ullUid, stCancelDeFriendRsp.stUin.dwLogicWorldId);
                if ((this.view != null) && this.view.IsActive())
                {
                    this.view.Refresh();
                }
            }
            else
            {
                Singleton<CUIManager>.instance.OpenTips("---black OnSCPKG_CMD_CANCEL_DEFRIEND error code:" + stCancelDeFriendRsp.dwResult, false, 1.5f, null, new object[0]);
            }
        }

        public void OnSCPKG_CMD_DEFRIEND(CSPkg msg)
        {
            SCPKG_CMD_DEFRIEND stDeFriendRsp = msg.stPkgData.stDeFriendRsp;
            if (stDeFriendRsp.dwResult == 0)
            {
                COMDT_FRIEND_INFO comdt_friend_info;
                COMDT_CHAT_PLAYER_INFO comdt_chat_player_info;
                this.model.GetUser(stDeFriendRsp.stUin.ullUid, stDeFriendRsp.stUin.dwLogicWorldId, out comdt_friend_info, out comdt_chat_player_info);
                if (comdt_friend_info == null)
                {
                    if (comdt_chat_player_info != null)
                    {
                        this.model.AddFriendBlack(comdt_chat_player_info, stDeFriendRsp.bGender, stDeFriendRsp.dwLastLoginTime);
                        if ((this.view != null) && this.view.IsActive())
                        {
                            this.view.Refresh();
                        }
                    }
                    else
                    {
                        DebugHelper.Assert(false, string.Concat(new object[] { "---black 找到不到 ulluid:", stDeFriendRsp.stUin.ullUid, ",worldID:", stDeFriendRsp.stUin.dwLogicWorldId, ",对应的玩家数据" }));
                    }
                }
                else
                {
                    this.model.AddFriendBlack(comdt_friend_info);
                    if ((this.view != null) && this.view.IsActive())
                    {
                        this.view.Refresh();
                    }
                }
                Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.PlayerBlock_Success);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stDeFriendRsp.dwResult), false, 1.5f, null, new object[0]);
            }
        }

        private void OnShareToggle(CUIEvent uievent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                bool flag = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_DONOTE_AND_REC);
                int num = 1;
                masterRoleInfo.snsSwitchBits ^= (uint) num;
                FriendSysNetCore.Send_Request_Sns_Switch(COM_REFUSE_TYPE.COM_REFUSE_TYPE_DONOTE_AND_REC, flag ? 0 : 1);
            }
        }

        public void OnSnsSwitchNtf(CSPkg msg)
        {
            COMDT_ACNT_UNIQ stUin = msg.stPkgData.stNtfRefuseRecall.stUin;
            COMDT_FRIEND_INFO comdt_friend_info = this.model.getFriendByUid(stUin.ullUid, CFriendModel.FriendType.SNS);
            if (comdt_friend_info != null)
            {
                comdt_friend_info.dwRefuseFriendBits = msg.stPkgData.stNtfRefuseRecall.dwRefuseFriendBits;
            }
        }

        public void OnSnsSwitchRsp(CSPkg msg)
        {
            if (msg.stPkgData.stRefuseRecallRsp.dwResult == 0)
            {
            }
        }

        public void Open_Friend_Verify(ulong ullUid, uint dwLogicWorldId, bool bAddSearchFirend)
        {
            if ((this.view != null) && (this.view.verficationView != null))
            {
                this.view.verficationView.Open(ullUid, dwLogicWorldId, bAddSearchFirend);
            }
        }

        private void QQBox_OnClick(CUIEvent uievent)
        {
            if (ApolloConfig.platform == ApolloPlatform.QQ)
            {
                MonoSingleton<IDIPSys>.GetInstance().RequestQQBox();
            }
        }

        private void Remove_And_Refresh(CFriendModel.FriendType type, COMDT_ACNT_UNIQ uniq)
        {
            this.model.Remove(type, uniq);
            if (this.view.IsActive())
            {
                this.view.Refresh();
            }
        }

        public void SetFilter(uint value)
        {
            this.model.fileter = value;
            if (this.view != null)
            {
                this.view.SyncGenderToggleState();
                this.view.Refresh_List(this.view.CurTab);
            }
        }

        public void ShareTo_SNSFriend_ReCall(string openId)
        {
            if (MonoSingleton<ShareSys>.instance.IsInstallPlatform())
            {
                string text = Singleton<CTextManager>.instance.GetText("Common_Sns_Tips_11");
                string desc = Singleton<CTextManager>.instance.GetText("Common_Sns_Tips_12");
                Singleton<ApolloHelper>.instance.ShareInviteFriend(openId, text, desc, ShareSys.SNS_SHARE_RECALL_FRIEND);
                Singleton<CUIManager>.instance.OpenTips("Common_Sns_Tips_13", true, 1.5f, null, new object[0]);
            }
        }

        public void Update()
        {
            this.view.Update();
        }

        private void Update_Send_Coin_Data(COMDT_ACNT_UNIQ uin, ulong donateAPSec, COM_FRIEND_TYPE friendType)
        {
            DateTime time = Utility.ToUtcTime2Local((long) donateAPSec);
            DateTime time2 = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
            if (((time2.Year == time.Year) && (time2.Month == time.Month)) && (time2.Day == time.Day))
            {
                this.model.HeartData.Add(uin, friendType);
            }
            if (((time2.Year >= time.Year) && ((time2.Year != time.Year) || (time2.Month >= time.Month))) && (((time2.Year != time.Year) || (time2.Month != time.Month)) || (time2.Day < time.Day)))
            {
            }
        }

        public ulong startCooldownTimestamp { get; set; }
    }
}

