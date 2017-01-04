namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameSystem;
    using ResData;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class ValueProperty : LogicComponent
    {
        private CrypticInt32 _nObjCurEp = 1;
        private CrypticInt32 _nObjCurHp = 1;
        private CrypticInt32 _soulExp = 0;
        private CrypticInt32 _soulLevel = 1;
        private CrypticInt32 _soulMaxExp = 0;
        private CrypticInt32 m_goldCoinInBattle = 0;
        private CrypticInt32 m_goldCoinIncomeInBattle = 0;
        private CrypticInt32 m_MaxGoldCoinIncomeInBattle = 0;
        public PropertyHelper mActorValue;
        private int nEpRecoveryTick;
        private int nHpRecoveryTick;
        public ActorValueStatistic ObjValueStatistic;
        private static readonly RangeConfig[] s_SpeedDownRanges = new RangeConfig[0];
        private static readonly RangeConfig[] s_SpeedUpRanges = new RangeConfig[0];

        public event ValueChangeDelegate HpChgEvent;

        public event ValueChangeDelegate SoulLevelChgEvent;

        public void AddSoulExp(int addVal, bool bFloatDigit, AddSoulType type)
        {
            if (Singleton<BattleLogic>.instance.m_GameInfo.gameContext.levelContext.IsSoulGrow())
            {
                this.actorSoulExp += addVal;
                while (this.actorSoulExp >= this.actorSoulMaxExp)
                {
                    this.actorSoulExp -= this.actorSoulMaxExp;
                    int num = this.actorSoulLevel + 1;
                    int maxSoulLvl = GetMaxSoulLvl();
                    if (num > maxSoulLvl)
                    {
                        this.actorSoulLevel = maxSoulLvl;
                        this.actorSoulExp = this.actorSoulMaxExp;
                        break;
                    }
                    this.actorSoulLevel = num;
                    this.ObjValueStatistic.iSoulExpMax = (this.ObjValueStatistic.iSoulExpMax <= addVal) ? addVal : this.ObjValueStatistic.iSoulExpMax;
                }
                if ((bFloatDigit && (addVal > 0)) && (base.actor.Visible && ActorHelper.IsHostCtrlActor(ref this.actorPtr)))
                {
                    Singleton<CBattleSystem>.GetInstance().CreateBattleFloatDigit(addVal, DIGIT_TYPE.ReceiveSpirit, base.actor.gameObject.transform.position);
                }
                Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int, int, int>("HeroSoulExpChange", base.actorPtr, addVal, this.actorSoulExp, this.actorSoulMaxExp);
            }
        }

        public override void Born(ActorRoot owner)
        {
            base.Born(owner);
            this._nObjCurHp = 1;
            this._soulLevel = 1;
            this._soulExp = 0;
            this._soulMaxExp = 0;
            this.m_goldCoinInBattle = 0;
            this.m_goldCoinIncomeInBattle = 0;
        }

        public void ChangeActorEp(int value, int addType)
        {
            if (addType == 0)
            {
                this.actorEp += value;
            }
            else if (addType == 1)
            {
                this.actorEp += (int) (((long) (this.actorEpTotal * value)) / 0x2710L);
            }
        }

        public void ChangeGoldCoinInBattle(int changeValue, bool isIncome, bool floatDigit = false, [Optional] Vector3 position, bool isLastHit = false)
        {
            int goldCoinInBattle = (int) this.m_goldCoinInBattle;
            this.m_goldCoinInBattle += changeValue;
            if ((changeValue > 0) && isIncome)
            {
                this.m_goldCoinIncomeInBattle += changeValue;
                this.m_MaxGoldCoinIncomeInBattle = (this.m_MaxGoldCoinIncomeInBattle <= changeValue) ? changeValue : this.m_MaxGoldCoinIncomeInBattle;
            }
            DebugHelper.Assert(this.m_goldCoinInBattle >= 0, "Wo ri, zhe zenme keneng");
            if (((floatDigit && (changeValue > 0)) && (isIncome && base.actor.Visible)) && ActorHelper.IsHostCtrlActor(ref this.actorPtr))
            {
                if (((position.x == 0f) && (position.y == 0f)) && (position.z == 0f))
                {
                    position = base.actor.gameObject.transform.position;
                }
                Singleton<CBattleSystem>.GetInstance().CreateBattleFloatDigit(changeValue, !isLastHit ? DIGIT_TYPE.ReceiveGoldCoinInBattle : DIGIT_TYPE.ReceiveLastHitGoldCoinInBattle, position);
            }
            Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int, int, bool>("HeroGoldCoinInBattleChange", base.actorPtr, changeValue, (int) this.m_goldCoinInBattle, isIncome);
        }

        private void ClearVariables()
        {
            this._nObjCurHp = 1;
            this._nObjCurEp = 1;
            this.mActorValue = null;
            this.nHpRecoveryTick = 0;
            this.nEpRecoveryTick = 0;
            this._soulLevel = 1;
            this._soulExp = 0;
            this._soulMaxExp = 0;
            this.HpChgEvent = null;
            this.SoulLevelChgEvent = null;
            this.ObjValueStatistic = null;
            this.m_goldCoinInBattle = 0;
            this.m_goldCoinIncomeInBattle = 0;
            this.m_MaxGoldCoinIncomeInBattle = 0;
        }

        public override void Deactive()
        {
            this.ClearVariables();
            base.Deactive();
        }

        public override void Fight()
        {
            base.Fight();
            this.nHpRecoveryTick = 0;
            this.nEpRecoveryTick = 0;
            DebugHelper.Assert(this.mActorValue != null, "mActorValue = null data is error");
            if (this.mActorValue != null)
            {
                VFactor hpRate = this.GetHpRate();
                DebugHelper.Assert(base.actor != null, "actor is null ? impossible...");
                if (base.actor != null)
                {
                    bool bPVPLevel = true;
                    SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                    if (curLvelContext != null)
                    {
                        bPVPLevel = curLvelContext.IsMobaMode();
                    }
                    this.mActorValue.AddSymbolPageAttToProp(ref base.actor.TheActorMeta, bPVPLevel);
                    IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
                    ActorServerData actorData = new ActorServerData();
                    if ((actorDataProvider != null) && actorDataProvider.GetActorServerData(ref base.actor.TheActorMeta, ref actorData))
                    {
                        this.mActorValue.SetSkinProp((uint) base.actor.TheActorMeta.ConfigId, actorData.SkinId, true);
                    }
                }
                this.SetHpByRate(hpRate);
            }
        }

        public void ForceSetSoulLevel(int inNewLevel)
        {
            int maxSoulLvl = GetMaxSoulLvl();
            if (inNewLevel > maxSoulLvl)
            {
                inNewLevel = maxSoulLvl;
            }
            if (inNewLevel < 1)
            {
                inNewLevel = 1;
            }
            this.actorSoulLevel = inNewLevel;
            int num2 = inNewLevel - 1;
            if (num2 >= 1)
            {
                ResSoulLvlUpInfo info = Singleton<BattleLogic>.instance.incomeCtrl.QuerySoulLvlUpInfo((uint) num2);
                if (info != null)
                {
                    this.actorSoulExp = ((int) info.dwExp) + 1;
                }
            }
            else
            {
                this.actorSoulExp = 1;
            }
        }

        public void ForceSoulLevelUp()
        {
            this.ForceSetSoulLevel(this.actorSoulLevel + 1);
        }

        public int GetGoldCoinInBattle()
        {
            return (int) this.m_goldCoinInBattle;
        }

        public int GetGoldCoinIncomeInBattle()
        {
            return (int) this.m_goldCoinIncomeInBattle;
        }

        public VFactor GetHpRate()
        {
            object[] inParameters = new object[] { base.actor.TheStaticData.TheResInfo.Name };
            DebugHelper.Assert(this.actorHpTotal > 0, " {0} GetHpRate actorHpTotal is zero", inParameters);
            if (this.actorHpTotal > 0)
            {
                return new VFactor((long) this.actorHp, (long) this.actorHpTotal);
            }
            return VFactor.one;
        }

        public int GetMaxGoldCoinIncomeInBattle()
        {
            return (int) this.m_MaxGoldCoinIncomeInBattle;
        }

        public static int GetMaxSoulLvl()
        {
            return Singleton<BattleLogic>.instance.incomeCtrl.GetSoulLvlUpInfoList().Count;
        }

        private static int HandleSpeedDown(int baseValue, int totalValue, int orignalSpeed)
        {
            int num = 0;
            bool flag = false;
            for (int i = 0; i < s_SpeedDownRanges.Length; i++)
            {
                RangeConfig config = s_SpeedDownRanges[i];
                int outMin = 0;
                int outMax = 0;
                if (config.Intersect(baseValue, totalValue, orignalSpeed, out outMin, out outMax))
                {
                    flag = true;
                    int num5 = outMax - outMin;
                    num5 *= 100 - config.Attenuation;
                    num5 /= 100;
                    num += num5;
                }
                else if (flag)
                {
                    break;
                }
            }
            return (orignalSpeed - num);
        }

        private static int HandleSpeedUp(int baseValue, int totalValue, int orignalSpeed)
        {
            int num = 0;
            bool flag = false;
            for (int i = 0; i < s_SpeedUpRanges.Length; i++)
            {
                RangeConfig config = s_SpeedUpRanges[i];
                int outMin = 0;
                int outMax = 0;
                if (config.Intersect(baseValue, orignalSpeed, totalValue, out outMin, out outMax))
                {
                    flag = true;
                    int num5 = outMax - outMin;
                    num5 *= 100 - config.Attenuation;
                    num5 /= 100;
                    num += num5;
                }
                else if (flag)
                {
                    break;
                }
            }
            return (orignalSpeed + num);
        }

        public override void Init()
        {
            base.Init();
            if (this.mActorValue == null)
            {
                this.mActorValue = new PropertyHelper();
                this.mActorValue.Init(ref base.actor.TheActorMeta);
            }
            if (this.ObjValueStatistic == null)
            {
                this.ObjValueStatistic = new ActorValueStatistic();
            }
            this.actorHp = 0;
            this.actorEp = 0;
            this.SetHpAndEpToInitialValue(0x2710, 0x2710);
        }

        public bool IsEnergyType(ENERGY_TYPE energyType)
        {
            return (this.mActorValue.EnergyType == energyType);
        }

        public override void OnUse()
        {
            base.OnUse();
            this.ClearVariables();
        }

        public override void Reactive()
        {
            base.Reactive();
            this.Init();
        }

        public void RecoverEp()
        {
            if (!base.actor.ActorControl.IsDeadState)
            {
                this.actorEp = this.actorEpTotal;
            }
        }

        public void RecoverHp()
        {
            int nAddHp = this.actorHpTotal - this.actorHp;
            base.actor.ActorControl.ReviveHp(nAddHp);
        }

        public void SetHpAndEpToInitialValue(int hpPercent = 0x2710, int epPercent = 0x2710)
        {
            this.actorHp = (this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue * hpPercent) / 0x2710;
            if (this.IsEnergyType(ENERGY_TYPE.Magic))
            {
                this.actorEp = (this.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue * epPercent) / 0x2710;
            }
            else if (!this.IsEnergyType(ENERGY_TYPE.Madness))
            {
                this.actorEp = 0;
            }
        }

        public void SetHpByRate(VFactor hpRate)
        {
            DebugHelper.Assert(hpRate.den != 0L, "SetHpByRate hpRate den is zero");
            if (hpRate.den != 0)
            {
                VFactor factor = hpRate * this.actorHpTotal;
                this.actorHp = factor.roundInt;
            }
        }

        public void SetSoulMaxExp()
        {
            ResSoulLvlUpInfo info = Singleton<BattleLogic>.instance.incomeCtrl.QuerySoulLvlUpInfo((uint) this.actorSoulLevel);
            if (info != null)
            {
                this.actorSoulMaxExp = (int) info.dwExp;
            }
        }

        private void UpdateEpRecovery(int nDelta)
        {
            if (base.actor.ActorControl.IsDeadState || base.actor.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_RecoverEnergy))
            {
                this.nEpRecoveryTick = 0;
            }
            else
            {
                this.nEpRecoveryTick += nDelta;
                if (this.nEpRecoveryTick >= this.mActorValue.EpRecFrequency)
                {
                    this.actorEp += this.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_EPRECOVER].totalValue / 5;
                    this.nEpRecoveryTick -= this.mActorValue.EpRecFrequency;
                }
            }
        }

        private void UpdateHpRecovery(int nDelta)
        {
            if (base.actor.ActorControl.IsDeadState)
            {
                this.nHpRecoveryTick = 0;
            }
            else
            {
                this.nHpRecoveryTick += nDelta;
                if (this.nHpRecoveryTick >= 0x1388)
                {
                    this.actorHp += this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_HPRECOVER].totalValue;
                    this.nHpRecoveryTick -= 0x1388;
                }
            }
        }

        public override void UpdateLogic(int nDelta)
        {
            this.UpdateHpRecovery(nDelta);
            if (this.IsEnergyType(ENERGY_TYPE.Magic) || this.IsEnergyType(ENERGY_TYPE.Madness))
            {
                this.UpdateEpRecovery(nDelta);
            }
        }

        public int actorEp
        {
            get
            {
                return (int) this._nObjCurEp;
            }
            set
            {
                int num = (value <= this.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue) ? ((value >= 0) ? value : 0) : this.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue;
                if (this._nObjCurEp != num)
                {
                    this._nObjCurEp = num;
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", base.actorPtr, num, this.actorEpTotal);
                }
                if (num == this.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue)
                {
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyMax", base.actorPtr, num, this.actorEpTotal);
                }
            }
        }

        public int actorEpRecTotal
        {
            get
            {
                return this.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_EPRECOVER].totalValue;
            }
        }

        public int actorEpTotal
        {
            get
            {
                return this.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue;
            }
        }

        public int actorHp
        {
            get
            {
                return (int) this._nObjCurHp;
            }
            set
            {
                int num = (value <= this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue) ? ((value >= 0) ? value : 0) : this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
                if (this._nObjCurHp != num)
                {
                    this._nObjCurHp = num;
                    if (this.HpChgEvent != null)
                    {
                        this.HpChgEvent();
                    }
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", base.actorPtr, num, this.actorHpTotal);
                }
            }
        }

        public int actorHpTotal
        {
            get
            {
                return this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
            }
        }

        public int actorMoveSpeed
        {
            get
            {
                if (this.mActorValue == null)
                {
                    return 0;
                }
                int totalValue = this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MOVESPD].totalValue;
                int baseValue = this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MOVESPD].baseValue;
                int num3 = totalValue - baseValue;
                bool flag = num3 > 0;
                if ((flag && ((s_SpeedUpRanges == null) || (s_SpeedUpRanges.Length == 0))) || (!flag && ((s_SpeedDownRanges == null) || (s_SpeedDownRanges.Length == 0))))
                {
                    return totalValue;
                }
                return (!flag ? HandleSpeedDown(baseValue, totalValue, baseValue) : HandleSpeedUp(baseValue, totalValue, baseValue));
            }
        }

        public int actorSoulExp
        {
            get
            {
                return (int) this._soulExp;
            }
            set
            {
                this._soulExp = value;
            }
        }

        public int actorSoulLevel
        {
            get
            {
                return (int) this._soulLevel;
            }
            set
            {
                if (Singleton<BattleLogic>.instance.m_GameInfo.gameContext.levelContext.IsSoulGrow())
                {
                    VFactor factor = new VFactor((long) this.actorHp, (long) this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue);
                    VFactor factor2 = new VFactor((long) this.actorEp, (long) this.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue);
                    int num = value;
                    bool flag = (num != this._soulLevel) || (value == 1);
                    this._soulLevel = value;
                    this.mActorValue.SoulLevel = this.actorSoulLevel;
                    this.SetSoulMaxExp();
                    VFactor factor3 = factor * this.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
                    this.actorHp = factor3.roundInt;
                    if (this.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue > 0)
                    {
                        VFactor factor4 = factor2 * this.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_MAXEP].totalValue;
                        this.actorEp = factor4.roundInt;
                    }
                    if (this.SoulLevelChgEvent != null)
                    {
                        this.SoulLevelChgEvent();
                    }
                    if (Singleton<BattleLogic>.GetInstance().m_GameInfo.gameContext.levelContext.IsSoulGrow() && flag)
                    {
                        if (base.actor.SkillControl != null)
                        {
                            base.actor.SkillControl.m_iSkillPoint = value - base.actor.SkillControl.GetAllSkillLevel();
                        }
                        Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", base.actorPtr, value);
                    }
                }
            }
        }

        public int actorSoulMaxExp
        {
            get
            {
                return (int) this._soulMaxExp;
            }
            set
            {
                this._soulMaxExp = value;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RangeConfig
        {
            public int MinValue;
            public int MaxValue;
            public int Attenuation;
            public static int Clamp(int value, int min, int max)
            {
                return ((value >= min) ? ((value <= max) ? value : max) : min);
            }

            public bool Intersect(int InBase, int InMin, int InMax, out int OutMin, out int OutMax)
            {
                OutMin = OutMax = 0;
                int min = this.MinValue + InBase;
                int max = this.MaxValue + InBase;
                if (InMax < min)
                {
                    return false;
                }
                if (InMin > max)
                {
                    return false;
                }
                OutMin = Clamp(InMin, min, max);
                OutMax = Clamp(InMax, min, max);
                return true;
            }
        }
    }
}

