namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using ResData;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class BuffSkill : BaseSkill
    {
        private ResBattleParam battleParam;
        public bool bExtraBuff;
        public bool bFirstEffect;
        private int buffLevel = 1;
        public ResSkillCombineCfgInfo cfgData;
        public ulong controlTime;
        public int[] CustomParams = new int[6];
        public int iBuffWorkTimes;
        public const int MaxCustomParam = 6;
        private int overlayCount;
        public ulong ulStartTime;

        private bool CheckUseRule(SkillUseContext context)
        {
            if (context.TargetActor.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneNegative) && ((this.cfgData.dwEffectType == 1) || (this.cfgData.dwEffectType == 2)))
            {
                return false;
            }
            if (context.TargetActor.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneControl) && (this.cfgData.dwEffectType == 2))
            {
                return false;
            }
            return true;
        }

        private void DealTenacity(PoolObjHandle<ActorRoot> target)
        {
            int inLengthMs = 0;
            int totalValue = 0;
            int num3 = 0;
            int num4 = 0;
            ValueDataInfo info = null;
            info = target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_TENACITY];
            totalValue = info.totalValue;
            num4 = (int) ((totalValue + (target.handle.ValueComponent.mActorValue.actorLvl * this.battleParam.dwM_Tenacity)) + this.battleParam.dwN_Tenacity);
            if (num4 != 0)
            {
                num3 = (totalValue * 0x2710) / num4;
            }
            num3 += target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_CTRLREDUCE].totalValue;
            inLengthMs = this.curAction.handle.length;
            if (num3 != 0)
            {
                inLengthMs = (inLengthMs * (0x2710 - num3)) / 0x2710;
                this.curAction.handle.ResetLength(inLengthMs, false);
            }
        }

        public ResDT_SkillFunc FindSkillFunc(int inSkillFuncType)
        {
            if (this.cfgData != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (this.cfgData.astSkillFuncInfo[i].dwSkillFuncType == inSkillFuncType)
                    {
                        return this.cfgData.astSkillFuncInfo[i];
                    }
                }
            }
            return null;
        }

        public bool FindSkillFunc(int inSkillFuncType, out ResDT_SkillFunc outSkillFunc)
        {
            outSkillFunc = null;
            if (this.cfgData != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (this.cfgData.astSkillFuncInfo[i].dwSkillFuncType == inSkillFuncType)
                    {
                        outSkillFunc = this.cfgData.astSkillFuncInfo[i];
                        return true;
                    }
                }
            }
            return false;
        }

        public int GetBuffLevel()
        {
            return this.buffLevel;
        }

        public int GetOverlayCount()
        {
            return this.overlayCount;
        }

        public int GetSkillFuncParam(int inSkillFuncType, int _index, bool _bGrow)
        {
            ResDT_SkillFunc func = this.FindSkillFunc(inSkillFuncType);
            if (func != null)
            {
                if ((_index < 0) || ((_index + 1) > 6))
                {
                    object[] objArray1 = new object[] { _index };
                    DebugHelper.Assert(false, "GetSkillFuncParam: index = {0}", objArray1);
                }
                if (_bGrow)
                {
                    int iParam = func.astSkillFuncParam[_index].iParam;
                    int num3 = func.astSkillFuncGroup[_index].iParam * (this.buffLevel - 1);
                    iParam += num3;
                    return (iParam * this.overlayCount);
                }
                return func.astSkillFuncParam[_index].iParam;
            }
            object[] inParameters = new object[] { inSkillFuncType };
            DebugHelper.Assert(false, "FindSkillFunc error: inSkillFuncType = {0}", inParameters);
            return 0;
        }

        public void Init(int id)
        {
            base.SkillID = id;
            this.cfgData = GameDataMgr.skillCombineDatabin.GetDataByKey((long) id);
            if (this.cfgData != null)
            {
                base.ActionName = StringHelper.UTF8BytesToString(ref this.cfgData.szPrefab);
                base.bAgeImmeExcute = this.cfgData.bAgeImmeExcute == 1;
            }
            for (int i = 0; i < 6; i++)
            {
                this.CustomParams[i] = 0;
            }
            this.controlTime = 0L;
            this.battleParam = GameDataMgr.battleParam.GetAnyData();
            this.ulStartTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
        }

        public override void OnActionStoped(ref PoolObjHandle<AGE.Action> action)
        {
            PoolObjHandle<ActorRoot> handle = new PoolObjHandle<ActorRoot>();
            action.handle.refParams.GetRefParam("TargetActor", ref handle);
            if (((handle != 0) && (handle.handle.BuffHolderComp != null)) && handle.handle.BuffHolderComp.bRemoveList)
            {
                handle.handle.BuffHolderComp.ActionRemoveBuff(this);
            }
            base.OnActionStoped(ref action);
        }

        public override void OnRelease()
        {
            this.buffLevel = 1;
            this.overlayCount = 0;
            Array.Clear(this.CustomParams, 0, this.CustomParams.Length);
            this.bExtraBuff = false;
            this.cfgData = null;
            this.battleParam = null;
            base.OnRelease();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.buffLevel = 1;
            this.overlayCount = 0;
            this.controlTime = 0L;
            Array.Clear(this.CustomParams, 0, this.CustomParams.Length);
            this.bExtraBuff = false;
            this.cfgData = null;
            this.battleParam = null;
            this.ulStartTime = 0L;
            this.iBuffWorkTimes = 0;
            this.bFirstEffect = false;
        }

        public void SetBuffLevel(int _level)
        {
            this.buffLevel = _level;
        }

        public void SetOverlayCount(int _count)
        {
            this.overlayCount = _count;
        }

        public override bool Use(PoolObjHandle<ActorRoot> user)
        {
            return this.UseImpl(user);
        }

        public override bool Use(PoolObjHandle<ActorRoot> user, ref SkillUseParam param)
        {
            base.skillContext.Copy(ref param);
            return this.UseImpl(user);
        }

        private bool UseImpl(PoolObjHandle<ActorRoot> user)
        {
            if (((base.skillContext == null) || (base.skillContext.TargetActor == 0)) || (this.cfgData == null))
            {
                return false;
            }
            BuffHolderComponent buffHolderComp = base.skillContext.TargetActor.handle.BuffHolderComp;
            if (buffHolderComp == null)
            {
                return false;
            }
            if (!this.CheckUseRule(base.skillContext))
            {
                return false;
            }
            if (!buffHolderComp.overlayRule.CheckOverlay(this))
            {
                return false;
            }
            bool flag = false;
            bool flag2 = false;
            VInt3 forward = VInt3.forward;
            switch (base.skillContext.AppointType)
            {
                case SkillRangeAppointType.Pos:
                    flag = true;
                    break;

                case SkillRangeAppointType.Directional:
                    flag2 = true;
                    forward = base.skillContext.UseVector;
                    break;

                case SkillRangeAppointType.Track:
                    flag = true;
                    flag2 = true;
                    forward = base.skillContext.EndVector - base.skillContext.UseVector;
                    if (forward.sqrMagnitudeLong < 1L)
                    {
                        forward = VInt3.forward;
                    }
                    break;
            }
            GameObject obj2 = (base.skillContext.Originator == 0) ? null : base.skillContext.Originator.handle.gameObject;
            GameObject obj3 = (base.skillContext.TargetActor == 0) ? null : base.skillContext.TargetActor.handle.gameObject;
            GameObject[] objArray1 = new GameObject[] { obj2, obj3 };
            base.curAction = new PoolObjHandle<AGE.Action>(ActionManager.Instance.PlayAction(base.ActionName, true, false, objArray1));
            if (base.curAction == 0)
            {
                return false;
            }
            this.curAction.handle.onActionStop += new ActionStopDelegate(this.OnActionStoped);
            this.curAction.handle.refParams.AddRefParam("SkillObj", this);
            this.curAction.handle.refParams.AddRefParam("SkillContext", base.skillContext);
            this.curAction.handle.refParams.AddRefParam("TargetActor", base.skillContext.TargetActor);
            this.curAction.handle.refParams.SetRefParam("_BulletPos", base.skillContext.EffectPos);
            this.curAction.handle.refParams.SetRefParam("_BulletDir", base.skillContext.EffectDir);
            if (flag)
            {
                this.curAction.handle.refParams.SetRefParam("_TargetPos", base.skillContext.UseVector);
            }
            if (flag2)
            {
                this.curAction.handle.refParams.SetRefParam("_TargetDir", forward);
            }
            if (this.cfgData != null)
            {
                SkillSlotType slotType = base.skillContext.SlotType;
                int iDuration = this.cfgData.iDuration;
                if ((slotType >= SkillSlotType.SLOT_SKILL_1) && (slotType <= SkillSlotType.SLOT_SKILL_3))
                {
                    int skillLevel = 1;
                    if ((base.skillContext.Originator != 0) && (base.skillContext.Originator.handle.SkillControl != null))
                    {
                        SkillSlot slot = null;
                        base.skillContext.Originator.handle.SkillControl.TryGetSkillSlot(slotType, out slot);
                        if (slot != null)
                        {
                            skillLevel = slot.GetSkillLevel();
                        }
                    }
                    skillLevel = (skillLevel >= 1) ? skillLevel : 1;
                    iDuration += (skillLevel - 1) * this.cfgData.iDurationGrow;
                }
                if ((((base.skillContext.Originator != 0) && (base.skillContext.Originator.handle != null)) && ((base.skillContext.Originator.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (base.skillContext.Originator.handle.TheStaticData.TheHeroOnlyInfo.AttackDistanceType == 2))) && (this.cfgData.iLongRangeReduction > 0))
                {
                    iDuration = (iDuration * this.cfgData.iLongRangeReduction) / 0x2710;
                }
                this.curAction.handle.ResetLength(iDuration, false);
                if (this.cfgData.dwEffectType == 2)
                {
                    this.DealTenacity(base.skillContext.TargetActor);
                }
            }
            bool flag3 = true;
            if ((this.cfgData.dwShowType != 0) || (this.cfgData.dwFloatTextID > 0))
            {
                BuffSkill skill = null;
                if (((base.skillContext.TargetActor == 0) || (base.skillContext.TargetActor.handle == null)) || ((base.skillContext.TargetActor.handle.BuffHolderComp == null) || (base.skillContext.TargetActor.handle.BuffHolderComp.SpawnedBuffList == null)))
                {
                    return false;
                }
                for (int i = 0; i < base.skillContext.TargetActor.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
                {
                    skill = base.skillContext.TargetActor.handle.BuffHolderComp.SpawnedBuffList[i];
                    if (((skill != null) && (skill.cfgData != null)) && (skill.cfgData.iCfgID == this.cfgData.iCfgID))
                    {
                        flag3 = false;
                        break;
                    }
                }
                if (flag3)
                {
                    SpawnBuffEventParam param = new SpawnBuffEventParam(this.cfgData.dwShowType, this.cfgData.dwFloatTextID, base.skillContext.TargetActor);
                    Singleton<GameSkillEventSys>.GetInstance().SendEvent<SpawnBuffEventParam>(GameSkillEventDef.Event_SpawnBuff, base.skillContext.TargetActor, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
                }
            }
            base.skillContext.TargetActor.handle.BuffHolderComp.AddBuff(this);
            if ((this.cfgData.dwEffectType == 2) && (this.cfgData.dwShowType != 2))
            {
                LimitMoveEventParam param2 = new LimitMoveEventParam(base.CurAction.handle.length, base.SkillID, base.skillContext.TargetActor);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<LimitMoveEventParam>(GameSkillEventDef.AllEvent_LimitMove, base.skillContext.TargetActor, ref param2, GameSkillEventChannel.Channel_AllActor);
            }
            if (base.bAgeImmeExcute)
            {
                this.curAction.handle.UpdateLogic((int) Singleton<FrameSynchr>.GetInstance().FrameDelta);
            }
            return true;
        }

        public override bool isBuff
        {
            get
            {
                return true;
            }
        }

        public string SkillFuncCombineDesc
        {
            get
            {
                return Utility.UTF8Convert(this.cfgData.szSkillCombineDesc);
            }
        }

        public string SkillFuncCombineName
        {
            get
            {
                return Utility.UTF8Convert(this.cfgData.szSkillCombineName);
            }
        }
    }
}

