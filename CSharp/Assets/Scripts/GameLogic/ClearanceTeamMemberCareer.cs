namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.GameKernal;
    using ResData;
    using System;

    [StarConditionAttrContext(9)]
    internal class ClearanceTeamMemberCareer : StarCondition
    {
        private StarEvaluationStatus CachedStatus;
        private int CareerCount;

        public override void Initialize(ResDT_ConditionInfo InConditionInfo)
        {
            base.Initialize(InConditionInfo);
        }

        public override void Start()
        {
            base.Start();
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            DebugHelper.Assert(hostPlayer != null);
            ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator = hostPlayer.GetAllHeroes().GetEnumerator();
            this.CareerCount = 0;
            int targetCareer = this.targetCareer;
            while (enumerator.MoveNext())
            {
                PoolObjHandle<ActorRoot> current = enumerator.Current;
                if (current.handle.TheActorMeta.ActorType == targetCareer)
                {
                    this.CareerCount++;
                }
            }
            this.CachedStatus = !SmartCompare.Compare<int>(this.CareerCount, this.targetCount, this.operation) ? StarEvaluationStatus.Failure : StarEvaluationStatus.Success;
        }

        public override StarEvaluationStatus status
        {
            get
            {
                return this.CachedStatus;
            }
        }

        private int targetCareer
        {
            get
            {
                return base.ConditionInfo.KeyDetail[1];
            }
        }

        private int targetCount
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
                return new int[] { this.CareerCount };
            }
        }
    }
}

