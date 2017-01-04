using com.tencent.pandora;
using System;
using UnityEngine;

public class com_tencent_pandora_NotificationCenterWrap
{
    private static System.Type classType = typeof(NotificationCenter);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_NotificationCenter(IntPtr L)
    {
        LuaDLL.luaL_error(L, "com.tencent.pandora.NotificationCenter class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int AddObserver(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 3:
            {
                NotificationCenter center = (NotificationCenter) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.NotificationCenter");
                Component observer = (Component) LuaScriptMgr.GetUnityObject(L, 2, typeof(Component));
                string luaString = LuaScriptMgr.GetLuaString(L, 3);
                center.AddObserver(observer, luaString);
                return 0;
            }
            case 4:
            {
                NotificationCenter center2 = (NotificationCenter) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.NotificationCenter");
                Component component2 = (Component) LuaScriptMgr.GetUnityObject(L, 2, typeof(Component));
                string name = LuaScriptMgr.GetLuaString(L, 3);
                Component sender = (Component) LuaScriptMgr.GetUnityObject(L, 4, typeof(Component));
                center2.AddObserver(component2, name, sender);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: com.tencent.pandora.NotificationCenter.AddObserver");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Clean(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        NotificationCenter.Clean();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int DefaultCenter(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        NotificationCenter center = NotificationCenter.DefaultCenter();
        LuaScriptMgr.Push(L, center);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_blNewsOpen(IntPtr L)
    {
        LuaScriptMgr.Push(L, NotificationCenter.blNewsOpen);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_blPLNewsOpen(IntPtr L)
    {
        LuaScriptMgr.Push(L, NotificationCenter.blPLNewsOpen);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_Eq(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        UnityEngine.Object luaObject = LuaScriptMgr.GetLuaObject(L, 1) as UnityEngine.Object;
        UnityEngine.Object obj3 = LuaScriptMgr.GetLuaObject(L, 2) as UnityEngine.Object;
        bool b = luaObject == obj3;
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int PostNotification(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                NotificationCenter center = (NotificationCenter) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.NotificationCenter");
                Notification aNotification = (Notification) LuaScriptMgr.GetNetObject(L, 2, typeof(Notification));
                center.PostNotification(aNotification);
                return 0;
            }
            case 3:
            {
                NotificationCenter center2 = (NotificationCenter) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.NotificationCenter");
                Component aSender = (Component) LuaScriptMgr.GetUnityObject(L, 2, typeof(Component));
                string luaString = LuaScriptMgr.GetLuaString(L, 3);
                center2.PostNotification(aSender, luaString);
                return 0;
            }
            case 4:
            {
                NotificationCenter center3 = (NotificationCenter) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.NotificationCenter");
                Component component2 = (Component) LuaScriptMgr.GetUnityObject(L, 2, typeof(Component));
                string aName = LuaScriptMgr.GetLuaString(L, 3);
                object varObject = LuaScriptMgr.GetVarObject(L, 4);
                center3.PostNotification(component2, aName, varObject);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: com.tencent.pandora.NotificationCenter.PostNotification");
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("DefaultCenter", new LuaCSFunction(com_tencent_pandora_NotificationCenterWrap.DefaultCenter)), new LuaMethod("AddObserver", new LuaCSFunction(com_tencent_pandora_NotificationCenterWrap.AddObserver)), new LuaMethod("RemoveObserver", new LuaCSFunction(com_tencent_pandora_NotificationCenterWrap.RemoveObserver)), new LuaMethod("PostNotification", new LuaCSFunction(com_tencent_pandora_NotificationCenterWrap.PostNotification)), new LuaMethod("Clean", new LuaCSFunction(com_tencent_pandora_NotificationCenterWrap.Clean)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_NotificationCenterWrap._Createcom_tencent_pandora_NotificationCenter)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_NotificationCenterWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(com_tencent_pandora_NotificationCenterWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("blPLNewsOpen", new LuaCSFunction(com_tencent_pandora_NotificationCenterWrap.get_blPLNewsOpen), new LuaCSFunction(com_tencent_pandora_NotificationCenterWrap.set_blPLNewsOpen)), new LuaField("blNewsOpen", new LuaCSFunction(com_tencent_pandora_NotificationCenterWrap.get_blNewsOpen), new LuaCSFunction(com_tencent_pandora_NotificationCenterWrap.set_blNewsOpen)) };
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.NotificationCenter", typeof(NotificationCenter), regs, fields, typeof(MonoBehaviour));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int RemoveObserver(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Component observer = (Component) LuaScriptMgr.GetUnityObject(L, 1, typeof(Component));
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        NotificationCenter.RemoveObserver(observer, luaString);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_blNewsOpen(IntPtr L)
    {
        NotificationCenter.blNewsOpen = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_blPLNewsOpen(IntPtr L)
    {
        NotificationCenter.blPLNewsOpen = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }
}

