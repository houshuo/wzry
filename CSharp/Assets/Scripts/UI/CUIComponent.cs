namespace Assets.Scripts.UI
{
    using System;
    using UnityEngine;

    public class CUIComponent : MonoBehaviour
    {
        [HideInInspector]
        public CUIFormScript m_belongedFormScript;
        [HideInInspector]
        public CUIListScript m_belongedListScript;
        [HideInInspector]
        public int m_indexInlist;
        protected bool m_isInitialized;
        public GameObject[] m_widgets = new GameObject[0];

        public virtual void Appear()
        {
        }

        public virtual void Close()
        {
        }

        protected void DispatchUIEvent(CUIEvent uiEvent)
        {
            if (Singleton<CUIEventManager>.GetInstance() != null)
            {
                Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
            }
        }

        protected T GetComponentInChildren<T>(GameObject go) where T: Component
        {
            T component = go.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
            for (int i = 0; i < go.transform.childCount; i++)
            {
                component = this.GetComponentInChildren<T>(go.transform.GetChild(i).gameObject);
                if (component != null)
                {
                    return component;
                }
            }
            return null;
        }

        public GameObject GetWidget(int index)
        {
            if ((index >= 0) && (index < this.m_widgets.Length))
            {
                return this.m_widgets[index];
            }
            return null;
        }

        public virtual void Hide()
        {
        }

        public virtual void Initialize(CUIFormScript formScript)
        {
            if (!this.m_isInitialized)
            {
                this.m_belongedFormScript = formScript;
                if (this.m_belongedFormScript != null)
                {
                    this.m_belongedFormScript.AddUIComponent(this);
                    this.SetSortingOrder(this.m_belongedFormScript.GetSortingOrder());
                }
                this.m_isInitialized = true;
            }
        }

        protected void InitializeComponent(GameObject root)
        {
            CUIComponent[] components = root.GetComponents<CUIComponent>();
            if ((components != null) && (components.Length > 0))
            {
                for (int j = 0; j < components.Length; j++)
                {
                    components[j].Initialize(this.m_belongedFormScript);
                }
            }
            for (int i = 0; i < root.transform.childCount; i++)
            {
                this.InitializeComponent(root.transform.GetChild(i).gameObject);
            }
        }

        protected GameObject Instantiate(GameObject srcGameObject)
        {
            GameObject obj2 = UnityEngine.Object.Instantiate(srcGameObject) as GameObject;
            obj2.transform.SetParent(srcGameObject.transform.parent);
            RectTransform transform = srcGameObject.transform as RectTransform;
            RectTransform transform2 = obj2.transform as RectTransform;
            if ((transform != null) && (transform2 != null))
            {
                transform2.pivot = transform.pivot;
                transform2.anchorMin = transform.anchorMin;
                transform2.anchorMax = transform.anchorMax;
                transform2.offsetMin = transform.offsetMin;
                transform2.offsetMax = transform.offsetMax;
                transform2.localPosition = transform.localPosition;
                transform2.localRotation = transform.localRotation;
                transform2.localScale = transform.localScale;
            }
            return obj2;
        }

        public void SetBelongedList(CUIListScript belongedListScript, int index)
        {
            this.m_belongedListScript = belongedListScript;
            this.m_indexInlist = index;
        }

        public virtual void SetSortingOrder(int sortingOrder)
        {
        }
    }
}

