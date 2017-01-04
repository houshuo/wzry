namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using ResData;
    using System;
    using System.Collections.Generic;

    public class BufferLogicEffect
    {
        private BuffHolderComponent buffHolder;
        private List<PoolObjHandle<BuffSkill>> extraHurtList = new List<PoolObjHandle<BuffSkill>>();
        private SkillSlotHurt[] skillSlotHurt = new SkillSlotHurt[8];

        public void AddBuff(BuffSkill inBuff)
        {
            ResDT_SkillFunc outSkillFunc = null;
            if ((inBuff != null) && inBuff.FindSkillFunc(0x48, out outSkillFunc))
            {
                this.extraHurtList.Add(new PoolObjHandle<BuffSkill>(inBuff));
            }
        }

        private void AddSkillSlotHurt(SkillSlotType _slotType, int _hurtValue)
        {
            this.skillSlotHurt[(int) _slotType].nextTotalHurt += _hurtValue;
        }

        public void Clear()
        {
            for (int i = 0; i < 8; i++)
            {
                this.skillSlotHurt[i].curTotalHurt = 0;
                this.skillSlotHurt[i].nextTotalHurt = 0;
                this.skillSlotHurt[i].skillUseCount = 0;
                this.skillSlotHurt[i].cdTime = 0;
                this.skillSlotHurt[i].recordTime = 0L;
            }
        }

        public void ClearBuff()
        {
            this.extraHurtList.Clear();
        }

        private int DamageExtraEffect(ref HurtDataInfo _hurt, int _hurtVale, int _hurtRate, int _typeMask, int _typeSubMask)
        {
            int num = 0;
            SkillSlot slot = null;
            BufferLogicEffect logicEffect = null;
            if (!_hurt.atker.handle.SkillControl.TryGetSkillSlot(_hurt.atkSlot, out slot))
            {
                return _hurtVale;
            }
            logicEffect = _hurt.atker.handle.BuffHolderComp.logicEffect;
            num = _hurtVale + logicEffect.GetSkillSlotExtraHurt(_hurt.atkSlot, _hurtRate);
            if (this.buffHolder.CheckTargetSubType(_typeMask, _typeSubMask, _hurt.target))
            {
                this.AddSkillSlotHurt(_hurt.atkSlot, num);
            }
            return num;
        }

        public List<PoolObjHandle<BuffSkill>> GetExtraHurtList()
        {
            return this.extraHurtList;
        }

        private int GetSkillSlotExtraHurt(SkillSlotType _slotType, int _hurtRate)
        {
            return ((this.skillSlotHurt[(int) _slotType].curTotalHurt * _hurtRate) / 0x2710);
        }

        public void Init(BuffHolderComponent _buffHolder)
        {
            this.buffHolder = _buffHolder;
            this.Clear();
        }

        public void InitSkillSlotExtraHurt(SkillSlot _slot)
        {
            int slotType = (int) _slot.SlotType;
            uint skillUseCount = _slot.GetSkillUseCount();
            if (this.extraHurtList.Count >= 1)
            {
                if (this.skillSlotHurt[slotType].skillUseCount >= uint.MaxValue)
                {
                    this.skillSlotHurt[slotType].skillUseCount = 0;
                }
                if ((skillUseCount == (this.skillSlotHurt[slotType].skillUseCount + 1)) && ((Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.skillSlotHurt[slotType].recordTime) <= this.skillSlotHurt[slotType].cdTime))
                {
                    this.skillSlotHurt[slotType].curTotalHurt = this.skillSlotHurt[slotType].nextTotalHurt;
                    this.skillSlotHurt[slotType].nextTotalHurt = 0;
                }
                else
                {
                    this.skillSlotHurt[slotType].curTotalHurt = 0;
                    this.skillSlotHurt[slotType].nextTotalHurt = 0;
                }
            }
        }

        public static int OnDamageExtraEffect(ref HurtDataInfo _hurt, int _hurtValue)
        {
            BuffSkill skill = null;
            SkillSlot slot = null;
            BufferLogicEffect logicEffect = null;
            List<PoolObjHandle<BuffSkill>> extraHurtList = null;
            if ((_hurt.atker != 0) && !_hurt.atker.handle.ActorControl.IsDeadState)
            {
                logicEffect = _hurt.atker.handle.BuffHolderComp.logicEffect;
                extraHurtList = logicEffect.GetExtraHurtList();
                for (int i = 0; i < extraHurtList.Count; i++)
                {
                    if (_hurt.target == 0)
                    {
                        return _hurtValue;
                    }
                    skill = extraHurtList[i];
                    int num = skill.GetSkillFuncParam(0x48, 0, false);
                    if ((_hurt.atkSlot == num) && _hurt.atker.handle.SkillControl.TryGetSkillSlot(_hurt.atkSlot, out slot))
                    {
                        int num2 = skill.GetSkillFuncParam(0x48, 1, false);
                        int num3 = skill.GetSkillFuncParam(0x48, 2, false);
                        int num4 = skill.GetSkillFuncParam(0x48, 3, false);
                        int num5 = skill.GetSkillFuncParam(0x48, 4, false);
                        logicEffect.SetSkillSlotUseTime(_hurt.atkSlot, num5);
                        logicEffect.SetSkillSlotUseCount(_hurt.atkSlot, slot.GetSkillUseCount());
                        return logicEffect.DamageExtraEffect(ref _hurt, _hurtValue, num2, num3, num4);
                    }
                }
            }
            return _hurtValue;
        }

        public void RemoveBuff(ref PoolObjHandle<BuffSkill> inBuff)
        {
            ResDT_SkillFunc outSkillFunc = null;
            if ((inBuff != 0) && inBuff.handle.FindSkillFunc(0x48, out outSkillFunc))
            {
                this.extraHurtList.Remove(inBuff);
            }
        }

        private void SetSkillSlotUseCount(SkillSlotType _slotType, uint _useCount)
        {
            this.skillSlotHurt[(int) _slotType].skillUseCount = _useCount;
        }

        private void SetSkillSlotUseTime(SkillSlotType _slotType, int _cdTime)
        {
            this.skillSlotHurt[(int) _slotType].cdTime = _cdTime;
            this.skillSlotHurt[(int) _slotType].recordTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
        }
    }
}

