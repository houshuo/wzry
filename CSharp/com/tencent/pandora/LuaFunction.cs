namespace com.tencent.pandora
{
    using System;

    public class LuaFunction : LuaBase
    {
        private int beginPos;
        internal LuaCSFunction function;
        private IntPtr L;

        public LuaFunction(LuaCSFunction function, LuaState interpreter)
        {
            this.beginPos = -1;
            base._Reference = 0;
            this.function = function;
            base._Interpreter = interpreter;
            this.L = base._Interpreter.L;
            base.translator = base._Interpreter.translator;
        }

        public LuaFunction(int reference, LuaState interpreter)
        {
            this.beginPos = -1;
            base._Reference = reference;
            this.function = null;
            base._Interpreter = interpreter;
            this.L = base._Interpreter.L;
            base.translator = base._Interpreter.translator;
        }

        public LuaFunction(int reference, IntPtr l)
        {
            this.beginPos = -1;
            base._Reference = reference;
            this.function = null;
            this.L = l;
            base.translator = ObjectTranslator.FromState(this.L);
            base._Interpreter = base.translator.interpreter;
        }

        public int BeginPCall()
        {
            LuaScriptMgr.PushTraceBack(this.L);
            this.beginPos = LuaDLL.lua_gettop(this.L);
            this.push(this.L);
            return this.beginPos;
        }

        internal object[] call(object[] args, Type[] returnTypes)
        {
            int nArgs = 0;
            LuaScriptMgr.PushTraceBack(this.L);
            int oldTop = LuaDLL.lua_gettop(this.L);
            if (!LuaDLL.lua_checkstack(this.L, args.Length + 6))
            {
                LuaDLL.lua_pop(this.L, 1);
                throw new LuaException("Lua stack overflow");
            }
            this.push(this.L);
            if (args != null)
            {
                nArgs = args.Length;
                for (int i = 0; i < args.Length; i++)
                {
                    base.PushArgs(this.L, args[i]);
                }
            }
            if (LuaDLL.lua_pcall(this.L, nArgs, -1, -nArgs - 2) != 0)
            {
                string message = LuaDLL.lua_tostring(this.L, -1);
                LuaDLL.lua_settop(this.L, oldTop - 1);
                if (message == null)
                {
                    message = "Unknown Lua Error";
                }
                throw new LuaScriptException(message, string.Empty);
            }
            object[] objArray = (returnTypes == null) ? base.translator.popValues(this.L, oldTop) : base.translator.popValues(this.L, oldTop, returnTypes);
            LuaDLL.lua_settop(this.L, oldTop - 1);
            return objArray;
        }

        public object[] Call()
        {
            int oldTop = this.BeginPCall();
            if (this.PCall(oldTop, 0))
            {
                object[] objArray = this.PopValues(oldTop);
                this.EndPCall(oldTop);
                return objArray;
            }
            LuaDLL.lua_settop(this.L, oldTop - 1);
            return null;
        }

        public object[] Call(params object[] args)
        {
            try
            {
                return this.call(args, null);
            }
            catch (Exception exception)
            {
                Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
                return null;
            }
        }

        public object[] Call(double arg1)
        {
            int oldTop = this.BeginPCall();
            LuaScriptMgr.Push(this.L, arg1);
            if (this.PCall(oldTop, 1))
            {
                object[] objArray = this.PopValues(oldTop);
                this.EndPCall(oldTop);
                return objArray;
            }
            LuaDLL.lua_settop(this.L, oldTop - 1);
            return null;
        }

        public void EndPCall(int oldTop)
        {
            LuaDLL.lua_settop(this.L, oldTop - 1);
        }

        public override bool Equals(object o)
        {
            if (!(o is LuaFunction))
            {
                return false;
            }
            LuaFunction function = (LuaFunction) o;
            if ((base._Reference != 0) && (function._Reference != 0))
            {
                return base._Interpreter.compareRef(function._Reference, base._Reference);
            }
            return (this.function == function.function);
        }

        public override int GetHashCode()
        {
            if (base._Reference != 0)
            {
                return base._Reference;
            }
            return this.function.GetHashCode();
        }

        public IntPtr GetLuaState()
        {
            return this.L;
        }

        public int GetReference()
        {
            return base._Reference;
        }

        public bool PCall(int oldTop, int args)
        {
            if (LuaDLL.lua_pcall(this.L, args, -1, -args - 2) == 0)
            {
                return true;
            }
            string message = LuaDLL.lua_tostring(this.L, -1);
            LuaDLL.lua_settop(this.L, oldTop - 1);
            if (message == null)
            {
                message = "Unknown Lua Error";
            }
            throw new LuaScriptException(message, string.Empty);
        }

        public object[] PopValues(int oldTop)
        {
            return base.translator.popValues(this.L, oldTop);
        }

        internal void push()
        {
            if (base._Reference != 0)
            {
                LuaDLL.lua_getref(this.L, base._Reference);
            }
            else
            {
                base._Interpreter.pushCSFunction(this.function);
            }
        }

        internal void push(IntPtr luaState)
        {
            if (base._Reference != 0)
            {
                LuaDLL.lua_getref(luaState, base._Reference);
            }
            else
            {
                base._Interpreter.pushCSFunction(this.function);
            }
        }

        public override string ToString()
        {
            return "function";
        }
    }
}

