namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine.UI;

    public class CUIToggleListElementScript : CUIListElementScript
    {
        private Toggle m_toggle;

        public override void ChangeDisplay(bool selected)
        {
            base.ChangeDisplay(selected);
            if (this.m_toggle != null)
            {
                this.m_toggle.isOn = selected;
            }
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                this.m_toggle = base.GetComponentInChildren<Toggle>(base.gameObject);
                if (this.m_toggle != null)
                {
                    this.m_toggle.interactable = false;
                }
            }
        }
    }
}

