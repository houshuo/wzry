using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickGetSymbol : NewbieGuideBaseScript
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
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
            if (form != null)
            {
                Transform transform = form.transform.Find("Panel_SymbolEquip/Panel_SymbolBag/Panel_BagList/bottomPanel/getSymbolBtn");
                if (transform != null)
                {
                    GameObject gameObject = transform.gameObject;
                    if (gameObject.activeInHierarchy)
                    {
                        base.AddHighLightGameObject(gameObject, true, form, true);
                        base.Initialize();
                    }
                }
            }
        }
    }
}

