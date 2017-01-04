namespace Assets.Scripts.Framework
{
    using System;

    [GameState]
    public class MovieState : BaseState
    {
        private void GoNextState()
        {
            Singleton<GameStateCtrl>.GetInstance().GotoState("LoginState");
        }

        public override void OnStateEnter()
        {
            this.GoNextState();
        }
    }
}

