namespace AGE
{
    using Assets.Scripts.Common;
    using System;

    [EventCategory("MMGame/Drama")]
    public class SetInvincibilityTick : TickEvent
    {
        [ObjectTemplate(new Type[] {  })]
        public int atkerId = 1;
        public bool bInvincible;
        [ObjectTemplate(new Type[] {  })]
        public int srcId;
        [ObjectTemplate(new Type[] {  })]
        public int targetId = 2;

        public override BaseEvent Clone()
        {
            SetInvincibilityTick tick = ClassObjPool<SetInvincibilityTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SetInvincibilityTick tick = src as SetInvincibilityTick;
            this.bInvincible = tick.bInvincible;
            this.srcId = tick.srcId;
            this.atkerId = tick.atkerId;
            this.targetId = tick.targetId;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
            if (actorHandle != 0)
            {
                actorHandle.handle.ObjLinker.Invincible = this.bInvincible;
            }
        }
    }
}

