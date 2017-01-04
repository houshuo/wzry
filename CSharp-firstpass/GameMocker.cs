using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

public class GameMocker
{
    private Dictionary<Assembly, AssemblyContext> _controlAssemblyDict = new Dictionary<Assembly, AssemblyContext>();
    private List<Type> _dictTypes;
    private List<Type> _esForceTypes = new List<Type>();
    private CodeContext _gsEntryBuilder;
    private static GameMocker _instance;
    private Dictionary<string, CodeContext> _isCodeDict;
    private List<Type> _listTypes;
    private StringBuilder _mockSkipTypesOutput;
    private StringBuilder _partialTypesOutput;
    private List<Type> _queueTypes;
    private List<Type> _skipMockTypes;
    private List<Type> _stackTypes;
    private Dictionary<Type, MockType> _typeDict;
    public const string GameStoreEntry = "GameStoreEntry";
    public const string MockRootPath = "C:/sgame_trunk/Project/Assets";
    public const string PluginMockPath = "C:/sgame_trunk/Project/Assets/Plugins/mock";
    public const string REFKEY = "__refKey";
    public const string RootPath = "C:/sgame_trunk";
    public const string ScriptMockPath = "C:/sgame_trunk/Project/Assets/Scripts/mock";

    private GameMocker()
    {
        this.AddForceESType(typeof(ListViewBase));
        this.AddForceESType(typeof(ListValueViewBase));
        this._listTypes = new List<Type>();
        this.AddListType(typeof(List<>), true);
        this.AddListType(typeof(ListView<>), true);
        this.AddListType(typeof(ListLinqView<>), true);
        this.AddListType(typeof(ListValueView<>), true);
        this._dictTypes = new List<Type>();
        this.AddDictType(typeof(Dictionary<,>), true);
        this.AddDictType(typeof(DictionaryView<,>), true);
        this.AddDictType(typeof(DictionaryObjectView<,>), true);
        this._queueTypes = new List<Type>();
        this._queueTypes.Add(typeof(Queue<>));
        this._stackTypes = new List<Type>();
        this._stackTypes.Add(typeof(Stack<>));
        this._skipMockTypes = new List<Type>();
    }

    public void AddDictType(Type type, bool forceES)
    {
        this._dictTypes.Add(type);
        if (forceES)
        {
            this.AddForceESType(type);
        }
    }

    public void AddForceESType(Type type)
    {
        this._esForceTypes.Add(type);
    }

    public void AddListType(Type type, bool forceES)
    {
        this._listTypes.Add(type);
        if (forceES)
        {
            this.AddForceESType(type);
        }
    }

    public void AddQueueType(Type type, bool forceES)
    {
        this._queueTypes.Add(type);
        if (forceES)
        {
            this.AddForceESType(type);
        }
    }

    public void AddSkipMockType(Type type)
    {
        this._skipMockTypes.Add(type);
    }

    public void AddStackType(Type type, bool forceES)
    {
        this._stackTypes.Add(type);
        if (forceES)
        {
            this.AddForceESType(type);
        }
    }

    private void BuildFieldStore(CodeContext cc, Type fieldType, string stFieldName, CodeBuilder cbStore, string rstFieldName, CodeBuilder cbRestore)
    {
        BaseDataType type = fieldType.ToBDT();
        cc.AddUseType(fieldType);
        if (type < BaseDataType.BASIC_COUNT)
        {
            cbStore.AppendLine("gs.Write" + type.ToString() + "(" + stFieldName + ");", 0, true);
            cbRestore.AppendLine(rstFieldName + " = gs.Read" + type.ToString() + "();", 0, true);
        }
        else if (type == BaseDataType.Enum)
        {
            cbStore.AppendLine("gs.WriteInt32((Int32)" + stFieldName + ");", 0, true);
            cbRestore.AppendLine(rstFieldName + " = (" + fieldType.GetPathName(true) + ")gs.ReadInt32();", 0, true);
        }
        else if (type == BaseDataType.Struct)
        {
            MockType mockType = this.GetMockType(fieldType);
            if (mockType != null)
            {
                if (mockType.canIStore)
                {
                    cbStore.AppendLine(stFieldName + ".__Store(gs);", 0, true);
                    cbRestore.AppendLine(rstFieldName + ".__Restore(gs);", 0, true);
                }
                else if (mockType.canEStore)
                {
                    string str = !mockType.publicEStore ? this.CombNestPathName(mockType) : mockType.buildinAssembly.StoreExtendName;
                    cbStore.AppendLine(string.Concat(new object[] { str, "._Store_", mockType.key, "(", stFieldName, ", gs);" }), 0, true);
                    cbRestore.AppendLine(string.Concat(new object[] { rstFieldName, " = ", str, "._Restore_", mockType.key, "(gs);" }), 0, true);
                }
            }
            else
            {
                this._mockSkipTypesOutput.AppendLine("SkipStructType:" + fieldType.GetPathName(true) + "#" + stFieldName);
            }
        }
        else if (((type == BaseDataType.Class) || (type == BaseDataType.Interface)) || ((type == BaseDataType.Delegate) || (type == BaseDataType.Array)))
        {
            MockType mt = this.GetMockType(fieldType);
            if (mt != null)
            {
                if (mt.canIStore)
                {
                    cbStore.AppendLine("gs.WriteIStore(" + stFieldName + ");", 0, true);
                    cbRestore.AppendLine("gs.ReadIStore();", 0, true);
                }
                else if (mt.canEStore)
                {
                    string str2 = !mt.publicEStore ? this.CombNestPathName(mt) : mt.buildinAssembly.StoreExtendName;
                    cbStore.AppendLine(string.Concat(new object[] { "gs.WriteEStore(", stFieldName, ", ", mt.key, ", ", str2, "._Store_", mt.key, ", ", str2, "._Restore_", mt.key, ");" }), 0, true);
                    cbRestore.AppendLine("gs.ReadEStore();", 0, true);
                }
            }
            else
            {
                this._mockSkipTypesOutput.AppendLine("SkipClassType:" + fieldType.GetPathName(true) + "#" + stFieldName);
            }
        }
        else
        {
            cbStore.AppendLine("gs.WriteObject(" + stFieldName + ");", 0, true);
            cbRestore.AppendLine("gs.ReadObject();", 0, true);
            this._mockSkipTypesOutput.AppendLine("MockGenericType:" + fieldType.GetPathName(true) + "#" + stFieldName);
        }
    }

