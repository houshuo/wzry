namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    [StarConditionAttrContext(3)]
    internal class StarConditionDeathStat : StarCondition
    {
        private bool bCheckResults = true;
        protected PoolObjHandle<ActorRoot> CachedAttacker;
        protected PoolObjHandle<ActorRoot> CachedSource;
        private int DealthCount;

        public override void Dispose()
        {
            base.Dispose();
        }

        public override bool GetActorRef(out PoolObjHandle<ActorRoot> OutSource, out PoolObjHandle<ActorRoot> OutAttacker)
        {
            OutSource = this.CachedSource;
            OutAttacker = this.CachedAttacker;
            return true;
        }

        public override void Initialize(ResDT_ConditionInfo InConditionInfo)
        {
            base.Initialize(InConditionInfo);
            this.bCheckResults = SmartCompare.Compare<int>(this.DealthCount, this.targetCount, this.operation);
        }

        public override void OnActorDeath(ref GameDeadEventParam prm)
        {
            if ((((prm.src != 0) && (prm.src.handle.TheActorMeta.ActorType == this.targetType)) && ((this.targetID == 0) || (this.targetID == prm.src.handle.TheActorMeta.ConfigId))) && this.ShouldCare(prm.src.handle))
            {
                this.DealthCount++;
                bool flag = SmartCompare.Compare<int>(this.DealthCount, this.targetCount, this.operation);
                if (this.bCheckResults != flag)
                {
                    this.bCheckResults = flag;
                    this.CachedSource = prm.src;
                    this.CachedAttacker = prm.orignalAtker;
                    this.TriggerChangedEvent();
                }
            }
        }

        private bool ShouldCare(ActorRoot src)
        {
            if (this.targetCamp == 0)
            {
                return (src.TheActorMeta.ActorCamp == Singleton<GamePlayerCenter>.instance.hostPlayerCamp);
            }
            return (src.TheActorMeta.ActorCamp != Singleton<GamePlayerCenter>.instance.hostPlayerCamp);
        }

        public override StarEvaluationStatus status
        {
            get
            {
                return (!this.bCheckResults ? StarEvaluationStatus.Failure : StarEvaluationStatus.Success);
            }
        }

        private int targetCamp
        {
            get
            {
                return base.ConditionInfo.KeyDetail[3];
            }
        }

        public int targetCount
        {
            get
            {
                return base.ConditionInfo.ValueDetail[0];
            }
        }

        public int targetID
        {
            get
            {
                return base.ConditionInfo.KeyDetail[2];
            }
        }

        public ActorTypeDef targetType
        {
            get
            {
                return (ActorTypeDef) base.ConditionInfo.KeyDetail[1];
            }
        }

        public override int[] values
        {
            get
            {
                return new int[] { this.DealthCount };
            }
        }
    }
}

