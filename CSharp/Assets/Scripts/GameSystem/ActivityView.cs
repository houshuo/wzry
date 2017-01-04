namespace Assets.Scripts.GameSystem
{
    using System;
    using UnityEngine;

    public abstract class ActivityView
    {
        private Activity _activity;
        private ActivityForm _form;
        private GameObject _root;
        protected ListView<ActivityWidget> _widgetList;

        public ActivityView(GameObject node, ActivityForm actvform, Activity actv)
        {
            this._root = node;
            this._form = actvform;
            this._activity = actv;
            this._widgetList = new ListView<ActivityWidget>();
        }

        public virtual void Clear()
        {
            for (int i = 0; i < this._widgetList.Count; i++)
            {
                this._widgetList[i].Clear();
            }
            this._widgetList.Clear();
        }

        public void ExcludeShow(ActivityWidget theOne)
        {
            for (int i = 0; i < this._widgetList.Count; i++)
            {
                this._widgetList[i].root.CustomSetActive(theOne == this._widgetList[i]);
            }
        }

        public void RestoreShow()
        {
            for (int i = 0; i < this._widgetList.Count; i++)
            {
                this._widgetList[i].root.CustomSetActive(true);
            }
        }

        public void Update()
        {
            for (int i = 0; i < this._widgetList.Count; i++)
            {
                this._widgetList[i].Update();
            }
        }

        public Activity activity
        {
            get
            {
                return this._activity;
            }
            protected set
            {
                this._activity = value;
            }
        }

        public ActivityForm form
        {
            get
            {
                return this._form;
            }
        }

        public GameObject root
        {
            get
            {
                return this._root;
            }
        }

        public int WidgetCount
        {
            get
            {
                return this._widgetList.Count;
            }
        }

        public ListView<ActivityWidget> WidgetList
        {
            get
            {
                return this._widgetList;
            }
        }
    }
}

