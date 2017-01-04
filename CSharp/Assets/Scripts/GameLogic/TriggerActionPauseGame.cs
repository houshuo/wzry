namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;

    public class TriggerActionPauseGame : TriggerActionBase
    {
        private int timer;

        public TriggerActionPauseGame(TriggerActionWrapper inWrapper) : base(inWrapper)
        {
            this.timer = -1;
        }

        private void DoPauseGame(bool bPause)
        {
            if (bPause)
            {
                Singleton<CBattleGuideManager>.instance.PauseGame(this, true);
            }
            else
            {
                Singleton<CBattleGuideManager>.instance.ResumeGame(this);
            }
        }

        private void OnTimeUp(int timersequence)
        {
            this.DoPauseGame(false);
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
                this.DoPauseGame(true);
                if (base.TotalTime > 0)
                {
                    this.timer = Singleton<CTimerManager>.GetInstance().AddTimer(base.TotalTime, 1, new CTimer.OnTimeUpHandler(this.OnTimeUp), true);
                }
            }
            return null;
        }

        public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            if (base.bStopWhenLeaving)
            {
                this.DoPauseGame(false);
            }
        }

        public override void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
        }
    }
}

