namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class BattleSkillHudControl : Singleton<BattleSkillHudControl>
    {
        private bool[] m_buttonHidden = new bool[8];
        private bool[] m_buttonHighLighted = new bool[8];
        private string m_joystickHighlightName = "Joystick/Axis/Cursor/Highlight";
        public bool[] m_learnBtnHidden = new bool[4];
        private bool[] m_learnBtnHighLighted = new bool[4];
        private bool[] m_restSkilBtnHighLighted = new bool[2];
        private bool[] m_RestSkillBtnHidden = new bool[2];

        public void Activate(SkillSlotType inSlotType, bool bActivate, bool bAll)
        {
            List<GameObject> list = this.QuerySkillButtons(inSlotType, bAll);
            for (int i = 0; i < list.Count; i++)
            {
                GameObject obj2 = list[i];
                if (obj2 != null)
                {
                    Button component = obj2.GetComponent<Button>();
                    if (component != null)
                    {
                        component.enabled = bActivate;
                    }
                    CUIEventScript script = obj2.GetComponent<CUIEventScript>();
                    if (script != null)
                    {
                        script.enabled = bActivate;
                    }
                }
            }
            if ((!bActivate && (Singleton<CBattleSystem>.GetInstance().FightForm != null)) && (Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager != null))
            {
                if (bAll)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Vector2 screenPosition = new Vector2();
                        Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.SkillButtonUp(Singleton<CBattleSystem>.GetInstance().FightFormScript, (SkillSlotType) j, false, screenPosition);
                    }
                }
                else
                {
                    Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.SkillButtonUp(Singleton<CBattleSystem>.GetInstance().FightFormScript, inSlotType, false, new Vector2());
                }
            }
        }

        private void ActivateBtn(GameObject btn, bool bActive)
        {
            if (btn != null)
            {
                Button component = btn.GetComponent<Button>();
                if (component != null)
                {
                    component.enabled = bActive;
                }
                CUIEventScript script = btn.GetComponent<CUIEventScript>();
                if (script != null)
                {
                    script.enabled = bActive;
                }
            }
        }

        public void ActivateLearnBtn(SkillSlotType inSlotType, bool bActivate, bool bAll)
        {
            if (bAll || ((inSlotType >= SkillSlotType.SLOT_SKILL_1) && (inSlotType <= SkillSlotType.SLOT_SKILL_3)))
            {
                List<GameObject> list = this.QuerySkillButtons(inSlotType, bAll);
                for (int i = 0; i < list.Count; i++)
                {
                    GameObject obj2 = list[i];
                    if (obj2 != null)
                    {
                        Transform transform = obj2.transform.FindChild("LearnBtn");
                        if (transform != null)
                        {
                            GameObject gameObject = transform.gameObject;
                            Button component = gameObject.GetComponent<Button>();
                            if (component != null)
                            {
                                component.enabled = bActivate;
                            }
                            CUIEventScript script = gameObject.GetComponent<CUIEventScript>();
                            if (script != null)
                            {
                                script.enabled = bActivate;
                            }
                        }
                    }
                }
            }
        }

        public void ActivateOtherBtn(enRestSkillSlotType inSlotType, bool bActivate, bool bAll)
        {
            if (bAll || ((inSlotType >= enRestSkillSlotType.BTN_SKILL_SELHERO) && (inSlotType <= enRestSkillSlotType.BTN_SKILL_COUNT)))
            {
                if (bAll)
                {
                    GameObject obj2 = null;
                    for (int i = 0; i < 2; i++)
                    {
                        obj2 = this.QueryRestSkillBtn((enRestSkillSlotType) i);
                        if (obj2 != null)
                        {
                            Button component = obj2.GetComponent<Button>();
                            if (component != null)
                            {
                                component.enabled = bActivate;
                            }
                            CUIEventScript script = obj2.GetComponent<CUIEventScript>();
                            if (script != null)
                            {
                                script.enabled = bActivate;
                            }
                        }
                    }
                }
                else
                {
                    GameObject obj3 = this.QueryRestSkillBtn(inSlotType);
                    if (obj3 != null)
                    {
                        Button button2 = obj3.GetComponent<Button>();
                        if (button2 != null)
                        {
                            button2.enabled = bActivate;
                        }
                        CUIEventScript script2 = obj3.GetComponent<CUIEventScript>();
                        if (script2 != null)
                        {
                            script2.enabled = bActivate;
                        }
                    }
                }
            }
        }

        public void ActivateUI(bool bActive)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
            if (form != null)
            {
                GameObject widget = form.GetWidget(0x45);
                this.ActivateBtn(widget, bActive);
            }
        }

        public void AddHighlightForActor(PoolObjHandle<ActorRoot> actor, bool bPauseGame)
        {
            if (actor != 0)
            {
                ActorRoot handle = actor.handle;
                if (handle.InCamera)
                {
                    GameObject content = Singleton<CResourceManager>.GetInstance().GetResource("UGUI/Form/System/Dialog/WeakGuideHighlighter.prefab", typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
                    if (content != null)
                    {
                        GameObject widget = UnityEngine.Object.Instantiate(content) as GameObject;
                        if (widget != null)
                        {
                            if (NewbieGuideScriptControl.FormGuideMask == null)
                            {
                                NewbieGuideScriptControl.OpenGuideForm();
                            }
                            CUIFormScript formGuideMask = NewbieGuideScriptControl.FormGuideMask;
                            Transform transform = formGuideMask.transform;
                            Vector3 screenPoint = (Vector3) CUIUtility.WorldToScreenPoint(Camera.main, (Vector3) handle.location);
                            Vector3 worldPosition = CUIUtility.ScreenToWorldPoint(formGuideMask.GetCamera(), screenPoint, transform.position.z);
                            Transform transform2 = widget.transform;
                            transform2.SetSiblingIndex(1);
                            transform2.SetParent(NewbieGuideScriptControl.FormGuideMask.transform);
                            formGuideMask.InitializeWidgetPosition(widget, worldPosition);
                            transform2.position = worldPosition;
                            transform2.localScale = Vector3.one;
                            CUIEventScript local1 = widget.AddComponent<CUIEventScript>();
                            local1.onClick = (CUIEventScript.OnUIEventHandler) Delegate.Combine(local1.onClick, new CUIEventScript.OnUIEventHandler(this.HighliterForActorClickHandler));
                            Singleton<CBattleGuideManager>.GetInstance().PauseGame(widget, false);
                        }
                    }
                }
            }
        }

        public void Highlight(SkillSlotType inSlotType, bool bHighlight, bool bAll, bool bDoActivating, bool bPauseGame)
        {
            List<GameObject> list = this.QuerySkillButtons(inSlotType, bAll);
            if ((list.Count == 1) && bDoActivating)
            {
                if (bHighlight)
                {
                    this.Activate(SkillSlotType.SLOT_SKILL_COUNT, false, true);
                    this.Activate(inSlotType, true, false);
                }
                else
                {
                    this.Activate(SkillSlotType.SLOT_SKILL_COUNT, true, true);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                GameObject button = list[i];
                if (button != null)
                {
                    Singleton<CBattleSystem>.GetInstance().FightForm.SetButtonHighLight(button, bHighlight);
                }
            }
            if (inSlotType < SkillSlotType.SLOT_SKILL_COUNT)
            {
                this.m_buttonHighLighted[(int) inSlotType] = bHighlight;
            }
            this.UpdateGamePausing(bPauseGame);
        }

        public void HighlightJoystick(bool bHighlight)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
            if (form != null)
            {
                form.transform.FindChild(this.m_joystickHighlightName).gameObject.CustomSetActive(bHighlight);
            }
        }

        public void HighlightLearnBtn(SkillSlotType inSlotType, bool bHighlight, bool bAll, bool bDoActivating, bool bPauseGame)
        {
            if (bAll || ((inSlotType >= SkillSlotType.SLOT_SKILL_1) && (inSlotType <= SkillSlotType.SLOT_SKILL_3)))
            {
                List<GameObject> list = this.QuerySkillButtons(inSlotType, bAll);
                if (list.Count != 0)
                {
                    bool flag = false;
                    if (list.Count == 1)
                    {
                        flag = true;
                    }
                    if (flag && bDoActivating)
                    {
                        if (bHighlight)
                        {
                            this.Activate(SkillSlotType.SLOT_SKILL_COUNT, false, true);
                            this.ActivateLearnBtn(SkillSlotType.SLOT_SKILL_COUNT, false, true);
                            this.ActivateLearnBtn(inSlotType, true, false);
                        }
                        else
                        {
                            this.Activate(SkillSlotType.SLOT_SKILL_COUNT, true, true);
                            this.ActivateLearnBtn(SkillSlotType.SLOT_SKILL_COUNT, true, true);
                        }
                    }
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] != null)
                        {
                            Transform transform = list[i].transform.FindChild("LearnBtn");
                            if (transform != null)
                            {
                                Singleton<CBattleSystem>.GetInstance().FightForm.SetLearnBtnHighLight(transform.gameObject, bHighlight);
                            }
                        }
                    }
                    if (inSlotType < SkillSlotType.SLOT_SKILL_COUNT)
                    {
                        this.m_learnBtnHighLighted[(int) inSlotType] = bHighlight;
                    }
                    this.UpdateGamePausing(bPauseGame);
                }
            }
        }

        public void HighlishtRestSkillBtn(enRestSkillSlotType inRestSkillType, bool bHighlight, bool bDoActivating, bool bPause)
        {
            GameObject button = this.QueryRestSkillBtn(inRestSkillType);
            if (button != null)
            {
                if (bDoActivating)
                {
                    if (bHighlight)
                    {
                        this.Activate(SkillSlotType.SLOT_SKILL_COUNT, false, true);
                        this.ActivateOtherBtn(enRestSkillSlotType.BTN_SKILL_COUNT, false, true);
                        this.ActivateOtherBtn(inRestSkillType, true, false);
                        this.ActivateUI(false);
                    }
                    else
                    {
                        this.Activate(SkillSlotType.SLOT_SKILL_COUNT, true, true);
                        this.ActivateOtherBtn(enRestSkillSlotType.BTN_SKILL_COUNT, true, true);
                        this.ActivateUI(true);
                    }
                }
                Singleton<CBattleSystem>.GetInstance().FightForm.SetButtonHighLight(button, bHighlight);
                if (inRestSkillType < enRestSkillSlotType.BTN_SKILL_COUNT)
                {
                    this.m_restSkilBtnHighLighted[(int) inRestSkillType] = bHighlight;
                }
                this.UpdateGamePausing(bPause);
            }
        }

        public void HighliterForActorClickHandler(CUIEvent uiEvt)
        {
            GameObject srcWidget = uiEvt.m_srcWidget;
            Singleton<CBattleGuideManager>.GetInstance().ResumeGame(srcWidget);
            Vector2 vector = CUIUtility.WorldToScreenPoint(NewbieGuideScriptControl.FormGuideMask.GetCamera(), srcWidget.transform.position);
            Singleton<LockModeScreenSelector>.GetInstance().OnClickBattleScene(vector);
            srcWidget.transform.SetParent(null);
            UnityEngine.Object.Destroy(srcWidget);
            NewbieGuideScriptControl.CloseGuideForm();
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleRequestUseSkill));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillBtnDown));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_LearnSkillBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleLearnSkillBtnClick));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAtkSelectHeroUp, new CUIEventManager.OnUIEventHandler(this.OnAtkSelectHero));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAtkSelectSoldierUp, new CUIEventManager.OnUIEventHandler(this.OnAtkSelectSoldier));
            Singleton<EventRouter>.GetInstance().AddEventHandler<int, bool>("HeroSkillLearnButtonStateChange", new Action<int, bool>(this.onSkillLearnBtnStateChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler("ResetSkillButtonManager", new System.Action(this.onResetSkillButtonManager));
            Singleton<EventRouter>.GetInstance().AddEventHandler("CommonAttack_Type_Changed", new System.Action(this.onCommonAttackTypeChange));
        }

        public void OnAtkSelectHero(CUIEvent uiEvent)
        {
            if (this.m_restSkilBtnHighLighted[0])
            {
                this.HighlishtRestSkillBtn(enRestSkillSlotType.BTN_SKILL_SELHERO, false, true, false);
            }
        }

        public void OnAtkSelectSoldier(CUIEvent uiEvent)
        {
            if (this.m_restSkilBtnHighLighted[1])
            {
                this.HighlishtRestSkillBtn(enRestSkillSlotType.BTN_SKILL_SELSOLDIER, false, true, false);
            }
        }

        public void OnBattleLearnSkillBtnClick(CUIEvent uiEvent)
        {
            string name = uiEvent.m_srcWidget.transform.parent.name;
            int index = int.Parse(name.Substring(name.Length - 1));
            if (this.m_learnBtnHighLighted[index])
            {
                this.HighlightLearnBtn((SkillSlotType) index, false, false, true, false);
            }
        }

        public void OnBattleRequestUseSkill(CUIEvent uiEvent)
        {
            if (this.m_buttonHighLighted[(int) uiEvent.m_eventParams.m_skillSlotType])
            {
                this.Highlight(uiEvent.m_eventParams.m_skillSlotType, false, false, true, false);
            }
        }

        private void OnBattleSkillBtnDown(CUIEvent uiEvent)
        {
            if ((uiEvent.m_eventParams.m_skillSlotType == SkillSlotType.SLOT_SKILL_0) && this.m_buttonHighLighted[0])
            {
                this.Highlight(uiEvent.m_eventParams.m_skillSlotType, false, false, true, false);
            }
        }

        private void onCommonAttackTypeChange()
        {
            for (int i = 0; i < 2; i++)
            {
                if (this.m_RestSkillBtnHidden[i])
                {
                    GameObject obj2 = this.QueryRestSkillBtn((enRestSkillSlotType) i);
                    if (obj2 != null)
                    {
                        obj2.CustomSetActive(false);
                    }
                }
            }
        }

        private void onResetSkillButtonManager()
        {
            for (int i = 0; i < 8; i++)
            {
                if (this.m_buttonHidden[i])
                {
                    Singleton<CBattleSystem>.GetInstance().FightForm.GetButton((SkillSlotType) i).m_button.CustomSetActive(false);
                }
            }
        }

        private void onSkillLearnBtnStateChange(int inSlotType, bool bShow)
        {
            if (this.m_learnBtnHidden[inSlotType] && bShow)
            {
                GameObject learnSkillButton = Singleton<CBattleSystem>.GetInstance().FightForm.GetButton((SkillSlotType) inSlotType).GetLearnSkillButton();
                if (learnSkillButton != null)
                {
                    learnSkillButton.CustomSetActive(false);
                }
            }
        }

        public GameObject QueryRestSkillBtn(enRestSkillSlotType inSlotType)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
            if (form == null)
            {
                return null;
            }
            GameObject obj2 = null;
            enRestSkillSlotType type = inSlotType;
            if (type != enRestSkillSlotType.BTN_SKILL_SELHERO)
            {
                if (type != enRestSkillSlotType.BTN_SKILL_SELSOLDIER)
                {
                    return obj2;
                }
            }
            else
            {
                return form.GetWidget(0x44);
            }
            return form.GetWidget(0x39);
        }

        public List<GameObject> QuerySkillButtons(SkillSlotType inSlotType, bool bAll)
        {
            List<GameObject> list = new List<GameObject>();
            if (bAll)
            {
                for (int i = 0; i < 8; i++)
                {
                    list.Add(Singleton<CBattleSystem>.GetInstance().FightForm.GetButton((SkillSlotType) i).m_button);
                }
                return list;
            }
            if (inSlotType < SkillSlotType.SLOT_SKILL_COUNT)
            {
                list.Add(Singleton<CBattleSystem>.GetInstance().FightForm.GetButton(inSlotType).m_button);
            }
            return list;
        }

        public void Show(SkillSlotType inSlotType, bool bShow, bool bAll, bool bPlayShowAnim = false)
        {
            List<GameObject> list = this.QuerySkillButtons(inSlotType, bAll);
            for (int i = 0; i < list.Count; i++)
            {
                GameObject obj2 = list[i];
                if (obj2 != null)
                {
                    obj2.CustomSetActive(bShow);
                    if ((bShow && bPlayShowAnim) && (inSlotType != SkillSlotType.SLOT_SKILL_0))
                    {
                        Transform trans = obj2.transform.FindChild("Present");
                        if (trans != null)
                        {
                            CUICommonSystem.PlayAnimation(trans, enSkillButtonAnimationName.normal.ToString());
                        }
                    }
                }
            }
            if ((!bShow && (Singleton<CBattleSystem>.GetInstance().FightForm != null)) && (Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager != null))
            {
                if (bAll)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Vector2 screenPosition = new Vector2();
                        Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.SkillButtonUp(Singleton<CBattleSystem>.GetInstance().FightFormScript, (SkillSlotType) j, false, screenPosition);
                    }
                }
                else
                {
                    Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.SkillButtonUp(Singleton<CBattleSystem>.GetInstance().FightFormScript, inSlotType, false, new Vector2());
                }
            }
            if (inSlotType < SkillSlotType.SLOT_SKILL_COUNT)
            {
                this.m_buttonHidden[(int) inSlotType] = !bShow;
            }
        }

        public void ShowLearnBtn(SkillSlotType inSlotType, bool bShow, bool bAll)
        {
            if (bAll || ((inSlotType >= SkillSlotType.SLOT_SKILL_1) && (inSlotType <= SkillSlotType.SLOT_SKILL_3)))
            {
                List<GameObject> list = this.QuerySkillButtons(inSlotType, bAll);
                for (int i = 0; i < list.Count; i++)
                {
                    GameObject obj2 = list[i];
                    if (obj2 != null)
                    {
                        Transform transform = obj2.transform.FindChild("LearnBtn");
                        if (transform != null)
                        {
                            transform.gameObject.CustomSetActive(bShow);
                            this.m_learnBtnHidden[(int) inSlotType] = !bShow;
                        }
                    }
                }
            }
        }

        public void ShowRestkSkillBtn(enRestSkillSlotType inRestSkillType, bool bShow)
        {
            GameObject obj2 = this.QueryRestSkillBtn(inRestSkillType);
            if (obj2 != null)
            {
                if (bShow)
                {
                    if (this.m_RestSkillBtnHidden[(int) inRestSkillType])
                    {
                        obj2.CustomSetActive(true);
                        this.m_RestSkillBtnHidden[(int) inRestSkillType] = false;
                    }
                }
                else
                {
                    obj2.CustomSetActive(false);
                    this.m_RestSkillBtnHidden[(int) inRestSkillType] = true;
                }
            }
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleRequestUseSkill));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillBtnDown));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_LearnSkillBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleLearnSkillBtnClick));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAtkSelectHeroUp, new CUIEventManager.OnUIEventHandler(this.OnAtkSelectHero));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAtkSelectSoldierUp, new CUIEventManager.OnUIEventHandler(this.OnAtkSelectSoldier));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<int, bool>("HeroSkillLearnButtonStateChange", new Action<int, bool>(this.onSkillLearnBtnStateChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("ResetSkillButtonManager", new System.Action(this.onResetSkillButtonManager));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler("CommonAttack_Type_Changed", new System.Action(this.onCommonAttackTypeChange));
        }

        public void UpdateGamePausing(bool bPauseGame)
        {
            if (bPauseGame)
            {
                Singleton<CBattleGuideManager>.GetInstance().PauseGame(this, false);
                Singleton<CUIParticleSystem>.GetInstance().ClearAll(true);
            }
            else
            {
                Singleton<CBattleGuideManager>.GetInstance().ResumeGame(this);
            }
        }
    }
}

