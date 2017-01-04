namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [MessageHandlerClass]
    public class CRollingSystem : Singleton<CRollingSystem>
    {
        private bool m_isInRollingCD;
        private List<RollingInfo> m_rollingInfos = new List<RollingInfo>();
        private const int PriorityEventRatio = 1;
        private const int PriorityHornRatio = 10;
        private const int PriorityIDIPRatio = 100;
        private const int PriorityMax = 0x3e8;
        private const int PriorityMin = 1;
        private const int ResetRollingInfoPriorityPeriod = 300;
        private const int RollingCDMilliSeconds = 0x4e20;

        private static int CalculatePriority(COM_ROLLINGMSG_TYPE type, byte innerPriority)
        {
            if (type == COM_ROLLINGMSG_TYPE.COM_ROLLINGMSG_TYPE_IDIP)
            {
                return (100 + innerPriority);
            }
            if (type == COM_ROLLINGMSG_TYPE.COM_ROLLINGMSG_TYPE_HORN)
            {
                return (10 + innerPriority);
            }
            if (type == COM_ROLLINGMSG_TYPE.COM_ROLLINGMSG_TYPE_EVENT)
            {
                return (1 + innerPriority);
            }
            object[] inParameters = new object[] { type };
            DebugHelper.Assert(false, "CalculatePriority.CalculatePriority(): invalid rolling msg type {0}", inParameters);
            return 1;
        }

        private static byte CalculateRepeatCount(uint startTime, uint endTime, ushort repeatPeriod)
        {
            byte num = 0;
            if ((endTime > startTime) && (repeatPeriod != 0))
            {
                num = (byte) ((endTime - startTime) / repeatPeriod);
            }
            return num;
        }

        private int GetNextShowRollingInfoIndex()
        {
            if (this.m_rollingInfos.Count == 0)
            {
                return -1;
            }
            int num = 0;
            for (int i = 0; i < this.m_rollingInfos.Count; i++)
            {
                if (this.m_rollingInfos[i].priority > num)
                {
                    num = i;
                }
            }
            return num;
        }

        private int GetOnShowRollingInfoIndex()
        {
            for (int i = 0; i < this.m_rollingInfos.Count; i++)
            {
                if (this.m_rollingInfos[i].isShowing)
                {
                    return i;
                }
            }
            return -1;
        }

        private void HandleOnShowRollingInfo()
        {
            int onShowRollingInfoIndex = this.GetOnShowRollingInfoIndex();
            if ((0 <= onShowRollingInfoIndex) && (onShowRollingInfoIndex < this.m_rollingInfos.Count))
            {
                if (this.m_rollingInfos[onShowRollingInfoIndex].repeatCount > 1)
                {
                    RollingInfo local1 = this.m_rollingInfos[onShowRollingInfoIndex];
                    local1.repeatCount = (byte) (local1.repeatCount - 1);
                    this.m_rollingInfos[onShowRollingInfoIndex].priority = 1;
                    this.m_rollingInfos[onShowRollingInfoIndex].isShowing = false;
                }
                else
                {
                    this.m_rollingInfos.RemoveAt(onShowRollingInfoIndex);
                }
            }
        }

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIComponent_AutoScroller_Scroll_Finish, new CUIEventManager.OnUIEventHandler(this.OnAutoScrollerScrollFinish));
        }

        private bool IsNeedResetResetPriority(RollingInfo rollingInfo)
        {
            return ((rollingInfo.repeatCount > 0) && ((CRoleInfo.GetCurrentUTCTime() - rollingInfo.resetPriorityTime) > 300));
        }

        private void OnAutoScrollerScrollFinish(CUIEvent uiEvent)
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if ((form != null) && (uiEvent.m_srcWidget == form.GetWidget(4)))
            {
                this.HandleOnShowRollingInfo();
                this.ResetRollingInfoPriority();
                this.m_isInRollingCD = true;
                Singleton<CTimerManager>.GetInstance().AddTimer(0x4e20, 1, new CTimer.OnTimeUpHandler(this.OnRollingCDOver));
            }
        }

        private void OnRollingCDOver(int timerSequence)
        {
            this.m_isInRollingCD = false;
            if (Singleton<LobbyLogic>.GetInstance().CanShowRolling)
            {
                this.StartRolling();
            }
        }

        [MessageHandler(0x5aa)]
        public static void ReceiveRollingMsgNtf(CSPkg msg)
        {
            SCPKG_ROLLINGMSG_NTF stRollingMsgNtf = msg.stPkgData.stRollingMsgNtf;
            int num = Mathf.Min(stRollingMsgNtf.astRollingMsg.Length, stRollingMsgNtf.bMsgCnt);
            for (int i = 0; i < num; i++)
            {
                RollingInfo item = new RollingInfo();
                CSDT_ROLLING_MSG csdt_rolling_msg = stRollingMsgNtf.astRollingMsg[i];
                item.resetPriorityTime = CRoleInfo.GetCurrentUTCTime();
                item.priority = CalculatePriority((COM_ROLLINGMSG_TYPE) csdt_rolling_msg.bType, csdt_rolling_msg.bPriority);
                item.repeatCount = CalculateRepeatCount(csdt_rolling_msg.dwStartTime, csdt_rolling_msg.dwEndTime, csdt_rolling_msg.wPeriod);
                item.content = Utility.UTF8Convert(csdt_rolling_msg.szContent, csdt_rolling_msg.wContentLen);
                if (csdt_rolling_msg.bType == 0)
                {
                    item.content = "<color=#00ff00>" + item.content + "</color>";
                }
                if (csdt_rolling_msg.bIsChat == 0)
                {
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<string>(EventID.ROLLING_SYSTEM_CHAT_INFO_RECEIVED, item.content);
                }
                Singleton<CRollingSystem>.GetInstance().m_rollingInfos.Add(item);
            }
            Singleton<CRollingSystem>.GetInstance().StartRolling();
        }

        private void ResetRollingInfoPriority()
        {
            for (int i = 0; i < this.m_rollingInfos.Count; i++)
            {
                if (this.IsNeedResetResetPriority(this.m_rollingInfos[i]))
                {
                    this.m_rollingInfos[i].priority = 0x3e8;
                    this.m_rollingInfos[i].resetPriorityTime = CRoleInfo.GetCurrentUTCTime();
                }
            }
        }

        public void StartRolling()
        {
            if (!this.m_isInRollingCD)
            {
                CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
                if (form != null)
                {
                    CUIAutoScroller component = form.GetWidget(4).GetComponent<CUIAutoScroller>();
                    this.StartRolling(component);
                }
            }
        }

        private void StartRolling(CUIAutoScroller autoScroller)
        {
            if (((autoScroller != null) && !autoScroller.IsScrollRunning()) && (Singleton<LobbyLogic>.GetInstance().CanShowRolling && !this.m_isInRollingCD))
            {
                RectTransform transform = autoScroller.transform as RectTransform;
                if (transform != null)
                {
                    if (Singleton<CLoudSpeakerSys>.instance.IsLoudSpeakerShowing())
                    {
                        transform.anchorMin = new Vector2(0.661f, 0.5f);
                    }
                    else
                    {
                        transform.anchorMin = new Vector2(0.3f, 0.5f);
                    }
                }
                int nextShowRollingInfoIndex = this.GetNextShowRollingInfoIndex();
                if ((0 <= nextShowRollingInfoIndex) && (nextShowRollingInfoIndex < this.m_rollingInfos.Count))
                {
                    autoScroller.SetText(this.m_rollingInfos[nextShowRollingInfoIndex].content);
                    autoScroller.StartAutoScroll(false);
                    this.m_rollingInfos[nextShowRollingInfoIndex].isShowing = true;
                }
            }
        }

        public void StopRolling()
        {
            CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
            if (form != null)
            {
                form.GetWidget(4).GetComponent<CUIAutoScroller>().StopAutoScroll();
            }
        }
    }
}

