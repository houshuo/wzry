namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    public class LockTargetAttackMode : BaseAttackMode
    {
        private uint lockTargetID;

        public override bool CancelCommonAttackMode()
        {
            if (base.actor.SkillControl.SkillUseCache != null)
            {
                base.actor.SkillControl.SkillUseCache.SetCommonAttackMode(false);
            }
            return true;
        }

        public void ClearTargetID()
        {
            LockTargetEventParam param = new LockTargetEventParam(this.lockTargetID);
            Singleton<GameSkillEventSys>.GetInstance().SendEvent<LockTargetEventParam>(GameSkillEventDef.Event_ClearLockTarget, base.GetActor(), ref param, GameSkillEventChannel.Channel_HostCtrlActor);
            this.lockTargetID = 0;
        }

        public override uint CommonAttackSearchEnemy(int srchR)
        {
            SkillCache skillUseCache = null;
            SelectEnemyType selectLowHp = SelectEnemyType.SelectLowHp;
            uint lockTargetID = this.lockTargetID;
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
            if (ownerPlayer != null)
            {
                if (base.IsValidTargetID(lockTargetID))
                {
                    return lockTargetID;
                }
                selectLowHp = ownerPlayer.AttackTargetMode;
            }
            if (selectLowHp == SelectEnemyType.SelectLowHp)
            {
                lockTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, srchR, 0);
            }
            else
            {
                lockTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, srchR, 0);
            }
            if (!base.IsValidTargetID(lockTargetID))
            {
                skillUseCache = base.actor.ActorControl.actor.SkillControl.SkillUseCache;
                if ((skillUseCache != null) && !skillUseCache.GetSpecialCommonAttack())
                {
                    this.CancelCommonAttackMode();
                    lockTargetID = 0;
                }
            }
            if (lockTargetID == 0)
            {
                this.ClearTargetID();
                return lockTargetID;
            }
            this.SetLockTargetID(lockTargetID);
            return lockTargetID;
        }

        public uint GetLockTargetID()
        {
            return this.lockTargetID;
        }

        public bool IsValidLockTargetID(uint _targetID)
        {
            bool flag = false;
            if (_targetID > 0)
            {
                PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(_targetID);
                flag = ((((actor != 0) && !actor.handle.ObjLinker.Invincible) && (!actor.handle.ActorControl.IsDeadState && !base.actor.IsSelfCamp((ActorRoot) actor))) && actor.handle.HorizonMarker.IsVisibleFor(base.actor.TheActorMeta.ActorCamp)) && actor.handle.AttackOrderReady;
                if (!flag)
                {
                    return flag;
                }
                long num = 0x2710L;
                num *= num;
                VInt3 num2 = base.actor.ActorControl.actorLocation - actor.handle.location;
                if (num2.sqrMagnitudeLong2D > num)
                {
                    return false;
                }
            }
            return flag;
        }

        public bool IsValidSkillTargetID(uint _targetID, uint _targetMask)
        {
            if (_targetID <= 0)
            {
                return false;
            }
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(_targetID);
            return ((actor != 0) && ((_targetMask & (((int) 1) << actor.handle.TheActorMeta.ActorType)) <= 0L));
        }

        public override void OnDead()
        {
            this.ClearTargetID();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.lockTargetID = 0;
        }

        public override VInt3 SelectSkillDirection(SkillSlot _slot)
        {
            SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
            Skill skill = (_slot.NextSkillObj == null) ? _slot.SkillObj : _slot.NextSkillObj;
            return (VInt3) _slot.skillIndicator.GetUseSkillDirection();
        }

        public override bool SelectSkillPos(SkillSlot _slot, out VInt3 _position)
        {
            SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
            Skill skill = (_slot.NextSkillObj == null) ? _slot.SkillObj : _slot.NextSkillObj;
            if (_slot.skillIndicator.IsAllowUseSkill())
            {
                _position = (VInt3) _slot.skillIndicator.GetUseSkillPosition();
                return true;
            }
            _position = VInt3.zero;
            return false;
        }

        public override uint SelectSkillTarget(SkillSlot _slot)
        {
            int iMaxSearchDistance = 0;
            uint dwSkillTargetFilter = 0;
            ActorRoot useSkillTargetLockAttackMode = null;
            uint lockTargetID = this.lockTargetID;
            SelectEnemyType selectLowHp = SelectEnemyType.SelectLowHp;
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
            SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
            Skill skill = (_slot.NextSkillObj == null) ? _slot.SkillObj : _slot.NextSkillObj;
            dwSkillTargetFilter = skill.cfgData.dwSkillTargetFilter;
            if (skill.cfgData.dwSkillTargetRule == 2)
            {
                return base.actor.ObjID;
            }
            useSkillTargetLockAttackMode = _slot.skillIndicator.GetUseSkillTargetLockAttackMode();
            if (useSkillTargetLockAttackMode != null)
            {
                if (this.IsValidLockTargetID(useSkillTargetLockAttackMode.ObjID))
                {
                    lockTargetID = useSkillTargetLockAttackMode.ObjID;
                    Singleton<NetLockAttackTarget>.GetInstance().SendLockAttackTarget(lockTargetID);
                }
                return lockTargetID;
            }
            if (!this.IsValidLockTargetID(this.lockTargetID))
            {
                if (ownerPlayer != null)
                {
                    selectLowHp = ownerPlayer.AttackTargetMode;
                }
                if (skill.AppointType == SkillRangeAppointType.Target)
                {
                    iMaxSearchDistance = skill.cfgData.iMaxSearchDistance;
                }
                else
                {
                    iMaxSearchDistance = (int) skill.cfgData.iMaxAttackDistance;
                }
                if (selectLowHp == SelectEnemyType.SelectLowHp)
                {
                    lockTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, iMaxSearchDistance, dwSkillTargetFilter);
                }
                else
                {
                    lockTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, iMaxSearchDistance, dwSkillTargetFilter);
                }
                if (this.IsValidLockTargetID(lockTargetID))
                {
                    Singleton<NetLockAttackTarget>.GetInstance().SendLockAttackTarget(lockTargetID);
                }
                return lockTargetID;
            }
            if (!this.IsValidSkillTargetID(lockTargetID, dwSkillTargetFilter))
            {
                lockTargetID = 0;
            }
            return lockTargetID;
        }

        public void SetLockTargetID(uint _targetID)
        {
            if (this.lockTargetID != _targetID)
            {
                this.lockTargetID = _targetID;
                LockTargetEventParam param = new LockTargetEventParam(this.lockTargetID);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<LockTargetEventParam>(GameSkillEventDef.Event_LockTarget, base.GetActor(), ref param, GameSkillEventChannel.Channel_HostCtrlActor);
            }
        }

        public override void UpdateLogic(int nDelta)
        {
            if ((this.lockTargetID != 0) && !this.IsValidLockTargetID(this.lockTargetID))
            {
                this.ClearTargetID();
            }
        }
    }
}

