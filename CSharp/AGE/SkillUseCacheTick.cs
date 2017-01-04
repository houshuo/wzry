namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    internal class SkillUseCacheTick : TickEvent
    {
        [ObjectTemplate(new Type[] {  })]
        public int targetId = -1;

        public override BaseEvent Clone()
        {
            SkillUseCacheTick tick = ClassObjPool<SkillUseCacheTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SkillUseCacheTick tick = src as SkillUseCacheTick;
            this.targetId = tick.targetId;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            if (actorHandle != 0)
            {
                SkillComponent skillControl = actorHandle.handle.SkillControl;
                if ((skillControl != null) && (skillControl.SkillUseCache != null))
                {
                    skillControl.SkillUseCache.UseSkillCache(actorHandle);
                }
            }
        }
    }
}

