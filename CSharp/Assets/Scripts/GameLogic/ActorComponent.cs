namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    public abstract class ActorComponent : BaseComponent, IPooledMonoBehaviour, IActorComponent
    {
        [HideInInspector]
        public ActorRoot actor;
        public PoolObjHandle<ActorRoot> actorPtr;

        protected ActorComponent()
        {
        }

        public virtual void Born(ActorRoot owner)
        {
            this.actor = owner;
            this.actorPtr = new PoolObjHandle<ActorRoot>(this.actor);
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

        public virtual void OnCreate()
        {
        }

        public virtual void OnGet()
        {
            this.actor = null;
            this.actorPtr.Release();
        }

        public virtual void OnRecycle()
        {
            this.actor = null;
            this.actorPtr.Release();
        }

        public virtual void Prepare()
        {
        }

        protected override void Start()
        {
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
    }
}

