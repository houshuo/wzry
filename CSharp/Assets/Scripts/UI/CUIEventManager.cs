namespace Assets.Scripts.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class CUIEventManager : Singleton<CUIEventManager>
    {
        [CompilerGenerated]
        private static OnUIEventHandler <>f__am$cache2;
        private OnUIEventHandler[] m_uiEventHandlerMap = new OnUIEventHandler[0x3716];
        private List<object> m_uiEvents = new List<object>();

        public void AddUIEventListener(enUIEventID eventID, OnUIEventHandler onUIEventHandler)
        {
            if (this.m_uiEventHandlerMap[(int) ((IntPtr) eventID)] == null)
            {
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = delegate {
                    };
                }
                this.m_uiEventHandlerMap[(int) ((IntPtr) eventID)] = <>f__am$cache2;
                this.m_uiEventHandlerMap[(int) ((IntPtr) eventID)] = (OnUIEventHandler) Delegate.Combine(this.m_uiEventHandlerMap[(int) ((IntPtr) eventID)], onUIEventHandler);
            }
            else
            {
                this.m_uiEventHandlerMap[(int) ((IntPtr) eventID)] = (OnUIEventHandler) Delegate.Remove(this.m_uiEventHandlerMap[(int) ((IntPtr) eventID)], onUIEventHandler);
                this.m_uiEventHandlerMap[(int) ((IntPtr) eventID)] = (OnUIEventHandler) Delegate.Combine(this.m_uiEventHandlerMap[(int) ((IntPtr) eventID)], onUIEventHandler);
            }
        }

        public void DispatchUIEvent(CUIEvent uiEvent)
        {
            uiEvent.m_inUse = true;
            OnUIEventHandler handler = this.m_uiEventHandlerMap[(int) ((IntPtr) uiEvent.m_eventID)];
            if (handler != null)
            {
                handler(uiEvent);
            }
            uiEvent.Clear();
        }

        public void DispatchUIEvent(enUIEventID eventID)
        {
            CUIEvent uIEvent = this.GetUIEvent();
            uIEvent.m_eventID = eventID;
            this.DispatchUIEvent(uIEvent);
        }

        public void DispatchUIEvent(enUIEventID eventID, stUIEventParams par)
        {
            CUIEvent uIEvent = this.GetUIEvent();
            uIEvent.m_eventID = eventID;
            uIEvent.m_eventParams = par;
            this.DispatchUIEvent(uIEvent);
        }

        public CUIEvent GetUIEvent()
        {
            for (int i = 0; i < this.m_uiEvents.Count; i++)
            {
                CUIEvent event2 = (CUIEvent) this.m_uiEvents[i];
                if (!event2.m_inUse)
                {
                    return event2;
                }
            }
            CUIEvent item = new CUIEvent();
            this.m_uiEvents.Add(item);
            return item;
        }

        public void RemoveUIEventListener(enUIEventID eventID, OnUIEventHandler onUIEventHandler)
        {
            if (this.m_uiEventHandlerMap[(int) ((IntPtr) eventID)] != null)
            {
                this.m_uiEventHandlerMap[(int) ((IntPtr) eventID)] = (OnUIEventHandler) Delegate.Remove(this.m_uiEventHandlerMap[(int) ((IntPtr) eventID)], onUIEventHandler);
            }
        }

        public delegate void OnUIEventHandler(CUIEvent uiEvent);
    }
}

