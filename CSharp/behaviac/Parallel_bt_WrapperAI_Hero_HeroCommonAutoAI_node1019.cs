namespace behaviac
{
    using System;

    internal class Parallel_bt_WrapperAI_Hero_HeroCommonAutoAI_node1019 : Parallel
    {
        public Parallel_bt_WrapperAI_Hero_HeroCommonAutoAI_node1019()
        {
            base.m_failPolicy = FAILURE_POLICY.FAIL_ON_ONE;
            base.m_succeedPolicy = SUCCESS_POLICY.SUCCEED_ON_ALL;
            base.m_exitPolicy = EXIT_POLICY.EXIT_ABORT_RUNNINGSIBLINGS;
            base.m_childFinishPolicy = CHILDFINISH_POLICY.CHILDFINISH_LOOP;
        }
    }
}

