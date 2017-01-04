namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    public class CSymbolItem : CUseable
    {
        public int[] m_pageWearCnt = new int[12];
        public ResSymbolInfo m_SymbolData;

        public CSymbolItem(ulong objID, uint baseID, int stackCount = 0, int addTime = 0)
        {
            this.m_SymbolData = GameDataMgr.symbolInfoDatabin.GetDataByKey(baseID);
            if (this.m_SymbolData != null)
            {
                base.m_type = COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL;
                base.m_objID = objID;
                base.m_baseID = baseID;
                base.m_name = StringHelper.UTF8BytesToString(ref this.m_SymbolData.szName);
                base.m_description = StringHelper.UTF8BytesToString(ref this.m_SymbolData.szDesc);
                base.m_iconID = this.m_SymbolData.dwIcon;
                base.m_stackCount = stackCount;
                base.m_stackMax = this.m_SymbolData.iOverLimit;
                base.m_goldCoinBuy = this.m_SymbolData.dwPVPCoinBuy;
                base.m_dianQuanBuy = this.m_SymbolData.dwCouponsBuy;
                base.m_diamondBuy = this.m_SymbolData.dwDiamondBuy;
                base.m_arenaCoinBuy = this.m_SymbolData.dwArenaCoinBuy;
                base.m_burningCoinBuy = this.m_SymbolData.dwBurningCoinBuy;
                base.m_dianQuanDirectBuy = 0;
                base.m_guildCoinBuy = this.m_SymbolData.dwGuildCoinBuy;
                base.m_coinSale = this.m_SymbolData.dwCoinSale;
                base.m_grade = (byte) (this.m_SymbolData.wLevel - 1);
                base.m_isSale = this.m_SymbolData.bIsSale;
                base.m_addTime = addTime;
                base.ResetTime();
            }
        }

        public override string GetIconPath()
        {
            return (CUIUtility.s_Sprite_Dynamic_Icon_Dir + base.m_iconID);
        }

        public int GetMaxWearCnt()
        {
            int num = 0;
            for (int i = 0; i < this.m_pageWearCnt.Length; i++)
            {
                if (num < this.m_pageWearCnt[i])
                {
                    num = this.m_pageWearCnt[i];
                }
            }
            return num;
        }

        public int GetPageWearCnt(int page)
        {
            return this.m_pageWearCnt[page];
        }

        public override int GetSalableCount()
        {
            if (this.IsGuildSymbol())
            {
                return base.GetSalableCount();
            }
            int num = base.m_stackCount - this.GetMaxWearCnt();
            if (num < 0)
            {
                num = 0;
            }
            return num;
        }

        public bool IsGuildSymbol()
        {
            return ((this.m_SymbolData != null) && (this.m_SymbolData.dwGuildFacLv > 0));
        }

        public void SetPageWearCnt(int pageIndex, ulong[] symbolIdArr)
        {
            this.m_pageWearCnt[pageIndex] = 0;
            for (int i = 0; i < symbolIdArr.Length; i++)
            {
                if (symbolIdArr[i] == base.m_objID)
                {
                    this.m_pageWearCnt[pageIndex]++;
                }
            }
        }

        public override COM_REWARDS_TYPE MapRewardType
        {
            get
            {
                return COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOL;
            }
        }
    }
}

