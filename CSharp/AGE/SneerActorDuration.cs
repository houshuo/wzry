namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SneerActorDuration : DurationEvent
    {
        private PoolObjHandle<ActorRoot> actorObj;
        [ObjectTemplate(new System.Type[] {  })]
        public int attackId = -1;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId = -1;

        public override BaseEvent Clone()
        {
            SneerActorDuration duration = ClassObjPool<SneerActorDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SneerActorDuration duration = src as SneerActorDuration;
            this.attackId = duration.attackId;
            this.targetId = duration.targetId;
            this.actorObj = duration.actorObj;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.actorObj = _action.GetActorHandle(this.targetId);
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.attackId);
            if ((this.actorObj != 0) && (actorHandle != 0))
            {
                ObjWrapper actorControl = this.actorObj.handle.ActorControl;
                if ((actorControl != null) && !actorControl.IsDeadState)
                {
                    actorControl.TerminateMove();
                    actorControl.ClearMoveCommand();
                    actorControl.ForceAbortCurUseSkill();
                    actorControl.SetOutOfControl(true, OutOfControlType.Taunt);
                    actorControl.SetTauntTarget(actorHandle);
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            base.Leave(_action, _track);
            if (this.actorObj != 0)
            {
                ObjWrapper actorControl = this.actorObj.handle.ActorControl;
                if (actorControl != null)
                {
                    actorControl.SetOutOfControl(false, OutOfControlType.Taunt);
                }
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.attackId = -1;
            this.targetId = -1;
            this.actorObj.Release();
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            base.Process(_action, _track, _localTime);
        }
    }
}

