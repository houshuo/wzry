namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;

    public class HeroSortImp : Singleton<HeroSortImp>, IMallSort<CMallSortHelper.HeroSortType>, IComparer<ResHeroCfgInfo>
    {
        private CultureInfo m_culture;
        private bool m_desc;
        private CRoleInfo m_roleInfo;
        private CMallSortHelper.HeroSortType m_sortType;

        public int Compare(ResHeroCfgInfo l, ResHeroCfgInfo r)
        {
            if (l == null)
            {
                return 1;
            }
            if (r == null)
            {
                return -1;
            }
            if (this.m_roleInfo == null)
            {
                return -1;
            }
            switch (this.m_sortType)
            {
                case CMallSortHelper.HeroSortType.Name:
                    return this.CompareName(l, r);

                case CMallSortHelper.HeroSortType.Coupons:
                    return this.CompareCoupons(l, r);

                case CMallSortHelper.HeroSortType.Coin:
                    return this.CompareCoin(l, r);

                case CMallSortHelper.HeroSortType.ReleaseTime:
                    return this.CompareReleaseTime(l, r);
            }
            return this.CompareDefault(l, r);
        }

        private int CompareCoin(ResHeroCfgInfo l, ResHeroCfgInfo r)
        {
            ResHeroPromotion resPromotion = CHeroDataFactory.CreateHeroData(l.dwCfgID).promotion();
            ResHeroPromotion promotion2 = CHeroDataFactory.CreateHeroData(r.dwCfgID).promotion();
            ResHeroShop shop = null;
            ResHeroShop shop2 = null;
            GameDataMgr.heroShopInfoDict.TryGetValue(l.dwCfgID, out shop);
            GameDataMgr.heroShopInfoDict.TryGetValue(r.dwCfgID, out shop2);
            stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(l, resPromotion);
            stPayInfoSet set2 = CMallSystem.GetPayInfoSetOfGood(r, promotion2);
            uint maxValue = uint.MaxValue;
            uint payValue = uint.MaxValue;
            for (int i = 0; i < payInfoSetOfGood.m_payInfoCount; i++)
            {
                if ((payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.GoldCoin) && (payInfoSetOfGood.m_payInfos[i].m_payValue <= maxValue))
                {
                    maxValue = payInfoSetOfGood.m_payInfos[i].m_payValue;
                }
            }
            for (int j = 0; j < set2.m_payInfoCount; j++)
            {
                if ((set2.m_payInfos[j].m_payType == enPayType.GoldCoin) && (set2.m_payInfos[j].m_payValue <= payValue))
                {
                    payValue = set2.m_payInfos[j].m_payValue;
                }
            }
            if ((maxValue == uint.MaxValue) && this.IsDesc())
            {
                maxValue = 0;
            }
            if ((payValue == uint.MaxValue) && this.IsDesc())
            {
                payValue = 0;
            }
            return maxValue.CompareTo(payValue);
        }

        private int CompareCoupons(ResHeroCfgInfo l, ResHeroCfgInfo r)
        {
            ResHeroPromotion resPromotion = CHeroDataFactory.CreateHeroData(l.dwCfgID).promotion();
            ResHeroPromotion promotion2 = CHeroDataFactory.CreateHeroData(r.dwCfgID).promotion();
            stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(l, resPromotion);
            stPayInfoSet set2 = CMallSystem.GetPayInfoSetOfGood(r, promotion2);
            uint maxValue = uint.MaxValue;
            uint payValue = uint.MaxValue;
            for (int i = 0; i < payInfoSetOfGood.m_payInfoCount; i++)
            {
                if ((((payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.Diamond) || (payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.DianQuan)) || (payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.DiamondAndDianQuan)) && (payInfoSetOfGood.m_payInfos[i].m_payValue < maxValue))
                {
                    maxValue = payInfoSetOfGood.m_payInfos[i].m_payValue;
                }
            }
            for (int j = 0; j < set2.m_payInfoCount; j++)
            {
                if ((((set2.m_payInfos[j].m_payType == enPayType.Diamond) || (set2.m_payInfos[j].m_payType == enPayType.DianQuan)) || (set2.m_payInfos[j].m_payType == enPayType.DiamondAndDianQuan)) && (set2.m_payInfos[j].m_payValue < payValue))
                {
                    payValue = set2.m_payInfos[j].m_payValue;
                }
            }
            if ((maxValue == uint.MaxValue) && this.IsDesc())
            {
                maxValue = 0;
            }
            if ((payValue == uint.MaxValue) && this.IsDesc())
            {
                payValue = 0;
            }
            return maxValue.CompareTo(payValue);
        }

        private int CompareDefault(ResHeroCfgInfo l, ResHeroCfgInfo r)
        {
            bool flag = this.m_roleInfo.IsHaveHero(l.dwCfgID, false);
            bool flag2 = this.m_roleInfo.IsHaveHero(r.dwCfgID, false);
            if (flag && !flag2)
            {
                return 1;
            }
            if (!flag && flag2)
            {
                return -1;
            }
            ResHeroPromotion promotion = CHeroDataFactory.CreateHeroData(l.dwCfgID).promotion();
            ResHeroPromotion promotion2 = CHeroDataFactory.CreateHeroData(r.dwCfgID).promotion();
            ResHeroShop shop = null;
            ResHeroShop shop2 = null;
            GameDataMgr.heroShopInfoDict.TryGetValue(l.dwCfgID, out shop);
            GameDataMgr.heroShopInfoDict.TryGetValue(r.dwCfgID, out shop2);
            uint dwSortIndex = (shop == null) ? uint.MaxValue : shop.dwSortId;
            uint num2 = (shop2 == null) ? uint.MaxValue : shop2.dwSortId;
            if (promotion != null)
            {
                dwSortIndex = promotion.dwSortIndex;
            }
            if (promotion2 != null)
            {
                num2 = promotion2.dwSortIndex;
            }
            if (dwSortIndex < num2)
            {
                return 1;
            }
            if (dwSortIndex > num2)
            {
                return -1;
            }
            if (l.dwCfgID < r.dwCfgID)
            {
                return -1;
            }
            if (l.dwCfgID > r.dwCfgID)
            {
                return 1;
            }
            return 0;
        }

        private int CompareName(ResHeroCfgInfo l, ResHeroCfgInfo r)
        {
            return string.Compare(l.szName, r.szName, this.m_culture, CompareOptions.None);
        }

        private int CompareReleaseTime(ResHeroCfgInfo l, ResHeroCfgInfo r)
        {
            ResHeroShop shop = null;
            ResHeroShop shop2 = null;
            GameDataMgr.heroShopInfoDict.TryGetValue(l.dwCfgID, out shop);
            GameDataMgr.heroShopInfoDict.TryGetValue(r.dwCfgID, out shop2);
            if (shop == null)
            {
                return 1;
            }
            if (shop2 == null)
            {
                return -1;
            }
            return shop.dwReleaseId.CompareTo(shop2.dwReleaseId);
        }

        public CMallSortHelper.HeroSortType GetCurSortType()
        {
            return this.m_sortType;
        }

        public string GetSortTypeName(CMallSortHelper.HeroSortType sortType = 0)
        {
            int num = (int) sortType;
            if ((num >= 0) && (num <= CMallSortHelper.heroSortTypeNameKeys.Length))
            {
                return Singleton<CTextManager>.GetInstance().GetText(CMallSortHelper.heroSortTypeNameKeys[(int) sortType]);
            }
            return null;
        }

        public override void Init()
        {
            base.Init();
            this.m_sortType = CMallSortHelper.HeroSortType.Default;
            this.m_desc = false;
            this.m_roleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            this.m_culture = new CultureInfo("zh-CN");
        }

        public bool IsDesc()
        {
            return this.m_desc;
        }

        public void SetSortType(CMallSortHelper.HeroSortType sortType = 0)
        {
            if (sortType != CMallSortHelper.HeroSortType.Default)
            {
                if (this.m_sortType != sortType)
                {
                    this.m_desc = false;
                }
                else
                {
                    this.m_desc = !this.m_desc;
                }
            }
            else
            {
                this.m_desc = false;
            }
            this.m_sortType = sortType;
        }

        public override void UnInit()
        {
            base.UnInit();
            this.m_sortType = CMallSortHelper.HeroSortType.Default;
            this.m_desc = false;
            this.m_roleInfo = null;
            this.m_culture = null;
        }
    }
}

