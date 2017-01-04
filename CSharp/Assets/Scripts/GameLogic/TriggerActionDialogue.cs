namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    public class TriggerActionDialogue : TriggerActionBase
    {
        public TriggerActionDialogue(TriggerActionWrapper inWrapper) : base(inWrapper)
        {
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger, object prm)
        {
            if (base.EnterUniqueId > 0)
            {
                int enterUniqueId = base.EnterUniqueId;
                GameObject inSrc = (src == 0) ? null : src.handle.gameObject;
                GameObject inAtker = (inTrigger == null) ? null : inTrigger.GetTriggerObj();
                if (inAtker == null)
                {
                    inAtker = (atker == 0) ? null : atker.handle.gameObject;
                }
                MonoSingleton<DialogueProcessor>.GetInstance().PlayDrama(enterUniqueId, inSrc, inAtker, false);
            }
            return null;
        }

        public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            if (base.LeaveUniqueId > 0)
            {
                int leaveUniqueId = base.LeaveUniqueId;
                GameObject inSrc = (src == 0) ? null : src.handle.gameObject;
                GameObject inAtker = (inTrigger == null) ? null : inTrigger.GetTriggerObj();
                MonoSingleton<DialogueProcessor>.GetInstance().PlayDrama(leaveUniqueId, inSrc, inAtker, false);
            }
        }
    }
}

