namespace com.tencent.pandora
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class LuaHelper
    {
        public static System.Action Action(LuaFunction func)
        {
            <Action>c__AnonStorey32 storey = new <Action>c__AnonStorey32 {
                func = func
            };
            return new System.Action(storey.<>m__8);
        }

        public static PanelManager GetPanelManager()
        {
            return AppFacade.Instance.GetManager<PanelManager>("PanelManager");
        }

        public static ResourceManager GetResManager()
        {
            return AppFacade.Instance.GetManager<ResourceManager>("ResourceManager");
        }

        public static Type GetType(string classname)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Type type = null;
            type = executingAssembly.GetType(classname);
            if (type == null)
            {
                type = executingAssembly.GetType(classname);
            }
            return type;
        }

        public static void OnCallLuaFunc(LuaStringBuffer data, LuaFunction func)
        {
            try
            {
                byte[] str = data.buffer;
                if (func != null)
                {
                    LuaScriptMgr manager = AppFacade.Instance.GetManager<LuaScriptMgr>("LuaScriptMgr");
                    int oldTop = func.BeginPCall();
                    LuaDLL.lua_pushlstring(manager.lua.L, str, str.Length);
                    if (func.PCall(oldTop, 1))
                    {
                        func.EndPCall(oldTop);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        public static void OnJsonCallFunc(string data, LuaFunction func)
        {
            if (func != null)
            {
                object[] args = new object[] { data };
                func.Call(args);
            }
        }

        [CompilerGenerated]
        private sealed class <Action>c__AnonStorey32
        {
            internal LuaFunction func;

            internal void <>m__8()
            {
                this.func.Call();
            }
        }
    }
}

