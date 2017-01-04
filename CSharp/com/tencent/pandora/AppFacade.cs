namespace com.tencent.pandora
{
    using System;

    public class AppFacade : Facade
    {
        private static AppFacade _instance;

        protected override void InitFramework()
        {
            Logger.d("init StartUpCommand");
            base.InitFramework();
            this.RegisterCommand("StartUp", typeof(StartUpCommand));
        }

        public void StartUp()
        {
            base.SendMessageCommand("StartUp", null);
            string[] commandName = new string[] { "StartUp" };
            base.RemoveMultiCommand(commandName);
        }

        public static AppFacade Instance
        {
            get
            {
                if (_instance == null)
                {
                    Logger.d("new AppFacade");
                    _instance = new AppFacade();
                }
                return _instance;
            }
        }
    }
}

