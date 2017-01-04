using com.tencent.pandora;
using System;
using UnityEngine;

public class AssetBundleWrap
{
    private static System.Type classType = typeof(AssetBundle);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateAssetBundle(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            AssetBundle bundle = new AssetBundle();
            LuaScriptMgr.Push(L, bundle);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: AssetBundle.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Contains(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        AssetBundle bundle = (AssetBundle) LuaScriptMgr.GetUnityObjectSelf(L, 1, "AssetBundle");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        bool b = bundle.Contains(luaString);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CreateFromFile(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        AssetBundle bundle = AssetBundle.CreateFromFile(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, bundle);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CreateFromMemory(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        AssetBundleCreateRequest o = AssetBundle.CreateFromMemory(LuaScriptMgr.GetArrayNumber<byte>(L, 1));
        LuaScriptMgr.PushObject(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CreateFromMemoryImmediate(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        AssetBundle bundle = AssetBundle.CreateFromMemoryImmediate(LuaScriptMgr.GetArrayNumber<byte>(L, 1));
        LuaScriptMgr.Push(L, bundle);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_mainAsset(IntPtr L)
    {
        AssetBundle luaObject = (AssetBundle) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name mainAsset");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index mainAsset on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.mainAsset);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Load(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                AssetBundle bundle = (AssetBundle) LuaScriptMgr.GetUnityObjectSelf(L, 1, "AssetBundle");
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                UnityEngine.Object obj2 = bundle.Load(luaString);
                LuaScriptMgr.Push(L, obj2);
                return 1;
            }
            case 3:
            {
                AssetBundle bundle2 = (AssetBundle) LuaScriptMgr.GetUnityObjectSelf(L, 1, "AssetBundle");
                string name = LuaScriptMgr.GetLuaString(L, 2);
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 3);
                UnityEngine.Object obj3 = bundle2.Load(name, typeObject);
                LuaScriptMgr.Push(L, obj3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: AssetBundle.Load");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadAll(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                UnityEngine.Object[] o = ((AssetBundle) LuaScriptMgr.GetUnityObjectSelf(L, 1, "AssetBundle")).LoadAll();
                LuaScriptMgr.PushArray(L, o);
                return 1;
            }
            case 2:
            {
                AssetBundle bundle2 = (AssetBundle) LuaScriptMgr.GetUnityObjectSelf(L, 1, "AssetBundle");
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
                UnityEngine.Object[] objArray2 = bundle2.LoadAll(typeObject);
                LuaScriptMgr.PushArray(L, objArray2);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: AssetBundle.LoadAll");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadAsync(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        AssetBundle bundle = (AssetBundle) LuaScriptMgr.GetUnityObjectSelf(L, 1, "AssetBundle");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 3);
        AssetBundleRequest o = bundle.LoadAsync(luaString, typeObject);
        LuaScriptMgr.PushObject(L, o);
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
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("CreateFromMemory", new LuaCSFunction(AssetBundleWrap.CreateFromMemory)), new LuaMethod("CreateFromMemoryImmediate", new LuaCSFunction(AssetBundleWrap.CreateFromMemoryImmediate)), new LuaMethod("CreateFromFile", new LuaCSFunction(AssetBundleWrap.CreateFromFile)), new LuaMethod("Contains", new LuaCSFunction(AssetBundleWrap.Contains)), new LuaMethod("Load", new LuaCSFunction(AssetBundleWrap.Load)), new LuaMethod("LoadAsync", new LuaCSFunction(AssetBundleWrap.LoadAsync)), new LuaMethod("LoadAll", new LuaCSFunction(AssetBundleWrap.LoadAll)), new LuaMethod("Unload", new LuaCSFunction(AssetBundleWrap.Unload)), new LuaMethod("New", new LuaCSFunction(AssetBundleWrap._CreateAssetBundle)), new LuaMethod("GetClassType", new LuaCSFunction(AssetBundleWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(AssetBundleWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("mainAsset", new LuaCSFunction(AssetBundleWrap.get_mainAsset), null) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.AssetBundle", typeof(AssetBundle), regs, fields, typeof(UnityEngine.Object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Unload(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        AssetBundle bundle = (AssetBundle) LuaScriptMgr.GetUnityObjectSelf(L, 1, "AssetBundle");
        bool boolean = LuaScriptMgr.GetBoolean(L, 2);
        bundle.Unload(boolean);
        return 0;
    }
}

