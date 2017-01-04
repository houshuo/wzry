namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Skill")]
    public class HitTriggerDuration : DurationCondition
    {
        [ObjectTemplate(new Type[] {  })]
        public int attackerId;
        public bool bAgeImmeExcute;
        public bool bCheckSight;
        public bool bEdgeCheck;
        public bool bExtraBuff;
        public bool bFileterMonter;
        public bool bFileterOrgan;
        public bool bFilterDead = true;
        public bool bFilterEnemy;
        public bool bFilterEye = true;
        public bool bFilterFriend = true;
        public bool bFilterHero;
        public bool bFilterMyself = true;
        public bool bTriggerBullet;
        public bool bTriggerMode;
        [AssetReference(AssetRefType.Action)]
        public string BulletActionName;
        public bool bUseTriggerObj = true;
        public int CollideMaxCount = -1;
        private HitTriggerDurationContext Context = new HitTriggerDurationContext();
        public HitTriggerSelectMode SelectMode;
        [AssetReference(AssetRefType.SkillCombine)]
        public int SelfSkillCombineID_1;
        [AssetReference(AssetRefType.SkillCombine)]
        public int SelfSkillCombineID_2;
        [AssetReference(AssetRefType.SkillCombine)]
        public int SelfSkillCombineID_3;
        [AssetReference(AssetRefType.SkillCombine)]
        public int TargetSkillCombine_1;
        [AssetReference(AssetRefType.SkillCombine)]
        public int TargetSkillCombine_2;
        [AssetReference(AssetRefType.SkillCombine)]
        public int TargetSkillCombine_3;
        public int TriggerActorCount = -1;
        public int TriggerActorInterval = 30;
        [ObjectTemplate(new Type[] {  })]
        public int triggerId;
        public int triggerInterval = 30;

        public override bool Check(AGE.Action _action, Track _track)
        {
            return this.Context.hit;
        }

        public override BaseEvent Clone()
        {
            HitTriggerDuration duration = ClassObjPool<HitTriggerDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            HitTriggerDuration duration = src as HitTriggerDuration;
            this.triggerId = duration.triggerId;
            this.attackerId = duration.attackerId;
            this.triggerInterval = duration.triggerInterval;
            this.bFilterEnemy = duration.bFilterEnemy;
            this.bFilterFriend = duration.bFilterFriend;
            this.bFilterHero = duration.bFilterHero;
            this.bFileterMonter = duration.bFileterMonter;
            this.bFileterOrgan = duration.bFileterOrgan;
            this.bFilterEye = duration.bFilterEye;
            this.bFilterMyself = duration.bFilterMyself;
            this.bFilterDead = duration.bFilterDead;
            this.TriggerActorCount = duration.TriggerActorCount;
            this.SelectMode = duration.SelectMode;
            this.TriggerActorInterval = duration.TriggerActorInterval;
            this.CollideMaxCount = duration.CollideMaxCount;
            this.bEdgeCheck = duration.bEdgeCheck;
            this.bExtraBuff = duration.bExtraBuff;
            this.SelfSkillCombineID_1 = duration.SelfSkillCombineID_1;
            this.SelfSkillCombineID_2 = duration.SelfSkillCombineID_2;
            this.SelfSkillCombineID_3 = duration.SelfSkillCombineID_3;
            this.TargetSkillCombine_1 = duration.TargetSkillCombine_1;
            this.TargetSkillCombine_2 = duration.TargetSkillCombine_2;
            this.TargetSkillCombine_3 = duration.TargetSkillCombine_3;
            this.bTriggerBullet = duration.bTriggerBullet;
            this.BulletActionName = duration.BulletActionName;
            this.bAgeImmeExcute = duration.bAgeImmeExcute;
            this.bUseTriggerObj = duration.bUseTriggerObj;
            this.bCheckSight = duration.bCheckSight;
            this.bTriggerMode = duration.bTriggerMode;
            this.Context.CopyData(ref duration.Context);
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.Context.Reset(this);
            this.Context.Enter(_action, _track);
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            base.Leave(_action, _track);
            this.Context.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.bEdgeCheck = false;
            this.bTriggerMode = false;
            this.bFilterEye = true;
            this.Context.OnUse();
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            this.Context.Process(_action, _track, _localTime);
            base.Process(_action, _track, _localTime);
        }
    }
}

