namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameSystem;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class SkillComponent : LogicComponent
    {
        public bool bImmediateAttack;
        private bool bIsCurAtkUseSkill;
        public CommonAttackType commonAttackType;
        public Skill CurUseSkill;
        public SkillSlot CurUseSkillSlot;
        public int[] DisableSkill = new int[8];
        public int m_iSkillPoint;
        public VInt3 RecordPosition = VInt3.zero;
        public SkillSlot[] SkillSlotArray = new SkillSlot[8];
        public SkillCache SkillUseCache;
        public ListView<BulletSkill> SpawnedBullets = new ListView<BulletSkill>();
        public CSkillStat stSkillStat;

        public bool AbortCurUseSkill(SkillAbortType _type)
        {
            if (this.CurUseSkillSlot != null)
            {
                return this.CurUseSkillSlot.Abort(_type);
            }
            return true;
        }

        public bool CanUseSkill(SkillSlotType slotType)
        {
            SkillSlot slot;
            if ((this.CurUseSkill != null) && !this.CurUseSkill.canAbort((SkillAbortType) slotType))
            {
                return false;
            }
            Skill skill = this.FindSkill(slotType);
            if ((skill == null) || (skill.cfgData == null))
            {
                return false;
            }
            return (this.TryGetSkillSlot(slotType, out slot) && slot.IsEnableSkillSlot());
        }

        public void ChangePassiveParam(int _id, int _index, int _value)
        {
            if (this.talentSystem != null)
            {
                this.talentSystem.ChangePassiveParam(_id, _index, _value);
            }
        }

        private void ClearSkillSlot()
        {
            for (int i = 0; i < 8; i++)
            {
                this.DisableSkill[i] = 0;
                this.SkillSlotArray[i] = null;
            }
        }

        public void CreateTalent(int _talentID)
        {
            this.talentSystem.InitTalent(_talentID);
        }

        public override void Deactive()
        {
            for (int i = 0; i < this.SpawnedBullets.Count; i++)
            {
                if (this.SpawnedBullets[i].isFinish)
                {
                    this.SpawnedBullets[i].Release();
                }
                else
                {
                    this.SpawnedBullets[i].bManaged = false;
                }
            }
            this.SpawnedBullets.Clear();
            base.Deactive();
        }

        public void DelayAbortCurUseSkill()
        {
            if ((this.CurUseSkillSlot != null) && (this.CurUseSkill != null))
            {
                if (!this.CurUseSkill.bProtectAbortSkill)
                {
                    this.CurUseSkillSlot.ForceAbort();
                }
                else
                {
                    this.CurUseSkill.bDelayAbortSkill = true;
                }
            }
        }

        public override void FightOver()
        {
            base.FightOver();
            for (int i = 0; i < 8; i++)
            {
                SkillSlot slot = this.SkillSlotArray[i];
                if (slot != null)
                {
                    slot.CancelUseSkill();
                }
            }
        }

        public Skill FindSkill(SkillSlotType slot)
        {
            SkillSlot slot2;
            if (this.TryGetSkillSlot(slot, out slot2))
            {
                return slot2.SkillObj;
            }
            return null;
        }

        public void ForceAbortCurUseSkill()
        {
            if (this.CurUseSkillSlot != null)
            {
                this.CurUseSkillSlot.ForceAbort();
            }
        }

        public uint GetAdvanceCommonAttackTarget()
        {
            return Singleton<CommonAttackSearcher>.GetInstance().AdvanceCommonAttackSearchEnemy(base.actorPtr, base.actor.ActorControl.SearchRange);
        }

        public int GetAllSkillLevel()
        {
            int num = 0;
            for (int i = 1; i <= 3; i++)
            {
                SkillSlot slot = this.SkillSlotArray[i];
                if (slot != null)
                {
                    num += slot.GetSkillLevel();
                }
            }
            return num;
        }

        public CommonAttackType GetCommonAttackType()
        {
            return this.commonAttackType;
        }

        public SkillSlot GetSkillSlot(SkillSlotType slot)
        {
            SkillSlot slot2 = null;
            this.TryGetSkillSlot(slot, out slot2);
            return slot2;
        }

        public bool HasPunishSkill()
        {
            SkillSlot slot = this.SkillSlotArray[5];
            return (((slot != null) && (slot.SkillObj != null)) && (slot.SkillObj.cfgData.bSkillType == 2));
        }

        public void HostCancelUseSkillSlot(SkillSlotType slot, enSkillJoystickMode mode)
        {
            SkillSlot slot2;
            if (this.TryGetSkillSlot(slot, out slot2))
            {
                slot2.CancelUseSkill();
                if (mode == enSkillJoystickMode.SelectTarget)
                {
                    MonoSingleton<CameraSystem>.GetInstance().SetFocusActor(base.actorPtr);
                }
                DefaultSkillEventParam param = new DefaultSkillEventParam(slot, 0, base.actorPtr);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<DefaultSkillEventParam>(GameSkillEventDef.Event_UseCanceled, base.actorPtr, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
            }
        }

        public override void Init()
        {
            base.Init();
            this.talentSystem = new TalentSystem();
            this.talentSystem.Init(base.actorPtr);
            this.stSkillStat = new CSkillStat();
            if (this.stSkillStat != null)
            {
                this.stSkillStat.Initialize(base.actorPtr);
                this.InitRandomSkill();
                this.InitPassiveSkill();
                IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
                IGameActorDataProvider provider2 = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
                ActorStaticSkillData skillData = new ActorStaticSkillData();
                for (int i = 0; i < 8; i++)
                {
                    if (i == 6)
                    {
                        SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                        if (curLvelContext.IsGameTypeGuide())
                        {
                            if (curLvelContext.m_mapID == CBattleGuideManager.GuideLevelID5v5)
                            {
                                goto Label_00D8;
                            }
                            continue;
                        }
                        if (!curLvelContext.IsMobaModeWithOutGuide() || (curLvelContext.m_pvpPlayerNum != 10))
                        {
                            continue;
                        }
                    }
                Label_00D8:
                    if (i == 7)
                    {
                        SLevelContext context2 = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                        if (!context2.m_bEnableOrnamentSlot || (context2.m_ornamentSkillId <= 0))
                        {
                            continue;
                        }
                    }
                    if ((((i == 4) || (i == 6)) || (i == 7)) && (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
                    {
                        SLevelContext context3 = Singleton<BattleLogic>.instance.GetCurLvelContext();
                        if ((context3 == null) || (context3.m_mapID <= 0))
                        {
                            continue;
                        }
                        int extraSkillId = context3.m_extraSkillId;
                        switch (i)
                        {
                            case 6:
                            {
                                extraSkillId = context3.m_extraSkill2Id;
                                if (extraSkillId > 0)
                                {
                                    break;
                                }
                                continue;
                            }
                            case 7:
                                extraSkillId = context3.m_ornamentSkillId;
                                break;

                            default:
                                this.CreateTalent(context3.m_extraPassiveSkillId);
                                break;
                        }
                        this.InitSkillSlot(i, extraSkillId, 0);
                        SkillSlot slot = this.SkillSlotArray[i];
                        if (slot != null)
                        {
                            slot.SetSkillLevel(1);
                        }
                        continue;
                    }
                    if (actorDataProvider.GetActorStaticSkillData(ref base.actor.TheActorMeta, (ActorSkillSlot) i, ref skillData))
                    {
                        this.InitSkillSlot(i, skillData.SkillId, skillData.PassiveSkillId);
                        if (((i > 3) || (i < 1)) || (!Singleton<BattleLogic>.GetInstance().m_GameInfo.gameContext.levelContext.IsSoulGrow() || (base.actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)))
                        {
                            SkillSlot slot2 = this.SkillSlotArray[i];
                            if (slot2 != null)
                            {
                                slot2.SetSkillLevel(1);
                            }
                        }
                    }
                }
                uint skillID = 0;
                if (provider2.GetActorServerCommonSkillData(ref base.actor.TheActorMeta, out skillID))
                {
                    int num5 = 5;
                    if (skillID != 0)
                    {
                        this.InitSkillSlot(num5, (int) skillID, 0);
                        SkillSlot slot3 = this.SkillSlotArray[num5];
                        if (slot3 != null)
                        {
                            slot3.SetSkillLevel(1);
                        }
                    }
                }
                if (base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                {
                    this.SkillUseCache = new SkillCache();
                }
            }
        }

        public void InitPassiveSkill()
        {
            int num = this.actorPtr.handle.TheStaticData.TheBaseAttribute.PassiveSkillID1;
            int num2 = this.actorPtr.handle.TheStaticData.TheBaseAttribute.PassiveSkillID2;
            if (num != 0)
            {
                this.CreateTalent(num);
            }
            if (num2 != 0)
            {
                this.CreateTalent(num2);
            }
        }

        private void InitRandomSkill()
        {
            if (base.actor != null)
            {
                this.InitRandomSkill(base.actor.TheStaticData.TheBaseAttribute.RandomPassiveSkillRule);
            }
        }

        public void InitRandomSkill(int inPassSkillRule)
        {
            int num = 0;
            int num2 = inPassSkillRule;
            if (num2 != 0)
            {
                DebugHelper.Assert(!Singleton<FrameSynchr>.instance.bActive || Singleton<FrameSynchr>.instance.isRunning);
                if (Singleton<FrameSynchr>.instance.isRunning)
                {
                    ResRandomSkillPassiveRule dataByKey = GameDataMgr.randomSkillPassiveDatabin.GetDataByKey((long) num2);
                    if (dataByKey != null)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            if (dataByKey.astRandomSkillPassiveID1[i].iParam == 0)
                            {
                                break;
                            }
                            num++;
                        }
                        if (num > 0)
                        {
                            ushort index = FrameRandom.Random((uint) num);
                            int iParam = dataByKey.astRandomSkillPassiveID1[index].iParam;
                            this.CreateTalent(iParam);
                            int num6 = dataByKey.astRandomSkillPassiveID2[index].iParam;
                            if (num6 != 0)
                            {
                                this.CreateTalent(num6);
                            }
                        }
                    }
                }
            }
        }

        public void InitSkillSlot(int _slotType, int _skillID, int _passiveID)
        {
            if (base.actorPtr == 0)
            {
                DebugHelper.Assert(base.actorPtr == 1);
            }
            else
            {
                Skill skill = new Skill(_skillID);
                PassiveSkill passive = null;
                if (_passiveID != 0)
                {
                    passive = new PassiveSkill(_passiveID, base.actorPtr);
                }
                SkillSlot slot = new SkillSlot((SkillSlotType) _slotType);
                slot.Init(ref this.actorPtr, skill, passive);
                slot.InitSkillControlIndicator();
                this.SkillSlotArray[_slotType] = slot;
            }
        }

        private bool InternalUseSkill(ref SkillUseParam param, bool bImmediate = false)
        {
            SkillSlot slot;
            if (!this.TryGetSkillSlot(param.SlotType, out slot))
            {
                return false;
            }
            slot.ReadySkillObj();
            Skill skillObj = slot.SkillObj;
            if (!bImmediate)
            {
                this.CurUseSkill = skillObj;
                this.CurUseSkillSlot = slot;
            }
            if (!skillObj.Use(base.actorPtr, ref param))
            {
                return false;
            }
            slot.AddSkillUseCount();
            this.SkillInfoStatistic(ref slot);
            this.bIsLastAtkUseSkill = this.bIsCurAtkUseSkill;
            if (param.SlotType == SkillSlotType.SLOT_SKILL_0)
            {
                this.bIsCurAtkUseSkill = false;
            }
            else
            {
                this.bIsCurAtkUseSkill = true;
            }
            ActorSkillEventParam param2 = new ActorSkillEventParam(base.GetActor(), param.SlotType);
            Singleton<GameSkillEventSys>.GetInstance().SendEvent<ActorSkillEventParam>(GameSkillEventDef.AllEvent_UseSkill, base.GetActor(), ref param2, GameSkillEventChannel.Channel_AllActor);
            return true;
        }

        public bool IsDisableSkillSlot(SkillSlotType _type)
        {
            int index = (int) _type;
            if (this.IsIngnoreDisableSkill(_type))
            {
                return false;
            }
            if ((_type < SkillSlotType.SLOT_SKILL_0) || (_type >= SkillSlotType.SLOT_SKILL_COUNT))
            {
                return false;
            }
            return (this.DisableSkill[index] > 0);
        }

        public bool IsEnableSkillSlot(SkillSlotType slot)
        {
            SkillSlot slot2;
            return (this.TryGetSkillSlot(slot, out slot2) && slot2.IsEnableSkillSlot());
        }

        private bool IsIngnoreDisableSkill(SkillSlotType _type)
        {
            SkillSlot slot = null;
            if ((_type < SkillSlotType.SLOT_SKILL_0) || (_type >= SkillSlotType.SLOT_SKILL_COUNT))
            {
                return false;
            }
            return ((this.TryGetSkillSlot(_type, out slot) && (slot.SkillObj != null)) && ((slot.SkillObj.cfgData != null) && (slot.SkillObj.cfgData.bBIngnoreDisable == 1)));
        }

        public bool IsSkillCDReady(SkillSlotType slot)
        {
            SkillSlot slot2;
            return (this.TryGetSkillSlot(slot, out slot2) && slot2.IsCDReady);
        }

        public bool IsSkillUseValid(SkillSlotType _type, ref SkillUseParam _param)
        {
            SkillSlot slot;
            return (this.TryGetSkillSlot(_type, out slot) && slot.IsSkillUseValid(ref _param));
        }

        public bool IsUseSkillJoystick(SkillSlotType slot)
        {
            SkillSlot slot2;
            if (Singleton<GameInput>.GetInstance().IsSmartUse())
            {
                return false;
            }
            return (this.TryGetSkillSlot(slot, out slot2) && slot2.IsUseSkillJoystick());
        }

        public override void LateUpdate(int nDelta)
        {
            SkillSlot slot = null;
            for (int i = 0; i < 8; i++)
            {
                slot = this.SkillSlotArray[i];
                if (slot != null)
                {
                    slot.LateUpdate(nDelta);
                }
            }
        }

        public void OnDead()
        {
            SkillSlot slot = null;
            for (int i = 0; i < 8; i++)
            {
                slot = this.SkillSlotArray[i];
                if (((slot != null) && (slot.skillChangeEvent != null)) && slot.skillChangeEvent.IsActive())
                {
                    slot.skillChangeEvent.Abort();
                }
            }
            if (this.CurUseSkill != null)
            {
                this.CurUseSkill.Stop();
            }
            for (int j = 0; j < this.SpawnedBullets.Count; j++)
            {
                BulletSkill skill = this.SpawnedBullets[j];
                if (skill.IsDeadRemove)
                {
                    skill.Stop();
                    skill.Release();
                    this.SpawnedBullets.RemoveAt(j);
                    j--;
                }
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.CurUseSkill = null;
            this.CurUseSkillSlot = null;
            this.SkillUseCache = null;
            this.talentSystem = null;
            this.bIsLastAtkUseSkill = false;
            this.bIsCurAtkUseSkill = false;
            this.RecordPosition = VInt3.zero;
            this.SpawnedBullets.Clear();
            this.m_iSkillPoint = 0;
            this.bZeroCd = false;
            this.ClearSkillSlot();
            this.commonAttackType = CommonAttackType.CommonAttackType1;
            this.stSkillStat = null;
            this.bImmediateAttack = false;
        }

        public override void Reactive()
        {
            base.Reactive();
            this.CurUseSkill = null;
            this.CurUseSkillSlot = null;
            this.RecordPosition = VInt3.zero;
            this.SpawnedBullets.Clear();
            this.m_iSkillPoint = 0;
            this.bZeroCd = false;
            this.ResetAllSkillSlot();
            this.ResetSkillCD();
            for (int i = 0; i < 8; i++)
            {
                this.DisableSkill[i] = 0;
            }
            if (this.SkillSlotArray != null)
            {
                for (int j = 0; j < this.SkillSlotArray.Length; j++)
                {
                    SkillSlot slot = this.SkillSlotArray[j];
                    if (slot != null)
                    {
                        slot.Reset();
                    }
                }
            }
            if (this.talentSystem != null)
            {
                this.talentSystem.Reset();
            }
        }

        public void ReadyUseSkillSlot(SkillSlotType slot)
        {
            SkillSlot slot2;
            if (this.TryGetSkillSlot(slot, out slot2))
            {
                slot2.ReadyUseSkill();
            }
        }

        public bool RemoveBuff(PoolObjHandle<ActorRoot> inTargetActor, int inSkillCombineId)
        {
            if (inTargetActor != 0)
            {
                inTargetActor.handle.BuffHolderComp.RemoveBuff(inSkillCombineId);
                return true;
            }
            return false;
        }

        public void RequestUseSkillSlot(SkillSlotType slot, enSkillJoystickMode mode, uint objID)
        {
            SkillSlot slot2;
            if (this.TryGetSkillSlot(slot, out slot2))
            {
                slot2.RequestUseSkill(mode, objID);
            }
        }

        public void ResetAllSkillSlot()
        {
            for (int i = 0; i < 8; i++)
            {
                if (this.SkillSlotArray[i] != null)
                {
                    this.SkillSlotArray[i].ResetSkillObj();
                    this.SkillSlotArray[i].skillIndicator.UnInitIndicatePrefab(false);
                }
            }
        }

        public void ResetSkillCD()
        {
            for (int i = 0; i < 8; i++)
            {
                if (this.SkillSlotArray[i] != null)
                {
                    this.SkillSlotArray[i].ResetSkillCD();
                }
            }
        }

        public void ResetSkillLevel()
        {
            PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
            int length = this.SkillSlotArray.Length;
            for (int i = 0; i < length; i++)
            {
                SkillSlot slot = this.SkillSlotArray[i];
                if ((slot != null) && (((slot.SlotType == SkillSlotType.SLOT_SKILL_1) || (slot.SlotType == SkillSlotType.SLOT_SKILL_2)) || (slot.SlotType == SkillSlotType.SLOT_SKILL_3)))
                {
                    slot.SetSkillLevel(0);
                    if ((captain == base.actorPtr) && (Singleton<CBattleSystem>.GetInstance().FightForm != null))
                    {
                        Singleton<CBattleSystem>.instance.FightForm.ClearSkillLvlStates(i);
                    }
                }
            }
            if ((captain == base.actorPtr) && (Singleton<CBattleSystem>.GetInstance().FightForm != null))
            {
                Singleton<CBattleSystem>.instance.FightForm.ResetSkillButtonManager(base.actorPtr);
            }
            this.m_iSkillPoint = 0;
        }

        public void SelectSkillTarget(SkillSlotType slot, Vector2 axis, bool isSkillCursorInCancelArea)
        {
            SkillSlot slot2;
            if (this.TryGetSkillSlot(slot, out slot2))
            {
                slot2.skillIndicator.SelectSkillTarget(axis, isSkillCursorInCancelArea);
            }
        }

        public void SetCommonAttackIndicator(bool bShow)
        {
            SkillSlot slot;
            if (this.TryGetSkillSlot(SkillSlotType.SLOT_SKILL_0, out slot))
            {
                if (base.actor.ActorControl.IsUseAdvanceCommonAttack())
                {
                    slot.skillIndicator.SetFixedPrefabShow(bShow);
                }
                else
                {
                    slot.skillIndicator.SetGuildPrefabShow(bShow);
                    slot.skillIndicator.SetEffectPrefabShow(false);
                }
            }
        }

        public void SetCommonAttackType(CommonAttackType _type)
        {
            this.commonAttackType = _type;
        }

        public void SetDisableSkillSlot(SkillSlotType _type, bool bAdd)
        {
            int index = (int) _type;
            if ((_type >= SkillSlotType.SLOT_SKILL_0) && (_type <= SkillSlotType.SLOT_SKILL_COUNT))
            {
                if (_type == SkillSlotType.SLOT_SKILL_COUNT)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (!this.IsIngnoreDisableSkill((SkillSlotType) i))
                        {
                            if (bAdd)
                            {
                                this.DisableSkill[i]++;
                            }
                            else
                            {
                                this.DisableSkill[i]--;
                            }
                        }
                    }
                }
                else if (!this.IsIngnoreDisableSkill(_type))
                {
                    if (bAdd)
                    {
                        this.DisableSkill[index]++;
                    }
                    else
                    {
                        this.DisableSkill[index]--;
                    }
                }
            }
        }

        private void SkillInfoStatistic(ref SkillSlot stSkillSlot)
        {
            if ((stSkillSlot != null) && ((this.stSkillStat.SkillStatistictInfo[(int) stSkillSlot.SlotType].iSkillCfgID == 0) || (this.stSkillStat.SkillStatistictInfo[(int) stSkillSlot.SlotType].iSkillCfgID == stSkillSlot.SkillObj.cfgData.iCfgID)))
            {
                this.stSkillStat.SkillStatistictInfo[(int) stSkillSlot.SlotType].iSkillCfgID = stSkillSlot.SkillObj.cfgData.iCfgID;
                SKILLSTATISTICTINFO skillstatistictinfo1 = this.stSkillStat.SkillStatistictInfo[(int) stSkillSlot.SlotType];
                skillstatistictinfo1.uiUsedTimes++;
                this.stSkillStat.SkillStatistictInfo[(int) stSkillSlot.SlotType].iAttackDistanceMax = Math.Max(this.stSkillStat.SkillStatistictInfo[(int) stSkillSlot.SlotType].iAttackDistanceMax, (int) stSkillSlot.SkillObj.cfgData.iMaxAttackDistance);
                long num = (long) (Time.realtimeSinceStartup * 1000f);
                uint num2 = (uint) (num - stSkillSlot.lLastUseTime);
                if (stSkillSlot.lLastUseTime != 0)
                {
                    uint num3 = Math.Min(this.stSkillStat.SkillStatistictInfo[(int) stSkillSlot.SlotType].uiCDIntervalMin, num2);
                    this.stSkillStat.SkillStatistictInfo[(int) stSkillSlot.SlotType].uiCDIntervalMin = num3;
                    uint num4 = Math.Max(this.stSkillStat.SkillStatistictInfo[(int) stSkillSlot.SlotType].uiCDIntervalMax, num2);
                    this.stSkillStat.SkillStatistictInfo[(int) stSkillSlot.SlotType].uiCDIntervalMax = num4;
                }
                stSkillSlot.lLastUseTime = num;
            }
        }

        public bool SpawnBuff(PoolObjHandle<ActorRoot> inTargetActor, ref SkillUseParam inParam, int inSkillCombineId, bool bExtraBuff = false)
        {
            if ((inTargetActor == 0) || (inSkillCombineId <= 0))
            {
                return false;
            }
            BuffSkill skill = ClassObjPool<BuffSkill>.Get();
            skill.Init(inSkillCombineId);
            skill.bExtraBuff = bExtraBuff;
            inParam.TargetActor = inTargetActor;
            inParam.Instigator = base.actor;
            bool flag = skill.Use(base.actorPtr, ref inParam);
            if (!flag)
            {
                skill.Release();
            }
            return flag;
        }

        public bool SpawnBuff(PoolObjHandle<ActorRoot> inTargetActor, SkillUseContext inContext, int inSkillCombineId, bool bExtraBuff = false)
        {
            if (((inTargetActor == 0) || (inContext == null)) || (inSkillCombineId <= 0))
            {
                return false;
            }
            BuffSkill skill = ClassObjPool<BuffSkill>.Get();
            skill.Init(inSkillCombineId);
            skill.bExtraBuff = bExtraBuff;
            skill.skillContext.Copy(inContext);
            skill.skillContext.TargetActor = inTargetActor;
            skill.skillContext.Instigator = base.actor;
            bool flag = skill.Use(base.actorPtr);
            if (!flag)
            {
                skill.Release();
            }
            return flag;
        }

        public PoolObjHandle<BulletSkill> SpawnBullet(SkillUseContext context, string _actionName, bool _bDeadRemove, bool _bAgeImmeExcute = false)
        {
            PoolObjHandle<BulletSkill> handle = new PoolObjHandle<BulletSkill>();
            if (context != null)
            {
                BulletSkill item = ClassObjPool<BulletSkill>.Get();
                item.Init(_actionName, _bDeadRemove);
                item.bAgeImmeExcute = _bAgeImmeExcute;
                if (item.Use(base.actorPtr, context))
                {
                    this.SpawnedBullets.Add(item);
                    return new PoolObjHandle<BulletSkill>(item);
                }
                item.Release();
            }
            return handle;
        }

        public void ToggleZeroCd()
        {
            this.ResetSkillCD();
            this.bZeroCd = !this.bZeroCd;
            Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", base.actorPtr, base.actor.ValueComponent.actorEp, base.actor.ValueComponent.actorEpTotal);
        }

        public bool TryGetSkillSlot(SkillSlotType _type, out SkillSlot _slot)
        {
            int index = (int) _type;
            if (((this.SkillSlotArray == null) || (index < 0)) || (index >= 8))
            {
                _slot = null;
                return false;
            }
            _slot = this.SkillSlotArray[index];
            if (_slot == null)
            {
                return false;
            }
            return true;
        }

        public override void Uninit()
        {
            base.Uninit();
            for (int i = 0; i < this.SpawnedBullets.Count; i++)
            {
                if (this.SpawnedBullets[i].isFinish)
                {
                    this.SpawnedBullets[i].Release();
                }
                else
                {
                    this.SpawnedBullets[i].bManaged = false;
                }
            }
            this.SpawnedBullets.Clear();
            for (int j = 0; j < 8; j++)
            {
                SkillSlot slot = this.SkillSlotArray[j];
                if (((slot != null) && (slot.PassiveSkillObj != null)) && (slot.PassiveSkillObj.passiveEvent != null))
                {
                    slot.PassiveSkillObj.passiveEvent.UnInit();
                }
            }
            if (this.talentSystem != null)
            {
                this.talentSystem.UnInit();
            }
            if (this.stSkillStat != null)
            {
                this.stSkillStat.UnInit();
            }
        }

        public override void UpdateLogic(int nDelta)
        {
            SkillSlot slot = null;
            if ((this.CurUseSkill != null) && this.CurUseSkill.isFinish)
            {
                this.CurUseSkill = null;
                this.CurUseSkillSlot = null;
            }
            for (int i = 0; i < this.SpawnedBullets.Count; i++)
            {
                BulletSkill skill = this.SpawnedBullets[i];
                if (skill != null)
                {
                    skill.UpdateLogic(nDelta);
                    if (skill.isFinish)
                    {
                        skill.Release();
                        this.SpawnedBullets.RemoveAt(i);
                        i--;
                    }
                }
            }
            for (int j = 0; j < 8; j++)
            {
                slot = this.SkillSlotArray[j];
                if (slot != null)
                {
                    slot.UpdateLogic(nDelta);
                }
            }
            if (this.talentSystem != null)
            {
                this.talentSystem.UpdateLogic(nDelta);
            }
        }

        public bool UseSkill(ref SkillUseParam param, bool bImmediate = false)
        {
            return this.InternalUseSkill(ref param, bImmediate);
        }

        public bool bIsLastAtkUseSkill { get; private set; }

        public bool bZeroCd { get; private set; }

        public bool isUsing
        {
            get
            {
                return (this.CurUseSkill != null);
            }
        }

        public TalentSystem talentSystem { get; private set; }
    }
}

