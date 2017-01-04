namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("Utility")]
    public class SetObjActiveTick : TickEvent
    {
        public bool enabled = true;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId;

        public override BaseEvent Clone()
        {
            SetObjActiveTick tick = ClassObjPool<SetObjActiveTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SetObjActiveTick tick = src as SetObjActiveTick;
            this.enabled = tick.enabled;
            this.targetId = tick.targetId;
        }

        public override void OnUse()
        {
            base.OnUse();
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            GameObject gameObject = _action.GetGameObject(this.targetId);
            if (gameObject != null)
            {
                gameObject.SetActive(this.enabled);
            }
        }
    }
}

