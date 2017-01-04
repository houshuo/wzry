namespace Assets.Scripts.Framework
{
    using Assets.Scripts.GameSystem;
    using System;

    [GameState]
    public class LaunchState : BaseState
    {
        private bool m_isBaseSystemPrepareComplete;
        private bool m_isSplashPlayComplete;
        private bool m_jumpState;

        private void CheckContionToNextState()
        {
            if ((this.m_isSplashPlayComplete && this.m_isBaseSystemPrepareComplete) && (!Singleton<CCheatSystem>.GetInstance().m_enabled && !this.m_jumpState))
            {
                this.m_jumpState = true;
                Singleton<GameStateCtrl>.GetInstance().GotoState("VersionUpdateState");
            }
        }

        private void OnCheatSystemDisable()
        {
            this.CheckContionToNextState();
        }

        private void OnPrepareBaseSystemComplete()
        {
            this.m_isBaseSystemPrepareComplete = true;
            this.CheckContionToNextState();
        }

        private void OnSplashLoadCompleted()
        {
            Singleton<CTimerManager>.GetInstance().AddTimer(0x3e8, 1, new CTimer.OnTimeUpHandler(this.OnTimiPlayComplete));
            Singleton<CTimerManager>.GetInstance().AddTimer(0xbb8, 1, new CTimer.OnTimeUpHandler(this.OnSplashPlayComplete));
            Singleton<CCheatSystem>.GetInstance().OpenCheatTriggerForm(new CCheatSystem.OnDisable(this.OnCheatSystemDisable));
        }

        private void OnSplashPlayComplete(int timerSequence)
        {
            this.m_isSplashPlayComplete = true;
            this.CheckContionToNextState();
        }

        public override void OnStateEnter()
        {
            Singleton<ResourceLoader>.GetInstance().LoadScene("SplashScene", new ResourceLoader.LoadCompletedDelegate(this.OnSplashLoadCompleted));
            this.m_isSplashPlayComplete = false;
            this.m_isBaseSystemPrepareComplete = false;
        }

        private void OnTimiPlayComplete(int timerSequence)
        {
            MonoSingleton<GameFramework>.GetInstance().StartPrepareBaseSystem(new GameFramework.DelegateOnBaseSystemPrepareComplete(this.OnPrepareBaseSystemComplete));
        }
    }
}

