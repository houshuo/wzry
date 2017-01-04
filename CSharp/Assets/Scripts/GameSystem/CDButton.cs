namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CDButton
    {
        private GameObject m_button;
        private Image m_buttonImage;
        private System.Action m_callback;
        private Image m_cooldownImage;
        private Image m_highLightImage;
        private Image m_iconImage;
        private uint m_maxCooldownTime;
        private ulong m_startCooldownTimestamp;
        private CUIEventScript m_uiEventScript;

        public CDButton(GameObject button)
        {
            if (button != null)
            {
                this.m_button = button;
                this.m_buttonImage = this.m_button.GetComponent<Image>();
                DebugHelper.Assert(this.m_buttonImage != null, "CDButton, m_buttonImage == null");
                Transform transform = button.transform.FindChild("high");
                if (transform != null)
                {
                    this.m_highLightImage = transform.gameObject.GetComponent<Image>();
                }
                Transform transform2 = button.transform.FindChild("icon");
                if (transform2 != null)
                {
                    this.m_iconImage = transform2.gameObject.GetComponent<Image>();
                }
                Transform transform3 = button.transform.FindChild("cd");
                if (transform3 != null)
                {
                    this.m_cooldownImage = transform3.gameObject.GetComponent<Image>();
                }
                this.m_uiEventScript = button.GetComponent<CUIEventScript>();
                DebugHelper.Assert(this.m_uiEventScript != null, "CDButton, m_uiEventScript == null");
            }
        }

        public void Clear()
        {
            this.m_buttonImage = null;
            this.m_iconImage = null;
            this.m_highLightImage = null;
            this.m_cooldownImage = null;
            this.m_uiEventScript = null;
            this.m_button = null;
            this.m_callback = null;
        }

        public void SetHighLight(bool highLight)
        {
            if (this.m_highLightImage != null)
            {
                this.m_highLightImage.gameObject.CustomSetActive(highLight);
            }
        }

        public void StartCooldown(uint maxCooldownTime, System.Action callback = null)
        {
            this.m_maxCooldownTime = maxCooldownTime;
            this.m_callback = callback;
            this.SetHighLight(false);
            if (this.m_cooldownImage != null)
            {
                if (maxCooldownTime > 0)
                {
                    this.m_cooldownImage.enabled = true;
                    this.m_cooldownImage.type = Image.Type.Filled;
                    this.m_cooldownImage.fillMethod = Image.FillMethod.Radial360;
                    this.m_cooldownImage.fillOrigin = 2;
                    this.m_startCooldownTimestamp = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                    this.m_cooldownImage.CustomFillAmount(1f);
                    if (this.m_uiEventScript != null)
                    {
                        this.m_uiEventScript.enabled = false;
                    }
                }
                else
                {
                    this.m_startCooldownTimestamp = 0L;
                    this.m_cooldownImage.enabled = false;
                }
            }
        }

        public void Update()
        {
            if ((this.m_startCooldownTimestamp != 0) && (this.m_maxCooldownTime != 0))
            {
                uint num = (uint) (Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.m_startCooldownTimestamp);
                if (num >= this.m_maxCooldownTime)
                {
                    this.m_startCooldownTimestamp = 0L;
                    if (this.m_cooldownImage != null)
                    {
                        this.m_cooldownImage.enabled = false;
                    }
                    if (this.m_uiEventScript != null)
                    {
                        this.m_uiEventScript.enabled = true;
                    }
                    if (this.m_callback != null)
                    {
                        this.m_callback();
                    }
                }
                else if (this.m_cooldownImage != null)
                {
                    this.m_cooldownImage.CustomFillAmount(((float) (this.m_maxCooldownTime - num)) / ((float) this.m_maxCooldownTime));
                }
            }
        }
    }
}

