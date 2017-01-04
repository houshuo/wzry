namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;

    public class TriggerActionSkillHud : TriggerActionBase
    {
        private int startTimer;
        private int timer;

        public TriggerActionSkillHud(TriggerActionWrapper inWrapper) : base(inWrapper)
        {
            this.timer = -1;
            this.startTimer = -1;
        }

        private void DoHighlight(bool bYes)
        {
            bool flag = base.EnterUniqueId > 0;
            SkillSlotType enterUniqueId = (SkillSlotType) base.EnterUniqueId;
            bool bAll = enterUniqueId == SkillSlotType.SLOT_SKILL_COUNT;
            if (flag)
            {
                Singleton<BattleSkillHudControl>.GetInstance().Highlight(enterUniqueId, bYes, bAll, false, false);
            }
        }

        private void OnActivating(int timersequence)
        {
            this.DoHighlight(base.bEnable);
            if (this.startTimer != -1)
            {
                Singleton<CTimerManager>.GetInstance().RemoveTimer(this.startTimer);
                this.startTimer = -1;
            }
            if (base.TotalTime > 0)
            {
                this.timer = Singleton<CTimerManager>.GetInstance().AddTimer(base.TotalTime, 1, new CTimer.OnTimeUpHandler(this.OnTimeUp), true);
            }
        }

        private void OnTimeUp(int timersequence)
        {
            this.DoHighlight(!base.bEnable);
            if (this.timer != -1)
            {
                Singleton<CTimerManager>.GetInstance().RemoveTimer(this.timer);
                this.timer = -1;
            }
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger, object prm)
        {
            if ((this.timer == -1) && (this.startTimer == -1))
            {
                if (base.ActiveTime > 0)
                {
                    this.startTimer = Singleton<CTimerManager>.GetInstance().AddTimer(base.ActiveTime, 1, new CTimer.OnTimeUpHandler(this.OnActivating), true);
                }
                else
                {
                    this.DoHighlight(base.bEnable);
                    if (base.TotalTime > 0)
                    {
                        this.timer = Singleton<CTimerManager>.GetInstance().AddTimer(base.TotalTime, 1, new CTimer.OnTimeUpHandler(this.OnTimeUp), true);
                    }
                }
            }
            return null;
        }

        public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            if (base.bStopWhenLeaving)
            {
                this.DoHighlight(!base.bEnable);
            }
        }

        public override void TriggerUpdate(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
        {
        }
    }
}

