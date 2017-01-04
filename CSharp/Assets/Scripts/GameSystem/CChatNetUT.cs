namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [MessageHandlerClass]
    public class CChatNetUT
    {
        [MessageHandler(0x516)]
        public static void On_Chat_NTF(CSPkg msg)
        {
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("On_Chat_GetMsg_NTF", msg);
        }

        [MessageHandler(0x51b)]
        public static void On_Offline_Chat_NTF(CSPkg msg)
        {
            Debug.Log("---Chat On_Offline_Chat_NTF...");
            Singleton<EventRouter>.GetInstance().BroadCastEvent<CSPkg>("Chat_Offline_GetMsg_NTF", msg);
        }

        public static void Send_Clear_Offline(List<int> delIndexList)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x51c);
            msg.stPkgData.stCleanOfflineChatReq.bDelChatCnt = (byte) delIndexList.Count;
            for (int i = 0; i < delIndexList.Count; i++)
            {
                msg.stPkgData.stCleanOfflineChatReq.ChatIndex[i] = delIndexList[i];
            }
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            Debug.Log("---Chat Send_Clear_Offline Req, count:" + delIndexList.Count);
        }

        public static void Send_GetChat_Req(EChatChannel channel)
        {
            if (Singleton<NetworkModule>.GetInstance().lobbySvr.connected)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x515);
                msg.stPkgData.stGetChatMsgReq.bChatType = (byte) CChatUT.Convert_Channel_ChatMsgType(channel);
                if (channel == EChatChannel.Lobby)
                {
                    if (Singleton<CChatController>.GetInstance().model.sysData.lastTimeStamp != 0)
                    {
                        msg.stPkgData.stGetChatMsgReq.dwLastTimeStamp = Singleton<CChatController>.GetInstance().model.sysData.lastTimeStamp;
                        Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                    }
                }
                else
                {
                    Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                }
            }
        }

        public static void Send_Guild_Chat(string text)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x514);
            msg.stPkgData.stChatReq.stChatMsg.bType = 4;
            msg.stPkgData.stChatReq.stChatMsg.stContent.select(4L);
            msg.stPkgData.stChatReq.stChatMsg.stContent.stGuild.szContent = UT.String2Bytes(text);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public static void Send_Leave_Settle()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1470);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public static void Send_Lobby_Chat(string text)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x514);
            msg.stPkgData.stChatReq.stChatMsg.bType = 1;
            msg.stPkgData.stChatReq.stChatMsg.stContent.select(1L);
            msg.stPkgData.stChatReq.stChatMsg.stContent.stLogicWord.szContent = UT.String2Bytes(text);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Lobby).Start_InputCD();
        }

        public static void Send_Private_Chat(ulong ullUid, uint logicWorldId, string text)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x514);
            msg.stPkgData.stChatReq.stChatMsg.bType = 2;
            msg.stPkgData.stChatReq.stChatMsg.stContent.select(2L);
            msg.stPkgData.stChatReq.stChatMsg.stContent.stPrivate.stTo.ullUid = ullUid;
            msg.stPkgData.stChatReq.stChatMsg.stContent.stPrivate.stTo.iLogicWorldID = (int) logicWorldId;
            COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.instance.model.GetGameOrSnsFriend(ullUid, logicWorldId);
            if (gameOrSnsFriend != null)
            {
                msg.stPkgData.stChatReq.stChatMsg.stContent.stPrivate.stTo.szName = gameOrSnsFriend.szUserName;
                msg.stPkgData.stChatReq.stChatMsg.stContent.stPrivate.stTo.dwLevel = gameOrSnsFriend.dwLevel;
            }
            msg.stPkgData.stChatReq.stChatMsg.stContent.stPrivate.szContent = UT.String2Bytes(text);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
            Debug.Log(string.Concat(new object[] { "--- send private chat, id:", ullUid, ",logicworldid:", logicWorldId, ",content:", text }));
        }

        public static void Send_Room_Chat(string text)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x514);
            msg.stPkgData.stChatReq.stChatMsg.bType = 3;
            msg.stPkgData.stChatReq.stChatMsg.stContent.select(3L);
            msg.stPkgData.stChatReq.stChatMsg.stContent.stRoom.szContent = UT.String2Bytes(text);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public static void Send_SelectHero_Chat(string text)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x514);
            msg.stPkgData.stChatReq.stChatMsg.bType = 5;
            msg.stPkgData.stChatReq.stChatMsg.stContent.select(5L);
            msg.stPkgData.stChatReq.stChatMsg.stContent.stBattle.bChatType = 2;
            msg.stPkgData.stChatReq.stChatMsg.stContent.stBattle.stChatInfo.select(2L);
            msg.stPkgData.stChatReq.stChatMsg.stContent.stBattle.stChatInfo.stContentStr.szContent = UT.String2Bytes(text);
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
        }

        public static void Send_SelectHero_Chat(uint template_id)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x514);
            msg.stPkgData.stChatReq.stChatMsg.bType = 5;
            msg.stPkgData.stChatReq.stChatMsg.stContent.select(5L);
            msg.stPkgData.stChatReq.stChatMsg.stContent.stBattle.bChatType = 1;
            msg.stPkgData.stChatReq.stChatMsg.stContent.stBattle.stChatInfo.select(1L);
            msg.stPkgData.stChatReq.stChatMsg.stContent.stBattle.stChatInfo.stContentID.dwTextID = template_id;
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
        }

        public static void Send_Settle_Chat(string text)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x514);
            msg.stPkgData.stChatReq.stChatMsg.bType = 8;
            msg.stPkgData.stChatReq.stChatMsg.stContent.select(8L);
            msg.stPkgData.stChatReq.stChatMsg.stContent.stSettle.szContent = UT.String2Bytes(text);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public static void Send_Team_Chat(string text)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x514);
            msg.stPkgData.stChatReq.stChatMsg.bType = 6;
            msg.stPkgData.stChatReq.stChatMsg.stContent.select(6L);
            msg.stPkgData.stChatReq.stChatMsg.stContent.stTeam.szContent = UT.String2Bytes(text);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }
    }
}

