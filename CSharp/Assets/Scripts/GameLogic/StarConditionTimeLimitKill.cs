namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    [StarConditionAttrContext(8)]
    internal class StarConditionTimeLimitKill : StarConditionKillBase
    {
        protected bool bIsTimeout;
        protected int KillCntWhenTimeout;
        protected ulong StartTime;
        protected ulong TimeoutFlag;

        private bool CheckTimeout()
        {
            ulong inFirst = Singleton<FrameSynchr>.instance.LogicFrameTick - this.StartTime;
            return !SmartCompare.Compare<ulong>(inFirst, (ulong) this.limitMSeconds, this.timeOperation);
        }

        public override void Initialize(ResDT_ConditionInfo InConditionInfo)
        {
            base.Initialize(InConditionInfo);
        }

        public override void OnActorDeath(ref GameDeadEventParam prm)
        {
            base.CachedSource = prm.src;
            base.CachedAttacker = prm.orignalAtker;
            if ((!this.bIsTimeout && !base.bCachedResult) && this.CheckTimeout())
            {
                this.bIsTimeout = true;
                this.KillCntWhenTimeout = base.killCnt;
                this.TriggerChangedEvent();
            }
            base.OnActorDeath(ref prm);
        }

        protected override void OnResultStateChanged()
        {
            this.TimeoutFlag = Singleton<FrameSynchr>.instance.LogicFrameTick;
            if (!this.bIsTimeout)
            {
                base.OnResultStateChanged();
            }
        }

        protected override void OnStatChanged()
        {
            if (!this.bIsTimeout)
            {
                this.TriggerChangedEvent();
            }
        }

        public override void Start()
        {
            base.Start();
            this.KillCntWhenTimeout = 0;
            this.TimeoutFlag = (ulong) (this.StartTime = 0L);
        }

        public override string description
        {
            get
            {
                if (this.bIsTimeout)
                {
                    return string.Format("[{0}/{1}]", this.KillCntWhenTimeout, this.targetCount);
                }
                return base.description;
            }
        }

        protected override bool isSelfCamp
        {
            get
            {
                return (base.ConditionInfo.KeyDetail[3] == 0);
            }
        }

        private int limitMSeconds
        {
            get
            {
                return base.ConditionInfo.ValueDetail[1];
            }
        }

        public override StarEvaluationStatus status
        {
            get
            {
                if (this.bIsTimeout)
                {
                    return StarEvaluationStatus.Failure;
                }
                return base.status;
            }
        }

        protected override int targetCount
        {
            get
            {
                return base.ConditionInfo.ValueDetail[0];
            }
        }

        protected override int targetID
        {
            get
            {
                return base.ConditionInfo.KeyDetail[2];
            }
        }

        protected override ActorTypeDef targetType
        {
            get
            {
                return (ActorTypeDef) base.ConditionInfo.KeyDetail[1];
            }
        }

        private int timeOperation
        {
            get
            {
                return base.ConditionInfo.ComparetorDetail[1];
            }
        }

        public override int[] values
        {
            get
            {
                DebugHelper.Assert(this.TimeoutFlag >= this.StartTime);
                ulong num = this.TimeoutFlag - this.StartTime;
                return new int[] { base.killCnt, ((int) num) };
            }
        }
    }
}

