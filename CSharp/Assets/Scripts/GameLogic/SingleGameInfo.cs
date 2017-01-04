namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameSystem;
    using System;

    public class SingleGameInfo : GameInfoBase
    {
        public override void EndGame()
        {
            Singleton<StarSystem>.instance.OnFailureEvaluationChanged -= new OnEvaluationChangedDelegate(this.OnFailureEvaluationChanged);
            Singleton<StarSystem>.instance.OnEvaluationChanged -= new OnEvaluationChangedDelegate(this.OnStarSystemChanged);
            Singleton<WinLoseByStarSys>.instance.OnEvaluationChanged -= new OnEvaluationChangedDelegate(BattleLogic.OnWinStarSysChanged);
            Singleton<WinLoseByStarSys>.instance.OnFailureEvaluationChanged -= new OnEvaluationChangedDelegate(BattleLogic.OnLoseStarSysChanged);
            Singleton<WinLoseByStarSys>.instance.Clear();
            base.EndGame();
        }

        private void OnFailureEvaluationChanged(IStarEvaluation InStarEvaluation, IStarCondition InStarCondition)
        {
            if ((Singleton<StarSystem>.instance.failureEvaluation == InStarEvaluation) && Singleton<StarSystem>.instance.isFailure)
            {
                PoolObjHandle<ActorRoot> handle;
                PoolObjHandle<ActorRoot> handle2;
                InStarCondition.GetActorRef(out handle, out handle2);
                Singleton<BattleLogic>.instance.OnFailure(handle, handle2);
            }
        }

        public override void OnLoadingProgress(float Progress)
        {
            CUILoadingSystem.OnSelfLoadProcess(Progress);
        }

        private void OnStarSystemChanged(IStarEvaluation InStarEvaluation, IStarCondition InStarCondition)
        {
            if ((Singleton<StarSystem>.instance.winEvaluation == InStarEvaluation) && Singleton<StarSystem>.instance.isFirstStarCompleted)
            {
                PoolObjHandle<ActorRoot> handle;
                PoolObjHandle<ActorRoot> handle2;
                InStarCondition.GetActorRef(out handle, out handle2);
                Singleton<BattleLogic>.instance.OnWinning(handle, handle2);
            }
        }

        public override void PostBeginPlay()
        {
            base.PostBeginPlay();
            if (!base.GameContext.levelContext.IsMobaMode())
            {
                Singleton<StarSystem>.instance.Reset(base.GameContext.levelContext.m_mapID);
                Singleton<StarSystem>.instance.OnEvaluationChanged += new OnEvaluationChangedDelegate(this.OnStarSystemChanged);
                Singleton<StarSystem>.instance.OnFailureEvaluationChanged += new OnEvaluationChangedDelegate(this.OnFailureEvaluationChanged);
                Singleton<StarSystem>.instance.Start();
            }
        }

        public override void PreBeginPlay()
        {
            this.LoadAllTeamActors();
        }

        public override void StartFight()
        {
            base.StartFight();
            if (Singleton<WinLoseByStarSys>.instance.Reset(base.GameContext.levelContext, false))
            {
                Singleton<WinLoseByStarSys>.instance.OnEvaluationChanged += new OnEvaluationChangedDelegate(BattleLogic.OnWinStarSysChanged);
                Singleton<WinLoseByStarSys>.instance.OnFailureEvaluationChanged += new OnEvaluationChangedDelegate(BattleLogic.OnLoseStarSysChanged);
                Singleton<WinLoseByStarSys>.instance.Start();
            }
        }
    }
}

