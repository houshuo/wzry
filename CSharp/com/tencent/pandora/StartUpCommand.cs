namespace com.tencent.pandora
{
    using System;

    public class StartUpCommand : ControllerCommand
    {
        public override void Execute(IMessage message)
        {
            Logger.d("start startupcommand");
            Logger.d("start startupcommand1");
            Logger.d("start startupcommand2");
            Logger.d("start startupcommand3");
            try
            {
                AppFacade.Instance.AddManager("LuaScriptMgr", new LuaScriptMgr());
            }
            catch (Exception exception)
            {
                Logger.d("Message:" + exception.Message);
            }
            Logger.d("start startupcommand4");
            AppFacade.Instance.AddManager<PanelManager>("PanelManager");
            Logger.d("start startupcommand5");
            AppFacade.Instance.AddManager<ResourceManager>("ResourceManager");
            Logger.d("start startupcommand6");
            AppFacade.Instance.AddManager<GameManager>("GameManager");
        }
    }
}

