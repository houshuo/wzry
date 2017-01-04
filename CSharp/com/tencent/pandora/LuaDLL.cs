namespace com.tencent.pandora
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;

    [SuppressUnmanagedCodeSecurity]
    public class LuaDLL
    {
        public static int LUA_MULTRET = -1;
        private const string LUADLL = "ulua";

        private static string AnsiToUnicode(IntPtr source, int strlen)
        {
            byte[] destination = new byte[strlen];
            Marshal.Copy(source, destination, 0, strlen);
            return Encoding.UTF8.GetString(destination);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_atpanic(IntPtr luaState, LuaCSFunction panicf);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_call(IntPtr luaState, int nArgs, int nResults);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern bool lua_checkstack(IntPtr luaState, int extra);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_close(IntPtr luaState);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_createtable(IntPtr luaState, int narr, int nrec);
        public static int lua_dostring(IntPtr luaState, string chunk)
        {
            return luaL_dostring(luaState, chunk);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_equal(IntPtr luaState, int index1, int index2);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_error(IntPtr luaState);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_gc(IntPtr luaState, LuaGCOptions what, int data);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_getfenv(IntPtr luaState, int stackPos);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_getfield(IntPtr luaState, int stackPos, string meta);
        public static void lua_getglobal(IntPtr luaState, string name)
        {
            lua_pushstring(luaState, name);
            lua_gettable(luaState, LuaIndexes.LUA_GLOBALSINDEX);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_getmetatable(IntPtr luaState, int objIndex);
        public static void lua_getref(IntPtr luaState, int reference)
        {
            lua_rawgeti(luaState, LuaIndexes.LUA_REGISTRYINDEX, reference);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_gettable(IntPtr luaState, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_gettop(IntPtr luaState);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern string lua_getupvalue(IntPtr L, int funcindex, int n);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_insert(IntPtr luaState, int newTop);
        public static bool lua_isboolean(IntPtr luaState, int index)
        {
            return (lua_type(luaState, index) == LuaTypes.LUA_TBOOLEAN);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern bool lua_iscfunction(IntPtr luaState, int index);
        public static int lua_isfunction(IntPtr luaState, int stackPos)
        {
            return Convert.ToInt32(lua_type(luaState, stackPos) == LuaTypes.LUA_TFUNCTION);
        }

        public static int lua_islightuserdata(IntPtr luaState, int stackPos)
        {
            return Convert.ToInt32(lua_type(luaState, stackPos) == LuaTypes.LUA_TLIGHTUSERDATA);
        }

        public static bool lua_isnil(IntPtr luaState, int index)
        {
            return (lua_type(luaState, index) == LuaTypes.LUA_TNIL);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern bool lua_isnumber(IntPtr luaState, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern bool lua_isstring(IntPtr luaState, int index);
        public static int lua_istable(IntPtr luaState, int stackPos)
        {
            return Convert.ToInt32(lua_type(luaState, stackPos) == LuaTypes.LUA_TTABLE);
        }

        public static int lua_isthread(IntPtr luaState, int stackPos)
        {
            return Convert.ToInt32(lua_type(luaState, stackPos) == LuaTypes.LUA_TTHREAD);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_isuserdata(IntPtr luaState, int stackPos);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_lessthan(IntPtr luaState, int stackPos1, int stackPos2);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_load(IntPtr luaState, LuaChunkReader chunkReader, ref ReaderInfo data, string chunkName);
        public static void lua_newtable(IntPtr luaState)
        {
            lua_createtable(luaState, 0, 0);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr lua_newthread(IntPtr L);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr lua_newuserdata(IntPtr luaState, int size);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_next(IntPtr luaState, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_objlen(IntPtr luaState, int stackPos);
        public static IntPtr lua_open()
        {
            return luaL_newstate();
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_pcall(IntPtr luaState, int nArgs, int nResults, int errfunc);
        public static void lua_pop(IntPtr luaState, int amount)
        {
            lua_settop(luaState, -amount - 1);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_pushboolean(IntPtr luaState, bool value);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_pushcclosure(IntPtr luaState, IntPtr fn, int n);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_pushinteger(IntPtr luaState, int number);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_pushlightuserdata(IntPtr luaState, IntPtr udata);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_pushlstring(IntPtr luaState, byte[] str, int size);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_pushnil(IntPtr luaState);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_pushnumber(IntPtr luaState, double number);
        public static void lua_pushstdcallcfunction(IntPtr luaState, LuaCSFunction function, int n = 0)
        {
            IntPtr functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate(function);
            lua_pushcclosure(luaState, functionPointerForDelegate, n);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_pushstring(IntPtr luaState, string str);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_pushthread(IntPtr L);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_pushvalue(IntPtr luaState, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_rawequal(IntPtr luaState, int stackPos1, int stackPos2);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_rawget(IntPtr luaState, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_rawgeti(IntPtr luaState, int tableIndex, int index);
        public static void lua_rawglobal(IntPtr luaState, string name)
        {
            lua_pushstring(luaState, name);
            lua_rawget(luaState, LuaIndexes.LUA_GLOBALSINDEX);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_rawset(IntPtr luaState, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_rawseti(IntPtr luaState, int tableIndex, int index);
        public static int lua_ref(IntPtr luaState, int lockRef)
        {
            if (lockRef != 0)
            {
                return luaL_ref(luaState, LuaIndexes.LUA_REGISTRYINDEX);
            }
            return 0;
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_remove(IntPtr luaState, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_replace(IntPtr luaState, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_resume(IntPtr L, int narg);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_setfenv(IntPtr luaState, int stackPos);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_setfield(IntPtr luaState, int stackPos, string name);
        public static void lua_setglobal(IntPtr luaState, string name)
        {
            lua_setfield(luaState, LuaIndexes.LUA_GLOBALSINDEX, name);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_setmetatable(IntPtr luaState, int objIndex);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_settable(IntPtr luaState, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void lua_settop(IntPtr luaState, int newTop);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_status(IntPtr L);
        public static int lua_strlen(IntPtr luaState, int stackPos)
        {
            return lua_objlen(luaState, stackPos);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern bool lua_toboolean(IntPtr luaState, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr lua_tocfunction(IntPtr luaState, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr lua_tolstring(IntPtr luaState, int index, out int strLen);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern double lua_tonumber(IntPtr luaState, int index);
        public static string lua_tostring(IntPtr luaState, int index)
        {
            int num;
            IntPtr ptr = lua_tolstring(luaState, index, out num);
            if (!(ptr != IntPtr.Zero))
            {
                return null;
            }
            string str = Marshal.PtrToStringAnsi(ptr, num);
            if (str == null)
            {
                return AnsiToUnicode(ptr, num);
            }
            return str;
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_tothread(IntPtr L, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr lua_touserdata(IntPtr luaState, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern LuaTypes lua_type(IntPtr luaState, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern string lua_typename(IntPtr luaState, LuaTypes type);
        public static void lua_unref(IntPtr luaState, int reference)
        {
            luaL_unref(luaState, LuaIndexes.LUA_REGISTRYINDEX, reference);
        }

        public static int lua_upvalueindex(int i)
        {
            return (LuaIndexes.LUA_GLOBALSINDEX - i);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_xmove(IntPtr from, IntPtr to, int n);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int lua_yield(IntPtr L, int nresults);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luaL_argerror(IntPtr luaState, int narg, string extramsg);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luaL_callmeta(IntPtr luaState, int stackPos, string name);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern bool luaL_checkmetatable(IntPtr luaState, int obj);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr luaL_checkudata(IntPtr luaState, int stackPos, string meta);
        public static int luaL_dofile(IntPtr luaState, string fileName)
        {
            int num = luaL_loadfile(luaState, fileName);
            if (num != 0)
            {
                return num;
            }
            return lua_pcall(luaState, 0, -1, 0);
        }

        public static int luaL_dostring(IntPtr luaState, string chunk)
        {
            int num = luaL_loadstring(luaState, chunk);
            if (num != 0)
            {
                return num;
            }
            return lua_pcall(luaState, 0, -1, 0);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void luaL_error(IntPtr luaState, string message);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern LuaTypes luaL_getmetafield(IntPtr luaState, int stackPos, string field);
        public static void luaL_getmetatable(IntPtr luaState, string meta)
        {
            lua_getfield(luaState, LuaIndexes.LUA_REGISTRYINDEX, meta);
        }

        public static int luaL_getn(IntPtr luaState, int i)
        {
            return lua_objlen(luaState, i);
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern string luaL_gsub(IntPtr luaState, string str, string pattern, string replacement);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luaL_loadbuffer(IntPtr luaState, byte[] buff, int size, string name);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luaL_loadfile(IntPtr luaState, string filename);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luaL_loadstring(IntPtr luaState, string chunk);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luaL_newmetatable(IntPtr luaState, string meta);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr luaL_newstate();
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void luaL_openlibs(IntPtr luaState);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luaL_ref(IntPtr luaState, int registryIndex);
        public static string luaL_typename(IntPtr luaState, int stackPos)
        {
            return lua_typename(luaState, lua_type(luaState, stackPos));
        }

        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luaL_typerror(IntPtr luaState, int narg, string tname);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void luaL_unref(IntPtr luaState, int registryIndex, int reference);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void luaL_where(IntPtr luaState, int level);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luanet_checkudata(IntPtr luaState, int obj, string meta);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr luanet_gettag();
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void luanet_newudata(IntPtr luaState, int val);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luanet_rawnetobj(IntPtr luaState, int obj);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luanet_tonetobject(IntPtr luaState, int obj);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luaopen_cjson(IntPtr L);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luaopen_cjson_safe(IntPtr L);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luaopen_lpeg(IntPtr L);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luaopen_pb(IntPtr L);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int luaopen_protobuf_c(IntPtr L);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void tolua_getfloat2(IntPtr luaState, int reference, int stack, ref float x, ref float y);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void tolua_getfloat3(IntPtr luaState, int reference, int stack, ref float x, ref float y, ref float z);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void tolua_getfloat4(IntPtr luaState, int reference, int stack, ref float x, ref float y, ref float z, ref float w);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void tolua_getfloat6(IntPtr luaState, int reference, int stack, ref float x, ref float y, ref float z, ref float x1, ref float y1, ref float z1);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern int tolua_openlibs(IntPtr L);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void tolua_pushfloat2(IntPtr luaState, int reference, float x, float y);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void tolua_pushfloat3(IntPtr luaState, int reference, float x, float y, float z);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void tolua_pushfloat4(IntPtr luaState, int reference, float x, float y, float z, float w);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern bool tolua_pushnewudata(IntPtr L, int metaRef, int weakTableRef, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern bool tolua_pushudata(IntPtr L, int reference, int index);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void tolua_setindex(IntPtr L);
        [DllImport("ulua", CallingConvention=CallingConvention.Cdecl)]
        public static extern void tolua_setnewindex(IntPtr L);
    }
}

