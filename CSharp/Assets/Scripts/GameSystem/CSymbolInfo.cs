namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class CSymbolInfo
    {
        private COMDT_SYMBOLPAGE_EXTRA[] GridBuyInfo;
        public int m_pageBuyCnt;
        public int m_pageCount;
        public int m_pageMaxLvlLimit;
        private string[] m_pageNameArr = new string[12];
        public uint m_selSymbolRcmdHeroId;
        public ushort m_selSymbolRcmdLevel = 1;
        private ulong[][] m_symbolPageArr = new ulong[12][];
        public static int s_maxCompLvl = 0;
        public static int s_maxSymbolLevel = 5;
        public static ListView<ResHeroSymbolLvl> s_symbolPvpLvlList = new ListView<ResHeroSymbolLvl>();

        public bool CheckAnyWearSymbol(out int posId, out uint symbolId)
        {
            posId = 0;
            symbolId = 0;
            CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
            int curUseableCount = useableContainer.GetCurUseableCount();
            CSymbolItem item = null;
            CSymbolItem item2 = null;
            int pos = 0;
            for (int i = 0; i < curUseableCount; i++)
            {
                CUseable useableByIndex = useableContainer.GetUseableByIndex(i);
                if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
                {
                    enFindSymbolWearPosCode code;
                    item = (CSymbolItem) useableByIndex;
                    if ((((item != null) && (item.m_stackCount > item.GetPageWearCnt(0))) && ((item.GetPageWearCnt(0) < CSymbolWearController.s_maxSameIDSymbolEquipNum) && this.GetWearPos(item, 0, out pos, out code))) && (((item2 == null) || (item.m_SymbolData.bColor < item2.m_SymbolData.bColor)) || ((item.m_SymbolData.bColor == item2.m_SymbolData.bColor) && (item.m_SymbolData.wLevel < item2.m_SymbolData.wLevel))))
                    {
                        item2 = item;
                        posId = pos;
                        symbolId = item.m_baseID;
                    }
                }
            }
            return (item2 != null);
        }

        public SymbolBuyCode CheckBuySymbolPage()
        {
            if (this.IsPageFull())
            {
                return SymbolBuyCode.PageFull;
            }
            RES_SHOPBUY_COINTYPE costType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN;
            uint costVal = 0;
            this.GetNewPageCost(out costType, out costVal);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
            if ((costType == RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN) && (masterRoleInfo.GoldCoin < costVal))
            {
                return SymbolBuyCode.CoinNotEnough;
            }
            if ((costType == RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS) && (masterRoleInfo.DianQuan < costVal))
            {
                return SymbolBuyCode.DiamondNotEnough;
            }
            return SymbolBuyCode.BuySuccess;
        }

        private static bool CheckComposeTypeSameId(int cnt, uint cfgId, int level)
        {
            uint key = GetComposeCfgId(level, RES_SYMBOLCOMP_TYPE.RES_SYMBOLCOMP_SAMEID, cfgId);
            ResSymbolComp dataByKey = GameDataMgr.symbolCompDatabin.GetDataByKey(key);
            if (dataByKey != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (dataByKey.astCompInfo[i].wSymbolCnt == cnt)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool CheckSymbolColor(ResSymbolPos symbolPos, byte color)
        {
            return ((symbolPos != null) && ((symbolPos.dwSymbolColor & (((int) 1) << color)) != 0L));
        }

        public static uint GetComposeCfgId(int level, RES_SYMBOLCOMP_TYPE compType, uint cfgId)
        {
            return (((uint) ((long) ((compType * ((RES_SYMBOLCOMP_TYPE) 0x2710)) + (level * 0x3e8)))) + cfgId);
        }

        public static RES_SYMBOLCOMP_TYPE GetComposeType(CSymbolItem[] symbolArr, out uint compParam)
        {
            compParam = 0;
            int length = symbolArr.Length;
            int cnt = 1;
            uint cfgId = 0;
            for (int i = 0; i < length; i++)
            {
                int num5 = 1;
                for (int k = 0; k < length; k++)
                {
                    if ((i != k) && (symbolArr[i].m_objID == symbolArr[k].m_objID))
                    {
                        cfgId = symbolArr[i].m_baseID;
                        num5++;
                    }
                }
                if (cnt < num5)
                {
                    cnt = num5;
                }
            }
            if (((cnt > 1) && CheckComposeTypeSameId(cnt, cfgId, symbolArr[0].m_SymbolData.wLevel)) && (GetSymbolCompInfo(symbolArr[0].m_SymbolData.wLevel, RES_SYMBOLCOMP_TYPE.RES_SYMBOLCOMP_SAMEID, cfgId) != null))
            {
                compParam = cfgId;
                return RES_SYMBOLCOMP_TYPE.RES_SYMBOLCOMP_SAMEID;
            }
            byte bColor = symbolArr[0].m_SymbolData.bColor;
            for (int j = 1; j < length; j++)
            {
                if (bColor != symbolArr[j].m_SymbolData.bColor)
                {
                    compParam = 0;
                    return RES_SYMBOLCOMP_TYPE.RES_SYMBOLCOMP_RANDOM;
                }
            }
            if (GetSymbolCompInfo(symbolArr[0].m_SymbolData.wLevel, RES_SYMBOLCOMP_TYPE.RES_SYMBOLCOMP_SAMECOLOR, bColor) != null)
            {
                compParam = bColor;
                return RES_SYMBOLCOMP_TYPE.RES_SYMBOLCOMP_SAMECOLOR;
            }
            compParam = 0;
            return RES_SYMBOLCOMP_TYPE.RES_SYMBOLCOMP_RANDOM;
        }

        private int GetFirstColorIndex(uint symBolColor)
        {
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.symbolPosDatabin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResSymbolPos pos = (ResSymbolPos) current.Value;
                if (pos.dwSymbolColor == symBolColor)
                {
                    return (pos.bSymbolPos - 1);
                }
            }
            return 0;
        }

        private uint GetLatestOpenLevel(uint playerLevel)
        {
            ResSymbolPos pos = null;
            for (int i = 0; i < this.GridBuyInfo.Length; i++)
            {
                ResSymbolPos symbolPos = GetSymbolPos(i);
                if ((symbolPos.wOpenLevel > playerLevel) && (this.GridBuyInfo[i].bBuyFlag == 0))
                {
                    if (pos == null)
                    {
                        pos = symbolPos;
                    }
                    if (symbolPos.wOpenLevel < pos.wOpenLevel)
                    {
                        pos = symbolPos;
                    }
                }
            }
            if (pos == null)
            {
                return (playerLevel + 1);
            }
            return pos.wOpenLevel;
        }

        public string GetMaxWearSymbolPageName(CSymbolItem symbol)
        {
            if (symbol == null)
            {
                return string.Empty;
            }
            string str = "\n";
            for (int i = 0; i < symbol.m_pageWearCnt.Length; i++)
            {
                if (symbol.m_stackCount <= symbol.m_pageWearCnt[i])
                {
                    str = str + string.Format("{0}; ", this.GetSymbolPageName(i));
                }
            }
            return str;
        }

        public static int GetMinWearPvpLvl(int symbolLvl)
        {
            int count = s_symbolPvpLvlList.Count;
            bool flag = false;
            for (int i = count - 1; i >= 0; i--)
            {
                if ((s_symbolPvpLvlList[i].wSymbolMaxLvl == symbolLvl) && (count > s_symbolPvpLvlList[i].wPvpLevel))
                {
                    count = s_symbolPvpLvlList[i].wPvpLevel;
                    flag = true;
                }
            }
            if (!flag)
            {
                count = 1;
            }
            return count;
        }

        public void GetNewPageCost(out RES_SHOPBUY_COINTYPE costType, out uint costVal)
        {
            costType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN;
            costVal = 0;
            ResShopInfo cfgShopInfo = CPurchaseSys.GetCfgShopInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_SYMBOLPAGE, this.m_pageBuyCnt + 1);
            if (cfgShopInfo != null)
            {
                costType = (RES_SHOPBUY_COINTYPE) cfgShopInfo.bCoinType;
                costVal = cfgShopInfo.dwCoinPrice;
            }
        }

        public int GetNextCanEquipPos(int page, int nowPos, ref ListView<CSymbolItem> symbolList)
        {
            int num = -1;
            if (((page >= 0) && (page < this.m_pageCount)) && ((nowPos >= 0) && (nowPos < 30)))
            {
                ulong[] numArray = this.m_symbolPageArr[page];
                uint pvpLevel = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().PvpLevel;
                uint dwSymbolColor = GetSymbolPos(nowPos).dwSymbolColor;
                int firstColorIndex = this.GetFirstColorIndex(dwSymbolColor);
                int num5 = -1;
                for (int i = firstColorIndex; i < numArray.Length; i++)
                {
                    if ((i != nowPos) && (numArray[i] == 0))
                    {
                        ResSymbolPos symbolPos = GetSymbolPos(i);
                        if ((symbolPos != null) && ((pvpLevel >= symbolPos.wOpenLevel) || (this.GridBuyInfo[i].bBuyFlag == 1)))
                        {
                            if (this.IsAnySymbolItemCanWear(page, i, ref symbolList))
                            {
                                num = i;
                                break;
                            }
                            if (num5 == -1)
                            {
                                num5 = i;
                            }
                        }
                    }
                }
                if (num == -1)
                {
                    for (int j = 0; j < firstColorIndex; j++)
                    {
                        if ((j != nowPos) && (numArray[j] == 0))
                        {
                            ResSymbolPos pos2 = GetSymbolPos(j);
                            if ((pos2 != null) && ((pvpLevel >= pos2.wOpenLevel) || (this.GridBuyInfo[j].bBuyFlag == 1)))
                            {
                                if (this.IsAnySymbolItemCanWear(page, j, ref symbolList))
                                {
                                    num = j;
                                    break;
                                }
                                if (num5 == -1)
                                {
                                    num5 = j;
                                }
                            }
                        }
                    }
                }
                if (num == -1)
                {
                    num = num5;
                }
            }
            return num;
        }

        public int GetNextFreePageLevel()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                DebugHelper.Assert(false, "GetNextFreePageLevel role is null");
                return 0;
            }
            if (this.m_pageCount < 12)
            {
                for (int i = 0; i < s_symbolPvpLvlList.Count; i++)
                {
                    if ((s_symbolPvpLvlList[i].wPvpLevel > masterRoleInfo.PvpLevel) && (s_symbolPvpLvlList[i].bPresentSymbolPage > 0))
                    {
                        return s_symbolPvpLvlList[i].wPvpLevel;
                    }
                }
            }
            return 0;
        }

        public ulong[] GetPageSymbolData(int pageIndex)
        {
            if ((pageIndex >= 0) && (pageIndex < this.m_pageCount))
            {
                return this.m_symbolPageArr[pageIndex];
            }
            return null;
        }

        public static ResSymbolComp GetSymbolCompInfo(int level, RES_SYMBOLCOMP_TYPE compType, uint comParam)
        {
            uint key = GetComposeCfgId(level, compType, comParam);
            return GameDataMgr.symbolCompDatabin.GetDataByKey(key);
        }

        public static int GetSymbolLvlLimit(int pvpLvl)
        {
            ResHeroSymbolLvl dataByKey = GameDataMgr.heroSymbolLvlDatabin.GetDataByKey((uint) ((ushort) pvpLvl));
            if (dataByKey == null)
            {
                return 0;
            }
            return dataByKey.wSymbolMaxLvl;
        }

        public static int GetSymbolLvWithArray(uint[] symbolArr)
        {
            int num = 0;
            ResSymbolInfo dataByKey = null;
            for (int i = 0; i < symbolArr.Length; i++)
            {
                dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(symbolArr[i]);
                if (dataByKey != null)
                {
                    num += dataByKey.wLevel;
                }
            }
            return num;
        }

        public int GetSymbolPageEft(int pageIndex)
        {
            ulong[] pageSymbolData = this.GetPageSymbolData(pageIndex);
            if (pageSymbolData == null)
            {
                return 0;
            }
            int num = 0;
            CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
            for (int i = 0; i < pageSymbolData.Length; i++)
            {
                if (pageSymbolData[i] > 0L)
                {
                    CSymbolItem useableByObjID = (CSymbolItem) useableContainer.GetUseableByObjID(pageSymbolData[i]);
                    if (useableByObjID != null)
                    {
                        num += useableByObjID.m_SymbolData.iCombatEft;
                    }
                }
            }
            return num;
        }

        public int GetSymbolPageMaxLvl(int pageIndex)
        {
            if ((pageIndex < 0) || (pageIndex >= this.m_pageCount))
            {
                return 0;
            }
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                DebugHelper.Assert(false, "role is null");
                return 0;
            }
            int num = 0;
            CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
            ulong objID = 0L;
            for (int i = 0; i < 30; i++)
            {
                if (this.m_symbolPageArr[pageIndex][i] > 0L)
                {
                    objID = this.m_symbolPageArr[pageIndex][i];
                    CUseable useableByObjID = useableContainer.GetUseableByObjID(objID);
                    if (useableByObjID != null)
                    {
                        CSymbolItem item = (CSymbolItem) useableByObjID;
                        if (item != null)
                        {
                            num += item.m_SymbolData.wLevel;
                        }
                    }
                }
            }
            return num;
        }

        public string GetSymbolPageName(int pageIdx)
        {
            if ((pageIdx < 0) || (pageIdx >= 12))
            {
                return string.Empty;
            }
            string str = this.m_pageNameArr[pageIdx];
            if (!string.IsNullOrEmpty(str))
            {
                return str;
            }
            return string.Format(Singleton<CTextManager>.GetInstance().GetText("Symbol_Page_Name"), pageIdx + 1);
        }

        public void GetSymbolPageProp(int pageIndex, ref int[] propArr, ref int[] propPctArr, bool bPvp)
        {
            if (((pageIndex >= 0) && (pageIndex < 12)) && (((propArr != null) && (propPctArr != null)) && (this.m_symbolPageArr != null)))
            {
                int index = 0;
                int num2 = 0x24;
                for (index = 0; index < num2; index++)
                {
                    propArr[index] = 0;
                    propPctArr[index] = 0;
                }
                ulong[] numArray = this.m_symbolPageArr[pageIndex];
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if ((masterRoleInfo != null) && (numArray != null))
                {
                    CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
                    if (useableContainer == null)
                    {
                        DebugHelper.Assert(false, "GetSymbolPageProp container is null");
                    }
                    else
                    {
                        for (index = 0; index < numArray.Length; index++)
                        {
                            if (numArray[index] > 0L)
                            {
                                CUseable useableByObjID = useableContainer.GetUseableByObjID(numArray[index]);
                                if (useableByObjID != null)
                                {
                                    ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(useableByObjID.m_baseID);
                                    if (dataByKey != null)
                                    {
                                        ResDT_FuncEft_Obj[] objArray = !bPvp ? dataByKey.astPveEftList : dataByKey.astFuncEftList;
                                        if (objArray != null)
                                        {
                                            for (int i = 0; i < objArray.Length; i++)
                                            {
                                                if (((objArray[i].wType != 0) && (objArray[i].wType < 0x24)) && (objArray[i].iValue != 0))
                                                {
                                                    if (objArray[i].bValType == 0)
                                                    {
                                                        propArr[objArray[i].wType] += objArray[i].iValue;
                                                    }
                                                    else if (objArray[i].bValType == 1)
                                                    {
                                                        propPctArr[objArray[i].wType] += objArray[i].iValue;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static ResSymbolPos GetSymbolPos(int pos)
        {
            ResSymbolPos dataByKey = GameDataMgr.symbolPosDatabin.GetDataByKey((uint) ((byte) (pos + 1)));
            if (dataByKey == null)
            {
                return null;
            }
            return dataByKey;
        }

        public static int GetSymbolPosOpenCnt(int pvpLvl)
        {
            int num = 0;
            for (int i = 0; i < 30; i++)
            {
                if (GetSymbolPos(i).wOpenLevel <= pvpLvl)
                {
                    num++;
                }
            }
            return num;
        }

        public enSymbolWearState GetSymbolPosWearState(int page, int pos, out int param)
        {
            param = 0;
            if (((page < 0) || (page >= this.m_pageCount)) || ((pos < 0) || (pos >= 30)))
            {
                return enSymbolWearState.OtherState;
            }
            ulong[] numArray = this.m_symbolPageArr[page];
            if (numArray[pos] > 0L)
            {
                return enSymbolWearState.WearSuccess;
            }
            uint pvpLevel = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().PvpLevel;
            uint latestOpenLevel = this.GetLatestOpenLevel(pvpLevel);
            ResSymbolPos symbolPos = GetSymbolPos(pos);
            DebugHelper.Assert(symbolPos != null, "symbolPos!=null");
            if (symbolPos == null)
            {
                return enSymbolWearState.OtherState;
            }
            param = symbolPos.wOpenLevel;
            if (pvpLevel >= symbolPos.wOpenLevel)
            {
                return enSymbolWearState.OpenToWear;
            }
            if (this.GridBuyInfo[pos].bBuyFlag == 1)
            {
                return enSymbolWearState.OpenToWear;
            }
            ulong[] numArray2 = this.m_symbolPageArr[page];
            int length = numArray2.Length;
            for (int i = 0; i < length; i++)
            {
                ResSymbolPos pos3 = GetSymbolPos(i);
                if (((pvpLevel < pos3.wOpenLevel) && (this.GridBuyInfo[pos3.bSymbolPos - 1].bBuyFlag == 0)) && (symbolPos.dwSymbolColor == pos3.dwSymbolColor))
                {
                    if (this.GridBuyInfo[symbolPos.bSymbolPos - 2].bBuyFlag == 1)
                    {
                        return ((((long) param) != latestOpenLevel) ? enSymbolWearState.CanBuy : enSymbolWearState.CanBuyAndWillOpen);
                    }
                    ResSymbolPos pos4 = GetSymbolPos(pos - 1);
                    if ((pos4 != null) && (pvpLevel >= pos4.wOpenLevel))
                    {
                        return ((((long) param) != latestOpenLevel) ? enSymbolWearState.CanBuy : enSymbolWearState.CanBuyAndWillOpen);
                    }
                    return enSymbolWearState.UnOpen;
                }
            }
            return enSymbolWearState.WillOpen;
        }

        public bool GetWearPos(CSymbolItem item, int page, out int pos, out enFindSymbolWearPosCode findCode)
        {
            pos = 0;
            findCode = enFindSymbolWearPosCode.FindNone;
            if ((item == null) || (item.m_SymbolData.wLevel > this.m_pageMaxLvlLimit))
            {
                findCode = enFindSymbolWearPosCode.FindSymbolLevelLimit;
                return false;
            }
            ulong[] numArray = this.m_symbolPageArr[page];
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            int length = numArray.Length;
            bool flag = true;
            for (int i = 0; i < length; i++)
            {
                ResSymbolPos symbolPos = GetSymbolPos(i);
                if ((symbolPos.dwSymbolColor & (((int) 1) << item.m_SymbolData.bColor)) != 0)
                {
                    flag = flag && (numArray[i] != 0L);
                }
                if (((numArray[i] == 0) && ((masterRoleInfo.PvpLevel >= symbolPos.wOpenLevel) || (this.GridBuyInfo[i].bBuyFlag == 1))) && ((symbolPos.dwSymbolColor & (((int) 1) << item.m_SymbolData.bColor)) != 0))
                {
                    pos = i;
                    findCode = enFindSymbolWearPosCode.FindSuccess;
                    return true;
                }
            }
            findCode = !flag ? enFindSymbolWearPosCode.FindSymbolNotOpen : enFindSymbolWearPosCode.FindSymbolPosFull;
            return false;
        }

        private void InitSymbolPvpLevelList()
        {
            s_symbolPvpLvlList.Clear();
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.heroSymbolLvlDatabin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResHeroSymbolLvl item = (ResHeroSymbolLvl) current.Value;
                if (item != null)
                {
                    s_symbolPvpLvlList.Add(item);
                }
            }
        }

        public bool IsAnySymbolItemCanWear(int pageIndex, int pos, ref ListView<CSymbolItem> symbolList)
        {
            ResSymbolPos symbolPos = GetSymbolPos(pos);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((((masterRoleInfo != null) && (pageIndex >= 0)) && ((pageIndex < this.m_pageCount) && (symbolPos != null))) && ((symbolList != null) && ((symbolPos.wOpenLevel <= masterRoleInfo.PvpLevel) || (this.GridBuyInfo[pos].bBuyFlag != 0))))
            {
                for (int i = 0; i < symbolList.Count; i++)
                {
                    if (this.IsSymbolPosMatchItem(pageIndex, symbolPos, symbolList[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsGridPosHasBuy(int gridPos)
        {
            return ((((this.GridBuyInfo != null) && (gridPos > 1)) && (gridPos <= this.GridBuyInfo.Length)) && (this.GridBuyInfo[gridPos - 1].bBuyFlag == 1));
        }

        public bool IsPageEmpty(int pageIndex)
        {
            if ((pageIndex < 0) || (pageIndex >= this.m_pageCount))
            {
                return false;
            }
            for (int i = 0; i < 30; i++)
            {
                if (this.m_symbolPageArr[pageIndex][i] > 0L)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsPageFull()
        {
            return (CPurchaseSys.GetCfgShopInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_SYMBOLPAGE, this.m_pageCount + 1) == null);
        }

        public bool IsSymbolPosMatchItem(int pageIndex, ResSymbolPos symbolPos, CSymbolItem symbol)
        {
            if (((pageIndex < 0) || (pageIndex >= this.m_pageCount)) || ((symbolPos == null) || (symbol == null)))
            {
                return false;
            }
            return ((CheckSymbolColor(symbolPos, symbol.m_SymbolData.bColor) && (symbol.m_stackCount > symbol.GetPageWearCnt(pageIndex))) && ((symbol.m_SymbolData.wLevel <= this.m_pageMaxLvlLimit) && (symbol.GetPageWearCnt(pageIndex) < CSymbolWearController.s_maxSameIDSymbolEquipNum)));
        }

        public void OnSymbolChange(int page, int pos, ulong objId, out uint cfgId, out enSymbolOperationType operType)
        {
            cfgId = 0;
            operType = enSymbolOperationType.Wear;
            if ((page >= 0) && (page < this.m_pageCount))
            {
                CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
                if (this.m_symbolPageArr[page][pos] > 0L)
                {
                    if (objId > 0L)
                    {
                        ulong num = this.m_symbolPageArr[page][pos];
                        this.m_symbolPageArr[page][pos] = objId;
                        this.SetSymbolItemWearCnt(num, page);
                        operType = enSymbolOperationType.Replace;
                    }
                    else
                    {
                        objId = this.m_symbolPageArr[page][pos];
                        this.m_symbolPageArr[page][pos] = 0L;
                        operType = enSymbolOperationType.TakeOff;
                    }
                }
                else
                {
                    this.m_symbolPageArr[page][pos] = objId;
                    operType = enSymbolOperationType.Wear;
                }
                CUseable useableByObjID = useableContainer.GetUseableByObjID(objId);
                if (useableByObjID != null)
                {
                    CSymbolItem item = (CSymbolItem) useableByObjID;
                    if (item != null)
                    {
                        cfgId = item.m_SymbolData.dwID;
                        item.SetPageWearCnt(page, this.m_symbolPageArr[page]);
                    }
                }
            }
        }

        public void OnSymbolPageClear(int pageIndex)
        {
            if ((pageIndex >= 0) && (pageIndex < this.m_pageCount))
            {
                ulong objId = 0L;
                for (int i = 0; i < 30; i++)
                {
                    if (this.m_symbolPageArr[pageIndex][i] > 0L)
                    {
                        objId = this.m_symbolPageArr[pageIndex][i];
                        this.m_symbolPageArr[pageIndex][i] = 0L;
                        this.SetSymbolItemWearCnt(objId, pageIndex);
                    }
                }
            }
        }

        private static void OnVisit(ResSymbolComp InSymbol)
        {
            if (s_maxCompLvl < InSymbol.wSymbolLevel)
            {
                s_maxCompLvl = InSymbol.wSymbolLevel;
            }
        }

        public void SetData(COMDT_ACNT_SYMBOLINFO svrInfo)
        {
            this.m_pageCount = svrInfo.bValidPageCnt;
            this.m_pageBuyCnt = svrInfo.bBuyPageCnt;
            for (int i = 0; i < svrInfo.bValidPageCnt; i++)
            {
                this.SetSymbolPageData(i, svrInfo.astPageList[i]);
            }
            this.SetSymbolPageMaxLevel();
            SetMaxSymbolCompLvl();
            this.InitSymbolPvpLevelList();
            this.GridBuyInfo = svrInfo.astPageExtra;
            this.m_selSymbolRcmdHeroId = svrInfo.stRecommend.dwSelHeroID;
            this.m_selSymbolRcmdLevel = svrInfo.stRecommend.wSelSymbolLvl;
        }

        public static void SetMaxSymbolCompLvl()
        {
            GameDataMgr.symbolCompDatabin.Accept(new Action<ResSymbolComp>(CSymbolInfo.OnVisit));
        }

        public void SetSymbolItemWearCnt(ulong objId, int pageIndex)
        {
            if ((pageIndex >= 0) && (pageIndex < this.m_pageCount))
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo == null)
                {
                    DebugHelper.Assert(false, "SetSymbolItemWearCnt role is null");
                }
                else
                {
                    CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
                    if ((objId > 0L) && (useableContainer != null))
                    {
                        CUseable useableByObjID = useableContainer.GetUseableByObjID(objId);
                        if (useableByObjID != null)
                        {
                            CSymbolItem item = (CSymbolItem) useableByObjID;
                            if (item != null)
                            {
                                item.SetPageWearCnt(pageIndex, this.m_symbolPageArr[pageIndex]);
                            }
                        }
                    }
                }
            }
        }

        public void SetSymbolPageBuyCnt(int cnt)
        {
            this.m_pageBuyCnt = cnt;
        }

        public void SetSymbolPageCount(int cnt)
        {
            this.m_pageCount = cnt;
            for (int i = 0; i < this.m_pageCount; i++)
            {
                if (this.m_symbolPageArr[i] == null)
                {
                    this.m_symbolPageArr[i] = new ulong[30];
                }
            }
        }

        private void SetSymbolPageData(int pageIndex, COMDT_SYMBOLPAGE_DETAIL pageDetail)
        {
            if ((pageIndex >= 0) && (pageIndex < this.m_pageCount))
            {
                if (this.m_symbolPageArr[pageIndex] == null)
                {
                    this.m_symbolPageArr[pageIndex] = new ulong[30];
                }
                this.m_pageNameArr[pageIndex] = StringHelper.UTF8BytesToString(ref pageDetail.szName);
                for (int i = 0; i < pageDetail.UniqueID.Length; i++)
                {
                    ulong objId = pageDetail.UniqueID[i];
                    this.m_symbolPageArr[pageIndex][i] = objId;
                    if (objId > 0L)
                    {
                        this.SetSymbolItemWearCnt(objId, pageIndex);
                    }
                }
            }
        }

        public void SetSymbolPageMaxLevel()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            ResHeroSymbolLvl dataByKey = GameDataMgr.heroSymbolLvlDatabin.GetDataByKey((uint) ((ushort) masterRoleInfo.PvpLevel));
            if (dataByKey != null)
            {
                this.m_pageMaxLvlLimit = dataByKey.wSymbolMaxLvl;
            }
        }

        public void SetSymbolPageName(int pageIndex, string pageName)
        {
            if ((pageIndex >= 0) && (pageIndex < 12))
            {
                this.m_pageNameArr[pageIndex] = pageName;
            }
        }

        public void UpdateBuyGridInfo(int gridPos)
        {
            DebugHelper.Assert(gridPos > 0, string.Format("grid pos must be above 0!! pos: {0}", gridPos));
            DebugHelper.Assert(gridPos <= this.GridBuyInfo.Length, string.Format("grid pos must less than GridBuyInfo.Length!! pos: {0}", gridPos));
            this.GridBuyInfo[gridPos - 1].bBuyFlag = 1;
        }

        public enum enSymbolOperationType
        {
            Wear,
            TakeOff,
            Replace
        }
    }
}

