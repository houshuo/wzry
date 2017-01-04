namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CSymbolWearController
    {
        private CSymbolItem m_curViewSymbol;
        private ListView<CSymbolItem> m_pageSymbolBagList = new ListView<CSymbolItem>();
        private int m_selectSymbolPos = -1;
        private int m_symbolPageIndex;
        private CSymbolSystem m_symbolSys;
        public static int s_maxSameIDSymbolEquipNum = 10;
        public static string s_symbolBagPanel = "Panel_SymbolEquip/Panel_SymbolBag";
        public static string s_symbolEquipPanel = "Panel_SymbolEquip";
        public static string s_symbolPagePanel = "Panel_SymbolEquip/Panel_SymbolPageRect/Panel_SymbolPage";
        public static readonly Vector2 s_symbolPos1 = new Vector2(25f, -1f);
        public static readonly Vector2 s_symbolPos2 = new Vector2(0f, -1f);

        public static void AddTip(GameObject target, string tip, enUseableTipsPos pos)
        {
            if (null != target)
            {
                stUIEventParams eventParams = new stUIEventParams {
                    tagStr = tip,
                    tag = (int) pos
                };
                CUIEventScript component = target.GetComponent<CUIEventScript>();
                if (component != null)
                {
                    component.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_CommonInfoOpen, eventParams);
                    component.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_CommonInfoClose, eventParams);
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_CommonInfoClose, eventParams);
                    component.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_CommonInfoClose, eventParams);
                }
            }
        }

        public void Clear()
        {
            this.ClearSymbolEquipData();
        }

        private void ClearSymbolEquipData()
        {
            this.m_selectSymbolPos = -1;
        }

        private void ConfirmWhenMoneyNotEnough(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            DebugHelper.Assert(tag > 0, "gridPos should be above 0!!!");
            ResShopInfo gridShopInfo = this.GetGridShopInfo(tag);
            DebugHelper.Assert(gridShopInfo != null, "shopCfg is NULL!!!");
            string goodName = StringHelper.UTF8BytesToString(ref gridShopInfo.szDesc);
            CMallSystem.TryToPay(enPayPurpose.Open, goodName, CMallSystem.ResBuyTypeToPayType(gridShopInfo.bCoinType), gridShopInfo.dwCoinPrice, enUIEventID.Symbol_ConfirmBuyGrid, ref uiEvent.m_eventParams, enUIEventID.None, true, true, false);
        }

        private ResShopInfo GetGridShopInfo(int pos)
        {
            return CPurchaseSys.GetCfgShopInfo(RES_SHOPBUY_TYPE.RES_BUYTYPE_SYMBOLPAGEPOS, pos);
        }

        public int GetSymbolListIndex(uint symbolCfgId)
        {
            if (this.m_pageSymbolBagList.Count > 0)
            {
                for (int i = 0; i < this.m_pageSymbolBagList.Count; i++)
                {
                    if (this.m_pageSymbolBagList[i].m_baseID == symbolCfgId)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static string GetSymbolWearTip(uint cfgId, bool bWear)
        {
            string format = !bWear ? Singleton<CTextManager>.GetInstance().GetText("Symbol_TakeOffTip") : Singleton<CTextManager>.GetInstance().GetText("Symbol_WearTip");
            ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(cfgId);
            if (dataByKey == null)
            {
                return string.Empty;
            }
            format = string.Format(format, StringHelper.UTF8BytesToString(ref dataByKey.szName)) + "\n";
            ushort item = 0;
            byte bValType = 0;
            int iValue = 0;
            for (int i = 0; i < dataByKey.astFuncEftList.Length; i++)
            {
                item = dataByKey.astFuncEftList[i].wType;
                bValType = dataByKey.astFuncEftList[i].bValType;
                iValue = dataByKey.astFuncEftList[i].iValue;
                if (item != 0)
                {
                    switch (bValType)
                    {
                        case 0:
                            if (CUICommonSystem.s_pctFuncEftList.IndexOf(item) != -1)
                            {
                                format = format + (!bWear ? string.Format("{0}-{1}", CUICommonSystem.s_attNameList[item], CUICommonSystem.GetValuePercent(iValue / 100)) : string.Format("{0}+{1}", CUICommonSystem.s_attNameList[item], CUICommonSystem.GetValuePercent(iValue / 100)));
                            }
                            else
                            {
                                format = format + (!bWear ? string.Format("{0}-{1}", CUICommonSystem.s_attNameList[item], ((float) iValue) / 100f) : string.Format("{0}+{1}", CUICommonSystem.s_attNameList[item], ((float) iValue) / 100f));
                            }
                            break;

                        case 1:
                            format = format + (!bWear ? string.Format("{0}-{1}", CUICommonSystem.s_attNameList[item], CUICommonSystem.GetValuePercent(iValue)) : string.Format("{0}+{1}", CUICommonSystem.s_attNameList[item], CUICommonSystem.GetValuePercent(iValue)));
                            break;
                    }
                }
            }
            return format;
        }

        public void Init(CSymbolSystem symbolSys)
        {
            this.m_symbolSys = symbolSys;
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_Off, new CUIEventManager.OnUIEventHandler(this.OnOffSymbol));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_View, new CUIEventManager.OnUIEventHandler(this.OnSymbolView));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BagItemEnable, new CUIEventManager.OnUIEventHandler(this.OnSymbolBagElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BagItemClick, new CUIEventManager.OnUIEventHandler(this.OnSymbolBagItemClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BagViewHide, new CUIEventManager.OnUIEventHandler(this.OnSymbolBagSymbolViewHide));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BagShow, new CUIEventManager.OnUIEventHandler(this.OnSymbolBagShow));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_View_NotWear_Item, new CUIEventManager.OnUIEventHandler(this.OnViewNotWearItem));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_ChangeSymbolClick, new CUIEventManager.OnUIEventHandler(this.OnChangeSymbolClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_ChangeConfirm, new CUIEventManager.OnUIEventHandler(this.OnSymbolChangeConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_PageClear, new CUIEventManager.OnUIEventHandler(this.OnSymbolPageClear));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_PageClearConfirm, new CUIEventManager.OnUIEventHandler(this.OnSymbolPageClearConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_OpenUnlockLvlTip, new CUIEventManager.OnUIEventHandler(this.OnOpenUnlockLvlTip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_PromptBuyGrid, new CUIEventManager.OnUIEventHandler(this.OnPromptBuyGrid));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_ConfirmBuyGrid, new CUIEventManager.OnUIEventHandler(this.OnConfirmBuyGrid));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_ConfirmWhenMoneyNotEnough, new CUIEventManager.OnUIEventHandler(this.ConfirmWhenMoneyNotEnough));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_SymbolPageDownBtnClick, new CUIEventManager.OnUIEventHandler(this.OnSymbolPageDownBtnClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_PageItemSelect, new CUIEventManager.OnUIEventHandler(this.OnSymbolPageItemSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_ChangePageName, new CUIEventManager.OnUIEventHandler(this.OnChangeSymbolPageName));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_PageAddClick, new CUIEventManager.OnUIEventHandler(this.OnSymbol_PageAddClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_ConfirmChgPageName, new CUIEventManager.OnUIEventHandler(this.OnConfirmChgPageName));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Purchase_BuySymbolPage, new CUIEventManager.OnUIEventHandler(this.OnBuySymbolPage));
            Singleton<EventRouter>.instance.AddEventHandler("MasterPvpLevelChanged", new System.Action(this.RefreshPageDropList));
        }

        private bool IsSymbolChangePanelShow(CUIFormScript form)
        {
            if (null == form)
            {
                return false;
            }
            bool flag = false;
            Transform transform = form.transform.Find(s_symbolEquipPanel);
            if (transform != null)
            {
                CUIComponent component = transform.GetComponent<CUIComponent>();
                if (component != null)
                {
                    GameObject widget = component.GetWidget(5);
                    flag = (widget != null) && widget.activeSelf;
                }
            }
            return flag;
        }

        private bool MovePosToNextCanEquipPos()
        {
            bool flag = false;
            ListView<CSymbolItem> allSymbolList = this.m_symbolSys.GetAllSymbolList();
            int num = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_symbolInfo.GetNextCanEquipPos(this.m_symbolPageIndex, this.m_selectSymbolPos, ref allSymbolList);
            if (num != -1)
            {
                stUIEventParams par = new stUIEventParams();
                par.symbolParam.symbol = null;
                par.symbolParam.page = this.m_symbolPageIndex;
                par.symbolParam.pos = num;
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Symbol_BagShow, par);
                flag = true;
            }
            return flag;
        }

        public static void OnBuyNewSymbolPage()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo.m_symbolInfo.IsPageFull())
            {
                string text = Singleton<CTextManager>.GetInstance().GetText("Symbol_Page_Buy_PageFull");
                Singleton<CUIManager>.GetInstance().OpenTips(text, false, 1.5f, null, new object[0]);
            }
            else
            {
                RES_SHOPBUY_COINTYPE costType = RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN;
                uint costVal = 0;
                masterRoleInfo.m_symbolInfo.GetNewPageCost(out costType, out costVal);
                string goodName = "新的符文页";
                stUIEventParams confirmEventParams = new stUIEventParams();
                CMallSystem.TryToPay(enPayPurpose.Buy, goodName, CMallSystem.ResBuyTypeToPayType((int) costType), costVal, enUIEventID.Purchase_BuySymbolPage, ref confirmEventParams, enUIEventID.None, true, true, false);
            }
        }

        public void OnBuySymbolPage(CUIEvent uiEvent)
        {
            SendBuySymbol(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_symbolInfo.m_pageBuyCnt + 1);
        }

        private void OnChangeSymbolClick(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (form != null)
            {
                Transform transform = form.transform.Find(s_symbolEquipPanel);
                if (transform != null)
                {
                    CUIComponent component = transform.GetComponent<CUIComponent>();
                    if (component != null)
                    {
                        component.GetWidget(0).CustomSetActive(true);
                        component.GetWidget(1).CustomSetActive(false);
                        component.GetWidget(6).CustomSetActive(false);
                        component.GetWidget(5).CustomSetActive(true);
                        this.RefreshSymbolChangePanel(this.m_curViewSymbol, null);
                        this.ShowSymbolBag(true);
                    }
                }
            }
        }

        private void OnChangeSymbolPageName(CUIEvent uiEvent)
        {
            string text = Singleton<CTextManager>.GetInstance().GetText("Symbol_ChangeName");
            Singleton<CUIManager>.GetInstance().OpenInputBox(text, string.Empty, enUIEventID.Symbol_ConfirmChgPageName);
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/Common/Form_InputBox.prefab");
            if (form != null)
            {
                Transform transform = form.transform.Find("Panel/inputText");
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if ((transform != null) && (masterRoleInfo != null))
                {
                    string symbolPageName = masterRoleInfo.m_symbolInfo.GetSymbolPageName(this.m_symbolPageIndex);
                    transform.GetComponent<InputField>().text = symbolPageName;
                }
            }
        }

        private void OnConfirmBuyGrid(CUIEvent uiEvent)
        {
            DebugHelper.Assert(uiEvent.m_eventParams.tag > 0, "gridPos should be above 0!!!");
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x459);
            msg.stPkgData.stShopBuyReq.iBuyType = 12;
            msg.stPkgData.stShopBuyReq.iBuySubType = uiEvent.m_eventParams.tag;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void OnConfirmChgPageName(CUIEvent uiEvent)
        {
            string text = uiEvent.m_srcFormScript.gameObject.transform.Find("Panel/inputText/Text").GetComponent<Text>().text;
            if (string.IsNullOrEmpty(text) || (text.Length > 6))
            {
                string strContent = Singleton<CTextManager>.GetInstance().GetText("Symbol_Name_LenError");
                Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
            }
            else
            {
                SendChgSymbolPageName(this.m_symbolPageIndex, text);
            }
        }

        private void OnOffSymbol(CUIEvent uiEvent)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x46f);
            msg.stPkgData.stSymbolOff.bPage = (byte) this.m_symbolPageIndex;
            msg.stPkgData.stSymbolOff.bPos = (byte) this.m_selectSymbolPos;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void OnOpenUnlockLvlTip(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            object[] replaceArr = new object[] { tag };
            Singleton<CUIManager>.GetInstance().OpenTips("Symbol_PosUnlock_lvl_Tip", true, 1f, null, replaceArr);
        }

        private void OnPromptBuyGrid(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            DebugHelper.Assert(tag > 0, "gridPos should be above 0!!!");
            ResShopInfo gridShopInfo = this.GetGridShopInfo(tag);
            DebugHelper.Assert(gridShopInfo != null, "shopCfg is NULL!!!");
            string goodName = StringHelper.UTF8BytesToString(ref gridShopInfo.szDesc);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                enPayType payType = CMallSystem.ResBuyTypeToPayType(gridShopInfo.bCoinType);
                uint dwCoinPrice = gridShopInfo.dwCoinPrice;
                if (CMallSystem.GetCurrencyValueFromRoleInfo(masterRoleInfo, payType) < dwCoinPrice)
                {
                    string str2 = string.Empty;
                    if (payType == enPayType.DiamondAndDianQuan)
                    {
                        uint diamond = masterRoleInfo.Diamond;
                        if (diamond < dwCoinPrice)
                        {
                            object[] objArray1 = new object[] { (dwCoinPrice - diamond).ToString(), Singleton<CTextManager>.GetInstance().GetText(CMallSystem.s_payTypeNameKeys[3]), (dwCoinPrice - diamond).ToString(), Singleton<CTextManager>.GetInstance().GetText(CMallSystem.s_payTypeNameKeys[2]) };
                            str2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("MixPayNotice"), objArray1);
                        }
                    }
                    string[] args = new string[] { gridShopInfo.dwCoinPrice.ToString(), Singleton<CTextManager>.GetInstance().GetText(CMallSystem.s_payTypeNameKeys[(int) payType]), Singleton<CTextManager>.GetInstance().GetText(CMallSystem.s_payPurposeNameKeys[4]), goodName, str2 };
                    string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("ConfirmPay", args), new object[0]);
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.Symbol_ConfirmWhenMoneyNotEnough, enUIEventID.None, uiEvent.m_eventParams, false);
                }
                else
                {
                    CMallSystem.TryToPay(enPayPurpose.Open, goodName, CMallSystem.ResBuyTypeToPayType(gridShopInfo.bCoinType), gridShopInfo.dwCoinPrice, enUIEventID.Symbol_ConfirmBuyGrid, ref uiEvent.m_eventParams, enUIEventID.None, true, true, false);
                }
            }
        }

        private void OnSymbol_PageAddClick(CUIEvent uiEvent)
        {
            OnBuyNewSymbolPage();
        }

        private void OnSymbolBagElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            GameObject gameObject = uiEvent.m_srcWidget.transform.Find("itemCell").gameObject;
            if (((this.m_symbolSys.m_selectMenuType == enSymbolMenuType.SymbolEquip) && (srcWidgetIndexInBelongedList >= 0)) && (srcWidgetIndexInBelongedList < this.m_pageSymbolBagList.Count))
            {
                this.SetSymbolListItem(uiEvent.m_srcFormScript, gameObject, this.m_pageSymbolBagList[srcWidgetIndexInBelongedList]);
            }
        }

        private void OnSymbolBagItemClick(CUIEvent uiEvent)
        {
            if ((uiEvent.m_srcWidgetIndexInBelongedList < 0) || (uiEvent.m_srcWidgetIndexInBelongedList >= this.m_pageSymbolBagList.Count))
            {
                DebugHelper.Assert(false, "OnSymbolBagItemClick index is out of array index");
            }
            else
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                CSymbolItem toSymbol = this.m_pageSymbolBagList[uiEvent.m_srcWidgetIndexInBelongedList];
                if (toSymbol != null)
                {
                    if (toSymbol.m_SymbolData.wLevel > masterRoleInfo.m_symbolInfo.m_pageMaxLvlLimit)
                    {
                        int minWearPvpLvl = CSymbolInfo.GetMinWearPvpLvl(toSymbol.m_SymbolData.wLevel);
                        string[] args = new string[] { minWearPvpLvl.ToString() };
                        Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Symbol_LvLimit", args), false, 1.5f, null, new object[0]);
                    }
                    else if (toSymbol.GetPageWearCnt(this.m_symbolPageIndex) >= s_maxSameIDSymbolEquipNum)
                    {
                        Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Symbol_CountLimit"), false, 1.5f, null, new object[0]);
                    }
                    else if (this.IsSymbolChangePanelShow(uiEvent.m_srcFormScript))
                    {
                        this.RefreshSymbolChangePanel(this.m_curViewSymbol, toSymbol);
                        CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
                        if (srcFormScript != null)
                        {
                            Transform transform = srcFormScript.transform.Find(s_symbolEquipPanel);
                            if (transform != null)
                            {
                                CUIComponent component = transform.GetComponent<CUIComponent>();
                                if (component != null)
                                {
                                    GameObject widget = component.GetWidget(7);
                                    GameObject obj3 = component.GetWidget(9);
                                    stUIEventParams eventParams = new stUIEventParams();
                                    eventParams.symbolParam.symbol = toSymbol;
                                    widget.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Symbol_ChangeConfirm, eventParams);
                                    widget.CustomSetActive(true);
                                    obj3.CustomSetActive(false);
                                }
                            }
                        }
                    }
                    else if (this.m_selectSymbolPos >= 0)
                    {
                        SendWearSymbol((byte) this.m_symbolPageIndex, (byte) this.m_selectSymbolPos, toSymbol.m_objID);
                    }
                    else
                    {
                        enFindSymbolWearPosCode code;
                        int pos = -1;
                        if (masterRoleInfo.m_symbolInfo.GetWearPos(toSymbol, this.m_symbolPageIndex, out pos, out code))
                        {
                            SendWearSymbol((byte) this.m_symbolPageIndex, (byte) pos, toSymbol.m_objID);
                        }
                        else if (code == enFindSymbolWearPosCode.FindSymbolPosFull)
                        {
                            Singleton<CUIManager>.GetInstance().OpenTips("Symbol_FindSymbolPosFull_Tip", true, 1.5f, null, new object[0]);
                        }
                        else if (code == enFindSymbolWearPosCode.FindSymbolNotOpen)
                        {
                            Singleton<CUIManager>.GetInstance().OpenTips("Symbol_FindSymbolNotOpen_Tip", true, 1.5f, null, new object[0]);
                        }
                        else if (code == enFindSymbolWearPosCode.FindSymbolLevelLimit)
                        {
                            Singleton<CUIManager>.GetInstance().OpenTips("Symbol_FindSymbolLevelLimit_Tip", true, 1.5f, null, new object[0]);
                        }
                    }
                }
            }
        }

        private void OnSymbolBagShow(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (form != null)
            {
                Transform transform = form.transform.Find(s_symbolEquipPanel);
                if (transform != null)
                {
                    CUIComponent component = transform.GetComponent<CUIComponent>();
                    if (component != null)
                    {
                        component.GetWidget(0).CustomSetActive(true);
                        component.GetWidget(1).CustomSetActive(false);
                        this.SetSeletSymbolPos(uiEvent.m_eventParams.symbolParam.pos);
                        this.ShowSymbolBag(false);
                    }
                }
            }
        }

        private void OnSymbolBagSymbolViewHide(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (form != null)
            {
                Transform transform = form.transform.Find(s_symbolEquipPanel);
                if (transform != null)
                {
                    CUIComponent component = transform.GetComponent<CUIComponent>();
                    if (component != null)
                    {
                        component.GetWidget(0).CustomSetActive(false);
                        component.GetWidget(1).CustomSetActive(false);
                        component.GetWidget(5).CustomSetActive(false);
                        component.GetWidget(6).CustomSetActive(true);
                        this.SetSymbolItemSelect(this.m_selectSymbolPos, false);
                    }
                }
            }
        }

        public static void OnSymbolBuySuccess(int pageCount, int buyCnt)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.m_symbolInfo.SetSymbolPageCount(pageCount);
                masterRoleInfo.m_symbolInfo.SetSymbolPageBuyCnt(buyCnt);
            }
            Singleton<CSymbolSystem>.GetInstance().m_symbolWearCtrl.RefreshPageDropList();
            Singleton<CUIManager>.GetInstance().OpenTips("购买成功", false, 1.5f, null, new object[0]);
        }

        private void OnSymbolChangeConfirm(CUIEvent uiEvent)
        {
            CSymbolItem symbol = uiEvent.m_eventParams.symbolParam.symbol;
            if (symbol != null)
            {
                SendWearSymbol((byte) this.m_symbolPageIndex, (byte) this.m_selectSymbolPos, symbol.m_objID);
            }
        }

        public void OnSymbolEquip(bool bMoveNext)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (form != null)
            {
                Transform transform = form.transform.Find(s_symbolEquipPanel);
                if (transform != null)
                {
                    CUIComponent component = transform.GetComponent<CUIComponent>();
                    if (component != null)
                    {
                        component.GetWidget(5).CustomSetActive(false);
                        component.GetWidget(6).CustomSetActive(true);
                        this.RefreshSymbolPage();
                        this.RefreshSymbolProp();
                        this.RefreshPageDropList();
                        if (this.m_selectSymbolPos >= 0)
                        {
                            if (!bMoveNext || !this.MovePosToNextCanEquipPos())
                            {
                                this.OnSymbolBagSymbolViewHide(null);
                            }
                        }
                        else
                        {
                            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Symbol_View_NotWear_Item);
                        }
                    }
                }
            }
        }

        public void OnSymbolEquipOff(bool bClear)
        {
            if (Singleton<CSymbolSystem>.GetInstance().m_selectMenuType == enSymbolMenuType.SymbolEquip)
            {
                this.RefreshSymbolPage();
                this.RefreshSymbolProp();
                this.RefreshPageDropList();
                if (bClear)
                {
                    this.m_selectSymbolPos = -1;
                }
                else
                {
                    stUIEventParams par = new stUIEventParams();
                    par.symbolParam.symbol = null;
                    par.symbolParam.page = this.m_symbolPageIndex;
                    par.symbolParam.pos = this.m_selectSymbolPos;
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Symbol_BagShow, par);
                }
            }
        }

        public static void OnSymbolGridBuySuccess(int gridPos)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.m_symbolInfo.UpdateBuyGridInfo(gridPos);
                Singleton<CSymbolSystem>.GetInstance().RefreshSymbolForm();
            }
        }

        public void OnSymbolPageChange()
        {
            this.RefreshSymbolPage();
            this.RefreshSymbolProp();
            this.OnSymbolBagSymbolViewHide(null);
        }

        private void OnSymbolPageClear(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if ((masterRoleInfo != null) && masterRoleInfo.m_symbolInfo.IsPageEmpty(this.m_symbolPageIndex))
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Symbol_Clear_NoWear_Tip", true, 1.5f, null, new object[0]);
            }
            else
            {
                string text = Singleton<CTextManager>.GetInstance().GetText("Symbol_Page_Clear_Tip");
                Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Symbol_PageClearConfirm, enUIEventID.None, false);
            }
        }

        private void OnSymbolPageClearConfirm(CUIEvent uiEvent)
        {
            SendReqClearSymbolPage(this.m_symbolPageIndex);
        }

        public void OnSymbolPageDownBtnClick(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (form != null)
            {
                Transform transform = form.GetWidget(1).transform.Find("DropList/List");
                transform.gameObject.CustomSetActive(!transform.gameObject.activeSelf);
            }
        }

        private void OnSymbolPageItemSelect(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                DebugHelper.Assert(false, "OnSymbolPageItemSelect role is null");
            }
            else
            {
                CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
                int elementAmount = component.GetElementAmount();
                int selectedIndex = component.GetSelectedIndex();
                GameObject widget = uiEvent.m_srcFormScript.GetWidget(1);
                widget.transform.Find("DropList/List").gameObject.CustomSetActive(false);
                if ((elementAmount == masterRoleInfo.m_symbolInfo.m_pageCount) || ((elementAmount > masterRoleInfo.m_symbolInfo.m_pageCount) && (selectedIndex != (elementAmount - 1))))
                {
                    this.m_symbolPageIndex = selectedIndex;
                    this.OnSymbolPageChange();
                    widget.transform.Find("DropList/Button_Down/Text").GetComponent<Text>().text = masterRoleInfo.m_symbolInfo.GetSymbolPageName(this.m_symbolPageIndex);
                    widget.transform.Find("DropList/Button_Down/SymbolLevel/Text").GetComponent<Text>().text = masterRoleInfo.m_symbolInfo.GetSymbolPageMaxLvl(this.m_symbolPageIndex).ToString();
                }
            }
        }

        private void OnSymbolView(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (form != null)
            {
                Transform transform = form.transform.Find(s_symbolEquipPanel);
                if (transform != null)
                {
                    CUIComponent component = transform.GetComponent<CUIComponent>();
                    if (component != null)
                    {
                        component.GetWidget(1).CustomSetActive(true);
                        component.GetWidget(0).CustomSetActive(false);
                        this.SetSeletSymbolPos(uiEvent.m_eventParams.symbolParam.pos);
                        this.ShowSymbolView(uiEvent);
                    }
                }
            }
        }

        private void OnViewNotWearItem(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (form != null)
            {
                this.SetSymbolItemSelect(this.m_selectSymbolPos, false);
                this.m_selectSymbolPos = -1;
                Transform transform = form.transform.Find(s_symbolEquipPanel);
                if (transform != null)
                {
                    CUIComponent component = transform.GetComponent<CUIComponent>();
                    if (component != null)
                    {
                        component.GetWidget(0).CustomSetActive(true);
                        component.GetWidget(1).CustomSetActive(false);
                        this.SetNotWearSymbolListData();
                        this.RefreshSymbolBag(true);
                    }
                }
            }
        }

        [MessageHandler(0x470)]
        public static void ReciveSymbolChange(CSPkg msg)
        {
            CSymbolInfo.enSymbolOperationType type;
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_SYMBOLCHG stSymbolChg = msg.stPkgData.stSymbolChg;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            uint cfgId = 0;
            masterRoleInfo.m_symbolInfo.OnSymbolChange(stSymbolChg.bPageIdx, stSymbolChg.bPosIdx, stSymbolChg.ullUniqueID, out cfgId, out type);
            switch (type)
            {
                case CSymbolInfo.enSymbolOperationType.Wear:
                case CSymbolInfo.enSymbolOperationType.Replace:
                {
                    string symbolWearTip = GetSymbolWearTip(cfgId, true);
                    Singleton<CUIManager>.GetInstance().OpenTips(symbolWearTip, false, 1.5f, null, new object[0]);
                    if (type == CSymbolInfo.enSymbolOperationType.Wear)
                    {
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_fuwen_zhuangbei", null);
                    }
                    Singleton<CSymbolSystem>.GetInstance().m_symbolWearCtrl.OnSymbolEquip(type == CSymbolInfo.enSymbolOperationType.Wear);
                    break;
                }
                default:
                    if (type == CSymbolInfo.enSymbolOperationType.TakeOff)
                    {
                        string strContent = GetSymbolWearTip(cfgId, false);
                        Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
                        Singleton<CSoundManager>.GetInstance().PostEvent("UI_rune_dblclick", null);
                        Singleton<CSymbolSystem>.GetInstance().m_symbolWearCtrl.OnSymbolEquipOff(false);
                    }
                    break;
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SymbolEquipSuc);
            Singleton<CBagSystem>.GetInstance().RefreshBagForm();
        }

        [MessageHandler(0x462)]
        public static void ReciveSymbolNameChange(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_SYMBOLNAMECHG stSymbolNameChgRsp = msg.stPkgData.stSymbolNameChgRsp;
            if (stSymbolNameChgRsp.iResult == 0)
            {
                int bPageIdx = stSymbolNameChgRsp.bPageIdx;
                string pageName = StringHelper.UTF8BytesToString(ref stSymbolNameChgRsp.szPageName);
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_symbolInfo.SetSymbolPageName(bPageIdx, pageName);
                Singleton<CSymbolSystem>.GetInstance().m_symbolWearCtrl.RefreshPageDropList();
            }
            else if (stSymbolNameChgRsp.iResult == 0x7d)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Symbol_Name_illegal", true, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x48b)]
        public static void ReciveSymbolPageClear(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_SYMBOLPAGE_CLR stSymbolPageClrRsp = msg.stPkgData.stSymbolPageClrRsp;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.m_symbolInfo.OnSymbolPageClear(stSymbolPageClrRsp.bSymbolPageIdx);
            }
            Singleton<CSymbolSystem>.GetInstance().m_symbolWearCtrl.OnSymbolEquipOff(true);
            Singleton<CBagSystem>.GetInstance().RefreshBagForm();
        }

        private void RefreshPageDropList()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (form != null)
            {
                this.SetPageDropListData(form, this.m_symbolPageIndex);
            }
        }

        private void RefreshSymbolBag(bool bNoWeatItem)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (form != null)
            {
                Transform transform = form.gameObject.transform.Find(s_symbolBagPanel);
                transform.Find("Panel_BagList/List").GetComponent<CUIListScript>().SetElementAmount(this.m_pageSymbolBagList.Count);
                Transform transform2 = transform.Find("Panel_BagList/lblTitle");
                if (transform2 != null)
                {
                    transform2.GetComponent<Text>().text = !bNoWeatItem ? Singleton<CTextManager>.GetInstance().GetText("Symbol_Title_SymbolBagList") : Singleton<CTextManager>.GetInstance().GetText("Symbol_Title_NoWear");
                }
                GameObject gameObject = transform.Find("Panel_BagList/lblTips").gameObject;
                bool flag = this.IsSymbolChangePanelShow(form);
                if (this.m_pageSymbolBagList.Count == 0)
                {
                    gameObject.CustomSetActive(true);
                }
                else
                {
                    gameObject.CustomSetActive(false);
                }
                Transform transform3 = form.transform.Find(s_symbolEquipPanel);
                if (transform3 != null)
                {
                    CUIComponent component = transform3.GetComponent<CUIComponent>();
                    if (component != null)
                    {
                        GameObject widget = component.GetWidget(8);
                        widget.CustomSetActive(!flag || (this.m_pageSymbolBagList.Count == 0));
                        if (CSysDynamicBlock.bLobbyEntryBlocked)
                        {
                            widget.CustomSetActive(false);
                        }
                        component.GetWidget(7).CustomSetActive(false);
                        component.GetWidget(9).CustomSetActive(flag && (this.m_pageSymbolBagList.Count > 0));
                    }
                }
            }
        }

        private void RefreshSymbolChangePanel(CSymbolItem fromSymbol, CSymbolItem toSymbol)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if ((null != form) && (fromSymbol != null))
            {
                Transform transform = form.transform.Find(s_symbolEquipPanel);
                if (transform != null)
                {
                    CUIComponent component = transform.GetComponent<CUIComponent>();
                    if (component != null)
                    {
                        GameObject widget = component.GetWidget(5);
                        if (null != widget)
                        {
                            GameObject gameObject = widget.transform.Find("symbolFrom").gameObject;
                            this.RefreshSymbolInfo(form, gameObject, fromSymbol);
                            GameObject obj4 = widget.transform.Find("symbolTo").gameObject;
                            obj4.CustomSetActive(toSymbol != null);
                            if (toSymbol != null)
                            {
                                this.RefreshSymbolInfo(form, obj4, toSymbol);
                            }
                        }
                    }
                }
            }
        }

        public void RefreshSymbolEquipPanel()
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath) != null)
            {
                this.m_symbolSys.SetSymbolData();
                this.RefreshSymbolPage();
                this.RefreshPageDropList();
                this.RefreshSymbolProp();
                this.OnSymbolBagSymbolViewHide(null);
            }
        }

        private void RefreshSymbolInfo(CUIFormScript form, GameObject symbolPanel, CSymbolItem symbol)
        {
            if ((null != symbolPanel) && (symbol != null))
            {
                symbolPanel.transform.Find("itemCell/imgIcon").GetComponent<Image>().SetSprite(symbol.GetIconPath(), form, true, false, false);
                symbolPanel.transform.Find("lblName").GetComponent<Text>().text = symbol.m_name;
                CSymbolSystem.RefreshSymbolPropContent(symbolPanel.transform.Find("symbolPropPanel").gameObject, symbol.m_baseID);
            }
        }

        private void RefreshSymbolPage()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (form != null)
            {
                Transform transform = form.gameObject.transform.Find(s_symbolPagePanel);
                if (transform != null)
                {
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if (masterRoleInfo != null)
                    {
                        ulong[] pageSymbolData = masterRoleInfo.m_symbolInfo.GetPageSymbolData(this.m_symbolPageIndex);
                        if (pageSymbolData != null)
                        {
                            for (int i = 0; i < 30; i++)
                            {
                                Transform transform2 = transform.Find(string.Format("itemCell{0}", i));
                                if (transform2 != null)
                                {
                                    this.SetSymbolItem(form, transform2.gameObject, i, pageSymbolData[i]);
                                }
                            }
                            Transform transform3 = form.transform.Find(s_symbolEquipPanel);
                            if (transform3 != null)
                            {
                                CUIComponent component = transform3.GetComponent<CUIComponent>();
                                if (component != null)
                                {
                                    component.GetWidget(10).GetComponent<Text>().text = masterRoleInfo.m_symbolInfo.GetSymbolPageMaxLvl(this.m_symbolPageIndex).ToString();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RefreshSymbolProp()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (form != null)
            {
                Transform transform = form.transform.Find(s_symbolEquipPanel);
                if (transform != null)
                {
                    CUIComponent component = transform.GetComponent<CUIComponent>();
                    if (component != null)
                    {
                        CSymbolSystem.RefreshSymbolPageProp(component.GetWidget(2).transform.Find("propList").gameObject, this.m_symbolPageIndex, true);
                        CSymbolSystem.RefreshSymbolPagePveEnhanceProp(component.GetWidget(3).transform.Find("propList").gameObject, this.m_symbolPageIndex);
                    }
                }
            }
        }

        public static void SendBuySymbol(int symbolIndex)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x459);
            msg.stPkgData.stShopBuyReq = new CSPKG_CMD_SHOPBUY();
            msg.stPkgData.stShopBuyReq.iBuyType = 7;
            msg.stPkgData.stShopBuyReq.iBuySubType = symbolIndex;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendChgSymbolPageName(int pageIndex, string pageName)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x461);
            msg.stPkgData.stSymbolNameChgReq.bPageIdx = (byte) pageIndex;
            StringHelper.StringToUTF8Bytes(pageName, ref msg.stPkgData.stSymbolNameChgReq.szPageName);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendReqClearSymbolPage(int pageIndex)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x48a);
            msg.stPkgData.stSymbolPageClrReq.bSymbolPageIdx = (byte) pageIndex;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendWearSymbol(byte page, byte pos, ulong objId)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x46e);
            msg.stPkgData.stSymbolWear.bPage = page;
            msg.stPkgData.stSymbolWear.bPos = pos;
            msg.stPkgData.stSymbolWear.ullUniqueID = objId;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void SetNotWearSymbolListData()
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                DebugHelper.Assert(false, "SetNotWearSymbolListData role is null");
            }
            else
            {
                this.m_pageSymbolBagList.Clear();
                ListView<CSymbolItem> allSymbolList = this.m_symbolSys.GetAllSymbolList();
                int count = allSymbolList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (((allSymbolList[i].m_stackCount > allSymbolList[i].GetPageWearCnt(this.m_symbolPageIndex)) && (allSymbolList[i].m_SymbolData.wLevel <= masterRoleInfo.m_symbolInfo.m_pageMaxLvlLimit)) && (allSymbolList[i].GetPageWearCnt(this.m_symbolPageIndex) < s_maxSameIDSymbolEquipNum))
                    {
                        this.m_pageSymbolBagList.Add(allSymbolList[i]);
                    }
                }
                SortSymbolList(ref this.m_pageSymbolBagList);
            }
        }

        public void SetPageDropListData(CUIFormScript form, int selectIndex)
        {
            GameObject widget = form.GetWidget(1);
            Transform transform = widget.transform.Find("DropList/List");
            transform.gameObject.CustomSetActive(false);
            CUIListScript component = transform.GetComponent<CUIListScript>();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            bool flag = masterRoleInfo.m_symbolInfo.IsPageFull();
            int nextFreePageLevel = masterRoleInfo.m_symbolInfo.GetNextFreePageLevel();
            component.SetElementAmount((nextFreePageLevel <= masterRoleInfo.PvpLevel) ? masterRoleInfo.m_symbolInfo.m_pageCount : (masterRoleInfo.m_symbolInfo.m_pageCount + 1));
            for (int i = 0; i < masterRoleInfo.m_symbolInfo.m_pageCount; i++)
            {
                CUIListElementScript elemenet = component.GetElemenet(i);
                if (elemenet != null)
                {
                    Text text = elemenet.gameObject.transform.Find("Text").GetComponent<Text>();
                    text.text = masterRoleInfo.m_symbolInfo.GetSymbolPageName(i);
                    text.GetComponent<RectTransform>().anchoredPosition = s_symbolPos1;
                    elemenet.gameObject.transform.Find("SymbolLevel/Text").GetComponent<Text>().text = masterRoleInfo.m_symbolInfo.GetSymbolPageMaxLvl(i).ToString();
                    elemenet.gameObject.transform.Find("SymbolLevel").gameObject.CustomSetActive(true);
                }
            }
            if (nextFreePageLevel > masterRoleInfo.PvpLevel)
            {
                CUIListElementScript script3 = component.GetElemenet(masterRoleInfo.m_symbolInfo.m_pageCount);
                if (script3 != null)
                {
                    Text text3 = script3.gameObject.transform.Find("Text").GetComponent<Text>();
                    text3.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Symbol_Free_GetPage"), nextFreePageLevel);
                    text3.GetComponent<RectTransform>().anchoredPosition = s_symbolPos2;
                    script3.gameObject.transform.Find("SymbolLevel").gameObject.CustomSetActive(false);
                }
            }
            component.SelectElement(selectIndex, true);
            if (flag)
            {
                form.GetWidget(2).CustomSetActive(false);
            }
            widget.transform.Find("DropList/Button_Down/Text").GetComponent<Text>().text = masterRoleInfo.m_symbolInfo.GetSymbolPageName(this.m_symbolPageIndex);
            widget.transform.Find("DropList/Button_Down/SymbolLevel/Text").GetComponent<Text>().text = masterRoleInfo.m_symbolInfo.GetSymbolPageMaxLvl(selectIndex).ToString();
        }

        private void SetPageSymbolData(bool bChange)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            ResSymbolPos symbolPos = CSymbolInfo.GetSymbolPos(this.m_selectSymbolPos);
            ListView<CSymbolItem> symbolList = new ListView<CSymbolItem>();
            this.m_pageSymbolBagList.Clear();
            ListView<CSymbolItem> allSymbolList = this.m_symbolSys.GetAllSymbolList();
            int count = allSymbolList.Count;
            for (int i = 0; i < count; i++)
            {
                if (((!bChange || (this.m_curViewSymbol == null)) || (allSymbolList[i].m_SymbolData.dwID != this.m_curViewSymbol.m_SymbolData.dwID)) && (CSymbolInfo.CheckSymbolColor(symbolPos, allSymbolList[i].m_SymbolData.bColor) && (allSymbolList[i].m_stackCount > allSymbolList[i].GetPageWearCnt(this.m_symbolPageIndex))))
                {
                    if ((allSymbolList[i].m_SymbolData.wLevel <= masterRoleInfo.m_symbolInfo.m_pageMaxLvlLimit) && (allSymbolList[i].GetPageWearCnt(this.m_symbolPageIndex) < s_maxSameIDSymbolEquipNum))
                    {
                        this.m_pageSymbolBagList.Add(allSymbolList[i]);
                    }
                    else
                    {
                        symbolList.Add(allSymbolList[i]);
                    }
                }
            }
            SortSymbolList(ref this.m_pageSymbolBagList);
            SortSymbolList(ref symbolList);
            this.m_pageSymbolBagList.AddRange(symbolList);
        }

        private void SetSeletSymbolPos(int newPos)
        {
            this.SetSymbolItemSelect(this.m_selectSymbolPos, false);
            this.m_selectSymbolPos = newPos;
            this.SetSymbolItemSelect(this.m_selectSymbolPos, true);
        }

        private void SetSymbolItem(CUIFormScript formScript, GameObject item, int i, ulong objId)
        {
            if ((Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath) != null) && (item != null))
            {
                CUseable symbolByObjID = Singleton<CSymbolSystem>.GetInstance().GetSymbolByObjID(objId);
                CUIEventScript component = item.GetComponent<CUIEventScript>();
                Transform transform = item.transform;
                GameObject gameObject = transform.Find("bg").gameObject;
                GameObject obj3 = transform.Find("imgLock").gameObject;
                GameObject obj4 = transform.Find("lblOpenLevel").gameObject;
                GameObject obj5 = transform.Find("imgIcon").gameObject;
                GameObject obj6 = transform.Find("imgLevel").gameObject;
                GameObject obj7 = transform.Find("imgCanBuy").gameObject;
                Text text = obj4.transform.Find("Level").GetComponent<Text>();
                gameObject.CustomSetActive(true);
                obj3.CustomSetActive(false);
                obj4.CustomSetActive(false);
                obj5.CustomSetActive(false);
                obj6.CustomSetActive(false);
                obj7.CustomSetActive(false);
                int param = 0;
                enSymbolWearState state = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_symbolInfo.GetSymbolPosWearState(this.m_symbolPageIndex, i, out param);
                stUIEventParams eventParams = new stUIEventParams();
                switch (state)
                {
                    case enSymbolWearState.WearSuccess:
                        if (symbolByObjID != null)
                        {
                            obj5.GetComponent<Image>().SetSprite(symbolByObjID.GetIconPath(), formScript, false, false, false);
                            obj5.CustomSetActive(true);
                            eventParams.symbolParam.symbol = (CSymbolItem) symbolByObjID;
                            eventParams.symbolParam.page = this.m_symbolPageIndex;
                            eventParams.symbolParam.pos = i;
                            component.SetUIEvent(enUIEventType.Click, enUIEventID.Symbol_View, eventParams);
                        }
                        break;

                    case enSymbolWearState.OpenToWear:
                        eventParams.symbolParam.symbol = (CSymbolItem) symbolByObjID;
                        eventParams.symbolParam.page = this.m_symbolPageIndex;
                        eventParams.symbolParam.pos = i;
                        component.SetUIEvent(enUIEventType.Click, enUIEventID.Symbol_BagShow, eventParams);
                        break;

                    case enSymbolWearState.WillOpen:
                        text.text = "Lv." + param.ToString();
                        obj4.CustomSetActive(true);
                        eventParams.tag = param;
                        component.SetUIEvent(enUIEventType.Click, enUIEventID.Symbol_OpenUnlockLvlTip, eventParams);
                        break;

                    case enSymbolWearState.UnOpen:
                        obj3.CustomSetActive(true);
                        gameObject.CustomSetActive(false);
                        eventParams.tag = param;
                        component.SetUIEvent(enUIEventType.Click, enUIEventID.Symbol_OpenUnlockLvlTip, eventParams);
                        break;

                    case enSymbolWearState.CanBuy:
                        gameObject.CustomSetActive(false);
                        obj7.CustomSetActive(true);
                        eventParams.tag = i + 1;
                        component.SetUIEvent(enUIEventType.Click, enUIEventID.Symbol_PromptBuyGrid, eventParams);
                        break;

                    case enSymbolWearState.CanBuyAndWillOpen:
                        gameObject.CustomSetActive(false);
                        text.text = "Lv." + param.ToString();
                        obj4.CustomSetActive(true);
                        eventParams.tag = i + 1;
                        component.SetUIEvent(enUIEventType.Click, enUIEventID.Symbol_PromptBuyGrid, eventParams);
                        break;
                }
            }
        }

        private void SetSymbolItemSelect(int pos, bool isSelect = true)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (((form != null) && (pos >= 0)) && (pos < 30))
            {
                Transform transform2 = form.gameObject.transform.Find(s_symbolPagePanel).Find(string.Format("itemCell{0}/imgSelect", pos));
                if (transform2 != null)
                {
                    transform2.gameObject.CustomSetActive(isSelect);
                }
            }
        }

        public void SetSymbolListItem(CUIFormScript formScript, GameObject itemObj, CSymbolItem item)
        {
            if ((itemObj != null) && (item != null))
            {
                Image component = itemObj.transform.Find("imgIcon").GetComponent<Image>();
                component.SetSprite(item.GetIconPath(), formScript, true, false, false);
                itemObj.transform.Find("SymbolName").GetComponent<Text>().text = item.m_name;
                itemObj.transform.Find("SymbolDesc").GetComponent<Text>().text = CSymbolSystem.GetSymbolAttString(item, true);
                Text text3 = itemObj.transform.Find("lblIconCount").GetComponent<Text>();
                int num = item.m_stackCount - item.GetPageWearCnt(this.m_symbolPageIndex);
                text3.text = string.Format("x{0}", num);
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if ((item.m_SymbolData.wLevel <= masterRoleInfo.m_symbolInfo.m_pageMaxLvlLimit) && (item.GetPageWearCnt(this.m_symbolPageIndex) < s_maxSameIDSymbolEquipNum))
                {
                    component.color = CUIUtility.s_Color_White;
                }
                else
                {
                    component.color = CUIUtility.s_Color_GrayShader;
                }
            }
        }

        private void ShowSymbolBag(bool bChange = false)
        {
            this.SetPageSymbolData(bChange);
            this.RefreshSymbolBag(false);
        }

        private void ShowSymbolView(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                Transform transform = srcFormScript.transform.Find(s_symbolEquipPanel);
                if (transform != null)
                {
                    CUIComponent component = transform.GetComponent<CUIComponent>();
                    if (component != null)
                    {
                        GameObject widget = component.GetWidget(1);
                        Text text = widget.transform.Find("root/lblName").GetComponent<Text>();
                        Image image = widget.transform.Find("root/itemCell0/imgIcon").GetComponent<Image>();
                        CSymbolItem symbol = uiEvent.m_eventParams.symbolParam.symbol;
                        this.m_curViewSymbol = symbol;
                        text.text = symbol.m_name;
                        image.SetSprite(symbol.GetIconPath(), uiEvent.m_srcFormScript, true, false, false);
                        CSymbolSystem.RefreshSymbolPropContent(widget.transform.Find("root/symbolPropPanel").gameObject, symbol.m_baseID);
                    }
                }
            }
        }

        public static void SortSymbolList(ref ListView<CSymbolItem> symbolList)
        {
            int count = symbolList.Count;
            CSymbolItem item = null;
            for (int i = 0; i < (count - 1); i++)
            {
                for (int j = 0; j < ((count - 1) - i); j++)
                {
                    if ((symbolList[j].m_SymbolData.wLevel < symbolList[j + 1].m_SymbolData.wLevel) || ((symbolList[j].m_SymbolData.wLevel == symbolList[j + 1].m_SymbolData.wLevel) && (symbolList[j].m_SymbolData.bColor > symbolList[j + 1].m_SymbolData.bColor)))
                    {
                        item = symbolList[j];
                        symbolList[j] = symbolList[j + 1];
                        symbolList[j + 1] = item;
                    }
                }
            }
        }

        public void SwitchToSymbolWearPanel(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (form != null)
            {
                Transform transform = form.transform.Find(s_symbolEquipPanel);
                if (transform != null)
                {
                    CUIComponent component = transform.GetComponent<CUIComponent>();
                    if (component != null)
                    {
                        this.SetSymbolItemSelect(this.m_selectSymbolPos, false);
                        this.ClearSymbolEquipData();
                        this.RefreshSymbolEquipPanel();
                        this.RefreshPageDropList();
                        CTextManager instance = Singleton<CTextManager>.GetInstance();
                        AddTip(component.GetWidget(2).transform.Find("titlePanel").gameObject, instance.GetText("Symbol_PvpProp_Desc"), enUseableTipsPos.enLeft);
                        AddTip(component.GetWidget(3).transform.Find("titlePanel").gameObject, instance.GetText("Symbol_EnhancePveProp_Desc"), enUseableTipsPos.enLeft);
                    }
                }
            }
        }

        public void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_Off, new CUIEventManager.OnUIEventHandler(this.OnOffSymbol));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_View, new CUIEventManager.OnUIEventHandler(this.OnSymbolView));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BagItemEnable, new CUIEventManager.OnUIEventHandler(this.OnSymbolBagElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BagItemClick, new CUIEventManager.OnUIEventHandler(this.OnSymbolBagItemClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BagViewHide, new CUIEventManager.OnUIEventHandler(this.OnSymbolBagSymbolViewHide));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BagShow, new CUIEventManager.OnUIEventHandler(this.OnSymbolBagShow));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_View_NotWear_Item, new CUIEventManager.OnUIEventHandler(this.OnViewNotWearItem));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_ChangeSymbolClick, new CUIEventManager.OnUIEventHandler(this.OnChangeSymbolClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_ChangeConfirm, new CUIEventManager.OnUIEventHandler(this.OnSymbolChangeConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_PageClear, new CUIEventManager.OnUIEventHandler(this.OnSymbolPageClear));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_PageClearConfirm, new CUIEventManager.OnUIEventHandler(this.OnSymbolPageClearConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_OpenUnlockLvlTip, new CUIEventManager.OnUIEventHandler(this.OnOpenUnlockLvlTip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_PromptBuyGrid, new CUIEventManager.OnUIEventHandler(this.OnPromptBuyGrid));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_ConfirmBuyGrid, new CUIEventManager.OnUIEventHandler(this.OnConfirmBuyGrid));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_ConfirmWhenMoneyNotEnough, new CUIEventManager.OnUIEventHandler(this.ConfirmWhenMoneyNotEnough));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_SymbolPageDownBtnClick, new CUIEventManager.OnUIEventHandler(this.OnSymbolPageDownBtnClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_PageItemSelect, new CUIEventManager.OnUIEventHandler(this.OnSymbolPageItemSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_ChangePageName, new CUIEventManager.OnUIEventHandler(this.OnChangeSymbolPageName));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_PageAddClick, new CUIEventManager.OnUIEventHandler(this.OnSymbol_PageAddClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_ConfirmChgPageName, new CUIEventManager.OnUIEventHandler(this.OnConfirmChgPageName));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Purchase_BuySymbolPage, new CUIEventManager.OnUIEventHandler(this.OnBuySymbolPage));
            Singleton<EventRouter>.instance.RemoveEventHandler("MasterPvpLevelChanged", new System.Action(this.RefreshPageDropList));
        }
    }
}

