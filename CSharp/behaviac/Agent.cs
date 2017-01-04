namespace behaviac
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class Agent : MonoBehaviour
    {
        private bool m_bActive = true;
        private ListView<BehaviorTreeTask> m_behaviorTreeTasks;
        private ListView<BehaviorTreeStackItem_t> m_btStack;
        public int m_contextId;
        private BehaviorTreeTask m_currentBT;
        private DictionaryView<CStringID, CNamedEvent> m_eventInfos;
        private int m_id = -1;
        private uint m_idFlag;
        private CTagObjectDescriptor m_objectDescriptor;
        public int m_priority;
        private bool m_referencetree;
        public behaviac.Variables m_variables;
        private static int ms_agent_index;
        private static Dictionary<string, int> ms_agent_type_index;
        private static uint ms_idMask;
        private static DictionaryView<CStringID, CTagObjectDescriptor> ms_metas;
        private static Dictionary<string, AgentName_t> ms_names;

        private void _btsetcurrent(string relativePath, TriggerMode triggerMode, bool bByEvent)
        {
            if (!string.IsNullOrEmpty(relativePath))
            {
                if (!Workspace.Load(relativePath))
                {
                    string str = base.GetType().FullName + "::" + base.name;
                }
                else
                {
                    Workspace.RecordBTAgentMapping(relativePath, this);
                    if (this.m_currentBT != null)
                    {
                        if (triggerMode == TriggerMode.TM_Return)
                        {
                            BehaviorTreeStackItem_t _t = new BehaviorTreeStackItem_t(this.m_currentBT, triggerMode, bByEvent);
                            this.BTStack.Add(_t);
                        }
                        else if (triggerMode == TriggerMode.TM_Transfer)
                        {
                            this.m_currentBT.abort(this);
                            this.m_currentBT.reset(this);
                        }
                    }
                    BehaviorTreeTask item = null;
                    for (int i = 0; i < this.BehaviorTreeTasks.Count; i++)
                    {
                        BehaviorTreeTask task2 = this.BehaviorTreeTasks[i];
                        if (task2.GetName() == relativePath)
                        {
                            item = task2;
                            break;
                        }
                    }
                    bool flag3 = false;
                    if ((item != null) && (this.BTStack.Count > 0))
                    {
                        for (int j = 0; j < this.BTStack.Count; j++)
                        {
                            BehaviorTreeStackItem_t _t2 = this.BTStack[j];
                            if (_t2.bt.GetName() == relativePath)
                            {
                                flag3 = true;
                                break;
                            }
                        }
                    }
                    if ((item == null) || flag3)
                    {
                        item = Workspace.CreateBehaviorTreeTask(relativePath);
                        this.BehaviorTreeTasks.Add(item);
                    }
                    this.m_currentBT = item;
                }
            }
        }

        public static bool BindInstance(Agent pAgentInstance)
        {
            return BindInstance(pAgentInstance, null, 0);
        }

        public static bool BindInstance(Agent pAgentInstance, string agentInstanceName)
        {
            return BindInstance(pAgentInstance, agentInstanceName, 0);
        }

        public static bool BindInstance(Agent pAgentInstance, string agentInstanceName, int contextId)
        {
            return Context.GetContext(contextId).BindInstance(pAgentInstance, agentInstanceName);
        }

        public void bteventtree(string relativePath, TriggerMode triggerMode)
        {
            this._btsetcurrent(relativePath, triggerMode, true);
        }

        public virtual EBTStatus btexec()
        {
            if (!this.m_bActive)
            {
                return EBTStatus.BT_INVALID;
            }
            this.UpdateVariableRegistry();
            EBTStatus status = this.btexec_();
            while (this.m_referencetree && (status == EBTStatus.BT_RUNNING))
            {
                this.m_referencetree = false;
                status = this.btexec_();
            }
            if (this.IsMasked())
            {
                this.LogVariables(false);
            }
            return status;
        }

        private EBTStatus btexec_()
        {
            if (this.m_currentBT == null)
            {
                return EBTStatus.BT_INVALID;
            }
            EBTStatus status = this.m_currentBT.exec(this);
            if (this == null)
            {
                return EBTStatus.BT_FAILURE;
            }
            while (status != EBTStatus.BT_RUNNING)
            {
                this.m_currentBT.reset(this);
                if (this.BTStack.Count <= 0)
                {
                    return status;
                }
                BehaviorTreeStackItem_t _t = this.BTStack[this.BTStack.Count - 1];
                this.m_currentBT = _t.bt;
                this.BTStack.RemoveAt(this.BTStack.Count - 1);
                if (_t.triggerMode == TriggerMode.TM_Return)
                {
                    string name = this.m_currentBT.GetName();
                    LogManager.Log(this, name, EActionResult.EAR_none, LogMode.ELM_return);
                    if (!_t.triggerByEvent)
                    {
                        this.m_currentBT.resume(this, status);
                        status = this.m_currentBT.exec(this);
                    }
                }
                else
                {
                    return this.m_currentBT.exec(this);
                }
            }
            return status;
        }

        public BehaviorTreeTask btgetcurrent()
        {
            return this.m_currentBT;
        }

        public virtual void bthotreloaded(BehaviorTree bt)
        {
            this.btunload_pars(bt);
        }

        public bool btload(State_t state)
        {
            state.Vars.CopyTo(this, this.m_variables);
            if (state.BT == null)
            {
                return false;
            }
            if (this.m_currentBT != null)
            {
                for (int i = 0; i < this.m_behaviorTreeTasks.Count; i++)
                {
                    BehaviorTreeTask behaviorTreeTask = this.m_behaviorTreeTasks[i];
                    if (behaviorTreeTask == this.m_currentBT)
                    {
                        Workspace.DestroyBehaviorTreeTask(behaviorTreeTask, this);
                        this.m_behaviorTreeTasks.Remove(behaviorTreeTask);
                        break;
                    }
                }
            }
            BehaviorNode node = state.BT.GetNode();
            this.m_currentBT = (BehaviorTreeTask) node.CreateAndInitTask();
            state.BT.CopyTo(this.m_currentBT);
            return true;
        }

        public bool btload(string relativePath)
        {
            return this.btload(relativePath, false);
        }

        public bool btload(string relativePath, bool bForce)
        {
            bool flag = Workspace.Load(relativePath, bForce);
            if (flag)
            {
                Workspace.RecordBTAgentMapping(relativePath, this);
            }
            return flag;
        }

        public void btonevent(string btEvent)
        {
            if (this.m_currentBT != null)
            {
                this.m_currentBT.onevent(this, btEvent);
            }
        }

        public void btreferencetree(string relativePath)
        {
            this._btsetcurrent(relativePath, TriggerMode.TM_Return, false);
            this.m_referencetree = true;
        }

        public void btreloadall()
        {
            this.m_currentBT = null;
            this.BTStack.Clear();
            if (this.m_behaviorTreeTasks != null)
            {
                List<string> list = new List<string>();
                foreach (BehaviorTreeTask task in this.m_behaviorTreeTasks)
                {
                    string name = task.GetName();
                    if (list.IndexOf(name) == -1)
                    {
                        list.Add(name);
                    }
                }
                for (int i = 0; i < list.Count; i++)
                {
                    string relativePath = list[i];
                    Workspace.Load(relativePath, true);
                }
                this.BehaviorTreeTasks.Clear();
            }
            this.Variables.Unload();
        }

        public void btresetcurrrent()
        {
            if (this.m_currentBT != null)
            {
                this.m_currentBT.reset(this);
            }
        }

        public bool btsave(State_t state)
        {
            this.m_variables.CopyTo(null, state.Vars);
            if (this.m_currentBT != null)
            {
                Workspace.DestroyBehaviorTreeTask(state.BT, this);
                BehaviorNode node = this.m_currentBT.GetNode();
                state.BT = (BehaviorTreeTask) node.CreateAndInitTask();
                this.m_currentBT.CopyTo(state.BT);
                return true;
            }
            return false;
        }

        public void btsetcurrent(string relativePath)
        {
            this._btsetcurrent(relativePath, TriggerMode.TM_Transfer, false);
        }

        public void btunload(string relativePath)
        {
            if ((this.m_currentBT != null) && (this.m_currentBT.GetName() == relativePath))
            {
                BehaviorTree node = this.m_currentBT.GetNode() as BehaviorTree;
                this.btunload_pars(node);
                this.m_currentBT = null;
            }
            for (int i = 0; i < this.BTStack.Count; i++)
            {
                BehaviorTreeStackItem_t item = this.BTStack[i];
                if (item.bt.GetName() == relativePath)
                {
                    this.BTStack.Remove(item);
                    break;
                }
            }
            for (int j = 0; j < this.BehaviorTreeTasks.Count; j++)
            {
                BehaviorTreeTask behaviorTreeTask = this.BehaviorTreeTasks[j];
                if (behaviorTreeTask.GetName() == relativePath)
                {
                    Workspace.DestroyBehaviorTreeTask(behaviorTreeTask, this);
                    this.BehaviorTreeTasks.Remove(behaviorTreeTask);
                    break;
                }
            }
            Workspace.UnLoad(relativePath);
        }

        private void btunload_pars(BehaviorTree bt)
        {
            if (bt.m_pars != null)
            {
                for (int i = 0; i < bt.m_pars.Count; i++)
                {
                    bt.m_pars[i].UnLoad(this);
                }
            }
        }

        public void btunloadall()
        {
            ListView<BehaviorTree> view = new ListView<BehaviorTree>();
            foreach (BehaviorTreeTask task in this.BehaviorTreeTasks)
            {
                BehaviorTree node = (BehaviorTree) task.GetNode();
                bool flag = false;
                foreach (BehaviorTree tree2 in view)
                {
                    if (tree2 == node)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    view.Add(node);
                }
                Workspace.DestroyBehaviorTreeTask(task, this);
            }
            foreach (BehaviorTree tree3 in view)
            {
                this.btunload_pars(tree3);
                Workspace.UnLoad(tree3.GetName());
            }
            this.BehaviorTreeTasks.Clear();
            this.m_currentBT = null;
            this.BTStack.Clear();
            this.Variables.Unload();
        }

        public static CMethodBase CreateMethod(CStringID agentClassId, CStringID methodClassId)
        {
            CMethodBase base2 = FindMethodBase(agentClassId, methodClassId);
            if (base2 != null)
            {
                return base2.clone();
            }
            return null;
        }

        public static Property CreateProperty(string typeName, string propertyName, string defaultValue)
        {
            CMemberBase base2 = FindMemberBase(propertyName);
            if (base2 != null)
            {
                return base2.CreateProperty(defaultValue, false);
            }
            return null;
        }

        private void CustomSetActive(bool bActive)
        {
            this.m_bActive = bActive;
        }

        private CNamedEvent findEvent(string eventName)
        {
            CTagObjectDescriptor descriptor = this.GetDescriptor();
            int contextId = this.GetContextId();
            CNamedEvent event2 = findNamedEventTemplate(descriptor.ms_methods, eventName, contextId);
            if (event2 != null)
            {
                CNamedEvent event3 = (CNamedEvent) event2.clone();
                CStringID gid = new CStringID(eventName);
                this.EventInfos[gid] = event3;
                return event3;
            }
            return null;
        }

        public CMemberBase FindMember(string propertyName)
        {
            uint propertyId = Utils.MakeVariableId(propertyName);
            return this.FindMember(propertyId);
        }

        public CMemberBase FindMember(uint propertyId)
        {
            CTagObjectDescriptor descriptor = this.GetDescriptor();
            CMemberBase base2 = null;
            for (int i = 0; i < descriptor.ms_members.Count; i++)
            {
                base2 = descriptor.ms_members[i];
                if (base2.GetId().GetId() == propertyId)
                {
                    return base2;
                }
            }
            return null;
        }

        public static CMemberBase FindMemberBase(string propertyName)
        {
            string agentClassName = null;
            int length = ParsePropertyNames(propertyName, ref agentClassName);
            if (length != -1)
            {
                string str = propertyName.Substring(0, length).Replace("::", ".");
                CStringID agentClassId = new CStringID(str);
                CStringID propertyId = new CStringID(agentClassName);
                return FindMemberBase(agentClassId, propertyId);
            }
            return null;
        }

        public static CMemberBase FindMemberBase(CStringID agentClassId, CStringID propertyId)
        {
            if (Metas.ContainsKey(agentClassId))
            {
                CTagObjectDescriptor descriptor = Metas[agentClassId];
                for (int i = 0; i < descriptor.ms_members.Count; i++)
                {
                    CMemberBase base2 = descriptor.ms_members[i];
                    if (base2.GetId() == propertyId)
                    {
                        return base2;
                    }
                }
                if (descriptor.type.BaseType != null)
                {
                    CStringID gid = new CStringID(descriptor.type.BaseType.FullName);
                    return FindMemberBase(gid, propertyId);
                }
            }
            return null;
        }

        public static CMethodBase FindMethodBase(string propertyName)
        {
            int num = propertyName.LastIndexOf(':');
            if (num != -1)
            {
                string str = propertyName.Substring(0, num - 1);
                CStringID agentClassId = new CStringID(str);
                CStringID propertyId = new CStringID(propertyName.Substring(num + 1));
                return FindMethodBase(agentClassId, propertyId);
            }
            return null;
        }

        public static CMethodBase FindMethodBase(CStringID agentClassId, CStringID propertyId)
        {
            if (Metas.ContainsKey(agentClassId))
            {
                CTagObjectDescriptor descriptor = Metas[agentClassId];
                for (int i = 0; i < descriptor.ms_methods.Count; i++)
                {
                    CMethodBase base2 = descriptor.ms_methods[i];
                    if (base2.GetId() == propertyId)
                    {
                        return base2;
                    }
                }
                if ((descriptor.type != null) && (descriptor.type.BaseType != null))
                {
                    CStringID gid = new CStringID(descriptor.type.BaseType.FullName);
                    return FindMethodBase(gid, propertyId);
                }
            }
            return null;
        }

        private static CNamedEvent findNamedEventTemplate(ListView<CMethodBase> methods, string eventName, int context_id)
        {
            return Context.GetContext(context_id).FindNamedEventTemplate(methods, eventName);
        }

        public void FireEvent(string eventName)
        {
            CNamedEvent event2 = this.findEvent(eventName);
            if (event2 == null)
            {
                int contextId = this.GetContextId();
                event2 = findNamedEventTemplate(this.GetDescriptor().ms_methods, eventName, contextId);
            }
            if (event2 != null)
            {
                event2.SetFired(this, true);
            }
        }

        public void FireEvent<ParamType>(string eventName, ParamType param)
        {
            CNamedEvent event2 = this.findEvent(eventName);
            if (event2 == null)
            {
                int contextId = this.GetContextId();
                event2 = findNamedEventTemplate(this.GetDescriptor().ms_methods, eventName, contextId);
            }
            if (event2 != null)
            {
                event2.SetParam<ParamType>(this, param);
                event2.SetFired(this, true);
            }
        }

        public void FireEvent<ParamType1, ParamType2>(string eventName, ParamType1 param1, ParamType2 param2)
        {
            CNamedEvent event2 = this.findEvent(eventName);
            if (event2 == null)
            {
                int contextId = this.GetContextId();
                event2 = findNamedEventTemplate(this.GetDescriptor().ms_methods, eventName, contextId);
            }
            if (event2 != null)
            {
                event2.SetParam<ParamType1, ParamType2>(this, param1, param2);
                event2.SetFired(this, true);
            }
        }

        public void FireEvent<ParamType1, ParamType2, ParamType3>(string eventName, ParamType1 param1, ParamType2 param2, ParamType3 param3)
        {
            CNamedEvent event2 = this.findEvent(eventName);
            if (event2 == null)
            {
                int contextId = this.GetContextId();
                event2 = findNamedEventTemplate(this.GetDescriptor().ms_methods, eventName, contextId);
            }
            if (event2 != null)
            {
                event2.SetParam<ParamType1, ParamType2, ParamType3>(this, param1, param2, param3);
                event2.SetFired(this, true);
            }
        }

        public string GetClassTypeName()
        {
            return base.GetType().FullName;
        }

        public int GetContextId()
        {
            return this.m_contextId;
        }

        public CTagObjectDescriptor GetDescriptor()
        {
            if (this.m_objectDescriptor == null)
            {
                this.m_objectDescriptor = GetDescriptorByName(base.GetType().FullName);
            }
            return this.m_objectDescriptor;
        }

        public static CTagObjectDescriptor GetDescriptorByName(string className)
        {
            CStringID key = new CStringID(className);
            if (Metas.ContainsKey(key))
            {
                return Metas[key];
            }
            CTagObjectDescriptor descriptor = new CTagObjectDescriptor();
            Metas.Add(key, descriptor);
            return descriptor;
        }

        public int GetId()
        {
            return this.m_id;
        }

        public static TAGENT GetInstance<TAGENT>() where TAGENT: Agent, new()
        {
            return GetInstance<TAGENT>(null, 0);
        }

        public static Agent GetInstance(string agentInstanceName)
        {
            return GetInstance(agentInstanceName, 0);
        }

        public static TAGENT GetInstance<TAGENT>(string agentInstanceName) where TAGENT: Agent, new()
        {
            return GetInstance<TAGENT>(agentInstanceName, 0);
        }

        public static Agent GetInstance(string agentInstanceName, int contextId)
        {
            return Context.GetContext(contextId).GetInstance(agentInstanceName);
        }

        public static TAGENT GetInstance<TAGENT>(string agentInstanceName, int contextId) where TAGENT: Agent, new()
        {
            string fullName = agentInstanceName;
            if (string.IsNullOrEmpty(fullName))
            {
                fullName = typeof(TAGENT).FullName;
            }
            return (TAGENT) GetInstance(fullName, contextId);
        }

        public string GetName()
        {
            return base.name;
        }

        public int GetPriority()
        {
            return this.m_priority;
        }

        public static string GetRegisteredClassName(string agentInstanceName)
        {
            if (Names.ContainsKey(agentInstanceName))
            {
                AgentName_t _t = Names[agentInstanceName];
                return _t.ClassName;
            }
            return null;
        }

        public static System.Type GetTypeFromName(string typeName)
        {
            CStringID key = new CStringID(typeName);
            if (Metas.ContainsKey(key))
            {
                CTagObjectDescriptor descriptor = Metas[key];
                return descriptor.type;
            }
            return null;
        }

        public object GetVariable(string variableName)
        {
            uint varId = Utils.MakeVariableId(variableName);
            return this.Variables.Get(this, varId);
        }

        public object GetVariable(uint variableId)
        {
            return this.Variables.Get(this, variableId);
        }

        public static uint IdMask()
        {
            return ms_idMask;
        }

        protected void Init()
        {
            Init_(this.m_contextId, this, this.m_priority, base.name);
        }

        protected static void Init_(int contextId, Agent pAgent, int priority, string agentInstanceName)
        {
            pAgent.m_contextId = contextId;
            pAgent.m_id = ms_agent_index++;
            pAgent.m_priority = priority;
            pAgent.SetName(agentInstanceName);
            pAgent.InitVariableRegistry();
            World objA = Context.GetContext(contextId).GetWorld(true);
            if (!object.ReferenceEquals(objA, null) && !object.ReferenceEquals(objA, pAgent))
            {
                objA.AddAgent(pAgent);
            }
            pAgent.SubsribeToNetwork();
        }

        private void InitVariableRegistry()
        {
            this.ResetChangedVariables();
        }

        public void Instantiate<VariableType>(VariableType value, Property property_)
        {
            this.Variables.Instantiate(property_, value);
        }

        public bool IsActive()
        {
            return this.m_bActive;
        }

        public static bool IsAgentClassName(CStringID agentClassId)
        {
            return Metas.ContainsKey(agentClassId);
        }

        public static bool IsAgentClassName(string agentClassName)
        {
            CStringID agentClassId = new CStringID(agentClassName);
            return IsAgentClassName(agentClassId);
        }

        public static bool IsDerived(Agent pAgent, string agentType)
        {
            for (System.Type type = pAgent.GetType(); type != null; type = type.BaseType)
            {
                if (type.FullName == agentType)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsFired(string eventName)
        {
            return false;
        }

        public bool IsMasked()
        {
            return ((this.m_idFlag & IdMask()) != 0);
        }

        public static bool IsNameRegistered(string agentInstanceName)
        {
            return Names.ContainsKey(agentInstanceName);
        }

        public static bool IsTypeRegisterd(string typeName)
        {
            CStringID key = new CStringID(typeName);
            return ms_metas.ContainsKey(key);
        }

        public void LogVariables(bool bForce)
        {
        }

        protected virtual void OnDestroy()
        {
            this.UnSubsribeToNetwork();
            if (this.m_contextId >= 0)
            {
                World objA = Context.GetContext(this.m_contextId).GetWorld(false);
                if (!object.ReferenceEquals(objA, null) && !object.ReferenceEquals(objA, this))
                {
                    objA.RemoveAgent(this);
                }
            }
            if (this.m_behaviorTreeTasks != null)
            {
                for (int i = 0; i < this.m_behaviorTreeTasks.Count; i++)
                {
                    BehaviorTreeTask behaviorTreeTask = this.m_behaviorTreeTasks[i];
                    Workspace.DestroyBehaviorTreeTask(behaviorTreeTask, this);
                }
                this.m_behaviorTreeTasks.Clear();
                this.m_behaviorTreeTasks = null;
            }
            if (this.m_eventInfos != null)
            {
                this.m_eventInfos.Clear();
                this.m_eventInfos = null;
            }
        }

        private static int ParsePropertyNames(string fullPropertnName, ref string agentClassName)
        {
            int startIndex = fullPropertnName.LastIndexOf(':');
            if (startIndex != -1)
            {
                startIndex++;
                int num2 = startIndex - 2;
                agentClassName = fullPropertnName.Substring(startIndex);
                return num2;
            }
            return -1;
        }

        public static bool RegisterName<TAGENT>() where TAGENT: Agent
        {
            return RegisterName<TAGENT>(null, null, null);
        }

        public static bool RegisterName<TAGENT>(string agentInstanceName) where TAGENT: Agent
        {
            return RegisterName<TAGENT>(agentInstanceName, null, null);
        }

        public static bool RegisterName<TAGENT>(string agentInstanceName, string displayName, string desc) where TAGENT: Agent
        {
            string fullName = agentInstanceName;
            if (string.IsNullOrEmpty(fullName))
            {
                fullName = typeof(TAGENT).FullName;
            }
            if (!Names.ContainsKey(fullName))
            {
                string className = typeof(TAGENT).FullName;
                Names[fullName] = new AgentName_t(fullName, className, displayName, desc);
                return true;
            }
            return false;
        }

        public static bool RegisterStaticClass(System.Type type, string displayName, string desc)
        {
            string fullName = type.FullName;
            if (!Names.ContainsKey(fullName))
            {
                Names[fullName] = new AgentName_t(fullName, fullName, displayName, desc);
                Utils.AddStaticClass(type);
                return true;
            }
            return false;
        }

        private void ReplicateProperties()
        {
        }

        private void ResetChangedVariables()
        {
            this.Variables.Reset();
        }

        public void ResetEvent(string eventName)
        {
            CStringID key = new CStringID(eventName);
            if (this.EventInfos.ContainsKey(key))
            {
                this.EventInfos[key].SetFired(this, false);
            }
            else
            {
                int contextId = this.GetContextId();
                CNamedEvent event3 = findNamedEventTemplate(this.GetDescriptor().ms_methods, eventName, contextId);
                if (event3 != null)
                {
                    event3.SetFired(this, false);
                }
            }
        }

        private void Save(CPropertyNode node)
        {
        }

        public void SetIdFlag(uint idMask)
        {
            this.m_idFlag = idMask;
        }

        public static void SetIdMask(uint idMask)
        {
            ms_idMask = idMask;
        }

        public void SetName(string instanceName)
        {
            if (string.IsNullOrEmpty(instanceName))
            {
                int num = 0;
                string fullName = base.GetType().FullName;
                string str2 = null;
                int num2 = fullName.LastIndexOf(':');
                if (num2 != -1)
                {
                    str2 = fullName.Substring(num2 + 1);
                }
                else
                {
                    str2 = fullName;
                }
                if (ms_agent_type_index == null)
                {
                    ms_agent_type_index = new Dictionary<string, int>();
                }
                if (!ms_agent_type_index.ContainsKey(fullName))
                {
                    num = 0;
                    ms_agent_type_index[fullName] = 1;
                }
                else
                {
                    Dictionary<string, int> dictionary;
                    string str3;
                    int num4;
                    int num3 = dictionary[str3];
                    (dictionary = ms_agent_type_index)[str3 = fullName] = (num4 = num3) + 1;
                    num = num4;
                }
                base.name = base.name + string.Format("{0}_{1}_{2}", str2, num, this.m_id);
            }
            else
            {
                base.name = instanceName;
            }
        }

        public void SetVariable<VariableType>(string variableName, VariableType value)
        {
            uint variableId = Utils.MakeVariableId(variableName);
            this.SetVariable<VariableType>(variableName, value, variableId);
        }

        public void SetVariable<VariableType>(string variableName, VariableType value, uint variableId)
        {
            if (variableId == 0)
            {
                variableId = Utils.MakeVariableId(variableName);
            }
            this.Variables.Set(this, null, variableName, value, variableId);
        }

        public void SetVariableFromString(string variableName, string valueStr)
        {
            this.Variables.SetFromString(this, variableName, valueStr);
        }

        public void SetVariableRegistry(CMemberBase pMember, string variableName, object value, string staticClassName, uint variableId)
        {
            if (!string.IsNullOrEmpty(variableName))
            {
                if (!string.IsNullOrEmpty(staticClassName))
                {
                    Context.GetContext(this.GetContextId()).SetStaticVariable<object>(pMember, variableName, value, staticClassName, variableId);
                }
                else
                {
                    this.Variables.Set(this, pMember, variableName, value, variableId);
                }
            }
        }

        private void SubsribeToNetwork()
        {
        }

        public static bool UnbindInstance<T>()
        {
            return UnbindInstance(typeof(T).FullName);
        }

        public static bool UnbindInstance(string agentInstanceName)
        {
            return UnbindInstance(agentInstanceName, 0);
        }

        public static bool UnbindInstance(string agentInstanceName, int contextId)
        {
            return Context.GetContext(contextId).UnbindInstance(agentInstanceName);
        }

        public void UnInstantiate(string variableName)
        {
            this.Variables.UnInstantiate(variableName);
        }

        public void UnLoad(string variableName)
        {
            this.Variables.UnLoad(variableName);
        }

        public static void UnRegisterName<TAGENT>() where TAGENT: Agent
        {
            UnRegisterName<TAGENT>(null);
        }

        public static void UnRegisterName<TAGENT>(string agentInstanceName) where TAGENT: Agent
        {
            string fullName = agentInstanceName;
            if (string.IsNullOrEmpty(fullName))
            {
                fullName = typeof(TAGENT).FullName;
            }
            if (Names.ContainsKey(fullName))
            {
                Names.Remove(fullName);
            }
        }

        private void UnSubsribeToNetwork()
        {
        }

        private void UpdateVariableRegistry()
        {
            this.ReplicateProperties();
        }

        private ListView<BehaviorTreeTask> BehaviorTreeTasks
        {
            get
            {
                if (this.m_behaviorTreeTasks == null)
                {
                    this.m_behaviorTreeTasks = new ListView<BehaviorTreeTask>();
                }
                return this.m_behaviorTreeTasks;
            }
        }

        private ListView<BehaviorTreeStackItem_t> BTStack
        {
            get
            {
                if (this.m_btStack == null)
                {
                    this.m_btStack = new ListView<BehaviorTreeStackItem_t>();
                }
                return this.m_btStack;
            }
        }

        private DictionaryView<CStringID, CNamedEvent> EventInfos
        {
            get
            {
                if (this.m_eventInfos == null)
                {
                    this.m_eventInfos = new DictionaryView<CStringID, CNamedEvent>();
                }
                return this.m_eventInfos;
            }
        }

        public static DictionaryView<CStringID, CTagObjectDescriptor> Metas
        {
            get
            {
                if (ms_metas == null)
                {
                    ms_metas = new DictionaryView<CStringID, CTagObjectDescriptor>();
                }
                return ms_metas;
            }
        }

        public static Dictionary<string, AgentName_t> Names
        {
            get
            {
                if (ms_names == null)
                {
                    ms_names = new Dictionary<string, AgentName_t>();
                }
                return ms_names;
            }
        }

        public behaviac.Variables Variables
        {
            get
            {
                if (this.m_variables == null)
                {
                    this.m_variables = new behaviac.Variables();
                }
                return this.m_variables;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AgentName_t
        {
            public string instantceName_;
            public string className_;
            public string displayName_;
            public string desc_;
            public AgentName_t(string instanceName, string className, string displayName, string desc)
            {
                this.instantceName_ = instanceName;
                this.className_ = className;
                if (!string.IsNullOrEmpty(displayName))
                {
                    this.displayName_ = displayName;
                }
                else
                {
                    this.displayName_ = this.instantceName_.Replace(".", "::");
                }
                if (!string.IsNullOrEmpty(desc))
                {
                    this.desc_ = desc;
                }
                else
                {
                    this.desc_ = this.displayName_;
                }
            }

            public string ClassName
            {
                get
                {
                    return this.className_;
                }
            }
        }

        private class BehaviorTreeStackItem_t
        {
            public BehaviorTreeTask bt;
            public bool triggerByEvent;
            public TriggerMode triggerMode;

            public BehaviorTreeStackItem_t(BehaviorTreeTask bt_, TriggerMode tm, bool bByEvent)
            {
                this.bt = bt_;
                this.triggerMode = tm;
                this.triggerByEvent = bByEvent;
            }
        }

        public class CTagObjectDescriptor
        {
            public string desc;
            public string displayName;
            public Agent.CTagObjectDescriptor m_parent;
            public ListView<CMemberBase> ms_members = new ListView<CMemberBase>();
            public ListView<CMethodBase> ms_methods = new ListView<CMethodBase>();
            public System.Type type;

            public CMemberBase GetMember(string memberName)
            {
                if (this.ms_members != null)
                {
                    for (int i = 0; i < this.ms_members.Count; i++)
                    {
                        CMemberBase base2 = this.ms_members[i];
                        if (base2.GetName() == memberName)
                        {
                            return base2;
                        }
                    }
                }
                if (this.m_parent != null)
                {
                    return this.m_parent.GetMember(memberName);
                }
                return null;
            }

            public void Load(Agent parent, ISerializableNode node)
            {
                foreach (CMemberBase base2 in this.ms_members)
                {
                    base2.Load(parent, node);
                }
                if (this.m_parent != null)
                {
                    this.m_parent.Load(parent, node);
                }
            }

            public void Save(Agent parent, ISerializableNode node)
            {
                if (this.m_parent != null)
                {
                    this.m_parent.Save(parent, node);
                }
                foreach (CMemberBase base2 in this.ms_members)
                {
                    base2.Save(parent, node);
                }
            }
        }

        public class State_t
        {
            protected BehaviorTreeTask m_bt;
            protected Variables m_vars;

            public State_t()
            {
                this.m_vars = new Variables();
            }

            public State_t(Agent.State_t c)
            {
                this.m_vars = new Variables();
                c.m_vars.CopyTo(null, this.m_vars);
                if (c.m_bt != null)
                {
                    BehaviorNode node = c.m_bt.GetNode();
                    this.m_bt = (BehaviorTreeTask) node.CreateAndInitTask();
                    c.m_bt.CopyTo(this.m_bt);
                }
            }

            public void Clear()
            {
                this.m_vars.Clear();
                this.m_bt = null;
            }

            ~State_t()
            {
                this.Clear();
            }

            public bool LoadFromFile(string fileName)
            {
                return false;
            }

            public bool SaveToFile(string fileName)
            {
                return false;
            }

            public BehaviorTreeTask BT
            {
                get
                {
                    return this.m_bt;
                }
                set
                {
                    this.m_bt = value;
                }
            }

            public Variables Vars
            {
                get
                {
                    return this.m_vars;
                }
            }
        }

        [TypeConverter]
        public class StructConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
            {
                return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is string)
                {
                }
                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}

