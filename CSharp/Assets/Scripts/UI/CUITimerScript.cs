namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class CUITimerScript : CUIComponent
    {
        public bool m_closeBelongedFormWhenTimeup;
        private double m_currentTime;
        [HideInInspector]
        public enUIEventID[] m_eventIDs = new enUIEventID[Enum.GetValues(typeof(enTimerEventType)).Length];
        public stUIEventParams[] m_eventParams = new stUIEventParams[Enum.GetValues(typeof(enTimerEventType)).Length];
        private bool m_isPaused;
        private bool m_isRunning;
        private double m_lastOnChangedTime;
        public double m_onChangedIntervalTime = 1.0;
        public bool m_pausedWhenAppPaused = true;
        private double m_pauseElastTime;
        private double m_pauseTime;
        public bool m_runImmediately;
        private double m_startTime;
        public enTimerDisplayType m_timerDisplayType;
        private Text m_timerText;
        public Assets.Scripts.UI.enTimerType m_timerType;
        public double m_totalTime;

        public override void Close()
        {
            base.Close();
            this.ResetTime();
        }

        private void DispatchTimerEvent(enTimerEventType eventType)
        {
            if (this.m_eventIDs[(int) eventType] != enUIEventID.None)
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_srcFormScript = base.m_belongedFormScript;
                uIEvent.m_srcWidget = base.gameObject;
                uIEvent.m_srcWidgetScript = this;
                uIEvent.m_srcWidgetBelongedListScript = base.m_belongedListScript;
                uIEvent.m_srcWidgetIndexInBelongedList = base.m_indexInlist;
                uIEvent.m_pointerEventData = null;
                uIEvent.m_eventID = this.m_eventIDs[(int) eventType];
                uIEvent.m_eventParams = this.m_eventParams[(int) eventType];
                base.DispatchUIEvent(uIEvent);
            }
        }

        public void EndTimer()
        {
            this.ResetTime();
            this.m_isRunning = false;
        }

        public float GetCurrentTime()
        {
            return (float) this.m_currentTime;
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                if (this.m_runImmediately)
                {
                    this.StartTimer();
                }
                this.m_timerText = base.GetComponentInChildren<Text>(base.gameObject);
                if ((this.m_timerDisplayType == enTimerDisplayType.None) && (this.m_timerText != null))
                {
                    this.m_timerText.gameObject.CustomSetActive(false);
                }
                this.RefreshTimeDisplay();
            }
        }

        public bool IsRunning()
        {
            return this.m_isRunning;
        }

        public void OnApplicationPause(bool pause)
        {
            if (this.m_pausedWhenAppPaused)
            {
                if (pause)
                {
                    this.PauseTimer();
                }
                else
                {
                    this.ResumeTimer();
                }
            }
        }

        public void PauseTimer()
        {
            if (!this.m_isPaused)
            {
                this.m_pauseTime = Time.realtimeSinceStartup;
                this.m_isPaused = true;
            }
        }

        private void RefreshTimeDisplay()
        {
            if ((this.m_timerText != null) && (this.m_timerDisplayType != enTimerDisplayType.None))
            {
                int currentTime = (int) this.m_currentTime;
                switch (this.m_timerDisplayType)
                {
                    case enTimerDisplayType.H_M_S:
                    {
                        int num2 = currentTime / 0xe10;
                        currentTime -= num2 * 0xe10;
                        int num3 = currentTime / 60;
                        int num4 = currentTime - (num3 * 60);
                        this.m_timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", num2, num3, num4);
                        break;
                    }
                    case enTimerDisplayType.M_S:
                    {
                        int num7 = currentTime / 60;
                        int num8 = currentTime - (num7 * 60);
                        this.m_timerText.text = string.Format("{0:D2}:{1:D2}", num7, num8);
                        break;
                    }
                    case enTimerDisplayType.S:
                        this.m_timerText.text = string.Format("{0:D}", currentTime);
                        break;

                    case enTimerDisplayType.H_M:
                    {
                        int num5 = currentTime / 0xe10;
                        currentTime -= num5 * 0xe10;
                        int num6 = currentTime / 60;
                        this.m_timerText.text = string.Format("{0:D2}:{1:D2}", num5, num6);
                        break;
                    }
                    case enTimerDisplayType.D_H_M_S:
                    {
                        int num9 = currentTime / 0x15180;
                        currentTime -= num9 * 0x15180;
                        int num10 = currentTime / 0xe10;
                        currentTime -= num10 * 0xe10;
                        int num11 = currentTime / 60;
                        int num12 = currentTime - (num11 * 60);
                        object[] args = new object[] { num9, num10, num11, num12 };
                        this.m_timerText.text = string.Format("{0}天{1:D2}:{2:D2}:{3:D2}", args);
                        break;
                    }
                    case enTimerDisplayType.D_H_M:
                    {
                        int num13 = currentTime / 0x15180;
                        currentTime -= num13 * 0x15180;
                        int num14 = currentTime / 0xe10;
                        currentTime -= num14 * 0xe10;
                        int num15 = currentTime / 60;
                        this.m_timerText.text = string.Format("{0}天{1:D2}:{2:D2}", num13, num14, num15);
                        break;
                    }
                    case enTimerDisplayType.D:
                    {
                        int num16 = currentTime / 0x15180;
                        this.m_timerText.text = string.Format("{0}天", num16);
                        break;
                    }
                }
            }
        }

        public void ResetTime()
        {
            this.m_startTime = Time.realtimeSinceStartup;
            this.m_pauseTime = 0.0;
            this.m_pauseElastTime = 0.0;
            if (this.m_timerType == Assets.Scripts.UI.enTimerType.CountUp)
            {
                this.m_currentTime = 0.0;
            }
            else if (this.m_timerType == Assets.Scripts.UI.enTimerType.CountDown)
            {
                this.m_currentTime = this.m_totalTime;
            }
            this.m_lastOnChangedTime = this.m_currentTime;
        }

        public void ReStartTimer()
        {
            this.EndTimer();
            this.StartTimer();
        }

        public void ResumeTimer()
        {
            if (this.m_isPaused)
            {
                this.m_pauseElastTime += Time.realtimeSinceStartup - this.m_pauseTime;
                this.m_isPaused = false;
            }
        }

        public void SetCurrentTime(float time)
        {
            this.m_currentTime = time;
        }

        public void SetOnChangedIntervalTime(float intervalTime)
        {
            this.m_onChangedIntervalTime = intervalTime;
        }

        public void SetTimerEventId(enTimerEventType eventType, enUIEventID eventId)
        {
            int index = (int) eventType;
            if ((index >= 0) && (index < this.m_eventIDs.Length))
            {
                this.m_eventIDs[index] = eventId;
            }
        }

        public void SetTotalTime(float time)
        {
            this.m_totalTime = time;
            this.RefreshTimeDisplay();
        }

        public void StartTimer()
        {
            if (!this.m_isRunning)
            {
                this.ResetTime();
                this.m_isRunning = true;
                this.DispatchTimerEvent(enTimerEventType.TimeStart);
            }
        }

        private void Update()
        {
            if ((base.m_belongedFormScript == null) || !base.m_belongedFormScript.IsClosed())
            {
                this.UpdateTimer();
            }
        }

        private void UpdateTimer()
        {
            if (this.m_isRunning && !this.m_isPaused)
            {
                bool flag = false;
                double currentTime = this.m_currentTime;
                switch (this.m_timerType)
                {
                    case Assets.Scripts.UI.enTimerType.CountUp:
                        this.m_currentTime = (Time.realtimeSinceStartup - this.m_startTime) - this.m_pauseElastTime;
                        flag = this.m_currentTime >= this.m_totalTime;
                        break;

                    case Assets.Scripts.UI.enTimerType.CountDown:
                        this.m_currentTime = this.m_totalTime - ((Time.realtimeSinceStartup - this.m_startTime) - this.m_pauseElastTime);
                        flag = this.m_currentTime <= 0.0;
                        break;
                }
                if (((int) currentTime) != ((int) this.m_currentTime))
                {
                    this.RefreshTimeDisplay();
                }
                if (Mathf.Abs((float) (this.m_currentTime - this.m_lastOnChangedTime)) >= this.m_onChangedIntervalTime)
                {
                    this.m_lastOnChangedTime = this.m_currentTime;
                    this.DispatchTimerEvent(enTimerEventType.TimeChanged);
                }
                if (flag)
                {
                    this.EndTimer();
                    this.DispatchTimerEvent(enTimerEventType.TimeUp);
                    if (this.m_closeBelongedFormWhenTimeup)
                    {
                        base.m_belongedFormScript.Close();
                    }
                }
            }
        }
    }
}

