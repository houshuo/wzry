namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    internal class SkillInputCacheDuration : DurationEvent
    {
        public bool cacheMove;
        public bool cacheSkill;
        private SkillComponent skillControl;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId;

        public override BaseEvent Clone()
        {
            SkillInputCacheDuration duration = ClassObjPool<SkillInputCacheDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SkillInputCacheDuration duration = src as SkillInputCacheDuration;
            this.targetId = duration.targetId;
            this.cacheSkill = duration.cacheSkill;
            this.cacheMove = duration.cacheMove;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            if (actorHandle != 0)
            {
                this.skillControl = actorHandle.handle.SkillControl;
                if ((this.skillControl != null) && (this.skillControl.SkillUseCache != null))
                {
                    if (this.cacheSkill)
                    {
                        this.skillControl.SkillUseCache.SetCacheSkill(true);
                    }
                    if (this.cacheMove)
                    {
                        this.skillControl.SkillUseCache.SetCacheMove(true);
                        DefaultGameEventParam prm = new DefaultGameEventParam(actorHandle, actorHandle);
                        Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorClearMove, ref prm);
                    }
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            if (this.skillControl != null)
            {
                if (this.skillControl.SkillUseCache != null)
                {
                    if (this.cacheSkill)
                    {
                        this.skillControl.SkillUseCache.SetCacheSkill(false);
                    }
                    if (this.cacheMove)
                    {
                        this.skillControl.SkillUseCache.SetCacheMove(false);
                    }
                }
                this.skillControl = null;
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.skillControl = null;
        }
    }
}

