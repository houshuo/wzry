namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class CPlayerProfile
    {
        private int _1V1TotalCount;
        private int _1V1WinCount;
        private int _2V2TotalCount;
        private int _2V2WinCount;
        private int _3V3TotalCount;
        private int _3V3WinCount;
        private int _5V5TotalCount;
        private int _5V5WinCount;
        public int _coinExpireHours;
        public uint _coinWinCount;
        private int _doubleKillCount;
        private int _entertainmentTotalCount;
        private int _entertainmentWinCount;
        public int _expExpireHours;
        public uint _expWinCount;
        public int _geiLiDuiYou;
        private COM_SNSGENDER _gender;
        public bool _haveExtraCoin;
        public bool _haveExtraExp;
        private int _heroCnt;
        private int _holyShitCount;
        private Dictionary<int, COMDT_HONORINFO> _honorDic = new Dictionary<int, COMDT_HONORINFO>();
        private bool _isOnLine;
        public int _keJingDuiShou;
        private int _loseMvpCount;
        public ListView<COMDT_MOST_USED_HERO_INFO> _mostUsedHeroList;
        private int _mvpCnt;
        private int _pentaKillCount;
        private uint _playerExp;
        private string _playerHeadUrl;
        private uint _playerLevel;
        private string _playerName;
        private uint _playerNeedExp;
        private uint _playerPvpExp;
        private uint _power;
        private COM_PRIVILEGE_TYPE _privilegeType;
        private uint _pvpLevel;
        private int _quataryKillCount;
        private uint _rankClass;
        private byte _rankGrade;
        private uint _rankHistoryHighestClass;
        private byte _rankHistoryHighestGrade;
        private int _rankTotalCount;
        private int _rankWinCount;
        private int _selectedHonorId;
        private int _skinCnt;
        private int _trippleKillCount;
        private int _vsAiTotalCount;
        private int _vsAiWinCount;
        [CompilerGenerated]
        private static Comparison<COMDT_MOST_USED_HERO_INFO> <>f__am$cache39;
        public uint creditScore;
        public int m_iLogicWorldId;
        public string m_personSign;
        public ulong m_uuid;
        public SCPKG_GAME_VIP_NTF m_vipInfo = new SCPKG_GAME_VIP_NTF();
        public uint qqVipMask;

        public void ConvertRoleInfoData(CRoleInfo roleInfo)
        {
            if (roleInfo == null)
            {
                this.ResetData();
            }
            else
            {
                this.ResetData();
                this.m_uuid = roleInfo.playerUllUID;
                this.m_iLogicWorldId = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID;
                this.m_vipInfo = roleInfo.GetNobeInfo();
                this._gender = roleInfo.m_gender;
                this._privilegeType = roleInfo.m_privilegeType;
                this._playerName = roleInfo.Name;
                this.m_personSign = roleInfo.PersonSign;
                this._playerHeadUrl = roleInfo.HeadUrl;
                this._playerLevel = roleInfo.Level;
                this._playerExp = roleInfo.Exp;
                this._playerNeedExp = roleInfo.NeedExp;
                this._power = roleInfo.BattlePower;
                this._pvpLevel = roleInfo.PvpLevel;
                this._playerPvpExp = roleInfo.PvpExp;
                this._rankGrade = roleInfo.m_rankGrade;
                this._rankHistoryHighestGrade = roleInfo.m_rankHistoryHighestGrade;
                this._rankClass = roleInfo.m_rankClass;
                this._rankHistoryHighestClass = roleInfo.m_rankHistoryHighestClass;
                this.GuildName = roleInfo.m_baseGuildInfo.name;
                this.GuildState = roleInfo.m_baseGuildInfo.guildState;
                this.creditScore = roleInfo.creditScore;
                for (int i = 0; i < roleInfo.pvpDetail.stKVDetail.dwNum; i++)
                {
                    COMDT_STATISTIC_KEY_VALUE_INFO comdt_statistic_key_value_info = roleInfo.pvpDetail.stKVDetail.astKVDetail[i];
                    switch (((RES_STATISTIC_SETTLE_DATA_TYPE) comdt_statistic_key_value_info.dwKey))
                    {
                        case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_MVP_CNT:
                            this._mvpCnt = (int) comdt_statistic_key_value_info.dwValue;
                            break;

                        case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_LOSE_SOUL:
                            this._loseMvpCount = (int) comdt_statistic_key_value_info.dwValue;
                            break;

                        case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_GODLIKE_CNT:
                            this._holyShitCount = (int) comdt_statistic_key_value_info.dwValue;
                            break;

                        case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_DOUBLE_KILL_CNT:
                            this._doubleKillCount = (int) comdt_statistic_key_value_info.dwValue;
                            break;

                        case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_TRIPLE_KILL_CNT:
                            this._trippleKillCount = (int) comdt_statistic_key_value_info.dwValue;
                            break;

                        case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_QUATARY_KILL_CNT:
                            this._quataryKillCount = (int) comdt_statistic_key_value_info.dwValue;
                            break;

                        case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_PENTA_KILL_CNT:
                            this._pentaKillCount = (int) comdt_statistic_key_value_info.dwValue;
                            break;
                    }
                }
                this._5V5TotalCount = (int) roleInfo.pvpDetail.stFiveVsFiveInfo.dwTotalNum;
                this._5V5WinCount = (int) roleInfo.pvpDetail.stFiveVsFiveInfo.dwWinNum;
                this._3V3TotalCount = (int) roleInfo.pvpDetail.stThreeVsThreeInfo.dwTotalNum;
                this._3V3WinCount = (int) roleInfo.pvpDetail.stThreeVsThreeInfo.dwWinNum;
                this._2V2TotalCount = (int) roleInfo.pvpDetail.stTwoVsTwoInfo.dwTotalNum;
                this._2V2WinCount = (int) roleInfo.pvpDetail.stTwoVsTwoInfo.dwWinNum;
                this._1V1TotalCount = (int) roleInfo.pvpDetail.stOneVsOneInfo.dwTotalNum;
                this._1V1WinCount = (int) roleInfo.pvpDetail.stOneVsOneInfo.dwWinNum;
                this._vsAiTotalCount = (int) roleInfo.pvpDetail.stVsMachineInfo.dwTotalNum;
                this._vsAiWinCount = (int) roleInfo.pvpDetail.stVsMachineInfo.dwWinNum;
                this._rankTotalCount = (int) roleInfo.pvpDetail.stLadderInfo.dwTotalNum;
                this._rankWinCount = (int) roleInfo.pvpDetail.stLadderInfo.dwWinNum;
                this._entertainmentTotalCount = (int) roleInfo.pvpDetail.stEntertainmentInfo.dwTotalNum;
                this._entertainmentWinCount = (int) roleInfo.pvpDetail.stEntertainmentInfo.dwWinNum;
                this._heroCnt = roleInfo.GetHaveHeroCount(false);
                this._skinCnt = roleInfo.GetHeroSkinCount(false);
                this._isOnLine = true;
                if (this._mostUsedHeroList == null)
                {
                    this._mostUsedHeroList = new ListView<COMDT_MOST_USED_HERO_INFO>();
                }
                else
                {
                    this._mostUsedHeroList.Clear();
                }
                int num2 = (int) Mathf.Min((float) roleInfo.MostUsedHeroDetail.dwHeroNum, (float) roleInfo.MostUsedHeroDetail.astHeroInfoList.Length);
                for (int j = 0; j < num2; j++)
                {
                    this._mostUsedHeroList.Add(roleInfo.MostUsedHeroDetail.astHeroInfoList[j]);
                }
                this.SortMostUsedHeroList();
                this._selectedHonorId = roleInfo.selectedHonorID;
                Dictionary<int, COMDT_HONORINFO>.Enumerator enumerator = roleInfo.honorDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<int, COMDT_HONORINFO> current = enumerator.Current;
                    int key = current.Key;
                    KeyValuePair<int, COMDT_HONORINFO> pair2 = enumerator.Current;
                    if (!this._honorDic.ContainsKey(pair2.Key))
                    {
                        KeyValuePair<int, COMDT_HONORINFO> pair3 = enumerator.Current;
                        this._honorDic.Add(key, pair3.Value);
                    }
                }
                this._geiLiDuiYou = roleInfo.GeiLiDuiYou;
                this._keJingDuiShou = roleInfo.KeJingDuiShou;
                this._haveExtraCoin = roleInfo.HaveExtraCoin();
                if (this._haveExtraCoin)
                {
                    this._coinExpireHours = roleInfo.GetCoinExpireHours();
                    this._coinWinCount = roleInfo.GetCoinWinCount();
                }
                this._haveExtraExp = roleInfo.HaveExtraExp();
                if (this._haveExtraExp)
                {
                    this._expExpireHours = roleInfo.GetExpExpireHours();
                    this._expWinCount = roleInfo.GetExpWinCount();
                }
            }
        }

        public void ConvertServerDetailData(CSDT_ACNT_DETAIL_INFO detailInfo)
        {
            this.ResetData();
            this._doubleKillCount = 0;
            this._trippleKillCount = 0;
            this._quataryKillCount = 0;
            this._pentaKillCount = 0;
            this._holyShitCount = 0;
            this._mvpCnt = 0;
            this._loseMvpCount = 0;
            if (detailInfo != null)
            {
                this._playerName = StringHelper.UTF8BytesToString(ref detailInfo.szAcntName);
                this.m_personSign = StringHelper.UTF8BytesToString(ref detailInfo.szSignatureInfo);
                this.m_uuid = detailInfo.ullUid;
                this.m_iLogicWorldId = detailInfo.iLogicWorldId;
                this.m_vipInfo = new SCPKG_GAME_VIP_NTF();
                this.m_vipInfo.stGameVipClient = detailInfo.stGameVip;
                this._playerHeadUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref detailInfo.szOpenUrl);
                this._playerLevel = detailInfo.dwLevel;
                ResAcntExpInfo dataByKey = GameDataMgr.acntExpDatabin.GetDataByKey(this._playerLevel);
                this._playerNeedExp = dataByKey.dwNeedExp;
                this._playerExp = detailInfo.dwExp;
                this.creditScore = detailInfo.dwCreditValue;
                this._power = detailInfo.dwPower;
                this._pvpLevel = detailInfo.dwPvpLevel;
                this._playerPvpExp = detailInfo.dwPvpExp;
                this._gender = (COM_SNSGENDER) detailInfo.bGender;
                this._privilegeType = (COM_PRIVILEGE_TYPE) detailInfo.bPrivilege;
                this._rankGrade = detailInfo.bGradeOfRank;
                this._rankHistoryHighestGrade = detailInfo.bMaxGradeOfRank;
                this._rankClass = detailInfo.dwCurClassOfRank;
                this._rankHistoryHighestClass = detailInfo.stRankInfo.dwTopClassOfRank;
                this.GuildName = StringHelper.UTF8BytesToString(ref detailInfo.stGuildInfo.szGuildName);
                this.GuildState = (COM_PLAYER_GUILD_STATE) detailInfo.stGuildInfo.bGuildState;
                this.qqVipMask = detailInfo.dwQQVIPMask;
                for (int i = 0; i < detailInfo.stStatistic.stKVDetail.dwNum; i++)
                {
                    COMDT_STATISTIC_KEY_VALUE_INFO comdt_statistic_key_value_info = detailInfo.stStatistic.stKVDetail.astKVDetail[i];
                    switch (((RES_STATISTIC_SETTLE_DATA_TYPE) comdt_statistic_key_value_info.dwKey))
                    {
                        case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_MVP_CNT:
                            this._mvpCnt = (int) comdt_statistic_key_value_info.dwValue;
                            break;

                        case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_LOSE_SOUL:
                            this._loseMvpCount = (int) comdt_statistic_key_value_info.dwValue;
                            break;

                        case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_GODLIKE_CNT:
                            this._holyShitCount = (int) comdt_statistic_key_value_info.dwValue;
                            break;

                        case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_DOUBLE_KILL_CNT:
                            this._doubleKillCount = (int) comdt_statistic_key_value_info.dwValue;
                            break;

                        case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_TRIPLE_KILL_CNT:
                            this._trippleKillCount = (int) comdt_statistic_key_value_info.dwValue;
                            break;

                        case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_QUATARY_KILL_CNT:
                            this._quataryKillCount = (int) comdt_statistic_key_value_info.dwValue;
                            break;

                        case RES_STATISTIC_SETTLE_DATA_TYPE.RES_STATISTIC_SETTLE_DATA_TYPE_PENTA_KILL_CNT:
                            this._pentaKillCount = (int) comdt_statistic_key_value_info.dwValue;
                            break;
                    }
                }
                this._5V5TotalCount = (int) detailInfo.stStatistic.stFiveVsFiveInfo.dwTotalNum;
                this._5V5WinCount = (int) detailInfo.stStatistic.stFiveVsFiveInfo.dwWinNum;
                this._3V3TotalCount = (int) detailInfo.stStatistic.stThreeVsThreeInfo.dwTotalNum;
                this._3V3WinCount = (int) detailInfo.stStatistic.stThreeVsThreeInfo.dwWinNum;
                this._2V2TotalCount = (int) detailInfo.stStatistic.stTwoVsTwoInfo.dwTotalNum;
                this._2V2WinCount = (int) detailInfo.stStatistic.stTwoVsTwoInfo.dwWinNum;
                this._1V1TotalCount = (int) detailInfo.stStatistic.stOneVsOneInfo.dwTotalNum;
                this._1V1WinCount = (int) detailInfo.stStatistic.stOneVsOneInfo.dwWinNum;
                this._vsAiTotalCount = (int) detailInfo.stStatistic.stVsMachineInfo.dwTotalNum;
                this._vsAiWinCount = (int) detailInfo.stStatistic.stVsMachineInfo.dwWinNum;
                this._rankTotalCount = (int) detailInfo.stStatistic.stLadderInfo.dwTotalNum;
                this._rankWinCount = (int) detailInfo.stStatistic.stLadderInfo.dwWinNum;
                this._entertainmentTotalCount = (int) detailInfo.stStatistic.stEntertainmentInfo.dwTotalNum;
                this._entertainmentWinCount = (int) detailInfo.stStatistic.stEntertainmentInfo.dwWinNum;
                this._heroCnt = (int) detailInfo.stMostUsedHero.dwTotalHeroNum;
                this._skinCnt = (int) detailInfo.stMostUsedHero.dwTotalSkinNum;
                this._isOnLine = detailInfo.bIsOnline != 0;
                if (this._mostUsedHeroList == null)
                {
                    this._mostUsedHeroList = new ListView<COMDT_MOST_USED_HERO_INFO>();
                }
                else
                {
                    this._mostUsedHeroList.Clear();
                }
                int num2 = (int) Mathf.Min((float) detailInfo.stMostUsedHero.dwHeroNum, (float) detailInfo.stMostUsedHero.astHeroInfoList.Length);
                for (int j = 0; j < num2; j++)
                {
                    this._mostUsedHeroList.Add(detailInfo.stMostUsedHero.astHeroInfoList[j]);
                }
                this.SortMostUsedHeroList();
                if ((detailInfo.stHonorInfo == null) || (detailInfo.stHonorInfo.bHonorCnt < 6))
                {
                    Singleton<CRoleInfoManager>.instance.InsertHonorOnDuplicateUpdate(ref this._honorDic, 1, 0);
                    Singleton<CRoleInfoManager>.instance.InsertHonorOnDuplicateUpdate(ref this._honorDic, 2, 0);
                    Singleton<CRoleInfoManager>.instance.InsertHonorOnDuplicateUpdate(ref this._honorDic, 6, 0);
                    Singleton<CRoleInfoManager>.instance.InsertHonorOnDuplicateUpdate(ref this._honorDic, 4, 0);
                    Singleton<CRoleInfoManager>.instance.InsertHonorOnDuplicateUpdate(ref this._honorDic, 5, 0);
                    Singleton<CRoleInfoManager>.instance.InsertHonorOnDuplicateUpdate(ref this._honorDic, 3, 0);
                }
                if (detailInfo.stHonorInfo != null)
                {
                    for (int k = 0; k < detailInfo.stHonorInfo.bHonorCnt; k++)
                    {
                        COMDT_HONORINFO comdt_honorinfo = detailInfo.stHonorInfo.astHonorInfo[k];
                        switch (comdt_honorinfo.iHonorID)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                                Singleton<CRoleInfoManager>.instance.InsertHonorOnDuplicateUpdate(ref this._honorDic, comdt_honorinfo.iHonorID, comdt_honorinfo.iHonorPoint);
                                break;
                        }
                    }
                    COMDT_HONORINFO comdt_honorinfo2 = new COMDT_HONORINFO();
                    if (this._honorDic.TryGetValue(detailInfo.stHonorInfo.iCurUseHonorID, out comdt_honorinfo2))
                    {
                        if (comdt_honorinfo2.iHonorLevel > 0)
                        {
                            this._selectedHonorId = detailInfo.stHonorInfo.iCurUseHonorID;
                        }
                        else
                        {
                            this._selectedHonorId = 0;
                        }
                    }
                }
                this._geiLiDuiYou = (int) detailInfo.stLikeNum.dwTeammateNum;
                this._keJingDuiShou = (int) detailInfo.stLikeNum.dwOpponentNum;
                this._haveExtraCoin = false;
                this._haveExtraExp = false;
            }
        }

        public static float Divide(uint a, uint b)
        {
            if (b == 0)
            {
                return 0f;
            }
            return (((float) a) / ((float) b));
        }

        public uint DoubleKill()
        {
            return (uint) this._doubleKillCount;
        }

        public int EntertainmentTotalGameCnt()
        {
            return this._entertainmentTotalCount;
        }

        public int EntertainmentWinGameCnt()
        {
            return this._entertainmentWinCount;
        }

        public float EntertainmentWins()
        {
            return Divide((uint) this.EntertainmentWinGameCnt(), (uint) this.EntertainmentTotalGameCnt());
        }

        public uint Exp()
        {
            return this._playerExp;
        }

        public uint FightingPower()
        {
            return this._power;
        }

        public COM_SNSGENDER Gender()
        {
            return this._gender;
        }

        public uint GetHistoryHighestRankClass()
        {
            return this._rankHistoryHighestClass;
        }

        public byte GetHistoryHighestRankGrade()
        {
            return this._rankHistoryHighestGrade;
        }

        public Dictionary<int, COMDT_HONORINFO> GetHonorDic()
        {
            return this._honorDic;
        }

        public byte GetRankClass()
        {
            return (byte) this._rankClass;
        }

        public byte GetRankGrade()
        {
            return this._rankGrade;
        }

        public int GetSelectedHonorId()
        {
            return this._selectedHonorId;
        }

        public string HeadPath()
        {
            return string.Empty;
        }

        public string HeadUrl()
        {
            return this._playerHeadUrl;
        }

        public int HeroCnt()
        {
            return this._heroCnt;
        }

        public uint HolyShit()
        {
            return (uint) this._holyShitCount;
        }

        public bool IsOnLine()
        {
            return this._isOnLine;
        }

        public uint LoseSoulCnt()
        {
            return (uint) this._loseMvpCount;
        }

        public ListView<COMDT_MOST_USED_HERO_INFO> MostUsedHeroList()
        {
            return this._mostUsedHeroList;
        }

        public uint MVPCnt()
        {
            return (uint) this._mvpCnt;
        }

        public string Name()
        {
            return this._playerName;
        }

        public uint NeedExp()
        {
            return this._playerNeedExp;
        }

        public uint PentaKill()
        {
            return (uint) this._pentaKillCount;
        }

        public COM_PRIVILEGE_TYPE PrivilegeType()
        {
            return this._privilegeType;
        }

        public int PvmTotalGameCnt()
        {
            return this._vsAiTotalCount;
        }

        public int PvmWinGameCnt()
        {
            return this._vsAiWinCount;
        }

        public float PvmWins()
        {
            return Divide((uint) this.PvmWinGameCnt(), (uint) this.PvmTotalGameCnt());
        }

        public int Pvp1V1TotalGameCnt()
        {
            return this._1V1TotalCount;
        }

        public int Pvp1V1WinGameCnt()
        {
            return this._1V1WinCount;
        }

        public float Pvp1V1Wins()
        {
            return Divide((uint) this.Pvp1V1WinGameCnt(), (uint) this.Pvp1V1TotalGameCnt());
        }

        public int Pvp2V2TotalGameCnt()
        {
            return this._2V2TotalCount;
        }

        public int Pvp2V2WinGameCnt()
        {
            return this._2V2WinCount;
        }

        public float Pvp2V2Wins()
        {
            return Divide((uint) this.Pvp2V2WinGameCnt(), (uint) this.Pvp2V2TotalGameCnt());
        }

        public int Pvp3V3TotalGameCnt()
        {
            return this._3V3TotalCount;
        }

        public int Pvp3V3WinGameCnt()
        {
            return this._3V3WinCount;
        }

        public float Pvp3V3Wins()
        {
            return Divide((uint) this.Pvp3V3WinGameCnt(), (uint) this.Pvp3V3TotalGameCnt());
        }

        public int Pvp5V5TotalGameCnt()
        {
            return this._5V5TotalCount;
        }

        public int Pvp5V5WinGameCnt()
        {
            return this._5V5WinCount;
        }

        public float Pvp5V5Wins()
        {
            return Divide((uint) this.Pvp5V5WinGameCnt(), (uint) this.Pvp5V5TotalGameCnt());
        }

        public uint PvpExp()
        {
            return this._playerPvpExp;
        }

        public uint PvpLevel()
        {
            return this._pvpLevel;
        }

        public uint PvpNeedExp()
        {
            ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint) ((byte) this.PvpLevel()));
            if (dataByKey != null)
            {
                return dataByKey.dwNeedExp;
            }
            return 0;
        }

        public uint QuataryKill()
        {
            return (uint) this._quataryKillCount;
        }

        public int RankTotalGameCnt()
        {
            return this._rankTotalCount;
        }

        public int RankWinGameCnt()
        {
            return this._rankWinCount;
        }

        public float RankWins()
        {
            return Divide((uint) this.RankWinGameCnt(), (uint) this.RankTotalGameCnt());
        }

        private void ResetData()
        {
            this.m_uuid = 0L;
            this.m_iLogicWorldId = 0;
            this.m_vipInfo = new SCPKG_GAME_VIP_NTF();
            this._playerName = null;
            this._playerHeadUrl = null;
            this.m_personSign = null;
            this._playerLevel = 0;
            this._playerExp = 0;
            this._playerNeedExp = 0;
            this._power = 0;
            this._pvpLevel = 0;
            this._playerPvpExp = 0;
            this._doubleKillCount = 0;
            this._trippleKillCount = 0;
            this._quataryKillCount = 0;
            this._pentaKillCount = 0;
            this._holyShitCount = 0;
            this._mvpCnt = 0;
            this._loseMvpCount = 0;
            this._rankGrade = 0;
            this._rankHistoryHighestGrade = 0;
            this._rankClass = 0;
            this._rankHistoryHighestClass = 0;
            this.GuildName = null;
            this.GuildState = COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_NULL;
            this._5V5TotalCount = 0;
            this._5V5WinCount = 0;
            this._3V3TotalCount = 0;
            this._3V3WinCount = 0;
            this._2V2TotalCount = 0;
            this._2V2WinCount = 0;
            this._1V1TotalCount = 0;
            this._1V1WinCount = 0;
            this._vsAiTotalCount = 0;
            this._vsAiWinCount = 0;
            this._privilegeType = COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_NONE;
            this._heroCnt = 0;
            this._skinCnt = 0;
            if (this._mostUsedHeroList == null)
            {
                this._mostUsedHeroList = new ListView<COMDT_MOST_USED_HERO_INFO>();
            }
            else
            {
                this._mostUsedHeroList.Clear();
            }
            this._selectedHonorId = 0;
            this._honorDic.Clear();
            this._haveExtraCoin = false;
            this._coinExpireHours = 0;
            this._coinWinCount = 0;
            this._haveExtraExp = false;
            this._expExpireHours = 0;
            this._expWinCount = 0;
        }

        public static string Round(float value)
        {
            if (Math.Abs((float) (value % 1f)) < float.Epsilon)
            {
                int num = (int) value;
                return num.ToString("D");
            }
            return value.ToString("0.0");
        }

        public int SkinCnt()
        {
            return this._skinCnt;
        }

        private void SortMostUsedHeroList()
        {
            if (this._mostUsedHeroList != null)
            {
                if (<>f__am$cache39 == null)
                {
                    <>f__am$cache39 = delegate (COMDT_MOST_USED_HERO_INFO b, COMDT_MOST_USED_HERO_INFO a) {
                        if ((a == null) && (b == null))
                        {
                            return 0;
                        }
                        if ((a != null) && (b == null))
                        {
                            return 1;
                        }
                        if ((a != null) || (b == null))
                        {
                            if ((a.dwGameWinNum + a.dwGameLoseNum) > (b.dwGameWinNum + b.dwGameLoseNum))
                            {
                                return 1;
                            }
                            if ((a.dwGameWinNum + a.dwGameLoseNum) == (b.dwGameWinNum + b.dwGameLoseNum))
                            {
                                return 0;
                            }
                        }
                        return -1;
                    };
                }
                this._mostUsedHeroList.Sort(<>f__am$cache39);
            }
        }

        public uint TripleKill()
        {
            return (uint) this._trippleKillCount;
        }

        public string GuildName { get; set; }

        public COM_PLAYER_GUILD_STATE GuildState { get; set; }
    }
}

