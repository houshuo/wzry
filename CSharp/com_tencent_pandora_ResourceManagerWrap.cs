using com.tencent.pandora;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class com_tencent_pandora_ResourceManagerWrap
{
    private static System.Type classType = typeof(ResourceManager);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_ResourceManager(IntPtr L)
    {
        LuaDLL.luaL_error(L, "com.tencent.pandora.ResourceManager class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int AssemplyUI(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        ResourceManager manager = (ResourceManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.ResourceManager");
        GameObject goParent = (GameObject) LuaScriptMgr.GetUnityObject(L, 2, typeof(GameObject));
        manager.AssemplyUI(goParent);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int DownloadRes(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((ResourceManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.ResourceManager")).DownloadRes();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_actId(IntPtr L)
    {
        LuaScriptMgr.Push(L, ResourceManager.actId);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_atlasUpdate(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name atlasUpdate");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index atlasUpdate on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.atlasUpdate);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_blUIResUnLoad(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name blUIResUnLoad");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index blUIResUnLoad on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.blUIResUnLoad);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_dicTryToLoad(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name dicTryToLoad");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index dicTryToLoad on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.dicTryToLoad);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_iLuaVer(IntPtr L)
    {
        LuaScriptMgr.Push(L, ResourceManager.iLuaVer);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_init_atlas(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name init_atlas");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index init_atlas on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.init_atlas);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isLuaBundleReady(IntPtr L)
    {
        LuaScriptMgr.Push(L, ResourceManager.isLuaBundleReady);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_luaFiles(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name luaFiles");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index luaFiles on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.luaFiles);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_mdictFont(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name mdictFont");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index mdictFont on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.mdictFont);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_needDownAtlas(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name needDownAtlas");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index needDownAtlas on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.needDownAtlas);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_needDownFont(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name needDownFont");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index needDownFont on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.needDownFont);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_passTimeSpan(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name passTimeSpan");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index passTimeSpan on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.passTimeSpan);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_startTimer(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startTimer");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startTimer on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.startTimer);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_useSA(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name useSA");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index useSA on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.useSA);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int getPlatformDesc(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        string str = ResourceManager.getPlatformDesc();
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetResFullName(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        ResourceManager manager = (ResourceManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.ResourceManager");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        string resFullName = manager.GetResFullName(luaString);
        LuaScriptMgr.Push(L, resFullName);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetResPath(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        ResourceManager manager = (ResourceManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.ResourceManager");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        string resPath = manager.GetResPath(luaString);
        LuaScriptMgr.Push(L, resPath);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int HasFont(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        ResourceManager manager = (ResourceManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.ResourceManager");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        bool b = manager.HasFont(luaString);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int InitAtlas(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((ResourceManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.ResourceManager")).InitAtlas();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadBundleRes(IntPtr L)
    {
        <LoadBundleRes>c__AnonStorey56 storey = new <LoadBundleRes>c__AnonStorey56 {
            L = L
        };
        LuaScriptMgr.CheckArgsCount(storey.L, 3);
        ResourceManager manager = (ResourceManager) LuaScriptMgr.GetUnityObjectSelf(storey.L, 1, "com.tencent.pandora.ResourceManager");
        string luaString = LuaScriptMgr.GetLuaString(storey.L, 2);
        Action<AssetBundle> reFunc = null;
        if (LuaDLL.lua_type(storey.L, 3) != LuaTypes.LUA_TFUNCTION)
        {
            reFunc = (Action<AssetBundle>) LuaScriptMgr.GetNetObject(storey.L, 3, typeof(Action<AssetBundle>));
        }
        else
        {
            <LoadBundleRes>c__AnonStorey55 storey2 = new <LoadBundleRes>c__AnonStorey55 {
                <>f__ref$86 = storey,
                func = LuaScriptMgr.GetLuaFunction(storey.L, 3)
            };
            reFunc = new Action<AssetBundle>(storey2.<>m__24);
        }
        manager.LoadBundleRes(luaString, reFunc);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadFromRemote(IntPtr L)
    {
        <LoadFromRemote>c__AnonStorey58 storey = new <LoadFromRemote>c__AnonStorey58 {
            L = L
        };
        LuaScriptMgr.CheckArgsCount(storey.L, 5);
        ResourceManager manager = (ResourceManager) LuaScriptMgr.GetUnityObjectSelf(storey.L, 1, "com.tencent.pandora.ResourceManager");
        string luaString = LuaScriptMgr.GetLuaString(storey.L, 2);
        string tag = LuaScriptMgr.GetLuaString(storey.L, 3);
        Action<WWW, string> onLoad = null;
        if (LuaDLL.lua_type(storey.L, 4) != LuaTypes.LUA_TFUNCTION)
        {
            onLoad = (Action<WWW, string>) LuaScriptMgr.GetNetObject(storey.L, 4, typeof(Action<WWW, string>));
        }
        else
        {
            <LoadFromRemote>c__AnonStorey57 storey2 = new <LoadFromRemote>c__AnonStorey57 {
                <>f__ref$88 = storey,
                func = LuaScriptMgr.GetLuaFunction(storey.L, 4)
            };
            onLoad = new Action<WWW, string>(storey2.<>m__25);
        }
        Action<WWW, string> onLoaddowning = null;
        if (LuaDLL.lua_type(storey.L, 5) != LuaTypes.LUA_TFUNCTION)
        {
            onLoaddowning = (Action<WWW, string>) LuaScriptMgr.GetNetObject(storey.L, 5, typeof(Action<WWW, string>));
        }
        else
        {
            <LoadFromRemote>c__AnonStorey59 storey3 = new <LoadFromRemote>c__AnonStorey59 {
                <>f__ref$88 = storey,
                func = LuaScriptMgr.GetLuaFunction(storey.L, 5)
            };
            onLoaddowning = new Action<WWW, string>(storey3.<>m__26);
        }
        manager.LoadFromRemote(luaString, tag, onLoad, onLoaddowning);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadLua(IntPtr L)
    {
        <LoadLua>c__AnonStorey54 storey = new <LoadLua>c__AnonStorey54 {
            L = L
        };
        LuaScriptMgr.CheckArgsCount(storey.L, 4);
        ResourceManager manager = (ResourceManager) LuaScriptMgr.GetUnityObjectSelf(storey.L, 1, "com.tencent.pandora.ResourceManager");
        string luaString = LuaScriptMgr.GetLuaString(storey.L, 2);
        Dictionary<string, TextAsset> dics = (Dictionary<string, TextAsset>) LuaScriptMgr.GetNetObject(storey.L, 3, typeof(Dictionary<string, TextAsset>));
        Action<bool> func = null;
        if (LuaDLL.lua_type(storey.L, 4) != LuaTypes.LUA_TFUNCTION)
        {
            func = (Action<bool>) LuaScriptMgr.GetNetObject(storey.L, 4, typeof(Action<bool>));
        }
        else
        {
            <LoadLua>c__AnonStorey53 storey2 = new <LoadLua>c__AnonStorey53 {
                <>f__ref$84 = storey,
                func = LuaScriptMgr.GetLuaFunction(storey.L, 4)
            };
            func = new Action<bool>(storey2.<>m__23);
        }
        manager.LoadLua(luaString, dics, func);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int LoadUIFont(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        ResourceManager manager = (ResourceManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.ResourceManager");
        LuaTable luaTable = LuaScriptMgr.GetLuaTable(L, 2);
        manager.LoadUIFont(luaTable);
        return 0;
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
    private static int LuaInitToVm(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        ResourceManager manager = (ResourceManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.ResourceManager");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        manager.LuaInitToVm(luaString);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnUILuaLoaded(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        ResourceManager manager = (ResourceManager) LuaScriptMgr.GetUnityObjectSelf(L, 1, "com.tencent.pandora.ResourceManager");
        object varObject = LuaScriptMgr.GetVarObject(L, 2);
        manager.OnUILuaLoaded(varObject);
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("HasFont", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.HasFont)), new LuaMethod("InitAtlas", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.InitAtlas)), new LuaMethod("OnUILuaLoaded", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.OnUILuaLoaded)), new LuaMethod("LuaInitToVm", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.LuaInitToVm)), new LuaMethod("GetResPath", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.GetResPath)), new LuaMethod("LoadUIFont", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.LoadUIFont)), new LuaMethod("LoadLua", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.LoadLua)), new LuaMethod("LoadBundleRes", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.LoadBundleRes)), new LuaMethod("GetResFullName", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.GetResFullName)), new LuaMethod("LoadFromRemote", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.LoadFromRemote)), new LuaMethod("DownloadRes", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.DownloadRes)), new LuaMethod("getPlatformDesc", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.getPlatformDesc)), new LuaMethod("AssemplyUI", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.AssemplyUI)), new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap._Createcom_tencent_pandora_ResourceManager)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("actId", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_actId), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_actId)), new LuaField("useSA", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_useSA), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_useSA)), new LuaField("atlasUpdate", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_atlasUpdate), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_atlasUpdate)), new LuaField("passTimeSpan", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_passTimeSpan), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_passTimeSpan)), new LuaField("blUIResUnLoad", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_blUIResUnLoad), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_blUIResUnLoad)), new LuaField("startTimer", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_startTimer), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_startTimer)), new LuaField("isLuaBundleReady", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_isLuaBundleReady), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_isLuaBundleReady)), new LuaField("iLuaVer", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_iLuaVer), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_iLuaVer)), new LuaField("luaFiles", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_luaFiles), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_luaFiles)), new LuaField("mdictFont", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_mdictFont), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_mdictFont)), new LuaField("dicTryToLoad", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_dicTryToLoad), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_dicTryToLoad)), new LuaField("init_atlas", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_init_atlas), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_init_atlas)), new LuaField("needDownAtlas", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_needDownAtlas), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_needDownAtlas)), new LuaField("needDownFont", new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.get_needDownFont), new LuaCSFunction(com_tencent_pandora_ResourceManagerWrap.set_needDownFont)) };
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.ResourceManager", typeof(ResourceManager), regs, fields, typeof(View));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_actId(IntPtr L)
    {
        ResourceManager.actId = LuaScriptMgr.GetString(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_atlasUpdate(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name atlasUpdate");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index atlasUpdate on a nil value");
            }
        }
        luaObject.atlasUpdate = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_blUIResUnLoad(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name blUIResUnLoad");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index blUIResUnLoad on a nil value");
            }
        }
        luaObject.blUIResUnLoad = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_dicTryToLoad(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name dicTryToLoad");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index dicTryToLoad on a nil value");
            }
        }
        luaObject.dicTryToLoad = (Dictionary<string, int>) LuaScriptMgr.GetNetObject(L, 3, typeof(Dictionary<string, int>));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_iLuaVer(IntPtr L)
    {
        ResourceManager.iLuaVer = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_init_atlas(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name init_atlas");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index init_atlas on a nil value");
            }
        }
        luaObject.init_atlas = (AssetBundle) LuaScriptMgr.GetUnityObject(L, 3, typeof(AssetBundle));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_isLuaBundleReady(IntPtr L)
    {
        ResourceManager.isLuaBundleReady = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_luaFiles(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name luaFiles");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index luaFiles on a nil value");
            }
        }
        luaObject.luaFiles = (Dictionary<string, TextAsset>) LuaScriptMgr.GetNetObject(L, 3, typeof(Dictionary<string, TextAsset>));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_mdictFont(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name mdictFont");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index mdictFont on a nil value");
            }
        }
        luaObject.mdictFont = (Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetNetObject(L, 3, typeof(Dictionary<string, UnityEngine.Object>));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_needDownAtlas(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name needDownAtlas");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index needDownAtlas on a nil value");
            }
        }
        luaObject.needDownAtlas = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_needDownFont(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name needDownFont");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index needDownFont on a nil value");
            }
        }
        luaObject.needDownFont = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_passTimeSpan(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name passTimeSpan");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index passTimeSpan on a nil value");
            }
        }
        luaObject.passTimeSpan = (float) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_startTimer(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name startTimer");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index startTimer on a nil value");
            }
        }
        luaObject.startTimer = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_useSA(IntPtr L)
    {
        ResourceManager luaObject = (ResourceManager) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name useSA");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index useSA on a nil value");
            }
        }
        luaObject.useSA = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [CompilerGenerated]
    private sealed class <LoadBundleRes>c__AnonStorey55
    {
        internal com_tencent_pandora_ResourceManagerWrap.<LoadBundleRes>c__AnonStorey56 <>f__ref$86;
        internal LuaFunction func;

        internal void <>m__24(AssetBundle param0)
        {
            int oldTop = this.func.BeginPCall();
            LuaScriptMgr.Push(this.<>f__ref$86.L, param0);
            this.func.PCall(oldTop, 1);
            this.func.EndPCall(oldTop);
        }
    }

    [CompilerGenerated]
    private sealed class <LoadBundleRes>c__AnonStorey56
    {
        internal IntPtr L;
    }

    [CompilerGenerated]
    private sealed class <LoadFromRemote>c__AnonStorey57
    {
        internal com_tencent_pandora_ResourceManagerWrap.<LoadFromRemote>c__AnonStorey58 <>f__ref$88;
        internal LuaFunction func;

        internal void <>m__25(WWW param0, string param1)
        {
            int oldTop = this.func.BeginPCall();
            LuaScriptMgr.PushObject(this.<>f__ref$88.L, param0);
            LuaScriptMgr.Push(this.<>f__ref$88.L, param1);
            this.func.PCall(oldTop, 2);
            this.func.EndPCall(oldTop);
        }
    }

    [CompilerGenerated]
    private sealed class <LoadFromRemote>c__AnonStorey58
    {
        internal IntPtr L;
    }

    [CompilerGenerated]
    private sealed class <LoadFromRemote>c__AnonStorey59
    {
        internal com_tencent_pandora_ResourceManagerWrap.<LoadFromRemote>c__AnonStorey58 <>f__ref$88;
        internal LuaFunction func;

        internal void <>m__26(WWW param0, string param1)
        {
            int oldTop = this.func.BeginPCall();
            LuaScriptMgr.PushObject(this.<>f__ref$88.L, param0);
            LuaScriptMgr.Push(this.<>f__ref$88.L, param1);
            this.func.PCall(oldTop, 2);
            this.func.EndPCall(oldTop);
        }
    }

    [CompilerGenerated]
    private sealed class <LoadLua>c__AnonStorey53
    {
        internal com_tencent_pandora_ResourceManagerWrap.<LoadLua>c__AnonStorey54 <>f__ref$84;
        internal LuaFunction func;

        internal void <>m__23(bool param0)
        {
            int oldTop = this.func.BeginPCall();
            LuaScriptMgr.Push(this.<>f__ref$84.L, param0);
            this.func.PCall(oldTop, 1);
            this.func.EndPCall(oldTop);
        }
    }

    [CompilerGenerated]
    private sealed class <LoadLua>c__AnonStorey54
    {
        internal IntPtr L;
    }
}

