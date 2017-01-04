namespace Assets.Scripts.Framework
{
    using System;

    public class GameDataValidator : Singleton<GameDataValidator>
    {
        public bool bDebugLossPickHeroMessage;

        public bool IsLoseImportantGameData()
        {
            return !Singleton<LobbyLogic>.instance.inMultiRoom;
        }

        public bool ValidateGameData()
        {
            if (!Singleton<GameReplayModule>.instance.isReplay && this.IsLoseImportantGameData())
            {
                object[] inParameters = new object[] { Environment.StackTrace };
                DebugHelper.Assert(false, "Warning, Lose HostPlayer, try to reconnect to gameserver. stacktrace:{0}", inParameters);
                DebugHelper.Assert(Singleton<NetworkModule>.instance.gameSvr != null, "invalid gameserver");
                Singleton<NetworkModule>.instance.gameSvr.RestartConnector();
                return false;
            }
            return true;
        }
    }
}

