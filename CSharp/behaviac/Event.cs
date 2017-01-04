namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class Event : ConditionBase
    {
        protected bool m_bTriggeredOnce = false;
        protected CMethodBase m_event;
        protected string m_referencedBehaviorPath;
        protected TriggerMode m_triggerMode = TriggerMode.TM_Transfer;

        protected override BehaviorTask createTask()
        {
            return new EventTask();
        }

        ~Event()
        {
            this.m_event = null;
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is Event) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            foreach (property_t _t in properties)
            {
                if (_t.name == "EventName")
                {
                    this.m_event = behaviac.Action.LoadMethod(_t.value);
                }
                else if (_t.name == "ReferenceFilename")
                {
                    this.m_referencedBehaviorPath = _t.value;
                }
                else if (_t.name == "TriggeredOnce")
                {
                    if (_t.value == "true")
                    {
                        this.m_bTriggeredOnce = true;
                    }
                }
                else if (_t.name == "TriggerMode")
                {
                    if (_t.value == "Transfer")
                    {
                        this.m_triggerMode = TriggerMode.TM_Transfer;
                    }
                    else if (_t.value == "Return")
                    {
                        this.m_triggerMode = TriggerMode.TM_Return;
                    }
                }
            }
        }

        public class EventTask : AttachmentTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            ~EventTask()
            {
            }

            public string GetEventName()
            {
                Event node = base.GetNode() as Event;
                return node.m_event.Name;
            }

            public TriggerMode GetTriggerMode()
            {
                Event node = base.GetNode() as Event;
                return node.m_triggerMode;
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
            }

            public override bool NeedRestart()
            {
                return true;
            }

            protected override bool onenter(Agent pAgent)
            {
                return true;
            }

            protected override void onexit(Agent pAgent, EBTStatus s)
            {
            }

            public override void save(ISerializableNode node)
            {
                base.save(node);
            }

            public bool TriggeredOnce()
            {
                Event node = base.GetNode() as Event;
                return node.m_bTriggeredOnce;
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                EBTStatus status = EBTStatus.BT_SUCCESS;
                Event node = base.GetNode() as Event;
                if (!string.IsNullOrEmpty(node.m_referencedBehaviorPath) && (pAgent != null))
                {
                    TriggerMode triggerMode = this.GetTriggerMode();
                    pAgent.bteventtree(node.m_referencedBehaviorPath, triggerMode);
                    pAgent.btexec();
                }
                return status;
            }
        }
    }
}