    private void BuildTypeEStore(MockType mt)
    {
        if (mt.gta == null)
        {
            CodeBuilder builder = null;
            if (mt.publicEStore)
            {
                builder = new CodeContext("_global_", string.Empty);
                builder.MoveIndent(1);
                mt.extendCode = builder as CodeContext;
            }
            else
            {
                List<MockType> list = this.FindNestPath(mt);
                if ((list == null) || (list.Count <= 0))
                {
                    return;
                }
                mt.extendCode = new CodeContext(!string.IsNullOrEmpty(list[0].nameSpace) ? list[0].nameSpace : "_global_", string.Empty);
                builder = mt.extendCode.CreateChild(1);
                for (int i = 0; i < list.Count; i++)
                {
                    builder.AppendLine(BuildTypeHeader(list[i], false), 0, true);
                    builder.AppendLine("}", 0, false);
                }
            }
            Type item = null;
            if (mt.type.IsGenericType)
            {
                item = mt.type.GetGenericTypeDefinition();
            }
            mt.autoExtend = false;
            builder.AppendLine("#region " + mt.pathName, 0, true);
            if (mt.bdt == BaseDataType.Struct)
            {
                builder.AppendLine(string.Concat(new object[] { "public static void _Store_", mt.key, "(", mt.pathName, " val, GameStore gs) {" }), 0, true);
                builder.AppendLine("}", 0, true);
                builder.AppendLine(string.Concat(new object[] { "[GameTypeAttr(", mt.key, ", typeof(", mt.pathDesc, "))]" }), 0, true);
                builder.AppendLine(string.Concat(new object[] { "public static ", mt.pathName, " _Restore_", mt.key, "(GameStore gs) {" }), 1, true);
                builder.AppendLine(mt.pathName + " val = new " + mt.pathName + "();", 0, true);
                builder.AppendLine("return val;", 0, true);
                builder.AppendLine(-1, "}", true);
            }
            else
            {
                CodeBuilder cbStore = builder.CreateChild(0);
                CodeBuilder cbRestore = builder.CreateChild(0);
                this.GenRefStoreHead(mt, cbStore, cbRestore);
                if (mt.bdt == BaseDataType.Array)
                {
                    this.GenArrayExtend(mt, cbStore, cbRestore, mt.extendCode);
                    mt.autoExtend = true;
                }
                else if (mt.bdt == BaseDataType.Delegate)
                {
                    this.GenDelegateExtend(mt, cbStore, cbRestore, mt.extendCode);
                    mt.autoExtend = true;
                }
                else if (this._listTypes.Contains(item))
                {
                    this.GenListExtend(mt, cbStore, cbRestore, mt.extendCode);
                    mt.autoExtend = true;
                }
                else if (this._dictTypes.Contains(item))
                {
                    this.GenDictExtend(mt, cbStore, cbRestore, mt.extendCode);
                    mt.autoExtend = true;
                }
                else if (this._queueTypes.Contains(item))
                {
                    this.GenQueueExtend(mt, cbStore, cbRestore, mt.extendCode);
                    mt.autoExtend = true;
                }
                else if (this._stackTypes.Contains(item))
                {
                    this.GenStackExtend(mt, cbStore, cbRestore, mt.extendCode);
                    mt.autoExtend = true;
                }
                else
                {
                    cbRestore.AppendLine("return null;", 0, true);
                }
                this.GenRefStoreFoot(mt, cbStore, cbRestore);
            }
            builder.AppendLine("#endregion " + mt.pathName, 0, false);
        }
    }

    private static string BuildTypeHeader(MockType mt, bool iStore)
    {
        Type type = mt.type;
        BaseDataType bdt = mt.bdt;
        bool flag = bdt == BaseDataType.Class;
        bool flag2 = bdt == BaseDataType.Interface;
        StringBuilder builder = new StringBuilder(0x20);
        bool flag3 = false;
        if (type.IsPublic)
        {
            builder.Append("public");
            flag3 = true;
        }
        if (type.IsAbstract && !flag2)
        {
            if (flag3)
            {
                builder.Append(" ");
            }
            builder.Append("abstract");
            flag3 = true;
        }
        if (flag3)
        {
            builder.Append(" ");
        }
        builder.Append("partial ");
        string str = !flag ? (!flag2 ? "struct" : "interface") : "class";
        builder.Append(str);
        builder.Append(" ");
        builder.Append(mt.thisName);
        if (iStore)
        {
            builder.Append(" : IStore");
        }
        builder.Append(" {");
        return builder.ToString();
    }

