namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SpawnBulletTick : TickEvent
    {
        [AssetReference(AssetRefType.Action)]
        public string ActionName;
        public bool bAgeImmeExcute;
        public bool bDeadRemove;
        [AssetReference(AssetRefType.Action)]
        public string SpecialActionName = string.Empty;
        [ObjectTemplate(new Type[] {  })]
        public int targetId = -1;

        public override BaseEvent Clone()
        {
            SpawnBulletTick tick = ClassObjPool<SpawnBulletTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SpawnBulletTick tick = src as SpawnBulletTick;
            this.targetId = tick.targetId;
            this.ActionName = tick.ActionName;
            this.SpecialActionName = tick.SpecialActionName;
            this.bDeadRemove = tick.bDeadRemove;
            this.bAgeImmeExcute = tick.bAgeImmeExcute;
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
                SkillComponent skillControl = actorHandle.handle.SkillControl;
                if (skillControl == null)
                {
                    if (ActionManager.Instance.isPrintLog)
                    {
                    }
                }
                else
                {
                    SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
                    if (refParamObject != null)
                    {
                        refParamObject.BulletPos = refParamObject.UseVector;
                        if (!refParamObject.bSpecialUse)
                        {
                            if (this.ActionName != string.Empty)
                            {
                                skillControl.SpawnBullet(refParamObject, this.ActionName, this.bDeadRemove, this.bAgeImmeExcute);
                            }
                        }
                        else if (this.SpecialActionName != string.Empty)
                        {
                            skillControl.SpawnBullet(refParamObject, this.SpecialActionName, this.bDeadRemove, this.bAgeImmeExcute);
                        }
                    }
                }
            }
        }
    }
}

