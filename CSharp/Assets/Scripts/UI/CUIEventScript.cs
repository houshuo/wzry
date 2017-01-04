namespace Assets.Scripts.UI
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class CUIEventScript : CUIComponent, IPointerDownHandler, IEventSystemHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerUpHandler
    {
        private const float c_clickAreaValue = 40f;
        private const float c_holdTimeValue = 1f;
        private bool m_canClick;
        [HideInInspector]
        public bool m_closeFormWhenClicked;
        private Vector2 m_downPosition;
        private float m_downTimestamp;
        private PointerEventData m_holdPointerEventData;
        private bool m_isDown;
        private bool m_isHold;
        private bool m_needClearInputStatus;
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
        public enUIEventID m_onDragEndEventID;
        [NonSerialized]
        public stUIEventParams m_onDragEndEventParams;
        [HideInInspector]
        public enUIEventID m_onDragEventID;
        [NonSerialized]
        public stUIEventParams m_onDragEventParams;
        [HideInInspector]
        public enUIEventID m_onDragStartEventID;
        [NonSerialized]
        public stUIEventParams m_onDragStartEventParams;
        [HideInInspector]
        public enUIEventID m_onDropEventID;
        [NonSerialized]
        public stUIEventParams m_onDropEventParams;
        [HideInInspector]
        public enUIEventID m_onHoldEndEventID;
        [NonSerialized]
        public stUIEventParams m_onHoldEndEventParams;
        [HideInInspector]
        public enUIEventID m_onHoldEventID;
        [NonSerialized]
        public stUIEventParams m_onHoldEventParams;
        [HideInInspector]
        public enUIEventID m_onHoldStartEventID;
        [NonSerialized]
        public stUIEventParams m_onHoldStartEventParams;
        [HideInInspector]
        public enUIEventID m_onUpEventID;
        [NonSerialized]
        public stUIEventParams m_onUpEventParams;
        public OnUIEventHandler onClick;

        public bool ClearInputStatus()
        {
            this.m_needClearInputStatus = true;
            return this.m_isDown;
        }

        private void DispatchUIEvent(enUIEventType eventType, PointerEventData pointerEventData)
        {
            CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
            switch (eventType)
            {
                case enUIEventType.Down:
                    if (this.m_onDownEventID != enUIEventID.None)
                    {
                        uIEvent.m_eventID = this.m_onDownEventID;
                        uIEvent.m_eventParams = this.m_onDownEventParams;
                        break;
                    }
                    return;

                case enUIEventType.Click:
                    this.PostWwiseEvent(this.m_onDownWwiseEvents);
                    this.PostWwiseEvent(this.m_onClickedWwiseEvents);
                    if (this.m_onClickEventID != enUIEventID.None)
                    {
                        uIEvent.m_eventID = this.m_onClickEventID;
                        uIEvent.m_eventParams = this.m_onClickEventParams;
                        break;
                    }
                    if (this.onClick != null)
                    {
                        uIEvent.m_eventID = enUIEventID.None;
                        uIEvent.m_eventParams = this.m_onClickEventParams;
                        uIEvent.m_srcWidget = base.gameObject;
                        this.onClick(uIEvent);
                    }
                    return;

                case enUIEventType.HoldStart:
                    if (this.m_onHoldStartEventID != enUIEventID.None)
                    {
                        uIEvent.m_eventID = this.m_onHoldStartEventID;
                        uIEvent.m_eventParams = this.m_onHoldStartEventParams;
                        break;
                    }
                    return;

                case enUIEventType.Hold:
                    if (this.m_onHoldEventID != enUIEventID.None)
                    {
                        uIEvent.m_eventID = this.m_onHoldEventID;
                        uIEvent.m_eventParams = this.m_onHoldEventParams;
                        break;
                    }
                    return;

                case enUIEventType.HoldEnd:
                    if (this.m_onHoldEndEventID != enUIEventID.None)
                    {
                        uIEvent.m_eventID = this.m_onHoldEndEventID;
                        uIEvent.m_eventParams = this.m_onHoldEndEventParams;
                        break;
                    }
                    return;

                case enUIEventType.DragStart:
                    if (this.m_onDragStartEventID != enUIEventID.None)
                    {
                        uIEvent.m_eventID = this.m_onDragStartEventID;
                        uIEvent.m_eventParams = this.m_onDragStartEventParams;
                        break;
                    }
                    return;

                case enUIEventType.Drag:
                    if (this.m_onDragEventID != enUIEventID.None)
                    {
                        uIEvent.m_eventID = this.m_onDragEventID;
                        uIEvent.m_eventParams = this.m_onDragEventParams;
                        break;
                    }
                    return;

                case enUIEventType.DragEnd:
                    if (this.m_onDragEndEventID != enUIEventID.None)
                    {
                        uIEvent.m_eventID = this.m_onDragEndEventID;
                        uIEvent.m_eventParams = this.m_onDragEndEventParams;
                        break;
                    }
                    return;

                case enUIEventType.Drop:
                    if (this.m_onDropEventID != enUIEventID.None)
                    {
                        uIEvent.m_eventID = this.m_onDropEventID;
                        uIEvent.m_eventParams = this.m_onDropEventParams;
                        break;
                    }
                    return;

                case enUIEventType.Up:
                    if (this.m_onUpEventID != enUIEventID.None)
                    {
                        uIEvent.m_eventID = this.m_onUpEventID;
                        uIEvent.m_eventParams = this.m_onUpEventParams;
                        break;
                    }
                    return;
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

        public void ExecuteInputStatus()
        {
            this.m_isDown = false;
            this.m_isHold = false;
            this.m_canClick = false;
            this.m_downTimestamp = 0f;
            this.m_downPosition = Vector2.zero;
            this.m_holdPointerEventData = null;
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
            }
        }

        private void LateUpdate()
        {
            if (this.m_needClearInputStatus)
            {
                this.ExecuteInputStatus();
                this.m_needClearInputStatus = false;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if ((this.m_canClick && (base.m_belongedFormScript != null)) && (base.m_belongedFormScript.ChangeScreenValueToForm(Vector2.Distance(eventData.position, this.m_downPosition)) > 40f))
            {
                this.m_canClick = false;
            }
            this.DispatchUIEvent(enUIEventType.DragStart, eventData);
            if ((base.m_belongedListScript != null) && (base.m_belongedListScript.m_scrollRect != null))
            {
                base.m_belongedListScript.m_scrollRect.OnBeginDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if ((this.m_canClick && (base.m_belongedFormScript != null)) && (base.m_belongedFormScript.ChangeScreenValueToForm(Vector2.Distance(eventData.position, this.m_downPosition)) > 40f))
            {
                this.m_canClick = false;
            }
            this.DispatchUIEvent(enUIEventType.Drag, eventData);
            if ((base.m_belongedListScript != null) && (base.m_belongedListScript.m_scrollRect != null))
            {
                base.m_belongedListScript.m_scrollRect.OnDrag(eventData);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            this.DispatchUIEvent(enUIEventType.Drop, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if ((this.m_canClick && (base.m_belongedFormScript != null)) && (base.m_belongedFormScript.ChangeScreenValueToForm(Vector2.Distance(eventData.position, this.m_downPosition)) > 40f))
            {
                this.m_canClick = false;
            }
            this.DispatchUIEvent(enUIEventType.DragEnd, eventData);
            if ((base.m_belongedListScript != null) && (base.m_belongedListScript.m_scrollRect != null))
            {
                base.m_belongedListScript.m_scrollRect.OnEndDrag(eventData);
            }
            this.ClearInputStatus();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            bool flag = true;
            if ((base.m_belongedFormScript != null) && !base.m_belongedFormScript.m_enableMultiClickedEvent)
            {
                flag = base.m_belongedFormScript.m_clickedEventDispatchedCounter == 0;
                base.m_belongedFormScript.m_clickedEventDispatchedCounter++;
            }
            if (this.m_canClick && flag)
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
            this.ClearInputStatus();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            this.m_isDown = true;
            this.m_isHold = false;
            this.m_canClick = true;
            this.m_downTimestamp = Time.realtimeSinceStartup;
            this.m_downPosition = eventData.position;
            this.m_holdPointerEventData = eventData;
            this.m_needClearInputStatus = false;
            this.DispatchUIEvent(enUIEventType.Down, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (this.m_isHold && (this.m_holdPointerEventData != null))
            {
                this.DispatchUIEvent(enUIEventType.HoldEnd, this.m_holdPointerEventData);
            }
            this.DispatchUIEvent(enUIEventType.Up, eventData);
            this.ClearInputStatus();
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

                case enUIEventType.HoldStart:
                    this.m_onHoldStartEventID = eventID;
                    break;

                case enUIEventType.Hold:
                    this.m_onHoldEventID = eventID;
                    break;

                case enUIEventType.HoldEnd:
                    this.m_onHoldEndEventID = eventID;
                    break;

                case enUIEventType.DragStart:
                    this.m_onDragStartEventID = eventID;
                    break;

                case enUIEventType.Drag:
                    this.m_onDragEventID = eventID;
                    break;

                case enUIEventType.DragEnd:
                    this.m_onDragEndEventID = eventID;
                    break;

                case enUIEventType.Drop:
                    this.m_onDropEventID = eventID;
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

                case enUIEventType.HoldStart:
                    this.m_onHoldStartEventID = eventID;
                    this.m_onHoldStartEventParams = eventParams;
                    break;

                case enUIEventType.Hold:
                    this.m_onHoldEventID = eventID;
                    this.m_onHoldEventParams = eventParams;
                    break;

                case enUIEventType.HoldEnd:
                    this.m_onHoldEndEventID = eventID;
                    this.m_onHoldEndEventParams = eventParams;
                    break;

                case enUIEventType.DragStart:
                    this.m_onDragStartEventID = eventID;
                    this.m_onDragStartEventParams = eventParams;
                    break;

                case enUIEventType.Drag:
                    this.m_onDragEventID = eventID;
                    this.m_onDragEventParams = eventParams;
                    break;

                case enUIEventType.DragEnd:
                    this.m_onDragEndEventID = eventID;
                    this.m_onDragEndEventParams = eventParams;
                    break;

                case enUIEventType.Drop:
                    this.m_onDropEventID = eventID;
                    this.m_onDropEventParams = eventParams;
                    break;

                case enUIEventType.Up:
                    this.m_onUpEventID = eventID;
                    this.m_onUpEventParams = eventParams;
                    break;
            }
        }

        private void Update()
        {
            if (!this.m_needClearInputStatus && this.m_isDown)
            {
                if (!this.m_isHold)
                {
                    if ((Time.realtimeSinceStartup - this.m_downTimestamp) >= 1f)
                    {
                        this.m_isHold = true;
                        this.m_canClick = false;
                        this.DispatchUIEvent(enUIEventType.HoldStart, this.m_holdPointerEventData);
                    }
                }
                else
                {
                    this.DispatchUIEvent(enUIEventType.Hold, this.m_holdPointerEventData);
                }
            }
        }

        public delegate void OnUIEventHandler(CUIEvent uiEvent);
    }
}