    private bool BuildTypeIStore(MockType mt, CodeContext cc = null, CodeBuilder cb = null)
    {
        if (!mt.genericIStore)
        {
            return false;
        }
        if (cc == null)
        {
            string nameSpace = mt.nameSpace;
            if (string.IsNullOrEmpty(nameSpace))
            {
                nameSpace = "_global_";
            }
            AssemblyContext context = this._controlAssemblyDict[mt.type.Assembly];
            string key = context.mockPath + "_" + nameSpace;
            if (this._isCodeDict.ContainsKey(key))
            {
                cc = this._isCodeDict[key];
            }
            else
            {
                cc = new CodeContext(nameSpace, context.mockPath);
                cc.AddUseNs("System");
                this._isCodeDict.Add(key, cc);
            }
        }
        if (cb == null)
        {
            cb = cc.CreateChild(!string.IsNullOrEmpty(mt.nameSpace) ? 1 : 0);
        }
        Type type = mt.type;
        BaseDataType bdt = mt.bdt;
        bool flag = bdt == BaseDataType.Class;
        bool flag2 = bdt == BaseDataType.Interface;
        bool flag3 = bdt == BaseDataType.Struct;
        cb.AppendLine(BuildTypeHeader(mt, true), 0, true);
        this._partialTypesOutput.AppendLine((!flag3 ? (!flag2 ? "class" : "interface") : "struct") + " " + mt.thisName);
        if (flag2)
        {
            cb.AppendLine("}", 0, false);
            return true;
        }
        Type[] nestedTypes = type.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public);
        for (int i = 0; i < nestedTypes.Length; i++)
        {
            MockType mockType = this.GetMockType(nestedTypes[i]);
            if ((mockType != null) && mockType.genericIStore)
            {
                this.BuildTypeIStore(mockType, cc, cb.CreateChild(1));
            }
        }
        CodeBuilder builder = cb.CreateChild(1);
        CodeBuilder builder2 = cb.CreateChild(1);
        CodeBuilder cbStore = cb.CreateChild(1);
        CodeBuilder cbRestore = cb.CreateChild(1);
        CodeBuilder builder5 = cb.CreateChild(1);
        CodeBuilder builder6 = cb.CreateChild(1);
        CodeBuilder builder7 = cb.CreateChild(1);
        cb.AppendLine("}", 0, false);
        bool flag4 = (type.BaseType != null) && this.IsControlledType(type.BaseType);
        bool flag5 = flag && flag4;
        if (flag)
        {
            builder.AppendLine(string.Concat(new object[] { "[GameTypeAttr(", mt.key, ", typeof(", mt.pureDesc, "))]" }), 0, true);
            builder.AppendLine("public static" + (!flag5 ? " " : " new ") + "System.Object __Create(GameStore gs) {", 1, true);
            builder.AppendLine("return null;", 0, true);
            builder.AppendLine(-1, "}", true);
        }
        if (!flag4)
        {
            if (flag)
            {
                builder5.AppendLine("private UInt32 __refKey = 0;", 0, true);
                builder5.AppendLine("public UInt32 __RefKey {", 1, true);
                builder5.AppendLine("get {", 1, true);
                builder5.AppendLine("if (__refKey == 0) {", 1, true);
                builder5.AppendLine("__refKey = GameStore.Instance.GenRefKey();", 0, true);
                builder5.AppendLine(-1, "}", true);
                builder5.AppendLine("return __refKey;", 0, true);
                builder5.AppendLine(-1, "}", true);
                builder5.AppendLine(-1, "}", true);
            }
            else
            {
                builder5.AppendLine("public UInt32 __RefKey {", 1, true);
                builder5.AppendLine("get {", 1, true);
                builder5.AppendLine("return UInt32.MaxValue;", 0, true);
                builder5.AppendLine(-1, "}", true);
                builder5.AppendLine(-1, "}", true);
            }
        }
        builder5.AppendLine("public" + (!flag5 ? (!type.IsSealed ? " virtual " : " ") : " override ") + "UInt16 __TypKey {", 1, true);
        builder5.AppendLine("get {", 1, true);
        builder5.AppendLine("return " + mt.key + ";", 0, true);
        builder5.AppendLine(-1, "}", true);
        builder5.AppendLine(-1, "}", true);
        cbStore.AppendLine("public static" + (!flag5 ? " " : " new ") + "void __StoreStatic(GameStore gs) {", 1, true);
        cbRestore.AppendLine("public static" + (!flag5 ? " " : " new ") + "void __RestoreStatic(GameStore gs) {", 1, true);
        bool flag6 = false;
        List<MockType> list = this.FindNotPublicNestStatic(mt, !mt.type.IsPublic);
        for (int j = 0; j < list.Count; j++)
        {
            MockType type4 = list[j];
            cbStore.AppendLine(type4.thisName + ".__StoreStatic(gs);", 0, true);
            cbRestore.AppendLine(type4.thisName + ".__RestoreStatic(gs);", 0, true);
            flag6 = true;
        }
        builder6.AppendLine("public" + (!flag5 ? (!type.IsSealed ? " virtual " : " ") : " override ") + "void __Store(GameStore gs) {", 1, true);
        builder7.AppendLine("public" + (!flag5 ? (!type.IsSealed ? " virtual " : " ") : " override ") + "void __Restore(GameStore gs) {", 1, true);
        if (flag4)
        {
            builder6.AppendLine("base.__Store(gs);", 0, true);
            builder7.AppendLine("base.__Restore(gs);", 0, true);
        }
        List<FieldInfo> fieldsWithFilter = this.GetFieldsWithFilter(type, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        for (int k = 0; k < fieldsWithFilter.Count; k++)
        {
            FieldInfo memFld = fieldsWithFilter[k];
            string stFieldName = this.ValidStoreField(memFld);
            if (stFieldName != null)
            {
                if (memFld.IsStatic)
                {
                    this.BuildFieldStore(cc, memFld.FieldType, stFieldName, cbStore, stFieldName, cbRestore);
                    flag6 = true;
                }
                else
                {
                    this.BuildFieldStore(cc, memFld.FieldType, stFieldName, builder6, stFieldName, builder7);
                }
            }
        }
        if (flag6)
        {
            cbStore.AppendLine(-1, "}", true);
            cbRestore.AppendLine(-1, "}", true);
        }
        else
        {
            cb.RemoveChild(cbStore);
            cb.RemoveChild(cbRestore);
        }
        builder6.AppendLine(-1, "}", true);
        builder7.AppendLine(-1, "}", true);
        return true;
    }

    private static string BuildVarDefine(Type type, string name)
    {
        string str2;
        string pathName = type.GetPathName(true);
        switch (type.ToBDT())
        {
            case BaseDataType.Char:
                str2 = "'\0'";
                break;

            case BaseDataType.Class:
            case BaseDataType.Interface:
            case BaseDataType.Array:
            case BaseDataType.String:
                str2 = "null";
                break;

            case BaseDataType.Struct:
                str2 = "new " + pathName + "()";
                break;

            case BaseDataType.Enum:
                str2 = "(" + pathName + ")0";
                break;

            case BaseDataType.Generic:
                str2 = "default(" + pathName + ")";
                break;

            case BaseDataType.Boolean:
                str2 = "false";
                break;

            default:
                str2 = "0";
                break;
        }
        string[] textArray1 = new string[] { pathName, " ", name, " = ", str2, ";" };
        return string.Concat(textArray1);
    }

