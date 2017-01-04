namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    public class TriggerActionSoldierLine : TriggerActionBase
    {
        public TriggerActionSoldierLine(TriggerActionWrapper inWrapper) : base(inWrapper)
        {
        }

        public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger, object prm)
        {
            this.UpdateSoldierRegionAvail(base.bEnable);
            return null;
        }

        public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
        {
            if (base.bStopWhenLeaving)
            {
                this.UpdateSoldierRegionAvail(!base.bEnable);
            }
        }

        private void UpdateSoldierRegionAvail(bool bAvailable)
        {
            if ((Singleton<BattleLogic>.GetInstance().mapLogic != null) && (base.RefObjList != null))
            {
                for (int i = 0; i < base.RefObjList.Length; i++)
                {
                    GameObject obj2 = base.RefObjList[i];
                    if (obj2 != null)
                    {
                        SoldierRegion component = obj2.GetComponent<SoldierRegion>();
                        if (component != null)
                        {
                            Singleton<BattleLogic>.GetInstance().mapLogic.EnableSoldierRegion(bAvailable, component);
                        }
                    }
                }
            }
        }
    }
}

