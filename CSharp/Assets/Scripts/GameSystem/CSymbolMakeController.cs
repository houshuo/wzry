namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CSymbolMakeController
    {
        private int m_breakDetailIndex;
        private ushort m_breakLevelMask;
        private ListView<CBreakSymbolItem>[] m_breakSymbolList = new ListView<CBreakSymbolItem>[s_maxSymbolBreakLevel];
        private ResSymbolInfo m_curTransformSymbol;
        private ListView<CSDT_SYMBOLOPT_INFO> m_svrBreakSymbolList = new ListView<CSDT_SYMBOLOPT_INFO>();
        private int m_symbolFilterLevel = 1;
        private enSymbolType m_symbolFilterType;
        private ListView<ResSymbolInfo> m_symbolMakeList = new ListView<ResSymbolInfo>();
        private CSymbolSystem m_symbolSys;
        private static ListView<ResSymbolInfo> s_allSymbolCfgList = new ListView<ResSymbolInfo>();
        private static int s_breakSymbolCoinCnt = 0;
        public static int s_maxSymbolBreakLevel = 4;
        public static string s_symbolBreakDetailPath = "UGUI/Form/System/Symbol/Form_SymbolBreakDetail.prefab";
        public static string s_symbolBreakPath = "UGUI/Form/System/Symbol/Form_SymbolBreak.prefab";
        public static string s_symbolTransformPath = "UGUI/Form/System/Symbol/Form_SymbolTransform.prefab";

        private bool CheckSymbolBreak(CSymbolItem symbol, ushort breakLvlMask)
        {
            return (((symbol != null) && (symbol.m_SymbolData.wLevel < CSymbolInfo.s_maxSymbolLevel)) && ((symbol.m_stackCount > symbol.GetMaxWearCnt()) && (((((int) 1) << symbol.m_SymbolData.wLevel) & breakLvlMask) != 0)));
        }

        public void Clear()
        {
            this.ClearSymbolMakeData();
        }

        private void ClearBreakSymbolListData(int index)
        {
            if ((index >= 0) && (index < this.m_breakSymbolList.Length))
            {
                this.m_breakSymbolList[index].Clear();
            }
        }

        private void ClearSymbolMakeData()
        {
            this.m_symbolFilterLevel = 1;
            this.m_symbolFilterType = enSymbolType.All;
        }

        private int GetBreakExcessSymbolCoinCnt(ushort breakLvlMask = 0xffff)
        {
            int num = 0;
            ListView<CSymbolItem> allSymbolList = this.m_symbolSys.GetAllSymbolList();
            for (int i = 0; i < allSymbolList.Count; i++)
            {
                if (this.CheckSymbolBreak(allSymbolList[i], breakLvlMask))
                {
                    num += (int) ((allSymbolList[i].m_stackCount - allSymbolList[i].GetMaxWearCnt()) * allSymbolList[i].m_SymbolData.dwBreakCoin);
                }
            }
            return num;
        }

        private int GetSelectBreakSymbolCoinCnt()
        {
            int num = 0;
            for (int i = 0; i < this.m_breakSymbolList.Length; i++)
            {
                if (this.m_breakSymbolList[i] != null)
                {
                    for (int j = 0; j < this.m_breakSymbolList[i].Count; j++)
                    {
                        if ((this.m_breakSymbolList[i][j].symbol != null) && this.m_breakSymbolList[i][j].bBreak)
                        {
                            num += (int) ((this.m_breakSymbolList[i][j].symbol.m_stackCount - this.m_breakSymbolList[i][j].symbol.GetMaxWearCnt()) * this.m_breakSymbolList[i][j].symbol.m_SymbolData.dwBreakCoin);
                        }
                    }
                }
            }
            return num;
        }

        private CSymbolItem GetSymbolByCfgID(uint cfgId)
        {
            ListView<CSymbolItem> allSymbolList = this.m_symbolSys.GetAllSymbolList();
            for (int i = 0; i < allSymbolList.Count; i++)
            {
                CSymbolItem item2 = allSymbolList[i];
                if ((item2 != null) && (item2.m_baseID == cfgId))
                {
                    return item2;
                }
            }
            return null;
        }

        public void Init(CSymbolSystem symbolSys)
        {
            this.m_symbolSys = symbolSys;
            this.InitSymbolCfgList();
            this.InitBreakSymbolList();
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_TypeMenuSelect, new CUIEventManager.OnUIEventHandler(this.OnSymbolTypeMenuSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_LevelMenuSelect, new CUIEventManager.OnUIEventHandler(this.OnSymbolLevelMenuSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_ListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnSymbolMakeListEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_ListItemClick, new CUIEventManager.OnUIEventHandler(this.OnSymbolMakeListClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnItemMakeClick, new CUIEventManager.OnUIEventHandler(this.OnMakeSymbolClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnItemMakeConfirm, new CUIEventManager.OnUIEventHandler(this.OnItemMakeConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnItemBreakClick, new CUIEventManager.OnUIEventHandler(this.OnBreakSymbolClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnItemBreakConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakSymbolConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnBreakExcessSymbolClick, new CUIEventManager.OnUIEventHandler(this.OnBreakExcessSymbolClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_SelectBreakLvlItem, new CUIEventManager.OnUIEventHandler(this.OnSelectBreakLvlItem));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_OnBreakExcessSymbolConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakExcessSymbolClickConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_ItemBreakAnimatorEnd, new CUIEventManager.OnUIEventHandler(this.OnItemBreakAnimatorEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BreakItemEnable, new CUIEventManager.OnUIEventHandler(this.OnBreakListItemEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BreakListItemSelToggle, new CUIEventManager.OnUIEventHandler(this.OnBreakListItemSelToggle));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_OpenBreakDetailForm, new CUIEventManager.OnUIEventHandler(this.OnOpenBreakDetailForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BreakDetailFormConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakDetailFormConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_BreakDetailFormCancle, new CUIEventManager.OnUIEventHandler(this.OnBreakDetailFormCancle));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolMake_CoinNotEnoughGotoSymbolMall, new CUIEventManager.OnUIEventHandler(this.OnCoinNotEnoughGotoSymbolMall));
        }

        private void InitBreakSymbolList()
        {
            for (int i = 0; i < this.m_breakSymbolList.Length; i++)
            {
                if (this.m_breakSymbolList[i] == null)
                {
                    this.m_breakSymbolList[i] = new ListView<CBreakSymbolItem>();
                }
                this.m_breakSymbolList[i].Clear();
            }
        }

        private void InitSymbolCfgList()
        {
            s_allSymbolCfgList.Clear();
            Dictionary<long, object>.Enumerator enumerator = GameDataMgr.symbolInfoDatabin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<long, object> current = enumerator.Current;
                ResSymbolInfo item = (ResSymbolInfo) current.Value;
                if ((item != null) && (item.bIsMakeShow > 0))
                {
                    s_allSymbolCfgList.Add(item);
                }
            }
        }

        private bool IsAllSymbolBreak(int index)
        {
            if ((index >= 0) && (index < this.m_breakSymbolList.Length))
            {
                for (int i = 0; i < this.m_breakSymbolList[index].Count; i++)
                {
                    if (!this.m_breakSymbolList[index][i].bBreak)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void OnBreakDetailFormCancle(CUIEvent uiEvent)
        {
            if (((this.m_breakDetailIndex >= 0) && (this.m_breakDetailIndex < this.m_breakSymbolList.Length)) && (this.m_breakSymbolList[this.m_breakDetailIndex] != null))
            {
                for (int i = 0; i < this.m_breakSymbolList[this.m_breakDetailIndex].Count; i++)
                {
                    if (this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreakToggle != this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreak)
                    {
                        this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreakToggle = this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreak;
                    }
                }
            }
            Singleton<CUIManager>.GetInstance().CloseForm(s_symbolBreakDetailPath);
            this.RefreshSymbolBreakForm();
        }

        private void OnBreakDetailFormConfirm(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_symbolBreakDetailPath);
            if (null != form)
            {
                if (((this.m_breakDetailIndex >= 0) && (this.m_breakDetailIndex < this.m_breakSymbolList.Length)) && (this.m_breakSymbolList[this.m_breakDetailIndex] != null))
                {
                    for (int i = 0; i < this.m_breakSymbolList[this.m_breakDetailIndex].Count; i++)
                    {
                        if (this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreak != this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreakToggle)
                        {
                            this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreak = this.m_breakSymbolList[this.m_breakDetailIndex][i].bBreakToggle;
                        }
                    }
                }
                Singleton<CUIManager>.GetInstance().CloseForm(s_symbolBreakDetailPath);
                this.RefreshSymbolBreakForm();
            }
        }

        private void OnBreakExcessSymbolClick(CUIEvent uiEvent)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_symbolBreakPath, false, true);
            if (script != null)
            {
                for (int i = 0; i < this.m_breakSymbolList.Length; i++)
                {
                    this.m_breakSymbolList[i].Clear();
                }
                this.m_breakLevelMask = 0;
                Transform transform = script.transform.Find("Panel_SymbolBreak/Panel_Content");
                for (int j = 0; j < s_maxSymbolBreakLevel; j++)
                {
                    GameObject gameObject = transform.Find(string.Format("breakElement{0}", j)).gameObject;
                    Transform transform2 = gameObject.transform.Find("OnBreakBtn");
                    Transform transform3 = gameObject.transform.Find("OnBreakBtn/Checkmark");
                    Transform transform4 = gameObject.transform.Find("detailButton");
                    if ((transform2 != null) && (transform3 != null))
                    {
                        transform3.gameObject.CustomSetActive(false);
                        CUIEventScript component = transform2.GetComponent<CUIEventScript>();
                        if (component != null)
                        {
                            stUIEventParams eventParams = new stUIEventParams {
                                tag = j
                            };
                            component.SetUIEvent(enUIEventType.Click, enUIEventID.SymbolMake_SelectBreakLvlItem, eventParams);
                        }
                    }
                    if (transform4 != null)
                    {
                        transform4.gameObject.CustomSetActive(false);
                        CUIEventScript script3 = transform4.GetComponent<CUIEventScript>();
                        if (script3 != null)
                        {
                            stUIEventParams params2 = new stUIEventParams {
                                tag = j
                            };
                            script3.SetUIEvent(enUIEventType.Click, enUIEventID.Symbol_OpenBreakDetailForm, params2);
                        }
                    }
                }
                this.RefreshSymbolBreakForm();
            }
        }

        private void OnBreakExcessSymbolClickConfirm(CUIEvent uiEvent)
        {
            if (this.m_breakLevelMask == 0)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("Symbol_Select_BreakLvl_Tip", true, 1.5f, null, new object[0]);
            }
            else
            {
                this.SetBreakSymbolList();
                if (this.m_svrBreakSymbolList.Count > 0)
                {
                    SendReqExcessSymbolBreak(this.m_svrBreakSymbolList);
                }
                else
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Symbol_No_More_Item_Break", true, 1.5f, null, new object[0]);
                }
            }
        }

        private void OnBreakListItemEnable(CUIEvent uiEvent)
        {
            if (((this.m_breakDetailIndex >= 0) && (this.m_breakDetailIndex < this.m_breakSymbolList.Length)) && ((this.m_breakSymbolList[this.m_breakDetailIndex] != null) && (null != uiEvent.m_srcWidget)))
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_breakSymbolList[this.m_breakDetailIndex].Count))
                {
                    CBreakSymbolItem item = this.m_breakSymbolList[this.m_breakDetailIndex][srcWidgetIndexInBelongedList];
                    if ((item != null) && (item.symbol != null))
                    {
                        Transform transform = uiEvent.m_srcWidget.transform.Find("itemCell");
                        this.m_symbolSys.m_symbolWearCtrl.SetSymbolListItem(uiEvent.m_srcFormScript, transform.gameObject, item.symbol);
                        Transform transform2 = transform.Find("selectFlag");
                        if (transform2 != null)
                        {
                            CUIEventScript component = transform2.GetComponent<CUIEventScript>();
                            if (component != null)
                            {
                                stUIEventParams eventParams = new stUIEventParams {
                                    tag = srcWidgetIndexInBelongedList
                                };
                                component.SetUIEvent(enUIEventType.Click, enUIEventID.Symbol_BreakListItemSelToggle, eventParams);
                            }
                            transform2.GetComponent<Toggle>().isOn = item.bBreakToggle;
                        }
                    }
                }
            }
        }

        private void OnBreakListItemSelToggle(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            if ((tag >= 0) && (tag < this.m_breakSymbolList[this.m_breakDetailIndex].Count))
            {
                CBreakSymbolItem item = this.m_breakSymbolList[this.m_breakDetailIndex][tag];
                if (((item != null) && (item.symbol != null)) && (uiEvent.m_srcWidget != null))
                {
                    Toggle component = uiEvent.m_srcWidget.GetComponent<Toggle>();
                    item.bBreakToggle = component.isOn;
                }
            }
        }

        private void OnBreakSymbolClick(CUIEvent uiEvent)
        {
            if (this.m_curTransformSymbol != null)
            {
                CSymbolItem symbolByCfgID = this.GetSymbolByCfgID(this.m_curTransformSymbol.dwID);
                if (symbolByCfgID == null)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Symbol__Item_Not_Exist_Tip", true, 1.5f, null, new object[0]);
                }
                else if (symbolByCfgID.m_stackCount > symbolByCfgID.GetMaxWearCnt())
                {
                    string text = Singleton<CTextManager>.GetInstance().GetText("Symbol_Break_Tip");
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.SymbolMake_OnItemBreakConfirm, enUIEventID.None, false);
                }
                else
                {
                    CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                    if (masterRoleInfo != null)
                    {
                        string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("Symbol_Break_Item_Wear_Tip"), masterRoleInfo.m_symbolInfo.GetMaxWearSymbolPageName(symbolByCfgID));
                        Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.SymbolMake_OnItemBreakConfirm, enUIEventID.None, false);
                    }
                }
            }
        }

        private void OnBreakSymbolConfirm(CUIEvent uiEvent)
        {
            if (this.m_curTransformSymbol != null)
            {
                CSymbolItem symbolByCfgID = this.GetSymbolByCfgID(this.m_curTransformSymbol.dwID);
                if (symbolByCfgID != null)
                {
                    this.OnSendReqSymbolBreak(symbolByCfgID.m_baseID, 1);
                }
            }
        }

        private void OnCoinNotEnoughGotoSymbolMall(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_symbolTransformPath);
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_GoToSymbolTab);
        }

        private void OnItemBreakAnimatorEnd(CUIEvent uiEvent)
        {
            if (s_breakSymbolCoinCnt > 0)
            {
                CUseable useable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSymbolCoin, s_breakSymbolCoinCnt);
                if (useable != null)
                {
                    CUseable[] items = new CUseable[] { useable };
                    Singleton<CUIManager>.GetInstance().OpenAwardTip(items, null, false, enUIEventID.None, false, false, "Form_Award");
                }
                s_breakSymbolCoinCnt = 0;
            }
        }

        private void OnItemMakeConfirm(CUIEvent uiEvent)
        {
            if (this.m_curTransformSymbol != null)
            {
                this.OnSendReqSymbolMake(this.m_curTransformSymbol.dwID, 1);
            }
        }

        private void OnMakeSymbolClick(CUIEvent uiEvent)
        {
            if (this.m_curTransformSymbol != null)
            {
                if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().SymbolCoin >= this.m_curTransformSymbol.dwMakeCoin)
                {
                    CSymbolItem symbolByCfgID = this.GetSymbolByCfgID(this.m_curTransformSymbol.dwID);
                    if (symbolByCfgID != null)
                    {
                        if (symbolByCfgID.m_stackCount >= symbolByCfgID.m_SymbolData.iOverLimit)
                        {
                            Singleton<CUIManager>.GetInstance().OpenTips("Symbol_Make_MaxCnt_Limit", true, 1.5f, null, new object[0]);
                        }
                        else if (symbolByCfgID.m_stackCount >= CSymbolWearController.s_maxSameIDSymbolEquipNum)
                        {
                            string text = Singleton<CTextManager>.GetInstance().GetText("Symbol_Make_WearMaxLimit_Tip");
                            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.SymbolMake_OnItemMakeConfirm, enUIEventID.None, false);
                        }
                        else
                        {
                            this.OnSendReqSymbolMake(this.m_curTransformSymbol.dwID, 1);
                        }
                    }
                    else
                    {
                        this.OnSendReqSymbolMake(this.m_curTransformSymbol.dwID, 1);
                    }
                }
                else if (CSysDynamicBlock.bLobbyEntryBlocked)
                {
                    Singleton<CUIManager>.GetInstance().OpenTips("Symbol_DynamicBlock_Coin_Not_Enough_Tip", true, 1.5f, null, new object[0]);
                }
                else
                {
                    string strContent = Singleton<CTextManager>.GetInstance().GetText("Symbol_Coin_Not_Enough_Tip");
                    Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.SymbolMake_CoinNotEnoughGotoSymbolMall, enUIEventID.None, false);
                }
            }
        }

        private void OnOpenBreakDetailForm(CUIEvent uiEvent)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_symbolBreakDetailPath, false, true);
            if (null != script)
            {
                int tag = uiEvent.m_eventParams.tag;
                Transform transform = script.transform.Find("Panel_SymbolBreak/Panel_Content/List");
                if (transform != null)
                {
                    CUIListScript component = transform.GetComponent<CUIListScript>();
                    if (((tag >= 0) && (tag < this.m_breakSymbolList.Length)) && (this.m_breakSymbolList[tag] != null))
                    {
                        this.m_breakDetailIndex = tag;
                        this.m_breakSymbolList[tag].Sort();
                        component.SetElementAmount(this.m_breakSymbolList[tag].Count);
                    }
                }
            }
        }

        private void OnReceiveBreakItemList()
        {
            for (int i = 0; i < this.m_breakSymbolList.Length; i++)
            {
                int index = 0;
                while (index < this.m_breakSymbolList[i].Count)
                {
                    if (this.m_breakSymbolList[i][index].bBreak)
                    {
                        this.m_breakSymbolList[i].RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }
        }

        private void OnSelectBreakLvlItem(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_symbolBreakPath, false, true);
            if (script != null)
            {
                Transform transform = script.transform.Find(string.Format("Panel_SymbolBreak/Panel_Content/breakElement{0}", tag));
                if (transform != null)
                {
                    Transform transform2 = transform.transform.Find("OnBreakBtn/Checkmark");
                    Transform transform3 = transform.transform.Find("detailButton");
                    if (transform2 != null)
                    {
                        transform2.gameObject.CustomSetActive(!transform2.gameObject.activeSelf);
                        this.ClearBreakSymbolListData(tag);
                        if (transform2.gameObject.activeSelf)
                        {
                            this.SetBreakSymbolListData(tag);
                        }
                        if (transform3 != null)
                        {
                            transform3.gameObject.CustomSetActive(transform2.gameObject.activeSelf);
                        }
                    }
                }
                this.RefreshSymbolBreakForm();
            }
        }

        private void OnSendReqSymbolBreak(uint symbolId, int count = 1)
        {
            if (this.m_symbolSys.m_selectMenuType == enSymbolMenuType.SymbolRecommend)
            {
                SendReqSymbolBreak(this.m_symbolSys.m_symbolRcmdCtrl.m_curHeroId, symbolId, count);
            }
            else
            {
                SendReqSymbolBreak(0, symbolId, count);
            }
        }

        private void OnSendReqSymbolMake(uint symbolId, int count = 1)
        {
            if (this.m_symbolSys.m_selectMenuType == enSymbolMenuType.SymbolRecommend)
            {
                SendReqSymbolMake(this.m_symbolSys.m_symbolRcmdCtrl.m_curHeroId, symbolId, count);
            }
            else
            {
                SendReqSymbolMake(0, symbolId, count);
            }
        }

        private void OnSymbolLevelMenuSelect(CUIEvent uiEvent)
        {
            CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            this.m_symbolFilterLevel = component.GetSelectedIndex() + 1;
            this.SetSymbolMakeListData();
            this.RefreshSymbolMakeList(uiEvent.m_srcFormScript);
        }

        private void OnSymbolMakeListClick(CUIEvent uiEvent)
        {
            this.m_curTransformSymbol = uiEvent.m_eventParams.symbolTransParam.symbolCfgInfo;
            Singleton<CUIManager>.GetInstance().OpenForm(s_symbolTransformPath, false, true);
            this.RefreshSymbolTransformForm();
        }

        private void OnSymbolMakeListEnable(CUIEvent uiEvent)
        {
            if ((uiEvent.m_srcWidgetIndexInBelongedList < 0) || (uiEvent.m_srcWidgetIndexInBelongedList >= this.m_symbolMakeList.Count))
            {
                DebugHelper.Assert(false, "OnSymbolMakeListEnable index out of range");
            }
            else
            {
                RefreshSymbolItem(this.m_symbolMakeList[uiEvent.m_srcWidgetIndexInBelongedList], uiEvent.m_srcWidget, uiEvent.m_srcFormScript);
            }
        }

        private void OnSymbolTypeMenuSelect(CUIEvent uiEvent)
        {
            int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
            this.m_symbolFilterType = (enSymbolType) selectedIndex;
            this.SetSymbolMakeListData();
            this.RefreshSymbolMakeList(uiEvent.m_srcFormScript);
        }

        private void PlayBatchBreakAnimator()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_symbolBreakPath);
            if (null != form)
            {
                Transform transform = form.transform.Find("Panel_SymbolBreak/Panel_Content");
                for (int i = 0; i < s_maxSymbolBreakLevel; i++)
                {
                    GameObject gameObject = transform.Find(string.Format("breakElement{0}", i)).gameObject;
                    Transform transform2 = gameObject.transform.Find("OnBreakBtn/Checkmark");
                    if ((transform2 != null) && transform2.gameObject.activeSelf)
                    {
                        CUICommonSystem.PlayAnimator(gameObject.transform.Find("Img_Lv").gameObject, "FenjieAnimation");
                    }
                }
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_fuwen_fenjie_piliang", null);
            }
        }

        private void PlaySingleBreakAnimator()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_symbolTransformPath);
            if (null != form)
            {
                CUICommonSystem.PlayAnimator(form.transform.Find("Panel_SymbolTranform/Panel_Content/iconImage").gameObject, "FenjieAnimation");
                Singleton<CSoundManager>.GetInstance().PostEvent("UI_fuwen_fenjie_dange", null);
            }
        }

        [MessageHandler(0x486)]
        public static void ReciveSymbolBreakRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_SYMBOL_BREAK stSymbolBreakRsp = msg.stPkgData.stSymbolBreakRsp;
            int num = 0;
            for (int i = 0; i < stSymbolBreakRsp.wSymbolCnt; i++)
            {
                ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(stSymbolBreakRsp.astSymbolList[i].dwSymbolID);
                if (dataByKey != null)
                {
                    num += (int) (dataByKey.dwBreakCoin * stSymbolBreakRsp.astSymbolList[i].iSymbolCnt);
                }
            }
            s_breakSymbolCoinCnt = num;
            if (num > 0)
            {
                if (stSymbolBreakRsp.bBreakType == 0)
                {
                    Singleton<CSymbolSystem>.GetInstance().m_symbolMakeCtrl.PlaySingleBreakAnimator();
                    Singleton<CSymbolSystem>.GetInstance().m_symbolMakeCtrl.RefreshSymbolTransformForm();
                }
                else if (stSymbolBreakRsp.bBreakType == 1)
                {
                    Singleton<CSymbolSystem>.GetInstance().m_symbolMakeCtrl.PlayBatchBreakAnimator();
                    Singleton<CSymbolSystem>.GetInstance().m_symbolMakeCtrl.OnReceiveBreakItemList();
                    Singleton<CSymbolSystem>.GetInstance().m_symbolMakeCtrl.RefreshSymbolBreakForm();
                }
            }
        }

        [MessageHandler(0x485)]
        public static void ReciveSymbolMakeRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_SYMBOL_MAKE stSymbolMakeRsp = msg.stPkgData.stSymbolMakeRsp;
            if (stSymbolMakeRsp.iResult == 0)
            {
                CUseable useable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, stSymbolMakeRsp.stSymbolInfo.dwSymbolID, stSymbolMakeRsp.stSymbolInfo.iSymbolCnt);
                if ((useable != null) && (((CSymbolItem) useable) != null))
                {
                    CUseableContainer container = new CUseableContainer(enCONTAINER_TYPE.ITEM);
                    container.Add(useable);
                    CUICommonSystem.ShowSymbol(container, enUIEventID.None);
                }
                Singleton<CSymbolSystem>.GetInstance().m_symbolMakeCtrl.RefreshSymbolTransformForm();
            }
        }

        private void RefreshSymbolBreakForm()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_symbolBreakPath);
            if (null != form)
            {
                Transform transform = form.transform.Find("Panel_SymbolBreak/Panel_Content");
                int num = 0;
                for (int i = 0; i < s_maxSymbolBreakLevel; i++)
                {
                    GameObject gameObject = transform.Find(string.Format("breakElement{0}", i)).gameObject;
                    Transform transform2 = gameObject.transform.Find("OnBreakBtn/Checkmark");
                    Transform transform3 = gameObject.transform.Find("OnBreakBtn/Text");
                    if ((transform2 != null) && transform2.gameObject.activeSelf)
                    {
                        num |= ((int) 1) << (i + 1);
                    }
                    if (transform3 != null)
                    {
                        if (this.IsAllSymbolBreak(i))
                        {
                            transform3.GetComponent<Text>().text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Symbol_BreakAllItem"), i + 1);
                        }
                        else
                        {
                            transform3.GetComponent<Text>().text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Symbol_BreakSomeItem"), i + 1);
                        }
                    }
                }
                this.m_breakLevelMask = (ushort) num;
                form.transform.Find("Panel_SymbolBreak/Panel_Bottom/Pl_countText/symbolCoinCntText").GetComponent<Text>().text = this.GetSelectBreakSymbolCoinCnt().ToString();
            }
        }

        public static void RefreshSymbolItem(ResSymbolInfo symbolInfo, GameObject widget, CUIFormScript form)
        {
            if ((symbolInfo != null) && (widget != null))
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    CUseable useableByBaseID = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM).GetUseableByBaseID(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, symbolInfo.dwID);
                    Image component = widget.transform.Find("iconImage").GetComponent<Image>();
                    Text text = widget.transform.Find("countText").GetComponent<Text>();
                    Text text2 = widget.transform.Find("nameText").GetComponent<Text>();
                    Text text3 = widget.transform.Find("descText").GetComponent<Text>();
                    component.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, symbolInfo.dwIcon), form, true, false, false);
                    text.text = (useableByBaseID == null) ? string.Empty : useableByBaseID.m_stackCount.ToString();
                    text2.text = symbolInfo.szName;
                    text3.text = CSymbolSystem.GetSymbolAttString(symbolInfo.dwID, true);
                    CUIEventScript script = widget.GetComponent<CUIEventScript>();
                    if (script != null)
                    {
                        stUIEventParams eventParams = new stUIEventParams();
                        eventParams.symbolTransParam.symbolCfgInfo = symbolInfo;
                        script.SetUIEvent(enUIEventType.Click, enUIEventID.SymbolMake_ListItemClick, eventParams);
                    }
                    if (useableByBaseID != null)
                    {
                        CUICommonSystem.PlayAnimator(widget, "Symbol_Normal");
                    }
                    else
                    {
                        CUICommonSystem.PlayAnimator(widget, "Symbol_Disabled");
                    }
                }
            }
        }

        public void RefreshSymbolMakeForm()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (form != null)
            {
                CTextManager instance = Singleton<CTextManager>.GetInstance();
                string[] titleList = new string[] { instance.GetText("Symbol_Prop_Tab_All"), instance.GetText("Symbol_Prop_Tab_Atk"), instance.GetText("Symbol_Prop_Tab_Hp"), instance.GetText("Symbol_Prop_Tab_Defense"), instance.GetText("Symbol_Prop_Tab_Function"), instance.GetText("Symbol_Prop_Tab_HpSteal"), instance.GetText("Symbol_Prop_Tab_AtkSpeed"), instance.GetText("Symbol_Prop_Tab_Crit"), instance.GetText("Symbol_Prop_Tab_Penetrate") };
                CUICommonSystem.InitMenuPanel(form.GetWidget(4), titleList, (int) this.m_symbolFilterType);
                string[] strArray2 = new string[] { "1", "2", "3", "4", "5" };
                CUICommonSystem.InitMenuPanel(form.GetWidget(5), strArray2, this.m_symbolFilterLevel - 1);
                this.m_symbolSys.SetSymbolData();
                this.SetSymbolMakeListData();
                this.RefreshSymbolMakeList(form);
                form.GetWidget(7).GetComponent<Text>().text = this.GetBreakExcessSymbolCoinCnt(0xffff).ToString();
            }
        }

        private void RefreshSymbolMakeList(CUIFormScript form)
        {
            if (form != null)
            {
                form.GetWidget(6).GetComponent<CUIListScript>().SetElementAmount(this.m_symbolMakeList.Count);
            }
        }

        private void RefreshSymbolTransformForm()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_symbolTransformPath);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (((form != null) && (this.m_curTransformSymbol != null)) && (masterRoleInfo != null))
            {
                GameObject gameObject = form.transform.Find("Panel_SymbolTranform/Panel_Content").gameObject;
                gameObject.transform.Find("iconImage").GetComponent<Image>().SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, this.m_curTransformSymbol.dwIcon), form, true, false, false);
                gameObject.transform.Find("nameText").GetComponent<Text>().text = StringHelper.UTF8BytesToString(ref this.m_curTransformSymbol.szName);
                Text component = gameObject.transform.Find("countText").GetComponent<Text>();
                component.text = string.Empty;
                int useableStackCount = 0;
                CUseableContainer useableContainer = masterRoleInfo.GetUseableContainer(enCONTAINER_TYPE.ITEM);
                if (useableContainer != null)
                {
                    useableStackCount = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, this.m_curTransformSymbol.dwID);
                    CTextManager instance = Singleton<CTextManager>.GetInstance();
                    component.text = (useableStackCount <= 0) ? instance.GetText("Symbol_Not_Own") : string.Format(instance.GetText("Symbol_Own_Cnt"), useableStackCount);
                }
                CSymbolSystem.RefreshSymbolPropContent(gameObject.transform.Find("symbolPropPanel").gameObject, this.m_curTransformSymbol.dwID);
                gameObject.transform.Find("makeCoinText").GetComponent<Text>().text = this.m_curTransformSymbol.dwMakeCoin.ToString();
                gameObject.transform.Find("breakCoinText").GetComponent<Text>().text = this.m_curTransformSymbol.dwBreakCoin.ToString();
                GameObject obj4 = gameObject.transform.Find("btnBreak").gameObject;
                obj4.GetComponent<Button>().interactable = useableStackCount > 0;
                obj4.GetComponent<CUIEventScript>().enabled = useableStackCount > 0;
            }
        }

        public static void SendReqExcessSymbolBreak(ListView<CSDT_SYMBOLOPT_INFO> breakSymbolList)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x484);
            msg.stPkgData.stSymbolBreak.wSymbolCnt = (ushort) breakSymbolList.Count;
            msg.stPkgData.stSymbolBreak.bBreakType = 1;
            for (int i = 0; (i < breakSymbolList.Count) && (i < 400); i++)
            {
                msg.stPkgData.stSymbolBreak.astSymbolList[i] = breakSymbolList[i];
            }
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendReqSymbolBreak(uint heroId, uint symbolId, int cnt)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x484);
            msg.stPkgData.stSymbolBreak.dwBelongHeroID = heroId;
            msg.stPkgData.stSymbolBreak.wSymbolCnt = 1;
            msg.stPkgData.stSymbolBreak.bBreakType = 0;
            CSDT_SYMBOLOPT_INFO csdt_symbolopt_info = new CSDT_SYMBOLOPT_INFO {
                dwSymbolID = symbolId,
                iSymbolCnt = cnt
            };
            msg.stPkgData.stSymbolBreak.astSymbolList[0] = csdt_symbolopt_info;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public static void SendReqSymbolMake(uint heroId, uint symbolId, int cnt)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x483);
            msg.stPkgData.stSymbolMake.dwBelongHeroID = heroId;
            msg.stPkgData.stSymbolMake.stSymbolInfo.dwSymbolID = symbolId;
            msg.stPkgData.stSymbolMake.stSymbolInfo.iSymbolCnt = cnt;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        private void SetBreakSymbolList()
        {
            this.m_svrBreakSymbolList.Clear();
            int length = this.m_breakSymbolList.Length;
            for (int i = 0; i < length; i++)
            {
                if (this.m_breakSymbolList[i] != null)
                {
                    for (int j = 0; j < this.m_breakSymbolList[i].Count; j++)
                    {
                        if ((this.m_breakSymbolList[i][j].symbol != null) && this.m_breakSymbolList[i][j].bBreak)
                        {
                            CSDT_SYMBOLOPT_INFO item = new CSDT_SYMBOLOPT_INFO {
                                dwSymbolID = this.m_breakSymbolList[i][j].symbol.m_baseID,
                                iSymbolCnt = this.m_breakSymbolList[i][j].symbol.m_stackCount - this.m_breakSymbolList[i][j].symbol.GetMaxWearCnt()
                            };
                            this.m_svrBreakSymbolList.Add(item);
                        }
                    }
                }
            }
        }

        private void SetBreakSymbolListData(int index)
        {
            if ((index >= 0) && (index < this.m_breakSymbolList.Length))
            {
                ListView<CSymbolItem> allSymbolList = this.m_symbolSys.GetAllSymbolList();
                int count = allSymbolList.Count;
                ushort breakLvlMask = (ushort) (((int) 1) << (index + 1));
                for (int i = 0; i < count; i++)
                {
                    if (this.CheckSymbolBreak(allSymbolList[i], breakLvlMask))
                    {
                        CBreakSymbolItem item = new CBreakSymbolItem(allSymbolList[i], true);
                        this.m_breakSymbolList[index].Add(item);
                    }
                }
            }
        }

        private void SetSymbolMakeListData()
        {
            this.m_symbolMakeList.Clear();
            int count = s_allSymbolCfgList.Count;
            for (int i = 0; i < count; i++)
            {
                if (((s_allSymbolCfgList[i] != null) && (s_allSymbolCfgList[i].wLevel == this.m_symbolFilterLevel)) && ((this.m_symbolFilterType == enSymbolType.All) || ((s_allSymbolCfgList[i].wType & (((int) 1) << this.m_symbolFilterType)) != 0)))
                {
                    this.m_symbolMakeList.Add(s_allSymbolCfgList[i]);
                }
            }
        }

        public void SwitchToSymbolMakePanel(CUIEvent uiEvent)
        {
            this.ClearSymbolMakeData();
            this.RefreshSymbolMakeForm();
            CSymbolSystem.RefreshSymbolCntText();
            CUseable useable = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSymbolCoin, 0);
            stUIEventParams eventParams = new stUIEventParams {
                iconUseable = useable,
                tag = 3
            };
            CUIEventScript component = uiEvent.m_srcFormScript.GetWidget(8).GetComponent<CUIEventScript>();
            if (component != null)
            {
                component.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
                component.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
                component.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
                component.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
            }
        }

        public void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_TypeMenuSelect, new CUIEventManager.OnUIEventHandler(this.OnSymbolTypeMenuSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_LevelMenuSelect, new CUIEventManager.OnUIEventHandler(this.OnSymbolLevelMenuSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_ListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnSymbolMakeListEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_ListItemClick, new CUIEventManager.OnUIEventHandler(this.OnSymbolMakeListClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnItemMakeClick, new CUIEventManager.OnUIEventHandler(this.OnMakeSymbolClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnItemMakeConfirm, new CUIEventManager.OnUIEventHandler(this.OnItemMakeConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnItemBreakClick, new CUIEventManager.OnUIEventHandler(this.OnBreakSymbolClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnItemBreakConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakSymbolConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnBreakExcessSymbolClick, new CUIEventManager.OnUIEventHandler(this.OnBreakExcessSymbolClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_SelectBreakLvlItem, new CUIEventManager.OnUIEventHandler(this.OnSelectBreakLvlItem));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_OnBreakExcessSymbolConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakExcessSymbolClickConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_ItemBreakAnimatorEnd, new CUIEventManager.OnUIEventHandler(this.OnItemBreakAnimatorEnd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BreakItemEnable, new CUIEventManager.OnUIEventHandler(this.OnBreakListItemEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BreakListItemSelToggle, new CUIEventManager.OnUIEventHandler(this.OnBreakListItemSelToggle));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_OpenBreakDetailForm, new CUIEventManager.OnUIEventHandler(this.OnOpenBreakDetailForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BreakDetailFormConfirm, new CUIEventManager.OnUIEventHandler(this.OnBreakDetailFormConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_BreakDetailFormCancle, new CUIEventManager.OnUIEventHandler(this.OnBreakDetailFormCancle));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolMake_CoinNotEnoughGotoSymbolMall, new CUIEventManager.OnUIEventHandler(this.OnCoinNotEnoughGotoSymbolMall));
        }
    }
}

