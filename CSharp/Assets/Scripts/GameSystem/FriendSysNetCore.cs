namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    [MessageHandlerClass]
    public class FriendSysNetCore
    {
        [MessageHandler(0x4b7)]
        public static void On_SC_BeFriend(CSPkg msg)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_RequestBeFriend", msg);
        }

        [MessageHandler(0x4bb)]
        public static void On_SC_Confrim_BeFriend(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_Confrim", msg);
            Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_Friend_Online_Change");
        }

        [MessageHandler(0x4b9)]
        public static void On_SC_Del_Friend(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_Delete", msg);
            Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_Friend_Online_Change");
        }

        [MessageHandler(0x4bd)]
        public static void On_SC_DENY_BeFriend(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_Deny", msg);
        }

        [MessageHandler(0x4b1)]
        public static void On_SC_Friend_Info(CSPkg msg)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_List", msg);
        }

        [MessageHandler(0x4d3)]
        public static void On_SC_Friend_Recommand_Info(CSPkg msg)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_Recommand_List", msg);
        }

        [MessageHandler(0x4b3)]
        public static void On_SC_Friend_Request_Info(CSPkg msg)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_Request_List", msg);
        }

        public static void On_SC_Invite_Friend_Game(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
        }

        [MessageHandler(0x4cf)]
        public static void On_SC_NTF_FRIEND_ADD(CSPkg msg)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_ADD_NTF", msg);
            Singleton<EventRouter>.GetInstance().BroadCastEvent("Chat_Friend_Online_Change");
        }

        [MessageHandler(0x4d0)]
        public static void On_SC_NTF_FRIEND_DEL(CSPkg msg)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_Delete_NTF", msg);
        }

        [MessageHandler(0x4ce)]
        public static void On_SC_NTF_FRIEND_REQUEST(CSPkg msg)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_Request_NTF", msg);
        }

        [MessageHandler(0x533)]
        public static void On_SC_Search_LBS_Rsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_LBS_SEARCH stLbsSearchRsq = msg.stPkgData.stLbsSearchRsq;
            if (stLbsSearchRsq.dwResult == 0)
            {
                Singleton<CFriendContoller>.instance.model.ClearLBSData();
                for (int i = 0; i < stLbsSearchRsq.dwLbsListNum; i++)
                {
                    CSDT_LBS_USER_INFO info = stLbsSearchRsq.astLbsList[i];
                    if ((info != null) && (info.stLbsUserInfo.stUin.ullUid != Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID))
                    {
                        Singleton<CFriendContoller>.instance.model.RemoveLBSUser(info.stLbsUserInfo.stUin.ullUid, info.stLbsUserInfo.stUin.dwLogicWorldId);
                        Singleton<CFriendContoller>.instance.model.AddLBSUser(info);
                    }
                }
                if (stLbsSearchRsq.dwLbsListNum == 0)
                {
                    string text = Singleton<CTextManager>.instance.GetText("LBS_Location_Error");
                    Singleton<CFriendContoller>.instance.model.searchLBSZero = text;
                    if ((Singleton<CFriendContoller>.instance.view != null) && (Singleton<CFriendContoller>.instance.view.ifnoText != null))
                    {
                        Singleton<CFriendContoller>.instance.view.ifnoText.text = text;
                    }
                }
                else
                {
                    Singleton<CFriendContoller>.instance.model.searchLBSZero = string.Empty;
                }
                Singleton<CFriendContoller>.instance.model.SortLBSFriend();
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Friend_LBS_User_Refresh");
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stLbsSearchRsq.dwResult), false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x4b5)]
        public static void On_SC_Serch_Player(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_Search", msg);
        }

        [MessageHandler(0x4d7)]
        public static void On_SCID_CMD_BLACKLIST(CSPkg msg)
        {
            Singleton<CFriendContoller>.instance.On_SCID_CMD_BLACKLIST(msg);
        }

        [MessageHandler(0x1004)]
        public static void On_SCID_CMD_NTF_FRIEND_GAME_STATE(CSPkg msg)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_GAME_STATE_NTF", msg);
        }

        [MessageHandler(0x1005)]
        public static void On_SCID_NTF_SNS_FRIEND(CSPkg msg)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_SNS_STATE_NTF", msg);
        }

        [MessageHandler(0x1008)]
        public static void On_SCID_NTF_SNS_FRIEND_NICKNAME(CSPkg msg)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_SNS_NICNAME_NTF", msg);
        }

        [MessageHandler(0x1006)]
        public static void On_SCID_SNS_FRIEND_CHG_PROFILE(CSPkg msg)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_SNS_CHG_PROFILE", msg);
        }

        [MessageHandler(0x4d6)]
        public static void On_SCID_SNS_FRIEND_RECALL_NTF(CSPkg msg)
        {
            Singleton<CFriendContoller>.instance.OnReCallFriendNtf(msg);
        }

        [MessageHandler(0x4d5)]
        public static void On_SCID_SNS_FRIEND_RECALLPOINT(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_RECALL_FRIEND stFriendRecallRsp = msg.stPkgData.stFriendRecallRsp;
            if (stFriendRecallRsp.dwResult == 0)
            {
                UT.Check_AddReCallCD(stFriendRecallRsp.stUin, (COM_FRIEND_TYPE) stFriendRecallRsp.bFriendType);
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Friend_FriendList_Refresh");
                Singleton<CUIManager>.GetInstance().OpenTips(UT.GetText("Common_Sns_Tips_10"), false, 1.5f, null, new object[0]);
                COMDT_FRIEND_INFO comdt_friend_info = Singleton<CFriendContoller>.instance.model.getFriendByUid(stFriendRecallRsp.stUin.ullUid, CFriendModel.FriendType.SNS);
                if (comdt_friend_info != null)
                {
                    Singleton<CFriendContoller>.instance.ShareTo_SNSFriend_ReCall(Utility.UTF8Convert(comdt_friend_info.szOpenId));
                }
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stFriendRecallRsp.dwResult), false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x530)]
        public static void On_SCPKG_CMD_CANCEL_DEFRIEND(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<CFriendContoller>.instance.OnSCPKG_CMD_CANCEL_DEFRIEND(msg);
        }

        [MessageHandler(0x52e)]
        public static void On_SCPKG_CMD_DEFRIEND(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            Singleton<CFriendContoller>.instance.OnSCPKG_CMD_DEFRIEND(msg);
        }

        [MessageHandler(0x4c8)]
        public static void On_SCPKG_CMD_NTF_CHG_INTIMACY(CSPkg msg)
        {
            Singleton<CFriendContoller>.instance.OnChangeIntimacy(msg);
        }

        [MessageHandler(0x4c3)]
        public static void On_Send_FriendPower(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_DONATE_FRIEND_POINT stFriendDonatePointRsp = msg.stPkgData.stFriendDonatePointRsp;
            if (stFriendDonatePointRsp.dwResult == 0)
            {
                UT.Check_AddHeartCD(stFriendDonatePointRsp.stUin, (COM_FRIEND_TYPE) stFriendDonatePointRsp.bFriendType);
                Singleton<EventRouter>.GetInstance().BroadCastEvent("Friend_FriendList_Refresh");
                Singleton<EventRouter>.GetInstance().BroadCastEvent<SCPKG_CMD_DONATE_FRIEND_POINT>("Friend_Send_Coin_Done", stFriendDonatePointRsp);
                if (Singleton<CFriendContoller>.instance.model.IsSnsFriend(stFriendDonatePointRsp.stUin.ullUid, stFriendDonatePointRsp.stUin.dwLogicWorldId))
                {
                    COMDT_FRIEND_INFO info = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.SNS, stFriendDonatePointRsp.stUin);
                    if (info != null)
                    {
                        string str = Utility.UTF8Convert(info.szOpenId);
                        if (!string.IsNullOrEmpty(str) && !CFriendModel.IsOnSnsSwitch(info.dwRefuseFriendBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_DONOTE_AND_REC))
                        {
                            string text = Singleton<CTextManager>.instance.GetText("Common_Sns_Tips_3");
                            string confirmStr = Singleton<CTextManager>.instance.GetText("Common_Sns_Tips_5");
                            string cancelStr = Singleton<CTextManager>.instance.GetText("Common_Sns_Tips_4");
                            stUIEventParams param = new stUIEventParams();
                            param.snsFriendEventParams.openId = str;
                            Singleton<CUIManager>.instance.OpenMessageBoxWithCancel(text, enUIEventID.Friend_Share_SendCoin, enUIEventID.None, param, confirmStr, cancelStr, false);
                            return;
                        }
                    }
                }
                Singleton<CUIManager>.GetInstance().OpenTips(UT.GetText("Friend_Tips_SendHeartOK"), false, 1.5f, null, new object[0]);
            }
            else
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stFriendDonatePointRsp.dwResult), false, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x52b)]
        public static void On_Sns_Switch_Ntf(CSPkg msg)
        {
            Singleton<CFriendContoller>.instance.OnSnsSwitchNtf(msg);
        }

        [MessageHandler(0x52c)]
        public static void On_Sns_Switch_Rep(CSPkg msg)
        {
            Singleton<CUIManager>.instance.CloseSendMsgAlert();
            Singleton<CFriendContoller>.instance.OnSnsSwitchRsp(msg);
        }

        [MessageHandler(0x4d1)]
        public static void OnSCID_CMD_NTF_FRIEND_LOGIN_STATUS(CSPkg msg)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Friend_Login_NTF", msg);
            SCPKG_CMD_NTF_FRIEND_LOGIN_STATUS stNtfFriendLoginStatus = msg.stPkgData.stNtfFriendLoginStatus;
            CFriendModel.FriendType type = (stNtfFriendLoginStatus.bFriendType != 1) ? CFriendModel.FriendType.SNS : CFriendModel.FriendType.GameFriend;
            bool flag = stNtfFriendLoginStatus.bLoginStatus == 0;
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CFriendModel.FriendType, ulong, uint, bool>("Chat_Friend_Online_Change", type, stNtfFriendLoginStatus.stUin.ullUid, stNtfFriendLoginStatus.stUin.dwLogicWorldId, flag);
        }

        public static void ReCallSnsFriend(COMDT_ACNT_UNIQ uniq, COM_FRIEND_TYPE friendType)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x4d4);
            msg.stPkgData.stFriendRecallReq.stUin.ullUid = uniq.ullUid;
            msg.stPkgData.stFriendRecallReq.stUin.dwLogicWorldId = uniq.dwLogicWorldId;
            msg.stPkgData.stFriendRecallReq.bFriendType = (byte) friendType;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void Send_Block(ulong ullUid, uint dwLogicWorldId)
        {
            if (ullUid != 0)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x52d);
                msg.stPkgData.stDeFriendReq.stUin.ullUid = ullUid;
                msg.stPkgData.stDeFriendReq.stUin.dwLogicWorldId = dwLogicWorldId;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        public static void Send_Cancle_Block(ulong ullUid, uint dwLogicWorldId)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x52f);
            msg.stPkgData.stCancelDeFriendReq.stUin.ullUid = ullUid;
            msg.stPkgData.stCancelDeFriendReq.stUin.dwLogicWorldId = dwLogicWorldId;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void Send_Clear_Location()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x534);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void Send_Confrim_BeFriend(COMDT_ACNT_UNIQ uniq)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x4ba);
            msg.stPkgData.stFriendAddConfirmReq.stUin.ullUid = uniq.ullUid;
            msg.stPkgData.stFriendAddConfirmReq.stUin.dwLogicWorldId = uniq.dwLogicWorldId;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void Send_Del_Friend(COMDT_ACNT_UNIQ uniq)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x4b8);
            msg.stPkgData.stFriendDelReq.stUin.ullUid = uniq.ullUid;
            msg.stPkgData.stFriendDelReq.stUin.dwLogicWorldId = uniq.dwLogicWorldId;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void Send_DENY_BeFriend(COMDT_ACNT_UNIQ uniq)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x4bc);
            msg.stPkgData.stFriendAddDenyReq.stUin.ullUid = uniq.ullUid;
            msg.stPkgData.stFriendAddDenyReq.stUin.dwLogicWorldId = uniq.dwLogicWorldId;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void Send_FriendCoin(COMDT_ACNT_UNIQ uniq, COM_FRIEND_TYPE friendType)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x4c2);
            msg.stPkgData.stFriendDonatePointReq.stUin.ullUid = uniq.ullUid;
            msg.stPkgData.stFriendDonatePointReq.stUin.dwLogicWorldId = uniq.dwLogicWorldId;
            msg.stPkgData.stFriendDonatePointReq.bFriendType = (byte) friendType;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void Send_Invite_Friend_Game(int bReserve)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x4be);
            msg.stPkgData.stFriendInviteGameReq.bReserve = (byte) bReserve;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void Send_Report_Clt_Location(int longitude, int latitude)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x531);
            msg.stPkgData.stLbsReportReq.iLongitude = longitude;
            msg.stPkgData.stLbsReportReq.iLatitude = latitude;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public static void Send_Request_BeFriend(COMDT_ACNT_UNIQ uniq, string veriyText = "")
        {
            Send_Request_BeFriend(uniq.ullUid, uniq.dwLogicWorldId, veriyText);
        }

        public static void Send_Request_BeFriend(ulong ullUid, uint dwLogicWorldId, string veriyText = "")
        {
            if (Singleton<CFriendContoller>.GetInstance().model.IsContain(CFriendModel.FriendType.GameFriend, ullUid, dwLogicWorldId))
            {
                Singleton<CUIManager>.GetInstance().OpenTips(UT.GetText("Friend_Tips_AlreadyBeFriend"), false, 1.5f, null, new object[0]);
            }
            else
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x4b6);
                msg.stPkgData.stFriendAddReq.stUin.ullUid = ullUid;
                msg.stPkgData.stFriendAddReq.stUin.dwLogicWorldId = dwLogicWorldId;
                if (!string.IsNullOrEmpty(veriyText))
                {
                    StringHelper.StringToUTF8Bytes(veriyText, ref msg.stPkgData.stFriendAddReq.szVerificationInfo);
                }
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            }
        }

        public static void Send_Request_RecommandFriend_List()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x4d2);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public static void Send_Request_Sns_Switch(COM_REFUSE_TYPE type, int tag)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x52a);
            msg.stPkgData.stRefuseRecallReq.bRefuseStatus = (byte) tag;
            msg.stPkgData.stRefuseRecallReq.bRefuseType = (byte) type;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void Send_Search_LBS_Req(byte gender, int longitude, int latitude, bool isShowAlert = true)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x532);
            msg.stPkgData.stLbsSearchReq.bGender = gender;
            msg.stPkgData.stLbsSearchReq.iLongitude = longitude;
            msg.stPkgData.stLbsSearchReq.iLatitude = latitude;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, isShowAlert);
        }

        public static void Send_Serch_Player(string name)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x4b4);
            msg.stPkgData.stFriendSearchPlayerReq.szUserName = Encoding.UTF8.GetBytes(name);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }
    }
}

