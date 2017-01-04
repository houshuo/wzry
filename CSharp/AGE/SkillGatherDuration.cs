namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SkillGatherDuration : DurationEvent
    {
        private PoolObjHandle<ActorRoot> actorObj;
        public bool bGatherTime;
        private int lastTime;
        [ObjectTemplate(new Type[] {  })]
        public int targetId;
        public int triggerRadius;
        public int triggerTime;

        public override BaseEvent Clone()
        {
            SkillGatherDuration duration = ClassObjPool<SkillGatherDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SkillGatherDuration duration = src as SkillGatherDuration;
            this.targetId = duration.targetId;
            this.triggerTime = duration.triggerTime;
            this.triggerRadius = duration.triggerRadius;
            this.bGatherTime = duration.bGatherTime;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.actorObj = _action.GetActorHandle(this.targetId);
            this.lastTime = 0;
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            this.lastTime = _action.CurrentTime;
            this.TriggerBullet();
            base.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = 0;
            this.lastTime = 0;
            this.actorObj.Release();
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            base.Process(_action, _track, _localTime);
        }

        public override bool SupportEditMode()
        {
            return true;
        }

        private void TriggerBullet()
        {
            BulletSkill skill = null;
            if (this.actorObj != 0)
            {
                int count = this.actorObj.handle.SkillControl.SpawnedBullets.Count;
                for (int i = 0; i < count; i++)
                {
                    skill = this.actorObj.handle.SkillControl.SpawnedBullets[i];
                    if ((skill != null) && (skill.CurAction != 0))
                    {
                        skill.CurAction.handle.refParams.SetRefParam("_TriggerBullet", true);
                        if (this.bGatherTime)
                        {
                            SkillUseContext refParamObject = skill.CurAction.handle.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
                            if (refParamObject != null)
                            {
                                refParamObject.GatherTime = this.lastTime / 0x3e8;
                                if (refParamObject.GatherTime <= 0)
                                {
                                    refParamObject.GatherTime = 1;
                                }
                                skill.lifeTime = this.triggerTime * refParamObject.GatherTime;
                            }
                        }
                    }
                }
            }
        }
    }
}

