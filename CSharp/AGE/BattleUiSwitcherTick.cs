namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Drama")]
    public class BattleUiSwitcherTick : TickEvent
    {
        public bool bIncludeBattleHero;
        public bool bIncludeBattleUi = true;
        public bool bIncludeFpsForm;
        public bool bOpenOrClose;

        public override BaseEvent Clone()
        {
            BattleUiSwitcherTick tick = ClassObjPool<BattleUiSwitcherTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            BattleUiSwitcherTick tick = src as BattleUiSwitcherTick;
            this.bOpenOrClose = tick.bOpenOrClose;
            this.bIncludeBattleUi = tick.bIncludeBattleUi;
            this.bIncludeBattleHero = tick.bIncludeBattleHero;
            this.bIncludeFpsForm = tick.bIncludeFpsForm;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            if (this.bIncludeBattleUi)
            {
                CUIFormScript fightFormScript = Singleton<CBattleSystem>.GetInstance().FightFormScript;
                if (fightFormScript != null)
                {
                    if (this.bOpenOrClose)
                    {
                        fightFormScript.Appear(enFormHideFlag.HideByCustom, true);
                        if (GameSettings.EnableOutline)
                        {
                            Transform transform = Camera.main.transform.Find(Camera.main.name + " particles");
                            if (null == transform)
                            {
                            }
                        }
                    }
                    else
                    {
                        fightFormScript.Hide(enFormHideFlag.HideByCustom, true);
                        if (GameSettings.EnableOutline)
                        {
                            Transform transform2 = Camera.main.transform.Find(Camera.main.name + " particles");
                            if (null == transform2)
                            {
                            }
                        }
                    }
                }
            }
            if (this.bIncludeBattleHero)
            {
                CUIFormScript formScript = Singleton<CBattleHeroInfoPanel>.GetInstance().m_FormScript;
                if (formScript != null)
                {
                    if (this.bOpenOrClose)
                    {
                        formScript.Appear(enFormHideFlag.HideByCustom, true);
                    }
                    else
                    {
                        formScript.Hide(enFormHideFlag.HideByCustom, true);
                    }
                }
            }
            if (this.bIncludeFpsForm)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.FPS_FORM_PATH);
                if (form != null)
                {
                    if (this.bOpenOrClose)
                    {
                        form.Appear(enFormHideFlag.HideByCustom, true);
                    }
                    else
                    {
                        form.Hide(enFormHideFlag.HideByCustom, true);
                    }
                }
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

