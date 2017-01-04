using com.tencent.pandora;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Dictionary_string_ObjectWrap
{
    private static System.Type classType = typeof(Dictionary<string, UnityEngine.Object>);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateDictionary_string_Object(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 0)
        {
            Dictionary<string, UnityEngine.Object> o = new Dictionary<string, UnityEngine.Object>();
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(int)))
        {
            int number = (int) LuaScriptMgr.GetNumber(L, 1);
            Dictionary<string, UnityEngine.Object> dictionary2 = new Dictionary<string, UnityEngine.Object>(number);
            LuaScriptMgr.PushObject(L, dictionary2);
            return 1;
        }
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(IDictionary<string, UnityEngine.Object>)))
        {
            IDictionary<string, UnityEngine.Object> dictionary = (IDictionary<string, UnityEngine.Object>) LuaScriptMgr.GetNetObject(L, 1, typeof(IDictionary<string, UnityEngine.Object>));
            Dictionary<string, UnityEngine.Object> dictionary4 = new Dictionary<string, UnityEngine.Object>(dictionary);
            LuaScriptMgr.PushObject(L, dictionary4);
            return 1;
        }
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(IEqualityComparer<string>)))
        {
            IEqualityComparer<string> comparer = (IEqualityComparer<string>) LuaScriptMgr.GetNetObject(L, 1, typeof(IEqualityComparer<string>));
            Dictionary<string, UnityEngine.Object> dictionary5 = new Dictionary<string, UnityEngine.Object>(comparer);
            LuaScriptMgr.PushObject(L, dictionary5);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(int), typeof(IEqualityComparer<string>)))
        {
            int capacity = (int) LuaScriptMgr.GetNumber(L, 1);
            IEqualityComparer<string> comparer2 = (IEqualityComparer<string>) LuaScriptMgr.GetNetObject(L, 2, typeof(IEqualityComparer<string>));
            Dictionary<string, UnityEngine.Object> dictionary6 = new Dictionary<string, UnityEngine.Object>(capacity, comparer2);
            LuaScriptMgr.PushObject(L, dictionary6);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(IDictionary<string, UnityEngine.Object>), typeof(IEqualityComparer<string>)))
        {
            IDictionary<string, UnityEngine.Object> dictionary7 = (IDictionary<string, UnityEngine.Object>) LuaScriptMgr.GetNetObject(L, 1, typeof(IDictionary<string, UnityEngine.Object>));
            IEqualityComparer<string> comparer3 = (IEqualityComparer<string>) LuaScriptMgr.GetNetObject(L, 2, typeof(IEqualityComparer<string>));
            Dictionary<string, UnityEngine.Object> dictionary8 = new Dictionary<string, UnityEngine.Object>(dictionary7, comparer3);
            LuaScriptMgr.PushObject(L, dictionary8);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Dictionary<string,Object>.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Add(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        Dictionary<string, UnityEngine.Object> dictionary = (Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetNetObjectSelf(L, 1, "Dictionary<string,Object>");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        UnityEngine.Object obj2 = LuaScriptMgr.GetUnityObject(L, 3, typeof(UnityEngine.Object));
        dictionary.Add(luaString, obj2);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Clear(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetNetObjectSelf(L, 1, "Dictionary<string,Object>")).Clear();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ContainsKey(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Dictionary<string, UnityEngine.Object> dictionary = (Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetNetObjectSelf(L, 1, "Dictionary<string,Object>");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        bool b = dictionary.ContainsKey(luaString);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ContainsValue(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Dictionary<string, UnityEngine.Object> dictionary = (Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetNetObjectSelf(L, 1, "Dictionary<string,Object>");
        UnityEngine.Object obj2 = LuaScriptMgr.GetUnityObject(L, 2, typeof(UnityEngine.Object));
        bool b = dictionary.ContainsValue(obj2);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Comparer(IntPtr L)
    {
        Dictionary<string, UnityEngine.Object> luaObject = (Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name Comparer");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index Comparer on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.Comparer);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Count(IntPtr L)
    {
        Dictionary<string, UnityEngine.Object> luaObject = (Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name Count");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index Count on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.Count);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Item(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Dictionary<string, UnityEngine.Object> dictionary = (Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetNetObjectSelf(L, 1, "Dictionary<string,Object>");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        UnityEngine.Object obj2 = dictionary[luaString];
        LuaScriptMgr.Push(L, obj2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Keys(IntPtr L)
    {
        Dictionary<string, UnityEngine.Object> luaObject = (Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name Keys");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index Keys on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.Keys);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Values(IntPtr L)
    {
        Dictionary<string, UnityEngine.Object> luaObject = (Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name Values");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index Values on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.Values);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetEnumerator(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Dictionary<string, UnityEngine.Object>.Enumerator o = ((Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetNetObjectSelf(L, 1, "Dictionary<string,Object>")).GetEnumerator();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetObjectData(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        Dictionary<string, UnityEngine.Object> dictionary = (Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetNetObjectSelf(L, 1, "Dictionary<string,Object>");
        SerializationInfo info = (SerializationInfo) LuaScriptMgr.GetNetObject(L, 2, typeof(SerializationInfo));
        StreamingContext context = (StreamingContext) LuaScriptMgr.GetNetObject(L, 3, typeof(StreamingContext));
        dictionary.GetObjectData(info, context);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int OnDeserialization(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Dictionary<string, UnityEngine.Object> dictionary = (Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetNetObjectSelf(L, 1, "Dictionary<string,Object>");
        object varObject = LuaScriptMgr.GetVarObject(L, 2);
        dictionary.OnDeserialization(varObject);
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("get_Item", new LuaCSFunction(Dictionary_string_ObjectWrap.get_Item)), new LuaMethod("set_Item", new LuaCSFunction(Dictionary_string_ObjectWrap.set_Item)), new LuaMethod("Add", new LuaCSFunction(Dictionary_string_ObjectWrap.Add)), new LuaMethod("Clear", new LuaCSFunction(Dictionary_string_ObjectWrap.Clear)), new LuaMethod("ContainsKey", new LuaCSFunction(Dictionary_string_ObjectWrap.ContainsKey)), new LuaMethod("ContainsValue", new LuaCSFunction(Dictionary_string_ObjectWrap.ContainsValue)), new LuaMethod("GetObjectData", new LuaCSFunction(Dictionary_string_ObjectWrap.GetObjectData)), new LuaMethod("OnDeserialization", new LuaCSFunction(Dictionary_string_ObjectWrap.OnDeserialization)), new LuaMethod("Remove", new LuaCSFunction(Dictionary_string_ObjectWrap.Remove)), new LuaMethod("TryGetValue", new LuaCSFunction(Dictionary_string_ObjectWrap.TryGetValue)), new LuaMethod("GetEnumerator", new LuaCSFunction(Dictionary_string_ObjectWrap.GetEnumerator)), new LuaMethod("New", new LuaCSFunction(Dictionary_string_ObjectWrap._CreateDictionary_string_Object)), new LuaMethod("GetClassType", new LuaCSFunction(Dictionary_string_ObjectWrap.GetClassType)) };
        LuaField[] fields = new LuaField[] { new LuaField("Count", new LuaCSFunction(Dictionary_string_ObjectWrap.get_Count), null), new LuaField("Comparer", new LuaCSFunction(Dictionary_string_ObjectWrap.get_Comparer), null), new LuaField("Keys", new LuaCSFunction(Dictionary_string_ObjectWrap.get_Keys), null), new LuaField("Values", new LuaCSFunction(Dictionary_string_ObjectWrap.get_Values), null) };
        LuaScriptMgr.RegisterLib(L, "Dictionary_string_Object", typeof(Dictionary<string, UnityEngine.Object>), regs, fields, typeof(object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Remove(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Dictionary<string, UnityEngine.Object> dictionary = (Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetNetObjectSelf(L, 1, "Dictionary<string,Object>");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        bool b = dictionary.Remove(luaString);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_Item(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        Dictionary<string, UnityEngine.Object> dictionary = (Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetNetObjectSelf(L, 1, "Dictionary<string,Object>");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        UnityEngine.Object obj2 = LuaScriptMgr.GetUnityObject(L, 3, typeof(UnityEngine.Object));
        dictionary[luaString] = obj2;
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int TryGetValue(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        Dictionary<string, UnityEngine.Object> dictionary = (Dictionary<string, UnityEngine.Object>) LuaScriptMgr.GetNetObjectSelf(L, 1, "Dictionary<string,Object>");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        UnityEngine.Object obj2 = null;
        bool b = dictionary.TryGetValue(luaString, out obj2);
        LuaScriptMgr.Push(L, b);
        LuaScriptMgr.Push(L, obj2);
        return 2;
    }
}

