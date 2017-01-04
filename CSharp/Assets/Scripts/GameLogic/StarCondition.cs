namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public abstract class StarCondition : IStarCondition
    {
        protected string Description = string.Empty;

        public event OnStarConditionChangedDelegate OnStarConditionChanged;

        protected StarCondition()
        {
        }

        public virtual void Dispose()
        {
        }

        public virtual bool GetActorRef(out PoolObjHandle<ActorRoot> OutSource, out PoolObjHandle<ActorRoot> OutAttacker)
        {
            OutSource = new PoolObjHandle<ActorRoot>(null);
            OutAttacker = new PoolObjHandle<ActorRoot>(null);
            return false;
        }

        public virtual void Initialize(ResDT_ConditionInfo InConditionInfo)
        {
            this.ConditionInfo = InConditionInfo;
        }

        public virtual void OnActorDeath(ref GameDeadEventParam prm)
        {
        }

        public virtual void OnCampScoreUpdated(ref SCampScoreUpdateParam prm)
        {
        }

        public static ulong PackUInt32ToUInt64(uint InHigh, uint InLow)
        {
            ulong num = 0L;
            num |= InHigh << 0x20;
            return (num | InLow);
        }

        public virtual void Start()
        {
        }

        protected virtual void TriggerChangedEvent()
        {
            if (this.OnStarConditionChanged != null)
            {
                this.OnStarConditionChanged(this);
            }
        }

        public static void UnPackUInt64ToUInt32(ulong InNumeric, out uint OutHigh, out uint OutLow)
        {
            OutHigh = (uint) (InNumeric >> 0x20);
            OutLow = (uint) (InNumeric | 0xffffffffL);
        }

        public ResDT_ConditionInfo ConditionInfo { get; protected set; }

        public ResDT_ConditionInfo configInfo
        {
            get
            {
                return this.ConditionInfo;
            }
        }

        public virtual int defaultValue
        {
            get
            {
                return this.ConditionInfo.ValueDetail[0];
            }
        }

        public virtual string description
        {
            get
            {
                return this.Description;
            }
        }

        public virtual int extraType
        {
            get
            {
                return this.ConditionInfo.KeyDetail[0];
            }
        }

        public virtual int[] keys
        {
            get
            {
                DebugHelper.Assert(this.ConditionInfo != null);
                return new int[] { this.ConditionInfo.KeyDetail[0], this.ConditionInfo.KeyDetail[1], this.ConditionInfo.KeyDetail[2], this.ConditionInfo.KeyDetail[3] };
            }
        }

        public virtual int operation
        {
            get
            {
                return this.ConditionInfo.ComparetorDetail[0];
            }
        }

        public string rawDescription
        {
            get
            {
                return this.Description;
            }
        }

        public virtual StarEvaluationStatus status
        {
            get
            {
                return StarEvaluationStatus.InProgressing;
            }
        }

        public virtual int type
        {
            get
            {
                return (int) this.ConditionInfo.dwType;
            }
        }

        public abstract int[] values { get; }
    }
}

