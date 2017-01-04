namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class BuffHolderComponent : LogicComponent
    {
        public bool bRemoveList = true;
        public BuffChangeSkillRule changeSkillRule;
        public BuffClearRule clearRule;
        private List<BuffSkill> delBuffList = new List<BuffSkill>(3);
        public BufferLogicEffect logicEffect;
        public BufferMarkRule markRule;
        public BuffOverlayRule overlayRule;
        public BuffProtectRule protectRule;
        public List<BuffSkill> SpawnedBuffList = new List<BuffSkill>();

        public void ActionRemoveBuff(BuffSkill inBuff)
        {
            if (this.SpawnedBuffList.Remove(inBuff))
            {
                PoolObjHandle<BuffSkill> handle = new PoolObjHandle<BuffSkill>(inBuff);
                this.protectRule.RemoveBuff(ref handle);
                this.logicEffect.RemoveBuff(ref handle);
                BuffChangeEventParam param = new BuffChangeEventParam(false, base.actorPtr, inBuff);
                Singleton<GameSkillEventSys>.GetInstance().SendEvent<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, base.actorPtr, ref param, GameSkillEventChannel.Channel_AllActor);
                inBuff.Release();
            }
        }

        public void AddBuff(BuffSkill inBuff)
        {
            this.SpawnedBuffList.Add(inBuff);
            this.protectRule.AddBuff(inBuff);
            this.logicEffect.AddBuff(inBuff);
            BuffChangeEventParam param = new BuffChangeEventParam(true, base.actorPtr, inBuff);
            Singleton<GameSkillEventSys>.GetInstance().SendEvent<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, base.actorPtr, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
            if (((inBuff.cfgData != null) && (inBuff.cfgData.bIsAssistEffect == 1)) && ((inBuff.skillContext.Originator != 0) && (base.actor.ValueComponent.actorHp > 0)))
            {
                if (base.actor.TheActorMeta.ActorCamp == inBuff.skillContext.Originator.handle.TheActorMeta.ActorCamp)
                {
                    base.actor.ActorControl.AddHelpSelfActor(inBuff.skillContext.Originator);
                }
                else
                {
                    base.actor.ActorControl.AddHurtSelfActor(inBuff.skillContext.Originator);
                }
            }
        }

        private bool CheckTargetFromEnemy(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> target)
        {
            bool flag = false;
            if ((src != 0) && (target != 0))
            {
                float num2;
                float num3;
                if (src.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
                {
                    return flag;
                }
                if (target.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Monster)
                {
                    return ActorHelper.IsHostEnemyActor(ref target);
                }
                MonsterWrapper wrapper = target.handle.AsMonster();
                if (wrapper == null)
                {
                    return flag;
                }
                RES_MONSTER_TYPE bMonsterType = (RES_MONSTER_TYPE) wrapper.cfgInfo.bMonsterType;
                if (bMonsterType == RES_MONSTER_TYPE.RES_MONSTER_TYPE_SOLDIERLINE)
                {
                    if (src.handle.TheActorMeta.ActorCamp != target.handle.TheActorMeta.ActorCamp)
                    {
                        flag = true;
                    }
                    return flag;
                }
                if (bMonsterType != RES_MONSTER_TYPE.RES_MONSTER_TYPE_JUNGLE)
                {
                    return flag;
                }
                switch (wrapper.GetActorSubSoliderType())
                {
                    case 8:
                    case 9:
                    case 7:
                    case 14:
                        return flag;

                    default:
                    {
                        num2 = 0f;
                        num3 = 0f;
                        Vector3 position = target.handle.gameObject.transform.position;
                        List<PoolObjHandle<ActorRoot>> organActors = Singleton<GameObjMgr>.instance.OrganActors;
                        int num4 = 0;
                        for (int i = 0; i < organActors.Count; i++)
                        {
                            PoolObjHandle<ActorRoot> handle = organActors[i];
                            if (handle.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2)
                            {
                                Vector3 b = handle.handle.gameObject.transform.position;
                                if (handle.handle.TheActorMeta.ActorCamp == src.handle.TheActorMeta.ActorCamp)
                                {
                                    num2 = Vector3.Distance(position, b);
                                }
                                else
                                {
                                    num3 = Vector3.Distance(position, b);
                                }
                                num4++;
                                if (num4 >= 2)
                                {
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
                if (num2 > num3)
                {
                    flag = true;
                }
            }
            return flag;
        }

        public bool CheckTargetSubType(int typeMask, int typeSubMask, PoolObjHandle<ActorRoot> target)
        {
            if (typeMask == 0)
            {
                return true;
            }
            if (target != 0)
            {
                int actorType = (int) target.handle.TheActorMeta.ActorType;
                if ((typeMask & (((int) 1) << actorType)) > 0)
                {
                    if (actorType != 1)
                    {
                        return true;
                    }
                    if (typeSubMask == 0)
                    {
                        return true;
                    }
                    if (target.handle.ActorControl.GetActorSubType() == typeSubMask)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ClearBuff()
        {
            BuffSkill skill;
            this.bRemoveList = false;
            for (int i = 0; i < this.SpawnedBuffList.Count; i++)
            {
                skill = this.SpawnedBuffList[i];
                if (skill != null)
                {
                    skill.Stop();
                }
            }
            if (this.protectRule != null)
            {
                this.protectRule.ClearBuff();
            }
            if (this.logicEffect != null)
            {
                this.logicEffect.ClearBuff();
            }
            for (int j = 0; j < this.SpawnedBuffList.Count; j++)
            {
                skill = this.SpawnedBuffList[j];
                if (skill != null)
                {
                    skill.Release();
                }
            }
            this.SpawnedBuffList.Clear();
            this.delBuffList.Clear();
            this.bRemoveList = true;
        }

        public void ClearEffectTypeBuff(int _typeMask)
        {
            if (this.SpawnedBuffList.Count != 0)
            {
                this.delBuffList = this.SpawnedBuffList;
                for (int i = 0; i < this.delBuffList.Count; i++)
                {
                    BuffSkill skill = this.delBuffList[i];
                    if ((_typeMask & (((int) 1) << skill.cfgData.dwEffectType)) > 0)
                    {
                        skill.Stop();
                    }
                }
                if (this.markRule != null)
                {
                    this.markRule.ClearBufferMark(_typeMask);
                }
            }
        }

        public override void Deactive()
        {
            this.ClearBuff();
            base.Deactive();
        }

        public BuffSkill FindBuff(int inSkillCombineId)
        {
            if (this.SpawnedBuffList != null)
            {
                for (int i = 0; i < this.SpawnedBuffList.Count; i++)
                {
                    BuffSkill skill = this.SpawnedBuffList[i];
                    if ((skill != null) && (skill.SkillID == inSkillCombineId))
                    {
                        return skill;
                    }
                }
            }
            return null;
        }

        public int FindBuffCount(int inSkillCombineId)
        {
            int num = 0;
            for (int i = 0; i < this.SpawnedBuffList.Count; i++)
            {
                BuffSkill skill = this.SpawnedBuffList[i];
                if ((skill != null) && (skill.SkillID == inSkillCombineId))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetCoinAddRate(PoolObjHandle<ActorRoot> _target)
        {
            int num = 0;
            BuffSkill skill = null;
            ResDT_SkillFunc outSkillFunc = null;
            if (_target != 0)
            {
                for (int i = 0; i < this.SpawnedBuffList.Count; i++)
                {
                    skill = this.SpawnedBuffList[i];
                    if ((skill != null) && skill.FindSkillFunc(0x47, out outSkillFunc))
                    {
                        int typeMask = skill.GetSkillFuncParam(0x47, 0, false);
                        int typeSubMask = skill.GetSkillFuncParam(0x47, 1, false);
                        int num5 = skill.GetSkillFuncParam(0x47, 2, false);
                        int num6 = skill.GetSkillFuncParam(0x47, 3, false);
                        if (this.CheckTargetSubType(typeMask, typeSubMask, _target))
                        {
                            bool flag = true;
                            if (num6 > 0)
                            {
                                flag = this.CheckTargetFromEnemy(base.actorPtr, _target);
                            }
                            if (flag)
                            {
                                num += num5;
                            }
                        }
                    }
                }
            }
            return num;
        }

        public int GetExtraHurtOutputRate(PoolObjHandle<ActorRoot> _attack)
        {
            int num = 0;
            BuffSkill skill = null;
            if (_attack == 0)
            {
                return 0;
            }
            for (int i = 0; i < _attack.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
            {
                skill = _attack.handle.BuffHolderComp.SpawnedBuffList[i];
                num += this.OnConditionExtraHurt(skill, _attack);
                num += this.OnTargetExtraHurt(skill, _attack);
                num += this.OnControlExtraHurt(skill, _attack);
            }
            return num;
        }

        public int GetSoulExpAddRate(PoolObjHandle<ActorRoot> _target)
        {
            int num = 0;
            BuffSkill skill = null;
            ResDT_SkillFunc outSkillFunc = null;
            if (_target != 0)
            {
                for (int i = 0; i < this.SpawnedBuffList.Count; i++)
                {
                    skill = this.SpawnedBuffList[i];
                    if ((skill != null) && skill.FindSkillFunc(0x31, out outSkillFunc))
                    {
                        int typeMask = skill.GetSkillFuncParam(0x31, 0, false);
                        int typeSubMask = skill.GetSkillFuncParam(0x31, 1, false);
                        int num5 = skill.GetSkillFuncParam(0x31, 2, false);
                        int num6 = skill.GetSkillFuncParam(0x31, 3, false);
                        if (this.CheckTargetSubType(typeMask, typeSubMask, _target))
                        {
                            bool flag = true;
                            if (num6 > 0)
                            {
                                flag = this.CheckTargetFromEnemy(base.actorPtr, _target);
                            }
                            if (flag)
                            {
                                num += num5;
                            }
                        }
                    }
                }
            }
            return num;
        }

        public override void Init()
        {
            this.overlayRule = new BuffOverlayRule();
            this.clearRule = new BuffClearRule();
            this.protectRule = new BuffProtectRule();
            this.changeSkillRule = new BuffChangeSkillRule();
            this.markRule = new BufferMarkRule();
            this.logicEffect = new BufferLogicEffect();
            this.overlayRule.Init(this);
            this.clearRule.Init(this);
            this.protectRule.Init(this);
            this.changeSkillRule.Init(this);
            this.markRule.Init(this);
            this.logicEffect.Init(this);
            base.Init();
        }

        public bool IsExistSkillFuncType(int inSkillFuncType)
        {
            BuffSkill skill = null;
            ResDT_SkillFunc outSkillFunc = null;
            for (int i = 0; i < this.SpawnedBuffList.Count; i++)
            {
                skill = this.SpawnedBuffList[i];
                if ((skill != null) && skill.FindSkillFunc(inSkillFuncType, out outSkillFunc))
                {
                    return true;
                }
            }
            return false;
        }

        public void OnAssistEffect(ref PoolObjHandle<ActorRoot> deadActor)
        {
            BuffSkill skill = null;
            ResDT_SkillFunc outSkillFunc = null;
            for (int i = 0; i < this.SpawnedBuffList.Count; i++)
            {
                skill = this.SpawnedBuffList[i];
                if ((skill != null) && skill.FindSkillFunc(0x21, out outSkillFunc))
                {
                    int inSkillCombineId = skill.GetSkillFuncParam(0x21, 0, false);
                    int num3 = skill.GetSkillFuncParam(0x21, 1, false);
                    int num4 = skill.GetSkillFuncParam(0x21, 2, false);
                    int typeMask = skill.GetSkillFuncParam(0x21, 3, false);
                    int typeSubMask = skill.GetSkillFuncParam(0x21, 4, false);
                    if ((num4 == 2) && this.CheckTargetSubType(typeMask, typeSubMask, deadActor))
                    {
                        SkillUseParam inParam = new SkillUseParam();
                        inParam.SetOriginator(base.actorPtr);
                        base.actor.SkillControl.SpawnBuff(base.actorPtr, ref inParam, inSkillCombineId, true);
                    }
                }
            }
        }

        private int OnConditionExtraHurt(BuffSkill _buffSkill, PoolObjHandle<ActorRoot> _attack)
        {
            int num = 0;
            ResDT_SkillFunc outSkillFunc = null;
            if ((_buffSkill != null) && _buffSkill.FindSkillFunc(0x2c, out outSkillFunc))
            {
                int num2 = _buffSkill.GetSkillFuncParam(0x2c, 0, false);
                int num3 = _buffSkill.GetSkillFuncParam(0x2c, 1, false);
                int num4 = _buffSkill.GetSkillFuncParam(0x2c, 2, false);
                int num5 = _buffSkill.GetSkillFuncParam(0x2c, 3, false);
                bool flag = num2 == 1;
                int num6 = !flag ? base.actor.ValueComponent.actorHp : _attack.handle.ValueComponent.actorHp;
                int num7 = !flag ? base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue : _attack.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
                int num8 = (num6 * 0x2710) / num7;
                if (num4 == 1)
                {
                    if (num8 <= num3)
                    {
                        num = num5;
                    }
                    return num;
                }
                if ((num4 == 4) && (num8 >= num3))
                {
                    num = num5;
                }
            }
            return num;
        }

        private int OnControlExtraHurt(BuffSkill _buffSkill, PoolObjHandle<ActorRoot> _attack)
        {
            ResDT_SkillFunc outSkillFunc = null;
            if (((_buffSkill != null) && _buffSkill.FindSkillFunc(0x33, out outSkillFunc)) && (base.actor != null))
            {
                BuffSkill skill = null;
                for (int i = 0; i < base.actor.BuffHolderComp.SpawnedBuffList.Count; i++)
                {
                    skill = base.actor.BuffHolderComp.SpawnedBuffList[i];
                    if ((skill != null) && (skill.cfgData.dwEffectType == 2))
                    {
                        return _buffSkill.GetSkillFuncParam(0x33, 0, false);
                    }
                }
            }
            return 0;
        }

        public int OnDamage(ref HurtDataInfo _hurt, int _hurtValue)
        {
            int num = _hurtValue;
            if (!_hurt.bLastHurt)
            {
                this.clearRule.CheckBuffClear(RES_SKILLFUNC_CLEAR_RULE.RES_SKILLFUNC_CLEAR_DAMAGE);
            }
            if (!_hurt.bExtraBuff)
            {
                this.OnDamageExtraEffect(_hurt.atker, _hurt.atkSlot);
            }
            num = (num * _hurt.iEffectFadeRate) / 0x2710;
            num = (num * _hurt.iOverlayFadeRate) / 0x2710;
            num = this.protectRule.ResistDamage(ref _hurt, num);
            return BufferLogicEffect.OnDamageExtraEffect(ref _hurt, num);
        }

        private void OnDamageExtraEffect(PoolObjHandle<ActorRoot> _attack, SkillSlotType _slotType)
        {
            BuffSkill inBuff = null;
            ResDT_SkillFunc outSkillFunc = null;
            if (_attack != 0)
            {
                for (int i = 0; i < _attack.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
                {
                    inBuff = _attack.handle.BuffHolderComp.SpawnedBuffList[i];
                    if ((inBuff != null) && inBuff.FindSkillFunc(0x21, out outSkillFunc))
                    {
                        bool flag = false;
                        bool flag2 = true;
                        int inSkillCombineId = inBuff.GetSkillFuncParam(0x21, 0, false);
                        int num3 = inBuff.GetSkillFuncParam(0x21, 1, false);
                        int num4 = inBuff.GetSkillFuncParam(0x21, 2, false);
                        int typeMask = inBuff.GetSkillFuncParam(0x21, 3, false);
                        int typeSubMask = inBuff.GetSkillFuncParam(0x21, 4, false);
                        int num7 = inBuff.GetSkillFuncParam(0x21, 5, false);
                        if ((num4 == 0) && this.CheckTargetSubType(typeMask, typeSubMask, base.actorPtr))
                        {
                            if (num3 == 0)
                            {
                                flag = true;
                            }
                            else if ((num3 & (((int) 1) << _slotType)) > 0)
                            {
                                flag = true;
                            }
                            if (num7 > 0)
                            {
                                if ((Singleton<FrameSynchr>.GetInstance().LogicFrameTick - inBuff.controlTime) >= num7)
                                {
                                    flag2 = true;
                                }
                                else
                                {
                                    flag2 = false;
                                }
                            }
                            if (flag && flag2)
                            {
                                SkillUseParam inParam = new SkillUseParam();
                                inParam.SetOriginator(_attack);
                                _attack.handle.SkillControl.SpawnBuff(base.actorPtr, ref inParam, inSkillCombineId, true);
                                inBuff.controlTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                                if (num7 == -1)
                                {
                                    _attack.handle.BuffHolderComp.RemoveBuff(inBuff);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void OnDamageExtraValueEffect(ref HurtDataInfo _hurt, PoolObjHandle<ActorRoot> _attack, SkillSlotType _slotType)
        {
            BuffSkill skill = null;
            ResDT_SkillFunc outSkillFunc = null;
            if (_attack != 0)
            {
                for (int i = 0; i < _attack.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
                {
                    skill = _attack.handle.BuffHolderComp.SpawnedBuffList[i];
                    if (_hurt.hurtType == HurtTypeDef.Therapic)
                    {
                        if ((skill != null) && skill.FindSkillFunc(0x40, out outSkillFunc))
                        {
                            int num2 = skill.GetSkillFuncParam(0x40, 0, false);
                            int num3 = skill.GetSkillFuncParam(0x40, 1, false);
                            _hurt.iAddTotalHurtType = num2;
                            _hurt.iAddTotalHurtValue = num3;
                        }
                    }
                    else
                    {
                        if ((((_slotType == SkillSlotType.SLOT_SKILL_0) && (_attack.handle.SkillControl != null)) && (_attack.handle.SkillControl.bIsLastAtkUseSkill && (skill != null))) && skill.FindSkillFunc(0x3d, out outSkillFunc))
                        {
                            int num4 = skill.GetSkillFuncParam(0x3d, 0, false);
                            int num5 = skill.GetSkillFuncParam(0x3d, 1, false);
                            int num6 = skill.GetSkillFuncParam(0x3d, 2, false);
                            int num7 = skill.GetSkillFuncParam(0x3d, 3, false);
                            if (num4 == 1)
                            {
                                num5 = (num5 * _hurt.hurtValue) / 0x2710;
                                _hurt.hurtValue += num5;
                                _hurt.adValue += num6;
                                _hurt.apValue += num7;
                            }
                            else
                            {
                                _hurt.hurtValue += num5;
                                _hurt.attackInfo.iActorATT += num6;
                                _hurt.attackInfo.iActorINT += num7;
                            }
                        }
                        if (((_hurt.target != 0) && (_hurt.target.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ)) && ((skill != null) && skill.FindSkillFunc(0x44, out outSkillFunc)))
                        {
                            int num8 = skill.GetSkillFuncParam(0x44, 0, false);
                            int num9 = skill.GetSkillFuncParam(0x44, 4, false);
                            int num10 = skill.GetSkillFuncParam(0x44, 5, false);
                            if (_hurt.target.handle.ValueComponent != null)
                            {
                                if (num8 == 1)
                                {
                                    num9 = (_hurt.target.handle.ValueComponent.actorHpTotal * num9) / 0x2710;
                                }
                                if (((_hurt.target.handle.ValueComponent.actorHp <= num9) && (_hurt.target.handle.ActorControl != null)) && ((Singleton<FrameSynchr>.instance.LogicFrameTick - _hurt.target.handle.ActorControl.lastExtraHurtByLowHpBuffTime) >= num10))
                                {
                                    _hurt.target.handle.ActorControl.lastExtraHurtByLowHpBuffTime = Singleton<FrameSynchr>.instance.LogicFrameTick;
                                    int num11 = skill.GetSkillFuncParam(0x44, 1, false);
                                    int num12 = skill.GetSkillFuncParam(0x44, 2, false);
                                    int num13 = skill.GetSkillFuncParam(0x44, 3, false);
                                    if (num8 == 1)
                                    {
                                        num11 = (num11 * _hurt.hurtValue) / 0x2710;
                                        num12 = (num12 * _hurt.adValue) / 0x2710;
                                        num13 = (num13 * _hurt.apValue) / 0x2710;
                                    }
                                    _hurt.hurtValue += num11;
                                    _hurt.adValue += num12;
                                    _hurt.apValue += num13;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void OnDead(PoolObjHandle<ActorRoot> _attack)
        {
            BuffSkill inBuff = null;
            ResDT_SkillFunc outSkillFunc = null;
            if (this.clearRule != null)
            {
                this.clearRule.CheckBuffNoClear(RES_SKILLFUNC_CLEAR_RULE.RES_SKILLFUNC_CLEAR_DEAD);
            }
            if (this.logicEffect != null)
            {
                this.logicEffect.Clear();
            }
            if (this.actorPtr.handle.ActorControl.IsKilledByHero())
            {
                _attack = this.actorPtr.handle.ActorControl.LastHeroAtker;
            }
            for (int i = 0; i < this.SpawnedBuffList.Count; i++)
            {
                inBuff = this.SpawnedBuffList[i];
                if ((inBuff != null) && inBuff.FindSkillFunc(0x20, out outSkillFunc))
                {
                    int reviveTime = inBuff.GetSkillFuncParam(0x20, 0, false);
                    int reviveLife = inBuff.GetSkillFuncParam(0x20, 1, false);
                    bool autoReset = inBuff.GetSkillFuncParam(0x20, 2, false) == 1;
                    bool bBaseRevive = inBuff.GetSkillFuncParam(0x20, 3, false) == 0;
                    bool bCDReset = inBuff.GetSkillFuncParam(0x20, 4, false) == 1;
                    base.actor.ActorControl.SetReviveContext(reviveTime, reviveLife, autoReset, bBaseRevive, bCDReset);
                    this.RemoveBuff(inBuff);
                }
                if ((((base.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (_attack != 0)) && ((_attack.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (inBuff != null))) && ((inBuff.cfgData != null) && (inBuff.cfgData.bIsInheritByKiller == 1)))
                {
                    this.RemoveBuff(inBuff);
                    SkillUseParam inParam = new SkillUseParam();
                    inParam.SetOriginator(_attack);
                    _attack.handle.SkillControl.SpawnBuff(_attack, ref inParam, inBuff.SkillID, true);
                }
            }
            this.OnDeadExtraEffect(_attack);
            this.markRule.Clear();
        }

        private void OnDeadExtraEffect(PoolObjHandle<ActorRoot> _attack)
        {
            if (_attack != 0)
            {
                BuffSkill skill = null;
                ResDT_SkillFunc outSkillFunc = null;
                for (int i = 0; i < _attack.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
                {
                    skill = _attack.handle.BuffHolderComp.SpawnedBuffList[i];
                    if ((skill != null) && skill.FindSkillFunc(0x21, out outSkillFunc))
                    {
                        int inSkillCombineId = skill.GetSkillFuncParam(0x21, 0, false);
                        int num3 = skill.GetSkillFuncParam(0x21, 1, false);
                        int num4 = skill.GetSkillFuncParam(0x21, 2, false);
                        int typeMask = skill.GetSkillFuncParam(0x21, 3, false);
                        int typeSubMask = skill.GetSkillFuncParam(0x21, 4, false);
                        if ((num4 == 1) && this.CheckTargetSubType(typeMask, typeSubMask, base.actorPtr))
                        {
                            SkillUseParam inParam = new SkillUseParam();
                            inParam.SetOriginator(_attack);
                            _attack.handle.SkillControl.SpawnBuff(_attack, ref inParam, inSkillCombineId, true);
                        }
                    }
                }
            }
        }

        private int OnTargetExtraHurt(BuffSkill _buffSkill, PoolObjHandle<ActorRoot> _attack)
        {
            int num = 0;
            ResDT_SkillFunc outSkillFunc = null;
            if ((_buffSkill != null) && _buffSkill.FindSkillFunc(0x30, out outSkillFunc))
            {
                int typeMask = _buffSkill.GetSkillFuncParam(0x30, 0, false);
                int typeSubMask = _buffSkill.GetSkillFuncParam(0x30, 1, false);
                int num4 = _buffSkill.GetSkillFuncParam(0x30, 2, false);
                if (this.CheckTargetSubType(typeMask, typeSubMask, base.actorPtr))
                {
                    num = num4;
                }
            }
            return num;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.SpawnedBuffList.Clear();
            this.overlayRule = null;
            this.clearRule = null;
            this.protectRule = null;
            this.changeSkillRule = null;
            this.markRule = null;
            this.logicEffect = null;
            this.bRemoveList = true;
            this.delBuffList.Clear();
        }

        public override void Reactive()
        {
            base.Reactive();
            this.overlayRule.Init(this);
            this.clearRule.Init(this);
            this.protectRule.Init(this);
            this.changeSkillRule.Init(this);
            this.markRule.Init(this);
            this.logicEffect.Init(this);
        }

        public void RemoveBuff(BuffSkill inBuff)
        {
            if (this.SpawnedBuffList.Count != 0)
            {
                this.delBuffList = this.SpawnedBuffList;
                for (int i = 0; i < this.delBuffList.Count; i++)
                {
                    BuffSkill skill = this.delBuffList[i];
                    if (skill == inBuff)
                    {
                        BuffChangeEventParam param = new BuffChangeEventParam(false, base.actorPtr, inBuff);
                        Singleton<GameSkillEventSys>.GetInstance().SendEvent<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, base.actorPtr, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
                        skill.Stop();
                        if (((inBuff.cfgData.dwEffectType == 2) && (inBuff.cfgData.dwShowType != 2)) && (base.actorPtr != 0))
                        {
                            LimitMoveEventParam param2 = new LimitMoveEventParam(0, inBuff.SkillID, base.actorPtr);
                            Singleton<GameSkillEventSys>.GetInstance().SendEvent<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, base.actorPtr, ref param2, GameSkillEventChannel.Channel_AllActor);
                        }
                    }
                }
            }
        }

        public void RemoveBuff(int inSkillCombineId)
        {
            if (this.SpawnedBuffList.Count != 0)
            {
                this.delBuffList = this.SpawnedBuffList;
                for (int i = 0; i < this.delBuffList.Count; i++)
                {
                    BuffSkill skill = this.delBuffList[i];
                    if ((skill != null) && (skill.SkillID == inSkillCombineId))
                    {
                        skill.Stop();
                        if (((skill.cfgData.dwEffectType == 2) && (skill.cfgData.dwShowType != 2)) && (base.actorPtr != 0))
                        {
                            LimitMoveEventParam param = new LimitMoveEventParam(0, skill.SkillID, base.actorPtr);
                            Singleton<GameSkillEventSys>.GetInstance().SendEvent<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, base.actorPtr, ref param, GameSkillEventChannel.Channel_AllActor);
                        }
                    }
                }
            }
        }

        public void RemoveSkillEffectGroup(int inGroupID)
        {
            if (this.SpawnedBuffList.Count != 0)
            {
                this.delBuffList = this.SpawnedBuffList;
                for (int i = 0; i < this.delBuffList.Count; i++)
                {
                    BuffSkill skill = this.delBuffList[i];
                    if (((skill != null) && (skill.cfgData != null)) && (skill.cfgData.iCroupID == inGroupID))
                    {
                        skill.Stop();
                        if (((skill.cfgData.dwEffectType == 2) && (skill.cfgData.dwShowType != 2)) && (base.actorPtr != 0))
                        {
                            LimitMoveEventParam param = new LimitMoveEventParam(0, skill.SkillID, base.actorPtr);
                            Singleton<GameSkillEventSys>.GetInstance().SendEvent<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, base.actorPtr, ref param, GameSkillEventChannel.Channel_AllActor);
                        }
                    }
                }
            }
        }

        public override void UpdateLogic(int nDelta)
        {
            if (this.markRule != null)
            {
                this.markRule.UpdateLogic(nDelta);
            }
        }
    }
}

