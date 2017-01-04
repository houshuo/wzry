namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using ResData;
    using System;
    using System.Collections.Generic;

    internal class LockModeKeySelector : Singleton<LockModeKeySelector>
    {
        private PoolObjHandle<ActorRoot> m_CurSelectedActor;
        private AttackTargetType m_TargetType;

        private ActorRoot GetLowerValueTargetByTag(PoolObjHandle<ActorRoot> InActor, List<ActorRoot> actorList, SelectEnemyType type)
        {
            int count = actorList.Count;
            if (count <= 0)
            {
                return null;
            }
            if (count > 1)
            {
                int num2 = 0;
                while (num2 < count)
                {
                    if (actorList[num2].SelfPtr == InActor)
                    {
                        break;
                    }
                    num2++;
                }
                if (num2 < (count - 1))
                {
                    return actorList[num2 + 1];
                }
            }
            return actorList[0];
        }

        private uint GetLowerValueTargetIdByTag(PoolObjHandle<ActorRoot> InActor, List<ActorRoot> actorList, SelectEnemyType type)
        {
            ActorRoot root = null;
            int count = actorList.Count;
            if (count > 0)
            {
                if (count == 1)
                {
                    root = actorList[0];
                }
                else if (this.m_CurSelectedActor != 0)
                {
                    root = this.GetLowerValueTargetByTag(this.m_CurSelectedActor, actorList, type);
                }
                else
                {
                    root = actorList[0];
                }
            }
            if (root != null)
            {
                this.m_CurSelectedActor = root.SelfPtr;
                return root.ObjID;
            }
            return 0;
        }

        private uint GetSelectTargetByTag(AttackTargetType targetType, SelectEnemyType selectType)
        {
            List<ActorRoot> actorList = new List<ActorRoot>();
            List<ActorRoot> list2 = new List<ActorRoot>();
            List<ActorRoot> list3 = new List<ActorRoot>();
            List<ActorRoot> list4 = new List<ActorRoot>();
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if ((hostPlayer == null) || (hostPlayer.Captain == 0))
            {
                return 0;
            }
            PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
            List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
            int count = gameActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle2 = gameActors[i];
                ActorRoot handle = handle2.handle;
                if (handle.HorizonMarker.IsVisibleFor(captain.handle.TheActorMeta.ActorCamp) && captain.handle.CanAttack(handle))
                {
                    if (targetType == AttackTargetType.ATTACK_TARGET_HERO)
                    {
                        if (TypeSearchCondition.Fit(handle, ActorTypeDef.Actor_Type_Hero) && DistanceSearchCondition.Fit(handle, (ActorRoot) captain, captain.handle.ActorControl.SearchRange))
                        {
                            actorList.Add(handle);
                        }
                    }
                    else if (TypeSearchCondition.Fit(handle, ActorTypeDef.Actor_Type_Organ))
                    {
                        if (DistanceSearchCondition.Fit(handle, (ActorRoot) captain, captain.handle.ActorControl.SearchRange))
                        {
                            list4.Add(handle);
                        }
                    }
                    else if (TypeSearchCondition.Fit(handle, ActorTypeDef.Actor_Type_Monster) && DistanceSearchCondition.Fit(handle, (ActorRoot) captain, captain.handle.ActorControl.SearchRange))
                    {
                        MonsterWrapper wrapper = handle.AsMonster();
                        if (((wrapper.cfgInfo.bSoldierType == 7) || (wrapper.cfgInfo.bSoldierType == 8)) || (wrapper.cfgInfo.bSoldierType == 9))
                        {
                            list3.Add(handle);
                        }
                        else
                        {
                            list2.Add(handle);
                        }
                    }
                }
            }
            if (targetType == AttackTargetType.ATTACK_TARGET_HERO)
            {
                this.SortActorListByTag(captain, ref actorList, selectType);
                return this.GetLowerValueTargetIdByTag(captain, actorList, selectType);
            }
            this.SortActorListByTag(captain, ref list3, selectType);
            this.SortActorListByTag(captain, ref list2, selectType);
            this.SortActorListByTag(captain, ref list4, selectType);
            List<ActorRoot> list6 = new List<ActorRoot>();
            int num4 = 0;
            for (num4 = 0; num4 < list3.Count; num4++)
            {
                list6.Add(list3[num4]);
            }
            for (num4 = 0; num4 < list2.Count; num4++)
            {
                list6.Add(list2[num4]);
            }
            for (num4 = 0; num4 < list4.Count; num4++)
            {
                list6.Add(list4[num4]);
            }
            return this.GetLowerValueTargetIdByTag(captain, list6, selectType);
        }

        public override void Init()
        {
            this.m_TargetType = AttackTargetType.ATTACK_TARGET_HERO;
        }

        public void OnHandleClickSelectTargetBtn(AttackTargetType _targetType)
        {
            OperateMode defaultMode = OperateMode.DefaultMode;
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if (hostPlayer != null)
            {
                defaultMode = hostPlayer.GetOperateMode();
            }
            if (defaultMode != OperateMode.DefaultMode)
            {
                if (_targetType != this.m_TargetType)
                {
                    this.m_CurSelectedActor.Release();
                }
                SelectEnemyType selectLowHp = SelectEnemyType.SelectLowHp;
                if (hostPlayer != null)
                {
                    selectLowHp = hostPlayer.AttackTargetMode;
                }
                uint selectTargetByTag = 0;
                selectTargetByTag = this.GetSelectTargetByTag(_targetType, selectLowHp);
                Singleton<NetLockAttackTarget>.GetInstance().SendLockAttackTarget(selectTargetByTag);
                this.m_TargetType = _targetType;
            }
        }

        public void SelectAttackTarget(AttackTargetType _targetType)
        {
        }

        private void SortActorListByTag(PoolObjHandle<ActorRoot> InActor, ref List<ActorRoot> actorList, SelectEnemyType type)
        {
            if (actorList.Count > 1)
            {
                int count = actorList.Count;
                ulong[] numArray = new ulong[count];
                if (type == SelectEnemyType.SelectLowHp)
                {
                    numArray[0] = TargetProperty.GetPropertyHpRate(actorList[0], RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP);
                }
                else
                {
                    VInt3 num5 = InActor.handle.location - actorList[0].location;
                    numArray[0] = (ulong) num5.sqrMagnitudeLong2D;
                }
                ulong propertyHpRate = 0L;
                for (int i = 1; i < count; i++)
                {
                    ActorRoot root = actorList[i];
                    if (type == SelectEnemyType.SelectLowHp)
                    {
                        propertyHpRate = TargetProperty.GetPropertyHpRate(root, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP);
                    }
                    else
                    {
                        VInt3 num6 = InActor.handle.location - actorList[0].location;
                        propertyHpRate = (ulong) num6.sqrMagnitudeLong2D;
                    }
                    numArray[i] = propertyHpRate;
                    int index = i;
                    while (index >= 1)
                    {
                        if (propertyHpRate >= numArray[index - 1])
                        {
                            break;
                        }
                        numArray[index] = numArray[index - 1];
                        actorList[index] = actorList[index - 1];
                        index--;
                    }
                    numArray[index] = propertyHpRate;
                    actorList[index] = root;
                }
            }
        }
    }
}

