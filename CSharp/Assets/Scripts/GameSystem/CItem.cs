namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CItem : CUseable
    {
        public ResPropInfo m_itemData;

        public CItem(ulong objID, uint baseID, int stackCount = 0, int addTime = 0)
        {
            this.m_itemData = GameDataMgr.itemDatabin.GetDataByKey(baseID);
            if (this.m_itemData == null)
            {
                Debug.Log("not item id" + baseID);
            }
            else
            {
                base.m_type = COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP;
                base.m_objID = objID;
                base.m_baseID = baseID;
                base.m_name = StringHelper.UTF8BytesToString(ref this.m_itemData.szName);
                base.m_description = StringHelper.UTF8BytesToString(ref this.m_itemData.szDesc);
                base.m_iconID = this.m_itemData.dwIcon;
                base.m_stackCount = stackCount;
                base.m_stackMax = this.m_itemData.iOverLimit;
                base.m_goldCoinBuy = this.m_itemData.dwPVPCoinBuy;
                base.m_dianQuanBuy = this.m_itemData.dwCouponsBuy;
                base.m_diamondBuy = this.m_itemData.dwDiamondBuy;
                base.m_arenaCoinBuy = this.m_itemData.dwArenaCoinBuy;
                base.m_burningCoinBuy = this.m_itemData.dwBurningCoinBuy;
                base.m_dianQuanDirectBuy = this.m_itemData.dwCouponsDirectBuy;
                base.m_guildCoinBuy = this.m_itemData.dwGuildCoinBuy;
                base.m_coinSale = this.m_itemData.dwCoinSale;
                base.m_grade = this.m_itemData.bGrade;
                base.m_isSale = this.m_itemData.bIsSale;
                base.m_isBatchUse = this.m_itemData.bIsBatchUse;
                base.m_addTime = addTime;
                base.ResetTime();
            }
        }

        public static uint GetExperienceCardHeroOrSkinId(uint itemId)
        {
            ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(itemId);
            if (dataByKey != null)
            {
                return (uint) dataByKey.EftParam[1];
            }
            return 0;
        }

        public override string GetIconPath()
        {
            return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + base.m_iconID);
        }

        public static bool IsGuildNameChangeCard(uint itemId)
        {
            ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(itemId);
            return ((dataByKey != null) && (((int) dataByKey.EftParam[0]) == 7));
        }

        public static bool IsHeroExChangeCoupons(uint itemID)
        {
            ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x86);
            DebugHelper.Assert(dataByKey != null, "global cfg databin err: hero exchange coupons id doesnt exist");
            return (itemID == dataByKey.dwConfValue);
        }

        public static bool IsHeroExperienceCard(uint itemId)
        {
            ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(itemId);
            return ((dataByKey != null) && (((int) dataByKey.EftParam[0]) == 4));
        }

        public static bool IsPlayerNameChangeCard(uint itemId)
        {
            ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(itemId);
            return ((dataByKey != null) && (((int) dataByKey.EftParam[0]) == 6));
        }

        public static bool IsSkinExChangeCoupons(uint itemID)
        {
            ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x87);
            DebugHelper.Assert(dataByKey != null, "global cfg databin err: skin exchange coupons id doesnt exist");
            return (itemID == dataByKey.dwConfValue);
        }

        public static bool IsSkinExperienceCard(uint itemId)
        {
            ResPropInfo dataByKey = GameDataMgr.itemDatabin.GetDataByKey(itemId);
            return ((dataByKey != null) && (((int) dataByKey.EftParam[0]) == 5));
        }

        public override COM_REWARDS_TYPE MapRewardType
        {
            get
            {
                return COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM;
            }
        }
    }
}

