using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickHeroSkill : NewbieGuideBaseScript
{
    protected override void Clear()
    {
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroSelect_Skill_Down, new CUIEventManager.OnUIEventHandler(this.onDownHandler));
        Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroSelect_Skill_Up, new CUIEventManager.OnUIEventHandler(this.onUpHandler));
        base.Clear();
    }

    protected override void Initialize()
    {
    }

    protected override bool IsDelegateClickEvent()
    {
        return true;
    }

    private void onDownHandler(CUIEvent evt)
    {
        base.HideHighlighterAndText();
    }

    private void onUpHandler(CUIEvent evt)
    {
        this.ClickHandler(evt);
    }

    protected override void Update()
    {
        if (base.isInitialize)
        {
            base.Update();
        }
        else
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
            if (form != null)
            {
                if (form.gameObject.transform.Find("PanelLeft/ListHostHeroInfoFull").gameObject.activeSelf)
                {
                    MonoSingleton<NewbieGuideManager>.GetInstance().StopCurrentGuide();
                }
                else
                {
                    GameObject gameObject = form.transform.FindChild("Other/SkillList").gameObject;
                    if (gameObject != null)
                    {
                        GameObject baseGo = gameObject.GetComponent<CUIListScript>().GetElemenet(0).transform.FindChild("heroSkillItemCell").gameObject;
                        if (baseGo != null)
                        {
                            base.AddHighLightGameObject(baseGo, true, form, true);
                            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_Skill_Down, new CUIEventManager.OnUIEventHandler(this.onDownHandler));
                            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSelect_Skill_Up, new CUIEventManager.OnUIEventHandler(this.onUpHandler));
                            base.Initialize();
                        }
                    }
                }
            }
        }
    }
}

