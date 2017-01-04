namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using ResData;
    using System;
    using System.Runtime.InteropServices;

    public class DefaultAttackMode : BaseAttackMode
    {
        private uint commonAttackTargetID;
        private uint showInfoTargetID;

        public override bool CancelCommonAttackMode()
        {
            if (base.actor.SkillControl.SkillUseCache != null)
            {
                base.actor.SkillControl.SkillUseCache.SetCommonAttackMode(false);
                this.ClearCommonAttackTarget();
            }
            return true;
        }

        public void ClearCommonAttackTarget()
        {
            if (this.commonAttackTargetID != 0)
            {
                this.commonAttackTargetID = 0;
            }
        }

        public void ClearShowTargetInfo()
        {
            if ((this.showInfoTargetID != 0) && ActorHelper.IsHostActor(ref this.actorPtr))
            {
                SelectTargetEventParam param = new SelectTargetEventParam(this.showInfoTargetID);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<SelectTargetEventParam>(GameSkillEventDef.Event_ClearTarget, base.GetActor(), ref param, GameSkillEventChannel.Channel_HostCtrlActor);
            }
            this.showInfoTargetID = 0;
        }

        public override uint CommonAttackSearchEnemy(int srchR)
        {
            SkillCache skillUseCache = null;
            SelectEnemyType selectLowHp = SelectEnemyType.SelectLowHp;
            uint commonAttackTargetID = this.commonAttackTargetID;
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
            if (ownerPlayer != null)
            {
                if (base.IsValidTargetID(commonAttackTargetID))
                {
                    this.SetCommonAttackTarget(commonAttackTargetID);
                    return commonAttackTargetID;
                }
                selectLowHp = ownerPlayer.AttackTargetMode;
            }
            if (selectLowHp == SelectEnemyType.SelectLowHp)
            {
                commonAttackTargetID = Singleton<CommonAttackSearcher>.GetInstance().CommonAttackSearchLowestHpTarget(base.actor.ActorControl, srchR);
            }
            else
            {
                commonAttackTargetID = Singleton<CommonAttackSearcher>.GetInstance().CommonAttackSearchNearestTarget(base.actor.ActorControl, srchR);
            }
            if (!base.IsValidTargetID(commonAttackTargetID))
            {
                skillUseCache = base.actor.ActorControl.actor.SkillControl.SkillUseCache;
                if ((skillUseCache != null) && !skillUseCache.GetSpecialCommonAttack())
                {
                    this.CancelCommonAttackMode();
                    commonAttackTargetID = 0;
                }
            }
            if (commonAttackTargetID == 0)
            {
                this.ClearCommonAttackTarget();
                return commonAttackTargetID;
            }
            this.SetCommonAttackTarget(commonAttackTargetID);
            return commonAttackTargetID;
        }

        public override void OnDead()
        {
            this.ClearCommonAttackTarget();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.commonAttackTargetID = 0;
            this.showInfoTargetID = 0;
        }

        public override VInt3 SelectSkillDirection(SkillSlot _slot)
        {
            SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
            Skill skill = (_slot.NextSkillObj == null) ? _slot.SkillObj : _slot.NextSkillObj;
            if (Singleton<GameInput>.GetInstance().IsSmartUse())
            {
                return instance.SelectTargetDir((SkillTargetRule) skill.cfgData.dwSkillTargetRule, _slot);
            }
            return (VInt3) _slot.skillIndicator.GetUseSkillDirection();
        }

        public override bool SelectSkillPos(SkillSlot _slot, out VInt3 _position)
        {
            bool bTarget = false;
            SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
            Skill skill = (_slot.NextSkillObj == null) ? _slot.SkillObj : _slot.NextSkillObj;
            if (Singleton<GameInput>.GetInstance().IsSmartUse())
            {
                _position = instance.SelectTargetPos((SkillTargetRule) skill.cfgData.dwSkillTargetRule, _slot, out bTarget);
                return bTarget;
            }
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
            ActorRoot useSkillTargetDefaultAttackMode;
            SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
            Skill skill = (_slot.NextSkillObj == null) ? _slot.SkillObj : _slot.NextSkillObj;
            if (Singleton<GameInput>.GetInstance().IsSmartUse() || (skill.cfgData.dwSkillTargetRule == 2))
            {
                useSkillTargetDefaultAttackMode = instance.SelectTarget((SkillTargetRule) skill.cfgData.dwSkillTargetRule, _slot);
            }
            else
            {
                useSkillTargetDefaultAttackMode = _slot.skillIndicator.GetUseSkillTargetDefaultAttackMode();
            }
            if ((useSkillTargetDefaultAttackMode != null) && base.IsValidTargetID(useSkillTargetDefaultAttackMode.ObjID))
            {
                this.SetShowTargetInfo(useSkillTargetDefaultAttackMode.ObjID);
            }
            return ((useSkillTargetDefaultAttackMode == null) ? 0 : useSkillTargetDefaultAttackMode.ObjID);
        }

        public void SetCommonAttackTarget(uint _targetID)
        {
            if (this.commonAttackTargetID != _targetID)
            {
                this.commonAttackTargetID = _targetID;
                this.SetShowTargetInfo(_targetID);
            }
        }

        public void SetShowTargetInfo(uint _TargetID)
        {
            this.showInfoTargetID = _TargetID;
            if (ActorHelper.IsHostActor(ref this.actorPtr))
            {
                SelectTargetEventParam param = new SelectTargetEventParam(this.showInfoTargetID);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<SelectTargetEventParam>(GameSkillEventDef.Event_SelectTarget, base.GetActor(), ref param, GameSkillEventChannel.Channel_HostCtrlActor);
            }
        }

        public override void UpdateLogic(int nDelta)
        {
            if ((this.showInfoTargetID != 0) && !base.IsValidTargetID(this.showInfoTargetID))
            {
                this.ClearShowTargetInfo();
            }
        }
    }
}

