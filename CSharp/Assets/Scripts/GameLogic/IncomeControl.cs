namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using ResData;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class IncomeControl
    {
        private bool bPvpMode;
        private bool bSoulGrow;
        private List<stActorIncome> m_actorIncomes = new List<stActorIncome>();
        private ListView<ActorRoot> m_allocIncomeRelatedHeros = new ListView<ActorRoot>();
        private ListView<ResSoulLvlUpInfo> m_allocSoulLvlList = new ListView<ResSoulLvlUpInfo>();
        public List<int> m_compensateRateList = new List<int>();
        private ResIncomeAllocRule[][] m_incomeAllocRules = new ResIncomeAllocRule[3][];
        public bool m_isExpCompensate;
        public ushort m_originalGoldCoinInBattle;
        private const int MAX_INCOM_RULE = 6;
        private const int MAX_INCOME_TYPE_CNT = 3;

        private void AllocIncome(PoolObjHandle<ActorRoot> target, PoolObjHandle<ActorRoot> attacker, ResDT_IncomeAttackRule allocIncomeRule, enIncomeType incomeType, int[] soulExpIncomeChangeRate, int computerChangeRate)
        {
            uint actorKilledIncome = this.GetActorKilledIncome(target, incomeType);
            this.m_actorIncomes.Clear();
            for (int i = 0; i < allocIncomeRule.astIncomeMemberArr.Length; i++)
            {
                this.m_allocIncomeRelatedHeros.Clear();
                RES_INCOME_MEMBER_CHOOSE_TYPE wMemberChooseType = (RES_INCOME_MEMBER_CHOOSE_TYPE) allocIncomeRule.astIncomeMemberArr[i].wMemberChooseType;
                this.GetAllocIncomeRelatedHeros(ref this.m_allocIncomeRelatedHeros, target, attacker, wMemberChooseType, allocIncomeRule, i);
                this.AllocIncomeToHeros(ref this.m_actorIncomes, this.m_allocIncomeRelatedHeros, target, attacker, incomeType, actorKilledIncome, allocIncomeRule, i, soulExpIncomeChangeRate, computerChangeRate);
                this.m_allocIncomeRelatedHeros.Clear();
            }
            for (int j = 0; j < this.m_actorIncomes.Count; j++)
            {
                stActorIncome income = this.m_actorIncomes[j];
                if (income.m_actorRoot != null)
                {
                    stActorIncome income2 = this.m_actorIncomes[j];
                    if (income2.m_actorRoot.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        stActorIncome income3 = this.m_actorIncomes[j];
                        uint incomeValue = income3.m_incomeValue;
                        if ((incomeType == enIncomeType.Soul) && this.m_isExpCompensate)
                        {
                            stActorIncome income4 = this.m_actorIncomes[j];
                            stActorIncome income5 = this.m_actorIncomes[j];
                            incomeValue = this.GetCompensateExp(income4.m_actorRoot, income5.m_incomeValue);
                        }
                        if (incomeType == enIncomeType.Soul)
                        {
                            stActorIncome income6 = this.m_actorIncomes[j];
                            income6.m_actorRoot.ValueComponent.AddSoulExp((int) incomeValue, false, AddSoulType.Income);
                        }
                        else if (incomeType == enIncomeType.GoldCoinInBattle)
                        {
                            stActorIncome income7 = this.m_actorIncomes[j];
                            bool isLastHit = object.ReferenceEquals(attacker.handle, income7.m_actorRoot);
                            stActorIncome income8 = this.m_actorIncomes[j];
                            income8.m_actorRoot.ValueComponent.ChangeGoldCoinInBattle((int) incomeValue, true, true, target.handle.gameObject.transform.position, isLastHit);
                        }
                        if (incomeType == enIncomeType.Soul)
                        {
                            stActorIncome income9 = this.m_actorIncomes[j];
                            if (income9.m_actorRoot.IsHostCamp() && (incomeValue > 0))
                            {
                                stActorIncome income10 = this.m_actorIncomes[j];
                                Singleton<CBattleSystem>.GetInstance().CreateBattleFloatDigit((int) incomeValue, DIGIT_TYPE.ReceiveSpirit, income10.m_actorRoot.gameObject.transform.position);
                            }
                        }
                    }
                }
            }
            this.m_actorIncomes.Clear();
        }

        private void AllocIncomeToHeros(ref List<stActorIncome> actorIncomes, ListView<ActorRoot> relatedHeros, PoolObjHandle<ActorRoot> target, PoolObjHandle<ActorRoot> attacker, enIncomeType incomeType, uint incomeValue, ResDT_IncomeAttackRule allocIncomeRule, int paramIndex, int[] soulExpIncomeChangeRate, int computerChangeRate)
        {
            int count = relatedHeros.Count;
            if (count > 0)
            {
                ResDT_AllocRuleParam param = allocIncomeRule.astIncomeMemberArr[paramIndex];
                int index = ((count <= 5) ? count : 5) - 1;
                int num3 = soulExpIncomeChangeRate[index];
                incomeValue = (uint) ((((uint) ((incomeValue * num3) / ((ulong) 0x2710L))) * param.iIncomeRate) / ((ulong) 0x2710L));
                uint num4 = 0;
                if (param.wDivideType == 1)
                {
                    num4 = (uint) (((ulong) incomeValue) / ((long) count));
                }
                else if (param.wDivideType == 2)
                {
                    num4 = incomeValue;
                }
                for (int i = 0; i < count; i++)
                {
                    uint num6 = num4;
                    if (incomeType == enIncomeType.Soul)
                    {
                        if (relatedHeros[i] == attacker.handle)
                        {
                            num6 = (uint) ((num4 * (0x2710 + relatedHeros[i].BuffHolderComp.GetSoulExpAddRate(target))) / ((ulong) 0x2710L));
                        }
                        Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(relatedHeros[i].TheActorMeta.PlayerId);
                        Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                        if (((!Singleton<LobbyLogic>.instance.inMultiGame && Singleton<BattleLogic>.instance.GetCurLvelContext().IsMobaMode()) && ((player != null) && player.Computer)) && ((hostPlayer != null) && (player.PlayerCamp != hostPlayer.PlayerCamp)))
                        {
                            num6 = (uint) ((num4 * computerChangeRate) / ((ulong) 0x2710L));
                        }
                    }
                    else if ((incomeType == enIncomeType.GoldCoinInBattle) && (relatedHeros[i] == attacker.handle))
                    {
                        num6 = (uint) ((num4 * (0x2710 + relatedHeros[i].BuffHolderComp.GetCoinAddRate(target))) / ((ulong) 0x2710L));
                    }
                    this.PutActorToIncomeList(ref actorIncomes, relatedHeros[i], incomeType, num6);
                    if (incomeType == enIncomeType.Soul)
                    {
                        AddSoulExpEventParam prm = new AddSoulExpEventParam(target, attacker, (int) num6);
                        Singleton<GameEventSys>.instance.SendEvent<AddSoulExpEventParam>(GameEventDef.Event_AddExpValue, ref prm);
                    }
                    else if (incomeType == enIncomeType.GoldCoinInBattle)
                    {
                    }
                }
            }
        }

        public void ClearAllocSoulLvlList()
        {
            this.m_allocSoulLvlList.Clear();
        }

        private uint GetActorKilledIncome(PoolObjHandle<ActorRoot> target, enIncomeType incomeType)
        {
            if (target != 0)
            {
                if (target.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                {
                    return this.GetHeroKilledIncome((HeroWrapper) target.handle.ActorControl, incomeType);
                }
                if (incomeType == enIncomeType.Soul)
                {
                    return Singleton<BattleLogic>.GetInstance().dynamicProperty.GetDynamicSoulExp(target.handle.TheStaticData.TheBaseAttribute.DynamicProperty, target.handle.TheStaticData.TheBaseAttribute.SoulExpGained);
                }
                if (incomeType == enIncomeType.GoldCoinInBattle)
                {
                    return Singleton<BattleLogic>.GetInstance().dynamicProperty.GetDynamicGoldCoinInBattle(target.handle.TheStaticData.TheBaseAttribute.DynamicProperty, target.handle.TheStaticData.TheBaseAttribute.GoldCoinInBattleGained, target.handle.TheStaticData.TheBaseAttribute.GoldCoinInBattleGainedFloatRange);
                }
            }
            return 0;
        }

        private void GetAllocIncomeRelatedHeros(ref ListView<ActorRoot> relatedHeros, PoolObjHandle<ActorRoot> target, PoolObjHandle<ActorRoot> attacker, RES_INCOME_MEMBER_CHOOSE_TYPE chooseType, ResDT_IncomeAttackRule allocIncomeRule, int paramIndex)
        {
            switch (chooseType)
            {
                case RES_INCOME_MEMBER_CHOOSE_TYPE.RES_INCOME_MEMBER_CAMP:
                {
                    List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
                    int count = heroActors.Count;
                    for (int i = 0; i < count; i++)
                    {
                        PoolObjHandle<ActorRoot> handle3 = heroActors[i];
                        if (handle3.handle.TheActorMeta.ActorCamp == attacker.handle.TheActorMeta.ActorCamp)
                        {
                            PoolObjHandle<ActorRoot> handle4 = heroActors[i];
                            if (handle4.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                            {
                                PoolObjHandle<ActorRoot> handle5 = heroActors[i];
                                if (!handle5.handle.ActorControl.IsDeadState || (allocIncomeRule.astIncomeMemberArr[paramIndex].bDeadAddIncome > 0))
                                {
                                    PoolObjHandle<ActorRoot> handle6 = heroActors[i];
                                    relatedHeros.Add(handle6.handle);
                                }
                            }
                        }
                    }
                    break;
                }
                case RES_INCOME_MEMBER_CHOOSE_TYPE.RES_INCOME_MEMBER_RANGE:
                {
                    if ((attacker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (!attacker.handle.ActorControl.IsDeadState || (allocIncomeRule.astIncomeMemberArr[paramIndex].bDeadAddIncome > 0)))
                    {
                        relatedHeros.Add(attacker.handle);
                    }
                    List<PoolObjHandle<ActorRoot>> list2 = Singleton<GameObjMgr>.instance.HeroActors;
                    int num3 = list2.Count;
                    for (int j = 0; j < num3; j++)
                    {
                        if (list2[j] != attacker)
                        {
                            PoolObjHandle<ActorRoot> handle7 = list2[j];
                            if (handle7.handle.TheActorMeta.ActorCamp == attacker.handle.TheActorMeta.ActorCamp)
                            {
                                PoolObjHandle<ActorRoot> handle8 = list2[j];
                                if ((handle8.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && this.IsActorInRange(list2[j], target, allocIncomeRule.astIncomeMemberArr[paramIndex].iRangeRadius))
                                {
                                    PoolObjHandle<ActorRoot> handle9 = list2[j];
                                    if (!handle9.handle.ActorControl.IsDeadState || (allocIncomeRule.astIncomeMemberArr[paramIndex].bDeadAddIncome > 0))
                                    {
                                        PoolObjHandle<ActorRoot> handle10 = list2[j];
                                        relatedHeros.Add(handle10.handle);
                                    }
                                }
                            }
                        }
                    }
                    break;
                }
                case RES_INCOME_MEMBER_CHOOSE_TYPE.RES_INCOME_MEMBER_LAST_KILL:
                    if ((attacker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (!attacker.handle.ActorControl.IsDeadState || (allocIncomeRule.astIncomeMemberArr[paramIndex].bDeadAddIncome > 0)))
                    {
                        relatedHeros.Add(attacker.handle);
                    }
                    break;

                case RES_INCOME_MEMBER_CHOOSE_TYPE.RES_INCOME_MEMBER_ASSIST:
                {
                    HashSet<uint>.Enumerator enumerator = BattleLogic.GetAssistSet(target, attacker, true).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(enumerator.Current);
                        if ((actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (!actor.handle.ActorControl.IsDeadState || (allocIncomeRule.astIncomeMemberArr[paramIndex].bDeadAddIncome > 0)))
                        {
                            relatedHeros.Add(actor.handle);
                        }
                    }
                    break;
                }
                case RES_INCOME_MEMBER_CHOOSE_TYPE.RES_INCOME_MEMBER_ALL_KILL:
                {
                    if ((attacker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (!attacker.handle.ActorControl.IsDeadState || (allocIncomeRule.astIncomeMemberArr[paramIndex].bDeadAddIncome > 0)))
                    {
                        relatedHeros.Add(attacker.handle);
                    }
                    HashSet<uint>.Enumerator enumerator2 = BattleLogic.GetAssistSet(target, attacker, true).GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        PoolObjHandle<ActorRoot> handle2 = Singleton<GameObjMgr>.GetInstance().GetActor(enumerator2.Current);
                        if (((handle2.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && !relatedHeros.Contains(handle2.handle)) && (!handle2.handle.ActorControl.IsDeadState || (allocIncomeRule.astIncomeMemberArr[paramIndex].bDeadAddIncome > 0)))
                        {
                            relatedHeros.Add(handle2.handle);
                        }
                    }
                    break;
                }
            }
        }

        private uint GetCompensateExp(ActorRoot actorRoot, uint exp)
        {
            if (this.m_isExpCompensate && (this.m_compensateRateList.Count != 0))
            {
                int num2 = Singleton<GameObjMgr>.GetInstance().GetHeroMaxLevel() - actorRoot.ValueComponent.actorSoulLevel;
                int num3 = 0;
                if ((num2 >= 0) && (num2 < this.m_compensateRateList.Count))
                {
                    num3 = this.m_compensateRateList[num2];
                }
                else if (num2 >= this.m_compensateRateList.Count)
                {
                    num3 = this.m_compensateRateList[this.m_compensateRateList.Count - 1];
                }
                exp = (uint) ((exp * (0x2710 + num3)) / ((ulong) 0x2710L));
            }
            return exp;
        }

        public uint GetHeroKilledIncome(HeroWrapper heroWrapper, enIncomeType incomeType)
        {
            if (heroWrapper == null)
            {
                return 0;
            }
            int contiKillNum = 0;
            ResSoulAddition dataByKey = null;
            if (heroWrapper.ContiKillNum > 0)
            {
                if (heroWrapper.ContiKillNum >= 7)
                {
                    contiKillNum = 7;
                }
                else
                {
                    contiKillNum = heroWrapper.ContiKillNum;
                }
                dataByKey = GameDataMgr.soulAdditionDatabin.GetDataByKey((long) contiKillNum);
            }
            else
            {
                if (heroWrapper.ContiDeadNum >= 7)
                {
                    contiKillNum = -7;
                }
                else
                {
                    contiKillNum = -heroWrapper.ContiDeadNum;
                }
                dataByKey = GameDataMgr.soulAdditionDatabin.GetDataByKey((long) contiKillNum);
            }
            int iExpAddRate = 0x2710;
            if (dataByKey != null)
            {
                if (incomeType == enIncomeType.Soul)
                {
                    iExpAddRate = dataByKey.iExpAddRate;
                }
                else if (incomeType == enIncomeType.GoldCoinInBattle)
                {
                    iExpAddRate = dataByKey.iGoldCoinInBattleAddRate;
                }
            }
            int actorSoulLevel = heroWrapper.actor.ValueComponent.actorSoulLevel;
            ResSoulLvlUpInfo info = this.QuerySoulLvlUpInfo((uint) actorSoulLevel);
            uint dwKilledExp = 0;
            uint dwExtraKillExp = 0;
            if (info != null)
            {
                if (incomeType == enIncomeType.Soul)
                {
                    dwKilledExp = info.dwKilledExp;
                    dwExtraKillExp = info.dwExtraKillExp;
                }
                else if (incomeType == enIncomeType.GoldCoinInBattle)
                {
                    dwKilledExp = info.wKillGoldCoinInBattle;
                    dwExtraKillExp = info.wExtraKillGoldCoinInBattle;
                }
            }
            uint num8 = (uint) ((dwKilledExp * iExpAddRate) / ((ulong) 0x2710L));
            if ((Singleton<BattleStatistic>.instance.GetCampScore(COM_PLAYERCAMP.COM_PLAYERCAMP_1) + Singleton<BattleStatistic>.instance.GetCampScore(COM_PLAYERCAMP.COM_PLAYERCAMP_2)) == 1)
            {
                num8 += dwExtraKillExp;
            }
            return num8;
        }

        private ResDT_IncomeAttackRule GetIncomeAllocRuleByAtker(PoolObjHandle<ActorRoot> attacker, ResIncomeAllocRule incomeRule)
        {
            if ((incomeRule != null) && (attacker != 0))
            {
                RES_INCOME_ATTACKER_TYPE res_income_attacker_type = RES_INCOME_ATTACKER_TYPE.RES_INCOME_ATTACKER_ALL;
                switch (attacker.handle.TheActorMeta.ActorType)
                {
                    case ActorTypeDef.Actor_Type_Hero:
                        res_income_attacker_type = RES_INCOME_ATTACKER_TYPE.RES_INCOME_ATTACKER_HERO;
                        break;

                    case ActorTypeDef.Actor_Type_Monster:
                    {
                        MonsterWrapper wrapper = attacker.handle.AsMonster();
                        if (wrapper != null)
                        {
                            RES_MONSTER_TYPE bMonsterType = (RES_MONSTER_TYPE) wrapper.cfgInfo.bMonsterType;
                            if (bMonsterType != RES_MONSTER_TYPE.RES_MONSTER_TYPE_JUNGLE)
                            {
                                if (bMonsterType == RES_MONSTER_TYPE.RES_MONSTER_TYPE_SOLDIERLINE)
                                {
                                    res_income_attacker_type = RES_INCOME_ATTACKER_TYPE.RES_INCOME_ATTACKER_SOLDIER;
                                }
                                break;
                            }
                            res_income_attacker_type = RES_INCOME_ATTACKER_TYPE.RES_INCOME_ATTACKER_MONSTER;
                        }
                        break;
                    }
                    case ActorTypeDef.Actor_Type_Organ:
                        res_income_attacker_type = RES_INCOME_ATTACKER_TYPE.RES_INCOME_ATTACKER_ORGAN;
                        break;
                }
                for (int i = 0; i < 4; i++)
                {
                    if ((incomeRule.astIncomeRule[i].bAttakerType == 1) || (res_income_attacker_type == ((RES_INCOME_ATTACKER_TYPE) incomeRule.astIncomeRule[i].bAttakerType)))
                    {
                        return incomeRule.astIncomeRule[i];
                    }
                }
            }
            return null;
        }

        public static uint GetLevelIncomeRuleID(SLevelContext _levelContext, IncomeControl inControl)
        {
            inControl.m_isExpCompensate = false;
            inControl.m_originalGoldCoinInBattle = 0;
            inControl.m_compensateRateList.Clear();
            uint soulID = 0;
            soulID = _levelContext.m_soulID;
            if (_levelContext.IsMobaMode())
            {
                inControl.InitExpCompensateInfo(_levelContext.m_isOpenExpCompensate, ref _levelContext.m_expCompensateInfo);
                inControl.m_originalGoldCoinInBattle = _levelContext.m_originalGoldCoinInBattle;
            }
            return soulID;
        }

        public ListView<ResSoulLvlUpInfo> GetSoulLvlUpInfoList()
        {
            return this.m_allocSoulLvlList;
        }

        public void Init(SLevelContext _levelContext)
        {
            uint levelIncomeRuleID = GetLevelIncomeRuleID(_levelContext, this);
            this.InitIncomeRule(levelIncomeRuleID);
            this.bSoulGrow = Singleton<BattleLogic>.GetInstance().m_GameInfo.gameContext.levelContext.IsSoulGrow();
            this.bPvpMode = _levelContext.IsMobaMode();
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
            this.m_actorIncomes.Clear();
            if (this.m_originalGoldCoinInBattle > 0)
            {
                List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
                for (int i = 0; i < heroActors.Count; i++)
                {
                    if (heroActors[i] != 0)
                    {
                        PoolObjHandle<ActorRoot> handle = heroActors[i];
                        Vector3 position = new Vector3();
                        handle.handle.ValueComponent.ChangeGoldCoinInBattle(this.m_originalGoldCoinInBattle, true, false, position, false);
                    }
                }
            }
        }

        public void InitExpCompensateInfo(byte bIsOpenExpCompensate, ref ResDT_ExpCompensateInfo[] expCompensateInfo)
        {
            this.m_isExpCompensate = bIsOpenExpCompensate > 0;
            if (this.m_isExpCompensate && (expCompensateInfo != null))
            {
                this.m_compensateRateList.Clear();
                int bLevelDiff = 0;
                int item = 0;
                int num3 = 0;
                for (int i = 0; i < expCompensateInfo.Length; i++)
                {
                    bLevelDiff = expCompensateInfo[i].bLevelDiff;
                    item = (int) expCompensateInfo[i].dwExtraExpRate;
                    if (bLevelDiff > 0)
                    {
                        for (int j = this.m_compensateRateList.Count; j < bLevelDiff; j++)
                        {
                            this.m_compensateRateList.Add(num3);
                        }
                        this.m_compensateRateList.Add(item);
                        num3 = item;
                    }
                }
            }
        }

        private void InitIncomeRule(uint incomeRuleID)
        {
            <InitIncomeRule>c__AnonStorey5D storeyd = new <InitIncomeRule>c__AnonStorey5D {
                incomeRuleID = incomeRuleID,
                <>f__this = this
            };
            for (int i = 0; i < 3; i++)
            {
                this.m_incomeAllocRules[i] = new ResIncomeAllocRule[6];
                for (int j = 0; j < 6; j++)
                {
                    this.m_incomeAllocRules[i][j] = null;
                }
            }
            if (storeyd.incomeRuleID != 0)
            {
                GameDataMgr.incomeAllocDatabin.Accept(new Action<ResIncomeAllocRule>(storeyd.<>m__30));
            }
        }

        private bool IsActorInRange(PoolObjHandle<ActorRoot> actor, PoolObjHandle<ActorRoot> referActor, int range)
        {
            if (range == 0)
            {
                return true;
            }
            long num = range * range;
            VInt3 num3 = actor.handle.location - referActor.handle.location;
            return (num3.sqrMagnitudeLong2D < num);
        }

        private void OnActorDead(ref GameDeadEventParam prm)
        {
            PoolObjHandle<ActorRoot> src = prm.src;
            PoolObjHandle<ActorRoot> orignalAtker = prm.orignalAtker;
            if (((src != 0) && (orignalAtker != 0)) && !prm.bImmediateRevive)
            {
                if (this.bSoulGrow)
                {
                    if (((src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (orignalAtker.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)) && src.handle.ActorControl.IsKilledByHero())
                    {
                        orignalAtker = src.handle.ActorControl.LastHeroAtker;
                    }
                    if (orignalAtker != 0)
                    {
                        this.OnActorDeadIncomeSoul(src, orignalAtker);
                        if ((orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) || (src.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero))
                        {
                            this.OnActorDeadIncomeGoldCoinInBattle(src, orignalAtker);
                        }
                    }
                }
                if (!this.bPvpMode)
                {
                    this.OnMonsterDeadGold(ref prm);
                }
            }
        }

        private void OnActorDeadIncomeGoldCoinInBattle(PoolObjHandle<ActorRoot> target, PoolObjHandle<ActorRoot> attacker)
        {
            if ((target != 0) && (attacker != 0))
            {
                bool flag = false;
                if (ActorHelper.IsHostCtrlActor(ref attacker) && ((target.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) || (target.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)))
                {
                    flag = true;
                    if (attacker.handle.EffectControl != null)
                    {
                        attacker.handle.EffectControl.PlayDyingGoldEffect(target);
                    }
                    Singleton<CSoundManager>.GetInstance().PlayBattleSound("Glod_Get", target, target.handle.gameObject);
                }
                ResIncomeAllocRule incomeRule = null;
                switch (target.handle.TheActorMeta.ActorType)
                {
                    case ActorTypeDef.Actor_Type_Hero:
                        incomeRule = this.m_incomeAllocRules[2][3];
                        break;

                    case ActorTypeDef.Actor_Type_Monster:
                    {
                        MonsterWrapper wrapper = target.handle.AsMonster();
                        if (wrapper != null)
                        {
                            RES_MONSTER_TYPE bMonsterType = (RES_MONSTER_TYPE) wrapper.cfgInfo.bMonsterType;
                            if (bMonsterType != RES_MONSTER_TYPE.RES_MONSTER_TYPE_JUNGLE)
                            {
                                if (bMonsterType == RES_MONSTER_TYPE.RES_MONSTER_TYPE_SOLDIERLINE)
                                {
                                    incomeRule = this.m_incomeAllocRules[2][2];
                                }
                                break;
                            }
                            if (((wrapper.cfgInfo.bSoldierType != 7) && (wrapper.cfgInfo.bSoldierType != 8)) && (wrapper.cfgInfo.bSoldierType != 9))
                            {
                                incomeRule = this.m_incomeAllocRules[2][4];
                                break;
                            }
                            incomeRule = this.m_incomeAllocRules[2][5];
                        }
                        break;
                    }
                    case ActorTypeDef.Actor_Type_Organ:
                        incomeRule = this.m_incomeAllocRules[2][1];
                        break;
                }
                if (incomeRule != null)
                {
                    ResDT_IncomeAttackRule incomeAllocRuleByAtker = this.GetIncomeAllocRuleByAtker(attacker, incomeRule);
                    if (incomeAllocRuleByAtker != null)
                    {
                        this.AllocIncome(target, attacker, incomeAllocRuleByAtker, enIncomeType.GoldCoinInBattle, incomeRule.IncomeChangeRate, incomeRule.iComputerChangeRate);
                    }
                }
            }
        }

        private void OnActorDeadIncomeSoul(PoolObjHandle<ActorRoot> target, PoolObjHandle<ActorRoot> attacker)
        {
            if ((target != 0) && (attacker != 0))
            {
                ResIncomeAllocRule incomeRule = null;
                switch (target.handle.TheActorMeta.ActorType)
                {
                    case ActorTypeDef.Actor_Type_Hero:
                        incomeRule = this.m_incomeAllocRules[1][3];
                        break;

                    case ActorTypeDef.Actor_Type_Monster:
                    {
                        MonsterWrapper wrapper = target.handle.AsMonster();
                        if (wrapper != null)
                        {
                            RES_MONSTER_TYPE bMonsterType = (RES_MONSTER_TYPE) wrapper.cfgInfo.bMonsterType;
                            if (bMonsterType != RES_MONSTER_TYPE.RES_MONSTER_TYPE_JUNGLE)
                            {
                                if (bMonsterType == RES_MONSTER_TYPE.RES_MONSTER_TYPE_SOLDIERLINE)
                                {
                                    incomeRule = this.m_incomeAllocRules[1][2];
                                }
                                break;
                            }
                            incomeRule = this.m_incomeAllocRules[1][4];
                        }
                        break;
                    }
                    case ActorTypeDef.Actor_Type_Organ:
                        incomeRule = this.m_incomeAllocRules[1][1];
                        break;
                }
                if (incomeRule != null)
                {
                    ResDT_IncomeAttackRule incomeAllocRuleByAtker = this.GetIncomeAllocRuleByAtker(attacker, incomeRule);
                    if (incomeAllocRuleByAtker != null)
                    {
                        this.AllocIncome(target, attacker, incomeAllocRuleByAtker, enIncomeType.Soul, incomeRule.IncomeChangeRate, incomeRule.iComputerChangeRate);
                    }
                }
            }
        }

        private void OnMonsterDeadGold(ref GameDeadEventParam prm)
        {
            MonsterWrapper wrapper = prm.src.handle.AsMonster();
            PoolObjHandle<ActorRoot> orignalAtker = prm.orignalAtker;
            if ((((wrapper != null) && (wrapper.cfgInfo != null)) && (wrapper.cfgInfo.bMonsterType == 2)) && ((orignalAtker != 0) && (orignalAtker.handle.EffectControl != null)))
            {
                orignalAtker.handle.EffectControl.PlayDyingGoldEffect(prm.src);
                Singleton<CSoundManager>.instance.PlayBattleSound("Glod_Get", prm.src, prm.src.handle.gameObject);
            }
        }

        private void PutActorToIncomeList(ref List<stActorIncome> actorIncomes, ActorRoot actorRoot, enIncomeType incomeType, uint incomeValue)
        {
            stActorIncome item = new stActorIncome();
            for (int i = 0; i < actorIncomes.Count; i++)
            {
                stActorIncome income2 = actorIncomes[i];
                if (income2.m_actorRoot == actorRoot)
                {
                    item.m_actorRoot = actorRoot;
                    item.m_incomeType = incomeType;
                    stActorIncome income3 = actorIncomes[i];
                    item.m_incomeValue = income3.m_incomeValue + incomeValue;
                    actorIncomes[i] = item;
                    return;
                }
            }
            item.m_actorRoot = actorRoot;
            item.m_incomeType = incomeType;
            item.m_incomeValue = incomeValue;
            actorIncomes.Add(item);
        }

        public ResSoulLvlUpInfo QuerySoulLvlUpInfo(uint inLevel)
        {
            int count = this.m_allocSoulLvlList.Count;
            for (int i = 0; i < count; i++)
            {
                ResSoulLvlUpInfo info = this.m_allocSoulLvlList[i];
                if ((info != null) && (info.dwLevel == inLevel))
                {
                    return info;
                }
            }
            return null;
        }

        public void ResetAllocSoulLvlMap()
        {
            this.ClearAllocSoulLvlList();
            SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
            DebugHelper.Assert(curLvelContext != null);
            if ((curLvelContext != null) && Singleton<BattleLogic>.instance.m_GameInfo.gameContext.levelContext.IsSoulGrow())
            {
                uint soulAllocId = curLvelContext.m_soulAllocId;
                int mapID = curLvelContext.m_mapID;
                HashSet<object> dataByKey = null;
                if (soulAllocId > 0)
                {
                    dataByKey = GameDataMgr.soulLvlUpDatabin.GetDataByKey(soulAllocId);
                }
                else
                {
                    dataByKey = GameDataMgr.soulLvlUpDatabin.GetDataByIndex(0);
                }
                if (dataByKey != null)
                {
                    HashSet<object>.Enumerator enumerator = dataByKey.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ResSoulLvlUpInfo current = enumerator.Current as ResSoulLvlUpInfo;
                        if (current != null)
                        {
                            this.m_allocSoulLvlList.Add(current);
                        }
                    }
                }
            }
        }

        public void uninit()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
            this.ClearAllocSoulLvlList();
        }

        [CompilerGenerated]
        private sealed class <InitIncomeRule>c__AnonStorey5D
        {
            internal IncomeControl <>f__this;
            internal uint incomeRuleID;

            internal void <>m__30(ResIncomeAllocRule rule)
            {
                if ((rule != null) && (rule.dwSoulID == this.incomeRuleID))
                {
                    this.<>f__this.m_incomeAllocRules[rule.wIncomeType][rule.wTargetType] = rule;
                }
            }
        }
    }
}

