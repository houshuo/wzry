namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;

    public class SkinSortImp : Singleton<SkinSortImp>, IMallSort<CMallSortHelper.SkinSortType>, IComparer<ResHeroSkin>
    {
        private CultureInfo m_culture;
        private bool m_desc;
        private CRoleInfo m_roleInfo;
        private CMallSortHelper.SkinSortType m_sortType;

        public int Compare(ResHeroSkin l, ResHeroSkin r)
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
                case CMallSortHelper.SkinSortType.Name:
                    return this.CompareName(l, r);

                case CMallSortHelper.SkinSortType.Coupons:
                    return this.CompareCoupons(l, r);

                case CMallSortHelper.SkinSortType.ReleaseTime:
                    return this.CompareReleaseTime(l, r);
            }
            return this.CompareDefault(l, r);
        }

        private int CompareCoupons(ResHeroSkin l, ResHeroSkin r)
        {
            ResSkinPromotion skinPromotion = CSkinInfo.GetSkinPromotion(l.dwHeroID, l.dwSkinID);
            ResSkinPromotion resPromotion = CSkinInfo.GetSkinPromotion(r.dwHeroID, r.dwSkinID);
            stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(l, skinPromotion);
            stPayInfoSet set2 = CMallSystem.GetPayInfoSetOfGood(r, resPromotion);
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

        private int CompareDefault(ResHeroSkin l, ResHeroSkin r)
        {
            int num = 0;
            ResSkinPromotion skinPromotion = CSkinInfo.GetSkinPromotion(l.dwHeroID, l.dwSkinID);
            ResSkinPromotion promotion2 = CSkinInfo.GetSkinPromotion(r.dwHeroID, r.dwSkinID);
            ResHeroSkinShop shop = null;
            GameDataMgr.skinShopInfoDict.TryGetValue(l.dwID, out shop);
            ResHeroSkinShop shop2 = null;
            GameDataMgr.skinShopInfoDict.TryGetValue(r.dwID, out shop2);
            uint dwSortIndex = (shop == null) ? uint.MaxValue : shop.dwSortId;
            uint num3 = (shop2 == null) ? uint.MaxValue : shop2.dwSortId;
            if (skinPromotion != null)
            {
                dwSortIndex = skinPromotion.dwSortIndex;
            }
            if (promotion2 != null)
            {
                num3 = promotion2.dwSortIndex;
            }
            if (dwSortIndex < num3)
            {
                num = 1;
            }
            if (dwSortIndex > num3)
            {
                num = -1;
            }
            bool flag = this.m_roleInfo.IsHaveHeroSkin(l.dwHeroID, l.dwSkinID, false);
            bool flag2 = this.m_roleInfo.IsHaveHeroSkin(r.dwHeroID, r.dwSkinID, false);
            if (flag && !flag2)
            {
                return 1;
            }
            if (!flag && flag2)
            {
                return -1;
            }
            if ((skinPromotion == null) || (skinPromotion.bSortIndexOnly <= 0))
            {
                if ((promotion2 != null) && (promotion2.bSortIndexOnly > 0))
                {
                    return num;
                }
                if (CSkinInfo.IsCanBuy(l.dwHeroID, l.dwSkinID) && !CSkinInfo.IsCanBuy(r.dwHeroID, r.dwSkinID))
                {
                    return -1;
                }
                if (!CSkinInfo.IsCanBuy(l.dwHeroID, l.dwSkinID) && CSkinInfo.IsCanBuy(r.dwHeroID, r.dwSkinID))
                {
                    return 1;
                }
                if (this.m_roleInfo.IsHaveHero(l.dwHeroID, false) && !this.m_roleInfo.IsHaveHero(r.dwHeroID, false))
                {
                    return -1;
                }
                if (!this.m_roleInfo.IsHaveHero(l.dwHeroID, false) && this.m_roleInfo.IsHaveHero(r.dwHeroID, false))
                {
                    return 1;
                }
            }
            return num;
        }

        private int CompareName(ResHeroSkin l, ResHeroSkin r)
        {
            string strA = string.Format("{0}{1}", l.szHeroName, l.szSkinName);
            string strB = string.Format("{0}{1}", r.szHeroName, r.szSkinName);
            return string.Compare(strA, strB, this.m_culture, CompareOptions.None);
        }

        private int CompareReleaseTime(ResHeroSkin l, ResHeroSkin r)
        {
            ResHeroSkinShop shop = null;
            ResHeroSkinShop shop2 = null;
            GameDataMgr.skinShopInfoDict.TryGetValue(l.dwID, out shop);
            GameDataMgr.skinShopInfoDict.TryGetValue(r.dwID, out shop2);
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

        public CMallSortHelper.SkinSortType GetCurSortType()
        {
            return this.m_sortType;
        }

        public string GetSortTypeName(CMallSortHelper.SkinSortType sortType = 0)
        {
            int num = (int) sortType;
            if ((num >= 0) && (num <= CMallSortHelper.skinSortTypeNameKeys.Length))
            {
                return Singleton<CTextManager>.GetInstance().GetText(CMallSortHelper.skinSortTypeNameKeys[(int) sortType]);
            }
            return null;
        }

        public override void Init()
        {
            base.Init();
            this.m_sortType = CMallSortHelper.SkinSortType.Default;
            this.m_desc = false;
            this.m_roleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            this.m_culture = new CultureInfo("zh-CN");
        }

        public bool IsDesc()
        {
            return this.m_desc;
        }

        public void SetSortType(CMallSortHelper.SkinSortType sortType = 0)
        {
            if (sortType != CMallSortHelper.SkinSortType.Default)
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
            this.m_roleInfo = null;
            this.m_culture = null;
        }
    }
}

