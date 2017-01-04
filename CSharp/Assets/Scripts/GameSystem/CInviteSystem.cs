namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    internal class CInviteSystem : Singleton<CInviteSystem>
    {
        private ListView<COMDT_FRIEND_INFO> allFriendList_internal;
        public int gameTimer = -1;
        private uint lastRefreshLBSTime;
        private ListView<GuildMemInfo> m_allGuildMemberList;
        private COM_INVITE_JOIN_TYPE m_inviteType;
        private bool m_isFirstlySelectGuildMemberTab = true;
        private bool m_isNeedRefreshGuildMemberPanel = true;
        private ListView<InviteState> m_stateList = new ListView<InviteState>();
        public static string PATH_INVITE_FORM = "UGUI/Form/System/PvP/Form_InviteFriend.prefab";
        private const int REFRESH_GUILD_MEMBER_GAME_STATE_SECONDS = 10;
        private const int REFRESH_GUILD_MEMBER_GAME_STATE_WAIT_MILLISECONDS = 0xbb8;
        private static uint s_refreshLBSTime;

        private void AddInviteStateList(ulong uid, uint time, enInviteState state)
        {
            for (int i = 0; i < this.m_stateList.Count; i++)
            {
                if (uid == this.m_stateList[i].uid)
                {
                    this.m_stateList[i].uid = uid;
                    this.m_stateList[i].time = time;
                    this.m_stateList[i].state = state;
                    return;
                }
            }
            InviteState item = new InviteState {
                uid = uid,
                time = time,
                state = state
            };
            this.m_stateList.Add(item);
        }

        private void ChangeInviteStateList(ulong uid, enInviteState state)
        {
            for (int i = 0; i < this.m_stateList.Count; i++)
            {
                if (uid == this.m_stateList[i].uid)
                {
                    this.m_stateList[i].state = state;
                    return;
                }
            }
        }

        public void CheckInviteListGameTimer()
        {
            if (this.gameTimer == -1)
            {
                this.gameTimer = Singleton<CTimerManager>.instance.AddTimer(0xfde8, 0, new CTimer.OnTimeUpHandler(this.OnInviteListGameTimer));
            }
        }

        public void Clear()
        {
            this.lastRefreshLBSTime = 0;
        }

        public void ClearInviteListGameTimer()
        {
            if (this.gameTimer != -1)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.gameTimer);
                this.gameTimer = -1;
            }
        }

        public void CloseInviteForm()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(PATH_INVITE_FORM);
            Singleton<CInviteSystem>.instance.ClearInviteListGameTimer();
            if (this.m_inviteType == COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM)
            {
                Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Team, 0L, 0);
                Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
                Singleton<CChatController>.instance.ShowPanel(true, false);
                Singleton<CChatController>.instance.view.UpView(false);
                Singleton<CChatController>.instance.model.sysData.ClearEntryText();
            }
        }

        public ListView<COMDT_FRIEND_INFO> GetAllFriendList()
        {
            return this.m_allFriendList;
        }

        public ListView<GuildMemInfo> GetAllGuildMemberList()
        {
            return this.m_allGuildMemberList;
        }

        public string GetInviteFriendName(ulong friendUid, uint friendLogicWorldId)
        {
            if (this.m_allFriendList != null)
            {
                for (int i = 0; i < this.m_allFriendList.Count; i++)
                {
                    if ((friendUid == this.m_allFriendList[i].stUin.ullUid) && (friendLogicWorldId == this.m_allFriendList[i].stUin.dwLogicWorldId))
                    {
                        return StringHelper.UTF8BytesToString(ref this.m_allFriendList[i].szUserName);
                    }
                }
            }
            return string.Empty;
        }

        public string GetInviteGuildMemberName(ulong guildMemberUid)
        {
            if (this.m_allGuildMemberList != null)
            {
                for (int i = 0; i < this.m_allGuildMemberList.Count; i++)
                {
                    if (guildMemberUid == this.m_allGuildMemberList[i].stBriefInfo.uulUid)
                    {
                        return this.m_allGuildMemberList[i].stBriefInfo.sName;
                    }
                }
            }
            return string.Empty;
        }

        private static string GetInviteRoomFailReason(string fName, int errCode)
        {
            string str = string.Empty;
            switch (errCode)
            {
                case 11:
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Can_Not_Find_Friend"), fName);

                case 12:
                {
                    COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(fName, CFriendModel.FriendType.GameFriend);
                    if (comdt_friend_info == null)
                    {
                        comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(fName, CFriendModel.FriendType.SNS);
                    }
                    if (comdt_friend_info != null)
                    {
                        comdt_friend_info.bIsOnline = 0;
                        Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_Friend_Online_Change");
                    }
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Friend_Off_Line"), fName);
                }
                case 13:
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Gaming_Tip"), fName);

                case 14:
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Invite_Refuse"), fName);

                case 15:
                case 0x10:
                case 0x11:
                case 0x12:
                case 0x13:
                case 20:
                case 0x19:
                case 0x1a:
                case 0x1b:
                case 0x1c:
                    return str;

                case 0x15:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Version_Different");

                case 0x16:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Version_Higher");

                case 0x17:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Version_Lower");

                case 0x18:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_ENTERTAINMENT_Lock");

                case 0x1d:
                    return Singleton<CTextManager>.GetInstance().GetText("CS_ROOMERR_PLAT_CHANNEL_CLOSE");

                case 30:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Room_Ban_Pick_Hero_Limit_Accept");
            }
            return str;
        }

        private enInviteState GetInviteState(ulong uid)
        {
            for (int i = 0; i < this.m_stateList.Count; i++)
            {
                if (uid == this.m_stateList[i].uid)
                {
                    return this.m_stateList[i].state;
                }
            }
            return enInviteState.None;
        }

        public string GetInviteStateStr(ulong uid)
        {
            switch (this.GetInviteState(uid))
            {
                case enInviteState.None:
                    return string.Format("<color=#00ff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Online"));

                case enInviteState.Invited:
                    return string.Format("<color=#ffffff>{0}</color>", Singleton<CTextManager>.instance.GetText("Guild_Has_Invited"));

                case enInviteState.BeRejcet:
                    return string.Format("<color=#ff0000>{0}</color>", Singleton<CTextManager>.instance.GetText("Invite_Friend_Tips_2"));
            }
            return string.Empty;
        }

        private static string GetInviteString(SCPKG_INVITE_JOIN_GAME_REQ msg)
        {
            string str = Utility.UTF8Convert(msg.stInviterInfo.szName);
            string text = string.Empty;
            string str3 = string.Empty;
            string str4 = string.Empty;
            uint dwRelationMask = msg.stInviterInfo.dwRelationMask;
            if ((dwRelationMask & 1) > 0)
            {
                text = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_1");
            }
            else if ((dwRelationMask & 2) > 0)
            {
                text = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_4");
            }
            else if ((dwRelationMask & 4) > 0)
            {
                text = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_5");
            }
            else
            {
                text = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_1");
            }
            if (msg.bInviteType == 1)
            {
                ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(msg.stInviteDetail.stRoomDetail.bMapType, msg.stInviteDetail.stRoomDetail.dwMapId);
                if (pvpMapCommonInfo != null)
                {
                    string[] textArray1 = new string[] { (pvpMapCommonInfo.bMaxAcntNum / 2).ToString(), (pvpMapCommonInfo.bMaxAcntNum / 2).ToString(), Utility.UTF8Convert(pvpMapCommonInfo.szName) };
                    str4 = Singleton<CTextManager>.instance.GetText("Invite_Map_Desc", textArray1);
                }
                str3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_4");
            }
            else if (msg.bInviteType == 2)
            {
                ResDT_LevelCommonInfo info2 = CLevelCfgLogicManager.GetPvpMapCommonInfo(msg.stInviteDetail.stTeamDetail.bMapType, msg.stInviteDetail.stTeamDetail.dwMapId);
                if (info2 != null)
                {
                    string[] textArray2 = new string[] { (info2.bMaxAcntNum / 2).ToString(), (info2.bMaxAcntNum / 2).ToString(), Utility.UTF8Convert(info2.szName) };
                    str4 = Singleton<CTextManager>.instance.GetText("Invite_Map_Desc", textArray2);
                }
                if (msg.stInviteDetail.stTeamDetail.bMapType == 3)
                {
                    str3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_1");
                }
                else if (msg.stInviteDetail.stTeamDetail.bPkAI == 1)
                {
                    str3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_2");
                }
                else
                {
                    str3 = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_3");
                }
            }
            string[] args = new string[] { str, text, str3, str4 };
            return Singleton<CTextManager>.instance.GetText("Be_Invited_Tips", args);
        }

        private static string GetInviteTeamFailReason(string fName, int errCode, uint timePunished = 0, uint punishType = 1)
        {
            string str = string.Empty;
            switch (errCode)
            {
                case 3:
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Can_Not_Find_Friend"), fName);

                case 4:
                {
                    COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(fName, CFriendModel.FriendType.GameFriend);
                    if (comdt_friend_info == null)
                    {
                        comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(fName, CFriendModel.FriendType.SNS);
                    }
                    if (comdt_friend_info != null)
                    {
                        comdt_friend_info.bIsOnline = 0;
                        Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_Friend_Online_Change");
                    }
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Friend_Off_Line"), fName);
                }
                case 5:
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Gaming_Tip"), fName);

                case 6:
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText("PVP_Invite_Refuse"), fName);

                case 7:
                case 8:
                case 10:
                case 11:
                case 13:
                case 0x16:
                case 0x18:
                case 0x19:
                    return str;

                case 9:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Team_Member_Full");

                case 12:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Invite_Result_4");

                case 14:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Version_Higher");

                case 15:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Version_Lower");

                case 0x10:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_ENTERTAINMENT_Lock");

                case 0x11:
                {
                    string str2 = string.Format("{0}分{1}秒", timePunished / 60, timePunished % 60);
                    string key = (punishType != 1) ? "PVP_Invite_HangUpPunished" : "PVP_Invite_Punished";
                    return string.Format(Singleton<CTextManager>.GetInstance().GetText(key), fName, str2);
                }
                case 0x12:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Invite_Result_1");

                case 0x13:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Invite_Result_2");

                case 20:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Invite_Result_3");

                case 0x15:
                    return Singleton<CTextManager>.GetInstance().GetText("Err_Invite_Result_5");

                case 0x17:
                    return Singleton<CTextManager>.GetInstance().GetText("Invite_Err_Credit_Limit");

                case 0x1a:
                    return Singleton<CTextManager>.GetInstance().GetText("COM_MATCHTEAMEERR_PLAT_CHANNEL_CLOSE");

                case 0x1b:
                    return Singleton<CTextManager>.GetInstance().GetText("COM_MATCHTEAMEERR_OBING");
            }
            return str;
        }

        public CUIListElementScript GetListItem(string username)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_INVITE_FORM);
            if (form != null)
            {
                CUIListScript component = form.transform.Find("Panel_Friend/List").gameObject.GetComponent<CUIListScript>();
                for (int i = 0; i < component.m_elementAmount; i++)
                {
                    CUIListElementScript elemenet = component.GetElemenet(i);
                    if (elemenet.gameObject.transform.Find("PlayerName").GetComponent<Text>().text == username)
                    {
                        return elemenet;
                    }
                }
            }
            return null;
        }

        private uint GetNextInviteSec(ulong uid, uint time)
        {
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x9c).dwConfValue;
            for (int i = 0; i < this.m_stateList.Count; i++)
            {
                if (uid == this.m_stateList[i].uid)
                {
                    return ((this.m_stateList[i].time + dwConfValue) - time);
                }
            }
            return 0;
        }

        private static int GuildMemberComparison(GuildMemInfo a, GuildMemInfo b)
        {
            if (CGuildHelper.IsMemberOnline(a) && !CGuildHelper.IsMemberOnline(b))
            {
                return -1;
            }
            if (!CGuildHelper.IsMemberOnline(a) && CGuildHelper.IsMemberOnline(b))
            {
                return 1;
            }
            return ((a.stBriefInfo.uulUid >= b.stBriefInfo.uulUid) ? 1 : -1);
        }

        private bool InInviteCdList(ulong uid, uint time)
        {
            for (int i = 0; i < this.m_stateList.Count; i++)
            {
                if (uid == this.m_stateList[i].uid)
                {
                    return ((time - this.m_stateList[i].time) < GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x9c).dwConfValue);
                }
            }
            return false;
        }

        public override void Init()
        {
            base.Init();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_SendInviteFriend, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_SendInviteGuildMember, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteGuildMember));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_SendInviteLBS, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteLBS));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_AcceptInvite, new CUIEventManager.OnUIEventHandler(this.OnInvite_Accept));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RejectInvite, new CUIEventManager.OnUIEventHandler(this.OnInvite_Reject));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_TimeOut, new CUIEventManager.OnUIEventHandler(this.OnInvateFriendTimeOut));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_FriendListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_FriendListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_GuildMemberListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_GuildMemberListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_LBSListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_LBSListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_TabChange, new CUIEventManager.OnUIEventHandler(this.OnInvite_TabChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Invite_RefreshGameStateTimeout, new CUIEventManager.OnUIEventHandler(this.OnInvite_RefreshGameStateTimeout));
            Singleton<EventRouter>.GetInstance().AddEventHandler<byte, string>(EventID.INVITE_ERRCODE_NTF, new Action<byte, string>(this.OnInviteErrCodeNtf));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_ADD_NTF", new Action<CSPkg>(this.OnFriendChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<CSPkg>("Friend_Delete_NTF", new Action<CSPkg>(this.OnFriendChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Chat_Friend_Online_Change", new System.Action(this.OnFriendOnlineChg));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.Friend_Game_State_Change, new System.Action(this.OnFriendOnlineChg));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Friend_LBS_User_Refresh", new System.Action(this.OnLBSListChg));
        }

        private void InitAllGuildMemberList()
        {
            ListView<GuildMemInfo> guildMemberInfos = CGuildHelper.GetGuildMemberInfos();
            this.m_allGuildMemberList = new ListView<GuildMemInfo>();
            this.m_allGuildMemberList.AddRange(guildMemberInfos);
            for (int i = this.m_allGuildMemberList.Count - 1; i >= 0; i--)
            {
                if (CGuildHelper.IsSelf(this.m_allGuildMemberList[i].stBriefInfo.uulUid) || Singleton<CFriendContoller>.instance.model.IsBlack(this.m_allGuildMemberList[i].stBriefInfo.uulUid, (uint) this.m_allGuildMemberList[i].stBriefInfo.dwLogicWorldId))
                {
                    this.m_allGuildMemberList.RemoveAt(i);
                }
            }
        }

        private void OnFriendChange(CSPkg msg)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_INVITE_FORM);
            if (form != null)
            {
                this.SortAllFriendList();
                CInviteView.SetInviteFriendData(form, this.m_inviteType);
            }
        }

        private void OnFriendOnlineChg()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_INVITE_FORM);
            if (form != null)
            {
                this.SortAllFriendList();
                CInviteView.SetInviteFriendData(form, this.m_inviteType);
            }
        }

        [MessageHandler(0x7e4)]
        public static void OnGetInvited(CSPkg msg)
        {
            if ((((!Singleton<BattleLogic>.GetInstance().isRuning && !Singleton<WatchController>.GetInstance().IsWatching) && (msg.stPkgData.stInviteJoinGameReq != null)) && !Singleton<SettlementSystem>.GetInstance().IsExistSettleForm()) && !Singleton<CFriendContoller>.instance.model.IsBlack(msg.stPkgData.stInviteJoinGameReq.stInviterInfo.ullUid, msg.stPkgData.stInviteJoinGameReq.stInviterInfo.dwLogicWorldID))
            {
                ShowNewBeingInvitedUI(msg.stPkgData.stInviteJoinGameReq);
            }
        }

        private void OnInvateFriendTimeOut(CUIEvent uiEvent)
        {
            if (uiEvent.m_srcFormScript != null)
            {
                Singleton<CUIManager>.GetInstance().CloseForm(uiEvent.m_srcFormScript);
            }
        }

        private void OnInvite_Accept(CUIEvent uiEvent)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7e5);
            msg.stPkgData.stInviteJoinGameRsp.bIndex = (byte) uiEvent.m_eventParams.tag;
            msg.stPkgData.stInviteJoinGameRsp.bResult = 0;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            Singleton<CUIManager>.GetInstance().CloseMessageBox();
        }

        private void OnInvite_FriendListElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_allFriendList.Count))
            {
                CInviteView.UpdateFriendListElement(srcWidget, this.m_allFriendList[srcWidgetIndexInBelongedList]);
            }
        }

        private void OnInvite_GuildMemberListElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_allGuildMemberList.Count))
            {
                CInviteView.UpdateGuildMemberListElement(srcWidget, this.m_allGuildMemberList[srcWidgetIndexInBelongedList]);
            }
        }

        private void OnInvite_LBSListElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject srcWidget = uiEvent.m_srcWidget;
            ListView<CSDT_LBS_USER_INFO> lBSList = Singleton<CFriendContoller>.instance.model.GetLBSList(CFriendModel.LBSGenderType.Both);
            if ((lBSList != null) && ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < lBSList.Count)))
            {
                CInviteView.UpdateLBSListElement(srcWidget, lBSList[srcWidgetIndexInBelongedList]);
            }
        }

        private void OnInvite_RefreshGameStateTimeout(CUIEvent uiEvent)
        {
            this.SendGetGuildMemberGameStateReq();
        }

        private void OnInvite_Reject(CUIEvent uiEvent)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7e5);
            msg.stPkgData.stInviteJoinGameRsp.bIndex = (byte) uiEvent.m_eventParams.tag;
            msg.stPkgData.stInviteJoinGameRsp.bResult = 14;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void OnInvite_SendInviteFriend(CUIEvent uiEvent)
        {
            COM_INVITE_JOIN_TYPE tag = (COM_INVITE_JOIN_TYPE) uiEvent.m_eventParams.tag;
            ulong uid = uiEvent.m_eventParams.commonUInt64Param1;
            uint logicWorldId = (uint) uiEvent.m_eventParams.tag2;
            uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
            if (this.InInviteCdList(uid, currentUTCTime))
            {
                object[] replaceArr = new object[] { this.GetNextInviteSec(uid, currentUTCTime) };
                Singleton<CUIManager>.instance.OpenTips("Invite_Friend_Tips_1", true, 1f, null, replaceArr);
            }
            else
            {
                bool flag = Singleton<CFriendContoller>.instance.model.IsGameFriend(uid, logicWorldId);
                CFriendModel.FriendType friendType = !flag ? CFriendModel.FriendType.SNS : CFriendModel.FriendType.GameFriend;
                byte num4 = !flag ? ((byte) 2) : ((byte) 1);
                COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByUid(uid, friendType);
                if (comdt_friend_info != null)
                {
                    switch (tag)
                    {
                        case COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_ROOM:
                        {
                            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7e1);
                            msg.stPkgData.stInviteFriendJoinRoomReq.stFriendInfo.ullUid = comdt_friend_info.stUin.ullUid;
                            msg.stPkgData.stInviteFriendJoinRoomReq.stFriendInfo.dwLogicWorldId = comdt_friend_info.stUin.dwLogicWorldId;
                            msg.stPkgData.stInviteFriendJoinRoomReq.bFriendType = num4;
                            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                            break;
                        }
                        case COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM:
                        {
                            CSPkg pkg2 = NetworkModule.CreateDefaultCSPKG(0x7e8);
                            pkg2.stPkgData.stInviteFriendJoinTeamReq.stFriendInfo.ullUid = comdt_friend_info.stUin.ullUid;
                            pkg2.stPkgData.stInviteFriendJoinTeamReq.stFriendInfo.dwLogicWorldId = comdt_friend_info.stUin.dwLogicWorldId;
                            pkg2.stPkgData.stInviteFriendJoinTeamReq.bFriendType = num4;
                            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref pkg2, false);
                            break;
                        }
                    }
                    if (uiEvent.m_srcWidget.transform.parent != null)
                    {
                        Transform transform = uiEvent.m_srcWidget.transform.parent.Find("Online");
                        if (transform != null)
                        {
                            transform.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Guild_Has_Invited");
                        }
                    }
                    this.AddInviteStateList(uid, currentUTCTime, enInviteState.Invited);
                }
            }
        }

        private void OnInvite_SendInviteGuildMember(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            ulong num2 = uiEvent.m_eventParams.commonUInt64Param1;
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7f2);
            msg.stPkgData.stInviteGuildMemberJoinReq.iInviteType = tag;
            msg.stPkgData.stInviteGuildMemberJoinReq.ullInviteeUid = num2;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            uiEvent.m_srcWidget.CustomSetActive(false);
            if (uiEvent.m_srcWidget.transform.parent != null)
            {
                Transform transform = uiEvent.m_srcWidget.transform.parent.Find("Online");
                if (transform != null)
                {
                    transform.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Guild_Has_Invited");
                }
            }
        }

        private void OnInvite_SendInviteLBS(CUIEvent uiEvent)
        {
            COM_INVITE_JOIN_TYPE tag = (COM_INVITE_JOIN_TYPE) uiEvent.m_eventParams.tag;
            ulong uid = uiEvent.m_eventParams.commonUInt64Param1;
            uint num2 = (uint) uiEvent.m_eventParams.tag2;
            uint num3 = (uint) uiEvent.m_eventParams.tag3;
            uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
            if (this.InInviteCdList(uid, currentUTCTime))
            {
                object[] replaceArr = new object[] { this.GetNextInviteSec(uid, currentUTCTime) };
                Singleton<CUIManager>.instance.OpenTips("Invite_Friend_Tips_1", true, 1f, null, replaceArr);
            }
            else
            {
                switch (tag)
                {
                    case COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_ROOM:
                    {
                        CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7e1);
                        msg.stPkgData.stInviteFriendJoinRoomReq.stFriendInfo.ullUid = uid;
                        msg.stPkgData.stInviteFriendJoinRoomReq.stFriendInfo.dwLogicWorldId = num2;
                        msg.stPkgData.stInviteFriendJoinRoomReq.bFriendType = 3;
                        msg.stPkgData.stInviteFriendJoinRoomReq.dwGameSvrEntity = num3;
                        Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                        break;
                    }
                    case COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM:
                    {
                        CSPkg pkg2 = NetworkModule.CreateDefaultCSPKG(0x7e8);
                        pkg2.stPkgData.stInviteFriendJoinTeamReq.stFriendInfo.ullUid = uid;
                        pkg2.stPkgData.stInviteFriendJoinTeamReq.stFriendInfo.dwLogicWorldId = num2;
                        pkg2.stPkgData.stInviteFriendJoinTeamReq.bFriendType = 3;
                        pkg2.stPkgData.stInviteFriendJoinTeamReq.dwGameSvrEntity = num3;
                        Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref pkg2, false);
                        break;
                    }
                }
                if (uiEvent.m_srcWidget.transform.parent != null)
                {
                    Transform transform = uiEvent.m_srcWidget.transform.parent.Find("Online");
                    if (transform != null)
                    {
                        transform.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("Guild_Has_Invited");
                    }
                }
                this.AddInviteStateList(uid, currentUTCTime, enInviteState.Invited);
            }
        }

        private void OnInvite_TabChange(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            GameObject widget = srcFormScript.GetWidget(0);
            GameObject obj3 = srcFormScript.GetWidget(1);
            GameObject obj4 = srcFormScript.GetWidget(10);
            switch (CInviteView.GetInviteListTab(component.GetSelectedIndex()))
            {
                case CInviteView.enInviteListTab.Friend:
                    widget.CustomSetActive(true);
                    obj3.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    break;

                case CInviteView.enInviteListTab.GuildMember:
                    obj3.CustomSetActive(true);
                    widget.CustomSetActive(false);
                    obj4.CustomSetActive(false);
                    if (this.m_isFirstlySelectGuildMemberTab)
                    {
                        this.InitAllGuildMemberList();
                        this.SendGetGuildMemberGameStateReq();
                        CUITimerScript script3 = srcFormScript.GetWidget(8).GetComponent<CUITimerScript>();
                        script3.SetTotalTime(1000f);
                        script3.SetOnChangedIntervalTime(10f);
                        script3.StartTimer();
                        this.m_isFirstlySelectGuildMemberTab = false;
                    }
                    break;

                case CInviteView.enInviteListTab.LBS:
                {
                    obj3.CustomSetActive(false);
                    widget.CustomSetActive(false);
                    obj4.CustomSetActive(true);
                    CInviteView.SetLBSData(srcFormScript, this.m_inviteType);
                    uint currentUTCTime = (uint) CRoleInfo.GetCurrentUTCTime();
                    if ((currentUTCTime - this.lastRefreshLBSTime) > RefreshLBSTime)
                    {
                        this.lastRefreshLBSTime = currentUTCTime;
                        CUIEvent event2 = new CUIEvent {
                            m_eventID = enUIEventID.Friend_LBS_Refresh
                        };
                        event2.m_eventParams.tag = 1;
                        Singleton<CUIEventManager>.instance.DispatchUIEvent(event2);
                    }
                    break;
                }
            }
        }

        private void OnInviteErrCodeNtf(byte errorCode, string userName)
        {
            if (errorCode == 14)
            {
                CUIListElementScript listItem = this.GetListItem(userName);
                if (listItem != null)
                {
                    listItem.transform.FindChild("Online").GetComponent<Text>().text = string.Format("<color=#ff0000>{0}</color>", Singleton<CTextManager>.instance.GetText("Invite_Friend_Tips_2"));
                }
                COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(userName, CFriendModel.FriendType.GameFriend);
                if (comdt_friend_info == null)
                {
                    comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByName(userName, CFriendModel.FriendType.SNS);
                }
                if (comdt_friend_info != null)
                {
                    this.ChangeInviteStateList(comdt_friend_info.stUin.ullUid, enInviteState.BeRejcet);
                }
            }
        }

        [MessageHandler(0x7e2)]
        public static void OnInviteFriendRoom(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            byte bErrcode = msg.stPkgData.stInviteFriendJoinRoomRsp.bErrcode;
            string str = StringHelper.UTF8BytesToString(ref msg.stPkgData.stInviteFriendJoinRoomRsp.szFriendName);
            if (bErrcode == 20)
            {
                DateTime banTime = MonoSingleton<IDIPSys>.GetInstance().GetBanTime(COM_ACNT_BANTIME_TYPE.COM_ACNT_BANTIME_BANPLAYPVP);
                object[] args = new object[] { banTime.Year, banTime.Month, banTime.Day, banTime.Hour, banTime.Minute };
                string strContent = string.Format("您被禁止竞技！截止时间为{0}年{1}月{2}日{3}时{4}分", args);
                Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(GetInviteRoomFailReason(StringHelper.UTF8BytesToString(ref msg.stPkgData.stInviteFriendJoinRoomRsp.szFriendName), msg.stPkgData.stInviteFriendJoinRoomRsp.bErrcode), false, 1.5f, null, new object[0]);
            }
            Singleton<EventRouter>.instance.BroadCastEvent<byte, string>(EventID.INVITE_ERRCODE_NTF, bErrcode, str);
        }

        [MessageHandler(0x7e9)]
        public static void OnInviteFriendTeam(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            byte bErrcode = msg.stPkgData.stInviteFriendJoinTeamRsp.bErrcode;
            string fName = StringHelper.UTF8BytesToString(ref msg.stPkgData.stInviteFriendJoinTeamRsp.szFriendName);
            uint timePunished = 0;
            uint punishType = 1;
            if (msg.stPkgData.stInviteFriendJoinTeamRsp.bErrcode == 0x11)
            {
                timePunished = msg.stPkgData.stInviteFriendJoinTeamRsp.dwParam;
                punishType = msg.stPkgData.stInviteFriendJoinTeamRsp.dwParam2;
            }
            Singleton<CUIManager>.GetInstance().OpenTips(GetInviteTeamFailReason(fName, bErrcode, timePunished, punishType), false, 1.5f, null, new object[0]);
            Singleton<EventRouter>.instance.BroadCastEvent<byte, string>(EventID.INVITE_ERRCODE_NTF, bErrcode, fName);
        }

        public void OnInviteListGameTimer(int index)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_INVITE_FORM);
            if (form != null)
            {
                CUIListScript component = form.transform.Find("Title/ListTab").GetComponent<CUIListScript>();
                if (component != null)
                {
                    switch (CInviteView.GetInviteListTab(component.GetSelectedIndex()))
                    {
                        case CInviteView.enInviteListTab.Friend:
                            CInviteView.SetInviteFriendData(form, this.m_inviteType);
                            break;

                        case CInviteView.enInviteListTab.GuildMember:
                            CInviteView.SetInviteGuildMemberData(form, this.m_inviteType);
                            break;
                    }
                }
            }
            else
            {
                this.ClearInviteListGameTimer();
            }
        }

        private void OnLBSListChg()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_INVITE_FORM);
            if (form != null)
            {
                CInviteView.SetLBSData(form, this.m_inviteType);
            }
        }

        private void OnRefreshGuildMemberGameStateTimeout(int timerSequence)
        {
            if (this.m_isNeedRefreshGuildMemberPanel)
            {
                this.RefreshGuildMemberPanel();
                this.m_isNeedRefreshGuildMemberPanel = false;
            }
        }

        public void OpenInviteForm(COM_INVITE_JOIN_TYPE inviteType)
        {
            this.m_stateList.Clear();
            this.m_isFirstlySelectGuildMemberTab = true;
            this.SortAllFriendList();
            CUIFormScript form = Singleton<CUIManager>.GetInstance().OpenForm(PATH_INVITE_FORM, false, true);
            if (form != null)
            {
                this.m_inviteType = inviteType;
                CInviteView.InitListTab(form);
                CInviteView.SetInviteFriendData(form, inviteType);
            }
            if (this.m_inviteType == COM_INVITE_JOIN_TYPE.COM_INVITE_JOIN_TEAM)
            {
                Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Team, 0L, 0);
                Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Team);
                Singleton<CChatController>.instance.ShowPanel(true, false);
                Singleton<CChatController>.instance.view.UpView(true);
                Singleton<CChatController>.instance.model.sysData.ClearEntryText();
            }
        }

        [MessageHandler(0x7f4)]
        public static void ReceiveGetGuildMemberGameStateRsp(CSPkg msg)
        {
            SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP stGetGuildMemberGameStateRsp = msg.stPkgData.stGetGuildMemberGameStateRsp;
            ListView<GuildMemInfo> allGuildMemberList = Singleton<CInviteSystem>.GetInstance().m_allGuildMemberList;
            for (int i = 0; i < stGetGuildMemberGameStateRsp.iMemberCnt; i++)
            {
                for (int j = 0; j < allGuildMemberList.Count; j++)
                {
                    if (CGuildHelper.IsMemberOnline(allGuildMemberList[j]) && (stGetGuildMemberGameStateRsp.astMemberInfo[i].ullUid == allGuildMemberList[j].stBriefInfo.uulUid))
                    {
                        allGuildMemberList[j].GameState = (COM_ACNT_GAME_STATE) stGetGuildMemberGameStateRsp.astMemberInfo[i].bGameState;
                        allGuildMemberList[j].dwGameStartTime = stGetGuildMemberGameStateRsp.astMemberInfo[i].dwGameStartTime;
                    }
                }
            }
            Singleton<CInviteSystem>.GetInstance().RefreshGuildMemberPanel();
            Singleton<CInviteSystem>.GetInstance().m_isNeedRefreshGuildMemberPanel = false;
        }

        private void RefreshGuildMemberPanel()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PATH_INVITE_FORM);
            if (form != null)
            {
                this.SortAllGuildMemberList();
                CInviteView.SetInviteGuildMemberData(form, this.m_inviteType);
            }
        }

        private void SendGetGuildMemberGameStateReq()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x7f3);
            CSPKG_GET_GUILD_MEMBER_GAME_STATE_REQ stGetGuildMemberGameStateReq = msg.stPkgData.stGetGuildMemberGameStateReq;
            int index = 0;
            for (int i = 0; i < this.m_allGuildMemberList.Count; i++)
            {
                if (CGuildHelper.IsMemberOnline(this.m_allGuildMemberList[i]))
                {
                    stGetGuildMemberGameStateReq.MemberUid[index] = this.m_allGuildMemberList[i].stBriefInfo.uulUid;
                    index++;
                }
            }
            stGetGuildMemberGameStateReq.iMemberCnt = index;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            this.m_isNeedRefreshGuildMemberPanel = true;
            Singleton<CTimerManager>.GetInstance().AddTimer(0xbb8, 1, new CTimer.OnTimeUpHandler(this.OnRefreshGuildMemberGameStateTimeout));
        }

        private static void ShowNewBeingInvitedUI(SCPKG_INVITE_JOIN_GAME_REQ info)
        {
            string str = CUIUtility.RemoveEmoji(StringHelper.UTF8BytesToString(ref info.stInviterInfo.szName));
            stUIEventParams eventParams = new stUIEventParams {
                tag = info.bIndex,
                tagStr = str
            };
            int result = 15;
            int.TryParse(Singleton<CTextManager>.instance.GetText("MessageBox_Close_Time"), out result);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(string.Format("{0}{1}", "UGUI/Form/Common/", "Form_BeInvited.prefab"));
            if (form != null)
            {
                Singleton<CUIManager>.GetInstance().CloseForm(form);
            }
            form = Singleton<CUIManager>.GetInstance().OpenForm(string.Format("{0}{1}", "UGUI/Form/Common/", "Form_BeInvited.prefab"), false, false);
            if (form != null)
            {
                form.m_formWidgets[0].GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Invite_RejectInvite, eventParams);
                form.m_formWidgets[1].GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Invite_AcceptInvite, eventParams);
                if (result != 0)
                {
                    Transform transform = form.transform.Find("closeTimer");
                    if (transform != null)
                    {
                        CUITimerScript script2 = transform.GetComponent<CUITimerScript>();
                        if (script2 != null)
                        {
                            script2.SetTotalTime((float) result);
                            script2.StartTimer();
                            script2.m_eventIDs[1] = enUIEventID.Invite_TimeOut;
                            script2.m_eventParams[1] = eventParams;
                        }
                    }
                }
                string str2 = null;
                string text = null;
                if (info.bInviteType == 1)
                {
                    ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(info.stInviteDetail.stRoomDetail.bMapType, info.stInviteDetail.stRoomDetail.dwMapId);
                    if (pvpMapCommonInfo != null)
                    {
                        string[] textArray1 = new string[] { (pvpMapCommonInfo.bMaxAcntNum / 2).ToString(), (pvpMapCommonInfo.bMaxAcntNum / 2).ToString(), Utility.UTF8Convert(pvpMapCommonInfo.szName) };
                        str2 = Singleton<CTextManager>.instance.GetText("Invite_Map_Desc", textArray1);
                    }
                    text = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_4");
                }
                else if (info.bInviteType == 2)
                {
                    ResDT_LevelCommonInfo info3 = CLevelCfgLogicManager.GetPvpMapCommonInfo(info.stInviteDetail.stTeamDetail.bMapType, info.stInviteDetail.stTeamDetail.dwMapId);
                    if (info3 != null)
                    {
                        string[] textArray2 = new string[] { (info3.bMaxAcntNum / 2).ToString(), (info3.bMaxAcntNum / 2).ToString(), Utility.UTF8Convert(info3.szName) };
                        str2 = Singleton<CTextManager>.instance.GetText("Invite_Map_Desc", textArray2);
                    }
                    if (info.stInviteDetail.stTeamDetail.bMapType == 3)
                    {
                        text = Singleton<CTextManager>.GetInstance().GetText("Invite_Match_Type_1");
                    }
                    else
                    {
                        text = Singleton<CTextManager>.GetInstance().GetText((info.stInviteDetail.stTeamDetail.bPkAI != 1) ? "Invite_Match_Type_3" : "Invite_Match_Type_2");
                    }
                }
                string[] args = new string[] { text, str2 };
                form.m_formWidgets[8].GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("Be_Invited_Tips", args);
                uint dwRelationMask = info.stInviterInfo.dwRelationMask;
                string str4 = null;
                if ((dwRelationMask & 1) > 0)
                {
                    str4 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_1");
                }
                else if ((dwRelationMask & 2) > 0)
                {
                    str4 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_4");
                }
                else if ((dwRelationMask & 4) > 0)
                {
                    str4 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_5");
                }
                else
                {
                    str4 = Singleton<CTextManager>.instance.GetText("Invite_Src_Type_1");
                }
                form.m_formWidgets[6].GetComponent<Text>().text = string.Format(Singleton<CTextManager>.instance.GetText("Be_Invited_FromType"), str4);
                form.m_formWidgets[5].GetComponent<Text>().text = str;
                COM_SNSGENDER bGender = (COM_SNSGENDER) info.stInviterInfo.bGender;
                Image component = form.m_formWidgets[4].GetComponent<Image>();
                component.gameObject.CustomSetActive(bGender != COM_SNSGENDER.COM_SNSGENDER_NONE);
                switch (bGender)
                {
                    case COM_SNSGENDER.COM_SNSGENDER_MALE:
                        CUIUtility.SetImageSprite(component, string.Format("{0}icon/Ico_boy.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false);
                        break;

                    case COM_SNSGENDER.COM_SNSGENDER_FEMALE:
                        CUIUtility.SetImageSprite(component, string.Format("{0}icon/Ico_girl.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false);
                        break;
                }
                form.m_formWidgets[3].GetComponent<CUIHttpImageScript>().SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(StringHelper.UTF8BytesToString(ref info.stInviterInfo.szHeadUrl)));
                MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(form.m_formWidgets[2].GetComponent<Image>(), (int) info.stInviterInfo.dwHeadImgId);
                form.m_formWidgets[7].CustomSetActive(info.stInviterInfo.bGradeOfRank > 0);
                if (info.stInviterInfo.bGradeOfRank > 0)
                {
                    CLadderView.ShowRankDetail(form.m_formWidgets[7], info.stInviterInfo.bGradeOfRank, 0, 1, false, true, false, true);
                }
            }
        }

        public void SortAllFriendList()
        {
            this.allFriendList_internal = Singleton<CFriendContoller>.GetInstance().model.GetAllFriend();
            this.allFriendList_internal.Sort(new Comparison<COMDT_FRIEND_INFO>(CFriendModel.FriendDataSort));
        }

        private void SortAllGuildMemberList()
        {
            this.m_allGuildMemberList.Sort(new Comparison<GuildMemInfo>(CInviteSystem.GuildMemberComparison));
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_SendInviteFriend, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteFriend));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_SendInviteGuildMember, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteGuildMember));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_SendInviteLBS, new CUIEventManager.OnUIEventHandler(this.OnInvite_SendInviteLBS));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_AcceptInvite, new CUIEventManager.OnUIEventHandler(this.OnInvite_Accept));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_RejectInvite, new CUIEventManager.OnUIEventHandler(this.OnInvite_Reject));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_TimeOut, new CUIEventManager.OnUIEventHandler(this.OnInvateFriendTimeOut));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_FriendListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_FriendListElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_GuildMemberListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_GuildMemberListElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_LBSListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnInvite_LBSListElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_TabChange, new CUIEventManager.OnUIEventHandler(this.OnInvite_TabChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Invite_RefreshGameStateTimeout, new CUIEventManager.OnUIEventHandler(this.OnInvite_RefreshGameStateTimeout));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<byte, string>(EventID.INVITE_ERRCODE_NTF, new Action<byte, string>(this.OnInviteErrCodeNtf));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<CSPkg>("Friend_ADD_NTF", new Action<CSPkg>(this.OnFriendChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<CSPkg>("Friend_Delete_NTF", new Action<CSPkg>(this.OnFriendChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("Chat_Friend_Online_Change", new System.Action(this.OnFriendOnlineChg));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.Friend_Game_State_Change, new System.Action(this.OnFriendOnlineChg));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("Friend_LBS_User_Refresh", new System.Action(this.OnLBSListChg));
        }

        public COM_INVITE_JOIN_TYPE InviteType
        {
            get
            {
                return this.m_inviteType;
            }
        }

        private ListView<COMDT_FRIEND_INFO> m_allFriendList
        {
            get
            {
                if (this.allFriendList_internal == null)
                {
                    this.SortAllFriendList();
                }
                return this.allFriendList_internal;
            }
        }

        public static uint RefreshLBSTime
        {
            get
            {
                if (s_refreshLBSTime == 0)
                {
                    ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0xd8);
                    if (dataByKey == null)
                    {
                        s_refreshLBSTime = 60;
                    }
                    else
                    {
                        s_refreshLBSTime = dataByKey.dwConfValue;
                    }
                }
                return s_refreshLBSTime;
            }
        }

        private enum enInviteState
        {
            None,
            Invited,
            BeRejcet
        }

        private class InviteState
        {
            public CInviteSystem.enInviteState state;
            public uint time;
            public ulong uid;
        }
    }
}

