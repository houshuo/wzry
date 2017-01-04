namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Drama")]
    public class SceneInterpolationTick : TickEvent
    {
        public float fadeTime = 2f;
        [ObjectTemplate(true)]
        public int targetId = -1;

        public override BaseEvent Clone()
        {
            SceneInterpolationTick tick = ClassObjPool<SceneInterpolationTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SceneInterpolationTick tick = src as SceneInterpolationTick;
            this.targetId = tick.targetId;
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
                SceneInterpolation component = gameObject.GetComponent<SceneInterpolation>();
                if (component == null)
                {
                    component = gameObject.AddComponent<SceneInterpolation>();
                }
                component.FadeTime = this.fadeTime;
                component.Play();
            }
        }
    }
}

