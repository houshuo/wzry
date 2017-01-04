namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    internal class FocusCameraDuration : DurationEvent
    {
        private PoolObjHandle<ActorRoot> targetActor;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId;

        public override BaseEvent Clone()
        {
            FocusCameraDuration duration = ClassObjPool<FocusCameraDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            FocusCameraDuration duration = src as FocusCameraDuration;
            this.targetId = duration.targetId;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            this.targetActor = _action.GetActorHandle(this.targetId);
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            if ((this.targetActor != 0) && ActorHelper.IsHostCtrlActor(ref this.targetActor))
            {
                MonoSingleton<CameraSystem>.instance.SetFocusActor(this.targetActor);
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetActor.Release();
        }
    }
}

