namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class CUIInputReminderScript : CUIComponent
    {
        public enCountType m_countType;
        public Text m_displayReminderText;
        private InputField m_inputField;

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                this.m_inputField = base.gameObject.GetComponent<InputField>();
                this.m_inputField.onValueChange.RemoveAllListeners();
                this.m_inputField.onValueChange.AddListener(new UnityAction<string>(this.OnTextContentChanged));
                if (this.m_displayReminderText != null)
                {
                    if (this.m_countType == enCountType.CountDown)
                    {
                        this.m_displayReminderText.text = this.m_inputField.characterLimit.ToString();
                    }
                    else if (this.m_countType == enCountType.CountUp)
                    {
                        this.m_displayReminderText.text = 0.ToString();
                    }
                }
                base.Initialize(formScript);
            }
        }

        private void OnTextContentChanged(string text)
        {
            if (this.m_displayReminderText != null)
            {
                if (this.m_countType == enCountType.CountDown)
                {
                    this.m_displayReminderText.text = (this.m_inputField.characterLimit - text.Length).ToString();
                }
                else if (this.m_countType == enCountType.CountUp)
                {
                    this.m_displayReminderText.text = text.Length.ToString();
                }
            }
        }

        public enum enCountType
        {
            CountDown,
            CountUp
        }
    }
}

