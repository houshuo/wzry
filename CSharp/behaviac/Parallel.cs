namespace behaviac
{
    using System;
    using System.Collections.Generic;

    public class Parallel : BehaviorNode
    {
        protected CHILDFINISH_POLICY m_childFinishPolicy = CHILDFINISH_POLICY.CHILDFINISH_LOOP;
        protected EXIT_POLICY m_exitPolicy = EXIT_POLICY.EXIT_NONE;
        protected FAILURE_POLICY m_failPolicy = FAILURE_POLICY.FAIL_ON_ONE;
        protected SUCCESS_POLICY m_succeedPolicy = SUCCESS_POLICY.SUCCEED_ON_ALL;

        protected override BehaviorTask createTask()
        {
            return new ParallelTask();
        }

        ~Parallel()
        {
        }

        public override bool IsValid(Agent pAgent, BehaviorTask pTask)
        {
            return ((pTask.GetNode() is Parallel) && base.IsValid(pAgent, pTask));
        }

        protected override void load(int version, string agentType, List<property_t> properties)
        {
            base.load(version, agentType, properties);
            for (int i = 0; i < properties.Count; i++)
            {
                property_t _t = properties[i];
                if (_t.name == "FailurePolicy")
                {
                    if (_t.value == "FAIL_ON_ONE")
                    {
                        this.m_failPolicy = FAILURE_POLICY.FAIL_ON_ONE;
                    }
                    else if (_t.value == "FAIL_ON_ALL")
                    {
                        this.m_failPolicy = FAILURE_POLICY.FAIL_ON_ALL;
                    }
                }
                else if (_t.name == "SuccessPolicy")
                {
                    if (_t.value == "SUCCEED_ON_ONE")
                    {
                        this.m_succeedPolicy = SUCCESS_POLICY.SUCCEED_ON_ONE;
                    }
                    else if (_t.value == "SUCCEED_ON_ALL")
                    {
                        this.m_succeedPolicy = SUCCESS_POLICY.SUCCEED_ON_ALL;
                    }
                }
                else if (_t.name == "ExitPolicy")
                {
                    if (_t.value == "EXIT_NONE")
                    {
                        this.m_exitPolicy = EXIT_POLICY.EXIT_NONE;
                    }
                    else if (_t.value == "EXIT_ABORT_RUNNINGSIBLINGS")
                    {
                        this.m_exitPolicy = EXIT_POLICY.EXIT_ABORT_RUNNINGSIBLINGS;
                    }
                }
                else if (_t.name == "ChildFinishPolicy")
                {
                    if (_t.value == "CHILDFINISH_ONCE")
                    {
                        this.m_childFinishPolicy = CHILDFINISH_POLICY.CHILDFINISH_ONCE;
                    }
                    else if (_t.value == "CHILDFINISH_LOOP")
                    {
                        this.m_childFinishPolicy = CHILDFINISH_POLICY.CHILDFINISH_LOOP;
                    }
                }
            }
        }

        private class ParallelTask : CompositeTask
        {
            public override void copyto(BehaviorTask target)
            {
                base.copyto(target);
            }

            ~ParallelTask()
            {
                base.m_children.Clear();
            }

            public override void Init(BehaviorNode node)
            {
                base.Init(node);
            }

            protected override bool isContinueTicking()
            {
                return true;
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

            protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
            {
                Parallel node = (Parallel) base.GetNode();
                bool flag = false;
                bool flag2 = false;
                bool flag3 = false;
                bool flag4 = true;
                bool flag5 = true;
                bool flag6 = node.m_childFinishPolicy == CHILDFINISH_POLICY.CHILDFINISH_LOOP;
                for (int i = 0; i < base.m_children.Count; i++)
                {
                    BehaviorTask task = base.m_children[i];
                    EBTStatus status = task.GetStatus();
                    if ((flag6 || (status == EBTStatus.BT_RUNNING)) || (status == EBTStatus.BT_INVALID))
                    {
                        EBTStatus status2 = task.exec(pAgent);
                        switch (status2)
                        {
                            case EBTStatus.BT_FAILURE:
                            {
                                flag2 = true;
                                flag5 = false;
                                continue;
                            }
                            case EBTStatus.BT_SUCCESS:
                            {
                                flag = true;
                                flag4 = false;
                                continue;
                            }
                        }
                        if (status2 == EBTStatus.BT_RUNNING)
                        {
                            flag3 = true;
                            flag4 = false;
                            flag5 = false;
                        }
                    }
                    else if (status == EBTStatus.BT_SUCCESS)
                    {
                        flag = true;
                        flag4 = false;
                    }
                    else
                    {
                        flag2 = true;
                        flag5 = false;
                    }
                }
                EBTStatus status3 = !flag3 ? EBTStatus.BT_FAILURE : EBTStatus.BT_RUNNING;
                if (((node.m_failPolicy == FAILURE_POLICY.FAIL_ON_ALL) && flag4) || ((node.m_failPolicy == FAILURE_POLICY.FAIL_ON_ONE) && flag2))
                {
                    status3 = EBTStatus.BT_FAILURE;
                }
                else if (((node.m_succeedPolicy == SUCCESS_POLICY.SUCCEED_ON_ALL) && flag5) || ((node.m_succeedPolicy == SUCCESS_POLICY.SUCCEED_ON_ONE) && flag))
                {
                    status3 = EBTStatus.BT_SUCCESS;
                }
                if ((node.m_exitPolicy == EXIT_POLICY.EXIT_ABORT_RUNNINGSIBLINGS) && ((status3 == EBTStatus.BT_FAILURE) || (status3 == EBTStatus.BT_SUCCESS)))
                {
                    for (int j = 0; j < base.m_children.Count; j++)
                    {
                        BehaviorTask task2 = base.m_children[j];
                        if (task2.GetStatus() == EBTStatus.BT_RUNNING)
                        {
                            task2.abort(pAgent);
                        }
                    }
                }
                return status3;
            }
        }
    }
}

