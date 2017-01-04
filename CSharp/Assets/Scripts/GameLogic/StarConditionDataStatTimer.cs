namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using CSProtocol;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    [StarConditionAttrContext(0x10)]
    internal class StarConditionDataStatTimer : StarCondition
    {
        private bool bCheckResults;
        private PoolObjHandle<ActorRoot> CachedAttacker;
        private PoolObjHandle<ActorRoot> CachedSource;
        private bool m_bTimeUp;
        private int TimeSeq = -1;

        public override void Dispose()
        {
            if (this.TimeSeq >= 0)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.TimeSeq);
                this.TimeSeq = -1;
            }
            base.Dispose();
        }

        public override bool GetActorRef(out PoolObjHandle<ActorRoot> OutSource, out PoolObjHandle<ActorRoot> OutAttacker)
        {
            OutSource = this.CachedSource;
            OutAttacker = this.CachedAttacker;
            return true;
        }

        public override void OnCampScoreUpdated(ref SCampScoreUpdateParam prm)
        {
            this.UpdateCheckResults();
        }

        private void OnTimeUp(int inSeq)
        {
            if (this.TimeSeq >= 0)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.TimeSeq);
                this.TimeSeq = -1;
                this.UpdateCheckResults();
                this.m_bTimeUp = true;
                this.TriggerChangedEvent();
            }
        }

        public override void Start()
        {
            base.Start();
            this.m_bTimeUp = false;
            this.TimeSeq = Singleton<CTimerManager>.instance.AddTimer(this.TimerDuration, 1, new CTimer.OnTimeUpHandler(this.OnTimeUp), true);
        }

        private void UpdateCheckResults()
        {
            COM_PLAYERCAMP hostPlayerCamp = Singleton<GamePlayerCenter>.instance.hostPlayerCamp;
            COM_PLAYERCAMP com_playercamp2 = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
            switch (hostPlayerCamp)
            {
                case COM_PLAYERCAMP.COM_PLAYERCAMP_1:
                    com_playercamp2 = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
                    break;

                case COM_PLAYERCAMP.COM_PLAYERCAMP_2:
                    com_playercamp2 = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
                    break;
            }
            uint score = (uint) Singleton<BattleStatistic>.instance.GetCampStat()[(uint) hostPlayerCamp].GetScore(this.DataSubType);
            uint inSecond = (uint) Singleton<BattleStatistic>.instance.GetCampStat()[(uint) com_playercamp2].GetScore(this.DataSubType);
            if (score != inSecond)
            {
                if (this.TargetCamp == 0)
                {
                    this.bCheckResults = SmartCompare.Compare<uint>(score, inSecond, this.operation);
                    Singleton<BattleStatistic>.instance.bSelfCampHaveWinningFlag = this.bCheckResults;
                }
                else if (this.TargetCamp == 1)
                {
                    this.bCheckResults = SmartCompare.Compare<uint>(inSecond, score, this.operation);
                    Singleton<BattleStatistic>.instance.bSelfCampHaveWinningFlag = !this.bCheckResults;
                }
            }
            else if ((score == 0) && (this.TargetCamp == 1))
            {
                this.bCheckResults = true;
                Singleton<BattleStatistic>.instance.bSelfCampHaveWinningFlag = false;
            }
        }

        public RES_STAR_CONDITION_DATA_SUB_TYPE DataSubType
        {
            get
            {
                return (RES_STAR_CONDITION_DATA_SUB_TYPE) base.ConditionInfo.KeyDetail[1];
            }
        }

        public override StarEvaluationStatus status
        {
            get
            {
                return ((!this.bCheckResults || !this.m_bTimeUp) ? StarEvaluationStatus.Failure : StarEvaluationStatus.Success);
            }
        }

        private int TargetCamp
        {
            get
            {
                return base.ConditionInfo.KeyDetail[2];
            }
        }

        public int TimerDuration
        {
            get
            {
                return (int) Singleton<WinLoseByStarSys>.instance.CurLevelTimeDuration;
            }
        }

        public override int[] values
        {
            get
            {
                return new int[1];
            }
        }
    }
}

