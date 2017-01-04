namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [EventCategory("MMGame/Drama")]
    public class SetPathVisibility : TickEvent
    {
        public bool enabled = true;
        public string[] excludeMeshes = new string[0];
        private Dictionary<string, bool> excludeMeshNames = new Dictionary<string, bool>();
        public string objPath = string.Empty;

        public override BaseEvent Clone()
        {
            SetPathVisibility visibility = ClassObjPool<SetPathVisibility>.Get();
            visibility.CopyData(this);
            return visibility;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SetPathVisibility visibility = src as SetPathVisibility;
            this.enabled = visibility.enabled;
            this.excludeMeshes = visibility.excludeMeshes;
            this.excludeMeshNames = visibility.excludeMeshNames;
            this.objPath = visibility.objPath;
        }

        public override void OnUse()
        {
            base.OnUse();
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            if (this.excludeMeshNames.Count != this.excludeMeshes.Length)
            {
                foreach (string str in this.excludeMeshes)
                {
                    string key = str;
                    this.excludeMeshNames.Add(key, true);
                }
            }
            GameObject obj2 = GameObject.Find(this.objPath);
            if (obj2 != null)
            {
                this.SetChild(obj2);
            }
        }

        private void SetChild(GameObject _obj)
        {
            string name = _obj.name;
            if (!this.excludeMeshNames.ContainsKey(name))
            {
                if (_obj.GetComponent<Renderer>() != null)
                {
                    _obj.GetComponent<Renderer>().enabled = this.enabled;
                }
                IEnumerator enumerator = _obj.transform.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        this.SetChild(current.gameObject);
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
            }
        }
    }
}

