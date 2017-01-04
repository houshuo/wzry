using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

internal class NewbieWeakGuideImpl
{
    public const string FormWeakGuidePath = "UGUI/Form/System/Dialog/Form_WeakGuide";
    private NewbieGuideWeakConf m_conf;
    private CUIFormScript m_formWeakGuide;
    private GameObject m_guideTextObj;
    private GameObject m_guideTextStatic;
    private CUIFormScript m_originalForm;
    private GameObject m_parentObj;
    private static readonly string WEAK_EFFECT_PATH = "UGUI/Form/System/Dialog/WeakGuideHighlighter.prefab";

    public bool AddEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl, out GameObject highligter)
    {
        GameObject obj2;
        highligter = null;
        switch (conf.dwType)
        {
            case 1:
                highligter = obj2 = this.ShowPvPEffect(conf, inControl);
                return (obj2 != null);

            case 2:
                highligter = obj2 = this.ShowPvEEffect(conf, inControl);
                return (obj2 != null);

            case 3:
                highligter = obj2 = this.ShowFullHeroPanelEffect(conf, inControl);
                return (obj2 != null);

            case 4:
                highligter = obj2 = this.ShowHeroSelConfirmEffect(conf, inControl);
                return (obj2 != null);

            case 5:
                highligter = obj2 = this.ShowHumanMatch33Effect(conf, inControl);
                return (obj2 != null);

            case 6:
                highligter = obj2 = this.ShowBattleHeroSelEffect(conf, inControl);
                return (obj2 != null);

            case 7:
                highligter = obj2 = this.ShowClickPVPBtnEffect(conf, inControl);
                return (obj2 != null);

            case 8:
                highligter = obj2 = this.ShowClickMatch55(conf, inControl);
                return (obj2 != null);

            case 9:
                highligter = obj2 = this.ShowClickStartMatch55(conf, inControl);
                return (obj2 != null);

            case 10:
                highligter = obj2 = this.ShowClickWinShare(conf, inControl);
                return (obj2 != null);

            case 11:
            {
                if ((conf.Param[0] != 0) && (conf.Param[1] != 0))
                {
                    int index = UnityEngine.Random.Range(0, 2);
                    uint nextStep = conf.Param[index];
                    inControl.Complete(conf.dwID, nextStep);
                    return true;
                }
                object[] objArray1 = new object[] { conf.dwType };
                DebugHelper.Assert(false, "newbieguide Invalide config -- {0}", objArray1);
                return false;
            }
            case 12:
                highligter = obj2 = this.ShowClickRankBtn(conf, inControl);
                return (obj2 != null);

            case 13:
                highligter = obj2 = this.ShowClickTrainBtn(conf, inControl);
                return (obj2 != null);

            case 14:
                highligter = obj2 = this.ShowClickTrainWheelDisc(conf, inControl);
                return (obj2 != null);

            case 15:
                highligter = obj2 = this.ShowClickMatch55Melee(conf, inControl);
                return (obj2 != null);

            case 0x10:
                highligter = obj2 = this.ShowClickMatchingConfirmBoxConfirm(conf, inControl);
                return (obj2 != null);

            case 0x11:
                highligter = obj2 = this.ShowClickVictoryTipsBtn(conf, inControl);
                return (obj2 != null);

            case 0x12:
                highligter = obj2 = this.ShowClickSaveReplayKit(conf, inControl);
                return (obj2 != null);
        }
        object[] inParameters = new object[] { conf.dwType };
        DebugHelper.Assert(false, "Invalide NewbieGuideWeakGuideType -- {0}", inParameters);
        return false;
    }

    private GameObject AddEffectInternal(GameObject effectParent, NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl, CUIFormScript inOriginalForm)
    {
        GameObject content = Singleton<CResourceManager>.GetInstance().GetResource(WEAK_EFFECT_PATH, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
        GameObject obj3 = UnityEngine.Object.Instantiate(content) as GameObject;
        obj3.transform.SetParent(effectParent.transform);
        Transform parent = effectParent.transform.FindChild("Panel");
        if ((parent != null) && parent.gameObject.activeInHierarchy)
        {
            obj3.transform.SetParent(parent);
        }
        obj3.transform.localScale = Vector3.one;
        obj3.transform.position = effectParent.transform.position;
        (obj3.transform as RectTransform).anchoredPosition = new Vector2((float) conf.iOffsetHighLightX, (float) conf.iOffsetHighLightY);
        if (conf.bNotShowArrow != 0)
        {
            obj3.transform.FindChild("Panel/ImageFinger").gameObject.CustomSetActive(false);
        }
        this.AddEffectText(conf, effectParent, inOriginalForm);
        return obj3;
    }

    private void AddEffectText(NewbieGuideWeakConf conf, GameObject inParentObj, CUIFormScript inOriginalForm)
    {
        this.ClearEffectText();
        if ((conf != null) && (conf.wSpecialTip > 0))
        {
            NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig(conf.wSpecialTip);
            if (specialTipConfig != null)
            {
                if (specialTipConfig.bGuideTextPos == 0)
                {
                    if (this.m_guideTextStatic != null)
                    {
                        this.m_guideTextStatic.CustomSetActive(true);
                        Transform transform = this.m_guideTextStatic.transform.FindChild("RightSpecial/Text");
                        if (transform != null)
                        {
                            Text component = transform.GetComponent<Text>();
                            if (component != null)
                            {
                                component.text = StringHelper.UTF8BytesToString(ref specialTipConfig.szTipText);
                            }
                        }
                    }
                }
                else
                {
                    this.m_guideTextObj = NewbieGuideBaseScript.InstantiateGuideText(specialTipConfig, inParentObj, this.m_formWeakGuide, inOriginalForm);
                }
            }
        }
        this.m_conf = conf;
        this.m_parentObj = inParentObj;
        this.m_originalForm = inOriginalForm;
        if (this.m_formWeakGuide != null)
        {
            this.m_formWeakGuide.SetPriority(this.m_originalForm.m_priority + 1);
        }
    }

    public void ClearEffectText()
    {
        this.m_guideTextStatic.CustomSetActive(false);
        if (this.m_guideTextObj != null)
        {
            this.m_guideTextObj.CustomSetActive(false);
            this.m_guideTextObj = null;
        }
    }

    public void CloseGuideForm()
    {
        if (this.m_formWeakGuide != null)
        {
            this.m_guideTextStatic.CustomSetActive(false);
            this.m_guideTextStatic = null;
            Singleton<CUIManager>.GetInstance().CloseForm(this.m_formWeakGuide);
            this.m_formWeakGuide = null;
        }
        this.m_guideTextStatic = null;
        this.m_parentObj = null;
        this.m_guideTextObj = null;
        this.m_originalForm = null;
    }

    public void Init()
    {
    }

    public void OpenGuideForm()
    {
        if (this.m_formWeakGuide == null)
        {
            this.m_formWeakGuide = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Dialog/Form_WeakGuide", true, true);
            if (this.m_formWeakGuide != null)
            {
                Transform transform = this.m_formWeakGuide.transform.FindChild("GuideTextStatic");
                if (transform != null)
                {
                    this.m_guideTextStatic = transform.gameObject;
                    if (this.m_guideTextStatic != null)
                    {
                        this.m_guideTextStatic.CustomSetActive(false);
                    }
                }
            }
        }
    }

    public void RemoveEffectText(NewbieGuideWeakConf conf)
    {
        if (conf != null)
        {
            this.ClearEffectText();
        }
    }

    private GameObject ShowBattleHeroSelEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
        if (form == null)
        {
            return null;
        }
        uint num = conf.Param[0];
        string name = string.Format("PanelLeft/ListHostHeroInfo/ScrollRect/Content/ListElement_{0}/heroItemCell", num);
        GameObject gameObject = form.gameObject.transform.Find(name).gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowClickMatch55(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
        if (form == null)
        {
            return null;
        }
        GameObject gameObject = form.gameObject.transform.FindChild("panelGroup2/btnGroup/Button4").gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowClickMatch55Melee(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
        if (form == null)
        {
            return null;
        }
        GameObject gameObject = form.gameObject.transform.Find("panelGroup2/btnGroup/Button3").gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowClickMatchingConfirmBoxConfirm(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX);
        if (form == null)
        {
            return null;
        }
        GameObject gameObject = form.gameObject.transform.Find("Panel/Panel/btnGroup/Button_Confirm").gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowClickPVPBtnEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
        if (form == null)
        {
            return null;
        }
        GameObject gameObject = form.gameObject.transform.FindChild("panelGroup1/btnGroup/Button1").gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowClickRankBtn(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
        if (form == null)
        {
            return null;
        }
        GameObject gameObject = form.gameObject.transform.FindChild("BtnCon/LadderBtn").gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowClickSaveReplayKit(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<SettlementSystem>.instance.SettlementFormName);
        if (form == null)
        {
            return null;
        }
        GameObject gameObject = form.transform.FindChild("Panel/ButtonGrid/BtnSaveReplay").gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowClickStartMatch55(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_MULTI);
        if (form == null)
        {
            return null;
        }
        GameObject gameObject = form.gameObject.transform.FindChild("Panel_Main/Btn_Matching").gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowClickTrainBtn(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
        if (form == null)
        {
            return null;
        }
        GameObject gameObject = form.gameObject.transform.Find("panelGroupBottom/ButtonTrain").gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowClickTrainWheelDisc(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
        if (form == null)
        {
            return null;
        }
        GameObject gameObject = form.gameObject.transform.Find("panelGroup4/btnGroup/Button2").gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowClickVictoryTipsBtn(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<SettlementSystem>.instance.SettlementFormName);
        if (form == null)
        {
            return null;
        }
        Transform transform = form.GetWidget(0x18).transform;
        GameObject gameObject = transform.FindChild("Btn").gameObject;
        PlayerKDA hostKDA = null;
        if ((Singleton<BattleLogic>.GetInstance().battleStat != null) && (Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat != null))
        {
            hostKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
        }
        string text = string.Empty;
        if (hostKDA == null)
        {
            text = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_DefaultHeroName");
        }
        else
        {
            ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
            uint key = 0;
            while (enumerator.MoveNext())
            {
                HeroKDA current = enumerator.Current;
                if (current != null)
                {
                    key = (uint) current.HeroId;
                    break;
                }
            }
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(key);
            if (dataByKey != null)
            {
                text = dataByKey.szName;
            }
            else
            {
                text = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_DefaultHeroName");
            }
        }
        transform.FindChild("Panel_Guide").gameObject.CustomSetActive(true);
        string[] args = new string[] { text };
        transform.FindChild("Panel_Guide/Text").GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_text", args);
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowClickWinShare(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<SettlementSystem>.instance.SettlementFormName);
        if (form == null)
        {
            return null;
        }
        GameObject gameObject = form.gameObject.transform.FindChild("Panel/ButtonGrid/ButtonShare").gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowFullHeroPanelEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
        if (form == null)
        {
            return null;
        }
        GameObject gameObject = form.gameObject.transform.Find("PanelLeft/ListHostHeroInfo/btnOpenFullHeroPanel").gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowHeroSelConfirmEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
        if (form == null)
        {
            return null;
        }
        GameObject gameObject = form.gameObject.transform.Find("PanelRight/btnConfirm").gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowHumanMatch33Effect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
        if (form == null)
        {
            return null;
        }
        GameObject gameObject = form.gameObject.transform.Find("panelGroup3/btnGroup/Button3").gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowPvEEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
        if (form == null)
        {
            return null;
        }
        GameObject gameObject = form.gameObject.transform.Find("BtnCon/PveBtn").gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    private GameObject ShowPvPEffect(NewbieGuideWeakConf conf, NewbieWeakGuideControl inControl)
    {
        CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
        if (form == null)
        {
            return null;
        }
        GameObject gameObject = form.gameObject.transform.Find("BtnCon/PvpBtn").gameObject;
        return this.AddEffectInternal(gameObject, conf, inControl, form);
    }

    public void UnInit()
    {
    }

    public void Update()
    {
        if (((this.m_conf == null) || (this.m_parentObj == null)) || ((this.m_guideTextObj == null) || (this.m_originalForm == null)))
        {
            if ((this.m_guideTextStatic != null) || (this.m_guideTextObj != null))
            {
                this.ClearEffectText();
            }
        }
        else
        {
            NewbieGuideSpecialTipConf specialTipConfig = Singleton<NewbieGuideDataManager>.GetInstance().GetSpecialTipConfig(this.m_conf.wSpecialTip);
            if ((specialTipConfig != null) && (specialTipConfig.bGuideTextPos > 0))
            {
                this.m_guideTextObj.CustomSetActive(this.m_parentObj.activeInHierarchy && !this.m_originalForm.IsHided());
                NewbieGuideBaseScript.UpdateGuideTextPos(specialTipConfig, this.m_parentObj, this.m_formWeakGuide, this.m_originalForm, this.m_guideTextObj);
            }
        }
    }
}

