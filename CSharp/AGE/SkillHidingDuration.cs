namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using CSProtocol;
    using System;

    [EventCategory("MMGame/Skill")]
    internal class SkillHidingDuration : DurationEvent
    {
        [AssetReference(AssetRefType.SkillCombine)]
        public int skillCombineID;
        private PoolObjHandle<ActorRoot> targetActor;
        [ObjectTemplate(new Type[] {  })]
        public int targetId;

        public override BaseEvent Clone()
        {
            SkillHidingDuration duration = ClassObjPool<SkillHidingDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SkillHidingDuration duration = src as SkillHidingDuration;
            this.targetId = duration.targetId;
            this.skillCombineID = duration.skillCombineID;
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            this.targetActor = _action.GetActorHandle(this.targetId);
            if (this.targetActor != 0)
            {
                this.targetActor.handle.HorizonMarker.AddHideMark(COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT, HorizonConfig.HideMark.Skill, 1);
                this.targetActor.handle.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Skill, true);
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            if (this.targetActor != 0)
            {
                COM_PLAYERCAMP[] othersCmp = BattleLogic.GetOthersCmp(this.targetActor.handle.TheActorMeta.ActorCamp);
                for (int i = 0; i < othersCmp.Length; i++)
                {
                    if (this.targetActor.handle.HorizonMarker.HasHideMark(othersCmp[i], HorizonConfig.HideMark.Skill))
                    {
                        this.targetActor.handle.HorizonMarker.AddHideMark(othersCmp[i], HorizonConfig.HideMark.Skill, -1);
                    }
                }
                this.targetActor.handle.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Skill, false);
                if (this.skillCombineID != 0)
                {
                    SkillUseParam inParam = new SkillUseParam();
                    inParam.SetOriginator(this.targetActor);
                    this.targetActor.handle.SkillControl.SpawnBuff(this.targetActor, ref inParam, this.skillCombineID, true);
                }
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetActor.Release();
        }
    }
}

