namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    [StarConditionAttrContext(11)]
    internal class ClearanceTimeLimit : StarCondition
    {
        private ulong EndTime;
        private ulong StartTime;

        public override void Initialize(ResDT_ConditionInfo InConditionInfo)
        {
            base.Initialize(InConditionInfo);
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
        }

        private void onFightOver(ref DefaultGameEventParam prm)
        {
            this.EndTime = Singleton<FrameSynchr>.instance.LogicFrameTick;
            if (this.status == StarEvaluationStatus.Success)
            {
                this.TriggerChangedEvent();
            }
        }

        public override void Start()
        {
            base.Start();
            this.EndTime = (ulong) (this.StartTime = 0L);
        }

        private ulong limitMSeconds
        {
            get
            {
                return (ulong) base.ConditionInfo.ValueDetail[0];
            }
        }

        public override StarEvaluationStatus status
        {
            get
            {
                ulong num = this.EndTime - this.StartTime;
                if (num == 0)
                {
                    num = Singleton<FrameSynchr>.instance.LogicFrameTick - this.StartTime;
                    return ((num <= this.limitMSeconds) ? StarEvaluationStatus.InProgressing : StarEvaluationStatus.Failure);
                }
                if (num > this.limitMSeconds)
                {
                    return StarEvaluationStatus.Failure;
                }
                return StarEvaluationStatus.Success;
            }
        }

        public override int[] values
        {
            get
            {
                ulong num = this.EndTime - this.StartTime;
                return new int[] { ((int) num) };
            }
        }
    }
}

