namespace com.tencent.pandora
{
    using System;

    public class LuaEventHandler
    {
        public LuaFunction handler;

        public void handleEvent(object[] args)
        {
            try
            {
                this.handler.Call(args);
            }
            catch (Exception exception)
            {
                Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }
    }
}

