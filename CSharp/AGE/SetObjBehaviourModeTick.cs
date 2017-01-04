namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SetObjBehaviourModeTick : TickEvent
    {
        public ObjBehaviMode Mode;

        public override BaseEvent Clone()
        {
            SetObjBehaviourModeTick tick = ClassObjPool<SetObjBehaviourModeTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SetObjBehaviourModeTick tick = src as SetObjBehaviourModeTick;
            this.Mode = tick.Mode;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.Mode = ObjBehaviMode.State_Idle;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            base.Process(_action, _track);
            PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
            if ((captain != 0) && (captain.handle.ActorControl != null))
            {
                captain.handle.ActorControl.SetObjBehaviMode(this.Mode);
            }
        }
    }
}

