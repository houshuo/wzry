namespace Assets.Scripts.GameLogic
{
    using CSProtocol;
    using System;

    public class TestGameContext : GameContextBase
    {
        public TestGameContext(ref SCPKG_STARTSINGLEGAMERSP InMessage)
        {
            Singleton<ActorDataCenter>.instance.ClearHeroServerData();
            Singleton<GamePlayerCenter>.GetInstance().AddPlayer(1, COM_PLAYERCAMP.COM_PLAYERCAMP_1, 0, 1, false, "test", 0, 0, 0L, 0, null, 0, 0, 0, 0);
            Singleton<GamePlayerCenter>.GetInstance().SetHostPlayer(1);
            base.LevelContext = new SLevelContext();
            base.LevelContext.SetGameType(COM_GAME_TYPE.COM_SINGLE_GAME_OF_ADVENTURE);
        }

        public override GameInfoBase CreateGameInfo()
        {
            SingleGameInfo info = new SingleGameInfo();
            info.Initialize(this);
            return info;
        }
    }
}

