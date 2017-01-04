namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CGuildInfoController : Singleton<CGuildInfoController>
    {
        private CGuildModel m_Model;
        private CGuildInfoView m_View;

        public void CloseForm()
        {
            this.m_View.CloseForm();
        }

        private void GuildApprove(COMDT_GUILD_MEMBER_BRIEF_INFO info, byte result)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8b1);
            msg.stPkgData.stApproveJoinGuildApply.stApplyInfo = info;
            msg.stPkgData.stApproveJoinGuildApply.bAgree = result;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            this.m_View.RefreshApplyListPanel();
        }

        public override void Init()
        {
            base.Init();
            this.m_Model = Singleton<CGuildModel>.GetInstance();
            this.m_View = Singleton<CGuildInfoView>.GetInstance();
            Singleton<EventRouter>.GetInstance().AddEventHandler<GuildInfo>("Receive_Guild_Info_Success", new Action<GuildInfo>(this.OnReceiveGuildInfoSuccess));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Receive_Guild_Info_Failed", new System.Action(this.OnReceiveGuildInfoFailed));
            Singleton<EventRouter>.GetInstance().AddEventHandler<GuildInfo>("Guild_Create_Or_Add_Success", new Action<GuildInfo>(this.OnCreateOrAddSuccess));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_New_Applicant", new System.Action(this.OnNewApplicant));
            Singleton<EventRouter>.GetInstance().AddEventHandler<RES_GUILD_DONATE_TYPE>("Request_Guild_Donate", new Action<RES_GUILD_DONATE_TYPE>(this.OnRequestGuildDonate));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Get_Guild_Dividend", new System.Action(this.OnGuildGetGuildDividend));
            Singleton<EventRouter>.GetInstance().AddEventHandler<uint>("Request_Guild_Setting_Modify", new Action<uint>(this.OnRequestGuildSettingModify));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Setting_Modify_Success", new System.Action(this.OnGuildSettingModifySuccess));
            Singleton<EventRouter>.GetInstance().AddEventHandler<uint>("Guild_Setting_Modify_Failed", new Action<uint>(this.OnGuildSettingModifyFailed));
            Singleton<EventRouter>.GetInstance().AddEventHandler<uint>("Guild_Setting_Modify_Icon", new Action<uint>(this.OnGuildSettingModifyIcon));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Setting_Modify_Icon_Success", new System.Action(this.OnGuildSettingModifyIconSuccess));
            Singleton<EventRouter>.GetInstance().AddEventHandler<string>("Guild_Modify_Bulletin", new Action<string>(this.OnGuildModifyBulletin));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Modify_Bulletin_Success", new System.Action(this.OnGuildModifyBulletinSuccess));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Request_Extend_Member_Limit", new System.Action(this.OnGuildRequestExtendMemberLimit));
            Singleton<EventRouter>.GetInstance().AddEventHandler<ulong, byte>("Guild_Approve", new Action<ulong, byte>(this.OnGuildApprove));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_New_Member", new System.Action(this.OnNewMember));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Request_Apply_List", new System.Action(this.OnRequestApplyList));
            Singleton<EventRouter>.GetInstance().AddEventHandler<List<stApplicantInfo>, uint, byte, byte>("Receive_Apply_List_Success", new Action<List<stApplicantInfo>, uint, byte, byte>(this.OnReceiveApplyListSuccess));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Quit", new System.Action(this.OnGuildQuit));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Quit_Success", new System.Action(this.OnGuildQuitSuccess));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Quit_Failed", new System.Action(this.OnGuildQuitFailed));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Member_Quit", new System.Action(this.OnMemberQuit));
            Singleton<EventRouter>.GetInstance().AddEventHandler<ulong>("Guild_Apply_Time_Up", new Action<ulong>(this.OnGuildApplyTimeUp));
            Singleton<EventRouter>.GetInstance().AddEventHandler<ulong>("Guild_Invite", new Action<ulong>(this.OnGuildInvite));
            Singleton<EventRouter>.GetInstance().AddEventHandler<ulong>("Guild_Recommend", new Action<ulong>(this.OnGuildRecommend));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Request_Recommend_List", new System.Action(this.OnRequestRecommendList));
            Singleton<EventRouter>.GetInstance().AddEventHandler<List<stRecommendInfo>, uint, byte, byte>("Receive_Recommend_List_Success", new Action<List<stRecommendInfo>, uint, byte, byte>(this.OnReceiveRecommendListSuccess));
            Singleton<EventRouter>.GetInstance().AddEventHandler<ulong>("Guild_Reject_Recommend", new Action<ulong>(this.OnGuildRejectRecommend));
            Singleton<EventRouter>.GetInstance().AddEventHandler<int>("Guild_Building_Upgrade", new Action<int>(this.OnGuildBuildingUpgrade));
            Singleton<EventRouter>.GetInstance().AddEventHandler<ulong, byte, ulong>("Guild_Position_Appoint", new Action<ulong, byte, ulong>(this.OnGuildPositionAppoint));
            Singleton<EventRouter>.GetInstance().AddEventHandler<ulong>("Guild_Position_Confirm_Fire_Member", new Action<ulong>(this.OnGuildPositionConfirmFireMember));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Position_Confirm_Recommend_Self_As_Chairman", new System.Action(this.OnGuildPositionConfirmRecommendSelfAsChairman));
            Singleton<EventRouter>.GetInstance().AddEventHandler<ulong, bool>("Guild_Position_Deal_Self_Recommend", new Action<ulong, bool>(this.OnGuildPositionDealSelfRecommend));
            Singleton<EventRouter>.GetInstance().AddEventHandler<enGuildRankpointRankListType, bool>("Guild_Request_Rankpoint_Rank_List", new Action<enGuildRankpointRankListType, bool>(this.OnGuildRankpointRequestRankList));
            Singleton<EventRouter>.GetInstance().AddEventHandler<int, int>("Guild_Preview_Get_Ranking_List", new Action<int, int>(this.OnGuildPreviewRequestRankingList));
            Singleton<EventRouter>.GetInstance().AddEventHandler<ListView<GuildInfo>, bool>("Receive_Guild_List_Success", new Action<ListView<GuildInfo>, bool>(this.OnReceiveGuildListSuccess));
            Singleton<EventRouter>.GetInstance().AddEventHandler<GuildInfo, int>("Receive_Guild_Search_Success", new Action<GuildInfo, int>(this.OnReceiveGuildSearchSuccess));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Accept_Invite", new System.Action(this.OnAcceptInvite));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Hyperlink_Search_Guild, new CUIEventManager.OnUIEventHandler(this.OnHyperLinkSearchGuild));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Guild_Hyperlink_Search_PrepareGuild, new CUIEventManager.OnUIEventHandler(this.OnHyperLinkSearchPrepareGuild));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_Sign", new System.Action(this.OnGuildSignIn));
            Singleton<EventRouter>.GetInstance().AddEventHandler<bool>("Guild_Sign_State_Changed", new Action<bool>(this.OnGuildSignStateChanged));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_QQGroup_Refresh_QQGroup_Panel", new System.Action(this.OnRefreshQQGroupPanel));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Guild_QQGroup_Request_Group_Guild_Id", new System.Action(this.OnRequestGroupGuildId));
            Singleton<EventRouter>.GetInstance().AddEventHandler<string>("Guild_QQGroup_Set_Guild_Group_Open_Id", new Action<string>(this.OnSetGuildGroupOpenId));
            Singleton<EventRouter>.GetInstance().AddEventHandler("MasterPvpLevelChanged", new System.Action(this.OnMasterPvpLevelChanged));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.NAMECHANGE_GUILD_NAME_CHANGE, new System.Action(this.OnGuildNameChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.GLOBAL_REFRESH_TIME, new System.Action(this.OnGlobalRefreshTime));
        }

        private void InviteFriend(string name, ulong uuid)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8b5);
            msg.stPkgData.stGuildInviteReq.ullBeInviteUid = uuid;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void OnAcceptInvite()
        {
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState != COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_No_Guild_Can_Accept_Invite_Tip", true, 1.5f, null, new object[0]);
            }
            else if (!CGuildHelper.IsInLastQuitGuildCd() && (this.m_Model.m_InviteGuildUuid != 0))
            {
                this.RequestDealInviteReq(1);
            }
        }

        public void OnCreateOrAddSuccess(GuildInfo info)
        {
            this.m_Model.CurrentGuildInfo = info;
            if (Singleton<CGuildListView>.GetInstance().IsShow())
            {
                Singleton<CGuildListView>.GetInstance().CloseForm();
                this.m_View.OpenForm();
            }
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Guild/Form_Guild_Preview.prefab");
            if (form != null)
            {
                form.Close();
                this.m_View.OpenForm();
            }
            string[] args = new string[] { info.stBriefInfo.sName };
            Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Guild_Join_Success_Tip", args), false, 1.5f, null, new object[0]);
        }

        private void OnGlobalRefreshTime()
        {
            if (this.m_View != null)
            {
                this.m_View.RefreshInfoPanelSignBtn();
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent<bool>("Guild_Sign_State_Changed", false);
            CGuildHelper.SetSendGuildMailCnt(0);
        }

        public void OnGuildApplyTimeUp(ulong uuid)
        {
            this.m_Model.RemoveApplicant(uuid);
        }

        public void OnGuildApprove(ulong uulUid, byte result)
        {
            stApplicantInfo applicantByUid = this.m_Model.GetApplicantByUid(uulUid);
            COMDT_GUILD_MEMBER_BRIEF_INFO info = new COMDT_GUILD_MEMBER_BRIEF_INFO();
            if (applicantByUid.stBriefInfo.uulUid == 0)
            {
                this.m_View.RefreshApplyListPanel();
            }
            else
            {
                this.m_Model.RemoveApplicant(applicantByUid.stBriefInfo.uulUid);
                info.dwGameEntity = applicantByUid.stBriefInfo.dwGameEntity;
                StringHelper.StringToUTF8Bytes(applicantByUid.stBriefInfo.szHeadUrl, ref info.szHeadUrl);
                info.dwLevel = applicantByUid.stBriefInfo.dwLevel;
                info.dwAbility = applicantByUid.stBriefInfo.dwAbility;
                info.iLogicWorldID = applicantByUid.stBriefInfo.dwLogicWorldId;
                StringHelper.StringToUTF8Bytes(applicantByUid.stBriefInfo.sName, ref info.szName);
                info.ullUid = applicantByUid.stBriefInfo.uulUid;
                info.stVip.dwScore = applicantByUid.stBriefInfo.stVip.score;
                info.stVip.dwCurLevel = applicantByUid.stBriefInfo.stVip.level;
                info.stVip.dwHeadIconId = applicantByUid.stBriefInfo.stVip.headIconId;
                this.GuildApprove(info, result);
            }
        }

        public void OnGuildBuildingUpgrade(int buildingType)
        {
            this.SendBuildingUpgradeReq(buildingType);
        }

        public void OnGuildGetGuildDividend()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8d6);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void OnGuildInvite(ulong uuid)
        {
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            this.InviteFriend(this.m_Model.CurrentGuildInfo.stBriefInfo.sName, uuid);
        }

        private void OnGuildModifyBulletin(string bulletinText)
        {
            this.RequestChgGuildNotice(bulletinText);
        }

        private void OnGuildModifyBulletinSuccess()
        {
            this.m_View.RefreshInfoPanelGuildBulletin();
        }

        private void OnGuildNameChange()
        {
            this.m_Model.CurrentGuildInfo.stBriefInfo.sName = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.name;
            this.m_View.RefreshGuildName();
        }

        public void OnGuildPositionAppoint(ulong uid, byte position, ulong replaceUid)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CPlayerInfoSystem>.GetInstance().sPlayerInfoFormPath);
            if (form != null)
            {
                form.Close();
            }
            this.RequestGuildAppointPosition(uid, position, replaceUid);
        }

        public void OnGuildPositionConfirmFireMember(ulong uid)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CPlayerInfoSystem>.GetInstance().sPlayerInfoFormPath);
            if (form != null)
            {
                form.Close();
            }
            this.RequestGuildFireMember(uid);
        }

        public void OnGuildPositionConfirmRecommendSelfAsChairman()
        {
            this.RequestRecommendSelfAsChairman();
        }

        public void OnGuildPositionDealSelfRecommend(ulong uid, bool isAgree)
        {
            this.RequestDealSelfRecommend(uid, isAgree);
        }

        private void OnGuildPreviewRequestRankingList(int start, int limit)
        {
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa2a);
            msg.stPkgData.stGetRankingListReq.bNumberType = 3;
            msg.stPkgData.stGetRankingListReq.iImageFlag = 0;
            msg.stPkgData.stGetRankingListReq.iStart = start;
            msg.stPkgData.stGetRankingListReq.iLimit = limit;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void OnGuildQuit()
        {
            this.m_Model.SetPlayerGuildStateToTemp();
            this.RequestQuitGuild();
        }

        public void OnGuildQuitFailed()
        {
        }

        public void OnGuildQuitSuccess()
        {
            this.m_View.CloseForm();
        }

        public void OnGuildRankpointRequestRankList(enGuildRankpointRankListType rankListType, bool isNeedOpenRankpointFormWhenNotRequest)
        {
            if (CGuildHelper.IsNeedRequestNewRankpoinRank(rankListType))
            {
                switch (rankListType)
                {
                    case enGuildRankpointRankListType.CurrentWeek:
                        this.RequestGetRankpointWeekRank(0);
                        break;

                    case enGuildRankpointRankListType.LastWeek:
                        this.RequestGetRankpointWeekRank(1);
                        break;

                    case enGuildRankpointRankListType.SeasonSelf:
                        this.RequestGetRankpointSeasonRankBySpecialScore();
                        this.RequestGetPlayerGuildRankInfo();
                        break;

                    case enGuildRankpointRankListType.SeasonBest:
                        this.RequestGetRankpointSeasonRank();
                        this.RequestGetPlayerGuildRankInfo();
                        break;
                }
            }
            else if (isNeedOpenRankpointFormWhenNotRequest)
            {
                this.m_View.OpenRankpointForm();
            }
        }

        public void OnGuildRecommend(ulong uuid)
        {
            this.SendRecommendReq(uuid);
        }

        public void OnGuildRejectRecommend(ulong uuid)
        {
            this.SendRejectRecommend(uuid);
            this.m_Model.RemoveRecommendInfo(uuid);
            this.m_View.RefreshApplyListPanel();
        }

        private void OnGuildRequestExtendMemberLimit()
        {
            this.RequestUpgradeGuildByDianQuan(this.m_Model.CurrentGuildInfo.stBriefInfo.bLevel);
        }

        public void OnGuildSettingModifyFailed(uint mask)
        {
        }

        private void OnGuildSettingModifyIcon(uint iconId)
        {
            this.RequestChgGuildHeadIdReq(iconId);
        }

        private void OnGuildSettingModifyIconSuccess()
        {
            this.m_View.RefreshInfoPanelGuildIcon();
        }

        public void OnGuildSettingModifySuccess()
        {
        }

        private void OnGuildSignIn()
        {
            this.RequestGuildSignIn();
        }

        private void OnGuildSignStateChanged(bool isSigned)
        {
            CGuildHelper.SetPlayerSigned(isSigned);
            this.m_View.RefreshInfoPanelSignBtn();
        }

        public void OnHyperLinkSearchGuild(CUIEvent uiEvent)
        {
            if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Already_In_Normal_Guild", true, 1.5f, null, new object[0]);
            }
            else if (Singleton<CGuildSystem>.GetInstance().IsInPrepareGuild())
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Already_In_Prepare_Guild", true, 1.5f, null, new object[0]);
            }
            else
            {
                this.m_Model.m_InvitePlayerUuid = uiEvent.m_eventParams.commonUInt64Param2;
                this.m_Model.m_InviteGuildUuid = uiEvent.m_eventParams.commonUInt64Param1;
                Singleton<CGuildSystem>.GetInstance().SearchGuild(this.m_Model.m_InviteGuildUuid, string.Empty, 1, false);
            }
        }

        public void OnHyperLinkSearchPrepareGuild(CUIEvent uiEvent)
        {
            if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Already_In_Normal_Guild", true, 1.5f, null, new object[0]);
            }
            else if (Singleton<CGuildSystem>.GetInstance().IsInPrepareGuild())
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Guild_Already_In_Prepare_Guild", true, 1.5f, null, new object[0]);
            }
            else
            {
                this.m_Model.m_InvitePlayerUuid = uiEvent.m_eventParams.commonUInt64Param2;
                this.m_Model.m_InviteGuildUuid = uiEvent.m_eventParams.commonUInt64Param1;
                Singleton<CGuildSystem>.GetInstance().SearchGuild(this.m_Model.m_InviteGuildUuid, string.Empty, 1, true);
            }
        }

        private void OnMasterPvpLevelChanged()
        {
            if (((this.m_Model != null) && (this.m_Model.CurrentGuildInfo != null)) && (this.m_Model.CurrentGuildInfo.listMemInfo != null))
            {
                for (int i = 0; i < this.m_Model.CurrentGuildInfo.listMemInfo.Count; i++)
                {
                    if (this.m_Model.CurrentGuildInfo.listMemInfo[i].stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
                    {
                        this.m_Model.CurrentGuildInfo.listMemInfo[i].stBriefInfo.dwLevel = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().PvpLevel;
                        return;
                    }
                }
            }
        }

        public void OnMemberQuit()
        {
            this.m_View.RefreshMemberPanel();
        }

        public void OnNewApplicant()
        {
            this.m_View.RefreshApplyListPanel();
        }

        public void OnNewMember()
        {
            this.m_View.RefreshMemberPanel();
        }

        public void OnReceiveApplyListSuccess(List<stApplicantInfo> applicantList, uint totalCnt, byte pageId, byte thisCnt)
        {
            this.m_Model.AddApplicants(applicantList);
            if (CGuildHelper.IsLastPage(pageId, totalCnt, 10))
            {
                this.m_Model.IsApplyAndRecommendListLastPage = true;
                this.m_View.RefreshApplyListPanel();
            }
            else
            {
                if (thisCnt == 10)
                {
                    this.m_Model.RequestApplyListPageId = pageId + 1;
                }
                else
                {
                    this.m_Model.RequestApplyListPageId = pageId;
                }
                this.m_Model.IsApplyAndRecommendListLastPage = false;
                this.m_View.RefreshApplyListPanel();
            }
        }

        public void OnReceiveGuildInfoFailed()
        {
        }

        public void OnReceiveGuildInfoSuccess(GuildInfo info)
        {
            this.m_Model.CurrentGuildInfo = info;
        }

        private void OnReceiveGuildListSuccess(ListView<GuildInfo> guildList, bool firstPage)
        {
            if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
            {
                if (firstPage)
                {
                    this.m_Model.ClearGuildInfoList();
                }
                this.m_Model.AddGuildInfoList(guildList);
                this.m_View.OpenGuildPreviewForm(false);
            }
        }

        private void OnReceiveGuildSearchSuccess(GuildInfo info, int searchType)
        {
            if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
            {
                this.m_Model.ClearGuildInfoList();
                this.m_Model.AddGuildInfo(info);
                this.m_View.RefreshPreviewForm(true);
            }
        }

        public void OnReceiveRecommendListSuccess(List<stRecommendInfo> recommendList, uint totalCnt, byte pageId, byte thisCnt)
        {
            this.m_Model.AddRecommendInfoList(recommendList);
            this.m_Model.IsApplyAndRecommendListLastPage = false;
            if (CGuildHelper.IsLastPage(pageId, totalCnt, 10))
            {
                this.m_Model.IsRequestApplyList = true;
                this.m_Model.RequestApplyListPageId = 0;
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Request_Apply_List");
            }
            else
            {
                if (thisCnt == 10)
                {
                    this.m_Model.RequestRecommendListPageId = pageId + 1;
                }
                else
                {
                    this.m_Model.RequestRecommendListPageId = pageId;
                }
                this.m_View.RefreshApplyListPanel();
            }
        }

        private void OnRefreshQQGroupPanel()
        {
            if (this.m_Model.CurrentGuildInfo.groupGuildId == 0)
            {
                this.m_View.RefreshQQGroupPanel(false, string.Empty, false);
            }
            else
            {
                this.m_View.QueryQQGroupInfo(0);
            }
        }

        public void OnRequestApplyList()
        {
            this.RequestApplyList(this.m_Model.RequestApplyListPageId);
        }

        private void OnRequestGroupGuildId()
        {
            this.RequestGroupGuildId();
        }

        public void OnRequestGuildDonate(RES_GUILD_DONATE_TYPE donateType)
        {
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8d4);
            msg.stPkgData.stGuildDonateReq.bType = (byte) donateType;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void OnRequestGuildSettingModify(uint mask)
        {
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            this.RequestGuildSettingModify(mask);
        }

        public void OnRequestRecommendList()
        {
            this.RequestRecommendList(this.m_Model.RequestRecommendListPageId);
        }

        private void OnSetGuildGroupOpenId(string groupOpenId)
        {
            this.RequestSetGuildGroupOpenId(groupOpenId);
        }

        public void OpenForm()
        {
            this.m_View.OpenForm();
        }

        public void ReqSendGuildMail(string title, string content)
        {
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x596);
            StringHelper.StringToUTF8Bytes(title, ref msg.stPkgData.stSendGuildMailReq.szSubject);
            StringHelper.StringToUTF8Bytes(content, ref msg.stPkgData.stSendGuildMailReq.szContent);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public void RequestApplyList(int page)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8ab);
            msg.stPkgData.stGetGuildApplyListReq.bPageId = (byte) page;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestChgGuildHeadIdReq(uint guildHeadId)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8e0);
            msg.stPkgData.stChgGuildHeadIDReq.dwHeadID = guildHeadId;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestChgGuildNotice(string bulletinText)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8e2);
            StringHelper.StringToUTF8Bytes(bulletinText, ref msg.stPkgData.stChgGuildNoticeReq.szNotice);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestDealInviteReq(byte agree)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8b9);
            msg.stPkgData.stDealGuildInvite.bAgree = agree;
            msg.stPkgData.stDealGuildInvite.ullInviteUid = this.m_Model.m_InvitePlayerUuid;
            msg.stPkgData.stDealGuildInvite.ullGuildID = this.m_Model.m_InviteGuildUuid;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestDealSelfRecommend(ulong uid, bool isAgree)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8d2);
            msg.stPkgData.stDealSelfRecommemdReq.ullUid = uid;
            msg.stPkgData.stDealSelfRecommemdReq.bAgree = !isAgree ? ((byte) 0) : ((byte) 1);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestGetPlayerGuildRankInfo()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa37);
            msg.stPkgData.stGetSpecialGuildRankInfoReq.bNumberType = 0x10;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestGetRankpointSeasonRank()
        {
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa2a);
            msg.stPkgData.stGetRankingListReq.bNumberType = 0x10;
            msg.stPkgData.stGetRankingListReq.iStart = 1;
            msg.stPkgData.stGetRankingListReq.iLimit = 100;
            msg.stPkgData.stGetRankingListReq.iImageFlag = 0;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestGetRankpointSeasonRankBySpecialScore()
        {
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa35);
            msg.stPkgData.stGetRankListBySpecialScoreReq.bNumberType = 0x10;
            msg.stPkgData.stGetRankListBySpecialScoreReq.iScore = (int) this.m_Model.CurrentGuildInfo.RankInfo.totalRankPoint;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestGetRankpointWeekRank(int imageFlag)
        {
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa2a);
            msg.stPkgData.stGetRankingListReq.bNumberType = 4;
            msg.stPkgData.stGetRankingListReq.iStart = 1;
            msg.stPkgData.stGetRankingListReq.iLimit = 100;
            msg.stPkgData.stGetRankingListReq.iImageFlag = imageFlag;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestGroupGuildId()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8e9);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            Debug.Log("Request 32 bit guild id");
        }

        private void RequestGuildAppointPosition(ulong uid, byte position, ulong replaceUid)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8c9);
            msg.stPkgData.stAppointPositionReq.ullUid = uid;
            msg.stPkgData.stAppointPositionReq.bPosition = position;
            msg.stPkgData.stAppointPositionReq.ullReplaceUid = replaceUid;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestGuildFireMember(ulong uid)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8cc);
            msg.stPkgData.stFireGuildMemberReq.ullUid = uid;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void RequestGuildInfo()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x89d);
            msg.stPkgData.stGetGuildInfoReq.ullGuildID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.uulUid;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestGuildSettingModify(uint mask)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8a9);
            msg.stPkgData.stModifyGuildSettingReq.dwSettingMask = mask;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestGuildSignIn()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8e6);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestQuitGuild()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8b2);
            msg.stPkgData.stQuitGuildReq.bBlank = 0;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public void RequestRecommendList(int page)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8bd);
            msg.stPkgData.stGetGuildRecommendListReq.bPageId = (byte) page;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestRecommendSelfAsChairman()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8cf);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void RequestSetGuildGroupOpenId(string groupOpenId)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8eb);
            StringHelper.StringToUTF8Bytes(groupOpenId, ref msg.stPkgData.stSetGuildGroupOpenIDReq.szGroupOpenID);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            Debug.Log("Send groupOpenId to server, groupOpenId=" + groupOpenId);
        }

        private void RequestUpgradeGuildByDianQuan(uint curGuildLevel)
        {
            Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8e4);
            msg.stPkgData.stUpgradeGuildByCouponsReq.dwCurLevel = curGuildLevel;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private void SendBuildingUpgradeReq(int buildingType)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8c3);
            msg.stPkgData.stGuildBuildingUpgradeReq.bBuildingType = (byte) buildingType;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void SendRecommendReq(ulong uuid)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8ba);
            msg.stPkgData.stGuildRecommendReq.ullAcntUid = uuid;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void SendRejectRecommend(ulong uuid)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x8bf);
            msg.stPkgData.stRejectGuildRecommend.ullPlayerUid = uuid;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }
    }
}

