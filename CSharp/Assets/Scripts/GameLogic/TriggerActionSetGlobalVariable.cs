namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;

    public class TriggerActionSetGlobalVariable : TriggerActionBase
    {
        public TriggerActionSetGlobalVariable(TriggerActionWrapper inWrapper) : base(inWrapper)
        {
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger, object prm)
        {
            if (Singleton<BattleLogic>.instance.m_globalTrigger != null)
            {
                Singleton<BattleLogic>.instance.m_globalTrigger.CurGlobalVariable = base.EnterUniqueId;
            }
            return null;
        }
    }
}

