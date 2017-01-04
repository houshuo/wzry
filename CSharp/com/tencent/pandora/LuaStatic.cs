namespace com.tencent.pandora
{
    using System;

    public static class LuaStatic
    {
        public static string init_luanet = "local metatable = {}\r\n            local rawget = rawget\r\n            local debug = debug\r\n            local import_type = luanet.import_type\r\n            local load_assembly = luanet.load_assembly\r\n            luanet.error, luanet.type = error, type\r\n            -- Lookup a .NET identifier component.\r\n            function metatable:__index(key) -- key is e.g. 'Form'\r\n            -- Get the fully-qualified name, e.g. 'System.Windows.Forms.Form'\r\n            local fqn = rawget(self,'.fqn')\r\n            fqn = ((fqn and fqn .. '.') or '') .. key\r\n\r\n            -- Try to find either a luanet function or a CLR type\r\n            local obj = rawget(luanet,key) or import_type(fqn)\r\n\r\n            -- If key is neither a luanet function or a CLR type, then it is simply\r\n            -- an identifier component.\r\n            if obj == nil then\r\n                -- It might be an assembly, so we load it too.\r\n                    pcall(load_assembly,fqn)\r\n                    obj = { ['.fqn'] = fqn }\r\n            setmetatable(obj, metatable)\r\n            end\r\n\r\n            -- Cache this lookup\r\n            rawset(self, key, obj)\r\n            return obj\r\n            end\r\n\r\n            -- A non-type has been called; e.g. foo = System.Foo()\r\n            function metatable:__call(...)\r\n            error('No such type: ' .. rawget(self,'.fqn'), 2)\r\n            end\r\n\r\n            -- This is the root of the .NET namespace\r\n            luanet['.fqn'] = false\r\n            setmetatable(luanet, metatable)\r\n\r\n            -- Preload the mscorlib assembly\r\n            luanet.load_assembly('mscorlib')\r\n\r\n            function traceback(msg)                \r\n                return debug.traceback(msg, 1)                \r\n            end";
        public static ReadLuaFile Load = new ReadLuaFile(LuaStatic.DefaultLoader);

        private static byte[] DefaultLoader(string path)
        {
            path = path.Replace("/", ".");
            char[] separator = new char[] { '.' };
            if (path.Split(separator).Length > 1)
            {
                char[] chArray2 = new char[] { '.' };
                char[] chArray3 = new char[] { '.' };
                path = path.Split(chArray2)[path.Split(chArray3).Length - 1];
            }
            return LuaHelper.GetResManager().luaFiles[path].bytes;
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int dofile(IntPtr L)
        {
            string name = string.Empty;
            name = LuaDLL.lua_tostring(L, 1);
            if (name.ToLower().EndsWith(".lua"))
            {
                int length = name.LastIndexOf('.');
                name = name.Substring(0, length);
            }
            name = name.Replace('.', '/');
            int num2 = LuaDLL.lua_gettop(L);
            byte[] buff = Load(name);
            if ((buff != null) && (LuaDLL.luaL_loadbuffer(L, buff, buff.Length, name) == 0))
            {
                LuaDLL.lua_call(L, 0, LuaDLL.LUA_MULTRET);
            }
            return (LuaDLL.lua_gettop(L) - num2);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int importWrap(IntPtr L)
        {
            string str = string.Empty;
            str = LuaDLL.lua_tostring(L, 1).Replace('.', '_');
            if (!string.IsNullOrEmpty(str))
            {
                LuaBinder.Bind(L, str);
            }
            return 0;
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int loader(IntPtr L)
        {
            string name = string.Empty;
            name = LuaDLL.lua_tostring(L, 1);
            if (name.ToLower().EndsWith(".lua"))
            {
                int length = name.LastIndexOf('.');
                name = name.Substring(0, length);
            }
            LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
            if (mgrFromLuaState == null)
            {
                return 0;
            }
            LuaDLL.lua_pushstdcallcfunction(L, mgrFromLuaState.lua.tracebackFunction, 0);
            int oldTop = LuaDLL.lua_gettop(L);
            byte[] buff = Load(name);
            if (buff == null)
            {
                if (!name.Contains("mobdebug"))
                {
                }
                LuaDLL.lua_pop(L, 1);
                return 0;
            }
            if (LuaDLL.luaL_loadbuffer(L, buff, buff.Length, name) != 0)
            {
                mgrFromLuaState.lua.ThrowExceptionFromError(oldTop);
                LuaDLL.lua_pop(L, 1);
            }
            return 1;
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int loadfile(IntPtr L)
        {
            return loader(L);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int panic(IntPtr L)
        {
            string message = string.Format("unprotected error in call to Lua API ({0})", LuaDLL.lua_tostring(L, -1));
            LuaDLL.lua_pop(L, 1);
            throw new LuaException(message);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int print(IntPtr L)
        {
            int num = LuaDLL.lua_gettop(L);
            string str = string.Empty;
            LuaDLL.lua_getglobal(L, "tostring");
            for (int i = 1; i <= num; i++)
            {
                LuaDLL.lua_pushvalue(L, -1);
                LuaDLL.lua_pushvalue(L, i);
                LuaDLL.lua_call(L, 1, 1);
                if (i > 1)
                {
                    str = str + "\t";
                }
                str = str + LuaDLL.lua_tostring(L, -1);
                LuaDLL.lua_pop(L, 1);
            }
            return 0;
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int traceback(IntPtr L)
        {
            LuaDLL.lua_getglobal(L, "debug");
            LuaDLL.lua_getfield(L, -1, "traceback");
            LuaDLL.lua_pushvalue(L, 1);
            LuaDLL.lua_pushnumber(L, 2.0);
            LuaDLL.lua_call(L, 2, 1);
            return 1;
        }
    }
}

