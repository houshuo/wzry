namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("Utility")]
    public class DestroyObject : TickEvent
    {
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId = -1;

        public override BaseEvent Clone()
        {
            DestroyObject obj2 = ClassObjPool<DestroyObject>.Get();
            obj2.CopyData(this);
            return obj2;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            DestroyObject obj2 = src as DestroyObject;
            this.targetId = obj2.targetId;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            GameObject gameObject = _action.GetGameObject(this.targetId);
            if (gameObject != null)
            {
                ActionManager.Instance.DestroyObject(gameObject);
            }
        }
    }
}

