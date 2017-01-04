namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class LuaMethodWrapper
    {
        private BindingFlags _BindingType;
        private ExtractValue _ExtractTarget;
        private MethodCache _LastCalledMethod;
        private MemberInfo[] _Members;
        private MethodBase _Method;
        private string _MethodName;
        private object _Target;
        public IReflect _TargetType;
        private ObjectTranslator _Translator;

        public LuaMethodWrapper(ObjectTranslator translator, object target, IReflect targetType, MethodBase method)
        {
            this._LastCalledMethod = new MethodCache();
            this._Translator = translator;
            this._Target = target;
            this._TargetType = targetType;
            if (targetType != null)
            {
                this._ExtractTarget = translator.typeChecker.getExtractor(targetType);
            }
            this._Method = method;
            this._MethodName = method.Name;
            if (method.IsStatic)
            {
                this._BindingType = BindingFlags.Static;
            }
            else
            {
                this._BindingType = BindingFlags.Instance;
            }
        }

        public LuaMethodWrapper(ObjectTranslator translator, IReflect targetType, string methodName, BindingFlags bindingType)
        {
            this._LastCalledMethod = new MethodCache();
            this._Translator = translator;
            this._MethodName = methodName;
            this._TargetType = targetType;
            if (targetType != null)
            {
                this._ExtractTarget = translator.typeChecker.getExtractor(targetType);
            }
            this._BindingType = bindingType;
            this._Members = targetType.UnderlyingSystemType.GetMember(methodName, MemberTypes.Method, (bindingType | BindingFlags.Public) | BindingFlags.IgnoreCase);
        }

        public int call(IntPtr luaState)
        {
            MethodBase method = this._Method;
            object obj2 = this._Target;
            bool flag = true;
            int num = 0;
            if (!LuaDLL.lua_checkstack(luaState, 5))
            {
                throw new LuaException("Lua stack overflow");
            }
            bool flag2 = (this._BindingType & BindingFlags.Static) == BindingFlags.Static;
            this.SetPendingException(null);
            if (method != null)
            {
                if (method.ContainsGenericParameters)
                {
                    this._Translator.matchParameters(luaState, method, ref this._LastCalledMethod);
                    if (!method.IsGenericMethodDefinition)
                    {
                        if (method.ContainsGenericParameters)
                        {
                            LuaDLL.luaL_error(luaState, "unable to invoke method on generic class as the current method is an open generic method");
                            LuaDLL.lua_pushnil(luaState);
                            return 1;
                        }
                    }
                    else
                    {
                        List<Type> list = new List<Type>();
                        foreach (object obj4 in this._LastCalledMethod.args)
                        {
                            list.Add(obj4.GetType());
                        }
                        MethodInfo info2 = (method as MethodInfo).MakeGenericMethod(list.ToArray());
                        this._Translator.push(luaState, info2.Invoke(obj2, this._LastCalledMethod.args));
                        flag = false;
                    }
                }
                else
                {
                    if ((!method.IsStatic && !method.IsConstructor) && (obj2 == null))
                    {
                        obj2 = this._ExtractTarget(luaState, 1);
                        LuaDLL.lua_remove(luaState, 1);
                    }
                    if (!this._Translator.matchParameters(luaState, method, ref this._LastCalledMethod))
                    {
                        LuaDLL.luaL_error(luaState, "invalid arguments to method call");
                        LuaDLL.lua_pushnil(luaState);
                        return 1;
                    }
                }
            }
            else
            {
                if (flag2)
                {
                    obj2 = null;
                }
                else
                {
                    obj2 = this._ExtractTarget(luaState, 1);
                }
                if (this._LastCalledMethod.cachedMethod != null)
                {
                    int num2 = !flag2 ? 1 : 0;
                    int num3 = LuaDLL.lua_gettop(luaState) - num2;
                    MethodBase cachedMethod = this._LastCalledMethod.cachedMethod;
                    if (num3 == this._LastCalledMethod.argTypes.Length)
                    {
                        if (!LuaDLL.lua_checkstack(luaState, this._LastCalledMethod.outList.Length + 6))
                        {
                            throw new LuaException("Lua stack overflow");
                        }
                        object[] parameters = this._LastCalledMethod.args;
                        try
                        {
                            for (int j = 0; j < this._LastCalledMethod.argTypes.Length; j++)
                            {
                                MethodArgs args = this._LastCalledMethod.argTypes[j];
                                object luaParamValue = args.extractValue(luaState, (j + 1) + num2);
                                if (this._LastCalledMethod.argTypes[j].isParamsArray)
                                {
                                    parameters[args.index] = this._Translator.tableToArray(luaParamValue, args.paramsArrayType);
                                }
                                else
                                {
                                    parameters[args.index] = luaParamValue;
                                }
                                if ((parameters[args.index] == null) && !LuaDLL.lua_isnil(luaState, (j + 1) + num2))
                                {
                                    throw new LuaException("argument number " + (j + 1) + " is invalid");
                                }
                            }
                            if ((this._BindingType & BindingFlags.Static) == BindingFlags.Static)
                            {
                                this._Translator.push(luaState, cachedMethod.Invoke(null, parameters));
                            }
                            else if (this._LastCalledMethod.cachedMethod.IsConstructor)
                            {
                                this._Translator.push(luaState, ((ConstructorInfo) cachedMethod).Invoke(parameters));
                            }
                            else
                            {
                                this._Translator.push(luaState, cachedMethod.Invoke(obj2, parameters));
                            }
                            flag = false;
                        }
                        catch (TargetInvocationException exception)
                        {
                            return this.SetPendingException(exception.GetBaseException());
                        }
                        catch (Exception exception2)
                        {
                            if (this._Members.Length == 1)
                            {
                                return this.SetPendingException(exception2);
                            }
                        }
                    }
                }
                if (flag)
                {
                    if (!flag2)
                    {
                        if (obj2 == null)
                        {
                            this._Translator.throwError(luaState, string.Format("instance method '{0}' requires a non null target object", this._MethodName));
                            LuaDLL.lua_pushnil(luaState);
                            return 1;
                        }
                        LuaDLL.lua_remove(luaState, 1);
                    }
                    bool flag3 = false;
                    string str = null;
                    foreach (MemberInfo info in this._Members)
                    {
                        str = info.ReflectedType.Name + "." + info.Name;
                        MethodBase base4 = (MethodInfo) info;
                        if (this._Translator.matchParameters(luaState, base4, ref this._LastCalledMethod))
                        {
                            flag3 = true;
                            break;
                        }
                    }
                    if (!flag3)
                    {
                        string message = (str != null) ? ("invalid arguments to method: " + str) : "invalid arguments to method call";
                        LuaDLL.luaL_error(luaState, message);
                        LuaDLL.lua_pushnil(luaState);
                        return 1;
                    }
                }
            }
            if (flag)
            {
                if (!LuaDLL.lua_checkstack(luaState, this._LastCalledMethod.outList.Length + 6))
                {
                    throw new LuaException("Lua stack overflow");
                }
                try
                {
                    if (flag2)
                    {
                        this._Translator.push(luaState, this._LastCalledMethod.cachedMethod.Invoke(null, this._LastCalledMethod.args));
                    }
                    else if (this._LastCalledMethod.cachedMethod.IsConstructor)
                    {
                        this._Translator.push(luaState, ((ConstructorInfo) this._LastCalledMethod.cachedMethod).Invoke(this._LastCalledMethod.args));
                    }
                    else
                    {
                        this._Translator.push(luaState, this._LastCalledMethod.cachedMethod.Invoke(obj2, this._LastCalledMethod.args));
                    }
                }
                catch (TargetInvocationException exception3)
                {
                    return this.SetPendingException(exception3.GetBaseException());
                }
                catch (Exception exception4)
                {
                    return this.SetPendingException(exception4);
                }
            }
            for (int i = 0; i < this._LastCalledMethod.outList.Length; i++)
            {
                num++;
                this._Translator.push(luaState, this._LastCalledMethod.args[this._LastCalledMethod.outList[i]]);
            }
            if (!this._LastCalledMethod.IsReturnVoid && (num > 0))
            {
                num++;
            }
            return ((num >= 1) ? num : 1);
        }

        private static bool IsInteger(double x)
        {
            return (Math.Ceiling(x) == x);
        }

        private int SetPendingException(Exception e)
        {
            return this._Translator.interpreter.SetPendingException(e);
        }
    }
}

