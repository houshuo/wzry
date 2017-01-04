namespace AGE
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Drama")]
    public class TriigerUIAddPartical : TickEvent
    {
        public string particleName = string.Empty;
        public float playTime;
        public Vector3 screenPos = new Vector3(0f, 0f, 0f);

        public override BaseEvent Clone()
        {
            TriigerUIAddPartical partical = ClassObjPool<TriigerUIAddPartical>.Get();
            partical.CopyData(this);
            return partical;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            TriigerUIAddPartical partical = src as TriigerUIAddPartical;
            this.particleName = partical.particleName;
            this.screenPos = partical.screenPos;
            this.playTime = partical.playTime;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            base.Process(_action, _track);
            Singleton<CUIParticleSystem>.GetInstance().AddParticle(this.particleName, this.playTime, new Vector2(this.screenPos.x, this.screenPos.y));
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

