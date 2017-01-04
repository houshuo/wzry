namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Skill")]
    public class ChangeAnimDuration : DurationCondition
    {
        private PoolObjHandle<ActorRoot> actorHandle;
        public bool bOnlyRecover;
        public string changedAnimName;
        private bool isDone;
        public string originalAnimName;
        [ObjectTemplate(new Type[] {  })]
        public int targetId;

        public override bool Check(AGE.Action _action, Track _track)
        {
            return this.isDone;
        }

        public override BaseEvent Clone()
        {
            ChangeAnimDuration duration = ClassObjPool<ChangeAnimDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            ChangeAnimDuration duration = src as ChangeAnimDuration;
            this.targetId = duration.targetId;
            this.actorHandle = duration.actorHandle;
            this.originalAnimName = duration.originalAnimName;
            this.changedAnimName = duration.changedAnimName;
            this.bOnlyRecover = duration.bOnlyRecover;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.Init(_action, _track);
        }

        private void Init(AGE.Action _action, Track _track)
        {
            this.actorHandle = _action.GetActorHandle(this.targetId);
            if (this.actorHandle == 0)
            {
                this.isDone = true;
            }
            else if (!this.bOnlyRecover)
            {
                this.actorHandle.handle.AnimControl.ChangeAnimParam(this.originalAnimName, this.changedAnimName);
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            this.isDone = true;
            if (this.actorHandle != 0)
            {
                this.actorHandle.handle.AnimControl.RecoverAnimParam(this.originalAnimName);
            }
            base.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.actorHandle.Release();
            this.originalAnimName = string.Empty;
            this.changedAnimName = string.Empty;
            this.isDone = false;
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            base.Process(_action, _track, _localTime);
        }
    }
}

