using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickAdvSelectToHall : NewbieGuideBaseScript
{
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
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
            GameObject gameObject = form.transform.FindChild("TopCommon/Button_Close").gameObject;
            base.AddHighLightGameObject(gameObject, true, form, true);
            base.Initialize();
        }
    }
}

