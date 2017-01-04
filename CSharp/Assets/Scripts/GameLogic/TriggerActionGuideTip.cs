namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;

    public class TriggerActionGuideTip : TriggerActionBase
    {
        private int GuideTipId;
        private int timer;

        public TriggerActionGuideTip(TriggerActionWrapper inWrapper) : base(inWrapper)
        {
            this.timer = -1;
        }

        private void OnTimeUp(int timersequence)
        {
            Singleton<TipProcessor>.GetInstance().EndDrama(this.GuideTipId);
            if (this.timer != -1)
            {
                Singleton<CTimerManager>.GetInstance().RemoveTimer(this.timer);
                this.timer = -1;
            }
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger, object prm)
        {
            if (this.timer == -1)
            {
                this.GuideTipId = base.EnterUniqueId;
                ActorRoot inSrc = (src == 0) ? null : src.handle;
                ActorRoot inAtker = (atker == 0) ? null : atker.handle;
                if (base.bEnable)
                {
                    Singleton<TipProcessor>.GetInstance().PlayDrama(this.GuideTipId, inSrc, inAtker);
                }
                else
                {
                    Singleton<TipProcessor>.GetInstance().EndDrama(this.GuideTipId);
                }
                if (base.TotalTime > 0)
                {
                    this.timer = Singleton<CTimerManager>.GetInstance().AddTimer(base.TotalTime, 1, new CTimer.OnTimeUpHandler(this.OnTimeUp), true);
                }
            }
            return null;
        }

        public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            int leaveUniqueId = base.LeaveUniqueId;
            ActorRoot inSrc = (src == 0) ? null : src.handle;
            ActorRoot inAtker = null;
            if (base.bEnable)
            {
                Singleton<TipProcessor>.GetInstance().PlayDrama(leaveUniqueId, inSrc, inAtker);
            }
            else
            {
                Singleton<TipProcessor>.GetInstance().EndDrama(leaveUniqueId);
            }
        }
    }
}

