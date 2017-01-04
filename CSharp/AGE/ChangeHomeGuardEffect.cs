namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameSystem;
    using System;

    [EventCategory("MMGame/Skill")]
    public class ChangeHomeGuardEffect : TickCondition
    {
        private bool bCheck = true;
        public bool bGuildMaxGrade;
        public bool bNormal;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId = -1;

        public override bool Check(AGE.Action _action, Track _track)
        {
            return this.bCheck;
        }

        public override BaseEvent Clone()
        {
            ChangeHomeGuardEffect effect = ClassObjPool<ChangeHomeGuardEffect>.Get();
            effect.CopyData(this);
            return effect;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            ChangeHomeGuardEffect effect = src as ChangeHomeGuardEffect;
            this.targetId = effect.targetId;
            this.bNormal = effect.bNormal;
            this.bGuildMaxGrade = effect.bGuildMaxGrade;
            this.bCheck = effect.bCheck;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.bNormal = false;
            this.bGuildMaxGrade = false;
            this.bCheck = true;
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
                if (this.bNormal)
                {
                    this.bCheck = !(CGuildSystem.s_isGuildMaxGrade && ActorHelper.IsHostActor(ref actorHandle));
                }
                else
                {
                    this.bCheck = CGuildSystem.s_isGuildMaxGrade && ActorHelper.IsHostActor(ref actorHandle);
                }
                base.Process(_action, _track);
            }
        }
    }
}

