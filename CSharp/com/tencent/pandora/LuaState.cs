namespace com.tencent.pandora
{
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Text;

    public class LuaState : IDisposable
    {
        internal LuaCSFunction dofileFunction;
        internal LuaCSFunction import_wrapFunction;
        public IntPtr L = LuaDLL.luaL_newstate();
        internal LuaCSFunction loaderFunction;
        internal LuaCSFunction loadfileFunction;
        internal LuaCSFunction panicCallback;
        internal LuaCSFunction printFunction;
        internal LuaCSFunction tracebackFunction;
        internal ObjectTranslator translator;

        public LuaState()
        {
            LuaDLL.luaL_openlibs(this.L);
            LuaDLL.lua_pushstring(this.L, "LUAINTERFACE LOADED");
            LuaDLL.lua_pushboolean(this.L, true);
            LuaDLL.lua_settable(this.L, LuaIndexes.LUA_REGISTRYINDEX);
            LuaDLL.lua_newtable(this.L);
            LuaDLL.lua_setglobal(this.L, "luanet");
            LuaDLL.lua_pushvalue(this.L, LuaIndexes.LUA_GLOBALSINDEX);
            LuaDLL.lua_getglobal(this.L, "luanet");
            LuaDLL.lua_pushstring(this.L, "getmetatable");
            LuaDLL.lua_getglobal(this.L, "getmetatable");
            LuaDLL.lua_settable(this.L, -3);
            LuaDLL.lua_pushstring(this.L, "rawget");
            LuaDLL.lua_getglobal(this.L, "rawget");
            LuaDLL.lua_settable(this.L, -3);
            LuaDLL.lua_pushstring(this.L, "rawset");
            LuaDLL.lua_getglobal(this.L, "rawset");
            LuaDLL.lua_settable(this.L, -3);
            LuaDLL.lua_replace(this.L, LuaIndexes.LUA_GLOBALSINDEX);
            this.translator = new ObjectTranslator(this, this.L);
            LuaDLL.lua_replace(this.L, LuaIndexes.LUA_GLOBALSINDEX);
            this.translator.PushTranslator(this.L);
            this.panicCallback = new LuaCSFunction(LuaStatic.panic);
            LuaDLL.lua_atpanic(this.L, this.panicCallback);
            this.printFunction = new LuaCSFunction(LuaStatic.print);
            LuaDLL.lua_pushstdcallcfunction(this.L, this.printFunction, 0);
            LuaDLL.lua_setfield(this.L, LuaIndexes.LUA_GLOBALSINDEX, "print");
            this.loadfileFunction = new LuaCSFunction(LuaStatic.loadfile);
            LuaDLL.lua_pushstdcallcfunction(this.L, this.loadfileFunction, 0);
            LuaDLL.lua_setfield(this.L, LuaIndexes.LUA_GLOBALSINDEX, "loadfile");
            this.dofileFunction = new LuaCSFunction(LuaStatic.dofile);
            LuaDLL.lua_pushstdcallcfunction(this.L, this.dofileFunction, 0);
            LuaDLL.lua_setfield(this.L, LuaIndexes.LUA_GLOBALSINDEX, "dofile");
            this.import_wrapFunction = new LuaCSFunction(LuaStatic.importWrap);
            LuaDLL.lua_pushstdcallcfunction(this.L, this.import_wrapFunction, 0);
            LuaDLL.lua_setfield(this.L, LuaIndexes.LUA_GLOBALSINDEX, "import");
            this.loaderFunction = new LuaCSFunction(LuaStatic.loader);
            LuaDLL.lua_pushstdcallcfunction(this.L, this.loaderFunction, 0);
            int index = LuaDLL.lua_gettop(this.L);
            LuaDLL.lua_getfield(this.L, LuaIndexes.LUA_GLOBALSINDEX, "package");
            LuaDLL.lua_getfield(this.L, -1, "loaders");
            int i = LuaDLL.lua_gettop(this.L);
            for (int j = LuaDLL.luaL_getn(this.L, i) + 1; j > 1; j--)
            {
                LuaDLL.lua_rawgeti(this.L, i, j - 1);
                LuaDLL.lua_rawseti(this.L, i, j);
            }
            LuaDLL.lua_pushvalue(this.L, index);
            LuaDLL.lua_rawseti(this.L, i, 1);
            LuaDLL.lua_settop(this.L, 0);
            this.DoString(LuaStatic.init_luanet);
            this.tracebackFunction = new LuaCSFunction(LuaStatic.traceback);
        }

        public void Close()
        {
            if (this.L != IntPtr.Zero)
            {
                this.translator.Destroy();
                LuaDLL.lua_close(this.L);
            }
        }

        internal bool compareRef(int ref1, int ref2)
        {
            if (ref1 == ref2)
            {
                return true;
            }
            int newTop = LuaDLL.lua_gettop(this.L);
            LuaDLL.lua_getref(this.L, ref1);
            LuaDLL.lua_getref(this.L, ref2);
            int num2 = LuaDLL.lua_equal(this.L, -1, -2);
            LuaDLL.lua_settop(this.L, newTop);
            return (num2 != 0);
        }

        public LuaFunction CreateFunction(object target, MethodBase function)
        {
            int newTop = LuaDLL.lua_gettop(this.L);
            LuaMethodWrapper wrapper = new LuaMethodWrapper(this.translator, target, function.DeclaringType, function);
            this.translator.push(this.L, new LuaCSFunction(wrapper.call));
            object obj2 = this.translator.getObject(this.L, -1);
            LuaFunction function2 = !(obj2 is LuaCSFunction) ? ((LuaFunction) obj2) : new LuaFunction((LuaCSFunction) obj2, this);
            LuaDLL.lua_settop(this.L, newTop);
            return function2;
        }

        internal void dispose(int reference)
        {
            if (this.L != IntPtr.Zero)
            {
                LuaDLL.lua_unref(this.L, reference);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            this.L = IntPtr.Zero;
            GC.SuppressFinalize(this);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public virtual void Dispose(bool dispose)
        {
            if (dispose && (this.translator != null))
            {
                this.translator.pendingEvents.Dispose();
                this.translator = null;
            }
        }

        public object[] DoFile(string fileName)
        {
            return this.DoFile(fileName, null);
        }

        public object[] DoFile(string fileName, LuaTable env)
        {
            LuaDLL.lua_pushstdcallcfunction(this.L, this.tracebackFunction, 0);
            int oldTop = LuaDLL.lua_gettop(this.L);
            byte[] buff = LuaStatic.Load(fileName);
            if (buff == null)
            {
                if (!fileName.Contains("mobdebug"))
                {
                }
                LuaDLL.lua_pop(this.L, 1);
                return null;
            }
            string name = Util.LuaPath(fileName);
            if (LuaDLL.luaL_loadbuffer(this.L, buff, buff.Length, name) == 0)
            {
                if (env != null)
                {
                    env.push(this.L);
                    LuaDLL.lua_setfenv(this.L, -2);
                }
                if (LuaDLL.lua_pcall(this.L, 0, -1, -2) == 0)
                {
                    object[] objArray = this.translator.popValues(this.L, oldTop);
                    LuaDLL.lua_pop(this.L, 1);
                    return objArray;
                }
                this.ThrowExceptionFromError(oldTop);
                LuaDLL.lua_pop(this.L, 1);
            }
            else
            {
                this.ThrowExceptionFromError(oldTop);
                LuaDLL.lua_pop(this.L, 1);
            }
            return null;
        }

        public object[] DoString(string chunk)
        {
            return this.DoString(chunk, "chunk", null);
        }

        public object[] DoString(string chunk, string chunkName, LuaTable env)
        {
            int oldTop = LuaDLL.lua_gettop(this.L);
            byte[] bytes = Encoding.UTF8.GetBytes(chunk);
            if (LuaDLL.luaL_loadbuffer(this.L, bytes, bytes.Length, chunkName) == 0)
            {
                if (env != null)
                {
                    env.push(this.L);
                    LuaDLL.lua_setfenv(this.L, -2);
                }
                if (LuaDLL.lua_pcall(this.L, 0, -1, 0) == 0)
                {
                    return this.translator.popValues(this.L, oldTop);
                }
                this.ThrowExceptionFromError(oldTop);
            }
            else
            {
                this.ThrowExceptionFromError(oldTop);
            }
            return null;
        }

        public LuaFunction GetFunction(string fullPath)
        {
            object obj2 = this[fullPath];
            return (!(obj2 is LuaCSFunction) ? ((LuaFunction) obj2) : new LuaFunction((LuaCSFunction) obj2, this));
        }

        public Delegate GetFunction(Type delegateType, string fullPath)
        {
            this.translator.throwError(this.L, "function delegates not implemnented");
            return null;
        }

        public double GetNumber(string fullPath)
        {
            return (double) this[fullPath];
        }

        internal object getObject(string[] remainingPath)
        {
            object obj2 = null;
            for (int i = 0; i < remainingPath.Length; i++)
            {
                LuaDLL.lua_pushstring(this.L, remainingPath[i]);
                LuaDLL.lua_gettable(this.L, -2);
                obj2 = this.translator.getObject(this.L, -1);
                if (obj2 == null)
                {
                    return obj2;
                }
            }
            return obj2;
        }

        internal object getObject(int reference, object field)
        {
            int newTop = LuaDLL.lua_gettop(this.L);
            LuaDLL.lua_getref(this.L, reference);
            this.translator.push(this.L, field);
            LuaDLL.lua_gettable(this.L, -2);
            object obj2 = this.translator.getObject(this.L, -1);
            LuaDLL.lua_settop(this.L, newTop);
            return obj2;
        }

        internal object getObject(int reference, string field)
        {
            int newTop = LuaDLL.lua_gettop(this.L);
            LuaDLL.lua_getref(this.L, reference);
            char[] separator = new char[] { '.' };
            object obj2 = this.getObject(field.Split(separator));
            LuaDLL.lua_settop(this.L, newTop);
            return obj2;
        }

        public string GetString(string fullPath)
        {
            return (string) this[fullPath];
        }

        public LuaTable GetTable(string fullPath)
        {
            return (LuaTable) this[fullPath];
        }

        public ListDictionary GetTableDict(LuaTable table)
        {
            ListDictionary dictionary = new ListDictionary();
            int newTop = LuaDLL.lua_gettop(this.L);
            this.translator.push(this.L, table);
            LuaDLL.lua_pushnil(this.L);
            while (LuaDLL.lua_next(this.L, -2) != 0)
            {
                dictionary[this.translator.getObject(this.L, -2)] = this.translator.getObject(this.L, -1);
                LuaDLL.lua_settop(this.L, -2);
            }
            LuaDLL.lua_settop(this.L, newTop);
            return dictionary;
        }

        public ObjectTranslator GetTranslator()
        {
            return this.translator;
        }

        public LuaFunction LoadFile(string fileName)
        {
            int oldTop = LuaDLL.lua_gettop(this.L);
            byte[] buff = null;
            buff = LuaHelper.GetResManager().luaFiles[fileName].bytes;
            if (LuaDLL.luaL_loadbuffer(this.L, buff, buff.Length, fileName) != 0)
            {
                this.ThrowExceptionFromError(oldTop);
            }
            LuaFunction function = this.translator.getFunction(this.L, -1);
            this.translator.popValues(this.L, oldTop);
            return function;
        }

        public LuaFunction LoadString(string chunk, string name)
        {
            return this.LoadString(chunk, name, null);
        }

        public LuaFunction LoadString(string chunk, string name, LuaTable env)
        {
            int oldTop = LuaDLL.lua_gettop(this.L);
            byte[] bytes = Encoding.UTF8.GetBytes(chunk);
            if (LuaDLL.luaL_loadbuffer(this.L, bytes, bytes.Length, name) != 0)
            {
                this.ThrowExceptionFromError(oldTop);
            }
            if (env != null)
            {
                env.push(this.L);
                LuaDLL.lua_setfenv(this.L, -2);
            }
            LuaFunction function = this.translator.getFunction(this.L, -1);
            this.translator.popValues(this.L, oldTop);
            return function;
        }

        public LuaTable NewTable()
        {
            int newTop = LuaDLL.lua_gettop(this.L);
            LuaDLL.lua_newtable(this.L);
            LuaTable table = (LuaTable) this.translator.getObject(this.L, -1);
            LuaDLL.lua_settop(this.L, newTop);
            return table;
        }

        public void NewTable(string fullPath)
        {
            char[] separator = new char[] { '.' };
            string[] strArray = fullPath.Split(separator);
            int newTop = LuaDLL.lua_gettop(this.L);
            if (strArray.Length == 1)
            {
                LuaDLL.lua_newtable(this.L);
                LuaDLL.lua_setglobal(this.L, fullPath);
            }
            else
            {
                LuaDLL.lua_getglobal(this.L, strArray[0]);
                for (int i = 1; i < (strArray.Length - 1); i++)
                {
                    LuaDLL.lua_pushstring(this.L, strArray[i]);
                    LuaDLL.lua_gettable(this.L, -2);
                }
                LuaDLL.lua_pushstring(this.L, strArray[strArray.Length - 1]);
                LuaDLL.lua_newtable(this.L);
                LuaDLL.lua_settable(this.L, -3);
            }
            LuaDLL.lua_settop(this.L, newTop);
        }

        internal void pushCSFunction(LuaCSFunction function)
        {
            this.translator.pushFunction(this.L, function);
        }

        internal object rawGetObject(int reference, string field)
        {
            int newTop = LuaDLL.lua_gettop(this.L);
            LuaDLL.lua_getref(this.L, reference);
            LuaDLL.lua_pushstring(this.L, field);
            LuaDLL.lua_rawget(this.L, -2);
            object obj2 = this.translator.getObject(this.L, -1);
            LuaDLL.lua_settop(this.L, newTop);
            return obj2;
        }

        public LuaFunction RegisterFunction(string path, object target, MethodBase function)
        {
            int newTop = LuaDLL.lua_gettop(this.L);
            LuaMethodWrapper wrapper = new LuaMethodWrapper(this.translator, target, function.DeclaringType, function);
            this.translator.push(this.L, new LuaCSFunction(wrapper.call));
            this[path] = this.translator.getObject(this.L, -1);
            LuaFunction function2 = this.GetFunction(path);
            LuaDLL.lua_settop(this.L, newTop);
            return function2;
        }

        internal void setObject(string[] remainingPath, object val)
        {
            for (int i = 0; i < (remainingPath.Length - 1); i++)
            {
                LuaDLL.lua_pushstring(this.L, remainingPath[i]);
                LuaDLL.lua_gettable(this.L, -2);
            }
            LuaDLL.lua_pushstring(this.L, remainingPath[remainingPath.Length - 1]);
            this.translator.push(this.L, val);
            LuaDLL.lua_settable(this.L, -3);
        }

        internal void setObject(int reference, object field, object val)
        {
            int newTop = LuaDLL.lua_gettop(this.L);
            LuaDLL.lua_getref(this.L, reference);
            this.translator.push(this.L, field);
            this.translator.push(this.L, val);
            LuaDLL.lua_settable(this.L, -3);
            LuaDLL.lua_settop(this.L, newTop);
        }

        internal void setObject(int reference, string field, object val)
        {
            int newTop = LuaDLL.lua_gettop(this.L);
            LuaDLL.lua_getref(this.L, reference);
            char[] separator = new char[] { '.' };
            this.setObject(field.Split(separator), val);
            LuaDLL.lua_settop(this.L, newTop);
        }

        internal int SetPendingException(Exception e)
        {
            if (e != null)
            {
                this.translator.throwError(this.L, e.ToString());
                LuaDLL.lua_pushnil(this.L);
                return 1;
            }
            return 0;
        }

        internal void ThrowExceptionFromError(int oldTop)
        {
            string message = LuaDLL.lua_tostring(this.L, -1);
            LuaDLL.lua_settop(this.L, oldTop);
            if (message == null)
            {
                message = "Unknown Lua Error";
            }
            throw new LuaScriptException(message, string.Empty);
        }

        public object this[string fullPath]
        {
            get
            {
                object obj2 = null;
                int newTop = LuaDLL.lua_gettop(this.L);
                char[] separator = new char[] { '.' };
                string[] sourceArray = fullPath.Split(separator);
                LuaDLL.lua_getglobal(this.L, sourceArray[0]);
                obj2 = this.translator.getObject(this.L, -1);
                if (sourceArray.Length > 1)
                {
                    string[] destinationArray = new string[sourceArray.Length - 1];
                    Array.Copy(sourceArray, 1, destinationArray, 0, sourceArray.Length - 1);
                    obj2 = this.getObject(destinationArray);
                }
                LuaDLL.lua_settop(this.L, newTop);
                return obj2;
            }
            set
            {
                int newTop = LuaDLL.lua_gettop(this.L);
                char[] separator = new char[] { '.' };
                string[] sourceArray = fullPath.Split(separator);
                if (sourceArray.Length == 1)
                {
                    this.translator.push(this.L, value);
                    LuaDLL.lua_setglobal(this.L, fullPath);
                }
                else
                {
                    LuaDLL.lua_rawglobal(this.L, sourceArray[0]);
                    if (LuaDLL.lua_type(this.L, -1) == LuaTypes.LUA_TNIL)
                    {
                        LuaDLL.lua_settop(this.L, newTop);
                        return;
                    }
                    string[] destinationArray = new string[sourceArray.Length - 1];
                    Array.Copy(sourceArray, 1, destinationArray, 0, sourceArray.Length - 1);
                    this.setObject(destinationArray, value);
                }
                LuaDLL.lua_settop(this.L, newTop);
            }
        }
    }
}

