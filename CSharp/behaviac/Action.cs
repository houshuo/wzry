namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class Action : BehaviorNode
    {
        protected CMethodBase m_method;
        private CMethodBase m_resultFunctor;
        private EBTStatus m_resultOption = EBTStatus.BT_INVALID;
        private EBTStatus m_resultPreconditionFail = EBTStatus.BT_FAILURE;

        protected override BehaviorTask createTask()
        {
            return new ActionTask();
        }

        ~Action()
        {
            this.m_method = null;
            this.m_resultFunctor = null;
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is behaviac.Action) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            foreach (property_t _t in properties)
            {
                if (_t.name == "Method")
                {
                    if (!string.IsNullOrEmpty(_t.value))
                    {
                        this.m_method = LoadMethod(_t.value);
                    }
                }
                else if (_t.name == "ResultOption")
                {
                    if (_t.value == "BT_INVALID")
                    {
                        this.m_resultOption = EBTStatus.BT_INVALID;
                    }
                    else if (_t.value == "BT_FAILURE")
                    {
                        this.m_resultOption = EBTStatus.BT_FAILURE;
                    }
                    else if (_t.value == "BT_RUNNING")
                    {
                        this.m_resultOption = EBTStatus.BT_RUNNING;
                    }
                    else
                    {
                        this.m_resultOption = EBTStatus.BT_SUCCESS;
                    }
                }
                else if (_t.name == "ResultFunctor")
                {
                    if ((_t.value != null) && (_t.value[0] != '\0'))
                    {
                        this.m_resultFunctor = LoadMethod(_t.value);
                    }
                }
                else if (_t.name == "PreconditionFailResult")
                {
                    if (_t.value == "BT_FAILURE")
                    {
                        this.m_resultPreconditionFail = EBTStatus.BT_FAILURE;
                    }
                    else if (_t.value == "BT_BT_SUCCESS")
                    {
                        this.m_resultPreconditionFail = EBTStatus.BT_SUCCESS;
                    }
                }
            }
        }

        public static CMethodBase LoadMethod(string value_)
        {
            string agentIntanceName = null;
            string agentClassName = null;
            string methodName = null;
            int startIndex = ParseMethodNames(value_, ref agentIntanceName, ref agentClassName, ref methodName);
            CStringID agentClassId = new CStringID(agentClassName);
            CStringID methodClassId = new CStringID(methodName);
            CMethodBase base2 = Agent.CreateMethod(agentClassId, methodClassId);
            if (base2 != null)
            {
                if (Agent.IsNameRegistered(agentIntanceName))
                {
                    base2.SetInstanceNameString(agentIntanceName, ParentType.PT_INSTANCE);
                }
                string str4 = value_.Substring(startIndex);
                List<string> paramsToken = null;
                int length = str4.Length;
                paramsToken = ParseForParams(str4.Substring(1, length - 2));
                if (paramsToken != null)
                {
                    base2.Load(null, paramsToken);
                }
            }
            return base2;
        }

        private static List<string> ParseForParams(string tsrc)
        {
            int length = tsrc.Length;
            int startIndex = 0;
            int num3 = 0;
            int num4 = 0;
            List<string> list = new List<string>();
            while (num3 < length)
            {
                if (tsrc[num3] == '"')
                {
                    num4++;
                    if ((num4 & 1) == 0)
                    {
                        num4 -= 2;
                    }
                }
                else if ((num4 == 0) && (tsrc[num3] == ','))
                {
                    int num5 = num3 - startIndex;
                    string item = tsrc.Substring(startIndex, num5);
                    list.Add(item);
                    startIndex = num3 + 1;
                }
                num3++;
            }
            int num6 = num3 - startIndex;
            if (num6 > 0)
            {
                string str2 = tsrc.Substring(startIndex, num6);
                list.Add(str2);
            }
            return list;
        }

        private static int ParseMethodNames(string fullName, ref string agentIntanceName, ref string agentClassName, ref string methodName)
        {
            int index = fullName.IndexOf('.');
            agentIntanceName = fullName.Substring(0, index);
            int startIndex = index + 1;
            int num3 = fullName.IndexOf('(', startIndex);
            int num4 = fullName.LastIndexOf(':', num3) + 1;
            int length = num3 - num4;
            methodName = fullName.Substring(num4, length);
            int num6 = (num4 - 2) - startIndex;
            agentClassName = fullName.Substring(startIndex, num6).Replace("::", ".");
            return num3;
        }

        private class ActionTask : LeafTask
        {
            private static int ms_lastNodeId = -2;

            private static void ClearNodeId()
            {
                ms_lastNodeId = -2;
            }

            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            ~ActionTask()
            {
            }

            public static int GetNodeId()
            {
                return ms_lastNodeId;
            }

            public override void load(ISerializableNode node)
            {
                base.load(node);
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

            private static void SetNodeId(int nodeId)
            {
                ms_lastNodeId = nodeId;
            }

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                behaviac.Action node = (behaviac.Action) base.GetNode();
                if (!this.CheckPredicates(pAgent))
                {
                    return node.m_resultPreconditionFail;
                }
                EBTStatus resultOption = EBTStatus.BT_SUCCESS;
                if (node.m_method == null)
                {
                    return node.update_impl(pAgent, childStatus);
                }
                ParentType parentType = node.m_method.GetParentType();
                Agent parent = pAgent;
                if (parentType == ParentType.PT_INSTANCE)
                {
                    parent = Agent.GetInstance(node.m_method.GetInstanceNameString(), parent.GetContextId());
                }
                SetNodeId(base.GetId());
                object param = node.m_method.run(parent, pAgent);
                if (node.m_resultOption != EBTStatus.BT_INVALID)
                {
                    resultOption = node.m_resultOption;
                }
                else if (node.m_resultFunctor != null)
                {
                    ParentType type2 = node.m_resultFunctor.GetParentType();
                    Agent instance = pAgent;
                    if (type2 == ParentType.PT_INSTANCE)
                    {
                        instance = Agent.GetInstance(node.m_resultFunctor.GetInstanceNameString(), parent.GetContextId());
                    }
                    resultOption = (EBTStatus) ((int) node.m_resultFunctor.run(instance, pAgent, param));
                }
                else
                {
                    resultOption = (EBTStatus) ((int) param);
                }
                ClearNodeId();
                return resultOption;
            }
        }
    }
}

