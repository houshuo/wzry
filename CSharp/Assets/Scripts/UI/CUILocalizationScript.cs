namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class CUILocalizationScript : CUIComponent
    {
        [HideInInspector]
        public string m_key;
        private Text m_text;

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                this.m_text = base.gameObject.GetComponent<Text>();
                this.SetDisplay();
            }
        }

        public void SetDisplay()
        {
            if (((this.m_text != null) && !string.IsNullOrEmpty(this.m_key)) && Singleton<CTextManager>.GetInstance().IsTextLoaded())
            {
                this.m_text.text = Singleton<CTextManager>.GetInstance().GetText(this.m_key);
            }
        }

        public void SetKey(string key)
        {
            this.m_key = key;
            this.SetDisplay();
        }
    }
}

