namespace behaviac
{
    using Mono.Xml;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Text;
    using System.Threading;

    public static class Workspace
    {
        [CompilerGenerated]
        private static Comparison<TypeInfo_t> <>f__am$cacheC;
        private static EFileFormat fileFormat_ = EFileFormat.EFF_xml;
        private static uint m_frame;
        private static string m_workspaceFileAbs = string.Empty;
        private static List<TypeInfo_t> ms_agentTypes = new List<TypeInfo_t>();
        private static DictionaryView<string, BTItem_t> ms_allBehaviorTreeTasks = new DictionaryView<string, BTItem_t>();
        private static DictionaryView<string, System.Type> ms_behaviorNodeTypes;
        private static DictionaryView<string, BehaviorTree> ms_behaviortrees;
        private static DictionaryView<string, MethodInfo> ms_btCreators;
        private static int ms_deltaFrames;
        private static string ms_workspaceExportPath;

        private static  event BehaviorNodeLoadedHandler_t OnNodeLoaded;

        public static  event DRespondToBreakHandler RespondToBreakHandler;

        public static BehaviorNode CreateBehaviorNode(string className)
        {
            if (ms_behaviorNodeTypes.ContainsKey(className))
            {
                System.Type type = ms_behaviorNodeTypes[className];
                return (Activator.CreateInstance(type) as BehaviorNode);
            }
            return null;
        }

        public static BehaviorTreeTask CreateBehaviorTreeTask(string relativePath)
        {
            BehaviorTree tree = null;
            if (BehaviorTrees.ContainsKey(relativePath))
            {
                tree = BehaviorTrees[relativePath];
            }
            else if (Load(relativePath))
            {
                tree = BehaviorTrees[relativePath];
            }
            if (tree == null)
            {
                return null;
            }
            BehaviorTreeTask item = tree.CreateAndInitTask() as BehaviorTreeTask;
            if (!ms_allBehaviorTreeTasks.ContainsKey(relativePath))
            {
                ms_allBehaviorTreeTasks[relativePath] = new BTItem_t();
            }
            BTItem_t _t = ms_allBehaviorTreeTasks[relativePath];
            if (!_t.bts.Contains(item))
            {
                _t.bts.Add(item);
            }
            return item;
        }

        public static void DestroyBehaviorTreeTask(BehaviorTreeTask behaviorTreeTask, Agent agent)
        {
            if (behaviorTreeTask != null)
            {
                if (ms_allBehaviorTreeTasks.ContainsKey(behaviorTreeTask.GetName()))
                {
                    BTItem_t _t = ms_allBehaviorTreeTasks[behaviorTreeTask.GetName()];
                    _t.bts.Remove(behaviorTreeTask);
                    if (agent != null)
                    {
                        _t.agents.Remove(agent);
                    }
                }
                BehaviorTask.DestroyTask(behaviorTreeTask);
            }
        }

        public static bool ExportMetas(string filePath)
        {
            return ExportMetas(filePath, false);
        }

        public static bool ExportMetas(string filePath, bool onlyExportPublicMembers)
        {
            return false;
        }

        public static bool GetAutoHotReload()
        {
            return false;
        }

        public static DictionaryView<string, BehaviorTree> GetBehaviorTrees()
        {
            return ms_behaviortrees;
        }

        public static int GetDeltaFrames()
        {
            return ms_deltaFrames;
        }

        public static string GetWorkspaceAbsolutePath()
        {
            return m_workspaceFileAbs;
        }

        public static void HandleNodeLoaded(string nodeType, List<property_t> properties)
        {
            if (OnNodeLoaded != null)
            {
                OnNodeLoaded(nodeType, properties);
            }
        }

        public static bool HandleRequests()
        {
            return false;
        }

        public static void HotReload()
        {
        }

        private static bool IsRegisterd(System.Type type)
        {
            <IsRegisterd>c__AnonStorey8A storeya = new <IsRegisterd>c__AnonStorey8A {
                type = type
            };
            return (ms_agentTypes.FindIndex(new Predicate<TypeInfo_t>(storeya.<>m__96)) != -1);
        }

