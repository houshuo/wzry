using com.tencent.pandora;
using System;
using UnityEngine;

public class TextureWrap
{
    private static System.Type classType = typeof(Texture);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateTexture(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            Texture texture = new Texture();
            LuaScriptMgr.Push(L, texture);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Texture.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_anisoLevel(IntPtr L)
    {
        Texture luaObject = (Texture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name anisoLevel");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index anisoLevel on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.anisoLevel);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_anisotropicFiltering(IntPtr L)
    {
        LuaScriptMgr.Push(L, Texture.anisotropicFiltering);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_filterMode(IntPtr L)
    {
        Texture luaObject = (Texture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name filterMode");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index filterMode on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.filterMode);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_height(IntPtr L)
    {
        Texture luaObject = (Texture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name height");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index height on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.height);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_masterTextureLimit(IntPtr L)
    {
        LuaScriptMgr.Push(L, Texture.masterTextureLimit);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_mipMapBias(IntPtr L)
    {
        Texture luaObject = (Texture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name mipMapBias");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index mipMapBias on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.mipMapBias);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_texelSize(IntPtr L)
    {
        Texture luaObject = (Texture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name texelSize");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index texelSize on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.texelSize);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_width(IntPtr L)
    {
        Texture luaObject = (Texture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name width");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index width on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.width);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_wrapMode(IntPtr L)
    {
        Texture luaObject = (Texture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name wrapMode");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index wrapMode on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.wrapMode);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetNativeTextureID(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int nativeTextureID = ((Texture) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Texture")).GetNativeTextureID();
        LuaScriptMgr.Push(L, nativeTextureID);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetNativeTexturePtr(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        IntPtr nativeTexturePtr = ((Texture) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Texture")).GetNativeTexturePtr();
        LuaScriptMgr.Push(L, nativeTexturePtr);
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
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("SetGlobalAnisotropicFilteringLimits", new LuaCSFunction(TextureWrap.SetGlobalAnisotropicFilteringLimits)), new LuaMethod("GetNativeTexturePtr", new LuaCSFunction(TextureWrap.GetNativeTexturePtr)), new LuaMethod("GetNativeTextureID", new LuaCSFunction(TextureWrap.GetNativeTextureID)), new LuaMethod("New", new LuaCSFunction(TextureWrap._CreateTexture)), new LuaMethod("GetClassType", new LuaCSFunction(TextureWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(TextureWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("masterTextureLimit", new LuaCSFunction(TextureWrap.get_masterTextureLimit), new LuaCSFunction(TextureWrap.set_masterTextureLimit)), new LuaField("anisotropicFiltering", new LuaCSFunction(TextureWrap.get_anisotropicFiltering), new LuaCSFunction(TextureWrap.set_anisotropicFiltering)), new LuaField("width", new LuaCSFunction(TextureWrap.get_width), new LuaCSFunction(TextureWrap.set_width)), new LuaField("height", new LuaCSFunction(TextureWrap.get_height), new LuaCSFunction(TextureWrap.set_height)), new LuaField("filterMode", new LuaCSFunction(TextureWrap.get_filterMode), new LuaCSFunction(TextureWrap.set_filterMode)), new LuaField("anisoLevel", new LuaCSFunction(TextureWrap.get_anisoLevel), new LuaCSFunction(TextureWrap.set_anisoLevel)), new LuaField("wrapMode", new LuaCSFunction(TextureWrap.get_wrapMode), new LuaCSFunction(TextureWrap.set_wrapMode)), new LuaField("mipMapBias", new LuaCSFunction(TextureWrap.get_mipMapBias), new LuaCSFunction(TextureWrap.set_mipMapBias)), new LuaField("texelSize", new LuaCSFunction(TextureWrap.get_texelSize), null) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.Texture", typeof(Texture), regs, fields, typeof(UnityEngine.Object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_anisoLevel(IntPtr L)
    {
        Texture luaObject = (Texture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name anisoLevel");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index anisoLevel on a nil value");
            }
        }
        luaObject.anisoLevel = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_anisotropicFiltering(IntPtr L)
    {
        Texture.anisotropicFiltering = (AnisotropicFiltering) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(AnisotropicFiltering)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_filterMode(IntPtr L)
    {
        Texture luaObject = (Texture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name filterMode");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index filterMode on a nil value");
            }
        }
        luaObject.filterMode = (FilterMode) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(FilterMode)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_height(IntPtr L)
    {
        Texture luaObject = (Texture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name height");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index height on a nil value");
            }
        }
        luaObject.height = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_masterTextureLimit(IntPtr L)
    {
        Texture.masterTextureLimit = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_mipMapBias(IntPtr L)
    {
        Texture luaObject = (Texture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name mipMapBias");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index mipMapBias on a nil value");
            }
        }
        luaObject.mipMapBias = (float) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_width(IntPtr L)
    {
        Texture luaObject = (Texture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name width");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index width on a nil value");
            }
        }
        luaObject.width = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_wrapMode(IntPtr L)
    {
        Texture luaObject = (Texture) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name wrapMode");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index wrapMode on a nil value");
            }
        }
        luaObject.wrapMode = (TextureWrapMode) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(TextureWrapMode)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetGlobalAnisotropicFilteringLimits(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        int globalMax = (int) LuaScriptMgr.GetNumber(L, 2);
        Texture.SetGlobalAnisotropicFilteringLimits(number, globalMax);
        return 0;
    }
}

