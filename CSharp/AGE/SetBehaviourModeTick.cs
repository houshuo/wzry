namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SetBehaviourModeTick : TickEvent
    {
        public bool clearMove;
        public bool delayStopCurSkill;
        public bool stopCurSkill;
        public bool stopMove = true;
        [ObjectTemplate(new Type[] {  })]
        public int targetId = -1;

        public override BaseEvent Clone()
        {
            SetBehaviourModeTick tick = ClassObjPool<SetBehaviourModeTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SetBehaviourModeTick tick = src as SetBehaviourModeTick;
            this.targetId = tick.targetId;
            this.stopMove = tick.stopMove;
            this.clearMove = tick.clearMove;
            this.stopCurSkill = tick.stopCurSkill;
            this.delayStopCurSkill = tick.delayStopCurSkill;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.stopMove = true;
            this.clearMove = false;
            this.stopCurSkill = false;
            this.delayStopCurSkill = false;
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
            else
            {
                ObjWrapper actorControl = actorHandle.handle.ActorControl;
                if (actorControl == null)
                {
                    if (ActionManager.Instance.isPrintLog)
                    {
                    }
                }
                else
                {
                    if (this.stopMove)
                    {
                        actorControl.TerminateMove();
                    }
                    if (this.clearMove)
                    {
                        actorControl.ClearMoveCommand();
                    }
                    if (this.stopCurSkill)
                    {
                        actorControl.ForceAbortCurUseSkill();
                    }
                    if (this.delayStopCurSkill && !this.stopCurSkill)
                    {
                        actorControl.DelayAbortCurUseSkill();
                    }
                }
            }
        }
    }
}

