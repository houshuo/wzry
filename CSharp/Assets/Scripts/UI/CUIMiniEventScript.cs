namespace Assets.Scripts.UI
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class CUIMiniEventScript : CUIComponent, IPointerDownHandler, IEventSystemHandler, IPointerClickHandler, IPointerUpHandler
    {
        public bool m_closeFormWhenClicked;
        public string[] m_onClickedWwiseEvents = new string[0];
        [HideInInspector]
        public enUIEventID m_onClickEventID;
        [NonSerialized]
        public stUIEventParams m_onClickEventParams;
        [HideInInspector]
        public enUIEventID m_onDownEventID;
        [NonSerialized]
        public stUIEventParams m_onDownEventParams;
        public string[] m_onDownWwiseEvents = new string[0];
        [HideInInspector]
        public enUIEventID m_onUpEventID;
        [NonSerialized]
        public stUIEventParams m_onUpEventParams;
        public OnUIEventHandler onClick;

        private void DispatchUIEvent(enUIEventType eventType, PointerEventData pointerEventData)
        {
            CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
            switch (eventType)
            {
                case enUIEventType.Down:
                    this.PostWwiseEvent(this.m_onDownWwiseEvents);
                    if (this.m_onDownEventID == enUIEventID.None)
                    {
                        return;
                    }
                    uIEvent.m_eventID = this.m_onDownEventID;
                    uIEvent.m_eventParams = this.m_onDownEventParams;
                    break;

                case enUIEventType.Click:
                    this.PostWwiseEvent(this.m_onClickedWwiseEvents);
                    if (this.m_onClickEventID == enUIEventID.None)
                    {
                        if (this.onClick != null)
                        {
                            uIEvent.m_eventID = enUIEventID.None;
                            uIEvent.m_eventParams = this.m_onClickEventParams;
                            this.onClick(uIEvent);
                        }
                        return;
                    }
                    uIEvent.m_eventID = this.m_onClickEventID;
                    uIEvent.m_eventParams = this.m_onClickEventParams;
                    break;

                case enUIEventType.Up:
                    if (this.m_onUpEventID == enUIEventID.None)
                    {
                        return;
                    }
                    uIEvent.m_eventID = this.m_onUpEventID;
                    uIEvent.m_eventParams = this.m_onUpEventParams;
                    break;
            }
            uIEvent.m_srcFormScript = base.m_belongedFormScript;
            uIEvent.m_srcWidgetBelongedListScript = base.m_belongedListScript;
            uIEvent.m_srcWidgetIndexInBelongedList = base.m_indexInlist;
            uIEvent.m_srcWidget = base.gameObject;
            uIEvent.m_srcWidgetScript = this;
            uIEvent.m_pointerEventData = pointerEventData;
            if ((eventType == enUIEventType.Click) && (this.onClick != null))
            {
                this.onClick(uIEvent);
            }
            base.DispatchUIEvent(uIEvent);
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            bool flag = true;
            if ((base.m_belongedFormScript != null) && !base.m_belongedFormScript.m_enableMultiClickedEvent)
            {
                flag = base.m_belongedFormScript.m_clickedEventDispatchedCounter == 0;
                base.m_belongedFormScript.m_clickedEventDispatchedCounter++;
            }
            if (flag)
            {
                if ((base.m_belongedListScript != null) && (base.m_indexInlist >= 0))
                {
                    base.m_belongedListScript.SelectElement(base.m_indexInlist, true);
                }
                this.DispatchUIEvent(enUIEventType.Click, eventData);
                if (this.m_closeFormWhenClicked && (base.m_belongedFormScript != null))
                {
                    base.m_belongedFormScript.Close();
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            this.DispatchUIEvent(enUIEventType.Down, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            this.DispatchUIEvent(enUIEventType.Up, eventData);
        }

        private void PostWwiseEvent(string[] wwiseEvents)
        {
            for (int i = 0; i < wwiseEvents.Length; i++)
            {
                if (!string.IsNullOrEmpty(wwiseEvents[i]))
                {
                    Singleton<CSoundManager>.GetInstance().PostEvent(wwiseEvents[i], null);
                }
            }
        }

        public void SetUIEvent(enUIEventType eventType, enUIEventID eventID)
        {
            switch (eventType)
            {
                case enUIEventType.Down:
                    this.m_onDownEventID = eventID;
                    break;

                case enUIEventType.Click:
                    this.m_onClickEventID = eventID;
                    break;

                case enUIEventType.Up:
                    this.m_onUpEventID = eventID;
                    break;
            }
        }

        public void SetUIEvent(enUIEventType eventType, enUIEventID eventID, stUIEventParams eventParams)
        {
            switch (eventType)
            {
                case enUIEventType.Down:
                    this.m_onDownEventID = eventID;
                    this.m_onDownEventParams = eventParams;
                    break;

                case enUIEventType.Click:
                    this.m_onClickEventID = eventID;
                    this.m_onClickEventParams = eventParams;
                    break;

                case enUIEventType.Up:
                    this.m_onUpEventID = eventID;
                    this.m_onUpEventParams = eventParams;
                    break;
            }
        }

        private void Update()
        {
        }

        public delegate void OnUIEventHandler(CUIEvent uiEvent);
    }
}

