namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Drama")]
    public class ShowImageDuration : DurationEvent
    {
        [ObjectTemplate(new System.Type[] {  })]
        public int atkerId;
        [ObjectTemplate(new System.Type[] {  })]
        public int srcId;

        public override BaseEvent Clone()
        {
            ShowImageDuration duration = ClassObjPool<ShowImageDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            ShowImageDuration duration = src as ShowImageDuration;
            this.srcId = duration.srcId;
            this.atkerId = duration.atkerId;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            GameObject gameObject = _action.GetGameObject(this.srcId);
            GameObject obj3 = _action.GetGameObject(this.atkerId);
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            GameObject gameObject = _action.GetGameObject(this.srcId);
            GameObject obj3 = _action.GetGameObject(this.atkerId);
            base.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.srcId = 0;
            this.atkerId = 0;
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

