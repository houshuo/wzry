using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickSymbol : NewbieGuideBaseScript
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
                Transform transform = form.transform.Find("Panel_SymbolEquip/Panel_SymbolBag/Panel_BagList/List");
                if (transform != null)
                {
                    CUIListScript component = transform.GetComponent<CUIListScript>();
                    if (component != null)
                    {
                        int index = base.currentConf.Param[1];
                        if (base.currentConf.Param[0] > 0)
                        {
                            index = Singleton<CSymbolSystem>.GetInstance().GetSymbolListIndex(NewbieGuideCheckTriggerConditionUtil.AvailableSymbolId);
                        }
                        CUIListElementScript elemenet = component.GetElemenet(index);
                        GameObject baseGo = (elemenet == null) ? null : elemenet.gameObject;
                        if ((baseGo != null) && baseGo.activeInHierarchy)
                        {
                            base.AddHighLightGameObject(baseGo, true, form, true);
                            base.Initialize();
                        }
                    }
                }
            }
        }
    }
}

