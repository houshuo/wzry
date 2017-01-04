namespace Assets.Scripts.GameLogic.GameKernal
{
    using CSProtocol;
    using System;

    internal class GameKernal : Singleton<Assets.Scripts.GameLogic.GameKernal.GameKernal>
    {
        private readonly GameBuilderEx _gameBuilder = new GameBuilderEx();

        public void StartMultiPlayerGame(SCPKG_MULTGAME_BEGINLOAD multiGameInfo)
        {
            this._gameBuilder.BuildMultiPlayerGame();
        }

        public void StartSoloGame(SCPKG_STARTSINGLEGAMERSP soloGameInfo)
        {
            this._gameBuilder.BuildSoloGame();
        }
    }
}