    public void CloseMock()
    {
        Dictionary<string, CodeContext>.Enumerator enumerator = this._isCodeDict.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<string, CodeContext> current = enumerator.Current;
            current.Value.Save(null);
        }
        Dictionary<Type, MockType>.Enumerator enumerator2 = this._typeDict.GetEnumerator();
        while (enumerator2.MoveNext())
        {
            KeyValuePair<Type, MockType> pair2 = enumerator2.Current;
            MockType type = pair2.Value;
            if ((type.extendCode != null) && (type.buildinAssembly != null))
            {
                if (type.publicEStore)
                {
                    if (type.autoExtend)
                    {
                        type.buildinAssembly.esPublicCodeAuto.MergeChild(type.extendCode);
                    }
                    else
                    {
                        type.buildinAssembly.esPublicCodeManual.MergeChild(type.extendCode);
                    }
                }
                else if (type.autoExtend)
                {
                    type.buildinAssembly.esPrivateCodeAuto.MergeChild(type.extendCode);
                }
                else
                {
                    type.buildinAssembly.esPrivateCodeManual.MergeChild(type.extendCode);
                }
            }
        }
        Dictionary<Assembly, AssemblyContext>.Enumerator enumerator3 = this._controlAssemblyDict.GetEnumerator();
        while (enumerator3.MoveNext())
        {
            KeyValuePair<Assembly, AssemblyContext> pair3 = enumerator3.Current;
            KeyValuePair<Assembly, AssemblyContext> pair4 = enumerator3.Current;
            pair3.Value.esPublicCodeAuto.Save(pair4.Value.StoreExtendName + "_public_auto.cs");
            KeyValuePair<Assembly, AssemblyContext> pair5 = enumerator3.Current;
            KeyValuePair<Assembly, AssemblyContext> pair6 = enumerator3.Current;
            pair5.Value.esPrivateCodeAuto.Save(pair6.Value.StoreExtendName + "_private_auto.cs");
            KeyValuePair<Assembly, AssemblyContext> pair7 = enumerator3.Current;
            KeyValuePair<Assembly, AssemblyContext> pair8 = enumerator3.Current;
            pair7.Value.esPublicCodeManual.Save(pair8.Value.StoreExtendName + "_public_manual.cs");
            KeyValuePair<Assembly, AssemblyContext> pair9 = enumerator3.Current;
            KeyValuePair<Assembly, AssemblyContext> pair10 = enumerator3.Current;
            pair9.Value.esPrivateCodeManual.Save(pair10.Value.StoreExtendName + "_private_manual.cs");
        }
        this._gsEntryBuilder.Save("GameStoreEntry.cs");
        try
        {
            File.WriteAllText("C:/sgame_trunk/PartialTypesOutput.txt", this._partialTypesOutput.ToString(), Encoding.UTF8);
            File.WriteAllText("C:/sgame_trunk/MockSkipTypesOutput.txt", this._mockSkipTypesOutput.ToString(), Encoding.UTF8);
        }
        catch (Exception)
        {
        }
    }

    private string CombNestPathName(MockType mt)
    {
        List<MockType> list = this.FindNestPath(mt);
        if (list == null)
        {
            return null;
        }
        string str = string.Empty;
        for (int i = 0; i < list.Count; i++)
        {
            if (i > 0)
            {
                str = str + ".";
            }
            str = str + list[i].pureName;
        }
        return str;
    }

    public List<Type> FindDescendTypes(Type type)
    {
        List<Type> list = new List<Type>();
        Dictionary<Assembly, AssemblyContext>.Enumerator enumerator = this._controlAssemblyDict.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<Assembly, AssemblyContext> current = enumerator.Current;
            AssemblyContext context = current.Value;
            for (int i = 0; i < context.typeList.Length; i++)
            {
                Type item = context.typeList[i];
                if (item.IsSubclassOf(type))
                {
                    list.Add(item);
                }
            }
        }
        return list;
    }

    public List<Type> FindImpledTypes(Type type)
    {
        List<Type> list = new List<Type>();
        Dictionary<Assembly, AssemblyContext>.Enumerator enumerator = this._controlAssemblyDict.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<Assembly, AssemblyContext> current = enumerator.Current;
            AssemblyContext context = current.Value;
            for (int i = 0; i < context.typeList.Length; i++)
            {
                Type type2 = context.typeList[i];
                if (type2.IsInterfaceImplied(type))
                {
                    list.Add(type2);
                }
            }
        }
        return list;
    }

    private List<MockType> FindNestPath(MockType mt)
    {
        if (mt.fromTypeList.Count <= 0)
        {
            return null;
        }
        List<MockType> list = new List<MockType>();
        for (MockType type = mt.fromTypeList[0]; type != null; type = this._typeDict[type.type.DeclaringType])
        {
            list.Insert(0, type);
            if ((!type.type.IsNested || (type.type.DeclaringType == null)) || !this._typeDict.ContainsKey(type.type.DeclaringType))
            {
                return list;
            }
        }
        return list;
    }

    private List<MockType> FindNotPublicNestStatic(MockType itrMT, bool notPublic)
    {
        List<MockType> list = new List<MockType>();
        Type[] nestedTypes = itrMT.type.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public);
        for (int i = 0; i < nestedTypes.Length; i++)
        {
            MockType mockType = this.GetMockType(nestedTypes[i]);
            if ((mockType != null) && ((mockType.hasStaticStore && (notPublic || !mockType.type.IsPublic)) || (this.FindNotPublicNestStatic(mockType, notPublic || !mockType.type.IsPublic).Count > 0)))
            {
                list.Add(mockType);
            }
        }
        return list;
    }

    private void GenArrayExtend(MockType mt, CodeBuilder cbStore, CodeBuilder cbRestore, CodeContext cc)
    {
        cbStore.AppendLine("gs.WriteInt32(theVal.Rank);", 0, true);
        cbRestore.AppendLine("int arrayRank = gs.ReadInt32();", 0, true);
        Type elementType = mt.type.GetElementType();
        int arrayRank = mt.type.GetArrayRank();
        string str = string.Empty;
        for (int i = 0; i < arrayRank; i++)
        {
            string str2 = "rankLen" + i;
            cbStore.AppendLine(string.Concat(new object[] { "int ", str2, " = theVal.GetLength(", i, ");" }), 0, true);
            cbStore.AppendLine("gs.WriteInt32(" + str2 + ");", 0, true);
            cbRestore.AppendLine("int " + str2 + " = gs.ReadInt32();", 0, true);
            if (i == 0)
            {
                str = str + "[";
            }
            else
            {
                str = str + ", ";
            }
            str = str + str2;
            if (i == (arrayRank - 1))
            {
                str = str + "]";
            }
        }
        string pathName = elementType.GetPathName(true);
        if (elementType.IsArray)
        {
            pathName = pathName.Insert(pathName.LastIndexOf('['), str);
        }
        else
        {
            pathName = pathName + str;
        }
        cbRestore.AppendLine(mt.pathName + " theVal = new " + pathName + ";", 0, true);
        string stFieldName = "theVal";
        for (int j = 0; j < arrayRank; j++)
        {
            string str5 = "i" + j;
            string str6 = "rankLen" + j;
            cbStore.AppendLine("for (int " + str5 + " = 0; " + str5 + " < " + str6 + "; ++" + str5 + ") {", 1, true);
            cbRestore.AppendLine("for (int " + str5 + " = 0; " + str5 + " < " + str6 + "; ++" + str5 + ") {", 1, true);
            if (j == 0)
            {
                stFieldName = stFieldName + "[";
            }
            else
            {
                stFieldName = stFieldName + ", ";
            }
            stFieldName = stFieldName + str5;
            if (j == (arrayRank - 1))
            {
                stFieldName = stFieldName + "]";
            }
        }
        this.BuildFieldStore(cc, elementType, stFieldName, cbStore, stFieldName, cbRestore);
        for (int k = 0; k < arrayRank; k++)
        {
            cbStore.AppendLine(-1, "}", true);
            cbRestore.AppendLine(-1, "}", true);
        }
        cbRestore.AppendLine("return theVal;", 0, true);
    }

    private void GenDelegateExtend(MockType mt, CodeBuilder cbStore, CodeBuilder cbRestore, CodeContext cc)
    {
        cbStore.AppendLine("Delegate[] dlgLst = theVal.GetInvocationList();", 0, true);
        cbStore.AppendLine("if (null == dlgLst || dlgLst.Length == 0) {", 1, true);
        cbStore.AppendLine("gs.WriteInt32(0);", 0, true);
        cbStore.AppendLine("return;", 0, true);
        cbStore.AppendLine(-1, "}", true);
        cbStore.AppendLine("gs.WriteInt32(dlgLst.Length);", 0, true);
        cbRestore.AppendLine("int dlgNum = gs.ReadInt32();", 0, true);
        cbStore.AppendLine("for (int i = 0; i < dlgLst.Length; ++i) {", 1, true);
        cbRestore.AppendLine("for (int i = 0; i < dlgNum; ++i) {", 1, true);
        cbStore.AppendLine("Delegate curDlg = dlgLst[i];", 0, true);
        cbStore.AppendLine("gs.WriteUInt32(GameStore.Instance.GetRefKey(curDlg.Target));", 0, true);
        cbRestore.AppendLine("UInt32 objKey = gs.ReadUInt32();", 0, true);
        cbStore.AppendLine("gs.WriteString(curDlg.Method.ReflectedType.GetPathName());", 0, true);
        cbRestore.AppendLine("String typeName = gs.ReadString();", 0, true);
        cbStore.AppendLine("gs.WriteString(curDlg.Method.Name);", 0, true);
        cbRestore.AppendLine("String methodName = gs.ReadString();", 0, true);
        cbRestore.AppendLine("if (objKey > 0) {", 1, true);
        cbRestore.AppendLine(-1, "}", true);
        cbRestore.AppendLine("else {", 1, true);
        cbRestore.AppendLine(-1, "}", true);
        cbStore.AppendLine(-1, "}", true);
        cbRestore.AppendLine(-1, "}", true);
        cbRestore.AppendLine("return null;", 0, true);
    }

    private void GenDictExtend(MockType mt, CodeBuilder cbStore, CodeBuilder cbRestore, CodeContext cc)
    {
        cbStore.AppendLine("gs.WriteInt32(theVal.Count);", 0, true);
        cbRestore.AppendLine("int eleCnt = gs.ReadInt32();", 0, true);
        cbRestore.AppendLine(mt.pathName + " theVal = new " + mt.pathName + "(eleCnt);", 0, true);
        cbStore.AppendLine("var emr = theVal.GetEnumerator();", 0, true);
        cbStore.AppendLine("while (emr.MoveNext()) {", 1, true);
        Type[] genericArguments = mt.type.GetGenericArguments();
        cbRestore.AppendLine(BuildVarDefine(genericArguments[0], "curKey"), 0, true);
        cbRestore.AppendLine(BuildVarDefine(genericArguments[1], "curVal"), 0, true);
        cbRestore.AppendLine("for (int i = 0; i < eleCnt; ++i) {", 1, true);
        this.BuildFieldStore(cc, genericArguments[0], "emr.Current.Key", cbStore, "curKey", cbRestore);
        this.BuildFieldStore(cc, genericArguments[1], "emr.Current.Value", cbStore, "curVal", cbRestore);
        cbRestore.AppendLine("theVal.Add(curKey, curVal);", 0, true);
        cbStore.AppendLine(-1, "}", true);
        cbRestore.AppendLine(-1, "}", true);
        cbRestore.AppendLine("return theVal;", 0, true);
    }

    private void GenListExtend(MockType mt, CodeBuilder cbStore, CodeBuilder cbRestore, CodeContext cc)
    {
        cbStore.AppendLine("gs.WriteInt32(theVal.Count);", 0, true);
        cbRestore.AppendLine("int eleCnt = gs.ReadInt32();", 0, true);
        cbRestore.AppendLine(mt.pathName + " theVal = new " + mt.pathName + "(eleCnt);", 0, true);
        Type[] genericArguments = mt.type.GetGenericArguments();
        cbRestore.AppendLine(BuildVarDefine(genericArguments[0], "eleVal"), 0, true);
        cbStore.AppendLine("for (int i = 0; i < theVal.Count; ++i) {", 1, true);
        cbRestore.AppendLine("for (int i = 0; i < eleCnt; ++i) {", 1, true);
        this.BuildFieldStore(cc, genericArguments[0], "theVal[i]", cbStore, "eleVal", cbRestore);
        cbRestore.AppendLine("theVal.Add(eleVal);", 0, true);
        cbStore.AppendLine(-1, "}", true);
        cbRestore.AppendLine(-1, "}", true);
        cbRestore.AppendLine("return theVal;", 0, true);
    }

    private void GenQueueExtend(MockType mt, CodeBuilder cbStore, CodeBuilder cbRestore, CodeContext cc)
    {
        cbStore.AppendLine("gs.WriteInt32(theVal.Count);", 0, true);
        cbRestore.AppendLine("int eleCnt = gs.ReadInt32();", 0, true);
        cbRestore.AppendLine(mt.pathName + " theVal = new " + mt.pathName + "(eleCnt);", 0, true);
        Type[] genericArguments = mt.type.GetGenericArguments();
        cbStore.AppendLine(BuildVarDefine(genericArguments[0], "eleVal"), 0, true);
        cbRestore.AppendLine(BuildVarDefine(genericArguments[0], "eleVal"), 0, true);
        cbStore.AppendLine("var emr = theVal.GetEnumerator();", 0, true);
        cbStore.AppendLine("while (emr.MoveNext()) {", 1, true);
        cbRestore.AppendLine("for (int i = 0; i < eleCnt; ++i) {", 1, true);
        cbStore.AppendLine("eleVal = emr.Current;", 0, true);
        this.BuildFieldStore(cc, genericArguments[0], "eleVal", cbStore, "eleVal", cbRestore);
        cbRestore.AppendLine("theVal.Enqueue(eleVal);", 0, true);
        cbStore.AppendLine(-1, "}", true);
        cbRestore.AppendLine(-1, "}", true);
        cbRestore.AppendLine("return theVal;", 0, true);
    }

    private void GenRefStoreFoot(MockType mt, CodeBuilder cbStore, CodeBuilder cbRestore)
    {
        cbStore.AppendLine(-1, "}", true);
        cbRestore.AppendLine(-1, "}", true);
    }

    private void GenRefStoreHead(MockType mt, CodeBuilder cbStore, CodeBuilder cbRestore)
    {
        cbStore.AppendLine("public static void _Store_" + mt.key + "(Object val, GameStore gs) {", 1, true);
        cbStore.AppendLine(mt.pathName + " theVal = val as " + mt.pathName + ";", 0, true);
        cbRestore.AppendLine(string.Concat(new object[] { "[GameTypeAttr(", mt.key, ", typeof(", mt.pathDesc, "))]" }), 0, true);
        cbRestore.AppendLine("public static Object _Restore_" + mt.key + "(GameStore gs) {", 1, true);
    }

    private void GenStackExtend(MockType mt, CodeBuilder cbStore, CodeBuilder cbRestore, CodeContext cc)
    {
        cbStore.AppendLine("gs.WriteInt32(theVal.Count);", 0, true);
        cbRestore.AppendLine("int eleCnt = gs.ReadInt32();", 0, true);
        cbRestore.AppendLine(mt.pathName + " theVal = new " + mt.pathName + "(eleCnt);", 0, true);
        Type type = mt.type.GetGenericArguments()[0];
        cbStore.AppendLine(BuildVarDefine(type, "eleVal"), 0, true);
        cbRestore.AppendLine(BuildVarDefine(type, "eleVal"), 0, true);
        string str = "System.Collections.Generic.List<" + type.GetPathName(true) + ">";
        cbRestore.AppendLine(str + " auxLst = new " + str + "(eleCnt);", 0, true);
        cbStore.AppendLine("var emr = theVal.GetEnumerator();", 0, true);
        cbStore.AppendLine("while (emr.MoveNext()) {", 1, true);
        cbRestore.AppendLine("for (int i = 0; i < eleCnt; ++i) {", 1, true);
        cbStore.AppendLine("eleVal = emr.Current;", 0, true);
        this.BuildFieldStore(cc, type, "eleVal", cbStore, "eleVal", cbRestore);
        cbRestore.AppendLine("auxLst.Add(eleVal);", 0, true);
        cbStore.AppendLine(-1, "}", true);
        cbRestore.AppendLine(-1, "}", true);
        cbRestore.AppendLine("for (int i = auxLst.Count - 1; i >= 0; --i) {", 1, true);
        cbRestore.AppendLine("theVal.Push(auxLst[i]);", 0, true);
        cbRestore.AppendLine(-1, "}", true);
        cbRestore.AppendLine("return theVal;", 0, true);
    }

    public List<FieldInfo> GetFieldsWithFilter(Type type, BindingFlags flags)
    {
        List<FieldInfo> list = new List<FieldInfo>();
        FieldInfo[] fields = type.GetFields(flags);
        for (int i = 0; i < fields.Length; i++)
        {
            if (!fields[i].IsDefined(typeof(SkipStoreAttr), false))
            {
                list.Add(fields[i]);
            }
        }
        return list;
    }

    private MockType GetMockType(Type orgType)
    {
        if (this._typeDict.ContainsKey(orgType))
        {
            return this._typeDict[orgType];
        }
        return null;
    }

    public bool IsControlledType(Type type)
    {
        return ((type != null) && this._controlAssemblyDict.ContainsKey(type.Assembly));
    }

    public bool IsForceESType(Type type)
    {
        if (type.IsGenericType && !type.IsGenericTypeDefinition)
        {
            type = type.GetGenericTypeDefinition();
        }
        return (this._esForceTypes.IndexOf(type) >= 0);
    }

    private static string NormalizeFieldName(string fieldName)
    {
        if (string.IsNullOrEmpty(fieldName) || (fieldName[0] != '<'))
        {
            return fieldName;
        }
        int index = fieldName.IndexOf('>', 2);
        if (index >= 2)
        {
            return fieldName.Substring(1, index - 1);
        }
        return null;
    }

    public void ProcessMock()
    {
        Dictionary<Type, MockType>.Enumerator enumerator = this._typeDict.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<Type, MockType> current = enumerator.Current;
            MockType mt = current.Value;
            if (mt.canIStore)
            {
                if (!mt.type.IsNested)
                {
                    this.BuildTypeIStore(mt, null, null);
                }
                if (mt.hasStaticStore && mt.type.IsAbsolutePublic())
                {
                    if (mt.type.IsGenericType)
                    {
                        for (int i = 0; i < mt.geneTypeList.Count; i++)
                        {
                            string pathName = mt.geneTypeList[i].pathName;
                            this._gsEntryBuilder.AppendLine(string.Concat(new object[] { "gs.RegisterClassDelegate(", mt.geneTypeList[i].key, ", ", pathName, ".__StoreStatic, ", pathName, ".__RestoreStatic);" }), 0, true);
                        }
                    }
                    else
                    {
                        this._gsEntryBuilder.AppendLine(string.Concat(new object[] { "gs.RegisterClassDelegate(", mt.key, ", ", mt.pathName, ".__StoreStatic, ", mt.pathName, ".__RestoreStatic);" }), 0, true);
                    }
                }
            }
            else if (mt.canEStore)
            {
                this.BuildTypeEStore(mt);
            }
        }
    }

    public void RegisterControlAssembly(string name, Assembly assembly, string mockPath, int buildOrder)
    {
        this._controlAssemblyDict.Add(assembly, new AssemblyContext(name, assembly, mockPath, buildOrder));
    }

    private void RegisterDerivedTypes(MockType mt)
    {
        if ((mt != null) && mt.controlled)
        {
            if (mt.bdt == BaseDataType.Class)
            {
                List<Type> list = this.FindDescendTypes(mt.type);
                for (int i = 0; i < list.Count; i++)
                {
                    this.RegisterMockType(list[i], null);
                }
            }
            else if (mt.bdt == BaseDataType.Interface)
            {
                List<Type> list2 = this.FindImpledTypes(mt.type);
                for (int j = 0; j < list2.Count; j++)
                {
                    this.RegisterMockType(list2[j], null);
                }
            }
        }
    }

    private void RegisterMockFromType(MockType thisMT, MockType fromMT)
    {
        if (fromMT != null)
        {
            int count = thisMT.fromTypeList.Count;
            thisMT.AddFromType(fromMT);
            if (count == 0)
            {
                this.RegisterDerivedTypes(thisMT);
            }
        }
    }

    private MockType RegisterMockType(Type thisType, MockType fromMT)
    {
        if (this.SkipMockType(thisType))
        {
            return null;
        }
        MockType thisMT = null;
        if (this._typeDict.ContainsKey(thisType))
        {
            thisMT = this._typeDict[thisType];
            this.RegisterMockFromType(thisMT, fromMT);
            return thisMT;
        }
        thisMT = new MockType(thisType, !this._controlAssemblyDict.ContainsKey(thisType.Assembly) ? null : this._controlAssemblyDict[thisType.Assembly]);
        this._typeDict.Add(thisType, thisMT);
        this.RegisterMockFromType(thisMT, fromMT);
        if (thisType.IsArray)
        {
            this.RegisterMockType(thisType.GetElementType(), fromMT);
        }
        if (thisType.IsGenericType)
        {
            if (thisType.IsGenericTypeDefinition)
            {
                Type[] genericArguments = thisType.GetGenericArguments();
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    Type[] genericParameterConstraints = genericArguments[i].GetGenericParameterConstraints();
                    for (int j = 0; j < genericParameterConstraints.Length; j++)
                    {
                        this.RegisterMockType(genericParameterConstraints[j], thisMT);
                    }
                }
            }
            else
            {
                MockType type2 = this.RegisterMockType(thisType.GetGenericTypeDefinition(), null);
                if (type2 != null)
                {
                    type2.AddGeneType(thisMT);
                }
                Type[] typeArray3 = thisType.GetGenericArguments();
                for (int k = 0; k < typeArray3.Length; k++)
                {
                    this.RegisterMockType(typeArray3[k], fromMT);
                }
            }
        }
        if (thisMT.genericIStore)
        {
            if (this.IsControlledType(thisType.BaseType))
            {
                this.RegisterMockType(thisType.BaseType, null);
            }
            else if (((thisType.BaseType == null) || (thisType.BaseType == typeof(object))) || (thisType.BaseType != typeof(ValueType)))
            {
            }
            if (thisType.IsNested)
            {
                this.RegisterMockType(thisType.DeclaringType, null);
            }
            Type[] nestedTypes = thisType.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public);
            for (int m = 0; m < nestedTypes.Length; m++)
            {
                this.RegisterMockType(nestedTypes[m], null);
            }
            List<FieldInfo> fieldsWithFilter = this.GetFieldsWithFilter(thisType, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            bool flag = false;
            for (int n = 0; n < fieldsWithFilter.Count; n++)
            {
                FieldInfo memFld = fieldsWithFilter[n];
                if (this.ValidStoreField(memFld) != null)
                {
                    if (memFld.IsStatic)
                    {
                        flag = true;
                    }
                    this.RegisterMockType(memFld.FieldType, thisMT);
                }
            }
            thisMT.hasStaticStore = flag;
        }
        return thisMT;
    }

    private bool SkipMockType(Type thisType)
    {
        BaseDataType type = thisType.ToBDT();
        return (((((type != BaseDataType.Class) && (type != BaseDataType.Struct)) && ((type != BaseDataType.Interface) && (type != BaseDataType.Array))) && (type != BaseDataType.Delegate)) || this._skipMockTypes.Contains(thisType));
    }

    public void StartMock(Type[] rootTypes)
    {
        this._typeDict = new Dictionary<Type, MockType>();
        this._isCodeDict = new Dictionary<string, CodeContext>();
        this._gsEntryBuilder = new CodeContext("_global_", "C:/sgame_trunk/Project/Assets/Scripts/mock");
        this._gsEntryBuilder.AppendLine("public partial class GameStoreEntry {", 1, true);
        this._gsEntryBuilder.AppendLine("private GameStoreEntry(GameStore gs) {", 0, true);
        this._gsEntryBuilder.AppendLine("}", 0, false);
        this._gsEntryBuilder.AppendLine(-1, "}", false);
        this._gsEntryBuilder.MoveIndent(2);
        this._partialTypesOutput = new StringBuilder(0x80000);
        this._partialTypesOutput.AppendLine("C:/sgame_trunk/Project/Assets|8");
        this._mockSkipTypesOutput = new StringBuilder(0x2800);
        for (int i = 0; i < rootTypes.Length; i++)
        {
            this.RegisterMockType(rootTypes[i], null);
        }
        this.ProcessMock();
    }

    private string ValidStoreField(FieldInfo memFld)
    {
        string str = NormalizeFieldName(memFld.Name);
        if (((!memFld.IsLiteral && !memFld.IsInitOnly) && (!string.IsNullOrEmpty(str) && (str != "__refKey"))) && !memFld.IsDefined(typeof(ObsoleteAttribute), false))
        {
            return str;
        }
        return null;
    }

    public static GameMocker Instance
    {
        get
        {
            if (_instance == null)
            {
                GameStoreExtend.Start();
                _instance = new GameMocker();
            }
            return _instance;
        }
    }

    private class AssemblyContext
    {
        public readonly Assembly assembly;
        public readonly int buildOrder;
        public readonly CodeContext esPrivateCodeAuto;
        public readonly CodeContext esPrivateCodeManual;
        public readonly CodeContext esPublicCodeAuto;
        public readonly CodeContext esPublicCodeManual;
        public readonly string mockPath;
        public readonly string name;
        public readonly Type[] typeList;

        public AssemblyContext(string nam, Assembly asmb, string mckPth, int bldOdr)
        {
            this.name = nam;
            this.assembly = asmb;
            this.mockPath = mckPth;
            this.typeList = this.assembly.GetTypes();
            this.buildOrder = bldOdr;
            this.esPublicCodeAuto = new CodeContext("_global_", this.mockPath);
            this.esPublicCodeAuto.AddUseNs("Object = System.Object");
            this.esPublicCodeAuto.AddUseNs("System");
            this.esPublicCodeAuto.AppendLine("public static partial class " + this.StoreExtendName + " {", 0, true);
            this.esPublicCodeAuto.AppendLine("}", 0, false);
            this.esPrivateCodeAuto = new CodeContext("_global_", this.mockPath);
            this.esPrivateCodeAuto.AddUseNs("Object = System.Object");
            this.esPrivateCodeAuto.AddUseNs("System");
            this.esPublicCodeManual = new CodeContext("_global_", this.mockPath);
            this.esPublicCodeManual.AddUseNs("Object = System.Object");
            this.esPublicCodeManual.AddUseNs("System");
            this.esPublicCodeManual.AppendLine("public static partial class " + this.StoreExtendName + " {", 0, true);
            this.esPublicCodeManual.AppendLine("}", 0, false);
            this.esPrivateCodeManual = new CodeContext("_global_", this.mockPath);
            this.esPrivateCodeManual.AddUseNs("Object = System.Object");
            this.esPrivateCodeManual.AddUseNs("System");
        }

        public string StoreExtendName
        {
            get
            {
                return ("StoreExtend" + this.name);
            }
        }
    }

    private class MockType
    {
        private GameMocker.AssemblyContext _buildinAssembly;
        public bool autoExtend;
        public readonly BaseDataType bdt;
        public readonly bool canEStore;
        public readonly bool canIStore;
        public readonly GameMocker.AssemblyContext controlledAssembly;
        public CodeContext extendCode;
        public readonly List<GameMocker.MockType> fromTypeList;
        public readonly bool genericIStore;
        public readonly List<GameMocker.MockType> geneTypeList;
        public readonly GameTypeAttr gta;
        public bool hasStaticStore;
        public readonly ushort key;
        public readonly string pathName;
        public readonly bool publicEStore;
        public readonly string thisName;
        public readonly Type type;

        public MockType(Type _type, GameMocker.AssemblyContext ca)
        {
            this.type = _type;
            this.controlledAssembly = ca;
            this.gta = GameStore.Instance.FindRegisterType(_type);
            this.key = (this.gta == null) ? GameStore.Instance.GenTypeKey() : this.gta.key;
            this.bdt = this.type.ToBDT();
            this.pathName = this.type.GetPathName(true);
            if (this.type.IsNested)
            {
                string pathName = this.type.DeclaringType.GetPathName(true);
                this.thisName = this.pathName.Substring(pathName.Length + 1);
            }
            else if (string.IsNullOrEmpty(this.nameSpace))
            {
                this.thisName = this.pathName;
            }
            else
            {
                this.thisName = this.pathName.Substring(this.nameSpace.Length + 1);
            }
            bool flag = GameMocker.Instance.IsForceESType(this.type);
            bool flag2 = IsStoreBDT(this.bdt);
            this.canIStore = (this.controlled && flag2) && !flag;
            this.genericIStore = this.canIStore && (!this.type.IsGenericType || this.type.IsGenericTypeDefinition);
            this.canEStore = !this.canIStore && !this.type.ContainsGenericParameters;
            this.publicEStore = this.canEStore && this.type.IsAbsolutePublic();
            this._buildinAssembly = null;
            this.extendCode = null;
            this.fromTypeList = new List<GameMocker.MockType>();
            this.geneTypeList = new List<GameMocker.MockType>();
            this.hasStaticStore = false;
            this.autoExtend = false;
        }

        public void AddFromType(GameMocker.MockType mt)
        {
            if ((mt != null) && (this.fromTypeList.IndexOf(mt) < 0))
            {
                this.fromTypeList.Add(mt);
                if ((this._buildinAssembly == null) || ((mt.controlledAssembly != null) && (mt.controlledAssembly.buildOrder < this._buildinAssembly.buildOrder)))
                {
                    this._buildinAssembly = mt.controlledAssembly;
                }
            }
        }

        public void AddGeneType(GameMocker.MockType mt)
        {
            if ((((mt != null) && mt.type.IsGenericType) && (!mt.type.ContainsGenericParameters && (mt.type.GetGenericTypeDefinition() == this.type))) && (this.geneTypeList.IndexOf(mt) < 0))
            {
                this.geneTypeList.Add(mt);
            }
        }

        public static bool IsStoreBDT(BaseDataType bdt)
        {
            return (((bdt == BaseDataType.Class) || (bdt == BaseDataType.Struct)) || (bdt == BaseDataType.Interface));
        }

        public GameMocker.AssemblyContext buildinAssembly
        {
            get
            {
                return this._buildinAssembly;
            }
        }

        public bool controlled
        {
            get
            {
                return (null != this.controlledAssembly);
            }
        }

        public string nameSpace
        {
            get
            {
                return this.type.Namespace;
            }
        }

        public string pathDesc
        {
            get
            {
                return this.type.GetPathName(false);
            }
        }

        public string pureDesc
        {
            get
            {
                if (!this.type.IsGenericType)
                {
                    return this.thisName;
                }
                string str = string.Empty;
                Type[] genericArguments = this.type.GetGenericArguments();
                for (int i = 1; i < genericArguments.Length; i++)
                {
                    str = str + ",";
                }
                return (this.thisName.Substring(0, this.thisName.IndexOf('<')) + "<" + str + ">");
            }
        }

        public string pureName
        {
            get
            {
                if (this.type.IsGenericType)
                {
                    return this.thisName.Substring(0, this.thisName.IndexOf('<'));
                }
                return this.thisName;
            }
        }
    }
}

