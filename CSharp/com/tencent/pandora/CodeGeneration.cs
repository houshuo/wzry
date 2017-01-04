namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class CodeGeneration
    {
        private AssemblyName assemblyName = new AssemblyName();
        private Dictionary<Type, LuaClassType> classCollection = new Dictionary<Type, LuaClassType>();
        private Type classHelper = typeof(LuaClassHelper);
        private Dictionary<Type, Type> delegateCollection = new Dictionary<Type, Type>();
        private Type delegateParent = typeof(LuaDelegate);
        private Dictionary<Type, Type> eventHandlerCollection = new Dictionary<Type, Type>();
        private Type eventHandlerParent = typeof(LuaEventHandler);
        private static readonly CodeGeneration instance = new CodeGeneration();
        private int luaClassNumber = 1;
        private AssemblyBuilder newAssembly;
        private ModuleBuilder newModule;

        private CodeGeneration()
        {
            this.assemblyName.Name = "LuaInterface_generatedcode";
            this.newAssembly = Thread.GetDomain().DefineDynamicAssembly(this.assemblyName, AssemblyBuilderAccess.Run);
            this.newModule = this.newAssembly.DefineDynamicModule("LuaInterface_generatedcode");
        }

        public void GenerateClass(Type klass, out Type newType, out Type[][] returnTypes, LuaTable luaTable)
        {
            string str;
            TypeBuilder builder;
            CodeGeneration generation = this;
            lock (generation)
            {
                str = "LuaGeneratedClass" + this.luaClassNumber;
                this.luaClassNumber++;
            }
            if (klass.IsInterface)
            {
                Type[] interfaces = new Type[] { klass, typeof(ILuaGeneratedType) };
                builder = this.newModule.DefineType(str, TypeAttributes.Public, typeof(object), interfaces);
            }
            else
            {
                Type[] typeArray2 = new Type[] { typeof(ILuaGeneratedType) };
                builder = this.newModule.DefineType(str, TypeAttributes.Public, klass, typeArray2);
            }
            FieldBuilder field = builder.DefineField("__luaInterface_luaTable", typeof(LuaTable), FieldAttributes.Public);
            FieldBuilder builder3 = builder.DefineField("__luaInterface_returnTypes", typeof(Type[][]), FieldAttributes.Public);
            Type[] parameterTypes = new Type[] { typeof(LuaTable), typeof(Type[][]) };
            ILGenerator iLGenerator = builder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameterTypes).GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            if (klass.IsInterface)
            {
                iLGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            }
            else
            {
                iLGenerator.Emit(OpCodes.Call, klass.GetConstructor(Type.EmptyTypes));
            }
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Stfld, field);
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldarg_2);
            iLGenerator.Emit(OpCodes.Stfld, builder3);
            iLGenerator.Emit(OpCodes.Ret);
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            MethodInfo[] methods = klass.GetMethods(bindingAttr);
            returnTypes = new Type[methods.Length][];
            int methodIndex = 0;
            foreach (MethodInfo info in methods)
            {
                if (klass.IsInterface)
                {
                    this.GenerateMethod(builder, info, MethodAttributes.NewSlot | MethodAttributes.HideBySig | MethodAttributes.Virtual, methodIndex, field, builder3, false, out returnTypes[methodIndex]);
                    methodIndex++;
                }
                else if ((!info.IsPrivate && !info.IsFinal) && (info.IsVirtual && (luaTable[info.Name] != null)))
                {
                    this.GenerateMethod(builder, info, (info.Attributes | MethodAttributes.NewSlot) ^ MethodAttributes.NewSlot, methodIndex, field, builder3, true, out returnTypes[methodIndex]);
                    methodIndex++;
                }
            }
            MethodBuilder methodInfoBody = builder.DefineMethod("__luaInterface_getLuaTable", MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public, typeof(LuaTable), new Type[0]);
            builder.DefineMethodOverride(methodInfoBody, typeof(ILuaGeneratedType).GetMethod("__luaInterface_getLuaTable"));
            iLGenerator = methodInfoBody.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, field);
            iLGenerator.Emit(OpCodes.Ret);
            newType = builder.CreateType();
        }

        private Type GenerateDelegate(Type delegateType)
        {
            string str;
            CodeGeneration generation = this;
            lock (generation)
            {
                str = "LuaGeneratedClass" + this.luaClassNumber;
                this.luaClassNumber++;
            }
            TypeBuilder builder = this.newModule.DefineType(str, TypeAttributes.Public, this.delegateParent);
            MethodInfo method = delegateType.GetMethod("Invoke");
            ParameterInfo[] parameters = method.GetParameters();
            Type[] parameterTypes = new Type[parameters.Length];
            Type returnType = method.ReturnType;
            int num = 0;
            int arg = 0;
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                parameterTypes[i] = parameters[i].ParameterType;
                if (!parameters[i].IsIn && parameters[i].IsOut)
                {
                    num++;
                }
                if (parameterTypes[i].IsByRef)
                {
                    arg++;
                }
            }
            int[] numArray = new int[arg];
            ILGenerator iLGenerator = builder.DefineMethod("CallFunction", method.Attributes, returnType, parameterTypes).GetILGenerator();
            iLGenerator.DeclareLocal(typeof(object[]));
            iLGenerator.DeclareLocal(typeof(object[]));
            iLGenerator.DeclareLocal(typeof(int[]));
            if (returnType != typeof(void))
            {
                iLGenerator.DeclareLocal(returnType);
            }
            else
            {
                iLGenerator.DeclareLocal(typeof(object));
            }
            iLGenerator.Emit(OpCodes.Ldc_I4, parameterTypes.Length);
            iLGenerator.Emit(OpCodes.Newarr, typeof(object));
            iLGenerator.Emit(OpCodes.Stloc_0);
            iLGenerator.Emit(OpCodes.Ldc_I4, (int) (parameterTypes.Length - num));
            iLGenerator.Emit(OpCodes.Newarr, typeof(object));
            iLGenerator.Emit(OpCodes.Stloc_1);
            iLGenerator.Emit(OpCodes.Ldc_I4, arg);
            iLGenerator.Emit(OpCodes.Newarr, typeof(int));
            iLGenerator.Emit(OpCodes.Stloc_2);
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            while (num4 < parameterTypes.Length)
            {
                iLGenerator.Emit(OpCodes.Ldloc_0);
                iLGenerator.Emit(OpCodes.Ldc_I4, num4);
                iLGenerator.Emit(OpCodes.Ldarg, (int) (num4 + 1));
                if (parameterTypes[num4].IsByRef)
                {
                    if (parameterTypes[num4].GetElementType().IsValueType)
                    {
                        iLGenerator.Emit(OpCodes.Ldobj, parameterTypes[num4].GetElementType());
                        iLGenerator.Emit(OpCodes.Box, parameterTypes[num4].GetElementType());
                    }
                    else
                    {
                        iLGenerator.Emit(OpCodes.Ldind_Ref);
                    }
                }
                else if (parameterTypes[num4].IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Box, parameterTypes[num4]);
                }
                iLGenerator.Emit(OpCodes.Stelem_Ref);
                if (parameterTypes[num4].IsByRef)
                {
                    iLGenerator.Emit(OpCodes.Ldloc_2);
                    iLGenerator.Emit(OpCodes.Ldc_I4, num6);
                    iLGenerator.Emit(OpCodes.Ldc_I4, num4);
                    iLGenerator.Emit(OpCodes.Stelem_I4);
                    numArray[num6] = num4;
                    num6++;
                }
                if (parameters[num4].IsIn || !parameters[num4].IsOut)
                {
                    iLGenerator.Emit(OpCodes.Ldloc_1);
                    iLGenerator.Emit(OpCodes.Ldc_I4, num5);
                    iLGenerator.Emit(OpCodes.Ldarg, (int) (num4 + 1));
                    if (parameterTypes[num4].IsByRef)
                    {
                        if (parameterTypes[num4].GetElementType().IsValueType)
                        {
                            iLGenerator.Emit(OpCodes.Ldobj, parameterTypes[num4].GetElementType());
                            iLGenerator.Emit(OpCodes.Box, parameterTypes[num4].GetElementType());
                        }
                        else
                        {
                            iLGenerator.Emit(OpCodes.Ldind_Ref);
                        }
                    }
                    else if (parameterTypes[num4].IsValueType)
                    {
                        iLGenerator.Emit(OpCodes.Box, parameterTypes[num4]);
                    }
                    iLGenerator.Emit(OpCodes.Stelem_Ref);
                    num5++;
                }
                num4++;
            }
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldloc_0);
            iLGenerator.Emit(OpCodes.Ldloc_1);
            iLGenerator.Emit(OpCodes.Ldloc_2);
            MethodInfo meth = this.delegateParent.GetMethod("callFunction");
            iLGenerator.Emit(OpCodes.Call, meth);
            if (returnType == typeof(void))
            {
                iLGenerator.Emit(OpCodes.Pop);
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            else if (returnType.IsValueType)
            {
                iLGenerator.Emit(OpCodes.Unbox, returnType);
                iLGenerator.Emit(OpCodes.Ldobj, returnType);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Castclass, returnType);
            }
            iLGenerator.Emit(OpCodes.Stloc_3);
            for (int j = 0; j < numArray.Length; j++)
            {
                iLGenerator.Emit(OpCodes.Ldarg, (int) (numArray[j] + 1));
                iLGenerator.Emit(OpCodes.Ldloc_0);
                iLGenerator.Emit(OpCodes.Ldc_I4, numArray[j]);
                iLGenerator.Emit(OpCodes.Ldelem_Ref);
                if (parameterTypes[numArray[j]].GetElementType().IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Unbox, parameterTypes[numArray[j]].GetElementType());
                    iLGenerator.Emit(OpCodes.Ldobj, parameterTypes[numArray[j]].GetElementType());
                    iLGenerator.Emit(OpCodes.Stobj, parameterTypes[numArray[j]].GetElementType());
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Castclass, parameterTypes[numArray[j]].GetElementType());
                    iLGenerator.Emit(OpCodes.Stind_Ref);
                }
            }
            if (returnType != typeof(void))
            {
                iLGenerator.Emit(OpCodes.Ldloc_3);
            }
            iLGenerator.Emit(OpCodes.Ret);
            return builder.CreateType();
        }

        private Type GenerateEvent(Type eventHandlerType)
        {
            string str;
            CodeGeneration generation = this;
            lock (generation)
            {
                str = "LuaGeneratedClass" + this.luaClassNumber;
                this.luaClassNumber++;
            }
            TypeBuilder builder = this.newModule.DefineType(str, TypeAttributes.Public, this.eventHandlerParent);
            Type[] parameterTypes = new Type[] { typeof(object), eventHandlerType };
            Type returnType = typeof(void);
            ILGenerator iLGenerator = builder.DefineMethod("HandleEvent", MethodAttributes.HideBySig | MethodAttributes.Public, returnType, parameterTypes).GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Ldarg_2);
            MethodInfo method = this.eventHandlerParent.GetMethod("handleEvent");
            iLGenerator.Emit(OpCodes.Call, method);
            iLGenerator.Emit(OpCodes.Ret);
            return builder.CreateType();
        }

        private void GenerateMethod(TypeBuilder myType, MethodInfo method, MethodAttributes attributes, int methodIndex, FieldInfo luaTableField, FieldInfo returnTypesField, bool generateBase, out Type[] returnTypes)
        {
            ParameterInfo[] parameters = method.GetParameters();
            Type[] parameterTypes = new Type[parameters.Length];
            List<Type> list = new List<Type>();
            int num = 0;
            int arg = 0;
            Type returnType = method.ReturnType;
            list.Add(returnType);
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                parameterTypes[i] = parameters[i].ParameterType;
                if (!parameters[i].IsIn && parameters[i].IsOut)
                {
                    num++;
                }
                if (parameterTypes[i].IsByRef)
                {
                    list.Add(parameterTypes[i].GetElementType());
                    arg++;
                }
            }
            int[] numArray = new int[arg];
            returnTypes = list.ToArray();
            if (generateBase)
            {
                string name = "__luaInterface_base_" + method.Name;
                ILGenerator generator = myType.DefineMethod(name, MethodAttributes.NewSlot | MethodAttributes.HideBySig | MethodAttributes.Public, returnType, parameterTypes).GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                for (int k = 0; k < parameterTypes.Length; k++)
                {
                    generator.Emit(OpCodes.Ldarg, (int) (k + 1));
                }
                generator.Emit(OpCodes.Call, method);
                generator.Emit(OpCodes.Ret);
            }
            MethodBuilder methodInfoBody = myType.DefineMethod(method.Name, attributes, returnType, parameterTypes);
            if (myType.BaseType.Equals(typeof(object)))
            {
                myType.DefineMethodOverride(methodInfoBody, method);
            }
            ILGenerator iLGenerator = methodInfoBody.GetILGenerator();
            iLGenerator.DeclareLocal(typeof(object[]));
            iLGenerator.DeclareLocal(typeof(object[]));
            iLGenerator.DeclareLocal(typeof(int[]));
            if (returnType != typeof(void))
            {
                iLGenerator.DeclareLocal(returnType);
            }
            else
            {
                iLGenerator.DeclareLocal(typeof(object));
            }
            iLGenerator.Emit(OpCodes.Ldc_I4, parameterTypes.Length);
            iLGenerator.Emit(OpCodes.Newarr, typeof(object));
            iLGenerator.Emit(OpCodes.Stloc_0);
            iLGenerator.Emit(OpCodes.Ldc_I4, (int) ((parameterTypes.Length - num) + 1));
            iLGenerator.Emit(OpCodes.Newarr, typeof(object));
            iLGenerator.Emit(OpCodes.Stloc_1);
            iLGenerator.Emit(OpCodes.Ldc_I4, arg);
            iLGenerator.Emit(OpCodes.Newarr, typeof(int));
            iLGenerator.Emit(OpCodes.Stloc_2);
            iLGenerator.Emit(OpCodes.Ldloc_1);
            iLGenerator.Emit(OpCodes.Ldc_I4_0);
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, luaTableField);
            iLGenerator.Emit(OpCodes.Stelem_Ref);
            int num5 = 0;
            int num6 = 1;
            int num7 = 0;
            while (num5 < parameterTypes.Length)
            {
                iLGenerator.Emit(OpCodes.Ldloc_0);
                iLGenerator.Emit(OpCodes.Ldc_I4, num5);
                iLGenerator.Emit(OpCodes.Ldarg, (int) (num5 + 1));
                if (parameterTypes[num5].IsByRef)
                {
                    if (parameterTypes[num5].GetElementType().IsValueType)
                    {
                        iLGenerator.Emit(OpCodes.Ldobj, parameterTypes[num5].GetElementType());
                        iLGenerator.Emit(OpCodes.Box, parameterTypes[num5].GetElementType());
                    }
                    else
                    {
                        iLGenerator.Emit(OpCodes.Ldind_Ref);
                    }
                }
                else if (parameterTypes[num5].IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Box, parameterTypes[num5]);
                }
                iLGenerator.Emit(OpCodes.Stelem_Ref);
                if (parameterTypes[num5].IsByRef)
                {
                    iLGenerator.Emit(OpCodes.Ldloc_2);
                    iLGenerator.Emit(OpCodes.Ldc_I4, num7);
                    iLGenerator.Emit(OpCodes.Ldc_I4, num5);
                    iLGenerator.Emit(OpCodes.Stelem_I4);
                    numArray[num7] = num5;
                    num7++;
                }
                if (parameters[num5].IsIn || !parameters[num5].IsOut)
                {
                    iLGenerator.Emit(OpCodes.Ldloc_1);
                    iLGenerator.Emit(OpCodes.Ldc_I4, num6);
                    iLGenerator.Emit(OpCodes.Ldarg, (int) (num5 + 1));
                    if (parameterTypes[num5].IsByRef)
                    {
                        if (parameterTypes[num5].GetElementType().IsValueType)
                        {
                            iLGenerator.Emit(OpCodes.Ldobj, parameterTypes[num5].GetElementType());
                            iLGenerator.Emit(OpCodes.Box, parameterTypes[num5].GetElementType());
                        }
                        else
                        {
                            iLGenerator.Emit(OpCodes.Ldind_Ref);
                        }
                    }
                    else if (parameterTypes[num5].IsValueType)
                    {
                        iLGenerator.Emit(OpCodes.Box, parameterTypes[num5]);
                    }
                    iLGenerator.Emit(OpCodes.Stelem_Ref);
                    num6++;
                }
                num5++;
            }
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, luaTableField);
            iLGenerator.Emit(OpCodes.Ldstr, method.Name);
            iLGenerator.Emit(OpCodes.Call, this.classHelper.GetMethod("getTableFunction"));
            Label label = iLGenerator.DefineLabel();
            iLGenerator.Emit(OpCodes.Dup);
            iLGenerator.Emit(OpCodes.Brtrue_S, label);
            iLGenerator.Emit(OpCodes.Pop);
            if (!method.IsAbstract)
            {
                iLGenerator.Emit(OpCodes.Ldarg_0);
                for (int m = 0; m < parameterTypes.Length; m++)
                {
                    iLGenerator.Emit(OpCodes.Ldarg, (int) (m + 1));
                }
                iLGenerator.Emit(OpCodes.Call, method);
                if (returnType == typeof(void))
                {
                    iLGenerator.Emit(OpCodes.Pop);
                }
                iLGenerator.Emit(OpCodes.Ret);
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            Label label2 = iLGenerator.DefineLabel();
            iLGenerator.Emit(OpCodes.Br_S, label2);
            iLGenerator.MarkLabel(label);
            iLGenerator.Emit(OpCodes.Ldloc_0);
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, returnTypesField);
            iLGenerator.Emit(OpCodes.Ldc_I4, methodIndex);
            iLGenerator.Emit(OpCodes.Ldelem_Ref);
            iLGenerator.Emit(OpCodes.Ldloc_1);
            iLGenerator.Emit(OpCodes.Ldloc_2);
            iLGenerator.Emit(OpCodes.Call, this.classHelper.GetMethod("callFunction"));
            iLGenerator.MarkLabel(label2);
            if (returnType == typeof(void))
            {
                iLGenerator.Emit(OpCodes.Pop);
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            else if (returnType.IsValueType)
            {
                iLGenerator.Emit(OpCodes.Unbox, returnType);
                iLGenerator.Emit(OpCodes.Ldobj, returnType);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Castclass, returnType);
            }
            iLGenerator.Emit(OpCodes.Stloc_3);
            for (int j = 0; j < numArray.Length; j++)
            {
                iLGenerator.Emit(OpCodes.Ldarg, (int) (numArray[j] + 1));
                iLGenerator.Emit(OpCodes.Ldloc_0);
                iLGenerator.Emit(OpCodes.Ldc_I4, numArray[j]);
                iLGenerator.Emit(OpCodes.Ldelem_Ref);
                if (parameterTypes[numArray[j]].GetElementType().IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Unbox, parameterTypes[numArray[j]].GetElementType());
                    iLGenerator.Emit(OpCodes.Ldobj, parameterTypes[numArray[j]].GetElementType());
                    iLGenerator.Emit(OpCodes.Stobj, parameterTypes[numArray[j]].GetElementType());
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Castclass, parameterTypes[numArray[j]].GetElementType());
                    iLGenerator.Emit(OpCodes.Stind_Ref);
                }
            }
            if (returnType != typeof(void))
            {
                iLGenerator.Emit(OpCodes.Ldloc_3);
            }
            iLGenerator.Emit(OpCodes.Ret);
        }

        public object GetClassInstance(Type klass, LuaTable luaTable)
        {
            LuaClassType type;
            if (this.classCollection.ContainsKey(klass))
            {
                type = this.classCollection[klass];
            }
            else
            {
                type = new LuaClassType();
                this.GenerateClass(klass, out type.klass, out type.returnTypes, luaTable);
                this.classCollection[klass] = type;
            }
            object[] args = new object[] { luaTable, type.returnTypes };
            return Activator.CreateInstance(type.klass, args);
        }

        public Delegate GetDelegate(Type delegateType, LuaFunction luaFunc)
        {
            Type type;
            List<Type> list = new List<Type>();
            if (this.delegateCollection.ContainsKey(delegateType))
            {
                type = this.delegateCollection[delegateType];
            }
            else
            {
                type = this.GenerateDelegate(delegateType);
                this.delegateCollection[delegateType] = type;
            }
            MethodInfo method = delegateType.GetMethod("Invoke");
            list.Add(method.ReturnType);
            foreach (ParameterInfo info2 in method.GetParameters())
            {
                if (info2.ParameterType.IsByRef)
                {
                    list.Add(info2.ParameterType);
                }
            }
            LuaDelegate target = (LuaDelegate) Activator.CreateInstance(type);
            target.function = luaFunc;
            target.returnTypes = list.ToArray();
            return Delegate.CreateDelegate(delegateType, target, "CallFunction");
        }

        public LuaEventHandler GetEvent(Type eventHandlerType, LuaFunction eventHandler)
        {
            Type type;
            if (this.eventHandlerCollection.ContainsKey(eventHandlerType))
            {
                type = this.eventHandlerCollection[eventHandlerType];
            }
            else
            {
                type = this.GenerateEvent(eventHandlerType);
                this.eventHandlerCollection[eventHandlerType] = type;
            }
            LuaEventHandler handler = (LuaEventHandler) Activator.CreateInstance(type);
            handler.handler = eventHandler;
            return handler;
        }

        public static CodeGeneration Instance
        {
            get
            {
                return instance;
            }
        }
    }
}

