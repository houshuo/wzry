namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class SectorTargetSearcher : Singleton<SectorTargetSearcher>
    {
        public ActorRoot GetEnemyTarget(ActorRoot InActor, int srchR, Vector3 useDirection, float srchAngle, uint filter)
        {
            ActorRoot root = null;
            ulong num = (ulong) (srchR * srchR);
            float num2 = srchAngle;
            for (int i = 0; i < 3; i++)
            {
                if (i != InActor.TheActorMeta.ActorCamp)
                {
                    List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP) i);
                    int count = campActors.Count;
                    for (int j = 0; j < count; j++)
                    {
                        PoolObjHandle<ActorRoot> handle = campActors[j];
                        ActorRoot target = handle.handle;
                        if (((((filter & (((int) 1) << target.TheActorMeta.ActorType)) <= 0L) && !target.ActorControl.IsDeadState) && target.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp)) && InActor.CanAttack(target))
                        {
                            VInt3 num8 = target.location - InActor.location;
                            if (num8.sqrMagnitudeLong2D < num)
                            {
                                Vector3 vector = (Vector3) (target.location - InActor.location);
                                float num7 = Mathf.Abs(Vector3.Angle(useDirection, vector.normalized));
                                if (num7 < num2)
                                {
                                    num2 = num7;
                                    root = target;
                                }
                            }
                        }
                    }
                }
            }
            return root;
        }
    }
}

