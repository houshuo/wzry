using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideBigMapGign : NewbieGuideBaseScript
{
    private CUIEventScript eventScript;
    private GameObject highlighter;
    private GameObject info;
    private const int SKIP_TIME = 0x2710;

    protected override void ClickHandler(CUIEvent uiEvent)
    {
        this.eventScript.onClick = (CUIEventScript.OnUIEventHandler) Delegate.Remove(this.eventScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
        this.info.CustomSetActive(false);
        this.highlighter.CustomSetActive(false);
        Singleton<CBattleGuideManager>.GetInstance().OpenFormShared(CBattleGuideManager.EBattleGuideFormType.BigMapGuide, 0x5dc, true);
        this.CompleteHandler();
    }

    protected override void Initialize()
    {
    }

    protected override bool IsDelegateClickEvent()
    {
        return true;
    }

    protected override bool IsShowGuideMask()
    {
        return false;
    }

    public override bool IsTimeOutSkip()
    {
        return false;
    }

    private void OnTimeUp(int delt)
    {
        this.eventScript.onClick = (CUIEventScript.OnUIEventHandler) Delegate.Remove(this.eventScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
        this.info.CustomSetActive(false);
        this.highlighter.CustomSetActive(false);
        MonoSingleton<NewbieGuideManager>.GetInstance().StopCurrentGuide();
    }

    protected override void Update()
    {
        if (base.isInitialize)
        {
            base.Update();
        }
        else
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
            if (form != null)
            {
                Transform transform = form.GetWidget(0x47).transform;
                this.info = transform.FindChild("info").gameObject;
                this.highlighter = transform.FindChild("highlighter").gameObject;
                if ((this.info != null) && (this.highlighter != null))
                {
                    this.info.CustomSetActive(true);
                    this.highlighter.CustomSetActive(true);
                    this.eventScript = transform.GetComponent<CUIEventScript>();
                    this.eventScript.onClick = (CUIEventScript.OnUIEventHandler) Delegate.Combine(this.eventScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
                    Singleton<CTimerManager>.GetInstance().AddTimer(0x2710, 1, new CTimer.OnTimeUpHandler(this.OnTimeUp));
                }
            }
            base.isInitialize = true;
            base.isGuiding = true;
            NewbieGuideScriptControl.CloseGuideForm();
        }
    }
}

