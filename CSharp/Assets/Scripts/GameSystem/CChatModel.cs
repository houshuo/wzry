namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class CChatModel
    {
        public bool bEnableInBattleInputChat;
        public CChatChannelMgr channelMgr = new CChatChannelMgr();
        private int index;
        public int m_curGroupID = 1;
        private List<HeroChatTemplateInfo> m_CurSelectGroupTemplateInfo = new List<HeroChatTemplateInfo>();
        public ListView<COMDT_CHAT_PLAYER_INFO> playerInfos = new ListView<COMDT_CHAT_PLAYER_INFO>();
        public List<HeroChatTemplateInfo> selectHeroTemplateList = new List<HeroChatTemplateInfo>();
        public List<COfflineChatIndex> stOfflineChatIndexList = new List<COfflineChatIndex>();
        public CChatSysData sysData = new CChatSysData();

        public void Add_Palyer_Info(COMDT_CHAT_PLAYER_INFO info)
        {
            if (info != null)
            {
                int num = this.Has_PLAYER_INFO(info);
                if (num == -1)
                {
                    this.playerInfos.Add(info);
                }
                else
                {
                    this.playerInfos[num] = info;
                }
            }
        }

        public void AddOfflineChatIndex(ulong ullUid, uint dwLogicWorldId, int index)
        {
            COfflineChatIndex item = null;
            for (int i = 0; i < this.stOfflineChatIndexList.Count; i++)
            {
                COfflineChatIndex index3 = this.stOfflineChatIndexList[i];
                if ((index3.ullUid == ullUid) && (index3.dwLogicWorldId == dwLogicWorldId))
                {
                    item = index3;
                }
            }
            if (item == null)
            {
                item = new COfflineChatIndex(ullUid, dwLogicWorldId);
                this.stOfflineChatIndexList.Add(item);
            }
            if (!item.indexList.Contains(index))
            {
                item.indexList.Add(index);
            }
        }

        public void Clear_HeroSelected()
        {
            this.index = 0;
            this.channelMgr.GetChannel(EChatChannel.Select_Hero).Clear();
        }

        public void ClearAll()
        {
            this.playerInfos.Clear();
            this.sysData.Clear();
            this.channelMgr.ClearAll();
            this.stOfflineChatIndexList.Clear();
        }

        public void ClearCOfflineChatIndex(COfflineChatIndex data)
        {
            if (data != null)
            {
                this.stOfflineChatIndexList.Remove(data);
            }
        }

        public void ClearCOfflineChatIndex(ulong ullUid, uint dwLogicWorldId)
        {
            int num = -1;
            for (int i = 0; i < this.stOfflineChatIndexList.Count; i++)
            {
                COfflineChatIndex index = this.stOfflineChatIndexList[i];
                if ((index.ullUid == ullUid) && (index.dwLogicWorldId == dwLogicWorldId))
                {
                    num = i;
                }
            }
            if (num != -1)
            {
                this.stOfflineChatIndexList.RemoveAt(num);
            }
        }

        public HeroChatTemplateInfo Get_HeroSelect_ChatTemplate(int index)
        {
            int count = this.m_CurSelectGroupTemplateInfo.Count;
            if ((index >= 0) && (index < count))
            {
                return this.m_CurSelectGroupTemplateInfo[index];
            }
            return new HeroChatTemplateInfo(-1);
        }

        public COMDT_CHAT_PLAYER_INFO Get_Palyer_Info(ulong ullUid, uint iLogicWorldID)
        {
            COMDT_CHAT_PLAYER_INFO comdt_chat_player_info = null;
            for (int i = 0; i < this.playerInfos.Count; i++)
            {
                comdt_chat_player_info = this.playerInfos[i];
                if ((comdt_chat_player_info.ullUid == ullUid) && (comdt_chat_player_info.iLogicWorldID == iLogicWorldID))
                {
                    return comdt_chat_player_info;
                }
            }
            return null;
        }

        public COfflineChatIndex GetCOfflineChatIndex(ulong ullUid, uint dwLogicWorldId)
        {
            for (int i = 0; i < this.stOfflineChatIndexList.Count; i++)
            {
                COfflineChatIndex index = this.stOfflineChatIndexList[i];
                if ((index.ullUid == ullUid) && (index.dwLogicWorldId == dwLogicWorldId))
                {
                    return index;
                }
            }
            return null;
        }

        private List<HeroChatTemplateInfo> GetCurGroupInfo(int groupID)
        {
            List<HeroChatTemplateInfo> list = new List<HeroChatTemplateInfo>();
            for (int i = 0; i < this.selectHeroTemplateList.Count; i++)
            {
                HeroChatTemplateInfo info = this.selectHeroTemplateList[i];
                if (info.GetGroupID() == groupID)
                {
                    list.Add(this.selectHeroTemplateList[i]);
                }
            }
            return list;
        }

        public List<HeroChatTemplateInfo> GetCurGroupTemplateInfo()
        {
            return this.m_CurSelectGroupTemplateInfo;
        }

        public int GetFriendTotal_UnreadCount()
        {
            return this.channelMgr.GetFriendTotal_UnreadCount();
        }

        public CChatEntity GetLastUnread_Selected()
        {
            ListView<CChatEntity> list = this.channelMgr.GetChannel(EChatChannel.Select_Hero).list;
            if (((list != null) && (list.Count != 0)) && (((list != null) && (this.index >= 0)) && (this.index < list.Count)))
            {
                return list[this.index++];
            }
            return null;
        }

        public int Has_PLAYER_INFO(COMDT_CHAT_PLAYER_INFO info)
        {
            int count = this.playerInfos.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.playerInfos[i].ullUid == info.ullUid)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool IsTemplate_IndexValid(int index)
        {
            return ((index >= 0) && (index < this.m_CurSelectGroupTemplateInfo.Count));
        }

        public void Load_HeroSelect_ChatTemplate()
        {
            if (this.selectHeroTemplateList.Count == 0)
            {
                DatabinTable<ResHeroSelectTextData, uint> selectHeroChatDatabin = GameDataMgr.m_selectHeroChatDatabin;
                if (selectHeroChatDatabin != null)
                {
                    Dictionary<long, object>.Enumerator enumerator = selectHeroChatDatabin.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<long, object> current = enumerator.Current;
                        ResHeroSelectTextData data = (ResHeroSelectTextData) current.Value;
                        HeroChatTemplateInfo item = new HeroChatTemplateInfo(0) {
                            dwID = (int) data.dwID,
                            dwGroupID = (int) data.dwGroupID,
                            dwTag = (int) data.dwTag,
                            templateString = StringHelper.UTF8BytesToString(ref data.szContent)
                        };
                        this.selectHeroTemplateList.Add(item);
                    }
                }
            }
        }

        public void Remove_Palyer_Info(COMDT_CHAT_PLAYER_INFO info)
        {
            if (info != null)
            {
                this.playerInfos.Remove(info);
            }
        }

        public void SetCurGroupTemplateInfo(uint cfgID)
        {
            this.m_curGroupID = 1;
            if (cfgID > 0)
            {
                this.m_curGroupID = (int) cfgID;
            }
            this.m_CurSelectGroupTemplateInfo.Clear();
            this.m_CurSelectGroupTemplateInfo = this.GetCurGroupInfo(this.m_curGroupID);
        }

        public void SetRestFreeCnt(EChatChannel v, uint count)
        {
            this.sysData.restChatFreeCnt = count;
        }

        public void SetTimeStamp(EChatChannel v, uint time)
        {
            this.sysData.lastTimeStamp = time;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HeroChatTemplateInfo
        {
            public int dwID;
            public int dwGroupID;
            public int dwTag;
            public string templateString;
            public HeroChatTemplateInfo(int id)
            {
                this.dwID = -1;
                this.dwTag = -1;
                this.dwGroupID = -1;
                this.templateString = string.Empty;
            }

            public string GetTemplateString()
            {
                return this.templateString;
            }

            public int GetID()
            {
                return this.dwID;
            }

            public int GetGroupID()
            {
                return this.dwGroupID;
            }

            public int GetTag()
            {
                return this.dwTag;
            }

            public bool isValid()
            {
                if (this.dwID == -1)
                {
                    return false;
                }
                return true;
            }
        }
    }
}

