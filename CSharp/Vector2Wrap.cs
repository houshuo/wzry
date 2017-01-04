using com.tencent.pandora;
using System;
using UnityEngine;

public class Vector2Wrap
{
    private static System.Type classType = typeof(Vector2);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _CreateVector2(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 2:
            {
                float number = (float) LuaScriptMgr.GetNumber(L, 1);
                float y = (float) LuaScriptMgr.GetNumber(L, 2);
                Vector2 vector = new Vector2(number, y);
                LuaScriptMgr.Push(L, vector);
                return 1;
            }
            case 0:
            {
                Vector2 vector2 = new Vector2();
                LuaScriptMgr.Push(L, vector2);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Vector2.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Angle(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Vector2 from = LuaScriptMgr.GetVector2(L, 1);
        Vector2 to = LuaScriptMgr.GetVector2(L, 2);
        float d = Vector2.Angle(from, to);
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ClampMagnitude(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
        float number = (float) LuaScriptMgr.GetNumber(L, 2);
        Vector2 vector2 = Vector2.ClampMagnitude(vector, number);
        LuaScriptMgr.Push(L, vector2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Distance(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Vector2 a = LuaScriptMgr.GetVector2(L, 1);
        Vector2 b = LuaScriptMgr.GetVector2(L, 2);
        float d = Vector2.Distance(a, b);
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Dot(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Vector2 lhs = LuaScriptMgr.GetVector2(L, 1);
        Vector2 rhs = LuaScriptMgr.GetVector2(L, 2);
        float d = Vector2.Dot(lhs, rhs);
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Equals(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Vector2 varObject = (Vector2) LuaScriptMgr.GetVarObject(L, 1);
        object other = LuaScriptMgr.GetVarObject(L, 2);
        bool b = varObject.Equals(other);
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_Item(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Vector2 vector = (Vector2) LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2");
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        float d = vector[number];
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_kEpsilon(IntPtr L)
    {
        LuaScriptMgr.Push(L, (float) 1E-05f);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_magnitude(IntPtr L)
    {
        object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name magnitude");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index magnitude on a nil value");
            }
        }
        Vector2 vector = (Vector2) luaObject;
        LuaScriptMgr.Push(L, vector.magnitude);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_normalized(IntPtr L)
    {
        object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name normalized");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index normalized on a nil value");
            }
        }
        Vector2 vector = (Vector2) luaObject;
        LuaScriptMgr.Push(L, vector.normalized);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_one(IntPtr L)
    {
        LuaScriptMgr.Push(L, Vector2.one);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_right(IntPtr L)
    {
        LuaScriptMgr.Push(L, Vector2.right);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_sqrMagnitude(IntPtr L)
    {
        object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name sqrMagnitude");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index sqrMagnitude on a nil value");
            }
        }
        Vector2 vector = (Vector2) luaObject;
        LuaScriptMgr.Push(L, vector.sqrMagnitude);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_up(IntPtr L)
    {
        LuaScriptMgr.Push(L, Vector2.up);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_x(IntPtr L)
    {
        object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name x");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index x on a nil value");
            }
        }
        Vector2 vector = (Vector2) luaObject;
        LuaScriptMgr.Push(L, vector.x);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_y(IntPtr L)
    {
        object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name y");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index y on a nil value");
            }
        }
        Vector2 vector = (Vector2) luaObject;
        LuaScriptMgr.Push(L, vector.y);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_zero(IntPtr L)
    {
        LuaScriptMgr.Push(L, Vector2.zero);
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
        int hashCode = ((Vector2) LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2")).GetHashCode();
        LuaScriptMgr.Push(L, hashCode);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lerp(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        Vector2 a = LuaScriptMgr.GetVector2(L, 1);
        Vector2 b = LuaScriptMgr.GetVector2(L, 2);
        float number = (float) LuaScriptMgr.GetNumber(L, 3);
        Vector2 vector3 = Vector2.Lerp(a, b, number);
        LuaScriptMgr.Push(L, vector3);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_Add(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
        Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
        Vector2 vector3 = vector + vector2;
        LuaScriptMgr.Push(L, vector3);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_Div(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
        float number = (float) LuaScriptMgr.GetNumber(L, 2);
        Vector2 vector2 = (Vector2) (vector / number);
        LuaScriptMgr.Push(L, vector2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_Eq(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
        Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
        bool b = vector == vector2;
        LuaScriptMgr.Push(L, b);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_Mul(IntPtr L)
    {
        int num = LuaDLL.lua_gettop(L);
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(float), typeof(LuaTable)))
        {
            float num2 = (float) LuaDLL.lua_tonumber(L, 1);
            Vector2 vector = LuaScriptMgr.GetVector2(L, 2);
            Vector2 vector2 = (Vector2) (num2 * vector);
            LuaScriptMgr.Push(L, vector2);
            return 1;
        }
        if ((num == 2) && LuaScriptMgr.CheckTypes(L, 1, typeof(LuaTable), typeof(float)))
        {
            Vector2 vector3 = LuaScriptMgr.GetVector2(L, 1);
            float num3 = (float) LuaDLL.lua_tonumber(L, 2);
            Vector2 vector4 = (Vector2) (vector3 * num3);
            LuaScriptMgr.Push(L, vector4);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Vector2.op_Multiply");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_Neg(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Vector2 vector2 = -LuaScriptMgr.GetVector2(L, 1);
        LuaScriptMgr.Push(L, vector2);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Lua_Sub(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Vector2 vector = LuaScriptMgr.GetVector2(L, 1);
        Vector2 vector2 = LuaScriptMgr.GetVector2(L, 2);
        Vector2 vector3 = vector - vector2;
        LuaScriptMgr.Push(L, vector3);
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
            LuaScriptMgr.Push(L, "Table: UnityEngine.Vector2");
        }
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Max(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Vector2 lhs = LuaScriptMgr.GetVector2(L, 1);
        Vector2 rhs = LuaScriptMgr.GetVector2(L, 2);
        Vector2 vector3 = Vector2.Max(lhs, rhs);
        LuaScriptMgr.Push(L, vector3);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Min(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Vector2 lhs = LuaScriptMgr.GetVector2(L, 1);
        Vector2 rhs = LuaScriptMgr.GetVector2(L, 2);
        Vector2 vector3 = Vector2.Min(lhs, rhs);
        LuaScriptMgr.Push(L, vector3);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int MoveTowards(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        Vector2 current = LuaScriptMgr.GetVector2(L, 1);
        Vector2 target = LuaScriptMgr.GetVector2(L, 2);
        float number = (float) LuaScriptMgr.GetNumber(L, 3);
        Vector2 vector3 = Vector2.MoveTowards(current, target, number);
        LuaScriptMgr.Push(L, vector3);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Normalize(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        Vector2 vector = (Vector2) LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2");
        vector.Normalize();
        LuaScriptMgr.SetValueObject(L, 1, vector);
        return 0;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { 
            new LuaMethod("get_Item", new LuaCSFunction(Vector2Wrap.get_Item)), new LuaMethod("set_Item", new LuaCSFunction(Vector2Wrap.set_Item)), new LuaMethod("Set", new LuaCSFunction(Vector2Wrap.Set)), new LuaMethod("Lerp", new LuaCSFunction(Vector2Wrap.Lerp)), new LuaMethod("MoveTowards", new LuaCSFunction(Vector2Wrap.MoveTowards)), new LuaMethod("Scale", new LuaCSFunction(Vector2Wrap.Scale)), new LuaMethod("Normalize", new LuaCSFunction(Vector2Wrap.Normalize)), new LuaMethod("ToString", new LuaCSFunction(Vector2Wrap.ToString)), new LuaMethod("GetHashCode", new LuaCSFunction(Vector2Wrap.GetHashCode)), new LuaMethod("Equals", new LuaCSFunction(Vector2Wrap.Equals)), new LuaMethod("Dot", new LuaCSFunction(Vector2Wrap.Dot)), new LuaMethod("Angle", new LuaCSFunction(Vector2Wrap.Angle)), new LuaMethod("Distance", new LuaCSFunction(Vector2Wrap.Distance)), new LuaMethod("ClampMagnitude", new LuaCSFunction(Vector2Wrap.ClampMagnitude)), new LuaMethod("SqrMagnitude", new LuaCSFunction(Vector2Wrap.SqrMagnitude)), new LuaMethod("Min", new LuaCSFunction(Vector2Wrap.Min)), 
            new LuaMethod("Max", new LuaCSFunction(Vector2Wrap.Max)), new LuaMethod("SmoothDamp", new LuaCSFunction(Vector2Wrap.SmoothDamp)), new LuaMethod("New", new LuaCSFunction(Vector2Wrap._CreateVector2)), new LuaMethod("GetClassType", new LuaCSFunction(Vector2Wrap.GetClassType)), new LuaMethod("__tostring", new LuaCSFunction(Vector2Wrap.Lua_ToString)), new LuaMethod("__add", new LuaCSFunction(Vector2Wrap.Lua_Add)), new LuaMethod("__sub", new LuaCSFunction(Vector2Wrap.Lua_Sub)), new LuaMethod("__mul", new LuaCSFunction(Vector2Wrap.Lua_Mul)), new LuaMethod("__div", new LuaCSFunction(Vector2Wrap.Lua_Div)), new LuaMethod("__eq", new LuaCSFunction(Vector2Wrap.Lua_Eq)), new LuaMethod("__unm", new LuaCSFunction(Vector2Wrap.Lua_Neg))
         };
        LuaField[] fields = new LuaField[] { new LuaField("kEpsilon", new LuaCSFunction(Vector2Wrap.get_kEpsilon), null), new LuaField("x", new LuaCSFunction(Vector2Wrap.get_x), new LuaCSFunction(Vector2Wrap.set_x)), new LuaField("y", new LuaCSFunction(Vector2Wrap.get_y), new LuaCSFunction(Vector2Wrap.set_y)), new LuaField("normalized", new LuaCSFunction(Vector2Wrap.get_normalized), null), new LuaField("magnitude", new LuaCSFunction(Vector2Wrap.get_magnitude), null), new LuaField("sqrMagnitude", new LuaCSFunction(Vector2Wrap.get_sqrMagnitude), null), new LuaField("zero", new LuaCSFunction(Vector2Wrap.get_zero), null), new LuaField("one", new LuaCSFunction(Vector2Wrap.get_one), null), new LuaField("up", new LuaCSFunction(Vector2Wrap.get_up), null), new LuaField("right", new LuaCSFunction(Vector2Wrap.get_right), null) };
        LuaScriptMgr.RegisterLib(L, "UnityEngine.Vector2", typeof(Vector2), regs, fields, null);
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Scale(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        Vector2 vector = (Vector2) LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2");
        Vector2 scale = LuaScriptMgr.GetVector2(L, 2);
        vector.Scale(scale);
        LuaScriptMgr.SetValueObject(L, 1, vector);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int Set(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        Vector2 vector = (Vector2) LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2");
        float number = (float) LuaScriptMgr.GetNumber(L, 2);
        float num2 = (float) LuaScriptMgr.GetNumber(L, 3);
        vector.Set(number, num2);
        LuaScriptMgr.SetValueObject(L, 1, vector);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_Item(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        Vector2 vector = (Vector2) LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2");
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        float num2 = (float) LuaScriptMgr.GetNumber(L, 3);
        vector[number] = num2;
        LuaScriptMgr.SetValueObject(L, 1, vector);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_x(IntPtr L)
    {
        object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name x");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index x on a nil value");
            }
        }
        Vector2 vector = (Vector2) luaObject;
        vector.x = (float) LuaScriptMgr.GetNumber(L, 3);
        LuaScriptMgr.SetValueObject(L, 1, vector);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_y(IntPtr L)
    {
        object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
        if (luaObject == null)
        {
            if (LuaDLL.lua_type(L, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaDLL.luaL_error(L, "unknown member name y");
            }
            else
            {
                LuaDLL.luaL_error(L, "attempt to index y on a nil value");
            }
        }
        Vector2 vector = (Vector2) luaObject;
        vector.y = (float) LuaScriptMgr.GetNumber(L, 3);
        LuaScriptMgr.SetValueObject(L, 1, vector);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SmoothDamp(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 4:
            {
                Vector2 current = LuaScriptMgr.GetVector2(L, 1);
                Vector2 target = LuaScriptMgr.GetVector2(L, 2);
                Vector2 currentVelocity = LuaScriptMgr.GetVector2(L, 3);
                float number = (float) LuaScriptMgr.GetNumber(L, 4);
                Vector2 vector4 = Vector2.SmoothDamp(current, target, ref currentVelocity, number);
                LuaScriptMgr.Push(L, vector4);
                LuaScriptMgr.Push(L, currentVelocity);
                return 2;
            }
            case 5:
            {
                Vector2 vector5 = LuaScriptMgr.GetVector2(L, 1);
                Vector2 vector6 = LuaScriptMgr.GetVector2(L, 2);
                Vector2 vector7 = LuaScriptMgr.GetVector2(L, 3);
                float smoothTime = (float) LuaScriptMgr.GetNumber(L, 4);
                float maxSpeed = (float) LuaScriptMgr.GetNumber(L, 5);
                Vector2 vector8 = Vector2.SmoothDamp(vector5, vector6, ref vector7, smoothTime, maxSpeed);
                LuaScriptMgr.Push(L, vector8);
                LuaScriptMgr.Push(L, vector7);
                return 2;
            }
            case 6:
            {
                Vector2 vector9 = LuaScriptMgr.GetVector2(L, 1);
                Vector2 vector10 = LuaScriptMgr.GetVector2(L, 2);
                Vector2 vector11 = LuaScriptMgr.GetVector2(L, 3);
                float num5 = (float) LuaScriptMgr.GetNumber(L, 4);
                float num6 = (float) LuaScriptMgr.GetNumber(L, 5);
                float deltaTime = (float) LuaScriptMgr.GetNumber(L, 6);
                Vector2 vector12 = Vector2.SmoothDamp(vector9, vector10, ref vector11, num5, num6, deltaTime);
                LuaScriptMgr.Push(L, vector12);
                LuaScriptMgr.Push(L, vector11);
                return 2;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Vector2.SmoothDamp");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SqrMagnitude(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        float d = ((Vector2) LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2")).SqrMagnitude();
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int ToString(IntPtr L)
    {
        switch (LuaDLL.lua_gettop(L))
        {
            case 1:
            {
                string str = ((Vector2) LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2")).ToString();
                LuaScriptMgr.Push(L, str);
                return 1;
            }
            case 2:
            {
                Vector2 vector2 = (Vector2) LuaScriptMgr.GetNetObjectSelf(L, 1, "Vector2");
                string luaString = LuaScriptMgr.GetLuaString(L, 2);
                string str3 = vector2.ToString(luaString);
                LuaScriptMgr.Push(L, str3);
                return 1;
            }
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: Vector2.ToString");
        return 0;
    }
}

