namespace Apollo
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class ApolloObject
    {
        private float lastTime;
        private static ulong s_objectId = 1L;

        protected ApolloObject()
        {
            this.Reflectible = true;
            this.AcceptMonoBehaviour = false;
            this.Removable = false;
            this.UpdateTimeLeft = -1f;
            this.init();
        }

        protected ApolloObject(bool reflectible, bool acceptMonoBehaviour)
        {
            this.UpdateTimeLeft = -1f;
            this.Reflectible = reflectible;
            this.AcceptMonoBehaviour = acceptMonoBehaviour;
            this.init();
        }

        public void Destroy()
        {
            this.Removable = true;
        }

        ~ApolloObject()
        {
            this.Destroy();
        }

        private void init()
        {
            if (this.Reflectible)
            {
                s_objectId += (ulong) 1L;
                this.ObjectId = s_objectId;
                if (s_objectId == 0)
                {
                    s_objectId = 1L;
                }
                ApolloObjectManager.Instance.AddObject(this);
            }
            if (this.AcceptMonoBehaviour)
            {
                ApolloObjectManager.Instance.AddAcceptUpdatedObject(this);
            }
        }

        public virtual void OnApplicationPause(bool pauseStatus)
        {
        }

        public virtual void OnApplicationQuit()
        {
        }

        public virtual void OnDisable()
        {
        }

        protected virtual void OnTimeOut()
        {
        }

        protected virtual void OnUpdate(float deltaTime)
        {
        }

        public virtual void Update()
        {
            float time = Time.time;
            float deltaTime = time - this.lastTime;
            this.lastTime = time;
            this.OnUpdate(deltaTime);
            if (this.UpdateTimeLeft > 0f)
            {
                this.UpdateTimeLeft -= deltaTime;
                if (this.UpdateTimeLeft <= 0f)
                {
                    this.OnTimeOut();
                }
            }
        }

        public bool AcceptMonoBehaviour { get; private set; }

        public ulong ObjectId { get; private set; }

        public bool Reflectible { get; private set; }

        internal bool Removable { get; set; }

        protected float UpdateTimeLeft { get; set; }
    }
}

