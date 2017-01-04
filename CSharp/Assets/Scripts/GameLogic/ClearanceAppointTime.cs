namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    [StarConditionAttrContext(14)]
    internal class ClearanceAppointTime : StarCondition
    {
        private bool bTimeOut;

        public void AddTimer(ref DefaultGameEventParam prm)
        {
            Singleton<CTimerManager>.instance.RemoveTimer(new CTimer.OnTimeUpHandler(this.OnAppointTimeFinished));
            Singleton<CTimerManager>.instance.AddTimer(this.timeMSeconds + 1, -1, new CTimer.OnTimeUpHandler(this.OnAppointTimeFinished));
        }

        public override void Dispose()
        {
            Singleton<CTimerManager>.instance.RemoveTimer(new CTimer.OnTimeUpHandler(this.OnAppointTimeFinished));
            base.Dispose();
        }

        public override void Initialize(ResDT_ConditionInfo InConditionInfo)
        {
            base.Initialize(InConditionInfo);
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.AddTimer));
        }

        protected void OnAppointTimeFinished(int seq)
        {
            if (!this.bTimeOut)
            {
                this.bTimeOut = true;
                this.TriggerChangedEvent();
            }
        }

        public override StarEvaluationStatus status
        {
            get
            {
                return (!this.bTimeOut ? StarEvaluationStatus.Failure : StarEvaluationStatus.Success);
            }
        }

        public int timeMSeconds
        {
            get
            {
                return base.ConditionInfo.ValueDetail[0];
            }
        }

        public override int[] values
        {
            get
            {
                return new int[] { (!this.bTimeOut ? Math.Max(0, this.timeMSeconds - 0x3e8) : (this.timeMSeconds + 1)) };
            }
        }
    }
}

