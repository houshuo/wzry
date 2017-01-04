using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class InBattleShortcutMenu
{
    private GameObject CancelBtn;
    private TabElement curSelecTabElement;
    public static readonly string InBattleShortcutMenu_FORM_PATH = "UGUI/Form/System/Chat/Form_ShortcutChat.prefab";
    private bool m_bEdit;
    private CUIFormScript m_form;
    private CUIListScript m_leftContentList;
    private CUIListScript m_rightContentList;
    private int m_rightTabIndex = -1;
    private CUIListScript m_rightTablist;
    private GameObject ReviseBtn;
    private GameObject SureBtn;

    private void _refresh_left_list(CUIListScript listScript, ListView<TabElement> data_list)
    {
        if (((listScript != null) && (data_list != null)) && (data_list.Count != 0))
        {
            int count = data_list.Count;
            listScript.SetElementAmount(count);
        }
    }

    private void _refresh_right_list(CUIListScript listScript, ListView<TabElement> data_list)
    {
        if (((listScript != null) && (data_list != null)) && (data_list.Count != 0))
        {
            int count = data_list.Count;
            listScript.SetElementAmount(count);
        }
    }

    public void Clear()
    {
        this.UnRegEvent();
        this.m_bEdit = false;
        this.m_rightTabIndex = -1;
        this.curSelecTabElement = null;
        this.m_rightTablist = null;
        this.m_rightContentList = null;
        this.m_leftContentList = null;
        this.ReviseBtn = null;
        this.CancelBtn = null;
        this.SureBtn = null;
        this.m_form = null;
    }

    private void On_InBatShortcut_Cancle(CUIEvent uievent)
    {
        InBattleMsgMgr instance = Singleton<InBattleMsgMgr>.instance;
        instance.SyncData(instance.lastMenuEntList, instance.menuEntList);
        this.EditMode = false;
        this.RefreshLeft();
    }

    private void On_InBatShortcut_Change(CUIEvent uievent)
    {
        this.EditMode = true;
    }

    private void On_InBatShortcut_CloseForm(CUIEvent uievent)
    {
        this.On_InBatShortcut_Cancle(null);
        this.Clear();
    }

    private void On_InBatShortcut_Delete(CUIEvent uievent)
    {
        int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
        ListView<TabElement> menuEntList = Singleton<InBattleMsgMgr>.instance.menuEntList;
        if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < menuEntList.Count))
        {
            TabElement data = menuEntList[srcWidgetIndexInBelongedList];
            data.cfgId = 0;
            data.configContent = string.Empty;
            this.SetLeftItemState(uievent.m_srcWidget.transform.parent.parent.gameObject, data, EItemState.Record);
        }
    }

    private void On_InBatShortcut_LeftItem_Enable(CUIEvent uievent)
    {
        int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
        ListView<TabElement> menuEntList = Singleton<InBattleMsgMgr>.instance.menuEntList;
        if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < menuEntList.Count))
        {
            TabElement data = menuEntList[srcWidgetIndexInBelongedList];
            if (data != null)
            {
                if (!this.EditMode)
                {
                    this.SetLeftItemState(uievent.m_srcWidget, data, EItemState.Normal);
                }
                else if (data.cfgId == 0)
                {
                    this.SetLeftItemState(uievent.m_srcWidget, data, EItemState.Record);
                }
                else
                {
                    this.SetLeftItemState(uievent.m_srcWidget, data, EItemState.Delete);
                }
            }
        }
    }

    private void On_InBatShortcut_OK(CUIEvent uievent)
    {
        InBattleMsgMgr instance = Singleton<InBattleMsgMgr>.instance;
        instance.SyncData(instance.menuEntList, instance.lastMenuEntList);
        InBattleMsgNetCore.SendShortCut_Config(instance.menuEntList);
        this.EditMode = false;
    }

    private void On_InBatShortcut_Record(CUIEvent uievent)
    {
        if (this.curSelecTabElement == null)
        {
            Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.instance.GetText("InBattleShortcutMenu_SelectRight"), false, 1.5f, null, new object[0]);
        }
        else
        {
            CUIListElementScript component = uievent.m_srcWidget.transform.parent.parent.GetComponent<CUIListElementScript>();
            if ((component != null) && ((component.m_indexInlist >= 0) && (component.m_indexInlist < Singleton<InBattleMsgMgr>.instance.menuEntList.Count)))
            {
                TabElement element = Singleton<InBattleMsgMgr>.instance.menuEntList[component.m_indexInlist];
                if (element != null)
                {
                    element.cfgId = this.curSelecTabElement.cfgId;
                    element.configContent = this.curSelecTabElement.configContent;
                    this.RefreshLeft();
                }
            }
        }
    }

    private void On_InBatShortcut_RightItem_Click(CUIEvent uievent)
    {
        int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
        TabElement element = Singleton<InBattleMsgMgr>.instance.GeTabElement(this.TabIndex, srcWidgetIndexInBelongedList);
        if (element != null)
        {
            this.curSelecTabElement = element;
        }
    }

    private void On_InBatShortcut_RightItem_Enable(CUIEvent uievent)
    {
        int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
        TabElement element = Singleton<InBattleMsgMgr>.instance.GeTabElement(this.TabIndex, srcWidgetIndexInBelongedList);
        InBattleMsgShower component = uievent.m_srcWidget.GetComponent<InBattleMsgShower>();
        if ((component != null) && (element != null))
        {
            string configContent = element.configContent;
            if (element.camp == 2)
            {
                configContent = "[全部] " + configContent;
            }
            component.Set(element.cfgId, configContent);
        }
    }

    private void On_InBatShortcut_RightTab_Change(CUIEvent uievent)
    {
        int selectedIndex = uievent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
        this.TabIndex = selectedIndex;
    }

    private void On_InBatShortcut_UseDefault(CUIEvent uievent)
    {
        Singleton<CUIManager>.instance.OpenMessageBoxWithCancel(Singleton<CTextManager>.instance.GetText("InBattleShortcutMenu_UseDefault"), enUIEventID.InBatShortcut_UseDefault_Ok, enUIEventID.None, false);
    }

    private void On_InBatShortcut_UseDefault_Ok(CUIEvent uievent)
    {
        Singleton<CUIManager>.instance.OpenTips("InBattleShortcutMenu_UseDefault_OK", true, 1.5f, null, new object[0]);
        Singleton<InBattleMsgMgr>.instance.UseDefaultShortcut();
        this.RefreshLeft();
    }

    public void OpenForm()
    {
        this.m_form = Singleton<CUIManager>.GetInstance().OpenForm(InBattleShortcutMenu_FORM_PATH, false, true);
        DebugHelper.Assert(this.m_form != null, "InBattleShortcutMenu m_form == null");
        if (this.m_form != null)
        {
            this.RegEvent();
            this.m_rightTablist = this.m_form.transform.Find("Panel_Main/Panel_Right/Menu").GetComponent<CUIListScript>();
            this.m_rightContentList = this.m_form.transform.Find("Panel_Main/Panel_Right/ShortcutList").GetComponent<CUIListScript>();
            DebugHelper.Assert(this.m_rightTablist != null, "InBattleShortcutMenu m_rightTablist == null");
            DebugHelper.Assert(this.m_rightContentList != null, "InBattleShortcutMenu m_rightContentList == null");
            if ((this.m_rightTablist != null) && (this.m_rightContentList != null))
            {
                UT.SetTabList(Singleton<InBattleMsgMgr>.instance.title_list, 0, this.m_rightTablist);
                this.m_leftContentList = this.m_form.transform.Find("Panel_Main/Panel_Left/SelectedList").GetComponent<CUIListScript>();
                this.ReviseBtn = this.m_form.transform.Find("Panel_Main/Panel_Left/BtnGroup/ReviseBtn").gameObject;
                this.CancelBtn = this.m_form.transform.Find("Panel_Main/Panel_Left/BtnGroup/CancelBtn").gameObject;
                this.SureBtn = this.m_form.transform.Find("Panel_Main/Panel_Left/BtnGroup/SureBtn").gameObject;
                DebugHelper.Assert(this.m_leftContentList != null, "InBattleShortcutMenu m_leftContentList == null");
                DebugHelper.Assert(this.ReviseBtn != null, "InBattleShortcutMenu ReviseBtn == null");
                DebugHelper.Assert(this.CancelBtn != null, "InBattleShortcutMenu CancelBtn == null");
                DebugHelper.Assert(this.SureBtn != null, "InBattleShortcutMenu SureBtn == null");
                this.SetBtnNormal();
                this.RefreshLeft();
            }
        }
    }

    public void Refresh_Right_List(int tabIndex)
    {
        InBattleMsgMgr instance = Singleton<InBattleMsgMgr>.instance;
        if (tabIndex < instance.title_list.Count)
        {
            ListView<TabElement> view = null;
            if (tabIndex < instance.title_list.Count)
            {
                string key = instance.title_list[tabIndex];
                instance.tabElements.TryGetValue(key, out view);
            }
            if (view != null)
            {
                this._refresh_right_list(this.m_rightContentList, view);
            }
        }
    }

    public void RefreshLeft()
    {
        this._refresh_left_list(this.m_leftContentList, Singleton<InBattleMsgMgr>.instance.menuEntList);
    }

    public void RegEvent()
    {
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_CloseForm, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_CloseForm));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_LeftItem_Enable, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_LeftItem_Enable));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_Delete, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Delete));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_Record, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Record));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_UseDefault, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_UseDefault));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_Change, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Change));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_OK, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_OK));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_Cancle, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Cancle));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_RightTab_Change, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_RightTab_Change));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_RightItem_Enable, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_RightItem_Enable));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_RightItem_Click, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_RightItem_Click));
        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBatShortcut_UseDefault_Ok, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_UseDefault_Ok));
    }

    public void SetBtnEdit()
    {
        if (this.ReviseBtn != null)
        {
            this.ReviseBtn.CustomSetActive(false);
        }
        if (this.CancelBtn != null)
        {
            this.CancelBtn.CustomSetActive(true);
        }
        if (this.SureBtn != null)
        {
            this.SureBtn.CustomSetActive(true);
        }
    }

    public void SetBtnNormal()
    {
        if (this.ReviseBtn != null)
        {
            this.ReviseBtn.CustomSetActive(true);
        }
        if (this.CancelBtn != null)
        {
            this.CancelBtn.CustomSetActive(false);
        }
        if (this.SureBtn != null)
        {
            this.SureBtn.CustomSetActive(false);
        }
    }

    private void SetLeftItemState(GameObject node, TabElement data, EItemState state)
    {
        if (data != null)
        {
            Text component = node.transform.Find("titleText").GetComponent<Text>();
            GameObject gameObject = node.transform.Find("BtnGroup").gameObject;
            if (state == EItemState.Normal)
            {
                component.gameObject.CustomSetActive(true);
                component.text = data.configContent;
                gameObject.CustomSetActive(false);
            }
            else if (state == EItemState.Delete)
            {
                gameObject.CustomSetActive(true);
                component.gameObject.CustomSetActive(true);
                component.text = data.configContent;
                GameObject obj3 = gameObject.transform.Find("WriteBtn").gameObject;
                if (obj3 != null)
                {
                    obj3.CustomSetActive(false);
                }
                GameObject obj4 = gameObject.transform.Find("RemoveBtn").gameObject;
                if (obj4 != null)
                {
                    obj4.CustomSetActive(true);
                }
            }
            else if (state == EItemState.Record)
            {
                gameObject.CustomSetActive(true);
                component.gameObject.CustomSetActive(false);
                GameObject obj5 = gameObject.transform.Find("WriteBtn").gameObject;
                if (obj5 != null)
                {
                    obj5.CustomSetActive(true);
                }
                GameObject obj6 = gameObject.transform.Find("RemoveBtn").gameObject;
                if (obj6 != null)
                {
                    obj6.CustomSetActive(false);
                }
            }
        }
    }

    public void UnRegEvent()
    {
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_CloseForm, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_CloseForm));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_LeftItem_Enable, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_LeftItem_Enable));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_Delete, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Delete));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_Record, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Record));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_UseDefault, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_UseDefault));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_Change, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Change));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_OK, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_OK));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_Cancle, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_Cancle));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_RightTab_Change, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_RightTab_Change));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_RightItem_Enable, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_RightItem_Enable));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_RightItem_Click, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_RightItem_Click));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBatShortcut_UseDefault_Ok, new CUIEventManager.OnUIEventHandler(this.On_InBatShortcut_UseDefault_Ok));
    }

    public bool EditMode
    {
        get
        {
            return this.m_bEdit;
        }
        set
        {
            this.m_bEdit = value;
            if (this.m_bEdit)
            {
                if (this.ReviseBtn != null)
                {
                    this.ReviseBtn.CustomSetActive(false);
                }
                if (this.CancelBtn != null)
                {
                    this.CancelBtn.CustomSetActive(true);
                }
                if (this.SureBtn != null)
                {
                    this.SureBtn.CustomSetActive(true);
                }
            }
            else
            {
                if (this.ReviseBtn != null)
                {
                    this.ReviseBtn.CustomSetActive(true);
                }
                if (this.CancelBtn != null)
                {
                    this.CancelBtn.CustomSetActive(false);
                }
                if (this.SureBtn != null)
                {
                    this.SureBtn.CustomSetActive(false);
                }
            }
            this.RefreshLeft();
        }
    }

    public int TabIndex
    {
        get
        {
            return this.m_rightTabIndex;
        }
        set
        {
            if (this.m_rightTabIndex != value)
            {
                this.m_rightTabIndex = value;
                this.Refresh_Right_List(this.m_rightTabIndex);
            }
        }
    }

    public enum EItemState
    {
        None,
        Normal,
        Delete,
        Record
    }
}

