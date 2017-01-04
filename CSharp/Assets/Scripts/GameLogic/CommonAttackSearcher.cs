namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using System;

    internal class CommonAttackSearcher : Singleton<CommonAttackSearcher>
    {
        public uint AdvanceCommonAttackSearchEnemy(PoolObjHandle<ActorRoot> InActor, int srchR)
        {
            uint objID = 0;
            SkillSlot slot = null;
            bool useAdvanceMode = false;
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref InActor);
            if (InActor.handle.SkillControl.TryGetSkillSlot(SkillSlotType.SLOT_SKILL_0, out slot) && (slot.skillIndicator != null))
            {
                useAdvanceMode = slot.skillIndicator.GetUseAdvanceMode();
            }
            if (!useAdvanceMode)
            {
                SelectEnemyType selectLowHp;
                if (ownerPlayer == null)
                {
                    selectLowHp = SelectEnemyType.SelectLowHp;
                }
                else
                {
                    selectLowHp = ownerPlayer.AttackTargetMode;
                }
                if (selectLowHp == SelectEnemyType.SelectLowHp)
                {
                    return this.CommonAttackSearchLowestHpTarget(InActor.handle.ActorControl, srchR);
                }
                return this.CommonAttackSearchNearestTarget(InActor.handle.ActorControl, srchR);
            }
            ActorRoot useSkillTargetDefaultAttackMode = null;
            if ((slot != null) || (slot.skillIndicator != null))
            {
                useSkillTargetDefaultAttackMode = slot.skillIndicator.GetUseSkillTargetDefaultAttackMode();
                if (useSkillTargetDefaultAttackMode != null)
                {
                    objID = useSkillTargetDefaultAttackMode.ObjID;
                }
            }
            return objID;
        }

        public uint CommonAttackSearchEnemy(PoolObjHandle<ActorRoot> InActor, int srchR)
        {
            SelectEnemyType selectLowHp;
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref InActor);
            if (ownerPlayer == null)
            {
                selectLowHp = SelectEnemyType.SelectLowHp;
            }
            else
            {
                selectLowHp = ownerPlayer.AttackTargetMode;
            }
            if (selectLowHp == SelectEnemyType.SelectLowHp)
            {
                return this.CommonAttackSearchLowestHpTarget(InActor.handle.ActorControl, srchR);
            }
            return this.CommonAttackSearchNearestTarget(InActor.handle.ActorControl, srchR);
        }

        public uint CommonAttackSearchLowestHpPriorityHero(ObjWrapper _wrapper, int _srchR)
        {
            return 0;
        }

        public uint CommonAttackSearchLowestHpPriorityMonster(ObjWrapper _wrapper, int _srchR)
        {
            return 0;
        }

        public uint CommonAttackSearchLowestHpTarget(ObjWrapper _wrapper, int _srchR)
        {
            ActorRoot root = null;
            ActorRoot root2 = null;
            root = Singleton<TargetSearcher>.GetInstance().GetLowestHpTarget(_wrapper.actor, _wrapper.AttackRange, TargetPriority.TargetPriority_Hero, 0, true, false);
            if ((root == null) || (root.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero))
            {
                root2 = Singleton<TargetSearcher>.GetInstance().GetLowestHpTarget(_wrapper.actor, _wrapper.GreaterRange, TargetPriority.TargetPriority_Hero, 0, true, false);
            }
            if ((root2 != null) && (root2.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
            {
                root = root2;
            }
            if (root == null)
            {
                root = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(_wrapper.actor, _srchR, 0, false);
            }
            if (root != null)
            {
                return root.ObjID;
            }
            root = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(_wrapper.actor, _srchR, 0, true);
            if (root != null)
            {
                return root.ObjID;
            }
            return 0;
        }

        public uint CommonAttackSearchNearestPriorityHero(ObjWrapper _wrapper, int _srchR)
        {
            return 0;
        }

        public uint CommonAttackSearchNearestPriorityMonster(ObjWrapper _wrapper, int _srchR)
        {
            return 0;
        }

        public uint CommonAttackSearchNearestTarget(ObjWrapper _wrapper, int _srchR)
        {
            ActorRoot root = null;
            root = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(_wrapper.actor, _wrapper.AttackRange, TargetPriority.TargetPriority_Hero, 0, false);
            if (root != null)
            {
                return root.ObjID;
            }
            root = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(_wrapper.actor, _wrapper.GreaterRange, TargetPriority.TargetPriority_Hero, 0, false);
            if (root != null)
            {
                return root.ObjID;
            }
            root = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(_wrapper.actor, _srchR, 0, false);
            if (root != null)
            {
                return root.ObjID;
            }
            root = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(_wrapper.actor, _srchR, 0, true);
            if (root != null)
            {
                return root.ObjID;
            }
            return 0;
        }
    }
}

