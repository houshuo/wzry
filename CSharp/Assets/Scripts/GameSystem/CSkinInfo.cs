namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class CSkinInfo
    {
        private uint m_wearId;
        public static ListView<ResHeroSkin> s_availableSkin = new ListView<ResHeroSkin>();
        private static DictionaryView<uint, ListView<ResHeroSkin>> s_heroSkinDic = new DictionaryView<uint, ListView<ResHeroSkin>>();

        public static ListView<ResHeroSkin> GetAvailableSkinByHeroId(uint heroId)
        {
            s_availableSkin.Clear();
            if (s_heroSkinDic.ContainsKey(heroId))
            {
                ListView<ResHeroSkin> view = s_heroSkinDic[heroId];
                for (int i = 0; i < view.Count; i++)
                {
                    if (GameDataMgr.IsSkinAvailable(view[i].dwID))
                    {
                        s_availableSkin.Add(view[i]);
                    }
                }
            }
            return s_availableSkin;
        }

        public static int GetCombatEft(uint heroId, uint skinId)
        {
            ResHeroSkin heroSkin = GetHeroSkin(heroId, skinId);
            if (heroSkin != null)
            {
                return (int) heroSkin.dwCombatAbility;
            }
            return 0;
        }

        public static ResHeroSkin GetHeroSkin(uint uniSkinId)
        {
            return GameDataMgr.heroSkinDatabin.GetDataByKey(uniSkinId);
        }

        public static ResHeroSkin GetHeroSkin(uint heroId, uint skinId)
        {
            ListView<ResHeroSkin> view = null;
            s_heroSkinDic.TryGetValue(heroId, out view);
            if (view != null)
            {
                for (int i = 0; i < view.Count; i++)
                {
                    if (((view[i] != null) && (view[i].dwHeroID == heroId)) && (view[i].dwSkinID == skinId))
                    {
                        return view[i];
                    }
                }
            }
            return GetHeroSkin((heroId * 100) + skinId);
        }

        public static int GetHeroSkinCnt(uint heroId)
        {
            return GetAvailableSkinByHeroId(heroId).Count;
        }

        public static uint GetHeroSkinCost(uint heroId, uint skinId, RES_SHOPBUY_COINTYPE costType)
        {
            ResHeroSkinShop shop = null;
            uint skinCfgId = GetSkinCfgId(heroId, skinId);
            GameDataMgr.skinShopInfoDict.TryGetValue(skinCfgId, out shop);
            uint num2 = 0;
            if (shop == null)
            {
                return num2;
            }
            RES_SHOPBUY_COINTYPE res_shopbuy_cointype = costType;
            switch (res_shopbuy_cointype)
            {
                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SKINCOIN:
                    return shop.dwBuySkinCoin;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND:
                    return shop.dwBuyDiamond;
            }
            if (res_shopbuy_cointype != RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS)
            {
                return num2;
            }
            return shop.dwBuyCoupons;
        }

        public static string GetHeroSkinPic(uint heroId, uint skinId)
        {
            string str = string.Empty;
            ResHeroSkin heroSkin = GetHeroSkin(heroId, skinId);
            if (heroSkin != null)
            {
                str = StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID);
            }
            return str;
        }

        public static void GetHeroSkinProp(uint heroId, uint skinId, ref int[] propArr, ref int[] propPctArr)
        {
            int index = 0;
            int num2 = 0x24;
            for (index = 0; index < num2; index++)
            {
                propArr[index] = 0;
                propPctArr[index] = 0;
            }
            ResHeroSkin heroSkin = GetHeroSkin(heroId, skinId);
            if (heroSkin != null)
            {
                for (index = 0; index < heroSkin.astAttr.Length; index++)
                {
                    if (heroSkin.astAttr[index].wType == 0)
                    {
                        break;
                    }
                    if (heroSkin.astAttr[index].bValType == 0)
                    {
                        propArr[heroSkin.astAttr[index].wType] += heroSkin.astAttr[index].iValue;
                    }
                    else if (heroSkin.astAttr[index].bValType == 1)
                    {
                        propPctArr[heroSkin.astAttr[index].wType] += heroSkin.astAttr[index].iValue;
                    }
                }
            }
        }

        public static int GetIndexBySkinId(uint heroId, uint skinId)
        {
            ListView<ResHeroSkin> availableSkinByHeroId = GetAvailableSkinByHeroId(heroId);
            if (availableSkinByHeroId != null)
            {
                for (int i = 0; i < availableSkinByHeroId.Count; i++)
                {
                    if (availableSkinByHeroId[i].dwSkinID == skinId)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static uint GetSkinCfgId(uint heroId, uint skinId)
        {
            ResHeroSkin heroSkin = GetHeroSkin(heroId, skinId);
            if (heroSkin != null)
            {
                return heroSkin.dwID;
            }
            return 0;
        }

        public static bool GetSkinFeatureCnt(uint heroId, uint skinId, out int cnt)
        {
            cnt = 0;
            if (skinId == 0)
            {
                return false;
            }
            ResHeroSkin heroSkin = GetHeroSkin(heroId, skinId);
            if (heroSkin != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (string.IsNullOrEmpty(heroSkin.astFeature[i].szDesc))
                    {
                        break;
                    }
                    cnt++;
                }
            }
            return (cnt > 0);
        }

        public static uint GetSkinIdByIndex(uint heroId, int index)
        {
            ListView<ResHeroSkin> availableSkinByHeroId = GetAvailableSkinByHeroId(heroId);
            if (((availableSkinByHeroId != null) && (index >= 0)) && (index < availableSkinByHeroId.Count))
            {
                return availableSkinByHeroId[index].dwSkinID;
            }
            return 0;
        }

        public static string GetSkinName(uint skinUniId)
        {
            ResHeroSkin dataByKey = GameDataMgr.heroSkinDatabin.GetDataByKey(skinUniId);
            if (dataByKey != null)
            {
                return StringHelper.UTF8BytesToString(ref dataByKey.szSkinName);
            }
            return string.Empty;
        }

        public static stPayInfoSet GetSkinPayInfoSet(uint heroId, uint skinId)
        {
            ResSkinPromotion resPromotion = new ResSkinPromotion();
            stPayInfoSet set = new stPayInfoSet();
            resPromotion = GetSkinPromotion(heroId, skinId);
            return CMallSystem.GetPayInfoSetOfGood(GetHeroSkin(heroId, skinId), resPromotion);
        }

        public static ResSkinPromotion GetSkinPromotion(uint uniSkinId)
        {
            ResHeroSkinShop shop = null;
            GameDataMgr.skinShopInfoDict.TryGetValue(uniSkinId, out shop);
            if (shop != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    uint key = shop.PromotionID[i];
                    if ((key != 0) && GameDataMgr.skinPromotionDict.ContainsKey(key))
                    {
                        ResSkinPromotion promotion = new ResSkinPromotion();
                        if ((GameDataMgr.skinPromotionDict.TryGetValue(key, out promotion) && (promotion.dwOnTimeGen <= CRoleInfo.GetCurrentUTCTime())) && (promotion.dwOffTimeGen >= CRoleInfo.GetCurrentUTCTime()))
                        {
                            return promotion;
                        }
                    }
                }
            }
            return null;
        }

        public static ResSkinPromotion GetSkinPromotion(uint heroId, uint skinId)
        {
            return GetSkinPromotion(GetSkinCfgId(heroId, skinId));
        }

        public uint GetWearSkinId()
        {
            return this.m_wearId;
        }

        public void Init(ushort skinId)
        {
            this.m_wearId = skinId;
        }

        public static void InitHeroSkinDicData()
        {
            s_heroSkinDic.Clear();
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.heroSkinDatabin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResHeroSkin item = current.Value as ResHeroSkin;
                if ((item != null) && (GameDataMgr.heroDatabin.GetDataByKey(item.dwHeroID) != null))
                {
                    if (!s_heroSkinDic.ContainsKey(item.dwHeroID))
                    {
                        ListView<ResHeroSkin> view = new ListView<ResHeroSkin>();
                        s_heroSkinDic.Add(item.dwHeroID, view);
                    }
                    s_heroSkinDic[item.dwHeroID].Add(item);
                }
            }
        }

        public static bool IsBuyForbiddenForRankBigGrade(uint heroId, uint skinId, out RES_RANK_LIMIT_TYPE rankLimitType, out byte rankLimitBigGrade, out ulong rankLimitParam, out bool isHaveRankBigGradeLimit)
        {
            ResHeroSkinShop shop;
            uint skinCfgId = GetSkinCfgId(heroId, skinId);
            if (GameDataMgr.skinShopInfoDict.TryGetValue(skinCfgId, out shop))
            {
                rankLimitType = (RES_RANK_LIMIT_TYPE) shop.bRankLimitType;
                rankLimitBigGrade = shop.bRankLimitGrade;
                rankLimitParam = shop.ullRankLimitParam;
                switch (rankLimitType)
                {
                    case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_CURGRADE:
                        isHaveRankBigGradeLimit = true;
                        return (CLadderSystem.GetRankBigGrade(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_rankGrade) < rankLimitBigGrade);

                    case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_SEASONGRADE:
                        isHaveRankBigGradeLimit = true;
                        return (CLadderSystem.GetRankBigGrade(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_rankSeasonHighestGrade) < rankLimitBigGrade);

                    case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_MAXGRADE:
                        isHaveRankBigGradeLimit = true;
                        return (CLadderSystem.GetRankBigGrade(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_rankHistoryHighestGrade) < rankLimitBigGrade);

                    case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_HISTORYGRADE:
                        isHaveRankBigGradeLimit = true;
                        if (!Singleton<CLadderSystem>.GetInstance().IsCurSeason(rankLimitParam))
                        {
                            return (CLadderSystem.GetRankBigGrade(Singleton<CLadderSystem>.GetInstance().GetHistorySeasonGrade(rankLimitParam)) < rankLimitBigGrade);
                        }
                        return (CLadderSystem.GetRankBigGrade(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_rankGrade) < rankLimitBigGrade);
                }
            }
            rankLimitType = RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_NULL;
            rankLimitBigGrade = 0;
            rankLimitParam = 0L;
            isHaveRankBigGradeLimit = false;
            return false;
        }

        public static bool IsCanBuy(uint heroId, uint skinId)
        {
            ResHeroSkin heroSkin = GetHeroSkin(heroId, skinId);
            if (heroSkin == null)
            {
                return false;
            }
            if (!GameDataMgr.IsSkinAvailableAtShop(heroSkin.dwID))
            {
                return false;
            }
            ResSkinPromotion skinPromotion = new ResSkinPromotion();
            stPayInfoSet payInfoSetOfGood = new stPayInfoSet();
            skinPromotion = GetSkinPromotion(heroSkin.dwID);
            if (skinPromotion != null)
            {
                payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(false, 0, skinPromotion.bIsBuyCoupons > 0, skinPromotion.dwBuyCoupons, skinPromotion.bIsBuyDiamond > 0, skinPromotion.dwBuyDiamond, 0x2710);
            }
            else
            {
                payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(heroSkin);
            }
            return (payInfoSetOfGood.m_payInfoCount > 0);
        }

        public static bool IsOwnSkin(uint skinId, ulong ownBits)
        {
            ulong num = ((ulong) 1L) << skinId;
            return ((num & ownBits) != 0);
        }

        public static void ResolveHeroSkin(uint skinCfgId, out uint heroId, out uint skinId)
        {
            heroId = 0;
            skinId = 0;
            ResHeroSkin dataByKey = GameDataMgr.heroSkinDatabin.GetDataByKey(skinCfgId);
            if (dataByKey != null)
            {
                heroId = dataByKey.dwHeroID;
                skinId = dataByKey.dwSkinID;
            }
        }

        public void SetWearSkinId(uint skinId)
        {
            this.m_wearId = skinId;
        }
    }
}

