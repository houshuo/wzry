using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickEquipPanelClose : NewbieGuideBaseScript
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
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CBattleEquipSystem.s_equipFormPath);
            if (form != null)
            {
                GameObject gameObject = form.transform.FindChild("Panel_Main/Button_Close").gameObject;
                DebugHelper.Assert(gameObject != null, "Can't find Button_Close~!!");
                base.AddHighLightGameObject(gameObject, true, form, true);
                base.Initialize();
            }
        }
    }
}

