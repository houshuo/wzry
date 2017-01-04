namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CHeroItem : CUseable
    {
        public ResHeroCfgInfo m_heroData;

        public CHeroItem(ulong objID, uint baseID, int stackCount = 0, int addTime = 0)
        {
            this.m_heroData = GameDataMgr.heroDatabin.GetDataByKey(baseID);
            if (this.m_heroData == null)
            {
                Debug.Log("not hero id" + baseID);
            }
            else
            {
                ResHeroShop shop = null;
                GameDataMgr.heroShopInfoDict.TryGetValue(baseID, out shop);
                base.m_type = COM_ITEM_TYPE.COM_OBJTYPE_HERO;
                base.m_objID = objID;
                base.m_baseID = baseID;
                base.m_name = StringHelper.UTF8BytesToString(ref this.m_heroData.szName);
                base.m_description = StringHelper.UTF8BytesToString(ref this.m_heroData.szHeroDesc);
                base.m_iconID = uint.Parse(StringHelper.UTF8BytesToString(ref this.m_heroData.szImagePath));
                base.m_stackCount = stackCount;
                base.m_stackMax = 1;
                base.m_goldCoinBuy = 0;
                base.m_dianQuanBuy = (shop == null) ? 1 : shop.dwBuyCoupons;
                base.m_diamondBuy = (shop == null) ? 1 : shop.dwBuyDiamond;
                base.m_arenaCoinBuy = (shop == null) ? 1 : shop.dwBuyArenaCoin;
                base.m_burningCoinBuy = (shop == null) ? 1 : shop.dwBuyBurnCoin;
                base.m_dianQuanDirectBuy = 0;
                base.m_coinSale = 0;
                base.m_grade = 3;
                base.m_isSale = 0;
                base.m_addTime = 0;
                base.ResetTime();
            }
        }

        public override string GetIconPath()
        {
            return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + base.m_iconID);
        }

        public override COM_REWARDS_TYPE MapRewardType
        {
            get
            {
                return COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO;
            }
        }
    }
}

