namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CFriendModel
    {
        private uint _fileter = 3;
        private static COM_APOLLO_TRANK_SCORE_TYPE _lastSortTpye;
        private DictionaryView<uint, ListView<COMDT_FRIEND_INFO>> _map = new DictionaryView<uint, ListView<COMDT_FRIEND_INFO>>();
        private ListView<COMDT_FRIEND_INFO> _rankingFriend = new ListView<COMDT_FRIEND_INFO>();
        private ListView<COMDT_FRIEND_INFO> cache_chatFriends_Results = new ListView<COMDT_FRIEND_INFO>();
        private ListView<COMDT_FRIEND_INFO> cache_OnlineFriends_Results = new ListView<COMDT_FRIEND_INFO>();
        public int cond1;
        public int cond2;
        public int freezeDayCount;
        public string friend_static_text = string.Empty;
        public uint fullIntimacy;
        private ListView<FriendInGame> gameStateList = new ListView<FriendInGame>();
        public string Guild_Has_Invited_txt;
        public string Guild_Has_Recommended_txt;
        public string Guild_Invite_txt;
        public string Guild_Recommend_txt;
        public CFriendHeartData HeartData = new CFriendHeartData();
        private List<stBlackName> m_blackList_friend = new List<stBlackName>();
        private List<stFriendVerifyContent> m_friendVerifyList = new List<stFriendVerifyContent>();
        private ListView<CSDT_LBS_USER_INFO> m_LBSList = new ListView<CSDT_LBS_USER_INFO>();
        private ListView<CSDT_LBS_USER_INFO> m_LBSListNan = new ListView<CSDT_LBS_USER_INFO>();
        private ListView<CSDT_LBS_USER_INFO> m_LBSListNv = new ListView<CSDT_LBS_USER_INFO>();
        private List<string> m_preconfigVerifyContentList = new List<string>();
        public bool m_shareLocation;
        private List<stFriendIntimacy> m_stFriendIntimacyList = new List<stFriendIntimacy>();
        public static Dictionary<ulong, string> RemarkNames = new Dictionary<ulong, string>();
        public string searchLBSZero = string.Empty;
        public CFriendReCallData SnsReCallData = new CFriendReCallData();
        public ulong ullSvrCurSec;

        public CFriendModel()
        {
            IEnumerator enumerator = Enum.GetValues(typeof(FriendType)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    int current = (int) enumerator.Current;
                    this._map.Add((uint) current, new ListView<COMDT_FRIEND_INFO>());
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            this.Guild_Invite_txt = Singleton<CTextManager>.GetInstance().GetText("Guild_Invite");
            this.Guild_Has_Invited_txt = Singleton<CTextManager>.GetInstance().GetText("Guild_Has_Invited");
            this.Guild_Recommend_txt = Singleton<CTextManager>.GetInstance().GetText("Guild_Recommend");
            this.Guild_Has_Recommended_txt = Singleton<CTextManager>.GetInstance().GetText("Guild_Has_Recommended");
        }

        public void Add(FriendType type, COMDT_FRIEND_INFO data, bool ingore_worldid = false)
        {
            this.AddRankingFriend(type, data);
            ListView<COMDT_FRIEND_INFO> list = this.GetList(type);
            int num = this.getIndex(data, list, ingore_worldid);
            if (num == -1)
            {
                UT.Add2List<COMDT_FRIEND_INFO>(data, list);
            }
            else
            {
                list[num] = data;
            }
            if (type == FriendType.RequestFriend)
            {
                Singleton<EventRouter>.instance.BroadCastEvent("Friend_LobbyIconRedDot_Refresh");
            }
        }

        public void AddFriendBlack(COMDT_FRIEND_INFO info)
        {
            if (info != null)
            {
                int friendBlackIndex = this.GetFriendBlackIndex(info.stUin.ullUid, info.stUin.dwLogicWorldId);
                if (friendBlackIndex != -1)
                {
                    this.m_blackList_friend.RemoveAt(friendBlackIndex);
                }
                this.m_blackList_friend.Add(this.Convert2BlackName(info));
            }
        }

        public void AddFriendBlack(COMDT_CHAT_PLAYER_INFO info, byte bGender, uint dwLastLoginTime)
        {
            if (info != null)
            {
                int friendBlackIndex = this.GetFriendBlackIndex(info.ullUid, (uint) info.iLogicWorldID);
                if (friendBlackIndex != -1)
                {
                    this.m_blackList_friend.RemoveAt(friendBlackIndex);
                }
                this.m_blackList_friend.Add(this.Convert2BlackName(info, bGender, dwLastLoginTime));
            }
        }

        public void AddFriendIntimacy(ulong ullUid, uint dwLogicWorldID, uint lastTime, ushort wIntimacyValue)
        {
            int friendIntimacyIndex = this.GetFriendIntimacyIndex(ullUid, dwLogicWorldID);
            if (friendIntimacyIndex != -1)
            {
                this.m_stFriendIntimacyList.RemoveAt(friendIntimacyIndex);
            }
            this.m_stFriendIntimacyList.Add(new stFriendIntimacy(ullUid, dwLogicWorldID, lastTime, wIntimacyValue));
        }

        public void AddFriendVerifyContent(ulong ullUid, uint dwLogicWorldID, string content)
        {
            int friendVerifyIndex = this.GetFriendVerifyIndex(ullUid, dwLogicWorldID);
            if (friendVerifyIndex != -1)
            {
                this.m_friendVerifyList.RemoveAt(friendVerifyIndex);
            }
            this.m_friendVerifyList.Add(new stFriendVerifyContent(ullUid, dwLogicWorldID, content));
        }

        public void AddLBSUser(CSDT_LBS_USER_INFO info)
        {
            this.AddLBSUser(info, this.m_LBSList);
            if (info.stLbsUserInfo.bGender == 2)
            {
                this.AddLBSUser(info, this.m_LBSListNv);
            }
            if (info.stLbsUserInfo.bGender == 1)
            {
                this.AddLBSUser(info, this.m_LBSListNan);
            }
        }

        private void AddLBSUser(CSDT_LBS_USER_INFO info, ListView<CSDT_LBS_USER_INFO> list)
        {
            if ((info != null) && (list != null))
            {
                int index = this.GetLBSListIndex(info.stLbsUserInfo.stUin.ullUid, info.stLbsUserInfo.stUin.dwLogicWorldId, list);
                if (index != -1)
                {
                    list.RemoveAt(index);
                }
                list.Add(info);
            }
        }

        public void AddRankingFriend(FriendType type, COMDT_FRIEND_INFO data)
        {
            if ((type == FriendType.SNS) || (type == FriendType.GameFriend))
            {
                bool flag = false;
                for (int i = 0; i < this._rankingFriend.Count; i++)
                {
                    if ((this._rankingFriend[i].stUin.ullUid == data.stUin.ullUid) && (this._rankingFriend[i].stUin.dwLogicWorldId == data.stUin.dwLogicWorldId))
                    {
                        this._rankingFriend[i] = data;
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    this._rankingFriend.Add(data);
                }
            }
        }

        public EIntimacyType CalcType(int value)
        {
            if (value <= this.cond1)
            {
                return EIntimacyType.Low;
            }
            if ((value > this.cond1) && (value <= this.cond2))
            {
                return EIntimacyType.Middle;
            }
            if ((value > this.cond2) && (value < this.fullIntimacy))
            {
                return EIntimacyType.High;
            }
            return EIntimacyType.full;
        }

        public void Clear(FriendType type)
        {
            this.GetList(type).Clear();
        }

        public void ClearAll()
        {
            DictionaryView<uint, ListView<COMDT_FRIEND_INFO>>.Enumerator enumerator = this._map.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, ListView<COMDT_FRIEND_INFO>> current = enumerator.Current;
                ListView<COMDT_FRIEND_INFO> view = current.Value;
                if (view != null)
                {
                    view.Clear();
                }
            }
            this.gameStateList.Clear();
            this._rankingFriend.Clear();
            this.SnsReCallData.Clear();
            this.HeartData.Clear();
            this.m_friendVerifyList.Clear();
            this.m_stFriendIntimacyList.Clear();
            this.m_LBSList.Clear();
            this.searchLBSZero = string.Empty;
            RemarkNames.Clear();
        }

        public void ClearLBSData()
        {
            this.m_LBSList.Clear();
            this.m_LBSListNan.Clear();
            this.m_LBSListNv.Clear();
        }

        public stBlackName Convert2BlackName(COMDT_FRIEND_INFO info)
        {
            return new stBlackName { ullUid = info.stUin.ullUid, dwLogicWorldId = info.stUin.dwLogicWorldId, name = UT.Bytes2String(info.szUserName), bIsOnline = info.bIsOnline, dwLastLoginTime = info.dwLastLoginTime, bGender = info.bGender, szHeadUrl = UT.Bytes2String(info.szHeadUrl), dwPvpLvl = info.dwPvpLvl };
        }

        public stBlackName Convert2BlackName(COMDT_CHAT_PLAYER_INFO info, byte bGender, uint dwLastLoginTime)
        {
            return new stBlackName { ullUid = info.ullUid, dwLogicWorldId = (uint) info.iLogicWorldID, name = UT.Bytes2String(info.szName), bIsOnline = 0, bGender = bGender, szHeadUrl = UT.Bytes2String(info.szHeadUrl), dwPvpLvl = info.dwLevel, dwLastLoginTime = dwLastLoginTime };
        }

        public void FilterRecommendFriends()
        {
            ListView<COMDT_FRIEND_INFO> view = this._map[3];
            if (view != null)
            {
                for (int i = 0; i < view.Count; i++)
                {
                    if (this.getFriendByUid(view[i].stUin.ullUid, FriendType.GameFriend) != null)
                    {
                        view.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private static void FindAll(ListView<COMDT_FRIEND_INFO> InSearch, Predicate<COMDT_FRIEND_INFO> match, ListView<COMDT_FRIEND_INFO> outputList)
        {
            if ((InSearch != null) && (outputList != null))
            {
                outputList.Clear();
                for (int i = 0; i < InSearch.Count; i++)
                {
                    if (match(InSearch[i]))
                    {
                        outputList.Add(InSearch[i]);
                    }
                }
            }
        }

        public static int FriendDataSort(COMDT_FRIEND_INFO l, COMDT_FRIEND_INFO r)
        {
            ushort num;
            ushort num2;
            EIntimacyType type;
            EIntimacyType type2;
            bool flag;
            bool flag2;
            if (l == r)
            {
                return 0;
            }
            if ((l == null) || (r == null))
            {
                return 0;
            }
            if (r.bIsOnline != l.bIsOnline)
            {
                return (r.bIsOnline - l.bIsOnline);
            }
            CFriendModel model = Singleton<CFriendContoller>.instance.model;
            model.GetFriendIntimacy(r.stUin.ullUid, r.stUin.dwLogicWorldId, out num, out type, out flag);
            model.GetFriendIntimacy(l.stUin.ullUid, l.stUin.dwLogicWorldId, out num2, out type2, out flag2);
            if (num2 != num)
            {
                return (num - num2);
            }
            if (r.stGameVip.dwCurLevel == l.stGameVip.dwCurLevel)
            {
                return (int) (r.dwPvpLvl - l.dwPvpLvl);
            }
            return (int) (r.stGameVip.dwCurLevel - l.stGameVip.dwCurLevel);
        }

        public static int FriendDataSortForChatFriendList(COMDT_FRIEND_INFO l, COMDT_FRIEND_INFO r)
        {
            if ((l != r) && ((l != null) && (r != null)))
            {
                CChatChannel friendChannel = Singleton<CChatController>.instance.model.channelMgr.GetFriendChannel(r.stUin.ullUid, r.stUin.dwLogicWorldId);
                CChatChannel channel2 = Singleton<CChatController>.instance.model.channelMgr.GetFriendChannel(l.stUin.ullUid, l.stUin.dwLogicWorldId);
                if (r.bIsOnline == l.bIsOnline)
                {
                    if (friendChannel.GetUnreadCount() != channel2.GetUnreadCount())
                    {
                        return (friendChannel.GetUnreadCount() - channel2.GetUnreadCount());
                    }
                    if (r.dwPvpLvl > l.dwPvpLvl)
                    {
                        return 1;
                    }
                    if (r.dwPvpLvl < l.dwPvpLvl)
                    {
                        return -1;
                    }
                    return 0;
                }
                if (l.bIsOnline < r.bIsOnline)
                {
                    if (channel2.GetUnreadCount() > 0)
                    {
                        return -1;
                    }
                    return 1;
                }
                if (r.bIsOnline < l.bIsOnline)
                {
                    if (friendChannel.GetUnreadCount() > 0)
                    {
                        return 1;
                    }
                    return -1;
                }
            }
            return 0;
        }

        public ListView<COMDT_FRIEND_INFO> GetAllFriend()
        {
            ListView<COMDT_FRIEND_INFO> view = this._map[1];
            ListView<COMDT_FRIEND_INFO> collection = this._map[4];
            ListView<COMDT_FRIEND_INFO> friendList = new ListView<COMDT_FRIEND_INFO>();
            friendList.AddRange(collection);
            for (int i = 0; i < view.Count; i++)
            {
                if (!Singleton<CFriendContoller>.instance.FilterSameFriend(view[i], friendList))
                {
                    friendList.Add(view[i]);
                }
            }
            return friendList;
        }

        public List<stBlackName> GetBlackList()
        {
            return this.m_blackList_friend;
        }

        public string GetBlackName(ulong ullUid, uint dwLogicWorldId)
        {
            for (int i = 0; i < this.m_blackList_friend.Count; i++)
            {
                stBlackName name = this.m_blackList_friend[i];
                if ((name.ullUid == ullUid) && (name.dwLogicWorldId == dwLogicWorldId))
                {
                    return name.name;
                }
            }
            return string.Empty;
        }

        public ListView<CSDT_LBS_USER_INFO> GetCurrentLBSList()
        {
            if (this.fileter == 3)
            {
                return this.GetLBSList(LBSGenderType.Both);
            }
            if (this.fileter == 2)
            {
                return this.GetLBSList(LBSGenderType.Nv);
            }
            if (this.fileter == 1)
            {
                return this.GetLBSList(LBSGenderType.Nan);
            }
            return null;
        }

        public int GetDataCount(FriendType type)
        {
            return this.GetList(type).Count;
        }

        private int GetFriendBlackIndex(ulong ullUid, uint dwLogicWorldID)
        {
            for (int i = 0; i < this.m_blackList_friend.Count; i++)
            {
                stBlackName name = this.m_blackList_friend[i];
                if ((name.ullUid == ullUid) && (name.dwLogicWorldId == dwLogicWorldID))
                {
                    return i;
                }
            }
            return -1;
        }

        public COMDT_FRIEND_INFO getFriendByName(string friendName, FriendType friendType)
        {
            ListView<COMDT_FRIEND_INFO> view = this._map[(uint) friendType];
            for (int i = 0; i < view.Count; i++)
            {
                if (Utility.UTF8Convert(view[i].szUserName) == friendName)
                {
                    return view[i];
                }
            }
            return null;
        }

        public COMDT_FRIEND_INFO getFriendByUid(ulong uid, FriendType friendType)
        {
            ListView<COMDT_FRIEND_INFO> view = this._map[(uint) friendType];
            for (int i = 0; i < view.Count; i++)
            {
                if (view[i].stUin.ullUid == uid)
                {
                    return view[i];
                }
            }
            return null;
        }

        private COMDT_FRIEND_INFO getFriendInfo(ulong ullUid, uint dwLogicWorldID, ListView<COMDT_FRIEND_INFO> list)
        {
            if (list == null)
            {
                return null;
            }
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                COMDT_FRIEND_INFO comdt_friend_info2 = list[i];
                if (((comdt_friend_info2 != null) && (comdt_friend_info2.stUin.ullUid == ullUid)) && (comdt_friend_info2.stUin.dwLogicWorldId == dwLogicWorldID))
                {
                    return comdt_friend_info2;
                }
            }
            return null;
        }

        public FriendInGame GetFriendInGaming(ulong ullUid, uint dwLogicWorldID)
        {
            FriendInGame game = null;
            for (int i = 0; i < this.gameStateList.Count; i++)
            {
                game = this.gameStateList[i];
                if (((game != null) && (game.ullUid == ullUid)) && (game.dwLogicWorldID == dwLogicWorldID))
                {
                    return game;
                }
            }
            return null;
        }

        public COM_ACNT_GAME_STATE GetFriendInGamingState(ulong ullUid, uint dwLogicWorldID)
        {
            FriendInGame game = null;
            for (int i = 0; i < this.gameStateList.Count; i++)
            {
                game = this.gameStateList[i];
                if (((game != null) && (game.ullUid == ullUid)) && (game.dwLogicWorldID == dwLogicWorldID))
                {
                    return game.State;
                }
            }
            return COM_ACNT_GAME_STATE.COM_ACNT_GAME_STATE_IDLE;
        }

        public bool GetFriendIntimacy(ulong ullUid, uint dwLogicWorldID, out ushort wIntimacyValue, out EIntimacyType type, out bool bFreeze)
        {
            for (int i = 0; i < this.m_stFriendIntimacyList.Count; i++)
            {
                stFriendIntimacy intimacy = this.m_stFriendIntimacyList[i];
                if ((intimacy.ullUid == ullUid) && (intimacy.dwLogicWorldID == dwLogicWorldID))
                {
                    wIntimacyValue = intimacy.wIntimacyValue;
                    type = intimacy.type;
                    bFreeze = intimacy.bFreeze;
                    return true;
                }
            }
            wIntimacyValue = 0;
            type = EIntimacyType.None;
            bFreeze = false;
            return false;
        }

        private int GetFriendIntimacyIndex(ulong ullUid, uint dwLogicWorldID)
        {
            for (int i = 0; i < this.m_stFriendIntimacyList.Count; i++)
            {
                stFriendIntimacy intimacy = this.m_stFriendIntimacyList[i];
                if ((intimacy.ullUid == ullUid) && (intimacy.dwLogicWorldID == dwLogicWorldID))
                {
                    return i;
                }
            }
            return -1;
        }

        public string GetFriendVerifyContent(ulong ullUid, uint dwLogicWorldID)
        {
            for (int i = 0; i < this.m_friendVerifyList.Count; i++)
            {
                stFriendVerifyContent content = this.m_friendVerifyList[i];
                if ((content.ullUid == ullUid) && (content.dwLogicWorldID == dwLogicWorldID))
                {
                    return content.content;
                }
            }
            return null;
        }

        private int GetFriendVerifyIndex(ulong ullUid, uint dwLogicWorldID)
        {
            for (int i = 0; i < this.m_friendVerifyList.Count; i++)
            {
                stFriendVerifyContent content = this.m_friendVerifyList[i];
                if ((content.ullUid == ullUid) && (content.dwLogicWorldID == dwLogicWorldID))
                {
                    return i;
                }
            }
            return -1;
        }

        public COMDT_FRIEND_INFO GetGameOrSnsFriend(ulong ullUid, uint dwLogicWorldID)
        {
            COMDT_FRIEND_INFO comdt_friend_info = this.getFriendInfo(ullUid, dwLogicWorldID, this.GetList(FriendType.GameFriend));
            if (comdt_friend_info == null)
            {
                comdt_friend_info = this.getFriendInfo(ullUid, dwLogicWorldID, this.GetList(FriendType.SNS));
            }
            return comdt_friend_info;
        }

        private int getIndex(COMDT_FRIEND_INFO info, ListView<COMDT_FRIEND_INFO> list, bool ingore_worldid = false)
        {
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (UT.BEqual_ACNT_UNIQ(list[i].stUin, info.stUin, ingore_worldid))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public COMDT_FRIEND_INFO GetInfo(FriendType type, COMDT_ACNT_UNIQ uniq)
        {
            return this.getFriendInfo(uniq.ullUid, uniq.dwLogicWorldId, this.GetList(type));
        }

        public COMDT_FRIEND_INFO GetInfo(FriendType type, ulong ullUid, uint dwLogicWorldID)
        {
            return this.getFriendInfo(ullUid, dwLogicWorldID, this.GetList(type));
        }

        public COMDT_FRIEND_INFO GetInfoAtIndex(FriendType type, int index)
        {
            ListView<COMDT_FRIEND_INFO> list = this.GetList(type);
            if ((list != null) && ((index >= 0) && (index < list.Count)))
            {
                return list[index];
            }
            return null;
        }

        private int GetItemIndex(ulong uid, uint worldID, ListView<COMDT_FRIEND_INFO> targetList)
        {
            if (targetList != null)
            {
                for (int i = 0; i < targetList.Count; i++)
                {
                    COMDT_FRIEND_INFO comdt_friend_info = targetList[i];
                    if ((comdt_friend_info != null) && ((comdt_friend_info.stUin.ullUid == uid) && (comdt_friend_info.stUin.dwLogicWorldId == worldID)))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public byte GetLBSGenterFilter()
        {
            if (this.fileter != 3)
            {
                if (this.fileter == 2)
                {
                    return 2;
                }
                if (this.fileter == 1)
                {
                    return 1;
                }
            }
            return 0;
        }

        public ListView<CSDT_LBS_USER_INFO> GetLBSList(LBSGenderType type = 0)
        {
            if (type == LBSGenderType.Both)
            {
                return this.m_LBSList;
            }
            if (type == LBSGenderType.Nan)
            {
                return this.m_LBSListNan;
            }
            if (type == LBSGenderType.Nv)
            {
                return this.m_LBSListNv;
            }
            return null;
        }

        public int GetLBSListIndex(ulong ullUid, uint dwLogicWorldId, LBSGenderType type = 0)
        {
            ListView<CSDT_LBS_USER_INFO> lBSList = this.GetLBSList(type);
            if (lBSList != null)
            {
                for (int i = 0; i < lBSList.Count; i++)
                {
                    CSDT_LBS_USER_INFO csdt_lbs_user_info = lBSList[i];
                    if (((csdt_lbs_user_info != null) && (csdt_lbs_user_info.stLbsUserInfo.stUin.ullUid == ullUid)) && (csdt_lbs_user_info.stLbsUserInfo.stUin.dwLogicWorldId == dwLogicWorldId))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public int GetLBSListIndex(ulong ullUid, uint dwLogicWorldId, ListView<CSDT_LBS_USER_INFO> list)
        {
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    CSDT_LBS_USER_INFO csdt_lbs_user_info = list[i];
                    if (((csdt_lbs_user_info != null) && (csdt_lbs_user_info.stLbsUserInfo.stUin.ullUid == ullUid)) && (csdt_lbs_user_info.stLbsUserInfo.stUin.dwLogicWorldId == dwLogicWorldId))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public CSDT_LBS_USER_INFO GetLBSUserInfo(ulong ullUid, uint dwLogicWorldId, LBSGenderType type = 0)
        {
            ListView<CSDT_LBS_USER_INFO> lBSList = this.GetLBSList(type);
            if (lBSList != null)
            {
                for (int i = 0; i < lBSList.Count; i++)
                {
                    CSDT_LBS_USER_INFO csdt_lbs_user_info = lBSList[i];
                    if (((csdt_lbs_user_info != null) && (csdt_lbs_user_info.stLbsUserInfo.stUin.ullUid == ullUid)) && (csdt_lbs_user_info.stLbsUserInfo.stUin.dwLogicWorldId == dwLogicWorldId))
                    {
                        return csdt_lbs_user_info;
                    }
                }
            }
            return null;
        }

        public ListView<COMDT_FRIEND_INFO> GetList(FriendType type)
        {
            return this._map[(uint) type];
        }

        public string GetName(ulong ullUid, uint dwLogicWorldId)
        {
            COMDT_FRIEND_INFO comdt_friend_info;
            COMDT_CHAT_PLAYER_INFO comdt_chat_player_info;
            this.GetUser(ullUid, dwLogicWorldId, out comdt_friend_info, out comdt_chat_player_info);
            if (comdt_friend_info != null)
            {
                return UT.Bytes2String(comdt_friend_info.szUserName);
            }
            if (comdt_chat_player_info != null)
            {
                return UT.Bytes2String(comdt_chat_player_info.szName);
            }
            return string.Empty;
        }

        public ListView<COMDT_FRIEND_INFO> GetOnlineFriendAndSnsFriendList()
        {
            ListView<COMDT_FRIEND_INFO> list = this.GetList(FriendType.GameFriend);
            ListView<COMDT_FRIEND_INFO> view2 = this.GetList(FriendType.SNS);
            this.cache_OnlineFriends_Results.Clear();
            for (int i = 0; i < view2.Count; i++)
            {
                if (OnlineFinder(view2[i]))
                {
                    this.cache_OnlineFriends_Results.Add(view2[i]);
                }
            }
            bool flag = false;
            for (int j = 0; j < list.Count; j++)
            {
                if (!OnlineFinder(list[j]))
                {
                    continue;
                }
                flag = false;
                for (int k = 0; k < this.cache_OnlineFriends_Results.Count; k++)
                {
                    if (this.cache_OnlineFriends_Results[k].stUin.ullUid == list[j].stUin.ullUid)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    this.cache_OnlineFriends_Results.Add(list[j]);
                }
            }
            return this.cache_OnlineFriends_Results;
        }

        public ListView<COMDT_FRIEND_INFO> GetOnlineFriendList()
        {
            FindAll(this.GetList(FriendType.GameFriend), new Predicate<COMDT_FRIEND_INFO>(CFriendModel.OnlineFinder), this.cache_OnlineFriends_Results);
            return this.cache_OnlineFriends_Results;
        }

        public string GetRandomVerifyContent()
        {
            if (this.m_preconfigVerifyContentList.Count == 0)
            {
                return null;
            }
            int num = UnityEngine.Random.Range(0, this.m_preconfigVerifyContentList.Count - 1);
            return this.m_preconfigVerifyContentList[num];
        }

        public ListView<COMDT_FRIEND_INFO> GetSortedRankingFriendList(COM_APOLLO_TRANK_SCORE_TYPE sortType)
        {
            this.SetSortType(sortType);
            this._rankingFriend.Sort(new Comparison<COMDT_FRIEND_INFO>(CFriendModel.RankingFriendSort));
            return this._rankingFriend;
        }

        public void GetUser(ulong ullUid, uint dwLogicWorldId, out COMDT_FRIEND_INFO friendInfo, out COMDT_CHAT_PLAYER_INFO chatInfo)
        {
            friendInfo = this.GetGameOrSnsFriend(ullUid, dwLogicWorldId);
            if (friendInfo == null)
            {
                chatInfo = Singleton<CChatController>.instance.model.Get_Palyer_Info(ullUid, dwLogicWorldId);
            }
            else
            {
                chatInfo = null;
            }
        }

        public ListView<COMDT_FRIEND_INFO> GetValidChatFriendList()
        {
            this.cache_chatFriends_Results.Clear();
            COMDT_FRIEND_INFO a = null;
            ListView<COMDT_FRIEND_INFO> list = this.GetList(FriendType.GameFriend);
            ListView<COMDT_FRIEND_INFO> view2 = this.GetList(FriendType.SNS);
            for (int i = 0; i < view2.Count; i++)
            {
                a = view2[i];
                if ((a != null) && (OnlineFinder(a) && (this.GetItemIndex(a.stUin.ullUid, a.stUin.dwLogicWorldId, this.cache_chatFriends_Results) == -1)))
                {
                    this.cache_chatFriends_Results.Add(a);
                }
            }
            for (int j = 0; j < list.Count; j++)
            {
                a = list[j];
                if ((a != null) && (OnlineFinder(a) && (this.GetItemIndex(a.stUin.ullUid, a.stUin.dwLogicWorldId, this.cache_chatFriends_Results) == -1)))
                {
                    this.cache_chatFriends_Results.Add(a);
                }
            }
            CChatChannel channel = null;
            ListView<CChatChannel> friendChannelList = Singleton<CChatController>.instance.model.channelMgr.FriendChannelList;
            for (int k = 0; k < friendChannelList.Count; k++)
            {
                channel = friendChannelList[k];
                if ((channel != null) && (channel.HasAnyValidChatEntity() && (this.GetItemIndex(channel.ullUid, channel.dwLogicWorldId, this.cache_chatFriends_Results) == -1)))
                {
                    COMDT_FRIEND_INFO gameOrSnsFriend = this.GetGameOrSnsFriend(channel.ullUid, channel.dwLogicWorldId);
                    if (gameOrSnsFriend != null)
                    {
                        this.cache_chatFriends_Results.Add(gameOrSnsFriend);
                    }
                }
            }
            return this.cache_chatFriends_Results;
        }

        public bool IsAnyFriendExist(bool requre_bOnline)
        {
            ListView<COMDT_FRIEND_INFO> list = this.GetList(FriendType.GameFriend);
            if (!requre_bOnline)
            {
                return (list.Count > 0);
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].bIsOnline == 1)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsBlack(ulong ullUid, uint dwLogicWorldId)
        {
            return (this.GetFriendBlackIndex(ullUid, dwLogicWorldId) != -1);
        }

        public bool IsContain(FriendType type, COMDT_FRIEND_INFO data)
        {
            ListView<COMDT_FRIEND_INFO> list = this.GetList(type);
            if (list == null)
            {
                return false;
            }
            return list.Contains(data);
        }

        public bool IsContain(FriendType type, ulong ullUid, uint dwLogicWorldID)
        {
            COMDT_FRIEND_INFO comdt_friend_info = null;
            ListView<COMDT_FRIEND_INFO> list = this.GetList(type);
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                comdt_friend_info = list[i];
                if (((comdt_friend_info != null) && (comdt_friend_info.stUin.ullUid == ullUid)) && (comdt_friend_info.stUin.dwLogicWorldId == dwLogicWorldID))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsFriendInGamingState(ulong ullUid, uint dwLogicWorldID, COM_ACNT_GAME_STATE State)
        {
            FriendInGame game = null;
            for (int i = 0; i < this.gameStateList.Count; i++)
            {
                game = this.gameStateList[i];
                if ((game.ullUid == ullUid) && (game.dwLogicWorldID == dwLogicWorldID))
                {
                    return (game.State == State);
                }
            }
            return false;
        }

        public bool IsFriendOfflineOnline(ulong ullUid, uint dwLogicWorldID)
        {
            ListView<COMDT_FRIEND_INFO> list = this.GetList(FriendType.GameFriend);
            ListView<COMDT_FRIEND_INFO> view2 = this.GetList(FriendType.SNS);
            COMDT_FRIEND_INFO comdt_friend_info = null;
            for (int i = 0; i < list.Count; i++)
            {
                comdt_friend_info = list[i];
                if ((comdt_friend_info != null) && ((comdt_friend_info.stUin.ullUid == ullUid) && (comdt_friend_info.stUin.dwLogicWorldId == dwLogicWorldID)))
                {
                    return (comdt_friend_info.bIsOnline == 1);
                }
            }
            for (int j = 0; j < view2.Count; j++)
            {
                comdt_friend_info = view2[j];
                if ((comdt_friend_info != null) && ((comdt_friend_info.stUin.ullUid == ullUid) && (comdt_friend_info.stUin.dwLogicWorldId == dwLogicWorldID)))
                {
                    return (comdt_friend_info.bIsOnline == 1);
                }
            }
            DebugHelper.Assert(false, "---IsFriendOfflineOnline should reach this, check out...");
            return false;
        }

        public bool IsGameFriend(ulong ullUid, uint logicWorldId)
        {
            ListView<COMDT_FRIEND_INFO> view = this._map[1];
            for (int i = 0; i < view.Count; i++)
            {
                if ((view[i].stUin.ullUid == ullUid) && (view[i].stUin.dwLogicWorldId == logicWorldId))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsOnSnsSwitch(uint switchBits, COM_REFUSE_TYPE type)
        {
            return ((switchBits & (((int) 1) << type)) > 0L);
        }

        public bool IsSnsFriend(ulong ullUid, uint logicWorldId)
        {
            ListView<COMDT_FRIEND_INFO> view = this._map[4];
            for (int i = 0; i < view.Count; i++)
            {
                if ((view[i].stUin.ullUid == ullUid) && (view[i].stUin.dwLogicWorldId == logicWorldId))
                {
                    return true;
                }
            }
            return false;
        }

        public static int LBSDataSort(CSDT_LBS_USER_INFO l, CSDT_LBS_USER_INFO r)
        {
            if (l == r)
            {
                return 0;
            }
            if ((l == null) || (r == null))
            {
                return 0;
            }
            if (r.dwDistance != l.dwDistance)
            {
                return (int) (l.dwDistance - r.dwDistance);
            }
            if (r.bGradeOfRank != l.bGradeOfRank)
            {
                return (r.bGradeOfRank - l.bGradeOfRank);
            }
            if (r.stLbsUserInfo.stGameVip.dwCurLevel == l.stLbsUserInfo.stGameVip.dwCurLevel)
            {
                return (int) (r.stLbsUserInfo.dwPvpLvl - l.stLbsUserInfo.dwPvpLvl);
            }
            return (int) (r.stLbsUserInfo.stGameVip.dwCurLevel - l.stLbsUserInfo.stGameVip.dwCurLevel);
        }

        public void LoadIntimacyConfig()
        {
            if (!int.TryParse(Singleton<CTextManager>.instance.GetText("Intimacy_FreezeDay"), out this.freezeDayCount))
            {
                DebugHelper.Assert(false, "---Intimacy  Intimacy_FreezeDay 对应的配置项 好像不是整数哦， check out");
            }
            if (!int.TryParse(Singleton<CTextManager>.instance.GetText("Intimacy_CondLow"), out this.cond1))
            {
                DebugHelper.Assert(false, "---Intimacy  Intimacy_CondLow 对应的配置项 好像不是整数哦， check out");
            }
            if (!int.TryParse(Singleton<CTextManager>.instance.GetText("Intimacy_CondMiddle"), out this.cond2))
            {
                DebugHelper.Assert(false, "---Intimacy  Intimacy_CondMiddle 对应的配置项 好像不是整数哦， check out");
            }
            this.fullIntimacy = 0x3e7;
        }

        public void LoadPreconfigVerifyContentList()
        {
            int num = 0;
            bool flag = true;
            while (flag)
            {
                string key = string.Format("FriendVerify_Text_{0}", num);
                string text = Singleton<CTextManager>.instance.GetText(key);
                if (string.Equals(key, text))
                {
                    flag = false;
                }
                else
                {
                    if (!this.m_preconfigVerifyContentList.Contains(text))
                    {
                        this.m_preconfigVerifyContentList.Add(text);
                    }
                    num++;
                }
            }
            this.friend_static_text = Singleton<CTextManager>.instance.GetText("FriendVerify_StaticText");
        }

        public uint NegFlag(uint value, int flag)
        {
            uint num = ((uint) 1) << flag;
            if ((value & num) != 0)
            {
                value &= ~num;
                return value;
            }
            value |= num;
            return value;
        }

        public static bool OnlineFinder(COMDT_FRIEND_INFO a)
        {
            return (a.bIsOnline == 1);
        }

        private static int RankingFriendSort(COMDT_FRIEND_INFO l, COMDT_FRIEND_INFO r)
        {
            int index = (int) _lastSortTpye;
            if (_lastSortTpye != COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_LADDER_POINT)
            {
                return -l.RankVal[index].CompareTo(r.RankVal[index]);
            }
            ulong dwRankClass = l.dwRankClass;
            ulong dwPvpLvl = r.dwRankClass;
            if ((dwRankClass > 0L) && (dwPvpLvl == 0))
            {
                return -1;
            }
            if ((dwRankClass == 0) && (dwPvpLvl > 0L))
            {
                return 1;
            }
            dwRankClass = l.RankVal[index];
            dwPvpLvl = r.RankVal[index];
            if (dwPvpLvl == dwRankClass)
            {
                dwRankClass = l.dwPvpLvl;
                dwPvpLvl = r.dwPvpLvl;
            }
            if (dwPvpLvl == dwRankClass)
            {
                dwRankClass = l.stUin.ullUid;
                dwPvpLvl = r.stUin.ullUid;
            }
            return -dwRankClass.CompareTo(dwPvpLvl);
        }

        public void Remove(FriendType type, COMDT_ACNT_UNIQ uniq)
        {
            COMDT_FRIEND_INFO info = this.GetInfo(type, uniq);
            this.Remove(type, info);
            if (type == FriendType.GameFriend)
            {
                this.RemoveIntimacy(uniq.ullUid, uniq.dwLogicWorldId);
            }
        }

        public void Remove(FriendType type, COMDT_FRIEND_INFO data)
        {
            this.RemoveRankingFriend(type, data);
            this.GetList(type).Remove(data);
            if (type == FriendType.RequestFriend)
            {
                Singleton<EventRouter>.instance.BroadCastEvent("Friend_LobbyIconRedDot_Refresh");
            }
            if (type == FriendType.Recommend)
            {
                Singleton<EventRouter>.instance.BroadCastEvent("Friend_RecommandFriend_Refresh");
            }
        }

        public void Remove(FriendType type, ulong ullUid, uint dwLogicWorldId)
        {
            COMDT_FRIEND_INFO data = this.GetInfo(type, ullUid, dwLogicWorldId);
            this.Remove(type, data);
        }

        public void RemoveFriendBlack(COMDT_FRIEND_INFO info)
        {
            int friendBlackIndex = this.GetFriendBlackIndex(info.stUin.ullUid, info.stUin.dwLogicWorldId);
            if (friendBlackIndex != -1)
            {
                this.m_blackList_friend.RemoveAt(friendBlackIndex);
            }
        }

        public void RemoveFriendBlack(ulong ullUid, uint dwLogicWorldID)
        {
            int friendBlackIndex = this.GetFriendBlackIndex(ullUid, dwLogicWorldID);
            if (friendBlackIndex != -1)
            {
                this.m_blackList_friend.RemoveAt(friendBlackIndex);
            }
        }

        public void RemoveFriendVerifyContent(ulong ullUid, uint dwLogicWorldID)
        {
            int friendVerifyIndex = this.GetFriendVerifyIndex(ullUid, dwLogicWorldID);
            if (friendVerifyIndex != -1)
            {
                this.m_friendVerifyList.RemoveAt(friendVerifyIndex);
            }
        }

        public void RemoveIntimacy(ulong ullUid, uint dwLogicWorldID)
        {
            int friendIntimacyIndex = this.GetFriendIntimacyIndex(ullUid, dwLogicWorldID);
            if (friendIntimacyIndex != -1)
            {
                this.m_stFriendIntimacyList.RemoveAt(friendIntimacyIndex);
            }
        }

        public void RemoveLBSUser(ulong ullUid, uint dwLogicWorldId)
        {
            this.RemoveLBSUser(ullUid, dwLogicWorldId, this.m_LBSList);
            this.RemoveLBSUser(ullUid, dwLogicWorldId, this.m_LBSListNan);
            this.RemoveLBSUser(ullUid, dwLogicWorldId, this.m_LBSListNv);
        }

        private void RemoveLBSUser(ulong ullUid, uint dwLogicWorldId, ListView<CSDT_LBS_USER_INFO> list)
        {
            if (list != null)
            {
                int index = this.GetLBSListIndex(ullUid, dwLogicWorldId, LBSGenderType.Both);
                if (index != -1)
                {
                    list.RemoveAt(index);
                }
            }
        }

        public void RemoveRankingFriend(FriendType type, COMDT_FRIEND_INFO data)
        {
            if ((type == FriendType.SNS) || (type == FriendType.GameFriend))
            {
                for (int i = 0; i < this._rankingFriend.Count; i++)
                {
                    if ((this._rankingFriend[i].stUin.ullUid == data.stUin.ullUid) && (this._rankingFriend[i].stUin.dwLogicWorldId == data.stUin.dwLogicWorldId))
                    {
                        this._rankingFriend.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void SetFriendGameState(ulong ullUid, uint dwLogicWorldID, COM_ACNT_GAME_STATE State, uint startTime, string nickName = "", bool IgnoreWorld_id = false)
        {
            FriendInGame game = null;
            for (int i = 0; i < this.gameStateList.Count; i++)
            {
                game = this.gameStateList[i];
                bool flag = false;
                if (!IgnoreWorld_id)
                {
                    flag = (game.ullUid == ullUid) && (game.dwLogicWorldID == dwLogicWorldID);
                }
                else
                {
                    flag = game.ullUid == ullUid;
                }
                if (flag)
                {
                    game.State = State;
                    game.dwLogicWorldID = dwLogicWorldID;
                    game.startTime = startTime;
                    return;
                }
            }
            this.gameStateList.Add(new FriendInGame(ullUid, dwLogicWorldID, State, startTime, nickName));
        }

        public void SetGameFriendGuildState(ulong uid, COM_PLAYER_GUILD_STATE guildState)
        {
            ListView<COMDT_FRIEND_INFO> list = this.GetList(FriendType.GameFriend);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].stUin.ullUid == uid)
                {
                    list[i].bGuildState = (byte) guildState;
                }
            }
        }

        public void SetSortType(COM_APOLLO_TRANK_SCORE_TYPE sortTpye)
        {
            _lastSortTpye = sortTpye;
        }

        public void SortGameFriend()
        {
            this.GetList(FriendType.GameFriend).Sort(new Comparison<COMDT_FRIEND_INFO>(CFriendModel.FriendDataSort));
        }

        public void SortLBSFriend()
        {
            this.m_LBSList.Sort(new Comparison<CSDT_LBS_USER_INFO>(CFriendModel.LBSDataSort));
            this.m_LBSListNan.Sort(new Comparison<CSDT_LBS_USER_INFO>(CFriendModel.LBSDataSort));
            this.m_LBSListNv.Sort(new Comparison<CSDT_LBS_USER_INFO>(CFriendModel.LBSDataSort));
        }

        public void SortSNSFriend()
        {
            this.GetList(FriendType.SNS).Sort(new Comparison<COMDT_FRIEND_INFO>(CFriendModel.FriendDataSort));
        }

        public bool EnableShareLocation
        {
            get
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                if (masterRoleInfo == null)
                {
                    return false;
                }
                uint num = 4;
                uint num2 = masterRoleInfo.snsSwitchBits & num;
                return (num2 > 0);
            }
            set
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    uint num = 4;
                    if (value)
                    {
                        masterRoleInfo.snsSwitchBits |= num;
                    }
                    else
                    {
                        masterRoleInfo.snsSwitchBits &= ~num;
                    }
                }
            }
        }

        public uint fileter
        {
            get
            {
                return this._fileter;
            }
            set
            {
                this._fileter = value;
            }
        }

        public enum EIntimacyType
        {
            full = 5,
            High = 4,
            Low = 2,
            Middle = 3,
            None = -1
        }

        public class FriendInGame
        {
            public uint dwLogicWorldID;
            public string nickName;
            public uint startTime;
            public COM_ACNT_GAME_STATE State;
            public ulong ullUid;

            public FriendInGame(ulong uid, uint worldID, COM_ACNT_GAME_STATE state, uint startTime, string nickName = "")
            {
                this.ullUid = uid;
                this.dwLogicWorldID = worldID;
                this.State = state;
                this.nickName = CUIUtility.RemoveEmoji(nickName);
                this.startTime = startTime;
            }

            public string NickName
            {
                get
                {
                    if (CFriendModel.RemarkNames.ContainsKey(this.ullUid))
                    {
                        string str = string.Empty;
                        CFriendModel.RemarkNames.TryGetValue(this.ullUid, out str);
                        if (!string.IsNullOrEmpty(str))
                        {
                            return str;
                        }
                    }
                    return this.nickName;
                }
            }
        }

        public enum FriendType
        {
            BlackList = 5,
            GameFriend = 1,
            Recommend = 3,
            RequestFriend = 2,
            SNS = 4
        }

        public enum LBSGenderType
        {
            Both,
            Nan,
            Nv
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stBlackName
        {
            public ulong ullUid;
            public uint dwLogicWorldId;
            public string name;
            public byte bIsOnline;
            public uint dwLastLoginTime;
            public byte bGender;
            public string szHeadUrl;
            public uint dwPvpLvl;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stFriendIntimacy
        {
            public ulong ullUid;
            public uint dwLogicWorldID;
            public uint dwLastIntimacyTime;
            public ushort wIntimacyValue;
            public CFriendModel.EIntimacyType type;
            public bool bFreeze;
            public stFriendIntimacy(ulong ullUid, uint dwLogicWorldID, uint dwLastIntimacyTime, ushort wIntimacyValue)
            {
                this.ullUid = ullUid;
                this.dwLogicWorldID = dwLogicWorldID;
                this.dwLastIntimacyTime = dwLastIntimacyTime;
                this.wIntimacyValue = wIntimacyValue;
                this.bFreeze = UT.IsFreeze(dwLastIntimacyTime);
                this.type = Singleton<CFriendContoller>.instance.model.CalcType(wIntimacyValue);
            }
        }
    }
}

