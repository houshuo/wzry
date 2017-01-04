namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class SkillSlot
    {
        public PoolObjHandle<ActorRoot> Actor;
        private bool bLimitUse;
        private int changeSkillCDRate;
        private int eventTime;
        public Skill InitSkillObj;
        private float keyPressTime;
        public long lLastUseTime;
        public const float MAX_KEY_PRESS_TIME = 0.1f;
        public const long SKILL_DISTANCE_CHECK_VALUE = 0x2ee0L;
        public SkillChangeEvent skillChangeEvent;
        public SkillControlIndicator skillIndicator;
        private int skillLevel;
        private uint skillUseCount;

        private SkillSlot()
        {
        }

        public SkillSlot(SkillSlotType type)
        {
            this.IsCDReady = true;
            this.SlotType = type;
            this.bLimitUse = false;
            this.skillChangeEvent = new SkillChangeEvent(this);
            this.skillIndicator = new SkillControlIndicator(this);
            this.changeSkillCDRate = 0;
            this.lLastUseTime = 0L;
            this.CurSkillCD = 0;
            this.skillUseCount = 0;
            this.keyPressTime = 0f;
        }

        public bool Abort(SkillAbortType _type)
        {
            if (this.SkillObj != null)
            {
                if (this.SkillObj.isFinish)
                {
                    return true;
                }
                if (!this.SkillObj.canAbort(_type))
                {
                    return false;
                }
                this.SkillObj.Stop();
                this.skillChangeEvent.Abort();
            }
            return true;
        }

        public void AddSkillUseCount()
        {
            if (this.skillUseCount >= uint.MaxValue)
            {
                this.skillUseCount = 0;
            }
            this.skillUseCount++;
            this.Actor.handle.BuffHolderComp.logicEffect.InitSkillSlotExtraHurt(this);
        }

        public void AutoSkillEnergyCost()
        {
            if (((this.SkillObj != null) && (this.SkillObj.cfgData != null)) && (this.SkillObj.cfgData.dwAutoEnergyCost == 0))
            {
                this.CurSkillEnergyCostTick();
            }
        }

        public void CancelUseSkill()
        {
            if (this.skillIndicator != null)
            {
                this.skillIndicator.UnInitIndicatePrefab(false);
            }
            this.keyPressTime = 0f;
        }

        public bool CanEnableSkillSlotByEnergy()
        {
            return ((this.IsCDReady && this.IsUnLock()) && (!this.bLimitUse && !this.Actor.handle.ActorControl.IsDeadState));
        }

        public void ChangeMaxCDRate(int _rate)
        {
            this.changeSkillCDRate += _rate;
        }

        public void ChangeSkillCD(int _time)
        {
            if (!this.IsCDReady)
            {
                this.CurSkillCD -= _time;
                if (this.CurSkillCD < 0)
                {
                    this.CurSkillCD = 0;
                    this.IsCDReady = true;
                }
                if (this.SlotType != SkillSlotType.SLOT_SKILL_0)
                {
                    DefaultSkillEventParam param = new DefaultSkillEventParam(this.SlotType, (int) this.CurSkillCD, this.Actor);
                    Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, this.Actor, ref param, GameSkillEventChannel.Channel_AllActor);
                    if (this.IsCDReady)
                    {
                        Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDEnd, this.Actor, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
                    }
                }
            }
        }

        public void CurSkillEnergyCostTick()
        {
            if (((this.Actor != 0) && (this.Actor.handle.ValueComponent != null)) && !this.Actor.handle.ValueComponent.IsEnergyType(ENERGY_TYPE.None))
            {
                this.Actor.handle.ValueComponent.actorEp -= this.CurSkillEnergyCostTotal();
            }
        }

        public int CurSkillEnergyCostTotal()
        {
            if (((this.Actor == 0) || (this.Actor.handle.SkillControl == null)) || !this.Actor.handle.SkillControl.bZeroCd)
            {
                int skillLevel = this.skillLevel;
                skillLevel = (skillLevel <= 0) ? 1 : skillLevel;
                if (this.SkillObj != null)
                {
                    return this.SkillObj.SkillEnergyCost(this.Actor, skillLevel);
                }
            }
            return 0;
        }

        public void DestoryIndicatePrefab()
        {
            if (this.skillIndicator != null)
            {
                this.skillIndicator.UnInitIndicatePrefab(true);
            }
        }

        public void ForceAbort()
        {
            if ((this.SkillObj != null) && !this.SkillObj.isFinish)
            {
                this.SkillObj.Stop();
                this.skillChangeEvent.Abort();
            }
        }

        public int GetSkillCDMax()
        {
            ValueDataInfo info = null;
            int iCoolDown = (int) this.SkillObj.cfgData.iCoolDown;
            int num2 = this.skillLevel - 1;
            if (num2 < 0)
            {
                num2 = 0;
            }
            iCoolDown += this.SkillObj.cfgData.iCoolDownGrowth * num2;
            if (this.Actor != 0)
            {
                int totalValue = 0;
                info = this.Actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_CDREDUCE];
                DebugHelper.Assert(info != null, "Failed get value data");
                if (info == null)
                {
                    return 10;
                }
                if (((info != null) && (this.SlotType != SkillSlotType.SLOT_SKILL_0)) && ((this.SlotType != SkillSlotType.SLOT_SKILL_4) && (this.SlotType != SkillSlotType.SLOT_SKILL_5)))
                {
                    totalValue = info.totalValue;
                }
                int num4 = totalValue + this.changeSkillCDRate;
                if (info.maxLimitValue > 0)
                {
                    num4 = (num4 <= info.maxLimitValue) ? num4 : info.maxLimitValue;
                }
                long num5 = iCoolDown * (0x2710L - num4);
                iCoolDown = (int) (num5 / 0x2710L);
            }
            iCoolDown = (iCoolDown >= 0) ? iCoolDown : 0;
            if (this.SlotType == SkillSlotType.SLOT_SKILL_0)
            {
                info = this.Actor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_ATKSPDADD];
                DebugHelper.Assert(info != null, "Failed get value data skill 0");
                iCoolDown = (iCoolDown * 0x2710) / (0x2710 + info.totalValue);
                iCoolDown = (iCoolDown >= 0) ? iCoolDown : 0;
            }
            if (((this.Actor != 0) && this.Actor.handle.SkillControl.bZeroCd) && (this.SlotType != SkillSlotType.SLOT_SKILL_0))
            {
                iCoolDown = 0;
            }
            return iCoolDown;
        }

        public int GetSkillLevel()
        {
            return this.skillLevel;
        }

        public uint GetSkillUseCount()
        {
            return this.skillUseCount;
        }

        public bool ImmediateAbort(SkillAbortType _type)
        {
            if (this.SkillObj != null)
            {
                if (this.SkillObj.isFinish)
                {
                    return true;
                }
                if (!this.SkillObj.canAbort(_type))
                {
                    return false;
                }
                this.SkillObj.Stop();
                this.skillChangeEvent.Abort();
            }
            return true;
        }

        public void Init(ref PoolObjHandle<ActorRoot> _actor, Skill skill, PassiveSkill passive)
        {
            this.Actor = _actor;
            this.IsCDReady = true;
            this.CurSkillCD = 0;
            this.SkillObj = skill;
            this.InitSkillObj = skill;
            this.NextSkillObj = null;
            this.SkillObj.SlotType = this.SlotType;
            this.PassiveSkillObj = passive;
            if (this.PassiveSkillObj != null)
            {
                this.PassiveSkillObj.SlotType = this.SlotType;
            }
        }

        public void InitSkillControlIndicator()
        {
            this.skillIndicator.InitControlIndicator();
            this.skillIndicator.CreateIndicatePrefab(this.SkillObj);
        }

        public bool IsAbort(SkillAbortType _type)
        {
            if (this.SkillObj != null)
            {
                if (this.SkillObj.isFinish)
                {
                    return true;
                }
                if (!this.SkillObj.canAbort(_type))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsEnableSkillSlot()
        {
            return ((this.IsCDReady && this.IsUnLock()) && ((!this.bLimitUse && !this.Actor.handle.ActorControl.IsDeadState) && this.IsEnergyEnough));
        }

        private bool IsSkillUnlock(int slotType)
        {
            if (((slotType >= 1) && (slotType <= 3)) && (this.Actor != 0))
            {
                SkillSlot skillSlot = this.Actor.handle.SkillControl.GetSkillSlot((SkillSlotType) slotType);
                if (skillSlot != null)
                {
                    return skillSlot.IsUnLock();
                }
            }
            return true;
        }

        public bool IsSkillUseValid(ref SkillUseParam _param)
        {
            Skill skill = (this.NextSkillObj == null) ? this.SkillObj : this.NextSkillObj;
            if (((skill != null) && (skill.cfgData != null)) && (this.Actor != 0))
            {
                long num = 0L;
                long num3 = 0L;
                if (((_param.SlotType != SkillSlotType.SLOT_SKILL_0) && (this.Actor != 0)) && ((this.Actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (_param.AppointType != skill.AppointType)))
                {
                    return false;
                }
                if ((skill.AppointType == SkillRangeAppointType.Target) || (skill.AppointType == SkillRangeAppointType.Auto))
                {
                    if (_param.bSpecialUse && (this.SlotType == SkillSlotType.SLOT_SKILL_0))
                    {
                        return true;
                    }
                    if (!this.IsValidSkillTarget(ref _param))
                    {
                        return false;
                    }
                    num3 = (skill.cfgData.iMaxAttackDistance * 0x2ee0L) / 0x2710L;
                    num3 += (_param.TargetActor.handle.shape == null) ? 0L : ((long) _param.TargetActor.handle.shape.AvgCollisionRadius);
                    num = num3 * num3;
                    VInt3 num4 = this.Actor.handle.location - _param.TargetActor.handle.location;
                    if (num4.sqrMagnitudeLong2D <= num)
                    {
                        return true;
                    }
                }
                else if (skill.AppointType == SkillRangeAppointType.Pos)
                {
                    num3 = (skill.cfgData.iMaxAttackDistance * 0x2ee0L) / 0x2710L;
                    num = num3 * num3;
                    VInt3 num5 = this.Actor.handle.location - _param.UseVector;
                    if (num5.sqrMagnitudeLong2D <= num)
                    {
                        return true;
                    }
                }
                else
                {
                    if (skill.AppointType == SkillRangeAppointType.Directional)
                    {
                        return true;
                    }
                    if (skill.AppointType == SkillRangeAppointType.Track)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsUnLock()
        {
            return (this.skillLevel > 0);
        }

        public bool IsUseSkillJoystick()
        {
            if ((this.SkillObj == null) || (this.SkillObj.cfgData == null))
            {
                return false;
            }
            if (!this.IsEnableSkillSlot() && !this.Actor.handle.ActorControl.IsUseAdvanceCommonAttack())
            {
                return false;
            }
            if ((this.SlotType == SkillSlotType.SLOT_SKILL_0) && !this.Actor.handle.ActorControl.IsUseAdvanceCommonAttack())
            {
                return false;
            }
            return true;
        }

        private bool IsValidSkillTarget(ref SkillUseParam _param)
        {
            Skill skill = (this.NextSkillObj == null) ? this.SkillObj : this.NextSkillObj;
            if (_param.TargetActor != 0)
            {
                if (((skill.cfgData.dwSkillTargetRule == 0) || (skill.cfgData.dwSkillTargetRule == 1)) || (skill.cfgData.dwSkillTargetRule == 3))
                {
                    if (this.Actor.handle.IsEnemyCamp((ActorRoot) _param.TargetActor) && this.IsValidSkillTargetFilter(ref _param.TargetActor))
                    {
                        return true;
                    }
                }
                else if (skill.cfgData.dwSkillTargetRule == 2)
                {
                    if (this.Actor == _param.TargetActor)
                    {
                        return true;
                    }
                }
                else if (((skill.cfgData.dwSkillTargetRule == 4) && this.Actor.handle.IsSelfCamp((ActorRoot) _param.TargetActor)) && this.IsValidSkillTargetFilter(ref _param.TargetActor))
                {
                    if ((skill.cfgData.bWheelType == 1) && (this.Actor == _param.TargetActor))
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        private bool IsValidSkillTargetFilter(ref PoolObjHandle<ActorRoot> _target)
        {
            Skill skill = (this.NextSkillObj == null) ? this.SkillObj : this.NextSkillObj;
            if ((skill.cfgData.dwSkillTargetFilter != 0) && ((skill.cfgData.dwSkillTargetFilter & (((int) 1) << _target.handle.TheActorMeta.ActorType)) > 0L))
            {
                return false;
            }
            return true;
        }

        public void LateUpdate(int nDelta)
        {
            this.skillIndicator.LateUpdate(nDelta);
        }

        public int NextSkillEnergyCostTotal()
        {
            if (((this.Actor == 0) || (this.Actor.handle.SkillControl == null)) || !this.Actor.handle.SkillControl.bZeroCd)
            {
                int skillLevel = this.skillLevel;
                skillLevel = (skillLevel <= 0) ? 1 : skillLevel;
                if (this.NextSkillObj != null)
                {
                    return this.NextSkillObj.SkillEnergyCost(this.Actor, skillLevel);
                }
                if (this.SkillObj != null)
                {
                    return this.SkillObj.SkillEnergyCost(this.Actor, skillLevel);
                }
            }
            return 0;
        }

        public void ReadySkillObj()
        {
            if (this.NextSkillObj != null)
            {
                this.SkillObj = this.NextSkillObj;
                this.NextSkillObj = null;
                this.skillChangeEvent.Stop();
            }
        }

        public void ReadyUseSkill()
        {
            Skill readySkillObj = (this.NextSkillObj == null) ? this.SkillObj : this.NextSkillObj;
            if (((readySkillObj != null) && (readySkillObj.cfgData != null)) && (readySkillObj.cfgData.bWheelType != 1))
            {
                OperateMode playerOperateMode = ActorHelper.GetPlayerOperateMode(ref this.Actor);
                if (!Singleton<GameInput>.GetInstance().IsSmartUse() && (playerOperateMode == OperateMode.DefaultMode))
                {
                    this.ReadyUseSkillDefaultAttackMode(readySkillObj);
                }
                else if (playerOperateMode == OperateMode.LockMode)
                {
                    this.ReadyUseSkillLockAttackMode(readySkillObj);
                }
            }
        }

        private void ReadyUseSkillDefaultAttackMode(Skill readySkillObj)
        {
            ActorRoot target = null;
            if (readySkillObj.AppointType != SkillRangeAppointType.Target)
            {
                target = Singleton<TargetSearcher>.GetInstance().GetLowestHpTarget(this.Actor.handle, (int) readySkillObj.cfgData.iMaxAttackDistance, TargetPriority.TargetPriority_Hero, readySkillObj.cfgData.dwSkillTargetFilter, true, true);
            }
            if (target != null)
            {
                this.skillIndicator.SetSkillUsePosition(target);
            }
            else
            {
                this.skillIndicator.SetSkillUseDefaultPosition();
            }
            if ((readySkillObj.AppointType == SkillRangeAppointType.Target) && (readySkillObj.cfgData.dwSkillTargetRule != 2))
            {
                this.skillIndicator.SetGuildPrefabShow(false);
                this.skillIndicator.SetGuildWarnPrefabShow(false);
                this.skillIndicator.SetUseAdvanceMode(false);
                this.skillIndicator.SetSkillUseDefaultPosition();
                this.skillIndicator.SetEffectPrefabShow(false);
                this.skillIndicator.SetFixedPrefabShow(true);
            }
            else
            {
                this.skillIndicator.SetGuildPrefabShow(true);
                this.skillIndicator.SetGuildWarnPrefabShow(false);
                this.skillIndicator.SetUseAdvanceMode(true);
            }
        }

        private void ReadyUseSkillLockAttackMode(Skill readySkillObj)
        {
            uint dwSkillTargetFilter = 0;
            SelectEnemyType selectLowHp = SelectEnemyType.SelectLowHp;
            uint lockTargetID = this.Actor.handle.LockTargetAttackModeControl.GetLockTargetID();
            if (!this.Actor.handle.LockTargetAttackModeControl.IsValidLockTargetID(lockTargetID))
            {
                Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.Actor);
                if (ownerPlayer != null)
                {
                    selectLowHp = ownerPlayer.AttackTargetMode;
                }
                if (readySkillObj.AppointType == SkillRangeAppointType.Target)
                {
                    lockTargetID = 0;
                }
                else
                {
                    int iMaxAttackDistance = (int) readySkillObj.cfgData.iMaxAttackDistance;
                    dwSkillTargetFilter = readySkillObj.cfgData.dwSkillTargetFilter;
                    if (selectLowHp == SelectEnemyType.SelectLowHp)
                    {
                        lockTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.Actor, iMaxAttackDistance, dwSkillTargetFilter);
                    }
                    else
                    {
                        lockTargetID = Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.Actor, iMaxAttackDistance, dwSkillTargetFilter);
                    }
                }
            }
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(lockTargetID);
            if (actor != 0)
            {
                this.skillIndicator.SetSkillUsePosition(actor.handle);
            }
            else
            {
                this.skillIndicator.SetSkillUseDefaultPosition();
            }
            if ((readySkillObj.AppointType == SkillRangeAppointType.Target) && (readySkillObj.cfgData.dwSkillTargetRule != 2))
            {
                this.skillIndicator.SetGuildPrefabShow(false);
                this.skillIndicator.SetGuildWarnPrefabShow(false);
                this.skillIndicator.SetUseAdvanceMode(false);
                this.skillIndicator.SetSkillUseDefaultPosition();
                this.skillIndicator.SetEffectPrefabShow(false);
                this.skillIndicator.SetFixedPrefabShow(true);
            }
            else
            {
                this.skillIndicator.SetGuildPrefabShow(true);
                this.skillIndicator.SetGuildWarnPrefabShow(false);
                this.skillIndicator.SetUseAdvanceMode(true);
            }
        }

        public void RequestUseSkill(enSkillJoystickMode mode, uint objID)
        {
            if (mode == enSkillJoystickMode.General)
            {
                this.RequestUseSkillGeneralMode();
            }
            else if (mode == enSkillJoystickMode.SelectTarget)
            {
                this.RequestUseSkillSelectMode(objID);
            }
        }

        private void RequestUseSkillGeneralMode()
        {
            bool flag = false;
            Skill skill = (this.NextSkillObj == null) ? this.SkillObj : this.NextSkillObj;
            this.skillIndicator.SetFixedPrefabShow(false);
            this.skillIndicator.SetGuildPrefabShow(false);
            this.skillIndicator.SetGuildWarnPrefabShow(false);
            if (((Time.realtimeSinceStartup - this.keyPressTime) >= 0.1f) && (((skill != null) && (skill.cfgData != null)) && this.IsEnableSkillSlot()))
            {
                if (Singleton<SkillDetectionControl>.GetInstance().Detection((SkillUseRule) skill.cfgData.dwSkillUseRule, this))
                {
                    flag = this.SendRequestUseSkill();
                    if (flag)
                    {
                        this.keyPressTime = Time.realtimeSinceStartup;
                    }
                }
                if (!flag)
                {
                    ActorSkillEventParam param = new ActorSkillEventParam(this.Actor, SkillSlotType.SLOT_SKILL_0);
                    Singleton<GameSkillEventSys>.GetInstance().SendEvent<ActorSkillEventParam>(GameSkillEventDef.Event_NoSkillTarget, this.Actor, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
                }
            }
        }

        private void RequestUseSkillSelectMode(uint objID)
        {
            Skill skill = (this.NextSkillObj == null) ? this.SkillObj : this.NextSkillObj;
            if (((objID == 0) || (skill == null)) || ((skill.cfgData == null) || !this.IsEnableSkillSlot()))
            {
                MonoSingleton<CameraSystem>.GetInstance().SetFocusActor(this.Actor);
            }
            else
            {
                PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
                if ((actor == 0) || actor.handle.ActorControl.IsDeadState)
                {
                    MonoSingleton<CameraSystem>.GetInstance().SetFocusActor(this.Actor);
                }
                else if (Singleton<SkillDetectionControl>.GetInstance().Detection((SkillUseRule) skill.cfgData.dwSkillUseRule, this))
                {
                    FrameCommand<UseObjectiveSkillCommand> command = FrameCommandFactory.CreateCSSyncFrameCommand<UseObjectiveSkillCommand>();
                    command.cmdData.ObjectID = objID;
                    command.cmdData.SlotType = this.SlotType;
                    command.cmdData.iSkillID = skill.SkillID;
                    command.Send(true);
                }
                else
                {
                    MonoSingleton<CameraSystem>.GetInstance().SetFocusActor(this.Actor);
                }
            }
        }

        public void Reset()
        {
            this.Actor.Validate();
            if (this.PassiveSkillObj != null)
            {
                this.PassiveSkillObj.Reset();
            }
            this.NextSkillObj = null;
            this.CurSkillCD = 0;
            this.skillUseCount = 0;
            this.IsCDReady = true;
            if (this.skillChangeEvent != null)
            {
                this.skillChangeEvent.Reset();
            }
            this.lLastUseTime = 0L;
            this.keyPressTime = 0f;
        }

        public void ResetSkillCD()
        {
            if (!this.IsCDReady)
            {
                this.CurSkillCD = 0;
                this.IsCDReady = true;
                if (this.SlotType != SkillSlotType.SLOT_SKILL_0)
                {
                    DefaultSkillEventParam param = new DefaultSkillEventParam(this.SlotType, (int) this.CurSkillCD, this.Actor);
                    Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, this.Actor, ref param, GameSkillEventChannel.Channel_AllActor);
                    if (this.IsCDReady)
                    {
                        Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDEnd, this.Actor, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
                    }
                }
            }
        }

        public void ResetSkillObj()
        {
            this.SkillObj = this.InitSkillObj;
            this.skillChangeEvent.Stop();
            this.NextSkillObj = null;
        }

        private bool SendRequestUseSkill()
        {
            Skill readySkillObj = (this.NextSkillObj == null) ? this.SkillObj : this.NextSkillObj;
            if (readySkillObj == null)
            {
                return false;
            }
            if (readySkillObj.AppointType == SkillRangeAppointType.Target)
            {
                return this.SendRequestUseSkillTarget(readySkillObj);
            }
            if (readySkillObj.AppointType == SkillRangeAppointType.Directional)
            {
                this.SendRequestUseSkillDir(readySkillObj);
            }
            else if (readySkillObj.AppointType == SkillRangeAppointType.Pos)
            {
                this.SendRequestUseSkillPos(readySkillObj);
            }
            return true;
        }

        private bool SendRequestUseSkillDir(Skill readySkillObj)
        {
            VInt3 one = VInt3.one;
            BaseAttackMode currentAttackMode = this.Actor.handle.ActorControl.GetCurrentAttackMode();
            FrameCommand<UseDirectionalSkillCommand> command = FrameCommandFactory.CreateCSSyncFrameCommand<UseDirectionalSkillCommand>();
            if (currentAttackMode != null)
            {
                one = currentAttackMode.SelectSkillDirection(this);
            }
            command.cmdData.SlotType = this.SlotType;
            command.cmdData.Direction = one;
            command.cmdData.iSkillID = readySkillObj.SkillID;
            command.Send(true);
            return true;
        }

        private bool SendRequestUseSkillPos(Skill readySkillObj)
        {
            bool flag = false;
            VInt3 zero = VInt3.zero;
            BaseAttackMode currentAttackMode = this.Actor.handle.ActorControl.GetCurrentAttackMode();
            FrameCommand<UsePositionSkillCommand> command = FrameCommandFactory.CreateCSSyncFrameCommand<UsePositionSkillCommand>();
            if (currentAttackMode != null)
            {
                flag = currentAttackMode.SelectSkillPos(this, out zero);
            }
            if (flag)
            {
                command.cmdData.Position = zero;
            }
            else
            {
                return false;
            }
            command.cmdData.SlotType = this.SlotType;
            command.cmdData.iSkillID = readySkillObj.SkillID;
            command.Send(true);
            return true;
        }

        private bool SendRequestUseSkillTarget(Skill readySkillObj)
        {
            uint num = 0;
            BaseAttackMode currentAttackMode = this.Actor.handle.ActorControl.GetCurrentAttackMode();
            FrameCommand<UseObjectiveSkillCommand> command = FrameCommandFactory.CreateCSSyncFrameCommand<UseObjectiveSkillCommand>();
            if (currentAttackMode != null)
            {
                num = currentAttackMode.SelectSkillTarget(this);
            }
            if (num == 0)
            {
                return false;
            }
            command.cmdData.ObjectID = num;
            command.cmdData.SlotType = this.SlotType;
            command.cmdData.iSkillID = readySkillObj.SkillID;
            command.Send(true);
            return true;
        }

        public void SendSkillCooldownEvent()
        {
            if (!this.IsCDReady)
            {
                ActorSkillEventParam param = new ActorSkillEventParam(this.Actor, SkillSlotType.SLOT_SKILL_0);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<ActorSkillEventParam>(GameSkillEventDef.Event_SkillCooldown, this.Actor, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
            }
        }

        public void SendSkillShortageEvent()
        {
            if (!this.IsEnergyEnough)
            {
                ActorSkillEventParam param = new ActorSkillEventParam(this.Actor, SkillSlotType.SLOT_SKILL_0);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<ActorSkillEventParam>(GameSkillEventDef.Enent_EnergyShortage, this.Actor, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
            }
        }

        public void SetMaxCDRate(int inRate)
        {
            this.changeSkillCDRate = inRate;
        }

        public void SetSkillLevel(int _level)
        {
            this.skillLevel = _level;
            if (this.skillLevel == 0)
            {
                DefaultSkillEventParam param = new DefaultSkillEventParam(this.SlotType, 0, this.Actor);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_DisableSkill, this.Actor, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
            }
            else if (this.skillLevel == 1)
            {
                DefaultSkillEventParam param2 = new DefaultSkillEventParam(this.SlotType, 0, this.Actor);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_EnableSkill, this.Actor, ref param2, GameSkillEventChannel.Channel_HostCtrlActor);
            }
        }

        public void StartSkillCD()
        {
            this.CurSkillCD = this.GetSkillCDMax();
            this.IsCDReady = false;
            this.eventTime = 0;
            DefaultSkillEventParam param = new DefaultSkillEventParam(this.SlotType, (int) this.CurSkillCD, this.Actor);
            if (this.SlotType != SkillSlotType.SLOT_SKILL_0)
            {
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDStart, this.Actor, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, this.Actor, ref param, GameSkillEventChannel.Channel_AllActor);
            }
        }

        public void UnInit()
        {
            this.Actor.Release();
            this.SkillObj = null;
            this.InitSkillObj = null;
            this.NextSkillObj = null;
            this.PassiveSkillObj = null;
            this.skillIndicator.UnInitIndicatePrefab(true);
        }

        public void UpdateLogic(int nDelta)
        {
            this.UpdateSkillCD(nDelta);
            this.skillChangeEvent.UpdateSkillCD(nDelta);
            if (((this.PassiveSkillObj != null) && (this.PassiveSkillObj.cfgData != null)) && this.IsSkillUnlock(this.PassiveSkillObj.cfgData.iSkillType))
            {
                this.PassiveSkillObj.UpdateLogic(nDelta);
            }
        }

        private void UpdateSkillCD(int nDelta)
        {
            if (!this.IsCDReady)
            {
                this.CurSkillCD -= nDelta;
                this.eventTime += nDelta;
                if (this.CurSkillCD <= 0)
                {
                    this.CurSkillCD = 0;
                    this.IsCDReady = true;
                }
                if ((this.eventTime >= 200) || (this.CurSkillCD == 0))
                {
                    if (this.SlotType != SkillSlotType.SLOT_SKILL_0)
                    {
                        DefaultSkillEventParam param = new DefaultSkillEventParam(this.SlotType, (int) this.CurSkillCD, this.Actor);
                        Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, this.Actor, ref param, GameSkillEventChannel.Channel_AllActor);
                        if (this.IsCDReady)
                        {
                            Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_SkillCDEnd, this.Actor, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
                        }
                    }
                    this.eventTime -= 200;
                }
            }
        }

        public CrypticInt32 CurSkillCD { get; set; }

        public bool EnableButtonFlag
        {
            get
            {
                if (((this.Actor == 0) || (this.Actor.handle == null)) || (this.Actor.handle.ValueComponent == null))
                {
                    return false;
                }
                return (((this.CurSkillCD <= 0) && this.IsEnergyEnough) && ((this.skillLevel > 0) && !this.Actor.handle.ActorControl.IsDeadState));
            }
        }

        public bool IsCDReady { get; set; }

        public bool IsEnergyEnough
        {
            get
            {
                if (((this.Actor == 0) || (this.Actor.handle == null)) || (this.Actor.handle.ValueComponent == null))
                {
                    return false;
                }
                if (!this.Actor.handle.ValueComponent.IsEnergyType(ENERGY_TYPE.None) && (this.Actor.handle.ValueComponent.actorEp < this.NextSkillEnergyCostTotal()))
                {
                    return false;
                }
                return true;
            }
        }

        public Skill NextSkillObj { get; set; }

        public PassiveSkill PassiveSkillObj { get; set; }

        public Skill SkillObj { get; set; }

        public SkillSlotType SlotType { get; set; }
    }
}

