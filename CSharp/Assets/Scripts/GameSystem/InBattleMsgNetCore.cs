namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using System;

    [MessageHandlerClass]
    public class InBattleMsgNetCore
    {
        [MessageHandler(0x1469)]
        public static void OnSendShortCut_Config_Rsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_SELFDEFINE_CHATINFO_CHG_RSP stSelfDefineChatInfoChgRsp = msg.stPkgData.stSelfDefineChatInfoChgRsp;
            if (stSelfDefineChatInfoChgRsp.bResult == 0)
            {
                Singleton<CUIManager>.instance.OpenTips("修改成功", false, 1.5f, null, new object[0]);
            }
            else
            {
                Singleton<CUIManager>.instance.OpenTips("err code:" + stSelfDefineChatInfoChgRsp.bResult, false, 1.5f, null, new object[0]);
            }
        }

        public static void SendInBattleMsg_InputChat(string content, byte camp)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x514);
            msg.stPkgData.stChatReq.stChatMsg.bType = 7;
            msg.stPkgData.stChatReq.stChatMsg.stContent.select(7L);
            msg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.bChatType = 3;
            msg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.stChatInfo.select(3L);
            msg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.stChatInfo.stContentStr.bCampLimit = camp;
            StringHelper.StringToUTF8Bytes(content, ref msg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.stChatInfo.stContentStr.szContent);
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
        }

        public static void SendInBattleMsg_PreConfig(uint id, COM_INBATTLE_CHAT_TYPE msgType, uint heroID)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x514);
            msg.stPkgData.stChatReq.stChatMsg.bType = 7;
            msg.stPkgData.stChatReq.stChatMsg.stContent.select(7L);
            msg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.bChatType = (byte) msgType;
            msg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.stChatInfo.select((long) msgType);
            msg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.stFrom.dwAcntHeroID = heroID;
            if (msgType == COM_INBATTLE_CHAT_TYPE.COM_INBATTLE_CHATTYPE_SIGNAL)
            {
                msg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.stChatInfo.stSignalID.dwTextID = id;
            }
            else if (msgType == COM_INBATTLE_CHAT_TYPE.COM_INBATTLE_CHATTYPE_BUBBLE)
            {
                msg.stPkgData.stChatReq.stChatMsg.stContent.stInBattle.stChatInfo.stBubbleID.dwTextID = id;
            }
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
        }

        public static void SendShortCut_Config(ListView<TabElement> list)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1468);
            for (int i = 0; i < Singleton<InBattleMsgMgr>.instance.totalCount; i++)
            {
                if (i < list.Count)
                {
                    TabElement element = list[i];
                    if (element != null)
                    {
                        COMDT_SELFDEFINE_DETAIL_CHATINFO comdt_selfdefine_detail_chatinfo = msg.stPkgData.stSelfDefineChatInfoChgReq.stChatInfo.astChatMsg[i];
                        comdt_selfdefine_detail_chatinfo.bChatType = 1;
                        comdt_selfdefine_detail_chatinfo.stChatInfo.select(1L);
                        comdt_selfdefine_detail_chatinfo.stChatInfo.stSignalID.dwTextID = element.cfgId;
                    }
                }
            }
            msg.stPkgData.stSelfDefineChatInfoChgReq.stChatInfo.bMsgCnt = Singleton<InBattleMsgMgr>.instance.totalCount;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }
    }
}

