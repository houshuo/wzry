namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class ObjectTranslator
    {
        public List<Assembly> assemblies;
        private LuaCSFunction ctypeFunction;
        private LuaCSFunction enumFromIntFunction;
        private LuaCSFunction getConstructorSigFunction;
        private LuaCSFunction getMethodSigFunction;
        private LuaCSFunction importTypeFunction;
        private static int indexTranslator = 0;
        internal LuaState interpreter;
        private static List<ObjectTranslator> list = new List<ObjectTranslator>();
        private LuaCSFunction loadAssemblyFunction;
        public MetaFunctions metaFunctions;
        private int nextObj;
        public readonly Dictionary<int, object> objects = new Dictionary<int, object>();
        public readonly Dictionary<object, int> objectsBackMap = new Dictionary<object, int>(new CompareObject());
        internal EventHandlerContainer pendingEvents = new EventHandlerContainer();
        private LuaCSFunction registerTableFunction;
        internal CheckType typeChecker;
        private static Dictionary<Type, int> typeMetaMap = new Dictionary<Type, int>();
        private LuaCSFunction unregisterTableFunction;
        private static Dictionary<Type, bool> valueTypeMap = new Dictionary<Type, bool>();

        public ObjectTranslator(LuaState interpreter, IntPtr luaState)
        {
            this.interpreter = interpreter;
            this.weakTableRef = -1;
            this.typeChecker = new CheckType(this);
            this.metaFunctions = new MetaFunctions(this);
            this.assemblies = new List<Assembly>();
            this.assemblies.Add(Assembly.GetExecutingAssembly());
            this.importTypeFunction = new LuaCSFunction(ObjectTranslator.importType);
            this.loadAssemblyFunction = new LuaCSFunction(ObjectTranslator.loadAssembly);
            this.registerTableFunction = new LuaCSFunction(ObjectTranslator.registerTable);
            this.unregisterTableFunction = new LuaCSFunction(ObjectTranslator.unregisterTable);
            this.getMethodSigFunction = new LuaCSFunction(ObjectTranslator.getMethodSignature);
            this.getConstructorSigFunction = new LuaCSFunction(ObjectTranslator.getConstructorSignature);
            this.ctypeFunction = new LuaCSFunction(ObjectTranslator.ctype);
            this.enumFromIntFunction = new LuaCSFunction(ObjectTranslator.enumFromInt);
            this.createLuaObjectList(luaState);
            this.createIndexingMetaFunction(luaState);
            this.createBaseClassMetatable(luaState);
            this.createClassMetatable(luaState);
            this.createFunctionMetatable(luaState);
            this.setGlobalFunctions(luaState);
        }

        public int addObject(object obj)
        {
            int num = this.nextObj++;
            this.objects[num] = obj;
            if (!obj.GetType().IsValueType)
            {
                this.objectsBackMap[obj] = num;
            }
            return num;
        }

        public int addObject(object obj, bool isValueType)
        {
            int num = this.nextObj++;
            this.objects[num] = obj;
            if (!isValueType)
            {
                this.objectsBackMap[obj] = num;
            }
            return num;
        }

        internal void collectObject(int udata)
        {
            object obj2;
            if (this.objects.TryGetValue(udata, out obj2))
            {
                this.objects.Remove(udata);
                if ((obj2 != null) && !obj2.GetType().IsValueType)
                {
                    this.objectsBackMap.Remove(obj2);
                }
            }
        }

        private void collectObject(object o, int udata)
        {
            this.objectsBackMap.Remove(o);
            this.objects.Remove(udata);
        }

        private void createBaseClassMetatable(IntPtr luaState)
        {
            LuaDLL.luaL_newmetatable(luaState, "luaNet_searchbase");
            LuaDLL.lua_pushstring(luaState, "__gc");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction, 0);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__tostring");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.toStringFunction, 0);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__index");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.baseIndexFunction, 0);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__newindex");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.newindexFunction, 0);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_settop(luaState, -2);
        }

        private void createClassMetatable(IntPtr luaState)
        {
            LuaDLL.luaL_newmetatable(luaState, "luaNet_class");
            LuaDLL.lua_pushstring(luaState, "__gc");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction, 0);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__tostring");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.toStringFunction, 0);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__index");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.classIndexFunction, 0);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__newindex");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.classNewindexFunction, 0);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__call");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.callConstructorFunction, 0);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_settop(luaState, -2);
        }

        private void createFunctionMetatable(IntPtr luaState)
        {
            LuaDLL.luaL_newmetatable(luaState, "luaNet_function");
            LuaDLL.lua_pushstring(luaState, "__gc");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction, 0);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__call");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.execDelegateFunction, 0);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_settop(luaState, -2);
        }

        private void createIndexingMetaFunction(IntPtr luaState)
        {
            LuaDLL.lua_pushstring(luaState, "luaNet_indexfunction");
            LuaDLL.luaL_dostring(luaState, MetaFunctions.luaIndexFunction);
            LuaDLL.lua_rawset(luaState, LuaIndexes.LUA_REGISTRYINDEX);
        }

        private void createLuaObjectList(IntPtr luaState)
        {
            LuaDLL.lua_pushstring(luaState, "luaNet_objects");
            LuaDLL.lua_newtable(luaState);
            LuaDLL.lua_pushvalue(luaState, -1);
            this.weakTableRef = LuaDLL.luaL_ref(luaState, LuaIndexes.LUA_REGISTRYINDEX);
            LuaDLL.lua_pushvalue(luaState, -1);
            LuaDLL.lua_setmetatable(luaState, -2);
            LuaDLL.lua_pushstring(luaState, "__mode");
            LuaDLL.lua_pushstring(luaState, "v");
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_settable(luaState, LuaIndexes.LUA_REGISTRYINDEX);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int ctype(IntPtr luaState)
        {
            ObjectTranslator translator = FromState(luaState);
            Type o = translator.typeOf(luaState, 1);
            if (o == null)
            {
                return translator.pushError(luaState, "not a CLR class");
            }
            translator.pushObject(luaState, o, "luaNet_metatable");
            return 1;
        }

        public void Destroy()
        {
            IntPtr l = this.interpreter.L;
            foreach (KeyValuePair<Type, int> pair in typeMetaMap)
            {
                int reference = pair.Value;
                LuaDLL.lua_unref(l, reference);
            }
            LuaDLL.lua_unref(l, this.weakTableRef);
            typeMetaMap.Clear();
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int enumFromInt(IntPtr luaState)
        {
            ObjectTranslator translator = FromState(luaState);
            Type enumType = translator.typeOf(luaState, 1);
            if ((enumType == null) || !enumType.IsEnum)
            {
                return translator.pushError(luaState, "not an enum");
            }
            object o = null;
            LuaTypes types = LuaDLL.lua_type(luaState, 2);
            if (types == LuaTypes.LUA_TNUMBER)
            {
                int num = (int) LuaDLL.lua_tonumber(luaState, 2);
                o = Enum.ToObject(enumType, num);
            }
            else
            {
                if (types != LuaTypes.LUA_TSTRING)
                {
                    return translator.pushError(luaState, "second argument must be a integer or a string");
                }
                string str = LuaDLL.lua_tostring(luaState, 2);
                string msg = null;
                try
                {
                    o = Enum.Parse(enumType, str);
                }
                catch (ArgumentException exception)
                {
                    msg = exception.Message;
                }
                if (msg != null)
                {
                    return translator.pushError(luaState, msg);
                }
            }
            translator.pushObject(luaState, o, "luaNet_metatable");
            return 1;
        }

        internal Type FindType(string className)
        {
            foreach (Assembly assembly in this.assemblies)
            {
                Type type = assembly.GetType(className);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

        public static ObjectTranslator FromState(IntPtr luaState)
        {
            LuaDLL.lua_getglobal(luaState, "_translator");
            int num = (int) LuaDLL.lua_tonumber(luaState, -1);
            LuaDLL.lua_pop(luaState, 1);
            return list[num];
        }

        internal object getAsType(IntPtr luaState, int stackPos, Type paramType)
        {
            ExtractValue value2 = this.typeChecker.checkType(luaState, stackPos, paramType);
            if (value2 != null)
            {
                return value2(luaState, stackPos);
            }
            return null;
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int getConstructorSignature(IntPtr luaState)
        {
            ObjectTranslator translator = FromState(luaState);
            IReflect targetType = null;
            int num = LuaDLL.luanet_checkudata(luaState, 1, "luaNet_class");
            if (num != -1)
            {
                targetType = (IReflect) translator.objects[num];
            }
            if (targetType == null)
            {
                translator.throwError(luaState, "get_constructor_bysig: first arg is invalid type reference");
            }
            Type[] types = new Type[LuaDLL.lua_gettop(luaState) - 1];
            for (int i = 0; i < types.Length; i++)
            {
                types[i] = translator.FindType(LuaDLL.lua_tostring(luaState, i + 2));
            }
            try
            {
                ConstructorInfo constructor = targetType.UnderlyingSystemType.GetConstructor(types);
                translator.pushFunction(luaState, new LuaCSFunction(new LuaMethodWrapper(translator, null, targetType, constructor).call));
            }
            catch (Exception exception)
            {
                translator.throwError(luaState, exception.Message);
                LuaDLL.lua_pushnil(luaState);
            }
            return 1;
        }

        internal LuaFunction getFunction(IntPtr luaState, int index)
        {
            LuaDLL.lua_pushvalue(luaState, index);
            return new LuaFunction(LuaDLL.luaL_ref(luaState, LuaIndexes.LUA_REGISTRYINDEX), this.interpreter);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int getMethodSignature(IntPtr luaState)
        {
            IReflect type;
            object obj2;
            ObjectTranslator translator = FromState(luaState);
            int num = LuaDLL.luanet_checkudata(luaState, 1, "luaNet_class");
            if (num != -1)
            {
                type = (IReflect) translator.objects[num];
                obj2 = null;
            }
            else
            {
                obj2 = translator.getRawNetObject(luaState, 1);
                if (obj2 == null)
                {
                    translator.throwError(luaState, "get_method_bysig: first arg is not type or object reference");
                    LuaDLL.lua_pushnil(luaState);
                    return 1;
                }
                type = obj2.GetType();
            }
            string name = LuaDLL.lua_tostring(luaState, 2);
            Type[] types = new Type[LuaDLL.lua_gettop(luaState) - 2];
            for (int i = 0; i < types.Length; i++)
            {
                types[i] = translator.FindType(LuaDLL.lua_tostring(luaState, i + 3));
            }
            try
            {
                MethodInfo method = type.GetMethod(name, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase, null, types, null);
                translator.pushFunction(luaState, new LuaCSFunction(new LuaMethodWrapper(translator, obj2, type, method).call));
            }
            catch (Exception exception)
            {
                translator.throwError(luaState, exception.Message);
                LuaDLL.lua_pushnil(luaState);
            }
            return 1;
        }

        internal object getNetObject(IntPtr luaState, int index)
        {
            int num = LuaDLL.luanet_tonetobject(luaState, index);
            if (num != -1)
            {
                return this.objects[num];
            }
            return null;
        }

        public object getObject(IntPtr luaState, int index)
        {
            return LuaScriptMgr.GetVarObject(luaState, index);
        }

        internal object getRawNetObject(IntPtr luaState, int index)
        {
            int key = LuaDLL.luanet_rawnetobj(luaState, index);
            object obj2 = null;
            this.objects.TryGetValue(key, out obj2);
            return obj2;
        }

        internal LuaTable getTable(IntPtr luaState, int index)
        {
            LuaDLL.lua_pushvalue(luaState, index);
            return new LuaTable(LuaDLL.luaL_ref(luaState, LuaIndexes.LUA_REGISTRYINDEX), this.interpreter);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int importType(IntPtr luaState)
        {
            ObjectTranslator translator = FromState(luaState);
            string className = LuaDLL.lua_tostring(luaState, 1);
            Type t = translator.FindType(className);
            if (t != null)
            {
                translator.pushType(luaState, t);
            }
            else
            {
                LuaDLL.lua_pushnil(luaState);
            }
            return 1;
        }

        private static bool IsILua(object o)
        {
            return ((o is ILuaGeneratedType) && (o.GetType().GetInterface("ILuaGeneratedType") != null));
        }

        private bool IsValueType(Type t)
        {
            bool isValueType = false;
            if (!valueTypeMap.TryGetValue(t, out isValueType))
            {
                isValueType = t.IsValueType;
                valueTypeMap.Add(t, isValueType);
            }
            return isValueType;
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int loadAssembly(IntPtr luaState)
        {
            ObjectTranslator translator = FromState(luaState);
            try
            {
                string assemblyString = LuaDLL.lua_tostring(luaState, 1);
                Assembly item = null;
                try
                {
                    item = Assembly.Load(assemblyString);
                }
                catch (BadImageFormatException)
                {
                }
                if (item == null)
                {
                    item = Assembly.Load(AssemblyName.GetAssemblyName(assemblyString));
                }
                if ((item != null) && !translator.assemblies.Contains(item))
                {
                    translator.assemblies.Add(item);
                }
            }
            catch (Exception exception)
            {
                translator.throwError(luaState, exception.Message);
            }
            return 0;
        }

        internal bool matchParameters(IntPtr luaState, MethodBase method, ref MethodCache methodCache)
        {
            return this.metaFunctions.matchParameters(luaState, method, ref methodCache);
        }

        internal object[] popValues(IntPtr luaState, int oldTop)
        {
            int num = LuaDLL.lua_gettop(luaState);
            if (oldTop == num)
            {
                return null;
            }
            List<object> list = new List<object>();
            for (int i = oldTop + 1; i <= num; i++)
            {
                list.Add(this.getObject(luaState, i));
            }
            LuaDLL.lua_settop(luaState, oldTop);
            return list.ToArray();
        }

        internal object[] popValues(IntPtr luaState, int oldTop, Type[] popTypes)
        {
            int num2;
            int num = LuaDLL.lua_gettop(luaState);
            if (oldTop == num)
            {
                return null;
            }
            List<object> list = new List<object>();
            if (popTypes[0] == typeof(void))
            {
                num2 = 1;
            }
            else
            {
                num2 = 0;
            }
            for (int i = oldTop + 1; i <= num; i++)
            {
                list.Add(this.getAsType(luaState, i, popTypes[num2]));
                num2++;
            }
            LuaDLL.lua_settop(luaState, oldTop);
            return list.ToArray();
        }

        internal void push(IntPtr luaState, object o)
        {
            LuaScriptMgr.PushVarObject(luaState, o);
        }

        public int pushError(IntPtr luaState, string msg)
        {
            LuaDLL.lua_pushnil(luaState);
            LuaDLL.lua_pushstring(luaState, msg);
            return 2;
        }

        internal void pushFunction(IntPtr luaState, LuaCSFunction func)
        {
            this.pushObject(luaState, func, "luaNet_function");
        }

        private static void PushMetaTable(IntPtr L, Type t)
        {
            int num = -1;
            if (!typeMetaMap.TryGetValue(t, out num))
            {
                LuaDLL.luaL_getmetatable(L, t.AssemblyQualifiedName);
                if (!LuaDLL.lua_isnil(L, -1))
                {
                    LuaDLL.lua_pushvalue(L, -1);
                    num = LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
                    typeMetaMap.Add(t, num);
                }
            }
            else
            {
                LuaDLL.lua_getref(L, num);
            }
        }

        private void pushNewObject(IntPtr luaState, object o, int index, string metatable)
        {
            LuaDLL.lua_getref(luaState, this.weakTableRef);
            LuaDLL.luanet_newudata(luaState, index);
            if (metatable == "luaNet_metatable")
            {
                Type type = o.GetType();
                PushMetaTable(luaState, o.GetType());
                if (LuaDLL.lua_isnil(luaState, -1))
                {
                    string assemblyQualifiedName = type.AssemblyQualifiedName;
                    LuaDLL.lua_settop(luaState, -2);
                    LuaDLL.luaL_newmetatable(luaState, assemblyQualifiedName);
                    LuaDLL.lua_pushstring(luaState, "cache");
                    LuaDLL.lua_newtable(luaState);
                    LuaDLL.lua_rawset(luaState, -3);
                    LuaDLL.lua_pushlightuserdata(luaState, LuaDLL.luanet_gettag());
                    LuaDLL.lua_pushnumber(luaState, 1.0);
                    LuaDLL.lua_rawset(luaState, -3);
                    LuaDLL.lua_pushstring(luaState, "__index");
                    LuaDLL.lua_pushstring(luaState, "luaNet_indexfunction");
                    LuaDLL.lua_rawget(luaState, LuaIndexes.LUA_REGISTRYINDEX);
                    LuaDLL.lua_rawset(luaState, -3);
                    LuaDLL.lua_pushstring(luaState, "__gc");
                    LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction, 0);
                    LuaDLL.lua_rawset(luaState, -3);
                    LuaDLL.lua_pushstring(luaState, "__tostring");
                    LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.toStringFunction, 0);
                    LuaDLL.lua_rawset(luaState, -3);
                    LuaDLL.lua_pushstring(luaState, "__newindex");
                    LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.newindexFunction, 0);
                    LuaDLL.lua_rawset(luaState, -3);
                }
            }
            else
            {
                LuaDLL.luaL_getmetatable(luaState, metatable);
            }
            LuaDLL.lua_setmetatable(luaState, -2);
            LuaDLL.lua_pushvalue(luaState, -1);
            LuaDLL.lua_rawseti(luaState, -3, index);
            LuaDLL.lua_remove(luaState, -2);
        }

        public void PushNewValueObject(IntPtr luaState, object o, int index)
        {
            LuaDLL.luanet_newudata(luaState, index);
            Type type = o.GetType();
            PushMetaTable(luaState, o.GetType());
            if (LuaDLL.lua_isnil(luaState, -1))
            {
                string assemblyQualifiedName = type.AssemblyQualifiedName;
                LuaDLL.lua_settop(luaState, -2);
                LuaDLL.luaL_newmetatable(luaState, assemblyQualifiedName);
                LuaDLL.lua_pushstring(luaState, "cache");
                LuaDLL.lua_newtable(luaState);
                LuaDLL.lua_rawset(luaState, -3);
                LuaDLL.lua_pushlightuserdata(luaState, LuaDLL.luanet_gettag());
                LuaDLL.lua_pushnumber(luaState, 1.0);
                LuaDLL.lua_rawset(luaState, -3);
                LuaDLL.lua_pushstring(luaState, "__index");
                LuaDLL.lua_pushstring(luaState, "luaNet_indexfunction");
                LuaDLL.lua_rawget(luaState, LuaIndexes.LUA_REGISTRYINDEX);
                LuaDLL.lua_rawset(luaState, -3);
                LuaDLL.lua_pushstring(luaState, "__gc");
                LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction, 0);
                LuaDLL.lua_rawset(luaState, -3);
                LuaDLL.lua_pushstring(luaState, "__tostring");
                LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.toStringFunction, 0);
                LuaDLL.lua_rawset(luaState, -3);
                LuaDLL.lua_pushstring(luaState, "__newindex");
                LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.newindexFunction, 0);
                LuaDLL.lua_rawset(luaState, -3);
            }
            LuaDLL.lua_setmetatable(luaState, -2);
        }

        public void pushObject(IntPtr luaState, object o, string metatable)
        {
            if (o == null)
            {
                LuaDLL.lua_pushnil(luaState);
            }
            else
            {
                int num = -1;
                bool isValueType = o.GetType().IsValueType;
                if (!isValueType && this.objectsBackMap.TryGetValue(o, out num))
                {
                    if (LuaDLL.tolua_pushudata(luaState, this.weakTableRef, num))
                    {
                        return;
                    }
                    this.collectObject(o, num);
                }
                num = this.addObject(o, isValueType);
                this.pushNewObject(luaState, o, num, metatable);
            }
        }

        public void PushTranslator(IntPtr L)
        {
            list.Add(this);
            LuaDLL.lua_pushnumber(L, (double) indexTranslator);
            LuaDLL.lua_setglobal(L, "_translator");
            indexTranslator++;
        }

        internal void pushType(IntPtr luaState, Type t)
        {
            this.pushObject(luaState, new ProxyType(t), "luaNet_class");
        }

        internal void PushValueResult(IntPtr lua, object o)
        {
            int index = this.addObject(o, true);
            this.PushNewValueObject(lua, o, index);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int registerTable(IntPtr luaState)
        {
            ObjectTranslator translator = FromState(luaState);
            if (LuaDLL.lua_type(luaState, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaTable luaTable = translator.getTable(luaState, 1);
                string className = LuaDLL.lua_tostring(luaState, 2);
                if (className != null)
                {
                    Type klass = translator.FindType(className);
                    if (klass != null)
                    {
                        object classInstance = CodeGeneration.Instance.GetClassInstance(klass, luaTable);
                        translator.pushObject(luaState, classInstance, "luaNet_metatable");
                        LuaDLL.lua_newtable(luaState);
                        LuaDLL.lua_pushstring(luaState, "__index");
                        LuaDLL.lua_pushvalue(luaState, -3);
                        LuaDLL.lua_settable(luaState, -3);
                        LuaDLL.lua_pushstring(luaState, "__newindex");
                        LuaDLL.lua_pushvalue(luaState, -3);
                        LuaDLL.lua_settable(luaState, -3);
                        LuaDLL.lua_setmetatable(luaState, 1);
                        LuaDLL.lua_pushstring(luaState, "base");
                        int index = translator.addObject(classInstance);
                        translator.pushNewObject(luaState, classInstance, index, "luaNet_searchbase");
                        LuaDLL.lua_rawset(luaState, 1);
                    }
                    else
                    {
                        translator.throwError(luaState, "register_table: can not find superclass '" + className + "'");
                    }
                }
                else
                {
                    translator.throwError(luaState, "register_table: superclass name can not be null");
                }
            }
            else
            {
                translator.throwError(luaState, "register_table: first arg is not a table");
            }
            return 0;
        }

        internal int returnValues(IntPtr luaState, object[] returnValues)
        {
            if (!LuaDLL.lua_checkstack(luaState, returnValues.Length + 5))
            {
                return 0;
            }
            for (int i = 0; i < returnValues.Length; i++)
            {
                this.push(luaState, returnValues[i]);
            }
            return returnValues.Length;
        }

        private void setGlobalFunctions(IntPtr luaState)
        {
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.indexFunction, 0);
            LuaDLL.lua_setglobal(luaState, "get_object_member");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.importTypeFunction, 0);
            LuaDLL.lua_setglobal(luaState, "import_type");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.loadAssemblyFunction, 0);
            LuaDLL.lua_setglobal(luaState, "load_assembly");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.registerTableFunction, 0);
            LuaDLL.lua_setglobal(luaState, "make_object");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.unregisterTableFunction, 0);
            LuaDLL.lua_setglobal(luaState, "free_object");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.getMethodSigFunction, 0);
            LuaDLL.lua_setglobal(luaState, "get_method_bysig");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.getConstructorSigFunction, 0);
            LuaDLL.lua_setglobal(luaState, "get_constructor_bysig");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.ctypeFunction, 0);
            LuaDLL.lua_setglobal(luaState, "ctype");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.enumFromIntFunction, 0);
            LuaDLL.lua_setglobal(luaState, "enum");
        }

        public void SetValueObject(IntPtr luaState, int stackPos, object obj)
        {
            int num = LuaDLL.luanet_rawnetobj(luaState, stackPos);
            if (num != -1)
            {
                this.objects[num] = obj;
            }
        }

        internal Array tableToArray(object luaParamValue, Type paramArrayType)
        {
            return this.metaFunctions.TableToArray(luaParamValue, paramArrayType);
        }

        internal void throwError(IntPtr luaState, string message)
        {
            LuaDLL.luaL_error(luaState, message);
        }

        private Type typeOf(IntPtr luaState, int idx)
        {
            int num = LuaDLL.luanet_checkudata(luaState, 1, "luaNet_class");
            if (num == -1)
            {
                return null;
            }
            ProxyType type = (ProxyType) this.objects[num];
            return type.UnderlyingSystemType;
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int unregisterTable(IntPtr luaState)
        {
            ObjectTranslator translator = FromState(luaState);
            try
            {
                if (LuaDLL.lua_getmetatable(luaState, 1) != 0)
                {
                    LuaDLL.lua_pushstring(luaState, "__index");
                    LuaDLL.lua_gettable(luaState, -2);
                    object obj2 = translator.getRawNetObject(luaState, -1);
                    if (obj2 == null)
                    {
                        translator.throwError(luaState, "unregister_table: arg is not valid table");
                    }
                    FieldInfo field = obj2.GetType().GetField("__luaInterface_luaTable");
                    if (field == null)
                    {
                        translator.throwError(luaState, "unregister_table: arg is not valid table");
                    }
                    field.SetValue(obj2, null);
                    LuaDLL.lua_pushnil(luaState);
                    LuaDLL.lua_setmetatable(luaState, 1);
                    LuaDLL.lua_pushstring(luaState, "base");
                    LuaDLL.lua_pushnil(luaState);
                    LuaDLL.lua_settable(luaState, 1);
                }
                else
                {
                    translator.throwError(luaState, "unregister_table: arg is not valid table");
                }
            }
            catch (Exception exception)
            {
                translator.throwError(luaState, exception.Message);
            }
            return 0;
        }

        public int weakTableRef { get; private set; }

        private class CompareObject : IEqualityComparer<object>
        {
            public bool Equals(object x, object y)
            {
                return (x == y);
            }

            public int GetHashCode(object obj)
            {
                if (obj != null)
                {
                    return obj.GetHashCode();
                }
                return 0;
            }
        }
    }
}

