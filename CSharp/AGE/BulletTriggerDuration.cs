namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Skill")]
    public class BulletTriggerDuration : DurationCondition
    {
        public int acceleration;
        [ObjectTemplate(new System.Type[] {  })]
        public int attackerId;
        public bool bAdjustSpeed;
        public bool bAgeImmeExcute;
        public bool bBulletUseDir;
        public bool bEdgeCheck;
        public bool bExtraBuff;
        public bool bFileterMonter;
        public bool bFileterOrgan;
        public bool bFilterDead = true;
        public bool bFilterEnemy;
        public bool bFilterFriend = true;
        public bool bFilterHero;
        public bool bFilterMyself = true;
        public bool bMoveRotate = true;
        public bool bTriggerBullet;
        [AssetReference(AssetRefType.Action)]
        public string BulletActionName;
        public int CollideMaxCount = -1;
        public EDependCheckType DependCheckType;
        [ObjectTemplate(new System.Type[] {  })]
        public int destId = -1;
        public int distance = 0xc350;
        public int gravity;
        private HitTriggerDurationContext HitTriggerContext = new HitTriggerDurationContext();
        private MoveBulletDurationContext MoveBulletContext = new MoveBulletDurationContext();
        public ActorMoveType MoveType;
        public VInt3 offsetDir = VInt3.zero;
        public HitTriggerSelectMode SelectMode;
        [AssetReference(AssetRefType.SkillCombine)]
        public int SelfSkillCombineID_1;
        [AssetReference(AssetRefType.SkillCombine)]
        public int SelfSkillCombineID_2;
        [AssetReference(AssetRefType.SkillCombine)]
        public int SelfSkillCombineID_3;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId;
        public VInt3 targetPosition;
        [AssetReference(AssetRefType.SkillCombine)]
        public int TargetSkillCombine_1;
        [AssetReference(AssetRefType.SkillCombine)]
        public int TargetSkillCombine_2;
        [AssetReference(AssetRefType.SkillCombine)]
        public int TargetSkillCombine_3;
        public int TriggerActorCount = -1;
        public int TriggerActorInterval = 30;
        [ObjectTemplate(new System.Type[] {  })]
        public int triggerId;
        public int triggerInterval = 30;
        public int velocity = 0x3a98;

        public override bool Check(AGE.Action _action, Track _track)
        {
            switch (this.DependCheckType)
            {
                case EDependCheckType.Hit:
                    return this.HitTriggerContext.hit;

                case EDependCheckType.Move:
                    return this.MoveBulletContext.stopCondtion;

                case EDependCheckType.HitAndMove:
                    return (this.HitTriggerContext.hit && this.MoveBulletContext.stopCondtion);

                case EDependCheckType.HitOrMove:
                    return (this.HitTriggerContext.hit || this.MoveBulletContext.stopCondtion);
            }
            return this.HitTriggerContext.hit;
        }

        public override BaseEvent Clone()
        {
            BulletTriggerDuration duration = ClassObjPool<BulletTriggerDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            BulletTriggerDuration duration = src as BulletTriggerDuration;
            this.triggerId = duration.triggerId;
            this.attackerId = duration.attackerId;
            this.triggerInterval = duration.triggerInterval;
            this.bFilterEnemy = duration.bFilterEnemy;
            this.bFilterFriend = duration.bFilterFriend;
            this.bFilterHero = duration.bFilterHero;
            this.bFileterMonter = duration.bFileterMonter;
            this.bFileterOrgan = duration.bFileterOrgan;
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
            this.targetId = duration.targetId;
            this.destId = duration.destId;
            this.MoveType = duration.MoveType;
            this.targetPosition = duration.targetPosition;
            this.offsetDir = duration.offsetDir;
            this.velocity = duration.velocity;
            this.distance = duration.distance;
            this.gravity = duration.gravity;
            this.bMoveRotate = duration.bMoveRotate;
            this.bAdjustSpeed = duration.bAdjustSpeed;
            this.bBulletUseDir = duration.bBulletUseDir;
            this.acceleration = duration.acceleration;
            this.DependCheckType = duration.DependCheckType;
            this.HitTriggerContext.CopyData(ref duration.HitTriggerContext);
            this.MoveBulletContext.CopyData(ref duration.MoveBulletContext);
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            this.MoveBulletContext.Reset(this);
            this.HitTriggerContext.Reset(this);
            this.MoveBulletContext.Enter(_action);
            this.HitTriggerContext.Enter(_action, _track);
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            base.Leave(_action, _track);
            this.MoveBulletContext.Leave(_action, _track);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.HitTriggerContext.OnUse();
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            int num = 2;
            while (!this.ShouldStop() && (this.MoveBulletContext.ProcessSubdivide(_action, _track, _localTime, num--) > 0))
            {
                this.HitTriggerContext.Process(_action, _track, this.MoveBulletContext.lastTime);
            }
            base.Process(_action, _track, _localTime);
        }

        private bool ShouldStop()
        {
            bool hit = false;
            if (this.DependCheckType == EDependCheckType.Hit)
            {
                hit = this.HitTriggerContext.hit;
            }
            else if (this.DependCheckType == EDependCheckType.Move)
            {
                hit = this.MoveBulletContext.stopCondtion;
            }
            else if (this.DependCheckType == EDependCheckType.HitOrMove)
            {
                hit = this.HitTriggerContext.hit || this.MoveBulletContext.stopCondtion;
            }
            else if (this.DependCheckType == EDependCheckType.HitAndMove)
            {
                hit = this.HitTriggerContext.hit && this.MoveBulletContext.stopCondtion;
            }
            if (hit)
            {
                this.MoveBulletContext.stopLerpCondtion = true;
            }
            return hit;
        }
    }
}

