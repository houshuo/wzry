namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class ObjWrapper : LogicComponent
    {
        [HideInInspector]
        public IFrameCommand _curMoveCommand;
        public int _inAttackCoolTick;
        public int _inBattleCoolTick;
        private int _moveCmdTimeoutFrame;
        [HideInInspector]
        private PoolObjHandle<ActorRoot> _myTarget = new PoolObjHandle<ActorRoot>();
        protected byte actorSubSoliderType;
        protected byte actorSubType;
        private const int BATTLE_COOL_TICKS = 90;
        public bool bForceNotRevive;
        public int curMoveSeq;
        [HideInInspector]
        public IFrameCommand curSkillCommand;
        [HideInInspector]
        public SkillUseParam curSkillUseInfo = new SkillUseParam();
        public List<KeyValuePair<uint, ulong>> helpSelfActorList;
        public List<KeyValuePair<uint, ulong>> hurtSelfActorList;
        public ulong lastExtraHurtByLowHpBuffTime;
        private PoolObjHandle<ActorRoot> lastHeroAtker;
        private ulong lastHeroAtkLogicTime;
        private ulong lastKillLogicTime;
        private COM_PLAYERCAMP m_attackeeCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT;
        public int m_battle_cool_ticks;
        private ObjBehaviMode m_beforeOutOfControlBehaviMode;
        public WaypointsHolder m_curWaypointsHolder;
        public Waypoint m_curWaypointTarget;
        public VInt3 m_curWaypointTargetPosition = VInt3.zero;
        public GameObject m_deadPointGo;
        private int m_exposeTimerSeq = -1;
        public bool m_followOther;
        public bool m_isAttacked;
        public bool m_isAttackedByEnemyHero;
        public bool m_isAutoAI;
        public bool m_isControledByMan = true;
        public bool m_isCurWaypointEndPoint;
        public bool m_isNeedToHelpOther;
        public bool m_isStartPoint;
        public uint m_leaderID;
        [HideInInspector]
        public PoolObjHandle<ActorRoot> m_needToHelpOtherToActtackTarget = new PoolObjHandle<ActorRoot>();
        public PoolObjHandle<ActorRoot> m_needToHelpTarget = new PoolObjHandle<ActorRoot>();
        public bool m_offline;
        public OutOfControl m_outOfControl;
        public GeoPolygon m_rangePolygon;
        protected ReviveContext m_reviveContext;
        private int m_reviveTick;
        [HideInInspector]
        public PoolObjHandle<ActorRoot> m_tauntMeActor = new PoolObjHandle<ActorRoot>();
        [HideInInspector]
        public PoolObjHandle<ActorRoot> m_terrorMeActor = new PoolObjHandle<ActorRoot>();
        public const int MOVE_SLOWEST_SPEED = 0x4b0;
        public const int MOVE_STANDARD_SPEED = 0xfa0;
        public ObjBehaviMode myBehavior;
        [HideInInspector]
        public PoolObjHandle<ActorRoot> myLastAtker = new PoolObjHandle<ActorRoot>();
        public ObjBehaviMode nextBehavior;
        private int[] NoAbility = new int[10];
        private static PoolObjHandle<ActorRoot> NullTarget = new PoolObjHandle<ActorRoot>();

        public event ActorEventHandler eventActorAssist;

        public event ActorDeadEventHandler eventActorDead;

        public event ActorEventHandler eventActorEnterCombat;

        public event ActorEventHandler eventActorExitCombat;

        public event ActorEventHandler eventActorRevive;

        public ObjWrapper()
        {
            ReviveContext context = new ReviveContext {
                ReviveLife = 0x2710,
                ReviveEnergy = 0x2710,
                bBaseRevive = true
            };
            this.m_reviveContext = context;
            this.hurtSelfActorList = new List<KeyValuePair<uint, ulong>>();
            this.helpSelfActorList = new List<KeyValuePair<uint, ulong>>();
            this.m_outOfControl = new OutOfControl(false, OutOfControlType.Null);
            this.lastHeroAtker = new PoolObjHandle<ActorRoot>();
        }

        public virtual void AddDisableSkillFlag(SkillSlotType _type)
        {
            if ((base.actorPtr != 0) && (this.actorPtr.handle.SkillControl != null))
            {
                this.actorPtr.handle.SkillControl.SetDisableSkillSlot(_type, true);
            }
        }

        public void AddHelpSelfActor(PoolObjHandle<ActorRoot> actor)
        {
            if ((actor != 0) && (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
            {
                ulong logicFrameTick = Singleton<FrameSynchr>.instance.LogicFrameTick;
                int num2 = 0;
                while (num2 < this.helpSelfActorList.Count)
                {
                    KeyValuePair<uint, ulong> pair = this.helpSelfActorList[num2];
                    if (pair.Key == actor.handle.ObjID)
                    {
                        this.helpSelfActorList[num2] = new KeyValuePair<uint, ulong>(actor.handle.ObjID, logicFrameTick);
                        break;
                    }
                    num2++;
                }
                if (num2 >= this.helpSelfActorList.Count)
                {
                    this.helpSelfActorList.Add(new KeyValuePair<uint, ulong>(actor.handle.ObjID, logicFrameTick));
                }
            }
        }

        public void AddHurtSelfActor(PoolObjHandle<ActorRoot> actor)
        {
            if ((actor != 0) && (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
            {
                ulong logicFrameTick = Singleton<FrameSynchr>.instance.LogicFrameTick;
                int num2 = 0;
                while (num2 < this.hurtSelfActorList.Count)
                {
                    KeyValuePair<uint, ulong> pair = this.hurtSelfActorList[num2];
                    if (pair.Key == actor.handle.ObjID)
                    {
                        this.hurtSelfActorList[num2] = new KeyValuePair<uint, ulong>(actor.handle.ObjID, logicFrameTick);
                        break;
                    }
                    num2++;
                }
                if (num2 >= this.hurtSelfActorList.Count)
                {
                    this.hurtSelfActorList.Add(new KeyValuePair<uint, ulong>(actor.handle.ObjID, logicFrameTick));
                }
            }
        }

        public virtual int AddNoAbilityFlag(ObjAbilityType abt)
        {
            this.NoAbility[(int) abt]++;
            return this.NoAbility[(int) abt];
        }

        public virtual int AddNoAbilityFlag(ObjAbilityType abt, int iValue)
        {
            this.NoAbility[(int) abt] += iValue;
            return this.NoAbility[(int) abt];
        }

        public void AttackAlongRoute(WaypointsHolder InRoute)
        {
            if (!this.IsDeadState && (InRoute != null))
            {
                this.ClearTarget();
                this.ClearMoveCommand();
                this.m_curWaypointsHolder = InRoute;
                this.m_curWaypointTarget = this.m_curWaypointsHolder.startPoint;
                if (this.m_curWaypointTarget != null)
                {
                    this.m_curWaypointTargetPosition = new VInt3(this.m_curWaypointTarget.transform.position);
                }
                this.SetObjBehaviMode(ObjBehaviMode.Attack_Path);
            }
        }

        public void AttackSelectActor(uint ObjID)
        {
            if (!this.IsDeadState)
            {
                this.ClearMoveCommand();
                PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(ObjID);
                if (actor != 0)
                {
                    this.SelectTarget(actor);
                    this.SetObjBehaviMode(ObjBehaviMode.Attack_Lock);
                }
            }
        }

        public virtual void BeAttackHit(PoolObjHandle<ActorRoot> atker)
        {
            if (!base.actor.IsSelfCamp(atker.handle))
            {
                this.SetInBattle();
                atker.handle.ActorControl.SetInBattle();
                atker.handle.ActorControl.SetInAttack(base.actorPtr);
                DefaultGameEventParam prm = new DefaultGameEventParam(base.GetActor(), atker);
                Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorBeAttack, ref prm);
                this.m_isAttacked = true;
            }
        }

        public override void Born(ActorRoot owner)
        {
            base.Born(owner);
            this.m_isNeedToHelpOther = false;
            this.curMoveCommand = null;
            this._moveCmdTimeoutFrame = 0;
            this.m_curWaypointsHolder = null;
            this.m_curWaypointTarget = null;
            this.m_isCurWaypointEndPoint = false;
            this.m_isStartPoint = false;
            this.m_isControledByMan = true;
            this.m_isAutoAI = false;
            this.m_offline = false;
            this.m_followOther = false;
            this.m_leaderID = 0;
            this.m_isAttackedByEnemyHero = false;
            this.m_isAttacked = false;
            this.bForceNotRevive = false;
            base.actor.SkillControl = base.actor.CreateLogicComponent<SkillComponent>(base.actor);
            base.actor.ValueComponent = base.actor.CreateLogicComponent<ValueProperty>(base.actor);
            base.actor.HurtControl = base.actor.CreateLogicComponent<HurtComponent>(base.actor);
            base.actor.BuffHolderComp = base.actor.CreateLogicComponent<BuffHolderComponent>(base.actor);
            base.actor.AnimControl = base.actor.CreateLogicComponent<AnimPlayComponent>(base.actor);
            base.actor.HudControl = base.actor.CreateLogicComponent<HudComponent3D>(base.actor);
            base.actor.ActorAgent = base.actor.CreateActorComponent<ObjAgent>(base.actor);
            base.actor.HorizonMarker = base.actor.CreateLogicComponent<HorizonMarker>(base.actor);
        }

        public void BTExMoveCmd()
        {
            if (this.curMoveCommand != null)
            {
                if (this.curMoveCommand.cmdType == 1)
                {
                    FrameCommand<MoveToPosCommand> curMoveCommand = (FrameCommand<MoveToPosCommand>) this.curMoveCommand;
                    this.RealMovePosition(curMoveCommand.cmdData.destPosition, curMoveCommand.cmdId);
                }
                else if (this.curMoveCommand.cmdType == 2)
                {
                    FrameCommand<MoveDirectionCommand> command2 = (FrameCommand<MoveDirectionCommand>) this.curMoveCommand;
                    VInt3 direction = VInt3.right.RotateY(command2.cmdData.Degree);
                    this.curMoveSeq = command2.cmdData.nSeq;
                    this.RealMoveDirection(direction, command2.cmdId);
                }
            }
        }

        public void CacheNoramlAttack(IFrameCommand cmd, SkillSlotType InSlot)
        {
            if (!this.IsDeadState && !base.actor.SkillControl.IsDisableSkillSlot(InSlot))
            {
                if (!base.actor.SkillControl.AbortCurUseSkill((SkillAbortType) InSlot))
                {
                    if (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        base.actor.SkillControl.SkillUseCache.SetCacheNormalAttackContext(cmd);
                    }
                }
                else
                {
                    this.ClearTarget();
                    this.SetObjBehaviMode(ObjBehaviMode.Normal_Attack);
                }
            }
        }

        public bool CanAttack(ActorRoot target)
        {
            return base.actor.CanAttack(target);
        }

        public bool CancelCommonAttackMode()
        {
            bool flag = this.IsUseAdvanceCommonAttack();
            if (flag)
            {
                BaseAttackMode currentAttackMode = this.GetCurrentAttackMode();
                if (currentAttackMode != null)
                {
                    return currentAttackMode.CancelCommonAttackMode();
                }
            }
            return flag;
        }

        public bool CanUseSkill(SkillSlotType slot)
        {
            if (base.actor.SkillControl.IsDisableSkillSlot(slot))
            {
                return false;
            }
            return base.actor.SkillControl.CanUseSkill(slot);
        }

        private void ClearExposeTimer()
        {
            if (this.m_exposeTimerSeq >= 0)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(this.m_exposeTimerSeq);
            }
            this.m_exposeTimerSeq = -1;
            this.m_attackeeCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT;
        }

        public void ClearMoveCommand()
        {
            this.curMoveCommand = null;
            DefaultGameEventParam prm = new DefaultGameEventParam(base.GetActor(), base.GetActor());
            Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorClearMove, ref prm);
        }

        public void ClearMoveCommandWithOutNotice()
        {
            this.curMoveCommand = null;
        }

        public void ClearTarget()
        {
            this.myTarget = NullTarget;
        }

        private void ClearVariables()
        {
            this.myBehavior = ObjBehaviMode.State_Idle;
            this.nextBehavior = ObjBehaviMode.State_Idle;
            this._myTarget.Release();
            this.m_needToHelpOtherToActtackTarget.Release();
            this.m_isNeedToHelpOther = false;
            this.m_needToHelpTarget.Release();
            this.curMoveCommand = null;
            this._moveCmdTimeoutFrame = 0;
            this.curMoveSeq = 0;
            this.curSkillUseInfo.Reset();
            this.curSkillCommand = null;
            this.myLastAtker.Release();
            this.m_tauntMeActor.Release();
            this.m_terrorMeActor.Release();
            this.m_curWaypointsHolder = null;
            this.m_curWaypointTarget = null;
            this.m_curWaypointTargetPosition = VInt3.zero;
            this.m_isCurWaypointEndPoint = false;
            this.m_isStartPoint = false;
            this.m_isControledByMan = true;
            this.m_isAutoAI = false;
            this.m_offline = false;
            this.m_followOther = false;
            this.m_leaderID = 0;
            this.m_isAttackedByEnemyHero = false;
            this.m_isAttacked = false;
            Array.Clear(this.NoAbility, 0, this.NoAbility.Length);
            this.m_battle_cool_ticks = 0;
            this._inBattleCoolTick = 0;
            this._inAttackCoolTick = 0;
            this.m_reviveTick = 0;
            this.bForceNotRevive = false;
            ReviveContext context = new ReviveContext {
                ReviveLife = 0x2710,
                ReviveEnergy = 0x2710,
                bBaseRevive = true
            };
            this.m_reviveContext = context;
            this.hurtSelfActorList.Clear();
            this.helpSelfActorList.Clear();
            this.m_outOfControl.ResetOnUse();
            this.m_beforeOutOfControlBehaviMode = ObjBehaviMode.State_Idle;
            this.m_rangePolygon = null;
            this.m_deadPointGo = null;
            this.lastHeroAtker.Release();
            this.lastHeroAtkLogicTime = 0L;
            this.lastKillLogicTime = 0L;
            this.lastExtraHurtByLowHpBuffTime = 0L;
            this.eventActorDead = null;
            this.eventActorRevive = null;
            this.eventActorAssist = null;
            this.eventActorEnterCombat = null;
            this.eventActorExitCombat = null;
            this.ClearExposeTimer();
        }

        public virtual int ClrNoAbilityFlag(ObjAbilityType abt)
        {
            this.NoAbility[(int) abt] = 0;
            return 0;
        }

        public void CmdAttackMoveToDest(IFrameCommand cmd, VInt3 dest)
        {
            if (!this.IsDeadState)
            {
                this.ClearTarget();
                this.ClearMoveCommand();
                this.curMoveCommand = cmd;
                this.SetObjBehaviMode(ObjBehaviMode.Attack_Move);
            }
        }

        public virtual void CmdCommonAttackMode(IFrameCommand cmd, sbyte Start, uint ObjID)
        {
            if (!this.IsDeadState)
            {
                if (Start == 0)
                {
                    this.nextBehavior = ObjBehaviMode.State_Idle;
                    if (!this.IsUseAdvanceCommonAttack() && (base.actor.SkillControl.SkillUseCache != null))
                    {
                        base.actor.SkillControl.SkillUseCache.SetCommonAttackMode(false);
                    }
                    if (base.actor.SkillControl.SkillUseCache != null)
                    {
                        base.actor.SkillControl.SkillUseCache.SetSpecialCommonAttack(false);
                    }
                    base.actor.SkillControl.SetCommonAttackIndicator(false);
                }
                else
                {
                    if (!base.actor.SkillControl.isUsing)
                    {
                        this.ClearTarget();
                        this.curSkillCommand = cmd;
                        this.SetObjBehaviMode(ObjBehaviMode.Normal_Attack);
                    }
                    else
                    {
                        this.ClearTarget();
                        this.curSkillCommand = cmd;
                        this.CacheNoramlAttack(cmd, SkillSlotType.SLOT_SKILL_0);
                    }
                    if (base.actor.SkillControl.SkillUseCache != null)
                    {
                        base.actor.SkillControl.SkillUseCache.SetCommonAttackMode(true);
                        base.actor.SkillControl.SkillUseCache.SetSpecialCommonAttack(true);
                        base.actor.SkillControl.SkillUseCache.SetNewAttackCommand();
                    }
                    base.actor.SkillControl.SetCommonAttackType((CommonAttackType) ObjID);
                    base.actor.SkillControl.SetCommonAttackIndicator(true);
                }
            }
        }

        public virtual void CmdCommonLearnSkill(IFrameCommand cmd)
        {
        }

        public virtual void CmdMoveDirection(IFrameCommand cmd, int nDegree)
        {
            if (!this.IsDeadState)
            {
                this.curMoveCommand = cmd;
                if (!base.actor.SkillControl.AbortCurUseSkill(SkillAbortType.TYPE_MOVE))
                {
                    if (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        base.actor.SkillControl.SkillUseCache.SetCacheMoveCommand(this.curMoveCommand);
                    }
                }
                else if (this.NoAbility[1] == 0)
                {
                    if ((this.curSkillCommand != null) && (this.curSkillCommand.frameNum == this.curMoveCommand.frameNum))
                    {
                        this.SetAutoAI(false);
                    }
                    else
                    {
                        this.SetAutoAI(false);
                        this.SetObjBehaviMode(ObjBehaviMode.Direction_Move);
                    }
                }
            }
        }

        public virtual void CmdMovePosition(IFrameCommand cmd, VInt3 dest)
        {
            if (!this.IsDeadState)
            {
                this.curMoveCommand = cmd;
                if ((this.NoAbility[1] == 0) && base.actor.SkillControl.AbortCurUseSkill(SkillAbortType.TYPE_MOVE))
                {
                    this.SetAutoAI(false);
                    this.SetObjBehaviMode(ObjBehaviMode.Destination_Move);
                }
            }
        }

        public void CmdStopMove()
        {
            if (!this.IsDeadState)
            {
                this.TerminateMove();
                this.ClearMoveCommand();
                if ((this.myBehavior == ObjBehaviMode.Direction_Move) || (this.myBehavior == ObjBehaviMode.Destination_Move))
                {
                    this.SetObjBehaviMode(ObjBehaviMode.State_Idle);
                }
            }
        }

        public void CmdUseSkill(IFrameCommand cmd, ref SkillUseParam param)
        {
            SkillSlot slot = null;
            int num = 0;
            if ((!this.IsDeadState && !base.actor.SkillControl.IsDisableSkillSlot(param.SlotType)) && base.actor.SkillControl.TryGetSkillSlot(param.SlotType, out slot))
            {
                Skill skill = (slot.NextSkillObj == null) ? slot.SkillObj : slot.NextSkillObj;
                if ((skill == null) || (skill.AppointType == param.AppointType))
                {
                    if (!base.actor.ValueComponent.IsEnergyType(ENERGY_TYPE.None))
                    {
                        if (!slot.IsEnergyEnough)
                        {
                            return;
                        }
                        num = slot.NextSkillEnergyCostTotal();
                    }
                    if (((slot.SkillObj != null) && (slot.SkillObj.cfgData != null)) && (slot.SkillObj.cfgData.bImmediateUse == 1))
                    {
                        if (((base.actor.SkillControl.CurUseSkillSlot != null) && (base.actor.SkillControl.CurUseSkillSlot.SkillObj != null)) && ((base.actor.SkillControl.CurUseSkillSlot.SkillObj.cfgData != null) && (base.actor.SkillControl.CurUseSkillSlot.SkillObj.cfgData.bIsInterruptImmediateUseSkill == 1)))
                        {
                            base.actor.SkillControl.AbortCurUseSkill((SkillAbortType) param.SlotType);
                        }
                        this.ImmediateUseSkill(cmd, ref param, slot);
                        base.actor.ValueComponent.actorEp -= num;
                        slot.StartSkillCD();
                    }
                    else if (!base.actor.SkillControl.AbortCurUseSkill((SkillAbortType) param.SlotType))
                    {
                        if (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                        {
                            base.actor.SkillControl.SkillUseCache.SetCacheSkillContext(cmd, param);
                        }
                    }
                    else
                    {
                        base.actor.SkillControl.CurUseSkillSlot = slot;
                        this.curSkillCommand = cmd;
                        this.curSkillUseInfo = param;
                        if (param.TargetActor != 0)
                        {
                            this.SelectTarget(param.TargetActor);
                        }
                        switch (param.SlotType)
                        {
                            case SkillSlotType.SLOT_SKILL_0:
                                this.SetObjBehaviMode(ObjBehaviMode.UseSkill_0);
                                break;

                            case SkillSlotType.SLOT_SKILL_1:
                                this.SetObjBehaviMode(ObjBehaviMode.UseSkill_1);
                                break;

                            case SkillSlotType.SLOT_SKILL_2:
                                this.SetObjBehaviMode(ObjBehaviMode.UseSkill_2);
                                break;

                            case SkillSlotType.SLOT_SKILL_3:
                                this.SetObjBehaviMode(ObjBehaviMode.UseSkill_3);
                                break;

                            case SkillSlotType.SLOT_SKILL_4:
                                this.SetObjBehaviMode(ObjBehaviMode.UseSkill_4);
                                break;

                            case SkillSlotType.SLOT_SKILL_5:
                                this.SetObjBehaviMode(ObjBehaviMode.UseSkill_5);
                                break;

                            case SkillSlotType.SLOT_SKILL_6:
                                this.SetObjBehaviMode(ObjBehaviMode.UseSkill_6);
                                break;

                            case SkillSlotType.SLOT_SKILL_7:
                                this.SetObjBehaviMode(ObjBehaviMode.UseSkill_7);
                                break;
                        }
                    }
                }
            }
        }

        public override void Deactive()
        {
            if (base.actor.ValueComponent != null)
            {
                base.actor.ValueComponent.HpChgEvent -= new ValueChangeDelegate(this.OnHpChange);
                base.actor.ValueComponent.SoulLevelChgEvent -= new ValueChangeDelegate(this.OnSoulLvlChange);
            }
            this.ClearVariables();
            if (base.actor.ActorAgent != null)
            {
                base.actor.ActorAgent.StopCurAgeAction();
                base.actor.ActorAgent.btresetcurrrent();
                base.actor.ActorAgent.m_variables.Clear();
            }
            base.Deactive();
        }

        public void DelayAbortCurUseSkill()
        {
            base.actor.SkillControl.DelayAbortCurUseSkill();
        }

        protected virtual bool DoesApplyExposingRule()
        {
            return false;
        }

        protected virtual bool DoesIgnoreAlreadyLit()
        {
            return true;
        }

        public void EnableRVO(bool enable)
        {
            if ((base.actor.MovementComponent != null) && (base.actor.MovementComponent.Pathfinding != null))
            {
                base.actor.MovementComponent.Pathfinding.EnableRVO(enable);
            }
        }

        public override void Fight()
        {
            if (!Singleton<FrameSynchr>.GetInstance().bActive)
            {
                this.m_battle_cool_ticks = 180;
            }
            else
            {
                this.m_battle_cool_ticks = 90;
            }
            this._inBattleCoolTick = this.m_battle_cool_ticks;
            this._inAttackCoolTick = this.m_battle_cool_ticks;
            base.actor.ValueComponent.SoulLevelChgEvent += new ValueChangeDelegate(this.OnSoulLvlChange);
            base.actor.ValueComponent.HpChgEvent += new ValueChangeDelegate(this.OnHpChange);
            base.actor.ValueComponent.mActorValue.SetChangeEvent(RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, new ValueChangeDelegate(this.OnHpChange));
            base.actor.ValueComponent.mActorValue.SetChangeEvent(RES_FUNCEFT_TYPE.RES_FUNCEFT_MOVESPD, new ValueChangeDelegate(this.OnMoveSpdChange));
            this.InitDefaultState();
        }

        public override void FightOver()
        {
            base.FightOver();
            this.SetObjBehaviMode(ObjBehaviMode.State_GameOver);
            this.TerminateMove();
            this.ClearMoveCommand();
            this.EnableRVO(false);
            this.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
            this.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Skill);
            this.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Control);
        }

        public void ForceAbortCurUseSkill()
        {
            base.actor.SkillControl.ForceAbortCurUseSkill();
        }

        public virtual byte GetActorSubSoliderType()
        {
            return this.actorSubSoliderType;
        }

        public virtual byte GetActorSubType()
        {
            return this.actorSubType;
        }

        public BaseAttackMode GetCurrentAttackMode()
        {
            OperateMode defaultMode = OperateMode.DefaultMode;
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
            if (ownerPlayer != null)
            {
                defaultMode = ownerPlayer.GetOperateMode();
            }
            if (defaultMode == OperateMode.DefaultMode)
            {
                return base.actor.DefaultAttackModeControl;
            }
            return base.actor.LockTargetAttackModeControl;
        }

        public uint GetLockTargetID()
        {
            if (base.actor.LockTargetAttackModeControl != null)
            {
                return base.actor.LockTargetAttackModeControl.GetLockTargetID();
            }
            return 0;
        }

        public Skill GetNextSkill(SkillSlotType _slotType)
        {
            SkillSlot slot = null;
            if (base.actor.SkillControl.TryGetSkillSlot(_slotType, out slot))
            {
                Skill skill = (slot.NextSkillObj == null) ? slot.SkillObj : slot.NextSkillObj;
                if ((skill != null) && (skill.cfgData != null))
                {
                    return skill;
                }
            }
            return null;
        }

        public bool GetNoAbilityFlag(ObjAbilityType abt)
        {
            return (this.NoAbility[(int) abt] > 0);
        }

        public virtual PoolObjHandle<ActorRoot> GetOrignalActor()
        {
            return base.actorPtr;
        }

        public VInt3 GetRouteCurWaypointPos()
        {
            Waypoint curWaypointTarget = this.m_curWaypointTarget;
            Vector3 vector = base.actor.location.vec3;
            Waypoint nextPoint = this.m_curWaypointsHolder.GetNextPoint(this.m_curWaypointTarget);
            if (nextPoint != null)
            {
                Vector3 position = this.m_curWaypointTarget.transform.position;
                Vector3 vector3 = nextPoint.transform.position;
                Vector3 vector4 = vector - vector3;
                vector4.y = 0f;
                Vector3 vector5 = position - vector3;
                vector5.y = 0f;
                Vector3 vector6 = vector - position;
                vector6.y = 0f;
                if (((vector4.sqrMagnitude < vector5.sqrMagnitude) || (vector4.sqrMagnitude < vector6.sqrMagnitude)) || (vector6.sqrMagnitude < 1f))
                {
                    this.m_curWaypointTarget = nextPoint;
                }
                this.m_isCurWaypointEndPoint = false;
            }
            else
            {
                this.m_isCurWaypointEndPoint = true;
            }
            if ((this.m_curWaypointTarget != curWaypointTarget) && (this.m_curWaypointTarget != null))
            {
                this.m_curWaypointTargetPosition = new VInt3(this.m_curWaypointTarget.transform.position);
            }
            VInt3 num3 = base.actorLocation - this.m_curWaypointTargetPosition;
            if (num3.sqrMagnitudeLong2D <= MonoSingleton<GlobalConfig>.instance.WaypointIgnoreDist)
            {
                Waypoint waypoint3 = this.m_curWaypointsHolder.GetNextPoint(this.m_curWaypointTarget);
                if (waypoint3 != null)
                {
                    this.m_curWaypointTarget = waypoint3;
                    this.m_curWaypointTargetPosition = new VInt3(this.m_curWaypointTarget.transform.position);
                }
                else
                {
                    this.m_isCurWaypointEndPoint = true;
                }
            }
            return this.m_curWaypointTargetPosition;
        }

        public VInt3 GetRouteCurWaypointPosPre()
        {
            Waypoint curWaypointTarget = this.m_curWaypointTarget;
            Vector3 vector = base.actor.location.vec3;
            Waypoint prePoint = this.m_curWaypointsHolder.GetPrePoint(this.m_curWaypointTarget);
            if (prePoint != null)
            {
                Vector3 position = this.m_curWaypointTarget.transform.position;
                Vector3 vector3 = prePoint.transform.position;
                Vector3 vector4 = vector - vector3;
                vector4.y = 0f;
                Vector3 vector5 = position - vector3;
                vector5.y = 0f;
                Vector3 vector6 = vector - position;
                vector6.y = 0f;
                if (((vector4.sqrMagnitude < vector5.sqrMagnitude) || (vector4.sqrMagnitude < vector6.sqrMagnitude)) || (vector6.sqrMagnitude < 1f))
                {
                    this.m_curWaypointTarget = prePoint;
                }
                this.m_isStartPoint = false;
            }
            else
            {
                this.m_isStartPoint = true;
            }
            if ((this.m_curWaypointTarget != curWaypointTarget) && (this.m_curWaypointTarget != null))
            {
                this.m_curWaypointTargetPosition = new VInt3(this.m_curWaypointTarget.transform.position);
            }
            VInt3 num3 = base.actorLocation - this.m_curWaypointTargetPosition;
            if (num3.sqrMagnitudeLong2D <= MonoSingleton<GlobalConfig>.instance.WaypointIgnoreDist)
            {
                Waypoint waypoint3 = this.m_curWaypointsHolder.GetPrePoint(this.m_curWaypointTarget);
                if (waypoint3 != null)
                {
                    this.m_curWaypointTarget = waypoint3;
                    this.m_curWaypointTargetPosition = new VInt3(this.m_curWaypointTarget.transform.position);
                }
                else
                {
                    this.m_isStartPoint = true;
                }
            }
            return this.m_curWaypointTargetPosition;
        }

        public Skill GetSkill(SkillSlotType slot)
        {
            return base.actor.SkillControl.FindSkill(slot);
        }

        public ResSkillCfgInfo GetSkillCfgData(SkillSlotType slot)
        {
            Skill skill = base.actor.SkillControl.FindSkill(slot);
            return ((skill == null) ? null : skill.cfgData);
        }

        public virtual string GetTypeName()
        {
            return "ObjWrapper";
        }

        public void HelpToAttack()
        {
            this.myTarget = this.m_needToHelpOtherToActtackTarget;
            this.m_needToHelpOtherToActtackTarget.Release();
            this.m_isNeedToHelpOther = false;
        }

        private void ImmediateUseSkill(IFrameCommand cmd, ref SkillUseParam param, SkillSlot skillSlot)
        {
            if (((!this.IsDeadState && base.actor.SkillControl.IsEnableSkillSlot(param.SlotType)) && (!base.actor.SkillControl.IsDisableSkillSlot(param.SlotType) && base.actor.SkillControl.IsSkillUseValid(param.SlotType, ref param))) && base.actor.SkillControl.UseSkill(ref param, true))
            {
            }
        }

        protected virtual void InitDefaultState()
        {
            this.SetObjBehaviMode(ObjBehaviMode.State_Idle);
            this.nextBehavior = ObjBehaviMode.State_Null;
        }

        public virtual bool IsBossOrHeroAutoAI()
        {
            return false;
        }

        public bool IsControlled()
        {
            return this.m_isControledByMan;
        }

        public bool IsCurWaypointValid()
        {
            return (this.m_curWaypointTarget != null);
        }

        private bool IsDuringExposing()
        {
            return (this.m_exposeTimerSeq >= 0);
        }

        public bool IsEnableReviveContext()
        {
            return this.m_reviveContext.bEnable;
        }

        public bool IsInMultiKill()
        {
            return ((this.lastKillLogicTime != 0) ? ((Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.lastKillLogicTime) < 0x2710L) : false);
        }

        public bool IsKilledByHero()
        {
            return (((this.lastHeroAtkLogicTime != 0) ? ((Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.lastHeroAtkLogicTime) < 0x2710L) : false) && ((bool) this.LastHeroAtker));
        }

        public bool IsPlayingAnimation()
        {
            if (base.actor.AnimControl != null)
            {
                return base.actor.ActorMeshAnimation.isPlaying;
            }
            Animation component = base.actor.ActorMesh.GetComponent<Animation>();
            return ((component != null) && component.isPlaying);
        }

        public bool IsTargetObjInSearchDistance()
        {
            Skill nextSkill = this.GetNextSkill(SkillSlotType.SLOT_SKILL_0);
            if ((nextSkill == null) || (this.myTarget == 0))
            {
                return false;
            }
            if (((this.myTarget == 0) || (this.myTarget.handle.shape == null)) || ((this.myTarget.handle.ActorAgent == null) || this.myTarget.handle.ActorControl.IsDeadState))
            {
                return false;
            }
            long iMaxSearchDistance = nextSkill.cfgData.iMaxSearchDistance;
            iMaxSearchDistance += this.myTarget.handle.shape.AvgCollisionRadius;
            iMaxSearchDistance *= iMaxSearchDistance;
            VInt3 num2 = base.actorLocation - this.myTarget.handle.location;
            if (num2.sqrMagnitudeLong2D > iMaxSearchDistance)
            {
                return false;
            }
            return true;
        }

        public bool IsUseAdvanceCommonAttack()
        {
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
            return ((ownerPlayer != null) ? ownerPlayer.IsUseAdvanceCommonAttack() : false);
        }

        public virtual void LogActorState()
        {
        }

        public void MoveToTarget()
        {
            if (this.myTarget != 0)
            {
                this.RealMovePosition(this.myTarget.handle.location, 0);
            }
        }

        public void NotifyAssistActor(ref PoolObjHandle<ActorRoot> assist)
        {
            if (assist != 0)
            {
                assist.handle.ActorControl.OnAssist(ref this.actorPtr);
                assist.handle.BuffHolderComp.OnAssistEffect(ref this.actorPtr);
            }
        }

        public void NotifySelfCampSelfBeAttacked(int srchR)
        {
            Singleton<TargetSearcher>.GetInstance().NotifySelfCampToAttack(base.actorPtr, srchR, this.myLastAtker);
        }

        public void NotifySelfCampSelfWillAttack(int srchR)
        {
            Singleton<TargetSearcher>.GetInstance().NotifySelfCampToAttack(base.actorPtr, srchR, this.myTarget);
        }

        protected virtual void OnAssist(ref PoolObjHandle<ActorRoot> deadActor)
        {
            if (this.eventActorAssist != null)
            {
                PoolObjHandle<ActorRoot> orignalActor = this.GetOrignalActor();
                DefaultGameEventParam prm = new DefaultGameEventParam(deadActor, base.actorPtr, ref orignalActor);
                this.eventActorAssist(ref prm);
            }
        }

        protected virtual void OnBehaviModeChange(ObjBehaviMode oldState, ObjBehaviMode curState)
        {
            if (base.actor != null)
            {
                if (curState == ObjBehaviMode.State_Dead)
                {
                    this.OnDead();
                }
                else if ((curState == ObjBehaviMode.State_Idle) && (oldState == ObjBehaviMode.State_Dead))
                {
                    this.OnRevive();
                }
            }
            this.updateAffectActors();
        }

        protected virtual void OnDead()
        {
            PoolObjHandle<ActorRoot> handle = (this.myLastAtker == 0) ? this.myLastAtker : this.myLastAtker.handle.ActorControl.GetOrignalActor();
            PoolObjHandle<ActorRoot> lastHeroAtker = handle;
            this.TerminateMove();
            this.ClearMoveCommand();
            this.EnableRVO(false);
            if (base.actor.HudControl != null)
            {
                base.actor.HudControl.OnActorDead();
            }
            if (base.actor.BuffHolderComp != null)
            {
                base.actor.BuffHolderComp.OnDead(handle);
            }
            if (this.m_reviveContext.bEnable)
            {
                this.m_reviveTick = (this.m_reviveContext.ReviveTime <= 0) ? this.CfgReviveCD : this.m_reviveContext.ReviveTime;
            }
            else
            {
                this.m_reviveTick = this.CfgReviveCD;
            }
            base.actor.SkillControl.OnDead();
            base.actor.SkillControl.ResetAllSkillSlot();
            if (this.actorPtr.handle.ActorControl.IsKilledByHero())
            {
                lastHeroAtker = this.actorPtr.handle.ActorControl.LastHeroAtker;
            }
            GameDeadEventParam prm = new GameDeadEventParam(base.actorPtr, this.myLastAtker, handle, lastHeroAtker, this.m_reviveContext.bEnable);
            if (this.eventActorDead != null)
            {
                this.eventActorDead(ref prm);
            }
            Singleton<GameEventSys>.instance.SendEvent<GameDeadEventParam>(GameEventDef.Event_ActorDead, ref prm);
            Singleton<GameEventSys>.instance.SendEvent<GameDeadEventParam>(GameEventDef.Event_PostActorDead, ref prm);
        }

        private void OnExposeOver(int inTimeSeq)
        {
            if (inTimeSeq == this.m_exposeTimerSeq)
            {
                Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
                base.actor.HorizonMarker.SetExposeMark(false, this.m_attackeeCamp, this.DoesIgnoreAlreadyLit());
                this.m_attackeeCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT;
                this.m_exposeTimerSeq = -1;
            }
        }

        protected virtual void OnHpChange()
        {
            if (base.actor.HudControl != null)
            {
                base.actor.HudControl.UpdateBloodBar(Math.Min(base.actor.ValueComponent.actorHp, base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue), base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue);
            }
            if ((base.actor.ValueComponent.actorHp <= 0) && !this.IsDeadState)
            {
                this.SetObjBehaviMode(ObjBehaviMode.State_Dead);
            }
        }

        protected virtual void OnMovement()
        {
        }

        private void OnMoveSpdChange()
        {
            if ((base.actor.ValueComponent != null) && (base.actor.ValueComponent.mActorValue != null))
            {
                int actorMoveSpeed = base.actor.ValueComponent.actorMoveSpeed;
                actorMoveSpeed = (actorMoveSpeed >= 0x4b0) ? actorMoveSpeed : 0x4b0;
                base.actor.ObjLinker.GroundSpeed = actorMoveSpeed;
                if (base.actor.MovementComponent != null)
                {
                    base.actor.MovementComponent.maxSpeed = actorMoveSpeed;
                    if ((base.actor.MovementComponent.Pathfinding != null) && (base.actor.MovementComponent.Pathfinding.rvo != null))
                    {
                        base.actor.MovementComponent.Pathfinding.rvo.maxSpeed = actorMoveSpeed;
                    }
                }
                if (base.actor.AnimControl != null)
                {
                    float speed = ((float) actorMoveSpeed) / 4000f;
                    base.actor.AnimControl.SetAnimPlaySpeed("Run", speed);
                }
                if (base.actor.ValueComponent.ObjValueStatistic != null)
                {
                    int iMoveSpeedMax = base.actor.ValueComponent.ObjValueStatistic.iMoveSpeedMax;
                    base.actor.ValueComponent.ObjValueStatistic.iMoveSpeedMax = (iMoveSpeedMax <= actorMoveSpeed) ? actorMoveSpeed : iMoveSpeedMax;
                }
            }
        }

        public virtual void OnMyTargetSwitch()
        {
        }

        protected virtual void OnRevive()
        {
            base.actor.InitVisible();
            base.actor.HorizonMarker.ResetSight();
            base.actor.ValueComponent.SetHpAndEpToInitialValue(this.m_reviveContext.ReviveLife, this.m_reviveContext.ReviveEnergy);
            if (this.m_reviveContext.bCDReset)
            {
                base.actor.SkillControl.ResetSkillCD();
            }
            if (this.m_reviveContext.AutoReset)
            {
                this.ResetReviveContext();
            }
            if (base.actor.HudControl != null)
            {
                base.actor.HudControl.OnActorRevive();
            }
            this.PlayAnimation("Idle", 0f, 0, true);
            DefaultGameEventParam prm = new DefaultGameEventParam(base.GetActor(), this.myLastAtker);
            Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, ref prm);
            if (this.eventActorRevive != null)
            {
                this.eventActorRevive(ref prm);
            }
            this.myLastAtker.Release();
            this.lastHeroAtker.Release();
        }

        public void OnShieldChange(int pType, int chgValue)
        {
            base.actor.HudControl.UpdateShieldValue((ProtectType) pType, chgValue);
        }

        protected virtual void OnSoulLvlChange()
        {
        }

        public override void OnUse()
        {
            base.OnUse();
            this.actorSubType = 0;
            this.actorSubSoliderType = 0;
            this.ClearVariables();
        }

        public void PlayAnimation(string animationName, float blendTime, int layer, bool loop)
        {
            if (base.actor.ActorMesh != null)
            {
                Animation component = null;
                AnimPlayComponent animControl = base.actor.AnimControl;
                if (animControl != null)
                {
                    PlayAnimParam param = new PlayAnimParam {
                        animName = animationName,
                        blendTime = blendTime,
                        loop = loop,
                        layer = layer,
                        speed = 1f
                    };
                    animControl.Play(param);
                }
                else
                {
                    component = base.actor.ActorMesh.GetComponent<Animation>();
                    if (component != null)
                    {
                        if (blendTime > 0f)
                        {
                            component.CrossFade(animationName, blendTime);
                        }
                        else
                        {
                            component.Stop();
                            component.Play(animationName);
                        }
                        AnimationState state = component[animationName];
                        if (state != null)
                        {
                            state.wrapMode = !loop ? WrapMode.Once : WrapMode.Loop;
                        }
                    }
                }
            }
        }

        public void PreMoveDirection(IFrameCommand cmd, short nDegree, int nSeq)
        {
        }

        protected virtual int QueryExposeDuration()
        {
            return Horizon.QueryExposeDurationNormal();
        }

        public override void Reactive()
        {
            base.Reactive();
            if (base.actor.ActorAgent != null)
            {
                base.actor.ActorAgent.Reset();
            }
            if (base.actor.ShadowEffect != null)
            {
                base.actor.ShadowEffect.ApplyShadowSettings();
            }
            this.EnableRVO(true);
        }

        public virtual void RealMoveDirection(VInt3 direction, uint id = 0)
        {
            if (base.actor.MovementComponent != null)
            {
                base.actor.MovementComponent.SetMoveParam(direction, true, true, id);
                if (this.NoAbility[1] == 0)
                {
                    if (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        this.EnableRVO(false);
                    }
                    base.actor.MovementComponent.ExcuteMove();
                }
            }
        }

        public virtual void RealMovePosition(VInt3 dest, uint id = 0)
        {
            if (base.actor.MovementComponent != null)
            {
                dest.y = 0;
                base.actor.MovementComponent.SetMoveParam(dest, false, true, id);
                if (this.NoAbility[1] == 0)
                {
                    base.actor.MovementComponent.ExcuteMove();
                }
            }
        }

        public virtual bool RealUseSkill(SkillSlotType InSlot)
        {
            if (!base.actor.SkillControl.AbortCurUseSkill((SkillAbortType) InSlot))
            {
                return false;
            }
            if ((this.IsDeadState || !base.actor.SkillControl.IsEnableSkillSlot(InSlot)) || (base.actor.SkillControl.IsDisableSkillSlot(InSlot) || !base.actor.SkillControl.IsSkillUseValid(InSlot, ref this.curSkillUseInfo)))
            {
                return false;
            }
            this.curSkillCommand = null;
            SkillUseParam curSkillUseInfo = this.curSkillUseInfo;
            if (!base.actor.SkillControl.UseSkill(ref curSkillUseInfo, false))
            {
                return false;
            }
            SkillSlot slot = null;
            if (!base.actor.SkillControl.TryGetSkillSlot(curSkillUseInfo.SlotType, out slot))
            {
                return false;
            }
            if (!base.actor.ValueComponent.IsEnergyType(ENERGY_TYPE.None))
            {
                slot.AutoSkillEnergyCost();
            }
            return true;
        }

        public void ResetReviveContext()
        {
            this.m_reviveContext.Reset();
        }

        public virtual void Revive(bool auto)
        {
            base.actor.ActorAgent.StopCurAgeAction();
            this.m_outOfControl.m_isOutOfControl = false;
            this.SetObjBehaviMode(ObjBehaviMode.State_Idle);
        }

        public void ReviveHp(int nAddHp)
        {
            if (!this.IsDeadState)
            {
                base.actor.ValueComponent.actorHp += nAddHp;
            }
        }

        public virtual void RmvDisableSkillFlag(SkillSlotType _type)
        {
            if ((base.actorPtr != 0) && (this.actorPtr.handle.SkillControl != null))
            {
                this.actorPtr.handle.SkillControl.SetDisableSkillSlot(_type, false);
            }
        }

        public virtual int RmvNoAbilityFlag(ObjAbilityType abt)
        {
            this.NoAbility[(int) abt]--;
            return this.NoAbility[(int) abt];
        }

        public virtual void SelectTarget(PoolObjHandle<ActorRoot> tar)
        {
            bool flag = false;
            if ((this.myTarget != 0) && (tar != 0))
            {
                if (this.myTarget.handle.ObjID != this.myTarget.handle.ObjID)
                {
                    flag = true;
                }
            }
            else if (this.myTarget == 0)
            {
                flag = true;
            }
            this.myTarget = tar;
        }

        public void SetAutoAI(bool autoAI)
        {
            if (this.m_isAutoAI != autoAI)
            {
                this.m_isAutoAI = autoAI;
                if (this.m_isAutoAI)
                {
                    base.actor.SkillControl.SkillUseCache.Clear();
                }
                if (!this.m_isAutoAI && !this.IsDeadState)
                {
                    this.SetObjBehaviMode(ObjBehaviMode.State_Idle);
                    this.TerminateMove();
                }
                DefaultGameEventParam prm = new DefaultGameEventParam(base.actorPtr, base.actorPtr);
                Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_AutoAISwitch, ref prm);
            }
            if (!autoAI)
            {
                this.m_offline = false;
                this.m_followOther = false;
                this.m_leaderID = 0;
            }
        }

        public void SetFollowOther(bool follow, uint leaderID)
        {
            this.m_followOther = follow;
            this.m_leaderID = leaderID;
            if (follow)
            {
                if (!this.m_isAutoAI)
                {
                    this.m_isAutoAI = true;
                    DefaultGameEventParam prm = new DefaultGameEventParam(base.actorPtr, base.actorPtr);
                    Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_AutoAISwitch, ref prm);
                }
            }
            else
            {
                leaderID = 0;
            }
        }

        public void SetHelpToAttackTarget(PoolObjHandle<ActorRoot> helpActor, PoolObjHandle<ActorRoot> enemyActor)
        {
            this.m_needToHelpTarget = helpActor;
            this.m_needToHelpOtherToActtackTarget = enemyActor;
            this.m_isNeedToHelpOther = true;
        }

        public void SetInAttack(PoolObjHandle<ActorRoot> inAttackee)
        {
            this._inAttackCoolTick = 0;
            base.actor.HorizonMarker.SetExposeMark(true, (inAttackee == 0) ? COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT : inAttackee.handle.TheActorMeta.ActorCamp, false);
        }

        public void SetInBattle()
        {
            SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
            Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
            if (((curLvelContext != null) && !curLvelContext.IsMobaMode()) && (ownerPlayer != null))
            {
                ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = ownerPlayer.GetAllHeroes();
                int count = allHeroes.Count;
                for (int i = 0; i < count; i++)
                {
                    if (allHeroes[i] != 0)
                    {
                        PoolObjHandle<ActorRoot> handle = allHeroes[i];
                        handle.handle.ActorControl.SetSelfInBattle();
                    }
                }
            }
            else
            {
                this.SetSelfInBattle();
            }
        }

        public void SetLockTargetID(uint _targetID)
        {
            if (base.actor.LockTargetAttackModeControl != null)
            {
                base.actor.LockTargetAttackModeControl.SetLockTargetID(_targetID);
            }
        }

        public void SetObjBehaviMode(ObjBehaviMode newMode)
        {
            if ((((this.myBehavior != ObjBehaviMode.State_GameOver) && ((this.myBehavior != ObjBehaviMode.State_Dead) || (newMode != ObjBehaviMode.State_GameOver))) && (((this.myBehavior != ObjBehaviMode.State_OutOfControl) || !this.m_outOfControl.m_isOutOfControl) || ((newMode == ObjBehaviMode.State_Dead) || (newMode == ObjBehaviMode.State_GameOver)))) && (this.myBehavior != newMode))
            {
                ObjBehaviMode myBehavior = this.myBehavior;
                this.myBehavior = newMode;
                this.OnBehaviModeChange(myBehavior, newMode);
                if ((base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (base.actor.SkillControl.SkillUseCache != null))
                {
                    base.actor.SkillControl.SkillUseCache.SetMoveToAttackTarget(false);
                }
            }
        }

        public void SetOffline(bool yesOrNot)
        {
            this.m_offline = yesOrNot;
            if (!this.m_offline)
            {
                this.m_followOther = false;
                this.m_leaderID = 0;
            }
            if (this.m_isAutoAI != yesOrNot)
            {
                this.m_isAutoAI = yesOrNot;
                if (!this.m_isAutoAI && !this.IsDeadState)
                {
                    this.SetObjBehaviMode(ObjBehaviMode.State_Idle);
                    this.TerminateMove();
                }
                DefaultGameEventParam prm = new DefaultGameEventParam(base.actorPtr, base.actorPtr);
                Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_AutoAISwitch, ref prm);
            }
        }

        public void SetOutOfControl(bool isOutOfControl, OutOfControlType type)
        {
            this.m_outOfControl.m_isOutOfControl = isOutOfControl;
            this.m_outOfControl.m_outOfControlType = type;
            if (isOutOfControl)
            {
                if (this.myBehavior != ObjBehaviMode.State_OutOfControl)
                {
                    this.m_beforeOutOfControlBehaviMode = this.myBehavior;
                }
                this.SetObjBehaviMode(ObjBehaviMode.State_OutOfControl);
            }
            else if ((this.myBehavior != ObjBehaviMode.State_Dead) && (this.myBehavior != ObjBehaviMode.State_GameOver))
            {
                this.SetObjBehaviMode(this.m_beforeOutOfControlBehaviMode);
            }
        }

        public void SetReviveContext(int reviveTime, int reviveLife, bool autoReset, bool bBaseRevive = true, bool bCDReset = false)
        {
            this.m_reviveContext.ReviveLife = reviveLife;
            this.m_reviveContext.ReviveTime = reviveTime;
            this.m_reviveContext.AutoReset = autoReset;
            this.m_reviveContext.bBaseRevive = bBaseRevive;
            this.m_reviveContext.bCDReset = bCDReset;
            this.m_reviveContext.bEnable = true;
            if ((this.m_reviveTick > 0) && (this.m_reviveContext.ReviveTime >= 0))
            {
                this.m_reviveTick = this.m_reviveContext.ReviveTime;
            }
        }

        public void SetSelected(bool selected)
        {
            if (this.m_isControledByMan != selected)
            {
                this.m_isControledByMan = selected;
                this.curMoveSeq = 0;
                base.actor.ObjLinker.nPreMoveSeq = -1;
                if (!this.m_isControledByMan && !this.IsDeadState)
                {
                    this.TerminateMove();
                    this.SetObjBehaviMode(ObjBehaviMode.State_Idle);
                }
            }
        }

        public void SetSelfExitBattle()
        {
            DefaultGameEventParam prm = new DefaultGameEventParam(base.GetActor(), base.GetActor());
            Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorExitCombat, ref prm);
            if (this.eventActorExitCombat != null)
            {
                this.eventActorExitCombat(ref prm);
            }
        }

        public void SetSelfInBattle()
        {
            bool isInBattle = this.IsInBattle;
            this._inBattleCoolTick = 0;
            if (!isInBattle)
            {
                DefaultGameEventParam prm = new DefaultGameEventParam(base.GetActor(), base.GetActor());
                Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorEnterCombat, ref prm);
                if (this.eventActorEnterCombat != null)
                {
                    this.eventActorEnterCombat(ref prm);
                }
            }
        }

        public bool SetSkill(SkillSlotType InSlot, bool bSpecial)
        {
            if (base.actor == null)
            {
                return false;
            }
            if (!bSpecial)
            {
                SkillSlot skillSlot = base.actor.SkillControl.GetSkillSlot(InSlot);
                if (((skillSlot == null) || (skillSlot.SkillObj == null)) || ((skillSlot.SkillObj.cfgData == null) || (this.myTarget == 0)))
                {
                    return false;
                }
                Skill skill = (skillSlot.NextSkillObj == null) ? skillSlot.SkillObj : skillSlot.NextSkillObj;
                if (skill.cfgData.dwRangeAppointType == 2)
                {
                    this.curSkillUseInfo = new SkillUseParam();
                    this.curSkillUseInfo.Init(InSlot, this.myTarget.handle.location);
                }
                else if (skill.cfgData.dwRangeAppointType == 1)
                {
                    this.curSkillUseInfo = new SkillUseParam();
                    this.curSkillUseInfo.Init(InSlot, this.myTarget.handle.ObjID);
                }
                else if (skill.cfgData.dwRangeAppointType == 3)
                {
                    this.curSkillUseInfo = new SkillUseParam();
                    this.curSkillUseInfo.Init(InSlot, this.myTarget.handle.location - base.actor.location, false);
                }
                else
                {
                    this.curSkillUseInfo = new SkillUseParam();
                    this.curSkillUseInfo.Init(InSlot, this.myTarget.handle.ObjID);
                }
            }
            else
            {
                this.curSkillUseInfo = new SkillUseParam();
                this.curSkillUseInfo.Init(InSlot, base.actor.forward, true);
            }
            return true;
        }

        public void SetTauntTarget(PoolObjHandle<ActorRoot> tar)
        {
            this.m_tauntMeActor = tar;
        }

        public void SetTerrorActor(PoolObjHandle<ActorRoot> tar)
        {
            this.m_terrorMeActor = tar;
        }

        public virtual int TakeDamage(ref HurtDataInfo hurt)
        {
            if (((hurt.atker != 0) && (hurt.target != 0)) && ((hurt.atker.handle.TheActorMeta.ActorCamp != hurt.target.handle.TheActorMeta.ActorCamp) && (hurt.hurtType != HurtTypeDef.Therapic)))
            {
                this.myLastAtker = hurt.atker;
                if (hurt.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                {
                    this.m_isAttackedByEnemyHero = true;
                    this.lastHeroAtker = hurt.atker;
                    this.lastHeroAtkLogicTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                }
            }
            if (hurt.hurtType != HurtTypeDef.Therapic)
            {
                base.actor.SkillControl.AbortCurUseSkill(SkillAbortType.TYPE_DAMAGE);
            }
            int num = base.actor.HurtControl.TakeDamage(ref hurt);
            if ((((hurt.atker != 0) && (hurt.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)) && (hurt.hurtType != HurtTypeDef.Therapic)) && (((base.actor.ValueComponent.actorHp * 100) < (base.actor.ValueComponent.actorHpTotal * 0x5f)) || ((this.myLastAtker != 0) && !this.myLastAtker.handle.AttackOrderReady)))
            {
                base.actor.ActorAgent.SetInDanger();
            }
            return num;
        }

        public virtual void TerminateMove()
        {
            if (base.actor.MovementComponent != null)
            {
                base.actor.MovementComponent.StopMove();
            }
        }

        public override void Uninit()
        {
            base.Uninit();
            this.ClearExposeTimer();
        }

        protected void updateAffectActors()
        {
            ulong logicFrameTick = Singleton<FrameSynchr>.instance.LogicFrameTick;
            int index = 0;
            while (index < this.hurtSelfActorList.Count)
            {
                KeyValuePair<uint, ulong> pair = this.hurtSelfActorList[index];
                if (logicFrameTick > (pair.Value + 0x2710L))
                {
                    this.hurtSelfActorList.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
            index = 0;
            while (index < this.helpSelfActorList.Count)
            {
                KeyValuePair<uint, ulong> pair2 = this.helpSelfActorList[index];
                if (logicFrameTick > (pair2.Value + 0x2710L))
                {
                    this.helpSelfActorList.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }

        public void UpdateAnimPlaySpeed()
        {
            if ((base.actor.AnimControl != null) && ((base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) || (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)))
            {
                int actorMoveSpeed = base.actor.ValueComponent.actorMoveSpeed;
                actorMoveSpeed = (actorMoveSpeed >= 0x4b0) ? actorMoveSpeed : 0x4b0;
                float speed = ((float) actorMoveSpeed) / 4000f;
                base.actor.AnimControl.SetAnimPlaySpeed("Run", speed);
            }
        }

        public void UpdateLastKillTime()
        {
            this.lastKillLogicTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
        }

        public override void UpdateLogic(int nDelta)
        {
            if ((this._inBattleCoolTick < this.m_battle_cool_ticks) && (++this._inBattleCoolTick == this.m_battle_cool_ticks))
            {
                this.SetSelfExitBattle();
            }
            if (this._inAttackCoolTick < this.m_battle_cool_ticks)
            {
                this._inAttackCoolTick++;
            }
            if (((this.myBehavior == ObjBehaviMode.Direction_Move) && (this.curMoveCommand != null)) && (++this._moveCmdTimeoutFrame > 60))
            {
                this.CmdStopMove();
                this.curMoveCommand = null;
            }
            if (this.IsDeadState)
            {
                this.m_reviveTick -= nDelta;
                if ((this.ReviveCooldown <= 0) && !this.bForceNotRevive)
                {
                    this.Revive(true);
                }
            }
            this.updateAffectActors();
        }

        public void UseGoHomeSkill()
        {
            if (!this.IsDeadState && (!base.actor.SkillControl.IsDisableSkillSlot(SkillSlotType.SLOT_SKILL_6) && base.actor.SkillControl.CanUseSkill(SkillSlotType.SLOT_SKILL_6)))
            {
                Skill skill = this.GetSkill(SkillSlotType.SLOT_SKILL_6);
                if (((skill != null) && (skill.cfgData != null)) && (skill.cfgData.bSkillType == 1))
                {
                    Skill curUseSkill = base.actor.SkillControl.CurUseSkill;
                    if (((curUseSkill == null) || (curUseSkill.SlotType != SkillSlotType.SLOT_SKILL_6)) && base.actor.SkillControl.AbortCurUseSkill(SkillAbortType.TYPE_DAMAGE))
                    {
                        SkillUseParam param = new SkillUseParam();
                        param.Init(SkillSlotType.SLOT_SKILL_6, base.actor.ObjID);
                        base.actor.SkillControl.UseSkill(ref param, false);
                    }
                }
            }
        }

        public void UseHpRecoverSkillToSelf()
        {
            if (!this.IsDeadState)
            {
                if (!base.actor.SkillControl.IsDisableSkillSlot(SkillSlotType.SLOT_SKILL_4) && base.actor.SkillControl.CanUseSkill(SkillSlotType.SLOT_SKILL_4))
                {
                    Skill skill = this.GetSkill(SkillSlotType.SLOT_SKILL_4);
                    if (((skill != null) && (skill.cfgData != null)) && (skill.cfgData.bSkillType == 3))
                    {
                        if (base.actor.SkillControl.AbortCurUseSkill(SkillAbortType.TYPE_SKILL_4))
                        {
                            SkillUseParam param = new SkillUseParam();
                            param.Init(SkillSlotType.SLOT_SKILL_4, base.actor.ObjID);
                            base.actor.SkillControl.UseSkill(ref param, false);
                        }
                        return;
                    }
                }
                if (!base.actor.SkillControl.IsDisableSkillSlot(SkillSlotType.SLOT_SKILL_6) && base.actor.SkillControl.CanUseSkill(SkillSlotType.SLOT_SKILL_6))
                {
                    Skill skill2 = this.GetSkill(SkillSlotType.SLOT_SKILL_6);
                    if ((((skill2 != null) && (skill2.cfgData != null)) && (skill2.cfgData.bSkillType == 3)) && base.actor.SkillControl.AbortCurUseSkill(SkillAbortType.TYPE_DAMAGE))
                    {
                        SkillUseParam param2 = new SkillUseParam();
                        param2.Init(SkillSlotType.SLOT_SKILL_6, base.actor.ObjID);
                        base.actor.SkillControl.UseSkill(ref param2, false);
                    }
                }
            }
        }

        public int AttackRange
        {
            get
            {
                Skill nextSkill = this.GetNextSkill(SkillSlotType.SLOT_SKILL_0);
                if (nextSkill != null)
                {
                    return (int) nextSkill.cfgData.iMaxAttackDistance;
                }
                return 100;
            }
        }

        public bool CanMove
        {
            get
            {
                return (base.actor.isMovable && (this.NoAbility[1] == 0));
            }
        }

        public bool CanRevive
        {
            get
            {
                return (this.IsDeadState && (this.ReviveCooldown <= 0));
            }
        }

        public bool CanRotate
        {
            get
            {
                return (base.actor.isRotatable && (this.NoAbility[5] == 0));
            }
        }

        public virtual int CfgReviveCD
        {
            get
            {
                return 0x3a98;
            }
        }

        public IFrameCommand curMoveCommand
        {
            get
            {
                return this._curMoveCommand;
            }
            private set
            {
                this._curMoveCommand = value;
                this._moveCmdTimeoutFrame = 0;
            }
        }

        public int GreaterRange
        {
            get
            {
                Skill nextSkill = this.GetNextSkill(SkillSlotType.SLOT_SKILL_0);
                if (nextSkill != null)
                {
                    return nextSkill.cfgData.iGreaterAttackDistance;
                }
                return 100;
            }
        }

        public bool IsBornState
        {
            get
            {
                return (this.myBehavior == ObjBehaviMode.State_Born);
            }
        }

        public bool IsDeadState
        {
            get
            {
                return (this.myBehavior == ObjBehaviMode.State_Dead);
            }
        }

        public bool IsInAttack
        {
            get
            {
                return (this._inAttackCoolTick < this.m_battle_cool_ticks);
            }
        }

        public bool IsInBattle
        {
            get
            {
                return (this._inBattleCoolTick < this.m_battle_cool_ticks);
            }
        }

        public bool IsMoveState
        {
            get
            {
                return (this.myBehavior == ObjBehaviMode.Direction_Move);
            }
        }

        public PoolObjHandle<ActorRoot> LastHeroAtker
        {
            get
            {
                return this.lastHeroAtker;
            }
        }

        public PoolObjHandle<ActorRoot> myTarget
        {
            get
            {
                return this._myTarget;
            }
            set
            {
                if (value != this._myTarget)
                {
                    this.OnMyTargetSwitch();
                    this._myTarget = value;
                }
            }
        }

        public int ReviveCooldown
        {
            get
            {
                return this.m_reviveTick;
            }
        }

        public int SearchRange
        {
            get
            {
                Skill nextSkill = this.GetNextSkill(SkillSlotType.SLOT_SKILL_0);
                if (nextSkill != null)
                {
                    return nextSkill.cfgData.iMaxSearchDistance;
                }
                return 100;
            }
        }
    }
}

