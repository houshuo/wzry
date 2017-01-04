namespace Assets.Scripts.GameLogic
{
    using ResData;
    using System;

    [StarConditionAttrContext(13)]
    internal class StarConditionPVPAchievement : StarCondition
    {
        private bool bHasComplete;
        private int CompleteCount;

        public override void Dispose()
        {
            Singleton<EventRouter>.instance.RemoveEventHandler<KillDetailInfo>(EventID.AchievementRecorderEvent, new Action<KillDetailInfo>(this.OnAchievementEvent));
            base.Dispose();
        }

        public override void Initialize(ResDT_ConditionInfo InConditionInfo)
        {
            base.Initialize(InConditionInfo);
            Singleton<EventRouter>.instance.AddEventHandler<KillDetailInfo>(EventID.AchievementRecorderEvent, new Action<KillDetailInfo>(this.OnAchievementEvent));
        }

        private void OnAchievementEvent(KillDetailInfo KillDetail)
        {
            if ((((KillDetail.Killer != 0) && (KillDetail.Killer.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)) && ActorHelper.IsHostCtrlActor(ref KillDetail.Killer)) && ((KillDetail.HeroContiKillType == this.targetAchievementType) || (KillDetail.HeroMultiKillType == this.targetAchievementType)))
            {
                this.CompleteCount++;
                if (!this.bHasComplete && (this.status == StarEvaluationStatus.Success))
                {
                    this.bHasComplete = true;
                    this.TriggerChangedEvent();
                }
            }
        }

        public override string description
        {
            get
            {
                return string.Format("[{0}/{1}]", this.CompleteCount, this.targetCount);
            }
        }

        public override StarEvaluationStatus status
        {
            get
            {
                return (!SmartCompare.Compare<int>(this.CompleteCount, this.targetCount, this.operation) ? StarEvaluationStatus.InProgressing : StarEvaluationStatus.Success);
            }
        }

        private KillDetailInfoType targetAchievementType
        {
            get
            {
                return (KillDetailInfoType) base.ConditionInfo.KeyDetail[1];
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
                return new int[] { this.CompleteCount };
            }
        }
    }
}

