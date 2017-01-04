namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;
    using System.Collections.Generic;

    public class BuffProtectRule
    {
        private List<PoolObjHandle<BuffSkill>> AllProtectBuffList = new List<PoolObjHandle<BuffSkill>>();
        private BuffHolderComponent buffHolder;
        private BuffLimiteHurt limiteMaxHpHurt;
        private uint m_uiBeProtectedTotalValue;
        private uint m_uiBeProtectedValueToHeroMagic;
        private uint m_uiBeProtectedValueToHeroPhys;
        private uint m_uiBeProtectedValueToHeroReal;
        private uint m_uiProtectTotalValue;
        private uint m_uiProtectValueFromHero;
        private List<PoolObjHandle<BuffSkill>> MagicProtectList = new List<PoolObjHandle<BuffSkill>>();
        private List<PoolObjHandle<BuffSkill>> NoHurtBuffList = new List<PoolObjHandle<BuffSkill>>();
        private List<PoolObjHandle<BuffSkill>> PhysicsProtectList = new List<PoolObjHandle<BuffSkill>>();
        public const int PROTECT_PARAM_INDEX = 3;
        private int protectValue;

        public void AddBuff(BuffSkill inBuff)
        {
            if (inBuff.cfgData.dwEffectSubType == 1)
            {
                this.PhysicsProtectList.Add(new PoolObjHandle<BuffSkill>(inBuff));
            }
            else if (inBuff.cfgData.dwEffectSubType == 2)
            {
                this.MagicProtectList.Add(new PoolObjHandle<BuffSkill>(inBuff));
            }
            else if (inBuff.cfgData.dwEffectSubType == 3)
            {
                this.AllProtectBuffList.Add(new PoolObjHandle<BuffSkill>(inBuff));
            }
            else if (((inBuff.cfgData.dwEffectSubType == 4) || (inBuff.cfgData.dwEffectSubType == 5)) || (inBuff.cfgData.dwEffectSubType == 6))
            {
                this.NoHurtBuffList.Add(new PoolObjHandle<BuffSkill>(inBuff));
            }
        }

        private bool CheckTargetNoDamage(ref HurtDataInfo _hurt, BuffSkill _buffSkill)
        {
            int num = _buffSkill.CustomParams[3];
            int num2 = _buffSkill.CustomParams[4];
            if (num == 0)
            {
                return true;
            }
            if (_hurt.atker != 0)
            {
                int actorType = (int) _hurt.atker.handle.TheActorMeta.ActorType;
                if ((num & (((int) 1) << actorType)) > 0)
                {
                    if (actorType != 1)
                    {
                        return true;
                    }
                    if (num2 == 0)
                    {
                        return true;
                    }
                    if (_hurt.atker.handle.ActorControl.GetActorSubType() == num2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ClearBuff()
        {
            this.ClearProtectBuff(this.PhysicsProtectList);
            this.ClearProtectBuff(this.MagicProtectList);
            this.ClearProtectBuff(this.AllProtectBuffList);
            this.NoHurtBuffList.Clear();
        }

        private void ClearProtectBuff(List<PoolObjHandle<BuffSkill>> _inList)
        {
            if (_inList.Count != 0)
            {
                PoolObjHandle<BuffSkill>[] handleArray = _inList.ToArray();
                for (int i = 0; i < handleArray.Length; i++)
                {
                    this.RemoveBuff(ref handleArray[i]);
                }
            }
        }

        public uint GetProtectTotalValue()
        {
            return this.m_uiProtectTotalValue;
        }

        public void Init(BuffHolderComponent _buffHolder)
        {
            this.protectValue = 0;
            this.buffHolder = _buffHolder;
            this.limiteMaxHpHurt.bValid = false;
            this.m_uiProtectTotalValue = 0;
            this.m_uiProtectValueFromHero = 0;
            this.m_uiBeProtectedTotalValue = 0;
            this.m_uiBeProtectedValueToHeroPhys = 0;
            this.m_uiBeProtectedValueToHeroMagic = 0;
            this.m_uiBeProtectedValueToHeroReal = 0;
        }

        private bool NoDamageImpl(ref HurtDataInfo _hurt)
        {
            BuffSkill skill = null;
            for (int i = 0; i < this.NoHurtBuffList.Count; i++)
            {
                skill = this.NoHurtBuffList[i];
                if (skill != null)
                {
                    if (skill.cfgData.dwEffectSubType == 6)
                    {
                        if (this.CheckTargetNoDamage(ref _hurt, skill))
                        {
                            return true;
                        }
                    }
                    else if (_hurt.hurtType == HurtTypeDef.PhysHurt)
                    {
                        if ((skill.cfgData.dwEffectSubType == 4) && this.CheckTargetNoDamage(ref _hurt, skill))
                        {
                            return true;
                        }
                    }
                    else if (((_hurt.hurtType == HurtTypeDef.MagicHurt) && (skill.cfgData.dwEffectSubType == 5)) && this.CheckTargetNoDamage(ref _hurt, skill))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void RemoveBuff(ref PoolObjHandle<BuffSkill> inBuff)
        {
            if (inBuff.handle.cfgData.dwEffectSubType == 1)
            {
                this.SendProtectEvent(0, -inBuff.handle.CustomParams[0]);
                this.PhysicsProtectList.Remove(inBuff);
            }
            else if (inBuff.handle.cfgData.dwEffectSubType == 2)
            {
                this.SendProtectEvent(1, -inBuff.handle.CustomParams[1]);
                this.MagicProtectList.Remove(inBuff);
            }
            else if (inBuff.handle.cfgData.dwEffectSubType == 3)
            {
                this.SendProtectEvent(2, -inBuff.handle.CustomParams[2]);
                this.AllProtectBuffList.Remove(inBuff);
            }
            else if (((inBuff.handle.cfgData.dwEffectSubType == 4) || (inBuff.handle.cfgData.dwEffectSubType == 5)) || (inBuff.handle.cfgData.dwEffectSubType == 6))
            {
                this.NoHurtBuffList.Remove(inBuff);
            }
        }

        public int ResistDamage(ref HurtDataInfo _hurt, int _hurtValue)
        {
            int changeValue = 0;
            int num2 = _hurtValue;
            if (_hurtValue > 0)
            {
                if (this.NoDamageImpl(ref _hurt))
                {
                    this.SendHurtImmuneEvent(_hurt.atker);
                    return 0;
                }
                if (_hurt.hurtType == HurtTypeDef.PhysHurt)
                {
                    changeValue = _hurtValue;
                    _hurtValue = this.ResistProtectImpl(_hurtValue, this.PhysicsProtectList, 0);
                    changeValue -= _hurtValue;
                    this.SendProtectEvent(0, -changeValue);
                    if (_hurtValue > 0)
                    {
                        changeValue = _hurtValue;
                        _hurtValue = this.ResistProtectImpl(_hurtValue, this.AllProtectBuffList, 2);
                        changeValue -= _hurtValue;
                        this.SendProtectEvent(2, -changeValue);
                    }
                }
                else if (_hurt.hurtType == HurtTypeDef.MagicHurt)
                {
                    changeValue = _hurtValue;
                    _hurtValue = this.ResistProtectImpl(_hurtValue, this.MagicProtectList, 1);
                    changeValue -= _hurtValue;
                    this.SendProtectEvent(1, -changeValue);
                    if (_hurtValue > 0)
                    {
                        changeValue = _hurtValue;
                        _hurtValue = this.ResistProtectImpl(_hurtValue, this.AllProtectBuffList, 2);
                        changeValue -= _hurtValue;
                        this.SendProtectEvent(2, -changeValue);
                    }
                }
                changeValue = num2 - _hurtValue;
                this.SendHurtAbsorbEvent(_hurt.atker, changeValue);
                this.StatProtectValue(ref _hurt, changeValue);
                if (this.limiteMaxHpHurt.bValid)
                {
                    int num3 = (this.buffHolder.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue * this.limiteMaxHpHurt.hurtRate) / 0x2710;
                    if (_hurtValue > num3)
                    {
                        _hurtValue = num3;
                    }
                }
                _hurtValue = this.ResistDeadDamage(ref _hurt, _hurtValue);
            }
            return _hurtValue;
        }

        private int ResistDeadDamage(ref HurtDataInfo _hurt, int _hurtValue)
        {
            BuffSkill inBuff = null;
            ResDT_SkillFunc outSkillFunc = null;
            if ((this.buffHolder != null) && (this.buffHolder.actor != null))
            {
                ActorRoot actor = this.buffHolder.actor;
                for (int i = 0; i < actor.BuffHolderComp.SpawnedBuffList.Count; i++)
                {
                    inBuff = actor.BuffHolderComp.SpawnedBuffList[i];
                    if ((inBuff != null) && inBuff.FindSkillFunc(0x36, out outSkillFunc))
                    {
                        int inSkillCombineId = inBuff.GetSkillFuncParam(0x36, 0, false);
                        if (inBuff.GetSkillFuncParam(0x36, 1, false) == 0)
                        {
                            if (actor.ValueComponent.actorHp <= _hurtValue)
                            {
                                SkillUseParam inParam = new SkillUseParam();
                                inParam.SetOriginator(_hurt.atker);
                                actor.SkillControl.SpawnBuff(actor.SelfPtr, ref inParam, inSkillCombineId, true);
                                this.buffHolder.RemoveBuff(inBuff);
                                DefaultGameEventParam prm = new DefaultGameEventParam(this.buffHolder.actorPtr, _hurt.atker);
                                Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorImmuneDeadHurt, ref prm);
                                _hurtValue = 0;
                            }
                        }
                        else
                        {
                            SkillUseParam param3 = new SkillUseParam();
                            param3.SetOriginator(_hurt.atker);
                            actor.SkillControl.SpawnBuff(actor.SelfPtr, ref param3, inSkillCombineId, true);
                            this.buffHolder.RemoveBuff(inBuff);
                            _hurtValue = 0;
                        }
                    }
                    if ((((_hurt.atkSlot == SkillSlotType.SLOT_SKILL_0) && (inBuff != null)) && (inBuff.FindSkillFunc(0x43, out outSkillFunc) && (_hurt.atker != 0))) && (_hurt.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
                    {
                        int num4 = inBuff.GetSkillFuncParam(0x43, 0, false);
                        int num5 = inBuff.GetSkillFuncParam(0x43, 4, false);
                        switch (num4)
                        {
                            case 1:
                                _hurtValue = (_hurtValue * (0x2710 - num5)) / 0x2710;
                                break;

                            case 0:
                                _hurtValue -= num5;
                                break;
                        }
                    }
                }
            }
            return _hurtValue;
        }

        private int ResistProtectImpl(int _hurtValue, List<PoolObjHandle<BuffSkill>> _inList, int _index)
        {
            if (_inList.Count != 0)
            {
                PoolObjHandle<BuffSkill>[] handleArray = _inList.ToArray();
                for (int i = 0; i < handleArray.Length; i++)
                {
                    BuffSkill handle = handleArray[i].handle;
                    if (handle.CustomParams[_index] > _hurtValue)
                    {
                        handle.CustomParams[_index] -= _hurtValue;
                        return 0;
                    }
                    _hurtValue -= handle.CustomParams[_index];
                    handle.CustomParams[_index] = 0;
                    this.SpawnSkillEffect(handle.CustomParams[3]);
                    this.buffHolder.RemoveBuff(handle);
                    _inList.Remove(handleArray[i]);
                }
            }
            return _hurtValue;
        }

        private void SendHurtAbsorbEvent(PoolObjHandle<ActorRoot> atker, int changeValue)
        {
            if ((changeValue > 0) && (this.protectValue != 0))
            {
                DefaultGameEventParam prm = new DefaultGameEventParam(this.buffHolder.actorPtr, atker);
                Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorHurtAbsorb, ref prm);
            }
        }

        private void SendHurtImmuneEvent(PoolObjHandle<ActorRoot> atker)
        {
            DefaultGameEventParam prm = new DefaultGameEventParam(this.buffHolder.actorPtr, atker);
            Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorImmune, ref prm);
        }

        public void SendProtectEvent(int type, int changeValue)
        {
            if (changeValue != 0)
            {
                this.protectValue += changeValue;
                this.buffHolder.actor.ActorControl.OnShieldChange(type, changeValue);
                if (this.protectValue == 0)
                {
                    ActorSkillEventParam param = new ActorSkillEventParam(this.buffHolder.actorPtr, SkillSlotType.SLOT_SKILL_0);
                    Singleton<GameSkillEventSys>.GetInstance().SendEvent<ActorSkillEventParam>(GameSkillEventDef.Event_ProtectDisappear, this.buffHolder.actorPtr, ref param, GameSkillEventChannel.Channel_HostCtrlActor);
                }
            }
        }

        public void SetLimiteMaxHurt(bool _bOpen, int _value)
        {
            this.limiteMaxHpHurt.bValid = _bOpen;
            this.limiteMaxHpHurt.hurtRate = _value;
        }

        private void SpawnSkillEffect(int _skillCombineID)
        {
            if (this.buffHolder.actorPtr != 0)
            {
                SkillUseParam inParam = new SkillUseParam();
                inParam.SetOriginator(this.buffHolder.actorPtr);
                this.buffHolder.actorPtr.handle.SkillControl.SpawnBuff(this.buffHolder.actorPtr, ref inParam, _skillCombineID, true);
            }
        }

        private void StatProtectValue(ref HurtDataInfo hurt, int iChangeValue)
        {
            if (iChangeValue > 0)
            {
                this.m_uiProtectTotalValue += (uint) iChangeValue;
                if (hurt.atker != 0)
                {
                    if (hurt.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        this.m_uiProtectValueFromHero += (uint) iChangeValue;
                    }
                    if ((hurt.atker.handle.BuffHolderComp != null) && (hurt.atker.handle.BuffHolderComp.protectRule != null))
                    {
                        hurt.atker.handle.BuffHolderComp.protectRule.BePortectedTotalValue += (uint) iChangeValue;
                        if ((hurt.target != 0) && (hurt.target.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero))
                        {
                            if (hurt.hurtType == HurtTypeDef.PhysHurt)
                            {
                                hurt.atker.handle.BuffHolderComp.protectRule.BeProtectedValueToHeroPhys += (uint) iChangeValue;
                            }
                            else if (hurt.hurtType == HurtTypeDef.MagicHurt)
                            {
                                hurt.atker.handle.BuffHolderComp.protectRule.BeProtectedValueToHeroMagic += (uint) iChangeValue;
                            }
                            else if (hurt.hurtType == HurtTypeDef.RealHurt)
                            {
                                hurt.atker.handle.BuffHolderComp.protectRule.BeProtectedValueToHeroReal += (uint) iChangeValue;
                            }
                        }
                    }
                }
            }
        }

        public uint BePortectedTotalValue
        {
            get
            {
                return this.m_uiBeProtectedTotalValue;
            }
            set
            {
                this.m_uiBeProtectedTotalValue = value;
            }
        }

        public uint BeProtectedValueToHeroMagic
        {
            get
            {
                return this.m_uiBeProtectedValueToHeroMagic;
            }
            set
            {
                this.m_uiBeProtectedValueToHeroMagic = value;
            }
        }

        public uint BeProtectedValueToHeroPhys
        {
            get
            {
                return this.m_uiBeProtectedValueToHeroPhys;
            }
            set
            {
                this.m_uiBeProtectedValueToHeroPhys = value;
            }
        }

        public uint BeProtectedValueToHeroReal
        {
            get
            {
                return this.m_uiBeProtectedValueToHeroReal;
            }
            set
            {
                this.m_uiBeProtectedValueToHeroReal = value;
            }
        }

        public uint ProtectValueFromHero
        {
            get
            {
                return this.m_uiProtectValueFromHero;
            }
        }
    }
}