        public static bool IsValidPath(string relativePath)
        {
            if ((relativePath[0] == '.') && ((relativePath[1] == '/') || (relativePath[1] == '\\')))
            {
                return false;
            }
            return ((relativePath[0] != '/') && (relativePath[0] != '\\'));
        }

        public static bool Load(string relativePath)
        {
            return Load(relativePath, false);
        }

        public static bool Load(string relativePath, bool bForce)
        {
            BehaviorTree tree = null;
            if (BehaviorTrees.ContainsKey(relativePath))
            {
                if (!bForce)
                {
                    return true;
                }
                tree = BehaviorTrees[relativePath];
            }
            string filePath = WorkspaceExportPath + relativePath;
            string ext = string.Empty;
            EFileFormat fileFormat = FileFormat;
            switch (fileFormat)
            {
                case EFileFormat.EFF_xml:
                    ext = ".xml";
                    break;

                case EFileFormat.EFF_default:
                    ext = ".xml";
                    if (!FileManager.Instance.FileExist(filePath, ext))
                    {
                        ext = ".bson";
                        if (FileManager.Instance.FileExist(filePath, ext))
                        {
                            throw new NotImplementedException("bson support has been removed!!!");
                        }
                        fileFormat = EFileFormat.EFF_cs;
                        break;
                    }
                    fileFormat = EFileFormat.EFF_xml;
                    break;
            }
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            if (tree == null)
            {
                flag3 = true;
                tree = new BehaviorTree();
                BehaviorTrees[relativePath] = tree;
            }
            switch (fileFormat)
            {
                case EFileFormat.EFF_xml:
                {
                    byte[] pBuffer = ReadFileToBuffer(filePath, ext);
                    if (pBuffer != null)
                    {
                        if (!flag3)
                        {
                            flag2 = true;
                            tree.Clear();
                        }
                        if (fileFormat == EFileFormat.EFF_xml)
                        {
                            flag = tree.load_xml(pBuffer);
                        }
                        PopFileFromBuffer(filePath, ext, pBuffer);
                    }
                    break;
                }
                case EFileFormat.EFF_cs:
                    if (!flag3)
                    {
                        flag2 = true;
                        tree.Clear();
                    }
                    try
                    {
                        MethodInfo method = null;
                        if (BTCreators.ContainsKey(relativePath))
                        {
                            method = BTCreators[relativePath];
                        }
                        else
                        {
                            System.Type type = Utils.GetType("behaviac.bt_" + relativePath.Replace("/", "_"));
                            if (type != null)
                            {
                                method = type.GetMethod("build_behavior_tree", BindingFlags.Public | BindingFlags.Static);
                                if (method != null)
                                {
                                    BTCreators[relativePath] = method;
                                }
                            }
                        }
                        if (method != null)
                        {
                            object[] parameters = new object[] { tree };
                            flag = (bool) method.Invoke(null, parameters);
                        }
                    }
                    catch (Exception exception)
                    {
                        string str4 = string.Format("The behavior {0} failed to be loaded : {1}", relativePath, exception.Message);
                    }
                    break;
            }
            if (flag)
            {
                if (!flag3)
                {
                }
                return flag;
            }
            if (flag3)
            {
                bool flag4 = BehaviorTrees.Remove(relativePath);
                return flag;
            }
            if (flag2)
            {
                BehaviorTrees.Remove(relativePath);
            }
            return flag;
        }

        public static void LoadWorkspaceAbsolutePath()
        {
        }

