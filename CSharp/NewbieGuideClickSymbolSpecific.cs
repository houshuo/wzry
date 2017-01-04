using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickSymbolSpecific : NewbieGuideBaseScript
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
                int num = base.currentConf.Param[0];
                string name = string.Format("Panel_SymbolEquip/Panel_SymbolPageRect/Panel_SymbolPage/itemCell{0}", num);
                Transform transform = form.transform.FindChild(name);
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

