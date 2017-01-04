namespace Assets.Scripts.GameLogic
{
    using CSProtocol;
    using System;
    using UnityEngine;

    public abstract class FuncRegion : MonoBehaviour, IUpdateLogic
    {
        public COM_PLAYERCAMP CampType;
        public bool isStartup;

        protected FuncRegion()
        {
        }

        public virtual void Startup()
        {
            this.isStartup = true;
        }

        public virtual void Stop()
        {
            this.isStartup = false;
        }

        public virtual void UpdateLogic(int delta)
        {
        }
    }
}

