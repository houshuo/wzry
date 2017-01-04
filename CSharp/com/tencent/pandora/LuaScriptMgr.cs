namespace com.tencent.pandora
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class LuaScriptMgr
    {
        private static ObjectTranslator _translator = null;
        private int arrayMetaRef;
        private static HashSet<System.Type> checkBaseType = new HashSet<System.Type>();
        private int delegateMetaRef;
        private Dictionary<string, LuaBase> dict;
        private static Dictionary<Enum, object> enumMap = new Dictionary<Enum, object>();
        private int enumMetaRef;
        public HashSet<string> fileList;
        private LuaFunction fixedUpdateFunc;
        private int iterMetaRef;
        private LuaFunction lateUpdateFunc;
        private LuaFunction levelLoaded;
        public LuaState lua;
        private string luaEnumIndex = "\n        local rawget = rawget                \n        local getmetatable = getmetatable         \n\n        local function indexEnum(obj,name)\n            local v = rawget(obj, name)\n            \n            if v ~= nil then\n                return v\n            end\n\n            local meta = getmetatable(obj)  \n            local func = rawget(meta, name)            \n            \n            if func ~= nil then\n                v = func()\n                rawset(obj, name, v)\n                return v\n            else\n                error('field '..name..' does not exist', 2)\n            end\n        end\n\n        return indexEnum\n    ";
        private string luaIndex = "        \n        local rawget = rawget\n        local rawset = rawset\n        local getmetatable = getmetatable      \n        local type = type  \n        local function index(obj,name)  \n            local o = obj            \n            local meta = getmetatable(o)            \n            local parent = meta\n            local v = nil\n            \n            while meta~= nil do\n                v = rawget(meta, name)\n                \n                if v~= nil then\n                    if parent ~= meta then rawset(parent, name, v) end\n\n                    local t = type(v)\n\n                    if t == 'function' then                    \n                        return v\n                    else\n                        local func = v[1]\n                \n                        if func ~= nil then\n                            return func(obj)                         \n                        end\n                    end\n                    break\n                end\n                \n                meta = getmetatable(meta)\n            end\n\n           error('unknown member name '..name, 2)\n           return nil\t        \n        end\n        return index";
        private string luaNewIndex = "\n        local rawget = rawget\n        local getmetatable = getmetatable   \n        local rawset = rawset     \n        local function newindex(obj, name, val)            \n            local meta = getmetatable(obj)            \n            local parent = meta\n            local v = nil\n            \n            while meta~= nil do\n                v = rawget(meta, name)\n                \n                if v~= nil then\n                    if parent ~= meta then rawset(parent, name, v) end\n                    local func = v[2]\n                    if func ~= nil then                        \n                        return func(obj, nil, val)                        \n                    end\n                    break\n                end\n                \n                meta = getmetatable(meta)\n            end  \n       \n            error('field or property '..name..' does not exist', 2)\n            return nil\t\t\n        end\n        return newindex";
        private string luaTableCall = "\n        local rawget = rawget\n        local getmetatable = getmetatable     \n\n        local function call(obj, ...)\n            local meta = getmetatable(obj)\n            local fun = rawget(meta, 'New')\n            \n            if fun ~= nil then\n                return fun(...)\n            else\n                error('unknow function __call',2)\n            end\n        end\n\n        return call\n    ";
        private static System.Type monoType = typeof(System.Type).GetType();
        private int packBounds;
        private int packColor;
        private int packQuat;
        private int packRay;
        private LuaFunction packRaycastHit;
        private LuaFunction packTouch;
        private int packVec2;
        private int packVec3;
        private int packVec4;
        public static LockFreeQueue<LuaRef> refGCList = new LockFreeQueue<LuaRef>(0x400);
        private static LuaFunction traceback = null;
        private int typeMetaRef;
        private int unpackBounds;
        private int unpackColor;
        private int unpackQuat;
        private int unpackRay;
        private int unpackVec2;
        private int unpackVec3;
        private int unpackVec4;
        private LuaFunction updateFunc;

        public LuaScriptMgr()
        {
            Instance = this;
            LuaStatic.Load = new ReadLuaFile(this.Loader);
            this.lua = new LuaState();
            _translator = this.lua.GetTranslator();
            LuaDLL.luaopen_cjson(this.lua.L);
            LuaDLL.luaopen_cjson_safe(this.lua.L);
            LuaDLL.tolua_openlibs(this.lua.L);
            this.fileList = new HashSet<string>();
            this.dict = new Dictionary<string, LuaBase>();
            LuaDLL.lua_pushstring(this.lua.L, "ToLua_Index");
            LuaDLL.luaL_dostring(this.lua.L, this.luaIndex);
            LuaDLL.lua_rawset(this.lua.L, LuaIndexes.LUA_REGISTRYINDEX);
            LuaDLL.lua_pushstring(this.lua.L, "ToLua_NewIndex");
            LuaDLL.luaL_dostring(this.lua.L, this.luaNewIndex);
            LuaDLL.lua_rawset(this.lua.L, LuaIndexes.LUA_REGISTRYINDEX);
            LuaDLL.lua_pushstring(this.lua.L, "ToLua_TableCall");
            LuaDLL.luaL_dostring(this.lua.L, this.luaTableCall);
            LuaDLL.lua_rawset(this.lua.L, LuaIndexes.LUA_REGISTRYINDEX);
            LuaDLL.lua_pushstring(this.lua.L, "ToLua_EnumIndex");
            LuaDLL.luaL_dostring(this.lua.L, this.luaEnumIndex);
            LuaDLL.lua_rawset(this.lua.L, LuaIndexes.LUA_REGISTRYINDEX);
            this.Bind();
            LuaDLL.lua_pushnumber(this.lua.L, 0.0);
            LuaDLL.lua_setglobal(this.lua.L, "_LuaScriptMgr");
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int __gc(IntPtr luaState)
        {
            int udata = LuaDLL.luanet_rawnetobj(luaState, 1);
            if (udata != -1)
            {
                ObjectTranslator.FromState(luaState).collectObject(udata);
            }
            return 0;
        }

        private void Bind()
        {
        }

        private void BindArray(IntPtr L)
        {
            LuaDLL.luaL_newmetatable(L, "luaNet_array");
            LuaDLL.lua_pushstring(L, "__index");
            LuaDLL.lua_pushstdcallcfunction(L, new LuaCSFunction(LuaScriptMgr.IndexArray), 0);
            LuaDLL.lua_rawset(L, -3);
            LuaDLL.lua_pushstring(L, "__gc");
            LuaDLL.lua_pushstdcallcfunction(L, new LuaCSFunction(LuaScriptMgr.__gc), 0);
            LuaDLL.lua_rawset(L, -3);
            LuaDLL.lua_pushstring(L, "__newindex");
            LuaDLL.lua_pushstdcallcfunction(L, new LuaCSFunction(LuaScriptMgr.NewIndexArray), 0);
            LuaDLL.lua_rawset(L, -3);
            this.arrayMetaRef = LuaDLL.luaL_ref(this.lua.L, LuaIndexes.LUA_REGISTRYINDEX);
            LuaDLL.lua_settop(L, 0);
        }

        public object[] CallLuaFunction(string name, params object[] args)
        {
            try
            {
                LuaBase base2 = null;
                if (this.dict.TryGetValue(name, out base2))
                {
                    LuaFunction function = base2 as LuaFunction;
                    return function.Call(args);
                }
                IntPtr l = this.lua.L;
                LuaFunction function2 = null;
                int newTop = LuaDLL.lua_gettop(l);
                if (PushLuaFunction(l, name))
                {
                    function2 = new LuaFunction(LuaDLL.luaL_ref(l, LuaIndexes.LUA_REGISTRYINDEX), this.lua);
                    LuaDLL.lua_settop(l, newTop);
                    object[] objArray = function2.Call(args);
                    function2.Dispose();
                    return objArray;
                }
                return null;
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
                return null;
            }
        }

        public static void CheckArgsCount(IntPtr L, int count)
        {
            int num = LuaDLL.lua_gettop(L);
            if (num != count)
            {
                string message = string.Format("no overload for method '{0}' takes '{1}' arguments", GetErrorFunc(1), num);
                LuaDLL.luaL_error(L, message);
            }
        }

        public static bool CheckParamsType(IntPtr L, System.Type t, int begin, int count)
        {
            if (t != typeof(object))
            {
                for (int i = 0; i < count; i++)
                {
                    if (!CheckType(L, t, i + begin))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool CheckTableType(IntPtr L, System.Type t, int stackPos)
        {
            if (t.IsArray)
            {
                return true;
            }
            if (t == typeof(LuaTable))
            {
                return true;
            }
            if (t.IsValueType)
            {
                int newTop = LuaDLL.lua_gettop(L);
                LuaDLL.lua_pushvalue(L, stackPos);
                LuaDLL.lua_pushstring(L, "class");
                LuaDLL.lua_gettable(L, -2);
                string str = LuaDLL.lua_tostring(L, -1);
                LuaDLL.lua_settop(L, newTop);
                switch (str)
                {
                    case "Vector3":
                        return (t == typeof(Vector3));

                    case "Vector2":
                        return (t == typeof(Vector2));

                    case "Quaternion":
                        return (t == typeof(Quaternion));

                    case "Color":
                        return (t == typeof(Color));

                    case "Vector4":
                        return (t == typeof(Vector4));

                    case "Ray":
                        return (t == typeof(Ray));
                }
            }
            return false;
        }

        public static bool CheckType(IntPtr L, System.Type t, int pos)
        {
            if (t == typeof(object))
            {
                return true;
            }
            LuaTypes luaType = LuaDLL.lua_type(L, pos);
            switch (luaType)
            {
                case LuaTypes.LUA_TNIL:
                    return (t == null);

                case LuaTypes.LUA_TBOOLEAN:
                    return (t == typeof(bool));

                case LuaTypes.LUA_TNUMBER:
                    return t.IsPrimitive;

                case LuaTypes.LUA_TSTRING:
                    return (t == typeof(string));

                case LuaTypes.LUA_TTABLE:
                    return CheckTableType(L, t, pos);

                case LuaTypes.LUA_TFUNCTION:
                    return (t == typeof(LuaFunction));

                case LuaTypes.LUA_TUSERDATA:
                    return CheckUserData(L, luaType, t, pos);
            }
            return false;
        }

        public static bool CheckTypes(IntPtr L, int begin, System.Type type0)
        {
            return CheckType(L, type0, begin);
        }

        public static bool CheckTypes(IntPtr L, int begin, params System.Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (!CheckType(L, types[i], i + begin))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckTypes(IntPtr L, int begin, System.Type type0, System.Type type1)
        {
            return (CheckType(L, type0, begin) && CheckType(L, type1, begin + 1));
        }

        public static bool CheckTypes(IntPtr L, int begin, System.Type type0, System.Type type1, System.Type type2)
        {
            return ((CheckType(L, type0, begin) && CheckType(L, type1, begin + 1)) && CheckType(L, type2, begin + 2));
        }

        public static bool CheckTypes(IntPtr L, int begin, System.Type type0, System.Type type1, System.Type type2, System.Type type3)
        {
            return (((CheckType(L, type0, begin) && CheckType(L, type1, begin + 1)) && CheckType(L, type2, begin + 2)) && CheckType(L, type3, begin + 3));
        }

        public static bool CheckTypes(IntPtr L, int begin, System.Type type0, System.Type type1, System.Type type2, System.Type type3, System.Type type4)
        {
            return (((CheckType(L, type0, begin) && CheckType(L, type1, begin + 1)) && (CheckType(L, type2, begin + 2) && CheckType(L, type3, begin + 3))) && CheckType(L, type4, begin + 4));
        }

        public static bool CheckTypes(IntPtr L, int begin, System.Type type0, System.Type type1, System.Type type2, System.Type type3, System.Type type4, System.Type type5)
        {
            return ((((CheckType(L, type0, begin) && CheckType(L, type1, begin + 1)) && (CheckType(L, type2, begin + 2) && CheckType(L, type3, begin + 3))) && CheckType(L, type4, begin + 4)) && CheckType(L, type5, begin + 5));
        }

        public static bool CheckTypes(IntPtr L, int begin, System.Type type0, System.Type type1, System.Type type2, System.Type type3, System.Type type4, System.Type type5, System.Type type6)
        {
            return ((((CheckType(L, type0, begin) && CheckType(L, type1, begin + 1)) && (CheckType(L, type2, begin + 2) && CheckType(L, type3, begin + 3))) && (CheckType(L, type4, begin + 4) && CheckType(L, type5, begin + 5))) && CheckType(L, type6, begin + 6));
        }

        public static bool CheckTypes(IntPtr L, int begin, System.Type type0, System.Type type1, System.Type type2, System.Type type3, System.Type type4, System.Type type5, System.Type type6, System.Type type7)
        {
            return ((((CheckType(L, type0, begin) && CheckType(L, type1, begin + 1)) && (CheckType(L, type2, begin + 2) && CheckType(L, type3, begin + 3))) && ((CheckType(L, type4, begin + 4) && CheckType(L, type5, begin + 5)) && CheckType(L, type6, begin + 6))) && CheckType(L, type7, begin + 7));
        }

        public static bool CheckTypes(IntPtr L, int begin, System.Type type0, System.Type type1, System.Type type2, System.Type type3, System.Type type4, System.Type type5, System.Type type6, System.Type type7, System.Type type8)
        {
            return ((((CheckType(L, type0, begin) && CheckType(L, type1, begin + 1)) && (CheckType(L, type2, begin + 2) && CheckType(L, type3, begin + 3))) && ((CheckType(L, type4, begin + 4) && CheckType(L, type5, begin + 5)) && (CheckType(L, type6, begin + 6) && CheckType(L, type7, begin + 7)))) && CheckType(L, type8, begin + 8));
        }

        public static bool CheckTypes(IntPtr L, int begin, System.Type type0, System.Type type1, System.Type type2, System.Type type3, System.Type type4, System.Type type5, System.Type type6, System.Type type7, System.Type type8, System.Type type9)
        {
            return (((((CheckType(L, type0, begin) && CheckType(L, type1, begin + 1)) && (CheckType(L, type2, begin + 2) && CheckType(L, type3, begin + 3))) && ((CheckType(L, type4, begin + 4) && CheckType(L, type5, begin + 5)) && (CheckType(L, type6, begin + 6) && CheckType(L, type7, begin + 7)))) && CheckType(L, type8, begin + 8)) && CheckType(L, type9, begin + 9));
        }

        private static bool CheckUserData(IntPtr L, LuaTypes luaType, System.Type t, int pos)
        {
            System.Type c = GetLuaObject(L, pos).GetType();
            if (t == c)
            {
                return true;
            }
            if (t == typeof(System.Type))
            {
                return (c == monoType);
            }
            return t.IsAssignableFrom(c);
        }

        private static void CreateTable(IntPtr L, string fullPath)
        {
            char[] separator = new char[] { '.' };
            string[] strArray = fullPath.Split(separator);
            int num = LuaDLL.lua_gettop(L);
            if (strArray.Length > 1)
            {
                LuaDLL.lua_getglobal(L, strArray[0]);
                if (LuaDLL.lua_type(L, -1) == LuaTypes.LUA_TNIL)
                {
                    LuaDLL.lua_pop(L, 1);
                    LuaDLL.lua_createtable(L, 0, 0);
                    LuaDLL.lua_pushstring(L, strArray[0]);
                    LuaDLL.lua_pushvalue(L, -2);
                    LuaDLL.lua_settable(L, LuaIndexes.LUA_GLOBALSINDEX);
                }
                for (int i = 1; i < (strArray.Length - 1); i++)
                {
                    LuaDLL.lua_pushstring(L, strArray[i]);
                    LuaDLL.lua_rawget(L, -2);
                    if (LuaDLL.lua_type(L, -1) == LuaTypes.LUA_TNIL)
                    {
                        LuaDLL.lua_pop(L, 1);
                        LuaDLL.lua_createtable(L, 0, 0);
                        LuaDLL.lua_pushstring(L, strArray[i]);
                        LuaDLL.lua_pushvalue(L, -2);
                        LuaDLL.lua_rawset(L, -4);
                    }
                }
                LuaDLL.lua_pushstring(L, strArray[strArray.Length - 1]);
                LuaDLL.lua_rawget(L, -2);
                if (LuaDLL.lua_type(L, -1) == LuaTypes.LUA_TNIL)
                {
                    LuaDLL.lua_pop(L, 1);
                    LuaDLL.lua_createtable(L, 0, 0);
                    LuaDLL.lua_pushstring(L, strArray[strArray.Length - 1]);
                    LuaDLL.lua_pushvalue(L, -2);
                    LuaDLL.lua_rawset(L, -4);
                }
            }
            else
            {
                LuaDLL.lua_getglobal(L, strArray[0]);
                if (LuaDLL.lua_type(L, -1) == LuaTypes.LUA_TNIL)
                {
                    LuaDLL.lua_pop(L, 1);
                    LuaDLL.lua_createtable(L, 0, 0);
                    LuaDLL.lua_pushstring(L, strArray[0]);
                    LuaDLL.lua_pushvalue(L, -2);
                    LuaDLL.lua_settable(L, LuaIndexes.LUA_GLOBALSINDEX);
                }
            }
            LuaDLL.lua_insert(L, num + 1);
            LuaDLL.lua_settop(L, num + 1);
        }

        public void Destroy()
        {
            Instance = null;
            this.SafeUnRef(ref this.enumMetaRef);
            this.SafeUnRef(ref this.typeMetaRef);
            this.SafeUnRef(ref this.delegateMetaRef);
            this.SafeUnRef(ref this.iterMetaRef);
            this.SafeUnRef(ref this.arrayMetaRef);
            this.SafeRelease(ref this.packRaycastHit);
            this.SafeRelease(ref this.packTouch);
            this.SafeRelease(ref this.updateFunc);
            this.SafeRelease(ref this.lateUpdateFunc);
            this.SafeRelease(ref this.fixedUpdateFunc);
            LuaDLL.lua_gc(this.lua.L, LuaGCOptions.LUA_GCCOLLECT, 0);
            foreach (KeyValuePair<string, LuaBase> pair in this.dict)
            {
                pair.Value.Dispose();
            }
            this.dict.Clear();
            this.fileList.Clear();
            this.lua.Close();
            this.lua.Dispose();
            this.lua = null;
            DelegateFactory.Clear();
            LuaBinder.wrapList.Clear();
            Debugger.Log("Lua module destroy", new object[0]);
        }

        public object[] DoFile(string fileName)
        {
            if (!this.fileList.Contains(fileName))
            {
                return this.lua.DoFile(fileName, null);
            }
            return null;
        }

        public object[] DoString(string nkey)
        {
            try
            {
                if ((LuaHelper.GetResManager().luaFiles != null) && LuaHelper.GetResManager().luaFiles.ContainsKey(nkey))
                {
                    if (LuaHelper.GetResManager().luaFiles[nkey] != null)
                    {
                        return this.lua.DoString(LuaHelper.GetResManager().luaFiles[nkey].text);
                    }
                    com.tencent.pandora.Logger.d("加载的此lua文件内容为空");
                }
                else
                {
                    com.tencent.pandora.Logger.d("加载的lua列表中无此文件," + nkey);
                }
                return null;
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d("FtError:DS:" + exception.ToString());
                return null;
            }
        }

        public static void DumpStack(IntPtr L)
        {
            int num = LuaDLL.lua_gettop(L);
            for (int i = 1; i <= num; i++)
            {
                LuaTypes types = LuaDLL.lua_type(L, i);
                switch (types)
                {
                    case LuaTypes.LUA_TBOOLEAN:
                    {
                        Debugger.Log(LuaDLL.lua_toboolean(L, i).ToString(), new object[0]);
                        continue;
                    }
                    case LuaTypes.LUA_TNUMBER:
                    {
                        Debugger.Log(LuaDLL.lua_tonumber(L, i).ToString(), new object[0]);
                        continue;
                    }
                    case LuaTypes.LUA_TSTRING:
                    {
                        Debugger.Log(LuaDLL.lua_tostring(L, i), new object[0]);
                        continue;
                    }
                }
                Debugger.Log(types.ToString(), new object[0]);
            }
        }

        public void FixedUpdate()
        {
            try
            {
                if (this.fixedUpdateFunc != null)
                {
                    this.fixedUpdateFunc.Call((double) Time.fixedDeltaTime);
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        public static bool[] GetArrayBool(IntPtr L, int stackPos)
        {
            LuaTypes types = LuaDLL.lua_type(L, stackPos);
            if (types == LuaTypes.LUA_TTABLE)
            {
                int index = 1;
                List<bool> list = new List<bool>();
                LuaDLL.lua_pushvalue(L, stackPos);
                while (true)
                {
                    LuaDLL.lua_rawgeti(L, -1, index);
                    types = LuaDLL.lua_type(L, -1);
                    if (types == LuaTypes.LUA_TNIL)
                    {
                        LuaDLL.lua_pop(L, 1);
                        return list.ToArray();
                    }
                    if (types != LuaTypes.LUA_TNUMBER)
                    {
                        goto Label_0099;
                    }
                    bool item = LuaDLL.lua_toboolean(L, -1);
                    list.Add(item);
                    LuaDLL.lua_pop(L, 1);
                    index++;
                }
            }
            switch (types)
            {
                case LuaTypes.LUA_TUSERDATA:
                {
                    bool[] netObject = GetNetObject<bool[]>(L, stackPos);
                    if (netObject == null)
                    {
                        break;
                    }
                    return netObject;
                }
                case LuaTypes.LUA_TNIL:
                    return null;
            }
        Label_0099:
            LuaDLL.luaL_error(L, string.Format("invalid arguments to method: {0}, pos {1}", GetErrorFunc(1), stackPos));
            return null;
        }

        public static T[] GetArrayNumber<T>(IntPtr L, int stackPos)
        {
            LuaTypes types = LuaDLL.lua_type(L, stackPos);
            if (types == LuaTypes.LUA_TTABLE)
            {
                int index = 1;
                T item = default(T);
                List<T> list = new List<T>();
                LuaDLL.lua_pushvalue(L, stackPos);
                while (true)
                {
                    LuaDLL.lua_rawgeti(L, -1, index);
                    types = LuaDLL.lua_type(L, -1);
                    if (types == LuaTypes.LUA_TNIL)
                    {
                        LuaDLL.lua_pop(L, 1);
                        return list.ToArray();
                    }
                    if (types != LuaTypes.LUA_TNUMBER)
                    {
                        goto Label_00BD;
                    }
                    item = (T) Convert.ChangeType(LuaDLL.lua_tonumber(L, -1), typeof(T));
                    list.Add(item);
                    LuaDLL.lua_pop(L, 1);
                    index++;
                }
            }
            switch (types)
            {
                case LuaTypes.LUA_TUSERDATA:
                {
                    T[] netObject = GetNetObject<T[]>(L, stackPos);
                    if (netObject == null)
                    {
                        break;
                    }
                    return netObject;
                }
                case LuaTypes.LUA_TNIL:
                    return null;
            }
        Label_00BD:
            LuaDLL.luaL_error(L, string.Format("invalid arguments to method: {0}, pos {1}", GetErrorFunc(1), stackPos));
            return null;
        }

        public static T[] GetArrayObject<T>(IntPtr L, int stackPos)
        {
            LuaTypes types = LuaDLL.lua_type(L, stackPos);
            if (types == LuaTypes.LUA_TTABLE)
            {
                int index = 1;
                T item = default(T);
                List<T> list = new List<T>();
                LuaDLL.lua_pushvalue(L, stackPos);
                System.Type t = typeof(T);
                while (true)
                {
                    LuaDLL.lua_rawgeti(L, -1, index);
                    if (LuaDLL.lua_type(L, -1) == LuaTypes.LUA_TNIL)
                    {
                        LuaDLL.lua_pop(L, 1);
                        return list.ToArray();
                    }
                    if (!CheckType(L, t, -1))
                    {
                        LuaDLL.lua_pop(L, 1);
                        goto Label_00C3;
                    }
                    item = (T) GetVarObject(L, -1);
                    list.Add(item);
                    LuaDLL.lua_pop(L, 1);
                    index++;
                }
            }
            switch (types)
            {
                case LuaTypes.LUA_TUSERDATA:
                {
                    T[] netObject = GetNetObject<T[]>(L, stackPos);
                    if (netObject == null)
                    {
                        break;
                    }
                    return netObject;
                }
                case LuaTypes.LUA_TNIL:
                    return null;
            }
        Label_00C3:
            LuaDLL.luaL_error(L, string.Format("invalid arguments to method: {0}, pos {1}", GetErrorFunc(1), stackPos));
            return null;
        }

        public static string[] GetArrayString(IntPtr L, int stackPos)
        {
            LuaTypes types = LuaDLL.lua_type(L, stackPos);
            if (types == LuaTypes.LUA_TTABLE)
            {
                int index = 1;
                string item = null;
                List<string> list = new List<string>();
                LuaDLL.lua_pushvalue(L, stackPos);
                while (true)
                {
                    LuaDLL.lua_rawgeti(L, -1, index);
                    if (LuaDLL.lua_type(L, -1) == LuaTypes.LUA_TNIL)
                    {
                        LuaDLL.lua_pop(L, 1);
                        return list.ToArray();
                    }
                    item = GetLuaString(L, -1);
                    list.Add(item);
                    LuaDLL.lua_pop(L, 1);
                    index++;
                }
            }
            switch (types)
            {
                case LuaTypes.LUA_TUSERDATA:
                {
                    string[] netObject = GetNetObject<string[]>(L, stackPos);
                    if (netObject == null)
                    {
                        break;
                    }
                    return netObject;
                }
                case LuaTypes.LUA_TNIL:
                    return null;
            }
            LuaDLL.luaL_error(L, string.Format("invalid arguments to method: {0}, pos {1}", GetErrorFunc(1), stackPos));
            return null;
        }

        public static bool GetBoolean(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_isboolean(L, stackPos))
            {
                return LuaDLL.lua_toboolean(L, stackPos);
            }
            LuaDLL.luaL_typerror(L, stackPos, "boolean");
            return false;
        }

        public static Bounds GetBounds(IntPtr L, int stackPos)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            float x = 0f;
            float y = 0f;
            float z = 0f;
            float num4 = 0f;
            float num5 = 0f;
            float num6 = 0f;
            LuaDLL.tolua_getfloat6(L, mgrFromLuaState.unpackBounds, stackPos, ref x, ref y, ref z, ref num4, ref num5, ref num6);
            Vector3 center = new Vector3(x, y, z);
            return new Bounds(center, new Vector3(num4, num5, num6));
        }

        public static Color GetColor(IntPtr L, int stackPos)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            float x = 0f;
            float y = 0f;
            float z = 0f;
            float w = 0f;
            LuaDLL.tolua_getfloat4(L, mgrFromLuaState.unpackColor, stackPos, ref x, ref y, ref z, ref w);
            return new Color(x, y, z, w);
        }

        private static object GetEnumObj(Enum e)
        {
            object obj2 = null;
            if (!enumMap.TryGetValue(e, out obj2))
            {
                obj2 = e;
                enumMap.Add(e, obj2);
            }
            return obj2;
        }

        private static string GetErrorFunc(int skip)
        {
            StackFrame frame = null;
            string fileName = string.Empty;
            StackTrace trace = new StackTrace(skip, true);
            int num = 0;
            do
            {
                frame = trace.GetFrame(num++);
                fileName = Path.GetFileName(frame.GetFileName());
            }
            while (!fileName.Contains("Wrap."));
            int num2 = fileName.LastIndexOf('\\');
            int num3 = fileName.LastIndexOf("Wrap.");
            string str2 = fileName.Substring(num2 + 1, (num3 - num2) - 1);
            return string.Format("{0}.{1}", str2, frame.GetMethod().Name);
        }

        public static LuaFunction GetFunction(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) != LuaTypes.LUA_TFUNCTION)
            {
                return null;
            }
            LuaDLL.lua_pushvalue(L, stackPos);
            return new LuaFunction(LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX), GetTranslator(L).interpreter);
        }

        public int GetFunctionRef(string name)
        {
            IntPtr l = this.lua.L;
            int newTop = LuaDLL.lua_gettop(l);
            int num2 = -1;
            if (PushLuaFunction(l, name))
            {
                num2 = LuaDLL.luaL_ref(l, LuaIndexes.LUA_REGISTRYINDEX);
            }
            else
            {
                object[] args = new object[] { name };
                Debugger.LogWarning("Lua function {0} not exists", args);
            }
            LuaDLL.lua_settop(l, newTop);
            return num2;
        }

        public IntPtr GetL()
        {
            return this.lua.L;
        }

        public LuaFunction GetLuaFunction(string name)
        {
            try
            {
                LuaBase base2 = null;
                if (!this.dict.TryGetValue(name, out base2))
                {
                    IntPtr l = this.lua.L;
                    int newTop = LuaDLL.lua_gettop(l);
                    if (PushLuaFunction(l, name))
                    {
                        base2 = new LuaFunction(LuaDLL.luaL_ref(l, LuaIndexes.LUA_REGISTRYINDEX), this.lua) {
                            name = name
                        };
                        this.dict.Add(name, base2);
                    }
                    else
                    {
                        object[] args = new object[] { name };
                        Debugger.LogError("Lua function {0} not exists", args);
                    }
                    LuaDLL.lua_settop(l, newTop);
                }
                else
                {
                    base2.AddRef();
                }
                return (base2 as LuaFunction);
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
                return null;
            }
        }

        public static LuaFunction GetLuaFunction(IntPtr L, int stackPos)
        {
            LuaFunction function = GetFunction(L, stackPos);
            if (function == null)
            {
                LuaDLL.luaL_typerror(L, stackPos, "function");
                return null;
            }
            return function;
        }

        public static object GetLuaObject(IntPtr L, int stackPos)
        {
            return GetTranslator(L).getRawNetObject(L, stackPos);
        }

        private int GetLuaReference(string str)
        {
            try
            {
                LuaFunction luaFunction = this.GetLuaFunction(str);
                if (luaFunction != null)
                {
                    return luaFunction.GetReference();
                }
                return -1;
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
                return -1;
            }
        }

        public static string GetLuaString(IntPtr L, int stackPos)
        {
            LuaTypes types = LuaDLL.lua_type(L, stackPos);
            string str = null;
            switch (types)
            {
                case LuaTypes.LUA_TSTRING:
                    return LuaDLL.lua_tostring(L, stackPos);

                case LuaTypes.LUA_TUSERDATA:
                {
                    object luaObject = GetLuaObject(L, stackPos);
                    if (luaObject == null)
                    {
                        LuaDLL.luaL_argerror(L, stackPos, "string expected, got nil");
                        return string.Empty;
                    }
                    if (luaObject.GetType() == typeof(string))
                    {
                        return (string) luaObject;
                    }
                    return luaObject.ToString();
                }
                case LuaTypes.LUA_TNUMBER:
                    return LuaDLL.lua_tonumber(L, stackPos).ToString();

                case LuaTypes.LUA_TBOOLEAN:
                    return LuaDLL.lua_toboolean(L, stackPos).ToString();

                case LuaTypes.LUA_TNIL:
                    return str;
            }
            LuaDLL.lua_getglobal(L, "tostring");
            LuaDLL.lua_pushvalue(L, stackPos);
            LuaDLL.lua_call(L, 1, 1);
            str = LuaDLL.lua_tostring(L, -1);
            LuaDLL.lua_pop(L, 1);
            return str;
        }

        public LuaTable GetLuaTable(string tableName)
        {
            LuaBase base2 = null;
            if (!this.dict.TryGetValue(tableName, out base2))
            {
                IntPtr l = this.lua.L;
                int newTop = LuaDLL.lua_gettop(l);
                if (PushLuaTable(l, tableName))
                {
                    base2 = new LuaTable(LuaDLL.luaL_ref(l, LuaIndexes.LUA_REGISTRYINDEX), this.lua) {
                        name = tableName
                    };
                    this.dict.Add(tableName, base2);
                }
                LuaDLL.lua_settop(l, newTop);
            }
            else
            {
                base2.AddRef();
            }
            return (base2 as LuaTable);
        }

        public static LuaTable GetLuaTable(IntPtr L, int stackPos)
        {
            LuaTable table = GetTable(L, stackPos);
            if (table == null)
            {
                LuaDLL.luaL_typerror(L, stackPos, "table");
                return null;
            }
            return table;
        }

        public static LuaScriptMgr GetMgrFromLuaState(IntPtr L)
        {
            return Instance;
        }

        public static T GetNetObject<T>(IntPtr L, int stackPos)
        {
            return (T) GetNetObject(L, stackPos, typeof(T));
        }

        public static object GetNetObject(IntPtr L, int stackPos, System.Type type)
        {
            if (!LuaDLL.lua_isnil(L, stackPos))
            {
                object luaObject = GetLuaObject(L, stackPos);
                if (luaObject == null)
                {
                    LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type.Name));
                    return null;
                }
                System.Type c = luaObject.GetType();
                if ((type == c) || type.IsAssignableFrom(c))
                {
                    return luaObject;
                }
                LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got {1}", type.Name, c.Name));
            }
            return null;
        }

        public static object GetNetObjectSelf(IntPtr L, int stackPos, string type)
        {
            object obj2 = GetTranslator(L).getRawNetObject(L, stackPos);
            if (obj2 == null)
            {
                LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type));
                return null;
            }
            return obj2;
        }

        public static double GetNumber(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_isnumber(L, stackPos))
            {
                return LuaDLL.lua_tonumber(L, stackPos);
            }
            LuaDLL.luaL_typerror(L, stackPos, "number");
            return 0.0;
        }

        public static object[] GetParamsObject(IntPtr L, int stackPos, int count)
        {
            List<object> list = new List<object>();
            object item = null;
            while (count > 0)
            {
                item = GetVarObject(L, stackPos);
                stackPos++;
                count--;
                if (item != null)
                {
                    list.Add(item);
                }
                else
                {
                    LuaDLL.luaL_argerror(L, stackPos, "object expected, got nil");
                    break;
                }
            }
            return list.ToArray();
        }

        public static T[] GetParamsObject<T>(IntPtr L, int stackPos, int count)
        {
            List<T> list = new List<T>();
            T item = default(T);
            while (count > 0)
            {
                object luaObject = GetLuaObject(L, stackPos);
                stackPos++;
                count--;
                if ((luaObject != null) && (luaObject.GetType() == typeof(T)))
                {
                    item = (T) luaObject;
                    list.Add(item);
                }
                else
                {
                    LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", typeof(T).Name));
                    break;
                }
            }
            return list.ToArray();
        }

        public static string[] GetParamsString(IntPtr L, int stackPos, int count)
        {
            List<string> list = new List<string>();
            string item = null;
            while (count > 0)
            {
                item = GetLuaString(L, stackPos);
                stackPos++;
                count--;
                if (item == null)
                {
                    LuaDLL.luaL_argerror(L, stackPos, "string expected, got nil");
                    break;
                }
                list.Add(item);
            }
            return list.ToArray();
        }

        public static Quaternion GetQuaternion(IntPtr L, int stackPos)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            float x = 0f;
            float y = 0f;
            float z = 0f;
            float w = 1f;
            LuaDLL.tolua_getfloat4(L, mgrFromLuaState.unpackQuat, stackPos, ref x, ref y, ref z, ref w);
            return new Quaternion(x, y, z, w);
        }

        public static Ray GetRay(IntPtr L, int stackPos)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            float x = 0f;
            float y = 0f;
            float z = 0f;
            float num4 = 0f;
            float num5 = 0f;
            float num6 = 0f;
            LuaDLL.tolua_getfloat6(L, mgrFromLuaState.unpackRay, stackPos, ref x, ref y, ref z, ref num4, ref num5, ref num6);
            Vector3 origin = new Vector3(x, y, z);
            return new Ray(origin, new Vector3(num4, num5, num6));
        }

        public static string GetString(IntPtr L, int stackPos)
        {
            string luaString = GetLuaString(L, stackPos);
            if (luaString == null)
            {
                LuaDLL.luaL_typerror(L, stackPos, "string");
            }
            return luaString;
        }

        public static LuaStringBuffer GetStringBuffer(IntPtr L, int stackPos)
        {
            LuaTypes types = LuaDLL.lua_type(L, stackPos);
            if (types == LuaTypes.LUA_TNIL)
            {
                return null;
            }
            if (types != LuaTypes.LUA_TSTRING)
            {
                LuaDLL.luaL_typerror(L, stackPos, "string");
                return null;
            }
            int len = 0;
            return new LuaStringBuffer(LuaDLL.lua_tolstring(L, stackPos, out len), len);
        }

        private static LuaTable GetTable(IntPtr L, int stackPos)
        {
            if (LuaDLL.lua_type(L, stackPos) != LuaTypes.LUA_TTABLE)
            {
                return null;
            }
            LuaDLL.lua_pushvalue(L, stackPos);
            return new LuaTable(LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX), GetTranslator(L).interpreter);
        }

        public static T GetTrackedObject<T>(IntPtr L, int stackPos) where T: TrackedReference
        {
            return (T) GetTrackedObject(L, stackPos, typeof(T));
        }

        public static TrackedReference GetTrackedObject(IntPtr L, int stackPos, System.Type type)
        {
            if (!LuaDLL.lua_isnil(L, stackPos))
            {
                object luaObject = GetLuaObject(L, stackPos);
                if (luaObject == null)
                {
                    LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type.Name));
                    return null;
                }
                TrackedReference reference = luaObject as TrackedReference;
                if (reference == null)
                {
                    LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type.Name));
                    return null;
                }
                System.Type type2 = luaObject.GetType();
                if ((type == type2) || type2.IsSubclassOf(type))
                {
                    return reference;
                }
                LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got {1}", type.Name, type2.Name));
            }
            return null;
        }

        public static object GetTrackedObjectSelf(IntPtr L, int stackPos, string type)
        {
            object obj2 = GetTranslator(L).getRawNetObject(L, stackPos);
            TrackedReference reference = (TrackedReference) obj2;
            if (reference == null)
            {
                LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type));
                return null;
            }
            return obj2;
        }

        private static ObjectTranslator GetTranslator(IntPtr L)
        {
            if (_translator == null)
            {
                return ObjectTranslator.FromState(L);
            }
            return _translator;
        }

        private int GetTypeMetaRef(System.Type t)
        {
            string assemblyQualifiedName = t.AssemblyQualifiedName;
            LuaDLL.luaL_getmetatable(this.lua.L, assemblyQualifiedName);
            return LuaDLL.luaL_ref(this.lua.L, LuaIndexes.LUA_REGISTRYINDEX);
        }

        public static System.Type GetTypeObject(IntPtr L, int stackPos)
        {
            object luaObject = GetLuaObject(L, stackPos);
            if ((luaObject == null) || (luaObject.GetType() != monoType))
            {
                LuaDLL.luaL_argerror(L, stackPos, string.Format("Type expected, got {0}", (luaObject != null) ? luaObject.GetType().Name : "nil"));
            }
            return (System.Type) luaObject;
        }

        public static T GetUnityObject<T>(IntPtr L, int stackPos) where T: UnityEngine.Object
        {
            return (T) GetUnityObject(L, stackPos, typeof(T));
        }

        public static UnityEngine.Object GetUnityObject(IntPtr L, int stackPos, System.Type type)
        {
            if (!LuaDLL.lua_isnil(L, stackPos))
            {
                object luaObject = GetLuaObject(L, stackPos);
                if (luaObject == null)
                {
                    LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type.Name));
                    return null;
                }
                UnityEngine.Object obj3 = (UnityEngine.Object) luaObject;
                if (obj3 == null)
                {
                    LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type.Name));
                    return null;
                }
                System.Type type2 = obj3.GetType();
                if ((type == type2) || type2.IsSubclassOf(type))
                {
                    return obj3;
                }
                LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got {1}", type.Name, type2.Name));
            }
            return null;
        }

        public static object GetUnityObjectSelf(IntPtr L, int stackPos, string type)
        {
            object obj2 = GetTranslator(L).getRawNetObject(L, stackPos);
            UnityEngine.Object obj3 = (UnityEngine.Object) obj2;
            if (obj3 == null)
            {
                LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type));
                return null;
            }
            return obj2;
        }

        public static object GetVarObject(IntPtr L, int stackPos)
        {
            switch (LuaDLL.lua_type(L, stackPos))
            {
                case LuaTypes.LUA_TBOOLEAN:
                    return LuaDLL.lua_toboolean(L, stackPos);

                case LuaTypes.LUA_TNUMBER:
                    return LuaDLL.lua_tonumber(L, stackPos);

                case LuaTypes.LUA_TSTRING:
                    return LuaDLL.lua_tostring(L, stackPos);

                case LuaTypes.LUA_TTABLE:
                    return GetVarTable(L, stackPos);

                case LuaTypes.LUA_TFUNCTION:
                    LuaDLL.lua_pushvalue(L, stackPos);
                    return new LuaFunction(LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX), GetTranslator(L).interpreter);

                case LuaTypes.LUA_TUSERDATA:
                {
                    int key = LuaDLL.luanet_rawnetobj(L, stackPos);
                    if (key == -1)
                    {
                        return null;
                    }
                    object obj2 = null;
                    GetTranslator(L).objects.TryGetValue(key, out obj2);
                    return obj2;
                }
            }
            return null;
        }

        public static object GetVarTable(IntPtr L, int stackPos)
        {
            int newTop = LuaDLL.lua_gettop(L);
            LuaDLL.lua_pushvalue(L, stackPos);
            LuaDLL.lua_pushstring(L, "class");
            LuaDLL.lua_gettable(L, -2);
            if (LuaDLL.lua_isnil(L, -1))
            {
                LuaDLL.lua_settop(L, newTop);
                LuaDLL.lua_pushvalue(L, stackPos);
                return new LuaTable(LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX), GetTranslator(L).interpreter);
            }
            string str = LuaDLL.lua_tostring(L, -1);
            LuaDLL.lua_settop(L, newTop);
            stackPos = (stackPos <= 0) ? ((stackPos + newTop) + 1) : stackPos;
            switch (str)
            {
                case "Vector3":
                    return GetVector3(L, stackPos);

                case "Vector2":
                    return GetVector2(L, stackPos);

                case "Quaternion":
                    return GetQuaternion(L, stackPos);

                case "Color":
                    return GetColor(L, stackPos);

                case "Vector4":
                    return GetVector4(L, stackPos);

                case "Ray":
                    return GetRay(L, stackPos);

                case "Bounds":
                    return GetBounds(L, stackPos);
            }
            LuaDLL.lua_pushvalue(L, stackPos);
            return new LuaTable(LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX), GetTranslator(L).interpreter);
        }

        public static Vector2 GetVector2(IntPtr L, int stackPos)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            float x = 0f;
            float y = 0f;
            LuaDLL.tolua_getfloat2(L, mgrFromLuaState.unpackVec2, stackPos, ref x, ref y);
            return new Vector2(x, y);
        }

        public static Vector3 GetVector3(IntPtr L, int stackPos)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            float x = 0f;
            float y = 0f;
            float z = 0f;
            LuaDLL.tolua_getfloat3(L, mgrFromLuaState.unpackVec3, stackPos, ref x, ref y, ref z);
            return new Vector3(x, y, z);
        }

        public static Vector4 GetVector4(IntPtr L, int stackPos)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            float x = 0f;
            float y = 0f;
            float z = 0f;
            float w = 0f;
            LuaDLL.tolua_getfloat4(L, mgrFromLuaState.unpackVec4, stackPos, ref x, ref y, ref z, ref w);
            return new Vector4(x, y, z, w);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int IndexArray(IntPtr L)
        {
            Array luaObject = GetLuaObject(L, 1) as Array;
            if (luaObject == null)
            {
                LuaDLL.luaL_error(L, "trying to index an invalid Array reference");
                LuaDLL.lua_pushnil(L);
                return 1;
            }
            LuaTypes types = LuaDLL.lua_type(L, 2);
            if (types == LuaTypes.LUA_TNUMBER)
            {
                int index = (int) LuaDLL.lua_tonumber(L, 2);
                if (index >= luaObject.Length)
                {
                    LuaDLL.luaL_error(L, string.Concat(new object[] { "array index out of bounds: ", index, " ", luaObject.Length }));
                    return 0;
                }
                object o = luaObject.GetValue(index);
                if (o == null)
                {
                    LuaDLL.luaL_error(L, string.Format("array index {0} is null", index));
                    return 0;
                }
                PushVarObject(L, o);
            }
            else if ((types == LuaTypes.LUA_TSTRING) && (GetLuaString(L, 2) == "Length"))
            {
                Push(L, luaObject.Length);
            }
            return 1;
        }

        private void InitLayers(IntPtr L)
        {
            this.GetLuaTable("Layer").push(L);
            for (int i = 0; i < 0x20; i++)
            {
                string str = LayerMask.LayerToName(i);
                if (str != string.Empty)
                {
                    LuaDLL.lua_pushstring(L, str);
                    Push(L, i);
                    LuaDLL.lua_rawset(L, -3);
                }
            }
            LuaDLL.lua_settop(L, 0);
        }

        public static bool IsClassOf(System.Type child, System.Type parent)
        {
            return ((child == parent) || parent.IsAssignableFrom(child));
        }

        public bool IsFuncExists(string name)
        {
            IntPtr l = this.lua.L;
            int newTop = LuaDLL.lua_gettop(l);
            if (PushLuaFunction(l, name))
            {
                LuaDLL.lua_settop(l, newTop);
                return true;
            }
            return false;
        }

        public void LateUpate()
        {
            try
            {
                if (this.lateUpdateFunc != null)
                {
                    this.lateUpdateFunc.Call();
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        public byte[] Loader(string name)
        {
            byte[] bytes = null;
            name = name.Replace("/", ".");
            char[] separator = new char[] { '.' };
            if (name.Split(separator).Length > 1)
            {
                char[] chArray2 = new char[] { '.' };
                char[] chArray3 = new char[] { '.' };
                name = name.Split(chArray2)[name.Split(chArray3).Length - 1];
            }
            try
            {
                bytes = LuaHelper.GetResManager().luaFiles[name].bytes;
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d("[Error] : LuaScriptMgr error," + exception.Message + ", key = " + name);
            }
            return bytes;
        }

        public void LuaGC(params string[] param)
        {
            try
            {
                LuaDLL.lua_gc(this.lua.L, LuaGCOptions.LUA_GCCOLLECT, 0);
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        private void LuaMem(params string[] param)
        {
            try
            {
                this.CallLuaFunction("mem_report", new object[0]);
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int NewIndexArray(IntPtr L)
        {
            Array luaObject = GetLuaObject(L, 1) as Array;
            if (luaObject == null)
            {
                LuaDLL.luaL_error(L, "trying to index and invalid object reference");
                return 0;
            }
            int number = (int) GetNumber(L, 2);
            object varObject = GetVarObject(L, 3);
            System.Type elementType = luaObject.GetType().GetElementType();
            if (!CheckType(L, elementType, 3))
            {
                LuaDLL.luaL_error(L, "trying to set object type is not correct");
                return 0;
            }
            varObject = Convert.ChangeType(varObject, elementType);
            luaObject.SetValue(varObject, number);
            return 0;
        }

        public void OnBundleLoaded()
        {
            try
            {
                this.DoString("Global");
                this.InitLayers(this.lua.L);
                this.unpackVec3 = this.GetLuaReference("Vector3.Get");
                this.unpackVec2 = this.GetLuaReference("Vector2.Get");
                this.unpackVec4 = this.GetLuaReference("Vector4.Get");
                this.unpackQuat = this.GetLuaReference("Quaternion.Get");
                this.unpackColor = this.GetLuaReference("Color.Get");
                this.unpackRay = this.GetLuaReference("Ray.Get");
                this.unpackBounds = this.GetLuaReference("Bounds.Get");
                this.packVec3 = this.GetLuaReference("Vector3.New");
                this.packVec2 = this.GetLuaReference("Vector2.New");
                this.packVec4 = this.GetLuaReference("Vector4.New");
                this.packQuat = this.GetLuaReference("Quaternion.New");
                this.packRaycastHit = this.GetLuaFunction("Raycast.New");
                this.packColor = this.GetLuaReference("Color.New");
                this.packRay = this.GetLuaReference("Ray.New");
                this.packTouch = this.GetLuaFunction("Touch.New");
                this.packBounds = this.GetLuaReference("Bounds.New");
                traceback = this.GetLuaFunction("traceback");
                this.DoString("Main");
                this.updateFunc = this.GetLuaFunction("Update");
                this.lateUpdateFunc = this.GetLuaFunction("LateUpdate");
                this.fixedUpdateFunc = this.GetLuaFunction("FixedUpdate");
                this.levelLoaded = this.GetLuaFunction("OnLevelWasLoaded");
                this.CallLuaFunction("Main", new object[0]);
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d("FERR:" + exception.ToString());
            }
        }

        public void OnLevelLoaded(int level)
        {
            this.levelLoaded.Call((double) level);
        }

        public static void Push(IntPtr L, ILuaGeneratedType o)
        {
            if (o == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                o.__luaInterface_getLuaTable().push(L);
            }
        }

        public static void Push(IntPtr L, LuaCSFunction func)
        {
            if (func == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                GetTranslator(L).pushFunction(L, func);
            }
        }

        public static void Push(IntPtr L, LuaFunction func)
        {
            if (func == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                func.push();
            }
        }

        public static void Push(IntPtr L, LuaStringBuffer lsb)
        {
            if ((lsb != null) && (lsb.buffer != null))
            {
                LuaDLL.lua_pushlstring(L, lsb.buffer, lsb.buffer.Length);
            }
            else
            {
                LuaDLL.lua_pushnil(L);
            }
        }

        public static void Push(IntPtr L, LuaTable table)
        {
            if (table == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                table.push(L);
            }
        }

        public static void Push(IntPtr L, bool b)
        {
            LuaDLL.lua_pushboolean(L, b);
        }

        public static void Push(IntPtr L, byte d)
        {
            LuaDLL.lua_pushinteger(L, d);
        }

        public static void Push(IntPtr L, char d)
        {
            LuaDLL.lua_pushinteger(L, d);
        }

        public static void Push(IntPtr L, IEnumerator o)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            ObjectTranslator translator = mgrFromLuaState.lua.translator;
            PushMetaObject(L, translator, o, mgrFromLuaState.iterMetaRef);
        }

        public static void Push(IntPtr L, decimal d)
        {
            LuaDLL.lua_pushnumber(L, (double) d);
        }

        public static void Push(IntPtr L, Delegate o)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            ObjectTranslator translator = mgrFromLuaState.lua.translator;
            PushMetaObject(L, translator, o, mgrFromLuaState.delegateMetaRef);
        }

        public static void Push(IntPtr L, double d)
        {
            LuaDLL.lua_pushnumber(L, d);
        }

        public static void Push(IntPtr L, Enum e)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            ObjectTranslator translator = mgrFromLuaState.lua.translator;
            int weakTableRef = translator.weakTableRef;
            object enumObj = GetEnumObj(e);
            int num2 = -1;
            if (translator.objectsBackMap.TryGetValue(enumObj, out num2))
            {
                if (LuaDLL.tolua_pushudata(L, weakTableRef, num2))
                {
                    return;
                }
                translator.collectObject(num2);
            }
            num2 = translator.addObject(enumObj, false);
            LuaDLL.tolua_pushnewudata(L, mgrFromLuaState.enumMetaRef, weakTableRef, num2);
        }

        public static void Push(IntPtr L, short d)
        {
            LuaDLL.lua_pushinteger(L, d);
        }

        public static void Push(IntPtr L, int d)
        {
            LuaDLL.lua_pushinteger(L, d);
        }

        public static void Push(IntPtr L, long d)
        {
            LuaDLL.lua_pushnumber(L, (double) d);
        }

        public static void Push(IntPtr L, IntPtr p)
        {
            LuaDLL.lua_pushlightuserdata(L, p);
        }

        public static void Push(IntPtr L, sbyte d)
        {
            LuaDLL.lua_pushinteger(L, (int) d);
        }

        public static void Push(IntPtr L, float d)
        {
            LuaDLL.lua_pushnumber(L, (double) d);
        }

        public static void Push(IntPtr L, string str)
        {
            LuaDLL.lua_pushstring(L, str);
        }

        public static void Push(IntPtr L, System.Type o)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            ObjectTranslator translator = mgrFromLuaState.lua.translator;
            PushMetaObject(L, translator, o, mgrFromLuaState.typeMetaRef);
        }

        public static void Push(IntPtr L, ushort d)
        {
            LuaDLL.lua_pushinteger(L, d);
        }

        public static void Push(IntPtr L, uint d)
        {
            LuaDLL.lua_pushnumber(L, (double) d);
        }

        public static void Push(IntPtr L, ulong d)
        {
            LuaDLL.lua_pushnumber(L, (double) d);
        }

        public static void Push(IntPtr L, Bounds bound)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            LuaDLL.lua_getref(L, mgrFromLuaState.packBounds);
            Push(L, bound.center);
            Push(L, bound.size);
            LuaDLL.lua_call(L, 2, -1);
        }

        public static void Push(IntPtr L, Color clr)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            LuaDLL.tolua_pushfloat4(L, mgrFromLuaState.packColor, clr.r, clr.g, clr.b, clr.a);
        }

        public static void Push(IntPtr L, UnityEngine.Object obj)
        {
            PushObject(L, (obj != null) ? obj : null);
        }

        public static void Push(IntPtr L, Quaternion q)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            LuaDLL.tolua_pushfloat4(L, mgrFromLuaState.packQuat, q.x, q.y, q.z, q.w);
        }

        public static void Push(IntPtr L, Ray ray)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            LuaDLL.lua_getref(L, mgrFromLuaState.packRay);
            LuaDLL.tolua_pushfloat3(L, mgrFromLuaState.packVec3, ray.direction.x, ray.direction.y, ray.direction.z);
            LuaDLL.tolua_pushfloat3(L, mgrFromLuaState.packVec3, ray.origin.x, ray.origin.y, ray.origin.z);
            LuaDLL.lua_call(L, 2, -1);
        }

        public static void Push(IntPtr L, RaycastHit hit)
        {
            GetMgrFromLuaState(L).packRaycastHit.push(L);
            Push(L, hit.collider);
            Push(L, hit.distance);
            Push(L, hit.normal);
            Push(L, hit.point);
            Push(L, hit.rigidbody);
            Push(L, hit.transform);
            LuaDLL.lua_call(L, 6, -1);
        }

        public static void Push(IntPtr L, Touch touch)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            mgrFromLuaState.packTouch.push(L);
            LuaDLL.lua_pushinteger(L, touch.fingerId);
            LuaDLL.tolua_pushfloat2(L, mgrFromLuaState.packVec2, touch.position.x, touch.position.y);
            LuaDLL.tolua_pushfloat2(L, mgrFromLuaState.packVec2, touch.rawPosition.x, touch.rawPosition.y);
            LuaDLL.tolua_pushfloat2(L, mgrFromLuaState.packVec2, touch.deltaPosition.x, touch.deltaPosition.y);
            LuaDLL.lua_pushnumber(L, (double) touch.deltaTime);
            LuaDLL.lua_pushinteger(L, touch.tapCount);
            LuaDLL.lua_pushinteger(L, (int) touch.phase);
            LuaDLL.lua_call(L, 7, -1);
        }

        public static void Push(IntPtr L, TrackedReference obj)
        {
            PushObject(L, (obj != null) ? obj : null);
        }

        public static void Push(IntPtr L, Vector2 v2)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            LuaDLL.tolua_pushfloat2(L, mgrFromLuaState.packVec2, v2.x, v2.y);
        }

        public static void Push(IntPtr L, Vector3 v3)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            LuaDLL.tolua_pushfloat3(L, mgrFromLuaState.packVec3, v3.x, v3.y, v3.z);
        }

        public static void Push(IntPtr L, Vector4 v4)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            LuaDLL.tolua_pushfloat4(L, mgrFromLuaState.packVec4, v4.x, v4.y, v4.z, v4.w);
        }

        public static void PushArray(IntPtr L, Array o)
        {
            LuaScriptMgr mgrFromLuaState = GetMgrFromLuaState(L);
            ObjectTranslator translator = mgrFromLuaState.lua.translator;
            PushMetaObject(L, translator, o, mgrFromLuaState.arrayMetaRef);
        }

        private static bool PushLuaFunction(IntPtr L, string fullPath)
        {
            int newTop = LuaDLL.lua_gettop(L);
            int length = fullPath.LastIndexOf('.');
            if (length > 0)
            {
                string str = fullPath.Substring(0, length);
                if (PushLuaTable(L, str))
                {
                    string str2 = fullPath.Substring(length + 1);
                    LuaDLL.lua_pushstring(L, str2);
                    LuaDLL.lua_rawget(L, -2);
                }
                if (LuaDLL.lua_type(L, -1) != LuaTypes.LUA_TFUNCTION)
                {
                    LuaDLL.lua_settop(L, newTop);
                    return false;
                }
                LuaDLL.lua_insert(L, newTop + 1);
                LuaDLL.lua_settop(L, newTop + 1);
            }
            else
            {
                LuaDLL.lua_getglobal(L, fullPath);
                if (LuaDLL.lua_type(L, -1) != LuaTypes.LUA_TFUNCTION)
                {
                    LuaDLL.lua_settop(L, newTop);
                    return false;
                }
            }
            return true;
        }

        private static bool PushLuaTable(IntPtr L, string fullPath)
        {
            char[] separator = new char[] { '.' };
            string[] strArray = fullPath.Split(separator);
            int newTop = LuaDLL.lua_gettop(L);
            LuaDLL.lua_pushstring(L, strArray[0]);
            LuaDLL.lua_rawget(L, LuaIndexes.LUA_GLOBALSINDEX);
            if (LuaDLL.lua_type(L, -1) != LuaTypes.LUA_TTABLE)
            {
                LuaDLL.lua_settop(L, newTop);
                LuaDLL.lua_pushnil(L);
                object[] args = new object[] { strArray[0] };
                Debugger.LogError("Push lua table {0} failed", args);
                return false;
            }
            for (int i = 1; i < strArray.Length; i++)
            {
                LuaDLL.lua_pushstring(L, strArray[i]);
                LuaDLL.lua_rawget(L, -2);
                if (LuaDLL.lua_type(L, -1) != LuaTypes.LUA_TTABLE)
                {
                    LuaDLL.lua_settop(L, newTop);
                    object[] objArray2 = new object[] { fullPath };
                    Debugger.LogError("Push lua table {0} failed", objArray2);
                    return false;
                }
            }
            if (strArray.Length > 1)
            {
                LuaDLL.lua_insert(L, newTop + 1);
                LuaDLL.lua_settop(L, newTop + 1);
            }
            return true;
        }

        private static void PushMetaObject(IntPtr L, ObjectTranslator translator, object o, int metaRef)
        {
            if (o == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                int weakTableRef = translator.weakTableRef;
                int num2 = -1;
                if (translator.objectsBackMap.TryGetValue(o, out num2))
                {
                    if (LuaDLL.tolua_pushudata(L, weakTableRef, num2))
                    {
                        return;
                    }
                    translator.collectObject(num2);
                }
                num2 = translator.addObject(o, false);
                LuaDLL.tolua_pushnewudata(L, metaRef, weakTableRef, num2);
            }
        }

        public static void PushObject(IntPtr L, object o)
        {
            GetTranslator(L).pushObject(L, o, "luaNet_metatable");
        }

        public static void PushTraceBack(IntPtr L)
        {
            if (traceback == null)
            {
                LuaDLL.lua_getglobal(L, "traceback");
            }
            else
            {
                traceback.push();
            }
        }

        public static void PushValue(IntPtr L, object obj)
        {
            GetTranslator(L).PushValueResult(L, obj);
        }

        public static void PushVarObject(IntPtr L, object o)
        {
            if (o == null)
            {
                LuaDLL.lua_pushnil(L);
            }
            else
            {
                System.Type child = o.GetType();
                if (child.IsValueType)
                {
                    if (child == typeof(bool))
                    {
                        bool flag = (bool) o;
                        LuaDLL.lua_pushboolean(L, flag);
                    }
                    else if (child.IsEnum)
                    {
                        Push(L, (Enum) o);
                    }
                    else if (child.IsPrimitive)
                    {
                        double number = Convert.ToDouble(o);
                        LuaDLL.lua_pushnumber(L, number);
                    }
                    else if (child == typeof(Vector3))
                    {
                        Push(L, (Vector3) o);
                    }
                    else if (child == typeof(Vector2))
                    {
                        Push(L, (Vector2) o);
                    }
                    else if (child == typeof(Vector4))
                    {
                        Push(L, (Vector4) o);
                    }
                    else if (child == typeof(Quaternion))
                    {
                        Push(L, (Quaternion) o);
                    }
                    else if (child == typeof(Color))
                    {
                        Push(L, (Color) o);
                    }
                    else if (child == typeof(RaycastHit))
                    {
                        Push(L, (RaycastHit) o);
                    }
                    else if (child == typeof(Touch))
                    {
                        Push(L, (Touch) o);
                    }
                    else if (child == typeof(Ray))
                    {
                        Push(L, (Ray) o);
                    }
                    else
                    {
                        PushValue(L, o);
                    }
                }
                else if (child.IsArray)
                {
                    PushArray(L, (Array) o);
                }
                else if (child == typeof(LuaCSFunction))
                {
                    GetTranslator(L).pushFunction(L, (LuaCSFunction) o);
                }
                else if (child.IsSubclassOf(typeof(Delegate)))
                {
                    Push(L, (Delegate) o);
                }
                else if (IsClassOf(child, typeof(IEnumerator)))
                {
                    Push(L, (IEnumerator) o);
                }
                else if (child == typeof(string))
                {
                    string str = (string) o;
                    LuaDLL.lua_pushstring(L, str);
                }
                else if (child == typeof(LuaStringBuffer))
                {
                    LuaStringBuffer buffer = (LuaStringBuffer) o;
                    LuaDLL.lua_pushlstring(L, buffer.buffer, buffer.buffer.Length);
                }
                else if (child.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    UnityEngine.Object obj2 = (UnityEngine.Object) o;
                    if (obj2 == null)
                    {
                        LuaDLL.lua_pushnil(L);
                    }
                    else
                    {
                        PushObject(L, o);
                    }
                }
                else if (child == typeof(LuaTable))
                {
                    ((LuaTable) o).push(L);
                }
                else if (child == typeof(LuaFunction))
                {
                    ((LuaFunction) o).push(L);
                }
                else if (child == monoType)
                {
                    Push(L, (System.Type) o);
                }
                else if (child.IsSubclassOf(typeof(TrackedReference)))
                {
                    TrackedReference reference = (TrackedReference) o;
                    if (reference == null)
                    {
                        LuaDLL.lua_pushnil(L);
                    }
                    else
                    {
                        PushObject(L, o);
                    }
                }
                else
                {
                    PushObject(L, o);
                }
            }
        }

        public static void RegisterLib(IntPtr L, string libName, LuaMethod[] regs)
        {
            CreateTable(L, libName);
            for (int i = 0; i < regs.Length; i++)
            {
                LuaDLL.lua_pushstring(L, regs[i].name);
                LuaDLL.lua_pushstdcallcfunction(L, regs[i].func, 0);
                LuaDLL.lua_rawset(L, -3);
            }
            LuaDLL.lua_settop(L, 0);
        }

        public static void RegisterLib(IntPtr L, string libName, System.Type t, LuaMethod[] regs)
        {
            CreateTable(L, libName);
            LuaDLL.luaL_getmetatable(L, t.AssemblyQualifiedName);
            if (LuaDLL.lua_isnil(L, -1))
            {
                LuaDLL.lua_pop(L, 1);
                LuaDLL.luaL_newmetatable(L, t.AssemblyQualifiedName);
            }
            LuaDLL.lua_pushstring(L, "ToLua_EnumIndex");
            LuaDLL.lua_rawget(L, LuaIndexes.LUA_REGISTRYINDEX);
            LuaDLL.lua_setfield(L, -2, "__index");
            LuaDLL.lua_pushstring(L, "__gc");
            LuaDLL.lua_pushstdcallcfunction(L, new LuaCSFunction(LuaScriptMgr.__gc), 0);
            LuaDLL.lua_rawset(L, -3);
            for (int i = 0; i < (regs.Length - 1); i++)
            {
                LuaDLL.lua_pushstring(L, regs[i].name);
                LuaDLL.lua_pushstdcallcfunction(L, regs[i].func, 0);
                LuaDLL.lua_rawset(L, -3);
            }
            int index = regs.Length - 1;
            LuaDLL.lua_pushstring(L, regs[index].name);
            LuaDLL.lua_pushstdcallcfunction(L, regs[index].func, 0);
            LuaDLL.lua_rawset(L, -4);
            LuaDLL.lua_setmetatable(L, -2);
            LuaDLL.lua_settop(L, 0);
        }

        public static void RegisterLib(IntPtr L, string libName, System.Type t, LuaMethod[] regs, LuaField[] fields, System.Type baseType)
        {
            CreateTable(L, libName);
            LuaDLL.luaL_getmetatable(L, t.AssemblyQualifiedName);
            if (LuaDLL.lua_isnil(L, -1))
            {
                LuaDLL.lua_pop(L, 1);
                LuaDLL.luaL_newmetatable(L, t.AssemblyQualifiedName);
            }
            if (baseType != null)
            {
                LuaDLL.luaL_getmetatable(L, baseType.AssemblyQualifiedName);
                if (LuaDLL.lua_isnil(L, -1))
                {
                    LuaDLL.lua_pop(L, 1);
                    LuaDLL.luaL_newmetatable(L, baseType.AssemblyQualifiedName);
                    checkBaseType.Add(baseType);
                }
                else
                {
                    checkBaseType.Remove(baseType);
                }
                LuaDLL.lua_setmetatable(L, -2);
            }
            LuaDLL.tolua_setindex(L);
            LuaDLL.tolua_setnewindex(L);
            LuaDLL.lua_pushstring(L, "__call");
            LuaDLL.lua_pushstring(L, "ToLua_TableCall");
            LuaDLL.lua_rawget(L, LuaIndexes.LUA_REGISTRYINDEX);
            LuaDLL.lua_rawset(L, -3);
            LuaDLL.lua_pushstring(L, "__gc");
            LuaDLL.lua_pushstdcallcfunction(L, new LuaCSFunction(LuaScriptMgr.__gc), 0);
            LuaDLL.lua_rawset(L, -3);
            for (int i = 0; i < regs.Length; i++)
            {
                LuaDLL.lua_pushstring(L, regs[i].name);
                LuaDLL.lua_pushstdcallcfunction(L, regs[i].func, 0);
                LuaDLL.lua_rawset(L, -3);
            }
            for (int j = 0; j < fields.Length; j++)
            {
                LuaDLL.lua_pushstring(L, fields[j].name);
                LuaDLL.lua_createtable(L, 2, 0);
                if (fields[j].getter != null)
                {
                    LuaDLL.lua_pushstdcallcfunction(L, fields[j].getter, 0);
                    LuaDLL.lua_rawseti(L, -2, 1);
                }
                if (fields[j].setter != null)
                {
                    LuaDLL.lua_pushstdcallcfunction(L, fields[j].setter, 0);
                    LuaDLL.lua_rawseti(L, -2, 2);
                }
                LuaDLL.lua_rawset(L, -3);
            }
            LuaDLL.lua_setmetatable(L, -2);
            LuaDLL.lua_settop(L, 0);
            checkBaseType.Remove(t);
        }

        public void RemoveLuaRes(string name)
        {
            this.dict.Remove(name);
        }

        private void SafeRelease(ref LuaFunction func)
        {
            try
            {
                if (func != null)
                {
                    func.Release();
                    func = null;
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        private void SafeUnRef(ref int reference)
        {
            try
            {
                if (reference > 0)
                {
                    LuaDLL.lua_unref(this.lua.L, reference);
                    reference = -1;
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        private void SendGMmsg(params string[] param)
        {
            string str = string.Empty;
            int num = 0;
            foreach (string str2 in param)
            {
                if (num > 0)
                {
                    str = str + " " + str2;
                }
                num++;
            }
            object[] args = new object[] { str };
            this.CallLuaFunction("GMMsg", args);
        }

        public static void SetValueObject(IntPtr L, int pos, object obj)
        {
            GetTranslator(L).SetValueObject(L, pos, obj);
        }

        public void Start()
        {
            try
            {
                this.OnBundleLoaded();
                this.enumMetaRef = this.GetTypeMetaRef(typeof(Enum));
                this.typeMetaRef = this.GetTypeMetaRef(typeof(System.Type));
                this.delegateMetaRef = this.GetTypeMetaRef(typeof(Delegate));
                this.iterMetaRef = this.GetTypeMetaRef(typeof(IEnumerator));
                foreach (System.Type type in checkBaseType)
                {
                    object[] args = new object[] { type.FullName };
                    Debugger.LogWarning("BaseType {0} not register to lua", args);
                }
                checkBaseType.Clear();
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        public static void ThrowLuaException(IntPtr L)
        {
            string str = LuaDLL.lua_tostring(L, -1);
            if (str == null)
            {
                str = "Unknown Lua Error";
            }
            throw new LuaScriptException(str.ToString(), string.Empty);
        }

        public static LuaFunction ToLuaFunction(IntPtr L, int stackPos)
        {
            LuaDLL.lua_pushvalue(L, stackPos);
            return new LuaFunction(LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX), GetTranslator(L).interpreter);
        }

        private static LuaTable ToLuaTable(IntPtr L, int stackPos)
        {
            LuaDLL.lua_pushvalue(L, stackPos);
            return new LuaTable(LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX), GetTranslator(L).interpreter);
        }

        public void Update()
        {
            try
            {
                if (this.updateFunc != null)
                {
                    int oldTop = this.updateFunc.BeginPCall();
                    IntPtr luaState = this.updateFunc.GetLuaState();
                    Push(luaState, Time.deltaTime);
                    Push(luaState, Time.unscaledDeltaTime);
                    this.updateFunc.PCall(oldTop, 2);
                    this.updateFunc.EndPCall(oldTop);
                }
                while (!refGCList.IsEmpty())
                {
                    LuaRef ref2 = refGCList.Dequeue();
                    LuaDLL.lua_unref(ref2.L, ref2.reference);
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.LogNetError(Configer.iCodeCSException, exception.Message + exception.StackTrace);
            }
        }

        public static LuaScriptMgr Instance
        {
            [CompilerGenerated]
            get
            {
                return <Instance>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <Instance>k__BackingField = value;
            }
        }
    }
}

