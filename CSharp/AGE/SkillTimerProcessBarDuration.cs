namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SkillTimerProcessBarDuration : DurationEvent
    {
        private PoolObjHandle<ActorRoot> actorObj;
        private ulong starTime;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId;
        private int totalTime;

        public override BaseEvent Clone()
        {
            SkillTimerProcessBarDuration duration = ClassObjPool<SkillTimerProcessBarDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SkillTimerProcessBarDuration duration = src as SkillTimerProcessBarDuration;
            this.targetId = duration.targetId;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.actorObj = _action.GetActorHandle(this.targetId);
            this.starTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
            this.totalTime = base.length;
            if ((this.actorObj != 0) && (this.totalTime > 0))
            {
                SkillTimerEventParam param = new SkillTimerEventParam(this.totalTime, this.starTime, this.actorObj);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<SkillTimerEventParam>(GameSkillEventDef.AllEvent_SetSkillTimer, this.actorObj, ref param, GameSkillEventChannel.Channel_AllActor);
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            base.Leave(_action, _track);
            if (this.actorObj != 0)
            {
                SkillTimerEventParam param = new SkillTimerEventParam(0, 0L, this.actorObj);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<SkillTimerEventParam>(GameSkillEventDef.AllEvent_SetSkillTimer, this.actorObj, ref param, GameSkillEventChannel.Channel_AllActor);
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.starTime = 0L;
            this.targetId = 0;
            this.totalTime = 0;
            this.actorObj.Release();
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

