using com.tencent.pandora;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectWrap
{
    private static System.Type classType = typeof(GameObject);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateGameObject(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        switch (num)
        {
            case 0:
            {
                GameObject obj2 = new GameObject();
                LuaScriptMgr.Push(L, obj2);
                return 1;
            }
            case 1:
            {
                GameObject obj3 = new GameObject(LuaScriptMgr.GetString(L, 1));
                LuaScriptMgr.Push(L, obj3);
                return 1;
            }
        }
        if (LuaScriptMgr.CheckTypes(L, 1, typeof(string)) && LuaScriptMgr.CheckParamsType(L, typeof(System.Type), 2, num - 1))
        {
            string name = LuaScriptMgr.GetString(L, 1);
            System.Type[] components = LuaScriptMgr.GetParamsObject<System.Type>(L, 2, num - 1);
            GameObject obj4 = new GameObject(name, components);
            LuaScriptMgr.Push(L, obj4);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int AddComponent(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(System.Type)))
        {
            GameObject obj2 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
            Component component = obj2.AddComponent(typeObject);
            LuaScriptMgr.Push(L, component);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string)))
        {
            GameObject obj3 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            string str = LuaScriptMgr.GetString(L, 2);
            Component component2 = obj3.AddComponent(str);
            LuaScriptMgr.Push(L, component2);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.AddComponent");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int BroadcastMessage(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 2)
        {
            GameObject obj2 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            string luaString = LuaScriptMgr.GetLuaString(L, 2);
            obj2.BroadcastMessage(luaString);
            return 0;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string), typeof(SendMessageOptions)))
        {
            GameObject obj3 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            string methodName = LuaScriptMgr.GetString(L, 2);
            SendMessageOptions luaObject = (SendMessageOptions) ((int) LuaScriptMgr.GetLuaObject(L, 3));
            obj3.BroadcastMessage(methodName, luaObject);
            return 0;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string), typeof(object)))
        {
            GameObject obj4 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            string str3 = LuaScriptMgr.GetString(L, 2);
            object varObject = LuaScriptMgr.GetVarObject(L, 3);
            obj4.BroadcastMessage(str3, varObject);
            return 0;
        }
        if (num == 4)
        {
            GameObject obj6 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            string str4 = LuaScriptMgr.GetLuaString(L, 2);
            object parameter = LuaScriptMgr.GetVarObject(L, 3);
            SendMessageOptions options = (SendMessageOptions) ((int) LuaScriptMgr.GetNetObject(L, 4, typeof(SendMessageOptions)));
            obj6.BroadcastMessage(str4, parameter, options);
            return 0;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.BroadcastMessage");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CompareTag(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        GameObject obj2 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        bool b = obj2.CompareTag(luaString);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int CreatePrimitive(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        PrimitiveType type = (PrimitiveType) ((int) LuaScriptMgr.GetNetObject(L, 1, typeof(PrimitiveType)));
        GameObject obj2 = GameObject.CreatePrimitive(type);
        LuaScriptMgr.Push(L, obj2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Find(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        GameObject obj2 = GameObject.Find(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, obj2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindGameObjectsWithTag(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        GameObject[] o = GameObject.FindGameObjectsWithTag(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.PushArray(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindGameObjectWithTag(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        GameObject obj2 = GameObject.FindGameObjectWithTag(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, obj2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindWithTag(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        GameObject obj2 = GameObject.FindWithTag(LuaScriptMgr.GetLuaString(L, 1));
        LuaScriptMgr.Push(L, obj2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_activeInHierarchy(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name activeInHierarchy");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index activeInHierarchy on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.activeInHierarchy);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_activeSelf(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name activeSelf");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index activeSelf on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.activeSelf);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_animation(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name animation");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index animation on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_animation());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_audio(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name audio");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index audio on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_audio());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_camera(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name camera");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index camera on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_camera());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_collider(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name collider");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index collider on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_collider());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_collider2D(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name collider2D");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index collider2D on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_collider2D());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_constantForce(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name constantForce");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index constantForce on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_constantForce());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_gameObject(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name gameObject");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index gameObject on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.gameObject);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_guiText(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name guiText");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index guiText on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_guiText());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_guiTexture(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name guiTexture");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index guiTexture on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_guiTexture());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_hingeJoint(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name hingeJoint");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index hingeJoint on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_hingeJoint());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_isStatic(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isStatic");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isStatic on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.isStatic);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_layer(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name layer");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index layer on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.layer);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_light(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name light");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index light on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_light());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_networkView(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name networkView");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index networkView on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_networkView());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_particleEmitter(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name particleEmitter");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index particleEmitter on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_particleEmitter());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_particleSystem(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name particleSystem");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index particleSystem on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_particleSystem());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_renderer(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name renderer");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index renderer on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_renderer());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_rigidbody(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name rigidbody");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index rigidbody on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_rigidbody());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_rigidbody2D(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name rigidbody2D");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index rigidbody2D on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.get_rigidbody2D());
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_tag(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name tag");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index tag on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.tag);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_transform(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name transform");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index transform on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.transform);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetComponent(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string)))
        {
            GameObject obj2 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            string str = LuaScriptMgr.GetString(L, 2);
            Component component = obj2.GetComponent(str);
            LuaScriptMgr.Push(L, component);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(System.Type)))
        {
            GameObject obj3 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
            Component component2 = obj3.GetComponent(typeObject);
            LuaScriptMgr.Push(L, component2);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.GetComponent");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetComponentInChildren(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        GameObject obj2 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
        System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
        Component componentInChildren = obj2.GetComponentInChildren(typeObject);
        LuaScriptMgr.Push(L, componentInChildren);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetComponentInParent(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        GameObject obj2 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
        System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
        Component componentInParent = obj2.GetComponentInParent(typeObject);
        LuaScriptMgr.Push(L, componentInParent);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetComponents(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                GameObject obj2 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
                Component[] components = obj2.GetComponents(typeObject);
                LuaScriptMgr.PushArray(L, components);
                return 1;
            }
            case 3:
            {
                GameObject obj3 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
                System.Type type = LuaScriptMgr.GetTypeObject(L, 2);
                List<Component> results = (List<Component>) LuaScriptMgr.GetNetObject(L, 3, typeof(List<Component>));
                obj3.GetComponents(type, results);
                return 0;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.GetComponents");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetComponentsInChildren(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                GameObject obj2 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
                Component[] componentsInChildren = obj2.GetComponentsInChildren(typeObject);
                LuaScriptMgr.PushArray(L, componentsInChildren);
                return 1;
            }
            case 3:
            {
                GameObject obj3 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
                System.Type type = LuaScriptMgr.GetTypeObject(L, 2);
                bool boolean = LuaScriptMgr.GetBoolean(L, 3);
                Component[] o = obj3.GetComponentsInChildren(type, boolean);
                LuaScriptMgr.PushArray(L, o);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.GetComponentsInChildren");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetComponentsInParent(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                GameObject obj2 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
                Component[] componentsInParent = obj2.GetComponentsInParent(typeObject);
                LuaScriptMgr.PushArray(L, componentsInParent);
                return 1;
            }
            case 3:
            {
                GameObject obj3 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
                System.Type type = LuaScriptMgr.GetTypeObject(L, 2);
                bool boolean = LuaScriptMgr.GetBoolean(L, 3);
                Component[] o = obj3.GetComponentsInParent(type, boolean);
                LuaScriptMgr.PushArray(L, o);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.GetComponentsInParent");
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

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { 
            new LuaMethod("SampleAnimation", new LuaCSFunction(GameObjectWrap.SampleAnimation)), new LuaMethod("CreatePrimitive", new LuaCSFunction(GameObjectWrap.CreatePrimitive)), new LuaMethod("GetComponent", new LuaCSFunction(GameObjectWrap.GetComponent)), new LuaMethod("GetComponentInChildren", new LuaCSFunction(GameObjectWrap.GetComponentInChildren)), new LuaMethod("GetComponentInParent", new LuaCSFunction(GameObjectWrap.GetComponentInParent)), new LuaMethod("GetComponents", new LuaCSFunction(GameObjectWrap.GetComponents)), new LuaMethod("GetComponentsInChildren", new LuaCSFunction(GameObjectWrap.GetComponentsInChildren)), new LuaMethod("GetComponentsInParent", new LuaCSFunction(GameObjectWrap.GetComponentsInParent)), new LuaMethod("SetActive", new LuaCSFunction(GameObjectWrap.SetActive)), new LuaMethod("CompareTag", new LuaCSFunction(GameObjectWrap.CompareTag)), new LuaMethod("FindGameObjectWithTag", new LuaCSFunction(GameObjectWrap.FindGameObjectWithTag)), new LuaMethod("FindWithTag", new LuaCSFunction(GameObjectWrap.FindWithTag)), new LuaMethod("FindGameObjectsWithTag", new LuaCSFunction(GameObjectWrap.FindGameObjectsWithTag)), new LuaMethod("SendMessageUpwards", new LuaCSFunction(GameObjectWrap.SendMessageUpwards)), new LuaMethod("SendMessage", new LuaCSFunction(GameObjectWrap.SendMessage)), new LuaMethod("BroadcastMessage", new LuaCSFunction(GameObjectWrap.BroadcastMessage)), 
            new LuaMethod("AddComponent", new LuaCSFunction(GameObjectWrap.AddComponent)), new LuaMethod("Find", new LuaCSFunction(GameObjectWrap.Find)), new LuaMethod("New", new LuaCSFunction(GameObjectWrap._CreateGameObject)), new LuaMethod("GetClassType", new LuaCSFunction(GameObjectWrap.GetClassType)), new LuaMethod("__eq", new LuaCSFunction(GameObjectWrap.Lua_Eq))
         };
        LuaField[] fields = new LuaField[] { 
            new LuaField("isStatic", new LuaCSFunction(GameObjectWrap.get_isStatic), new LuaCSFunction(GameObjectWrap.set_isStatic)), new LuaField("transform", new LuaCSFunction(GameObjectWrap.get_transform), null), new LuaField("rigidbody", new LuaCSFunction(GameObjectWrap.get_rigidbody), null), new LuaField("rigidbody2D", new LuaCSFunction(GameObjectWrap.get_rigidbody2D), null), new LuaField("camera", new LuaCSFunction(GameObjectWrap.get_camera), null), new LuaField("light", new LuaCSFunction(GameObjectWrap.get_light), null), new LuaField("animation", new LuaCSFunction(GameObjectWrap.get_animation), null), new LuaField("constantForce", new LuaCSFunction(GameObjectWrap.get_constantForce), null), new LuaField("renderer", new LuaCSFunction(GameObjectWrap.get_renderer), null), new LuaField("audio", new LuaCSFunction(GameObjectWrap.get_audio), null), new LuaField("guiText", new LuaCSFunction(GameObjectWrap.get_guiText), null), new LuaField("networkView", new LuaCSFunction(GameObjectWrap.get_networkView), null), new LuaField("guiTexture", new LuaCSFunction(GameObjectWrap.get_guiTexture), null), new LuaField("collider", new LuaCSFunction(GameObjectWrap.get_collider), null), new LuaField("collider2D", new LuaCSFunction(GameObjectWrap.get_collider2D), null), new LuaField("hingeJoint", new LuaCSFunction(GameObjectWrap.get_hingeJoint), null), 
            new LuaField("particleEmitter", new LuaCSFunction(GameObjectWrap.get_particleEmitter), null), new LuaField("particleSystem", new LuaCSFunction(GameObjectWrap.get_particleSystem), null), new LuaField("layer", new LuaCSFunction(GameObjectWrap.get_layer), new LuaCSFunction(GameObjectWrap.set_layer)), new LuaField("activeSelf", new LuaCSFunction(GameObjectWrap.get_activeSelf), null), new LuaField("activeInHierarchy", new LuaCSFunction(GameObjectWrap.get_activeInHierarchy), null), new LuaField("tag", new LuaCSFunction(GameObjectWrap.get_tag), new LuaCSFunction(GameObjectWrap.set_tag)), new LuaField("gameObject", new LuaCSFunction(GameObjectWrap.get_gameObject), null)
         };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.GameObject", typeof(GameObject), regs, fields, typeof(UnityEngine.Object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SampleAnimation(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        GameObject obj2 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
        AnimationClip clip = (AnimationClip) LuaScriptMgr.GetUnityObject(L, 2, typeof(AnimationClip));
        float number = (float) LuaScriptMgr.GetNumber(L, 3);
        obj2.SampleAnimation(clip, number);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SendMessage(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 2)
        {
            GameObject obj2 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            string luaString = LuaScriptMgr.GetLuaString(L, 2);
            obj2.SendMessage(luaString);
            return 0;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string), typeof(SendMessageOptions)))
        {
            GameObject obj3 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            string methodName = LuaScriptMgr.GetString(L, 2);
            SendMessageOptions luaObject = (SendMessageOptions) ((int) LuaScriptMgr.GetLuaObject(L, 3));
            obj3.SendMessage(methodName, luaObject);
            return 0;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string), typeof(object)))
        {
            GameObject obj4 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            string str3 = LuaScriptMgr.GetString(L, 2);
            object varObject = LuaScriptMgr.GetVarObject(L, 3);
            obj4.SendMessage(str3, varObject);
            return 0;
        }
        if (num == 4)
        {
            GameObject obj6 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            string str4 = LuaScriptMgr.GetLuaString(L, 2);
            object obj7 = LuaScriptMgr.GetVarObject(L, 3);
            SendMessageOptions options = (SendMessageOptions) ((int) LuaScriptMgr.GetNetObject(L, 4, typeof(SendMessageOptions)));
            obj6.SendMessage(str4, obj7, options);
            return 0;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.SendMessage");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SendMessageUpwards(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 2)
        {
            GameObject obj2 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            string luaString = LuaScriptMgr.GetLuaString(L, 2);
            obj2.SendMessageUpwards(luaString);
            return 0;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string), typeof(SendMessageOptions)))
        {
            GameObject obj3 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            string methodName = LuaScriptMgr.GetString(L, 2);
            SendMessageOptions luaObject = (SendMessageOptions) ((int) LuaScriptMgr.GetLuaObject(L, 3));
            obj3.SendMessageUpwards(methodName, luaObject);
            return 0;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string), typeof(object)))
        {
            GameObject obj4 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            string str3 = LuaScriptMgr.GetString(L, 2);
            object varObject = LuaScriptMgr.GetVarObject(L, 3);
            obj4.SendMessageUpwards(str3, varObject);
            return 0;
        }
        if (num == 4)
        {
            GameObject obj6 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
            string str4 = LuaScriptMgr.GetLuaString(L, 2);
            object obj7 = LuaScriptMgr.GetVarObject(L, 3);
            SendMessageOptions options = (SendMessageOptions) ((int) LuaScriptMgr.GetNetObject(L, 4, typeof(SendMessageOptions)));
            obj6.SendMessageUpwards(str4, obj7, options);
            return 0;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.SendMessageUpwards");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_isStatic(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name isStatic");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index isStatic on a nil value");
            }
        }
        luaObject.isStatic = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_layer(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name layer");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index layer on a nil value");
            }
        }
        luaObject.layer = (int) LuaScriptMgr.GetNumber(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_tag(IntPtr L)
    {
        GameObject luaObject = (GameObject) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name tag");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index tag on a nil value");
            }
        }
        luaObject.tag = LuaScriptMgr.GetString(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SetActive(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        GameObject obj2 = (GameObject) LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
        bool boolean = LuaScriptMgr.GetBoolean(L, 2);
        obj2.SetActive(boolean);
        return 0;
    }
}

