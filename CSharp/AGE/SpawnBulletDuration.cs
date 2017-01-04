namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SpawnBulletDuration : DurationEvent
    {
        [AssetReference(AssetRefType.Action)]
        public string ActionName;
        public bool bAgeImmeExcute;
        public bool bDeadRemove;
        public bool bRandom = true;
        private int deltaTime;
        private int lastTime;
        private SkillComponent skillControl;
        public int spawnFreq = 0x3e8;
        public int spawnMax = 1;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId = -1;
        public VInt3[] transArray = new VInt3[50];

        public override BaseEvent Clone()
        {
            SpawnBulletDuration duration = ClassObjPool<SpawnBulletDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SpawnBulletDuration duration = src as SpawnBulletDuration;
            this.targetId = duration.targetId;
            this.ActionName = duration.ActionName;
            this.spawnMax = duration.spawnMax;
            this.spawnFreq = duration.spawnFreq;
            this.bRandom = duration.bRandom;
            this.skillControl = duration.skillControl;
            this.lastTime = duration.lastTime;
            this.deltaTime = duration.deltaTime;
            this.bDeadRemove = duration.bDeadRemove;
            this.bAgeImmeExcute = duration.bAgeImmeExcute;
            Array.Resize<VInt3>(ref this.transArray, duration.transArray.Length);
            for (int i = 0; i < duration.transArray.Length; i++)
            {
                this.transArray[i] = duration.transArray[i];
            }
        }

        public override void Enter(AGE.Action _action, Track _track)
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
                this.skillControl = actorHandle.handle.SkillControl;
                if (this.skillControl == null)
                {
                    if (ActionManager.Instance.isPrintLog)
                    {
                    }
                }
                else
                {
                    this.SpawnBullet(_action);
                }
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.ActionName = string.Empty;
            this.spawnMax = 1;
            this.spawnFreq = 0x3e8;
            this.bRandom = true;
            this.skillControl = null;
            this.lastTime = 0;
            this.deltaTime = 0;
            this.bDeadRemove = false;
        }

        public override void Process(AGE.Action _action, Track _track, int _localTime)
        {
            this.deltaTime += _localTime - this.lastTime;
            this.lastTime = _localTime;
            if (this.deltaTime > this.spawnFreq)
            {
                this.SpawnBullet(_action);
                this.deltaTime -= this.spawnFreq;
            }
        }

        private void SpawnBullet(AGE.Action _action)
        {
            SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
            for (int i = 0; i < this.spawnMax; i++)
            {
                int index = 0;
                VInt3 zero = VInt3.zero;
                if (this.transArray.Length < 0)
                {
                    zero = VInt3.zero;
                }
                else
                {
                    if (this.bRandom)
                    {
                        index = FrameRandom.Random((uint) this.transArray.Length);
                    }
                    else
                    {
                        index = i % this.transArray.Length;
                    }
                    zero = this.transArray[index];
                }
                refParamObject.BulletPos = zero;
                PoolObjHandle<BulletSkill> handle = this.skillControl.SpawnBullet(refParamObject, this.ActionName, this.bDeadRemove, this.bAgeImmeExcute);
            }
        }
    }
}

