namespace Assets.Scripts.GameSystem
{
    using Apollo;
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class RankingSystem : Singleton<RankingSystem>
    {
        private string _allViewName;
        private Animator _animator;
        private Dictionary<stFriendByUUIDAndLogicID, int> _coinSentFriendDic = new Dictionary<stFriendByUUIDAndLogicID, int>();
        private int _curRankingListIndex = -1;
        private RankingType _curRankingType = RankingType.None;
        private RankingSubView _curSubViewType;
        private CSDT_RANKING_LIST_SUCC _dailyRankMatchInfo;
        private RankingSubView _defualtSubViewType = RankingSubView.Friend;
        private CUIFormScript _form;
        private string _friendViewName;
        private Dictionary<uint, LocalRankingInfo> _godRankInfo = new Dictionary<uint, LocalRankingInfo>();
        private uint _myLastFriendRank = 0x98967f;
        private GameObject _myselfInfo;
        private bool _rankingBackupListReady;
        private readonly LocalRankingInfo[] _rankingInfo = new LocalRankingInfo[12];
        private CUIListScript _rankingList;
        private bool _rankingListReady;
        private bool _rankingSelfReady;
        private ScrollRect _scroll;
        private ListView<COMDT_FRIEND_INFO> _sortedFriendRankList;
        private CUIListScript _tabList;
        private CUIListScript _viewList;
        private const string AnimCondition = "IsDisplayRankingPanel";
        private int m_curRankGodViewIndex;
        private ListView<ResHeroCfgInfo> m_heroList;
        private uint m_heroMasterId;
        private CSDT_TRANK_TLOG_INFO[] m_uiTlog = new CSDT_TRANK_TLOG_INFO[0x42];
        private const int MaxRankItemCountPerPage = 100;
        public static readonly string s_rankingForm = string.Format("{0}{1}", "UGUI/Form/System/", "Ranking/Form_Ranking.prefab");
        public static readonly string s_rankingGodDetailForm = string.Format("{0}{1}", "UGUI/Form/System/", "Ranking/Form_RankingGodDetail.prefab");

        internal void Clear()
        {
            this._tabList = null;
            this._rankingList = null;
            this._animator = null;
            this._form = null;
            this._curRankingType = RankingType.None;
            this._curSubViewType = RankingSubView.All;
        }

        public void ClearAll()
        {
            this.Clear();
            this._defualtSubViewType = RankingSubView.Friend;
        }

        public void ClearUiTlog()
        {
            for (int i = 0; i < this.m_uiTlog.Length; i++)
            {
                this.m_uiTlog[i] = new CSDT_TRANK_TLOG_INFO();
            }
        }

        protected void CommitUpdate()
        {
            if (this._rankingList.GetSelectedElement() != null)
            {
                this._rankingList.GetSelectedElement().ChangeDisplay(false);
            }
            this._curRankingListIndex = -1;
            if (this.NeedToRetrieveNewList())
            {
                this.RetrieveNewList();
            }
            else
            {
                this._rankingList.MoveElementInScrollArea(0, true);
                this.UpdateAllElementInView();
                this.UpdateSelfInfo();
            }
        }

        protected int ConvertFriendRankIndex(int rankNo)
        {
            int num = rankNo;
            if (rankNo >= this._myLastFriendRank)
            {
                num--;
            }
            return num;
        }

        private static void ConvertPvpLevelAndPhase(uint score, out int level, out int remaining)
        {
            level = 1;
            uint num = score;
            int num2 = 1;
            int num3 = GameDataMgr.acntPvpExpDatabin.Count();
            for (int i = 1; i <= (num3 - 1); i++)
            {
                ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint) ((byte) i));
                if (num < dataByKey.dwNeedExp)
                {
                    num2 = i;
                    break;
                }
                num -= dataByKey.dwNeedExp;
                num2 = i + 1;
            }
            level = num2;
            remaining = (int) num;
        }

        protected static RankingType ConvertRankingLocalType(COM_APOLLO_TRANK_SCORE_TYPE rankType)
        {
            RankingType power = RankingType.Power;
            COM_APOLLO_TRANK_SCORE_TYPE com_apollo_trank_score_type = rankType;
            switch (com_apollo_trank_score_type)
            {
                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_POWER:
                    return RankingType.Power;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_PVP_EXP:
                    return RankingType.PvpRank;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_HERO_NUM:
                    return RankingType.HeroNum;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_SKIN_NUM:
                    return RankingType.SkinNum;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_LADDER_POINT:
                    return RankingType.Ladder;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_ACHIEVEMENT:
                    return RankingType.Achievement;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_WIN_GAMENUM:
                    return RankingType.WinCount;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_CONTINOUS_WIN:
                    return RankingType.ConWinCount;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_USE_COUPONS:
                    return RankingType.ConsumeQuan;

                case COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_VIP_SCORE:
                    return RankingType.GameVip;
            }
            if (com_apollo_trank_score_type != COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_MASTER_HERO)
            {
                return power;
            }
            return RankingType.God;
        }

        protected static COM_APOLLO_TRANK_SCORE_TYPE ConvertRankingServerType(RankingType rankType)
        {
            COM_APOLLO_TRANK_SCORE_TYPE com_apollo_trank_score_type = COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_NULL;
            switch (rankType)
            {
                case RankingType.PvpRank:
                    return COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_PVP_EXP;

                case RankingType.Power:
                    return COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_POWER;

                case RankingType.HeroNum:
                    return COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_HERO_NUM;

                case RankingType.SkinNum:
                    return COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_SKIN_NUM;

                case RankingType.Ladder:
                    return COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_LADDER_POINT;

                case RankingType.Achievement:
                    return COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_ACHIEVEMENT;

                case RankingType.WinCount:
                    return COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_WIN_GAMENUM;

                case RankingType.ConWinCount:
                    return COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_CONTINOUS_WIN;

                case RankingType.ConsumeQuan:
                    return COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_USE_COUPONS;

                case RankingType.GameVip:
                    return COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_VIP_SCORE;

                case RankingType.God:
                    return COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_MASTER_HERO;
            }
            return com_apollo_trank_score_type;
        }

        private void DoDisplayAnimation()
        {
            if (this._animator != null)
            {
                this._animator.SetBool("IsDisplayRankingPanel", true);
            }
        }

        protected void DoHideAnimation(CUIEvent uiEvent)
        {
            if (this._animator != null)
            {
                this._animator.SetBool("IsDisplayRankingPanel", false);
            }
        }

        private void EmptyOneElement(GameObject objElement, int viewIndex)
        {
            objElement.GetComponent<RankingItemHelper>().RankingNumText.CustomSetActive(false);
        }

        private static ListView<ResHeroCfgInfo> GetHeroList(enHeroJobType jobType, bool bOwn = false)
        {
            ListView<ResHeroCfgInfo> view = new ListView<ResHeroCfgInfo>();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "CEquipSystem ResetHeroList role is null");
            if (masterRoleInfo != null)
            {
                ListView<ResHeroCfgInfo> allHeroList = CHeroDataFactory.GetAllHeroList();
                for (int i = 0; i < allHeroList.Count; i++)
                {
                    if ((((jobType == enHeroJobType.All) || (allHeroList[i].bMainJob == ((byte) jobType))) || (allHeroList[i].bMinorJob == ((byte) jobType))) && (!bOwn || masterRoleInfo.IsHaveHero(allHeroList[i].dwCfgID, false)))
                    {
                        view.Add(allHeroList[i]);
                    }
                }
            }
            return view;
        }

        private CSDT_RANKING_LIST_SUCC GetLadderRankingInfoList(CSDT_RANKING_LIST_SUCC realTimeInfo)
        {
            if (this.IsDailyRankMatchEmpty())
            {
                return realTimeInfo;
            }
            CSDT_RANKING_LIST_SUCC csdt_ranking_list_succ = new CSDT_RANKING_LIST_SUCC {
                iLogicWorldID = realTimeInfo.iLogicWorldID,
                dwTimeLimit = realTimeInfo.dwTimeLimit,
                bNumberType = realTimeInfo.bNumberType,
                iStart = realTimeInfo.iStart,
                iLimit = realTimeInfo.iLimit,
                bImage = realTimeInfo.bImage,
                dwItemNum = realTimeInfo.dwItemNum,
                astItemDetail = new CSDT_RANKING_LIST_ITEM_INFO[realTimeInfo.dwItemNum]
            };
            bool[] flagArray = new bool[realTimeInfo.dwItemNum];
            uint index = 0;
            for (int i = 0; i < this._dailyRankMatchInfo.dwItemNum; i++)
            {
                for (int k = 0; k < realTimeInfo.dwItemNum; k++)
                {
                    if (this._dailyRankMatchInfo.astItemDetail[i].stExtraInfo.stDetailInfo.stDailyRankMatch.ullUid == realTimeInfo.astItemDetail[k].stExtraInfo.stDetailInfo.stDailyRankMatch.ullUid)
                    {
                        if (CLadderSystem.IsMaxRankGrade(realTimeInfo.astItemDetail[k].stExtraInfo.stDetailInfo.stDailyRankMatch.stRankInfo.bGradeOfRank))
                        {
                            csdt_ranking_list_succ.astItemDetail[index] = realTimeInfo.astItemDetail[k];
                            csdt_ranking_list_succ.astItemDetail[index].dwRankNo = index + 1;
                            index++;
                            flagArray[k] = true;
                        }
                        break;
                    }
                }
            }
            for (int j = 0; j < realTimeInfo.dwItemNum; j++)
            {
                if (!flagArray[j])
                {
                    csdt_ranking_list_succ.astItemDetail[index] = realTimeInfo.astItemDetail[j];
                    csdt_ranking_list_succ.astItemDetail[index].dwRankNo = index + 1;
                    index++;
                }
            }
            DebugHelper.Assert(index == realTimeInfo.dwItemNum, "resultInfoIndex != realTimeInfo.dwItemNum. Please check code!!!");
            return csdt_ranking_list_succ;
        }

        public uint GetLadderRankNo(ulong playerUid)
        {
            CSDT_RANKING_LIST_SUCC listInfo = this._rankingInfo[4].ListInfo;
            if (listInfo != null)
            {
                for (int i = 0; i < listInfo.dwItemNum; i++)
                {
                    if (listInfo.astItemDetail[i].stExtraInfo.stDetailInfo.stDailyRankMatch.ullUid == playerUid)
                    {
                        return listInfo.astItemDetail[i].dwRankNo;
                    }
                }
            }
            return 0;
        }

        private static uint GetLocalHeroId()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            uint @int = (uint) PlayerPrefs.GetInt(string.Format("Sgame_uid_{0}_rank_hero_id", masterRoleInfo.playerUllUID));
            if (GameDataMgr.heroDatabin.GetDataByKey(@int) != null)
            {
                return @int;
            }
            return GameDataMgr.heroDatabin.GetAnyData().dwCfgID;
        }

        private LocalRankingInfo GetLocalRankingInfo(RankingType rankingType, uint subType = 0)
        {
            if ((rankingType > RankingType.None) && (rankingType <= RankingType.GameVip))
            {
                return this._rankingInfo[(int) rankingType];
            }
            if ((rankingType == RankingType.God) && this._godRankInfo.ContainsKey(subType))
            {
                return this._godRankInfo[subType];
            }
            return new LocalRankingInfo();
        }

        public int GetMyFriendRankNo()
        {
            int num = -1;
            RankingType ladder = RankingType.Ladder;
            if (this._rankingInfo[(int) ladder].SelfInfo != null)
            {
                uint dwScore = this._rankingInfo[(int) ladder].SelfInfo.dwScore;
                ListView<COMDT_FRIEND_INFO> sortedRankingFriendList = Singleton<CFriendContoller>.instance.model.GetSortedRankingFriendList(ConvertRankingServerType(ladder));
                uint num3 = (uint) (sortedRankingFriendList.Count + 1);
                uint num4 = 0;
                uint dwPvpLvl = 0;
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                for (int i = 0; i < num3; i++)
                {
                    num4 = 0;
                    num = i;
                    if (i < sortedRankingFriendList.Count)
                    {
                        num4 = sortedRankingFriendList[i].RankVal[(int) ConvertRankingServerType(ladder)];
                        dwPvpLvl = sortedRankingFriendList[i].dwPvpLvl;
                    }
                    if (((i < sortedRankingFriendList.Count) && (dwScore >= num4)) && (((ladder != RankingType.Ladder) || (dwScore != num4)) || (masterRoleInfo.PvpLevel >= dwPvpLvl)))
                    {
                        return num;
                    }
                }
            }
            return num;
        }

        private static string GetPvpRankName(int level)
        {
            ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint) ((byte) level));
            return ((dataByKey != null) ? string.Format("Lv.{0}", dataByKey.bLevel) : string.Empty);
        }

        private static string GetPvpRankNameEx(uint score)
        {
            int level = 1;
            int remaining = 0;
            ConvertPvpLevelAndPhase(score, out level, out remaining);
            return GetPvpRankName(level);
        }

        public uint GetRankClass(ulong playerUid)
        {
            if (this._dailyRankMatchInfo != null)
            {
                for (int i = 0; i < this._dailyRankMatchInfo.dwItemNum; i++)
                {
                    if (this._dailyRankMatchInfo.astItemDetail[i].stExtraInfo.stDetailInfo.stDailyRankMatch.ullUid == playerUid)
                    {
                        return this._dailyRankMatchInfo.astItemDetail[i].dwRankNo;
                    }
                }
            }
            return 0;
        }

        private COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER GetRankItemDetailInfo(RankingType rankType, int listIndex, uint subType = 0)
        {
            LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(rankType, subType);
            CSDT_RANKING_LIST_ITEM_INFO csdt_ranking_list_item_info = null;
            if (((localRankingInfo.ListInfo != null) && (listIndex < localRankingInfo.ListInfo.astItemDetail.Length)) && (listIndex < localRankingInfo.ListInfo.dwItemNum))
            {
                csdt_ranking_list_item_info = localRankingInfo.ListInfo.astItemDetail[listIndex];
            }
            else if (((localRankingInfo.BackupListInfo != null) && ((listIndex - 100) >= 0)) && (((listIndex - 100) < localRankingInfo.BackupListInfo.astItemDetail.Length) && ((listIndex - 100) < localRankingInfo.BackupListInfo.dwItemNum)))
            {
                csdt_ranking_list_item_info = localRankingInfo.BackupListInfo.astItemDetail[listIndex - 100];
            }
            else
            {
                csdt_ranking_list_item_info = new CSDT_RANKING_LIST_ITEM_INFO();
            }
            switch (rankType)
            {
                case RankingType.PvpRank:
                    return csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stPvpExp;

                case RankingType.Power:
                    return csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stPower;

                case RankingType.HeroNum:
                    return csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stHeroNum;

                case RankingType.SkinNum:
                    return csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stSkinNum;

                case RankingType.Ladder:
                    return csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stLadderPoint;

                case RankingType.Achievement:
                    return csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stAchievement;

                case RankingType.WinCount:
                    return csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stWinGameNum;

                case RankingType.ConWinCount:
                    return csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stContinousWin;

                case RankingType.ConsumeQuan:
                    return csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stUseCoupons;

                case RankingType.GameVip:
                    return csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stVipScore;

                case RankingType.God:
                    return csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stMasterHero.stAcntInfo;
            }
            return new COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER();
        }

        public CSDT_RANKING_LIST_SUCC GetRankList(RankingType rankingType)
        {
            return this._rankingInfo[(int) rankingType].ListInfo;
        }

        private COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO GetSelfHeroMasterInfo(uint heroId)
        {
            LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(RankingType.God, heroId);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (localRankingInfo.ListInfo != null)
            {
                for (int i = 0; i < localRankingInfo.ListInfo.dwItemNum; i++)
                {
                    if (localRankingInfo.ListInfo.astItemDetail[i].stExtraInfo.stDetailInfo.stMasterHero.stAcntInfo.ullUid == masterRoleInfo.playerUllUID)
                    {
                        return localRankingInfo.ListInfo.astItemDetail[i].stExtraInfo.stDetailInfo.stMasterHero;
                    }
                }
            }
            return null;
        }

        public CSDT_TRANK_TLOG_INFO[] GetUiTlog()
        {
            int index = 0;
            for (int i = 0; i < this.m_uiTlog.Length; i++)
            {
                if (this.m_uiTlog[i].dwCnt > 0)
                {
                    index++;
                }
            }
            CSDT_TRANK_TLOG_INFO[] csdt_trank_tlog_infoArray = new CSDT_TRANK_TLOG_INFO[index];
            index = 0;
            for (int j = 0; j < this.m_uiTlog.Length; j++)
            {
                if (this.m_uiTlog[j].dwCnt > 0)
                {
                    csdt_trank_tlog_infoArray[index] = new CSDT_TRANK_TLOG_INFO();
                    csdt_trank_tlog_infoArray[index].dwType = this.m_uiTlog[j].dwType;
                    csdt_trank_tlog_infoArray[index].dwCnt = this.m_uiTlog[j].dwCnt;
                    index++;
                }
            }
            return csdt_trank_tlog_infoArray;
        }

        public void ImpResRankingDetail(SCPKG_GET_RANKING_ACNT_INFO_RSP rsp)
        {
            RankingType type = ConvertRankingLocalType((COM_APOLLO_TRANK_SCORE_TYPE) rsp.stAcntRankingDetail.stOfSucc.bNumberType);
            this._rankingInfo[(int) type].LastRetrieveTime = (uint) CRoleInfo.GetCurrentUTCTime();
            this._rankingInfo[(int) type].SelfInfo = rsp.stAcntRankingDetail.stOfSucc;
            if (type == RankingType.Ladder)
            {
                this._rankingInfo[(int) type].SelfInfo.dwRankNo = this.GetLadderRankNo(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID);
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent("Rank_Friend_List");
            if (this._form != null)
            {
                this._rankingSelfReady = true;
                if (((type == this._curRankingType) && this._rankingSelfReady) && this._rankingListReady)
                {
                    this.UpdateAllElementInView();
                    this.UpdateSelfInfo();
                }
                this.TryToUnlock();
            }
        }

        public void ImpResRankingList(SCPKG_GET_RANKING_LIST_RSP rsp)
        {
            if (this._form != null)
            {
                RankingType type = ConvertRankingLocalType((COM_APOLLO_TRANK_SCORE_TYPE) rsp.stRankingListDetail.stOfSucc.bNumberType);
                if (rsp.stRankingListDetail.stOfSucc.iStart != 1)
                {
                    if (rsp.stRankingListDetail.stOfSucc.iStart == 0x65)
                    {
                        this._rankingInfo[(int) type].LastRetrieveTime = (uint) CRoleInfo.GetCurrentUTCTime();
                        this._rankingInfo[(int) type].BackupListInfo = rsp.stRankingListDetail.stOfSucc;
                        this._rankingBackupListReady = true;
                    }
                }
                else
                {
                    this._rankingInfo[(int) type].LastRetrieveTime = (uint) CRoleInfo.GetCurrentUTCTime();
                    if (rsp.stRankingListDetail.stOfSucc.bNumberType == 7)
                    {
                        this._rankingInfo[(int) type].ListInfo = this.GetLadderRankingInfoList(rsp.stRankingListDetail.stOfSucc);
                    }
                    else if (rsp.stRankingListDetail.stOfSucc.bNumberType != 0x41)
                    {
                        this._rankingInfo[(int) type].ListInfo = rsp.stRankingListDetail.stOfSucc;
                    }
                    else
                    {
                        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                        uint dwSubType = rsp.dwSubType;
                        LocalRankingInfo info2 = new LocalRankingInfo {
                            ListInfo = rsp.stRankingListDetail.stOfSucc,
                            LastRetrieveTime = (uint) CRoleInfo.GetCurrentUTCTime()
                        };
                        bool flag = false;
                        info2.SelfInfo = new CSDT_GET_RANKING_ACNT_DETAIL_SELF();
                        for (int i = 0; i < info2.ListInfo.dwItemNum; i++)
                        {
                            if (info2.ListInfo.astItemDetail[i].stExtraInfo.stDetailInfo.stMasterHero.stAcntInfo.ullUid == masterRoleInfo.playerUllUID)
                            {
                                info2.SelfInfo.bNumberType = info2.ListInfo.bNumberType;
                                info2.SelfInfo.dwScore = 0;
                                info2.SelfInfo.dwRankNo = (uint) (i + 1);
                                info2.SelfInfo.iRankChgNo = 0;
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            info2.SelfInfo.bNumberType = info2.ListInfo.bNumberType;
                            info2.SelfInfo.dwScore = 0;
                            info2.SelfInfo.dwRankNo = 0;
                            info2.SelfInfo.iRankChgNo = 0;
                        }
                        if (!this._godRankInfo.ContainsKey(dwSubType))
                        {
                            this._godRankInfo.Add(dwSubType, info2);
                        }
                        else
                        {
                            this._godRankInfo[dwSubType] = info2;
                        }
                        this._rankingSelfReady = true;
                        this._rankingListReady = true;
                        this._rankingBackupListReady = true;
                    }
                    this._rankingListReady = true;
                }
                if (((type == this._curRankingType) && this._rankingListReady) && (this._rankingSelfReady && this._rankingBackupListReady))
                {
                    this.UpdateGodTitle();
                    this.UpdateAllElementInView();
                    this.UpdateSelfInfo();
                }
                this.TryToUnlock();
            }
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnRanking_OpenForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnRanking_CloseForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ShowAllRankType, new CUIEventManager.OnUIEventHandler(this.OnShowAllRankTypeMenu));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeView, new CUIEventManager.OnUIEventHandler(this.OnChangeSubViewTab));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToLadder, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToLadder));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToHeroCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToHeroCount));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToSkinCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToSkinCount));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToAchievement, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToAchievement));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToWinCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToWinCount));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToConWinCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToConWinCount));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToVip, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToVip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToArena, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToArena));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ChangeRankTypeToGod, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToGod));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ArenaElementEnable, new CUIEventManager.OnUIEventHandler(RankingView.OnRankingArenaElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_HoldDetail, new CUIEventManager.OnUIEventHandler(this.OnHoldDetail));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ElementEnable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnAddFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ArenaAddFriend, new CUIEventManager.OnUIEventHandler(this.OnArenaAddFriend));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ClickListItem, new CUIEventManager.OnUIEventHandler(this.OnClickOneListItem));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ClickMe, new CUIEventManager.OnUIEventHandler(this.OnClickMe));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_ClickCloseBtn, new CUIEventManager.OnUIEventHandler(this.DoHideAnimation));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Friend_SNS_SendCoin, new CUIEventManager.OnUIEventHandler(this.OnFriendSnsSendCoin));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Friend_GAME_SendCoin, new CUIEventManager.OnUIEventHandler(this.OnFriendSendCoin));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Open_HeroChg_Form, new CUIEventManager.OnUIEventHandler(this.OnGodOpenHeroForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Open_HeroChg_Rule_Form, new CUIEventManager.OnUIEventHandler(this.OnGodOpenRuleForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Open_HeroChg_Detail_Form, new CUIEventManager.OnUIEventHandler(this.OnGodOpenDetailForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Click_HeroChg_Detail_Tab, new CUIEventManager.OnUIEventHandler(this.OnGodDetailTabClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_HeroChg_Title_Click, new CUIEventManager.OnUIEventHandler(this.OnGodHeroChgTitleClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_HeroChg_Hero_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnGodHeroChgItemEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_HeroChg_Hero_Click, new CUIEventManager.OnUIEventHandler(this.OnGodHeroChgItemClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Click_Detail_Equip, new CUIEventManager.OnUIEventHandler(this.OnRankingDetailEquipClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Detail_Symbol_Enable, new CUIEventManager.OnUIEventHandler(this.OnRankingDetailSymbolEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Ranking_Close_Detail_Form, new CUIEventManager.OnUIEventHandler(this.OnRankingCloseDetailForm));
            Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_LIST_RSP>("Ranking_Get_Ranking_List", new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetRankingList));
            Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_ACNT_INFO_RSP>("Ranking_Get_Ranking_Account_Info", new Action<SCPKG_GET_RANKING_ACNT_INFO_RSP>(this.OnGetRankingAccountInfo));
            Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_CMD_DONATE_FRIEND_POINT>("Friend_Send_Coin_Done", new Action<SCPKG_CMD_DONATE_FRIEND_POINT>(this.OnCoinSent));
            Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_LIST_RSP>("Ranking_Get_Ranking_Daily_RankMatch", new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetRankingDailyRankMatch));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Rank_Arena_List", new System.Action(this.OnRankArenaList));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Arena_Fighter_Changed", new System.Action(this.OnArenaFighterChanged));
            Singleton<EventRouter>.GetInstance().AddEventHandler("Arena_Record_List", new System.Action(this.OnArenaRecordList));
            for (int i = 0; i < this._rankingInfo.Length; i++)
            {
                this._rankingInfo[i].LastRetrieveTime = 0;
                this._rankingInfo[i].ListInfo = null;
                this._rankingInfo[i].SelfInfo = null;
                this._rankingInfo[i].BackupListInfo = null;
            }
            this.ClearUiTlog();
        }

        internal void InitWidget(bool isShowAllRankTypeBtn = true)
        {
            this._form = Singleton<CUIManager>.GetInstance().OpenForm(s_rankingForm, false, true);
            this._animator = this._form.gameObject.GetComponent<Animator>();
            this._tabList = this._form.m_formWidgets[1].GetComponent<CUIListScript>();
            this._viewList = this._form.m_formWidgets[13].GetComponent<CUIListScript>();
            this._rankingList = this._form.m_formWidgets[3].GetComponent<CUIListScript>();
            this._scroll = this._form.m_formWidgets[4].GetComponent<ScrollRect>();
            this._myselfInfo = this._form.m_formWidgets[8];
            this._scroll.elasticity = 0.08f;
            this._rankingList.SetElementAmount(0);
            this._rankingList.m_alwaysDispatchSelectedChangeEvent = true;
            this._tabList.SetElementAmount(1);
            this._tabList.SelectElement(0, true);
            CUIListElementScript elemenet = this._viewList.GetElemenet(0);
            if (elemenet != null)
            {
                this._allViewName = Singleton<CTextManager>.GetInstance().GetText("ranking_ViewAll");
                this._friendViewName = Singleton<CTextManager>.GetInstance().GetText("ranking_ViewFriend");
                elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>().text = this._allViewName;
            }
            elemenet = this._viewList.GetElemenet(1);
            if (elemenet != null)
            {
                elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>().text = this._friendViewName;
            }
            this._form.m_formWidgets[14].CustomSetActive(false);
            this.SetMenuElementText();
            this.TryToChangeRankType(RankingType.Ladder);
            if (this._defualtSubViewType == RankingSubView.All)
            {
                this._viewList.SelectElement(0, true);
            }
            else if (this._defualtSubViewType == RankingSubView.Friend)
            {
                this._viewList.SelectElement(1, true);
            }
            this._form.GetWidget(0x10).CustomSetActive(isShowAllRankTypeBtn);
        }

        private bool IsDailyRankMatchEmpty()
        {
            return ((this._dailyRankMatchInfo == null) || (this._dailyRankMatchInfo.dwItemNum == 0));
        }

        private bool IsMyFriendRankIndex(int index)
        {
            return (index == this._myLastFriendRank);
        }

        protected bool NeedToRetrieveNewList()
        {
            LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(this._curRankingType, this.m_heroMasterId);
            return ((((localRankingInfo.SelfInfo == null) || (localRankingInfo.ListInfo == null)) || (localRankingInfo.LastRetrieveTime == 0)) || (CRoleInfo.GetCurrentUTCTime() >= (localRankingInfo.LastRetrieveTime + localRankingInfo.ListInfo.dwTimeLimit)));
        }

        protected void OnAddFriend(CUIEvent uiEvent)
        {
            if (this._rankingList != null)
            {
                this._curRankingListIndex = this._rankingList.GetSelectedIndex();
                ulong ullUid = 0L;
                int dwLogicWorldId = 0;
                if (this._curSubViewType == RankingSubView.Friend)
                {
                    if (this._curRankingListIndex != this._myLastFriendRank)
                    {
                        int num3 = this.ConvertFriendRankIndex(this._curRankingListIndex);
                        COMDT_FRIEND_INFO comdt_friend_info = this._sortedFriendRankList[num3];
                        ullUid = comdt_friend_info.stUin.ullUid;
                        dwLogicWorldId = (int) comdt_friend_info.stUin.dwLogicWorldId;
                    }
                    else
                    {
                        ullUid = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID;
                        dwLogicWorldId = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID;
                    }
                }
                else
                {
                    COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER comdt_ranking_list_item_extra_player = this.GetRankItemDetailInfo(this._curRankingType, this._curRankingListIndex, 0);
                    if (comdt_ranking_list_item_extra_player != null)
                    {
                        ullUid = comdt_ranking_list_item_extra_player.ullUid;
                        dwLogicWorldId = comdt_ranking_list_item_extra_player.iLogicWorldId;
                    }
                }
                Singleton<CFriendContoller>.instance.Open_Friend_Verify(ullUid, (uint) dwLogicWorldId, false);
            }
        }

        private void OnArenaAddFriend(CUIEvent uiEvent)
        {
            ulong ullUid = uiEvent.m_eventParams.commonUInt64Param1;
            int tag = uiEvent.m_eventParams.tag;
            Singleton<CFriendContoller>.instance.Open_Friend_Verify(ullUid, (uint) tag, false);
        }

        private void OnArenaFighterChanged()
        {
            if (this._curRankingType == RankingType.Arena)
            {
                RankingView.UpdateArenaSelfInfo();
            }
        }

        private void OnArenaRecordList()
        {
            if (this._curRankingType == RankingType.Arena)
            {
                RankingView.UpdateArenaSelfInfo();
            }
        }

        protected void OnChangeRankToAchievement(CUIEvent uiEvent)
        {
            this.TryToChangeRankType(RankingType.Achievement);
            RankingView.HideAllRankMenu();
            COM_APOLLO_TRANK_SCORE_TYPE com_apollo_trank_score_type = ConvertRankingServerType(RankingType.Achievement);
            this.m_uiTlog[(int) com_apollo_trank_score_type].dwType = (uint) com_apollo_trank_score_type;
            CSDT_TRANK_TLOG_INFO csdt_trank_tlog_info1 = this.m_uiTlog[(int) com_apollo_trank_score_type];
            csdt_trank_tlog_info1.dwCnt++;
        }

        protected void OnChangeRankToArena(CUIEvent uiEvent)
        {
            if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ARENA))
            {
                this.TryToChangeRankType(RankingType.Arena);
                RankingView.HideAllRankMenu();
                Singleton<CMiShuSystem>.GetInstance().SetNewFlagForArenaRankBtn(false);
                COM_APOLLO_TRANK_SCORE_TYPE com_apollo_trank_score_type = ConvertRankingServerType(RankingType.Arena);
                this.m_uiTlog[(int) com_apollo_trank_score_type].dwType = (uint) com_apollo_trank_score_type;
                CSDT_TRANK_TLOG_INFO csdt_trank_tlog_info1 = this.m_uiTlog[(int) com_apollo_trank_score_type];
                csdt_trank_tlog_info1.dwCnt++;
            }
            else
            {
                ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint) 9);
                Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
            }
        }

        protected void OnChangeRankToConWinCount(CUIEvent uiEvent)
        {
            this.TryToChangeRankType(RankingType.ConWinCount);
            RankingView.HideAllRankMenu();
            COM_APOLLO_TRANK_SCORE_TYPE com_apollo_trank_score_type = ConvertRankingServerType(RankingType.ConWinCount);
            this.m_uiTlog[(int) com_apollo_trank_score_type].dwType = (uint) com_apollo_trank_score_type;
            CSDT_TRANK_TLOG_INFO csdt_trank_tlog_info1 = this.m_uiTlog[(int) com_apollo_trank_score_type];
            csdt_trank_tlog_info1.dwCnt++;
        }

        protected void OnChangeRankToGod(CUIEvent uiEvent)
        {
            this.m_heroMasterId = GetLocalHeroId();
            this.TryToChangeRankType(RankingType.God);
            RankingView.HideAllRankMenu();
            Singleton<CMiShuSystem>.GetInstance().SetNewFlagForGodRankBtn(false);
            COM_APOLLO_TRANK_SCORE_TYPE com_apollo_trank_score_type = ConvertRankingServerType(RankingType.God);
            this.m_uiTlog[(int) com_apollo_trank_score_type].dwType = (uint) com_apollo_trank_score_type;
            CSDT_TRANK_TLOG_INFO csdt_trank_tlog_info1 = this.m_uiTlog[(int) com_apollo_trank_score_type];
            csdt_trank_tlog_info1.dwCnt++;
        }

        protected void OnChangeRankToHeroCount(CUIEvent uiEvent)
        {
            this.TryToChangeRankType(RankingType.HeroNum);
            RankingView.HideAllRankMenu();
            COM_APOLLO_TRANK_SCORE_TYPE com_apollo_trank_score_type = ConvertRankingServerType(RankingType.HeroNum);
            this.m_uiTlog[(int) com_apollo_trank_score_type].dwType = (uint) com_apollo_trank_score_type;
            CSDT_TRANK_TLOG_INFO csdt_trank_tlog_info1 = this.m_uiTlog[(int) com_apollo_trank_score_type];
            csdt_trank_tlog_info1.dwCnt++;
        }

        protected void OnChangeRankToLadder(CUIEvent uiEvent)
        {
            this.TryToChangeRankType(RankingType.Ladder);
            RankingView.HideAllRankMenu();
            COM_APOLLO_TRANK_SCORE_TYPE com_apollo_trank_score_type = ConvertRankingServerType(RankingType.Ladder);
            this.m_uiTlog[(int) com_apollo_trank_score_type].dwType = (uint) com_apollo_trank_score_type;
            CSDT_TRANK_TLOG_INFO csdt_trank_tlog_info1 = this.m_uiTlog[(int) com_apollo_trank_score_type];
            csdt_trank_tlog_info1.dwCnt++;
        }

        protected void OnChangeRankToSkinCount(CUIEvent uiEvent)
        {
            this.TryToChangeRankType(RankingType.SkinNum);
            RankingView.HideAllRankMenu();
            COM_APOLLO_TRANK_SCORE_TYPE com_apollo_trank_score_type = ConvertRankingServerType(RankingType.SkinNum);
            this.m_uiTlog[(int) com_apollo_trank_score_type].dwType = (uint) com_apollo_trank_score_type;
            CSDT_TRANK_TLOG_INFO csdt_trank_tlog_info1 = this.m_uiTlog[(int) com_apollo_trank_score_type];
            csdt_trank_tlog_info1.dwCnt++;
        }

        protected void OnChangeRankToVip(CUIEvent uiEvent)
        {
            this.TryToChangeRankType(RankingType.GameVip);
            RankingView.HideAllRankMenu();
            COM_APOLLO_TRANK_SCORE_TYPE com_apollo_trank_score_type = ConvertRankingServerType(RankingType.GameVip);
            this.m_uiTlog[(int) com_apollo_trank_score_type].dwType = (uint) com_apollo_trank_score_type;
            CSDT_TRANK_TLOG_INFO csdt_trank_tlog_info1 = this.m_uiTlog[(int) com_apollo_trank_score_type];
            csdt_trank_tlog_info1.dwCnt++;
        }

        protected void OnChangeRankToWinCount(CUIEvent uiEvent)
        {
            this.TryToChangeRankType(RankingType.WinCount);
            RankingView.HideAllRankMenu();
            COM_APOLLO_TRANK_SCORE_TYPE com_apollo_trank_score_type = ConvertRankingServerType(RankingType.WinCount);
            this.m_uiTlog[(int) com_apollo_trank_score_type].dwType = (uint) com_apollo_trank_score_type;
            CSDT_TRANK_TLOG_INFO csdt_trank_tlog_info1 = this.m_uiTlog[(int) com_apollo_trank_score_type];
            csdt_trank_tlog_info1.dwCnt++;
        }

        protected void OnChangeSubViewTab(CUIEvent uiEvent)
        {
            if (this._curSubViewType != this._viewList.GetSelectedIndex())
            {
                this._defualtSubViewType = this._curSubViewType = (RankingSubView) this._viewList.GetSelectedIndex();
                this._form.m_formWidgets[15].CustomSetActive(false);
                this._form.m_formWidgets[0x15].CustomSetActive(false);
                this.UpdateTabText();
                if ((this._curRankingType != RankingType.Arena) && (this._curRankingType != RankingType.God))
                {
                    this.CommitUpdate();
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<RankingSubView>("Rank_List", this._curSubViewType);
                }
            }
        }

        protected void OnClickMe(CUIEvent uiEvent)
        {
            if ((this._rankingList != null) && (this._rankingList.GetSelectedElement() != null))
            {
                this._rankingList.GetSelectedElement().ChangeDisplay(false);
            }
        }

        protected void OnClickOneListItem(CUIEvent uiEvent)
        {
            if (this._rankingList != null)
            {
                this._curRankingListIndex = this._rankingList.GetSelectedIndex();
                this._rankingList.GetSelectedElement().ChangeDisplay(true);
            }
        }

        private void OnCoinSent(SCPKG_CMD_DONATE_FRIEND_POINT data)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_rankingForm);
            if (form != null)
            {
                CFriendModel.FriendType type = (data.bFriendType != 1) ? CFriendModel.FriendType.SNS : CFriendModel.FriendType.GameFriend;
                ulong ullUid = data.stUin.ullUid;
                uint dwLogicWorldId = data.stUin.dwLogicWorldId;
                stFriendByUUIDAndLogicID key = new stFriendByUUIDAndLogicID(ullUid, dwLogicWorldId, type);
                int num3 = -1;
                if (this._coinSentFriendDic.TryGetValue(key, out num3))
                {
                    this._coinSentFriendDic.Remove(key);
                }
                else
                {
                    num3 = -1;
                }
                if ((this._curRankingType == RankingType.Arena) && (num3 >= 0))
                {
                    CUIListElementScript elemenet = form.GetWidget(0x11).GetComponent<CUIListScript>().GetElemenet(num3);
                    if (elemenet != null)
                    {
                        CUIEvent uiEvent = new CUIEvent {
                            m_eventID = enUIEventID.Ranking_ArenaElementEnable,
                            m_srcFormScript = form,
                            m_srcWidget = elemenet.gameObject,
                            m_srcWidgetIndexInBelongedList = num3
                        };
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
                    }
                }
                else if ((this._curRankingType != RankingType.God) || (num3 < 0))
                {
                    if ((num3 >= 0) && (this._rankingList != null))
                    {
                        CUIListElementScript script4 = this._rankingList.GetElemenet(num3);
                        if (script4 != null)
                        {
                            CUIEvent event3 = new CUIEvent {
                                m_eventID = enUIEventID.Ranking_ElementEnable,
                                m_srcFormScript = form,
                                m_srcWidget = script4.gameObject,
                                m_srcWidgetIndexInBelongedList = num3
                            };
                            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event3);
                            return;
                        }
                    }
                    if (form != null)
                    {
                        this.UpdateAllElementInView();
                    }
                }
            }
        }

        protected void OnElementEnable(CUIEvent uiEvent)
        {
            if (this._curRankingType != RankingType.God)
            {
                this.UpdateOneElement(uiEvent.m_srcWidget, uiEvent.m_srcWidgetIndexInBelongedList);
                CUIListElementScript component = uiEvent.m_srcWidget.GetComponent<CUIListElementScript>();
                if ((component != null) && ((this._curRankingListIndex == -1) || (uiEvent.m_srcWidgetIndexInBelongedList != this._curRankingListIndex)))
                {
                    component.ChangeDisplay(false);
                }
            }
            else
            {
                LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(this._curRankingType, this.m_heroMasterId);
                RankingView.UpdateOneGodElement(uiEvent.m_srcWidget, uiEvent.m_srcWidgetIndexInBelongedList, localRankingInfo.ListInfo);
            }
        }

        private void OnFriendSendCoin(CUIEvent uiEvent)
        {
            stFriendByUUIDAndLogicID key = new stFriendByUUIDAndLogicID(uiEvent.m_eventParams.commonUInt64Param1, (uint) uiEvent.m_eventParams.commonUInt64Param2, CFriendModel.FriendType.GameFriend);
            if (!this._coinSentFriendDic.ContainsKey(key))
            {
                this._coinSentFriendDic.Add(key, uiEvent.m_eventParams.tag);
            }
            uiEvent.m_eventID = enUIEventID.Friend_SendCoin;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
        }

        private void OnFriendSnsSendCoin(CUIEvent uiEvent)
        {
            stFriendByUUIDAndLogicID key = new stFriendByUUIDAndLogicID(uiEvent.m_eventParams.commonUInt64Param1, (uint) uiEvent.m_eventParams.commonUInt64Param2, CFriendModel.FriendType.SNS);
            if (!this._coinSentFriendDic.ContainsKey(key))
            {
                this._coinSentFriendDic.Add(key, uiEvent.m_eventParams.tag);
            }
            uiEvent.m_eventID = enUIEventID.Friend_SNS_SendCoin;
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
        }

        public void OnGetRankingAccountInfo(SCPKG_GET_RANKING_ACNT_INFO_RSP rsp)
        {
            Singleton<RankingSystem>.instance.ImpResRankingDetail(rsp);
        }

        public void OnGetRankingDailyRankMatch(SCPKG_GET_RANKING_LIST_RSP rsp)
        {
            if (rsp.stRankingListDetail.stOfSucc.bNumberType == 0x40)
            {
                this._dailyRankMatchInfo = rsp.stRankingListDetail.stOfSucc;
            }
        }

        public void OnGetRankingList(SCPKG_GET_RANKING_LIST_RSP rsp)
        {
            Singleton<RankingSystem>.instance.ImpResRankingList(rsp);
        }

        private void OnGodDetailTabClick(CUIEvent uiEvent)
        {
            if (this._curRankingType == RankingType.God)
            {
                int selectedIndex = ((CUIListScript) uiEvent.m_srcWidgetScript).GetSelectedIndex();
                LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(this._curRankingType, this.m_heroMasterId);
                if (((localRankingInfo.ListInfo != null) && (this.m_curRankGodViewIndex >= 0)) && (this.m_curRankGodViewIndex < localRankingInfo.ListInfo.dwItemNum))
                {
                    RankingView.OnRankGodDetailTab(selectedIndex, localRankingInfo.ListInfo.astItemDetail[this.m_curRankGodViewIndex].stExtraInfo.stDetailInfo.stMasterHero, this.m_heroMasterId);
                }
            }
        }

        private void OnGodHeroChgItemClick(CUIEvent uiEvent)
        {
            this.m_heroMasterId = uiEvent.m_eventParams.heroId;
            SetLocalHeroId(this.m_heroMasterId);
            Singleton<CUIManager>.GetInstance().CloseForm(RankingView.s_ChooseHeroPath);
            this.CommitUpdate();
        }

        private void OnGodHeroChgItemEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_heroList.Count))
            {
                RankingView.OnHeroItemEnable(uiEvent, this.m_heroList[srcWidgetIndexInBelongedList]);
            }
        }

        private void OnGodHeroChgTitleClick(CUIEvent uiEvent)
        {
            CUIListScript srcWidgetScript = (CUIListScript) uiEvent.m_srcWidgetScript;
            enHeroJobType selectedIndex = (enHeroJobType) srcWidgetScript.GetSelectedIndex();
            this.m_heroList = GetHeroList(selectedIndex, false);
            RankingView.RefreshGodHeroForm(this.m_heroList);
        }

        private void OnGodOpenDetailForm(CUIEvent uiEvent)
        {
            this.m_curRankGodViewIndex = uiEvent.m_eventParams.tag;
            RankingView.OnRankGodDetailEquipClick(null, string.Empty, string.Empty);
            RankingView.ShowRankGodDetailPanel();
            RankingView.UpdateGodFindBtns(this._rankingList, this.m_curRankGodViewIndex);
        }

        private void OnGodOpenHeroForm(CUIEvent uiEvent)
        {
            this.m_heroList = GetHeroList(enHeroJobType.All, false);
            RankingView.OpenHeroChooseForm();
            RankingView.RefreshGodHeroForm(this.m_heroList);
        }

        private void OnGodOpenRuleForm(CUIEvent uiEvent)
        {
            ResRuleText dataByKey = GameDataMgr.s_ruleTextDatabin.GetDataByKey((uint) 14);
            if (dataByKey != null)
            {
                string title = StringHelper.UTF8BytesToString(ref dataByKey.szTitle);
                string info = StringHelper.UTF8BytesToString(ref dataByKey.szContent);
                Singleton<CUIManager>.GetInstance().OpenInfoForm(title, info);
            }
        }

        public void OnHideAnimationEnd()
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_rankingForm);
        }

        protected void OnHoldDetail(CUIEvent uiEvent)
        {
            if (this._rankingList != null)
            {
                this._curRankingListIndex = this._rankingList.GetSelectedIndex();
                ulong ullUid = 0L;
                int dwLogicWorldId = 0;
                if ((this._curSubViewType == RankingSubView.Friend) && (this._curRankingType != RankingType.God))
                {
                    if (this._curRankingListIndex == this._myLastFriendRank)
                    {
                        return;
                    }
                    int num3 = this.ConvertFriendRankIndex(this._curRankingListIndex);
                    COMDT_FRIEND_INFO comdt_friend_info = this._sortedFriendRankList[num3];
                    ullUid = comdt_friend_info.stUin.ullUid;
                    dwLogicWorldId = (int) comdt_friend_info.stUin.dwLogicWorldId;
                }
                else
                {
                    COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER comdt_ranking_list_item_extra_player = this.GetRankItemDetailInfo(this._curRankingType, this._curRankingListIndex, this.m_heroMasterId);
                    if (comdt_ranking_list_item_extra_player != null)
                    {
                        ullUid = comdt_ranking_list_item_extra_player.ullUid;
                        dwLogicWorldId = comdt_ranking_list_item_extra_player.iLogicWorldId;
                        if ((ullUid == Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID) && (dwLogicWorldId == MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID))
                        {
                            return;
                        }
                    }
                }
                CUIEvent event2 = new CUIEvent {
                    m_eventID = enUIEventID.Mini_Player_Info_Open_Form,
                    m_srcFormScript = uiEvent.m_srcFormScript
                };
                event2.m_eventParams.tag = 1;
                event2.m_eventParams.commonUInt64Param1 = ullUid;
                event2.m_eventParams.tag2 = dwLogicWorldId;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event2);
            }
        }

        private void OnRankArenaList()
        {
            if (this._curRankingType == RankingType.Arena)
            {
                RankingView.RefreshRankArena();
            }
        }

        protected void OnRanking_CloseForm(CUIEvent uiEvent)
        {
            this.Clear();
            Singleton<CLobbySystem>.instance.ShowHideRankingBtn(true);
        }

        protected void OnRanking_OpenForm(CUIEvent uiEvent)
        {
            Singleton<CLobbySystem>.instance.ShowHideRankingBtn(false);
            bool isShowAllRankTypeBtn = Singleton<CUIManager>.GetInstance().GetForm(CLadderSystem.FORM_LADDER_ENTRY) == null;
            this.InitWidget(isShowAllRankTypeBtn);
            this.TryToChangeRankType(RankingType.Ladder);
            this.DoDisplayAnimation();
        }

        private void OnRankingCloseDetailForm(CUIEvent uiEvent)
        {
            RankingView.UpdateGodFindBtns(this._rankingList, -1);
        }

        private void OnRankingDetailEquipClick(CUIEvent uiEvent)
        {
            RankingView.OnRankGodDetailEquipClick(uiEvent.m_eventParams.battleEquipPar.equipInfo, uiEvent.m_eventParams.tagStr, uiEvent.m_eventParams.tagStr1);
        }

        private void OnRankingDetailSymbolEnable(CUIEvent uiEvent)
        {
            RankingView.UpdateSymbolItem(uiEvent.m_eventParams.symbolParam.symbol, uiEvent.m_srcWidget, uiEvent.m_srcFormScript);
        }

        protected void OnShowAllRankTypeMenu(CUIEvent uiEvent)
        {
            if (this._form.m_formWidgets[14].activeSelf)
            {
                RankingView.HideAllRankMenu();
            }
            else
            {
                RankingView.ShowAllRankMenu();
            }
        }

        public void OpenRankingForm(RankingType rankingType, uint subType = 0)
        {
            if (rankingType == RankingType.God)
            {
                this.m_heroMasterId = subType;
                SetLocalHeroId(subType);
            }
            Singleton<CLobbySystem>.instance.ShowHideRankingBtn(false);
            bool isShowAllRankTypeBtn = Singleton<CUIManager>.GetInstance().GetForm(CLadderSystem.FORM_LADDER_ENTRY) == null;
            this.InitWidget(isShowAllRankTypeBtn);
            this.TryToChangeRankType(rankingType);
            this.DoDisplayAnimation();
        }

        protected void ReqRankingDetail(int listIndex, bool isSelf = false, RankingType rankType = -1)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa2c);
            if (isSelf)
            {
                msg.stPkgData.stGetRankingAcntInfoReq.bNumberType = (byte) ConvertRankingServerType(rankType);
            }
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        protected void ReqRankingList(COM_APOLLO_TRANK_SCORE_TYPE rankType, int subType = 0)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa2a);
            msg.stPkgData.stGetRankingListReq.iStart = 1;
            msg.stPkgData.stGetRankingListReq.bNumberType = (byte) rankType;
            msg.stPkgData.stGetRankingListReq.iSubType = subType;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
            this._rankingBackupListReady = true;
        }

        private void RetrieveNewList()
        {
            RankingType rankType = this._curRankingType;
            if ((rankType == RankingType.Ladder) && this.IsDailyRankMatchEmpty())
            {
                this.ReqRankingList(COM_APOLLO_TRANK_SCORE_TYPE.COM_APOLLO_TRANK_SCORE_TYPE_DAILY_RANKMATCH, 0);
            }
            if (rankType == RankingType.God)
            {
                this.ReqRankingList(ConvertRankingServerType(rankType), (int) this.m_heroMasterId);
            }
            else
            {
                this.ReqRankingDetail(-1, true, rankType);
                this.ReqRankingList(ConvertRankingServerType(rankType), 0);
            }
            this._rankingListReady = this._rankingSelfReady = false;
        }

        protected void RetrieveSortedFriendRankList()
        {
            this._myLastFriendRank = 0x98967f;
            this._sortedFriendRankList = Singleton<CFriendContoller>.instance.model.GetSortedRankingFriendList(ConvertRankingServerType(this._curRankingType));
        }

        public void SendReqRankingDetail()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa2c);
            msg.stPkgData.stGetRankingAcntInfoReq.bNumberType = 7;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        private static void SetLocalHeroId(uint heroId)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if (GameDataMgr.heroDatabin.GetDataByKey(heroId) != null)
            {
                PlayerPrefs.SetInt(string.Format("Sgame_uid_{0}_rank_hero_id", masterRoleInfo.playerUllUID), (int) heroId);
                PlayerPrefs.Save();
            }
        }

        private void SetMenuElementText()
        {
            GameObject obj2 = this._form.m_formWidgets[14];
            if (obj2 != null)
            {
                obj2.transform.FindChild("ListElement0").gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ranking_LadderRankName");
                obj2.transform.FindChild("ListElement1").gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ranking_HeroCountRankName");
                obj2.transform.FindChild("ListElement2").gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ranking_SkinCountRankName");
                obj2.transform.FindChild("ListElement3").gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ranking_AchieveRankName");
                obj2.transform.FindChild("ListElement4").gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ranking_WinCountRankName");
                obj2.transform.FindChild("ListElement5").gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ranking_ConWinRankName");
                obj2.transform.FindChild("ListElement6").gameObject.transform.FindChild("Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ranking_GameVIPRankName");
            }
        }

        protected void TryToChangeRankType(RankingType rankType)
        {
            if (rankType != this._curRankingType)
            {
                this._form.m_formWidgets[15].CustomSetActive(false);
                this._form.m_formWidgets[0x15].CustomSetActive(false);
                this._curRankingType = rankType;
                this.UpdateTabText();
                if (rankType == RankingType.Arena)
                {
                    if ((this._viewList != null) && (this._viewList.gameObject != null))
                    {
                        this._viewList.gameObject.CustomSetActive(false);
                        this._form.m_formWidgets[3].CustomSetActive(false);
                        this._form.m_formWidgets[0x11].CustomSetActive(true);
                        this._form.m_formWidgets[8].CustomSetActive(false);
                        this._form.m_formWidgets[0x12].CustomSetActive(true);
                        this._form.m_formWidgets[0x13].CustomSetActive(false);
                        RankingView.UpdateArenaSelfInfo();
                        RankingView.RefreshRankArena();
                        CArenaSystem.SendGetRecordMSG(false);
                        CArenaSystem.SendGetFightHeroListMSG(false);
                        CArenaSystem.SendGetRankListMSG(false);
                    }
                }
                else
                {
                    if ((this._viewList != null) && (this._viewList.gameObject != null))
                    {
                        this._viewList.gameObject.CustomSetActive(rankType != RankingType.God);
                    }
                    this._form.m_formWidgets[3].CustomSetActive(true);
                    this._form.m_formWidgets[0x11].CustomSetActive(false);
                    this._form.m_formWidgets[8].CustomSetActive(true);
                    this._form.m_formWidgets[0x12].CustomSetActive(false);
                    this.UpdateGodTitle();
                    this.CommitUpdate();
                }
            }
        }

        private void TryToUnlock()
        {
            if ((this._rankingListReady && this._rankingSelfReady) && this._rankingBackupListReady)
            {
                Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                this._rankingListReady = this._rankingSelfReady = this._rankingBackupListReady = false;
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnRanking_OpenForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnRanking_CloseForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ShowAllRankType, new CUIEventManager.OnUIEventHandler(this.OnShowAllRankTypeMenu));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeView, new CUIEventManager.OnUIEventHandler(this.OnChangeSubViewTab));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToLadder, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToLadder));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToHeroCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToHeroCount));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToSkinCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToSkinCount));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToAchievement, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToAchievement));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToWinCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToWinCount));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToConWinCount, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToConWinCount));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToVip, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToVip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToArena, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToArena));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ChangeRankTypeToGod, new CUIEventManager.OnUIEventHandler(this.OnChangeRankToGod));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ArenaElementEnable, new CUIEventManager.OnUIEventHandler(RankingView.OnRankingArenaElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_HoldDetail, new CUIEventManager.OnUIEventHandler(this.OnHoldDetail));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ElementEnable, new CUIEventManager.OnUIEventHandler(this.OnElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_AddFriend, new CUIEventManager.OnUIEventHandler(this.OnAddFriend));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ArenaAddFriend, new CUIEventManager.OnUIEventHandler(this.OnArenaAddFriend));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ClickListItem, new CUIEventManager.OnUIEventHandler(this.OnClickOneListItem));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ClickMe, new CUIEventManager.OnUIEventHandler(this.OnClickMe));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_ClickCloseBtn, new CUIEventManager.OnUIEventHandler(this.DoHideAnimation));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Friend_SNS_SendCoin, new CUIEventManager.OnUIEventHandler(this.OnFriendSnsSendCoin));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Friend_GAME_SendCoin, new CUIEventManager.OnUIEventHandler(this.OnFriendSendCoin));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Open_HeroChg_Form, new CUIEventManager.OnUIEventHandler(this.OnGodOpenHeroForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Open_HeroChg_Rule_Form, new CUIEventManager.OnUIEventHandler(this.OnGodOpenRuleForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Open_HeroChg_Detail_Form, new CUIEventManager.OnUIEventHandler(this.OnGodOpenDetailForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Click_HeroChg_Detail_Tab, new CUIEventManager.OnUIEventHandler(this.OnGodDetailTabClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_HeroChg_Title_Click, new CUIEventManager.OnUIEventHandler(this.OnGodHeroChgTitleClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_HeroChg_Hero_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnGodHeroChgItemEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_HeroChg_Hero_Click, new CUIEventManager.OnUIEventHandler(this.OnGodHeroChgItemClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Click_Detail_Equip, new CUIEventManager.OnUIEventHandler(this.OnRankingDetailEquipClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Detail_Symbol_Enable, new CUIEventManager.OnUIEventHandler(this.OnRankingDetailSymbolEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Ranking_Close_Detail_Form, new CUIEventManager.OnUIEventHandler(this.OnRankingCloseDetailForm));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("Rank_Arena_List", new System.Action(this.OnRankArenaList));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("Arena_Fighter_Changed", new System.Action(this.OnArenaFighterChanged));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("Arena_Record_List", new System.Action(this.OnArenaRecordList));
        }

        protected void UpdateAllElementInView()
        {
            LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(this._curRankingType, this.m_heroMasterId);
            uint dwItemNum = 0;
            if (this._curRankingType == RankingType.God)
            {
                dwItemNum = localRankingInfo.ListInfo.dwItemNum;
                if (localRankingInfo.BackupListInfo != null)
                {
                    dwItemNum += localRankingInfo.BackupListInfo.dwItemNum;
                }
                this._rankingList.SetElementAmount((int) dwItemNum);
                this._rankingList.MoveElementInScrollArea(0, true);
                this._form.m_formWidgets[0x15].CustomSetActive(dwItemNum == 0);
                this._form.m_formWidgets[15].CustomSetActive(false);
            }
            else
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
                if (this._curSubViewType != RankingSubView.All)
                {
                    int elementAmount = this._rankingList.GetElementAmount();
                    for (int j = 0; j < elementAmount; j++)
                    {
                        if ((this._rankingList.GetElemenet(j) != null) && this._rankingList.IsElementInScrollArea(j))
                        {
                            this.EmptyOneElement(this._rankingList.GetElemenet(j).gameObject, j);
                        }
                    }
                }
                if (this._curSubViewType == RankingSubView.Friend)
                {
                    this.RetrieveSortedFriendRankList();
                    dwItemNum = (uint) (this._sortedFriendRankList.Count + 1);
                    uint dwScore = localRankingInfo.SelfInfo.dwScore;
                    for (int k = 0; k < dwItemNum; k++)
                    {
                        this._myLastFriendRank = (uint) k;
                        uint num6 = 0;
                        uint dwPvpLvl = 0;
                        if (k < this._sortedFriendRankList.Count)
                        {
                            num6 = this._sortedFriendRankList[k].RankVal[(int) ConvertRankingServerType(this._curRankingType)];
                            dwPvpLvl = this._sortedFriendRankList[k].dwPvpLvl;
                        }
                        if (((k < this._sortedFriendRankList.Count) && (dwScore >= num6)) && (((this._curRankingType != RankingType.Ladder) || (dwScore != num6)) || (masterRoleInfo.PvpLevel >= dwPvpLvl)))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    dwItemNum = localRankingInfo.ListInfo.dwItemNum;
                    if (localRankingInfo.BackupListInfo != null)
                    {
                        dwItemNum += localRankingInfo.BackupListInfo.dwItemNum;
                    }
                }
                this._rankingList.SetElementAmount((int) dwItemNum);
                this._rankingList.MoveElementInScrollArea(0, true);
                for (int i = 0; i < dwItemNum; i++)
                {
                    if ((this._rankingList.GetElemenet(i) != null) && this._rankingList.IsElementInScrollArea(i))
                    {
                        this.UpdateOneElement(this._rankingList.GetElemenet(i).gameObject, i);
                    }
                }
                this._form.m_formWidgets[0x15].CustomSetActive(false);
                this._form.m_formWidgets[15].CustomSetActive(dwItemNum == 0);
            }
        }

        private void UpdateGodTitle()
        {
            GameObject obj2 = this._form.m_formWidgets[0x13];
            if (this._curRankingType == RankingType.God)
            {
                obj2.CustomSetActive(true);
                this.m_heroList = GetHeroList(enHeroJobType.All, false);
                for (int i = 0; i < this.m_heroList.Count; i++)
                {
                    if (this.m_heroList[i].dwCfgID == this.m_heroMasterId)
                    {
                        RankingView.UpdateRankGodTitle(this.m_heroList[i]);
                        break;
                    }
                }
            }
            else
            {
                obj2.CustomSetActive(false);
            }
            RankingView.ResetRankListPos(this._curRankingType);
        }

        private void UpdateOneElement(GameObject objElement, int viewIndex)
        {
            LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(this._curRankingType, 0);
            if (localRankingInfo.ListInfo != null)
            {
                RankingItemHelper component = objElement.GetComponent<RankingItemHelper>();
                uint score = 0;
                string name = string.Empty;
                uint pvpLevel = 1;
                string serverUrl = null;
                ulong ullUid = 0L;
                uint dwLogicWorldID = 0;
                uint dwCurLevel = 0;
                uint dwHeadIconId = 0;
                uint dwQQVIPMask = 0;
                COM_PRIVILEGE_TYPE privilegeType = COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
                if (this._curSubViewType == RankingSubView.Friend)
                {
                    if (this.IsMyFriendRankIndex(viewIndex))
                    {
                        CSDT_GET_RANKING_ACNT_DETAIL_SELF selfInfo = localRankingInfo.SelfInfo;
                        if ((selfInfo != null) && (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() != null))
                        {
                            score = selfInfo.dwScore;
                            name = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().Name;
                            pvpLevel = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel;
                            serverUrl = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().HeadUrl;
                            privilegeType = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType;
                            ullUid = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID;
                            dwLogicWorldID = (uint) MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID;
                            dwCurLevel = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel;
                            dwHeadIconId = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId;
                            dwQQVIPMask = 0xdf1f9;
                            RankingView.SetGameObjChildText(this._myselfInfo, "NameGroup/PlayerName", Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().Name);
                            RankingView.SetGameObjChildText(this._myselfInfo, "PlayerLv", string.Format("Lv.{0}", Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel.ToString(CultureInfo.InvariantCulture)));
                        }
                    }
                    else
                    {
                        int num8 = this.ConvertFriendRankIndex(viewIndex);
                        if (((this._sortedFriendRankList != null) && (num8 < this._sortedFriendRankList.Count)) && (num8 >= 0))
                        {
                            COMDT_FRIEND_INFO comdt_friend_info = this._sortedFriendRankList[num8];
                            if (comdt_friend_info != null)
                            {
                                score = comdt_friend_info.RankVal[(int) ConvertRankingServerType(this._curRankingType)];
                                name = StringHelper.UTF8BytesToString(ref comdt_friend_info.szUserName);
                                pvpLevel = comdt_friend_info.dwPvpLvl;
                                serverUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref comdt_friend_info.szHeadUrl);
                                ullUid = comdt_friend_info.stUin.ullUid;
                                dwLogicWorldID = comdt_friend_info.stUin.dwLogicWorldId;
                                dwCurLevel = comdt_friend_info.stGameVip.dwCurLevel;
                                dwHeadIconId = comdt_friend_info.stGameVip.dwHeadIconId;
                                dwQQVIPMask = comdt_friend_info.dwQQVIPMask;
                                privilegeType = (COM_PRIVILEGE_TYPE) comdt_friend_info.bPrivilege;
                                CFriendModel.FriendInGame friendInGaming = Singleton<CFriendContoller>.instance.model.GetFriendInGaming(ullUid, dwLogicWorldID);
                                string nickName = string.Empty;
                                if (friendInGaming != null)
                                {
                                    nickName = friendInGaming.NickName;
                                }
                                if (!string.IsNullOrEmpty(nickName))
                                {
                                    name = string.Format("{0}({1})", StringHelper.UTF8BytesToString(ref comdt_friend_info.szUserName), nickName);
                                }
                            }
                        }
                    }
                }
                else
                {
                    int index = viewIndex;
                    CSDT_RANKING_LIST_ITEM_INFO csdt_ranking_list_item_info = null;
                    if (((localRankingInfo.ListInfo.astItemDetail != null) && (index < localRankingInfo.ListInfo.astItemDetail.Length)) && (index < localRankingInfo.ListInfo.dwItemNum))
                    {
                        csdt_ranking_list_item_info = localRankingInfo.ListInfo.astItemDetail[index];
                    }
                    else
                    {
                        int num10 = index - 100;
                        if ((((localRankingInfo.BackupListInfo != null) && (localRankingInfo.BackupListInfo.astItemDetail != null)) && ((num10 >= 0) && (num10 < localRankingInfo.BackupListInfo.astItemDetail.Length))) && (num10 < localRankingInfo.BackupListInfo.dwItemNum))
                        {
                            csdt_ranking_list_item_info = localRankingInfo.BackupListInfo.astItemDetail[num10];
                        }
                    }
                    if (csdt_ranking_list_item_info != null)
                    {
                        score = csdt_ranking_list_item_info.dwRankScore;
                        COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER comdt_ranking_list_item_extra_player = this.GetRankItemDetailInfo(this._curRankingType, index, 0);
                        if (comdt_ranking_list_item_extra_player != null)
                        {
                            name = StringHelper.UTF8BytesToString(ref comdt_ranking_list_item_extra_player.szPlayerName);
                            pvpLevel = comdt_ranking_list_item_extra_player.dwPvpLevel;
                            serverUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref comdt_ranking_list_item_extra_player.szHeadUrl);
                            ullUid = comdt_ranking_list_item_extra_player.ullUid;
                            dwLogicWorldID = (uint) comdt_ranking_list_item_extra_player.iLogicWorldId;
                            dwCurLevel = comdt_ranking_list_item_extra_player.stGameVip.dwCurLevel;
                            dwHeadIconId = comdt_ranking_list_item_extra_player.stGameVip.dwHeadIconId;
                            privilegeType = (COM_PRIVILEGE_TYPE) comdt_ranking_list_item_extra_player.bPrivilege;
                            dwQQVIPMask = comdt_ranking_list_item_extra_player.dwVipLevel;
                        }
                    }
                }
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.WxIcon, privilegeType, ApolloPlatform.Wechat, true, false);
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.QqIcon, privilegeType, ApolloPlatform.QQ, true, false);
                MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.GuestIcon, privilegeType, ApolloPlatform.Guest, true, false);
                RankingView.SetGameObjChildText(objElement, "NameGroup/PlayerName", name);
                RankingView.SetGameObjChildText(objElement, "PlayerLv", string.Format("Lv.{0}", Math.Max(1, pvpLevel)));
                RankingView.SetUrlHeadIcon(component.HeadIcon, serverUrl);
                RankingView.SetPlatChannel(objElement, dwLogicWorldID);
                component.FindBtn.CustomSetActive(false);
                if (this._curRankingType == RankingType.Ladder)
                {
                    component.LadderGo.CustomSetActive(true);
                    objElement.transform.FindChild("Value").gameObject.CustomSetActive(false);
                    objElement.transform.FindChild("ValueType").gameObject.CustomSetActive(false);
                }
                else
                {
                    component.LadderGo.CustomSetActive(false);
                    objElement.transform.FindChild("Value").gameObject.CustomSetActive(true);
                    objElement.transform.FindChild("ValueType").gameObject.CustomSetActive(true);
                }
                switch (this._curRankingType)
                {
                    case RankingType.PvpRank:
                    {
                        RankingView.SetGameObjChildText(objElement, "ValueType", GetPvpRankNameEx(score));
                        int level = 1;
                        int remaining = 0;
                        ConvertPvpLevelAndPhase(score, out level, out remaining);
                        RankingView.SetGameObjChildText(objElement, "Value", remaining.ToString(CultureInfo.InvariantCulture));
                        break;
                    }
                    case RankingType.HeroNum:
                        RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemHeroCountName"));
                        RankingView.SetGameObjChildText(objElement, "Value", score.ToString(CultureInfo.InvariantCulture));
                        break;

                    case RankingType.SkinNum:
                        RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemSkinCountName"));
                        RankingView.SetGameObjChildText(objElement, "Value", score.ToString(CultureInfo.InvariantCulture));
                        break;

                    case RankingType.Ladder:
                    {
                        int rankGrade = CLadderSystem.ConvertEloToRank(score);
                        int curXingByEloAndRankLv = CLadderSystem.GetCurXingByEloAndRankLv(score, (uint) rankGrade);
                        if ((pvpLevel < CLadderSystem.REQ_PLAYER_LEVEL) || !(!this.IsMyFriendRankIndex(viewIndex) ? Singleton<CLadderSystem>.GetInstance().IsHaveFightRecord(false, rankGrade, curXingByEloAndRankLv) : Singleton<CLadderSystem>.GetInstance().IsHaveFightRecord(true, -1, -1)))
                        {
                            RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemNoLadderName"));
                            component.LadderGo.CustomSetActive(false);
                            objElement.transform.FindChild("Value").gameObject.CustomSetActive(false);
                            objElement.transform.FindChild("ValueType").gameObject.CustomSetActive(true);
                            break;
                        }
                        CLadderView.ShowRankDetail(component.LadderGo, (byte) rankGrade, this.GetRankClass(ullUid), 1, false, true, false, true);
                        component.LadderXing.GetComponent<Text>().text = string.Format("x{0}", curXingByEloAndRankLv);
                        break;
                    }
                    case RankingType.Achievement:
                        RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemAchieveName"));
                        RankingView.SetGameObjChildText(objElement, "Value", score.ToString(CultureInfo.InvariantCulture));
                        break;

                    case RankingType.WinCount:
                        RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemWinCountName"));
                        RankingView.SetGameObjChildText(objElement, "Value", score.ToString(CultureInfo.InvariantCulture));
                        break;

                    case RankingType.ConWinCount:
                        RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemConWinCountName"));
                        RankingView.SetGameObjChildText(objElement, "Value", score.ToString(CultureInfo.InvariantCulture));
                        break;

                    case RankingType.ConsumeQuan:
                        RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemConsumeQuanName"));
                        RankingView.SetGameObjChildText(objElement, "Value", score.ToString(CultureInfo.InvariantCulture));
                        break;

                    case RankingType.GameVip:
                        RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemGameVIPName"));
                        RankingView.SetGameObjChildText(objElement, "Value", score.ToString(CultureInfo.InvariantCulture));
                        break;

                    case RankingType.God:
                    {
                        string[] args = new string[] { "0", "0" };
                        RankingView.SetGameObjChildText(objElement, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemHeroMasterName", args));
                        RankingView.SetGameObjChildText(objElement, "Value", string.Empty);
                        break;
                    }
                }
                uint rankNumber = (uint) (viewIndex + 1);
                RankingView.RankingNumSet(rankNumber, component);
                COMDT_FRIEND_INFO comdt_friend_info2 = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.GameFriend, ullUid, dwLogicWorldID);
                COMDT_FRIEND_INFO comdt_friend_info3 = Singleton<CFriendContoller>.instance.model.GetInfo(CFriendModel.FriendType.SNS, ullUid, dwLogicWorldID);
                bool flag = comdt_friend_info2 != null;
                bool flag2 = comdt_friend_info3 != null;
                COMDT_ACNT_UNIQ uniq = new COMDT_ACNT_UNIQ {
                    ullUid = ullUid,
                    dwLogicWorldId = dwLogicWorldID
                };
                if (this._curSubViewType == RankingSubView.Friend)
                {
                    component.AddFriend.CustomSetActive(false);
                }
                else
                {
                    uint playerUllUID = (uint) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID;
                    component.AddFriend.CustomSetActive((!flag && !flag2) && (playerUllUID != ullUid));
                }
                CUIEventScript script = component.SendCoin.GetComponent<CUIEventScript>();
                if (flag2)
                {
                    bool flag3 = Singleton<CFriendContoller>.instance.model.HeartData.BCanSendHeart(uniq, COM_FRIEND_TYPE.COM_FRIEND_TYPE_GAME);
                    bool flag4 = Singleton<CFriendContoller>.instance.model.HeartData.BCanSendHeart(uniq, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
                    component.ShowSendButton(flag3 && flag4);
                    script.m_onClickEventID = enUIEventID.Ranking_Friend_SNS_SendCoin;
                    script.m_onClickEventParams.tag = viewIndex;
                    script.m_onClickEventParams.commonUInt64Param1 = ullUid;
                    script.m_onClickEventParams.commonUInt64Param2 = dwLogicWorldID;
                    component.Online.CustomSetActive(true);
                    if (component.Online != null)
                    {
                        Text componetInChild = Utility.GetComponetInChild<Text>(component.Online, "Text");
                        if (componetInChild != null)
                        {
                            componetInChild.text = (comdt_friend_info3.bIsOnline == 0) ? Singleton<CTextManager>.GetInstance().GetText("Common_Offline") : string.Format("<color=#00ff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Online"));
                        }
                    }
                }
                else if (flag)
                {
                    bool flag5 = Singleton<CFriendContoller>.instance.model.HeartData.BCanSendHeart(uniq, COM_FRIEND_TYPE.COM_FRIEND_TYPE_GAME);
                    bool flag6 = Singleton<CFriendContoller>.instance.model.HeartData.BCanSendHeart(uniq, COM_FRIEND_TYPE.COM_FRIEND_TYPE_SNS);
                    component.ShowSendButton(flag5 && flag6);
                    script.m_onClickEventID = enUIEventID.Ranking_Friend_GAME_SendCoin;
                    script.m_onClickEventParams.tag = viewIndex;
                    script.m_onClickEventParams.commonUInt64Param1 = ullUid;
                    script.m_onClickEventParams.commonUInt64Param2 = dwLogicWorldID;
                    component.Online.CustomSetActive(true);
                    if (component.Online != null)
                    {
                        Text text2 = Utility.GetComponetInChild<Text>(component.Online, "Text");
                        if (text2 != null)
                        {
                            text2.text = (comdt_friend_info2.bIsOnline == 0) ? Singleton<CTextManager>.GetInstance().GetText("Common_Offline") : string.Format("<color=#00ff00>{0}</color>", Singleton<CTextManager>.instance.GetText("Common_Online"));
                        }
                    }
                }
                else
                {
                    component.SendCoin.CustomSetActive(false);
                    component.Online.CustomSetActive(false);
                    component.Online.CustomSetActive(false);
                }
                if (dwQQVIPMask == 0xdf1f9)
                {
                    MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(component.QqVip.GetComponent<Image>());
                }
                else
                {
                    MonoSingleton<NobeSys>.GetInstance().SetOtherQQVipHead(component.QqVip.GetComponent<Image>(), (int) dwQQVIPMask);
                }
                MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.VipIcon.GetComponent<Image>(), (int) dwCurLevel, false);
                MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component.HeadIconFrame.GetComponent<Image>(), (int) dwHeadIconId);
            }
        }

        protected void UpdateSelfInfo()
        {
            uint num6;
            uint dwWinCnt;
            uint dwRankNo;
            LocalRankingInfo localRankingInfo = this.GetLocalRankingInfo(this._curRankingType, this.m_heroMasterId);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            CSDT_GET_RANKING_ACNT_DETAIL_SELF selfInfo = localRankingInfo.SelfInfo;
            RankingItemHelper component = this._myselfInfo.GetComponent<RankingItemHelper>();
            uint pvpLevel = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel;
            RankingView.SetGameObjChildText(this._myselfInfo, "NameGroup/PlayerName", Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().Name);
            RankingView.SetGameObjChildText(this._myselfInfo, "PlayerLv", string.Format("Lv.{0}", pvpLevel.ToString(CultureInfo.InvariantCulture)));
            RankingView.SetHostUrlHeadIcon(component.HeadIcon);
            if (this._curRankingType == RankingType.Ladder)
            {
                component.LadderGo.CustomSetActive(true);
                this._myselfInfo.transform.FindChild("Value").gameObject.CustomSetActive(false);
                this._myselfInfo.transform.FindChild("ValueType").gameObject.CustomSetActive(false);
            }
            else
            {
                component.LadderGo.CustomSetActive(false);
                this._myselfInfo.transform.FindChild("Value").gameObject.CustomSetActive(true);
                this._myselfInfo.transform.FindChild("ValueType").gameObject.CustomSetActive(true);
            }
            switch (this._curRankingType)
            {
                case RankingType.PvpRank:
                {
                    RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", GetPvpRankNameEx(selfInfo.dwScore));
                    int level = 1;
                    int remaining = 0;
                    ConvertPvpLevelAndPhase(selfInfo.dwScore, out level, out remaining);
                    RankingView.SetGameObjChildText(this._myselfInfo, "Value", remaining.ToString(CultureInfo.InvariantCulture));
                    if (GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint) ((byte) level)) == null)
                    {
                    }
                    goto Label_0587;
                }
                case RankingType.HeroNum:
                    RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemHeroCountName"));
                    RankingView.SetGameObjChildText(this._myselfInfo, "Value", selfInfo.dwScore.ToString(CultureInfo.InvariantCulture));
                    goto Label_0587;

                case RankingType.SkinNum:
                    RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemSkinCountName"));
                    RankingView.SetGameObjChildText(this._myselfInfo, "Value", selfInfo.dwScore.ToString(CultureInfo.InvariantCulture));
                    goto Label_0587;

                case RankingType.Ladder:
                    if ((pvpLevel < CLadderSystem.REQ_PLAYER_LEVEL) || !Singleton<CLadderSystem>.GetInstance().IsHaveFightRecord(true, -1, -1))
                    {
                        RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemNoLadderName"));
                        component.LadderGo.CustomSetActive(false);
                        this._myselfInfo.transform.FindChild("Value").gameObject.CustomSetActive(false);
                        this._myselfInfo.transform.FindChild("ValueType").gameObject.CustomSetActive(true);
                    }
                    else
                    {
                        int num4 = CLadderSystem.ConvertEloToRank(selfInfo.dwScore);
                        CLadderView.ShowRankDetail(component.LadderGo, (byte) num4, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass(), 1, false, true, false, true);
                        int curXingByEloAndRankLv = CLadderSystem.GetCurXingByEloAndRankLv(selfInfo.dwScore, (uint) num4);
                        component.LadderXing.GetComponent<Text>().text = string.Format("x{0}", curXingByEloAndRankLv);
                    }
                    goto Label_0587;

                case RankingType.Achievement:
                    RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemAchieveName"));
                    RankingView.SetGameObjChildText(this._myselfInfo, "Value", selfInfo.dwScore.ToString(CultureInfo.InvariantCulture));
                    goto Label_0587;

                case RankingType.WinCount:
                    RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemWinCountName"));
                    RankingView.SetGameObjChildText(this._myselfInfo, "Value", selfInfo.dwScore.ToString(CultureInfo.InvariantCulture));
                    goto Label_0587;

                case RankingType.ConWinCount:
                    RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemConWinCountName"));
                    RankingView.SetGameObjChildText(this._myselfInfo, "Value", selfInfo.dwScore.ToString(CultureInfo.InvariantCulture));
                    goto Label_0587;

                case RankingType.ConsumeQuan:
                    RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemConsumeQuanName"));
                    RankingView.SetGameObjChildText(this._myselfInfo, "Value", selfInfo.dwScore.ToString(CultureInfo.InvariantCulture));
                    goto Label_0587;

                case RankingType.GameVip:
                    RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemGameVIPName"));
                    RankingView.SetGameObjChildText(this._myselfInfo, "Value", selfInfo.dwScore.ToString(CultureInfo.InvariantCulture));
                    goto Label_0587;

                case RankingType.God:
                {
                    COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO selfHeroMasterInfo = this.GetSelfHeroMasterInfo(this.m_heroMasterId);
                    num6 = 0;
                    dwWinCnt = 0;
                    if ((selfHeroMasterInfo == null) || (selfHeroMasterInfo.dwGameCnt == 0))
                    {
                        CHeroInfo heroInfo = masterRoleInfo.GetHeroInfo(this.m_heroMasterId, false);
                        if ((heroInfo != null) && (heroInfo.m_masterHeroFightCnt != 0))
                        {
                            num6 = (heroInfo.m_masterHeroWinCnt * 100) / heroInfo.m_masterHeroFightCnt;
                            dwWinCnt = heroInfo.m_masterHeroWinCnt;
                        }
                        break;
                    }
                    num6 = (selfHeroMasterInfo.dwWinCnt * 100) / selfHeroMasterInfo.dwGameCnt;
                    dwWinCnt = selfHeroMasterInfo.dwWinCnt;
                    break;
                }
                default:
                    goto Label_0587;
            }
            string[] args = new string[] { num6.ToString(), dwWinCnt.ToString() };
            RankingView.SetGameObjChildText(this._myselfInfo, "ValueType", Singleton<CTextManager>.GetInstance().GetText("ranking_ItemHeroMasterName", args));
            RankingView.SetGameObjChildText(this._myselfInfo, "Value", string.Empty);
        Label_0587:
            dwRankNo = 0;
            if ((this._curRankingType != RankingType.God) && (this._curSubViewType == RankingSubView.Friend))
            {
                dwRankNo = this._myLastFriendRank + 1;
            }
            else
            {
                dwRankNo = selfInfo.dwRankNo;
            }
            if ((selfInfo.iRankChgNo == 0) || (this._curSubViewType != RankingSubView.All))
            {
                component.RankingUpDownIcon.CustomSetActive(false);
                RankingView.SetGameObjChildText(this._myselfInfo, "ChangeNum", "--");
            }
            else if (selfInfo.iRankChgNo > 0)
            {
                component.RankingUpDownIcon.CustomSetActive(true);
                component.RankingUpDownIcon.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                RankingView.SetGameObjChildText(this._myselfInfo, "ChangeNum", selfInfo.iRankChgNo.ToString(CultureInfo.InvariantCulture));
            }
            else if (selfInfo.iRankChgNo < 0)
            {
                component.RankingUpDownIcon.CustomSetActive(true);
                component.RankingUpDownIcon.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
                RankingView.SetGameObjChildText(this._myselfInfo, "ChangeNum", -selfInfo.iRankChgNo.ToString(CultureInfo.InvariantCulture));
            }
            RankingView.RankingNumSet(dwRankNo, component);
            MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(component.QqVip.GetComponent<Image>());
            MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.VipIcon.GetComponent<Image>(), (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false);
            MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component.HeadIconFrame.GetComponent<Image>(), (int) Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwHeadIconId);
            MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.WxIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Wechat, true, false);
            MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.QqIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.QQ, true, false);
            MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(component.GuestIcon, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().m_privilegeType, ApolloPlatform.Guest, true, false);
        }

        private void UpdateTabText()
        {
            CUIListElementScript elemenet = this._tabList.GetElemenet(0);
            if (elemenet != null)
            {
                string text = null;
                if (this._curRankingType == RankingType.HeroNum)
                {
                    text = Singleton<CTextManager>.GetInstance().GetText("ranking_HeroCountRankName");
                }
                else if (this._curRankingType == RankingType.SkinNum)
                {
                    text = Singleton<CTextManager>.GetInstance().GetText("ranking_SkinCountRankName");
                }
                else if (this._curRankingType == RankingType.Ladder)
                {
                    text = Singleton<CTextManager>.GetInstance().GetText("ranking_LadderRankName");
                }
                else if (this._curRankingType == RankingType.Achievement)
                {
                    text = Singleton<CTextManager>.GetInstance().GetText("ranking_AchieveRankName");
                }
                else if (this._curRankingType == RankingType.WinCount)
                {
                    text = Singleton<CTextManager>.GetInstance().GetText("ranking_WinCountRankName");
                }
                else if (this._curRankingType == RankingType.ConWinCount)
                {
                    text = Singleton<CTextManager>.GetInstance().GetText("ranking_ConWinRankName");
                }
                else if (this._curRankingType == RankingType.ConsumeQuan)
                {
                    text = Singleton<CTextManager>.GetInstance().GetText("ranking_ConsumeQuanRankName");
                }
                else if (this._curRankingType == RankingType.GameVip)
                {
                    text = Singleton<CTextManager>.GetInstance().GetText("ranking_GameVIPRankName");
                }
                else
                {
                    if (this._curRankingType == RankingType.Arena)
                    {
                        text = Singleton<CTextManager>.GetInstance().GetText("ranking_ArenaRankName");
                        elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>().text = text;
                        return;
                    }
                    if (this._curRankingType == RankingType.God)
                    {
                        text = Singleton<CTextManager>.GetInstance().GetText("ranking_GodRankName");
                        elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>().text = text;
                        return;
                    }
                }
                switch (this._curSubViewType)
                {
                    case RankingSubView.All:
                        elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>().text = string.Format("{0}{1}", this._allViewName, text);
                        break;

                    case RankingSubView.Friend:
                        elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>().text = string.Format("{0}{1}", this._friendViewName, text);
                        break;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct LocalRankingInfo
        {
            public uint LastRetrieveTime;
            public CSDT_RANKING_LIST_SUCC ListInfo;
            public CSDT_GET_RANKING_ACNT_DETAIL_SELF SelfInfo;
            public CSDT_RANKING_LIST_SUCC BackupListInfo;
        }

        public enum RankingSubView
        {
            All,
            Friend,
            GuildMember
        }

        public enum RankingType
        {
            Achievement = 5,
            Arena = 11,
            ConsumeQuan = 8,
            ConWinCount = 7,
            GameVip = 9,
            God = 10,
            HeroNum = 2,
            Ladder = 4,
            MaxNum = 12,
            None = -1,
            Power = 1,
            PvpRank = 0,
            SkinNum = 3,
            WinCount = 6
        }
    }
}

