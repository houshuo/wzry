﻿using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickSettingOp : NewbieGuideBaseScript
{
    protected override void Initialize()
    {
    }

    protected override bool IsDelegateClickEvent()
    {
        return true;
    }

    public override bool IsTimeOutSkip()
    {
        return false;
    }

    protected override void Update()
    {
        if (base.isInitialize)
        {
            base.Update();
        }
        else
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSettingsSys.SETTING_FORM);
            if (form != null)
            {
                GameObject gameObject = form.transform.FindChild("BattleBg/TopCommon/Panel_Menu/ListMenu/ScrollRect/Content/ListElement_1").gameObject;
                if (gameObject.activeInHierarchy)
                {
                    base.AddHighLightGameObject(gameObject, true, form, true);
                    base.Initialize();
                }
            }
        }
    }
}

