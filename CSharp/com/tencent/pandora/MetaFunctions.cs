namespace com.tencent.pandora
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public class MetaFunctions
    {
        internal LuaCSFunction baseIndexFunction;
        internal LuaCSFunction callConstructorFunction;
        internal LuaCSFunction classIndexFunction;
        internal LuaCSFunction classNewindexFunction;
        internal LuaCSFunction execDelegateFunction;
        internal LuaCSFunction gcFunction;
        internal LuaCSFunction indexFunction;
        internal static string luaIndexFunction = "\n        local function index(obj,name)        \n        local meta=getmetatable(obj)\n        local cached=meta.cache[name]        \n        if cached then\n           return cached\n        else\n           local value,isFunc = get_object_member(obj,name)\n           if value==nil and type(isFunc)=='string' then error(isFunc,2) end\n           if isFunc then\n            meta.cache[name]=value\n           end\n           return value\n         end\n    end\n    return index";
        private Hashtable memberCache = new Hashtable();
        internal LuaCSFunction newindexFunction;
        internal LuaCSFunction toStringFunction;
        private ObjectTranslator translator;

        public MetaFunctions(ObjectTranslator translator)
        {
            this.translator = translator;
            this.gcFunction = new LuaCSFunction(MetaFunctions.collectObject);
            this.toStringFunction = new LuaCSFunction(MetaFunctions.toString);
            this.indexFunction = new LuaCSFunction(MetaFunctions.getMethod);
            this.newindexFunction = new LuaCSFunction(MetaFunctions.setFieldOrProperty);
            this.baseIndexFunction = new LuaCSFunction(MetaFunctions.getBaseMethod);
            this.callConstructorFunction = new LuaCSFunction(MetaFunctions.callConstructor);
            this.classIndexFunction = new LuaCSFunction(MetaFunctions.getClassMethod);
            this.classNewindexFunction = new LuaCSFunction(MetaFunctions.setClassFieldOrProperty);
            this.execDelegateFunction = new LuaCSFunction(MetaFunctions.runFunctionDelegate);
        }

        private bool _IsParamsArray(IntPtr luaState, int currentLuaParam, ParameterInfo currentNetParam, out ExtractValue extractValue)
        {
            extractValue = null;
            if (currentNetParam.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0)
            {
                LuaTypes types;
                try
                {
                    types = LuaDLL.lua_type(luaState, currentLuaParam);
                }
                catch (Exception)
                {
                    extractValue = null;
                    return false;
                }
                if (types == LuaTypes.LUA_TTABLE)
                {
                    try
                    {
                        extractValue = this.translator.typeChecker.getExtractor(typeof(LuaTable));
                    }
                    catch (Exception)
                    {
                    }
                    if (extractValue != null)
                    {
                        return true;
                    }
                }
                else
                {
                    Type elementType = currentNetParam.ParameterType.GetElementType();
                    try
                    {
                        extractValue = this.translator.typeChecker.checkType(luaState, currentLuaParam, elementType);
                    }
                    catch (Exception)
                    {
                    }
                    if (extractValue != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool _IsTypeCorrect(IntPtr luaState, int currentLuaParam, ParameterInfo currentNetParam, out ExtractValue extractValue)
        {
            try
            {
                ExtractValue value2;
                extractValue = value2 = this.translator.typeChecker.checkType(luaState, currentLuaParam, currentNetParam.ParameterType);
                return (value2 != null);
            }
            catch
            {
                extractValue = null;
                return false;
            }
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int callConstructor(IntPtr luaState)
        {
            ObjectTranslator translator = ObjectTranslator.FromState(luaState);
            MethodCache methodCache = new MethodCache();
            object obj2 = translator.getRawNetObject(luaState, 1);
            if ((obj2 == null) || !(obj2 is IReflect))
            {
                LuaDLL.luaL_error(luaState, "trying to call constructor on an invalid type reference");
                LuaDLL.lua_pushnil(luaState);
                return 1;
            }
            IReflect reflect = (IReflect) obj2;
            LuaDLL.lua_remove(luaState, 1);
            ConstructorInfo[] constructors = reflect.UnderlyingSystemType.GetConstructors();
            foreach (ConstructorInfo info in constructors)
            {
                if (translator.metaFunctions.matchParameters(luaState, info, ref methodCache))
                {
                    try
                    {
                        translator.push(luaState, info.Invoke(methodCache.args));
                    }
                    catch (TargetInvocationException exception)
                    {
                        translator.metaFunctions.ThrowError(luaState, exception);
                        LuaDLL.lua_pushnil(luaState);
                    }
                    catch
                    {
                        LuaDLL.lua_pushnil(luaState);
                    }
                    return 1;
                }
            }
            string str = (constructors.Length != 0) ? constructors[0].Name : "unknown";
            LuaDLL.luaL_error(luaState, string.Format("{0} does not contain constructor({1}) argument match", reflect.UnderlyingSystemType, str));
            LuaDLL.lua_pushnil(luaState);
            return 1;
        }

        private object checkMemberCache(Hashtable memberCache, IReflect objType, string memberName)
        {
            Hashtable hashtable = (Hashtable) memberCache[objType];
            if (hashtable != null)
            {
                return hashtable[memberName];
            }
            return null;
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int collectObject(IntPtr luaState)
        {
            int udata = LuaDLL.luanet_rawnetobj(luaState, 1);
            if (udata != -1)
            {
                ObjectTranslator.FromState(luaState).collectObject(udata);
            }
            return 0;
        }

        public static void dumpStack(ObjectTranslator translator, IntPtr luaState)
        {
            int num = LuaDLL.lua_gettop(luaState);
            for (int i = 1; i <= num; i++)
            {
                LuaTypes type = LuaDLL.lua_type(luaState, i);
                string str = (type != LuaTypes.LUA_TTABLE) ? LuaDLL.lua_typename(luaState, type) : "table";
                string str2 = LuaDLL.lua_tostring(luaState, i);
                if (type == LuaTypes.LUA_TUSERDATA)
                {
                    str2 = translator.getRawNetObject(luaState, i).ToString();
                }
            }
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int getBaseMethod(IntPtr luaState)
        {
            ObjectTranslator translator = ObjectTranslator.FromState(luaState);
            object obj2 = translator.getRawNetObject(luaState, 1);
            if (obj2 == null)
            {
                translator.throwError(luaState, "trying to index an invalid object reference");
                LuaDLL.lua_pushnil(luaState);
                LuaDLL.lua_pushboolean(luaState, false);
                return 2;
            }
            string methodName = LuaDLL.lua_tostring(luaState, 2);
            if (methodName == null)
            {
                LuaDLL.lua_pushnil(luaState);
                LuaDLL.lua_pushboolean(luaState, false);
                return 2;
            }
            translator.metaFunctions.getMember(luaState, obj2.GetType(), obj2, "__luaInterface_base_" + methodName, BindingFlags.Instance | BindingFlags.IgnoreCase);
            LuaDLL.lua_settop(luaState, -2);
            if (LuaDLL.lua_type(luaState, -1) == LuaTypes.LUA_TNIL)
            {
                LuaDLL.lua_settop(luaState, -2);
                return translator.metaFunctions.getMember(luaState, obj2.GetType(), obj2, methodName, BindingFlags.Instance | BindingFlags.IgnoreCase);
            }
            LuaDLL.lua_pushboolean(luaState, false);
            return 2;
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int getClassMethod(IntPtr luaState)
        {
            ObjectTranslator translator = ObjectTranslator.FromState(luaState);
            object obj2 = translator.getRawNetObject(luaState, 1);
            if ((obj2 == null) || !(obj2 is IReflect))
            {
                translator.throwError(luaState, "trying to index an invalid type reference");
                LuaDLL.lua_pushnil(luaState);
                return 1;
            }
            IReflect objType = (IReflect) obj2;
            if (LuaDLL.lua_isnumber(luaState, 2))
            {
                int length = (int) LuaDLL.lua_tonumber(luaState, 2);
                translator.push(luaState, Array.CreateInstance(objType.UnderlyingSystemType, length));
                return 1;
            }
            string methodName = LuaDLL.lua_tostring(luaState, 2);
            if (methodName == null)
            {
                LuaDLL.lua_pushnil(luaState);
                return 1;
            }
            return translator.metaFunctions.getMember(luaState, objType, null, methodName, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.IgnoreCase);
        }

        private int getMember(IntPtr luaState, IReflect objType, object obj, string methodName, BindingFlags bindingType)
        {
            bool flag = false;
            MemberInfo member = null;
            object obj2 = this.checkMemberCache(this.memberCache, objType, methodName);
            if (obj2 is LuaCSFunction)
            {
                this.translator.pushFunction(luaState, (LuaCSFunction) obj2);
                this.translator.push(luaState, true);
                return 2;
            }
            if (obj2 != null)
            {
                member = (MemberInfo) obj2;
            }
            else
            {
                MemberInfo[] infoArray = objType.GetMember(methodName, (bindingType | BindingFlags.Public) | BindingFlags.IgnoreCase);
                if (infoArray.Length > 0)
                {
                    member = infoArray[0];
                }
                else
                {
                    infoArray = objType.GetMember(methodName, ((bindingType | BindingFlags.Static) | BindingFlags.Public) | BindingFlags.IgnoreCase);
                    if (infoArray.Length > 0)
                    {
                        member = infoArray[0];
                        flag = true;
                    }
                }
            }
            if (member != null)
            {
                if (member.MemberType == MemberTypes.Field)
                {
                    FieldInfo info2 = (FieldInfo) member;
                    if (obj2 == null)
                    {
                        this.setMemberCache(this.memberCache, objType, methodName, member);
                    }
                    try
                    {
                        this.translator.push(luaState, info2.GetValue(obj));
                    }
                    catch
                    {
                        LuaDLL.lua_pushnil(luaState);
                    }
                }
                else if (member.MemberType == MemberTypes.Property)
                {
                    PropertyInfo info3 = (PropertyInfo) member;
                    if (obj2 == null)
                    {
                        this.setMemberCache(this.memberCache, objType, methodName, member);
                    }
                    try
                    {
                        object o = info3.GetGetMethod().Invoke(obj, null);
                        this.translator.push(luaState, o);
                    }
                    catch (ArgumentException)
                    {
                        if ((objType is Type) && (((Type) objType) != typeof(object)))
                        {
                            return this.getMember(luaState, ((Type) objType).BaseType, obj, methodName, bindingType);
                        }
                        LuaDLL.lua_pushnil(luaState);
                    }
                    catch (TargetInvocationException exception)
                    {
                        this.ThrowError(luaState, exception);
                        LuaDLL.lua_pushnil(luaState);
                    }
                }
                else if (member.MemberType == MemberTypes.Event)
                {
                    EventInfo eventInfo = (EventInfo) member;
                    if (obj2 == null)
                    {
                        this.setMemberCache(this.memberCache, objType, methodName, member);
                    }
                    this.translator.push(luaState, new RegisterEventHandler(this.translator.pendingEvents, obj, eventInfo));
                }
                else if (!flag)
                {
                    if (member.MemberType != MemberTypes.NestedType)
                    {
                        LuaCSFunction function = new LuaCSFunction(new LuaMethodWrapper(this.translator, objType, methodName, bindingType).call);
                        if (obj2 == null)
                        {
                            this.setMemberCache(this.memberCache, objType, methodName, function);
                        }
                        this.translator.pushFunction(luaState, function);
                        this.translator.push(luaState, true);
                        return 2;
                    }
                    if (obj2 == null)
                    {
                        this.setMemberCache(this.memberCache, objType, methodName, member);
                    }
                    string name = member.Name;
                    string className = member.DeclaringType.FullName + "+" + name;
                    Type t = this.translator.FindType(className);
                    this.translator.pushType(luaState, t);
                }
                else
                {
                    this.translator.throwError(luaState, "can't pass instance to static method " + methodName);
                    LuaDLL.lua_pushnil(luaState);
                }
            }
            else
            {
                this.translator.throwError(luaState, "unknown member name " + methodName);
                LuaDLL.lua_pushnil(luaState);
            }
            this.translator.push(luaState, false);
            return 2;
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int getMethod(IntPtr luaState)
        {
            ObjectTranslator translator = ObjectTranslator.FromState(luaState);
            object obj2 = translator.getRawNetObject(luaState, 1);
            if (obj2 == null)
            {
                translator.throwError(luaState, "trying to index an invalid object reference");
                LuaDLL.lua_pushnil(luaState);
                return 1;
            }
            object obj3 = translator.getObject(luaState, 2);
            string methodName = obj3 as string;
            Type objType = obj2.GetType();
            try
            {
                if ((methodName != null) && translator.metaFunctions.isMemberPresent(objType, methodName))
                {
                    return translator.metaFunctions.getMember(luaState, objType, obj2, methodName, BindingFlags.Instance | BindingFlags.IgnoreCase);
                }
            }
            catch
            {
            }
            bool flag = true;
            if (objType.IsArray && (obj3 is double))
            {
                int index = (int) ((double) obj3);
                Array array = obj2 as Array;
                if (index >= array.Length)
                {
                    object[] objArray1 = new object[] { "array index out of bounds: ", index, " ", array.Length };
                    return translator.pushError(luaState, string.Concat(objArray1));
                }
                object o = array.GetValue(index);
                translator.push(luaState, o);
                flag = false;
            }
            else
            {
                foreach (MethodInfo info in objType.GetMethods())
                {
                    if ((info.Name == "get_Item") && (info.GetParameters().Length == 1))
                    {
                        MethodInfo info2 = info;
                        ParameterInfo[] infoArray3 = (info2 == null) ? null : info2.GetParameters();
                        if ((infoArray3 == null) || (infoArray3.Length != 1))
                        {
                            return translator.pushError(luaState, "method not found (or no indexer): " + obj3);
                        }
                        obj3 = translator.getAsType(luaState, 2, infoArray3[0].ParameterType);
                        try
                        {
                            object[] parameters = new object[] { obj3 };
                            object obj5 = info2.Invoke(obj2, parameters);
                            translator.push(luaState, obj5);
                            flag = false;
                        }
                        catch (TargetInvocationException exception)
                        {
                            if (exception.InnerException is KeyNotFoundException)
                            {
                                return translator.pushError(luaState, "key '" + obj3 + "' not found ");
                            }
                            object[] objArray3 = new object[] { "exception indexing '", obj3, "' ", exception.Message };
                            return translator.pushError(luaState, string.Concat(objArray3));
                        }
                    }
                }
            }
            if (flag)
            {
                return translator.pushError(luaState, "cannot find " + obj3);
            }
            LuaDLL.lua_pushboolean(luaState, false);
            return 2;
        }

        private static bool IsInteger(double x)
        {
            return (Math.Ceiling(x) == x);
        }

        private bool isMemberPresent(IReflect objType, string methodName)
        {
            return ((this.checkMemberCache(this.memberCache, objType, methodName) != null) || (objType.GetMember(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase).Length > 0));
        }

        internal bool matchParameters(IntPtr luaState, MethodBase method, ref MethodCache methodCache)
        {
            bool flag = true;
            ParameterInfo[] parameters = method.GetParameters();
            int currentLuaParam = 1;
            int num2 = LuaDLL.lua_gettop(luaState);
            ArrayList list = new ArrayList();
            List<int> list2 = new List<int>();
            List<MethodArgs> list3 = new List<MethodArgs>();
            foreach (ParameterInfo info in parameters)
            {
                if (!info.IsIn && info.IsOut)
                {
                    list2.Add(list.Add(null));
                }
                else
                {
                    ExtractValue value2;
                    if (currentLuaParam > num2)
                    {
                        if (info.IsOptional)
                        {
                            list.Add(info.DefaultValue);
                            goto Label_01A4;
                        }
                        flag = false;
                        break;
                    }
                    if (this._IsTypeCorrect(luaState, currentLuaParam, info, out value2))
                    {
                        int item = list.Add(value2(luaState, currentLuaParam));
                        MethodArgs args = new MethodArgs {
                            index = item,
                            extractValue = value2
                        };
                        list3.Add(args);
                        if (info.ParameterType.IsByRef)
                        {
                            list2.Add(item);
                        }
                        currentLuaParam++;
                    }
                    else if (this._IsParamsArray(luaState, currentLuaParam, info, out value2))
                    {
                        object luaParamValue = value2(luaState, currentLuaParam);
                        Type elementType = info.ParameterType.GetElementType();
                        Array array = this.TableToArray(luaParamValue, elementType);
                        int num5 = list.Add(array);
                        MethodArgs args2 = new MethodArgs {
                            index = num5,
                            extractValue = value2,
                            isParamsArray = true,
                            paramsArrayType = elementType
                        };
                        list3.Add(args2);
                        currentLuaParam++;
                    }
                    else if (info.IsOptional)
                    {
                        list.Add(info.DefaultValue);
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                Label_01A4:;
                }
            }
            if (currentLuaParam != (num2 + 1))
            {
                flag = false;
            }
            if (flag)
            {
                methodCache.args = list.ToArray();
                methodCache.cachedMethod = method;
                methodCache.outList = list2.ToArray();
                methodCache.argTypes = list3.ToArray();
            }
            return flag;
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int runFunctionDelegate(IntPtr luaState)
        {
            LuaCSFunction function = (LuaCSFunction) ObjectTranslator.FromState(luaState).getRawNetObject(luaState, 1);
            LuaDLL.lua_remove(luaState, 1);
            return function(luaState);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int setClassFieldOrProperty(IntPtr luaState)
        {
            ObjectTranslator translator = ObjectTranslator.FromState(luaState);
            object obj2 = translator.getRawNetObject(luaState, 1);
            if ((obj2 == null) || !(obj2 is IReflect))
            {
                translator.throwError(luaState, "trying to index an invalid type reference");
                return 0;
            }
            IReflect targetType = (IReflect) obj2;
            return translator.metaFunctions.setMember(luaState, targetType, null, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.IgnoreCase);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int setFieldOrProperty(IntPtr luaState)
        {
            string str;
            ObjectTranslator translator = ObjectTranslator.FromState(luaState);
            object target = translator.getRawNetObject(luaState, 1);
            if (target == null)
            {
                translator.throwError(luaState, "trying to index and invalid object reference");
                return 0;
            }
            Type targetType = target.GetType();
            if (!translator.metaFunctions.trySetMember(luaState, targetType, target, BindingFlags.Instance | BindingFlags.IgnoreCase, out str))
            {
                try
                {
                    if (targetType.IsArray && LuaDLL.lua_isnumber(luaState, 2))
                    {
                        int index = (int) LuaDLL.lua_tonumber(luaState, 2);
                        Array array = (Array) target;
                        object obj3 = translator.getAsType(luaState, 3, array.GetType().GetElementType());
                        array.SetValue(obj3, index);
                    }
                    else
                    {
                        MethodInfo method = targetType.GetMethod("set_Item");
                        if (method != null)
                        {
                            ParameterInfo[] parameters = method.GetParameters();
                            Type parameterType = parameters[1].ParameterType;
                            object obj4 = translator.getAsType(luaState, 3, parameterType);
                            Type paramType = parameters[0].ParameterType;
                            object obj5 = translator.getAsType(luaState, 2, paramType);
                            object[] objArray = new object[] { obj5, obj4 };
                            method.Invoke(target, objArray);
                        }
                        else
                        {
                            translator.throwError(luaState, str);
                        }
                    }
                }
                catch (SEHException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    translator.metaFunctions.ThrowError(luaState, exception);
                }
            }
            return 0;
        }

        private int setMember(IntPtr luaState, IReflect targetType, object target, BindingFlags bindingType)
        {
            string str;
            if (!this.trySetMember(luaState, targetType, target, bindingType, out str))
            {
                this.translator.throwError(luaState, str);
            }
            return 0;
        }

        private void setMemberCache(Hashtable memberCache, IReflect objType, string memberName, object member)
        {
            Hashtable hashtable = (Hashtable) memberCache[objType];
            if (hashtable == null)
            {
                hashtable = new Hashtable();
                memberCache[objType] = hashtable;
            }
            hashtable[memberName] = member;
        }

        internal Array TableToArray(object luaParamValue, Type paramArrayType)
        {
            Array array;
            if (luaParamValue is LuaTable)
            {
                LuaTable table = (LuaTable) luaParamValue;
                IDictionaryEnumerator enumerator = table.GetEnumerator();
                enumerator.Reset();
                array = Array.CreateInstance(paramArrayType, table.Values.Count);
                for (int i = 0; enumerator.MoveNext(); i++)
                {
                    object obj2 = enumerator.Value;
                    if (((paramArrayType == typeof(object)) && (obj2 != null)) && ((obj2.GetType() == typeof(double)) && IsInteger((double) obj2)))
                    {
                        obj2 = Convert.ToInt32((double) obj2);
                    }
                    array.SetValue(Convert.ChangeType(obj2, paramArrayType), i);
                }
                return array;
            }
            array = Array.CreateInstance(paramArrayType, 1);
            array.SetValue(luaParamValue, 0);
            return array;
        }

        private void ThrowError(IntPtr luaState, Exception e)
        {
            TargetInvocationException exception = e as TargetInvocationException;
            if (exception != null)
            {
                e = exception.InnerException;
            }
            this.translator.throwError(luaState, e.Message);
        }

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int toString(IntPtr luaState)
        {
            ObjectTranslator translator = ObjectTranslator.FromState(luaState);
            object obj2 = translator.getRawNetObject(luaState, 1);
            if (obj2 != null)
            {
                translator.push(luaState, obj2.ToString() + ": " + obj2.GetHashCode());
            }
            else
            {
                LuaDLL.lua_pushnil(luaState);
            }
            return 1;
        }

        private bool trySetMember(IntPtr luaState, IReflect targetType, object target, BindingFlags bindingType, out string detailMessage)
        {
            detailMessage = null;
            if (LuaDLL.lua_type(luaState, 2) != LuaTypes.LUA_TSTRING)
            {
                detailMessage = "property names must be strings";
                return false;
            }
            string memberName = LuaDLL.lua_tostring(luaState, 2);
            if (((memberName == null) || (memberName.Length < 1)) || (!char.IsLetter(memberName[0]) && (memberName[0] != '_')))
            {
                detailMessage = "invalid property name";
                return false;
            }
            MemberInfo member = (MemberInfo) this.checkMemberCache(this.memberCache, targetType, memberName);
            if (member == null)
            {
                MemberInfo[] infoArray = targetType.GetMember(memberName, (bindingType | BindingFlags.Public) | BindingFlags.IgnoreCase);
                if (infoArray.Length <= 0)
                {
                    detailMessage = "field or property '" + memberName + "' does not exist";
                    return false;
                }
                member = infoArray[0];
                this.setMemberCache(this.memberCache, targetType, memberName, member);
            }
            if (member.MemberType == MemberTypes.Field)
            {
                FieldInfo info2 = (FieldInfo) member;
                object obj2 = this.translator.getAsType(luaState, 3, info2.FieldType);
                try
                {
                    info2.SetValue(target, obj2);
                }
                catch (Exception exception)
                {
                    this.ThrowError(luaState, exception);
                }
                return true;
            }
            if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo info3 = (PropertyInfo) member;
                object obj3 = this.translator.getAsType(luaState, 3, info3.PropertyType);
                try
                {
                    info3.SetValue(target, obj3, null);
                }
                catch (Exception exception2)
                {
                    this.ThrowError(luaState, exception2);
                }
                return true;
            }
            detailMessage = "'" + memberName + "' is not a .net field or property";
            return false;
        }
    }
}

