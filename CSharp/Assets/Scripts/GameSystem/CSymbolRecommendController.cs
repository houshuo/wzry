namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [MessageHandlerClass]
    public class CSymbolRecommendController
    {
        private bool m_bOwnHeroFlag = true;
        private DictionaryView<uint, List<uint>[]> m_cfgHeroRcmdSymbolList = new DictionaryView<uint, List<uint>[]>();
        public uint m_curHeroId;
        private enHeroJobType m_curHeroJob;
        private ListView<ResHeroCfgInfo> m_heroList = new ListView<ResHeroCfgInfo>();
        private List<uint>[] m_rcmdSymbolList = new List<uint>[CSymbolInfo.s_maxSymbolLevel];
        private ushort m_symbolRcmdLevel = 1;
        private CSymbolSystem m_symbolSys;
        private static string s_ChooseHeroPath = "UGUI/Form/System/Symbol/Form_ChooseHero.prefab";

        public void Clear()
        {
        }

        private void GetHeroRcmdSymbolList()
        {
            if (!this.m_cfgHeroRcmdSymbolList.TryGetValue(this.m_curHeroId, out this.m_rcmdSymbolList))
            {
                ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(this.m_curHeroId);
                if (dataByKey == null)
                {
                    DebugHelper.Assert(false, "GetHeroRcmdSymbolList heroCfgInfo is null heroId = " + this.m_curHeroId);
                }
                else
                {
                    List<uint>[] listArray = new List<uint>[CSymbolInfo.s_maxSymbolLevel];
                    HashSet<object>.Enumerator enumerator = GameDataMgr.symbolRcmdDatabin.GetDataByKey(dataByKey.dwSymbolRcmdID).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ResSymbolRcmd current = (ResSymbolRcmd) enumerator.Current;
                        if (listArray[current.wSymbolLvl - 1] == null)
                        {
                            listArray[current.wSymbolLvl - 1] = new List<uint>();
                        }
                        for (int i = 0; i < current.SymbolID.Length; i++)
                        {
                            if (current.SymbolID[i] > 0)
                            {
                                listArray[current.wSymbolLvl - 1].Add(current.SymbolID[i]);
                            }
                        }
                    }
                    this.m_cfgHeroRcmdSymbolList.Add(this.m_curHeroId, listArray);
                    this.m_rcmdSymbolList = listArray;
                }
            }
        }

        public void Init(CSymbolSystem symbolSys)
        {
            this.m_symbolSys = symbolSys;
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolRcmd_OpenChangeHeroForm, new CUIEventManager.OnUIEventHandler(this.OnOpenChangeHeroForm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolRcmd_HeroListItemClick, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolRcmd_LevelListIndexChange, new CUIEventManager.OnUIEventHandler(this.OnSymbolRcmdLevelChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolRcmd_HeroTypeChange, new CUIEventManager.OnUIEventHandler(this.OnHeroTypeListSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolRcmd_HeroOwnFlagChange, new CUIEventManager.OnUIEventHandler(this.OnHeroOwnFlagChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolRcmd_HeroListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SymbolRcmd_RcmdListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnRcmdListItemEnable));
        }

        private void OnHeroListElementClick(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_ChooseHeroPath);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.m_symbolInfo.m_selSymbolRcmdHeroId = uiEvent.m_eventParams.heroId;
                this.m_curHeroId = masterRoleInfo.m_symbolInfo.m_selSymbolRcmdHeroId;
                this.RefreshSymbolRcmdPanel();
                SendReqSelectHeroLvl(this.m_curHeroId, this.m_symbolRcmdLevel);
            }
        }

        private void OnHeroListElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_heroList.Count))
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                DebugHelper.Assert(masterRoleInfo != null, "CSymbolRcmdCtrl OnHeroListElementEnable role is null");
                if ((masterRoleInfo != null) && (masterRoleInfo.m_customRecommendEquipDictionary != null))
                {
                    ResHeroCfgInfo info2 = this.m_heroList[srcWidgetIndexInBelongedList];
                    if ((info2 != null) && (uiEvent.m_srcWidget != null))
                    {
                        Transform transform = uiEvent.m_srcWidget.transform.Find("heroItemCell");
                        if (transform != null)
                        {
                            CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, transform.gameObject, info2.szImagePath, enHeroHeadType.enIcon, !masterRoleInfo.IsHaveHero(info2.dwCfgID, false));
                            CUIEventScript component = transform.GetComponent<CUIEventScript>();
                            if (component != null)
                            {
                                stUIEventParams eventParams = new stUIEventParams {
                                    heroId = info2.dwCfgID
                                };
                                component.SetUIEvent(enUIEventType.Click, enUIEventID.SymbolRcmd_HeroListItemClick, eventParams);
                            }
                            Transform transform2 = transform.Find("TxtFree");
                            if (transform2 != null)
                            {
                                transform2.gameObject.CustomSetActive(masterRoleInfo.IsFreeHero(info2.dwCfgID));
                            }
                        }
                    }
                }
            }
        }

        private void OnHeroOwnFlagChange(CUIEvent uiEvent)
        {
            this.RefreshHeroListPanel();
        }

        private void OnHeroTypeListSelect(CUIEvent uiEvent)
        {
            CUIListScript srcWidgetScript = (CUIListScript) uiEvent.m_srcWidgetScript;
            if (srcWidgetScript != null)
            {
                this.m_curHeroJob = (enHeroJobType) srcWidgetScript.GetSelectedIndex();
                this.RefreshHeroListPanel();
            }
        }

        private void OnOpenChangeHeroForm(CUIEvent uiEvent)
        {
            CUIFormScript script = Singleton<CUIManager>.GetInstance().OpenForm(s_ChooseHeroPath, false, true);
            if (null != script)
            {
                this.m_curHeroJob = enHeroJobType.All;
                GameObject widget = script.GetWidget(0);
                if (widget != null)
                {
                    GameObject obj3 = script.GetWidget(1);
                    if (obj3 != null)
                    {
                        obj3.GetComponent<Toggle>().isOn = this.m_bOwnHeroFlag;
                    }
                    string text = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_All");
                    string str2 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Tank");
                    string str3 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Soldier");
                    string str4 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Assassin");
                    string str5 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Master");
                    string str6 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Archer");
                    string str7 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Aid");
                    string[] titleList = new string[] { text, str2, str3, str4, str5, str6, str7 };
                    CUICommonSystem.InitMenuPanel(widget, titleList, (int) this.m_curHeroJob);
                }
            }
        }

        private void OnRcmdListItemEnable(CUIEvent uiEvent)
        {
            if ((this.m_rcmdSymbolList != null) && (this.m_rcmdSymbolList[this.m_symbolRcmdLevel - 1] != null))
            {
                if ((uiEvent.m_srcWidgetIndexInBelongedList < 0) || (uiEvent.m_srcWidgetIndexInBelongedList >= this.m_rcmdSymbolList[this.m_symbolRcmdLevel - 1].Count))
                {
                    DebugHelper.Assert(false, "OnRcmdListItemEnable index out of range");
                }
                else
                {
                    CSymbolMakeController.RefreshSymbolItem(GameDataMgr.symbolInfoDatabin.GetDataByKey(this.m_rcmdSymbolList[this.m_symbolRcmdLevel - 1][uiEvent.m_srcWidgetIndexInBelongedList]), uiEvent.m_srcWidget, uiEvent.m_srcFormScript);
                }
            }
        }

        public void OnSymbolFormClose()
        {
            if (this.m_curHeroId != 0)
            {
                SendReqSelectHeroLvl(this.m_curHeroId, this.m_symbolRcmdLevel);
            }
        }

        private void OnSymbolRcmdLevelChange(CUIEvent uiEvent)
        {
            CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
            this.m_symbolRcmdLevel = (ushort) (component.GetSelectedIndex() + 1);
            this.RefreshRcmdSymbolListPanel();
        }

        [MessageHandler(0x49f)]
        public static void ReciveSymbolRcmdHeroLvl(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_SYMBOLRCMD_SEL stSymbolRcmdSelRsp = msg.stPkgData.stSymbolRcmdSelRsp;
            if (stSymbolRcmdSelRsp.iResult == 0)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if ((masterRoleInfo != null) && (masterRoleInfo.m_symbolInfo != null))
                {
                    masterRoleInfo.m_symbolInfo.m_selSymbolRcmdHeroId = stSymbolRcmdSelRsp.dwHeroID;
                    masterRoleInfo.m_symbolInfo.m_selSymbolRcmdLevel = stSymbolRcmdSelRsp.wSymbolLvl;
                }
            }
        }

        [MessageHandler(0x49d)]
        public static void ReciveSymbolRcmdWearRsp(CSPkg msg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            SCPKG_CMD_SYMBOLRCMD_WEAR stSymbolRcmdWearRsp = msg.stPkgData.stSymbolRcmdWearRsp;
        }

        private void RefreshHeroImage()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (null != form)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                DebugHelper.Assert(masterRoleInfo != null, "RefreshHeroImage role is null");
                if (masterRoleInfo != null)
                {
                    GameObject widget = form.GetWidget(12);
                    ResHeroCfgInfo dataByKey = null;
                    if (masterRoleInfo.m_symbolInfo.m_selSymbolRcmdHeroId == 0)
                    {
                        dataByKey = GameDataMgr.heroDatabin.GetDataByKey(masterRoleInfo.GetFirstHeroId());
                    }
                    else
                    {
                        dataByKey = GameDataMgr.heroDatabin.GetDataByKey(masterRoleInfo.m_symbolInfo.m_selSymbolRcmdHeroId);
                    }
                    if (dataByKey != null)
                    {
                        masterRoleInfo.m_symbolInfo.m_selSymbolRcmdHeroId = dataByKey.dwCfgID;
                        this.m_curHeroId = dataByKey.dwCfgID;
                        CUICommonSystem.SetHeroItemImage(form, widget.gameObject, dataByKey.szImagePath, enHeroHeadType.enIcon, false);
                        GameObject obj3 = form.GetWidget(13);
                        if (obj3 != null)
                        {
                            obj3.GetComponent<Text>().text = dataByKey.szName;
                        }
                    }
                }
            }
        }

        private void RefreshHeroListPanel()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_ChooseHeroPath);
            if (null != form)
            {
                GameObject widget = form.GetWidget(1);
                if (widget != null)
                {
                    this.m_bOwnHeroFlag = widget.GetComponent<Toggle>().isOn;
                }
                this.ResetHeroList(this.m_curHeroJob, this.m_bOwnHeroFlag);
                GameObject obj3 = form.GetWidget(2);
                if (obj3 != null)
                {
                    obj3.GetComponent<CUIListScript>().SetElementAmount(this.m_heroList.Count);
                }
            }
        }

        private void RefreshRcmdSymbolListPanel()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (null != form)
            {
                this.GetHeroRcmdSymbolList();
                GameObject widget = form.GetWidget(10);
                if ((widget != null) && (this.m_rcmdSymbolList[this.m_symbolRcmdLevel - 1] != null))
                {
                    widget.GetComponent<CUIListScript>().SetElementAmount(this.m_rcmdSymbolList[this.m_symbolRcmdLevel - 1].Count);
                    Transform transform = widget.transform.Find("Text");
                    if (transform != null)
                    {
                        ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(this.m_curHeroId);
                        transform.GetComponent<Text>().text = string.Format(Singleton<CTextManager>.GetInstance().GetText("SymbolRcmd_HeroLvlText"), dataByKey.szName, this.m_symbolRcmdLevel);
                    }
                }
            }
        }

        public void RefreshSymbolRcmdPanel()
        {
            this.m_symbolSys.SetSymbolData();
            this.RefreshHeroImage();
            this.RefreshRcmdSymbolListPanel();
        }

        private void ResetHeroList(enHeroJobType jobType, bool bOwn)
        {
            this.m_heroList.Clear();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "CSymbolRcommendController ResetHeroList role is null");
            if (masterRoleInfo != null)
            {
                ListView<ResHeroCfgInfo> allHeroList = CHeroDataFactory.GetAllHeroList();
                for (int i = 0; i < allHeroList.Count; i++)
                {
                    if ((((jobType == enHeroJobType.All) || (allHeroList[i].bMainJob == ((byte) jobType))) || (allHeroList[i].bMinorJob == ((byte) jobType))) && (!bOwn || masterRoleInfo.IsHaveHero(allHeroList[i].dwCfgID, false)))
                    {
                        this.m_heroList.Add(allHeroList[i]);
                    }
                }
            }
        }

        public static void SendReqSelectHeroLvl(uint heroId, ushort level)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x49e);
            msg.stPkgData.stSymbolRcmdSelReq.dwHeroID = heroId;
            msg.stPkgData.stSymbolRcmdSelReq.wSymbolLvl = level;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
        }

        public static void SendReqWearRcmdSymbol(int pageIndex)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x49c);
            msg.stPkgData.stSymbolRcmdWearReq.bSymbolPageIdx = (byte) pageIndex;
            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
        }

        public void SwitchToSymbolRcmdPanel(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo == null)
            {
                DebugHelper.Assert(false, "SwitchToSymbolRcmdPanel role is null");
            }
            else
            {
                this.m_curHeroId = masterRoleInfo.m_symbolInfo.m_selSymbolRcmdHeroId;
                this.m_symbolRcmdLevel = masterRoleInfo.m_symbolInfo.m_selSymbolRcmdLevel;
                this.m_symbolRcmdLevel = (ushort) Math.Max(1, Math.Min(this.m_symbolRcmdLevel, CSymbolInfo.s_maxSymbolLevel));
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
                if (null != form)
                {
                    CSymbolSystem.RefreshSymbolCntText();
                    this.RefreshSymbolRcmdPanel();
                    string[] titleList = new string[] { "1", "2", "3", "4", "5" };
                    CUICommonSystem.InitMenuPanel(form.GetWidget(11), titleList, this.m_symbolRcmdLevel - 1);
                }
            }
        }

        public void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolRcmd_OpenChangeHeroForm, new CUIEventManager.OnUIEventHandler(this.OnOpenChangeHeroForm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolRcmd_HeroListItemClick, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolRcmd_LevelListIndexChange, new CUIEventManager.OnUIEventHandler(this.OnSymbolRcmdLevelChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolRcmd_HeroTypeChange, new CUIEventManager.OnUIEventHandler(this.OnHeroTypeListSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolRcmd_HeroOwnFlagChange, new CUIEventManager.OnUIEventHandler(this.OnHeroOwnFlagChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolRcmd_HeroListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SymbolRcmd_RcmdListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnRcmdListItemEnable));
        }
    }
}

