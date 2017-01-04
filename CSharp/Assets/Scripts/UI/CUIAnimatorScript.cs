namespace Assets.Scripts.UI
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class CUIAnimatorScript : CUIComponent
    {
        private Animator m_animator;
        private int m_currentAnimatorStateCounter;
        [HideInInspector]
        public enUIEventID[] m_eventIDs = new enUIEventID[Enum.GetValues(typeof(enAnimatorEventType)).Length];
        public stUIEventParams[] m_eventParams = new stUIEventParams[Enum.GetValues(typeof(enAnimatorEventType)).Length];
        private bool m_isEnableAnimatorBeHide = true;
        private bool m_isEnableBeHide = true;

        private void DispatchAnimatorEvent(enAnimatorEventType animatorEventType, string stateName)
        {
            if (this.m_eventIDs[(int) animatorEventType] != enUIEventID.None)
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_srcFormScript = base.m_belongedFormScript;
                uIEvent.m_srcWidget = base.gameObject;
                uIEvent.m_srcWidgetScript = this;
                uIEvent.m_srcWidgetBelongedListScript = base.m_belongedListScript;
                uIEvent.m_srcWidgetIndexInBelongedList = base.m_indexInlist;
                uIEvent.m_pointerEventData = null;
                uIEvent.m_eventID = this.m_eventIDs[(int) animatorEventType];
                uIEvent.m_eventParams = this.m_eventParams[(int) animatorEventType];
                uIEvent.m_eventParams.tagStr = stateName;
                base.DispatchUIEvent(uIEvent);
            }
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                this.m_animator = base.gameObject.GetComponent<Animator>();
                this.m_isEnableBeHide = base.enabled;
                this.m_isEnableAnimatorBeHide = this.m_animator.enabled;
            }
        }

        public bool IsAnimationStopped(string animationName)
        {
            return (string.IsNullOrEmpty(animationName) || !string.Equals(this.m_currentAnimatorStateName, animationName));
        }

        public void PlayAnimator(string stateName)
        {
            if (this.m_animator == null)
            {
                this.m_animator = base.gameObject.GetComponent<Animator>();
            }
            if (!this.m_animator.enabled)
            {
                this.m_animator.enabled = true;
            }
            this.m_animator.Play(stateName, 0, 0f);
            this.m_currentAnimatorStateName = stateName;
            this.m_animator.Update(0f);
            this.m_animator.Update(0f);
            this.m_currentAnimatorStateCounter = (int) this.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            this.DispatchAnimatorEvent(enAnimatorEventType.AnimatorStart, this.m_currentAnimatorStateName);
        }

        public void SetAnimatorEnable(bool isEnable)
        {
            if (this.m_animator != null)
            {
                this.m_animator.enabled = isEnable;
                base.enabled = isEnable;
            }
        }

        public void SetBool(string name, bool value)
        {
            this.m_animator.SetBool(name, value);
        }

        public void SetInteger(string name, int value)
        {
            this.m_animator.SetInteger(name, value);
        }

        public void SetUIEvent(enAnimatorEventType eventType, enUIEventID eventID, stUIEventParams eventParams)
        {
            this.m_eventIDs[(int) eventType] = eventID;
            this.m_eventParams[(int) eventType] = eventParams;
        }

        public void StopAnimator()
        {
        }

        private void Update()
        {
            if ((((base.m_belongedFormScript == null) || !base.m_belongedFormScript.IsClosed()) && (this.m_currentAnimatorStateName != null)) && (((int) this.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime) > this.m_currentAnimatorStateCounter))
            {
                this.m_animator.StopPlayback();
                string currentAnimatorStateName = this.m_currentAnimatorStateName;
                this.m_currentAnimatorStateName = null;
                this.m_currentAnimatorStateCounter = 0;
                this.DispatchAnimatorEvent(enAnimatorEventType.AnimatorEnd, currentAnimatorStateName);
            }
        }

        public string m_currentAnimatorStateName { get; private set; }
    }
}

