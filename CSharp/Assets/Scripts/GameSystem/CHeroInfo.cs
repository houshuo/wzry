namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;

    public class CHeroInfo
    {
        public ResHeroCfgInfo cfgInfo;
        public int m_awakeState;
        public uint m_awakeStepID;
        public uint m_experienceDeadLine;
        public bool m_isStepFinish;
        public uint m_masterHeroFightCnt;
        public uint m_masterHeroWinCnt;
        private static int m_maxProficiency;
        public uint m_Proficiency;
        public byte m_ProficiencyLV;
        public int m_selectPageIndex;
        public CSkinInfo m_skinInfo = new CSkinInfo();
        public byte[] m_talentBuyList;
        public PropertyHelper mActorValue = new PropertyHelper();
        public uint MaskBits;
        public ResHeroShop shopCfgInfo;
        public CSkillData skillInfo;

        public int GetCombatEft()
        {
            DebugHelper.Assert(this.mActorValue != null, "GetCombatEft mActorValue is null");
            int combatEftByStarLevel = 0;
            if (this.mActorValue != null)
            {
                combatEftByStarLevel = GetCombatEftByStarLevel(this.mActorValue.actorLvl, this.mActorValue.actorStar);
            }
            int combatEft = CSkinInfo.GetCombatEft(this.cfgInfo.dwCfgID, this.m_skinInfo.GetWearSkinId());
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "GetCombatEft master is null");
            int symbolPageEft = 0;
            if (masterRoleInfo != null)
            {
                symbolPageEft = masterRoleInfo.m_symbolInfo.GetSymbolPageEft(this.m_selectPageIndex);
            }
            return ((combatEftByStarLevel + combatEft) + symbolPageEft);
        }

        public static int GetCombatEftByStarLevel(int level, int star)
        {
            ResHeroLvlUpInfo dataByKey = GameDataMgr.heroLvlUpDatabin.GetDataByKey((uint) level);
            if (((dataByKey != null) && (star >= 1)) && (star <= 5))
            {
                return dataByKey.StarCombatEft[star - 1];
            }
            return 0;
        }

        public static int GetExperienceHeroOrSkinExtendDays(uint extendSeconds)
        {
            TimeSpan span = new TimeSpan((extendSeconds + 0xe10) * 0x989680L);
            return span.Days;
        }

        public static int GetExperienceHeroOrSkinValidDays(uint experienceDeadLine)
        {
            int num = ((int) experienceDeadLine) - CRoleInfo.GetCurrentUTCTime();
            TimeSpan span = new TimeSpan((num + 0xe10) * 0x989680L);
            return span.Days;
        }

        public static string GetFeatureStr(RES_HERO_JOB_FEATURE featureType)
        {
            string str = string.Empty;
            CTextManager instance = Singleton<CTextManager>.GetInstance();
            switch (featureType)
            {
                case RES_HERO_JOB_FEATURE.RES_JOB_FEATURE_DASH:
                    return instance.GetText("Hero_Job_Feature_Dash");

                case RES_HERO_JOB_FEATURE.RES_JOB_FEATURE_CONTROL:
                    return instance.GetText("Hero_Job_Feature_Control");

                case RES_HERO_JOB_FEATURE.RES_JOB_FEATURE_ACTIVE:
                    return instance.GetText("Hero_Job_Feature_Active");

                case RES_HERO_JOB_FEATURE.RES_JOB_FEATURE_SLAVE:
                    return instance.GetText("Hero_Job_Feature_Slave");

                case RES_HERO_JOB_FEATURE.RES_JOB_FEATURE_RECOVER:
                    return instance.GetText("Hero_Job_Feature_Recover");

                case RES_HERO_JOB_FEATURE.RES_JOB_FEATURE_HPSTEAL:
                    return instance.GetText("Hero_Job_Feature_HpSteal");

                case RES_HERO_JOB_FEATURE.RES_JOB_FEATURE_POKE:
                    return instance.GetText("Hero_Job_Feature_Poke");

                case RES_HERO_JOB_FEATURE.RES_JOB_FEATURE_BUFF:
                    return instance.GetText("Hero_Job_Feature_Buff");
            }
            return str;
        }

        public static uint GetHeroCost(uint heroId, RES_SHOPBUY_COINTYPE costType)
        {
            ResHeroShop shop = null;
            GameDataMgr.heroShopInfoDict.TryGetValue(heroId, out shop);
            uint num = 0;
            if (shop != null)
            {
                switch (costType)
                {
                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
                        return shop.dwBuyCoupons;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ENCHANTPT:
                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SKINCOIN:
                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN:
                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SYMBOLCOIN:
                        return num;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
                        return shop.dwBuyCoin;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
                        return shop.dwBuyBurnCoin;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
                        return shop.dwBuyArenaCoin;

                    case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND:
                        return shop.dwBuyDiamond;
                }
            }
            return num;
        }

        public static string GetHeroDesc(uint heroId)
        {
            return Utility.UTF8Convert(GameDataMgr.heroDatabin.GetDataByKey(heroId).szHeroDesc);
        }

        public static string GetHeroJob(uint heroId)
        {
            string str = string.Empty;
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
            CTextManager instance = Singleton<CTextManager>.GetInstance();
            if (dataByKey != null)
            {
                if (dataByKey.bMainJob > 0)
                {
                    str = str + GetHeroJobStr((RES_HERO_JOB) dataByKey.bMainJob);
                }
                if (dataByKey.bMinorJob > 0)
                {
                    str = string.Format("{0}/{1}", str, GetHeroJobStr((RES_HERO_JOB) dataByKey.bMinorJob));
                }
            }
            return str;
        }

        public static string GetHeroJobStr(RES_HERO_JOB jobType)
        {
            string str = string.Empty;
            CTextManager instance = Singleton<CTextManager>.GetInstance();
            switch (jobType)
            {
                case RES_HERO_JOB.RES_HEROJOB_TANK:
                    return instance.GetText("Hero_Job_Tank");

                case RES_HERO_JOB.RES_HEROJOB_SOLDIER:
                    return instance.GetText("Hero_Job_Soldier");

                case RES_HERO_JOB.RES_HEROJOB_ASSASSIN:
                    return instance.GetText("Hero_Job_Assassin");

                case RES_HERO_JOB.RES_HEROJOB_MASTER:
                    return instance.GetText("Hero_Job_Master");

                case RES_HERO_JOB.RES_HEROJOB_ARCHER:
                    return instance.GetText("Hero_Job_Archer");

                case RES_HERO_JOB.RES_HEROJOB_AID:
                    return instance.GetText("Hero_Job_Aid");
            }
            return str;
        }

        public static ResHeroProficiency GetHeroProficiency(int job, int level)
        {
            HashSet<object>.Enumerator enumerator = GameDataMgr.heroProficiencyDatabin.GetDataByKey((int) ((byte) level)).GetEnumerator();
            while (enumerator.MoveNext())
            {
                ResHeroProficiency current = (ResHeroProficiency) enumerator.Current;
                if (current.bJob == job)
                {
                    return current;
                }
            }
            return null;
        }

        public static string GetHeroTitle(uint heroId, uint skinId)
        {
            string str = string.Empty;
            if (skinId == 0)
            {
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
                if (dataByKey != null)
                {
                    str = StringHelper.UTF8BytesToString(ref dataByKey.szHeroTitle);
                }
                return str;
            }
            ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
            if (heroSkin != null)
            {
                str = StringHelper.UTF8BytesToString(ref heroSkin.szSkinName);
            }
            return str;
        }

        public static int GetInitCombatByHeroId(uint id)
        {
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(id);
            if (dataByKey == null)
            {
                return 0;
            }
            int combatEftByStarLevel = GetCombatEftByStarLevel(1, dataByKey.iInitialStar);
            int combatEft = CSkinInfo.GetCombatEft(id, 0);
            return (combatEftByStarLevel + combatEft);
        }

        public static string GetJobFeature(uint heroId)
        {
            string str = string.Empty;
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
            CTextManager instance = Singleton<CTextManager>.GetInstance();
            if (dataByKey != null)
            {
                string featureStr = string.Empty;
                for (int i = 0; i < dataByKey.JobFeature.Length; i++)
                {
                    featureStr = GetFeatureStr((RES_HERO_JOB_FEATURE) dataByKey.JobFeature[i]);
                    if (!string.IsNullOrEmpty(featureStr))
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            str = string.Format("{0}/{1}", str, featureStr);
                        }
                        else
                        {
                            str = featureStr;
                        }
                    }
                }
            }
            return str;
        }

        public static int GetMaxProficiency()
        {
            if (m_maxProficiency <= 0)
            {
                for (int i = 0; i < GameDataMgr.heroProficiencyDatabin.Count(); i++)
                {
                    HashSet<object>.Enumerator enumerator = GameDataMgr.heroProficiencyDatabin.GetDataByIndex(i).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        m_maxProficiency = Math.Max(m_maxProficiency, ((ResHeroProficiency) enumerator.Current).bLevel);
                    }
                }
            }
            return m_maxProficiency;
        }

        public void Init(ulong playerId, COMDT_HEROINFO svrInfo)
        {
            this.cfgInfo = GameDataMgr.heroDatabin.GetDataByKey(svrInfo.stCommonInfo.dwHeroID);
            GameDataMgr.heroShopInfoDict.TryGetValue(svrInfo.stCommonInfo.dwHeroID, out this.shopCfgInfo);
            this.m_selectPageIndex = svrInfo.stCommonInfo.bSymbolPageWear;
            if (this.mActorValue == null)
            {
                this.mActorValue = new PropertyHelper();
            }
            this.mActorValue.Init(svrInfo);
            if (this.skillInfo == null)
            {
                this.skillInfo = new CSkillData();
            }
            this.skillInfo.InitSkillData(this.cfgInfo, svrInfo.stCommonInfo.stSkill);
            this.m_Proficiency = svrInfo.stCommonInfo.stProficiency.dwProficiency;
            this.m_ProficiencyLV = svrInfo.stCommonInfo.stProficiency.bLv;
            this.MaskBits = svrInfo.stCommonInfo.dwMaskBits;
            this.m_skinInfo.Init(svrInfo.stCommonInfo.wSkinID);
            this.m_talentBuyList = new byte[svrInfo.stCommonInfo.stTalent.astTalentInfo.Length];
            for (int i = 0; i < svrInfo.stCommonInfo.stTalent.astTalentInfo.Length; i++)
            {
                this.m_talentBuyList[i] = svrInfo.stCommonInfo.stTalent.astTalentInfo[i].bIsBuyed;
            }
            this.m_awakeState = svrInfo.stCommonInfo.stTalent.bWakeState;
            this.m_awakeStepID = svrInfo.stCommonInfo.stTalent.stWakeStep.dwStepID;
            this.m_isStepFinish = svrInfo.stCommonInfo.stTalent.stWakeStep.bIsFinish == 1;
            this.m_experienceDeadLine = !this.IsExperienceHero() ? 0 : svrInfo.stCommonInfo.dwDeadLine;
            this.m_masterHeroFightCnt = svrInfo.stCommonInfo.dwMasterTotalFightCnt;
            this.m_masterHeroWinCnt = svrInfo.stCommonInfo.dwMasterTotalWinCnt;
        }

        public bool IsExperienceHero()
        {
            return ((this.MaskBits & 8) != 0);
        }

        public bool IsValidExperienceHero()
        {
            bool flag = CRoleInfo.GetCurrentUTCTime() < this.m_experienceDeadLine;
            return (this.IsExperienceHero() && flag);
        }

        public void OnHeroInfoUpdate(SCPKG_NTF_HERO_INFO_UPD svrHeroInfoUp)
        {
            for (int i = 0; i < svrHeroInfoUp.iHeroUpdNum; i++)
            {
                CS_HEROINFO_UPD_TYPE bUpdType = (CS_HEROINFO_UPD_TYPE) svrHeroInfoUp.astHeroUpdInfo[i].bUpdType;
                int slotId = svrHeroInfoUp.astHeroUpdInfo[i].stValueParam.Value[0];
                switch (bUpdType)
                {
                    case CS_HEROINFO_UPD_TYPE.CS_HEROINFO_UPD_LEVEL:
                        this.mActorValue.actorLvl = slotId;
                        break;

                    case CS_HEROINFO_UPD_TYPE.CS_HEROINFO_UPD_EXP:
                        this.mActorValue.actorExp = slotId;
                        break;

                    case CS_HEROINFO_UPD_TYPE.CS_HEROINFO_UPD_STAR:
                        this.mActorValue.actorStar = slotId;
                        break;

                    case CS_HEROINFO_UPD_TYPE.CS_HEROINFO_UPD_QUALITY:
                        this.mActorValue.actorQuality = slotId;
                        break;

                    case CS_HEROINFO_UPD_TYPE.CS_HEROINFO_UPD_SUBQUALITY:
                        this.mActorValue.actorSubQuality = slotId;
                        break;

                    case CS_HEROINFO_UPD_TYPE.CS_HEROINFO_UPD_UNLOCKSKILLSLOT:
                        this.skillInfo.UnLockSkill(slotId);
                        break;

                    case CS_HEROINFO_UPD_TYPE.CS_HEROINFO_UPD_PROFICIENCY:
                        this.m_ProficiencyLV = (byte) slotId;
                        this.m_Proficiency = (uint) svrHeroInfoUp.astHeroUpdInfo[i].stValueParam.Value[1];
                        break;

                    case CS_HEROINFO_UPD_TYPE.CS_HEROINFO_UPD_MASKBITS:
                    {
                        uint maskBits = this.MaskBits;
                        this.MaskBits = (uint) slotId;
                        if (((maskBits & 2) == 0) && ((this.MaskBits & 2) > 0))
                        {
                            Singleton<EventRouter>.instance.BroadCastEvent<string>("HeroUnlockPvP", StringHelper.UTF8BytesToString(ref this.cfgInfo.szName));
                        }
                        break;
                    }
                    case CS_HEROINFO_UPD_TYPE.CS_HEROINFO_UPD_LIMITTIME:
                    {
                        string str = StringHelper.UTF8BytesToString(ref this.cfgInfo.szName);
                        Singleton<EventRouter>.instance.BroadCastEvent<string, uint, uint>("HeroExperienceTimeUpdate", str, this.m_experienceDeadLine, (uint) slotId);
                        this.m_experienceDeadLine = (uint) slotId;
                        break;
                    }
                    case CS_HEROINFO_UPD_TYPE.CS_HEROINFO_UPD_MASTERGAMECNT:
                        this.m_masterHeroWinCnt = (uint) slotId;
                        this.m_masterHeroFightCnt = (uint) svrHeroInfoUp.astHeroUpdInfo[i].stValueParam.Value[1];
                        break;
                }
            }
        }

        public void OnHeroSkinWear(uint skinId)
        {
            uint wearSkinId = this.m_skinInfo.GetWearSkinId();
            this.mActorValue.SetSkinProp(this.cfgInfo.dwCfgID, wearSkinId, false);
            this.m_skinInfo.SetWearSkinId(skinId);
            this.mActorValue.SetSkinProp(this.cfgInfo.dwCfgID, skinId, true);
        }

        public void OnSymbolPageChange(int pageIdx)
        {
            this.m_selectPageIndex = pageIdx;
            Singleton<EventRouter>.instance.BroadCastEvent<uint>("HeroSymbolPageChange", this.cfgInfo.dwCfgID);
        }
    }
}

