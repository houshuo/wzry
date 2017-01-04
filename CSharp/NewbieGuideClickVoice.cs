using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickVoice : NewbieGuideBaseScript
{
    private float timer;
    private float timeToWait = 2f;

    protected override void Initialize()
    {
    }

    protected override bool IsDelegateClickEvent()
    {
        return true;
    }

    protected override void Update()
    {
        if (base.isInitialize)
        {
            base.Update();
        }
        else
        {
            this.timer += Time.deltaTime;
            if (this.timer >= this.timeToWait)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
                if (form != null)
                {
                    GameObject widget = form.GetWidget(0x36);
                    base.AddHighLightGameObject(widget, true, form, true);
                    base.Initialize();
                }
            }
        }
    }
}

