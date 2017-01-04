using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickBuyEquip : NewbieGuideBaseScript
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
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
            if (form != null)
            {
                GameObject widget = form.GetWidget(0x2f);
                DebugHelper.Assert(widget != null, "can't find equip item!");
                if (widget.activeInHierarchy && !form.IsHided())
                {
                    base.AddHighLightGameObject(widget, true, form, true);
                    base.Initialize();
                }
            }
        }
    }
}

