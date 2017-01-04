namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class CSoundChat
    {
        private bool bTimerInCD;
        private Image m_cooldownImage;
        private ulong m_startCooldownTimestamp;
        private int m_timer;
        private GameObject tipObj;
        private Text tipText;

        public void Clear()
        {
            this.tipObj = null;
            this.tipText = null;
            Singleton<CTimerManager>.instance.RemoveTimer(this.m_timer);
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Voice_Btn, new CUIEventManager.OnUIEventHandler(this.OnBattle_Voice_Btn));
        }

        private void EndCooldown()
        {
            this.m_startCooldownTimestamp = 0L;
            if (this.m_cooldownImage != null)
            {
                this.m_cooldownImage.enabled = false;
            }
        }

        public void Init(GameObject cooldownImage, GameObject tipObj)
        {
            this.tipObj = tipObj;
            this.tipObj.CustomSetActive(false);
            this.tipText = this.tipObj.transform.Find("Text").GetComponent<Text>();
            this.m_cooldownImage = cooldownImage.GetComponent<Image>();
            this.m_timer = Singleton<CTimerManager>.instance.AddTimer(MonoSingleton<VoiceSys>.GetInstance().TotalVoiceTime * 0x3e8, -1, new CTimer.OnTimeUpHandler(this.OnTimerEnd));
            Singleton<CTimerManager>.instance.PauseTimer(this.m_timer);
            Singleton<CTimerManager>.instance.ResetTimer(this.m_timer);
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Voice_Btn, new CUIEventManager.OnUIEventHandler(this.OnBattle_Voice_Btn));
        }

        public void OnBattle_Voice_Btn(CUIEvent uiEvent)
        {
            if (!Singleton<CBattleGuideManager>.instance.bPauseGame)
            {
                if (CFakePvPHelper.bInFakeSelect)
                {
                    if (!MonoSingleton<VoiceSys>.GetInstance().IsUseVoiceSysSetting)
                    {
                        this.tipText.text = "语音聊天未开启，请在设置界面中打开";
                        if (this.tipObj != null)
                        {
                            if (this.bTimerInCD)
                            {
                                return;
                            }
                            Singleton<CSoundManager>.instance.PlayBattleSound2D("UI_common_tishi");
                            this.StartTimer();
                            this.tipObj.CustomSetActive(true);
                            this.StartCooldown(0x7d0);
                        }
                        return;
                    }
                }
                else
                {
                    if (!MonoSingleton<VoiceSys>.GetInstance().GlobalVoiceSetting)
                    {
                        this.tipText.text = "暂时无法连接语音服务器，请稍后尝试";
                        if (this.tipObj != null)
                        {
                            if (this.bTimerInCD)
                            {
                                return;
                            }
                            Singleton<CSoundManager>.instance.PlayBattleSound2D("UI_common_tishi");
                            this.StartTimer();
                            this.tipObj.CustomSetActive(true);
                            this.StartCooldown(0x7d0);
                        }
                        return;
                    }
                    if (!MonoSingleton<VoiceSys>.GetInstance().IsUseVoiceSysSetting)
                    {
                        this.tipText.text = "语音聊天未开启，请在设置界面中打开";
                        if (this.tipObj != null)
                        {
                            if (this.bTimerInCD)
                            {
                                return;
                            }
                            Singleton<CSoundManager>.instance.PlayBattleSound2D("UI_common_tishi");
                            this.StartTimer();
                            this.tipObj.CustomSetActive(true);
                            this.StartCooldown(0x7d0);
                        }
                        return;
                    }
                    if (!MonoSingleton<VoiceSys>.GetInstance().IsInVoiceRoom())
                    {
                        this.tipText.text = "语音服务器未连接";
                        if (this.tipObj != null)
                        {
                            if (this.bTimerInCD)
                            {
                                return;
                            }
                            Singleton<CSoundManager>.instance.PlayBattleSound2D("UI_common_tishi");
                            this.StartTimer();
                            this.tipObj.CustomSetActive(true);
                            this.StartCooldown(0x7d0);
                        }
                        return;
                    }
                }
                if ((this.tipObj != null) && !this.bTimerInCD)
                {
                    Singleton<CSoundManager>.instance.PlayBattleSound2D("UI_common_tishi");
                    this.StartTimer();
                    this.tipObj.CustomSetActive(false);
                    MonoSingleton<VoiceSys>.GetInstance().OpenSoundInBattle();
                    this.StartCooldown((uint) (MonoSingleton<VoiceSys>.GetInstance().TotalVoiceTime * 0x3e8));
                }
            }
        }

        private void OnTimerEnd(int timersequence)
        {
            if (this.tipObj != null)
            {
                this.bTimerInCD = false;
                Singleton<CTimerManager>.instance.PauseTimer(this.m_timer);
                Singleton<CTimerManager>.instance.ResetTimer(this.m_timer);
                this.tipObj.CustomSetActive(false);
                MonoSingleton<VoiceSys>.GetInstance().CloseSoundInBattle();
                this.EndCooldown();
            }
        }

        private void StartCooldown(uint maxCooldownTime)
        {
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
                }
                else
                {
                    this.m_startCooldownTimestamp = 0L;
                    this.m_cooldownImage.enabled = false;
                }
            }
        }

        private void StartTimer()
        {
            this.bTimerInCD = true;
            Singleton<CTimerManager>.instance.ResumeTimer(this.m_timer);
        }

        public void Update()
        {
            this.UpdateCooldown();
        }

        private void UpdateCooldown()
        {
            if (this.m_startCooldownTimestamp != 0)
            {
                uint num = (uint) (Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.m_startCooldownTimestamp);
                if (num >= (MonoSingleton<VoiceSys>.GetInstance().TotalVoiceTime * 0x3e8))
                {
                    this.m_startCooldownTimestamp = 0L;
                    if (this.m_cooldownImage != null)
                    {
                        this.m_cooldownImage.enabled = false;
                    }
                }
                else if (this.m_cooldownImage != null)
                {
                    float num2 = (((MonoSingleton<VoiceSys>.GetInstance().TotalVoiceTime * 0x3e8) - num) * 1f) / ((float) (MonoSingleton<VoiceSys>.GetInstance().TotalVoiceTime * 0x3e8));
                    this.m_cooldownImage.CustomFillAmount(num2);
                }
            }
        }
    }
}

