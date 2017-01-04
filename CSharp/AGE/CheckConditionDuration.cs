namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Skill")]
    public class CheckConditionDuration : DurationCondition
    {
        public bool bActorDead;
        private bool bCondition;
        public bool bHitTargetHero;
        public bool bTriggerBullet;
        private PoolObjHandle<ActorRoot> targetActor;
        [ObjectTemplate(new Type[] {  })]
        public int targetId;
        public int trackId = -1;

        public override bool Check(AGE.Action _action, Track _track)
        {
            return this.bCondition;
        }

        public override BaseEvent Clone()
        {
            CheckConditionDuration duration = ClassObjPool<CheckConditionDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            CheckConditionDuration duration = src as CheckConditionDuration;
            this.trackId = duration.trackId;
            this.bHitTargetHero = duration.bHitTargetHero;
            this.bTriggerBullet = duration.bTriggerBullet;
            this.bActorDead = duration.bActorDead;
            this.targetId = duration.targetId;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            if (this.bActorDead)
            {
                this.targetActor = _action.GetActorHandle(this.targetId);
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.trackId = -1;
            this.bCondition = false;
            this.targetActor.Release();
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            if (this.bHitTargetHero)
            {
                _action.refParams.GetRefParam("_HitTargetHero", ref this.bCondition);
            }
            else if (this.bTriggerBullet)
            {
                _action.refParams.GetRefParam("_TriggerBullet", ref this.bCondition);
            }
            else if ((this.bActorDead && (this.targetActor != 0)) && this.targetActor.handle.ActorControl.IsDeadState)
            {
                this.bCondition = true;
            }
            base.Process(_action, _track, _localTime);
        }
    }
}

