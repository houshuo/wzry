using com.tencent.pandora;
using System;
using UnityEngine;

public class ObjectWrap
{
    private static System.Type classType = typeof(UnityEngine.Object);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateObject(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            UnityEngine.Object obj2 = new UnityEngine.Object();
            LuaScriptMgr.Push(L, obj2);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Object.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Destroy(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                UnityEngine.Object luaObject = (UnityEngine.Object) LuaScriptMgr.GetLuaObject(L, 1);
                LuaScriptMgr.__gc(L);
                UnityEngine.Object.Destroy(luaObject);
                return 0;
            }
            case 2:
            {
                UnityEngine.Object obj3 = (UnityEngine.Object) LuaScriptMgr.GetLuaObject(L, 1);
                float number = (float) LuaScriptMgr.GetNumber(L, 2);
                UnityEngine.Object.Destroy(obj3, number);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Object.Destroy");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int DestroyImmediate(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                UnityEngine.Object luaObject = (UnityEngine.Object) LuaScriptMgr.GetLuaObject(L, 1);
                LuaScriptMgr.__gc(L);
                UnityEngine.Object.DestroyImmediate(luaObject);
                return 0;
            }
            case 2:
            {
                UnityEngine.Object obj3 = (UnityEngine.Object) LuaScriptMgr.GetLuaObject(L, 1);
                bool boolean = LuaScriptMgr.GetBoolean(L, 2);
                UnityEngine.Object.DestroyImmediate(obj3, boolean);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Object.DestroyImmediate");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int DestroyObject(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                UnityEngine.Object luaObject = (UnityEngine.Object) LuaScriptMgr.GetLuaObject(L, 1);
                LuaScriptMgr.__gc(L);
                UnityEngine.Object.DestroyObject(luaObject);
                return 0;
            }
            case 2:
            {
                UnityEngine.Object obj3 = (UnityEngine.Object) LuaScriptMgr.GetLuaObject(L, 1);
                float number = (float) LuaScriptMgr.GetNumber(L, 2);
                UnityEngine.Object.DestroyObject(obj3, number);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Object.DestroyObject");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int DontDestroyOnLoad(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        UnityEngine.Object.DontDestroyOnLoad(LuaScriptMgr.GetUnityObject(L, 1, typeof(UnityEngine.Object)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Equals(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        UnityEngine.Object varObject = LuaScriptMgr.GetVarObject(L, 1) as UnityEngine.Object;
        object o = LuaScriptMgr.GetVarObject(L, 2);
        bool b = (varObject == null) ? (o == null) : varObject.Equals(o);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindObjectOfType(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        UnityEngine.Object obj2 = UnityEngine.Object.FindObjectOfType(LuaScriptMgr.GetTypeObject(L, 1));
        LuaScriptMgr.Push(L, obj2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindObjectsOfType(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        UnityEngine.Object[] o = UnityEngine.Object.FindObjectsOfType(LuaScriptMgr.GetTypeObject(L, 1));
        LuaScriptMgr.PushArray(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_hideFlags(IntPtr L)
    {
        UnityEngine.Object luaObject = (UnityEngine.Object) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name hideFlags");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index hideFlags on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.hideFlags);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_name(IntPtr L)
    {
        UnityEngine.Object luaObject = (UnityEngine.Object) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name name");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index name on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.name);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetHashCode(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int hashCode = ((UnityEngine.Object) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Object")).GetHashCode();
        LuaScriptMgr.Push(L, hashCode);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetInstanceID(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int instanceID = ((UnityEngine.Object) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Object")).GetInstanceID();
        LuaScriptMgr.Push(L, instanceID);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Instantiate(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                UnityEngine.Object obj3 = UnityEngine.Object.Instantiate(LuaScriptMgr.GetUnityObject(L, 1, typeof(UnityEngine.Object)));
                LuaScriptMgr.Push(L, obj3);
                return 1;
            }
            case 3:
            {
                UnityEngine.Object original = LuaScriptMgr.GetUnityObject(L, 1, typeof(UnityEngine.Object));
                Vector3 position = LuaScriptMgr.GetVector3(L, 2);
                Quaternion rotation = LuaScriptMgr.GetQuaternion(L, 3);
                UnityEngine.Object obj5 = UnityEngine.Object.Instantiate(original, position, rotation);
                LuaScriptMgr.Push(L, obj5);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Object.Instantiate");
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
    private static int Lua_ToString(IntPtr L)
    {
        object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject != null)
        {
            LuaScriptMgr.Push(L, luaObject.ToString());
        }
        else
        {
            LuaScriptMgr.Push(L, "Table: UnityEngine.Object");
        }
        return 1;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("Equals", new LuaCSFunction(ObjectWrap.Equals)), new LuaMethod("GetHashCode", new LuaCSFunction(ObjectWrap.GetHashCode)), new LuaMethod("GetInstanceID", new LuaCSFunction(ObjectWrap.GetInstanceID)), new LuaMethod("Instantiate", new LuaCSFunction(ObjectWrap.Instantiate)), new LuaMethod("FindObjectsOfType", new LuaCSFunction(ObjectWrap.FindObjectsOfType)), new LuaMethod("FindObjectOfType", new LuaCSFunction(ObjectWrap.FindObjectOfType)), new LuaMethod("DontDestroyOnLoad", new LuaCSFunction(ObjectWrap.DontDestroyOnLoad)), new LuaMethod("ToString", new LuaCSFunction(ObjectWrap.ToString)), new LuaMethod("DestroyObject", new LuaCSFunction(ObjectWrap.DestroyObject)), new LuaMethod("DestroyImmediate", new LuaCSFunction(ObjectWrap.DestroyImmediate)), new LuaMethod("Destroy", new LuaCSFunction(ObjectWrap.Destroy)), new LuaMethod("New", new LuaCSFunction(ObjectWrap._CreateObject)), new LuaMethod("GetClassType", new LuaCSFunction(ObjectWrap.GetClassType)), new LuaMethod("__tostring", new LuaCSFunction(ObjectWrap.Lua_ToString)), new LuaMethod("__eq", new LuaCSFunction(ObjectWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("name", new LuaCSFunction(ObjectWrap.get_name), new LuaCSFunction(ObjectWrap.set_name)), new LuaField("hideFlags", new LuaCSFunction(ObjectWrap.get_hideFlags), new LuaCSFunction(ObjectWrap.set_hideFlags)) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.Object", typeof(UnityEngine.Object), regs, fields, typeof(object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_hideFlags(IntPtr L)
    {
        UnityEngine.Object luaObject = (UnityEngine.Object) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name hideFlags");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index hideFlags on a nil value");
            }
        }
        luaObject.hideFlags = (HideFlags) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(HideFlags)));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_name(IntPtr L)
    {
        UnityEngine.Object luaObject = (UnityEngine.Object) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name name");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index name on a nil value");
            }
        }
        luaObject.name = LuaScriptMgr.GetString(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ToString(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = ((UnityEngine.Object) LuaScriptMgr.GetUnityObjectSelf(L, 1, "Object")).ToString();
        LuaScriptMgr.Push(L, str);
        return 1;
    }
}

