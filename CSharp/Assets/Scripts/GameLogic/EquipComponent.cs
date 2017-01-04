namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameSystem;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class EquipComponent : LogicComponent
    {
        public const int c_equipGridCount = 6;
        private Dictionary<ushort, uint> m_equipBoughtHistory = new Dictionary<ushort, uint>(0x18);
        private Dictionary<ushort, CEquipBuffInfoGroup> m_equipBuffInfoMap = new Dictionary<ushort, CEquipBuffInfoGroup>();
        private int m_equipCount;
        private stEquipInfo[] m_equipInfos = new stEquipInfo[6];
        private List<CEquipPassiveCdInfo> m_equipPassiveCdList = new List<CEquipPassiveCdInfo>();
        private Dictionary<ushort, CEquipPassiveSkillInfoGroup> m_equipPassiveSkillInfoMap = new Dictionary<ushort, CEquipPassiveSkillInfoGroup>();
        private CExistEquipInfoSet m_existEquipInfoSet = new CExistEquipInfoSet();
        private int m_frame;
        public bool m_hasLeftEquipBoughtArea;
        public int m_iBuyEquipCount;
        public int m_iFastBuyEquipCount;
        public bool m_isInEquipBoughtArea;
        private ListView<CRecommendEquipInfo> m_recommendEquipInfos = new ListView<CRecommendEquipInfo>(6);
        public static uint s_equipEffectSequence;

        public void AddEquip(ushort equipID)
        {
            for (int i = 0; i < 6; i++)
            {
                if ((this.m_equipInfos[i].m_equipID == equipID) && (this.m_equipInfos[i].m_amount < this.m_equipInfos[i].m_maxAmount))
                {
                    this.m_equipInfos[i].m_amount++;
                    this.AddEquipEffect(equipID);
                    this.AddEquipToBuyHistory(equipID);
                    this.m_existEquipInfoSet.Refresh(this.m_equipInfos);
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<uint, stEquipInfo[]>("HeroEquipInBattleChange", base.actor.ObjID, this.m_equipInfos);
                    return;
                }
            }
            if (this.m_equipCount < 6)
            {
                int index = -1;
                for (int j = 0; j < 6; j++)
                {
                    if (this.m_equipInfos[j].m_equipID == 0)
                    {
                        index = j;
                        break;
                    }
                }
                if (index >= 0)
                {
                    this.m_equipInfos[index].m_equipID = equipID;
                    this.m_equipInfos[index].m_amount = 1;
                    this.m_equipInfos[index].m_maxAmount = 1;
                    this.m_equipCount++;
                    this.AddEquipEffect(equipID);
                    this.AddEquipToBuyHistory(equipID);
                    this.m_existEquipInfoSet.Refresh(this.m_equipInfos);
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<uint, stEquipInfo[]>("HeroEquipInBattleChange", base.actor.ObjID, this.m_equipInfos);
                }
            }
        }

        private void AddEquipBuff(ushort equipID, uint buffID, ushort buffGroupID, uint equipBuyPrice, uint sequence)
        {
            CEquipBuffInfoGroup group = null;
            if (this.m_equipBuffInfoMap.ContainsKey(buffGroupID))
            {
                this.m_equipBuffInfoMap.TryGetValue(buffGroupID, out group);
            }
            else
            {
                group = new CEquipBuffInfoGroup(buffGroupID);
                this.m_equipBuffInfoMap.Add(buffGroupID, group);
            }
            group.m_isChanged = true;
            ListView<CEquipBuffInfo> equipBuffInfos = group.m_equipBuffInfos;
            CEquipBuffInfo item = null;
            for (int i = 0; i < equipBuffInfos.Count; i++)
            {
                if (equipBuffInfos[i].m_isNeedRemoved && equipBuffInfos[i].IsEqual(buffID, buffGroupID))
                {
                    item = equipBuffInfos[i];
                    item.m_isNeedRemoved = false;
                    item.m_equipID = equipID;
                    item.m_equipBuyPrice = (CrypticInt32) equipBuyPrice;
                    item.m_sequence = sequence;
                    break;
                }
            }
            if (item == null)
            {
                item = new CEquipBuffInfo(equipID, equipBuyPrice, buffID, buffGroupID, sequence);
                equipBuffInfos.Add(item);
            }
        }

        private void AddEquipEffect(ushort equipID)
        {
            ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipID);
            if (dataByKey != null)
            {
                this.AddEquipPropertyValue(dataByKey);
                for (int i = 0; i < dataByKey.astPassiveSkill.Length; i++)
                {
                    if (dataByKey.astPassiveSkill[i].dwID > 0)
                    {
                        this.AddEquipPassiveSkill(equipID, dataByKey.astPassiveSkill[i].dwID, dataByKey.astPassiveSkill[i].wUniquePassiveGroup, (uint) dataByKey.dwBuyPrice, dataByKey.astPassiveSkill[i].PassiveRmvSkillFuncID, s_equipEffectSequence++);
                    }
                }
                for (int j = 0; j < dataByKey.astEffectCombine.Length; j++)
                {
                    if (dataByKey.astEffectCombine[j].dwID > 0)
                    {
                        this.AddEquipBuff(equipID, dataByKey.astEffectCombine[j].dwID, dataByKey.astEffectCombine[j].wUniquePassiveGroup, (uint) dataByKey.dwBuyPrice, s_equipEffectSequence++);
                    }
                }
            }
        }

        private void AddEquipPassiveCdInfo(uint passiveSkillId, int cd)
        {
            for (int i = 0; i < this.m_equipPassiveCdList.Count; i++)
            {
                if (this.m_equipPassiveCdList[i].m_passiveSkillId == passiveSkillId)
                {
                    this.m_equipPassiveCdList[i].m_passiveCd = cd;
                    return;
                }
            }
            CEquipPassiveCdInfo item = new CEquipPassiveCdInfo(passiveSkillId, cd);
            this.m_equipPassiveCdList.Add(item);
        }

        private void AddEquipPassiveSkill(ushort equipID, uint passiveSkillID, ushort passiveSkillGroupID, uint equipBuyPrice, uint[] passiveSkillRelatedFuncIDs, uint sequence)
        {
            CEquipPassiveSkillInfoGroup group = null;
            if (this.m_equipPassiveSkillInfoMap.ContainsKey(passiveSkillGroupID))
            {
                this.m_equipPassiveSkillInfoMap.TryGetValue(passiveSkillGroupID, out group);
            }
            else
            {
                group = new CEquipPassiveSkillInfoGroup(passiveSkillGroupID);
                this.m_equipPassiveSkillInfoMap.Add(passiveSkillGroupID, group);
            }
            group.m_isChanged = true;
            ListView<CEquipPassiveSkillInfo> equipPassiveSkillInfos = group.m_equipPassiveSkillInfos;
            CEquipPassiveSkillInfo item = null;
            for (int i = 0; i < equipPassiveSkillInfos.Count; i++)
            {
                if (equipPassiveSkillInfos[i].m_isNeedRemoved && equipPassiveSkillInfos[i].IsEqual(passiveSkillID, passiveSkillGroupID, passiveSkillRelatedFuncIDs))
                {
                    item = equipPassiveSkillInfos[i];
                    item.m_isNeedRemoved = false;
                    item.m_equipID = equipID;
                    item.m_equipBuyPrice = (CrypticInt32) equipBuyPrice;
                    item.m_sequence = sequence;
                    break;
                }
            }
            if (item == null)
            {
                item = new CEquipPassiveSkillInfo(equipID, equipBuyPrice, passiveSkillID, passiveSkillGroupID, passiveSkillRelatedFuncIDs, sequence);
                equipPassiveSkillInfos.Add(item);
            }
        }

        private void AddEquipPropertyValue(ResEquipInBattle resEquipInBattle)
        {
            this.HandleEquipPropertyValue(resEquipInBattle, 1);
        }

        private void AddEquipToBuyHistory(ushort equipID)
        {
            if (!this.m_equipBoughtHistory.ContainsKey(equipID))
            {
                uint num = (uint) (Singleton<FrameSynchr>.instance.LogicFrameTick / ((ulong) 0x3e8L));
                this.m_equipBoughtHistory.Add(equipID, num);
            }
            for (int i = 0; i < this.m_recommendEquipInfos.Count; i++)
            {
                if ((this.m_recommendEquipInfos[i].m_equipID == equipID) && !this.m_recommendEquipInfos[i].m_hasBeenBought)
                {
                    this.m_recommendEquipInfos[i].m_hasBeenBought = true;
                    break;
                }
            }
        }

        private void ClearEquips()
        {
            for (int i = 0; i < 6; i++)
            {
                this.m_equipInfos[i].m_equipID = 0;
                this.m_equipInfos[i].m_amount = 0;
                this.m_equipInfos[i].m_maxAmount = 1;
            }
            this.m_equipCount = 0;
            this.m_frame = 0;
            this.m_equipPassiveSkillInfoMap.Clear();
            this.m_equipBuffInfoMap.Clear();
            this.m_existEquipInfoSet.Clear();
            this.m_recommendEquipInfos.Clear();
            this.m_equipBoughtHistory.Clear();
            this.m_isInEquipBoughtArea = false;
            this.m_hasLeftEquipBoughtArea = false;
            this.m_iFastBuyEquipCount = 0;
            this.m_iBuyEquipCount = 0;
        }

        public override void Deactive()
        {
            this.ClearEquips();
            base.Deactive();
        }

        private void DisableEquipBuff(CEquipBuffInfo equipBuffInfo)
        {
            if (equipBuffInfo.m_isEnabled)
            {
                base.actor.SkillControl.RemoveBuff(base.actorPtr, (int) equipBuffInfo.m_buffID);
                equipBuffInfo.m_isEnabled = false;
            }
        }

        private void DisableEquipPassiveSkill(CEquipPassiveSkillInfo equipPassiveSkillInfo)
        {
            if (equipPassiveSkillInfo.m_isEnabled)
            {
                int talentCDTime = base.actor.SkillControl.talentSystem.GetTalentCDTime((int) equipPassiveSkillInfo.m_passiveSkillID);
                if (talentCDTime > 0)
                {
                    this.AddEquipPassiveCdInfo(equipPassiveSkillInfo.m_passiveSkillID, talentCDTime);
                }
                base.actor.SkillControl.talentSystem.RemoveTalent((int) equipPassiveSkillInfo.m_passiveSkillID);
                if (((base.actor.BuffHolderComp != null) && (equipPassiveSkillInfo.m_passiveSkillRelatedFuncIDs != null)) && (equipPassiveSkillInfo.m_passiveSkillRelatedFuncIDs.Length > 0))
                {
                    for (int i = 0; i < equipPassiveSkillInfo.m_passiveSkillRelatedFuncIDs.Length; i++)
                    {
                        if (equipPassiveSkillInfo.m_passiveSkillRelatedFuncIDs[i] > 0)
                        {
                            base.actor.BuffHolderComp.RemoveBuff((int) equipPassiveSkillInfo.m_passiveSkillRelatedFuncIDs[i]);
                        }
                    }
                }
                equipPassiveSkillInfo.m_isEnabled = false;
            }
        }

        private void EnableEquipBuff(CEquipBuffInfo equipBuffInfo)
        {
            if (!equipBuffInfo.m_isEnabled)
            {
                SkillUseParam inParam = new SkillUseParam();
                inParam.SetOriginator(base.actorPtr);
                base.actor.SkillControl.SpawnBuff(base.actorPtr, ref inParam, (int) equipBuffInfo.m_buffID, false);
                equipBuffInfo.m_isEnabled = true;
            }
        }

        private void EnableEquipPassiveSkill(CEquipPassiveSkillInfo equipPassiveSkillInfo)
        {
            if (!equipPassiveSkillInfo.m_isEnabled)
            {
                int cd = 0;
                if (this.RemoveEquipPassiveCd(equipPassiveSkillInfo.m_passiveSkillID, out cd))
                {
                    base.actor.SkillControl.talentSystem.InitTalent((int) equipPassiveSkillInfo.m_passiveSkillID, cd);
                }
                else
                {
                    base.actor.SkillControl.talentSystem.InitTalent((int) equipPassiveSkillInfo.m_passiveSkillID);
                }
                equipPassiveSkillInfo.m_isEnabled = true;
            }
        }

        public uint GetEquipAmount(ushort equipID)
        {
            uint num = 0;
            for (int i = 0; i < 6; i++)
            {
                if (this.m_equipInfos[i].m_equipID == equipID)
                {
                    num += this.m_equipInfos[i].m_amount;
                }
            }
            return num;
        }

        public Dictionary<ushort, uint> GetEquipBoughtHistory()
        {
            return this.m_equipBoughtHistory;
        }

        public stEquipInfo[] GetEquips()
        {
            return this.m_equipInfos;
        }

        public CExistEquipInfoSet GetExistEquipInfoSet()
        {
            return this.m_existEquipInfoSet;
        }

        public ListView<CRecommendEquipInfo> GetRecommendEquipInfos()
        {
            return this.m_recommendEquipInfos;
        }

        private void HandleEquipPropertyValue(ResEquipInBattle resEquipInBattle, int flag)
        {
            if (resEquipInBattle.dwPhyAttack > 0)
            {
                ValueDataInfo info1 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT];
                info1.addValue += (int) (resEquipInBattle.dwPhyAttack * flag);
            }
            if (resEquipInBattle.dwAttackSpeed > 0)
            {
                ValueDataInfo info2 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_ATKSPDADD];
                info2.addValue += (int) (resEquipInBattle.dwAttackSpeed * flag);
            }
            if (resEquipInBattle.dwCriticalHit > 0)
            {
                ValueDataInfo info3 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_CRITRATE];
                info3.addValue += (int) (resEquipInBattle.dwCriticalHit * flag);
            }
            if (resEquipInBattle.dwHealthSteal > 0)
            {
                ValueDataInfo info4 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYVAMP];
                info4.addValue += (int) (resEquipInBattle.dwHealthSteal * flag);
            }
            if (resEquipInBattle.dwMagicAttack > 0)
            {
                ValueDataInfo info5 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCATKPT];
                info5.addValue += (int) (resEquipInBattle.dwMagicAttack * flag);
            }
            if (resEquipInBattle.dwCDReduce > 0)
            {
                ValueDataInfo info6 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_CDREDUCE];
                info6.addValue += (int) (resEquipInBattle.dwCDReduce * flag);
            }
            if (base.actor.ValueComponent.IsEnergyType(ENERGY_TYPE.Magic))
            {
                if (resEquipInBattle.dwMagicPoint > 0)
                {
                    VFactor factor = new VFactor((long) base.actor.ValueComponent.actorEp, (long) base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue);
                    ValueDataInfo info7 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP];
                    info7.addValue += (int) (resEquipInBattle.dwMagicPoint * flag);
                    if (((flag > 0) && !base.actor.ActorControl.IsDeadState) && (base.actor.ValueComponent.actorEp > 0))
                    {
                        VFactor factor3 = factor * base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue;
                        base.actor.ValueComponent.actorEp = factor3.roundInt;
                    }
                }
                if (resEquipInBattle.dwMagicRecover > 0)
                {
                    ValueDataInfo info8 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_EPRECOVER];
                    info8.addValue += (int) (resEquipInBattle.dwMagicRecover * flag);
                }
            }
            if (resEquipInBattle.dwPhyDefence > 0)
            {
                ValueDataInfo info9 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT];
                info9.addValue += (int) (resEquipInBattle.dwPhyDefence * flag);
            }
            if (resEquipInBattle.dwMagicDefence > 0)
            {
                ValueDataInfo info10 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT];
                info10.addValue += (int) (resEquipInBattle.dwMagicDefence * flag);
            }
            if (resEquipInBattle.dwHealthPoint > 0)
            {
                VFactor factor2 = new VFactor((long) base.actor.ValueComponent.actorHp, (long) base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue);
                ValueDataInfo info11 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP];
                info11.addValue += (int) (resEquipInBattle.dwHealthPoint * flag);
                if ((flag > 0) && !base.actor.ActorControl.IsDeadState)
                {
                    VFactor factor4 = factor2 * base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
                    base.actor.ValueComponent.actorHp = factor4.roundInt;
                }
            }
            if (resEquipInBattle.dwHealthRecover > 0)
            {
                ValueDataInfo info12 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_HPRECOVER];
                info12.addValue += (int) (resEquipInBattle.dwHealthRecover * flag);
            }
            if (resEquipInBattle.dwMoveSpeed > 0)
            {
                ValueDataInfo info13 = base.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MOVESPD];
                info13.addRatio += (int) (resEquipInBattle.dwMoveSpeed * flag);
            }
        }

        public bool HasEquip(ushort equipID, uint amount = 1)
        {
            return (this.GetEquipAmount(equipID) >= amount);
        }

        public bool HasEquipInGroup(ushort group)
        {
            for (int i = 0; i < 6; i++)
            {
                if ((this.m_equipInfos[i].m_equipID != 0) && (GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) this.m_equipInfos[i].m_equipID).wGroup == group))
                {
                    return true;
                }
            }
            return false;
        }

        public override void Init()
        {
            base.Init();
            this.ClearEquips();
            this.InitializeRecommendEquips();
        }

        private void InitializeRecommendEquips()
        {
            DebugHelper.Assert(base.actor != null, "InitializeRecommendEquips with actor==null");
            ActorServerData actorData = new ActorServerData();
            IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.GetInstance().GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
            if (actorDataProvider != null)
            {
                actorDataProvider.GetActorServerData(ref base.actor.TheActorMeta, ref actorData);
            }
            else
            {
                DebugHelper.Assert(false, "Failed Get gameActorDataProvider");
            }
            DebugHelper.Assert(actorData.m_customRecommendEquips != null, "actorServerData.m_customRecommendEquips==null");
            for (int i = 0; i < actorData.m_customRecommendEquips.Length; i++)
            {
                if (actorData.m_customRecommendEquips[i] != 0)
                {
                    ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) actorData.m_customRecommendEquips[i]);
                    if ((dataByKey != null) && (dataByKey.bInvalid <= 0))
                    {
                        this.m_recommendEquipInfos.Add(new CRecommendEquipInfo(actorData.m_customRecommendEquips[i], dataByKey));
                    }
                }
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent<uint>("HeroRecommendEquipInit", base.actor.ObjID);
        }

        public bool IsEquipCanAddedToGrid(ushort equipID, ref CBattleEquipSystem.CEquipBuyPrice equipBuyPrice)
        {
            for (int i = 0; i < 6; i++)
            {
                if ((this.m_equipInfos[i].m_equipID == equipID) && (this.m_equipInfos[i].m_amount < this.m_equipInfos[i].m_maxAmount))
                {
                    return true;
                }
            }
            if (this.m_equipCount < 6)
            {
                return true;
            }
            for (int j = 0; j < 6; j++)
            {
                if ((this.m_equipInfos[j].m_equipID == 0) || (this.m_equipInfos[j].m_amount == 0))
                {
                    return true;
                }
                for (int k = 0; k < equipBuyPrice.m_swappedPreEquipInfoCount; k++)
                {
                    if (((equipBuyPrice.m_swappedPreEquipInfos[k].m_equipID > 0) && (equipBuyPrice.m_swappedPreEquipInfos[k].m_swappedAmount > 0)) && ((this.m_equipInfos[j].m_equipID == equipBuyPrice.m_swappedPreEquipInfos[k].m_equipID) && (this.m_equipInfos[j].m_amount <= equipBuyPrice.m_swappedPreEquipInfos[k].m_swappedAmount)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsPermitedToBuyEquip(bool isInEquipLimitedLevel)
        {
            if (isInEquipLimitedLevel)
            {
                return (base.actor.ActorControl.IsDeadState || (this.m_isInEquipBoughtArea && !this.m_hasLeftEquipBoughtArea));
            }
            return true;
        }

        public override void OnUse()
        {
            base.OnUse();
            for (int i = 0; i < 6; i++)
            {
                this.m_equipInfos[i] = new stEquipInfo();
            }
            this.m_equipCount = 0;
            this.m_frame = 0;
            this.m_equipPassiveSkillInfoMap.Clear();
            this.m_equipBuffInfoMap.Clear();
            this.m_existEquipInfoSet.Clear();
            this.m_recommendEquipInfos.Clear();
            this.m_equipBoughtHistory.Clear();
            this.m_equipPassiveCdList.Clear();
            this.m_isInEquipBoughtArea = false;
            this.m_hasLeftEquipBoughtArea = false;
            this.m_iFastBuyEquipCount = 0;
            this.m_iBuyEquipCount = 0;
        }

        public void RemoveEquip(int equipIndex)
        {
            if (((this.m_equipCount > 0) && (equipIndex >= 0)) && (((equipIndex < 6) && (this.m_equipInfos[equipIndex].m_equipID != 0)) && (this.m_equipInfos[equipIndex].m_amount != 0)))
            {
                this.RemoveEquipEffect(this.m_equipInfos[equipIndex].m_equipID);
                this.m_equipInfos[equipIndex].m_amount--;
                if (this.m_equipInfos[equipIndex].m_amount == 0)
                {
                    this.m_equipInfos[equipIndex].m_equipID = 0;
                    this.m_equipInfos[equipIndex].m_amount = 0;
                    this.m_equipInfos[equipIndex].m_maxAmount = 1;
                    this.m_equipCount--;
                }
                this.m_existEquipInfoSet.Refresh(this.m_equipInfos);
                Singleton<EventRouter>.GetInstance().BroadCastEvent<uint, stEquipInfo[]>("HeroEquipInBattleChange", base.actor.ObjID, this.m_equipInfos);
            }
        }

        public void RemoveEquip(ushort equipID)
        {
            for (int i = 0; i < 6; i++)
            {
                if ((this.m_equipInfos[i].m_equipID == equipID) && (this.m_equipInfos[i].m_amount > 0))
                {
                    this.RemoveEquip(i);
                    return;
                }
            }
        }

        private void RemoveEquipBuff(ushort equipID, uint buffID, ushort buffGroupID)
        {
            CEquipBuffInfoGroup group = null;
            if (this.m_equipBuffInfoMap.ContainsKey(buffGroupID))
            {
                this.m_equipBuffInfoMap.TryGetValue(buffGroupID, out group);
                group.m_isChanged = true;
                ListView<CEquipBuffInfo> equipBuffInfos = group.m_equipBuffInfos;
                for (int i = 0; i < equipBuffInfos.Count; i++)
                {
                    if (((equipBuffInfos[i].m_equipID == equipID) && (equipBuffInfos[i].m_buffID == buffID)) && (equipBuffInfos[i].m_buffGroupID == buffGroupID))
                    {
                        equipBuffInfos[i].m_isNeedRemoved = true;
                        break;
                    }
                }
            }
        }

        private void RemoveEquipEffect(ushort equipID)
        {
            ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint) equipID);
            if (dataByKey != null)
            {
                this.RemoveEquipPropertyValue(dataByKey);
                for (int i = 0; i < dataByKey.astPassiveSkill.Length; i++)
                {
                    if (dataByKey.astPassiveSkill[i].dwID > 0)
                    {
                        this.RemoveEquipPassiveSkill(equipID, dataByKey.astPassiveSkill[i].dwID, dataByKey.astPassiveSkill[i].wUniquePassiveGroup);
                    }
                }
                for (int j = 0; j < dataByKey.astEffectCombine.Length; j++)
                {
                    if (dataByKey.astEffectCombine[j].dwID > 0)
                    {
                        this.RemoveEquipBuff(equipID, dataByKey.astEffectCombine[j].dwID, dataByKey.astEffectCombine[j].wUniquePassiveGroup);
                    }
                }
            }
        }

        private bool RemoveEquipPassiveCd(uint passiveSkillId, out int cd)
        {
            cd = 0;
            for (int i = 0; i < this.m_equipPassiveCdList.Count; i++)
            {
                if (this.m_equipPassiveCdList[i].m_passiveSkillId == passiveSkillId)
                {
                    cd = this.m_equipPassiveCdList[i].m_passiveCd;
                    this.m_equipPassiveCdList.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        private void RemoveEquipPassiveSkill(ushort equipID, uint passiveSkillID, ushort passiveSkillGroupID)
        {
            CEquipPassiveSkillInfoGroup group = null;
            if (this.m_equipPassiveSkillInfoMap.ContainsKey(passiveSkillGroupID))
            {
                this.m_equipPassiveSkillInfoMap.TryGetValue(passiveSkillGroupID, out group);
                group.m_isChanged = true;
                ListView<CEquipPassiveSkillInfo> equipPassiveSkillInfos = group.m_equipPassiveSkillInfos;
                for (int i = 0; i < equipPassiveSkillInfos.Count; i++)
                {
                    if (((equipPassiveSkillInfos[i].m_equipID == equipID) && (equipPassiveSkillInfos[i].m_passiveSkillID == passiveSkillID)) && (equipPassiveSkillInfos[i].m_passiveSkillGroupID == passiveSkillGroupID))
                    {
                        equipPassiveSkillInfos[i].m_isNeedRemoved = true;
                        break;
                    }
                }
            }
        }

        private void RemoveEquipPropertyValue(ResEquipInBattle resEquipInBattle)
        {
            this.HandleEquipPropertyValue(resEquipInBattle, -1);
        }

        public void ResetHasLeftEquipBoughtArea()
        {
            this.m_hasLeftEquipBoughtArea = false;
        }

        public void TryBuyEquipmentIfComputer()
        {
            CBattleEquipSystem battleEquipSystem = Singleton<CBattleSystem>.GetInstance().m_battleEquipSystem;
            if ((battleEquipSystem != null) && base.actor.ActorAgent.IsAutoAI())
            {
                ushort[] quicklyBuyEquipList = battleEquipSystem.GetQuicklyBuyEquipList(ref this.actorPtr);
                for (int i = 0; i < quicklyBuyEquipList.Length; i++)
                {
                    if (quicklyBuyEquipList[i] > 0)
                    {
                        battleEquipSystem.ExecuteBuyEquipFrameCommand(quicklyBuyEquipList[i], ref this.actorPtr);
                        break;
                    }
                }
            }
        }

        public override void Uninit()
        {
            base.Uninit();
            this.ClearEquips();
        }

        private void UpdateEquipBuff(CEquipBuffInfoGroup equipBuffInfoGroup)
        {
            if (equipBuffInfoGroup.m_isChanged)
            {
                ListView<CEquipBuffInfo> equipBuffInfos = equipBuffInfoGroup.m_equipBuffInfos;
                int index = 0;
                while (index < equipBuffInfos.Count)
                {
                    if (equipBuffInfos[index].m_isNeedRemoved)
                    {
                        this.DisableEquipBuff(equipBuffInfos[index]);
                        equipBuffInfos.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
                if (equipBuffInfoGroup.m_groupID == 0)
                {
                    for (int i = 0; i < equipBuffInfos.Count; i++)
                    {
                        this.EnableEquipBuff(equipBuffInfos[i]);
                    }
                }
                else
                {
                    equipBuffInfos.Sort();
                    for (int j = 0; j < equipBuffInfos.Count; j++)
                    {
                        if (j == (equipBuffInfos.Count - 1))
                        {
                            this.EnableEquipBuff(equipBuffInfos[j]);
                        }
                        else
                        {
                            this.DisableEquipBuff(equipBuffInfos[j]);
                        }
                    }
                }
                equipBuffInfoGroup.m_isChanged = false;
            }
        }

        public void UpdateEquipEffect()
        {
            Dictionary<ushort, CEquipPassiveSkillInfoGroup>.Enumerator enumerator = this.m_equipPassiveSkillInfoMap.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<ushort, CEquipPassiveSkillInfoGroup> current = enumerator.Current;
                this.UpdateEquipPassiveSkill(current.Value);
            }
            Dictionary<ushort, CEquipBuffInfoGroup>.Enumerator enumerator2 = this.m_equipBuffInfoMap.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                KeyValuePair<ushort, CEquipBuffInfoGroup> pair2 = enumerator2.Current;
                this.UpdateEquipBuff(pair2.Value);
            }
        }

        private void UpdateEquipPassiveSkill(CEquipPassiveSkillInfoGroup equipPassiveSkillInfoGroup)
        {
            if (equipPassiveSkillInfoGroup.m_isChanged)
            {
                ListView<CEquipPassiveSkillInfo> equipPassiveSkillInfos = equipPassiveSkillInfoGroup.m_equipPassiveSkillInfos;
                int index = 0;
                while (index < equipPassiveSkillInfos.Count)
                {
                    if (equipPassiveSkillInfos[index].m_isNeedRemoved)
                    {
                        this.DisableEquipPassiveSkill(equipPassiveSkillInfos[index]);
                        equipPassiveSkillInfos.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
                if (equipPassiveSkillInfoGroup.m_groupID == 0)
                {
                    for (int i = 0; i < equipPassiveSkillInfos.Count; i++)
                    {
                        this.EnableEquipPassiveSkill(equipPassiveSkillInfos[i]);
                    }
                }
                else
                {
                    equipPassiveSkillInfos.Sort();
                    for (int j = 0; j < equipPassiveSkillInfos.Count; j++)
                    {
                        if (j == (equipPassiveSkillInfos.Count - 1))
                        {
                            this.EnableEquipPassiveSkill(equipPassiveSkillInfos[j]);
                        }
                        else
                        {
                            this.DisableEquipPassiveSkill(equipPassiveSkillInfos[j]);
                        }
                    }
                }
                equipPassiveSkillInfoGroup.m_isChanged = false;
            }
        }

        public override void UpdateLogic(int nDelta)
        {
            int index = 0;
            while (index < this.m_equipPassiveCdList.Count)
            {
                CEquipPassiveCdInfo local1 = this.m_equipPassiveCdList[index];
                local1.m_passiveCd -= nDelta;
                if (this.m_equipPassiveCdList[index].m_passiveCd <= 0)
                {
                    this.m_equipPassiveCdList.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
            this.m_frame++;
            if ((((this.m_frame + ((int) base.actor.ObjID)) % 30) == 0) && this.IsPermitedToBuyEquip(Singleton<CBattleSystem>.GetInstance().m_battleEquipSystem.IsInEquipLimitedLevel()))
            {
                this.TryBuyEquipmentIfComputer();
            }
        }
    }
}

