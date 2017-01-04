namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Drama")]
    public class ForbidInputTick : TickEvent
    {
        public bool bForbid = true;
        [ObjectTemplate(new System.Type[] {  })]
        public int srcId;

        public override BaseEvent Clone()
        {
            ForbidInputTick tick = ClassObjPool<ForbidInputTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            ForbidInputTick tick = src as ForbidInputTick;
            this.srcId = tick.srcId;
            this.bForbid = tick.bForbid;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.srcId = 0;
            this.bForbid = true;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            GameObject obj2 = (Singleton<CBattleSystem>.GetInstance().FightFormScript == null) ? null : Singleton<CBattleSystem>.GetInstance().FightFormScript.gameObject;
            if (obj2 != null)
            {
                GameObject gameObject = obj2.transform.FindChild("Joystick").gameObject;
                bool bActive = !this.bForbid;
                CUIJoystickScript component = gameObject.GetComponent<CUIJoystickScript>();
                if (component != null)
                {
                    component.ResetAxis();
                }
                gameObject.CustomSetActive(bActive);
                component = gameObject.GetComponent<CUIJoystickScript>();
                if (component != null)
                {
                    component.ResetAxis();
                }
                if ((this.bForbid && (Singleton<CBattleSystem>.GetInstance().FightForm != null)) && (Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager != null))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 screenPosition = new Vector2();
                        Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.SkillButtonUp(Singleton<CBattleSystem>.GetInstance().FightFormScript, (SkillSlotType) i, false, screenPosition);
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

