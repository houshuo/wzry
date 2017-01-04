namespace com.tencent.pandora
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;

    public static class ToLuaExport
    {
        [CompilerGenerated]
        private static Predicate<MethodInfo> <>f__am$cache15;
        [CompilerGenerated]
        private static Predicate<MethodInfo> <>f__am$cache16;
        private static ObjAmbig ambig = ObjAmbig.NetObj;
        public static string baseClassName = null;
        private static BindingFlags binding = (BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
        public static string className = string.Empty;
        public static HashSet<System.Type> eventSet = new HashSet<System.Type>();
        public static string extendName = string.Empty;
        public static System.Type extendType = null;
        private static FieldInfo[] fields = null;
        public static bool isStaticClass = true;
        public static string libClassName = string.Empty;
        public static List<string> memberFilter = new List<string> { 
            "AnimationClip.averageDuration", "AnimationClip.averageAngularSpeed", "AnimationClip.averageSpeed", "AnimationClip.apparentSpeed", "AnimationClip.isLooping", "AnimationClip.isAnimatorMotion", "AnimationClip.isHumanMotion", "AnimatorOverrideController.PerformOverrideClipListCleanup", "Caching.SetNoBackupFlag", "Caching.ResetNoBackupFlag", "Light.areaSize", "Security.GetChainOfTrustValue", "Texture2D.alphaIsTransparency", "WWW.movie", "WebCamTexture.MarkNonReadable", "WebCamTexture.isReadable", 
            "Graphic.OnRebuildRequested", "Text.OnRebuildRequested", "UIInput.ProcessEvent", "UIWidget.showHandlesWithMoveTool", "UIWidget.showHandles", "Application.ExternalEval", "Resources.LoadAssetAtPath", "Input.IsJoystickPreconfigured", "String.Chars"
         };
        private static MethodInfo[] methods = null;
        private static Dictionary<string, int> nameCounter = null;
        private static MetaOp op = MetaOp.None;
        private static List<PropertyInfo> propList = new List<PropertyInfo>();
        private static PropertyInfo[] props = null;
        private static StringBuilder sb = null;
        public static System.Type type = null;
        private static Dictionary<System.Type, int> typeSize;
        private static HashSet<string> usingList = new HashSet<string>();
        public static string wrapClassName = string.Empty;

        static ToLuaExport()
        {
            Dictionary<System.Type, int> dictionary = new Dictionary<System.Type, int>();
            dictionary.Add(typeof(bool), 1);
            dictionary.Add(typeof(char), 2);
            dictionary.Add(typeof(byte), 3);
            dictionary.Add(typeof(sbyte), 4);
            dictionary.Add(typeof(ushort), 5);
            dictionary.Add(typeof(short), 6);
            dictionary.Add(typeof(uint), 7);
            dictionary.Add(typeof(int), 8);
            dictionary.Add(typeof(float), 9);
            dictionary.Add(typeof(ulong), 10);
            dictionary.Add(typeof(long), 11);
            dictionary.Add(typeof(double), 12);
            typeSize = dictionary;
        }

        public static string _C(string str)
        {
            if ((str.Length > 1) && (str[str.Length - 1] == '&'))
            {
                str = str.Remove(str.Length - 1);
            }
            if ((str == "System.Single") || (str == "Single"))
            {
                return "float";
            }
            if ((str == "System.String") || (str == "String"))
            {
                return "string";
            }
            if ((str == "System.Int32") || (str == "Int32"))
            {
                return "int";
            }
            if ((str == "System.Int64") || (str == "Int64"))
            {
                return "long";
            }
            if ((str == "System.SByte") || (str == "SByte"))
            {
                return "sbyte";
            }
            if ((str == "System.Byte") || (str == "Byte"))
            {
                return "byte";
            }
            if ((str == "System.Int16") || (str == "Int16"))
            {
                return "short";
            }
            if ((str == "System.UInt16") || (str == "UInt16"))
            {
                return "ushort";
            }
            if ((str == "System.Char") || (str == "Char"))
            {
                return "char";
            }
            if ((str == "System.UInt32") || (str == "UInt32"))
            {
                return "uint";
            }
            if ((str == "System.UInt64") || (str == "UInt64"))
            {
                return "ulong";
            }
            if ((str == "System.Decimal") || (str == "Decimal"))
            {
                return "decimal";
            }
            if ((str == "System.Double") || (str == "Double"))
            {
                return "double";
            }
            if ((str == "System.Boolean") || (str == "Boolean"))
            {
                return "bool";
            }
            if (str == "System.Object")
            {
                return "object";
            }
            if (str.Contains("."))
            {
                int length = str.LastIndexOf('.');
                string item = str.Substring(0, length);
                if ((str.Length > 12) && (str.Substring(0, 12) == "UnityEngine."))
                {
                    if (item == "UnityEngine")
                    {
                        usingList.Add("UnityEngine");
                    }
                    if (str == "UnityEngine.Object")
                    {
                        ambig |= ObjAmbig.U3dObj;
                    }
                }
                else if ((str.Length > 7) && (str.Substring(0, 7) == "System."))
                {
                    if (item == "System.Collections")
                    {
                        usingList.Add(item);
                    }
                    else if (item == "System.Collections.Generic")
                    {
                        usingList.Add(item);
                    }
                    else if (item == "System")
                    {
                        usingList.Add(item);
                    }
                    if (str == "System.Object")
                    {
                        str = "object";
                    }
                }
                if (usingList.Contains(item))
                {
                    str = str.Substring(length + 1);
                }
            }
            if (str.Contains("+"))
            {
                return str.Replace('+', '.');
            }
            if (str == extendName)
            {
                return GetTypeStr(type);
            }
            return str;
        }

        private static void CallOpFunction(string name, int count, string ret)
        {
            string str = string.Empty;
            for (int i = 0; i < count; i++)
            {
                str = str + "\t";
            }
            if (name == "op_Addition")
            {
                sb.AppendFormat("{0}{1} o = arg0 + arg1;\r\n", str, ret);
            }
            else if (name == "op_Subtraction")
            {
                sb.AppendFormat("{0}{1} o = arg0 - arg1;\r\n", str, ret);
            }
            else if (name == "op_Equality")
            {
                sb.AppendFormat("{0}bool o = arg0 == arg1;\r\n", str);
            }
            else if (name == "op_Multiply")
            {
                sb.AppendFormat("{0}{1} o = arg0 * arg1;\r\n", str, ret);
            }
            else if (name == "op_Division")
            {
                sb.AppendFormat("{0}{1} o = arg0 / arg1;\r\n", str, ret);
            }
            else if (name == "op_UnaryNegation")
            {
                sb.AppendFormat("{0}{1} o = -arg0;\r\n", str, ret);
            }
        }

        private static void CheckObjectNull()
        {
            if (type.IsValueType)
            {
                sb.AppendLine("\t\tif (o == null)");
            }
            else
            {
                sb.AppendLine("\t\tif (obj == null)");
            }
        }

        public static void Clear()
        {
            className = null;
            type = null;
            isStaticClass = false;
            baseClassName = null;
            usingList.Clear();
            op = MetaOp.None;
            sb = new StringBuilder();
            methods = null;
            fields = null;
            props = null;
            propList.Clear();
            ambig = ObjAmbig.NetObj;
            wrapClassName = string.Empty;
            libClassName = string.Empty;
        }

        private static int Compare(MethodBase lhs, MethodBase rhs)
        {
            int num = !lhs.IsStatic ? 1 : 0;
            int num2 = !rhs.IsStatic ? 1 : 0;
            ParameterInfo[] parameters = lhs.GetParameters();
            ParameterInfo[] infos = rhs.GetParameters();
            int optionalParamPos = GetOptionalParamPos(parameters);
            int index = GetOptionalParamPos(infos);
            if ((optionalParamPos >= 0) && (index < 0))
            {
                return 1;
            }
            if ((optionalParamPos < 0) && (index >= 0))
            {
                return -1;
            }
            if ((optionalParamPos >= 0) && (index >= 0))
            {
                optionalParamPos += num;
                index += num2;
                if (optionalParamPos != index)
                {
                    return ((optionalParamPos <= index) ? 1 : -1);
                }
                optionalParamPos -= num;
                index -= num2;
                if ((parameters[optionalParamPos].ParameterType.GetElementType() == typeof(object)) && (infos[index].ParameterType.GetElementType() != typeof(object)))
                {
                    return 1;
                }
                if ((parameters[optionalParamPos].ParameterType.GetElementType() != typeof(object)) && (infos[index].ParameterType.GetElementType() == typeof(object)))
                {
                    return -1;
                }
            }
            int num5 = num + parameters.Length;
            int num6 = num2 + infos.Length;
            if (num5 > num6)
            {
                return 1;
            }
            if (num5 != num6)
            {
                return -1;
            }
            List<ParameterInfo> list = new List<ParameterInfo>(parameters);
            List<ParameterInfo> list2 = new List<ParameterInfo>(infos);
            if (list.Count > list2.Count)
            {
                if (list[0].ParameterType == typeof(object))
                {
                    return 1;
                }
                list.RemoveAt(0);
            }
            else if (list2.Count > list.Count)
            {
                if (list2[0].ParameterType == typeof(object))
                {
                    return -1;
                }
                list2.RemoveAt(0);
            }
            for (int i = 0; i < list.Count; i++)
            {
                if ((list[i].ParameterType == typeof(object)) && (list2[i].ParameterType != typeof(object)))
                {
                    return 1;
                }
                if ((list[i].ParameterType != typeof(object)) && (list2[i].ParameterType == typeof(object)))
                {
                    return -1;
                }
            }
            return 0;
        }

        private static int CompareMethod(MethodBase l, MethodBase r)
        {
            int num = 0;
            if (!CompareParmsCount(l, r))
            {
                return -1;
            }
            ParameterInfo[] parameters = l.GetParameters();
            ParameterInfo[] infoArray2 = r.GetParameters();
            List<System.Type> list = new List<System.Type>();
            List<System.Type> list2 = new List<System.Type>();
            if (!l.IsStatic)
            {
                list.Add(type);
            }
            if (!r.IsStatic)
            {
                list2.Add(type);
            }
            for (int i = 0; i < parameters.Length; i++)
            {
                list.Add(parameters[i].ParameterType);
            }
            for (int j = 0; j < infoArray2.Length; j++)
            {
                list2.Add(infoArray2[j].ParameterType);
            }
            for (int k = 0; k < list.Count; k++)
            {
                if (!typeSize.ContainsKey(list[k]) || !typeSize.ContainsKey(list2[k]))
                {
                    if (list[k] != list2[k])
                    {
                        return -1;
                    }
                }
                else if ((list[k].IsPrimitive && list2[k].IsPrimitive) && (num == 0))
                {
                    num = (typeSize[list[k]] < typeSize[list2[k]]) ? 2 : 1;
                }
                else if (list[k] != list2[k])
                {
                    return -1;
                }
            }
            if ((num == 0) && l.IsStatic)
            {
                num = 2;
            }
            return num;
        }

        private static bool CompareParmsCount(MethodBase l, MethodBase r)
        {
            if (l == r)
            {
                return false;
            }
            int num = !l.IsStatic ? 1 : 0;
            int num2 = !r.IsStatic ? 1 : 0;
            num += l.GetParameters().Length;
            num2 += r.GetParameters().Length;
            return (num == num2);
        }

        private static void DefaultConstruct()
        {
            sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
            sb.AppendFormat("\tstatic int _Create{0}(IntPtr L)\r\n", wrapClassName);
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tLuaScriptMgr.CheckArgsCount(L, 0);");
            sb.AppendFormat("\t\t{0} obj = new {0}();\r\n", className);
            string pushFunction = GetPushFunction(type);
            sb.AppendFormat("\t\tLuaScriptMgr.{0}(L, obj);\r\n", pushFunction);
            sb.AppendLine("\t\treturn 1;");
            sb.AppendLine("\t}");
        }

        private static void GenBaseOpFunction(List<MethodInfo> list)
        {
            for (System.Type type = ToLuaExport.type.BaseType; type != null; type = type.BaseType)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase);
                for (int i = 0; i < methods.Length; i++)
                {
                    MetaOp op = GetOp(methods[i].Name);
                    if ((op != MetaOp.None) && ((ToLuaExport.op & op) == MetaOp.None))
                    {
                        list.Add(methods[i]);
                        ToLuaExport.op |= op;
                    }
                }
            }
        }

        private static void GenConstruct()
        {
            <GenConstruct>c__AnonStorey42 storey = new <GenConstruct>c__AnonStorey42();
            if (isStaticClass || typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                NoConsturct();
            }
            else
            {
                ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Instance | binding);
                if (extendType != null)
                {
                    ConstructorInfo[] infoArray2 = extendType.GetConstructors(BindingFlags.Instance | binding);
                    if (((infoArray2 != null) && (infoArray2.Length > 0)) && HasAttribute(infoArray2[0], typeof(UseDefinedAttribute)))
                    {
                        sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
                        sb.AppendFormat("\tstatic int _Create{0}(IntPtr L)\r\n", wrapClassName);
                        sb.AppendLine("\t{");
                        if (HasAttribute(infoArray2[0], typeof(UseDefinedAttribute)))
                        {
                            string str = extendType.GetField(extendName + "Defined").GetValue(null) as string;
                            sb.AppendLine(str);
                            sb.AppendLine("\t}");
                            return;
                        }
                    }
                }
                if (constructors.Length == 0)
                {
                    if (!type.IsValueType)
                    {
                        NoConsturct();
                    }
                    else
                    {
                        DefaultConstruct();
                    }
                }
                else
                {
                    storey.list = new List<ConstructorInfo>();
                    for (int i = 0; i < constructors.Length; i++)
                    {
                        <GenConstruct>c__AnonStorey41 storey2 = new <GenConstruct>c__AnonStorey41();
                        if (!HasDecimal(constructors[i].GetParameters()) && !IsObsolete(constructors[i]))
                        {
                            storey2.r = constructors[i];
                            int index = storey.list.FindIndex(new Predicate<ConstructorInfo>(storey2.<>m__18));
                            if (index >= 0)
                            {
                                if (CompareMethod(storey.list[index], storey2.r) == 2)
                                {
                                    storey.list.RemoveAt(index);
                                    storey.list.Add(storey2.r);
                                }
                            }
                            else
                            {
                                storey.list.Add(storey2.r);
                            }
                        }
                    }
                    if (storey.list.Count == 0)
                    {
                        if (!type.IsValueType)
                        {
                            NoConsturct();
                        }
                        else
                        {
                            DefaultConstruct();
                        }
                    }
                    else
                    {
                        storey.list.Sort(new Comparison<ConstructorInfo>(ToLuaExport.Compare));
                        sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
                        sb.AppendFormat("\tstatic int _Create{0}(IntPtr L)\r\n", wrapClassName);
                        sb.AppendLine("\t{");
                        sb.AppendLine("\t\tint count = LuaDLL.lua_gettop(L);");
                        sb.AppendLine();
                        List<ConstructorInfo> list = new List<ConstructorInfo>();
                        <GenConstruct>c__AnonStorey43 storey3 = new <GenConstruct>c__AnonStorey43 {
                            <>f__ref$66 = storey,
                            i = 0
                        };
                        while (storey3.i < storey.list.Count)
                        {
                            if ((storey.list.FindIndex(new Predicate<ConstructorInfo>(storey3.<>m__19)) >= 0) || (HasOptionalParam(storey.list[storey3.i].GetParameters()) && (storey.list[storey3.i].GetParameters().Length > 1)))
                            {
                                list.Add(storey.list[storey3.i]);
                            }
                            storey3.i++;
                        }
                        MethodBase md = storey.list[0];
                        bool flag = storey.list[0].GetParameters().Length == 0;
                        if (HasOptionalParam(md.GetParameters()))
                        {
                            ParameterInfo[] parameters = md.GetParameters();
                            ParameterInfo info2 = parameters[parameters.Length - 1];
                            string typeStr = GetTypeStr(info2.ParameterType.GetElementType());
                            if (parameters.Length > 1)
                            {
                                string str3 = GenParamTypes(parameters, true);
                                object[] args = new object[] { str3, typeStr, parameters.Length, GetCountStr(parameters.Length - 1) };
                                sb.AppendFormat("\t\tif (LuaScriptMgr.CheckTypes(L, 1, {0}) && LuaScriptMgr.CheckParamsType(L, typeof({1}), {2}, {3}))\r\n", args);
                            }
                            else
                            {
                                sb.AppendFormat("\t\tif (LuaScriptMgr.CheckParamsType(L, typeof({0}), {1}, {2}))\r\n", typeStr, parameters.Length, GetCountStr(parameters.Length - 1));
                            }
                        }
                        else
                        {
                            ParameterInfo[] p = md.GetParameters();
                            if ((storey.list.Count == 1) || (md.GetParameters().Length != storey.list[1].GetParameters().Length))
                            {
                                sb.AppendFormat("\t\tif (count == {0})\r\n", p.Length);
                            }
                            else
                            {
                                string str4 = GenParamTypes(p, true);
                                sb.AppendFormat("\t\tif (count == {0} && LuaScriptMgr.CheckTypes(L, 1, {1}))\r\n", p.Length, str4);
                            }
                        }
                        sb.AppendLine("\t\t{");
                        int num4 = ProcessParams(md, 3, true, storey.list.Count > 1, false);
                        sb.AppendFormat("\t\t\treturn {0};\r\n", num4);
                        sb.AppendLine("\t\t}");
                        for (int j = 1; j < storey.list.Count; j++)
                        {
                            flag = (storey.list[j].GetParameters().Length != 0) ? flag : true;
                            md = storey.list[j];
                            ParameterInfo[] infoArray5 = md.GetParameters();
                            if (!HasOptionalParam(md.GetParameters()))
                            {
                                if (list.Contains(storey.list[j]))
                                {
                                    string str5 = GenParamTypes(infoArray5, true);
                                    sb.AppendFormat("\t\telse if (count == {0} && LuaScriptMgr.CheckTypes(L, 1, {1}))\r\n", infoArray5.Length, str5);
                                }
                                else
                                {
                                    sb.AppendFormat("\t\telse if (count == {0})\r\n", infoArray5.Length);
                                }
                            }
                            else
                            {
                                ParameterInfo info3 = infoArray5[infoArray5.Length - 1];
                                string str6 = GetTypeStr(info3.ParameterType.GetElementType());
                                if (infoArray5.Length > 1)
                                {
                                    string str7 = GenParamTypes(infoArray5, true);
                                    object[] objArray2 = new object[] { str7, str6, infoArray5.Length, GetCountStr(infoArray5.Length - 1) };
                                    sb.AppendFormat("\t\telse if (LuaScriptMgr.CheckTypes(L, 1, {0}) && LuaScriptMgr.CheckParamsType(L, typeof({1}), {2}, {3}))\r\n", objArray2);
                                }
                                else
                                {
                                    sb.AppendFormat("\t\telse if (LuaScriptMgr.CheckParamsType(L, typeof({0}), {1}, {2}))\r\n", str6, infoArray5.Length, GetCountStr(infoArray5.Length - 1));
                                }
                            }
                            sb.AppendLine("\t\t{");
                            num4 = ProcessParams(md, 3, true, true, false);
                            sb.AppendFormat("\t\t\treturn {0};\r\n", num4);
                            sb.AppendLine("\t\t}");
                        }
                        if (type.IsValueType && !flag)
                        {
                            sb.AppendLine("\t\telse if (count == 0)");
                            sb.AppendLine("\t\t{");
                            sb.AppendFormat("\t\t\t{0} obj = new {0}();\r\n", className);
                            string pushFunction = GetPushFunction(type);
                            sb.AppendFormat("\t\t\tLuaScriptMgr.{0}(L, obj);\r\n", pushFunction);
                            sb.AppendLine("\t\t\treturn 1;");
                            sb.AppendLine("\t\t}");
                        }
                        sb.AppendLine("\t\telse");
                        sb.AppendLine("\t\t{");
                        sb.AppendFormat("\t\t\tLuaDLL.luaL_error(L, \"invalid arguments to method: {0}.New\");\r\n", className);
                        sb.AppendLine("\t\t}");
                        sb.AppendLine();
                        sb.AppendLine("\t\treturn 0;");
                        sb.AppendLine("\t}");
                    }
                }
            }
        }

        private static void GenDelegateBody(System.Type t, string head, bool haveState)
        {
            eventSet.Add(t);
            MethodInfo method = t.GetMethod("Invoke");
            ParameterInfo[] parameters = method.GetParameters();
            int length = parameters.Length;
            if (length == 0)
            {
                sb.AppendLine("() =>");
                if (method.ReturnType == typeof(void))
                {
                    sb.AppendFormat("{0}{{\r\n{0}\tfunc.Call();\r\n{0}}};\r\n", head);
                }
                else
                {
                    sb.AppendFormat("{0}{{\r\n{0}\tobject[] objs = func.Call();\r\n", head);
                    sb.AppendFormat("{1}\treturn ({0})objs[0];\r\n", GetTypeStr(method.ReturnType), head);
                    sb.AppendFormat("{0}}};\r\n", head);
                }
            }
            else
            {
                sb.AppendFormat("(param0", new object[0]);
                for (int i = 1; i < length; i++)
                {
                    sb.AppendFormat(", param{0}", i);
                }
                sb.AppendFormat(") =>\r\n{0}{{\r\n{0}", head);
                sb.AppendLine("\tint top = func.BeginPCall();");
                if (!haveState)
                {
                    sb.AppendFormat("{0}\tIntPtr L = func.GetLuaState();\r\n", head);
                }
                for (int j = 0; j < length; j++)
                {
                    string pushFunction = GetPushFunction(parameters[j].ParameterType);
                    sb.AppendFormat("{2}\tLuaScriptMgr.{0}(L, param{1});\r\n", pushFunction, j, head);
                }
                sb.AppendFormat("{1}\tfunc.PCall(top, {0});\r\n", length, head);
                if (method.ReturnType == typeof(void))
                {
                    sb.AppendFormat("{0}\tfunc.EndPCall(top);\r\n", head);
                }
                else
                {
                    sb.AppendFormat("{0}\tobject[] objs = func.PopValues(top);\r\n", head);
                    sb.AppendFormat("{0}\tfunc.EndPCall(top);\r\n", head);
                    sb.AppendFormat("{1}\treturn ({0})objs[0];\r\n", GetTypeStr(method.ReturnType), head);
                }
                sb.AppendFormat("{0}}};\r\n", head);
            }
        }

        public static void GenDelegates(DelegateType[] list)
        {
            usingList.Add("System");
            usingList.Add("System.Collections.Generic");
            for (int i = 0; i < list.Length; i++)
            {
                System.Type c = list[i].type;
                if (!typeof(Delegate).IsAssignableFrom(c))
                {
                    return;
                }
            }
            sb.AppendLine("namespace com.tencent.pandora");
            sb.AppendLine("{");
            sb.AppendLine("public static class DelegateFactory");
            sb.AppendLine("{");
            sb.AppendLine("\tdelegate Delegate DelegateValue(LuaFunction func);");
            sb.AppendLine("\tstatic Dictionary<Type, DelegateValue> dict = new Dictionary<Type, DelegateValue>();");
            sb.AppendLine();
            sb.AppendLine("\t[NoToLuaAttribute]");
            sb.AppendLine("\tpublic static void Register(IntPtr L)");
            sb.AppendLine("\t{");
            for (int j = 0; j < list.Length; j++)
            {
                string strType = list[j].strType;
                string name = list[j].name;
                sb.AppendFormat("\t\tdict.Add(typeof({0}), new DelegateValue({1}));\r\n", strType, name);
            }
            sb.AppendLine("\t}\r\n");
            sb.AppendLine("\t[NoToLuaAttribute]");
            sb.AppendLine("\tpublic static Delegate CreateDelegate(Type t, LuaFunction func)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tDelegateValue create = null;\r\n");
            sb.AppendLine("\t\tif (!dict.TryGetValue(t, out create))");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tDebugger.LogError(\"Delegate {0} not register\", t.FullName);");
            sb.AppendLine("\t\t\treturn null;");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t\treturn create(func);");
            sb.AppendLine("\t}\r\n");
            for (int k = 0; k < list.Length; k++)
            {
                System.Type type = list[k].type;
                string str3 = list[k].strType;
                string str4 = list[k].name;
                sb.AppendFormat("\tpublic static Delegate {0}(LuaFunction func)\r\n", str4);
                sb.AppendLine("\t{");
                sb.AppendFormat("\t\t{0} d = ", str3);
                GenDelegateBody(type, "\t\t", false);
                sb.AppendLine("\t\treturn d;");
                sb.AppendLine("\t}\r\n");
            }
            sb.AppendLine("\tpublic static void Clear()");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tdict.Clear();");
            sb.AppendLine("\t}\r\n");
            sb.AppendLine("}");
            sb.AppendLine("}");
            SaveFile(AppConst.LuaBasePath + "Base/DelegateFactory.cs");
            Clear();
        }

        private static void GenEnum()
        {
            fields = type.GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static);
            List<FieldInfo> list = new List<FieldInfo>(fields);
            for (int i = list.Count - 1; i > 0; i--)
            {
                if (IsObsolete(list[i]))
                {
                    list.RemoveAt(i);
                }
            }
            fields = list.ToArray();
            sb.AppendFormat("public class {0}Wrap\r\n", wrapClassName);
            sb.AppendLine("{");
            sb.AppendLine("\tstatic LuaMethod[] enums = new LuaMethod[]");
            sb.AppendLine("\t{");
            for (int j = 0; j < fields.Length; j++)
            {
                sb.AppendFormat("\t\tnew LuaMethod(\"{0}\", Get{0}),\r\n", fields[j].Name);
            }
            sb.AppendFormat("\t\tnew LuaMethod(\"IntToEnum\", IntToEnum),\r\n", new object[0]);
            sb.AppendLine("\t};");
            sb.AppendLine("\r\n\tpublic static void Register(IntPtr L)");
            sb.AppendLine("\t{");
            sb.AppendFormat("\t\tLuaScriptMgr.RegisterLib(L, \"{0}\", typeof({0}), enums);\r\n", libClassName);
            sb.AppendLine("\t}");
            for (int k = 0; k < fields.Length; k++)
            {
                sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
                sb.AppendFormat("\tstatic int Get{0}(IntPtr L)\r\n", fields[k].Name);
                sb.AppendLine("\t{");
                sb.AppendFormat("\t\tLuaScriptMgr.Push(L, {0}.{1});\r\n", className, fields[k].Name);
                sb.AppendLine("\t\treturn 1;");
                sb.AppendLine("\t}");
            }
        }

        private static void GenEnumTranslator()
        {
            sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
            sb.AppendLine("\tstatic int IntToEnum(IntPtr L)");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tint arg0 = (int)LuaDLL.lua_tonumber(L, 1);");
            sb.AppendFormat("\t\t{0} o = ({0})arg0;\r\n", className);
            sb.AppendLine("\t\tLuaScriptMgr.Push(L, o);");
            sb.AppendLine("\t\treturn 1;");
            sb.AppendLine("\t}");
        }

        public static void Generate(params string[] param)
        {
            <Generate>c__AnonStorey3F storeyf = new <Generate>c__AnonStorey3F();
            object[] args = new object[] { className };
            Debugger.Log("Begin Generate lua Wrap for class {0}\r\n", args);
            if (className == "CNewsData")
            {
            }
            sb = new StringBuilder();
            usingList.Add("System");
            GetTypeStr(type);
            if (wrapClassName == string.Empty)
            {
                wrapClassName = className;
            }
            if (libClassName == string.Empty)
            {
                libClassName = className;
            }
            if (type.IsEnum)
            {
                GenEnum();
                GenEnumTranslator();
                sb.AppendLine("}\r\n");
                SaveFile(AppConst.LuaWrapPath + wrapClassName + "Wrap.cs");
            }
            else
            {
                nameCounter = new Dictionary<string, int>();
                List<MethodInfo> list = new List<MethodInfo>();
                if (baseClassName != null)
                {
                    binding |= BindingFlags.DeclaredOnly;
                }
                else if ((baseClassName == null) && isStaticClass)
                {
                    binding |= BindingFlags.DeclaredOnly;
                }
                if (type.IsInterface)
                {
                    list.AddRange(type.GetMethods());
                }
                else
                {
                    list.AddRange(type.GetMethods(BindingFlags.Instance | binding));
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        if ((list[i].Name.Contains("op_") || list[i].Name.Contains("add_")) || list[i].Name.Contains("remove_"))
                        {
                            if (!IsNeedOp(list[i].Name))
                            {
                                list.RemoveAt(i);
                            }
                        }
                        else if (IsObsolete(list[i]))
                        {
                            list.RemoveAt(i);
                        }
                    }
                }
                storeyf.ps = type.GetProperties();
                <Generate>c__AnonStorey40 storey = new <Generate>c__AnonStorey40 {
                    <>f__ref$63 = storeyf,
                    i = 0
                };
                while (storey.i < storeyf.ps.Length)
                {
                    int index = list.FindIndex(new Predicate<MethodInfo>(storey.<>m__15));
                    if ((index >= 0) && (list[index].Name != "get_Item"))
                    {
                        list.RemoveAt(index);
                    }
                    index = list.FindIndex(new Predicate<MethodInfo>(storey.<>m__16));
                    if ((index >= 0) && (list[index].Name != "set_Item"))
                    {
                        list.RemoveAt(index);
                    }
                    storey.i++;
                }
                ProcessExtends(list);
                GenBaseOpFunction(list);
                methods = list.ToArray();
                sb.AppendFormat("public class {0}Wrap\r\n", wrapClassName);
                sb.AppendLine("{");
                GenRegFunc();
                GenConstruct();
                GenGetType();
                GenIndexFunc();
                GenNewIndexFunc();
                GenToStringFunc();
                GenFunction();
                sb.AppendLine("}\r\n");
                string luaWrapPath = AppConst.LuaWrapPath;
                if (!Directory.Exists(luaWrapPath))
                {
                    Directory.CreateDirectory(luaWrapPath);
                }
                SaveFile(luaWrapPath + wrapClassName + "Wrap.cs");
            }
        }

        private static void GenFunction()
        {
            HashSet<string> set = new HashSet<string>();
            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo mb = methods[i];
                if (mb.IsGenericMethod)
                {
                    object[] args = new object[] { mb.Name };
                    Debugger.Log("Generic Method {0} cannot be export to lua", args);
                }
                else
                {
                    if (nameCounter[mb.Name] > 1)
                    {
                        if (set.Contains(mb.Name))
                        {
                            goto Label_0240;
                        }
                        MethodInfo info2 = GenOverrideFunc(mb.Name);
                        if (info2 == null)
                        {
                            set.Add(mb.Name);
                            goto Label_0240;
                        }
                        mb = info2;
                    }
                    set.Add(mb.Name);
                    sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
                    sb.AppendFormat("\tstatic int {0}(IntPtr L)\r\n", GetFuncName(mb.Name));
                    sb.AppendLine("\t{");
                    if (HasAttribute(mb, typeof(OnlyGCAttribute)))
                    {
                        sb.AppendLine("\t\tLuaScriptMgr.__gc(L);");
                        sb.AppendLine("\t\treturn 0;");
                        sb.AppendLine("\t}");
                    }
                    else if (HasAttribute(mb, typeof(UseDefinedAttribute)))
                    {
                        string str = extendType.GetField(mb.Name + "Defined").GetValue(null) as string;
                        sb.AppendLine(str);
                        sb.AppendLine("\t}");
                    }
                    else
                    {
                        ParameterInfo[] parameters = mb.GetParameters();
                        int num2 = !mb.IsStatic ? 2 : 1;
                        if (!HasOptionalParam(parameters))
                        {
                            int num3 = (parameters.Length + num2) - 1;
                            sb.AppendFormat("\t\tLuaScriptMgr.CheckArgsCount(L, {0});\r\n", num3);
                        }
                        else
                        {
                            sb.AppendLine("\t\tint count = LuaDLL.lua_gettop(L);");
                        }
                        int num4 = (mb.ReturnType != typeof(void)) ? 1 : 0;
                        num4 += ProcessParams(mb, 2, false, false, false);
                        sb.AppendFormat("\t\treturn {0};\r\n", num4);
                        sb.AppendLine("\t}");
                    }
                Label_0240:;
                }
            }
        }

        private static void GenGetType()
        {
            sb.AppendFormat("\r\n\tstatic Type classType = typeof({0});\r\n", className);
            sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
            sb.AppendFormat("\tstatic int {0}(IntPtr L)\r\n", "GetClassType");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tLuaScriptMgr.Push(L, classType);");
            sb.AppendLine("\t\treturn 1;");
            sb.AppendLine("\t}");
        }

        private static void GenIndexFunc()
        {
            for (int i = 0; i < fields.Length; i++)
            {
                sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
                sb.AppendFormat("\tstatic int get_{0}(IntPtr L)\r\n", fields[i].Name);
                sb.AppendLine("\t{");
                string pushFunction = GetPushFunction(fields[i].FieldType);
                if (fields[i].IsStatic)
                {
                    sb.AppendFormat("\t\tLuaScriptMgr.{2}(L, {0}.{1});\r\n", className, fields[i].Name, pushFunction);
                }
                else
                {
                    sb.AppendFormat("\t\tobject o = LuaScriptMgr.GetLuaObject(L, 1);\r\n", new object[0]);
                    if (!type.IsValueType)
                    {
                        sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", className);
                    }
                    sb.AppendLine();
                    CheckObjectNull();
                    sb.AppendLine("\t\t{");
                    sb.AppendLine("\t\t\tLuaTypes types = LuaDLL.lua_type(L, 1);");
                    sb.AppendLine();
                    sb.AppendLine("\t\t\tif (types == LuaTypes.LUA_TTABLE)");
                    sb.AppendLine("\t\t\t{");
                    sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"unknown member name {0}\");\r\n", fields[i].Name);
                    sb.AppendLine("\t\t\t}");
                    sb.AppendLine("\t\t\telse");
                    sb.AppendLine("\t\t\t{");
                    sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"attempt to index {0} on a nil value\");\r\n", fields[i].Name);
                    sb.AppendLine("\t\t\t}");
                    sb.AppendLine("\t\t}");
                    sb.AppendLine();
                    if (type.IsValueType)
                    {
                        sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", className);
                    }
                    sb.AppendFormat("\t\tLuaScriptMgr.{1}(L, obj.{0});\r\n", fields[i].Name, pushFunction);
                }
                sb.AppendLine("\t\treturn 1;");
                sb.AppendLine("\t}");
            }
            for (int j = 0; j < props.Length; j++)
            {
                if (props[j].CanRead)
                {
                    bool flag = true;
                    if (propList.IndexOf(props[j]) >= 0)
                    {
                        flag = false;
                    }
                    sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
                    sb.AppendFormat("\tstatic int get_{0}(IntPtr L)\r\n", props[j].Name);
                    sb.AppendLine("\t{");
                    string str2 = GetPushFunction(props[j].PropertyType);
                    if (flag)
                    {
                        sb.AppendFormat("\t\tLuaScriptMgr.{2}(L, {0}.{1});\r\n", className, props[j].Name, str2);
                    }
                    else
                    {
                        sb.AppendFormat("\t\tobject o = LuaScriptMgr.GetLuaObject(L, 1);\r\n", new object[0]);
                        if (!type.IsValueType)
                        {
                            sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", className);
                        }
                        sb.AppendLine();
                        CheckObjectNull();
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\tLuaTypes types = LuaDLL.lua_type(L, 1);");
                        sb.AppendLine();
                        sb.AppendLine("\t\t\tif (types == LuaTypes.LUA_TTABLE)");
                        sb.AppendLine("\t\t\t{");
                        sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"unknown member name {0}\");\r\n", props[j].Name);
                        sb.AppendLine("\t\t\t}");
                        sb.AppendLine("\t\t\telse");
                        sb.AppendLine("\t\t\t{");
                        sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"attempt to index {0} on a nil value\");\r\n", props[j].Name);
                        sb.AppendLine("\t\t\t}");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine();
                        if (type.IsValueType)
                        {
                            sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", className);
                        }
                        sb.AppendFormat("\t\tLuaScriptMgr.{1}(L, obj.{0});\r\n", props[j].Name, str2);
                    }
                    sb.AppendLine("\t\treturn 1;");
                    sb.AppendLine("\t}");
                }
            }
        }

        private static void GenLuaFields()
        {
            fields = type.GetFields((BindingFlags.SetField | BindingFlags.GetField | BindingFlags.Instance) | binding);
            props = type.GetProperties((BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.Instance) | binding);
            propList.AddRange(type.GetProperties(BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
            List<FieldInfo> list = new List<FieldInfo>();
            list.AddRange(fields);
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (IsObsolete(list[i]))
                {
                    list.RemoveAt(i);
                }
            }
            fields = list.ToArray();
            List<PropertyInfo> list2 = new List<PropertyInfo>();
            list2.AddRange(props);
            for (int j = list2.Count - 1; j >= 0; j--)
            {
                if ((list2[j].Name == "Item") || IsObsolete(list2[j]))
                {
                    list2.RemoveAt(j);
                }
            }
            props = list2.ToArray();
            for (int k = propList.Count - 1; k >= 0; k--)
            {
                if ((propList[k].Name == "Item") || IsObsolete(propList[k]))
                {
                    propList.RemoveAt(k);
                }
            }
            if (((fields.Length != 0) || (props.Length != 0)) || (!isStaticClass || (baseClassName != null)))
            {
                sb.AppendLine("\t\tLuaField[] fields = new LuaField[]");
                sb.AppendLine("\t\t{");
                for (int m = 0; m < fields.Length; m++)
                {
                    if ((fields[m].IsLiteral || fields[m].IsPrivate) || fields[m].IsInitOnly)
                    {
                        sb.AppendFormat("\t\t\tnew LuaField(\"{0}\", get_{0}, null),\r\n", fields[m].Name);
                    }
                    else
                    {
                        sb.AppendFormat("\t\t\tnew LuaField(\"{0}\", get_{0}, set_{0}),\r\n", fields[m].Name);
                    }
                }
                for (int n = 0; n < props.Length; n++)
                {
                    if ((props[n].CanRead && props[n].CanWrite) && props[n].GetSetMethod(true).IsPublic)
                    {
                        sb.AppendFormat("\t\t\tnew LuaField(\"{0}\", get_{0}, set_{0}),\r\n", props[n].Name);
                    }
                    else if (props[n].CanRead)
                    {
                        sb.AppendFormat("\t\t\tnew LuaField(\"{0}\", get_{0}, null),\r\n", props[n].Name);
                    }
                    else if (props[n].CanWrite)
                    {
                        sb.AppendFormat("\t\t\tnew LuaField(\"{0}\", null, set_{0}),\r\n", props[n].Name);
                    }
                }
                sb.AppendLine("\t\t};\r\n");
            }
        }

        private static void GenLuaMethods()
        {
            sb.AppendLine("\t\tLuaMethod[] regs = new LuaMethod[]");
            sb.AppendLine("\t\t{");
            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo info = methods[i];
                int num2 = 1;
                if (!info.IsGenericMethod)
                {
                    if (!nameCounter.TryGetValue(info.Name, out num2))
                    {
                        if (!info.Name.Contains("op_"))
                        {
                            sb.AppendFormat("\t\t\tnew LuaMethod(\"{0}\", {0}),\r\n", info.Name);
                        }
                        nameCounter[info.Name] = 1;
                    }
                    else
                    {
                        nameCounter[info.Name] = num2 + 1;
                    }
                }
            }
            sb.AppendFormat("\t\t\tnew LuaMethod(\"New\", _Create{0}),\r\n", wrapClassName);
            sb.AppendLine("\t\t\tnew LuaMethod(\"GetClassType\", GetClassType),");
            if (<>f__am$cache15 == null)
            {
                <>f__am$cache15 = p => p.Name == "ToString";
            }
            if ((Array.FindIndex<MethodInfo>(methods, <>f__am$cache15) >= 0) && !isStaticClass)
            {
                sb.AppendLine("\t\t\tnew LuaMethod(\"__tostring\", Lua_ToString),");
            }
            GenOperatorReg();
            sb.AppendLine("\t\t};\r\n");
        }

        private static void GenNewIndexFunc()
        {
            for (int i = 0; i < fields.Length; i++)
            {
                if ((!fields[i].IsLiteral && !fields[i].IsInitOnly) && !fields[i].IsPrivate)
                {
                    sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
                    sb.AppendFormat("\tstatic int set_{0}(IntPtr L)\r\n", fields[i].Name);
                    sb.AppendLine("\t{");
                    string o = !fields[i].IsStatic ? "obj" : className;
                    if (!fields[i].IsStatic)
                    {
                        sb.AppendFormat("\t\tobject o = LuaScriptMgr.GetLuaObject(L, 1);\r\n", new object[0]);
                        if (!type.IsValueType)
                        {
                            sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", className);
                        }
                        sb.AppendLine();
                        CheckObjectNull();
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\tLuaTypes types = LuaDLL.lua_type(L, 1);");
                        sb.AppendLine();
                        sb.AppendLine("\t\t\tif (types == LuaTypes.LUA_TTABLE)");
                        sb.AppendLine("\t\t\t{");
                        sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"unknown member name {0}\");\r\n", fields[i].Name);
                        sb.AppendLine("\t\t\t}");
                        sb.AppendLine("\t\t\telse");
                        sb.AppendLine("\t\t\t{");
                        sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"attempt to index {0} on a nil value\");\r\n", fields[i].Name);
                        sb.AppendLine("\t\t\t}");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine();
                        if (type.IsValueType)
                        {
                            sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", className);
                        }
                    }
                    NewIndexSetValue(fields[i].FieldType, o, fields[i].Name);
                    if (!fields[i].IsStatic && type.IsValueType)
                    {
                        sb.AppendLine("\t\tLuaScriptMgr.SetValueObject(L, 1, obj);");
                    }
                    sb.AppendLine("\t\treturn 0;");
                    sb.AppendLine("\t}");
                }
            }
            for (int j = 0; j < props.Length; j++)
            {
                if (props[j].CanWrite && props[j].GetSetMethod(true).IsPublic)
                {
                    bool flag = true;
                    if (propList.IndexOf(props[j]) >= 0)
                    {
                        flag = false;
                    }
                    sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
                    sb.AppendFormat("\tstatic int set_{0}(IntPtr L)\r\n", props[j].Name);
                    sb.AppendLine("\t{");
                    string str2 = !flag ? "obj" : className;
                    if (!flag)
                    {
                        sb.AppendFormat("\t\tobject o = LuaScriptMgr.GetLuaObject(L, 1);\r\n", new object[0]);
                        if (!type.IsValueType)
                        {
                            sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", className);
                        }
                        sb.AppendLine();
                        CheckObjectNull();
                        sb.AppendLine("\t\t{");
                        sb.AppendLine("\t\t\tLuaTypes types = LuaDLL.lua_type(L, 1);");
                        sb.AppendLine();
                        sb.AppendLine("\t\t\tif (types == LuaTypes.LUA_TTABLE)");
                        sb.AppendLine("\t\t\t{");
                        sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"unknown member name {0}\");\r\n", props[j].Name);
                        sb.AppendLine("\t\t\t}");
                        sb.AppendLine("\t\t\telse");
                        sb.AppendLine("\t\t\t{");
                        sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"attempt to index {0} on a nil value\");\r\n", props[j].Name);
                        sb.AppendLine("\t\t\t}");
                        sb.AppendLine("\t\t}");
                        sb.AppendLine();
                        if (type.IsValueType)
                        {
                            sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", className);
                        }
                    }
                    NewIndexSetValue(props[j].PropertyType, str2, props[j].Name);
                    if (!flag && type.IsValueType)
                    {
                        sb.AppendLine("\t\tLuaScriptMgr.SetValueObject(L, 1, obj);");
                    }
                    sb.AppendLine("\t\treturn 0;");
                    sb.AppendLine("\t}");
                }
            }
        }

        private static void GenOperatorReg()
        {
            if ((op & MetaOp.Add) != MetaOp.None)
            {
                sb.AppendLine("\t\t\tnew LuaMethod(\"__add\", Lua_Add),");
            }
            if ((op & MetaOp.Sub) != MetaOp.None)
            {
                sb.AppendLine("\t\t\tnew LuaMethod(\"__sub\", Lua_Sub),");
            }
            if ((op & MetaOp.Mul) != MetaOp.None)
            {
                sb.AppendLine("\t\t\tnew LuaMethod(\"__mul\", Lua_Mul),");
            }
            if ((op & MetaOp.Div) != MetaOp.None)
            {
                sb.AppendLine("\t\t\tnew LuaMethod(\"__div\", Lua_Div),");
            }
            if ((op & MetaOp.Eq) != MetaOp.None)
            {
                sb.AppendLine("\t\t\tnew LuaMethod(\"__eq\", Lua_Eq),");
            }
            if ((op & MetaOp.Neg) != MetaOp.None)
            {
                sb.AppendLine("\t\t\tnew LuaMethod(\"__unm\", Lua_Neg),");
            }
        }

        public static MethodInfo GenOverrideFunc(string name)
        {
            <GenOverrideFunc>c__AnonStorey45 storey = new <GenOverrideFunc>c__AnonStorey45 {
                list = new List<MethodInfo>()
            };
            for (int i = 0; i < methods.Length; i++)
            {
                if (((methods[i].Name == name) && !methods[i].IsGenericMethod) && !HasDecimal(methods[i].GetParameters()))
                {
                    Push(storey.list, methods[i]);
                }
            }
            if (storey.list.Count == 1)
            {
                return storey.list[0];
            }
            storey.list.Sort(new Comparison<MethodInfo>(ToLuaExport.Compare));
            sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
            sb.AppendFormat("\tstatic int {0}(IntPtr L)\r\n", GetFuncName(name));
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tint count = LuaDLL.lua_gettop(L);");
            List<MethodInfo> list = new List<MethodInfo>();
            <GenOverrideFunc>c__AnonStorey46 storey2 = new <GenOverrideFunc>c__AnonStorey46 {
                <>f__ref$69 = storey,
                i = 0
            };
            while (storey2.i < storey.list.Count)
            {
                if ((storey.list.FindIndex(new Predicate<MethodInfo>(storey2.<>m__1B)) >= 0) || (HasOptionalParam(storey.list[storey2.i].GetParameters()) && (storey.list[storey2.i].GetParameters().Length > 1)))
                {
                    list.Add(storey.list[storey2.i]);
                }
                storey2.i++;
            }
            sb.AppendLine();
            MethodInfo md = storey.list[0];
            int num3 = (md.ReturnType != typeof(void)) ? 1 : 0;
            int num4 = !md.IsStatic ? 1 : 0;
            int num5 = num4 + 1;
            int num6 = md.GetParameters().Length + num4;
            int num7 = storey.list[1].GetParameters().Length + (!storey.list[1].IsStatic ? 1 : 0);
            bool beLuaString = true;
            bool beCheckTypes = true;
            if (HasOptionalParam(md.GetParameters()))
            {
                ParameterInfo[] parameters = md.GetParameters();
                ParameterInfo info2 = parameters[parameters.Length - 1];
                string typeStr = GetTypeStr(info2.ParameterType.GetElementType());
                if (parameters.Length > 1)
                {
                    string str2 = GenParamTypes(parameters, md.IsStatic);
                    object[] args = new object[] { num5, str2, typeStr, parameters.Length + num4, GetCountStr((parameters.Length + num4) - 1) };
                    sb.AppendFormat("\t\tif (LuaScriptMgr.CheckTypes(L, 1, {1}) && LuaScriptMgr.CheckParamsType(L, typeof({2}), {3}, {4}))\r\n", args);
                }
                else
                {
                    sb.AppendFormat("\t\tif (LuaScriptMgr.CheckParamsType(L, typeof({0}), {1}, {2}))\r\n", typeStr, parameters.Length + num4, GetCountStr((parameters.Length + num4) - 1));
                }
            }
            else if (num6 != num7)
            {
                sb.AppendFormat("\t\tif (count == {0})\r\n", md.GetParameters().Length + num4);
                beLuaString = false;
                beCheckTypes = false;
            }
            else
            {
                ParameterInfo[] p = md.GetParameters();
                if (p.Length > 0)
                {
                    string str3 = GenParamTypes(p, md.IsStatic);
                    sb.AppendFormat("\t\tif (count == {0} && LuaScriptMgr.CheckTypes(L, 1, {2}))\r\n", p.Length + num4, num5, str3);
                }
                else
                {
                    sb.AppendFormat("\t\tif (count == {0})\r\n", p.Length + num4);
                }
            }
            sb.AppendLine("\t\t{");
            int num8 = ProcessParams(md, 3, false, (storey.list.Count > 1) && beLuaString, beCheckTypes);
            sb.AppendFormat("\t\t\treturn {0};\r\n", num3 + num8);
            sb.AppendLine("\t\t}");
            for (int j = 1; j < storey.list.Count; j++)
            {
                beLuaString = true;
                beCheckTypes = true;
                md = storey.list[j];
                num4 = !md.IsStatic ? 1 : 0;
                num5 = num4 + 1;
                num3 = (md.ReturnType != typeof(void)) ? 1 : 0;
                if (!HasOptionalParam(md.GetParameters()))
                {
                    ParameterInfo[] infoArray3 = md.GetParameters();
                    if (list.Contains(storey.list[j]))
                    {
                        string str4 = GenParamTypes(infoArray3, md.IsStatic);
                        sb.AppendFormat("\t\telse if (count == {0} && LuaScriptMgr.CheckTypes(L, 1, {2}))\r\n", infoArray3.Length + num4, num5, str4);
                    }
                    else
                    {
                        sb.AppendFormat("\t\telse if (count == {0})\r\n", infoArray3.Length + num4);
                        beLuaString = false;
                        beCheckTypes = false;
                    }
                }
                else
                {
                    ParameterInfo[] infoArray4 = md.GetParameters();
                    ParameterInfo info3 = infoArray4[infoArray4.Length - 1];
                    string str5 = GetTypeStr(info3.ParameterType.GetElementType());
                    if (infoArray4.Length > 1)
                    {
                        string str6 = GenParamTypes(infoArray4, md.IsStatic);
                        object[] objArray2 = new object[] { num5, str6, str5, infoArray4.Length + num4, GetCountStr((infoArray4.Length + num4) - 1) };
                        sb.AppendFormat("\t\telse if (LuaScriptMgr.CheckTypes(L, 1, {1}) && LuaScriptMgr.CheckParamsType(L, typeof({2}), {3}, {4}))\r\n", objArray2);
                    }
                    else
                    {
                        sb.AppendFormat("\t\telse if (LuaScriptMgr.CheckParamsType(L, typeof({0}), {1}, {2}))\r\n", str5, infoArray4.Length + num4, GetCountStr((infoArray4.Length + num4) - 1));
                    }
                }
                sb.AppendLine("\t\t{");
                num8 = ProcessParams(md, 3, false, beLuaString, beCheckTypes);
                sb.AppendFormat("\t\t\treturn {0};\r\n", num3 + num8);
                sb.AppendLine("\t\t}");
            }
            sb.AppendLine("\t\telse");
            sb.AppendLine("\t\t{");
            sb.AppendFormat("\t\t\tLuaDLL.luaL_error(L, \"invalid arguments to method: {0}.{1}\");\r\n", className, name);
            sb.AppendLine("\t\t}");
            sb.AppendLine();
            sb.AppendLine("\t\treturn 0;");
            sb.AppendLine("\t}");
            return null;
        }

        private static string GenParamTypes(ParameterInfo[] p, bool isStatic)
        {
            StringBuilder builder = new StringBuilder();
            List<System.Type> list = new List<System.Type>();
            if (!isStatic)
            {
                list.Add(type);
            }
            for (int i = 0; i < p.Length; i++)
            {
                if (!IsParams(p[i]))
                {
                    if (p[i].Attributes != ParameterAttributes.Out)
                    {
                        list.Add(p[i].ParameterType);
                    }
                    else
                    {
                        list.Add(null);
                    }
                }
            }
            for (int j = 0; j < (list.Count - 1); j++)
            {
                builder.Append(GetTypeOf(list[j], ", "));
            }
            builder.Append(GetTypeOf(list[list.Count - 1], string.Empty));
            return builder.ToString();
        }

        private static void GenRegFunc()
        {
            sb.AppendLine("\tpublic static void Register(IntPtr L)");
            sb.AppendLine("\t{");
            GenLuaMethods();
            GenLuaFields();
            if (baseClassName == null)
            {
                if ((isStaticClass && (fields.Length == 0)) && (props.Length == 0))
                {
                    sb.AppendFormat("\t\tLuaScriptMgr.RegisterLib(L, \"{0}\", regs);\r\n", libClassName);
                }
                else
                {
                    sb.AppendFormat("\t\tLuaScriptMgr.RegisterLib(L, \"{0}\", typeof({1}), regs, fields, null);\r\n", libClassName, className);
                }
            }
            else
            {
                sb.AppendFormat("\t\tLuaScriptMgr.RegisterLib(L, \"{0}\", typeof({1}), regs, fields, typeof({2}));\r\n", libClassName, className, baseClassName);
            }
            sb.AppendLine("\t}");
        }

        private static void GenToStringFunc()
        {
            if (<>f__am$cache16 == null)
            {
                <>f__am$cache16 = p => p.Name == "ToString";
            }
            if ((Array.FindIndex<MethodInfo>(methods, <>f__am$cache16) >= 0) && !isStaticClass)
            {
                sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
                sb.AppendLine("\tstatic int Lua_ToString(IntPtr L)");
                sb.AppendLine("\t{");
                sb.AppendLine("\t\tobject obj = LuaScriptMgr.GetLuaObject(L, 1);\r\n");
                sb.AppendLine("\t\tif (obj != null)");
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t\tLuaScriptMgr.Push(L, obj.ToString());");
                sb.AppendLine("\t\t}");
                sb.AppendLine("\t\telse");
                sb.AppendLine("\t\t{");
                sb.AppendFormat("\t\t\tLuaScriptMgr.Push(L, \"Table: {0}\");\r\n", libClassName);
                sb.AppendLine("\t\t}");
                sb.AppendLine();
                sb.AppendLine("\t\treturn 1;");
                sb.AppendLine("\t}");
            }
        }

        private static string GetCountStr(int count)
        {
            if (count != 0)
            {
                return string.Format("count - {0}", count);
            }
            return "count";
        }

        private static string GetFuncName(string name)
        {
            if (name == "op_Addition")
            {
                return "Lua_Add";
            }
            if (name == "op_Subtraction")
            {
                return "Lua_Sub";
            }
            if (name == "op_Equality")
            {
                return "Lua_Eq";
            }
            if (name == "op_Multiply")
            {
                return "Lua_Mul";
            }
            if (name == "op_Division")
            {
                return "Lua_Div";
            }
            if (name == "op_UnaryNegation")
            {
                return "Lua_Neg";
            }
            return name;
        }

        private static string[] GetGenericLibName(System.Type[] types)
        {
            string[] strArray = new string[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                System.Type elementType = types[i];
                if (elementType.IsGenericType)
                {
                    strArray[i] = GetGenericLibName(types[i]);
                }
                else if (elementType.IsArray)
                {
                    elementType = elementType.GetElementType();
                    strArray[i] = _C(elementType.ToString()) + "s";
                }
                else
                {
                    strArray[i] = _C(elementType.ToString());
                }
            }
            return strArray;
        }

        public static string GetGenericLibName(System.Type type)
        {
            string str2;
            System.Type[] genericArguments = type.GetGenericArguments();
            string name = type.Name;
            if (name.Contains("NewsConfiger"))
            {
                com.tencent.pandora.Logger.d(1 + string.Empty);
            }
            if (name.Contains("`"))
            {
                str2 = name.Substring(0, name.IndexOf('`'));
            }
            else
            {
                str2 = name;
            }
            return (_C(str2) + "_" + string.Join("_", GetGenericLibName(genericArguments)));
        }

        private static string[] GetGenericName(System.Type[] types)
        {
            string[] strArray = new string[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsGenericType)
                {
                    strArray[i] = GetGenericName(types[i]);
                }
                else
                {
                    strArray[i] = GetTypeStr(types[i]);
                }
            }
            return strArray;
        }

        private static string GetGenericName(System.Type t)
        {
            System.Type[] genericArguments = t.GetGenericArguments();
            string fullName = t.FullName;
            string str2 = _C(fullName.Substring(0, fullName.IndexOf('`')));
            if (fullName.Contains("+"))
            {
                int index = fullName.IndexOf("+");
                int num2 = fullName.IndexOf("[");
                if (num2 > index)
                {
                    string str3 = fullName.Substring(index + 1, (num2 - index) - 1);
                    string[] textArray1 = new string[] { str2, "<", string.Join(",", GetGenericName(genericArguments)), ">.", str3 };
                    return string.Concat(textArray1);
                }
            }
            return (str2 + "<" + string.Join(",", GetGenericName(genericArguments)) + ">");
        }

        private static MetaOp GetOp(string name)
        {
            if (name == "op_Addition")
            {
                return MetaOp.Add;
            }
            if (name == "op_Subtraction")
            {
                return MetaOp.Sub;
            }
            if (name == "op_Equality")
            {
                return MetaOp.Eq;
            }
            if (name == "op_Multiply")
            {
                return MetaOp.Mul;
            }
            if (name == "op_Division")
            {
                return MetaOp.Div;
            }
            if (name == "op_UnaryNegation")
            {
                return MetaOp.Neg;
            }
            return MetaOp.None;
        }

        private static int GetOptionalParamPos(ParameterInfo[] infos)
        {
            for (int i = 0; i < infos.Length; i++)
            {
                if (IsParams(infos[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        private static string GetPushFunction(System.Type t)
        {
            if (t.IsEnum)
            {
                return "Push";
            }
            if (((((t == typeof(bool)) || t.IsPrimitive) || ((t == typeof(string)) || (t == typeof(LuaTable)))) || (((t == typeof(LuaCSFunction)) || (t == typeof(LuaFunction))) || (typeof(UnityEngine.Object).IsAssignableFrom(t) || (t == typeof(System.Type))))) || (((t == typeof(IntPtr)) || typeof(Delegate).IsAssignableFrom(t)) || (((t == typeof(LuaStringBuffer)) || typeof(TrackedReference).IsAssignableFrom(t)) || typeof(IEnumerator).IsAssignableFrom(t))))
            {
                return "Push";
            }
            if ((((t == typeof(Vector3)) || (t == typeof(Vector2))) || ((t == typeof(Vector4)) || (t == typeof(Quaternion)))) || (((t == typeof(Color)) || (t == typeof(RaycastHit))) || (((t == typeof(Ray)) || (t == typeof(Touch))) || (t == typeof(Bounds)))))
            {
                return "Push";
            }
            if (t == typeof(object))
            {
                return "PushVarObject";
            }
            if (t.IsValueType)
            {
                return "PushValue";
            }
            if (t.IsArray)
            {
                return "PushArray";
            }
            return "PushObject";
        }

        private static System.Type GetRefBaseType(string str)
        {
            int index = str.IndexOf("&");
            string typeName = (index < 0) ? str : str.Remove(index);
            System.Type type = System.Type.GetType(typeName);
            if (type == null)
            {
                type = System.Type.GetType(typeName + ", UnityEngine");
            }
            if (type == null)
            {
                type = System.Type.GetType(typeName + ", Assembly-CSharp-firstpass");
            }
            return type;
        }

        private static string GetTypeOf(System.Type t, string sep)
        {
            if (t == null)
            {
                return string.Format("null{0}", sep);
            }
            if (IsLuaTableType(t))
            {
                return string.Format("typeof(LuaTable{1}){0}", sep, !t.IsArray ? string.Empty : "[]");
            }
            return string.Format("typeof({0}){1}", GetTypeStr(t), sep);
        }

        public static string GetTypeStr(System.Type t)
        {
            if (t.IsArray)
            {
                t = t.GetElementType();
                return (GetTypeStr(t) + "[]");
            }
            if (t.IsGenericType)
            {
                return GetGenericName(t);
            }
            return _C(t.ToString());
        }

        public static bool HasAttribute(MemberInfo mb, System.Type atrtype)
        {
            object[] customAttributes = mb.GetCustomAttributes(true);
            for (int i = 0; i < customAttributes.Length; i++)
            {
                if (customAttributes[i].GetType() == atrtype)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool HasDecimal(ParameterInfo[] pi)
        {
            for (int i = 0; i < pi.Length; i++)
            {
                if (pi[i].ParameterType == typeof(decimal))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool HasOptionalParam(ParameterInfo[] infos)
        {
            for (int i = 0; i < infos.Length; i++)
            {
                if (IsParams(infos[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsLuaTableType(System.Type t)
        {
            if (t.IsArray)
            {
                t = t.GetElementType();
            }
            return (((((t == typeof(Vector3)) || (t == typeof(Vector2))) || ((t == typeof(Vector4)) || (t == typeof(Quaternion)))) || ((t == typeof(Color)) || (t == typeof(Ray)))) || (t == typeof(Bounds)));
        }

        public static bool IsMemberFilter(MemberInfo mi)
        {
            return memberFilter.Contains(type.Name + "." + mi.Name);
        }

        private static bool IsNeedOp(string name)
        {
            if (name == "op_Addition")
            {
                op |= MetaOp.Add;
            }
            else if (name == "op_Subtraction")
            {
                op |= MetaOp.Sub;
            }
            else if (name == "op_Equality")
            {
                op |= MetaOp.Eq;
            }
            else if (name == "op_Multiply")
            {
                op |= MetaOp.Mul;
            }
            else if (name == "op_Division")
            {
                op |= MetaOp.Div;
            }
            else if (name == "op_UnaryNegation")
            {
                op |= MetaOp.Neg;
            }
            else
            {
                return false;
            }
            return true;
        }

        public static bool IsObsolete(MemberInfo mb)
        {
            object[] customAttributes = mb.GetCustomAttributes(true);
            for (int i = 0; i < customAttributes.Length; i++)
            {
                System.Type type = customAttributes[i].GetType();
                if ((type == typeof(ObsoleteAttribute)) || (type == typeof(NoToLuaAttribute)))
                {
                    return true;
                }
            }
            return IsMemberFilter(mb);
        }

        private static bool IsParams(ParameterInfo param)
        {
            return (param.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0);
        }

        private static void NewIndexSetValue(System.Type t, string o, string name)
        {
            if (t.IsArray)
            {
                System.Type elementType = t.GetElementType();
                string typeStr = GetTypeStr(elementType);
                if (elementType == typeof(bool))
                {
                    sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetArrayBool(L, 3);\r\n", o, name);
                }
                else if (elementType.IsPrimitive)
                {
                    sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetArrayNumber<{2}>(L, 3);\r\n", o, name, typeStr);
                }
                else if (elementType == typeof(string))
                {
                    sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetArrayString(L, 3);\r\n", o, name);
                }
                else
                {
                    if (elementType == typeof(UnityEngine.Object))
                    {
                        ambig |= ObjAmbig.U3dObj;
                    }
                    sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetArrayObject<{2}>(L, 3);\r\n", o, name, typeStr);
                }
            }
            else if (t == typeof(bool))
            {
                sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetBoolean(L, 3);\r\n", o, name);
            }
            else if (t == typeof(string))
            {
                sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetString(L, 3);\r\n", o, name);
            }
            else if (t.IsPrimitive)
            {
                sb.AppendFormat("\t\t{0}.{1} = ({2})LuaScriptMgr.GetNumber(L, 3);\r\n", o, name, _C(t.ToString()));
            }
            else if (t == typeof(LuaFunction))
            {
                sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetLuaFunction(L, 3);\r\n", o, name);
            }
            else if (t == typeof(LuaTable))
            {
                sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetLuaTable(L, 3);\r\n", o, name);
            }
            else if (t == typeof(object))
            {
                sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetVarObject(L, 3);\r\n", o, name);
            }
            else if (t == typeof(Vector3))
            {
                sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetVector3(L, 3);\r\n", o, name);
            }
            else if (t == typeof(Quaternion))
            {
                sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetQuaternion(L, 3);\r\n", o, name);
            }
            else if (t == typeof(Vector2))
            {
                sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetVector2(L, 3);\r\n", o, name);
            }
            else if (t == typeof(Vector4))
            {
                sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetVector4(L, 3);\r\n", o, name);
            }
            else if (t == typeof(Color))
            {
                sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetColor(L, 3);\r\n", o, name);
            }
            else if (t == typeof(Ray))
            {
                sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetRay(L, 3);\r\n", o, name);
            }
            else if (t == typeof(Bounds))
            {
                sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetBounds(L, 3);\r\n", o, name);
            }
            else if (t == typeof(LuaStringBuffer))
            {
                sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetStringBuffer(L, 3);\r\n", o, name);
            }
            else if (typeof(TrackedReference).IsAssignableFrom(t))
            {
                sb.AppendFormat("\t\t{0}.{1} = ({2})LuaScriptMgr.GetTrackedObject(L, 3, typeof(2));\r\n", o, name, GetTypeStr(t));
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(t))
            {
                sb.AppendFormat("\t\t{0}.{1} = ({2})LuaScriptMgr.GetUnityObject(L, 3, typeof({2}));\r\n", o, name, GetTypeStr(t));
            }
            else if (typeof(Delegate).IsAssignableFrom(t))
            {
                sb.AppendLine("\t\tLuaTypes funcType = LuaDLL.lua_type(L, 3);\r\n");
                sb.AppendLine("\t\tif (funcType != LuaTypes.LUA_TFUNCTION)");
                sb.AppendLine("\t\t{");
                sb.AppendFormat("\t\t\t{0}.{1} = ({2})LuaScriptMgr.GetNetObject(L, 3, typeof({2}));\r\n", o, name, GetTypeStr(t));
                sb.AppendLine("\t\t}\r\n\t\telse");
                sb.AppendLine("\t\t{");
                sb.AppendLine("\t\t\tLuaFunction func = LuaScriptMgr.ToLuaFunction(L, 3);");
                sb.AppendFormat("\t\t\t{0}.{1} = ", o, name);
                GenDelegateBody(t, "\t\t\t", true);
                sb.AppendLine("\t\t}");
            }
            else if (typeof(object).IsAssignableFrom(t) || t.IsEnum)
            {
                sb.AppendFormat("\t\t{0}.{1} = ({2})LuaScriptMgr.GetNetObject(L, 3, typeof({2}));\r\n", o, name, GetTypeStr(t));
            }
            else if (t == typeof(System.Type))
            {
                sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetTypeObject(L, 3);\r\n", o, name);
            }
            else
            {
                object[] args = new object[] { t };
                Debugger.LogError("not defined type {0}", args);
            }
        }

        private static void NoConsturct()
        {
            sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
            sb.AppendFormat("\tstatic int _Create{0}(IntPtr L)\r\n", wrapClassName);
            sb.AppendLine("\t{");
            sb.AppendFormat("\t\tLuaDLL.luaL_error(L, \"{0} class does not have a constructor function\");\r\n", className);
            sb.AppendLine("\t\treturn 0;");
            sb.AppendLine("\t}");
        }

        private static void ProcessExtends(List<MethodInfo> list)
        {
            extendName = "ToLua_" + libClassName.Replace(".", "_");
            extendType = System.Type.GetType(extendName + ", Assembly-CSharp-Editor");
            if (extendType != null)
            {
                <ProcessExtends>c__AnonStorey47 storey = new <ProcessExtends>c__AnonStorey47 {
                    list2 = new List<MethodInfo>()
                };
                storey.list2.AddRange(extendType.GetMethods((BindingFlags.Instance | binding) | BindingFlags.DeclaredOnly));
                <ProcessExtends>c__AnonStorey48 storey2 = new <ProcessExtends>c__AnonStorey48 {
                    <>f__ref$71 = storey,
                    i = storey.list2.Count - 1
                };
                while (storey2.i >= 0)
                {
                    if (((!storey.list2[storey2.i].Name.Contains("op_") && !storey.list2[storey2.i].Name.Contains("add_")) && !storey.list2[storey2.i].Name.Contains("remove_")) || IsNeedOp(storey.list2[storey2.i].Name))
                    {
                        list.RemoveAll(new Predicate<MethodInfo>(storey2.<>m__1D));
                        if (!IsObsolete(storey.list2[storey2.i]))
                        {
                            list.Add(storey.list2[storey2.i]);
                        }
                    }
                    storey2.i--;
                }
            }
        }

        private static int ProcessParams(MethodBase md, int tab, bool beConstruct, bool beLuaString, bool beCheckTypes = false)
        {
            ParameterInfo[] parameters = md.GetParameters();
            int length = parameters.Length;
            string str = string.Empty;
            for (int i = 0; i < tab; i++)
            {
                str = str + "\t";
            }
            if (!md.IsStatic && !beConstruct)
            {
                if (md.Name == "Equals")
                {
                    if (!type.IsValueType)
                    {
                        sb.AppendFormat("{0}{1} obj = LuaScriptMgr.GetVarObject(L, 1) as {1};\r\n", str, className);
                    }
                    else
                    {
                        sb.AppendFormat("{0}{1} obj = ({1})LuaScriptMgr.GetVarObject(L, 1);\r\n", str, className);
                    }
                }
                else if ((className != "Type") && (className != "System.Type"))
                {
                    if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                    {
                        sb.AppendFormat("{0}{1} obj = ({1})LuaScriptMgr.GetUnityObjectSelf(L, 1, \"{1}\");\r\n", str, className);
                    }
                    else if (typeof(TrackedReference).IsAssignableFrom(type))
                    {
                        sb.AppendFormat("{0}{1} obj = ({1})LuaScriptMgr.GetTrackedObjectSelf(L, 1, \"{1}\");\r\n", str, className);
                    }
                    else
                    {
                        sb.AppendFormat("{0}{1} obj = ({1})LuaScriptMgr.GetNetObjectSelf(L, 1, \"{1}\");\r\n", str, className);
                    }
                }
                else
                {
                    sb.AppendFormat("{0}{1} obj = LuaScriptMgr.GetTypeObject(L, 1);\r\n", str, className);
                }
            }
            for (int j = 0; j < length; j++)
            {
                ParameterInfo param = parameters[j];
                string typeStr = GetTypeStr(param.ParameterType);
                string str3 = "arg" + j;
                int num4 = (!md.IsStatic && !beConstruct) ? 2 : 1;
                if (param.Attributes == ParameterAttributes.Out)
                {
                    if (GetRefBaseType(param.ParameterType.ToString()).IsValueType)
                    {
                        sb.AppendFormat("{0}{1} {2};\r\n", str, typeStr, str3);
                    }
                    else
                    {
                        sb.AppendFormat("{0}{1} {2} = null;\r\n", str, typeStr, str3);
                    }
                }
                else if (param.ParameterType == typeof(bool))
                {
                    if (beCheckTypes)
                    {
                        sb.AppendFormat("{2}bool {0} = LuaDLL.lua_toboolean(L, {1});\r\n", str3, j + num4, str);
                    }
                    else
                    {
                        sb.AppendFormat("{2}bool {0} = LuaScriptMgr.GetBoolean(L, {1});\r\n", str3, j + num4, str);
                    }
                }
                else if (param.ParameterType == typeof(string))
                {
                    string str4 = !beLuaString ? "GetLuaString" : "GetString";
                    object[] args = new object[] { str3, j + num4, str, str4 };
                    sb.AppendFormat("{2}string {0} = LuaScriptMgr.{3}(L, {1});\r\n", args);
                }
                else if (param.ParameterType.IsPrimitive)
                {
                    if (beCheckTypes)
                    {
                        object[] objArray2 = new object[] { typeStr, str3, j + num4, str };
                        sb.AppendFormat("{3}{0} {1} = ({0})LuaDLL.lua_tonumber(L, {2});\r\n", objArray2);
                    }
                    else
                    {
                        object[] objArray3 = new object[] { typeStr, str3, j + num4, str };
                        sb.AppendFormat("{3}{0} {1} = ({0})LuaScriptMgr.GetNumber(L, {2});\r\n", objArray3);
                    }
                }
                else if (param.ParameterType == typeof(LuaFunction))
                {
                    if (beCheckTypes)
                    {
                        sb.AppendFormat("{2}LuaFunction {0} = LuaScriptMgr.ToLuaFunction(L, {1});\r\n", str3, j + num4, str);
                    }
                    else
                    {
                        sb.AppendFormat("{2}LuaFunction {0} = LuaScriptMgr.GetLuaFunction(L, {1});\r\n", str3, j + num4, str);
                    }
                }
                else if (param.ParameterType.IsSubclassOf(typeof(MulticastDelegate)))
                {
                    sb.AppendFormat("{0}{1} {2} = null;\r\n", str, typeStr, str3);
                    sb.AppendFormat("{0}LuaTypes funcType{1} = LuaDLL.lua_type(L, {1});\r\n", str, j + num4);
                    sb.AppendLine();
                    sb.AppendFormat("{0}if (funcType{1} != LuaTypes.LUA_TFUNCTION)\r\n", str, j + num4);
                    sb.AppendLine(str + "{");
                    if (beCheckTypes)
                    {
                        object[] objArray4 = new object[] { typeStr, str3, j + num4, str + "\t" };
                        sb.AppendFormat("{3} {1} = ({0})LuaScriptMgr.GetLuaObject(L, {2});\r\n", objArray4);
                    }
                    else
                    {
                        object[] objArray5 = new object[] { typeStr, str3, j + num4, str + "\t" };
                        sb.AppendFormat("{3} {1} = ({0})LuaScriptMgr.GetNetObject(L, {2}, typeof({0}));\r\n", objArray5);
                    }
                    sb.AppendFormat("{0}}}\r\n{0}else\r\n{0}{{\r\n", str);
                    sb.AppendFormat("{0}\tLuaFunction func = LuaScriptMgr.GetLuaFunction(L, {1});\r\n", str, j + num4);
                    sb.AppendFormat("{0}\t{1} = ", str, str3);
                    GenDelegateBody(param.ParameterType, str + "\t", true);
                    sb.AppendLine(str + "}\r\n");
                }
                else if (param.ParameterType == typeof(LuaTable))
                {
                    if (beCheckTypes)
                    {
                        sb.AppendFormat("{2}LuaTable {0} = LuaScriptMgr.ToLuaTable(L, {1});\r\n", str3, j + num4, str);
                    }
                    else
                    {
                        sb.AppendFormat("{2}LuaTable {0} = LuaScriptMgr.GetLuaTable(L, {1});\r\n", str3, j + num4, str);
                    }
                }
                else if ((param.ParameterType == typeof(Vector2)) || (GetRefBaseType(param.ParameterType.ToString()) == typeof(Vector2)))
                {
                    sb.AppendFormat("{2}Vector2 {0} = LuaScriptMgr.GetVector2(L, {1});\r\n", str3, j + num4, str);
                }
                else if ((param.ParameterType == typeof(Vector3)) || (GetRefBaseType(param.ParameterType.ToString()) == typeof(Vector3)))
                {
                    sb.AppendFormat("{2}Vector3 {0} = LuaScriptMgr.GetVector3(L, {1});\r\n", str3, j + num4, str);
                }
                else if ((param.ParameterType == typeof(Vector4)) || (GetRefBaseType(param.ParameterType.ToString()) == typeof(Vector4)))
                {
                    sb.AppendFormat("{2}Vector4 {0} = LuaScriptMgr.GetVector4(L, {1});\r\n", str3, j + num4, str);
                }
                else if ((param.ParameterType == typeof(Quaternion)) || (GetRefBaseType(param.ParameterType.ToString()) == typeof(Quaternion)))
                {
                    sb.AppendFormat("{2}Quaternion {0} = LuaScriptMgr.GetQuaternion(L, {1});\r\n", str3, j + num4, str);
                }
                else if ((param.ParameterType == typeof(Color)) || (GetRefBaseType(param.ParameterType.ToString()) == typeof(Color)))
                {
                    sb.AppendFormat("{2}Color {0} = LuaScriptMgr.GetColor(L, {1});\r\n", str3, j + num4, str);
                }
                else if ((param.ParameterType == typeof(Ray)) || (GetRefBaseType(param.ParameterType.ToString()) == typeof(Ray)))
                {
                    sb.AppendFormat("{2}Ray {0} = LuaScriptMgr.GetRay(L, {1});\r\n", str3, j + num4, str);
                }
                else if ((param.ParameterType == typeof(Bounds)) || (GetRefBaseType(param.ParameterType.ToString()) == typeof(Bounds)))
                {
                    sb.AppendFormat("{2}Bounds {0} = LuaScriptMgr.GetBounds(L, {1});\r\n", str3, j + num4, str);
                }
                else if (param.ParameterType == typeof(object))
                {
                    sb.AppendFormat("{2}object {0} = LuaScriptMgr.GetVarObject(L, {1});\r\n", str3, j + num4, str);
                }
                else if (param.ParameterType == typeof(System.Type))
                {
                    object[] objArray6 = new object[] { str, typeStr, str3, j + num4 };
                    sb.AppendFormat("{0}{1} {2} = LuaScriptMgr.GetTypeObject(L, {3});\r\n", objArray6);
                }
                else if (param.ParameterType == typeof(LuaStringBuffer))
                {
                    sb.AppendFormat("{2}LuaStringBuffer {0} = LuaScriptMgr.GetStringBuffer(L, {1});\r\n", str3, j + num4, str);
                }
                else if (param.ParameterType.IsArray)
                {
                    System.Type elementType = param.ParameterType.GetElementType();
                    string str5 = GetTypeStr(elementType);
                    string str6 = "GetArrayObject";
                    bool flag = false;
                    bool flag2 = false;
                    bool flag3 = false;
                    if (elementType == typeof(bool))
                    {
                        str6 = "GetArrayBool";
                    }
                    else if (elementType.IsPrimitive)
                    {
                        flag = true;
                        str6 = "GetArrayNumber";
                    }
                    else if (elementType == typeof(string))
                    {
                        flag2 = IsParams(param);
                        str6 = !flag2 ? "GetArrayString" : "GetParamsString";
                    }
                    else
                    {
                        flag = true;
                        flag2 = IsParams(param);
                        str6 = !flag2 ? "GetArrayObject" : "GetParamsObject";
                        if (elementType == typeof(object))
                        {
                            flag3 = true;
                        }
                        if (elementType == typeof(UnityEngine.Object))
                        {
                            ambig |= ObjAmbig.U3dObj;
                        }
                    }
                    if (flag)
                    {
                        if (flag2)
                        {
                            if (!flag3)
                            {
                                object[] objArray7 = new object[] { str5, j + num4, j, GetCountStr((j + num4) - 1), str6, str };
                                sb.AppendFormat("{5}{0}[] objs{2} = LuaScriptMgr.{4}<{0}>(L, {1}, {3});\r\n", objArray7);
                            }
                            else
                            {
                                object[] objArray8 = new object[] { j + num4, j, GetCountStr((j + num4) - 1), str6, str };
                                sb.AppendFormat("{4}object[] objs{1} = LuaScriptMgr.{3}(L, {0}, {2});\r\n", objArray8);
                            }
                        }
                        else
                        {
                            object[] objArray9 = new object[] { str5, j + num4, j, str6, str };
                            sb.AppendFormat("{4}{0}[] objs{2} = LuaScriptMgr.{3}<{0}>(L, {1});\r\n", objArray9);
                        }
                    }
                    else if (flag2)
                    {
                        object[] objArray10 = new object[] { str5, j + num4, j, GetCountStr((j + num4) - 1), str6, str };
                        sb.AppendFormat("{5}{0}[] objs{2} = LuaScriptMgr.{4}(L, {1}, {3});\r\n", objArray10);
                    }
                    else
                    {
                        object[] objArray11 = new object[] { str5, j + num4, j, (j + num4) - 1, str6, str };
                        sb.AppendFormat("{5}{0}[] objs{2} = LuaScriptMgr.{4}(L, {1});\r\n", objArray11);
                    }
                }
                else if (md.Name == "op_Equality")
                {
                    if (!type.IsValueType)
                    {
                        object[] objArray12 = new object[] { typeStr, str3, j + num4, str };
                        sb.AppendFormat("{3}{0} {1} = LuaScriptMgr.GetLuaObject(L, {2}) as {0};\r\n", objArray12);
                    }
                    else
                    {
                        object[] objArray13 = new object[] { typeStr, str3, j + num4, str };
                        sb.AppendFormat("{3}{0} {1} = ({0})LuaScriptMgr.GetVarObject(L, {2});\r\n", objArray13);
                    }
                }
                else if (beCheckTypes)
                {
                    object[] objArray14 = new object[] { typeStr, str3, j + num4, str };
                    sb.AppendFormat("{3}{0} {1} = ({0})LuaScriptMgr.GetLuaObject(L, {2});\r\n", objArray14);
                }
                else if (typeof(UnityEngine.Object).IsAssignableFrom(param.ParameterType))
                {
                    object[] objArray15 = new object[] { typeStr, str3, j + num4, str };
                    sb.AppendFormat("{3}{0} {1} = ({0})LuaScriptMgr.GetUnityObject(L, {2}, typeof({0}));\r\n", objArray15);
                }
                else if (typeof(TrackedReference).IsAssignableFrom(param.ParameterType))
                {
                    object[] objArray16 = new object[] { typeStr, str3, j + num4, str };
                    sb.AppendFormat("{3}{0} {1} = ({0})LuaScriptMgr.GetTrackedObject(L, {2}, typeof({0}));\r\n", objArray16);
                }
                else
                {
                    object[] objArray17 = new object[] { typeStr, str3, j + num4, str };
                    sb.AppendFormat("{3}{0} {1} = ({0})LuaScriptMgr.GetNetObject(L, {2}, typeof({0}));\r\n", objArray17);
                }
            }
            StringBuilder builder = new StringBuilder();
            List<string> list = new List<string>();
            List<System.Type> list2 = new List<System.Type>();
            for (int k = 0; k < (length - 1); k++)
            {
                ParameterInfo info2 = parameters[k];
                if (!info2.ParameterType.IsArray)
                {
                    if (!info2.ParameterType.ToString().Contains("&"))
                    {
                        builder.Append("arg");
                    }
                    else
                    {
                        if (info2.Attributes == ParameterAttributes.Out)
                        {
                            builder.Append("out arg");
                        }
                        else
                        {
                            builder.Append("ref arg");
                        }
                        list.Add("arg" + k);
                        list2.Add(GetRefBaseType(info2.ParameterType.ToString()));
                    }
                }
                else
                {
                    builder.Append("objs");
                }
                builder.Append(k);
                builder.Append(",");
            }
            if (length > 0)
            {
                ParameterInfo info3 = parameters[length - 1];
                if (!info3.ParameterType.IsArray)
                {
                    if (!info3.ParameterType.ToString().Contains("&"))
                    {
                        builder.Append("arg");
                    }
                    else
                    {
                        if (info3.Attributes == ParameterAttributes.Out)
                        {
                            builder.Append("out arg");
                        }
                        else
                        {
                            builder.Append("ref arg");
                        }
                        list.Add("arg" + (length - 1));
                        list2.Add(GetRefBaseType(info3.ParameterType.ToString()));
                    }
                }
                else
                {
                    builder.Append("objs");
                }
                builder.Append((int) (length - 1));
            }
            if (beConstruct)
            {
                sb.AppendFormat("{2}{0} obj = new {0}({1});\r\n", className, builder.ToString(), str);
                string pushFunction = GetPushFunction(type);
                sb.AppendFormat("{0}LuaScriptMgr.{1}(L, obj);\r\n", str, pushFunction);
                for (int n = 0; n < list.Count; n++)
                {
                    pushFunction = GetPushFunction(list2[n]);
                    sb.AppendFormat("{1}LuaScriptMgr.{2}(L, {0});\r\n", list[n], str, pushFunction);
                }
                return (list.Count + 1);
            }
            string str8 = !md.IsStatic ? "obj" : className;
            MethodInfo info4 = md as MethodInfo;
            if (info4.ReturnType == typeof(void))
            {
                if (md.Name == "set_Item")
                {
                    switch (length)
                    {
                        case 2:
                            sb.AppendFormat("{0}{1}[arg0] = arg1;\r\n", str, str8);
                            break;

                        case 3:
                            sb.AppendFormat("{0}{1}[arg0, arg1] = arg2;\r\n", str, str8);
                            break;
                    }
                }
                else
                {
                    object[] objArray18 = new object[] { str8, md.Name, builder.ToString(), str };
                    sb.AppendFormat("{3}{0}.{1}({2});\r\n", objArray18);
                }
                if (!md.IsStatic && type.IsValueType)
                {
                    sb.AppendFormat("{0}LuaScriptMgr.SetValueObject(L, 1, obj);\r\n", str);
                }
            }
            else
            {
                string ret = GetTypeStr(info4.ReturnType);
                if (md.Name.Contains("op_"))
                {
                    CallOpFunction(md.Name, tab, ret);
                }
                else if (md.Name == "get_Item")
                {
                    object[] objArray19 = new object[] { str8, md.Name, builder.ToString(), ret, str };
                    sb.AppendFormat("{4}{3} o = {0}[{2}];\r\n", objArray19);
                }
                else if (md.Name == "Equals")
                {
                    if (type.IsValueType)
                    {
                        sb.AppendFormat("{0}bool o = obj.Equals(arg0);\r\n", str);
                    }
                    else
                    {
                        sb.AppendFormat("{0}bool o = obj != null ? obj.Equals(arg0) : arg0 == null;\r\n", str);
                    }
                }
                else
                {
                    object[] objArray20 = new object[] { str8, md.Name, builder.ToString(), ret, str };
                    sb.AppendFormat("{4}{3} o = {0}.{1}({2});\r\n", objArray20);
                }
                string str10 = GetPushFunction(info4.ReturnType);
                sb.AppendFormat("{0}LuaScriptMgr.{1}(L, o);\r\n", str, str10);
            }
            for (int m = 0; m < list.Count; m++)
            {
                string str11 = GetPushFunction(list2[m]);
                sb.AppendFormat("{1}LuaScriptMgr.{2}(L, {0});\r\n", list[m], str, str11);
            }
            return list.Count;
        }

        private static void Push(List<MethodInfo> list, MethodInfo r)
        {
            <Push>c__AnonStorey44 storey = new <Push>c__AnonStorey44 {
                r = r
            };
            int index = list.FindIndex(new Predicate<MethodInfo>(storey.<>m__1A));
            if (index >= 0)
            {
                if (CompareMethod(list[index], storey.r) == 2)
                {
                    list.RemoveAt(index);
                    list.Add(storey.r);
                }
            }
            else
            {
                list.Add(storey.r);
            }
        }

        private static void SaveFile(string file)
        {
            using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
            {
                StringBuilder builder = new StringBuilder();
                foreach (string str in usingList)
                {
                    builder.AppendFormat("using {0};\r\n", str);
                }
                builder.AppendLine("using com.tencent.pandora;");
                if (ambig == ObjAmbig.All)
                {
                    builder.AppendLine("using Object = UnityEngine.Object;");
                }
                builder.AppendLine();
                writer.Write(builder.ToString());
                writer.Write(sb.ToString());
                writer.Flush();
                writer.Close();
            }
        }

        [CompilerGenerated]
        private sealed class <GenConstruct>c__AnonStorey41
        {
            internal ConstructorInfo r;

            internal bool <>m__18(ConstructorInfo p)
            {
                return (ToLuaExport.CompareMethod(p, this.r) >= 0);
            }
        }

        [CompilerGenerated]
        private sealed class <GenConstruct>c__AnonStorey42
        {
            internal List<ConstructorInfo> list;
        }

        [CompilerGenerated]
        private sealed class <GenConstruct>c__AnonStorey43
        {
            internal ToLuaExport.<GenConstruct>c__AnonStorey42 <>f__ref$66;
            internal int i;

            internal bool <>m__19(ConstructorInfo p)
            {
                return ((p != this.<>f__ref$66.list[this.i]) && (p.GetParameters().Length == this.<>f__ref$66.list[this.i].GetParameters().Length));
            }
        }

        [CompilerGenerated]
        private sealed class <Generate>c__AnonStorey3F
        {
            internal PropertyInfo[] ps;
        }

        [CompilerGenerated]
        private sealed class <Generate>c__AnonStorey40
        {
            internal ToLuaExport.<Generate>c__AnonStorey3F <>f__ref$63;
            internal int i;

            internal bool <>m__15(MethodInfo m)
            {
                return (m.Name == ("get_" + this.<>f__ref$63.ps[this.i].Name));
            }

            internal bool <>m__16(MethodInfo m)
            {
                return (m.Name == ("set_" + this.<>f__ref$63.ps[this.i].Name));
            }
        }

        [CompilerGenerated]
        private sealed class <GenOverrideFunc>c__AnonStorey45
        {
            internal List<MethodInfo> list;
        }

        [CompilerGenerated]
        private sealed class <GenOverrideFunc>c__AnonStorey46
        {
            internal ToLuaExport.<GenOverrideFunc>c__AnonStorey45 <>f__ref$69;
            internal int i;

            internal bool <>m__1B(MethodInfo p)
            {
                return ToLuaExport.CompareParmsCount(p, this.<>f__ref$69.list[this.i]);
            }
        }

        [CompilerGenerated]
        private sealed class <ProcessExtends>c__AnonStorey47
        {
            internal List<MethodInfo> list2;
        }

        [CompilerGenerated]
        private sealed class <ProcessExtends>c__AnonStorey48
        {
            internal ToLuaExport.<ProcessExtends>c__AnonStorey47 <>f__ref$71;
            internal int i;

            internal bool <>m__1D(MethodInfo md)
            {
                return (md.Name == this.<>f__ref$71.list2[this.i].Name);
            }
        }

        [CompilerGenerated]
        private sealed class <Push>c__AnonStorey44
        {
            internal MethodInfo r;

            internal bool <>m__1A(MethodInfo p)
            {
                return ((p.Name == this.r.Name) && (ToLuaExport.CompareMethod(p, this.r) >= 0));
            }
        }
    }
}

