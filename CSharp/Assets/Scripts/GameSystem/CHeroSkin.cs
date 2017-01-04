namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    public class CHeroSkin : CUseable
    {
        public uint m_heroId;
        public object[] m_heroSkinData;
        public uint m_skinId;

        public CHeroSkin(ulong objID, uint baseID, int stackCount = 0, int addTime = 0)
        {
            ResHeroSkin dataByKey = GameDataMgr.heroSkinDatabin.GetDataByKey(baseID);
            if (dataByKey != null)
            {
                ResHeroSkinShop shop = null;
                GameDataMgr.skinShopInfoDict.TryGetValue(baseID, out shop);
                this.m_heroId = dataByKey.dwHeroID;
                this.m_skinId = dataByKey.dwSkinID;
                base.m_type = COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN;
                base.m_objID = objID;
                base.m_baseID = baseID;
                base.m_name = StringHelper.UTF8BytesToString(ref dataByKey.szSkinName);
                if (shop != null)
                {
                    base.m_description = StringHelper.UTF8BytesToString(ref shop.szSkinDesc);
                }
                else
                {
                    base.m_description = string.Empty;
                }
                base.m_iconID = uint.Parse(StringHelper.UTF8BytesToString(ref dataByKey.szSkinPicID));
                base.m_stackCount = stackCount;
                base.m_stackMax = 1;
                base.m_skinCoinBuy = (shop == null) ? 1 : shop.dwChgItemCnt;
                base.m_dianQuanBuy = (shop == null) ? 1 : shop.dwBuyCoupons;
                base.m_diamondBuy = (shop == null) ? 1 : shop.dwBuyDiamond;
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
                return COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN;
            }
        }
    }
}

