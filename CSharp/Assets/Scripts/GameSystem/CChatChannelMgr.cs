namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public class CChatChannelMgr
    {
        public EChatTab ChatTab;
        public List<uint> CurActiveChannels = new List<uint>();
        public ListView<CChatChannel> FriendChannelList = new ListView<CChatChannel>();
        public ListView<CChatChannel> NormalChannelList = new ListView<CChatChannel>();

        public CChatChannelMgr()
        {
            this.NormalChannelList.Add(new CChatChannel(EChatChannel.Lobby, 0, 0L, 0));
            this.SetChatTab(EChatTab.Normal);
        }

        public CChatChannel _getChannel(EChatChannel type, ulong ullUid = 0, uint dwLogicWorldId = 0)
        {
            if (type != EChatChannel.Friend)
            {
                for (int i = 0; i < this.NormalChannelList.Count; i++)
                {
                    if ((this.NormalChannelList[i] != null) && (this.NormalChannelList[i].ChannelType == type))
                    {
                        return this.NormalChannelList[i];
                    }
                }
            }
            else
            {
                for (int j = 0; j < this.FriendChannelList.Count; j++)
                {
                    if (((this.FriendChannelList[j] != null) && (this.FriendChannelList[j].ullUid == ullUid)) && (this.FriendChannelList[j].dwLogicWorldId == dwLogicWorldId))
                    {
                        return this.FriendChannelList[j];
                    }
                }
            }
            return null;
        }

        public void Add_ChatEntity(CChatEntity chatEnt, EChatChannel type, ulong ullUid = 0, uint dwLogicWorldId = 0)
        {
            CChatChannel channel = this._getChannel(type, ullUid, dwLogicWorldId);
            if (channel == null)
            {
                channel = this.CreateChannel(type, ullUid, dwLogicWorldId);
            }
            if ((chatEnt.type != EChaterType.System) || (chatEnt.type != EChaterType.OfflineInfo))
            {
                if (Singleton<CChatController>.instance.view.ChatParser != null)
                {
                    Singleton<CChatController>.instance.view.ChatParser.bProc_ChatEntry = false;
                    Singleton<CChatController>.instance.view.ChatParser.maxWidth = CChatParser.chat_list_max_width;
                    Singleton<CChatController>.instance.view.ChatParser.Parse(chatEnt.text, CChatParser.start_x, chatEnt);
                }
                else
                {
                    DebugHelper.Assert(false, "CChatController.instance.view.ChatParser = null! StackTrace = " + new StackTrace().ToString());
                }
            }
            CChatEntity last = channel.GetLast();
            if (((last != null) && (last.time != 0)) && ((chatEnt.time - last.time) > 60))
            {
                channel.Add(CChatUT.Build_4_Time());
            }
            channel.Add(chatEnt);
        }

        public void Add_CurChatFriend(CChatEntity chatEnt)
        {
            DebugHelper.Assert((Singleton<CChatController>.instance.model.sysData.ullUid != 0) && (Singleton<CChatController>.instance.model.sysData.dwLogicWorldId != 0));
            this.Add_ChatEntity(chatEnt, EChatChannel.Friend, Singleton<CChatController>.instance.model.sysData.ullUid, Singleton<CChatController>.instance.model.sysData.dwLogicWorldId);
        }

        public void Clear(EChatChannel type, ulong ullUid = 0, uint dwLogicWorldId = 0)
        {
            CChatChannel channel = this._getChannel(type, ullUid, dwLogicWorldId);
            if (channel != null)
            {
                channel.Clear();
            }
        }

        public void ClearAll()
        {
            for (int i = 0; i < this.NormalChannelList.Count; i++)
            {
                if (this.NormalChannelList[i] != null)
                {
                    this.NormalChannelList[i].Clear();
                }
            }
            for (int j = 0; j < this.FriendChannelList.Count; j++)
            {
                if (this.FriendChannelList[j] != null)
                {
                    this.FriendChannelList[j].Clear();
                }
            }
            this.SetChatTab(EChatTab.Normal);
        }

        public CChatChannel CreateChannel(EChatChannel type, ulong ullUid = 0, uint dwLogicWorldId = 0)
        {
            CChatChannel item = this._getChannel(type, ullUid, dwLogicWorldId);
            if (item == null)
            {
                if (type != EChatChannel.Friend)
                {
                    item = new CChatChannel(type, 0, 0L, 0);
                    this.NormalChannelList.Add(item);
                    return item;
                }
                item = new CChatChannel(type, 0x1b58, ullUid, dwLogicWorldId);
                item.list.Add(CChatUT.Build_4_System(Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_4")));
                item.ReadAll();
                this.FriendChannelList.Add(item);
            }
            return item;
        }

        public CChatChannel GetChannel(EChatChannel type)
        {
            CChatChannel channel = this._getChannel(type, 0L, 0);
            if (channel == null)
            {
                channel = this.CreateChannel(type, 0L, 0);
            }
            return channel;
        }

        public CChatChannel GetFriendChannel(ulong ullUid = 0, uint dwLogicWorldId = 0)
        {
            CChatChannel channel = this._getChannel(EChatChannel.Friend, ullUid, dwLogicWorldId);
            if (channel == null)
            {
                channel = this.CreateChannel(EChatChannel.Friend, ullUid, dwLogicWorldId);
            }
            return channel;
        }

        public int GetFriendTotal_UnreadCount()
        {
            int num = 0;
            for (int i = 0; i < this.FriendChannelList.Count; i++)
            {
                CChatChannel channel = this.FriendChannelList[i];
                COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.instance.model.GetGameOrSnsFriend(channel.ullUid, channel.dwLogicWorldId);
                if ((gameOrSnsFriend != null) && (gameOrSnsFriend.bIsOnline == 1))
                {
                    num += channel.GetUnreadCount();
                }
            }
            return num;
        }

        public int GetUnreadCount(EChatChannel type, ulong ullUid = 0, uint dwLogicWorldId = 0)
        {
            CChatChannel channel = this._getChannel(type, ullUid, dwLogicWorldId);
            if (channel == null)
            {
                channel = this.CreateChannel(type, ullUid, dwLogicWorldId);
            }
            return channel.GetUnreadCount();
        }

        public void SetChatTab(EChatTab type)
        {
            this.CurActiveChannels.Clear();
            if (type == EChatTab.Normal)
            {
                this.CurActiveChannels.Add(2);
                this.CurActiveChannels.Add(3);
                if ((Singleton<CGuildSystem>.GetInstance() != null) && Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
                {
                    this.CurActiveChannels.Add(4);
                }
                this.ChatTab = type;
            }
            else if (type == EChatTab.Room)
            {
                this.CurActiveChannels.Add(1);
                this.CurActiveChannels.Add(3);
                this.ChatTab = type;
            }
            else if (type == EChatTab.Team)
            {
                this.CurActiveChannels.Add(0);
                this.CurActiveChannels.Add(3);
                this.ChatTab = type;
            }
            else if (type == EChatTab.Settle)
            {
                this.CurActiveChannels.Add(8);
                this.ChatTab = type;
            }
        }

        public enum EChatTab
        {
            Normal,
            Room,
            Team,
            Settle
        }
    }
}

