namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    public class CChatUT
    {
        public static string Build_4_ChatEntry(bool bFriend, CChatEntity ent)
        {
            string str = !bFriend ? Singleton<CTextManager>.instance.GetText("chat_title_total") : Singleton<CTextManager>.instance.GetText("chat_title_friend");
            if (bFriend)
            {
                COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.GetInstance().model.GetGameOrSnsFriend(ent.ullUid, ent.iLogicWorldID);
                if (gameOrSnsFriend != null)
                {
                    str = str + ColorString(0, UT.Bytes2String(gameOrSnsFriend.szUserName)) + ":" + ent.text;
                }
                return str;
            }
            COMDT_CHAT_PLAYER_INFO comdt_chat_player_info = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ent.ullUid, ent.iLogicWorldID);
            if ((str != null) && (comdt_chat_player_info != null))
            {
                str = str + ColorString(0, UT.Bytes2String(comdt_chat_player_info.szName)) + ":" + ent.text;
            }
            return str;
        }

        public static string Build_4_EntryString(EChatChannel type, ulong ullUid, uint iLogicWorldID, string rawText)
        {
            string str = string.Empty;
            if (type == EChatChannel.Friend)
            {
                COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.GetInstance().model.GetGameOrSnsFriend(ullUid, iLogicWorldID);
                if (gameOrSnsFriend != null)
                {
                    str = string.Format(CChatController.fmt, UT.Bytes2String(gameOrSnsFriend.szUserName), rawText);
                }
                return str;
            }
            if (type == EChatChannel.Lobby)
            {
                COMDT_CHAT_PLAYER_INFO comdt_chat_player_info = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ullUid, iLogicWorldID);
                if (comdt_chat_player_info != null)
                {
                    str = string.Format(CChatController.fmt, UT.Bytes2String(comdt_chat_player_info.szName), rawText);
                }
                return str;
            }
            if (type == EChatChannel.Guild)
            {
                COMDT_CHAT_PLAYER_INFO comdt_chat_player_info2 = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ullUid, iLogicWorldID);
                if (comdt_chat_player_info2 != null)
                {
                    str = string.Format(CChatController.fmt, UT.Bytes2String(comdt_chat_player_info2.szName), rawText);
                }
                return str;
            }
            if (type == EChatChannel.Room)
            {
                COMDT_CHAT_PLAYER_INFO comdt_chat_player_info3 = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ullUid, iLogicWorldID);
                if (comdt_chat_player_info3 != null)
                {
                    str = string.Format(CChatController.fmt, UT.Bytes2String(comdt_chat_player_info3.szName), rawText);
                }
                return str;
            }
            if (type == EChatChannel.Team)
            {
                COMDT_CHAT_PLAYER_INFO comdt_chat_player_info4 = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ullUid, iLogicWorldID);
                if (comdt_chat_player_info4 != null)
                {
                    str = string.Format(CChatController.fmt, UT.Bytes2String(comdt_chat_player_info4.szName), rawText);
                }
                return str;
            }
            if (type == EChatChannel.Settle)
            {
                COMDT_CHAT_PLAYER_INFO comdt_chat_player_info5 = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ullUid, iLogicWorldID);
                if (comdt_chat_player_info5 != null)
                {
                    str = string.Format(CChatController.fmt, UT.Bytes2String(comdt_chat_player_info5.szName), rawText);
                }
                return str;
            }
            return "ERROR, in Build_4_EntryString";
        }

        public static CChatEntity Build_4_Friend(COMDT_CHAT_MSG_PRIVATE data)
        {
            CChatEntity entity = new CChatEntity {
                ullUid = data.stFrom.ullUid,
                iLogicWorldID = (uint) data.stFrom.iLogicWorldID,
                text = UT.Bytes2String(data.szContent),
                type = EChaterType.Friend
            };
            GetUser(entity.type, entity.ullUid, entity.iLogicWorldID, out entity.name, out entity.level, out entity.head_url, out entity.stGameVip);
            entity.time = CRoleInfo.GetCurrentUTCTime();
            return entity;
        }

        public static CChatEntity Build_4_Guild(COMDT_CHAT_MSG_GUILD data)
        {
            CChatEntity entity = new CChatEntity {
                ullUid = data.stFrom.ullUid,
                iLogicWorldID = (uint) data.stFrom.iLogicWorldID,
                text = UT.Bytes2String(data.szContent),
                type = EChaterType.Strenger
            };
            GetUser(entity.type, entity.ullUid, entity.iLogicWorldID, out entity.name, out entity.level, out entity.head_url, out entity.stGameVip);
            entity.time = CRoleInfo.GetCurrentUTCTime();
            return entity;
        }

        public static CChatEntity Build_4_Lobby(COMDT_CHAT_MSG_LOGIC_WORLD data)
        {
            CChatEntity entity = new CChatEntity {
                ullUid = data.stFrom.ullUid,
                iLogicWorldID = (uint) data.stFrom.iLogicWorldID,
                text = UT.Bytes2String(data.szContent),
                type = EChaterType.Strenger
            };
            GetUser(entity.type, entity.ullUid, entity.iLogicWorldID, out entity.name, out entity.level, out entity.head_url, out entity.stGameVip);
            entity.time = CRoleInfo.GetCurrentUTCTime();
            return entity;
        }

        public static CChatEntity Build_4_LoudSpeaker(COMDT_CHAT_MSG_HORN data)
        {
            CChatEntity entity = new CChatEntity {
                ullUid = data.stFrom.ullUid,
                iLogicWorldID = (uint) data.stFrom.iLogicWorldID,
                text = UT.Bytes2String(data.szContent),
                type = EChaterType.LoudSpeaker
            };
            GetUser(entity.type, entity.ullUid, entity.iLogicWorldID, out entity.name, out entity.level, out entity.head_url, out entity.stGameVip);
            entity.time = CRoleInfo.GetCurrentUTCTime();
            return entity;
        }

        public static string Build_4_LoudSpeaker_EntryString(ulong ullUid, uint iLogicWorldID, string rawText)
        {
            COMDT_CHAT_PLAYER_INFO comdt_chat_player_info = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ullUid, iLogicWorldID);
            if (comdt_chat_player_info != null)
            {
                return string.Format(CChatController.fmt_gold_name, UT.Bytes2String(comdt_chat_player_info.szName), rawText);
            }
            return "ERROR, in Build_4_EntryString";
        }

        public static CChatEntity Build_4_Offline_Friend(COMDT_OFFLINE_CHAT_MSG data)
        {
            CChatEntity entity = new CChatEntity {
                ullUid = data.stChatMsg.stFrom.ullUid,
                iLogicWorldID = (uint) data.stChatMsg.stFrom.iLogicWorldID,
                text = UT.Bytes2String(data.stChatMsg.szContent),
                type = EChaterType.Friend
            };
            GetUser(entity.type, entity.ullUid, entity.iLogicWorldID, out entity.name, out entity.level, out entity.head_url, out entity.stGameVip);
            return entity;
        }

        public static CChatEntity Build_4_OfflineInfo(string content)
        {
            return new CChatEntity { type = EChaterType.OfflineInfo, text = content };
        }

        public static CChatEntity Build_4_OfflineOrOnline(bool bOffline)
        {
            return new CChatEntity { text = !bOffline ? Singleton<CTextManager>.instance.GetText("FriendChat_online_Info") : Singleton<CTextManager>.instance.GetText("FriendChat_Offline_Info"), type = EChaterType.OfflineInfo };
        }

        public static CChatEntity Build_4_Room(COMDT_CHAT_MSG_ROOM data)
        {
            CChatEntity entity = new CChatEntity {
                ullUid = data.stFrom.ullUid,
                iLogicWorldID = (uint) data.stFrom.iLogicWorldID,
                text = UT.Bytes2String(data.szContent),
                type = EChaterType.Strenger
            };
            GetUser(entity.type, entity.ullUid, entity.iLogicWorldID, out entity.name, out entity.level, out entity.head_url, out entity.stGameVip);
            return entity;
        }

        public static CChatEntity Build_4_SelectHero(COMDT_CHAT_MSG_BATTLE data)
        {
            CChatEntity entity = new CChatEntity {
                ullUid = data.stFrom.ullUid,
                iLogicWorldID = (uint) data.stFrom.iLogicWorldID
            };
            if (entity.ullUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
            {
                entity.type = EChaterType.Self;
            }
            else
            {
                entity.type = EChaterType.Strenger;
            }
            if (data.bChatType == 1)
            {
                CChatModel.HeroChatTemplateInfo info = Singleton<CChatController>.instance.model.Get_HeroSelect_ChatTemplate((int) data.stChatInfo.stContentID.dwTextID);
                if (info.isValid())
                {
                    entity.text = info.templateString;
                }
            }
            if (data.bChatType == 2)
            {
                entity.text = UT.Bytes2String(data.stChatInfo.stContentStr.szContent);
            }
            GetUser(entity.type, entity.ullUid, entity.iLogicWorldID, out entity.name, out entity.level, out entity.head_url, out entity.stGameVip);
            return entity;
        }

        public static CChatEntity Build_4_Self(string content)
        {
            CChatEntity entity = new CChatEntity {
                ullUid = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID,
                iLogicWorldID = 0,
                text = content,
                type = EChaterType.Self
            };
            GetUser(entity.type, entity.ullUid, entity.iLogicWorldID, out entity.name, out entity.level, out entity.head_url, out entity.stGameVip);
            entity.time = CRoleInfo.GetCurrentUTCTime();
            return entity;
        }

        public static CChatEntity Build_4_Settle(COMDT_CHAT_MSG_SETTLE data)
        {
            CChatEntity entity = new CChatEntity {
                ullUid = data.stFrom.ullUid,
                iLogicWorldID = (uint) data.stFrom.iLogicWorldID,
                text = UT.Bytes2String(data.szContent),
                type = EChaterType.Strenger
            };
            GetUser(entity.type, entity.ullUid, entity.iLogicWorldID, out entity.name, out entity.level, out entity.head_url, out entity.stGameVip);
            entity.time = CRoleInfo.GetCurrentUTCTime();
            return entity;
        }

        public static CChatEntity Build_4_Speaker(COMDT_CHAT_MSG_HORN data)
        {
            CChatEntity entity = new CChatEntity {
                ullUid = data.stFrom.ullUid,
                iLogicWorldID = (uint) data.stFrom.iLogicWorldID,
                text = UT.Bytes2String(data.szContent),
                type = EChaterType.Speaker
            };
            GetUser(entity.type, entity.ullUid, entity.iLogicWorldID, out entity.name, out entity.level, out entity.head_url, out entity.stGameVip);
            entity.time = CRoleInfo.GetCurrentUTCTime();
            return entity;
        }

        public static string Build_4_Speaker_EntryString(ulong ullUid, uint iLogicWorldID, string rawText)
        {
            COMDT_CHAT_PLAYER_INFO comdt_chat_player_info = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ullUid, iLogicWorldID);
            if (comdt_chat_player_info != null)
            {
                return string.Format(CChatController.fmt, UT.Bytes2String(comdt_chat_player_info.szName), rawText);
            }
            return "ERROR, in Build_4_EntryString";
        }

        public static CChatEntity Build_4_System(string content)
        {
            return new CChatEntity { type = EChaterType.System, text = content };
        }

        public static CChatEntity Build_4_Team(COMDT_CHAT_MSG_TEAM data)
        {
            CChatEntity entity = new CChatEntity {
                ullUid = data.stFrom.ullUid,
                iLogicWorldID = (uint) data.stFrom.iLogicWorldID,
                text = UT.Bytes2String(data.szContent),
                type = EChaterType.Strenger
            };
            GetUser(entity.type, entity.ullUid, entity.iLogicWorldID, out entity.name, out entity.level, out entity.head_url, out entity.stGameVip);
            entity.time = CRoleInfo.GetCurrentUTCTime();
            return entity;
        }

        public static CChatEntity Build_4_Time()
        {
            DateTime time = Utility.ToUtcTime2Local((long) CRoleInfo.GetCurrentUTCTime());
            return new CChatEntity { ullUid = 0L, iLogicWorldID = 0, text = time.TimeOfDay.ToString(), type = EChaterType.Time };
        }

        public static CChatEntity Build_4_Time(int curSec)
        {
            CChatEntity entity = new CChatEntity();
            DateTime time = Utility.ToUtcTime2Local((long) curSec);
            entity.ullUid = 0L;
            entity.iLogicWorldID = 0;
            object[] args = new object[] { time.Year, time.Month, time.Day, time.TimeOfDay.ToString() };
            entity.text = string.Format("{0}/{1}/{2}  {3}", args);
            entity.type = EChaterType.Time;
            return entity;
        }

        public static string ColorString(uint color, string text)
        {
            return string.Format("<color=green>{0}</color>", text);
        }

        public static COM_CHAT_MSG_TYPE Convert_Channel_ChatMsgType(EChatChannel type)
        {
            switch (type)
            {
                case EChatChannel.Team:
                    return COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_TEAM;

                case EChatChannel.Room:
                    return COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_ROOM;

                case EChatChannel.Lobby:
                    return COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_LOGIC_WORLD;

                case EChatChannel.Friend:
                    return COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_PRIVATE;

                case EChatChannel.Guild:
                    return COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_GUILD;

                case EChatChannel.Settle:
                    return COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_SETTLE;
            }
            return (COM_CHAT_MSG_TYPE) 0x186a0;
        }

        public static EChatChannel Convert_ChatMsgType_Channel(COM_CHAT_MSG_TYPE type)
        {
            switch (type)
            {
                case COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_LOGIC_WORLD:
                    return EChatChannel.Lobby;

                case COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_PRIVATE:
                    return EChatChannel.Friend;

                case COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_ROOM:
                    return EChatChannel.Room;

                case COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_GUILD:
                    return EChatChannel.Guild;

                case COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_BATTLE:
                    return EChatChannel.Select_Hero;

                case COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_TEAM:
                    return EChatChannel.Team;

                case COM_CHAT_MSG_TYPE.COM_CHAT_MSG_TYPE_SETTLE:
                    return EChatChannel.Settle;
            }
            return EChatChannel.None;
        }

        public static EChatChannel Convert_ChatMsgType_Channel(byte v)
        {
            return Convert_ChatMsgType_Channel((COM_CHAT_MSG_TYPE) v);
        }

        public static void EnterFriend()
        {
        }

        public static void EnterRoom()
        {
            Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Room, 0L, 0);
            Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Room);
            Singleton<CChatController>.instance.ShowPanel(true, false);
            Singleton<CChatController>.GetInstance().bSendChat = true;
            Singleton<CChatController>.instance.view.UpView(true);
            Singleton<CChatController>.instance.model.sysData.ClearEntryText();
        }

        public static void EnterSettle()
        {
            Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Room, 0L, 0);
            Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Room);
            Singleton<CChatController>.instance.ShowPanel(true, false);
            Singleton<CChatController>.GetInstance().bSendChat = true;
            Singleton<CChatController>.instance.view.UpView(true);
            Singleton<CChatController>.instance.model.sysData.ClearEntryText();
        }

        public static string GetEChatChannel_Text(EChatChannel channel)
        {
            switch (channel)
            {
                case EChatChannel.Team:
                    return Singleton<CTextManager>.GetInstance().GetText("chat_title_team");

                case EChatChannel.Room:
                    return Singleton<CTextManager>.instance.GetText("chat_title_room");

                case EChatChannel.Lobby:
                    return Singleton<CTextManager>.instance.GetText("chat_title_total");

                case EChatChannel.Friend:
                    return Singleton<CTextManager>.instance.GetText("chat_title_friend");

                case EChatChannel.Guild:
                    return Singleton<CTextManager>.GetInstance().GetText("chat_title_guild");

                case EChatChannel.Settle:
                    return Singleton<CTextManager>.GetInstance().GetText("Chat_title_Settle");
            }
            return "error";
        }

        public static void GetUser(EChaterType type, ulong ulluid, uint logicworld_id, out string name, out string level, out string head_url, out COMDT_GAME_VIP_CLIENT stGameVip)
        {
            name = "error";
            level = "-1";
            head_url = string.Empty;
            stGameVip = null;
            if (type == EChaterType.Self)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    name = masterRoleInfo.Name;
                    level = masterRoleInfo.PvpLevel.ToString();
                    stGameVip = masterRoleInfo.GetNobeInfo().stGameVipClient;
                    head_url = masterRoleInfo.HeadUrl.Substring(0, masterRoleInfo.HeadUrl.LastIndexOf("/"));
                }
            }
            else if (type == EChaterType.Friend)
            {
                COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.GetInstance().model.GetGameOrSnsFriend(ulluid, logicworld_id);
                if (gameOrSnsFriend != null)
                {
                    name = UT.Bytes2String(gameOrSnsFriend.szUserName);
                    level = ((int) gameOrSnsFriend.dwPvpLvl).ToString();
                    head_url = UT.Bytes2String(gameOrSnsFriend.szHeadUrl);
                    stGameVip = gameOrSnsFriend.stGameVip;
                }
            }
            else if ((type == EChaterType.Strenger) || (type == EChaterType.GuildMember))
            {
                COMDT_CHAT_PLAYER_INFO comdt_chat_player_info = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ulluid, logicworld_id);
                if (comdt_chat_player_info != null)
                {
                    name = UT.Bytes2String(comdt_chat_player_info.szName);
                    level = ((int) comdt_chat_player_info.dwLevel).ToString();
                    head_url = UT.Bytes2String(comdt_chat_player_info.szHeadUrl);
                    stGameVip = comdt_chat_player_info.stVip;
                }
            }
            else if ((type == EChaterType.Speaker) || (type == EChaterType.LoudSpeaker))
            {
                COMDT_CHAT_PLAYER_INFO comdt_chat_player_info2 = Singleton<CChatController>.GetInstance().model.Get_Palyer_Info(ulluid, logicworld_id);
                if (comdt_chat_player_info2 != null)
                {
                    name = UT.Bytes2String(comdt_chat_player_info2.szName);
                    level = ((int) comdt_chat_player_info2.dwLevel).ToString();
                    head_url = UT.Bytes2String(comdt_chat_player_info2.szHeadUrl);
                    stGameVip = comdt_chat_player_info2.stVip;
                }
            }
        }

        public static void LeaveFriend()
        {
        }

        public static void LeaveLobby()
        {
        }

        public static void LeaveRoom()
        {
            Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Room, 0L, 0);
            Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
            Singleton<CChatController>.instance.view.UpView(false);
            Singleton<CChatController>.instance.model.sysData.ClearEntryText();
        }

        public static void LeaveSettle()
        {
            Singleton<CChatController>.instance.ShowPanel(false, false);
        }
    }
}

