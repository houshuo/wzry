namespace com.tencent.pandora
{
    using System;
    using System.Collections;
    using System.Reflection;

    public class LuaTable : LuaBase
    {
        public LuaTable(int reference, LuaState interpreter)
        {
            base._Reference = reference;
            base._Interpreter = interpreter;
            base.translator = interpreter.translator;
        }

        public LuaTable(int reference, IntPtr L)
        {
            base._Reference = reference;
            base.translator = ObjectTranslator.FromState(L);
            base._Interpreter = base.translator.interpreter;
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return base._Interpreter.GetTableDict(this).GetEnumerator();
        }

        internal void push(IntPtr luaState)
        {
            LuaDLL.lua_getref(luaState, base._Reference);
        }

        internal object rawget(string field)
        {
            return base._Interpreter.rawGetObject(base._Reference, field);
        }

        public LuaFunction RawGetFunc(string field)
        {
            IntPtr l = base._Interpreter.L;
            LuaFunction function = null;
            int newTop = LuaDLL.lua_gettop(l);
            LuaDLL.lua_getref(l, base._Reference);
            LuaDLL.lua_pushstring(l, field);
            LuaDLL.lua_gettable(l, -2);
            if (LuaDLL.lua_type(l, -1) == LuaTypes.LUA_TFUNCTION)
            {
                function = new LuaFunction(LuaDLL.luaL_ref(l, LuaIndexes.LUA_REGISTRYINDEX), l);
            }
            LuaDLL.lua_settop(l, newTop);
            return function;
        }

        internal object rawgetFunction(string field)
        {
            object obj2 = base._Interpreter.rawGetObject(base._Reference, field);
            if (obj2 is LuaCSFunction)
            {
                return new LuaFunction((LuaCSFunction) obj2, base._Interpreter);
            }
            return obj2;
        }

        public void Set(string key, object o)
        {
            IntPtr l = base._Interpreter.L;
            this.push(l);
            LuaDLL.lua_pushstring(l, key);
            base.PushArgs(l, o);
            LuaDLL.lua_rawset(l, -3);
            LuaDLL.lua_settop(l, 0);
        }

        public void SetMetaTable(LuaTable metaTable)
        {
            this.push(base._Interpreter.L);
            metaTable.push(base._Interpreter.L);
            LuaDLL.lua_setmetatable(base._Interpreter.L, -2);
            LuaDLL.lua_pop(base._Interpreter.L, 1);
        }

        public T[] ToArray<T>()
        {
            IntPtr l = base._Interpreter.L;
            this.push(l);
            return LuaScriptMgr.GetArrayObject<T>(l, -1);
        }

        public override string ToString()
        {
            return "table";
        }

        public int Count
        {
            get
            {
                return base._Interpreter.GetTableDict(this).Count;
            }
        }

        public object this[string field]
        {
            get
            {
                return base._Interpreter.getObject(base._Reference, field);
            }
            set
            {
                base._Interpreter.setObject(base._Reference, field, value);
            }
        }

        public object this[object field]
        {
            get
            {
                return base._Interpreter.getObject(base._Reference, field);
            }
            set
            {
                base._Interpreter.setObject(base._Reference, field, value);
            }
        }

        public ICollection Keys
        {
            get
            {
                return base._Interpreter.GetTableDict(this).Keys;
            }
        }

        public ICollection Values
        {
            get
            {
                return base._Interpreter.GetTableDict(this).Values;
            }
        }
    }
}

