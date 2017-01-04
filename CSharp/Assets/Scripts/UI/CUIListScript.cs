namespace Assets.Scripts.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class CUIListScript : CUIComponent
    {
        public bool m_alwaysDispatchSelectedChangeEvent;
        [HideInInspector]
        public bool m_autoAdjustScrollAreaSize;
        public bool m_autoCenteredElements;
        protected GameObject m_content;
        protected RectTransform m_contentRectTransform;
        protected Vector2 m_contentSize;
        public int m_elementAmount;
        protected Vector2 m_elementDefaultSize;
        public float m_elementLayoutOffset;
        protected string m_elementName;
        protected ListView<CUIListElementScript> m_elementScripts;
        public Vector2 m_elementSpacing;
        protected List<stRect> m_elementsRect;
        protected List<Vector2> m_elementsSize;
        protected GameObject m_elementTemplate;
        [HideInInspector]
        public string m_externalElementPrefabPath;
        public GameObject m_extraContent;
        public float m_fSpeed = 0.2f;
        protected Vector2 m_lastContentPosition;
        protected int m_lastSelectedElementIndex = -1;
        [HideInInspector]
        public enUIEventID m_listScrollChangedEventID;
        public stUIEventParams m_listScrollChangedEventParams;
        [HideInInspector]
        public enUIEventID m_listSelectChangedEventID;
        public stUIEventParams m_listSelectChangedEventParams;
        public enUIListType m_listType;
        [HideInInspector]
        public Vector2 m_scrollAreaSize;
        protected Scrollbar m_scrollBar;
        public bool m_scrollExternal;
        [HideInInspector]
        public ScrollRect m_scrollRect;
        [HideInInspector]
        public Vector2 m_scrollRectAreaMaxSize = new Vector2(100f, 100f);
        protected Vector2 m_scrollValue;
        protected int m_selectedElementIndex = -1;
        protected ListView<CUIListElementScript> m_unUsedElementScripts;
        [HideInInspector]
        public bool m_useExternalElement;
        public bool m_useOptimized;
        public GameObject m_ZeroTipsObj;

        protected CUIListElementScript CreateElement(int index, ref stRect rect)
        {
            CUIListElementScript item = null;
            if (this.m_unUsedElementScripts.Count > 0)
            {
                item = this.m_unUsedElementScripts[0];
                this.m_unUsedElementScripts.RemoveAt(0);
            }
            else if (this.m_elementTemplate != null)
            {
                GameObject root = base.Instantiate(this.m_elementTemplate);
                root.transform.SetParent(this.m_content.transform);
                root.transform.localScale = Vector3.one;
                base.InitializeComponent(root);
                item = root.GetComponent<CUIListElementScript>();
            }
            if (item != null)
            {
                item.Enable(this, index, this.m_elementName, ref rect, this.IsSelectedIndex(index));
                this.m_elementScripts.Add(item);
            }
            return item;
        }

        protected void DetectScroll()
        {
            if (this.m_contentRectTransform.anchoredPosition != this.m_lastContentPosition)
            {
                if ((this.m_listType == enUIListType.Horizontal) || (this.m_listType == enUIListType.HorizontalGrid))
                {
                    float x = (this.m_contentSize.x != this.m_scrollAreaSize.x) ? (this.m_contentRectTransform.anchoredPosition.x / (this.m_contentSize.x - this.m_scrollAreaSize.x)) : 0f;
                    this.OnScrollValueChanged(new Vector2(x, 0f));
                }
                else if ((this.m_listType == enUIListType.VerticalGrid) || (this.m_listType == enUIListType.Vertical))
                {
                    float y = (this.m_contentSize.y != this.m_scrollAreaSize.y) ? (this.m_contentRectTransform.anchoredPosition.y / (this.m_contentSize.y - this.m_scrollAreaSize.y)) : 0f;
                    this.OnScrollValueChanged(new Vector2(0f, y));
                }
                this.m_lastContentPosition = this.m_contentRectTransform.anchoredPosition;
            }
        }

        protected void DispatchElementSelectChangedEvent()
        {
            if (this.m_listSelectChangedEventID != enUIEventID.None)
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_eventID = this.m_listSelectChangedEventID;
                uIEvent.m_eventParams = this.m_listSelectChangedEventParams;
                uIEvent.m_srcFormScript = base.m_belongedFormScript;
                uIEvent.m_srcWidgetBelongedListScript = base.m_belongedListScript;
                uIEvent.m_srcWidgetIndexInBelongedList = base.m_indexInlist;
                uIEvent.m_srcWidget = base.gameObject;
                uIEvent.m_srcWidgetScript = this;
                uIEvent.m_pointerEventData = null;
                base.DispatchUIEvent(uIEvent);
            }
        }

        protected void DispatchScrollChangedEvent()
        {
            if (this.m_listScrollChangedEventID != enUIEventID.None)
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_eventID = this.m_listScrollChangedEventID;
                uIEvent.m_eventParams = this.m_listScrollChangedEventParams;
                uIEvent.m_srcFormScript = base.m_belongedFormScript;
                uIEvent.m_srcWidgetBelongedListScript = base.m_belongedListScript;
                uIEvent.m_srcWidgetIndexInBelongedList = base.m_indexInlist;
                uIEvent.m_srcWidget = base.gameObject;
                uIEvent.m_srcWidgetScript = this;
                uIEvent.m_pointerEventData = null;
                base.DispatchUIEvent(uIEvent);
            }
        }

        public CUIListElementScript GetElemenet(int index)
        {
            if ((index >= 0) && (index < this.m_elementAmount))
            {
                if (!this.m_useOptimized)
                {
                    return this.m_elementScripts[index];
                }
                for (int i = 0; i < this.m_elementScripts.Count; i++)
                {
                    if (this.m_elementScripts[i].m_index == index)
                    {
                        return this.m_elementScripts[i];
                    }
                }
            }
            return null;
        }

        public int GetElementAmount()
        {
            return this.m_elementAmount;
        }

        public CUIListElementScript GetLastSelectedElement()
        {
            return this.GetElemenet(this.m_lastSelectedElementIndex);
        }

        public int GetLastSelectedIndex()
        {
            return this.m_lastSelectedElementIndex;
        }

        public Vector2 GetScrollValue()
        {
            return this.m_scrollValue;
        }

        public CUIListElementScript GetSelectedElement()
        {
            return this.GetElemenet(this.m_selectedElementIndex);
        }

        public int GetSelectedIndex()
        {
            return this.m_selectedElementIndex;
        }

        public void HideExtraContent()
        {
            if (this.m_extraContent != null)
            {
                this.m_extraContent.CustomSetActive(false);
            }
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                this.m_selectedElementIndex = -1;
                this.m_lastSelectedElementIndex = -1;
                this.m_scrollRect = base.GetComponentInChildren<ScrollRect>(base.gameObject);
                if (this.m_scrollRect != null)
                {
                    this.m_scrollRect.enabled = false;
                    RectTransform transform = this.m_scrollRect.transform as RectTransform;
                    this.m_scrollAreaSize = new Vector2(transform.rect.width, transform.rect.height);
                    this.m_content = this.m_scrollRect.content.gameObject;
                }
                this.m_scrollBar = base.GetComponentInChildren<Scrollbar>(base.gameObject);
                if ((this.m_listType == enUIListType.Vertical) || (this.m_listType == enUIListType.VerticalGrid))
                {
                    if (this.m_scrollBar != null)
                    {
                        this.m_scrollBar.direction = Scrollbar.Direction.BottomToTop;
                    }
                    DebugHelper.Assert(this.m_scrollRect != null);
                    if (this.m_scrollRect != null)
                    {
                        this.m_scrollRect.horizontal = false;
                        this.m_scrollRect.vertical = true;
                        this.m_scrollRect.horizontalScrollbar = null;
                        this.m_scrollRect.verticalScrollbar = this.m_scrollBar;
                    }
                }
                else if ((this.m_listType == enUIListType.Horizontal) || (this.m_listType == enUIListType.HorizontalGrid))
                {
                    if (this.m_scrollBar != null)
                    {
                        this.m_scrollBar.direction = Scrollbar.Direction.LeftToRight;
                    }
                    DebugHelper.Assert(this.m_scrollRect != null);
                    if (this.m_scrollRect != null)
                    {
                        this.m_scrollRect.horizontal = true;
                        this.m_scrollRect.vertical = false;
                        this.m_scrollRect.horizontalScrollbar = this.m_scrollBar;
                        this.m_scrollRect.verticalScrollbar = null;
                    }
                }
                this.m_elementScripts = new ListView<CUIListElementScript>();
                this.m_unUsedElementScripts = new ListView<CUIListElementScript>();
                if (this.m_useOptimized && (this.m_elementsRect == null))
                {
                    this.m_elementsRect = new List<stRect>();
                }
                CUIListElementScript component = null;
                if (this.m_useExternalElement)
                {
                    GameObject content = (GameObject) Singleton<CResourceManager>.GetInstance().GetResource(this.m_externalElementPrefabPath, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content;
                    component = content.GetComponent<CUIListElementScript>();
                    if (component != null)
                    {
                        component.Initialize(formScript);
                        this.m_elementTemplate = content;
                        this.m_elementName = content.name;
                        this.m_elementDefaultSize = component.m_defaultSize;
                    }
                }
                else
                {
                    component = base.GetComponentInChildren<CUIListElementScript>(base.gameObject);
                    if (component != null)
                    {
                        component.Initialize(formScript);
                        this.m_elementTemplate = component.gameObject;
                        this.m_elementName = component.gameObject.name;
                        this.m_elementDefaultSize = component.m_defaultSize;
                        if (this.m_elementTemplate != null)
                        {
                            this.m_elementTemplate.name = this.m_elementName + "_Template";
                        }
                    }
                }
                if (this.m_elementTemplate != null)
                {
                    CUIListElementScript script2 = this.m_elementTemplate.GetComponent<CUIListElementScript>();
                    if ((script2 != null) && script2.m_useSetActiveForDisplay)
                    {
                        this.m_elementTemplate.CustomSetActive(false);
                    }
                    else
                    {
                        if (!this.m_elementTemplate.activeSelf)
                        {
                            this.m_elementTemplate.SetActive(true);
                        }
                        CanvasGroup group = this.m_elementTemplate.GetComponent<CanvasGroup>();
                        if (group == null)
                        {
                            group = this.m_elementTemplate.AddComponent<CanvasGroup>();
                        }
                        group.alpha = 0f;
                        group.blocksRaycasts = false;
                    }
                }
                if (this.m_content != null)
                {
                    this.m_contentRectTransform = this.m_content.transform as RectTransform;
                    this.m_contentRectTransform.pivot = new Vector2(0f, 1f);
                    this.m_contentRectTransform.anchorMin = new Vector2(0f, 1f);
                    this.m_contentRectTransform.anchorMax = new Vector2(0f, 1f);
                    this.m_contentRectTransform.anchoredPosition = Vector2.zero;
                    this.m_contentRectTransform.localRotation = Quaternion.identity;
                    this.m_contentRectTransform.localScale = new Vector3(1f, 1f, 1f);
                    this.m_lastContentPosition = this.m_contentRectTransform.anchoredPosition;
                }
                if (this.m_extraContent != null)
                {
                    RectTransform transform2 = this.m_extraContent.transform as RectTransform;
                    transform2.pivot = new Vector2(0f, 1f);
                    transform2.anchorMin = new Vector2(0f, 1f);
                    transform2.anchorMax = new Vector2(0f, 1f);
                    transform2.anchoredPosition = Vector2.zero;
                    transform2.localRotation = Quaternion.identity;
                    transform2.localScale = Vector3.one;
                    if (this.m_elementTemplate != null)
                    {
                        transform2.sizeDelta = new Vector2((this.m_elementTemplate.transform as RectTransform).rect.width, transform2.sizeDelta.y);
                    }
                    if ((transform2.parent != null) && (this.m_content != null))
                    {
                        transform2.parent.SetParent(this.m_content.transform);
                    }
                    this.m_extraContent.SetActive(false);
                }
                this.SetElementAmount(this.m_elementAmount);
            }
        }

        public bool IsElementInScrollArea(int index)
        {
            if ((index < 0) || (index >= this.m_elementAmount))
            {
                return false;
            }
            stRect rect = !this.m_useOptimized ? this.m_elementScripts[index].m_rect : this.m_elementsRect[index];
            return this.IsRectInScrollArea(ref rect);
        }

        protected bool IsRectInScrollArea(ref stRect rect)
        {
            Vector2 zero = Vector2.zero;
            zero.x = this.m_contentRectTransform.anchoredPosition.x + rect.m_left;
            zero.y = this.m_contentRectTransform.anchoredPosition.y + rect.m_top;
            return ((((zero.x + rect.m_width) >= 0f) && (zero.x <= this.m_scrollAreaSize.x)) && (((zero.y - rect.m_height) <= 0f) && (zero.y >= -this.m_scrollAreaSize.y)));
        }

        public virtual bool IsSelectedIndex(int index)
        {
            return (this.m_selectedElementIndex == index);
        }

        protected stRect LayoutElement(int index, ref Vector2 contentSize, ref Vector2 offset)
        {
            stRect rect;
            rect = new stRect {
                m_width = (((this.m_elementsSize != null) && (this.m_listType != enUIListType.Vertical)) && ((this.m_listType != enUIListType.VerticalGrid) && (this.m_listType != enUIListType.HorizontalGrid))) ? ((int) this.m_elementsSize[index].x) : ((int) this.m_elementDefaultSize.x),
                m_height = (((this.m_elementsSize != null) && (this.m_listType != enUIListType.Horizontal)) && ((this.m_listType != enUIListType.VerticalGrid) && (this.m_listType != enUIListType.HorizontalGrid))) ? ((int) this.m_elementsSize[index].y) : ((int) this.m_elementDefaultSize.y),
                m_left = (int) offset.x,
                m_top = (int) offset.y,
                m_right = rect.m_left + rect.m_width,
                m_bottom = rect.m_top - rect.m_height,
                m_center = new Vector2(rect.m_left + (rect.m_width * 0.5f), rect.m_top - (rect.m_height * 0.5f))
            };
            if (rect.m_right > contentSize.x)
            {
                contentSize.x = rect.m_right;
            }
            if (-rect.m_bottom > contentSize.y)
            {
                contentSize.y = -rect.m_bottom;
            }
            switch (this.m_listType)
            {
                case enUIListType.Vertical:
                    offset.y -= rect.m_height + this.m_elementSpacing.y;
                    return rect;

                case enUIListType.Horizontal:
                    offset.x += rect.m_width + this.m_elementSpacing.x;
                    return rect;

                case enUIListType.VerticalGrid:
                    offset.x += rect.m_width + this.m_elementSpacing.x;
                    if ((offset.x + rect.m_width) > this.m_scrollAreaSize.x)
                    {
                        offset.x = 0f;
                        offset.y -= rect.m_height + this.m_elementSpacing.y;
                    }
                    return rect;

                case enUIListType.HorizontalGrid:
                    offset.y -= rect.m_height + this.m_elementSpacing.y;
                    if ((-offset.y + rect.m_height) > this.m_scrollAreaSize.y)
                    {
                        offset.y = 0f;
                        offset.x += rect.m_width + this.m_elementSpacing.x;
                    }
                    return rect;
            }
            return rect;
        }

        public void MoveElementInScrollArea(int index, bool moveImmediately)
        {
            if ((index >= 0) && (index < this.m_elementAmount))
            {
                Vector2 zero = Vector2.zero;
                Vector2 vector2 = Vector2.zero;
                stRect rect = !this.m_useOptimized ? this.m_elementScripts[index].m_rect : this.m_elementsRect[index];
                vector2.x = this.m_contentRectTransform.anchoredPosition.x + rect.m_left;
                vector2.y = this.m_contentRectTransform.anchoredPosition.y + rect.m_top;
                if (vector2.x < 0f)
                {
                    zero.x = -vector2.x;
                }
                else if ((vector2.x + rect.m_width) > this.m_scrollAreaSize.x)
                {
                    zero.x = this.m_scrollAreaSize.x - (vector2.x + rect.m_width);
                }
                if (vector2.y > 0f)
                {
                    zero.y = -vector2.y;
                }
                else if ((vector2.y - rect.m_height) < -this.m_scrollAreaSize.y)
                {
                    zero.y = -this.m_scrollAreaSize.y - (vector2.y - rect.m_height);
                }
                if (moveImmediately)
                {
                    this.m_contentRectTransform.anchoredPosition += zero;
                }
                else
                {
                    Vector2 to = this.m_contentRectTransform.anchoredPosition + zero;
                    LeanTween.value(base.gameObject, pos => this.m_contentRectTransform.anchoredPosition = pos, this.m_contentRectTransform.anchoredPosition, to, this.m_fSpeed);
                }
            }
        }

        private void OnDestroy()
        {
            if (LeanTween.IsInitialised())
            {
                LeanTween.cancel(base.gameObject);
            }
        }

        protected void OnScrollValueChanged(Vector2 value)
        {
            this.m_scrollValue = value;
            this.DispatchScrollChangedEvent();
        }

        protected virtual void ProcessElements()
        {
            this.m_contentSize = Vector2.zero;
            Vector2 zero = Vector2.zero;
            if ((this.m_listType == enUIListType.Vertical) || (this.m_listType == enUIListType.VerticalGrid))
            {
                zero.y += this.m_elementLayoutOffset;
            }
            else
            {
                zero.x += this.m_elementLayoutOffset;
            }
            for (int i = 0; i < this.m_elementAmount; i++)
            {
                stRect item = this.LayoutElement(i, ref this.m_contentSize, ref zero);
                if (this.m_useOptimized)
                {
                    if (i < this.m_elementsRect.Count)
                    {
                        this.m_elementsRect[i] = item;
                    }
                    else
                    {
                        this.m_elementsRect.Add(item);
                    }
                }
                if (!this.m_useOptimized || this.IsRectInScrollArea(ref item))
                {
                    this.CreateElement(i, ref item);
                }
            }
            if (this.m_extraContent != null)
            {
                if (this.m_elementAmount > 0)
                {
                    this.ProcessExtraContent(ref this.m_contentSize, zero);
                }
                else
                {
                    this.m_extraContent.CustomSetActive(false);
                }
            }
            this.ResizeContent(ref this.m_contentSize, false);
        }

        private void ProcessExtraContent(ref Vector2 contentSize, Vector2 offset)
        {
            RectTransform transform = this.m_extraContent.transform as RectTransform;
            transform.anchoredPosition = offset;
            this.m_extraContent.CustomSetActive(true);
            if ((this.m_listType == enUIListType.Horizontal) || (this.m_listType == enUIListType.HorizontalGrid))
            {
                contentSize.x += transform.rect.width + this.m_elementSpacing.x;
            }
            else
            {
                contentSize.y += transform.rect.height + this.m_elementSpacing.y;
            }
        }

        protected void ProcessUnUsedElement()
        {
            if ((this.m_unUsedElementScripts != null) && (this.m_unUsedElementScripts.Count > 0))
            {
                for (int i = 0; i < this.m_unUsedElementScripts.Count; i++)
                {
                    this.m_unUsedElementScripts[i].Disable();
                }
            }
        }

        protected void RecycleElement(bool disableElement)
        {
            while (this.m_elementScripts.Count > 0)
            {
                CUIListElementScript item = this.m_elementScripts[0];
                this.m_elementScripts.RemoveAt(0);
                if (disableElement)
                {
                    item.Disable();
                }
                this.m_unUsedElementScripts.Add(item);
            }
        }

        protected void RecycleElement(CUIListElementScript elementScript, bool disableElement)
        {
            if (disableElement)
            {
                elementScript.Disable();
            }
            this.m_elementScripts.Remove(elementScript);
            this.m_unUsedElementScripts.Add(elementScript);
        }

        public void ResetContentPosition()
        {
            if (LeanTween.IsInitialised())
            {
                LeanTween.cancel(base.gameObject);
            }
            if (this.m_contentRectTransform != null)
            {
                this.m_contentRectTransform.anchoredPosition = Vector2.zero;
            }
        }

        protected virtual void ResizeContent(ref Vector2 size, bool resetPosition)
        {
            float x = 0f;
            float y = 0f;
            if (this.m_autoAdjustScrollAreaSize)
            {
                Vector2 scrollAreaSize = this.m_scrollAreaSize;
                this.m_scrollAreaSize = size;
                if (this.m_scrollAreaSize.x > this.m_scrollRectAreaMaxSize.x)
                {
                    this.m_scrollAreaSize.x = this.m_scrollRectAreaMaxSize.x;
                }
                if (this.m_scrollAreaSize.y > this.m_scrollRectAreaMaxSize.y)
                {
                    this.m_scrollAreaSize.y = this.m_scrollRectAreaMaxSize.y;
                }
                Vector2 vector2 = this.m_scrollAreaSize - scrollAreaSize;
                if (vector2 != Vector2.zero)
                {
                    RectTransform transform = base.gameObject.transform as RectTransform;
                    if (transform.anchorMin == transform.anchorMax)
                    {
                        transform.sizeDelta += vector2;
                    }
                }
            }
            else if (this.m_autoCenteredElements)
            {
                if ((this.m_listType == enUIListType.Vertical) && (size.y < this.m_scrollAreaSize.y))
                {
                    y = (size.y - this.m_scrollAreaSize.y) / 2f;
                }
                else if ((this.m_listType == enUIListType.Horizontal) && (size.x < this.m_scrollAreaSize.x))
                {
                    x = (this.m_scrollAreaSize.x - size.x) / 2f;
                }
                else
                {
                    if ((this.m_listType != enUIListType.VerticalGrid) || (size.x >= this.m_scrollAreaSize.x))
                    {
                        if ((this.m_listType != enUIListType.HorizontalGrid) || (size.y >= this.m_scrollAreaSize.y))
                        {
                            goto Label_02A9;
                        }
                        float num6 = size.y + this.m_elementSpacing.y;
                        while (true)
                        {
                            float num5 = num6 + this.m_elementDefaultSize.y;
                            if (num5 > this.m_scrollAreaSize.y)
                            {
                                y = (size.y - this.m_scrollAreaSize.y) / 2f;
                                goto Label_02A9;
                            }
                            size.y = num5;
                            num6 = num5 + this.m_elementSpacing.y;
                        }
                    }
                    float num4 = size.x + this.m_elementSpacing.x;
                    while (true)
                    {
                        float num3 = num4 + this.m_elementDefaultSize.x;
                        if (num3 > this.m_scrollAreaSize.x)
                        {
                            break;
                        }
                        size.x = num3;
                        num4 = num3 + this.m_elementSpacing.x;
                    }
                    x = (this.m_scrollAreaSize.x - size.x) / 2f;
                }
            }
        Label_02A9:
            if (size.x < this.m_scrollAreaSize.x)
            {
                size.x = this.m_scrollAreaSize.x;
            }
            if (size.y < this.m_scrollAreaSize.y)
            {
                size.y = this.m_scrollAreaSize.y;
            }
            if (this.m_contentRectTransform != null)
            {
                this.m_contentRectTransform.sizeDelta = size;
                if (resetPosition)
                {
                    this.m_contentRectTransform.anchoredPosition = Vector2.zero;
                }
                if (this.m_autoCenteredElements)
                {
                    if (x != 0f)
                    {
                        this.m_contentRectTransform.anchoredPosition = new Vector2(x, this.m_contentRectTransform.anchoredPosition.y);
                    }
                    if (y != 0f)
                    {
                        this.m_contentRectTransform.anchoredPosition = new Vector2(this.m_contentRectTransform.anchoredPosition.x, y);
                    }
                }
            }
        }

        public virtual void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
        {
            this.m_lastSelectedElementIndex = this.m_selectedElementIndex;
            this.m_selectedElementIndex = index;
            if (this.m_lastSelectedElementIndex == this.m_selectedElementIndex)
            {
                if (this.m_alwaysDispatchSelectedChangeEvent)
                {
                    this.DispatchElementSelectChangedEvent();
                }
            }
            else
            {
                if (this.m_lastSelectedElementIndex >= 0)
                {
                    CUIListElementScript elemenet = this.GetElemenet(this.m_lastSelectedElementIndex);
                    if (elemenet != null)
                    {
                        elemenet.ChangeDisplay(false);
                    }
                }
                if (this.m_selectedElementIndex >= 0)
                {
                    CUIListElementScript script2 = this.GetElemenet(this.m_selectedElementIndex);
                    if (script2 != null)
                    {
                        script2.ChangeDisplay(true);
                        if (script2.onSelected != null)
                        {
                            script2.onSelected();
                        }
                    }
                }
                if (isDispatchSelectedChangeEvent)
                {
                    this.DispatchElementSelectChangedEvent();
                }
            }
        }

        public void SetElementAmount(int amount)
        {
            this.SetElementAmount(amount, null);
        }

        public virtual void SetElementAmount(int amount, List<Vector2> elementsSize)
        {
            if (amount < 0)
            {
                amount = 0;
            }
            if ((elementsSize == null) || (amount == elementsSize.Count))
            {
                this.RecycleElement(false);
                this.m_elementAmount = amount;
                this.m_elementsSize = elementsSize;
                this.ProcessElements();
                this.ProcessUnUsedElement();
                if (this.m_ZeroTipsObj != null)
                {
                    if (amount <= 0)
                    {
                        this.m_ZeroTipsObj.SetActive(true);
                    }
                    else
                    {
                        this.m_ZeroTipsObj.SetActive(false);
                    }
                }
            }
        }

        public void SetElementSelectChangedEvent(enUIEventID eventID)
        {
            this.m_listSelectChangedEventID = eventID;
        }

        public void SetElementSelectChangedEvent(enUIEventID eventID, stUIEventParams eventParams)
        {
            this.m_listSelectChangedEventID = eventID;
            this.m_listSelectChangedEventParams = eventParams;
        }

        public void ShowExtraContent()
        {
            if (this.m_extraContent != null)
            {
                this.m_extraContent.CustomSetActive(true);
            }
        }

        protected virtual void Update()
        {
            if ((base.m_belongedFormScript == null) || !base.m_belongedFormScript.IsClosed())
            {
                if (this.m_useOptimized)
                {
                    this.UpdateElementsScroll();
                }
                if ((this.m_scrollRect != null) && !this.m_scrollExternal)
                {
                    if ((this.m_contentSize.x > this.m_scrollAreaSize.x) || (this.m_contentSize.y > this.m_scrollAreaSize.y))
                    {
                        if (!this.m_scrollRect.enabled)
                        {
                            this.m_scrollRect.enabled = true;
                        }
                    }
                    else if (((Mathf.Abs(this.m_contentRectTransform.anchoredPosition.x) < 0.001) && (Mathf.Abs(this.m_contentRectTransform.anchoredPosition.y) < 0.001)) && this.m_scrollRect.enabled)
                    {
                        this.m_scrollRect.enabled = false;
                    }
                    this.DetectScroll();
                }
            }
        }

        protected void UpdateElementsScroll()
        {
            int num = 0;
            while (num < this.m_elementScripts.Count)
            {
                if (!this.IsRectInScrollArea(ref this.m_elementScripts[num].m_rect))
                {
                    this.RecycleElement(this.m_elementScripts[num], true);
                }
                else
                {
                    num++;
                }
            }
            for (int i = 0; i < this.m_elementAmount; i++)
            {
                stRect rect = this.m_elementsRect[i];
                if (this.IsRectInScrollArea(ref rect))
                {
                    bool flag = false;
                    for (int j = 0; j < this.m_elementScripts.Count; j++)
                    {
                        if (this.m_elementScripts[j].m_index == i)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        this.CreateElement(i, ref rect);
                    }
                }
            }
        }
    }
}

