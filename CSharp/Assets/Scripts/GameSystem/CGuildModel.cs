namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization.Formatters.Binary;

    public class CGuildModel : Singleton<CGuildModel>
    {
        public const string APPLIED_GUILD_DIC_BIN_FILE_PRE = "applyed_guild_02_";
        public const string INVITED_FRIEND_DIC_BIN_FILE_PRE = "invited_friend_02_";
        private Dictionary<ulong, stApplicantInfo> m_ApplicantDic = new Dictionary<ulong, stApplicantInfo>();
        private Dictionary<ulong, stAppliedGuildInfo> m_AppliedGuildDic = new Dictionary<ulong, stAppliedGuildInfo>();
        private GuildInfo m_CurrentGuildInfo = new GuildInfo();
        private PrepareGuildInfo m_CurrentPrepareGuildInfo = new PrepareGuildInfo();
        private DictionaryView<ulong, GuildInfo> m_GuildDic = new DictionaryView<ulong, GuildInfo>();
        private Dictionary<ulong, stInvitedFriend> m_InvitedFriendDic = new Dictionary<ulong, stInvitedFriend>();
        public ulong m_InviteGuildUuid = 0L;
        public ulong m_InvitePlayerUuid = 0L;
        private Dictionary<ulong, int> m_inviteTimeInfoDic = new Dictionary<ulong, int>();
        public string m_InvitGuildName = null;
        public bool m_IsInited = false;
        private bool m_IsLocalDataInited = false;
        public int m_LastApplicantListRequestTime;
        public int m_LastGuildListRequestTime;
        public int m_LastPrepareGuildListRequestTime;
        public int m_LastRequestJoinGuildTime;
        public int m_LastRequestJoinPrepareGuildTime;
        public COM_PLAYER_GUILD_STATE m_PlayerGuildLastState = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL;
        private DictionaryView<ulong, PrepareGuildInfo> m_PrepareGuildDic = new DictionaryView<ulong, PrepareGuildInfo>();
        public uint m_PrepareGuildOldestRequestTime = uint.MaxValue;
        private Dictionary<ulong, stRecommendInfo> m_recommendInfoDic = new Dictionary<ulong, stRecommendInfo>();
        private Dictionary<ulong, int> m_recommendTimeInfoDic = new Dictionary<ulong, int>();
        public int m_RequestJoinGuildCnt;
        public const int RankpointRankTimeLimit = 300;
        public const byte REQUEST_APPLICANT_LIST_CACHE_TIME = 10;
        public const byte REQUEST_GUILD_LIST_CACHE_TIME = 10;
        public const byte REQUEST_PREPARE_GUILD_LIST_CACHE_TIME = 10;
        public const int STATE_SYNC_INTERVAL = 0x1388;

        public CGuildModel()
        {
            this.RankpointMemberInfoList = new List<KeyValuePair<ulong, MemberRankInfo>>();
            this.RankpointRankInfoLists = new ListView<RankpointRankInfo>[4];
            for (int i = 0; i < this.RankpointRankInfoLists.Length; i++)
            {
                this.RankpointRankInfoLists[i] = new ListView<RankpointRankInfo>();
            }
            this.RankpointRankGottens = new bool[4];
            this.RankpointRankLastGottenTimes = new int[4];
        }

        public void AddApplicant(stApplicantInfo applicant)
        {
            if (!this.m_ApplicantDic.ContainsKey(applicant.stBriefInfo.uulUid))
            {
                try
                {
                    this.m_ApplicantDic.Add(applicant.stBriefInfo.uulUid, applicant);
                }
                catch (Exception)
                {
                }
            }
        }

        public void AddApplicants(List<stApplicantInfo> applicants)
        {
            int count = applicants.Count;
            for (int i = 0; i < count; i++)
            {
                this.AddApplicant(applicants[i]);
            }
        }

        public byte AddAppliedGuildInfo(stAppliedGuildInfo info, bool persist = true)
        {
            try
            {
                this.m_AppliedGuildDic.Add(info.stBriefInfo.uulUid, info);
                if (persist)
                {
                    this.WriteAppliedGuildDicToBinFile();
                }
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public void AddGuildInfo(GuildInfo info)
        {
            if (!this.m_GuildDic.ContainsKey(info.stBriefInfo.uulUid))
            {
                this.m_GuildDic.Add(info.stBriefInfo.uulUid, info);
            }
        }

        public void AddGuildInfoList(ListView<GuildInfo> guildInfoList)
        {
            int count = guildInfoList.Count;
            for (int i = 0; i < count; i++)
            {
                this.AddGuildInfo(guildInfoList[i]);
            }
        }

        public byte AddInvitedFriend(stInvitedFriend friend, bool persist = true)
        {
            if (!this.m_InvitedFriendDic.ContainsKey(friend.uulUid))
            {
                try
                {
                    this.m_InvitedFriendDic.Add(friend.uulUid, friend);
                    if (persist)
                    {
                        this.WriteInvitedFriendDicToBinFile();
                    }
                    return 1;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            return 0;
        }

        public void AddInviteTimeInfo(ulong uid, int inviteTime)
        {
            if (!this.m_inviteTimeInfoDic.ContainsKey(uid))
            {
                this.m_inviteTimeInfoDic.Add(uid, inviteTime);
            }
        }

        public void AddPrepareGuildInfo(PrepareGuildInfo info)
        {
            try
            {
                this.m_PrepareGuildDic.Add(info.stBriefInfo.uulUid, info);
            }
            catch (Exception)
            {
            }
        }

        public void AddPrepareGuildInfoList(ListView<PrepareGuildInfo> prepareGuildInfoList)
        {
            int num = 0;
            int count = prepareGuildInfoList.Count;
            while (num < count)
            {
                this.AddPrepareGuildInfo(prepareGuildInfoList[num]);
                num++;
            }
        }

        public void AddRecommendInfo(stRecommendInfo info)
        {
            if (!this.m_recommendInfoDic.ContainsKey(info.uid))
            {
                this.m_recommendInfoDic.Add(info.uid, info);
            }
        }

        public void AddRecommendInfoList(List<stRecommendInfo> infoList)
        {
            for (int i = 0; i < infoList.Count; i++)
            {
                this.AddRecommendInfo(infoList[i]);
            }
        }

        public void AddRecommendTimeInfo(ulong uid, int recommendTime)
        {
            if (!this.m_recommendTimeInfoDic.ContainsKey(uid))
            {
                this.m_recommendTimeInfoDic.Add(uid, recommendTime);
            }
        }

        public void AddSelfRecommendInfo(ulong uid, uint time)
        {
            for (int i = 0; i < this.CurrentGuildInfo.listSelfRecommendInfo.Count; i++)
            {
                if (this.CurrentGuildInfo.listSelfRecommendInfo[i].uid == uid)
                {
                    this.CurrentGuildInfo.listSelfRecommendInfo[i].time = time;
                    return;
                }
            }
            GuildSelfRecommendInfo item = new GuildSelfRecommendInfo {
                uid = uid,
                time = time
            };
            this.CurrentGuildInfo.listSelfRecommendInfo.Add(item);
        }

        public void ClearAppliedGuildDic()
        {
            if (this.m_AppliedGuildDic != null)
            {
                this.m_AppliedGuildDic.Clear();
            }
        }

        public void ClearGuildInfoList()
        {
            this.m_GuildDic.Clear();
        }

        public void ClearInvitedFriendDic()
        {
            if (this.m_InvitedFriendDic != null)
            {
                this.m_InvitedFriendDic.Clear();
            }
        }

        public void ClearPrepareGuildInfoList()
        {
            this.m_PrepareGuildDic.Clear();
        }

        public stApplicantInfo GetApplicantByUid(ulong uid)
        {
            stApplicantInfo info = new stApplicantInfo();
            if (this.m_ApplicantDic.ContainsKey(uid))
            {
                this.m_ApplicantDic.TryGetValue(uid, out info);
            }
            return info;
        }

        public List<stApplicantInfo> GetApplicants()
        {
            List<stApplicantInfo> list = new List<stApplicantInfo>(this.m_ApplicantDic.Count);
            Dictionary<ulong, stApplicantInfo>.Enumerator enumerator = this.m_ApplicantDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<ulong, stApplicantInfo> current = enumerator.Current;
                list.Add(current.Value);
            }
            return list;
        }

        public int GetApplicantsCount()
        {
            return ((this.m_ApplicantDic != null) ? this.m_ApplicantDic.Count : 0);
        }

        public Dictionary<ulong, stAppliedGuildInfo> GetAppliedGuildDic()
        {
            if (!this.m_IsInited)
            {
                this.InitLoginData();
            }
            if (!this.m_IsLocalDataInited)
            {
                this.InitLocalData();
            }
            return this.m_AppliedGuildDic;
        }

        public stAppliedGuildInfo GetAppliedGuildInfoByUid(ulong uulUid)
        {
            if (!this.m_IsInited)
            {
                this.InitLoginData();
            }
            if (!this.m_IsLocalDataInited)
            {
                this.InitLocalData();
            }
            stAppliedGuildInfo info = new stAppliedGuildInfo();
            uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey((uint) 9).dwConfValue;
            int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
            if (this.m_AppliedGuildDic.ContainsKey(uulUid))
            {
                this.m_AppliedGuildDic.TryGetValue(uulUid, out info);
                if (info.stBriefInfo.uulUid == 0)
                {
                    return info;
                }
                if ((info.dwApplyTime + dwConfValue) < currentUTCTime)
                {
                    this.m_AppliedGuildDic.Remove(info.stBriefInfo.uulUid);
                    this.WriteAppliedGuildDicToBinFile();
                    return new stAppliedGuildInfo();
                }
            }
            return info;
        }

        public GuildInfo GetGuildInfoByIndex(int idx)
        {
            if ((idx >= 0) && (idx < this.m_GuildDic.Count))
            {
                int num = 0;
                DictionaryView<ulong, GuildInfo>.Enumerator enumerator = this.m_GuildDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (num == idx)
                    {
                        KeyValuePair<ulong, GuildInfo> current = enumerator.Current;
                        return current.Value;
                    }
                    num++;
                }
            }
            return null;
        }

        public int GetGuildInfoCount()
        {
            return this.m_GuildDic.Count;
        }

        public GuildMemInfo GetGuildMemberInfoByName(string name)
        {
            for (int i = 0; i < this.CurrentGuildInfo.listMemInfo.Count; i++)
            {
                if (this.CurrentGuildInfo.listMemInfo[i].stBriefInfo.sName == name)
                {
                    return this.CurrentGuildInfo.listMemInfo[i];
                }
            }
            return null;
        }

        public GuildMemInfo GetGuildMemberInfoByUid(ulong uid)
        {
            for (int i = 0; i < this.CurrentGuildInfo.listMemInfo.Count; i++)
            {
                if (this.CurrentGuildInfo.listMemInfo[i].stBriefInfo.uulUid == uid)
                {
                    return this.CurrentGuildInfo.listMemInfo[i];
                }
            }
            return null;
        }

        public stInvitedFriend GetInvitedFriendByUid(ulong uulUid)
        {
            if (!this.m_IsInited)
            {
                this.InitLoginData();
            }
            if (!this.m_IsLocalDataInited)
            {
                this.InitLocalData();
            }
            stInvitedFriend friend = new stInvitedFriend();
            if (this.m_InvitedFriendDic.ContainsKey(uulUid))
            {
                this.m_InvitedFriendDic.TryGetValue(uulUid, out friend);
            }
            return friend;
        }

        public Dictionary<ulong, stInvitedFriend> GetInvitedFriendDic()
        {
            if (!this.m_IsInited)
            {
                this.InitLoginData();
            }
            if (!this.m_IsLocalDataInited)
            {
                this.InitLocalData();
            }
            return this.m_InvitedFriendDic;
        }

        public int GetInviteTimeInfoByUid(ulong uid)
        {
            return (!this.m_inviteTimeInfoDic.ContainsKey(uid) ? -1 : this.m_inviteTimeInfoDic[uid]);
        }

        public GuildMemInfo GetPlayerGuildMemberInfo()
        {
            for (int i = 0; i < this.CurrentGuildInfo.listMemInfo.Count; i++)
            {
                if (this.CurrentGuildInfo.listMemInfo[i].stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
                {
                    return this.CurrentGuildInfo.listMemInfo[i];
                }
            }
            return null;
        }

        public PrepareGuildInfo GetPrepareGuildInfoByIndex(int idx)
        {
            if ((idx >= 0) && (idx < this.m_PrepareGuildDic.Count))
            {
                int num = 0;
                DictionaryView<ulong, PrepareGuildInfo>.Enumerator enumerator = this.m_PrepareGuildDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (num == idx)
                    {
                        KeyValuePair<ulong, PrepareGuildInfo> current = enumerator.Current;
                        return current.Value;
                    }
                    num++;
                }
            }
            return null;
        }

        public int GetPrepareGuildInfoCount()
        {
            return this.m_PrepareGuildDic.Count;
        }

        public List<stRecommendInfo> GetRecommendInfo()
        {
            List<stRecommendInfo> list = new List<stRecommendInfo>(this.m_recommendInfoDic.Count);
            Dictionary<ulong, stRecommendInfo>.Enumerator enumerator = this.m_recommendInfoDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<ulong, stRecommendInfo> current = enumerator.Current;
                list.Add(current.Value);
            }
            return list;
        }

        public stRecommendInfo GetRecommendInfoByUid(ulong uulUid)
        {
            stRecommendInfo info = new stRecommendInfo();
            if (this.m_recommendInfoDic.ContainsKey(uulUid))
            {
                this.m_recommendInfoDic.TryGetValue(uulUid, out info);
                return info;
            }
            return info;
        }

        public int GetRecommendInfoCount()
        {
            return ((this.m_recommendInfoDic != null) ? this.m_recommendInfoDic.Count : 0);
        }

        public int GetRecommendTimeInfoByUid(ulong uid)
        {
            return (!this.m_recommendTimeInfoDic.ContainsKey(uid) ? -1 : this.m_recommendTimeInfoDic[uid]);
        }

        private void InitLocalData()
        {
            this.ReadAppliedGuildDicFromBinFile();
            this.ReadInvitedFriendDicFromBinFile();
            this.m_IsLocalDataInited = true;
        }

        public void InitLoginData()
        {
            if (!this.m_IsInited)
            {
                this.m_PlayerGuildLastState = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState;
                this.m_IsInited = true;
            }
        }

        public bool IsInGuildStep()
        {
            if (!this.m_IsInited)
            {
                this.InitLoginData();
            }
            return (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState != COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL);
        }

        private void ReadAppliedGuildDicFromBinFile()
        {
            string cachePath = CFileManager.GetCachePath("applyed_guild_02_" + Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID);
            FileStream serializationStream = null;
            if (CFileManager.IsFileExist(cachePath))
            {
                try
                {
                    serializationStream = new FileStream(cachePath, FileMode.Open, FileAccess.Read, FileShare.None);
                    BinaryFormatter formatter = new BinaryFormatter();
                    this.m_AppliedGuildDic = (Dictionary<ulong, stAppliedGuildInfo>) formatter.Deserialize(serializationStream);
                    serializationStream.Close();
                    serializationStream.Dispose();
                }
                catch (Exception)
                {
                    if (serializationStream != null)
                    {
                        serializationStream.Close();
                        serializationStream.Dispose();
                    }
                }
            }
        }

        private void ReadInvitedFriendDicFromBinFile()
        {
            string cachePath = CFileManager.GetCachePath("invited_friend_02_" + Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID);
            FileStream serializationStream = null;
            if (CFileManager.IsFileExist(cachePath))
            {
                try
                {
                    serializationStream = new FileStream(cachePath, FileMode.Open, FileAccess.Read, FileShare.None);
                    BinaryFormatter formatter = new BinaryFormatter();
                    this.m_InvitedFriendDic = (Dictionary<ulong, stInvitedFriend>) formatter.Deserialize(serializationStream);
                    serializationStream.Close();
                    serializationStream.Dispose();
                }
                catch (Exception)
                {
                    if (serializationStream != null)
                    {
                        serializationStream.Close();
                        serializationStream.Dispose();
                    }
                }
            }
        }

        public void RemoveApplicant(ulong uulUid)
        {
            if (this.m_ApplicantDic.ContainsKey(uulUid))
            {
                try
                {
                    this.m_ApplicantDic.Remove(uulUid);
                }
                catch (Exception)
                {
                }
            }
        }

        public void RemoveRecommendInfo(ulong uulUid)
        {
            if (this.m_recommendInfoDic.ContainsKey(uulUid))
            {
                this.m_recommendInfoDic.Remove(uulUid);
            }
        }

        public void RemoveSelfRecommendInfo(ulong uid)
        {
            for (int i = 0; i < this.CurrentGuildInfo.listSelfRecommendInfo.Count; i++)
            {
                if (this.CurrentGuildInfo.listSelfRecommendInfo[i].uid == uid)
                {
                    this.CurrentGuildInfo.listSelfRecommendInfo.RemoveAt(i);
                    return;
                }
            }
        }

        public void ResetCurrentGuildInfo()
        {
            if (this.m_CurrentGuildInfo != null)
            {
                this.m_CurrentGuildInfo.Reset();
            }
        }

        public void ResetCurrentPrepareGuildInfo()
        {
            if (this.m_CurrentPrepareGuildInfo != null)
            {
                this.m_CurrentPrepareGuildInfo.Reset();
            }
        }

        public void SetBuildingInfoList(GuildBuildingInfo buildingInfo)
        {
            for (int i = 0; i < this.CurrentGuildInfo.listBuildingInfo.Count; i++)
            {
                if (this.CurrentGuildInfo.listBuildingInfo[i].type == buildingInfo.type)
                {
                    this.CurrentGuildInfo.listBuildingInfo[i].level = buildingInfo.level;
                    return;
                }
            }
            this.CurrentGuildInfo.listBuildingInfo.Add(buildingInfo);
        }

        public void SetPlayerGuildStateToTemp()
        {
            Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_WAIT_RESULT;
            Singleton<CTimerManager>.GetInstance().AddTimer(0x1388, 1, new CTimer.OnTimeUpHandler(this.UpdateGuildState));
        }

        public void UpdateGuildState(int seq)
        {
            Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo.guildState = this.m_PlayerGuildLastState;
        }

        private void WriteAppliedGuildDicToBinFile()
        {
            string cachePath = CFileManager.GetCachePath("applyed_guild_02_" + Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID);
            FileStream serializationStream = null;
            try
            {
                if (!CFileManager.IsFileExist(cachePath))
                {
                    serializationStream = new FileStream(cachePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                }
                else
                {
                    serializationStream = new FileStream(cachePath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
                }
                new BinaryFormatter().Serialize(serializationStream, this.m_AppliedGuildDic);
                serializationStream.Flush();
                serializationStream.Close();
                serializationStream.Dispose();
            }
            catch (Exception)
            {
                if (serializationStream != null)
                {
                    serializationStream.Close();
                    serializationStream.Dispose();
                }
            }
        }

        private void WriteInvitedFriendDicToBinFile()
        {
            string cachePath = CFileManager.GetCachePath("invited_friend_02_" + Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID);
            FileStream serializationStream = null;
            try
            {
                if (!CFileManager.IsFileExist(cachePath))
                {
                    serializationStream = new FileStream(cachePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                }
                else
                {
                    serializationStream = new FileStream(cachePath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
                }
                new BinaryFormatter().Serialize(serializationStream, this.m_InvitedFriendDic);
                serializationStream.Flush();
                serializationStream.Close();
                serializationStream.Dispose();
            }
            catch (Exception)
            {
                if (serializationStream != null)
                {
                    serializationStream.Close();
                    serializationStream.Dispose();
                }
            }
        }

        public RES_GUILD_DONATE_TYPE CurrentDonateType { get; set; }

        public GuildInfo CurrentGuildInfo
        {
            get
            {
                return this.m_CurrentGuildInfo;
            }
            set
            {
                this.m_CurrentGuildInfo = value;
            }
        }

        public PrepareGuildInfo CurrentPrepareGuildInfo
        {
            get
            {
                return this.m_CurrentPrepareGuildInfo;
            }
            set
            {
                this.m_CurrentPrepareGuildInfo = value;
            }
        }

        public GuildMemInfo CurrentSelectedMemberInfo { get; set; }

        public bool IsApplyAndRecommendListLastPage { get; set; }

        public bool IsLocalDataInited
        {
            get
            {
                return this.m_IsLocalDataInited;
            }
            set
            {
                this.m_IsLocalDataInited = value;
            }
        }

        public bool IsRequestApplyList { get; set; }

        public uint PlayerDailyActive
        {
            get
            {
                for (int i = 0; i < this.CurrentGuildInfo.listMemInfo.Count; i++)
                {
                    if (this.CurrentGuildInfo.listMemInfo[i].stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
                    {
                        return this.CurrentGuildInfo.listMemInfo[i].CurrActive;
                    }
                }
                return 0;
            }
            set
            {
                for (int i = 0; i < this.CurrentGuildInfo.listMemInfo.Count; i++)
                {
                    if (this.CurrentGuildInfo.listMemInfo[i].stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
                    {
                        this.CurrentGuildInfo.listMemInfo[i].CurrActive = value;
                    }
                }
            }
        }

        public uint PlayerWeekActive
        {
            get
            {
                for (int i = 0; i < this.CurrentGuildInfo.listMemInfo.Count; i++)
                {
                    if (this.CurrentGuildInfo.listMemInfo[i].stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
                    {
                        return this.CurrentGuildInfo.listMemInfo[i].WeekActive;
                    }
                }
                return 0;
            }
            set
            {
                for (int i = 0; i < this.CurrentGuildInfo.listMemInfo.Count; i++)
                {
                    if (this.CurrentGuildInfo.listMemInfo[i].stBriefInfo.uulUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
                    {
                        this.CurrentGuildInfo.listMemInfo[i].WeekActive = value;
                    }
                }
            }
        }

        public List<KeyValuePair<ulong, MemberRankInfo>> RankpointMemberInfoList { get; set; }

        public bool[] RankpointRankGottens { get; set; }

        public ListView<RankpointRankInfo>[] RankpointRankInfoLists { get; set; }

        public int[] RankpointRankLastGottenTimes { get; set; }

        public int RequestApplyListPageId { get; set; }

        public int RequestRecommendListPageId { get; set; }
    }
}

