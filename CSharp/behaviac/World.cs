namespace behaviac
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class World : Agent
    {
        private List<HeapItem_t> m_agents;
        private bool m_bTickAgents;

        protected World()
        {
        }

        public void AddAgent(Agent pAgent)
        {
            int id = pAgent.GetId();
            int priority = pAgent.GetPriority();
            int num3 = -1;
            for (int i = 0; i < this.Agents.Count; i++)
            {
                HeapItem_t _t2 = this.Agents[i];
                if (_t2.priority == priority)
                {
                    num3 = i;
                    break;
                }
            }
            if (num3 == -1)
            {
                HeapItem_t item = new HeapItem_t {
                    agents = new DictionaryView<int, Agent>(),
                    priority = priority
                };
                item.agents[id] = pAgent;
                this.Agents.Add(item);
            }
            else
            {
                HeapItem_t _t3 = this.Agents[num3];
                _t3.agents[id] = pAgent;
            }
        }

        public override EBTStatus btexec()
        {
            Workspace.LogFrames();
            Workspace.HandleRequests();
            if (Workspace.GetAutoHotReload())
            {
                Workspace.HotReload();
            }
            base.btexec();
            if (this.m_bTickAgents)
            {
                this.btexec_agents();
            }
            return EBTStatus.BT_RUNNING;
        }

        private void btexec_agents()
        {
            this.Agents.Sort();
            for (int i = 0; i < this.Agents.Count; i++)
            {
                HeapItem_t _t = this.Agents[i];
                foreach (KeyValuePair<int, Agent> pair in _t.agents)
                {
                    if (pair.Value.IsActive())
                    {
                        pair.Value.btexec();
                        if (!this.m_bTickAgents)
                        {
                            break;
                        }
                    }
                }
            }
            if (Agent.IdMask() != 0)
            {
                Context.GetContext(base.GetContextId()).LogStaticVariables(null);
            }
        }

        public static World GetInstance(int contextId)
        {
            return Context.GetContext(contextId).GetWorld(true);
        }

        protected void Init()
        {
            Context.GetContext(base.m_contextId).SetWorld(this);
            base.Init();
            this.m_bTickAgents = true;
        }

        public bool IsTickAgents()
        {
            return this.m_bTickAgents;
        }

        public void LogCurrentStates()
        {
            string str = string.Format("LogCurrentStates {0} {1}", base.GetClassTypeName(), this.Agents.Count);
            foreach (HeapItem_t _t in this.Agents)
            {
                foreach (KeyValuePair<int, Agent> pair in _t.agents)
                {
                    if (pair.Value.IsMasked())
                    {
                        pair.Value.LogVariables(true);
                    }
                }
            }
            if (base.IsMasked())
            {
                base.LogVariables(true);
            }
        }

        protected void OnDestroy()
        {
            Context context = Context.GetContext(base.m_contextId);
            if (context.GetWorld(false) == this)
            {
                context.SetWorld(null);
            }
            base.OnDestroy();
        }

        public void RemoveAgent(Agent pAgent)
        {
            int id = pAgent.GetId();
            int priority = pAgent.GetPriority();
            int num3 = -1;
            for (int i = 0; i < this.Agents.Count; i++)
            {
                HeapItem_t _t = this.Agents[i];
                if (_t.priority == priority)
                {
                    num3 = i;
                    break;
                }
            }
            if (num3 != -1)
            {
                HeapItem_t _t2 = this.Agents[num3];
                if (_t2.agents.ContainsKey(id))
                {
                    HeapItem_t _t3 = this.Agents[num3];
                    _t3.agents.Remove(id);
                }
            }
        }

        public void RemoveAllAgents()
        {
            this.Agents.Clear();
        }

        public void ToggleTickAgents(bool bTickAgents)
        {
            this.m_bTickAgents = bTickAgents;
        }

        public List<HeapItem_t> Agents
        {
            get
            {
                if (this.m_agents == null)
                {
                    this.m_agents = new List<HeapItem_t>();
                }
                return this.m_agents;
            }
            set
            {
                this.m_agents = value;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HeapItem_t : IComparable<World.HeapItem_t>
        {
            public int priority;
            public DictionaryView<int, Agent> agents;
            public int CompareTo(World.HeapItem_t other)
            {
                if (this.priority < other.priority)
                {
                    return -1;
                }
                if (this.priority > other.priority)
                {
                    return 1;
                }
                return 0;
            }
        }
    }
}

