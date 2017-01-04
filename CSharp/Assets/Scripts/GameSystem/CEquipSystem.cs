namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.UI;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [MessageHandlerClass]
    public class CEquipSystem : Singleton<CEquipSystem>
    {
        private bool bEditEquip;
        private const int c_2ndEquipMaxCount = 3;
        private const int c_3rdEquipMaxCountPer2ndEquip = 2;
        private const int c_backEquipMaxCount = 20;
        public const int c_equipLevelMaxCount = 3;
        private const int c_equipLineCnt = 14;
        private const int c_maxEquipCntPerLevel = 8;
        private float c_moveAnimaTime = 0.1f;
        public const int c_recommendEquipMaxCount = 6;
        private bool m_bOwnHeroFlag = true;
        private enEquipUsage m_curEquipUsage = enEquipUsage.PhyAttack;
        private enHeroJobType m_curHeroJob;
        private CUIFormScript m_customEquipForm;
        private Dictionary<uint, ushort[]> m_defaultRecommendEquipsDictionary;
        private ushort[] m_editCustomRecommendEquips = new ushort[6];
        private uint m_editHeroID;
        private Dictionary<ushort, CEquipInfo> m_equipInfoDictionary;
        private List<ushort>[][] m_equipList;
        private Dictionary<uint, stEquipRankInfo> m_equipRankItemDetail = new Dictionary<uint, stEquipRankInfo>();
        private CEquipRelationPath m_equipRelationPath;
        private stEquipTree m_equipTree;
        private ListView<ResHeroCfgInfo> m_heroList = new ListView<ResHeroCfgInfo>();
        private uint m_reqRankHeroId;
        private bool m_revertDefaultEquip;
        private CEquipInfo m_selEquipInfo;
        private Transform m_selEquipItemObj;
        private float m_uiCustomEquipContentHeight;
        private float m_uiEquipItemContentDefaultHeight;
        private float m_uiEquipItemContentHightDiff = 70f;
        private float m_uiEquipItemHeight;
        private bool m_useGodEquip;
        private static string s_ChooseHeroPath = "UGUI/Form/System/CustomRecommendEquip/Form_ChooseHero.prefab";
        private static string s_CustomRecommendEquipPath = "UGUI/Form/System/CustomRecommendEquip/Form_CustomRecommendEquip.prefab";
        private static string s_EquipListNodePrefabPath = "UGUI/Form/System/CustomRecommendEquip/Panel_EquipList.prefab";
        private static string s_EquipTreePath = "UGUI/Form/System/CustomRecommendEquip/Form_EquipTree.prefab";
        private static int s_equipUsageAmount = Enum.GetValues(typeof(enEquipUsage)).Length;
        private static string s_GodEquipPath = "UGUI/Form/System/CustomRecommendEquip/Form_GodEquip.prefab";

        private void ClearCurSelectEquipItem()
        {
            this.m_equipRelationPath.Reset();
            if (this.m_selEquipItemObj != null)
            {
                this.SetEquipItemSelectFlag(this.m_selEquipItemObj, false);
                this.m_selEquipItemObj = null;
            }
            this.m_selEquipInfo = null;
        }

        private void ClearEquipList()
        {
            for (int i = 0; i < s_equipUsageAmount; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    this.m_equipList[i][j].Clear();
                }
            }
        }

        public void CloseRightPanel()
        {
            if (null != this.m_customEquipForm)
            {
                this.m_customEquipForm.GetWidget(2).CustomSetActive(false);
                this.m_customEquipForm.GetWidget(3).CustomSetActive(false);
                this.m_customEquipForm.GetWidget(4).CustomSetActive(false);
            }
        }

        private void DefaultRecommendEquipsInVisitor(ResRecommendEquipInBattle resRecommendEquipInBattle)
        {
            if (this.m_defaultRecommendEquipsDictionary.ContainsKey(resRecommendEquipInBattle.wHeroID))
            {
                this.m_defaultRecommendEquipsDictionary.Remove(resRecommendEquipInBattle.wHeroID);
            }
            if (resRecommendEquipInBattle.RecommendEquipID.Length == 6)
            {
                ushort[] numArray = new ushort[6];
                for (int i = 0; i < 6; i++)
                {
                    CEquipInfo info = null;
                    if ((resRecommendEquipInBattle.RecommendEquipID[i] == 0) || (this.m_equipInfoDictionary.TryGetValue(resRecommendEquipInBattle.RecommendEquipID[i], out info) && (info.m_resEquipInBattle.bInvalid <= 0)))
                    {
                        numArray[i] = resRecommendEquipInBattle.RecommendEquipID[i];
                    }
                    else
                    {
                        Debug.LogError(string.Concat(new object[] { "Gao Mao a! tuijian zhuangbei limian tian le ge bu ke yong de zhuagnbei!!! HeroID = ", resRecommendEquipInBattle.wHeroID, ", equipID = ", resRecommendEquipInBattle.RecommendEquipID[i] }));
                        numArray[i] = 0;
                    }
                }
                this.m_defaultRecommendEquipsDictionary.Add(resRecommendEquipInBattle.wHeroID, numArray);
            }
            else
            {
                Debug.LogError("Gao Mao a! tuijian zhuangbei de shuliang dou meiyou tian dui!!! HeroID = " + resRecommendEquipInBattle.wHeroID);
            }
        }

        private void EditCustomRecommendEquipByGodEquip(int cnt, ref uint[] equipList)
        {
            if ((equipList != null) && (cnt <= equipList.Length))
            {
                for (int i = 0; i < 6; i++)
                {
                    this.m_editCustomRecommendEquips[i] = 0;
                }
                for (int j = 0; (j < cnt) && (j < 6); j++)
                {
                    this.m_editCustomRecommendEquips[j] = (ushort) equipList[j];
                }
            }
        }

        private void EquipInBattleInVisitor(ResEquipInBattle resEquipInBattle)
        {
            if (this.m_equipInfoDictionary.ContainsKey(resEquipInBattle.wID))
            {
                this.m_equipInfoDictionary.Remove(resEquipInBattle.wID);
            }
            this.m_equipInfoDictionary.Add(resEquipInBattle.wID, new CEquipInfo(resEquipInBattle.wID));
            if ((((resEquipInBattle.bUsage >= 0) && (resEquipInBattle.bUsage < s_equipUsageAmount)) && ((resEquipInBattle.bLevel >= 1) && (resEquipInBattle.bLevel <= 3))) && (resEquipInBattle.bInvalid == 0))
            {
                this.m_equipList[resEquipInBattle.bUsage][resEquipInBattle.bLevel - 1].Add(resEquipInBattle.wID);
            }
        }

        public ushort[] GetDefaultRecommendEquipInfo(uint heroID)
        {
            ushort[] numArray = null;
            if (this.m_defaultRecommendEquipsDictionary.TryGetValue(heroID, out numArray))
            {
                return numArray;
            }
            return null;
        }

        public CEquipInfo GetEquipInfo(ushort equipID)
        {
            CEquipInfo info = null;
            if (this.m_equipInfoDictionary.TryGetValue(equipID, out info))
            {
                return info;
            }
            return null;
        }

        public Dictionary<ushort, CEquipInfo> GetEquipInfoDictionary()
        {
            return this.m_equipInfoDictionary;
        }

        public List<ushort>[][] GetEquipList()
        {
            return this.m_equipList;
        }

        private GameObject GetEquipListNodeWidget(enEquipListNodeWidget widgetId)
        {
            if (this.m_customEquipForm != null)
            {
                GameObject widget = this.m_customEquipForm.GetWidget(5);
                if (widget != null)
                {
                    Transform transform = widget.transform.Find("Panel_EquipList");
                    if (transform != null)
                    {
                        CUIComponent component = transform.GetComponent<CUIComponent>();
                        if (component != null)
                        {
                            return component.GetWidget((int) widgetId);
                        }
                    }
                }
            }
            return null;
        }

        private void GetEquipTree(ushort equipID, ref stEquipTree equipTree)
        {
            equipTree.Create(equipID, this.m_equipInfoDictionary);
        }

        public override void Init()
        {
            this.m_equipList = new List<ushort>[s_equipUsageAmount][];
            for (int i = 0; i < s_equipUsageAmount; i++)
            {
                this.m_equipList[i] = new List<ushort>[3];
                for (int j = 0; j < 3; j++)
                {
                    this.m_equipList[i][j] = new List<ushort>();
                }
            }
            this.m_equipTree = new stEquipTree(3, 2, 20);
            this.m_equipRelationPath = new CEquipRelationPath();
            this.m_equipInfoDictionary = new Dictionary<ushort, CEquipInfo>();
            this.m_defaultRecommendEquipsDictionary = new Dictionary<uint, ushort[]>();
            GameDataMgr.m_equipInBattleDatabin.Accept(new Action<ResEquipInBattle>(this.EquipInBattleInVisitor));
            this.InitializeBackEquipListForEachEquip();
            GameDataMgr.m_recommendEquipInBattleDatabin.Accept(new Action<ResRecommendEquipInBattle>(this.DefaultRecommendEquipsInVisitor));
            this.InitUIEventListener();
        }

        private void InitEquipItemHorizontalLine(Transform equipPanel, int level)
        {
            if (null != equipPanel)
            {
                Transform transform = null;
                Transform transform2 = null;
                Transform transform3 = null;
                int index = 0;
                for (int i = 0; i < 8; i++)
                {
                    transform = equipPanel.Find(string.Format("equipItem{0}", i));
                    transform2 = transform.Find("imgLineFront");
                    if (level <= 1)
                    {
                        transform2.gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        index = (level <= 2) ? 0 : 1;
                        this.m_equipRelationPath.InitializeHorizontalLine(index, i, CEquipLineSet.enHorizontalLineType.Right, transform2.gameObject);
                    }
                    transform3 = transform.Find("imgLineBack");
                    if (level >= 3)
                    {
                        transform3.gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        index = (level >= 2) ? 1 : 0;
                        this.m_equipRelationPath.InitializeHorizontalLine(index, i, CEquipLineSet.enHorizontalLineType.Left, transform3.gameObject);
                    }
                }
            }
        }

        private void InitEquipPathLine()
        {
            if (this.m_customEquipForm != null)
            {
                this.m_equipRelationPath.Clear();
                GameObject equipListNodeWidget = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipVerticalLinesPanel);
                if (null != equipListNodeWidget)
                {
                    Transform transform = equipListNodeWidget.transform;
                    for (int i = 0; i < 14; i++)
                    {
                        Transform transform2 = transform.Find(string.Format("imgLine{0}", i));
                        int startRow = i % 7;
                        this.m_equipRelationPath.InitializeVerticalLine(i / 7, startRow, startRow + 1, transform2.gameObject);
                    }
                    GameObject obj3 = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipPanel_Level1);
                    if (obj3 != null)
                    {
                        this.InitEquipItemHorizontalLine(obj3.transform, 1);
                    }
                    GameObject obj4 = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipPanel_Level2);
                    if (obj4 != null)
                    {
                        this.InitEquipItemHorizontalLine(obj4.transform, 2);
                    }
                    GameObject obj5 = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipPanel_Level3);
                    if (obj5 != null)
                    {
                        this.InitEquipItemHorizontalLine(obj5.transform, 3);
                    }
                }
            }
        }

        private void InitializeBackEquipListForEachEquip()
        {
            Dictionary<ushort, CEquipInfo>.Enumerator enumerator = this.m_equipInfoDictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<ushort, CEquipInfo> current = enumerator.Current;
                CEquipInfo info = current.Value;
                if (info.m_resEquipInBattle != null)
                {
                    for (int i = 0; i < info.m_resEquipInBattle.PreEquipID.Length; i++)
                    {
                        if (info.m_resEquipInBattle.PreEquipID[i] > 0)
                        {
                            CEquipInfo info2 = null;
                            if (this.m_equipInfoDictionary.TryGetValue(info.m_resEquipInBattle.PreEquipID[i], out info2))
                            {
                                info2.AddBackEquipID(info.m_equipID);
                            }
                        }
                    }
                }
            }
        }

        private void InitializeEditCustomRecommendEquips(uint heroID, ref bool useCustomRecommendEquips)
        {
            for (int i = 0; i < 6; i++)
            {
                this.m_editCustomRecommendEquips[i] = 0;
            }
            this.m_editHeroID = heroID;
            if (heroID != 0)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if ((masterRoleInfo != null) && (masterRoleInfo.m_customRecommendEquipDictionary != null))
                {
                    ushort[] defaultRecommendEquipInfo = null;
                    useCustomRecommendEquips = false;
                    if (masterRoleInfo.m_customRecommendEquipDictionary.TryGetValue(heroID, out defaultRecommendEquipInfo))
                    {
                        useCustomRecommendEquips = true;
                    }
                    else
                    {
                        useCustomRecommendEquips = false;
                        defaultRecommendEquipInfo = this.GetDefaultRecommendEquipInfo(heroID);
                    }
                    if (defaultRecommendEquipInfo != null)
                    {
                        bool flag = false;
                        for (int j = 0; j < 6; j++)
                        {
                            if (defaultRecommendEquipInfo[j] == 0)
                            {
                                this.m_editCustomRecommendEquips[j] = 0;
                            }
                            else
                            {
                                CEquipInfo info2 = null;
                                if (this.m_equipInfoDictionary.TryGetValue(defaultRecommendEquipInfo[j], out info2) && (info2.m_resEquipInBattle.bInvalid <= 0))
                                {
                                    this.m_editCustomRecommendEquips[j] = defaultRecommendEquipInfo[j];
                                }
                                else
                                {
                                    this.m_editCustomRecommendEquips[j] = 0;
                                    if (useCustomRecommendEquips)
                                    {
                                        flag = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void InitUIEventListener()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnCustomEquipOpen));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_FormClose, new CUIEventManager.OnUIEventHandler(this.OnCustomEquipClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_UsageListSelect, new CUIEventManager.OnUIEventHandler(this.OnCustomEquipListSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_ItemClick, new CUIEventManager.OnUIEventHandler(this.OnEuipItemClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_EditItemClick, new CUIEventManager.OnUIEventHandler(this.OnCustomEditItemClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_ViewEquipTree, new CUIEventManager.OnUIEventHandler(this.OnViewEquipTree));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_ChangeHero, new CUIEventManager.OnUIEventHandler(this.OnChangeHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_ModifyEquip, new CUIEventManager.OnUIEventHandler(this.OnModifyEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_ConfirmModify, new CUIEventManager.OnUIEventHandler(this.OnConfirmModifyEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_CancleModify, new CUIEventManager.OnUIEventHandler(this.OnCancleModifyEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_AddEquip, new CUIEventManager.OnUIEventHandler(this.OnAddEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_DeleteEquip, new CUIEventManager.OnUIEventHandler(this.OnDeleteEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_ShowConfirmRevertDefaultTip, new CUIEventManager.OnUIEventHandler(this.OnShowConfirmRevertDefaultTip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_RevertDefault, new CUIEventManager.OnUIEventHandler(this.OnRevertDefaultEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_Expand, new CUIEventManager.OnUIEventHandler(this.OnExpandCustomEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_PackUp, new CUIEventManager.OnUIEventHandler(this.OnPackUpCustomEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_HeroTypeListSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroTypeListSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_HeroListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_HeroListItemClick, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_OwnFlagChange, new CUIEventManager.OnUIEventHandler(this.OnHeroOwnFlagChange));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_ViewGodEquip, new CUIEventManager.OnUIEventHandler(this.OnViewGodEquip));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_GodEquipItemEnable, new CUIEventManager.OnUIEventHandler(this.OnGodEquipItemEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_GodEquipUseBtnClick, new CUIEventManager.OnUIEventHandler(this.OnGodEquipUseBtnClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_GodEquipReqTimeOut, new CUIEventManager.OnUIEventHandler(this.OnReqGodEquipTimeOut));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_BackEquipListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnBackEquipListElementEnable));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.CustomEquip_CircleTimerUp, new CUIEventManager.OnUIEventHandler(this.OnCircleTimerUp));
            Singleton<EventRouter>.GetInstance().AddEventHandler<SCPKG_GET_RANKING_LIST_RSP>(EventID.CUSTOM_EQUIP_RANK_LIST_GET, new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetCustomEquipRankList));
        }

        private bool IsCustomEquipPanelExpanded()
        {
            if (this.m_customEquipForm != null)
            {
                GameObject widget = this.m_customEquipForm.GetWidget(11);
                if (widget != null)
                {
                    return widget.activeSelf;
                }
            }
            return false;
        }

        private bool IsEditCustomRecommendEquipUseDefaultSetting()
        {
            if (this.m_editHeroID == 0)
            {
                return false;
            }
            ushort[] defaultRecommendEquipInfo = this.GetDefaultRecommendEquipInfo(this.m_editHeroID);
            if (defaultRecommendEquipInfo == null)
            {
                return false;
            }
            for (int i = 0; i < 6; i++)
            {
                if (this.m_editCustomRecommendEquips[i] != defaultRecommendEquipInfo[i])
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsEquipListNodeExsit()
        {
            if (this.m_customEquipForm != null)
            {
                GameObject widget = this.m_customEquipForm.GetWidget(5);
                if (widget != null)
                {
                    return (widget.transform.Find("Panel_EquipList") != null);
                }
            }
            return false;
        }

        private bool IsHeroCustomEquip(uint heroId, ref int cnt)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "CEquipSystem IsHeroCustomEquip role is null");
            if ((masterRoleInfo == null) || (masterRoleInfo.m_customRecommendEquipDictionary == null))
            {
                return false;
            }
            ushort[] numArray = null;
            cnt = 0;
            if (!masterRoleInfo.m_customRecommendEquipDictionary.TryGetValue(heroId, out numArray))
            {
                return false;
            }
            if (numArray != null)
            {
                for (int i = 0; i < numArray.Length; i++)
                {
                    CEquipInfo equipInfo = this.GetEquipInfo(numArray[i]);
                    if ((equipInfo != null) && (equipInfo.m_resEquipInBattle.bInvalid == 0))
                    {
                        cnt++;
                    }
                }
            }
            return true;
        }

        private void LoadEquipListNode()
        {
            if (this.m_customEquipForm != null)
            {
                GameObject widget = this.m_customEquipForm.GetWidget(5);
                if (widget != null)
                {
                    CUICommonSystem.LoadUIPrefab(s_EquipListNodePrefabPath, "Panel_EquipList", widget, this.m_customEquipForm);
                }
            }
        }

        private void OnAddEquip(CUIEvent uiEvent)
        {
            if (this.m_selEquipInfo == null)
            {
                Singleton<CUIManager>.GetInstance().OpenTips("CustomEquip_ChooseEquipTip", true, 1.5f, null, new object[0]);
            }
            else
            {
                int tag = uiEvent.m_eventParams.tag;
                if ((tag >= 0) && (tag < this.m_editCustomRecommendEquips.Length))
                {
                    this.m_editCustomRecommendEquips[tag] = this.m_selEquipInfo.m_equipID;
                }
                this.RefreshCustomEquips(false);
            }
        }

        private void OnBackEquipListElementEnable(CUIEvent uiEvent)
        {
            if (this.m_equipTree.m_backEquipIDs != null)
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                if (((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_equipTree.m_backEquipCount)) && (uiEvent.m_srcWidget != null))
                {
                    this.RefreshEquipTreeItem(uiEvent.m_srcWidget.transform, this.m_equipTree.m_backEquipIDs[srcWidgetIndexInBelongedList]);
                }
            }
        }

        private void OnCancleModifyEquip(CUIEvent uiEvent)
        {
            this.bEditEquip = false;
            this.RefreshEquipCustomPanel(false);
        }

        private void OnChangeHero(CUIEvent uiEvent)
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

        private void OnCircleTimerUp(CUIEvent uiEvent)
        {
            this.LoadEquipListNode();
            this.OnEquipListNodeLoaded();
        }

        private void OnConfirmModifyEquip(CUIEvent uiEvent)
        {
            this.SaveEditCustomRecommendEquip();
        }

        private void OnCustomEditItemClick(CUIEvent uiEvent)
        {
            CEquipInfo equipInfo = uiEvent.m_eventParams.battleEquipPar.equipInfo;
            Transform equipItemObj = uiEvent.m_eventParams.battleEquipPar.equipItemObj;
            if ((equipInfo != null) && (null != this.m_customEquipForm))
            {
                this.ClearCurSelectEquipItem();
                if (equipInfo.m_resEquipInBattle.bUsage != ((byte) this.m_curEquipUsage))
                {
                    GameObject widget = this.m_customEquipForm.GetWidget(0);
                    if (widget != null)
                    {
                        CUIListScript component = widget.GetComponent<CUIListScript>();
                        if (component != null)
                        {
                            component.SelectElement(equipInfo.m_resEquipInBattle.bUsage - 1, true);
                        }
                    }
                }
                this.m_equipRelationPath.Display(equipInfo.m_equipID, this.m_equipList[(int) this.m_curEquipUsage], this.m_equipInfoDictionary);
                this.m_selEquipItemObj = equipItemObj;
                this.SetEquipItemSelectFlag(this.m_selEquipItemObj, true);
                this.RefreshRightPanel(equipInfo);
            }
        }

        private void OnCustomEquipClose(CUIEvent uiEvent)
        {
            this.ClearCurSelectEquipItem();
            this.m_equipRelationPath.Clear();
            this.m_customEquipForm = null;
            this.bEditEquip = false;
            this.m_useGodEquip = false;
            this.m_revertDefaultEquip = false;
        }

        private void OnCustomEquipListSelect(CUIEvent uiEvent)
        {
            this.ClearCurSelectEquipItem();
            this.CloseRightPanel();
            CUIListScript srcWidgetScript = (CUIListScript) uiEvent.m_srcWidgetScript;
            if (srcWidgetScript != null)
            {
                this.m_curEquipUsage = (enEquipUsage) (srcWidgetScript.GetSelectedIndex() + 1);
                this.RefreshEquipListPanel(true);
            }
        }

        private void OnCustomEquipOpen(CUIEvent uiEvent)
        {
            this.m_customEquipForm = Singleton<CUIManager>.GetInstance().OpenForm(s_CustomRecommendEquipPath, false, true);
            this.m_curEquipUsage = enEquipUsage.PhyAttack;
            this.bEditEquip = false;
            this.m_useGodEquip = false;
            this.m_revertDefaultEquip = false;
            if (this.m_customEquipForm != null)
            {
                if (this.IsEquipListNodeExsit())
                {
                    this.OnEquipListNodeLoaded();
                }
                else
                {
                    GameObject obj2 = this.m_customEquipForm.GetWidget(6);
                    if (obj2 != null)
                    {
                        obj2.CustomSetActive(true);
                        CUITimerScript component = obj2.GetComponent<CUITimerScript>();
                        if (component != null)
                        {
                            component.StartTimer();
                        }
                    }
                }
                if (this.m_uiCustomEquipContentHeight == 0f)
                {
                    GameObject obj3 = this.m_customEquipForm.GetWidget(12);
                    if (obj3 != null)
                    {
                        this.m_uiCustomEquipContentHeight = (obj3.transform as RectTransform).rect.height;
                    }
                }
                GameObject widget = this.m_customEquipForm.GetWidget(0);
                CTextManager instance = Singleton<CTextManager>.GetInstance();
                string[] titleList = new string[] { instance.GetText("Equip_Usage_PhyAttack"), instance.GetText("Equip_Usage_MagicAttack"), instance.GetText("Equip_Usage_Defence"), instance.GetText("Equip_Usage_Move"), instance.GetText("Equip_Usage_Jungle") };
                CUICommonSystem.InitMenuPanel(widget, titleList, ((int) this.m_curEquipUsage) - 1);
                this.RefreshEquipCustomPanel(true);
                GameObject obj5 = this.m_customEquipForm.GetWidget(13);
                if (obj5 != null)
                {
                    obj5.CustomSetActive(!CSysDynamicBlock.bLobbyEntryBlocked);
                }
                MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEnterCustomRecommendEquip, new uint[0]);
            }
            CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_BeizhanBtn);
        }

        private void OnDeleteEquip(CUIEvent uiEvent)
        {
            int tag = uiEvent.m_eventParams.tag;
            if ((tag >= 0) && (tag < this.m_editCustomRecommendEquips.Length))
            {
                this.m_editCustomRecommendEquips[tag] = 0;
            }
            this.RefreshCustomEquips(false);
        }

        private void OnEquipListNodeLoaded()
        {
            if (null != this.m_customEquipForm)
            {
                GameObject widget = this.m_customEquipForm.GetWidget(6);
                if (widget != null)
                {
                    widget.CustomSetActive(false);
                }
                if (this.m_uiEquipItemHeight == 0f)
                {
                    GameObject equipListNodeWidget = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipItem);
                    if (equipListNodeWidget != null)
                    {
                        this.m_uiEquipItemHeight = (equipListNodeWidget.transform as RectTransform).rect.height;
                    }
                }
                if (this.m_uiEquipItemContentDefaultHeight == 0f)
                {
                    GameObject obj4 = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipItemContent);
                    if (obj4 != null)
                    {
                        this.m_uiEquipItemContentDefaultHeight = (obj4.transform as RectTransform).rect.height;
                    }
                }
                if (this.m_uiCustomEquipContentHeight == 0f)
                {
                    GameObject obj5 = this.m_customEquipForm.GetWidget(12);
                    if (obj5 != null)
                    {
                        this.m_uiCustomEquipContentHeight = (obj5.transform as RectTransform).rect.height;
                    }
                }
                this.InitEquipPathLine();
                this.RefreshEquipListPanel(true);
            }
        }

        private void OnEuipItemClick(CUIEvent uiEvent)
        {
            if (null != this.m_customEquipForm)
            {
                this.ClearCurSelectEquipItem();
                this.m_selEquipInfo = uiEvent.m_eventParams.battleEquipPar.equipInfo;
                this.m_selEquipItemObj = uiEvent.m_eventParams.battleEquipPar.equipItemObj;
                if ((this.m_selEquipInfo != null) && (this.m_selEquipItemObj != null))
                {
                    this.SetEquipItemSelectFlag(this.m_selEquipItemObj, true);
                    this.RefreshRightPanel(this.m_selEquipInfo);
                    this.m_equipRelationPath.Display(this.m_selEquipInfo.m_equipID, this.m_equipList[(int) this.m_curEquipUsage], this.m_equipInfoDictionary);
                }
            }
        }

        private void OnExpandCustomEquip(CUIEvent uiEvent)
        {
            if (null != this.m_customEquipForm)
            {
                GameObject widget = this.m_customEquipForm.GetWidget(12);
                GameObject obj3 = this.m_customEquipForm.GetWidget(11);
                if ((widget != null) && (obj3 != null))
                {
                    <OnExpandCustomEquip>c__AnonStorey69 storey = new <OnExpandCustomEquip>c__AnonStorey69();
                    obj3.CustomSetActive(true);
                    LeanTween.cancel(widget);
                    storey.customContentRectTransform = widget.transform as RectTransform;
                    Vector2 to = new Vector2(storey.customContentRectTransform.anchoredPosition.x, storey.customContentRectTransform.anchoredPosition.y + this.m_uiCustomEquipContentHeight);
                    LeanTween.value(widget, new Action<Vector2>(storey.<>m__51), storey.customContentRectTransform.anchoredPosition, to, this.c_moveAnimaTime);
                }
                GameObject equipListNodeWidget = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipItemContent);
                if (equipListNodeWidget != null)
                {
                    RectTransform transform = equipListNodeWidget.transform as RectTransform;
                    float x = transform.offsetMin.x;
                    float y = transform.offsetMin.y - this.m_uiEquipItemContentHightDiff;
                    transform.offsetMin = new Vector2(x, y);
                }
            }
        }

        private void OnGetCustomEquipRankList(SCPKG_GET_RANKING_LIST_RSP rankListRsp)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            CSDT_RANKING_LIST_SUCC stOfSucc = rankListRsp.stRankingListDetail.stOfSucc;
            if (stOfSucc.bNumberType == 0x16)
            {
                stEquipRankInfo info = new stEquipRankInfo {
                    equipRankItemCnt = (int) stOfSucc.dwItemNum,
                    rankDetail = stOfSucc.astItemDetail
                };
                if (this.m_equipRankItemDetail.ContainsKey(this.m_reqRankHeroId))
                {
                    this.m_equipRankItemDetail[this.m_reqRankHeroId] = info;
                }
                else
                {
                    this.m_equipRankItemDetail.Add(this.m_reqRankHeroId, info);
                }
                this.RefreshGodEquipForm(this.m_reqRankHeroId);
            }
        }

        private void OnGodEquipItemEnable(CUIEvent uiEvent)
        {
            stEquipRankInfo info;
            if (this.m_equipRankItemDetail.TryGetValue(this.m_reqRankHeroId, out info))
            {
                int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
                if (((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < info.equipRankItemCnt)) && (info.rankDetail != null))
                {
                    CSDT_RANKING_LIST_ITEM_INFO csdt_ranking_list_item_info = info.rankDetail[srcWidgetIndexInBelongedList];
                    if ((csdt_ranking_list_item_info != null) && (null != uiEvent.m_srcWidget))
                    {
                        Transform transform = uiEvent.m_srcWidget.transform;
                        Transform transform2 = transform.Find("Bg");
                        Transform transform3 = transform.Find("BgNo1");
                        Transform transform4 = transform.Find("123No");
                        Transform transform5 = transform.Find("NumText");
                        Transform transform6 = transform.Find("winCntText");
                        transform3.gameObject.CustomSetActive(srcWidgetIndexInBelongedList == 0);
                        transform2.gameObject.CustomSetActive(srcWidgetIndexInBelongedList > 0);
                        transform4.GetChild(0).gameObject.CustomSetActive(0 == srcWidgetIndexInBelongedList);
                        transform4.GetChild(1).gameObject.CustomSetActive(1 == srcWidgetIndexInBelongedList);
                        transform4.GetChild(2).gameObject.CustomSetActive(2 == srcWidgetIndexInBelongedList);
                        transform5.gameObject.CustomSetActive(srcWidgetIndexInBelongedList > 2);
                        transform5.GetComponent<Text>().text = csdt_ranking_list_item_info.dwRankNo.ToString();
                        uint num2 = (csdt_ranking_list_item_info.dwRankScore <= 0x5f5e0ff) ? csdt_ranking_list_item_info.dwRankScore : 0x5f5e0ff;
                        string[] args = new string[] { num2.ToString() };
                        transform6.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("CustomEquip_WinCntText", args);
                        int index = 0;
                        index = 0;
                        while (index < csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.bEquipNum)
                        {
                            ushort equipID = (ushort) csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.EquipID[index];
                            Transform transform7 = transform.Find("equipItem" + index);
                            if (transform7 != null)
                            {
                                Transform transform8 = transform7.Find("imgIcon");
                                CEquipInfo equipInfo = this.GetEquipInfo(equipID);
                                if (equipInfo != null)
                                {
                                    transform8.gameObject.CustomSetActive(true);
                                    transform8.GetComponent<Image>().SetSprite(equipInfo.m_equipIconPath, this.m_customEquipForm, true, false, false);
                                }
                                else
                                {
                                    transform8.gameObject.CustomSetActive(false);
                                }
                            }
                            index++;
                        }
                        while (index < 6)
                        {
                            Transform transform9 = transform.Find("equipItem" + index);
                            if (transform9 != null)
                            {
                                transform9.Find("imgIcon").gameObject.CustomSetActive(false);
                            }
                            index++;
                        }
                        Transform transform11 = transform.Find("useButton");
                        if (transform11 != null)
                        {
                            CUIEventScript component = transform11.GetComponent<CUIEventScript>();
                            if (component != null)
                            {
                                stUIEventParams eventParams = new stUIEventParams {
                                    tag = srcWidgetIndexInBelongedList
                                };
                                component.SetUIEvent(enUIEventType.Click, enUIEventID.CustomEquip_GodEquipUseBtnClick, eventParams);
                            }
                        }
                    }
                }
            }
        }

        private void OnGodEquipUseBtnClick(CUIEvent uiEvent)
        {
            stEquipRankInfo info;
            if (this.m_equipRankItemDetail.TryGetValue(this.m_reqRankHeroId, out info) && (((uiEvent.m_eventParams.tag >= 0) && (uiEvent.m_eventParams.tag < info.equipRankItemCnt)) && (info.rankDetail != null)))
            {
                CSDT_RANKING_LIST_ITEM_INFO csdt_ranking_list_item_info = info.rankDetail[uiEvent.m_eventParams.tag];
                this.EditCustomRecommendEquipByGodEquip(csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.bEquipNum, ref csdt_ranking_list_item_info.stExtraInfo.stDetailInfo.stCustomEquip.EquipID);
                this.m_useGodEquip = true;
                this.SaveEditCustomRecommendEquip();
                Singleton<CUIManager>.GetInstance().CloseForm(s_GodEquipPath);
            }
        }

        private void OnHeroListElementClick(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().CloseForm(s_ChooseHeroPath);
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                this.ClearCurSelectEquipItem();
                this.CloseRightPanel();
                masterRoleInfo.m_customRecommendEquipsLastChangedHeroID = uiEvent.m_eventParams.heroId;
                this.RefreshEquipCustomPanel(true);
            }
        }

        private void OnHeroListElementEnable(CUIEvent uiEvent)
        {
            int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
            if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_heroList.Count))
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                DebugHelper.Assert(masterRoleInfo != null, "CEquipSystem OnHeroListElementEnable role is null");
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
                                component.SetUIEvent(enUIEventType.Click, enUIEventID.CustomEquip_HeroListItemClick, eventParams);
                            }
                            Transform transform2 = transform.Find("equipedPanel");
                            if (transform2 != null)
                            {
                                int cnt = 0;
                                bool bActive = this.IsHeroCustomEquip(info2.dwCfgID, ref cnt);
                                transform2.gameObject.CustomSetActive(bActive);
                                if (bActive)
                                {
                                    Text text = transform2.Find("Text").GetComponent<Text>();
                                    if (cnt < 6)
                                    {
                                        string[] args = new string[] { cnt.ToString(), 6.ToString() };
                                        text.text = Singleton<CTextManager>.GetInstance().GetText("CustomEquip_EquipCnt", args);
                                    }
                                    else
                                    {
                                        text.text = Singleton<CTextManager>.GetInstance().GetText("CustomEquip_EquipComplete");
                                    }
                                }
                            }
                            Transform transform3 = transform.Find("TxtFree");
                            if (transform3 != null)
                            {
                                transform3.gameObject.CustomSetActive(masterRoleInfo.IsFreeHero(info2.dwCfgID));
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

        private void OnModifyEquip(CUIEvent uiEvent)
        {
            this.bEditEquip = true;
            this.RefreshEquipCustomPanel(false);
        }

        private void OnPackUpCustomEquip(CUIEvent uiEvent)
        {
            <OnPackUpCustomEquip>c__AnonStorey6B storeyb = new <OnPackUpCustomEquip>c__AnonStorey6B();
            if (null != this.m_customEquipForm)
            {
                GameObject widget = this.m_customEquipForm.GetWidget(12);
                storeyb.equipCustomPanel = this.m_customEquipForm.GetWidget(11);
                if ((widget != null) && (storeyb.equipCustomPanel != null))
                {
                    <OnPackUpCustomEquip>c__AnonStorey6A storeya = new <OnPackUpCustomEquip>c__AnonStorey6A {
                        <>f__ref$107 = storeyb
                    };
                    LeanTween.cancel(widget);
                    storeya.customContentRectTransform = widget.transform as RectTransform;
                    Vector2 to = new Vector2(storeya.customContentRectTransform.anchoredPosition.x, storeya.customContentRectTransform.anchoredPosition.y - this.m_uiCustomEquipContentHeight);
                    LeanTween.value(widget, new Action<Vector2>(storeya.<>m__52), storeya.customContentRectTransform.anchoredPosition, to, this.c_moveAnimaTime).setOnComplete(new System.Action(storeya.<>m__53));
                }
                GameObject equipListNodeWidget = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipItemContent);
                if (equipListNodeWidget != null)
                {
                    RectTransform transform = equipListNodeWidget.transform as RectTransform;
                    float x = transform.offsetMin.x;
                    float y = transform.offsetMin.y + this.m_uiEquipItemContentHightDiff;
                    transform.offsetMin = new Vector2(x, y);
                }
            }
        }

        private void OnReqGodEquipTimeOut(CUIEvent uiEvent)
        {
            this.RefreshGodEquipForm(this.m_reqRankHeroId);
        }

        private void OnRevertDefaultEquip(CUIEvent uiEvent)
        {
            this.m_revertDefaultEquip = true;
            this.RevertEditCustomRecommendEquipToDefault();
            this.SaveEditCustomRecommendEquip();
        }

        private void OnShowConfirmRevertDefaultTip(CUIEvent uiEvent)
        {
            string text = Singleton<CTextManager>.GetInstance().GetText("CustomEquip_ConfirmRevertDefaultTip");
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.CustomEquip_RevertDefault, enUIEventID.None, false);
        }

        private void OnViewEquipTree(CUIEvent uiEvent)
        {
            CEquipInfo equipInfo = uiEvent.m_eventParams.battleEquipPar.equipInfo;
            if (equipInfo != null)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_EquipTreePath);
                if (null == form)
                {
                    form = Singleton<CUIManager>.GetInstance().OpenForm(s_EquipTreePath, false, true);
                }
                if (form != null)
                {
                    this.RefreshEquipTreeForm(form, equipInfo);
                }
            }
        }

        private void OnViewGodEquip(CUIEvent uiEvent)
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "OnViewGodEquip role is null");
            if (masterRoleInfo != null)
            {
                this.m_reqRankHeroId = masterRoleInfo.m_customRecommendEquipsLastChangedHeroID;
                CSPkg msg = NetworkModule.CreateDefaultCSPKG(0xa2a);
                msg.stPkgData.stGetRankingListReq.bNumberType = 0x16;
                msg.stPkgData.stGetRankingListReq.iSubType = (int) masterRoleInfo.m_customRecommendEquipsLastChangedHeroID;
                Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, false);
                Singleton<CUIManager>.GetInstance().OpenForm(s_GodEquipPath, false, true);
                Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(Singleton<CTextManager>.GetInstance().GetText("CustomEquip_ReqGodEquipTip"), 10, enUIEventID.CustomEquip_GodEquipReqTimeOut);
            }
        }

        public void OpenTipsOnSvrRsp()
        {
            if (this.m_useGodEquip)
            {
                this.m_useGodEquip = false;
                Singleton<CUIManager>.GetInstance().OpenTips("CustomEquip_UseGodEquipTip", true, 1.5f, null, new object[0]);
            }
            if (this.m_revertDefaultEquip)
            {
                this.m_revertDefaultEquip = false;
                Singleton<CUIManager>.GetInstance().OpenTips("CustomEquip_RevertDefaultTip", true, 1.5f, null, new object[0]);
            }
        }

        [MessageHandler(0x1454)]
        public static void RecieveSCRecoverSystemEquipRsp(CSPkg csPkg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (csPkg.stPkgData.stRecoverSystemEquipChgRsp.bResult == 0)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    uint dwHeroId = csPkg.stPkgData.stRecoverSystemEquipChgRsp.dwHeroId;
                    masterRoleInfo.m_customRecommendEquipsLastChangedHeroID = dwHeroId;
                    masterRoleInfo.m_customRecommendEquipDictionary.Remove(dwHeroId);
                    Singleton<CEquipSystem>.GetInstance().RefreshEquipCustomPanel(true);
                    Singleton<CEquipSystem>.GetInstance().OpenTipsOnSvrRsp();
                }
            }
        }

        [MessageHandler(0x1452)]
        public static void RecieveSCSelfDefineHeroEquipChgRsp(CSPkg csPkg)
        {
            Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
            if (csPkg.stPkgData.stSelfDefineHeroEquipChgRsp.bResult == 0)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    uint dwHeroId = csPkg.stPkgData.stSelfDefineHeroEquipChgRsp.stHeroEquipChgInfo.dwHeroId;
                    masterRoleInfo.m_customRecommendEquipsLastChangedHeroID = dwHeroId;
                    ushort[] numArray = null;
                    if (masterRoleInfo.m_customRecommendEquipDictionary.TryGetValue(dwHeroId, out numArray))
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            numArray[i] = (ushort) csPkg.stPkgData.stSelfDefineHeroEquipChgRsp.stHeroEquipChgInfo.HeroEquipList[i];
                        }
                    }
                    else
                    {
                        numArray = new ushort[6];
                        for (int j = 0; j < 6; j++)
                        {
                            numArray[j] = (ushort) csPkg.stPkgData.stSelfDefineHeroEquipChgRsp.stHeroEquipChgInfo.HeroEquipList[j];
                        }
                        masterRoleInfo.m_customRecommendEquipDictionary.Add(dwHeroId, numArray);
                    }
                    Singleton<CEquipSystem>.GetInstance().RefreshEquipCustomPanel(true);
                    Singleton<CEquipSystem>.GetInstance().OpenTipsOnSvrRsp();
                }
            }
        }

        private void RefreshCustomEquipHero()
        {
            if (null != this.m_customEquipForm)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                DebugHelper.Assert(masterRoleInfo != null, "RefreshEquipCustomPanel role is null");
                if (masterRoleInfo != null)
                {
                    GameObject widget = this.m_customEquipForm.GetWidget(7);
                    ResHeroCfgInfo dataByKey = null;
                    if (masterRoleInfo.m_customRecommendEquipsLastChangedHeroID == 0)
                    {
                        dataByKey = GameDataMgr.heroDatabin.GetDataByKey(masterRoleInfo.GetFirstHeroId());
                    }
                    else
                    {
                        dataByKey = GameDataMgr.heroDatabin.GetDataByKey(masterRoleInfo.m_customRecommendEquipsLastChangedHeroID);
                    }
                    if (dataByKey != null)
                    {
                        masterRoleInfo.m_customRecommendEquipsLastChangedHeroID = dataByKey.dwCfgID;
                        CUICommonSystem.SetHeroItemImage(this.m_customEquipForm, widget.gameObject, dataByKey.szImagePath, enHeroHeadType.enIcon, false);
                    }
                }
            }
        }

        private void RefreshCustomEquips(bool bInitEditEquips)
        {
            if (null != this.m_customEquipForm)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                if (masterRoleInfo != null)
                {
                    bool useCustomRecommendEquips = false;
                    if (bInitEditEquips)
                    {
                        this.InitializeEditCustomRecommendEquips(masterRoleInfo.m_customRecommendEquipsLastChangedHeroID, ref useCustomRecommendEquips);
                    }
                    GameObject widget = this.m_customEquipForm.GetWidget(1);
                    for (int i = 0; i < this.m_editCustomRecommendEquips.Length; i++)
                    {
                        Transform equipItem = widget.transform.Find("equipItem" + i);
                        if (equipItem != null)
                        {
                            Transform transform2 = equipItem.Find("addButton");
                            Transform transform3 = equipItem.Find("deleteButton");
                            Transform transform4 = equipItem.Find("imgIcon");
                            CEquipInfo equipInfo = this.GetEquipInfo(this.m_editCustomRecommendEquips[i]);
                            if (equipInfo != null)
                            {
                                transform4.gameObject.CustomSetActive(true);
                                this.RefreshEquipItemSimpleInfo(equipItem, equipInfo);
                                CUIMiniEventScript component = transform4.GetComponent<CUIMiniEventScript>();
                                if (component != null)
                                {
                                    stUIEventParams eventParams = new stUIEventParams();
                                    eventParams.battleEquipPar.equipInfo = equipInfo;
                                    eventParams.battleEquipPar.equipItemObj = equipItem;
                                    component.SetUIEvent(enUIEventType.Click, enUIEventID.CustomEquip_EditItemClick, eventParams);
                                }
                                transform3.gameObject.CustomSetActive(this.bEditEquip);
                                transform2.gameObject.CustomSetActive(false);
                                if (this.bEditEquip)
                                {
                                    CUIMiniEventScript script2 = transform3.GetComponent<CUIMiniEventScript>();
                                    if (script2 != null)
                                    {
                                        stUIEventParams params2 = new stUIEventParams {
                                            tag = i
                                        };
                                        script2.SetUIEvent(enUIEventType.Click, enUIEventID.CustomEquip_DeleteEquip, params2);
                                    }
                                }
                            }
                            else
                            {
                                transform4.gameObject.CustomSetActive(false);
                                transform3.gameObject.CustomSetActive(false);
                                transform2.gameObject.CustomSetActive(this.bEditEquip);
                                this.SetEquipItemSelectFlag(equipItem, false);
                                if (this.bEditEquip)
                                {
                                    CUIMiniEventScript script3 = transform2.GetComponent<CUIMiniEventScript>();
                                    if (script3 != null)
                                    {
                                        stUIEventParams params3 = new stUIEventParams {
                                            tag = i
                                        };
                                        script3.SetUIEvent(enUIEventType.Click, enUIEventID.CustomEquip_AddEquip, params3);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RefreshEquipBackList(Transform equipBackList, uint cnt, ref ushort[] backEquipIDs)
        {
            if (null != equipBackList)
            {
                equipBackList.GetComponent<CUIListScript>().SetElementAmount((int) cnt);
            }
        }

        private void RefreshEquipCustomPanel(bool bRefreshHero)
        {
            if (null != this.m_customEquipForm)
            {
                CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
                DebugHelper.Assert(masterRoleInfo != null, "RefreshEquipCustomPanel role is null");
                if (masterRoleInfo != null)
                {
                    if (bRefreshHero)
                    {
                        this.RefreshCustomEquipHero();
                    }
                    this.RefreshCustomEquips(true);
                    GameObject widget = this.m_customEquipForm.GetWidget(8);
                    GameObject obj3 = this.m_customEquipForm.GetWidget(9);
                    GameObject obj4 = this.m_customEquipForm.GetWidget(10);
                    if (widget != null)
                    {
                        widget.CustomSetActive(!this.bEditEquip);
                    }
                    if (obj3 != null)
                    {
                        obj3.CustomSetActive(this.bEditEquip);
                    }
                    if (obj4 != null)
                    {
                        obj4.CustomSetActive(this.bEditEquip);
                    }
                }
            }
        }

        private void RefreshEquipItem(Transform equipItem, ushort equipID)
        {
            if (((null != equipItem) && (null != this.m_customEquipForm)) && (equipID != 0))
            {
                CEquipInfo equipInfo = this.GetEquipInfo(equipID);
                if (equipInfo == null)
                {
                    DebugHelper.Assert(equipInfo != null, "GetEquipInfo is null equipId = " + equipID);
                }
                else
                {
                    this.RefreshEquipItemSimpleInfo(equipItem, equipInfo);
                    ResEquipInBattle resEquipInBattle = equipInfo.m_resEquipInBattle;
                    if (resEquipInBattle != null)
                    {
                        equipItem.Find("imgIcon").GetComponent<Image>().SetSprite(equipInfo.m_equipIconPath, this.m_customEquipForm, true, false, false);
                        equipItem.Find("nameText").GetComponent<Text>().text = equipInfo.m_equipName;
                        equipItem.Find("priceText").GetComponent<Text>().text = ((uint) resEquipInBattle.dwBuyPrice).ToString();
                        CUIMiniEventScript component = equipItem.GetComponent<CUIMiniEventScript>();
                        if (component != null)
                        {
                            stUIEventParams eventParams = new stUIEventParams();
                            eventParams.battleEquipPar.equipInfo = equipInfo;
                            eventParams.battleEquipPar.equipItemObj = equipItem;
                            component.SetUIEvent(enUIEventType.Click, enUIEventID.CustomEquip_ItemClick, eventParams);
                        }
                    }
                }
            }
        }

        private void RefreshEquipItemSimpleInfo(Transform equipItem, CEquipInfo equipInfo)
        {
            if (((null != equipItem) && (equipInfo != null)) && (equipInfo.m_resEquipInBattle != null))
            {
                equipItem.Find("imgIcon").GetComponent<Image>().SetSprite(equipInfo.m_equipIconPath, this.m_customEquipForm, true, false, false);
            }
        }

        private void RefreshEquipLevelPanel(Transform equipPanel, int level)
        {
            if (null != equipPanel)
            {
                List<ushort> list = this.m_equipList[(int) this.m_curEquipUsage][level - 1];
                int count = list.Count;
                int num2 = 0;
                num2 = 0;
                while ((num2 < 8) && (num2 < count))
                {
                    Transform equipItem = equipPanel.Find(string.Format("equipItem{0}", num2));
                    this.RefreshEquipItem(equipItem, list[num2]);
                    CanvasGroup component = equipItem.GetComponent<CanvasGroup>();
                    if (component != null)
                    {
                        component.alpha = 1f;
                        component.blocksRaycasts = true;
                    }
                    num2++;
                }
                while (num2 < 8)
                {
                    CanvasGroup group2 = equipPanel.Find(string.Format("equipItem{0}", num2)).GetComponent<CanvasGroup>();
                    if (group2 != null)
                    {
                        group2.alpha = 0f;
                        group2.blocksRaycasts = false;
                    }
                    num2++;
                }
            }
        }

        private void RefreshEquipListPanel(bool isSwichUsage)
        {
            if (null != this.m_customEquipForm)
            {
                GameObject equipListNodeWidget = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipPanel_Level1);
                if (equipListNodeWidget != null)
                {
                    this.RefreshEquipLevelPanel(equipListNodeWidget.transform, 1);
                }
                GameObject obj3 = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipPanel_Level2);
                if (obj3 != null)
                {
                    this.RefreshEquipLevelPanel(obj3.transform, 2);
                }
                GameObject obj4 = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipPanel_Level3);
                if (obj4 != null)
                {
                    this.RefreshEquipLevelPanel(obj4.transform, 3);
                }
                if (isSwichUsage)
                {
                    int count = 0;
                    List<ushort>[] listArray = this.m_equipList[(int) this.m_curEquipUsage];
                    for (int i = 0; i < listArray.Length; i++)
                    {
                        if (listArray[i].Count > count)
                        {
                            count = listArray[i].Count;
                        }
                    }
                    float num3 = this.m_uiEquipItemContentDefaultHeight - ((8 - count) * this.m_uiEquipItemHeight);
                    GameObject obj5 = this.GetEquipListNodeWidget(enEquipListNodeWidget.EquipItemContent);
                    if (obj5 != null)
                    {
                        RectTransform transform = obj5.transform as RectTransform;
                        if (this.IsCustomEquipPanelExpanded())
                        {
                            transform.offsetMin = new Vector2(0f, -num3 - this.m_uiEquipItemContentHightDiff);
                        }
                        else
                        {
                            transform.offsetMin = new Vector2(0f, -num3);
                        }
                        transform.offsetMax = new Vector2(0f, 0f);
                    }
                }
            }
        }

        private void RefreshEquipTreeForm(CUIFormScript equipTreeForm, CEquipInfo equipInfo)
        {
            if ((null != equipTreeForm) && (equipInfo != null))
            {
                this.GetEquipTree(equipInfo.m_equipID, ref this.m_equipTree);
                GameObject widget = equipTreeForm.GetWidget(0);
                if (widget != null)
                {
                    this.RefreshEquipTreePanel(widget.transform, ref this.m_equipTree, equipInfo);
                }
                GameObject obj3 = equipTreeForm.GetWidget(1);
                if (obj3 != null)
                {
                    this.RefreshEquipBackList(obj3.transform, this.m_equipTree.m_backEquipCount, ref this.m_equipTree.m_backEquipIDs);
                }
                GameObject obj4 = equipTreeForm.GetWidget(2);
                if (obj4 != null)
                {
                    obj4.transform.Find("equipNameText").GetComponent<Text>().text = equipInfo.m_equipName;
                    obj4.transform.Find("equipPropertyDescText").GetComponent<Text>().text = equipInfo.m_equipPropertyDesc;
                }
                GameObject obj5 = equipTreeForm.GetWidget(3);
                if (obj5 != null)
                {
                    obj5.GetComponent<Text>().text = ((uint) equipInfo.m_resEquipInBattle.dwBuyPrice).ToString();
                }
            }
        }

        private void RefreshEquipTreeItem(Transform equipItem, ushort equipID)
        {
            if (((null != equipItem) && (null != this.m_customEquipForm)) && (equipID != 0))
            {
                CEquipInfo equipInfo = this.GetEquipInfo(equipID);
                if (equipInfo == null)
                {
                    DebugHelper.Assert(equipInfo != null, "GetEquipInfo is null equipId = " + equipID);
                }
                else
                {
                    this.RefreshEquipItemSimpleInfo(equipItem, equipInfo);
                    if (equipInfo.m_resEquipInBattle != null)
                    {
                        CUIMiniEventScript component = equipItem.GetComponent<CUIMiniEventScript>();
                        if (component != null)
                        {
                            stUIEventParams eventParams = new stUIEventParams();
                            eventParams.battleEquipPar.equipInfo = equipInfo;
                            component.SetUIEvent(enUIEventType.Click, enUIEventID.CustomEquip_ViewEquipTree, eventParams);
                        }
                    }
                }
            }
        }

        private void RefreshEquipTreePanel(Transform equipTreePanel, ref stEquipTree equipTree, CEquipInfo equipInfo)
        {
            if ((null != equipTreePanel) && (equipInfo != null))
            {
                Transform equipItem = equipTreePanel.transform.Find("rootEquipItem");
                this.RefreshEquipTreeItem(equipItem, equipInfo.m_equipID);
                Transform lineGroupPanel = equipTreePanel.transform.Find("lineGroupPanel");
                this.RefreshLineGroupPanel(lineGroupPanel, 3, (int) equipTree.m_2ndEquipCount);
                Transform transform3 = equipTreePanel.transform.Find("preEquipGroupPanel");
                if (null != transform3)
                {
                    ushort equipID = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        equipID = equipTree.m_2ndEquipIDs[i];
                        Transform transform4 = transform3.Find("preEquipGroup" + i);
                        if (transform4 != null)
                        {
                            transform4.gameObject.CustomSetActive(equipID > 0);
                            if (equipID > 0)
                            {
                                Transform transform5 = transform4.Find("2ndEquipItem");
                                this.RefreshEquipTreeItem(transform5, equipID);
                                lineGroupPanel = transform4.transform.Find("lineGroupPanel");
                                this.RefreshLineGroupPanel(lineGroupPanel, 2, (int) equipTree.m_3rdEquipCounts[i]);
                                ushort num3 = 0;
                                for (int j = 0; j < 2; j++)
                                {
                                    num3 = equipTree.m_3rdEquipIDs[i][j];
                                    Transform transform6 = transform4.Find("preEquipPanel/3rdEquipItem" + j);
                                    transform6.gameObject.CustomSetActive(num3 > 0);
                                    this.RefreshEquipTreeItem(transform6, num3);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RefreshGodEquipForm(uint heroId)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(s_GodEquipPath);
            if (null != form)
            {
                Transform transform = form.transform.Find("Panel/godEquipPanel/godEquipList");
                if (null != transform)
                {
                    stEquipRankInfo info;
                    int amount = 0;
                    if (this.m_equipRankItemDetail.TryGetValue(heroId, out info))
                    {
                        amount = info.equipRankItemCnt;
                    }
                    CUIListScript component = transform.GetComponent<CUIListScript>();
                    if (component != null)
                    {
                        component.SetElementAmount(amount);
                    }
                    if (amount == 0)
                    {
                        Transform transform2 = form.transform.Find("Panel/godEquipPanel/info_node");
                        if (transform2 != null)
                        {
                            transform2.gameObject.CustomSetActive(true);
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

        private void RefreshLineGroupPanel(Transform lineGroupPanel, int maxLineCnt, int curLineCnt)
        {
            if (null != lineGroupPanel)
            {
                for (int i = 0; i < maxLineCnt; i++)
                {
                    Transform transform = lineGroupPanel.Find("linePanel" + i);
                    if (transform != null)
                    {
                        transform.gameObject.CustomSetActive((i + 1) == curLineCnt);
                    }
                }
            }
        }

        private void RefreshRightPanel(CEquipInfo equipInfo)
        {
            if ((null != this.m_customEquipForm) && (equipInfo != null))
            {
                GameObject widget = this.m_customEquipForm.GetWidget(2);
                if (widget != null)
                {
                    widget.CustomSetActive(true);
                    widget.transform.Find("equipNameText").GetComponent<Text>().text = equipInfo.m_equipName;
                    Text text2 = widget.transform.Find("Panel_euipProperty/equipPropertyDescText").GetComponent<Text>();
                    text2.text = equipInfo.m_equipPropertyDesc;
                    RectTransform transform = text2.transform as RectTransform;
                    transform.anchoredPosition = new Vector2(0f, 0f);
                }
                GameObject obj3 = this.m_customEquipForm.GetWidget(3);
                obj3.CustomSetActive(false);
                obj3.transform.Find("buyPriceText").GetComponent<Text>().text = ((uint) equipInfo.m_resEquipInBattle.dwBuyPrice).ToString();
                GameObject obj4 = this.m_customEquipForm.GetWidget(4);
                obj4.CustomSetActive(true);
                CUIEventScript component = obj4.GetComponent<CUIEventScript>();
                if (component != null)
                {
                    stUIEventParams eventParams = new stUIEventParams();
                    eventParams.battleEquipPar.equipInfo = equipInfo;
                    component.SetUIEvent(enUIEventType.Click, enUIEventID.CustomEquip_ViewEquipTree, eventParams);
                }
            }
        }

        private void ResetHeroList(enHeroJobType jobType, bool bOwn)
        {
            this.m_heroList.Clear();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            DebugHelper.Assert(masterRoleInfo != null, "CEquipSystem ResetHeroList role is null");
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

        private void RevertEditCustomRecommendEquipToDefault()
        {
            ushort[] defaultRecommendEquipInfo = this.GetDefaultRecommendEquipInfo(this.m_editHeroID);
            if (defaultRecommendEquipInfo != null)
            {
                for (int i = 0; i < 6; i++)
                {
                    this.m_editCustomRecommendEquips[i] = defaultRecommendEquipInfo[i];
                }
            }
            else
            {
                for (int j = 0; j < 6; j++)
                {
                    this.m_editCustomRecommendEquips[j] = 0;
                }
            }
        }

        private void SaveEditCustomRecommendEquip()
        {
            if (this.m_editHeroID != 0)
            {
                this.bEditEquip = false;
                if (this.IsEditCustomRecommendEquipUseDefaultSetting())
                {
                    CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1453);
                    msg.stPkgData.stRecoverSystemEquipChgReq.dwHeroId = this.m_editHeroID;
                    Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
                }
                else
                {
                    CSPkg pkg2 = NetworkModule.CreateDefaultCSPKG(0x1451);
                    pkg2.stPkgData.stSelfDefineHeroEquipChgReq.stHeroEquipChgInfo.dwHeroId = this.m_editHeroID;
                    for (int i = 0; i < 6; i++)
                    {
                        pkg2.stPkgData.stSelfDefineHeroEquipChgReq.stHeroEquipChgInfo.HeroEquipList[i] = this.m_editCustomRecommendEquips[i];
                    }
                    Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref pkg2, true);
                }
            }
        }

        private void SetEquipItemSelectFlag(Transform equipItemObj, bool bSelect)
        {
            if (equipItemObj != null)
            {
                Transform transform = equipItemObj.Find("selectImg");
                if (transform != null)
                {
                    transform.gameObject.CustomSetActive(bSelect);
                }
            }
        }

        private void UinitUIEventListener()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnCustomEquipOpen));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_FormClose, new CUIEventManager.OnUIEventHandler(this.OnCustomEquipClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_UsageListSelect, new CUIEventManager.OnUIEventHandler(this.OnCustomEquipListSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_ItemClick, new CUIEventManager.OnUIEventHandler(this.OnEuipItemClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_EditItemClick, new CUIEventManager.OnUIEventHandler(this.OnCustomEditItemClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_ViewEquipTree, new CUIEventManager.OnUIEventHandler(this.OnViewEquipTree));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_ChangeHero, new CUIEventManager.OnUIEventHandler(this.OnChangeHero));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_ModifyEquip, new CUIEventManager.OnUIEventHandler(this.OnModifyEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_ConfirmModify, new CUIEventManager.OnUIEventHandler(this.OnConfirmModifyEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_CancleModify, new CUIEventManager.OnUIEventHandler(this.OnCancleModifyEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_AddEquip, new CUIEventManager.OnUIEventHandler(this.OnAddEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_DeleteEquip, new CUIEventManager.OnUIEventHandler(this.OnDeleteEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_ShowConfirmRevertDefaultTip, new CUIEventManager.OnUIEventHandler(this.OnShowConfirmRevertDefaultTip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_RevertDefault, new CUIEventManager.OnUIEventHandler(this.OnRevertDefaultEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_Expand, new CUIEventManager.OnUIEventHandler(this.OnExpandCustomEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_PackUp, new CUIEventManager.OnUIEventHandler(this.OnPackUpCustomEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_ViewGodEquip, new CUIEventManager.OnUIEventHandler(this.OnViewGodEquip));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_HeroTypeListSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroTypeListSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_HeroListItemEnable, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_HeroListItemClick, new CUIEventManager.OnUIEventHandler(this.OnHeroListElementClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_OwnFlagChange, new CUIEventManager.OnUIEventHandler(this.OnHeroOwnFlagChange));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_GodEquipItemEnable, new CUIEventManager.OnUIEventHandler(this.OnGodEquipItemEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_GodEquipUseBtnClick, new CUIEventManager.OnUIEventHandler(this.OnGodEquipUseBtnClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_GodEquipReqTimeOut, new CUIEventManager.OnUIEventHandler(this.OnReqGodEquipTimeOut));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_BackEquipListElementEnable, new CUIEventManager.OnUIEventHandler(this.OnBackEquipListElementEnable));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.CustomEquip_CircleTimerUp, new CUIEventManager.OnUIEventHandler(this.OnCircleTimerUp));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<SCPKG_GET_RANKING_LIST_RSP>(EventID.CUSTOM_EQUIP_RANK_LIST_GET, new Action<SCPKG_GET_RANKING_LIST_RSP>(this.OnGetCustomEquipRankList));
        }

        public override void UnInit()
        {
            this.UinitUIEventListener();
        }

        [CompilerGenerated]
        private sealed class <OnExpandCustomEquip>c__AnonStorey69
        {
            internal RectTransform customContentRectTransform;

            internal void <>m__51(Vector2 pos)
            {
                this.customContentRectTransform.anchoredPosition = pos;
            }
        }

        [CompilerGenerated]
        private sealed class <OnPackUpCustomEquip>c__AnonStorey6A
        {
            internal CEquipSystem.<OnPackUpCustomEquip>c__AnonStorey6B <>f__ref$107;
            internal RectTransform customContentRectTransform;

            internal void <>m__52(Vector2 pos)
            {
                this.customContentRectTransform.anchoredPosition = pos;
            }

            internal void <>m__53()
            {
                this.<>f__ref$107.equipCustomPanel.CustomSetActive(false);
            }
        }

        [CompilerGenerated]
        private sealed class <OnPackUpCustomEquip>c__AnonStorey6B
        {
            internal GameObject equipCustomPanel;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct stEquipRankInfo
        {
            public int equipRankItemCnt;
            public CSDT_RANKING_LIST_ITEM_INFO[] rankDetail;
        }
    }
}

