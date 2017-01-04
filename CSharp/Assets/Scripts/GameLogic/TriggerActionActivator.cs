namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    public class TriggerActionActivator : TriggerActionBase
    {
        public TriggerActionActivator(TriggerActionWrapper inWrapper) : base(inWrapper)
        {
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger, object prm)
        {
            GameObject[] refObjList = base.RefObjList;
            if ((refObjList != null) && (refObjList.Length > 0))
            {
                foreach (GameObject obj2 in refObjList)
                {
                    if (obj2 != null)
                    {
                        obj2.CustomSetActive(base.bEnable);
                    }
                }
            }
            if (base.bSrc && (src != 0))
            {
                src.handle.gameObject.CustomSetActive(base.bEnable);
            }
            if (base.bAtker && (atker != 0))
            {
                atker.handle.gameObject.CustomSetActive(base.bEnable);
            }
            return null;
        }

        public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            if (base.bStopWhenLeaving)
            {
                GameObject[] refObjList = base.RefObjList;
                if ((refObjList != null) && (refObjList.Length > 0))
                {
                    foreach (GameObject obj2 in refObjList)
                    {
                        if (obj2 != null)
                        {
                            obj2.CustomSetActive(!base.bEnable);
                        }
                    }
                }
                if (base.bSrc && (src != 0))
                {
                    src.handle.gameObject.CustomSetActive(!base.bEnable);
                }
                if (base.bAtker && (inTrigger != null))
                {
                    inTrigger.GetTriggerObj().CustomSetActive(!base.bEnable);
                }
            }
        }
    }
}

