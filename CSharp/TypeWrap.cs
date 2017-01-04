using com.tencent.pandora;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

public class TypeWrap
{
    private static System.Type classType = typeof(System.Type);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateType(IntPtr L)
    {
        LuaDLL.luaL_error(L, "Type class does not have a constructor function");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Equals(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(System.Type), typeof(System.Type)))
        {
            System.Type varObject = LuaScriptMgr.GetVarObject(L, 1) as System.Type;
            System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
            bool b = (varObject == null) ? (typeObject == null) : varObject.Equals(typeObject);
            LuaScriptMgr.Push(L, b);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(System.Type), typeof(object)))
        {
            System.Type type3 = LuaScriptMgr.GetVarObject(L, 1) as System.Type;
            object o = LuaScriptMgr.GetVarObject(L, 2);
            bool flag2 = (type3 == null) ? (o == null) : type3.Equals(o);
            LuaScriptMgr.Push(L, flag2);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.Equals");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindInterfaces(IntPtr L)
    {
        <FindInterfaces>c__AnonStorey4E storeye = new <FindInterfaces>c__AnonStorey4E {
            L = L
        };
        LuaScriptMgr.CheckArgsCount(storeye.L, 3);
        System.Type typeObject = LuaScriptMgr.GetTypeObject(storeye.L, 1);
        TypeFilter filter = null;
        if (LuaDLL.lua_type(storeye.L, 2) != LuaTypes.LUA_TFUNCTION)
        {
            filter = (TypeFilter) LuaScriptMgr.GetNetObject(storeye.L, 2, typeof(TypeFilter));
        }
        else
        {
            <FindInterfaces>c__AnonStorey4D storeyd = new <FindInterfaces>c__AnonStorey4D {
                <>f__ref$78 = storeye,
                func = LuaScriptMgr.GetLuaFunction(storeye.L, 2)
            };
            filter = new TypeFilter(storeyd.<>m__20);
        }
        object varObject = LuaScriptMgr.GetVarObject(storeye.L, 3);
        System.Type[] o = typeObject.FindInterfaces(filter, varObject);
        LuaScriptMgr.PushArray(storeye.L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int FindMembers(IntPtr L)
    {
        <FindMembers>c__AnonStorey50 storey = new <FindMembers>c__AnonStorey50 {
            L = L
        };
        LuaScriptMgr.CheckArgsCount(storey.L, 5);
        System.Type typeObject = LuaScriptMgr.GetTypeObject(storey.L, 1);
        MemberTypes memberType = (MemberTypes) ((int) LuaScriptMgr.GetNetObject(storey.L, 2, typeof(MemberTypes)));
        BindingFlags bindingAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(storey.L, 3, typeof(BindingFlags)));
        MemberFilter filter = null;
        if (LuaDLL.lua_type(storey.L, 4) != LuaTypes.LUA_TFUNCTION)
        {
            filter = (MemberFilter) LuaScriptMgr.GetNetObject(storey.L, 4, typeof(MemberFilter));
        }
        else
        {
            <FindMembers>c__AnonStorey4F storeyf = new <FindMembers>c__AnonStorey4F {
                <>f__ref$80 = storey,
                func = LuaScriptMgr.GetLuaFunction(storey.L, 4)
            };
            filter = new MemberFilter(storeyf.<>m__21);
        }
        object varObject = LuaScriptMgr.GetVarObject(storey.L, 5);
        MemberInfo[] o = typeObject.FindMembers(memberType, bindingAttr, filter, varObject);
        LuaScriptMgr.PushArray(storey.L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Assembly(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name Assembly");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index Assembly on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.Assembly);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_AssemblyQualifiedName(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name AssemblyQualifiedName");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index AssemblyQualifiedName on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.AssemblyQualifiedName);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Attributes(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name Attributes");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index Attributes on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.Attributes);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_BaseType(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name BaseType");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index BaseType on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.BaseType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_ContainsGenericParameters(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name ContainsGenericParameters");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index ContainsGenericParameters on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.ContainsGenericParameters);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_DeclaringMethod(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name DeclaringMethod");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index DeclaringMethod on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.DeclaringMethod);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_DeclaringType(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name DeclaringType");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index DeclaringType on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.DeclaringType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_DefaultBinder(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, System.Type.DefaultBinder);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Delimiter(IntPtr L)
    {
        LuaScriptMgr.Push(L, System.Type.Delimiter);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_EmptyTypes(IntPtr L)
    {
        LuaScriptMgr.PushArray(L, System.Type.EmptyTypes);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_FilterAttribute(IntPtr L)
    {
        LuaScriptMgr.Push(L, System.Type.FilterAttribute);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_FilterName(IntPtr L)
    {
        LuaScriptMgr.Push(L, System.Type.FilterName);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_FilterNameIgnoreCase(IntPtr L)
    {
        LuaScriptMgr.Push(L, System.Type.FilterNameIgnoreCase);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_FullName(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name FullName");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index FullName on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.FullName);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_GenericParameterAttributes(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name GenericParameterAttributes");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index GenericParameterAttributes on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.GenericParameterAttributes);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_GenericParameterPosition(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name GenericParameterPosition");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index GenericParameterPosition on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.GenericParameterPosition);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_GUID(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name GUID");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index GUID on a nil value");
            }
        }
        LuaScriptMgr.PushValue(L, luaObject.GUID);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_HasElementType(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name HasElementType");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index HasElementType on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.HasElementType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsAbstract(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsAbstract");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsAbstract on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsAbstract);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsAnsiClass(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsAnsiClass");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsAnsiClass on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsAnsiClass);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsArray(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsArray");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsArray on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsArray);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsAutoClass(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsAutoClass");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsAutoClass on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsAutoClass);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsAutoLayout(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsAutoLayout");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsAutoLayout on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsAutoLayout);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsByRef(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsByRef");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsByRef on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsByRef);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsClass(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsClass");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsClass on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsClass);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsCOMObject(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsCOMObject");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsCOMObject on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsCOMObject);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsContextful(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsContextful");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsContextful on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsContextful);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsEnum(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsEnum");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsEnum on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsEnum);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsExplicitLayout(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsExplicitLayout");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsExplicitLayout on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsExplicitLayout);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsGenericParameter(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsGenericParameter");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsGenericParameter on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsGenericParameter);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsGenericType(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsGenericType");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsGenericType on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsGenericType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsGenericTypeDefinition(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsGenericTypeDefinition");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsGenericTypeDefinition on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsGenericTypeDefinition);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsImport(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsImport");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsImport on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsImport);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsInterface(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsInterface");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsInterface on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsInterface);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsLayoutSequential(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsLayoutSequential");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsLayoutSequential on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsLayoutSequential);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsMarshalByRef(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsMarshalByRef");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsMarshalByRef on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsMarshalByRef);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsNested(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsNested");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsNested on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsNested);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsNestedAssembly(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsNestedAssembly");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsNestedAssembly on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsNestedAssembly);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsNestedFamANDAssem(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsNestedFamANDAssem");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsNestedFamANDAssem on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsNestedFamANDAssem);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsNestedFamily(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsNestedFamily");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsNestedFamily on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsNestedFamily);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsNestedFamORAssem(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsNestedFamORAssem");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsNestedFamORAssem on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsNestedFamORAssem);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsNestedPrivate(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsNestedPrivate");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsNestedPrivate on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsNestedPrivate);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsNestedPublic(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsNestedPublic");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsNestedPublic on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsNestedPublic);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsNotPublic(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsNotPublic");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsNotPublic on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsNotPublic);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsPointer(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsPointer");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsPointer on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsPointer);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsPrimitive(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsPrimitive");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsPrimitive on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsPrimitive);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsPublic(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsPublic");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsPublic on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsPublic);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsSealed(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsSealed");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsSealed on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsSealed);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsSerializable(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsSerializable");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsSerializable on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsSerializable);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsSpecialName(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsSpecialName");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsSpecialName on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsSpecialName);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsUnicodeClass(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsUnicodeClass");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsUnicodeClass on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsUnicodeClass);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsValueType(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsValueType");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsValueType on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsValueType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_IsVisible(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name IsVisible");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index IsVisible on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.IsVisible);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_MemberType(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name MemberType");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index MemberType on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.MemberType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Missing(IntPtr L)
    {
        LuaScriptMgr.PushVarObject(L, System.Type.Missing);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Module(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name Module");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index Module on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.Module);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Namespace(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name Namespace");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index Namespace on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.Namespace);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_ReflectedType(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name ReflectedType");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index ReflectedType on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.ReflectedType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_StructLayoutAttribute(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name StructLayoutAttribute");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index StructLayoutAttribute on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.StructLayoutAttribute);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_TypeHandle(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name TypeHandle");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index TypeHandle on a nil value");
            }
        }
        LuaScriptMgr.PushValue(L, luaObject.TypeHandle);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_TypeInitializer(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name TypeInitializer");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index TypeInitializer on a nil value");
            }
        }
        LuaScriptMgr.PushObject(L, luaObject.TypeInitializer);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_UnderlyingSystemType(IntPtr L)
    {
        System.Type luaObject = (System.Type) LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name UnderlyingSystemType");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index UnderlyingSystemType on a nil value");
            }
        }
        LuaScriptMgr.Push(L, luaObject.UnderlyingSystemType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetArrayRank(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int arrayRank = LuaScriptMgr.GetTypeObject(L, 1).GetArrayRank();
        LuaScriptMgr.Push(L, arrayRank);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetConstructor(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                System.Type[] arrayObject = LuaScriptMgr.GetArrayObject<System.Type>(L, 2);
                ConstructorInfo constructor = typeObject.GetConstructor(arrayObject);
                LuaScriptMgr.PushObject(L, constructor);
                return 1;
            }
            case 5:
            {
                System.Type type2 = LuaScriptMgr.GetTypeObject(L, 1);
                BindingFlags bindingAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags)));
                Binder binder = (Binder) LuaScriptMgr.GetNetObject(L, 3, typeof(Binder));
                System.Type[] types = LuaScriptMgr.GetArrayObject<System.Type>(L, 4);
                ParameterModifier[] modifiers = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 5);
                ConstructorInfo o = type2.GetConstructor(bindingAttr, binder, types, modifiers);
                LuaScriptMgr.PushObject(L, o);
                return 1;
            }
            case 6:
            {
                System.Type type3 = LuaScriptMgr.GetTypeObject(L, 1);
                BindingFlags flags2 = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags)));
                Binder binder2 = (Binder) LuaScriptMgr.GetNetObject(L, 3, typeof(Binder));
                CallingConventions callConvention = (CallingConventions) ((int) LuaScriptMgr.GetNetObject(L, 4, typeof(CallingConventions)));
                System.Type[] typeArray3 = LuaScriptMgr.GetArrayObject<System.Type>(L, 5);
                ParameterModifier[] modifierArray2 = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 6);
                ConstructorInfo info3 = type3.GetConstructor(flags2, binder2, callConvention, typeArray3, modifierArray2);
                LuaScriptMgr.PushObject(L, info3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetConstructor");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetConstructors(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                ConstructorInfo[] constructors = LuaScriptMgr.GetTypeObject(L, 1).GetConstructors();
                LuaScriptMgr.PushArray(L, constructors);
                return 1;
            }
            case 2:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                BindingFlags bindingAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags)));
                ConstructorInfo[] o = typeObject.GetConstructors(bindingAttr);
                LuaScriptMgr.PushArray(L, o);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetConstructors");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetDefaultMembers(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        MemberInfo[] defaultMembers = LuaScriptMgr.GetTypeObject(L, 1).GetDefaultMembers();
        LuaScriptMgr.PushArray(L, defaultMembers);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetElementType(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        System.Type elementType = LuaScriptMgr.GetTypeObject(L, 1).GetElementType();
        LuaScriptMgr.Push(L, elementType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetEvent(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                EventInfo o = typeObject.GetEvent(luaString);
                LuaScriptMgr.PushObject(L, o);
                return 1;
            }
            case 3:
            {
                System.Type type2 = LuaScriptMgr.GetTypeObject(L, 1);
                string name = LuaScriptMgr.GetLuaString(L, 2);
                BindingFlags bindingAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags)));
                EventInfo info2 = type2.GetEvent(name, bindingAttr);
                LuaScriptMgr.PushObject(L, info2);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetEvent");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetEvents(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                EventInfo[] events = LuaScriptMgr.GetTypeObject(L, 1).GetEvents();
                LuaScriptMgr.PushArray(L, events);
                return 1;
            }
            case 2:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                BindingFlags bindingAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags)));
                EventInfo[] o = typeObject.GetEvents(bindingAttr);
                LuaScriptMgr.PushArray(L, o);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetEvents");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetField(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                FieldInfo field = typeObject.GetField(luaString);
                LuaScriptMgr.PushObject(L, field);
                return 1;
            }
            case 3:
            {
                System.Type type2 = LuaScriptMgr.GetTypeObject(L, 1);
                string name = LuaScriptMgr.GetLuaString(L, 2);
                BindingFlags bindingAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags)));
                FieldInfo o = type2.GetField(name, bindingAttr);
                LuaScriptMgr.PushObject(L, o);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetField");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetFields(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                FieldInfo[] fields = LuaScriptMgr.GetTypeObject(L, 1).GetFields();
                LuaScriptMgr.PushArray(L, fields);
                return 1;
            }
            case 2:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                BindingFlags bindingAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags)));
                FieldInfo[] o = typeObject.GetFields(bindingAttr);
                LuaScriptMgr.PushArray(L, o);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetFields");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetGenericArguments(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        System.Type[] genericArguments = LuaScriptMgr.GetTypeObject(L, 1).GetGenericArguments();
        LuaScriptMgr.PushArray(L, genericArguments);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetGenericParameterConstraints(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        System.Type[] genericParameterConstraints = LuaScriptMgr.GetTypeObject(L, 1).GetGenericParameterConstraints();
        LuaScriptMgr.PushArray(L, genericParameterConstraints);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetGenericTypeDefinition(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        System.Type genericTypeDefinition = LuaScriptMgr.GetTypeObject(L, 1).GetGenericTypeDefinition();
        LuaScriptMgr.Push(L, genericTypeDefinition);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetHashCode(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        int hashCode = LuaScriptMgr.GetTypeObject(L, 1).GetHashCode();
        LuaScriptMgr.Push(L, hashCode);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetInterface(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                System.Type o = typeObject.GetInterface(luaString);
                LuaScriptMgr.Push(L, o);
                return 1;
            }
            case 3:
            {
                System.Type type3 = LuaScriptMgr.GetTypeObject(L, 1);
                string name = LuaScriptMgr.GetLuaString(L, 2);
                bool boolean = LuaScriptMgr.GetBoolean(L, 3);
                System.Type type4 = type3.GetInterface(name, boolean);
                LuaScriptMgr.Push(L, type4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetInterface");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetInterfaceMap(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
        System.Type interfaceType = LuaScriptMgr.GetTypeObject(L, 2);
        InterfaceMapping interfaceMap = typeObject.GetInterfaceMap(interfaceType);
        LuaScriptMgr.PushValue(L, interfaceMap);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetInterfaces(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        System.Type[] interfaces = LuaScriptMgr.GetTypeObject(L, 1).GetInterfaces();
        LuaScriptMgr.PushArray(L, interfaces);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetMember(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                MemberInfo[] member = typeObject.GetMember(luaString);
                LuaScriptMgr.PushArray(L, member);
                return 1;
            }
            case 3:
            {
                System.Type type2 = LuaScriptMgr.GetTypeObject(L, 1);
                string name = LuaScriptMgr.GetLuaString(L, 2);
                BindingFlags bindingAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags)));
                MemberInfo[] o = type2.GetMember(name, bindingAttr);
                LuaScriptMgr.PushArray(L, o);
                return 1;
            }
            case 4:
            {
                System.Type type3 = LuaScriptMgr.GetTypeObject(L, 1);
                string str3 = LuaScriptMgr.GetLuaString(L, 2);
                MemberTypes type = (MemberTypes) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(MemberTypes)));
                BindingFlags flags2 = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 4, typeof(BindingFlags)));
                MemberInfo[] infoArray3 = type3.GetMember(str3, type, flags2);
                LuaScriptMgr.PushArray(L, infoArray3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetMember");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetMembers(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                MemberInfo[] members = LuaScriptMgr.GetTypeObject(L, 1).GetMembers();
                LuaScriptMgr.PushArray(L, members);
                return 1;
            }
            case 2:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                BindingFlags bindingAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags)));
                MemberInfo[] o = typeObject.GetMembers(bindingAttr);
                LuaScriptMgr.PushArray(L, o);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetMembers");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetMethod(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 2)
        {
            System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
            string luaString = LuaScriptMgr.GetLuaString(L, 2);
            MethodInfo method = typeObject.GetMethod(luaString);
            LuaScriptMgr.PushObject(L, method);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(System.Type), typeof(string), typeof(System.Type[])))
        {
            System.Type type2 = LuaScriptMgr.GetTypeObject(L, 1);
            string name = LuaScriptMgr.GetString(L, 2);
            System.Type[] arrayObject = LuaScriptMgr.GetArrayObject<System.Type>(L, 3);
            MethodInfo o = type2.GetMethod(name, arrayObject);
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(System.Type), typeof(string), typeof(BindingFlags)))
        {
            System.Type type3 = LuaScriptMgr.GetTypeObject(L, 1);
            string str3 = LuaScriptMgr.GetString(L, 2);
            BindingFlags luaObject = (BindingFlags) ((int) LuaScriptMgr.GetLuaObject(L, 3));
            MethodInfo info3 = type3.GetMethod(str3, luaObject);
            LuaScriptMgr.PushObject(L, info3);
            return 1;
        }
        if (num == 4)
        {
            System.Type type4 = LuaScriptMgr.GetTypeObject(L, 1);
            string str4 = LuaScriptMgr.GetLuaString(L, 2);
            System.Type[] types = LuaScriptMgr.GetArrayObject<System.Type>(L, 3);
            ParameterModifier[] modifiers = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 4);
            MethodInfo info4 = type4.GetMethod(str4, types, modifiers);
            LuaScriptMgr.PushObject(L, info4);
            return 1;
        }
        if (num == 6)
        {
            System.Type type5 = LuaScriptMgr.GetTypeObject(L, 1);
            string str5 = LuaScriptMgr.GetLuaString(L, 2);
            BindingFlags bindingAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags)));
            Binder binder = (Binder) LuaScriptMgr.GetNetObject(L, 4, typeof(Binder));
            System.Type[] typeArray3 = LuaScriptMgr.GetArrayObject<System.Type>(L, 5);
            ParameterModifier[] modifierArray2 = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 6);
            MethodInfo info5 = type5.GetMethod(str5, bindingAttr, binder, typeArray3, modifierArray2);
            LuaScriptMgr.PushObject(L, info5);
            return 1;
        }
        if (num == 7)
        {
            System.Type type6 = LuaScriptMgr.GetTypeObject(L, 1);
            string str6 = LuaScriptMgr.GetLuaString(L, 2);
            BindingFlags flags3 = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags)));
            Binder binder2 = (Binder) LuaScriptMgr.GetNetObject(L, 4, typeof(Binder));
            CallingConventions callConvention = (CallingConventions) ((int) LuaScriptMgr.GetNetObject(L, 5, typeof(CallingConventions)));
            System.Type[] typeArray4 = LuaScriptMgr.GetArrayObject<System.Type>(L, 6);
            ParameterModifier[] modifierArray3 = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 7);
            MethodInfo info6 = type6.GetMethod(str6, flags3, binder2, callConvention, typeArray4, modifierArray3);
            LuaScriptMgr.PushObject(L, info6);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetMethod");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetMethods(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                MethodInfo[] methods = LuaScriptMgr.GetTypeObject(L, 1).GetMethods();
                LuaScriptMgr.PushArray(L, methods);
                return 1;
            }
            case 2:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                BindingFlags bindingAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags)));
                MethodInfo[] o = typeObject.GetMethods(bindingAttr);
                LuaScriptMgr.PushArray(L, o);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetMethods");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetNestedType(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                System.Type nestedType = typeObject.GetNestedType(luaString);
                LuaScriptMgr.Push(L, nestedType);
                return 1;
            }
            case 3:
            {
                System.Type type3 = LuaScriptMgr.GetTypeObject(L, 1);
                string name = LuaScriptMgr.GetLuaString(L, 2);
                BindingFlags bindingAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags)));
                System.Type o = type3.GetNestedType(name, bindingAttr);
                LuaScriptMgr.Push(L, o);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetNestedType");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetNestedTypes(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                System.Type[] nestedTypes = LuaScriptMgr.GetTypeObject(L, 1).GetNestedTypes();
                LuaScriptMgr.PushArray(L, nestedTypes);
                return 1;
            }
            case 2:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                BindingFlags bindingAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags)));
                System.Type[] o = typeObject.GetNestedTypes(bindingAttr);
                LuaScriptMgr.PushArray(L, o);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetNestedTypes");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetProperties(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                PropertyInfo[] properties = LuaScriptMgr.GetTypeObject(L, 1).GetProperties();
                LuaScriptMgr.PushArray(L, properties);
                return 1;
            }
            case 2:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                BindingFlags bindingAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags)));
                PropertyInfo[] o = typeObject.GetProperties(bindingAttr);
                LuaScriptMgr.PushArray(L, o);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetProperties");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetProperty(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 2)
        {
            System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
            string luaString = LuaScriptMgr.GetLuaString(L, 2);
            PropertyInfo property = typeObject.GetProperty(luaString);
            LuaScriptMgr.PushObject(L, property);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(System.Type), typeof(string), typeof(System.Type[])))
        {
            System.Type type2 = LuaScriptMgr.GetTypeObject(L, 1);
            string name = LuaScriptMgr.GetString(L, 2);
            System.Type[] arrayObject = LuaScriptMgr.GetArrayObject<System.Type>(L, 3);
            PropertyInfo o = type2.GetProperty(name, arrayObject);
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(System.Type), typeof(string), typeof(System.Type)))
        {
            System.Type type3 = LuaScriptMgr.GetTypeObject(L, 1);
            string str3 = LuaScriptMgr.GetString(L, 2);
            System.Type returnType = LuaScriptMgr.GetTypeObject(L, 3);
            PropertyInfo info3 = type3.GetProperty(str3, returnType);
            LuaScriptMgr.PushObject(L, info3);
            return 1;
        }
        if ((num == 3) && LuaScriptMgr.CheckTypes(L, 1, typeof(System.Type), typeof(string), typeof(BindingFlags)))
        {
            System.Type type5 = LuaScriptMgr.GetTypeObject(L, 1);
            string str4 = LuaScriptMgr.GetString(L, 2);
            BindingFlags luaObject = (BindingFlags) ((int) LuaScriptMgr.GetLuaObject(L, 3));
            PropertyInfo info4 = type5.GetProperty(str4, luaObject);
            LuaScriptMgr.PushObject(L, info4);
            return 1;
        }
        if (num == 4)
        {
            System.Type type6 = LuaScriptMgr.GetTypeObject(L, 1);
            string str5 = LuaScriptMgr.GetLuaString(L, 2);
            System.Type type7 = LuaScriptMgr.GetTypeObject(L, 3);
            System.Type[] types = LuaScriptMgr.GetArrayObject<System.Type>(L, 4);
            PropertyInfo info5 = type6.GetProperty(str5, type7, types);
            LuaScriptMgr.PushObject(L, info5);
            return 1;
        }
        if (num == 5)
        {
            System.Type type8 = LuaScriptMgr.GetTypeObject(L, 1);
            string str6 = LuaScriptMgr.GetLuaString(L, 2);
            System.Type type9 = LuaScriptMgr.GetTypeObject(L, 3);
            System.Type[] typeArray3 = LuaScriptMgr.GetArrayObject<System.Type>(L, 4);
            ParameterModifier[] modifiers = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 5);
            PropertyInfo info6 = type8.GetProperty(str6, type9, typeArray3, modifiers);
            LuaScriptMgr.PushObject(L, info6);
            return 1;
        }
        if (num == 7)
        {
            System.Type type10 = LuaScriptMgr.GetTypeObject(L, 1);
            string str7 = LuaScriptMgr.GetLuaString(L, 2);
            BindingFlags bindingAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags)));
            Binder binder = (Binder) LuaScriptMgr.GetNetObject(L, 4, typeof(Binder));
            System.Type type11 = LuaScriptMgr.GetTypeObject(L, 5);
            System.Type[] typeArray4 = LuaScriptMgr.GetArrayObject<System.Type>(L, 6);
            ParameterModifier[] modifierArray2 = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 7);
            PropertyInfo info7 = type10.GetProperty(str7, bindingAttr, binder, type11, typeArray4, modifierArray2);
            LuaScriptMgr.PushObject(L, info7);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetProperty");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetType(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 1)
        {
            System.Type type = LuaScriptMgr.GetTypeObject(L, 1).GetType();
            LuaScriptMgr.Push(L, type);
            return 1;
        }
        if ((num == 1) && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
        {
            System.Type o = System.Type.GetType(LuaScriptMgr.GetString(L, 1));
            LuaScriptMgr.Push(L, o);
            return 1;
        }
        if (num == 2)
        {
            string luaString = LuaScriptMgr.GetLuaString(L, 1);
            bool boolean = LuaScriptMgr.GetBoolean(L, 2);
            System.Type type4 = System.Type.GetType(luaString, boolean);
            LuaScriptMgr.Push(L, type4);
            return 1;
        }
        if (num == 3)
        {
            string typeName = LuaScriptMgr.GetLuaString(L, 1);
            bool throwOnError = LuaScriptMgr.GetBoolean(L, 2);
            bool ignoreCase = LuaScriptMgr.GetBoolean(L, 3);
            System.Type type5 = System.Type.GetType(typeName, throwOnError, ignoreCase);
            LuaScriptMgr.Push(L, type5);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetType");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetTypeArray(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        System.Type[] o = System.Type.GetTypeArray(LuaScriptMgr.GetArrayObject<object>(L, 1));
        LuaScriptMgr.PushArray(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetTypeCode(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        TypeCode typeCode = System.Type.GetTypeCode(LuaScriptMgr.GetTypeObject(L, 1));
        LuaScriptMgr.Push(L, typeCode);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetTypeFromCLSID(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 1)
        {
            Guid clsid = (Guid) LuaScriptMgr.GetNetObject(L, 1, typeof(Guid));
            System.Type typeFromCLSID = System.Type.GetTypeFromCLSID(clsid);
            LuaScriptMgr.Push(L, typeFromCLSID);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(Guid), typeof(string)))
        {
            Guid luaObject = (Guid) LuaScriptMgr.GetLuaObject(L, 1);
            string server = LuaScriptMgr.GetString(L, 2);
            System.Type o = System.Type.GetTypeFromCLSID(luaObject, server);
            LuaScriptMgr.Push(L, o);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(Guid), typeof(bool)))
        {
            Guid guid3 = (Guid) LuaScriptMgr.GetLuaObject(L, 1);
            bool throwOnError = LuaDLL.lua_toboolean(L, 2);
            System.Type type3 = System.Type.GetTypeFromCLSID(guid3, throwOnError);
            LuaScriptMgr.Push(L, type3);
            return 1;
        }
        if (num == 3)
        {
            Guid guid4 = (Guid) LuaScriptMgr.GetNetObject(L, 1, typeof(Guid));
            string luaString = LuaScriptMgr.GetLuaString(L, 2);
            bool boolean = LuaScriptMgr.GetBoolean(L, 3);
            System.Type type4 = System.Type.GetTypeFromCLSID(guid4, luaString, boolean);
            LuaScriptMgr.Push(L, type4);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetTypeFromCLSID");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetTypeFromHandle(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        RuntimeTypeHandle handle = (RuntimeTypeHandle) LuaScriptMgr.GetNetObject(L, 1, typeof(RuntimeTypeHandle));
        System.Type typeFromHandle = System.Type.GetTypeFromHandle(handle);
        LuaScriptMgr.Push(L, typeFromHandle);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetTypeFromProgID(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if (num == 1)
        {
            System.Type typeFromProgID = System.Type.GetTypeFromProgID(LuaScriptMgr.GetLuaString(L, 1));
            LuaScriptMgr.Push(L, typeFromProgID);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string)))
        {
            string progID = LuaScriptMgr.GetString(L, 1);
            string server = LuaScriptMgr.GetString(L, 2);
            System.Type o = System.Type.GetTypeFromProgID(progID, server);
            LuaScriptMgr.Push(L, o);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(bool)))
        {
            string str4 = LuaScriptMgr.GetString(L, 1);
            bool throwOnError = LuaDLL.lua_toboolean(L, 2);
            System.Type type3 = System.Type.GetTypeFromProgID(str4, throwOnError);
            LuaScriptMgr.Push(L, type3);
            return 1;
        }
        if (num == 3)
        {
            string luaString = LuaScriptMgr.GetLuaString(L, 1);
            string str6 = LuaScriptMgr.GetLuaString(L, 2);
            bool boolean = LuaScriptMgr.GetBoolean(L, 3);
            System.Type type4 = System.Type.GetTypeFromProgID(luaString, str6, boolean);
            LuaScriptMgr.Push(L, type4);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetTypeFromProgID");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetTypeHandle(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        RuntimeTypeHandle typeHandle = System.Type.GetTypeHandle(LuaScriptMgr.GetVarObject(L, 1));
        LuaScriptMgr.PushValue(L, typeHandle);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int InvokeMember(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 6:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                BindingFlags invokeAttr = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags)));
                Binder binder = (Binder) LuaScriptMgr.GetNetObject(L, 4, typeof(Binder));
                object varObject = LuaScriptMgr.GetVarObject(L, 5);
                object[] arrayObject = LuaScriptMgr.GetArrayObject<object>(L, 6);
                object o = typeObject.InvokeMember(luaString, invokeAttr, binder, varObject, arrayObject);
                LuaScriptMgr.PushVarObject(L, o);
                return 1;
            }
            case 7:
            {
                System.Type type2 = LuaScriptMgr.GetTypeObject(L, 1);
                string name = LuaScriptMgr.GetLuaString(L, 2);
                BindingFlags flags2 = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags)));
                Binder binder2 = (Binder) LuaScriptMgr.GetNetObject(L, 4, typeof(Binder));
                object target = LuaScriptMgr.GetVarObject(L, 5);
                object[] args = LuaScriptMgr.GetArrayObject<object>(L, 6);
                CultureInfo culture = (CultureInfo) LuaScriptMgr.GetNetObject(L, 7, typeof(CultureInfo));
                object obj5 = type2.InvokeMember(name, flags2, binder2, target, args, culture);
                LuaScriptMgr.PushVarObject(L, obj5);
                return 1;
            }
            case 9:
            {
                System.Type type3 = LuaScriptMgr.GetTypeObject(L, 1);
                string str3 = LuaScriptMgr.GetLuaString(L, 2);
                BindingFlags flags3 = (BindingFlags) ((int) LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags)));
                Binder binder3 = (Binder) LuaScriptMgr.GetNetObject(L, 4, typeof(Binder));
                object obj6 = LuaScriptMgr.GetVarObject(L, 5);
                object[] objArray3 = LuaScriptMgr.GetArrayObject<object>(L, 6);
                ParameterModifier[] modifiers = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 7);
                CultureInfo info2 = (CultureInfo) LuaScriptMgr.GetNetObject(L, 8, typeof(CultureInfo));
                string[] arrayString = LuaScriptMgr.GetArrayString(L, 9);
                object obj7 = type3.InvokeMember(str3, flags3, binder3, obj6, objArray3, modifiers, info2, arrayString);
                LuaScriptMgr.PushVarObject(L, obj7);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.InvokeMember");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsAssignableFrom(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
        System.Type c = LuaScriptMgr.GetTypeObject(L, 2);
        bool b = typeObject.IsAssignableFrom(c);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsInstanceOfType(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
        object varObject = LuaScriptMgr.GetVarObject(L, 2);
        bool b = typeObject.IsInstanceOfType(varObject);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int IsSubclassOf(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
        System.Type c = LuaScriptMgr.GetTypeObject(L, 2);
        bool b = typeObject.IsSubclassOf(c);
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
            LuaScriptMgr.Push(L, "Table: System.Type");
        }
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int MakeArrayType(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                System.Type o = LuaScriptMgr.GetTypeObject(L, 1).MakeArrayType();
                LuaScriptMgr.Push(L, o);
                return 1;
            }
            case 2:
            {
                System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
                int number = (int) LuaScriptMgr.GetNumber(L, 2);
                System.Type type4 = typeObject.MakeArrayType(number);
                LuaScriptMgr.Push(L, type4);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Type.MakeArrayType");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int MakeByRefType(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        System.Type o = LuaScriptMgr.GetTypeObject(L, 1).MakeByRefType();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int MakeGenericType(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        System.Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
        System.Type[] typeArguments = LuaScriptMgr.GetParamsObject<System.Type>(L, 2, num - 1);
        System.Type o = typeObject.MakeGenericType(typeArguments);
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int MakePointerType(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        System.Type o = LuaScriptMgr.GetTypeObject(L, 1).MakePointerType();
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ReflectionOnlyGetType(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        string luaString = LuaScriptMgr.GetLuaString(L, 1);
        bool boolean = LuaScriptMgr.GetBoolean(L, 2);
        bool ignoreCase = LuaScriptMgr.GetBoolean(L, 3);
        System.Type o = System.Type.ReflectionOnlyGetType(luaString, boolean, ignoreCase);
        LuaScriptMgr.Push(L, o);
        return 1;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { 
            new LuaMethod("Equals", new LuaCSFunction(TypeWrap.Equals)), new LuaMethod("GetType", new LuaCSFunction(TypeWrap.GetType)), new LuaMethod("GetTypeArray", new LuaCSFunction(TypeWrap.GetTypeArray)), new LuaMethod("GetTypeCode", new LuaCSFunction(TypeWrap.GetTypeCode)), new LuaMethod("GetTypeFromCLSID", new LuaCSFunction(TypeWrap.GetTypeFromCLSID)), new LuaMethod("GetTypeFromHandle", new LuaCSFunction(TypeWrap.GetTypeFromHandle)), new LuaMethod("GetTypeFromProgID", new LuaCSFunction(TypeWrap.GetTypeFromProgID)), new LuaMethod("GetTypeHandle", new LuaCSFunction(TypeWrap.GetTypeHandle)), new LuaMethod("IsSubclassOf", new LuaCSFunction(TypeWrap.IsSubclassOf)), new LuaMethod("FindInterfaces", new LuaCSFunction(TypeWrap.FindInterfaces)), new LuaMethod("GetInterface", new LuaCSFunction(TypeWrap.GetInterface)), new LuaMethod("GetInterfaceMap", new LuaCSFunction(TypeWrap.GetInterfaceMap)), new LuaMethod("GetInterfaces", new LuaCSFunction(TypeWrap.GetInterfaces)), new LuaMethod("IsAssignableFrom", new LuaCSFunction(TypeWrap.IsAssignableFrom)), new LuaMethod("IsInstanceOfType", new LuaCSFunction(TypeWrap.IsInstanceOfType)), new LuaMethod("GetArrayRank", new LuaCSFunction(TypeWrap.GetArrayRank)), 
            new LuaMethod("GetElementType", new LuaCSFunction(TypeWrap.GetElementType)), new LuaMethod("GetEvent", new LuaCSFunction(TypeWrap.GetEvent)), new LuaMethod("GetEvents", new LuaCSFunction(TypeWrap.GetEvents)), new LuaMethod("GetField", new LuaCSFunction(TypeWrap.GetField)), new LuaMethod("GetFields", new LuaCSFunction(TypeWrap.GetFields)), new LuaMethod("GetHashCode", new LuaCSFunction(TypeWrap.GetHashCode)), new LuaMethod("GetMember", new LuaCSFunction(TypeWrap.GetMember)), new LuaMethod("GetMembers", new LuaCSFunction(TypeWrap.GetMembers)), new LuaMethod("GetMethod", new LuaCSFunction(TypeWrap.GetMethod)), new LuaMethod("GetMethods", new LuaCSFunction(TypeWrap.GetMethods)), new LuaMethod("GetNestedType", new LuaCSFunction(TypeWrap.GetNestedType)), new LuaMethod("GetNestedTypes", new LuaCSFunction(TypeWrap.GetNestedTypes)), new LuaMethod("GetProperties", new LuaCSFunction(TypeWrap.GetProperties)), new LuaMethod("GetProperty", new LuaCSFunction(TypeWrap.GetProperty)), new LuaMethod("GetConstructor", new LuaCSFunction(TypeWrap.GetConstructor)), new LuaMethod("GetConstructors", new LuaCSFunction(TypeWrap.GetConstructors)), 
            new LuaMethod("GetDefaultMembers", new LuaCSFunction(TypeWrap.GetDefaultMembers)), new LuaMethod("FindMembers", new LuaCSFunction(TypeWrap.FindMembers)), new LuaMethod("InvokeMember", new LuaCSFunction(TypeWrap.InvokeMember)), new LuaMethod("ToString", new LuaCSFunction(TypeWrap.ToString)), new LuaMethod("GetGenericArguments", new LuaCSFunction(TypeWrap.GetGenericArguments)), new LuaMethod("GetGenericTypeDefinition", new LuaCSFunction(TypeWrap.GetGenericTypeDefinition)), new LuaMethod("MakeGenericType", new LuaCSFunction(TypeWrap.MakeGenericType)), new LuaMethod("GetGenericParameterConstraints", new LuaCSFunction(TypeWrap.GetGenericParameterConstraints)), new LuaMethod("MakeArrayType", new LuaCSFunction(TypeWrap.MakeArrayType)), new LuaMethod("MakeByRefType", new LuaCSFunction(TypeWrap.MakeByRefType)), new LuaMethod("MakePointerType", new LuaCSFunction(TypeWrap.MakePointerType)), new LuaMethod("ReflectionOnlyGetType", new LuaCSFunction(TypeWrap.ReflectionOnlyGetType)), new LuaMethod("New", new LuaCSFunction(TypeWrap._CreateType)), new LuaMethod("GetClassType", new LuaCSFunction(TypeWrap.GetClassType)), new LuaMethod("__tostring", new LuaCSFunction(TypeWrap.Lua_ToString))
         };
        LuaField[] fields = new LuaField[] { 
            new LuaField("Delimiter", new LuaCSFunction(TypeWrap.get_Delimiter), null), new LuaField("EmptyTypes", new LuaCSFunction(TypeWrap.get_EmptyTypes), null), new LuaField("FilterAttribute", new LuaCSFunction(TypeWrap.get_FilterAttribute), null), new LuaField("FilterName", new LuaCSFunction(TypeWrap.get_FilterName), null), new LuaField("FilterNameIgnoreCase", new LuaCSFunction(TypeWrap.get_FilterNameIgnoreCase), null), new LuaField("Missing", new LuaCSFunction(TypeWrap.get_Missing), null), new LuaField("Assembly", new LuaCSFunction(TypeWrap.get_Assembly), null), new LuaField("AssemblyQualifiedName", new LuaCSFunction(TypeWrap.get_AssemblyQualifiedName), null), new LuaField("Attributes", new LuaCSFunction(TypeWrap.get_Attributes), null), new LuaField("BaseType", new LuaCSFunction(TypeWrap.get_BaseType), null), new LuaField("DeclaringType", new LuaCSFunction(TypeWrap.get_DeclaringType), null), new LuaField("DefaultBinder", new LuaCSFunction(TypeWrap.get_DefaultBinder), null), new LuaField("FullName", new LuaCSFunction(TypeWrap.get_FullName), null), new LuaField("GUID", new LuaCSFunction(TypeWrap.get_GUID), null), new LuaField("HasElementType", new LuaCSFunction(TypeWrap.get_HasElementType), null), new LuaField("IsAbstract", new LuaCSFunction(TypeWrap.get_IsAbstract), null), 
            new LuaField("IsAnsiClass", new LuaCSFunction(TypeWrap.get_IsAnsiClass), null), new LuaField("IsArray", new LuaCSFunction(TypeWrap.get_IsArray), null), new LuaField("IsAutoClass", new LuaCSFunction(TypeWrap.get_IsAutoClass), null), new LuaField("IsAutoLayout", new LuaCSFunction(TypeWrap.get_IsAutoLayout), null), new LuaField("IsByRef", new LuaCSFunction(TypeWrap.get_IsByRef), null), new LuaField("IsClass", new LuaCSFunction(TypeWrap.get_IsClass), null), new LuaField("IsCOMObject", new LuaCSFunction(TypeWrap.get_IsCOMObject), null), new LuaField("IsContextful", new LuaCSFunction(TypeWrap.get_IsContextful), null), new LuaField("IsEnum", new LuaCSFunction(TypeWrap.get_IsEnum), null), new LuaField("IsExplicitLayout", new LuaCSFunction(TypeWrap.get_IsExplicitLayout), null), new LuaField("IsImport", new LuaCSFunction(TypeWrap.get_IsImport), null), new LuaField("IsInterface", new LuaCSFunction(TypeWrap.get_IsInterface), null), new LuaField("IsLayoutSequential", new LuaCSFunction(TypeWrap.get_IsLayoutSequential), null), new LuaField("IsMarshalByRef", new LuaCSFunction(TypeWrap.get_IsMarshalByRef), null), new LuaField("IsNestedAssembly", new LuaCSFunction(TypeWrap.get_IsNestedAssembly), null), new LuaField("IsNestedFamANDAssem", new LuaCSFunction(TypeWrap.get_IsNestedFamANDAssem), null), 
            new LuaField("IsNestedFamily", new LuaCSFunction(TypeWrap.get_IsNestedFamily), null), new LuaField("IsNestedFamORAssem", new LuaCSFunction(TypeWrap.get_IsNestedFamORAssem), null), new LuaField("IsNestedPrivate", new LuaCSFunction(TypeWrap.get_IsNestedPrivate), null), new LuaField("IsNestedPublic", new LuaCSFunction(TypeWrap.get_IsNestedPublic), null), new LuaField("IsNotPublic", new LuaCSFunction(TypeWrap.get_IsNotPublic), null), new LuaField("IsPointer", new LuaCSFunction(TypeWrap.get_IsPointer), null), new LuaField("IsPrimitive", new LuaCSFunction(TypeWrap.get_IsPrimitive), null), new LuaField("IsPublic", new LuaCSFunction(TypeWrap.get_IsPublic), null), new LuaField("IsSealed", new LuaCSFunction(TypeWrap.get_IsSealed), null), new LuaField("IsSerializable", new LuaCSFunction(TypeWrap.get_IsSerializable), null), new LuaField("IsSpecialName", new LuaCSFunction(TypeWrap.get_IsSpecialName), null), new LuaField("IsUnicodeClass", new LuaCSFunction(TypeWrap.get_IsUnicodeClass), null), new LuaField("IsValueType", new LuaCSFunction(TypeWrap.get_IsValueType), null), new LuaField("MemberType", new LuaCSFunction(TypeWrap.get_MemberType), null), new LuaField("Module", new LuaCSFunction(TypeWrap.get_Module), null), new LuaField("Namespace", new LuaCSFunction(TypeWrap.get_Namespace), null), 
            new LuaField("ReflectedType", new LuaCSFunction(TypeWrap.get_ReflectedType), null), new LuaField("TypeHandle", new LuaCSFunction(TypeWrap.get_TypeHandle), null), new LuaField("TypeInitializer", new LuaCSFunction(TypeWrap.get_TypeInitializer), null), new LuaField("UnderlyingSystemType", new LuaCSFunction(TypeWrap.get_UnderlyingSystemType), null), new LuaField("ContainsGenericParameters", new LuaCSFunction(TypeWrap.get_ContainsGenericParameters), null), new LuaField("IsGenericTypeDefinition", new LuaCSFunction(TypeWrap.get_IsGenericTypeDefinition), null), new LuaField("IsGenericType", new LuaCSFunction(TypeWrap.get_IsGenericType), null), new LuaField("IsGenericParameter", new LuaCSFunction(TypeWrap.get_IsGenericParameter), null), new LuaField("IsNested", new LuaCSFunction(TypeWrap.get_IsNested), null), new LuaField("IsVisible", new LuaCSFunction(TypeWrap.get_IsVisible), null), new LuaField("GenericParameterPosition", new LuaCSFunction(TypeWrap.get_GenericParameterPosition), null), new LuaField("GenericParameterAttributes", new LuaCSFunction(TypeWrap.get_GenericParameterAttributes), null), new LuaField("DeclaringMethod", new LuaCSFunction(TypeWrap.get_DeclaringMethod), null), new LuaField("StructLayoutAttribute", new LuaCSFunction(TypeWrap.get_StructLayoutAttribute), null)
         };
        LuaScriptMgr.RegisterLib(L, "System.Type", typeof(System.Type), regs, fields, typeof(object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ToString(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        string str = LuaScriptMgr.GetTypeObject(L, 1).ToString();
        LuaScriptMgr.Push(L, str);
        return 1;
    }

    [CompilerGenerated]
    private sealed class <FindInterfaces>c__AnonStorey4D
    {
        internal TypeWrap.<FindInterfaces>c__AnonStorey4E <>f__ref$78;
        internal LuaFunction func;

        internal bool <>m__20(System.Type param0, object param1)
        {
            int oldTop = this.func.BeginPCall();
            LuaScriptMgr.Push(this.<>f__ref$78.L, param0);
            LuaScriptMgr.PushVarObject(this.<>f__ref$78.L, param1);
            this.func.PCall(oldTop, 2);
            object[] objArray = this.func.PopValues(oldTop);
            this.func.EndPCall(oldTop);
            return (bool) objArray[0];
        }
    }

    [CompilerGenerated]
    private sealed class <FindInterfaces>c__AnonStorey4E
    {
        internal IntPtr L;
    }

    [CompilerGenerated]
    private sealed class <FindMembers>c__AnonStorey4F
    {
        internal TypeWrap.<FindMembers>c__AnonStorey50 <>f__ref$80;
        internal LuaFunction func;

        internal bool <>m__21(MemberInfo param0, object param1)
        {
            int oldTop = this.func.BeginPCall();
            LuaScriptMgr.PushObject(this.<>f__ref$80.L, param0);
            LuaScriptMgr.PushVarObject(this.<>f__ref$80.L, param1);
            this.func.PCall(oldTop, 2);
            object[] objArray = this.func.PopValues(oldTop);
            this.func.EndPCall(oldTop);
            return (bool) objArray[0];
        }
    }

    [CompilerGenerated]
    private sealed class <FindMembers>c__AnonStorey50
    {
        internal IntPtr L;
    }
}