        private static bool LoadWorkspaceSetting(string file, string ext, ref string workspaceFile)
        {
            try
            {
                byte[] bytes = ReadFileToBuffer(file, ext);
                if (bytes != null)
                {
                    string xml = Encoding.UTF8.GetString(bytes);
                    Mono.Xml.SecurityParser parser = new Mono.Xml.SecurityParser();
                    parser.LoadXml(xml);
                    SecurityElement element = parser.ToXml();
                    if (element.Tag == "workspace")
                    {
                        workspaceFile = element.Attribute("path");
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                string str2 = string.Format("Load Workspace {0} Error : {1}", file, exception.Message);
            }
            return false;
        }

        public static void LogFrames()
        {
            object[] args = new object[] { m_frame++ };
            LogManager.Log("[frame]{0}\n", args);
        }

        private static void PopFileFromBuffer(string file, string ext, byte[] pBuffer)
        {
            FileManager.Instance.FileClose(file, ext, pBuffer);
        }

        private static byte[] ReadFileToBuffer(string file, string ext)
        {
            return FileManager.Instance.FileOpen(file, ext);
        }

        public static void RecordBTAgentMapping(string relativePath, Agent agent)
        {
            if (ms_allBehaviorTreeTasks == null)
            {
                ms_allBehaviorTreeTasks = new DictionaryView<string, BTItem_t>();
            }
            if (!ms_allBehaviorTreeTasks.ContainsKey(relativePath))
            {
                ms_allBehaviorTreeTasks[relativePath] = new BTItem_t();
            }
            BTItem_t _t = ms_allBehaviorTreeTasks[relativePath];
            if (_t.agents.IndexOf(agent) == -1)
            {
                _t.agents.Add(agent);
            }
        }

        public static void RegisterBehaviorNode()
        {
            RegisterBehaviorNode(Assembly.GetCallingAssembly());
        }

        public static void RegisterBehaviorNode(Assembly a)
        {
            if (ms_behaviorNodeTypes == null)
            {
                ms_behaviorNodeTypes = new DictionaryView<string, System.Type>();
            }
            foreach (System.Type type in a.GetTypes())
            {
                if (type.IsSubclassOf(typeof(BehaviorNode)) && !type.IsAbstract)
                {
                    ms_behaviorNodeTypes[type.Name] = type;
                }
            }
        }

        private static void RegisterMetas()
        {
            RegisterMetas(Assembly.GetCallingAssembly());
        }

        private static void RegisterMetas(Assembly a)
        {
            ListView<System.Type> view = new ListView<System.Type>();
            foreach (System.Type type in a.GetTypes())
            {
                if ((type.IsSubclassOf(typeof(Agent)) || Utils.IsStaticType(type)) && !IsRegisterd(type))
                {
                    Attribute[] customAttributes = (Attribute[]) type.GetCustomAttributes(typeof(TypeMetaInfoAttribute), false);
                    if (customAttributes.Length > 0)
                    {
                        TypeInfo_t item = new TypeInfo_t {
                            type = type
                        };
                        ms_agentTypes.Add(item);
                        if ((type.BaseType != null) && ((type.BaseType == typeof(Agent)) || type.BaseType.IsSubclassOf(typeof(Agent))))
                        {
                            view.Add(type.BaseType);
                        }
                        if (Utils.IsStaticType(type))
                        {
                            TypeMetaInfoAttribute attribute = customAttributes[0] as TypeMetaInfoAttribute;
                            Agent.RegisterStaticClass(type, attribute.DisplayName, attribute.Description);
                        }
                    }
                }
            }
            <RegisterMetas>c__AnonStorey8B storeyb = new <RegisterMetas>c__AnonStorey8B();
            using (ListView<System.Type>.Enumerator enumerator = view.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    storeyb.type = enumerator.Current;
                    if (!IsRegisterd(storeyb.type) && (ms_agentTypes.Find(new Predicate<TypeInfo_t>(storeyb.<>m__97)) == null))
                    {
                        TypeInfo_t _t3 = new TypeInfo_t {
                            type = storeyb.type,
                            bIsInherited = true
                        };
                        ms_agentTypes.Add(_t3);
                    }
                }
            }
            if (<>f__am$cacheC == null)
            {
                <>f__am$cacheC = delegate (TypeInfo_t x, TypeInfo_t y) {
                    if (x.bIsInherited && !y.bIsInherited)
                    {
                        return -1;
                    }
                    if (!x.bIsInherited && y.bIsInherited)
                    {
                        return 1;
                    }
                    if (x.type.IsSubclassOf(y.type))
                    {
                        return 1;
                    }
                    if (y.type.IsSubclassOf(x.type))
                    {
                        return -1;
                    }
                    return x.type.FullName.CompareTo(y.type.FullName);
                };
            }
            ms_agentTypes.Sort(<>f__am$cacheC);
            foreach (TypeInfo_t _t4 in ms_agentTypes)
            {
                RegisterType(_t4.type, true);
            }
        }

        private static void RegisterType(System.Type type, bool bIsAgentType)
        {
            Attribute[] customAttributes = (Attribute[]) type.GetCustomAttributes(typeof(TypeMetaInfoAttribute), false);
            if (!bIsAgentType || (customAttributes.Length > 0))
            {
                TypeMetaInfoAttribute attribute = (customAttributes.Length <= 0) ? null : ((TypeMetaInfoAttribute) customAttributes[0]);
                Agent.CTagObjectDescriptor descriptorByName = Agent.GetDescriptorByName(type.FullName);
                if ((type.BaseType == typeof(Agent)) || type.BaseType.IsSubclassOf(typeof(Agent)))
                {
                    Agent.CTagObjectDescriptor descriptor2 = Agent.GetDescriptorByName(type.BaseType.FullName);
                    descriptorByName.m_parent = descriptor2;
                }
                descriptorByName.type = type;
                descriptorByName.displayName = ((attribute != null) && !string.IsNullOrEmpty(attribute.DisplayName)) ? attribute.DisplayName : type.FullName;
                descriptorByName.desc = ((attribute != null) && !string.IsNullOrEmpty(attribute.Description)) ? attribute.Description : descriptorByName.displayName;
                if (!Utils.IsEnumType(type))
                {
                    BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                    foreach (FieldInfo info in type.GetFields(bindingAttr))
                    {
                        bool flag = false;
                        MemberMetaInfoAttribute a = null;
                        if (bIsAgentType)
                        {
                            Attribute[] attributeArray2 = (Attribute[]) info.GetCustomAttributes(typeof(MemberMetaInfoAttribute), false);
                            if (attributeArray2.Length > 0)
                            {
                                a = (MemberMetaInfoAttribute) attributeArray2[0];
                                flag = true;
                            }
                        }
                        else
                        {
                            flag = true;
                        }
                        if (flag)
                        {
                            CMemberBase item = new CMemberBase(info, a);
                            CMemberBase base3 = null;
                            for (int i = 0; i < descriptorByName.ms_members.Count; i++)
                            {
                                if (item.GetId() == descriptorByName.ms_members[i].GetId())
                                {
                                    base3 = descriptorByName.ms_members[i];
                                    break;
                                }
                            }
                            descriptorByName.ms_members.Add(item);
                            if ((Utils.IsCustomClassType(info.FieldType) || Utils.IsEnumType(info.FieldType)) && !Agent.IsTypeRegisterd(info.FieldType.FullName))
                            {
                                RegisterType(info.FieldType, false);
                            }
                        }
                    }
                    if (bIsAgentType)
                    {
                        foreach (MethodInfo info2 in type.GetMethods(bindingAttr))
                        {
                            Attribute[] attributeArray3 = (Attribute[]) info2.GetCustomAttributes(typeof(MethodMetaInfoAttribute), false);
                            if (attributeArray3.Length > 0)
                            {
                                MethodMetaInfoAttribute attribute3 = (MethodMetaInfoAttribute) attributeArray3[0];
                                CMethodBase base4 = new CMethodBase(info2, attribute3, null);
                                descriptorByName.ms_methods.Add(base4);
                                foreach (ParameterInfo info3 in info2.GetParameters())
                                {
                                    if ((Utils.IsCustomClassType(info3.ParameterType) || Utils.IsEnumType(info3.ParameterType)) && !Agent.IsTypeRegisterd(info3.ParameterType.FullName))
                                    {
                                        RegisterType(info3.ParameterType, false);
                                    }
                                }
                                if ((Utils.IsCustomClassType(info2.ReturnType) || Utils.IsEnumType(info2.ReturnType)) && !Agent.IsTypeRegisterd(info2.ReturnType.FullName))
                                {
                                    RegisterType(info2.ReturnType, false);
                                }
                            }
                        }
                        foreach (System.Type type2 in type.GetNestedTypes(bindingAttr))
                        {
                            Attribute[] attributeArray4 = (Attribute[]) type2.GetCustomAttributes(typeof(EventMetaInfoAttribute), false);
                            if (attributeArray4.Length > 0)
                            {
                                EventMetaInfoAttribute attribute4 = (EventMetaInfoAttribute) attributeArray4[0];
                                CNamedEvent event2 = new CNamedEvent(type2.GetMethod("Invoke"), attribute4, type2.Name);
                                descriptorByName.ms_methods.Add(event2);
                            }
                        }
                    }
                }
            }
        }

        public static void RespondToBreak(string msg, string title)
        {
            if (RespondToBreakHandler != null)
            {
                RespondToBreakHandler(msg, title);
            }
            else
            {
                WaitforContinue();
                Thread.Sleep(500);
            }
        }

        public static void SetAutoHotReload(bool enable)
        {
        }

        public static void SetDeltaFrames(int deltaFrames)
        {
            ms_deltaFrames = deltaFrames;
        }

        public static bool SetWorkspaceSettings(string workspaceExportPath)
        {
            return SetWorkspaceSettings(workspaceExportPath, EFileFormat.EFF_xml);
        }

        public static bool SetWorkspaceSettings(string workspaceExportPath, EFileFormat format)
        {
            bool flag = string.IsNullOrEmpty(ms_workspaceExportPath);
            ms_workspaceExportPath = workspaceExportPath;
            if (!ms_workspaceExportPath.EndsWith("/"))
            {
                ms_workspaceExportPath = ms_workspaceExportPath + '/';
            }
            fileFormat_ = format;
            if (string.IsNullOrEmpty(ms_workspaceExportPath))
            {
                return false;
            }
            LoadWorkspaceAbsolutePath();
            ms_deltaFrames = 1;
            if (flag)
            {
                Details.RegisterCompareValue();
                Details.RegisterComputeValue();
                RegisterBehaviorNode();
                RegisterMetas();
            }
            return true;
        }

        public static void UnLoad(string relativePath)
        {
            if (BehaviorTrees.ContainsKey(relativePath))
            {
                BehaviorTrees.Remove(relativePath);
            }
        }

        public static void UnLoadAll()
        {
            ms_allBehaviorTreeTasks.Clear();
            BehaviorTrees.Clear();
            BTCreators.Clear();
        }

        public static void WaitforContinue()
        {
        }

        private static DictionaryView<string, BehaviorTree> BehaviorTrees
        {
            get
            {
                if (ms_behaviortrees == null)
                {
                    ms_behaviortrees = new DictionaryView<string, BehaviorTree>();
                }
                return ms_behaviortrees;
            }
        }

        private static DictionaryView<string, MethodInfo> BTCreators
        {
            get
            {
                if (ms_btCreators == null)
                {
                    ms_btCreators = new DictionaryView<string, MethodInfo>();
                }
                return ms_btCreators;
            }
        }

        public static EFileFormat FileFormat
        {
            get
            {
                return fileFormat_;
            }
            set
            {
                fileFormat_ = value;
            }
        }

        public static string WorkspaceExportPath
        {
            get
            {
                return ms_workspaceExportPath;
            }
        }

        [CompilerGenerated]
        private sealed class <IsRegisterd>c__AnonStorey8A
        {
            internal System.Type type;

            internal bool <>m__96(Workspace.TypeInfo_t t)
            {
                return (t.type == this.type);
            }
        }

        [CompilerGenerated]
        private sealed class <RegisterMetas>c__AnonStorey8B
        {
            internal System.Type type;

            internal bool <>m__97(Workspace.TypeInfo_t t)
            {
                return (t.type == this.type);
            }
        }

        private delegate void BehaviorNodeLoadedHandler_t(string nodeType, List<property_t> properties);

        private class BreakpointInfo_t
        {
            public EActionResult action_result = EActionResult.EAR_all;
            public ushort hit_config = 0;
        }

        private class BTItem_t
        {
            public ListView<Agent> agents = new ListView<Agent>();
            public ListView<BehaviorTreeTask> bts = new ListView<BehaviorTreeTask>();
        }

        public delegate void DRespondToBreakHandler(string msg, string title);

        [Flags]
        public enum EFileFormat
        {
            EFF_cs = 4,
            EFF_default = 5,
            EFF_xml = 1
        }

        private class TypeInfo_t
        {
            public bool bIsInherited;
            public System.Type type;
        }
    }
}

