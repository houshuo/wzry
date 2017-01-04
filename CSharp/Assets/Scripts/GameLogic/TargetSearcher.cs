namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class TargetSearcher : Singleton<TargetSearcher>
    {
        private PoolObjHandle<ActorRoot> _coordActor;
        private bool _coordCheckSight;
        private bool _coordFilterAlly;
        private bool _coordFilterEnemy;
        private SceneManagement.Process _coordHandler;
        private VCollisionShape _coordShape;
        private AreaEventTrigger _coordTriggerRef;
        private List<PoolObjHandle<ActorRoot>> collidedActors = new List<PoolObjHandle<ActorRoot>>();

        public void BeginCollidedActorList(PoolObjHandle<ActorRoot> InActor, VCollisionShape collisionShape, bool bFilterEnemy, bool bFilterAlly, AreaEventTrigger InTriggerRef, bool bCheckSight)
        {
            this.collidedActors.Clear();
            if (collisionShape != null)
            {
                this._coordActor = InActor;
                this._coordShape = collisionShape;
                this._coordFilterEnemy = bFilterEnemy;
                this._coordFilterAlly = bFilterAlly;
                this._coordTriggerRef = InTriggerRef;
                this._coordCheckSight = bCheckSight;
                SceneManagement instance = Singleton<SceneManagement>.GetInstance();
                SceneManagement.Coordinate coord = new SceneManagement.Coordinate();
                instance.GetCoord(ref coord, collisionShape);
                instance.UpdateDirtyNodes();
                instance.ForeachActors(coord, this._coordHandler);
                this._coordShape = null;
                this._coordTriggerRef = null;
            }
        }

        public void EndCollidedActorList()
        {
            this.collidedActors.Clear();
        }

        private void FilterCoordActor(ref PoolObjHandle<ActorRoot> actorPtr)
        {
            ActorRoot handle = actorPtr.handle;
            if (((((!handle.ActorControl.IsDeadState && (handle.shape != null)) && (actorPtr != this._coordActor)) && (((this._coordActor == 0) || ((!this._coordFilterAlly || !handle.IsSelfCamp(this._coordActor.handle)) && ((!this._coordFilterEnemy && (!handle.ObjLinker.Invincible || (handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE))) || !handle.IsEnemyCamp(this._coordActor.handle)))) && (((handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ) || handle.AttackOrderReady) && (!this._coordCheckSight || handle.HorizonMarker.IsVisibleFor(this._coordActor.handle.TheActorMeta.ActorCamp))))) && ((null == this._coordTriggerRef) || this._coordTriggerRef.ActorFilter(ref actorPtr))) && handle.shape.Intersects(this._coordShape))
            {
                this.collidedActors.Add(actorPtr);
            }
        }

        public int GetActorsInCircle(VInt3 center, int radius, PoolObjHandle<ActorRoot>[] outResults, AreaEventTrigger InTriggerRef)
        {
            int num = 0;
            ulong num2 = (ulong) (radius * radius);
            List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
            int count = gameActors.Count;
            for (int i = 0; i < count; i++)
            {
                if (num >= outResults.Length)
                {
                    return num;
                }
                PoolObjHandle<ActorRoot> act = gameActors[i];
                VInt3 num6 = act.handle.location - center;
                if ((num6.sqrMagnitudeLong2D < num2) && ((null == InTriggerRef) || InTriggerRef.ActorFilter(ref act)))
                {
                    outResults[num++] = act;
                }
            }
            return num;
        }

        public List<PoolObjHandle<ActorRoot>> GetActorsInPolygon(GeoPolygon collisionPolygon, AreaEventTrigger InTriggerRef)
        {
            if (collisionPolygon == null)
            {
                return null;
            }
            List<PoolObjHandle<ActorRoot>> list = null;
            List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
            int count = gameActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> act = gameActors[i];
                if (((act != 0) && ((null == InTriggerRef) || InTriggerRef.ActorFilter(ref act))) && collisionPolygon.IsPointIn(act.handle.location))
                {
                    if (list == null)
                    {
                        list = new List<PoolObjHandle<ActorRoot>>();
                    }
                    list.Add(act);
                }
            }
            return list;
        }

        public ActorRoot GetAnyEnemyInCircle(ActorRoot InActor, VInt3 Pos, int srchR)
        {
            long num = srchR * srchR;
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
                        if (InActor.CanAttack(target))
                        {
                            VInt3 num6 = target.location - Pos;
                            if (num6.sqrMagnitudeLong2D < num)
                            {
                                return target;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public List<PoolObjHandle<ActorRoot>> GetCollidedActors()
        {
            return this.collidedActors;
        }

        public int GetEnemyCountInRange(ActorRoot InActor, int srchR)
        {
            long num = srchR * srchR;
            int num2 = 0;
            List<PoolObjHandle<ActorRoot>> campActors = null;
            if (InActor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
            {
                campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
            }
            else if (InActor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
            {
                campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
            }
            else
            {
                return 0;
            }
            int count = campActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = campActors[i];
                VInt3 num6 = handle.handle.location - InActor.location;
                if (num6.sqrMagnitudeLong2D < num)
                {
                    num2++;
                }
            }
            return num2;
        }

        public int GetEnemyHeroCountInRange(ActorRoot InActor, int srchR)
        {
            long num = srchR * srchR;
            int num2 = 0;
            List<PoolObjHandle<ActorRoot>> campActors = null;
            if (InActor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
            {
                campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
            }
            else if (InActor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
            {
                campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
            }
            else
            {
                return 0;
            }
            int count = campActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = campActors[i];
                ActorRoot root = handle.handle;
                if (root.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                {
                    VInt3 num6 = root.location - InActor.location;
                    if (num6.sqrMagnitudeLong2D < num)
                    {
                        num2++;
                    }
                }
            }
            return num2;
        }

        public ActorRoot GetLowestHpTarget(ActorRoot InActor, int srchR, TargetPriority priotity, uint filter, bool bEnemy = true, bool bWithMonsterNotInBattle = true)
        {
            List<ActorRoot> list = new List<ActorRoot>();
            List<ActorRoot> list2 = new List<ActorRoot>();
            List<ActorRoot> list3 = new List<ActorRoot>();
            TargetPropertyLessEqualFilter filter2 = new TargetPropertyLessEqualFilter(list, ulong.MaxValue);
            TargetPropertyLessEqualFilter filter3 = new TargetPropertyLessEqualFilter(list2, ulong.MaxValue);
            TargetDistanceNearFilter filter4 = new TargetDistanceNearFilter(ulong.MaxValue);
            if (bEnemy)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i != InActor.TheActorMeta.ActorCamp)
                    {
                        List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP) i);
                        int num2 = campActors.Count;
                        for (int j = 0; j < num2; j++)
                        {
                            PoolObjHandle<ActorRoot> handle2 = campActors[j];
                            ActorRoot target = handle2.handle;
                            if ((((filter & (((int) 1) << target.TheActorMeta.ActorType)) <= 0L) && target.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp)) && InActor.CanAttack(target))
                            {
                                if (TypeSearchCondition.Fit(target, ActorTypeDef.Actor_Type_Hero))
                                {
                                    if (DistanceSearchCondition.Fit(target, InActor, srchR))
                                    {
                                        filter2.Searcher(target, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, new PropertyDelegate(TargetProperty.GetPropertyHpRate));
                                    }
                                }
                                else if (TypeSearchCondition.Fit(target, ActorTypeDef.Actor_Type_Organ))
                                {
                                    if (DistanceSearchCondition.Fit(target, InActor, srchR))
                                    {
                                        list3.Add(target);
                                    }
                                }
                                else if ((TypeSearchCondition.Fit(target, ActorTypeDef.Actor_Type_Monster) && TypeSearchCondition.FitWithJungleMonsterNotInBattle(target, bWithMonsterNotInBattle)) && DistanceSearchCondition.Fit(target, InActor, srchR))
                                {
                                    filter3.Searcher(target, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, new PropertyDelegate(TargetProperty.GetPropertyHpRate));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                List<PoolObjHandle<ActorRoot>> list5 = Singleton<GameObjMgr>.GetInstance().GetCampActors(InActor.TheActorMeta.ActorCamp);
                int num4 = list5.Count;
                for (int k = 0; k < num4; k++)
                {
                    PoolObjHandle<ActorRoot> handle3 = list5[k];
                    ActorRoot root2 = handle3.handle;
                    if ((((filter & (((int) 1) << root2.TheActorMeta.ActorType)) <= 0L) && !root2.ActorControl.IsDeadState) && root2.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
                    {
                        if (TypeSearchCondition.Fit(root2, ActorTypeDef.Actor_Type_Hero))
                        {
                            if (DistanceSearchCondition.Fit(root2, InActor, srchR))
                            {
                                filter2.Searcher(root2, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, new PropertyDelegate(TargetProperty.GetPropertyHpRate));
                            }
                        }
                        else if (TypeSearchCondition.Fit(root2, ActorTypeDef.Actor_Type_Organ))
                        {
                            if (DistanceSearchCondition.Fit(root2, InActor, srchR))
                            {
                                list3.Add(root2);
                            }
                        }
                        else if (TypeSearchCondition.Fit(root2, ActorTypeDef.Actor_Type_Monster) && DistanceSearchCondition.Fit(root2, InActor, srchR))
                        {
                            filter3.Searcher(root2, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, new PropertyDelegate(TargetProperty.GetPropertyHpRate));
                        }
                    }
                }
            }
            int num6 = (list.Count + list3.Count) + list2.Count;
            int count = list.Count;
            if (count > 0)
            {
                ActorRoot root3 = null;
                if (count == 1)
                {
                    root3 = list[0];
                }
                else
                {
                    root3 = filter4.Searcher(list.GetEnumerator(), InActor);
                }
                PoolObjHandle<ActorRoot> selfPtr = new PoolObjHandle<ActorRoot>();
                if (root3 != null)
                {
                    selfPtr = root3.SelfPtr;
                }
                SkillChooseTargetEventParam prm = new SkillChooseTargetEventParam(selfPtr, InActor.SelfPtr, num6);
                Singleton<GameEventSys>.instance.SendEvent<SkillChooseTargetEventParam>(GameEventDef.Event_ActorBeChosenAsTarget, ref prm);
                return root3;
            }
            count = list3.Count;
            if (count > 0)
            {
                if (count == 1)
                {
                    return list3[0];
                }
                return filter4.Searcher(list3.GetEnumerator(), InActor);
            }
            count = list2.Count;
            if (count <= 0)
            {
                return null;
            }
            if (count == 1)
            {
                return list2[0];
            }
            return filter4.Searcher(list2.GetEnumerator(), InActor);
        }

        public ActorRoot GetLowHpTeamMember(ActorRoot InActor, int srchR, int HPRate, uint filter)
        {
            ulong num = (ulong) (srchR * srchR);
            List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(InActor.TheActorMeta.ActorCamp);
            int count = campActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = campActors[i];
                ActorRoot root2 = handle.handle;
                if (((filter & (((int) 1) << root2.TheActorMeta.ActorType)) <= 0L) && !root2.ActorControl.IsDeadState)
                {
                    VInt3 num5 = root2.location - InActor.location;
                    if ((num5.sqrMagnitudeLong2D < num) && ((root2.ValueComponent.actorHp * 0x2710) < (root2.ValueComponent.actorHpTotal * HPRate)))
                    {
                        return root2;
                    }
                }
            }
            return null;
        }

        public ActorRoot GetNearestEnemy(ActorRoot InActor, int srchR, uint filter, bool bWithMonsterNotInBattle = true)
        {
            ActorRoot root = null;
            ulong num = (ulong) (srchR * srchR);
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
                        if (((filter & (((int) 1) << target.TheActorMeta.ActorType)) <= 0L) && target.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
                        {
                            if (!bWithMonsterNotInBattle)
                            {
                                MonsterWrapper actorControl = target.ActorControl as MonsterWrapper;
                                if (actorControl != null)
                                {
                                    ResMonsterCfgInfo cfgInfo = actorControl.cfgInfo;
                                    if ((cfgInfo != null) && (cfgInfo.bMonsterType == 2))
                                    {
                                        switch (target.ActorAgent.GetCurBehavior())
                                        {
                                            case ObjBehaviMode.State_Idle:
                                            case ObjBehaviMode.State_Dead:
                                            case ObjBehaviMode.State_Null:
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                            VInt3 num6 = target.location - InActor.location;
                            ulong num5 = (ulong) num6.sqrMagnitudeLong2D;
                            if ((num5 < num) && InActor.CanAttack(target))
                            {
                                root = target;
                                num = num5;
                            }
                        }
                    }
                }
            }
            return root;
        }

        public ActorRoot GetNearestEnemy(ActorRoot InActor, int srchR, TargetPriority priotity, uint filter, bool bWithMonsterNotInBattle = true)
        {
            ActorRoot root = null;
            ActorRoot root2 = null;
            ulong num = (ulong) (srchR * srchR);
            ulong num2 = num;
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
                        if (((filter & (((int) 1) << target.TheActorMeta.ActorType)) <= 0L) && target.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
                        {
                            if (!bWithMonsterNotInBattle)
                            {
                                MonsterWrapper actorControl = target.ActorControl as MonsterWrapper;
                                if (actorControl != null)
                                {
                                    ResMonsterCfgInfo cfgInfo = actorControl.cfgInfo;
                                    if ((cfgInfo != null) && (cfgInfo.bMonsterType == 2))
                                    {
                                        switch (target.ActorAgent.GetCurBehavior())
                                        {
                                            case ObjBehaviMode.State_Idle:
                                            case ObjBehaviMode.State_Dead:
                                            case ObjBehaviMode.State_Null:
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                            if ((((priotity == TargetPriority.TargetPriority_Hero) && (target.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)) || ((priotity == TargetPriority.TargetPriority_Monster) && (target.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster))) || ((priotity == TargetPriority.TargetPriority_Organ) && (target.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)))
                            {
                                VInt3 num8 = target.location - InActor.location;
                                ulong num6 = (ulong) num8.sqrMagnitudeLong2D;
                                if ((num6 < num) && InActor.CanAttack(target))
                                {
                                    root = target;
                                    num = num6;
                                }
                            }
                            else
                            {
                                VInt3 num9 = target.location - InActor.location;
                                ulong num7 = (ulong) num9.sqrMagnitudeLong2D;
                                if ((num7 < num2) && InActor.CanAttack(target))
                                {
                                    root2 = target;
                                    num2 = num7;
                                }
                            }
                        }
                    }
                }
            }
            return ((root == null) ? root2 : root);
        }

        public ActorRoot GetNearestEnemyDogfaceFirst(ActorRoot InActor, int srchR)
        {
            ActorRoot root = null;
            ActorRoot root2 = null;
            ulong num = (ulong) (srchR * srchR);
            ulong num2 = (ulong) (srchR * srchR);
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
                        if (target.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
                        {
                            if (target.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                            {
                                VInt3 num8 = target.location - InActor.location;
                                ulong num6 = (ulong) num8.sqrMagnitudeLong2D;
                                if ((num6 < num) && InActor.CanAttack(target))
                                {
                                    root = target;
                                    num = num6;
                                }
                            }
                            else
                            {
                                VInt3 num9 = target.location - InActor.location;
                                ulong num7 = (ulong) num9.sqrMagnitudeLong2D;
                                if ((num7 < num2) && InActor.CanAttack(target))
                                {
                                    root2 = target;
                                    num2 = num7;
                                }
                            }
                        }
                    }
                }
            }
            return ((root2 == null) ? root : root2);
        }

        public ActorRoot GetNearestEnemyDogfaceFirstAndDogfaceHasPriority(ActorRoot InActor, int srchR)
        {
            ActorRoot root = null;
            ActorRoot root2 = null;
            ActorRoot root3 = null;
            ActorRoot root4 = null;
            ActorRoot root5 = null;
            ulong num = (ulong) (srchR * srchR);
            ulong num2 = num;
            ulong num3 = num;
            ulong num4 = num;
            ulong num5 = num;
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
                        if (target.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
                        {
                            if (target.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                            {
                                VInt3 num14 = target.location - InActor.location;
                                ulong num9 = (ulong) num14.sqrMagnitudeLong2D;
                                if ((num9 < num5) && InActor.CanAttack(target))
                                {
                                    root5 = target;
                                    num5 = num9;
                                }
                            }
                            else
                            {
                                MonsterWrapper wrapper = target.AsMonster();
                                if (wrapper != null)
                                {
                                    if (wrapper.cfgInfo.bSoldierType == 3)
                                    {
                                        VInt3 num15 = target.location - InActor.location;
                                        ulong num10 = (ulong) num15.sqrMagnitudeLong2D;
                                        if ((num10 < num) && InActor.CanAttack(target))
                                        {
                                            root = target;
                                            num = num10;
                                        }
                                    }
                                    else if (wrapper.cfgInfo.bSoldierType == 1)
                                    {
                                        VInt3 num16 = target.location - InActor.location;
                                        ulong num11 = (ulong) num16.sqrMagnitudeLong2D;
                                        if ((num11 < num2) && InActor.CanAttack(target))
                                        {
                                            root2 = target;
                                            num2 = num11;
                                        }
                                    }
                                    else if (wrapper.cfgInfo.bSoldierType == 2)
                                    {
                                        VInt3 num17 = target.location - InActor.location;
                                        ulong num12 = (ulong) num17.sqrMagnitudeLong2D;
                                        if ((num12 < num3) && InActor.CanAttack(target))
                                        {
                                            root3 = target;
                                            num3 = num12;
                                        }
                                    }
                                    else
                                    {
                                        VInt3 num18 = target.location - InActor.location;
                                        ulong num13 = (ulong) num18.sqrMagnitudeLong2D;
                                        if ((num13 < num4) && InActor.CanAttack(target))
                                        {
                                            root4 = target;
                                            num4 = num13;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            ActorRoot root7 = null;
            if (root != null)
            {
                return root;
            }
            if (root2 != null)
            {
                return root2;
            }
            if (root3 != null)
            {
                return root3;
            }
            if (root4 != null)
            {
                return root4;
            }
            if (root5 != null)
            {
                root7 = root5;
            }
            return root7;
        }

        public ActorRoot GetNearestEnemyIgnoreVisible(ActorRoot InActor, int srchR, uint filter)
        {
            ActorRoot root = null;
            ulong num = (ulong) (srchR * srchR);
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
                        if ((filter & (((int) 1) << target.TheActorMeta.ActorType)) <= 0L)
                        {
                            VInt3 num6 = target.location - InActor.location;
                            ulong num5 = (ulong) num6.sqrMagnitudeLong2D;
                            if ((num5 < num) && InActor.CanAttack(target))
                            {
                                root = target;
                                num = num5;
                            }
                        }
                    }
                }
            }
            return root;
        }

        public ActorRoot GetNearestEnemyWithoutNotInBattleJungleMonster(ActorRoot InActor, int srchR)
        {
            ActorRoot root = null;
            ulong num = (ulong) (srchR * srchR);
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
                        if (target.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
                        {
                            MonsterWrapper wrapper = target.AsMonster();
                            if (wrapper != null)
                            {
                                ResMonsterCfgInfo cfgInfo = wrapper.cfgInfo;
                                if ((cfgInfo != null) && (cfgInfo.bMonsterType == 2))
                                {
                                    switch (target.ActorAgent.GetCurBehavior())
                                    {
                                        case ObjBehaviMode.State_Idle:
                                        case ObjBehaviMode.State_Dead:
                                        case ObjBehaviMode.State_Null:
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                            VInt3 num6 = target.location - InActor.location;
                            ulong num5 = (ulong) num6.sqrMagnitudeLong2D;
                            if ((num5 < num) && InActor.CanAttack(target))
                            {
                                root = target;
                                num = num5;
                            }
                        }
                    }
                }
            }
            return root;
        }

        public ActorRoot GetNearestEnemyWithoutNotInBattleJungleMonsterWithoutActor(ActorRoot InActor, int srchR, uint withOutActor)
        {
            if (InActor == null)
            {
                return null;
            }
            ActorRoot root = null;
            ulong num = (ulong) (srchR * srchR);
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
                        if (((target != null) && (target.ObjID != withOutActor)) && target.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
                        {
                            MonsterWrapper wrapper = target.AsMonster();
                            if (wrapper != null)
                            {
                                ResMonsterCfgInfo cfgInfo = wrapper.cfgInfo;
                                if ((cfgInfo != null) && (cfgInfo.bMonsterType == 2))
                                {
                                    switch (target.ActorAgent.GetCurBehavior())
                                    {
                                        case ObjBehaviMode.State_Idle:
                                        case ObjBehaviMode.State_Dead:
                                        case ObjBehaviMode.State_Null:
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                            VInt3 num6 = target.location - InActor.location;
                            ulong num5 = (ulong) num6.sqrMagnitudeLong2D;
                            if ((num5 < num) && InActor.CanAttack(target))
                            {
                                root = target;
                                num = num5;
                            }
                        }
                    }
                }
            }
            return root;
        }

        public ActorRoot GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonster(ActorRoot InActor, int srchR, TargetPriority priotity)
        {
            ActorRoot root = null;
            ActorRoot root2 = null;
            ulong num = (ulong) (srchR * srchR);
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
                        if (target.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
                        {
                            MonsterWrapper wrapper = target.AsMonster();
                            if (wrapper != null)
                            {
                                ResMonsterCfgInfo cfgInfo = wrapper.cfgInfo;
                                if ((cfgInfo != null) && (cfgInfo.bMonsterType == 2))
                                {
                                    switch (target.ActorAgent.GetCurBehavior())
                                    {
                                        case ObjBehaviMode.State_Idle:
                                        case ObjBehaviMode.State_Dead:
                                        case ObjBehaviMode.State_Null:
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                            if ((((priotity == TargetPriority.TargetPriority_Hero) && (target.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)) || ((priotity == TargetPriority.TargetPriority_Monster) && (target.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster))) || ((priotity == TargetPriority.TargetPriority_Organ) && (target.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)))
                            {
                                VInt3 num7 = target.location - InActor.location;
                                ulong num5 = (ulong) num7.sqrMagnitudeLong2D;
                                if ((num5 >= num) || !InActor.CanAttack(target))
                                {
                                    continue;
                                }
                                root = target;
                                num = num5;
                                if (priotity != TargetPriority.TargetPriority_Organ)
                                {
                                    continue;
                                }
                                break;
                            }
                            VInt3 num8 = target.location - InActor.location;
                            if ((num8.sqrMagnitudeLong2D < num) && InActor.CanAttack(target))
                            {
                                root2 = target;
                            }
                        }
                    }
                }
            }
            return ((root == null) ? root2 : root);
        }

        public ActorRoot GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor(ActorRoot InActor, int srchR, TargetPriority priotity, uint withOutActor)
        {
            ActorRoot root = null;
            ActorRoot root2 = null;
            ulong num = (ulong) (srchR * srchR);
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
                        if (((target != null) && (target.ObjID != withOutActor)) && target.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
                        {
                            MonsterWrapper wrapper = target.AsMonster();
                            if (wrapper != null)
                            {
                                ResMonsterCfgInfo cfgInfo = wrapper.cfgInfo;
                                if ((cfgInfo != null) && (cfgInfo.bMonsterType == 2))
                                {
                                    switch (target.ActorAgent.GetCurBehavior())
                                    {
                                        case ObjBehaviMode.State_Idle:
                                        case ObjBehaviMode.State_Dead:
                                        case ObjBehaviMode.State_Null:
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                            if ((((priotity == TargetPriority.TargetPriority_Hero) && (target.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)) || ((priotity == TargetPriority.TargetPriority_Monster) && (target.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster))) || ((priotity == TargetPriority.TargetPriority_Organ) && (target.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)))
                            {
                                VInt3 num7 = target.location - InActor.location;
                                ulong num5 = (ulong) num7.sqrMagnitudeLong2D;
                                if ((num5 >= num) || !InActor.CanAttack(target))
                                {
                                    continue;
                                }
                                root = target;
                                num = num5;
                                if (priotity != TargetPriority.TargetPriority_Organ)
                                {
                                    continue;
                                }
                                break;
                            }
                            VInt3 num8 = target.location - InActor.location;
                            if ((num8.sqrMagnitudeLong2D < num) && InActor.CanAttack(target))
                            {
                                root2 = target;
                            }
                        }
                    }
                }
            }
            return ((root == null) ? root2 : root);
        }

        public ActorRoot GetNearestFriendly(ActorRoot InActor, int srchR, uint filter)
        {
            ActorRoot root = null;
            ulong num = (ulong) (srchR * srchR);
            List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(InActor.TheActorMeta.ActorCamp);
            for (int i = 0; i < campActors.Count; i++)
            {
                PoolObjHandle<ActorRoot> handle = campActors[i];
                ActorRoot root2 = handle.handle;
                if (((filter & (((int) 1) << root2.TheActorMeta.ActorType)) <= 0L) && !root2.ActorControl.IsDeadState)
                {
                    VInt3 num4 = root2.location - InActor.location;
                    ulong num3 = (ulong) num4.sqrMagnitudeLong2D;
                    if (num3 < num)
                    {
                        root = root2;
                        num = num3;
                    }
                }
            }
            return root;
        }

        public ActorRoot GetNearestSelfCampHero(ActorRoot InActor, int range)
        {
            long num = range * range;
            ActorRoot root = null;
            List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(InActor.TheActorMeta.ActorCamp);
            int count = campActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = campActors[i];
                ActorRoot root2 = handle.handle;
                if (((root2.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && !root2.ActorControl.IsDeadState) && (root2.ObjID != InActor.ObjID))
                {
                    VInt3 num5 = root2.location - InActor.location;
                    long num4 = num5.sqrMagnitudeLong2D;
                    if (num4 < num)
                    {
                        root = root2;
                        num = num4;
                    }
                }
            }
            return root;
        }

        public ActorRoot GetNearRandomEnemy(ActorRoot InActor, int srchR)
        {
            ulong num = (ulong) (srchR * srchR);
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
                        if (target.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
                        {
                            VInt3 num6 = target.location - InActor.location;
                            if ((num6.sqrMagnitudeLong2D < num) && InActor.CanAttack(target))
                            {
                                return target;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public List<ActorRoot> GetOurCampActors(ActorRoot InActor, int srchR)
        {
            List<ActorRoot> list = null;
            long num = srchR * srchR;
            List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(InActor.TheActorMeta.ActorCamp);
            int count = campActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = campActors[i];
                ActorRoot item = handle.handle;
                if (item.ObjID != InActor.ObjID)
                {
                    VInt3 num5 = item.location - InActor.location;
                    if (num5.sqrMagnitudeLong2D < num)
                    {
                        if (list == null)
                        {
                            list = new List<ActorRoot>();
                        }
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public bool HasCantAttackEnemyBuilding(ActorRoot InActor, int srchR)
        {
            ulong num = (ulong) (srchR * srchR);
            List<PoolObjHandle<ActorRoot>> organActors = Singleton<GameObjMgr>.GetInstance().OrganActors;
            int count = organActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = organActors[i];
                ActorRoot actor = handle.handle;
                if (actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
                {
                    VInt3 num5 = actor.location - InActor.location;
                    if (((num5.sqrMagnitudeLong2D < num) && !actor.ActorControl.IsDeadState) && (!InActor.IsSelfCamp(actor) && !actor.AttackOrderReady))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HasEnemyBuildingAndEnemyBuildingWillAttackSelf(ActorRoot InActor, int srchR)
        {
            bool flag = false;
            ulong num = (ulong) (srchR * srchR);
            List<PoolObjHandle<ActorRoot>> organActors = Singleton<GameObjMgr>.GetInstance().OrganActors;
            int count = organActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = organActors[i];
                ActorRoot actor = handle.handle;
                if (actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
                {
                    VInt3 num9 = actor.location - InActor.location;
                    if (((num9.sqrMagnitudeLong2D < num) && !actor.ActorControl.IsDeadState) && !InActor.IsSelfCamp(actor))
                    {
                        flag = true;
                        long num5 = actor.ActorControl.AttackRange * actor.ActorControl.AttackRange;
                        List<PoolObjHandle<ActorRoot>> soldierActors = Singleton<GameObjMgr>.GetInstance().SoldierActors;
                        int num6 = soldierActors.Count;
                        for (int j = 0; j < num6; j++)
                        {
                            PoolObjHandle<ActorRoot> handle2 = soldierActors[j];
                            ActorRoot root2 = handle2.handle;
                            if (root2.IsSelfCamp(InActor))
                            {
                                VInt3 num10 = root2.location - actor.location;
                                if (num10.sqrMagnitudeLong2D < num5)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return flag;
        }

        public override void Init()
        {
            base.Init();
            this._coordHandler = new SceneManagement.Process(this.FilterCoordActor);
        }

        public void NotifySelfCampToAttack(PoolObjHandle<ActorRoot> InActor, int srchR, PoolObjHandle<ActorRoot> target)
        {
            long num = srchR;
            num *= num;
            List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(InActor.handle.TheActorMeta.ActorCamp);
            int count = campActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = campActors[i];
                ActorRoot root = handle.handle;
                if (root.ObjID != InActor.handle.ObjID)
                {
                    VInt3 num5 = root.location - InActor.handle.location;
                    if (num5.sqrMagnitudeLong2D < num)
                    {
                        root.ActorControl.SetHelpToAttackTarget(InActor, target);
                    }
                }
            }
        }
    }
}

