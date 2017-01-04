using com.tencent.pandora;
using System;
using System.Collections.Generic;
using UnityEngine;

public class com_tencent_pandora_LuaBehaviourWrap
{
    private static System.Type classType = typeof(LuaBehaviour);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_LuaBehaviour(IntPtr L)
    {
        LuaDLL.luaL_error(L, "com.tencent.pandora.LuaBehaviour class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int AddTimeFunc(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        LuaBehaviour behaviour = (LuaBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.LuaBehaviour");
        LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 2);
        behaviour.AddTimeFunc(luaFunction);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int AddUGUIClick(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        LuaBehaviour behaviour = (LuaBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.LuaBehaviour");
        GameObject go = (GameObject) LuaScriptMgr.GetUnityObject(L, 2, typeof(GameObject));
        LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 3);
        behaviour.AddUGUIClick(go, luaFunction);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CallMethod(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        LuaBehaviour behaviour = (LuaBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.LuaBehaviour");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        object[] args = LuaScriptMgr.GetParamsObject(L, 3, num - 2);
        object[] o = behaviour.CallMethod(luaString, args);
        LuaScriptMgr.PushArray(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ClearClick(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((LuaBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.LuaBehaviour")).ClearClick();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_initialize(IntPtr L)
    {
        LuaScriptMgr.Push(L, LuaBehaviour.initialize);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isUseCor(IntPtr L)
    {
        LuaBehaviour luaObject = (LuaBehaviour) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isUseCor");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isUseCor on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.isUseCor);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_timers(IntPtr L)
    {
        LuaBehaviour luaObject = (LuaBehaviour) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name timers");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index timers on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.timers);
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

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("AddTimeFunc", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.AddTimeFunc)), new LuaMethod("AddUGUIClick", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.AddUGUIClick)), new LuaMethod("ClearClick", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.ClearClick)), new LuaMethod("CallMethod", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.CallMethod)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap._Createcom_tencent_pandora_LuaBehaviour)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("isUseCor", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.get_isUseCor), new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.set_isUseCor)), new LuaField("initialize", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.get_initialize), new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.set_initialize)), new LuaField("timers", new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.get_timers), new LuaCSFunction(com_tencent_pandora_LuaBehaviourWrap.set_timers)) };
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.LuaBehaviour", typeof(LuaBehaviour), regs, fields, typeof(View));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_initialize(IntPtr L)
    {
        LuaBehaviour.initialize = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_isUseCor(IntPtr L)
    {
        LuaBehaviour luaObject = (LuaBehaviour) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isUseCor");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isUseCor on a nil value");
            }
        }
        luaObject.isUseCor = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_timers(IntPtr L)
    {
        LuaBehaviour luaObject = (LuaBehaviour) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name timers");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index timers on a nil value");
            }
        }
        luaObject.timers = (List<LuaFunction>) LuaScriptMgr.GetNetObject(L, 3, typeof(List<LuaFunction>));
        return 0;
    }
}

