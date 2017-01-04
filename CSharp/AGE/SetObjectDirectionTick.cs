namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SetObjectDirectionTick : TickEvent
    {
        public VInt3 targetDir = VInt3.forward;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId = -1;

        public override BaseEvent Clone()
        {
            SetObjectDirectionTick tick = ClassObjPool<SetObjectDirectionTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SetObjectDirectionTick tick = src as SetObjectDirectionTick;
            this.targetId = tick.targetId;
            this.targetDir = tick.targetDir;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.targetDir = VInt3.forward;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            if (actorHandle == 0)
            {
                if (ActionManager.Instance.isPrintLog)
                {
                }
            }
            else if (this.targetDir.sqrMagnitudeLong < 1L)
            {
                if (ActionManager.Instance.isPrintLog)
                {
                }
            }
            else
            {
                actorHandle.handle.MovementComponent.SetRotate(this.targetDir, true);
            }
        }
    }
}

