namespace behaviac
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class Context
    {
        private DictionaryView<string, Agent> m_namedAgents = new DictionaryView<string, Agent>();
        private DictionaryView<string, Variables> m_static_variables = new DictionaryView<string, Variables>();
        private World m_world = null;
        private static DictionaryView<int, Context> ms_contexts = new DictionaryView<int, Context>();
        private DictionaryView<string, DictionaryView<CStringID, CNamedEvent>> ms_eventInfosGlobal = new DictionaryView<string, DictionaryView<CStringID, CNamedEvent>>();

        private Context(int contextId)
        {
        }

        public bool BindInstance(Agent pAgentInstance)
        {
            return this.BindInstance(pAgentInstance, null);
        }

        public bool BindInstance(Agent pAgentInstance, string agentInstanceName)
        {
            if (string.IsNullOrEmpty(agentInstanceName))
            {
                agentInstanceName = pAgentInstance.GetType().FullName;
            }
            if (Agent.IsNameRegistered(agentInstanceName))
            {
                string registeredClassName = Agent.GetRegisteredClassName(agentInstanceName);
                if (Agent.IsDerived(pAgentInstance, registeredClassName))
                {
                    this.m_namedAgents[agentInstanceName] = pAgentInstance;
                    return true;
                }
            }
            return false;
        }

        public static void Cleanup(int contextId)
        {
            if (ms_contexts != null)
            {
                if (contextId == -1)
                {
                    ms_contexts.Clear();
                }
                else if (ms_contexts.ContainsKey(contextId))
                {
                    ms_contexts.Remove(contextId);
                }
            }
        }

        private void CleanupInstances()
        {
            this.m_namedAgents.Clear();
        }

        private void CleanupStaticVariables()
        {
            foreach (KeyValuePair<string, Variables> pair in this.m_static_variables)
            {
                pair.Value.Clear();
            }
            this.m_static_variables.Clear();
        }

        ~Context()
        {
            this.m_world = null;
            this.CleanupStaticVariables();
            this.CleanupInstances();
            this.ms_eventInfosGlobal.Clear();
        }

        public CNamedEvent FindEventStatic(string eventName, string className)
        {
            if (this.ms_eventInfosGlobal.ContainsKey(className))
            {
                DictionaryView<CStringID, CNamedEvent> view = this.ms_eventInfosGlobal[className];
                CStringID key = new CStringID(eventName);
                if (view.ContainsKey(key))
                {
                    return view[key];
                }
            }
            return null;
        }

        public CNamedEvent FindNamedEventTemplate(ListView<CMethodBase> methods, string eventName)
        {
            CStringID gid = new CStringID(eventName);
            for (int i = methods.Count - 1; i >= 0; i--)
            {
                CMethodBase base2 = methods[i];
                string name = base2.GetName();
                CStringID gid2 = new CStringID(name);
                if ((gid2 == gid) && base2.IsNamedEvent())
                {
                    CNamedEvent pEvent = (CNamedEvent) base2;
                    if (pEvent.IsStatic())
                    {
                        this.InsertEventGlobal(pEvent.GetClassNameString(), pEvent);
                        return pEvent;
                    }
                    return pEvent;
                }
            }
            return null;
        }

        private static bool GetClassNameString(string variableName, ref string className)
        {
            int num = variableName.LastIndexOf(':');
            if (num > 0)
            {
                className = variableName.Substring(0, num - 1);
                return true;
            }
            className = variableName;
            return true;
        }

        public static Context GetContext(int contextId)
        {
            if (ms_contexts.ContainsKey(contextId))
            {
                return ms_contexts[contextId];
            }
            Context context2 = new Context(contextId);
            ms_contexts[contextId] = context2;
            return context2;
        }

        public Agent GetInstance(string agentInstanceName)
        {
            if (!string.IsNullOrEmpty(agentInstanceName))
            {
                string className = null;
                GetClassNameString(agentInstanceName, ref className);
                if (this.m_namedAgents.ContainsKey(className))
                {
                    return this.m_namedAgents[className];
                }
            }
            return null;
        }

        public World GetWorld(bool bCreate)
        {
            if ((object.ReferenceEquals(this.m_world, null) && bCreate) && object.ReferenceEquals(this.m_world, null))
            {
                GameObject target = new GameObject("_world_");
                UnityEngine.Object.DontDestroyOnLoad(target);
                this.m_world = target.AddComponent<DefaultWorld>();
            }
            return this.m_world;
        }

        public void InsertEventGlobal(string className, CNamedEvent pEvent)
        {
            if (this.FindEventStatic(className, pEvent.GetName()) == null)
            {
                if (!this.ms_eventInfosGlobal.ContainsKey(className))
                {
                    this.ms_eventInfosGlobal.Add(className, new DictionaryView<CStringID, CNamedEvent>());
                }
                DictionaryView<CStringID, CNamedEvent> view = this.ms_eventInfosGlobal[className];
                CNamedEvent event3 = (CNamedEvent) pEvent.clone();
                CStringID key = new CStringID(event3.GetName());
                view.Add(key, event3);
            }
        }

        public static void LogCurrentStates()
        {
            if (ms_contexts != null)
            {
                foreach (KeyValuePair<int, Context> pair in ms_contexts)
                {
                    if (!object.ReferenceEquals(pair.Value.m_world, null))
                    {
                        pair.Value.m_world.LogCurrentStates();
                    }
                }
            }
        }

        public void LogStaticVariables(string agentClassName)
        {
            if (!string.IsNullOrEmpty(agentClassName))
            {
                if (this.m_static_variables.ContainsKey(agentClassName))
                {
                    this.m_static_variables[agentClassName].Log(null, false);
                }
            }
            else
            {
                foreach (KeyValuePair<string, Variables> pair in this.m_static_variables)
                {
                    pair.Value.Log(null, false);
                }
            }
        }

        public void ResetChangedVariables()
        {
            foreach (KeyValuePair<string, Variables> pair in this.m_static_variables)
            {
                pair.Value.Reset();
            }
        }

        public void SetStaticVariable<VariableType>(CMemberBase pMember, string variableName, VariableType value, string staticClassName, uint variableId)
        {
            if (!this.m_static_variables.ContainsKey(staticClassName))
            {
                this.m_static_variables[staticClassName] = new Variables();
            }
            this.m_static_variables[staticClassName].Set(null, pMember, variableName, value, variableId);
        }

        public void SetWorld(World pWorld)
        {
            if (this.m_world == null)
            {
                this.m_world = pWorld;
            }
            else
            {
                if ((pWorld != null) && (this.m_world is DefaultWorld))
                {
                    pWorld.Agents = this.m_world.Agents;
                    this.m_world.Agents.Clear();
                }
                this.m_world = pWorld;
            }
        }

        public bool UnbindInstance<T>()
        {
            string fullName = typeof(T).FullName;
            return this.UnbindInstance(fullName);
        }

        public bool UnbindInstance(string agentInstanceName)
        {
            if (Agent.IsNameRegistered(agentInstanceName) && this.m_namedAgents.ContainsKey(agentInstanceName))
            {
                this.m_namedAgents.Remove(agentInstanceName);
                return true;
            }
            return false;
        }
    }
}

