namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class CReplayKitSys : Singleton<CReplayKitSys>
    {
        [CompilerGenerated]
        private static CTimer.OnTimeUpHandler <>f__am$cache4;
        private bool m_capable;
        private bool m_needDiscard;
        private uint MIN_SPACE_LIMIT = 200;
        private uint WARNING_SPACE_LIMIT = 500;

        public void ChangeReplayKitStatus(Transform container, Status status)
        {
            if (container != null)
            {
                if (!this.Cap)
                {
                    container.gameObject.CustomSetActive(false);
                }
                else if (!GameSettings.EnableReplayKit)
                {
                    container.gameObject.CustomSetActive(false);
                }
                else if (container.gameObject.activeSelf)
                {
                    Transform transform = container.transform.Find("Recording");
                    Transform transform2 = container.transform.Find("Paused");
                    Transform transform3 = container.transform.Find("Transition");
                    if ((transform == null) || (transform2 == null))
                    {
                        container.gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        switch (status)
                        {
                            case Status.Recording:
                                transform.gameObject.CustomSetActive(true);
                                transform2.gameObject.CustomSetActive(false);
                                transform3.gameObject.CustomSetActive(false);
                                break;

                            case Status.Paused:
                                transform.gameObject.CustomSetActive(false);
                                transform2.gameObject.CustomSetActive(true);
                                transform3.gameObject.CustomSetActive(false);
                                break;

                            case Status.Transition:
                                transform.gameObject.CustomSetActive(false);
                                transform2.gameObject.CustomSetActive(false);
                                transform3.gameObject.CustomSetActive(true);
                                break;
                        }
                    }
                }
            }
        }

        public StorageStatus CheckStorage(bool showTips = true)
        {
            return StorageStatus.Ok;
        }

        public override void Init()
        {
            base.Init();
            this.m_capable = false;
            this.m_needDiscard = false;
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReplayKit_Start_Recording, new CUIEventManager.OnUIEventHandler(this.OnRecord));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReplayKit_Pause_Recording, new CUIEventManager.OnUIEventHandler(this.OnPause));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReplayKit_Preview_Record, new CUIEventManager.OnUIEventHandler(this.OnPreview));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.ReplayKit_Discard_Record, new CUIEventManager.OnUIEventHandler(this.OnDiscard));
            Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.GLOBAL_SERVER_TO_CLIENT_CFG_READY, new System.Action(this.SetReplayKitGlobalCfg));
        }

        public void InitReplayKit(Transform container, bool autoRecord = false, bool autoPreview = false)
        {
            if (container != null)
            {
                if (!this.Cap)
                {
                    container.gameObject.CustomSetActive(false);
                }
                else if (!GameSettings.EnableReplayKit)
                {
                    container.gameObject.CustomSetActive(false);
                }
                else if (this.CheckStorage(false) == StorageStatus.Disable)
                {
                    container.gameObject.CustomSetActive(false);
                    CUIEvent uiEvent = new CUIEvent {
                        m_eventID = enUIEventID.ReplayKit_Pause_Recording
                    };
                    uiEvent.m_eventParams.tag2 = 1;
                    Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
                }
                else
                {
                    if (Singleton<BattleLogic>.GetInstance().isRuning)
                    {
                        SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                        if ((curLvelContext != null) && (curLvelContext.IsGameTypeGuide() || !curLvelContext.IsMobaMode()))
                        {
                            container.gameObject.CustomSetActive(false);
                            CUIEvent event3 = new CUIEvent {
                                m_eventID = enUIEventID.ReplayKit_Pause_Recording
                            };
                            event3.m_eventParams.tag2 = 1;
                            Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event3);
                            return;
                        }
                    }
                    else if (Singleton<CHeroSelectBaseSystem>.instance.m_isInHeroSelectState && (Singleton<CHeroSelectBaseSystem>.GetInstance().IsSpecTraingMode() || !Singleton<CHeroSelectBaseSystem>.GetInstance().IsMobaMode()))
                    {
                        container.gameObject.CustomSetActive(false);
                        CUIEvent event4 = new CUIEvent {
                            m_eventID = enUIEventID.ReplayKit_Pause_Recording
                        };
                        event4.m_eventParams.tag2 = 1;
                        Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(event4);
                        return;
                    }
                    Transform transform = container.transform.Find("Recording");
                    Transform transform2 = container.transform.Find("Paused");
                    Transform transform3 = container.transform.Find("Transition");
                    if ((transform == null) || (transform2 == null))
                    {
                        container.gameObject.CustomSetActive(false);
                    }
                    else
                    {
                        CUIEventScript component = transform.GetComponent<CUIEventScript>();
                        CUIEventScript script2 = transform2.GetComponent<CUIEventScript>();
                        if (component != null)
                        {
                            component.m_onClickEventID = enUIEventID.ReplayKit_Pause_Recording;
                            if (autoPreview)
                            {
                                component.m_onClickEventParams.tag = 1;
                            }
                        }
                        if (script2 != null)
                        {
                            script2.m_onClickEventID = enUIEventID.ReplayKit_Start_Recording;
                        }
                        if (autoRecord)
                        {
                            if (GameSettings.EnableReplayKitAutoMode)
                            {
                                transform.gameObject.CustomSetActive(true);
                                transform2.gameObject.CustomSetActive(false);
                                transform3.gameObject.CustomSetActive(false);
                                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.ReplayKit_Start_Recording);
                            }
                            else if (this.IsRecording())
                            {
                                transform.gameObject.CustomSetActive(true);
                                transform2.gameObject.CustomSetActive(false);
                                transform3.gameObject.CustomSetActive(false);
                            }
                            else
                            {
                                transform.gameObject.CustomSetActive(false);
                                transform2.gameObject.CustomSetActive(true);
                                transform3.gameObject.CustomSetActive(false);
                            }
                        }
                        else if (this.IsRecording())
                        {
                            transform.gameObject.CustomSetActive(true);
                            transform2.gameObject.CustomSetActive(false);
                            transform3.gameObject.CustomSetActive(false);
                        }
                        else
                        {
                            transform.gameObject.CustomSetActive(false);
                            transform2.gameObject.CustomSetActive(true);
                            transform3.gameObject.CustomSetActive(false);
                        }
                    }
                }
            }
        }

        public void InitReplayKitRecordBtn(Transform container)
        {
            if (container != null)
            {
                if (!this.Cap || !GameSettings.EnableReplayKit)
                {
                    container.gameObject.CustomSetActive(false);
                }
                else
                {
                    CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(container.gameObject, "Record");
                    if (componetInChild != null)
                    {
                        componetInChild.m_onClickEventID = enUIEventID.ReplayKit_Preview_Record;
                    }
                    Transform transform = container.transform.Find("Extra/BtnGroup/BtnYes");
                    if (transform != null)
                    {
                        CUIEventScript component = transform.GetComponent<CUIEventScript>();
                        if (component != null)
                        {
                            component.m_onClickEventID = enUIEventID.ReplayKit_Preview_Record;
                        }
                    }
                    Transform transform2 = container.transform.Find("Extra/BtnGroup/BtnNo");
                    if (transform2 != null)
                    {
                        CUIEventScript script3 = transform2.GetComponent<CUIEventScript>();
                        if (script3 != null)
                        {
                            script3.m_onClickEventID = enUIEventID.ReplayKit_Discard_Record;
                        }
                    }
                    container.gameObject.CustomSetActive(false);
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = sequence => Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.ReplayKit_Pause_Recording);
                    }
                    Singleton<CTimerManager>.GetInstance().AddTimer(0x3e8, 1, <>f__am$cache4);
                }
            }
        }

        public bool IsRecording()
        {
            return ((this.Cap && GameSettings.EnableReplayKit) && false);
        }

        private void OnDiscard(CUIEvent uiEvent)
        {
            if (this.m_capable && GameSettings.EnableReplayKit)
            {
            }
        }

        private void OnPause(CUIEvent uiEvent)
        {
            if (this.m_capable && GameSettings.EnableReplayKit)
            {
                bool flag = uiEvent.m_eventParams.tag != 0;
                this.m_needDiscard = uiEvent.m_eventParams.tag2 != 0;
            }
        }

        private void OnPreview(CUIEvent uiEvent)
        {
            if (this.m_capable && GameSettings.EnableReplayKit)
            {
            }
        }

        private void OnRecord(CUIEvent uiEvent)
        {
            if (this.m_capable && GameSettings.EnableReplayKit)
            {
            }
        }

        private void SetReplayKitGlobalCfg()
        {
            if (GameDataMgr.svr2CltCfgDict != null)
            {
                if (GameDataMgr.svr2CltCfgDict.ContainsKey(6))
                {
                    ResGlobalInfo info = new ResGlobalInfo();
                    if (GameDataMgr.svr2CltCfgDict.TryGetValue(6, out info) && this.m_capable)
                    {
                        this.m_capable = info.dwConfValue > 0;
                    }
                }
                if (GameDataMgr.svr2CltCfgDict.ContainsKey(7))
                {
                    ResGlobalInfo info2 = new ResGlobalInfo();
                    if (GameDataMgr.svr2CltCfgDict.TryGetValue(7, out info2))
                    {
                        this.MIN_SPACE_LIMIT = info2.dwConfValue;
                    }
                }
                if (GameDataMgr.svr2CltCfgDict.ContainsKey(8))
                {
                    ResGlobalInfo info3 = new ResGlobalInfo();
                    if (GameDataMgr.svr2CltCfgDict.TryGetValue(8, out info3))
                    {
                        this.WARNING_SPACE_LIMIT = info3.dwConfValue;
                    }
                }
            }
        }

        public override void UnInit()
        {
            base.UnInit();
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReplayKit_Start_Recording, new CUIEventManager.OnUIEventHandler(this.OnRecord));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReplayKit_Pause_Recording, new CUIEventManager.OnUIEventHandler(this.OnPause));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReplayKit_Preview_Record, new CUIEventManager.OnUIEventHandler(this.OnPreview));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.ReplayKit_Discard_Record, new CUIEventManager.OnUIEventHandler(this.OnDiscard));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.GLOBAL_SERVER_TO_CLIENT_CFG_READY, new System.Action(this.SetReplayKitGlobalCfg));
        }

        public bool Cap
        {
            get
            {
                return this.m_capable;
            }
        }

        public enum Status
        {
            Recording,
            Paused,
            Transition
        }

        public enum StorageStatus
        {
            Disable = -1,
            Ok = 0,
            Warning = -2
        }
    }
}

