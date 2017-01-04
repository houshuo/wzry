namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CSymbolSystem : Singleton<CSymbolSystem>
    {
        public enSymbolMenuType m_selectMenuType;
        private ListView<CSymbolItem> m_symbolList = new ListView<CSymbolItem>();
        public CSymbolMakeController m_symbolMakeCtrl = new CSymbolMakeController();
        public CSymbolRecommendController m_symbolRcmdCtrl = new CSymbolRecommendController();
        public CSymbolWearController m_symbolWearCtrl = new CSymbolWearController();
        public static string s_symbolEquipModulePath = "UGUI/Form/System/Symbol/Panel_SymbolEquip.prefab";
        public static string s_symbolEquipPanel = "Panel_SymbolEquip";
        public static string s_symbolFormPath = "UGUI/Form/System/Symbol/Form_Symbol.prefab";
        public static string s_symbolMakePanel = "Panel_SymbolMake";
        public static int[] s_symbolPagePropArr = new int[0x24];
        public static int[] s_symbolPagePropPctArr = new int[0x24];
        public static int[] s_symbolPropPctAddArr = new int[0x24];
        public static int[] s_symbolPropValAddArr = new int[0x24];

        public void Clear()
        {
            this.m_selectMenuType = enSymbolMenuType.SymbolEquip;
            this.m_symbolWearCtrl.Clear();
            this.m_symbolMakeCtrl.Clear();
            this.m_symbolRcmdCtrl.Clear();
        }

        public ListView<CSymbolItem> GetAllSymbolList()
        {
            return this.m_symbolList;
        }

        public static string GetSymbolAttString(CSymbolItem symbol, bool bPvp = true)
        {
            if (bPvp)
            {
                return CUICommonSystem.GetFuncEftDesc(ref symbol.m_SymbolData.astFuncEftList, true);
            }
            return CUICommonSystem.GetFuncEftDesc(ref symbol.m_SymbolData.astPveEftList, true);
        }

        public static string GetSymbolAttString(uint cfgId, bool bPvp = true)
        {
            ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(cfgId);
            if (dataByKey == null)
            {
                return string.Empty;
            }
            if (bPvp)
            {
                return CUICommonSystem.GetFuncEftDesc(ref dataByKey.astFuncEftList, true);
            }
            return CUICommonSystem.GetFuncEftDesc(ref dataByKey.astPveEftList, true);
        }

        public CSymbolItem GetSymbolByObjID(ulong objID)
        {
            for (int i = 0; i < this.m_symbolList.Count; i++)
            {
                CSymbolItem item2 = this.m_symbolList[i];
                if ((item2 != null) && (item2.m_objID == objID))
                {
                    return item2;
                }
            }
            return null;
        }

        public int GetSymbolListIndex(uint symbolCfgId)
        {
            return this.m_symbolWearCtrl.GetSymbolListIndex(symbolCfgId);
        }

        public static void GetSymbolProp(uint symbolId, ref int[] propArr, ref int[] propPctArr, bool bPvp)
        {
            int index = 0;
            int num2 = 0x24;
            for (index = 0; index < num2; index++)
            {
                propArr[index] = 0;
                propPctArr[index] = 0;
            }
            ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(symbolId);
            if (dataByKey != null)
            {
                ResDT_FuncEft_Obj[] objArray = !bPvp ? dataByKey.astPveEftList : dataByKey.astFuncEftList;
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

        public override void Init()
        {
            this.m_symbolWearCtrl.Init(this);
            this.m_symbolMakeCtrl.Init(this);
            this.m_symbolRcmdCtrl.Init(this);
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_FormClose, new CUIEventManager.OnUIEventHandler(this.OnSymbolFormClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_RevertToVisible, new CUIEventManager.OnUIEventHandler(this.OnSymbolRevertToVisible));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_OpenForm_ToMakeTab, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolFormToMakeTab));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_MenuSelect, new CUIEventManager.OnUIEventHandler(this.OnMenuSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Symbol_Load_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnLoadSubModule));
            Singleton<EventRouter>.instance.AddEventHandler("MasterSymbolCoinChanged", new System.Action(CSymbolSystem.RefreshSymbolCntText));
        }

        private void OnLoadSubModule(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if (srcFormScript != null)
            {
                if (uiEvent.m_srcWidget != null)
                {
                    uiEvent.m_srcWidget.CustomSetActive(false);
                }
                if (srcFormScript.transform.Find(s_symbolEquipPanel) == null)
                {
                    CUICommonSystem.LoadUIPrefab(s_symbolEquipModulePath, s_symbolEquipPanel, srcFormScript.gameObject, srcFormScript);
                    Transform transform = srcFormScript.transform.Find(s_symbolEquipPanel);
                    if (transform != null)
                    {
                        transform.SetSiblingIndex(2);
                        GameObject widget = srcFormScript.GetWidget(0);
                        if (widget != null)
                        {
                            CUIListScript component = widget.transform.GetComponent<CUIListScript>();
                            component.SelectElement(-1, false);
                            component.SelectElement((int) this.m_selectMenuType, true);
                        }
                    }
                }
            }
        }

        private void OnMenuSelect(CUIEvent uiEvent)
        {
            CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
            if ((srcFormScript != null) && (srcFormScript.transform.Find(s_symbolEquipPanel) != null))
            {
                CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
                if (null != component)
                {
                    int selectedIndex = component.GetSelectedIndex();
                    this.m_selectMenuType = (enSymbolMenuType) selectedIndex;
                    Transform transform = srcFormScript.transform;
                    if (this.m_selectMenuType == enSymbolMenuType.SymbolEquip)
                    {
                        transform.Find(s_symbolEquipPanel).gameObject.CustomSetActive(true);
                        transform.Find(s_symbolMakePanel).gameObject.CustomSetActive(false);
                        srcFormScript.GetWidget(9).CustomSetActive(false);
                        this.m_symbolWearCtrl.SwitchToSymbolWearPanel(uiEvent);
                    }
                    else if (this.m_selectMenuType == enSymbolMenuType.SymbolMake)
                    {
                        transform.Find(s_symbolEquipPanel).gameObject.CustomSetActive(false);
                        transform.Find(s_symbolMakePanel).gameObject.CustomSetActive(true);
                        srcFormScript.GetWidget(9).CustomSetActive(false);
                        this.m_symbolMakeCtrl.SwitchToSymbolMakePanel(uiEvent);
                    }
                    else if (this.m_selectMenuType == enSymbolMenuType.SymbolRecommend)
                    {
                        transform.Find(s_symbolEquipPanel).gameObject.CustomSetActive(false);
                        transform.Find(s_symbolMakePanel).gameObject.CustomSetActive(false);
                        srcFormScript.GetWidget(9).CustomSetActive(true);
                        this.m_symbolRcmdCtrl.SwitchToSymbolRcmdPanel(uiEvent);
                        Singleton<CMiShuSystem>.GetInstance().HideNewFlag(component.GetElemenet(1).gameObject, enNewFlagKey.New_BtnSymbolFlagKey_V1);
                        Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(7, null);
                    }
                    uiEvent.m_srcFormScript.GetWidget(1).CustomSetActive(this.m_selectMenuType == enSymbolMenuType.SymbolEquip);
                    uiEvent.m_srcFormScript.GetWidget(8).CustomSetActive(this.m_selectMenuType != enSymbolMenuType.SymbolEquip);
                }
            }
        }

        private void OnOpenSymbolForm(CUIEvent uiEvent)
        {
            this.m_selectMenuType = enSymbolMenuType.SymbolEquip;
            this.OpenSymbolForm();
            Singleton<CLobbySystem>.GetInstance().OnCheckSymbolEquipAlert();
            CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_SymbolBtn);
        }

        private void OnOpenSymbolFormToMakeTab(CUIEvent uiEvent)
        {
            this.m_selectMenuType = enSymbolMenuType.SymbolMake;
            this.OpenSymbolForm();
        }

        private void OnSymbolFormClose(CUIEvent uiEvent)
        {
            this.m_symbolRcmdCtrl.OnSymbolFormClose();
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Tips_ItemInfoClose);
            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Tips_CommonInfoClose);
        }

        private void OnSymbolRevertToVisible(CUIEvent uiEvent)
        {
        }

        private void OpenSymbolForm()
        {
            MonoSingleton<NewbieGuideManager>.GetInstance().SetNewbieBit(14, true, false);
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_symbolFormPath, false, true);
            if (script != null)
            {
                string text = Singleton<CTextManager>.GetInstance().GetText("Symbol_Sheet_Symbol");
                string str2 = Singleton<CTextManager>.GetInstance().GetText("SymbolRcmd_Tab");
                string str3 = Singleton<CTextManager>.GetInstance().GetText("Symbol_Sheet_Make");
                string[] titleList = new string[] { text, str2, str3 };
                GameObject widget = script.GetWidget(0);
                CUICommonSystem.InitMenuPanel(widget, titleList, (int) this.m_selectMenuType);
                CUIListScript component = widget.GetComponent<CUIListScript>();
                Singleton<CMiShuSystem>.GetInstance().ShowNewFlag(component.GetElemenet(1).gameObject, enNewFlagKey.New_BtnSymbolFlagKey_V1);
            }
        }

        [MessageHandler(0x46d)]
        public static void ReciveSymbolDatail(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_SYMBOLDETAIL stSymbolDetail = msg.stPkgData.stSymbolDetail;
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.m_symbolInfo.SetData(stSymbolDetail.stSymbolInfo);
                Singleton<CSymbolSystem>.GetInstance().RefreshSymbolForm();
            }
            else
            {
                DebugHelper.Assert(false, "ReciveSymbolDatail Master RoleInfo is NULL!!!");
            }
        }

        public static void RefreshPropPanel(GameObject propPanel, ref int[] propArr, ref int[] propPctArr)
        {
            int num = 0x24;
            int amount = 0;
            for (int i = 0; i < num; i++)
            {
                if ((propArr[i] > 0) || (propPctArr[i] > 0))
                {
                    amount++;
                }
            }
            CUIListScript component = propPanel.GetComponent<CUIListScript>();
            component.SetElementAmount(amount);
            amount = 0;
            for (int j = 0; j < num; j++)
            {
                if ((propArr[j] > 0) || (propPctArr[j] > 0))
                {
                    CUIListElementScript elemenet = component.GetElemenet(amount);
                    DebugHelper.Assert(elemenet != null);
                    if (elemenet != null)
                    {
                        Text text = elemenet.gameObject.transform.Find("titleText").GetComponent<Text>();
                        Text text2 = elemenet.gameObject.transform.Find("valueText").GetComponent<Text>();
                        DebugHelper.Assert(text != null);
                        if (text != null)
                        {
                            text.text = CUICommonSystem.s_attNameList[j];
                        }
                        DebugHelper.Assert(text2 != null);
                        if (text2 != null)
                        {
                            if (propArr[j] > 0)
                            {
                                if (CUICommonSystem.s_pctFuncEftList.IndexOf((uint) j) != -1)
                                {
                                    text2.text = string.Format("+{0}", CUICommonSystem.GetValuePercent(propArr[j] / 100));
                                }
                                else
                                {
                                    text2.text = string.Format("+{0}", ((float) propArr[j]) / 100f);
                                }
                            }
                            else if (propPctArr[j] > 0)
                            {
                                text2.text = string.Format("+{0}", CUICommonSystem.GetValuePercent(propPctArr[j]));
                            }
                        }
                    }
                    amount++;
                }
            }
        }

        public static void RefreshSymbolCntText()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_symbolFormPath);
            if (form != null)
            {
                Text component = form.GetWidget(8).transform.Find("symbolCoinCntText").GetComponent<Text>();
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (component != null)
                {
                    if (masterRoleInfo != null)
                    {
                        component.text = masterRoleInfo.SymbolCoin.ToString();
                    }
                    else
                    {
                        component.text = string.Empty;
                    }
                }
            }
        }

        public void RefreshSymbolForm()
        {
            if (Singleton<CUIManager>.GetInstance().GetForm(s_symbolFormPath) != null)
            {
                if (this.m_selectMenuType == enSymbolMenuType.SymbolEquip)
                {
                    this.m_symbolWearCtrl.RefreshSymbolEquipPanel();
                }
                else if (this.m_selectMenuType == enSymbolMenuType.SymbolMake)
                {
                    this.m_symbolMakeCtrl.RefreshSymbolMakeForm();
                }
                else if (this.m_selectMenuType == enSymbolMenuType.SymbolRecommend)
                {
                    this.m_symbolRcmdCtrl.RefreshSymbolRcmdPanel();
                }
            }
        }

        public static void RefreshSymbolPageProp(GameObject propListPanel, int pageIndex, bool bPvp = true)
        {
            if (propListPanel != null)
            {
                Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_symbolInfo.GetSymbolPageProp(pageIndex, ref s_symbolPagePropArr, ref s_symbolPagePropPctArr, bPvp);
                RefreshPropPanel(propListPanel, ref s_symbolPagePropArr, ref s_symbolPagePropPctArr);
            }
        }

        public static void RefreshSymbolPagePveEnhanceProp(GameObject propList, int pageIndex)
        {
            if (propList != null)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                masterRoleInfo.m_symbolInfo.GetSymbolPageProp(pageIndex, ref s_symbolPagePropArr, ref s_symbolPagePropPctArr, true);
                masterRoleInfo.m_symbolInfo.GetSymbolPageProp(pageIndex, ref s_symbolPropValAddArr, ref s_symbolPropPctAddArr, false);
                int num = 0x24;
                for (int i = 0; i < num; i++)
                {
                    s_symbolPropValAddArr[i] -= s_symbolPagePropArr[i];
                    s_symbolPropPctAddArr[i] -= s_symbolPagePropPctArr[i];
                }
                RefreshPropPanel(propList, ref s_symbolPropValAddArr, ref s_symbolPropPctAddArr);
            }
        }

        public static void RefreshSymbolPropContent(GameObject propPanel, uint symbolId)
        {
            ResSymbolInfo dataByKey = GameDataMgr.symbolInfoDatabin.GetDataByKey(symbolId);
            if ((propPanel != null) && (dataByKey != null))
            {
                GetSymbolProp(symbolId, ref s_symbolPagePropArr, ref s_symbolPagePropPctArr, true);
                GetSymbolProp(symbolId, ref s_symbolPropValAddArr, ref s_symbolPropPctAddArr, false);
                int num = 0x24;
                for (int i = 0; i < num; i++)
                {
                    s_symbolPropValAddArr[i] -= s_symbolPagePropArr[i];
                    s_symbolPropPctAddArr[i] -= s_symbolPagePropPctArr[i];
                }
                RefreshPropPanel(propPanel.transform.Find("pvpPropPanel").Find("propList").gameObject, ref s_symbolPagePropArr, ref s_symbolPagePropPctArr);
                RefreshPropPanel(propPanel.transform.Find("pveEnhancePropPanel").Find("propList").gameObject, ref s_symbolPropValAddArr, ref s_symbolPropPctAddArr);
            }
        }

        public static void SendQuerySymbol()
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x46c);
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public void SetSymbolData()
        {
            CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
            int curUseableCount = useableContainer.GetCurUseableCount();
            CUseable useableByIndex = null;
            this.m_symbolList.Clear();
            int index = 0;
            for (index = 0; index < curUseableCount; index++)
            {
                useableByIndex = useableContainer.GetUseableByIndex(index);
                if (useableByIndex.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
                {
                    CSymbolItem item = (CSymbolItem) useableByIndex;
                    if (item != null)
                    {
                        this.m_symbolList.Add(item);
                    }
                }
            }
        }

        public override void UnInit()
        {
            this.m_symbolWearCtrl.UnInit();
            this.m_symbolMakeCtrl.UnInit();
            this.m_symbolRcmdCtrl.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_FormClose, new CUIEventManager.OnUIEventHandler(this.OnSymbolFormClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_RevertToVisible, new CUIEventManager.OnUIEventHandler(this.OnSymbolRevertToVisible));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_OpenForm_ToMakeTab, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolFormToMakeTab));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_MenuSelect, new CUIEventManager.OnUIEventHandler(this.OnMenuSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Symbol_Load_Sub_Module, new CUIEventManager.OnUIEventHandler(this.OnLoadSubModule));
            Singleton<EventRouter>.instance.RemoveEventHandler("MasterSymbolCoinChanged", new System.Action(CSymbolSystem.RefreshSymbolCntText));
        }
    }
}

