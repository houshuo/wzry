namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    public abstract class TriggerActionBase
    {
        public int ActiveTime;
        public bool bAtker;
        public bool bEnable;
        public bool bSrc;
        public bool bStopWhenLeaving;
        public int EnterUniqueId;
        public int LeaveUniqueId;
        public int Offset_x;
        public int Offset_y;
        public GameObject[] RefObjList;
        public AreaEventTrigger.STimingAction[] TimingActionsInter;
        public int TotalTime;
        public int UpdateUniqueId;

        public TriggerActionBase(TriggerActionWrapper inWrapper)
        {
            this.RefObjList = inWrapper.RefObjList;
            this.TimingActionsInter = inWrapper.TimingActionsInter;
            this.EnterUniqueId = inWrapper.EnterUniqueId;
            this.LeaveUniqueId = inWrapper.LeaveUniqueId;
            this.UpdateUniqueId = inWrapper.UpdateUniqueId;
            this.bEnable = inWrapper.bEnable;
            this.bStopWhenLeaving = inWrapper.bStopWhenLeaving;
            this.bSrc = inWrapper.bSrc;
            this.bAtker = inWrapper.bAtker;
            this.TotalTime = inWrapper.TotalTime;
            this.ActiveTime = inWrapper.ActiveTime;
            this.Offset_x = inWrapper.Offset_x;
            this.Offset_y = inWrapper.Offset_y;
        }

        public void AppendRefObj(GameObject[] inRefObjList)
        {
            if ((inRefObjList != null) && (inRefObjList.Length != 0))
            {
                this.RefObjList = inRefObjList;
            }
        }

        public virtual void Destroy()
        {
        }

        public virtual void OnCoolDown(ITrigger inTrigger)
        {
        }

        public virtual void OnTriggerStart(ITrigger inTrigger)
        {
        }

        public abstract RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger, object prm);
        public virtual void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
        }

        public virtual void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
        }
    }
}

