namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("Utility")]
    public class FindGameObjectDuration : DurationEvent
    {
        public string objectName;
        [ObjectTemplate(true)]
        public int targetId = -1;

        public override BaseEvent Clone()
        {
            FindGameObjectDuration duration = ClassObjPool<FindGameObjectDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            FindGameObjectDuration duration = src as FindGameObjectDuration;
            this.targetId = duration.targetId;
            this.objectName = duration.objectName;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            if (this.targetId != -1)
            {
                _action.ExpandGameObject(this.targetId);
                if (string.IsNullOrEmpty(this.objectName))
                {
                    _action.SetGameObject(this.targetId, null);
                }
                else
                {
                    _action.SetGameObject(this.targetId, GameObject.Find(this.objectName));
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            if (this.targetId != -1)
            {
                _action.SetGameObject(this.targetId, null);
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.objectName = null;
        }
    }
}

