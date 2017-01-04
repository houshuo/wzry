namespace Assets.Scripts.UI
{
    using Assets.Scripts.Framework;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CUIStepListScript : CUIListScript
    {
        private bool m_bDontUpdate;
        private float m_contentExtendSize;
        private float m_contentFixScrollTargetPosition;
        private bool m_fixingScroll;
        private float m_fixScrollSpeed;
        public float m_fixScrollTime = 0.3f;
        public float m_minSpeedToFixScroll = 50f;
        [HideInInspector]
        public enUIEventID m_onStartDraggingEventID;
        public stUIEventParams m_onStartDraggingEventParams;
        public float m_reductionRate = 0.7f;
        private bool m_scrollRectIsDragged;
        private float m_scrollRectLastScrollSpeed;
        private float m_selectAreaMax;
        private float m_selectAreaMin;
        private float m_stepListCenter;

        private void DispatchOnStartDraggingEvent()
        {
            if (this.m_onStartDraggingEventID != enUIEventID.None)
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_eventID = this.m_onStartDraggingEventID;
                uIEvent.m_eventParams = this.m_onStartDraggingEventParams;
                uIEvent.m_srcFormScript = base.m_belongedFormScript;
                uIEvent.m_srcWidgetBelongedListScript = base.m_belongedListScript;
                uIEvent.m_srcWidgetIndexInBelongedList = base.m_indexInlist;
                uIEvent.m_srcWidget = base.gameObject;
                uIEvent.m_srcWidgetScript = this;
                uIEvent.m_pointerEventData = null;
                base.DispatchUIEvent(uIEvent);
            }
        }

        private float GetContentTargetPosition(int selectedIndex)
        {
            if ((selectedIndex < 0) || (selectedIndex >= base.m_elementAmount))
            {
                return 0f;
            }
            stRect rect = base.m_elementsRect[selectedIndex];
            float num = this.m_stepListCenter - rect.m_center.x;
            if (num > 0f)
            {
                return 0f;
            }
            if (num < (this.m_scrollAreaSize.x - this.m_contentSize.x))
            {
                num = this.m_scrollAreaSize.x - this.m_contentSize.x;
            }
            return num;
        }

        private float GetElementScale(int index)
        {
            stRect rect = base.m_elementsRect[index];
            int num = (int) (rect.m_center.x + base.m_contentRectTransform.anchoredPosition.x);
            int num2 = (int) Mathf.Abs((float) (num - this.m_stepListCenter));
            int num3 = num2 / ((int) this.m_elementDefaultSize.x);
            int num4 = num2 % ((int) this.m_elementDefaultSize.x);
            float num5 = Mathf.Pow(this.m_reductionRate, (float) num3);
            float num6 = Mathf.Pow(this.m_reductionRate, (float) (num3 + 1));
            return (num5 - ((num5 - num6) * (((float) num4) / this.m_elementDefaultSize.x)));
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.m_listType = enUIListType.Horizontal;
                base.m_elementSpacing = Vector2.zero;
                base.m_elementLayoutOffset = 0f;
                int elementAmount = base.m_elementAmount;
                base.m_elementAmount = 0;
                base.Initialize(formScript);
                CUIListElementScript component = base.m_elementTemplate.GetComponent<CUIListElementScript>();
                if (component != null)
                {
                    component.m_pivotType = enPivotType.Centre;
                }
                this.m_contentExtendSize = (this.m_scrollAreaSize.x - this.m_elementDefaultSize.x) * 0.5f;
                this.m_stepListCenter = this.m_contentExtendSize + (this.m_elementDefaultSize.x * 0.5f);
                this.m_selectAreaMin = this.m_contentExtendSize;
                this.m_selectAreaMax = this.m_scrollAreaSize.x - this.m_contentExtendSize;
                if (base.m_scrollRect != null)
                {
                    this.m_scrollRectLastScrollSpeed = base.m_scrollRect.velocity.x;
                }
                if (base.m_elementsRect == null)
                {
                    base.m_elementsRect = new List<stRect>();
                }
                base.SetElementAmount(elementAmount);
                if (base.m_elementAmount > 0)
                {
                    this.SelectElementImmediately(0);
                }
            }
        }

        private bool IsElementInSelectedArea(int index)
        {
            stRect rect = base.m_elementsRect[index];
            float num = rect.m_center.x + base.m_contentRectTransform.anchoredPosition.x;
            return ((num > this.m_selectAreaMin) && (num < this.m_selectAreaMax));
        }

        protected override void ProcessElements()
        {
            base.m_contentSize = Vector2.zero;
            Vector2 zero = Vector2.zero;
            this.m_contentSize.x += this.m_contentExtendSize;
            zero.x += this.m_contentExtendSize;
            for (int i = 0; i < base.m_elementAmount; i++)
            {
                stRect item = base.LayoutElement(i, ref this.m_contentSize, ref zero);
                if (i < base.m_elementsRect.Count)
                {
                    base.m_elementsRect[i] = item;
                }
                else
                {
                    base.m_elementsRect.Add(item);
                }
                if (!base.m_useOptimized || base.IsRectInScrollArea(ref item))
                {
                    CUIListElementScript script = base.CreateElement(i, ref item);
                }
            }
            this.m_contentSize.x += this.m_contentExtendSize;
            this.ResizeContent(ref this.m_contentSize, false);
        }

        protected override void ResizeContent(ref Vector2 size, bool resetPosition)
        {
            if (base.m_contentRectTransform != null)
            {
                base.m_contentRectTransform.sizeDelta = size;
                base.m_contentRectTransform.pivot = new Vector2(0f, 0.5f);
                base.m_contentRectTransform.anchorMin = new Vector2(0f, 0.5f);
                base.m_contentRectTransform.anchorMax = new Vector2(0f, 0.5f);
                base.m_contentRectTransform.anchoredPosition = Vector2.zero;
                if (resetPosition)
                {
                    base.m_contentRectTransform.anchoredPosition = Vector2.zero;
                }
            }
        }

        public override void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
        {
            base.SelectElement(index, true);
            if ((index >= 0) && (index < base.m_elementAmount))
            {
                this.m_contentFixScrollTargetPosition = this.GetContentTargetPosition(index);
                if (base.m_contentRectTransform.anchoredPosition.x != this.m_contentFixScrollTargetPosition)
                {
                    if (base.m_scrollRect != null)
                    {
                        base.m_scrollRect.StopMovement();
                    }
                    this.m_fixScrollSpeed = (this.m_contentFixScrollTargetPosition - base.m_contentRectTransform.anchoredPosition.x) / (this.m_fixScrollTime * GameFramework.c_renderFPS);
                    if (Mathf.Abs(this.m_fixScrollSpeed) < 0.001f)
                    {
                        base.m_contentRectTransform.anchoredPosition = new Vector2(this.m_contentFixScrollTargetPosition, base.m_contentRectTransform.anchoredPosition.y);
                    }
                    else
                    {
                        this.m_fixingScroll = true;
                    }
                    this.m_scrollRectIsDragged = false;
                }
            }
        }

        public void SelectElementImmediately(int index)
        {
            base.SelectElement(index, true);
            if ((index >= 0) && (index < base.m_elementAmount))
            {
                this.m_contentFixScrollTargetPosition = this.GetContentTargetPosition(index);
                if (base.m_contentRectTransform.anchoredPosition.x != this.m_contentFixScrollTargetPosition)
                {
                    base.m_contentRectTransform.anchoredPosition = new Vector2(this.m_contentFixScrollTargetPosition, base.m_contentRectTransform.anchoredPosition.y);
                }
                this.m_scrollRectIsDragged = false;
                this.m_fixingScroll = false;
            }
        }

        public void SetDontUpdate(bool bDontUpdate)
        {
            this.m_bDontUpdate = bDontUpdate;
        }

        protected override void Update()
        {
            if ((base.m_belongedFormScript == null) || !base.m_belongedFormScript.IsClosed())
            {
                if (base.m_useOptimized)
                {
                    base.UpdateElementsScroll();
                }
                if (!this.m_bDontUpdate && (base.m_scrollRect != null))
                {
                    for (int i = 0; i < base.m_elementAmount; i++)
                    {
                        CUIListElementScript elemenet = base.GetElemenet(i);
                        if (elemenet != null)
                        {
                            elemenet.gameObject.transform.localScale = (Vector3) (Vector3.one * this.GetElementScale(i));
                        }
                    }
                    if ((this.m_fixingScroll && (base.m_selectedElementIndex >= 0)) && (base.m_selectedElementIndex < base.m_elementAmount))
                    {
                        base.m_scrollRect.enabled = false;
                        float x = base.m_contentRectTransform.anchoredPosition.x + this.m_fixScrollSpeed;
                        if (((x > this.m_contentFixScrollTargetPosition) && (this.m_fixScrollSpeed > 0f)) || ((x < this.m_contentFixScrollTargetPosition) && (this.m_fixScrollSpeed < 0f)))
                        {
                            base.m_contentRectTransform.anchoredPosition = new Vector2(this.m_contentFixScrollTargetPosition, base.m_contentRectTransform.anchoredPosition.y);
                            this.m_fixScrollSpeed = 0f;
                            this.m_fixingScroll = false;
                            base.m_scrollRect.StopMovement();
                        }
                        else
                        {
                            base.m_contentRectTransform.anchoredPosition = new Vector2(x, base.m_contentRectTransform.anchoredPosition.y);
                        }
                    }
                    else
                    {
                        base.m_scrollRect.enabled = true;
                        if ((bool) base.m_scrollRect.GetType().GetField("m_Dragging", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(base.m_scrollRect))
                        {
                            if (!this.m_scrollRectIsDragged)
                            {
                                this.DispatchOnStartDraggingEvent();
                            }
                            this.m_scrollRectIsDragged = true;
                        }
                        else if (!this.m_scrollRectIsDragged)
                        {
                            if (((base.m_selectedElementIndex >= 0) && (base.m_selectedElementIndex < base.m_elementAmount)) && (base.m_contentRectTransform.anchoredPosition.x != this.m_contentFixScrollTargetPosition))
                            {
                                base.m_contentRectTransform.anchoredPosition = new Vector2(this.m_contentFixScrollTargetPosition, base.m_contentRectTransform.anchoredPosition.y);
                                for (int j = 0; j < base.m_elementAmount; j++)
                                {
                                    CUIListElementScript script2 = base.GetElemenet(j);
                                    if (script2 != null)
                                    {
                                        script2.gameObject.transform.localScale = (Vector3) (Vector3.one * this.GetElementScale(j));
                                    }
                                }
                            }
                        }
                        else
                        {
                            float num3 = Mathf.Abs(base.m_scrollRect.velocity.x);
                            int index = -1;
                            if (base.m_contentRectTransform.anchoredPosition.x > 0f)
                            {
                                index = 0;
                            }
                            else if ((base.m_contentRectTransform.anchoredPosition.x + this.m_contentSize.x) < this.m_scrollAreaSize.x)
                            {
                                index = base.m_elementAmount - 1;
                            }
                            else if ((num3 <= this.m_scrollRectLastScrollSpeed) && (num3 < this.m_minSpeedToFixScroll))
                            {
                                for (int k = 0; k < base.m_elementAmount; k++)
                                {
                                    if ((index < 0) && this.IsElementInSelectedArea(k))
                                    {
                                        index = k;
                                        break;
                                    }
                                }
                            }
                            if (index >= 0)
                            {
                                this.SelectElement(index, true);
                                this.m_scrollRectIsDragged = false;
                            }
                        }
                        this.m_scrollRectLastScrollSpeed = Mathf.Abs(base.m_scrollRect.velocity.x);
                    }
                    base.DetectScroll();
                }
            }
        }
    }
}

