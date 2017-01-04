namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    public abstract class LogicComponent : PooledClassObject, IActorComponent
    {
        [HideInInspector]
        public ActorRoot actor;
        public PoolObjHandle<ActorRoot> actorPtr;

        protected LogicComponent()
        {
        }

        public virtual void Born(ActorRoot owner)
        {
            this.actor = owner;
            this.actorPtr = new PoolObjHandle<ActorRoot>(this.actor);
        }

        public virtual void Deactive()
        {
        }

        public virtual void Fight()
        {
        }

        public virtual void FightOver()
        {
        }

        public PoolObjHandle<ActorRoot> GetActor()
        {
            return this.actorPtr;
        }

        public virtual void Init()
        {
        }

        public virtual void LateUpdate(int delta)
        {
        }

        public override void OnRelease()
        {
            this.actor = null;
            this.actorPtr.Release();
            base.OnRelease();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.actor = null;
            this.actorPtr.Release();
        }

        public virtual void Prepare()
        {
        }

        public virtual void Reactive()
        {
            this.actorPtr.Validate();
        }

        public virtual void Uninit()
        {
        }

        public virtual void UpdateLogic(int delta)
        {
        }

        public VInt3 actorLocation
        {
            get
            {
                return this.actor.location;
            }
        }

        public GameObject gameObject
        {
            get
            {
                return this.actor.gameObject;
            }
        }
    }
}

