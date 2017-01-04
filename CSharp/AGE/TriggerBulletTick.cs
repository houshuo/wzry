namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class TriggerBulletTick : TickEvent
    {
        private PoolObjHandle<ActorRoot> targetActor;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId = -1;
        public int triggerRadius;

        public override BaseEvent Clone()
        {
            TriggerBulletTick tick = ClassObjPool<TriggerBulletTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            TriggerBulletTick tick = src as TriggerBulletTick;
            this.targetId = tick.targetId;
            this.triggerRadius = tick.triggerRadius;
        }

        public override void OnRelease()
        {
            base.OnRelease();
            this.targetActor.Release();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            this.targetActor = _action.GetActorHandle(this.targetId);
            if (this.targetActor != 0)
            {
                VInt3 zero = VInt3.zero;
                BulletSkill skill = null;
                int count = this.targetActor.handle.SkillControl.SpawnedBullets.Count;
                for (int i = 0; i < count; i++)
                {
                    skill = this.targetActor.handle.SkillControl.SpawnedBullets[i];
                    if ((skill != null) && (skill.CurAction != 0))
                    {
                        skill.CurAction.handle.refParams.GetRefParam("_BulletPos", ref zero);
                        this.TriggerBullet(zero, (AGE.Action) skill.CurAction);
                    }
                }
            }
        }

        private void TriggerBullet(VInt3 _bulletPos, AGE.Action _bulletAction)
        {
            VInt3 num2 = this.targetActor.handle.location - _bulletPos;
            if (num2.sqrMagnitudeLong2D <= (this.triggerRadius * this.triggerRadius))
            {
                _bulletAction.refParams.SetRefParam("_TriggerBullet", true);
            }
        }
    }
}

