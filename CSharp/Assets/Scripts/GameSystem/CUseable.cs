namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CUseable
    {
        public int ExtraFromData;
        public int ExtraFromType;
        public int m_addTime;
        public uint m_arenaCoinBuy;
        public uint m_baseID;
        public uint m_burningCoinBuy;
        public uint m_coinSale;
        public string m_description = string.Empty;
        public uint m_diamondBuy;
        public uint m_dianQuanBuy;
        public uint m_dianQuanDirectBuy;
        public ulong m_getTime;
        public uint m_goldCoinBuy;
        public byte m_grade;
        public uint m_guildCoinBuy;
        public uint m_iconID;
        public byte m_isBatchUse;
        public byte m_isSale;
        public ulong m_itemSortNum;
        public string m_name = string.Empty;
        public ulong m_objID;
        public uint m_skinCoinBuy;
        public int m_stackCount = 1;
        public int m_stackMax;
        public int m_stackMulti;
        public COM_ITEM_TYPE m_type;

        public uint GetBuyPrice(RES_SHOPBUY_COINTYPE coinType)
        {
            switch (coinType)
            {
                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
                    return this.m_dianQuanBuy;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
                    return this.m_goldCoinBuy;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
                    return this.m_burningCoinBuy;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
                    return this.m_arenaCoinBuy;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SKINCOIN:
                    return this.m_skinCoinBuy;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_GUILDCOIN:
                    return this.m_guildCoinBuy;

                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND:
                case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_MIXPAY:
                    return this.m_diamondBuy;
            }
            return 0;
        }

        public virtual string GetIconPath()
        {
            return string.Empty;
        }

        public static ushort GetMultiple(ref COMDT_REWARD_MULTIPLE_DETAIL multipleDetail, CUseable usb)
        {
            for (int i = 0; i < multipleDetail.bNum; i++)
            {
                COMDT_REWARD_MULTIPLE_INFO comdt_reward_multiple_info = multipleDetail.astMultiple[i];
                if (comdt_reward_multiple_info.wRewardType == ((ushort) usb.MapRewardType))
                {
                    if (comdt_reward_multiple_info.wRewardType == 1)
                    {
                        if (((CItem) usb).m_itemData.bClass == comdt_reward_multiple_info.dwRewardTypeParam)
                        {
                            return (ushort) (comdt_reward_multiple_info.stMultipleInfo.dwWealRatio / 0x2710);
                        }
                    }
                    else
                    {
                        return (ushort) (comdt_reward_multiple_info.stMultipleInfo.dwWealRatio / 0x2710);
                    }
                }
            }
            return 0;
        }

        public static ushort GetMultiple(ref COMDT_REWARD_MULTIPLE_DETAIL multipleDetail, ushort rewardType, short subType = -1)
        {
            for (int i = 0; i < multipleDetail.bNum; i++)
            {
                COMDT_REWARD_MULTIPLE_INFO comdt_reward_multiple_info = multipleDetail.astMultiple[i];
                if ((comdt_reward_multiple_info.wRewardType == rewardType) && ((subType < 0) || (((uint) subType) == comdt_reward_multiple_info.dwRewardTypeParam)))
                {
                    return (ushort) ((((((((comdt_reward_multiple_info.stMultipleInfo.dwWealRatio + comdt_reward_multiple_info.stMultipleInfo.dwQQVIPRatio) + comdt_reward_multiple_info.stMultipleInfo.dwPropRatio) + comdt_reward_multiple_info.stMultipleInfo.dwPvpDailyRatio) + comdt_reward_multiple_info.stMultipleInfo.dwWXGameCenterLoginRatio) + comdt_reward_multiple_info.stMultipleInfo.dwGuildRatio) + comdt_reward_multiple_info.stMultipleInfo.dwQQGameCenterLoginRatio) + comdt_reward_multiple_info.stMultipleInfo.dwIOSVisitorLoginRatio) / 100);
                }
            }
            return 0;
        }

        public static COMDT_MULTIPLE_INFO GetMultipleInfo(ref COMDT_REWARD_MULTIPLE_DETAIL multipleDetail, ushort rewardType, short subType = -1)
        {
            for (int i = 0; i < multipleDetail.bNum; i++)
            {
                COMDT_REWARD_MULTIPLE_INFO comdt_reward_multiple_info = multipleDetail.astMultiple[i];
                if ((comdt_reward_multiple_info.wRewardType == rewardType) && ((subType < 0) || (((uint) subType) == comdt_reward_multiple_info.dwRewardTypeParam)))
                {
                    return comdt_reward_multiple_info.stMultipleInfo;
                }
            }
            return null;
        }

        public static uint GetQqVipExtraCoin(uint totalCoin, ref COMDT_REWARD_MULTIPLE_DETAIL multipleDetail, ushort rewardType)
        {
            for (int i = 0; i < multipleDetail.bNum; i++)
            {
                COMDT_REWARD_MULTIPLE_INFO comdt_reward_multiple_info = multipleDetail.astMultiple[i];
                if (comdt_reward_multiple_info.wRewardType == rewardType)
                {
                    if (comdt_reward_multiple_info.stMultipleInfo.dwQQVIPRatio <= 0)
                    {
                        break;
                    }
                    float num2 = ((float) comdt_reward_multiple_info.stMultipleInfo.dwQQVIPRatio) / 10000f;
                    return (uint) ((((float) totalCoin) / ((((float) comdt_reward_multiple_info.stMultipleInfo.dwWealRatio) / 10000f) + num2)) * num2);
                }
            }
            return 0;
        }

        public virtual int GetSalableCount()
        {
            return this.m_stackCount;
        }

        public void ResetTime()
        {
            this.m_getTime = this.m_addTime + ((ulong) Time.time);
        }

        public void SetMultiple(ref COMDT_REWARD_MULTIPLE_DETAIL multipleDetail, bool preCond = true)
        {
            this.m_stackMulti = !preCond ? 0 : GetMultiple(ref multipleDetail, this);
        }

        public virtual bool HasOwnMax
        {
            get
            {
                return false;
            }
        }

        public virtual COM_REWARDS_TYPE MapRewardType
        {
            get
            {
                return COM_REWARDS_TYPE.COM_REWARDS_TYPE_MAX;
            }
        }
    }
}

