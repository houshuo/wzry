namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [MessageHandlerClass]
    public class CRoleInfo
    {
        private BitArray _clientBits;
        private bool _clientBitsChanged;
        private uint _extraCoinHours;
        private uint _extraExpHours;
        private uint _extraTimeCoin;
        private uint _extraTimeExp;
        private uint _extraWinCoin;
        private uint _extraWinCoinValue;
        private uint _extraWinExp;
        private uint _extraWinExpValue;
        private uint _firstWinLv = 1;
        private uint _nextFirstWinAvailableTime;
        public List<uint> battleHeroList = new List<uint>();
        private bool bNewbieAchieveChanged;
        public uint creditScore;
        public uint freeHeroExpireTime;
        public List<COMDT_FREEHERO_DETAIL> freeHeroList = new List<COMDT_FREEHERO_DETAIL>();
        public ListView<COMDT_FREEHERO_INFO> freeHeroSymbolList = new ListView<COMDT_FREEHERO_INFO>();
        public int GeiLiDuiYou;
        private const int GET_FREEHERO_DELAY = 15;
        private int GetFreeHeroTimer;
        public int getFriendCoinCnt;
        private BitArray GuidedStateBits;
        private DictionaryView<uint, CHeroInfo> heroDic = new DictionaryView<uint, CHeroInfo>();
        public Dictionary<uint, uint> heroExperienceSkinDic = new Dictionary<uint, uint>();
        private Dictionary<uint, ulong> heroSkinDic = new Dictionary<uint, ulong>();
        public Dictionary<int, COMDT_HONORINFO> honorDic = new Dictionary<int, COMDT_HONORINFO>();
        private Dictionary<int, stInBattleLevelBits> inBattleNewbieBits;
        public int KeJingDuiShou;
        public CAchieveInfo m_achieveInfo = new CAchieveInfo();
        private uint m_arenaCoin;
        public List<uint> m_arenaDefHeroList = new List<uint>();
        public GuildBaseInfo m_baseGuildInfo;
        private uint m_burningCoin;
        public int m_coinDrawIndex;
        public stCoinGetInfoDaily[] m_coinGetInfoDaily = new stCoinGetInfoDaily[3];
        private uint m_curActionPoint;
        public Dictionary<uint, ushort[]> m_customRecommendEquipDictionary;
        public uint m_customRecommendEquipsLastChangedHeroID;
        private uint m_diamond;
        public uint m_DiamondOpenBoxCnt;
        private ulong m_dianquan;
        private uint m_exp;
        private uint m_expPool;
        public GuildExtInfo m_extGuildInfo;
        private uint m_firstLoginTime;
        private long m_firstLoginZeroDay;
        public stShopBuyDrawInfo[] m_freeDrawInfo;
        private byte m_gameDifficult;
        public COM_SNSGENDER m_gender;
        public static int m_globalRefreshTimerSeq = -1;
        private uint m_goldCoin;
        private CrypticInt32 m_headId;
        private string m_HeadUrl;
        public uint m_initHeroId;
        private CUseableContainer m_itemContainer;
        private uint m_JiFen;
        private uint m_level;
        public CLicenseInfo m_licenseInfo = new CLicenseInfo();
        private byte m_MaterialDirectBuyLimit;
        private uint m_maxActionPoint;
        public uint m_maxSkillPt;
        private string m_Name;
        private uint m_needExp;
        private int m_newDayTimer = -1;
        public int m_nHeroSkinCount;
        public uint m_payLevel;
        private string m_personSign;
        public COM_PRIVILEGE_TYPE m_privilegeType;
        private uint m_pvpExp;
        private uint m_pvpLevel;
        public uint m_rankClass;
        public ulong m_rankCurSeasonStartTime;
        public byte m_rankGrade;
        public uint m_rankHistoryHighestClass;
        public byte m_rankHistoryHighestGrade;
        public uint m_rankSeasonHighestClass;
        public byte m_rankSeasonHighestGrade;
        public enROLEINFO_TYPE m_roleType = enROLEINFO_TYPE.PLAYER;
        public uint m_skillPoint;
        public int m_skillPtRefreshSec;
        private uint m_skinCoin;
        private uint m_symbolCoin;
        public CSymbolInfo m_symbolInfo = new CSymbolInfo();
        private CrypticInt32 m_titleId;
        public uint m_updateTimeMSec;
        private uint m_vipFlags;
        private bool m_vipFlagsValid;
        private SCPKG_GAME_VIP_NTF m_vipInfo = new SCPKG_GAME_VIP_NTF();
        private int m_vipReadTimer;
        private BitArray NewbieAchieveBits;
        public PVE_ADV_COMPLETE_INFO[] pveLevelDetail = new PVE_ADV_COMPLETE_INFO[4];
        public CSDT_PVPDETAIL_INFO pvpDetail = new CSDT_PVPDETAIL_INFO();
        private static int s_sysTime;
        private static float s_upToLoginSec;
        public int selectedHonorID;
        public uint snsSwitchBits;
        private COMDT_MOST_USED_HERO_DETAIL stMostUsedHero = new COMDT_MOST_USED_HERO_DETAIL();

        public CRoleInfo(enROLEINFO_TYPE type, ulong uuID)
        {
            this.playerUllUID = uuID;
            this.m_roleType = type;
            this.m_itemContainer = new CUseableContainer(enCONTAINER_TYPE.ITEM);
            this.m_baseGuildInfo = new GuildBaseInfo();
            this.m_extGuildInfo = new GuildExtInfo();
            this.m_vipFlags = 0;
            this.m_vipFlagsValid = false;
            if (this.m_licenseInfo == null)
            {
                this.m_licenseInfo = new CLicenseInfo();
            }
            this.m_licenseInfo.InitLicenseCfgInfo();
            for (int i = 0; i < 3; i++)
            {
                this.m_coinGetInfoDaily[i] = new stCoinGetInfoDaily();
            }
        }

        public bool CheckCoinEnough(RES_SHOPBUY_COINTYPE coinType, uint targetValue)
        {
            switch (coinType)
            {
                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
                    return (this.DianQuan >= targetValue);

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
                    return (this.GoldCoin >= targetValue);

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
                    return (this.BurningCoin >= targetValue);

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
                    return (this.ArenaCoin >= targetValue);

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SKINCOIN:
                    return (this.SkinCoin >= targetValue);

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SYMBOLCOIN:
                    return (this.SymbolCoin >= targetValue);

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND:
                    return (this.Diamond >= targetValue);

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_MIXPAY:
                    return ((this.Diamond + this.DianQuan) >= targetValue);
            }
            return false;
        }

        public bool CheckHeroBuyable(uint inHeroId, RES_SHOPBUY_COINTYPE coinType)
        {
            IHeroData data = CHeroDataFactory.CreateHeroData(inHeroId);
            if (data != null)
            {
                if (data.bPlayerOwn)
                {
                    return false;
                }
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                ResHeroShop shop = null;
                GameDataMgr.heroShopInfoDict.TryGetValue(inHeroId, out shop);
                if ((masterRoleInfo == null) || (shop == null))
                {
                    return false;
                }
                switch (coinType)
                {
                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
                        if (masterRoleInfo.DianQuan < shop.dwBuyCoupons)
                        {
                            break;
                        }
                        return true;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
                        if (masterRoleInfo.GoldCoin < shop.dwBuyCoin)
                        {
                            break;
                        }
                        return true;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
                        if (masterRoleInfo.BurningCoin < shop.dwBuyBurnCoin)
                        {
                            break;
                        }
                        return true;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
                        if (masterRoleInfo.ArenaCoin < shop.dwBuyArenaCoin)
                        {
                            break;
                        }
                        return true;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND:
                        if (masterRoleInfo.Diamond < shop.dwBuyDiamond)
                        {
                            break;
                        }
                        return true;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_MIXPAY:
                        if ((masterRoleInfo.DianQuan + masterRoleInfo.Diamond) < shop.dwBuyDiamond)
                        {
                            break;
                        }
                        return true;
                }
            }
            return false;
        }

        public void CleanUpBattleRecord()
        {
            for (int i = 0; i < this.pvpDetail.stKVDetail.dwNum; i++)
            {
                COMDT_STATISTIC_KEY_VALUE_INFO comdt_statistic_key_value_info = this.pvpDetail.stKVDetail.astKVDetail[i];
                comdt_statistic_key_value_info.dwValue = 0;
            }
            this.pvpDetail.stFiveVsFiveInfo.dwTotalNum = 0;
            this.pvpDetail.stFiveVsFiveInfo.dwWinNum = 0;
            this.pvpDetail.stThreeVsThreeInfo.dwTotalNum = 0;
            this.pvpDetail.stThreeVsThreeInfo.dwWinNum = 0;
            this.pvpDetail.stTwoVsTwoInfo.dwTotalNum = 0;
            this.pvpDetail.stTwoVsTwoInfo.dwWinNum = 0;
            this.pvpDetail.stOneVsOneInfo.dwTotalNum = 0;
            this.pvpDetail.stOneVsOneInfo.dwWinNum = 0;
            this.pvpDetail.stVsMachineInfo.dwTotalNum = 0;
            this.pvpDetail.stVsMachineInfo.dwWinNum = 0;
            this.pvpDetail.stLadderInfo.dwTotalNum = 0;
            this.pvpDetail.stLadderInfo.dwWinNum = 0;
            this.pvpDetail.stEntertainmentInfo.dwTotalNum = 0;
            this.pvpDetail.stEntertainmentInfo.dwWinNum = 0;
            this.MostUsedHeroDetail.dwHeroNum = 0;
            Singleton<CPlayerPvpHistoryController>.instance.ClearHostData();
        }

        public void Clear()
        {
            this.getFriendCoinCnt = 0;
            this.heroDic.Clear();
            this.heroSkinDic.Clear();
            this.heroExperienceSkinDic.Clear();
            this.GetUseableContainer(enCONTAINER_TYPE.ITEM).Clear();
            this.honorDic.Clear();
        }

        public void GetCoinDailyInfo(RES_COIN_GET_PATH_TYPE pathType, out uint getCnt, out uint limitCnt)
        {
            getCnt = 0;
            limitCnt = 0;
            switch (pathType)
            {
                case RES_COIN_GET_PATH_TYPE.RES_COIN_GET_PATH_PVP_BATTLE:
                    getCnt = this.m_coinGetInfoDaily[0].GetCntDaily;
                    limitCnt = this.m_coinGetInfoDaily[0].LimitCntDaily;
                    break;

                case RES_COIN_GET_PATH_TYPE.RES_COIN_GET_PATH_TASK_REWARD:
                    getCnt = this.m_coinGetInfoDaily[1].GetCntDaily;
                    limitCnt = this.m_coinGetInfoDaily[1].LimitCntDaily;
                    break;

                case RES_COIN_GET_PATH_TYPE.RES_COIN_GET_PATH_FRIEND:
                    getCnt = this.m_coinGetInfoDaily[2].GetCntDaily;
                    limitCnt = this.m_coinGetInfoDaily[2].LimitCntDaily;
                    break;
            }
        }

        public int GetCoinExpireHours()
        {
            return this.GetExpireHours(this._extraCoinHours);
        }

        public uint GetCoinWinCount()
        {
            return this._extraWinCoinValue;
        }

        public static ResCreditLevelInfo GetCreditLevelInfo(int creditLevel)
        {
            ResCreditLevelInfo dataByIndex = null;
            int count = GameDataMgr.creditLevelDatabin.count;
            for (int i = 0; i < count; i++)
            {
                dataByIndex = GameDataMgr.creditLevelDatabin.GetDataByIndex(i);
                if (dataByIndex.bCreditLevel == creditLevel)
                {
                    return dataByIndex;
                }
                dataByIndex = null;
            }
            return dataByIndex;
        }

        public static ResCreditLevelInfo GetCreditLevelInfoByScore(int creditScore)
        {
            ResCreditLevelInfo anyData = GameDataMgr.creditLevelDatabin.GetAnyData();
            int count = GameDataMgr.creditLevelDatabin.count;
            for (int i = 0; i < count; i++)
            {
                anyData = GameDataMgr.creditLevelDatabin.GetDataByIndex(i);
                if ((anyData.dwCreditThresholdLow <= creditScore) && (anyData.dwCreditThresholdHigh >= creditScore))
                {
                    return anyData;
                }
            }
            return anyData;
        }

        public ResPVPRatio GetCurDailyAdd()
        {
            ResPVPRatio ratio = null;
            uint key = 0;
            DictionaryView<uint, ResPVPRatio>.Enumerator enumerator = GameDataMgr.pvpRatioDict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, ResPVPRatio> current = enumerator.Current;
                key = current.Key;
                KeyValuePair<uint, ResPVPRatio> pair2 = enumerator.Current;
                ratio = pair2.Value;
                if (this.dailyPvpCnt <= key)
                {
                    return ratio;
                }
            }
            return ratio;
        }

        public int GetCurFirstWinRemainingTimeSec()
        {
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null)
            {
                uint currentUTCTime = (uint) GetCurrentUTCTime();
                if (this._nextFirstWinAvailableTime >= currentUTCTime)
                {
                    return (int) (this._nextFirstWinAvailableTime - currentUTCTime);
                }
            }
            return -1;
        }

        public static int GetCurrentUTCTime()
        {
            int num = (int) (Time.realtimeSinceStartup - s_upToLoginSec);
            return (s_sysTime + num);
        }

        public static int GetElapseSecondsSinceLogin()
        {
            return (int) (Time.realtimeSinceStartup - s_upToLoginSec);
        }

        public int GetExperienceHeroLeftTime(uint heroId)
        {
            CHeroInfo info;
            if (this.heroDic.TryGetValue(heroId, out info) && info.IsExperienceHero())
            {
                return (((int) info.m_experienceDeadLine) - GetCurrentUTCTime());
            }
            return 0;
        }

        public int GetExperienceHeroValidDays(uint heroId)
        {
            CHeroInfo info;
            if (this.heroDic.TryGetValue(heroId, out info))
            {
                return CHeroInfo.GetExperienceHeroOrSkinValidDays(info.m_experienceDeadLine);
            }
            return 0;
        }

        public int GetExperienceSkinLeftTime(uint heroId, uint skinId)
        {
            uint skinCfgId = CSkinInfo.GetSkinCfgId(heroId, skinId);
            if (this.heroExperienceSkinDic.ContainsKey(skinCfgId))
            {
                return (this.heroExperienceSkinDic[skinCfgId] - GetCurrentUTCTime());
            }
            return 0;
        }

        public int GetExpExpireHours()
        {
            return this.GetExpireHours(this._extraExpHours);
        }

        private int GetExpireHours(uint value)
        {
            return UT.CalcDeltaHorus((uint) GetCurrentUTCTime(), value);
        }

        public uint GetExpWinCount()
        {
            return this._extraWinExpValue;
        }

        public uint GetFirstHeroId()
        {
            return this.m_initHeroId;
        }

        public stShopBuyDrawInfo GetFreeDrawInfo(COM_SHOP_DRAW_TYPE drawType)
        {
            int index = (int) drawType;
            if (index < this.m_freeDrawInfo.Length)
            {
                return this.m_freeDrawInfo[index];
            }
            return new stShopBuyDrawInfo();
        }

        public COMDT_FREEHERO_INFO GetFreeHeroSymbol(uint heroId)
        {
            for (int i = 0; i < this.freeHeroSymbolList.Count; i++)
            {
                if (this.freeHeroSymbolList[i].dwHeroID == heroId)
                {
                    return this.freeHeroSymbolList[i];
                }
            }
            return null;
        }

        public byte GetFreeHeroSymbolId(uint heroId)
        {
            for (int i = 0; i < this.freeHeroSymbolList.Count; i++)
            {
                if (this.freeHeroSymbolList[i].dwHeroID == heroId)
                {
                    return this.freeHeroSymbolList[i].bSymbolPageWear;
                }
            }
            return 0;
        }

        public uint GetFreeHeroWearSkinId(uint heroId)
        {
            for (int i = 0; i < this.freeHeroSymbolList.Count; i++)
            {
                if (this.freeHeroSymbolList[i].dwHeroID == heroId)
                {
                    return this.freeHeroSymbolList[i].wSkinID;
                }
            }
            return 0;
        }

        public uint GetGuideLevel2FadeHeroId()
        {
            uint dwConfValue = 0;
            if (GameDataMgr.globalInfoDatabin != null)
            {
                ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x66);
                if (dataByKey != null)
                {
                    dwConfValue = dataByKey.dwConfValue;
                }
            }
            return dwConfValue;
        }

        public int GetHaveHeroCount(bool isIncludeValidExperienceHero)
        {
            int num = 0;
            DictionaryView<uint, CHeroInfo>.Enumerator enumerator = this.heroDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (isIncludeValidExperienceHero)
                {
                    KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
                    if (!this.IsOwnHero(current.Key))
                    {
                        KeyValuePair<uint, CHeroInfo> pair2 = enumerator.Current;
                        if (!this.IsValidExperienceHero(pair2.Key))
                        {
                            continue;
                        }
                    }
                    num++;
                }
                else
                {
                    KeyValuePair<uint, CHeroInfo> pair3 = enumerator.Current;
                    if (this.IsOwnHero(pair3.Key))
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        public CHeroInfo GetHeroInfo(uint id, bool isIncludeValidExperienceHero = false)
        {
            if (isIncludeValidExperienceHero)
            {
                if (this.IsOwnHero(id) || this.IsValidExperienceHero(id))
                {
                    return this.heroDic[id];
                }
            }
            else if (this.IsOwnHero(id))
            {
                return this.heroDic[id];
            }
            return null;
        }

        public bool GetHeroInfo(uint id, out CHeroInfo info, bool isIncludeValidExperienceHero = false)
        {
            if (isIncludeValidExperienceHero)
            {
                if (this.IsOwnHero(id) || this.IsValidExperienceHero(id))
                {
                    info = this.heroDic[id];
                    return true;
                }
            }
            else if (this.IsOwnHero(id))
            {
                info = this.heroDic[id];
                return true;
            }
            info = null;
            return false;
        }

        public DictionaryView<uint, CHeroInfo> GetHeroInfoDic()
        {
            return this.heroDic;
        }

        public static void GetHeroPreLevleAndExp(uint HeroId, uint inDltExp, out int preLevel, out uint preExp)
        {
            CHeroInfo info;
            preExp = 0;
            preLevel = 0;
            if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().heroDic.TryGetValue(HeroId, out info))
            {
                preLevel = info.mActorValue.actorLvl;
                preExp = (uint) info.mActorValue.actorExp;
                uint num = inDltExp;
                while (num > preExp)
                {
                    if (preLevel == 1)
                    {
                        break;
                    }
                    num -= preExp;
                    preLevel--;
                    preExp = GameDataMgr.heroLvlUpDatabin.GetDataByKey((uint) preLevel).dwExp;
                }
                preExp -= num;
            }
        }

        public int GetHeroSkinCount(bool isIncludeValidExperienceHero)
        {
            int num = 0;
            Dictionary<uint, ulong>.Enumerator enumerator = this.heroSkinDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, ulong> current = enumerator.Current;
                ulong num2 = current.Value;
                for (int i = 1; i < 20; i++)
                {
                    if ((num2 & (((ulong) 1L) << i)) > 0L)
                    {
                        num++;
                    }
                }
            }
            if (isIncludeValidExperienceHero)
            {
                num += this.heroExperienceSkinDic.Count;
            }
            return num;
        }

        public string GetHeroSkinPic(uint heroId)
        {
            return CSkinInfo.GetHeroSkinPic(heroId, this.GetHeroWearSkinId(heroId));
        }

        public HeroSkinState GetHeroSkinState(uint heroId, uint skinId)
        {
            if (this.IsHaveHero(heroId, false))
            {
                if (this.IsHaveHeroSkin(heroId, skinId, false))
                {
                    if (this.GetHeroWearSkinId(heroId) == skinId)
                    {
                        return HeroSkinState.NormalHero_NormalSkin_Wear;
                    }
                    return HeroSkinState.NormalHero_NormalSkin_Own;
                }
                if (!this.IsValidExperienceSkin(heroId, skinId))
                {
                    return HeroSkinState.NormalHero_Skin_NotOwn;
                }
                if (this.GetHeroWearSkinId(heroId) == skinId)
                {
                    return HeroSkinState.NormalHero_LimitSkin_Wear;
                }
                return HeroSkinState.NormalHero_LimitSkin_Own;
            }
            if (this.IsValidExperienceHero(heroId) || this.IsFreeHero(heroId))
            {
                if (this.IsHaveHeroSkin(heroId, skinId, false) || (skinId == 0))
                {
                    if (this.GetHeroWearSkinId(heroId) == skinId)
                    {
                        return HeroSkinState.LimitHero_NormalSkin_Wear;
                    }
                    return HeroSkinState.LimitHero_NormalSkin_Own;
                }
                if (!this.IsValidExperienceSkin(heroId, skinId))
                {
                    return HeroSkinState.LimitHero_Skin_NotOwn;
                }
                if (this.GetHeroWearSkinId(heroId) == skinId)
                {
                    return HeroSkinState.LimitHero_LimitSkin_Wear;
                }
                return HeroSkinState.LimitHero_LimitSkin_Own;
            }
            if (skinId == 0)
            {
                return HeroSkinState.NoHero_Skin_Wear;
            }
            if (this.IsHaveHeroSkin(heroId, skinId, false))
            {
                return HeroSkinState.NoHero_NormalSkin_Own;
            }
            if (this.IsValidExperienceSkin(heroId, skinId))
            {
                return HeroSkinState.NoHero_LimitSkin_Own;
            }
            return HeroSkinState.NoHero_Skin_NotOwn;
        }

        public uint GetHeroWearSkinId(uint heroId)
        {
            if (this.heroDic.ContainsKey(heroId))
            {
                return this.heroDic[heroId].m_skinInfo.GetWearSkinId();
            }
            if (this.IsFreeHero(heroId))
            {
                return this.GetFreeHeroWearSkinId(heroId);
            }
            return 0;
        }

        public SCPKG_GAME_VIP_NTF GetNobeInfo()
        {
            return this.m_vipInfo;
        }

        public static void GetPlayerPreLevleAndExp(uint inDltExp, out int preLevel, out uint preExp)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            preLevel = (int) masterRoleInfo.Level;
            preExp = masterRoleInfo.Exp;
            uint num = inDltExp;
            while (num > preExp)
            {
                if (preLevel == 1)
                {
                    break;
                }
                num -= preExp;
                preLevel--;
                preExp = GameDataMgr.acntExpDatabin.GetDataByKey((uint) preLevel).dwNeedExp;
            }
            preExp -= num;
        }

        public CUseableContainer GetUseableContainer(enCONTAINER_TYPE type)
        {
            CUseableContainer container = null;
            if (type != enCONTAINER_TYPE.ITEM)
            {
                return container;
            }
            return this.m_itemContainer;
        }

        public bool HasVip(int vipBitFlag)
        {
            return (this.m_vipFlagsValid && ((this.m_vipFlags & vipBitFlag) > 0));
        }

        public bool HaveExtraCoin()
        {
            return ((this._extraTimeCoin > 0) || (this._extraWinCoin > 0));
        }

        public bool HaveExtraExp()
        {
            return ((this._extraTimeExp > 0) || (this._extraWinExp > 0));
        }

        private void InitAllHeroExperienceSkin(COMDT_HERO_LIMIT_SKIN_LIST heroExperienceSkinList)
        {
            this.heroExperienceSkinDic.Clear();
            for (int i = 0; i < heroExperienceSkinList.dwNum; i++)
            {
                if (!this.heroExperienceSkinDic.ContainsKey(heroExperienceSkinList.astSkinList[i].dwSkinID))
                {
                    this.heroExperienceSkinDic.Add(heroExperienceSkinList.astSkinList[i].dwSkinID, heroExperienceSkinList.astSkinList[i].dwDeadLine);
                }
            }
        }

        private void InitAllHeroSkin(COMDT_HERO_SKIN_LIST heroSkinList)
        {
            this.heroSkinDic.Clear();
            for (int i = 0; i < heroSkinList.dwHeroNum; i++)
            {
                if (!this.heroSkinDic.ContainsKey(heroSkinList.astHeroSkinList[i].dwHeroID))
                {
                    this.heroSkinDic.Add(heroSkinList.astHeroSkinList[i].dwHeroID, heroSkinList.astHeroSkinList[i].ullSkinBits);
                }
            }
        }

        private void InitBattleHeroList(COMDT_BATTLELIST_LIST battleHeroList)
        {
        }

        public void InitClientBits(COMDT_NEWCLIENT_BITS bits)
        {
            int num = bits.BitsDetail.Length * 0x40;
            bool[] values = new bool[num];
            for (int i = 0; i < num; i++)
            {
                int index = i / 0x40;
                int num4 = i % 0x40;
                values[i] = (bits.BitsDetail[index] & (((ulong) 1L) << num4)) > 0L;
            }
            this._clientBits = new BitArray(values);
        }

        public void InitGuidedStateBits(COMDT_NEWBIE_STATUS_BITS bits)
        {
            int num = bits.BitsDetail.Length * 0x40;
            bool[] values = new bool[num];
            for (int i = 0; i < num; i++)
            {
                int index = i / 0x40;
                int num4 = i % 0x40;
                values[i] = (bits.BitsDetail[index] & (((ulong) 1L) << num4)) > 0L;
            }
            this.GuidedStateBits = new BitArray(values);
        }

        public void InitHero(COMDT_HEROINFO heroInfo)
        {
            if (GameDataMgr.heroDatabin.GetDataByKey(heroInfo.stCommonInfo.dwHeroID) != null)
            {
                if (!this.heroDic.ContainsKey(heroInfo.stCommonInfo.dwHeroID))
                {
                    CHeroInfo info2 = new CHeroInfo();
                    this.heroDic.Add(heroInfo.stCommonInfo.dwHeroID, info2);
                }
                this.heroDic[heroInfo.stCommonInfo.dwHeroID].Init(this.playerUllUID, heroInfo);
            }
        }

        private void InitializeCustomRecommendEquip(COMDT_SELFDEFINE_EQUIP_INFO selfDefineEquipInfo)
        {
            if (this.m_customRecommendEquipDictionary == null)
            {
                this.m_customRecommendEquipDictionary = new Dictionary<uint, ushort[]>();
            }
            this.m_customRecommendEquipsLastChangedHeroID = selfDefineEquipInfo.dwLastChgHeroId;
            for (int i = 0; i < selfDefineEquipInfo.wHeroNum; i++)
            {
                uint dwHeroId = selfDefineEquipInfo.astEquipInfoList[i].dwHeroId;
                ushort[] numArray = new ushort[6];
                for (int j = 0; j < 6; j++)
                {
                    numArray[j] = (ushort) selfDefineEquipInfo.astEquipInfoList[i].HeroEquipList[j];
                }
                if (this.m_customRecommendEquipDictionary.ContainsKey(dwHeroId))
                {
                    this.m_customRecommendEquipDictionary.Remove(dwHeroId);
                }
                this.m_customRecommendEquipDictionary.Add(dwHeroId, numArray);
            }
        }

        public void InitInBattleNewbieBits(COMDT_INBATTLE_NEWBIE_BITS_DETAIL inBattleBits)
        {
            this.inBattleNewbieBits = new Dictionary<int, stInBattleLevelBits>();
            for (int i = 0; i < inBattleBits.bLevelNum; i++)
            {
                COMDT_INBATTLE_NEWBIE_BITS_INFO comdt_inbattle_newbie_bits_info = inBattleBits.astLevelDetail[i];
                if (!this.inBattleNewbieBits.ContainsKey(comdt_inbattle_newbie_bits_info.iLevelID))
                {
                    stInBattleLevelBits bits = new stInBattleLevelBits {
                        bReportFinished = comdt_inbattle_newbie_bits_info.bReportFinished,
                        finishedDetail = new List<uint>()
                    };
                    for (int j = 0; j < comdt_inbattle_newbie_bits_info.bFinishedNum; j++)
                    {
                        bits.finishedDetail.Add(comdt_inbattle_newbie_bits_info.FinishedDetail[j]);
                    }
                    this.inBattleNewbieBits.Add(comdt_inbattle_newbie_bits_info.iLevelID, bits);
                }
            }
        }

        public void InitNewbieAchieveBits(COMDT_CLIENT_BITS bits)
        {
            int num = bits.BitsDetail.Length * 0x40;
            bool[] values = new bool[num];
            for (int i = 0; i < num; i++)
            {
                int index = i / 0x40;
                int num4 = i % 0x40;
                values[i] = (bits.BitsDetail[index] & (((ulong) 1L) << num4)) > 0L;
            }
            this.NewbieAchieveBits = new BitArray(values);
        }

        public bool IsCanBuySkinButNotHaveHero(uint heroId, uint skinId)
        {
            return (CSkinInfo.IsCanBuy(heroId, skinId) && !this.IsHaveHero(heroId, false));
        }

        public bool IsCanUseHero(uint heroId)
        {
            return (this.IsHaveHero(heroId, true) || this.IsFreeHero(heroId));
        }

        public bool IsCanUseSkin(uint heroId, uint skinId)
        {
            return (this.IsCanUseHero(heroId) && ((skinId == 0) || this.IsHaveHeroSkin(heroId, skinId, true)));
        }

        public bool IsClientBitsSet(int inIndex)
        {
            return this._clientBits.Get(inIndex);
        }

        public bool IsCreditFreeHero(uint heroId)
        {
            for (int i = 0; i < this.freeHeroList.Count; i++)
            {
                if (this.freeHeroList[i].dwFreeHeroID == heroId)
                {
                    return (this.freeHeroList[i].dwCreditLevel > 0);
                }
            }
            return false;
        }

        public bool IsExperienceHero(uint heroId)
        {
            CHeroInfo info;
            return this.heroDic.TryGetValue(heroId, out info);
        }

        public bool IsFirstWinOpen()
        {
            return (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().PvpLevel >= this._firstWinLv);
        }

        public bool IsFreeHero(uint heroId)
        {
            for (int i = 0; i < this.freeHeroList.Count; i++)
            {
                if (this.freeHeroList[i].dwFreeHeroID == heroId)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsFreeHeroAndSelfDontHave(uint heroId)
        {
            bool flag = true;
            if (this.heroDic.ContainsKey(heroId))
            {
                return false;
            }
            if (!this.IsFreeHero(heroId))
            {
                flag = false;
            }
            return flag;
        }

        public bool IsGuidedStateSet(int inIndex)
        {
            return this.GuidedStateBits.Get(inIndex);
        }

        public bool IsHaveHero(uint id, bool isIncludeValidExperienceHero)
        {
            if (isIncludeValidExperienceHero)
            {
                return (this.IsOwnHero(id) || this.IsValidExperienceHero(id));
            }
            return this.IsOwnHero(id);
        }

        public bool IsHaveHeroSkin(uint skinUniId, bool isIncludeValidExperienceSkin = false)
        {
            uint num;
            uint num2;
            CSkinInfo.ResolveHeroSkin(skinUniId, out num, out num2);
            return this.IsHaveHeroSkin(num, num2, isIncludeValidExperienceSkin);
        }

        public bool IsHaveHeroSkin(uint heroId, uint skinId, bool isIncludeValidExperienceSkin = false)
        {
            if (this.heroSkinDic.ContainsKey(heroId))
            {
                ulong num = this.heroSkinDic[heroId];
                if ((num & (((ulong) 1L) << skinId)) > 0L)
                {
                    return true;
                }
            }
            return (isIncludeValidExperienceSkin && this.IsValidExperienceSkin(heroId, skinId));
        }

        public bool IsNewbieAchieveSet(int inIndex)
        {
            if (inIndex >= this.NewbieAchieveBits.Count)
            {
                return false;
            }
            return this.NewbieAchieveBits.Get(inIndex);
        }

        public bool IsOldPlayer()
        {
            return ((this.GuidedStateBits != null) && this.GuidedStateBits[0x69]);
        }

        public bool IsOldPlayerGuided()
        {
            return ((this.NewbieAchieveBits != null) && this.NewbieAchieveBits[0x11]);
        }

        public bool IsOwnHero(uint heroId)
        {
            CHeroInfo info;
            return (this.heroDic.TryGetValue(heroId, out info) && !info.IsExperienceHero());
        }

        public bool IsTrainingLevelFin()
        {
            bool flag = this.IsGuidedStateSet(0x53) & this.IsGuidedStateSet(0x54);
            flag &= this.IsGuidedStateSet(0x55);
            flag &= this.IsGuidedStateSet(0x62);
            return (flag & this.IsGuidedStateSet(0));
        }

        public bool IsValidExperienceHero(uint heroId)
        {
            CHeroInfo info;
            return (this.heroDic.TryGetValue(heroId, out info) && info.IsValidExperienceHero());
        }

        public bool IsValidExperienceSkin(uint heroId, uint skinId)
        {
            uint skinCfgId = CSkinInfo.GetSkinCfgId(heroId, skinId);
            return (this.heroExperienceSkinDic.ContainsKey(skinCfgId) && (GetCurrentUTCTime() < ((ulong) this.heroExperienceSkinDic[skinCfgId])));
        }

        public void OnAddHeroSkin(uint heroId, uint skinId)
        {
            ulong num = ((ulong) 1L) << skinId;
            if (this.heroSkinDic.ContainsKey(heroId))
            {
                Dictionary<uint, ulong> dictionary;
                uint num2;
                ulong num3 = dictionary[num2];
                (dictionary = this.heroSkinDic)[num2 = heroId] = num3 | num;
            }
            else
            {
                this.heroSkinDic.Add(heroId, (ulong) skinId);
                this.heroSkinDic[heroId] = num;
            }
        }

        [MessageHandler(0x1194)]
        public static void OnDailyCheckDataNtf(CSPkg msg)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.dailyPvpCnt = msg.stPkgData.stDailyCheckDataNtf.dwDailyPvpCnt;
            }
        }

        private void OnGlobalRefreshTimerEnd(int seq)
        {
            m_globalRefreshTimerSeq = -1;
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.GLOBAL_REFRESH_TIME);
            this.SetGlobalRefreshTimer((uint) GetCurrentUTCTime(), false);
        }

        public void OnGmAddAllSkin()
        {
            DictionaryView<uint, CHeroInfo>.Enumerator enumerator = this.heroDic.GetEnumerator();
            uint key = 0;
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
                key = current.Key;
                if (this.heroSkinDic.ContainsKey(key))
                {
                    this.heroSkinDic[key] = ulong.MaxValue;
                }
                else
                {
                    this.heroSkinDic.Add(key, ulong.MaxValue);
                }
            }
        }

        public void OnHeroInfoUpdate(SCPKG_NTF_HERO_INFO_UPD svrHeroInfoUp)
        {
            CHeroInfo info;
            if (this.GetHeroInfo(svrHeroInfoUp.dwHeroID, out info, true))
            {
                info.OnHeroInfoUpdate(svrHeroInfoUp);
            }
        }

        public void OnLevelUp(SCPKG_NTF_ACNT_LEVELUP ntfAcntLevelUp)
        {
            this.m_maxActionPoint = ntfAcntLevelUp.dwNewMaxAP;
            this.m_curActionPoint = ntfAcntLevelUp.dwNewCurAP;
            this.SetLevel(ntfAcntLevelUp.dwNewLevel, (CS_ACNT_UPDATE_FROMTYPE) ntfAcntLevelUp.bFromType);
            this.m_exp = ntfAcntLevelUp.dwNewExp;
            Singleton<EventRouter>.instance.BroadCastEvent("MasterAttributesChanged");
        }

        private void OnNewDayNtf(int timerSequence)
        {
            uint num = Utility.GetNewDayDeltaSec(GetCurrentUTCTime()) * 0x3e8;
            CTimer timer = Singleton<CTimerManager>.instance.GetTimer(timerSequence);
            timer.ResetTotalTime((int) num);
            timer.Reset();
            timer.Resume();
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.NEWDAY_NTF);
        }

        [MessageHandler(0x501)]
        public static void OnNextFirstWinTimeChange(CSPkg msg)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                CSPKG_NEXTFIRSTWINSEC_NTF stNextFirstWinSecNtf = msg.stPkgData.stNextFirstWinSecNtf;
                masterRoleInfo.SetFirstWinRemainingTime(stNextFirstWinSecNtf.dwNextFirstWinSec);
            }
        }

        public void OnPvpLevelUp(SCPKG_NTF_ACNT_PVPLEVELUP ntfAcntPvpLevelUp)
        {
            int pvpLevel = (int) this.m_pvpLevel;
            this.SetPvpLevel(ntfAcntPvpLevelUp.dwNewLevel);
            this.m_pvpExp = ntfAcntPvpLevelUp.dwNewExp;
            this.m_symbolInfo.SetSymbolPageMaxLevel();
            this.m_symbolInfo.SetSymbolPageCount(ntfAcntPvpLevelUp.bSymbolPageCnt);
            Singleton<EventRouter>.instance.BroadCastEvent("MasterPvpLevelChanged");
            Singleton<EventRouter>.instance.BroadCastEvent("MasterAttributesChanged");
            if (((pvpLevel > 0) && (this.m_pvpLevel > pvpLevel)) && ((Singleton<GameStateCtrl>.GetInstance().GetCurrentState() is LobbyState) && (ntfAcntPvpLevelUp.bFromType != 0)))
            {
                CUIEvent uiEvent = new CUIEvent {
                    m_eventID = enUIEventID.Settle_OpenLvlUp
                };
                uiEvent.m_eventParams.tag = pvpLevel;
                uiEvent.m_eventParams.tag2 = (int) this.m_pvpLevel;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
            }
        }

        public void OnUpdate(SCPKG_NTF_ACNT_INFO_UPD ntfAcntInfoUpd)
        {
            for (int i = 0; i < ntfAcntInfoUpd.iAcntUpdNum; i++)
            {
                this.OnUpdate(ref ntfAcntInfoUpd.astAcntUpdInfo[i]);
            }
            Singleton<EventRouter>.instance.BroadCastEvent("MasterAttributesChanged");
        }

        public void OnUpdate(ref SCDT_NTF_ACNT_INFO_UPD ntfAcntInfoUpd)
        {
            switch (ntfAcntInfoUpd.bUpdType)
            {
                case 1:
                    this.SetLevel(this.UInt32ChgAdjust(this.Level, ntfAcntInfoUpd.iUpdValChg), CS_ACNT_UPDATE_FROMTYPE.CS_ACNT_UPDATE_FROMTYPE_NULL);
                    break;

                case 2:
                    this.m_exp = this.UInt32ChgAdjust(this.m_exp, ntfAcntInfoUpd.iUpdValChg);
                    break;

                case 3:
                    this.m_maxActionPoint = this.UInt32ChgAdjust(this.m_maxActionPoint, ntfAcntInfoUpd.iUpdValChg);
                    break;

                case 4:
                    this.m_curActionPoint = this.UInt32ChgAdjust(this.m_curActionPoint, ntfAcntInfoUpd.iUpdValChg);
                    break;

                case 6:
                    this.m_diamond = this.UInt32ChgAdjust(this.m_diamond, ntfAcntInfoUpd.iUpdValChg);
                    break;

                case 8:
                    this.m_skillPoint = this.UInt32ChgAdjust(this.m_skillPoint, ntfAcntInfoUpd.iUpdValChg);
                    this.SetSkillPoint(this.m_skillPoint, true, true);
                    break;

                case 10:
                    this.SetPvpLevel(this.UInt32ChgAdjust(this.m_pvpLevel, ntfAcntInfoUpd.iUpdValChg));
                    break;

                case 11:
                    this.m_pvpExp = this.UInt32ChgAdjust(this.m_pvpExp, ntfAcntInfoUpd.iUpdValChg);
                    break;

                case 12:
                    this.m_goldCoin = this.UInt32ChgAdjust(this.m_goldCoin, ntfAcntInfoUpd.iUpdValChg);
                    break;

                case 13:
                    this.m_burningCoin = this.UInt32ChgAdjust(this.m_burningCoin, ntfAcntInfoUpd.iUpdValChg);
                    break;

                case 14:
                    this.m_arenaCoin = this.UInt32ChgAdjust(this.m_arenaCoin, ntfAcntInfoUpd.iUpdValChg);
                    break;

                case 15:
                    this.m_expPool = this.UInt32ChgAdjust(this.m_expPool, ntfAcntInfoUpd.iUpdValChg);
                    if (ntfAcntInfoUpd.bFromType == 1)
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips(string.Format("经验池增加{0}", ntfAcntInfoUpd.iUpdValChg), false, 1.5f, null, new object[0]);
                    }
                    break;

                case 0x10:
                    this.m_skinCoin = this.UInt32ChgAdjust(this.m_skinCoin, ntfAcntInfoUpd.iUpdValChg);
                    break;

                case 0x11:
                    this.m_symbolCoin = this.UInt32ChgAdjust(this.m_symbolCoin, ntfAcntInfoUpd.iUpdValChg);
                    Singleton<EventRouter>.instance.BroadCastEvent("MasterSymbolCoinChanged");
                    break;

                case 0x12:
                {
                    HuoyueData data = Singleton<CTaskSys>.instance.model.huoyue_data;
                    data.day_curNum = data.week_curNum = this.UInt32ChgAdjust(data.day_curNum, ntfAcntInfoUpd.iUpdValChg);
                    Singleton<EventRouter>.instance.BroadCastEvent("TASK_HUOYUEDU_Change");
                    break;
                }
                case 0x13:
                    this.GeiLiDuiYou = (int) this.UInt32ChgAdjust((uint) this.GeiLiDuiYou, ntfAcntInfoUpd.iUpdValChg);
                    break;

                case 20:
                    this.KeJingDuiShou = (int) this.UInt32ChgAdjust((uint) this.KeJingDuiShou, ntfAcntInfoUpd.iUpdValChg);
                    break;

                case 0x15:
                    this.m_JiFen = this.UInt32ChgAdjust(this.m_JiFen, ntfAcntInfoUpd.iUpdValChg);
                    break;
            }
        }

        [MessageHandler(0x1069)]
        public static void OnVipInfoRsp(CSPkg msg)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.SetVipFlags(msg.stPkgData.stQQVIPInfoRsp.dwVIPFlag);
            }
        }

        public void OnWearHeroSkin(uint heroId, uint skinId)
        {
            if (this.heroDic.ContainsKey(heroId))
            {
                this.heroDic[heroId].OnHeroSkinWear(skinId);
            }
            if (this.IsFreeHero(heroId))
            {
                this.SetFreeHeroWearSkinId(heroId, skinId);
            }
        }

        [MessageHandler(0x1457)]
        public static void ReciveCreditScoreInfo(CSPkg msg)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.creditScore = msg.stPkgData.stNtfAcntCreditValue.dwCreditValue;
            }
        }

        public void ReqSetInBattleNewbieBit(uint id, bool bIsLastAge)
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            if (Singleton<BattleLogic>.GetInstance().isRuning && ((curLvelContext != null) && curLvelContext.IsGameTypeGuide()))
            {
                stInBattleLevelBits bits;
                if (this.inBattleNewbieBits.TryGetValue(curLvelContext.m_mapID, out bits))
                {
                    if (bits.bReportFinished > 0)
                    {
                        return;
                    }
                    if (bits.finishedDetail.Contains(id))
                    {
                        return;
                    }
                }
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x10d5);
                msg.stPkgData.stDyeInBattleNewbieBitReq.iLevelID = curLvelContext.m_mapID;
                msg.stPkgData.stDyeInBattleNewbieBitReq.dwFinishedAgeID = id;
                msg.stPkgData.stDyeInBattleNewbieBitReq.bIsLastAge = !bIsLastAge ? ((byte) 0) : ((byte) 1);
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                this.SetInBattleNewbieBit(curLvelContext.m_mapID, id, bIsLastAge);
            }
        }

        public void SetAttributes(SCPKG_CMD_GAMELOGINRSP rsp)
        {
            this.playerUllUID = rsp.ullGameAcntUid;
            this.m_firstLoginTime = rsp.dwFirstLoginTime;
            this.m_firstLoginZeroDay = Utility.GetZeroBaseSecond((long) this.m_firstLoginTime);
            this.SetLevel(rsp.dwLevel, CS_ACNT_UPDATE_FROMTYPE.CS_ACNT_UPDATE_FROMTYPE_NULL);
            this.SetPvpLevel(rsp.dwPvpLevel);
            this.m_exp = rsp.dwExp;
            this.m_pvpExp = rsp.dwPvpExp;
            this.m_dianquan = 0L;
            this.m_goldCoin = rsp.stCoinList.CoinCnt[1];
            this.m_burningCoin = rsp.stCoinList.CoinCnt[2];
            this.m_arenaCoin = rsp.stCoinList.CoinCnt[3];
            this.SkinCoin = rsp.stCoinList.CoinCnt[4];
            this.SymbolCoin = rsp.stCoinList.CoinCnt[5];
            this.m_diamond = rsp.stCoinList.CoinCnt[6];
            this.m_JiFen = rsp.stCoinList.CoinCnt[7];
            this.m_maxActionPoint = rsp.dwMaxActionPoint;
            this.m_curActionPoint = rsp.dwCurActionPoint;
            this.m_titleId = (CrypticInt32) rsp.dwTitleId;
            this.m_headId = (CrypticInt32) rsp.dwHeadId;
            this.m_Name = StringHelper.UTF8BytesToString(ref rsp.szName);
            this.m_HeadUrl = Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(ref rsp.szHeadUrl);
            this.m_personSign = StringHelper.UTF8BytesToString(ref rsp.szSignatureInfo);
            this.m_privilegeType = (COM_PRIVILEGE_TYPE) rsp.bPrivilege;
            this.m_gameDifficult = rsp.bAcntNewbieType;
            this.m_gender = (COM_SNSGENDER) rsp.bGender;
            this.getFriendCoinCnt = rsp.bGetCoinNums;
            this.GeiLiDuiYou = (int) rsp.stLikeNum.dwTeammateNum;
            this.KeJingDuiShou = (int) rsp.stLikeNum.dwOpponentNum;
            this.snsSwitchBits = rsp.dwRefuseFriendBits;
            Debug.Log("--- LBS， get server snsSwitchBits:" + this.snsSwitchBits);
            MonoSingleton<GPSSys>.instance.Clear();
            UT.CheckGPS();
            this.creditScore = rsp.dwCreditValue;
            this.ExpPool = rsp.dwHeroPoolExp;
            Singleton<CChatController>.GetInstance().model.sysData.lastTimeStamp = rsp.dwServerCurTimeSec;
            this.setShopBuyRcd(ref rsp.stShopBuyRcd);
            this.SetGlobalRefreshTimer(rsp.dwServerCurTimeSec, false);
            this.SetPvELevelInfo(ref rsp.stPveProgress, rsp.dwServerCurTimeSec);
            this.m_skillPoint = rsp.dwSkillPoint;
            if ((rsp.dwSPUpdTimeSec * 0x3e8) > 0xffffffffL)
            {
                this.m_updateTimeMSec = uint.MaxValue;
            }
            else
            {
                this.m_updateTimeMSec = rsp.dwSPUpdTimeSec * 0x3e8;
            }
            this.m_maxSkillPt = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x1a).dwConfValue;
            this.m_skillPtRefreshSec = (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x18).dwConfValue;
            this.SetGuildData(ref rsp);
            this.SetExtraCoinAndExp(rsp.stPropMultiple);
            this.stMostUsedHero = rsp.stMostUsedHero;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.SetVipFlags(rsp.dwQQVipInfo);
            }
            DateTime time = Utility.ToUtcTime2Local((long) rsp.dwLastLoginTime);
            if (Utility.ToUtcTime2Local((long) rsp.dwServerCurTimeSec).Date == time.Date)
            {
                this.bFirstLoginToday = false;
            }
            else
            {
                this.bFirstLoginToday = true;
            }
            this.SetFirstWinRemainingTime(rsp.dwNextFirstWinSec);
            this.SetFirstWinLevelLimit(rsp.dwFirstWinPvpLvl);
            uint num = Utility.GetNewDayDeltaSec(GetCurrentUTCTime()) * 0x3e8;
            CTimer timer = Singleton<CTimerManager>.instance.GetTimer(this.m_newDayTimer);
            if (timer == null)
            {
                this.m_newDayTimer = Singleton<CTimerManager>.instance.AddTimer((int) num, 1, new CTimer.OnTimeUpHandler(this.OnNewDayNtf));
            }
            else
            {
                timer.ResetTotalTime((int) num);
                timer.Reset();
                timer.Resume();
            }
            this.m_licenseInfo.SetSvrLicenseData(rsp.stLicense);
            Singleton<CTaskSys>.instance.SetCardExpireTime(RES_PROP_VALFUNC_TYPE.RES_PROP_VALFUNC_MONTH_CARD, rsp.stMonthWeekCardInfo.dwMonthExpireTimeStamp);
            Singleton<CTaskSys>.instance.SetCardExpireTime(RES_PROP_VALFUNC_TYPE.RES_PROP_VALFUNC_WEEK_CARD, rsp.stMonthWeekCardInfo.dwWeekExpireTimeStamp);
            Singleton<CTaskSys>.instance.InitReport();
        }

        public void SetClientBits(int inIndex, bool bOpen, bool bSync = false)
        {
            if (this.IsClientBitsSet(inIndex) != bOpen)
            {
                this._clientBits.Set(inIndex, bOpen);
                this._clientBitsChanged = true;
            }
            if (bSync)
            {
                this.SyncClientBitsToSvr();
            }
        }

        public void SetCoinGetCntData(SCPKG_COINGETPATH_RSP coinData)
        {
            if (coinData != null)
            {
                for (int i = 0; i < coinData.dwPathNum; i++)
                {
                    if ((coinData.astPathCoin[i].dwPath >= 0) && (coinData.astPathCoin[i].dwPath < 3))
                    {
                        this.m_coinGetInfoDaily[coinData.astPathCoin[i].dwPath].GetCntDaily = coinData.astPathCoin[i].dwCoin;
                    }
                }
            }
        }

        public void SetCoinGetLimitDailyCnt()
        {
            ResGlobalInfo info = new ResGlobalInfo();
            if (GameDataMgr.svr2CltCfgDict.TryGetValue(1, out info))
            {
                this.m_coinGetInfoDaily[0].LimitCntDaily = info.dwConfValue;
            }
            if (GameDataMgr.svr2CltCfgDict.TryGetValue(2, out info))
            {
                this.m_coinGetInfoDaily[1].LimitCntDaily = info.dwConfValue;
            }
            uint dwConfValue = 0;
            if (GameDataMgr.svr2CltCfgDict.TryGetValue(3, out info))
            {
                dwConfValue = info.dwConfValue;
            }
            if (GameDataMgr.svr2CltCfgDict.TryGetValue(4, out info))
            {
                this.m_coinGetInfoDaily[2].LimitCntDaily = dwConfValue * info.dwConfValue;
            }
        }

        public void SetExtraCoinAndExp(COMDT_PROP_MULTIPLE data)
        {
            if (data != null)
            {
                this._extraTimeCoin = 0;
                this._extraWinCoin = 0;
                this._extraTimeExp = 0;
                this._extraWinExp = 0;
                this._extraWinCoinValue = 0;
                this._extraWinExpValue = 0;
                this._extraCoinHours = 0;
                this._extraExpHours = 0;
                for (int i = 0; i < data.wCnt; i++)
                {
                    COMDT_PROP_MULTIPLE_INFO comdt_prop_multiple_info = data.astMultipleInfo[i];
                    switch (comdt_prop_multiple_info.bMultipleType)
                    {
                        case 2:
                            if (comdt_prop_multiple_info.bTimeType == 1)
                            {
                                this._extraTimeExp = comdt_prop_multiple_info.dwRatio;
                                this._extraExpHours = comdt_prop_multiple_info.dwTimeValue;
                            }
                            else if (comdt_prop_multiple_info.bTimeType == 2)
                            {
                                this._extraWinExp = comdt_prop_multiple_info.dwRatio;
                                this._extraWinExpValue = comdt_prop_multiple_info.dwTimeValue;
                            }
                            break;

                        case 3:
                            if (comdt_prop_multiple_info.bTimeType == 1)
                            {
                                this._extraTimeCoin = comdt_prop_multiple_info.dwRatio;
                                this._extraCoinHours = comdt_prop_multiple_info.dwTimeValue;
                            }
                            else if (comdt_prop_multiple_info.bTimeType == 2)
                            {
                                this._extraWinCoin = comdt_prop_multiple_info.dwRatio;
                                this._extraWinCoinValue = comdt_prop_multiple_info.dwTimeValue;
                            }
                            break;
                    }
                }
                this.UpdateCoinAndExpValidTime();
            }
        }

        public void SetFirstWinLevelLimit(uint level)
        {
            this._firstWinLv = Math.Max(this._firstWinLv, level);
        }

        public void SetFirstWinRemainingTime(uint time)
        {
            this._nextFirstWinAvailableTime = time;
        }

        public void SetFreeHeroInfo(COMDT_FREEHERO stFreeHero)
        {
            uint freeHeroExpireTime = this.freeHeroExpireTime;
            this.freeHeroExpireTime = stFreeHero.dwDeadline;
            this.freeHeroList.Clear();
            for (int i = 0; i < stFreeHero.stFreeHeroList.wFreeCnt; i++)
            {
                if (CSysDynamicBlock.bLobbyEntryBlocked)
                {
                    ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(stFreeHero.stFreeHeroList.astFreeHeroDetail[i].dwFreeHeroID);
                    if ((dataByKey != null) && (dataByKey.bIOSHide == 1))
                    {
                        continue;
                    }
                }
                this.freeHeroList.Add(stFreeHero.stFreeHeroList.astFreeHeroDetail[i]);
            }
            if ((this.freeHeroExpireTime != 0) && (freeHeroExpireTime != this.freeHeroExpireTime))
            {
                if (this.GetFreeHeroTimer != 0)
                {
                    Singleton<CTimerManager>.GetInstance().RemoveTimer(this.GetFreeHeroTimer);
                }
                int num3 = (int) ((this.freeHeroExpireTime - GetCurrentUTCTime()) + ((ulong) 15L));
                this.GetFreeHeroTimer = Singleton<CTimerManager>.GetInstance().AddTimer(num3 * 0x3e8, 1, new CTimer.OnTimeUpHandler(Singleton<CPvPHeroShop>.GetInstance().GetFreeHero));
            }
        }

        public void SetFreeHeroSymbol(COMDT_FREEHERO_INACNT stFreeHeroSymbol)
        {
            this.freeHeroSymbolList.Clear();
            for (int i = 0; i < stFreeHeroSymbol.bHeroCnt; i++)
            {
                this.freeHeroSymbolList.Add(stFreeHeroSymbol.astHeroInfo[i]);
            }
        }

        public void SetFreeHeroSymbolId(uint heroId, byte symbolId)
        {
            for (int i = 0; i < this.freeHeroSymbolList.Count; i++)
            {
                if (this.freeHeroSymbolList[i].dwHeroID == heroId)
                {
                    this.freeHeroSymbolList[i].bSymbolPageWear = symbolId;
                    return;
                }
            }
            COMDT_FREEHERO_INFO item = new COMDT_FREEHERO_INFO {
                dwHeroID = heroId,
                bSymbolPageWear = symbolId
            };
            this.freeHeroSymbolList.Add(item);
        }

        public void SetFreeHeroWearSkinId(uint heroId, uint skinId)
        {
            if (this.IsFreeHero(heroId))
            {
                for (int i = 0; i < this.freeHeroSymbolList.Count; i++)
                {
                    if (this.freeHeroSymbolList[i].dwHeroID == heroId)
                    {
                        this.freeHeroSymbolList[i].wSkinID = (ushort) skinId;
                        return;
                    }
                }
                COMDT_FREEHERO_INFO item = new COMDT_FREEHERO_INFO {
                    dwHeroID = heroId,
                    wSkinID = (ushort) skinId
                };
                this.freeHeroSymbolList.Add(item);
            }
        }

        public void SetGlobalRefreshTimer(uint serverTime, bool force = false)
        {
            if (force || (m_globalRefreshTimerSeq == -1))
            {
                Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref m_globalRefreshTimerSeq);
                DateTime time = Utility.ToUtcTime2Local((long) serverTime);
                int num = 0;
                uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 10).dwConfValue;
                num = Utility.Hours2Second((int) (dwConfValue / 100)) + Utility.Minutes2Seconds((int) (dwConfValue % 100));
                int num3 = ((num - Convert.ToInt32(time.TimeOfDay.TotalSeconds)) <= 0) ? ((num - Convert.ToInt32(time.TimeOfDay.TotalSeconds)) + 0x15180) : (num - Convert.ToInt32(time.TimeOfDay.TotalSeconds));
                m_globalRefreshTimerSeq = Singleton<CTimerManager>.GetInstance().AddTimer(num3 * 0x3e8, 1, new CTimer.OnTimeUpHandler(this.OnGlobalRefreshTimerEnd));
            }
        }

        public void SetGuidedStateSet(int inIndex, bool bOpen)
        {
            this.GuidedStateBits.Set(inIndex, bOpen);
            if (bOpen)
            {
                NewbieGuideSkipConditionType type = NewbieGuideCheckSkipConditionUtil.TranslateToSkipCond(inIndex);
                if (type != NewbieGuideSkipConditionType.Invalid)
                {
                    MonoSingleton<NewbieGuideManager>.GetInstance().CheckSkipCondition(type, new uint[0]);
                }
            }
        }

        public void SetGuildData(ref SCPKG_CMD_GAMELOGINRSP rsp)
        {
            this.m_baseGuildInfo.guildState = (COM_PLAYER_GUILD_STATE) rsp.stGuildBaseInfo.bGuildState;
            this.m_baseGuildInfo.uulUid = rsp.stGuildBaseInfo.ullGuildID;
            this.m_extGuildInfo.bApplyJoinGuildNum = rsp.stGuildExtInfo.bApplyJoinGuildNum;
            this.m_extGuildInfo.dwClearApplyJoinGuildNumTime = rsp.stGuildExtInfo.dwClearApplyJoinGuildNumTime;
            this.m_extGuildInfo.dwLastCreateGuildTime = rsp.stGuildExtInfo.dwLastCreateGuildTime;
            this.m_extGuildInfo.dwLastQuitGuildTime = rsp.stGuildExtInfo.dwLastQuitGuildTime;
            this.m_extGuildInfo.bSendGuildMailCnt = rsp.stGuildExtInfo.bSendGuildMailCnt;
        }

        public void SetHeroExp(uint id, int level, int exp)
        {
            CHeroInfo info;
            if (this.GetHeroInfo(id, out info, false))
            {
                info.mActorValue.actorLvl = level;
                info.mActorValue.actorExp = exp;
            }
        }

        public void SetHeroInfo(SCPKG_ACNTHEROINFO_NTY ntfHeroInfo)
        {
            Debug.Log("SetHeroInfo..." + ntfHeroInfo.stHeroInfo.dwHeroNum);
            this.heroDic.Clear();
            for (int i = 0; i < ntfHeroInfo.stHeroInfo.dwHeroNum; i++)
            {
                this.InitHero(ntfHeroInfo.stHeroInfo.astHeroInfoList[i]);
            }
            this.InitBattleHeroList(ntfHeroInfo.stBattleListInfo);
            this.InitAllHeroSkin(ntfHeroInfo.stSkinInfo);
            this.InitAllHeroExperienceSkin(ntfHeroInfo.stLimitSkinInfo);
            this.m_initHeroId = ntfHeroInfo.stHeroCtrlInfo.dwInitHeroID;
            this.m_arenaDefHeroList.Clear();
            if (ntfHeroInfo.stHeroCtrlInfo.stBattleListOfArena.wHeroCnt > 0)
            {
                this.m_arenaDefHeroList.AddRange(ntfHeroInfo.stHeroCtrlInfo.stBattleListOfArena.BattleHeroList);
            }
            this.InitializeCustomRecommendEquip(ntfHeroInfo.stSelfDefineEquipInfo);
            this.m_nHeroSkinCount = this.GetHeroSkinCount(false);
        }

        public void SetHeroSelSkillID(uint heroID, uint selSkillID)
        {
            CHeroInfo heroInfo = this.GetHeroInfo(heroID, true);
            if (heroInfo != null)
            {
                heroInfo.skillInfo.SelSkillID = selSkillID;
            }
            else if (this.IsFreeHero(heroID))
            {
                COMDT_FREEHERO_INFO freeHeroSymbol = this.GetFreeHeroSymbol(heroID);
                if (freeHeroSymbol != null)
                {
                    freeHeroSymbol.dwSkillID = selSkillID;
                }
                else
                {
                    freeHeroSymbol = new COMDT_FREEHERO_INFO {
                        dwHeroID = heroID,
                        dwSkillID = selSkillID
                    };
                    this.freeHeroSymbolList.Add(freeHeroSymbol);
                }
            }
        }

        public void SetHeroSkinData(COMDT_HERO_SKIN heroSkin)
        {
            if (!this.heroSkinDic.ContainsKey(heroSkin.dwHeroID))
            {
                this.heroSkinDic.Add(heroSkin.dwHeroID, heroSkin.ullSkinBits);
            }
            else
            {
                this.heroSkinDic[heroSkin.dwHeroID] = heroSkin.ullSkinBits;
            }
        }

        public void SetHeroSymbolPageIdx(uint heroId, int pageIdx)
        {
            CHeroInfo info;
            if (this.GetHeroInfo(heroId, out info, true))
            {
                info.OnSymbolPageChange(pageIdx);
            }
            else
            {
                this.SetFreeHeroSymbolId(heroId, (byte) pageIdx);
            }
        }

        private void SetInBattleNewbieBit(int iLevelId, uint ageId, bool bIsLastAge)
        {
            stInBattleLevelBits bits;
            if (!this.inBattleNewbieBits.TryGetValue(iLevelId, out bits))
            {
                bits = new stInBattleLevelBits {
                    bReportFinished = !bIsLastAge ? ((byte) 0) : ((byte) 1),
                    finishedDetail = new List<uint>()
                };
                bits.finishedDetail.Add(ageId);
                this.inBattleNewbieBits.Add(iLevelId, bits);
            }
            else
            {
                bits.bReportFinished = !bIsLastAge ? ((byte) 0) : ((byte) 1);
                bits.finishedDetail.Add(ageId);
            }
        }

        private void SetLevel(uint newLevel, CS_ACNT_UPDATE_FROMTYPE lvlUpType)
        {
            uint level = this.m_level;
            this.m_level = newLevel;
            ResAcntExpInfo dataByKey = GameDataMgr.acntExpDatabin.GetDataByKey(this.m_level);
            this.m_needExp = dataByKey.dwNeedExp;
            if (((level > 0) && (this.m_level > level)) && (Singleton<GameStateCtrl>.GetInstance().GetCurrentState() is LobbyState))
            {
                if (lvlUpType == CS_ACNT_UPDATE_FROMTYPE.CS_ACNT_UPDATE_FROMTYPE_SWEEP)
                {
                    CAdventureView.SetMopupLevelUp(level, this.m_level);
                }
                else
                {
                    CUIEvent uiEvent = new CUIEvent {
                        m_eventID = enUIEventID.Settle_OpenLvlUp
                    };
                    uiEvent.m_eventParams.tag = (int) level;
                    uiEvent.m_eventParams.tag2 = (int) this.m_level;
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
                }
            }
        }

        public void SetNewbieAchieve(int inIndex, bool bOpen, bool bSync = false)
        {
            if (inIndex < this.NewbieAchieveBits.Count)
            {
                if (this.IsNewbieAchieveSet(inIndex) != bOpen)
                {
                    this.NewbieAchieveBits.Set(inIndex, bOpen);
                    this.bNewbieAchieveChanged = true;
                }
                if (bSync)
                {
                    this.SyncNewbieAchieveToSvr(false);
                }
            }
        }

        public void SetNobeInfo(SCPKG_GAME_VIP_NTF info)
        {
            this.m_vipInfo = info;
        }

        public void SetPvELevelInfo(ref COMDT_ACNT_LEVEL_COMPLETE_DETAIL detail, uint ServerTime)
        {
            CAdventureSys.LEVEL_DIFFICULT_OPENED = detail.bLastOpenDiffType;
            CAdventureSys.CHAPTER_NUM = Math.Min(detail.bChapterNum, GameDataMgr.chapterInfoDatabin.Count());
            for (int i = 0; i < CAdventureSys.LEVEL_DIFFICULT_OPENED; i++)
            {
                this.pveLevelDetail[i] = new PVE_ADV_COMPLETE_INFO();
            }
            for (int j = 0; j < CAdventureSys.CHAPTER_NUM; j++)
            {
                COMDT_CHAPTER_COMPLETE_INFO comdt_chapter_complete_info = detail.astChapterDetail[j];
                for (int k = 0; k < CAdventureSys.LEVEL_DIFFICULT_OPENED; k++)
                {
                    PVE_CHAPTER_COMPLETE_INFO pve_chapter_complete_info = this.pveLevelDetail[k].ChapterDetailList[j];
                    pve_chapter_complete_info.bLevelNum = comdt_chapter_complete_info.bLevelNum;
                    pve_chapter_complete_info.bIsGetBonus = comdt_chapter_complete_info.astDiffDetail[k].bGetBonus;
                }
                for (byte m = 0; m < comdt_chapter_complete_info.bLevelNum; m = (byte) (m + 1))
                {
                    for (int n = 0; n < CAdventureSys.LEVEL_DIFFICULT_OPENED; n++)
                    {
                        PVE_LEVEL_COMPLETE_INFO pve_level_complete_info = this.pveLevelDetail[n].ChapterDetailList[j].LevelDetailList[m];
                        COMDT_LEVEL_COMPLETE_INFO comdt_level_complete_info = comdt_chapter_complete_info.astLevelDetail[m];
                        pve_level_complete_info.iLevelID = comdt_level_complete_info.iLevelID;
                        pve_level_complete_info.levelStatus = comdt_level_complete_info.astDiffDetail[n].bLevelStatus;
                        pve_level_complete_info.bStarBits = comdt_level_complete_info.astDiffDetail[n].bStarBits;
                    }
                }
            }
            Singleton<EventRouter>.instance.BroadCastEvent(EventID.PVE_LEVEL_DETAIL_CHANGED);
        }

        public void SetPvpLevel(uint value)
        {
            this.m_pvpLevel = value;
            Singleton<EventRouter>.instance.BroadCastEvent("Chat_PlayerLevel_Set");
        }

        public static void SetServerTime(int serverTime)
        {
            s_sysTime = serverTime;
            s_upToLoginSec = Time.realtimeSinceStartup;
        }

        private void setShopBuyRcd(ref CSDT_ACNT_SHOPBUY_INFO rcd)
        {
            SetServerTime(rcd.iGameSysTime);
            this.m_freeDrawInfo = new stShopBuyDrawInfo[rcd.astShopDrawInfo.Length];
            for (int i = 0; i < this.m_freeDrawInfo.Length; i++)
            {
                this.m_freeDrawInfo[i].dwLeftFreeDrawCD = s_sysTime + rcd.astShopDrawInfo[i].iLeftFreeSec;
                this.m_freeDrawInfo[i].dwLeftFreeDrawCnt = rcd.astShopDrawInfo[i].iLeftFreeCnt;
            }
            this.m_coinDrawIndex = rcd.bCurCoinDrawStep;
            this.m_DiamondOpenBoxCnt = rcd.dwOpenBoxByCouponsCnt;
            uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x56).dwConfValue;
            if ((dwConfValue - rcd.dwDirectBuyItemCnt) >= 0)
            {
                this.m_MaterialDirectBuyLimit = (byte) (((byte) dwConfValue) - rcd.dwDirectBuyItemCnt);
            }
            else
            {
                this.m_MaterialDirectBuyLimit = 0;
            }
            if (rcd.stSymbolDrawCommon != null)
            {
                if (rcd.stSymbolDrawCommon.bSubDrawCnt > 0)
                {
                    for (byte j = 0; j < rcd.stSymbolDrawCommon.bSubDrawCnt; j = (byte) (j + 1))
                    {
                        COMDT_DRAWCNT_SUBINFO comdt_drawcnt_subinfo = rcd.stSymbolDrawCommon.astSubDrawInfo[j];
                        if (comdt_drawcnt_subinfo != null)
                        {
                            switch (comdt_drawcnt_subinfo.iSubType)
                            {
                                case 2:
                                    CMallSymbolGiftController.SymbolGiftCommonDrawedCntInfo[comdt_drawcnt_subinfo.iSubType] = comdt_drawcnt_subinfo.iDrawCnt;
                                    break;

                                case 3:
                                    CMallSymbolGiftController.SymbolGiftCommonDrawedCntInfo[comdt_drawcnt_subinfo.iSubType] = comdt_drawcnt_subinfo.iDrawCnt;
                                    break;

                                case 4:
                                    CMallSymbolGiftController.SymbolGiftCommonDrawedCntInfo[comdt_drawcnt_subinfo.iSubType] = comdt_drawcnt_subinfo.iDrawCnt;
                                    break;
                            }
                        }
                    }
                }
                int iFreeDrawTotalCnt = rcd.stSymbolDrawCommon.iFreeDrawTotalCnt;
                if (iFreeDrawTotalCnt >= 0)
                {
                    CMallSymbolGiftController.SymbolGiftCommonDrawedCntInfo[1] = iFreeDrawTotalCnt;
                }
            }
            if (rcd.stSymbolDrawSenior != null)
            {
                if (rcd.stSymbolDrawSenior.bSubDrawCnt > 0)
                {
                    for (byte k = 0; k < rcd.stSymbolDrawSenior.bSubDrawCnt; k = (byte) (k + 1))
                    {
                        COMDT_DRAWCNT_SUBINFO comdt_drawcnt_subinfo2 = rcd.stSymbolDrawSenior.astSubDrawInfo[k];
                        if (comdt_drawcnt_subinfo2 != null)
                        {
                            switch (comdt_drawcnt_subinfo2.iSubType)
                            {
                                case 2:
                                    CMallSymbolGiftController.SymbolGiftSeniorDrawedCntInfo[comdt_drawcnt_subinfo2.iSubType] = comdt_drawcnt_subinfo2.iDrawCnt;
                                    break;

                                case 3:
                                    CMallSymbolGiftController.SymbolGiftSeniorDrawedCntInfo[comdt_drawcnt_subinfo2.iSubType] = comdt_drawcnt_subinfo2.iDrawCnt;
                                    break;

                                case 4:
                                    CMallSymbolGiftController.SymbolGiftSeniorDrawedCntInfo[comdt_drawcnt_subinfo2.iSubType] = comdt_drawcnt_subinfo2.iDrawCnt;
                                    break;
                            }
                        }
                    }
                }
                int num6 = rcd.stSymbolDrawSenior.iFreeDrawTotalCnt;
                if (num6 >= 0)
                {
                    CMallSymbolGiftController.SymbolGiftSeniorDrawedCntInfo[1] = num6;
                }
            }
        }

        public void SetSkillPoint(uint pt, bool bRefreshTime = false, bool bDispathEvent = false)
        {
            this.m_skillPoint = pt;
            if (bRefreshTime)
            {
                if ((this.m_skillPtRefreshSec * 0x3e8) > 0xffffffffL)
                {
                    this.m_updateTimeMSec = uint.MaxValue;
                }
                else
                {
                    this.m_updateTimeMSec = (uint) (this.m_skillPtRefreshSec * 0x3e8);
                }
            }
            if (bDispathEvent)
            {
                Singleton<EventRouter>.instance.BroadCastEvent<uint>("SkillPointChange", this.m_skillPoint);
            }
        }

        private void SetVipFlags(uint flags)
        {
            this.m_vipFlags = flags;
            this.m_vipFlagsValid = true;
            Singleton<EventRouter>.GetInstance().BroadCastEvent("VipInfoHadSet");
        }

        public void SyncClientBitsToSvr()
        {
            if (this._clientBitsChanged)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xfa3);
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 0x40; j++)
                    {
                        msg.stPkgData.stUpdNewClientBits.stClientBits.BitsDetail[i] += !this._clientBits[(0x40 * i) + j] ? ((ulong) 0L) : (((ulong) 1L) << j);
                    }
                }
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                this._clientBitsChanged = false;
            }
        }

        public void SyncNewbieAchieveToSvr(bool resetOldPlayerGuided = false)
        {
            if (this.bNewbieAchieveChanged || resetOldPlayerGuided)
            {
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xfa0);
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 0x40; j++)
                    {
                        if ((!resetOldPlayerGuided || (i != 0)) || (j != 0x11))
                        {
                            msg.stPkgData.stUpdateClientBitsNtf.stClientBits.BitsDetail[i] += !this.NewbieAchieveBits[(0x40 * i) + j] ? ((ulong) 0L) : (((ulong) 1L) << j);
                        }
                    }
                }
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                this.bNewbieAchieveChanged = false;
            }
        }

        public uint UInt32ChgAdjust(uint bs, int chg)
        {
            if (chg >= 0)
            {
                if ((bs + chg) >= 0xffffffffL)
                {
                    return uint.MaxValue;
                }
                return (bs + ((uint) chg));
            }
            uint num = (uint) Math.Abs(chg);
            if (bs <= num)
            {
                return 0;
            }
            return (bs - num);
        }

        public void UpdateCoinAndExpValidTime()
        {
            if ((this._extraTimeCoin > 0) && (this.GetExpireHours(this._extraCoinHours) == 0))
            {
                this._extraTimeCoin = 0;
            }
            if ((this._extraTimeExp > 0) && (this.GetExpireHours(this._extraExpHours) == 0))
            {
                this._extraTimeExp = 0;
            }
        }

        public void UpdateLogic(int delta)
        {
            this.m_updateTimeMSec = this.UInt32ChgAdjust(this.m_updateTimeMSec, -delta);
        }

        public uint AccountRegisterTime
        {
            get
            {
                return this.m_firstLoginTime;
            }
        }

        public long AccountRegisterTime_ZeroDay
        {
            get
            {
                return this.m_firstLoginZeroDay;
            }
        }

        public uint ArenaCoin
        {
            get
            {
                return this.m_arenaCoin;
            }
            set
            {
                this.m_arenaCoin = value;
            }
        }

        public uint BattlePower
        {
            get
            {
                ListView<CHeroInfo> view = new ListView<CHeroInfo>(this.heroDic.Count);
                DictionaryView<uint, CHeroInfo>.Enumerator enumerator = this.heroDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
                    view.Add(current.Value);
                }
                ushort count = (ushort) view.Count;
                uint bs = 0;
                for (ushort i = 0; i < count; i = (ushort) (i + 1))
                {
                    bs = this.UInt32ChgAdjust(bs, view[i].GetCombatEft());
                }
                return bs;
            }
        }

        public bool bCanRecvCoin
        {
            get
            {
                uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x11).dwConfValue;
                return (this.getFriendCoinCnt < dwConfValue);
            }
        }

        public bool bFirstLoginToday { get; private set; }

        public uint BurningCoin
        {
            get
            {
                return this.m_burningCoin;
            }
            set
            {
                this.m_burningCoin = value;
            }
        }

        public uint CurActionPoint
        {
            get
            {
                return this.m_curActionPoint;
            }
            set
            {
                this.m_curActionPoint = value;
            }
        }

        public uint dailyPvpCnt { get; set; }

        public uint Diamond
        {
            get
            {
                return this.m_diamond;
            }
            set
            {
                this.m_diamond = value;
                Singleton<EventRouter>.instance.BroadCastEvent("MasterAttributesChanged");
                Singleton<EventRouter>.instance.BroadCastEvent("MasterCurrencyChanged");
            }
        }

        public ulong DianQuan
        {
            get
            {
                return this.m_dianquan;
            }
            set
            {
                this.m_dianquan = value;
                Singleton<EventRouter>.instance.BroadCastEvent("MasterAttributesChanged");
                Singleton<EventRouter>.instance.BroadCastEvent("MasterCurrencyChanged");
            }
        }

        public uint Exp
        {
            get
            {
                return this.m_exp;
            }
            set
            {
                this.m_exp = value;
                while (this.m_exp >= this.m_needExp)
                {
                    this.m_exp -= this.m_needExp;
                    this.SetLevel(this.UInt32ChgAdjust(this.Level, 1), CS_ACNT_UPDATE_FROMTYPE.CS_ACNT_UPDATE_FROMTYPE_NULL);
                }
            }
        }

        public uint ExpPool
        {
            get
            {
                return this.m_expPool;
            }
            set
            {
                this.m_expPool = value;
            }
        }

        public COM_ACNT_NEWBIE_TYPE GameDifficult
        {
            get
            {
                return (COM_ACNT_NEWBIE_TYPE) this.m_gameDifficult;
            }
        }

        public uint GoldCoin
        {
            get
            {
                return this.m_goldCoin;
            }
            set
            {
                this.m_goldCoin = value;
                Singleton<EventRouter>.instance.BroadCastEvent("MasterCurrencyChanged");
            }
        }

        public int HeadId
        {
            get
            {
                return (int) this.m_headId;
            }
        }

        public string HeadUrl
        {
            get
            {
                return this.m_HeadUrl;
            }
            set
            {
                this.m_HeadUrl = value;
            }
        }

        public uint JiFen
        {
            get
            {
                return this.m_JiFen;
            }
        }

        public uint Level
        {
            get
            {
                return this.m_level;
            }
        }

        public byte MaterialDirectBuyLimit
        {
            get
            {
                return this.m_MaterialDirectBuyLimit;
            }
            set
            {
                this.m_MaterialDirectBuyLimit = (value >= 0) ? value : ((byte) 0);
            }
        }

        public uint MaxActionPoint
        {
            get
            {
                return this.m_maxActionPoint;
            }
        }

        public COMDT_MOST_USED_HERO_DETAIL MostUsedHeroDetail
        {
            get
            {
                return this.stMostUsedHero;
            }
        }

        public string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }

        public uint NeedExp
        {
            get
            {
                return this.m_needExp;
            }
        }

        public string PersonSign
        {
            get
            {
                return this.m_personSign;
            }
            set
            {
                this.m_personSign = value;
            }
        }

        public ulong playerUllUID { get; set; }

        public uint PvpExp
        {
            get
            {
                return this.m_pvpExp;
            }
            set
            {
                this.m_pvpExp = value;
            }
        }

        public uint PvpLevel
        {
            get
            {
                return this.m_pvpLevel;
            }
        }

        public uint PvpNeedExp
        {
            get
            {
                ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint) ((byte) this.m_pvpLevel));
                if (dataByKey != null)
                {
                    return dataByKey.dwNeedExp;
                }
                return 0;
            }
        }

        public uint SkinCoin
        {
            get
            {
                return this.m_skinCoin;
            }
            set
            {
                this.m_skinCoin = value;
            }
        }

        public uint SymbolCoin
        {
            get
            {
                return this.m_symbolCoin;
            }
            set
            {
                this.m_symbolCoin = value;
            }
        }

        public int TitleId
        {
            get
            {
                return (int) this.m_titleId;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct stCoinGetInfoDaily
        {
            public uint GetCntDaily;
            public uint LimitCntDaily;
        }
    }
}

