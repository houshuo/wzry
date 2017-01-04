namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class CUIEvent
    {
        public enUIEventID m_eventID;
        public stUIEventParams m_eventParams;
        public bool m_inUse;
        public PointerEventData m_pointerEventData;
        public CUIFormScript m_srcFormScript;
        public GameObject m_srcWidget;
        public CUIListScript m_srcWidgetBelongedListScript;
        public int m_srcWidgetIndexInBelongedList;
        public CUIComponent m_srcWidgetScript;

        public void Clear()
        {
            this.m_srcFormScript = null;
            this.m_srcWidget = null;
            this.m_srcWidgetScript = null;
            this.m_srcWidgetBelongedListScript = null;
            this.m_srcWidgetIndexInBelongedList = -1;
            this.m_pointerEventData = null;
            this.m_eventID = enUIEventID.None;
            this.m_inUse = false;
        }
    }
}

