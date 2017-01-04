namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CEquip : CUseable
    {
        public ResEquipInfo m_equipData;

        public CEquip(ulong objID, uint baseID, int stackCount = 0, int addTime = 0)
        {
            this.m_equipData = GameDataMgr.equipInfoDatabin.GetDataByKey(baseID);
            if (this.m_equipData == null)
            {
                Debug.Log("not equip id" + baseID);
            }
            else
            {
                base.m_type = COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP;
                base.m_objID = objID;
                base.m_baseID = baseID;
                base.m_name = StringHelper.UTF8BytesToString(ref this.m_equipData.szName);
                base.m_description = StringHelper.UTF8BytesToString(ref this.m_equipData.szDesc);
                base.m_iconID = this.m_equipData.dwIcon;
                base.m_stackCount = stackCount;
                base.m_stackMax = this.m_equipData.iOverLimit;
                base.m_goldCoinBuy = this.m_equipData.dwPVPCoinBuy;
                base.m_dianQuanBuy = this.m_equipData.dwCouponsBuy;
                base.m_diamondBuy = 0;
                base.m_arenaCoinBuy = this.m_equipData.dwArenaCoinBuy;
                base.m_burningCoinBuy = this.m_equipData.dwBurningCoinBuy;
                base.m_guildCoinBuy = this.m_equipData.dwGuildCoinBuy;
                base.m_dianQuanDirectBuy = 0;
                base.m_coinSale = this.m_equipData.dwCoinSale;
                base.m_grade = this.m_equipData.bGrade;
                base.m_isSale = this.m_equipData.bIsSale;
                base.m_addTime = addTime;
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
                return COM_REWARDS_TYPE.COM_REWARDS_TYPE_EQUIP;
            }
        }
    }
}

