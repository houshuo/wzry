namespace Assets.Scripts.Framework
{
    using System;
    using UnityEngine;

    public class GameStateCtrl : Singleton<GameStateCtrl>
    {
        private StateMachine gameState = new StateMachine();

        public IState GetCurrentState()
        {
            return this.gameState.TopState();
        }

        public void GotoState(string name)
        {
            string message = string.Format("GameStateCtrl Goto State {0}", name);
            Debug.Log(message);
            DebugHelper.CustomLog(message);
            this.gameState.ChangeState(name);
        }

        public void Initialize()
        {
            this.gameState.RegisterStateByAttributes<GameStateAttribute>(typeof(GameStateAttribute).Assembly);
        }

        public void Uninitialize()
        {
            this.gameState.Clear();
            this.gameState = null;
        }

        public string currentStateName
        {
            get
            {
                IState currentState = this.GetCurrentState();
                return ((currentState == null) ? "unkown state" : currentState.name);
            }
        }

        public bool isBattleState
        {
            get
            {
                return (this.gameState.TopState() is BattleState);
            }
        }

        public bool isHeroChooseState
        {
            get
            {
                return (this.gameState.TopState() is HeroChooseState);
            }
        }

        public bool isLoadingState
        {
            get
            {
                return (this.gameState.TopState() is LoadingState);
            }
        }

        public bool isLobbyState
        {
            get
            {
                return (this.gameState.TopState() is LobbyState);
            }
        }

        public bool isLoginState
        {
            get
            {
                return (this.gameState.TopState() is LoginState);
            }
        }
    }
}

