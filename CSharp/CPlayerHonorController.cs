using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[MessageHandlerClass]
public class CPlayerHonorController : Singleton<CPlayerHonorController>
{
    [CompilerGenerated]
    private static Comparison<COMDT_HONORINFO> <>f__am$cache1;
    private List<COMDT_HONORINFO> m_honorInfoList;

    public void Draw(CUIFormScript form)
    {
        if (form != null)
        {
            GameObject widget = form.GetWidget(9);
            if (widget != null)
            {
                Utility.FindChild(widget, "pnlHonorInfo").CustomSetActive(true);
                GameObject obj3 = Utility.FindChild(widget, "pnlHonorInfo/pnlContainer/HonorList");
                if (obj3 != null)
                {
                    CUIListScript component = obj3.GetComponent<CUIListScript>();
                    if (component != null)
                    {
                        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnHonorItemEnable));
                        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Select_Change, new CUIEventManager.OnUIEventHandler(this.OnHonorSelectChange));
                        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Chosen, new CUIEventManager.OnUIEventHandler(this.OnHonorChosen));
                        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_Honor_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnHonorItemEnable));
                        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_Honor_Select_Change, new CUIEventManager.OnUIEventHandler(this.OnHonorSelectChange));
                        Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_Honor_Chosen, new CUIEventManager.OnUIEventHandler(this.OnHonorChosen));
                        this.m_honorInfoList.Clear();
                        CPlayerProfile profile = Singleton<CPlayerInfoSystem>.GetInstance().GetProfile();
                        Dictionary<int, COMDT_HONORINFO> honorDic = profile.GetHonorDic();
                        int selectedHonorId = profile.GetSelectedHonorId();
                        Dictionary<int, COMDT_HONORINFO>.Enumerator enumerator = honorDic.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            KeyValuePair<int, COMDT_HONORINFO> current = enumerator.Current;
                            COMDT_HONORINFO item = current.Value;
                            if (item != null)
                            {
                                this.m_honorInfoList.Add(item);
                            }
                        }
                        if (this.m_honorInfoList != null)
                        {
                            if (<>f__am$cache1 == null)
                            {
                                <>f__am$cache1 = delegate (COMDT_HONORINFO l, COMDT_HONORINFO r) {
                                    if (l == null)
                                    {
                                        return 1;
                                    }
                                    if (r == null)
                                    {
                                        return -1;
                                    }
                                    return l.iHonorID.CompareTo(r.iHonorID);
                                };
                            }
                            this.m_honorInfoList.Sort(<>f__am$cache1);
                        }
                        component.SetElementAmount(this.m_honorInfoList.Count);
                        component.SelectElement(-1, false);
                        COMDT_HONORINFO comdt_honorinfo2 = null;
                        honorDic.TryGetValue(selectedHonorId, out comdt_honorinfo2);
                        this.UpdateSelectedHonor(form, comdt_honorinfo2);
                    }
                }
            }
        }
    }

    public string GetHonorImagePath(int id, int level)
    {
        ResHonor dataByKey = GameDataMgr.resHonor.GetDataByKey((long) id);
        if (dataByKey == null)
        {
            return string.Format("{0}{1}{2}", CUIUtility.s_Sprite_System_Honor_Dir, "Img_Honor_", 0);
        }
        if ((level < 0) || (level > dataByKey.astHonorLevel.Length))
        {
            return null;
        }
        if (level == 0)
        {
            return string.Format("{0}{1}{2}", CUIUtility.s_Sprite_System_Honor_Dir, "Img_Honor_", 0);
        }
        object[] args = new object[] { CUIUtility.s_Sprite_System_Honor_Dir, "Img_Honor_", id, "_", level };
        return string.Format("{0}{1}{2}{3}{4}", args);
    }

    private string GetHonorProgress(int point, ResHonor honorCfg)
    {
        string str = string.Empty;
        if (honorCfg != null)
        {
            for (int i = honorCfg.astHonorLevel.Length - 1; i >= 0; i--)
            {
                if (point < honorCfg.astHonorLevel[i].iMaxPoint)
                {
                    str = string.Format("{0}/{1}", point, honorCfg.astHonorLevel[i].iMaxPoint);
                }
                else if (i == (honorCfg.astHonorLevel.Length - 1))
                {
                    str = string.Format("{0}", point);
                }
                else
                {
                    return str;
                }
            }
        }
        return str;
    }

    public override void Init()
    {
        base.Init();
        this.m_honorInfoList = new List<COMDT_HONORINFO>();
    }

    public void Load(CUIFormScript form)
    {
        if (form != null)
        {
            CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Player/HonorInfo", "pnlHonorInfo", form.GetWidget(9), form);
        }
    }

    public bool Loaded(CUIFormScript form)
    {
        if (form == null)
        {
            return false;
        }
        GameObject widget = form.GetWidget(9);
        if (widget == null)
        {
            return false;
        }
        if (Utility.FindChild(widget, "pnlHonorInfo") == null)
        {
            return false;
        }
        return true;
    }

    private void OnHonorChosen(CUIEvent uiEvent)
    {
        GameObject widget = uiEvent.m_srcFormScript.GetWidget(9);
        if (widget != null)
        {
            GameObject obj3 = Utility.FindChild(widget, "pnlHonorInfo/pnlContainer/HonorList");
            if (obj3 != null)
            {
                CUIListScript component = obj3.GetComponent<CUIListScript>();
                if (component != null)
                {
                    int selectedIndex = component.GetSelectedIndex();
                    if (((uiEvent.m_srcFormScript != null) && (selectedIndex >= 0)) && (selectedIndex < this.m_honorInfoList.Count))
                    {
                        COMDT_HONORINFO comdt_honorinfo = this.m_honorInfoList[selectedIndex];
                        if ((comdt_honorinfo != null) && (comdt_honorinfo.iHonorLevel > 0))
                        {
                            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x588);
                            msg.stPkgData.stUseHonorReq.iHonorID = comdt_honorinfo.iHonorID;
                            Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref msg, true);
                        }
                    }
                }
            }
        }
    }

    private void OnHonorItemEnable(CUIEvent uiEvent)
    {
        int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
        if ((srcWidgetIndexInBelongedList >= 0) && (srcWidgetIndexInBelongedList < this.m_honorInfoList.Count))
        {
            COMDT_HONORINFO honorInfo = this.m_honorInfoList[srcWidgetIndexInBelongedList];
            if (honorInfo != null)
            {
                ResHonor dataByKey = GameDataMgr.resHonor.GetDataByKey((long) honorInfo.iHonorID);
                if ((dataByKey != null) && ((honorInfo.iHonorLevel >= 0) && (honorInfo.iHonorLevel <= dataByKey.astHonorLevel.Length)))
                {
                    CUIListElementScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
                    if (srcWidgetScript != null)
                    {
                        GameObject widget = srcWidgetScript.GetWidget(0);
                        GameObject obj3 = srcWidgetScript.GetWidget(1);
                        GameObject obj4 = srcWidgetScript.GetWidget(2);
                        GameObject obj5 = srcWidgetScript.GetWidget(3);
                        GameObject chosenGo = srcWidgetScript.GetWidget(4);
                        if (widget != null)
                        {
                            this.SetHonorImage(widget.transform, honorInfo);
                        }
                        if (obj3 != null)
                        {
                            this.SetHonorAssitImage(obj3.transform, honorInfo, uiEvent.m_srcFormScript);
                        }
                        if (obj4 != null)
                        {
                            this.SetHonorPoint(obj4.transform, honorInfo);
                        }
                        this.SetHonorStatus(chosenGo, honorInfo);
                        obj5.CustomSetActive(false);
                    }
                }
            }
        }
    }

    private void OnHonorSelectChange(CUIEvent uiEvent)
    {
        CUIListScript srcWidgetScript = uiEvent.m_srcWidgetScript as CUIListScript;
        if (srcWidgetScript != null)
        {
            int selectedIndex = srcWidgetScript.GetSelectedIndex();
            if (((uiEvent.m_srcFormScript != null) && (selectedIndex >= 0)) && (selectedIndex < this.m_honorInfoList.Count))
            {
                COMDT_HONORINFO honorInfo = this.m_honorInfoList[selectedIndex];
                if (honorInfo != null)
                {
                    this.UpdateSelectedHonor(uiEvent.m_srcFormScript, honorInfo);
                }
            }
        }
    }

    private void OnPlayerInfoTabChange()
    {
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnHonorItemEnable));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Select_Change, new CUIEventManager.OnUIEventHandler(this.OnHonorSelectChange));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Chosen, new CUIEventManager.OnUIEventHandler(this.OnHonorChosen));
        Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.PlayerInfoSystem_Tab_Change, new System.Action(this.OnPlayerInfoTabChange));
    }

    [MessageHandler(0x589)]
    public static void ReceiveHonorChosenRsp(CSPkg msg)
    {
        Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CPlayerInfoSystem>.GetInstance().sPlayerInfoFormPath);
        if (msg.stPkgData.stUseHonorRsp.iErrorCode != 0)
        {
            Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(0x589, msg.stPkgData.stUseHonorRsp.iErrorCode), false, 1.5f, null, new object[0]);
            if (form != null)
            {
                COMDT_HONORINFO honorInfo = new COMDT_HONORINFO {
                    iHonorID = msg.stPkgData.stUseHonorRsp.iHonorID,
                    iHonorLevel = 0
                };
                Singleton<CPlayerHonorController>.GetInstance().UpdateSelectedHonor(form, honorInfo);
            }
        }
        else
        {
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            if (masterRoleInfo != null)
            {
                masterRoleInfo.selectedHonorID = msg.stPkgData.stUseHonorRsp.iHonorID;
                Singleton<CPlayerInfoSystem>.GetInstance().GetProfile().ConvertRoleInfoData(masterRoleInfo);
                if (form != null)
                {
                    Singleton<CPlayerHonorController>.GetInstance().Draw(form);
                }
            }
        }
    }

    private void SetHonorAssitImage(Transform imgTransform, COMDT_HONORINFO honorInfo, CUIFormScript form)
    {
        if (imgTransform != null)
        {
            Image component = imgTransform.GetComponent<Image>();
            if (component != null)
            {
                switch (honorInfo.iHonorID)
                {
                    case 1:
                    {
                        string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Large_Dir, "Img_Icon_Red_Mvp");
                        component.SetSprite(prefabPath, form, true, false, false);
                        break;
                    }
                    case 2:
                    {
                        string str2 = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Large_Dir, "HurtMost");
                        component.SetSprite(str2, form, true, false, false);
                        break;
                    }
                    case 3:
                    {
                        string str6 = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Large_Dir, "KillMost");
                        component.SetSprite(str6, form, true, false, false);
                        break;
                    }
                    case 4:
                    {
                        string str4 = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Large_Dir, "MostMoney");
                        component.SetSprite(str4, form, true, false, false);
                        break;
                    }
                    case 5:
                    {
                        string str5 = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Large_Dir, "AsssistMost");
                        component.SetSprite(str5, form, true, false, false);
                        break;
                    }
                    case 6:
                    {
                        string str3 = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Large_Dir, "HurtTakenMost");
                        component.SetSprite(str3, form, true, false, false);
                        break;
                    }
                }
            }
        }
    }

    private void SetHonorDesc(Transform descTransform, COMDT_HONORINFO honorInfo)
    {
        if (descTransform != null)
        {
            Text component = descTransform.GetComponent<Text>();
            if (component != null)
            {
                ResHonor dataByKey = GameDataMgr.resHonor.GetDataByKey((long) honorInfo.iHonorID);
                if (dataByKey == null)
                {
                    component.text = string.Empty;
                }
                else
                {
                    component.text = dataByKey.szDesc;
                }
            }
        }
    }

    private void SetHonorImage(Transform imgTransform, COMDT_HONORINFO honorInfo)
    {
        string honorImagePath = this.GetHonorImagePath(honorInfo.iHonorID, honorInfo.iHonorLevel);
        imgTransform.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(honorImagePath, false, false));
    }

    private void SetHonorName(Transform labelTransform, COMDT_HONORINFO honorInfo)
    {
        if (labelTransform != null)
        {
            CTextManager instance = Singleton<CTextManager>.GetInstance();
            Text component = labelTransform.GetComponent<Text>();
            if (component != null)
            {
                switch (honorInfo.iHonorID)
                {
                    case 1:
                        component.text = instance.GetText("Player_Info_Honor_Name_MVP");
                        return;

                    case 2:
                        component.text = instance.GetText("Player_Info_Honor_Name_MAXDAMAGETOHERO");
                        return;

                    case 3:
                        component.text = instance.GetText("Player_Info_Honor_Name_MAXKILL");
                        return;

                    case 4:
                        component.text = instance.GetText("Player_Info_Honor_Name_MAXMONEY");
                        return;

                    case 5:
                        component.text = instance.GetText("Player_Info_Honor_Name_MAXASSIS");
                        return;

                    case 6:
                        component.text = instance.GetText("Player_Info_Honor_Name_MAXRECVDAMAGE");
                        return;
                }
                component.text = string.Empty;
            }
        }
    }

    private void SetHonorPoint(Transform cntTransform, COMDT_HONORINFO honorInfo)
    {
        if (cntTransform != null)
        {
            Text component = cntTransform.GetComponent<Text>();
            if (component != null)
            {
                ResHonor dataByKey = GameDataMgr.resHonor.GetDataByKey((long) honorInfo.iHonorID);
                if (dataByKey == null)
                {
                    component.text = string.Empty;
                }
                else
                {
                    component.text = this.GetHonorProgress(honorInfo.iHonorPoint, dataByKey);
                }
            }
        }
    }

    private void SetHonorStatus(GameObject chosenGo, COMDT_HONORINFO honorInfo)
    {
        if (honorInfo == null)
        {
            chosenGo.CustomSetActive(false);
        }
        else
        {
            int selectedHonorId = Singleton<CPlayerInfoSystem>.GetInstance().GetProfile().GetSelectedHonorId();
            if (honorInfo.iHonorID == selectedHonorId)
            {
                chosenGo.CustomSetActive(true);
            }
            else
            {
                chosenGo.CustomSetActive(false);
            }
        }
    }

    public override void UnInit()
    {
        base.UnInit();
        this.m_honorInfoList = null;
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnHonorItemEnable));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Select_Change, new CUIEventManager.OnUIEventHandler(this.OnHonorSelectChange));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Chosen, new CUIEventManager.OnUIEventHandler(this.OnHonorChosen));
        Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.Mall_Change_Tab, new System.Action(this.OnPlayerInfoTabChange));
    }

    public void UpdateSelectedHonor(CUIFormScript form, COMDT_HONORINFO honorInfo)
    {
        CTextManager instance = Singleton<CTextManager>.GetInstance();
        string text = instance.GetText("Player_Info_Honor_Btn_Using");
        string strName = instance.GetText("Player_Info_Honor_Btn_Use");
        string str3 = instance.GetText("Player_Info_Honor_Btn_Browse");
        GameObject widget = form.GetWidget(9);
        if (widget != null)
        {
            Transform transform = form.transform.Find("pnlBg/pnlBody/pnlHonorInfo/pnlContainer/SelectedHonor/Button");
            Button btn = (transform != null) ? transform.GetComponent<Button>() : null;
            GameObject obj3 = Utility.FindChild(widget, "pnlHonorInfo/pnlContainer/SelectedHonor/CurrentHonor/normal");
            GameObject obj4 = Utility.FindChild(widget, "pnlHonorInfo/pnlContainer/SelectedHonor/CurrentHonor/label");
            GameObject obj5 = Utility.FindChild(widget, "pnlHonorInfo/pnlContainer/SelectedHonor/CurrentHonor/cnt");
            GameObject obj6 = Utility.FindChild(widget, "pnlHonorInfo/pnlContainer/SelectedHonor/Text");
            CPlayerProfile profile = Singleton<CPlayerInfoSystem>.GetInstance().GetProfile();
            int selectedHonorId = profile.GetSelectedHonorId();
            CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
            bool flag = true;
            if ((masterRoleInfo != null) && (masterRoleInfo.playerUllUID != profile.m_uuid))
            {
                flag = false;
            }
            if (honorInfo == null)
            {
                honorInfo = new COMDT_HONORINFO();
                if (btn != null)
                {
                    CUICommonSystem.SetButtonEnable(btn, false, false, true);
                    CUICommonSystem.SetButtonName(transform.gameObject, strName);
                }
                if (obj3 != null)
                {
                    this.SetHonorImage(obj3.transform, honorInfo);
                }
                if (obj4 != null)
                {
                    this.SetHonorName(obj4.transform, honorInfo);
                }
                if (obj5 != null)
                {
                    this.SetHonorPoint(obj5.transform, honorInfo);
                }
                if (obj6 != null)
                {
                    this.SetHonorDesc(obj6.transform, honorInfo);
                }
                if (!flag && (btn != null))
                {
                    CUICommonSystem.SetButtonEnable(btn, false, false, true);
                    CUICommonSystem.SetButtonName(transform.gameObject, str3);
                }
            }
            else
            {
                if (obj3 != null)
                {
                    this.SetHonorImage(obj3.transform, honorInfo);
                }
                if (obj4 != null)
                {
                    this.SetHonorName(obj4.transform, honorInfo);
                }
                if (obj5 != null)
                {
                    this.SetHonorPoint(obj5.transform, honorInfo);
                }
                if (obj6 != null)
                {
                    this.SetHonorDesc(obj6.transform, honorInfo);
                }
                if (!flag && (btn != null))
                {
                    CUICommonSystem.SetButtonEnable(btn, false, false, true);
                    CUICommonSystem.SetButtonName(transform.gameObject, str3);
                }
                else if ((honorInfo.iHonorID == selectedHonorId) && (honorInfo.iHonorID != 0))
                {
                    if (btn != null)
                    {
                        CUICommonSystem.SetButtonEnable(btn, false, false, true);
                        CUICommonSystem.SetButtonName(transform.gameObject, text);
                    }
                }
                else if (honorInfo.iHonorLevel <= 0)
                {
                    if (btn != null)
                    {
                        CUICommonSystem.SetButtonEnable(btn, false, false, true);
                        CUICommonSystem.SetButtonName(transform.gameObject, strName);
                    }
                }
                else if (btn != null)
                {
                    CUICommonSystem.SetButtonEnable(btn, true, true, true);
                    CUICommonSystem.SetButtonName(transform.gameObject, strName);
                }
            }
        }
    }
}

