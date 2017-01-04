namespace Assets.Scripts.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CUIExpandListScript : CUIListScript
    {
        private Vector2 m_contentAnchoredPosition;
        public float m_contentFixingSpeed = 1200f;
        private Vector2 m_elementExpandedSize;
        public float m_expandedTime = 0.15f;
        private enExpandListSelectingState m_selectingState;
        private Vector2 m_targetContentAnchoredPosition;
        private float m_timeSlice;

        private Vector2 GetTargetContentAnchoredPosition(int selectedElementIndex)
        {
            if ((selectedElementIndex < 0) || (selectedElementIndex >= base.m_elementAmount))
            {
                return this.m_contentAnchoredPosition;
            }
            stRect rect = base.m_elementsRect[base.m_selectedElementIndex];
            rect.m_width = (int) this.m_elementExpandedSize.x;
            rect.m_height = (int) this.m_elementExpandedSize.y;
            rect.m_right = rect.m_left + rect.m_width;
            rect.m_bottom = rect.m_top - rect.m_height;
            Vector2 contentAnchoredPosition = this.m_contentAnchoredPosition;
            if (base.m_listType == enUIListType.Horizontal)
            {
                if ((contentAnchoredPosition.x + rect.m_right) > this.m_scrollAreaSize.x)
                {
                    contentAnchoredPosition.x = this.m_scrollAreaSize.x - rect.m_right;
                }
                if ((contentAnchoredPosition.x + rect.m_left) < 0f)
                {
                    contentAnchoredPosition.x = -rect.m_left;
                }
                return contentAnchoredPosition;
            }
            if (base.m_listType == enUIListType.Vertical)
            {
                if ((contentAnchoredPosition.y + rect.m_bottom) < -this.m_scrollAreaSize.y)
                {
                    contentAnchoredPosition.y = -this.m_scrollAreaSize.y - rect.m_bottom;
                }
                if ((contentAnchoredPosition.y + rect.m_top) > 0f)
                {
                    contentAnchoredPosition.y = -rect.m_top;
                }
            }
            return contentAnchoredPosition;
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                if (base.m_listType == enUIListType.VerticalGrid)
                {
                    base.m_listType = enUIListType.Vertical;
                }
                else if (base.m_listType == enUIListType.HorizontalGrid)
                {
                    base.m_listType = enUIListType.Horizontal;
                }
                if (base.m_elementsRect == null)
                {
                    base.m_elementsRect = new List<stRect>();
                }
                base.Initialize(formScript);
                if (base.m_elementTemplate != null)
                {
                    CUIExpandListElementScript component = base.m_elementTemplate.GetComponent<CUIExpandListElementScript>();
                    if (component != null)
                    {
                        this.m_elementExpandedSize = component.m_expandedSize;
                    }
                }
            }
        }

        private stRect LayoutExpandElement(int index, float expandedRate, ref Vector2 contentSize, ref Vector2 offset)
        {
            stRect rect = new stRect();
            if (base.m_listType == enUIListType.Horizontal)
            {
                rect.m_width = (int) (this.m_elementDefaultSize.x + ((this.m_elementExpandedSize.x - this.m_elementDefaultSize.x) * expandedRate));
                rect.m_height = (int) this.m_elementDefaultSize.y;
            }
            else
            {
                rect.m_width = (int) this.m_elementDefaultSize.x;
                rect.m_height = (int) (this.m_elementDefaultSize.y + ((this.m_elementExpandedSize.y - this.m_elementDefaultSize.y) * expandedRate));
            }
            rect.m_left = (int) offset.x;
            rect.m_top = (int) offset.y;
            rect.m_right = rect.m_left + rect.m_width;
            rect.m_bottom = rect.m_top - rect.m_height;
            rect.m_center = new Vector2(rect.m_left + (rect.m_width * 0.5f), rect.m_top - (rect.m_height * 0.5f));
            if (rect.m_right > contentSize.x)
            {
                contentSize.x = rect.m_right;
            }
            if (-rect.m_bottom > contentSize.y)
            {
                contentSize.y = -rect.m_bottom;
            }
            if (base.m_listType == enUIListType.Horizontal)
            {
                offset.x += rect.m_width + this.m_elementSpacing.x;
                return rect;
            }
            if (base.m_listType == enUIListType.Vertical)
            {
                offset.y -= rect.m_height + this.m_elementSpacing.y;
            }
            return rect;
        }

        protected override void ProcessElements()
        {
            base.m_contentSize = Vector2.zero;
            Vector2 zero = Vector2.zero;
            if (base.m_listType == enUIListType.Horizontal)
            {
                zero.x += base.m_elementLayoutOffset;
            }
            else if (base.m_listType == enUIListType.Vertical)
            {
                zero.y += base.m_elementLayoutOffset;
            }
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
                    base.CreateElement(i, ref item);
                }
            }
            this.ResizeContent(ref this.m_contentSize, false);
        }

        public override void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
        {
            if (this.m_selectingState == enExpandListSelectingState.None)
            {
                base.m_lastSelectedElementIndex = base.m_selectedElementIndex;
                base.m_selectedElementIndex = index;
                if (base.m_lastSelectedElementIndex == base.m_selectedElementIndex)
                {
                    base.m_selectedElementIndex = -1;
                }
                if (base.m_lastSelectedElementIndex >= 0)
                {
                    CUIListElementScript elemenet = base.GetElemenet(base.m_lastSelectedElementIndex);
                    if (elemenet != null)
                    {
                        elemenet.ChangeDisplay(false);
                    }
                }
                if (base.m_selectedElementIndex >= 0)
                {
                    CUIListElementScript script2 = base.GetElemenet(base.m_selectedElementIndex);
                    if (script2 != null)
                    {
                        script2.ChangeDisplay(true);
                        if (script2.onSelected != null)
                        {
                            script2.onSelected();
                        }
                    }
                }
                base.DispatchElementSelectChangedEvent();
                this.m_contentAnchoredPosition = base.m_contentRectTransform.anchoredPosition;
                this.m_timeSlice = 0f;
                if (base.m_lastSelectedElementIndex >= 0)
                {
                    this.m_selectingState = enExpandListSelectingState.Retract;
                }
                else if (base.m_selectedElementIndex >= 0)
                {
                    this.m_targetContentAnchoredPosition = this.GetTargetContentAnchoredPosition(base.m_selectedElementIndex);
                    this.m_selectingState = enExpandListSelectingState.Move;
                    this.m_timeSlice = 0f;
                }
            }
        }

        public void SelectElementImmediately(int index)
        {
            base.SelectElement(index, true);
            base.m_contentSize = Vector2.zero;
            Vector2 zero = Vector2.zero;
            if (base.m_listType == enUIListType.Horizontal)
            {
                zero.x += base.m_elementLayoutOffset;
            }
            else if (base.m_listType == enUIListType.Vertical)
            {
                zero.y += base.m_elementLayoutOffset;
            }
            for (int i = 0; i < base.m_elementAmount; i++)
            {
                stRect item = this.LayoutExpandElement(i, (i != index) ? ((float) 0) : ((float) 1), ref this.m_contentSize, ref zero);
                if (i < base.m_elementsRect.Count)
                {
                    base.m_elementsRect[i] = item;
                }
                else
                {
                    base.m_elementsRect.Add(item);
                }
            }
            this.ResizeContent(ref this.m_contentSize, false);
            if ((index < 0) || (index >= base.m_elementAmount))
            {
                base.m_contentRectTransform.anchoredPosition = Vector2.zero;
            }
            else
            {
                base.m_contentRectTransform.anchoredPosition = this.GetTargetContentAnchoredPosition(index);
            }
            for (int j = 0; j < base.m_elementAmount; j++)
            {
                stRect rect = base.m_elementsRect[j];
                CUIListElementScript elemenet = base.GetElemenet(j);
                if (elemenet != null)
                {
                    elemenet.SetRect(ref rect);
                }
                else if (!base.m_useOptimized || base.IsRectInScrollArea(ref rect))
                {
                    base.CreateElement(j, ref rect);
                }
            }
        }

        protected override void Update()
        {
            if ((base.m_belongedFormScript == null) || !base.m_belongedFormScript.IsClosed())
            {
                if (this.m_selectingState != enExpandListSelectingState.None)
                {
                    if (base.m_scrollRect.enabled)
                    {
                        base.m_scrollRect.StopMovement();
                        base.m_scrollRect.enabled = false;
                    }
                    this.UpdateSelectedElement(this.m_selectingState);
                }
                else if (!base.m_scrollRect.enabled)
                {
                    base.m_scrollRect.StopMovement();
                    base.m_scrollRect.enabled = true;
                }
                if (base.m_useOptimized)
                {
                    base.UpdateElementsScroll();
                }
            }
        }

        private void UpdateSelectedElement(enExpandListSelectingState selectingState)
        {
            Vector2 zero;
            int num7;
            switch (selectingState)
            {
                case enExpandListSelectingState.Retract:
                    if (this.m_timeSlice >= this.m_expandedTime)
                    {
                        if ((base.m_selectedElementIndex >= 0) && (base.m_selectedElementIndex < base.m_elementAmount))
                        {
                            this.m_targetContentAnchoredPosition = this.GetTargetContentAnchoredPosition(base.m_selectedElementIndex);
                            this.m_selectingState = enExpandListSelectingState.Move;
                            this.m_timeSlice = 0f;
                        }
                        else
                        {
                            this.m_selectingState = enExpandListSelectingState.None;
                        }
                        goto Label_055A;
                    }
                    this.m_timeSlice += Time.deltaTime;
                    base.m_contentSize = Vector2.zero;
                    zero = Vector2.zero;
                    if (base.m_listType != enUIListType.Horizontal)
                    {
                        if (base.m_listType == enUIListType.Vertical)
                        {
                            zero.y += base.m_elementLayoutOffset;
                        }
                        break;
                    }
                    zero.x += base.m_elementLayoutOffset;
                    break;

                case enExpandListSelectingState.Move:
                    if (!(this.m_contentAnchoredPosition != this.m_targetContentAnchoredPosition))
                    {
                        this.m_selectingState = enExpandListSelectingState.Expand;
                        this.m_timeSlice = 0f;
                    }
                    else if (base.m_listType != enUIListType.Horizontal)
                    {
                        if (base.m_listType == enUIListType.Vertical)
                        {
                            int num4 = (this.m_targetContentAnchoredPosition.y <= this.m_contentAnchoredPosition.y) ? -1 : 1;
                            this.m_contentAnchoredPosition.y += (Time.deltaTime * this.m_contentFixingSpeed) * num4;
                            if (((num4 > 0) && (this.m_contentAnchoredPosition.y >= this.m_targetContentAnchoredPosition.y)) || ((num4 < 0) && (this.m_contentAnchoredPosition.y <= this.m_targetContentAnchoredPosition.y)))
                            {
                                this.m_contentAnchoredPosition = this.m_targetContentAnchoredPosition;
                            }
                        }
                    }
                    else
                    {
                        int num3 = (this.m_targetContentAnchoredPosition.x <= this.m_contentAnchoredPosition.x) ? -1 : 1;
                        this.m_contentAnchoredPosition.x += (Time.deltaTime * this.m_contentFixingSpeed) * num3;
                        if (((num3 > 0) && (this.m_contentAnchoredPosition.x >= this.m_targetContentAnchoredPosition.x)) || ((num3 < 0) && (this.m_contentAnchoredPosition.x <= this.m_targetContentAnchoredPosition.x)))
                        {
                            this.m_contentAnchoredPosition = this.m_targetContentAnchoredPosition;
                        }
                    }
                    goto Label_055A;

                case enExpandListSelectingState.Expand:
                    if (this.m_timeSlice >= this.m_expandedTime)
                    {
                        this.m_selectingState = enExpandListSelectingState.None;
                    }
                    else
                    {
                        this.m_timeSlice += Time.deltaTime;
                        base.m_contentSize = Vector2.zero;
                        Vector2 offset = Vector2.zero;
                        if (base.m_listType != enUIListType.Horizontal)
                        {
                            if (base.m_listType == enUIListType.Vertical)
                            {
                                offset.y += base.m_elementLayoutOffset;
                            }
                        }
                        else
                        {
                            offset.x += base.m_elementLayoutOffset;
                        }
                        for (int j = 0; j < base.m_elementAmount; j++)
                        {
                            float num6 = 0f;
                            if (j == base.m_selectedElementIndex)
                            {
                                num6 = this.m_timeSlice / this.m_expandedTime;
                                num6 = Mathf.Clamp(num6, 0f, 1f);
                            }
                            stRect item = this.LayoutExpandElement(j, num6, ref this.m_contentSize, ref offset);
                            if (j < base.m_elementsRect.Count)
                            {
                                base.m_elementsRect[j] = item;
                            }
                            else
                            {
                                base.m_elementsRect.Add(item);
                            }
                        }
                        this.ResizeContent(ref this.m_contentSize, false);
                    }
                    goto Label_055A;

                default:
                    goto Label_055A;
            }
            for (int i = 0; i < base.m_elementAmount; i++)
            {
                float num2 = 0f;
                if (i == base.m_lastSelectedElementIndex)
                {
                    num2 = 1f - (this.m_timeSlice / this.m_expandedTime);
                    num2 = Mathf.Clamp(num2, 0f, 1f);
                }
                stRect rect = this.LayoutExpandElement(i, num2, ref this.m_contentSize, ref zero);
                if (i < base.m_elementsRect.Count)
                {
                    base.m_elementsRect[i] = rect;
                }
                else
                {
                    base.m_elementsRect.Add(rect);
                }
            }
            this.ResizeContent(ref this.m_contentSize, false);
            if ((base.m_selectedElementIndex < 0) || (base.m_selectedElementIndex >= base.m_elementAmount))
            {
                if (base.m_listType == enUIListType.Horizontal)
                {
                    if (this.m_contentAnchoredPosition.x > 0f)
                    {
                        this.m_contentAnchoredPosition.x = 0f;
                    }
                    else if ((this.m_contentAnchoredPosition.x + this.m_contentSize.x) < this.m_scrollAreaSize.x)
                    {
                        this.m_contentAnchoredPosition.x = this.m_scrollAreaSize.x - this.m_contentSize.x;
                    }
                }
                else if (base.m_listType == enUIListType.Vertical)
                {
                    if (this.m_contentAnchoredPosition.y < 0f)
                    {
                        this.m_contentAnchoredPosition.y = 0f;
                    }
                    else if ((this.m_contentAnchoredPosition.y - this.m_contentSize.y) > -this.m_scrollAreaSize.y)
                    {
                        this.m_contentAnchoredPosition.y = -this.m_scrollAreaSize.y + this.m_contentSize.y;
                    }
                }
            }
        Label_055A:
            num7 = 0;
            while (num7 < base.m_elementAmount)
            {
                stRect rect3 = base.m_elementsRect[num7];
                CUIListElementScript elemenet = base.GetElemenet(num7);
                if (elemenet != null)
                {
                    elemenet.SetRect(ref rect3);
                }
                num7++;
            }
            base.m_contentRectTransform.anchoredPosition = this.m_contentAnchoredPosition;
        }
    }
}

