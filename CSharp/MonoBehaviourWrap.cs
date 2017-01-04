using com.tencent.pandora;
using System;
using System.Collections;
using UnityEngine;

public class MonoBehaviourWrap
{
    private static System.Type classType = typeof(MonoBehaviour);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateMonoBehaviour(IntPtr L)
    {
        LuaDLL.luaL_error(L, "MonoBehaviour class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CancelInvoke(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
                ((MonoBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "MonoBehaviour")).CancelInvoke();
                return 0;

            case 2:
            {
                MonoBehaviour behaviour2 = (MonoBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "MonoBehaviour");
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                behaviour2.CancelInvoke(luaString);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: MonoBehaviour.CancelInvoke");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_useGUILayout(IntPtr L)
    {
        MonoBehaviour luaObject = (MonoBehaviour) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name useGUILayout");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index useGUILayout on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.useGUILayout);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Invoke(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        MonoBehaviour behaviour = (MonoBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "MonoBehaviour");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        float number = (float) LuaScriptMgr.GetNumber(L, 3);
        behaviour.Invoke(luaString, number);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int InvokeRepeating(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 4);
        MonoBehaviour behaviour = (MonoBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "MonoBehaviour");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        float number = (float) LuaScriptMgr.GetNumber(L, 3);
        float repeatRate = (float) LuaScriptMgr.GetNumber(L, 4);
        behaviour.InvokeRepeating(luaString, number, repeatRate);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsInvoking(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                bool b = ((MonoBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "MonoBehaviour")).IsInvoking();
                LuaScriptMgr.Push(L, b);
                return 1;
            }
            case 2:
            {
                MonoBehaviour behaviour2 = (MonoBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "MonoBehaviour");
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                bool flag2 = behaviour2.IsInvoking(luaString);
                LuaScriptMgr.Push(L, flag2);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: MonoBehaviour.IsInvoking");
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
    private static int print(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        MonoBehaviour.print(LuaScriptMgr.GetVarObject(L, 1));
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { new LuaMethod("Invoke", new LuaCSFunction(MonoBehaviourWrap.Invoke)), new LuaMethod("InvokeRepeating", new LuaCSFunction(MonoBehaviourWrap.InvokeRepeating)), new LuaMethod("CancelInvoke", new LuaCSFunction(MonoBehaviourWrap.CancelInvoke)), new LuaMethod("IsInvoking", new LuaCSFunction(MonoBehaviourWrap.IsInvoking)), new LuaMethod("StartCoroutine", new LuaCSFunction(MonoBehaviourWrap.StartCoroutine)), new LuaMethod("StartCoroutine_Auto", new LuaCSFunction(MonoBehaviourWrap.StartCoroutine_Auto)), new LuaMethod("StopCoroutine", new LuaCSFunction(MonoBehaviourWrap.StopCoroutine)), new LuaMethod("StopAllCoroutines", new LuaCSFunction(MonoBehaviourWrap.StopAllCoroutines)), new LuaMethod("print", new LuaCSFunction(MonoBehaviourWrap.print)), new LuaMethod("New", new LuaCSFunction(MonoBehaviourWrap._CreateMonoBehaviour)), new LuaMethod("GetClassType", new LuaCSFunction(MonoBehaviourWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(MonoBehaviourWrap.Lua_Eq)) };
        LuaField[] fields = new LuaField[] { new LuaField("useGUILayout", new LuaCSFunction(MonoBehaviourWrap.get_useGUILayout), new LuaCSFunction(MonoBehaviourWrap.set_useGUILayout)) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.MonoBehaviour", typeof(MonoBehaviour), regs, fields, typeof(Behaviour));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_useGUILayout(IntPtr L)
    {
        MonoBehaviour luaObject = (MonoBehaviour) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name useGUILayout");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index useGUILayout on a nil value");
            }
        }
        luaObject.useGUILayout = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int StartCoroutine(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(MonoBehaviour), typeof(string)))
        {
            MonoBehaviour behaviour = (MonoBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "MonoBehaviour");
            string methodName = LuaScriptMgr.GetString(L, 2);
            Coroutine o = behaviour.StartCoroutine(methodName);
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(MonoBehaviour), typeof(IEnumerator)))
        {
            MonoBehaviour behaviour2 = (MonoBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "MonoBehaviour");
            IEnumerator luaObject = (IEnumerator) LuaScriptMgr.GetLuaObject(L, 2);
            Coroutine coroutine2 = behaviour2.StartCoroutine(luaObject);
            LuaScriptMgr.PushObject(L, coroutine2);
            return 1;
        }
        if (num == 3)
        {
            MonoBehaviour behaviour3 = (MonoBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "MonoBehaviour");
            string luaString = LuaScriptMgr.GetLuaString(L, 2);
            object varObject = LuaScriptMgr.GetVarObject(L, 3);
            Coroutine coroutine3 = behaviour3.StartCoroutine(luaString, varObject);
            LuaScriptMgr.PushObject(L, coroutine3);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: MonoBehaviour.StartCoroutine");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int StartCoroutine_Auto(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        MonoBehaviour behaviour = (MonoBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "MonoBehaviour");
        IEnumerator routine = (IEnumerator) LuaScriptMgr.GetNetObject(L, 2, typeof(IEnumerator));
        Coroutine o = behaviour.StartCoroutine_Auto(routine);
        LuaScriptMgr.PushObject(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int StopAllCoroutines(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((MonoBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "MonoBehaviour")).StopAllCoroutines();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int StopCoroutine(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(MonoBehaviour), typeof(Coroutine)))
        {
            MonoBehaviour behaviour = (MonoBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "MonoBehaviour");
            Coroutine luaObject = (Coroutine) LuaScriptMgr.GetLuaObject(L, 2);
            behaviour.StopCoroutine(luaObject);
            return 0;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(MonoBehaviour), typeof(IEnumerator)))
        {
            MonoBehaviour behaviour2 = (MonoBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "MonoBehaviour");
            IEnumerator routine = (IEnumerator) LuaScriptMgr.GetLuaObject(L, 2);
            behaviour2.StopCoroutine(routine);
            return 0;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(MonoBehaviour), typeof(string)))
        {
            MonoBehaviour behaviour3 = (MonoBehaviour) LuaScriptMgr.GetUnityObjectSelf(L, 1, "MonoBehaviour");
            string methodName = LuaScriptMgr.GetString(L, 2);
            behaviour3.StopCoroutine(methodName);
            return 0;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: MonoBehaviour.StopCoroutine");
        return 0;
    }
}

