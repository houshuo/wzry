namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("Utility")]
    public class SetTagLayer : TickEvent
    {
        public bool enableLayer;
        public bool enableTag;
        public int layer;
        public string tag = string.Empty;
        [ObjectTemplate(true)]
        public int targetId = -1;

        public override BaseEvent Clone()
        {
            SetTagLayer layer = ClassObjPool<SetTagLayer>.Get();
            layer.CopyData(this);
            return layer;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SetTagLayer layer = src as SetTagLayer;
            this.targetId = layer.targetId;
            this.enableLayer = layer.enableLayer;
            this.layer = layer.layer;
            this.enableTag = layer.enableTag;
            this.tag = layer.tag;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.enableLayer = false;
            this.layer = 0;
            this.enableTag = false;
            this.tag = string.Empty;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            GameObject gameObject = _action.GetGameObject(this.targetId);
            if (gameObject == null)
            {
                Debug.LogWarning("not find setting layer/tag target object");
            }
            else
            {
                if (this.enableLayer)
                {
                    gameObject.layer = this.layer;
                    Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
                    for (int i = 0; i < componentsInChildren.Length; i++)
                    {
                        componentsInChildren[i].gameObject.layer = this.layer;
                    }
                }
                if (this.enableTag)
                {
                    gameObject.tag = this.tag;
                    Transform[] transformArray2 = gameObject.GetComponentsInChildren<Transform>();
                    for (int j = 0; j < transformArray2.Length; j++)
                    {
                        transformArray2[j].gameObject.tag = this.tag;
                    }
                }
            }
        }
    }
}

