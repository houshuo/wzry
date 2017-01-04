namespace Assets.Scripts.UI
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class CUIListElementScript : CUIComponent
    {
        public bool m_autoAddUIEventScript = true;
        private CanvasGroup m_canvasGroup;
        [HideInInspector]
        public Color m_defaultColor;
        [HideInInspector]
        public ImageAlphaTexLayout m_defaultLayout;
        [HideInInspector]
        public Vector2 m_defaultSize;
        [HideInInspector]
        public Sprite m_defaultSprite;
        [HideInInspector]
        public Color m_defaultTextColor;
        private Image m_image;
        [HideInInspector]
        public int m_index;
        [HideInInspector]
        public enUIEventID m_onEnableEventID;
        public stUIEventParams m_onEnableEventParams;
        [HideInInspector]
        public enPivotType m_pivotType = enPivotType.LeftTop;
        public stRect m_rect;
        public ImageAlphaTexLayout m_selectedLayout;
        public Sprite m_selectedSprite;
        public GameObject m_selectFrontObj;
        public Color m_selectTextColor = new Color(1f, 1f, 1f, 1f);
        public Text m_textObj;
        public bool m_useSetActiveForDisplay;
        public OnSelectedDelegate onSelected;

        public virtual void ChangeDisplay(bool selected)
        {
            if ((this.m_image != null) && (this.m_selectedSprite != null))
            {
                if (selected)
                {
                    this.m_image.sprite = this.m_selectedSprite;
                    this.m_image.color = new Color(this.m_defaultColor.r, this.m_defaultColor.g, this.m_defaultColor.b, 255f);
                }
                else
                {
                    this.m_image.sprite = this.m_defaultSprite;
                    this.m_image.color = this.m_defaultColor;
                }
                if (this.m_image is Image2)
                {
                    Image2 image = this.m_image as Image2;
                    image.alphaTexLayout = !selected ? this.m_defaultLayout : this.m_selectedLayout;
                }
            }
            if (this.m_selectFrontObj != null)
            {
                this.m_selectFrontObj.CustomSetActive(selected);
            }
            if (this.m_textObj != null)
            {
                this.m_textObj.color = !selected ? this.m_defaultTextColor : this.m_selectTextColor;
            }
        }

        public void Disable()
        {
            if (this.m_useSetActiveForDisplay)
            {
                base.gameObject.CustomSetActive(false);
            }
            else
            {
                this.m_canvasGroup.alpha = 0f;
                this.m_canvasGroup.blocksRaycasts = false;
            }
        }

        protected void DispatchOnEnableEvent()
        {
            if (this.m_onEnableEventID != enUIEventID.None)
            {
                CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
                uIEvent.m_eventID = this.m_onEnableEventID;
                uIEvent.m_eventParams = this.m_onEnableEventParams;
                uIEvent.m_srcFormScript = base.m_belongedFormScript;
                uIEvent.m_srcWidgetBelongedListScript = base.m_belongedListScript;
                uIEvent.m_srcWidgetIndexInBelongedList = base.m_indexInlist;
                uIEvent.m_srcWidget = base.gameObject;
                uIEvent.m_srcWidgetScript = this;
                uIEvent.m_pointerEventData = null;
                base.DispatchUIEvent(uIEvent);
            }
        }

        public void Enable(CUIListScript belongedList, int index, string name, ref stRect rect, bool selected)
        {
            base.m_belongedListScript = belongedList;
            this.m_index = index;
            base.gameObject.name = name + "_" + index.ToString();
            if (this.m_useSetActiveForDisplay)
            {
                base.gameObject.CustomSetActive(true);
            }
            else
            {
                this.m_canvasGroup.alpha = 1f;
                this.m_canvasGroup.blocksRaycasts = true;
            }
            this.SetComponentBelongedList(base.gameObject);
            this.SetRect(ref rect);
            this.ChangeDisplay(selected);
            this.DispatchOnEnableEvent();
        }

        protected virtual Vector2 GetDefaultSize()
        {
            return new Vector2(((RectTransform) base.gameObject.transform).rect.width, ((RectTransform) base.gameObject.transform).rect.height);
        }

        public override void Initialize(CUIFormScript formScript)
        {
            if (!base.m_isInitialized)
            {
                base.Initialize(formScript);
                this.m_image = base.gameObject.GetComponent<Image>();
                if (this.m_image != null)
                {
                    this.m_defaultSprite = this.m_image.sprite;
                    this.m_defaultColor = this.m_image.color;
                    if (this.m_image is Image2)
                    {
                        Image2 image = this.m_image as Image2;
                        this.m_defaultLayout = image.alphaTexLayout;
                    }
                }
                if (this.m_autoAddUIEventScript && (base.gameObject.GetComponent<CUIEventScript>() == null))
                {
                    base.gameObject.AddComponent<CUIEventScript>().Initialize(formScript);
                }
                if (!this.m_useSetActiveForDisplay)
                {
                    this.m_canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
                    if (this.m_canvasGroup == null)
                    {
                        this.m_canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
                    }
                }
                this.m_defaultSize = this.GetDefaultSize();
                this.InitRectTransform();
                if (this.m_textObj != null)
                {
                    this.m_defaultTextColor = this.m_textObj.color;
                }
            }
        }

        private void InitRectTransform()
        {
            RectTransform transform = base.gameObject.transform as RectTransform;
            transform.anchorMin = new Vector2(0f, 1f);
            transform.anchorMax = new Vector2(0f, 1f);
            transform.pivot = (this.m_pivotType != enPivotType.Centre) ? new Vector2(0f, 1f) : new Vector2(0.5f, 0.5f);
            transform.sizeDelta = this.m_defaultSize;
            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        public void OnSelected(BaseEventData baseEventData)
        {
            base.m_belongedListScript.SelectElement(this.m_index, true);
        }

        public void SetComponentBelongedList(GameObject gameObject)
        {
            CUIComponent[] components = gameObject.GetComponents<CUIComponent>();
            if ((components != null) && (components.Length > 0))
            {
                for (int j = 0; j < components.Length; j++)
                {
                    components[j].SetBelongedList(base.m_belongedListScript, this.m_index);
                }
            }
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                this.SetComponentBelongedList(gameObject.transform.GetChild(i).gameObject);
            }
        }

        public void SetOnEnableEvent(enUIEventID eventID)
        {
            this.m_onEnableEventID = eventID;
        }

        public void SetOnEnableEvent(enUIEventID eventID, stUIEventParams eventParams)
        {
            this.m_onEnableEventID = eventID;
            this.m_onEnableEventParams = eventParams;
        }

        public void SetRect(ref stRect rect)
        {
            this.m_rect = rect;
            RectTransform transform = base.gameObject.transform as RectTransform;
            transform.sizeDelta = new Vector2((float) this.m_rect.m_width, (float) this.m_rect.m_height);
            if (this.m_pivotType == enPivotType.Centre)
            {
                transform.anchoredPosition = rect.m_center;
            }
            else
            {
                transform.anchoredPosition = new Vector2((float) rect.m_left, (float) rect.m_top);
            }
        }

        public delegate void OnSelectedDelegate();
    }
}

