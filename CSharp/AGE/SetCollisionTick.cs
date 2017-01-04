namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using System;

    [EventCategory("MMGame/Skill")]
    public class SetCollisionTick : TickEvent
    {
        private PoolObjHandle<ActorRoot> actor;
        public VInt3 Pos = VInt3.zero;
        public int Radius = 0x3e8;
        public VInt3 Size = VInt3.one;
        [ObjectTemplate(new System.Type[] {  })]
        public int targetId = -1;
        public ColliderType type;

        public override BaseEvent Clone()
        {
            SetCollisionTick tick = ClassObjPool<SetCollisionTick>.Get();
            tick.CopyData(this);
            return tick;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SetCollisionTick tick = src as SetCollisionTick;
            this.targetId = tick.targetId;
            this.type = tick.type;
            this.Pos = tick.Pos;
            this.Size = tick.Size;
            this.Radius = tick.Radius;
        }

        private static T GetCollisionShape<T>(ActorRoot actorRoot, CollisionShapeType shapeType) where T: VCollisionShape, new()
        {
            if ((actorRoot.shape != null) && (actorRoot.shape.GetShapeType() == shapeType))
            {
                return (actorRoot.shape as T);
            }
            T local = Activator.CreateInstance<T>();
            local.Born(actorRoot);
            return local;
        }

        public override void OnRelease()
        {
            base.OnRelease();
            this.actor.Release();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.targetId = -1;
            this.type = ColliderType.Box;
            this.Pos = VInt3.zero;
            this.Size = VInt3.one;
            this.Radius = 0x3e8;
        }

        public override void Process(AGE.Action _action, Track _track)
        {
            this.actor = _action.GetActorHandle(this.targetId);
            if (this.actor == 0)
            {
                if (ActionManager.Instance.isPrintLog)
                {
                }
            }
            else
            {
                ActorRoot handle = this.actor.handle;
                if (this.type == ColliderType.Box)
                {
                    VCollisionBox collisionShape = GetCollisionShape<VCollisionBox>(handle, CollisionShapeType.Box);
                    collisionShape.Pos = this.Pos;
                    collisionShape.Size = this.Size;
                    collisionShape.dirty = true;
                    collisionShape.ConditionalUpdateShape();
                }
                else if (this.type == ColliderType.Sphere)
                {
                    VCollisionSphere sphere = GetCollisionShape<VCollisionSphere>(handle, CollisionShapeType.Sphere);
                    sphere.Pos = this.Pos;
                    sphere.Radius = this.Radius;
                    sphere.dirty = true;
                    sphere.ConditionalUpdateShape();
                }
            }
        }

        public override void ProcessBlend(AGE.Action _action, Track _track, TickEvent _prevEvent, float _blendWeight)
        {
        }

        public enum ColliderType
        {
            Box,
            Sphere
        }
    }
}

