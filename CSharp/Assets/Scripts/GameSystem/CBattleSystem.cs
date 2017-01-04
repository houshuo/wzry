namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using UnityEngine;

    public class CBattleSystem : Singleton<CBattleSystem>
    {
        private IBattleForm _battleForm;
        private Assets.Scripts.GameSystem.BattleStatView _battleStatView;
        private KillNotify _killNotify;
        private MinimapSys _miniMapSys;
        public float m_AveBattleFPS;
        public CBattleEquipSystem m_battleEquipSystem;
        private CBattleFloatDigitManager m_battleFloatDigitManager;
        public float m_BattleFPSCount;
        private int m_frameCount;
        public float m_MaxBattleFPS;
        public float m_MinBattleFPS = float.MaxValue;
        private Assets.Scripts.GameSystem.TowerHitMgr m_towerHitMgr;
        public Vector2 UI_world_Factor_Big;
        public Vector2 UI_world_Factor_Small;
        public Vector2 world_UI_Factor_Big;
        public Vector2 world_UI_Factor_Small;

        public void BattleStart()
        {
            if (this._battleForm != null)
            {
                this._battleForm.BattleStart();
            }
        }

        public void CloseForm()
        {
            if (this._battleForm != null)
            {
                this._battleForm.CloseForm();
            }
        }

        public void CollectFloatDigitInSingleFrame(PoolObjHandle<ActorRoot> attacker, PoolObjHandle<ActorRoot> target, DIGIT_TYPE digitType, int value)
        {
            if (this.IsFormOpen)
            {
                this.m_battleFloatDigitManager.CollectFloatDigitInSingleFrame(attacker, target, digitType, value);
            }
        }

        public void CreateBattleFloatDigit(int digitValue, DIGIT_TYPE digitType, Vector3 worldPosition)
        {
            if (this.IsFormOpen)
            {
                this.m_battleFloatDigitManager.CreateBattleFloatDigit(digitValue, digitType, ref worldPosition);
            }
        }

        public void CreateBattleFloatDigit(int digitValue, DIGIT_TYPE digitType, Vector3 worldPosition, int animatIndex)
        {
            if (this.IsFormOpen)
            {
                this.m_battleFloatDigitManager.CreateBattleFloatDigit(digitValue, digitType, ref worldPosition, animatIndex);
            }
        }

        public void CreateOtherFloatText(enOtherFloatTextContent otherFloatTextContent, Vector3 worldPosition, params string[] args)
        {
            if (this.IsFormOpen)
            {
                this.m_battleFloatDigitManager.CreateOtherFloatText(otherFloatTextContent, ref worldPosition, args);
            }
        }

        public void CreateRestrictFloatText(RESTRICT_TYPE restrictType, Vector3 worldPosition)
        {
            if (this.IsFormOpen)
            {
                this.m_battleFloatDigitManager.CreateRestrictFloatText(restrictType, ref worldPosition);
            }
        }

        public void CreateSpecifiedFloatText(uint floatTextID, Vector3 worldPosition)
        {
            if (this.IsFormOpen)
            {
                this.m_battleFloatDigitManager.CreateSpecifiedFloatText(floatTextID, ref worldPosition);
            }
        }

        public override void Init()
        {
            this.m_battleFloatDigitManager = new CBattleFloatDigitManager();
            this.m_battleEquipSystem = new CBattleEquipSystem();
        }

        public void LateUpdate()
        {
            if (this.IsFormOpen)
            {
                this.m_battleFloatDigitManager.LateUpdate();
                if (this._battleStatView != null)
                {
                    this._battleStatView.LateUpdate();
                }
            }
            if (this._battleForm != null)
            {
                this._battleForm.LateUpdate();
            }
        }

        public CUIFormScript LoadForm(FormType formType)
        {
            if (formType == FormType.Fight)
            {
                return Singleton<CUIManager>.GetInstance().OpenForm(Assets.Scripts.GameSystem.FightForm.s_battleUIForm, false, true);
            }
            if (formType == FormType.Watch)
            {
                return Singleton<CUIManager>.GetInstance().OpenForm(Assets.Scripts.GameSystem.WatchForm.s_watchUIForm, false, true);
            }
            return null;
        }

        private void OnActorGoldCoinInBattleChanged(PoolObjHandle<ActorRoot> actor, int changeValue, int currentValue, bool isIncome)
        {
            if ((Singleton<BattleLogic>.GetInstance().m_GameInfo.gameContext.levelContext.IsMobaMode() && ((actor != 0) && ActorHelper.IsHostCtrlActor(ref actor))) && (this.m_battleEquipSystem != null))
            {
                this.m_battleEquipSystem.OnActorGoldChange(changeValue, currentValue);
            }
        }

        private void OnBattleEquipBagItemSelect(CUIEvent uiEvent)
        {
            if (this.m_battleEquipSystem != null)
            {
                this.m_battleEquipSystem.OnEquipBagItemSelect(uiEvent);
            }
        }

        private void OnBattleEquipBuy(CUIEvent uiEvent)
        {
            if (this.m_battleEquipSystem != null)
            {
                this.m_battleEquipSystem.OnEquipBuyBtnClick(uiEvent);
            }
        }

        private void OnBattleEquipFormClose(CUIEvent uiEvent)
        {
            if (this.m_battleEquipSystem != null)
            {
                this.m_battleEquipSystem.OnEquipFormClose(uiEvent);
            }
        }

        private void OnBattleEquipFormOpen(CUIEvent uiEvent)
        {
            if (this.m_battleEquipSystem != null)
            {
                this.m_battleEquipSystem.OnEquipFormOpen(uiEvent);
            }
        }

        private void OnBattleEquipQuicklyBuy(CUIEvent uiEvent)
        {
            if (this.m_battleEquipSystem != null)
            {
                this.m_battleEquipSystem.OnBattleEquipQuicklyBuy(uiEvent);
            }
        }

        private void OnBattleEquipSale(CUIEvent uiEvent)
        {
            if (this.m_battleEquipSystem != null)
            {
                this.m_battleEquipSystem.OnEquipSaleBtnClick(uiEvent);
            }
        }

        private void OnBattleEquipTypeListSelect(CUIEvent uiEvent)
        {
            if (this.m_battleEquipSystem != null)
            {
                this.m_battleEquipSystem.OnEquipTypeListSelect(uiEvent);
            }
        }

        private void OnBattleEuipItemSelect(CUIEvent uiEvent)
        {
            if (this.m_battleEquipSystem != null)
            {
                this.m_battleEquipSystem.OnEquipItemSelect(uiEvent);
            }
        }

        private void OnFloatTextAnimEnd(CUIEvent uiEvent)
        {
            this.m_battleFloatDigitManager.ClearBattleFloatText(uiEvent.m_srcWidgetScript as CUIAnimatorScript);
        }

        public void OnFormClosed()
        {
            this.UnregisterEvents();
            this.m_battleFloatDigitManager.ClearAllBattleFloatText();
            this.m_battleEquipSystem.Clear();
            if (this._miniMapSys != null)
            {
                this._miniMapSys.Clear();
                this._miniMapSys = null;
            }
            if (this._killNotify != null)
            {
                this._killNotify.Clear();
                this._killNotify = null;
            }
            if (this.m_towerHitMgr != null)
            {
                this.m_towerHitMgr.Clear();
                this.m_towerHitMgr = null;
            }
            if (this._battleStatView != null)
            {
                this._battleStatView.Clear();
                this._battleStatView = null;
            }
            this._battleForm = null;
        }

        private void OnPlayerSpawnBuff(ref SpawnBuffEventParam _param)
        {
            if (_param.src != 0)
            {
                if (_param.showType != 0)
                {
                    this.CreateRestrictFloatText((RESTRICT_TYPE) _param.showType, (Vector3) _param.src.handle.location);
                }
                else if (_param.floatTextID > 0)
                {
                    this.CreateSpecifiedFloatText(_param.floatTextID, (Vector3) _param.src.handle.location);
                }
            }
        }

        private void OnToggleStatView(CUIEvent uiEvent)
        {
            if (this._battleStatView != null)
            {
                this._battleStatView.Toggle();
            }
        }

        public void OpenForm(FormType formType)
        {
            this.m_MaxBattleFPS = 0f;
            this.m_MinBattleFPS = float.MaxValue;
            this.m_AveBattleFPS = 0f;
            this.m_BattleFPSCount = 0f;
            this.m_frameCount = 0;
            if (formType == FormType.Fight)
            {
                this._battleForm = new Assets.Scripts.GameSystem.FightForm();
            }
            else if (formType == FormType.Watch)
            {
                this._battleForm = new Assets.Scripts.GameSystem.WatchForm();
            }
            if ((this._battleForm == null) || !this._battleForm.OpenForm())
            {
                this._battleForm = null;
            }
            else
            {
                Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
                SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
                this.m_battleEquipSystem.Initialize(this.FightFormScript, hostPlayer.Captain, curLvelContext.IsMobaMode(), curLvelContext.m_isBattleEquipLimit);
                this._miniMapSys = new MinimapSys();
                this._miniMapSys.Init(this.FormScript, curLvelContext);
                this._killNotify = new KillNotify();
                this._killNotify.Init(this.FormScript);
                this._killNotify.Hide();
                this.m_towerHitMgr = new Assets.Scripts.GameSystem.TowerHitMgr();
                this.m_towerHitMgr.Init();
                if (curLvelContext.IsMobaMode())
                {
                    this._battleStatView = new Assets.Scripts.GameSystem.BattleStatView();
                    this._battleStatView.Init();
                }
                this.RegisterEvents();
            }
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            preloadTab.AddMesh(CUIParticleSystem.s_particleSkillBtnEffect_Path);
        }

        private void RegisterEvents()
        {
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SpawnBuffEventParam>(GameSkillEventDef.Event_SpawnBuff, new GameSkillEvent<SpawnBuffEventParam>(this.OnPlayerSpawnBuff));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnFloatTextAnimEnd, new CUIEventManager.OnUIEventHandler(this.OnFloatTextAnimEnd));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_Form_Open, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipFormOpen));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_Form_Close, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipFormClose));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_TypeList_Select, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipTypeListSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_Item_Select, new CUIEventManager.OnUIEventHandler(this.OnBattleEuipItemSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_BagItem_Select, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBagItemSelect));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_BuyBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBuy));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_SaleBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipSale));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleEquip_RecommendEquip_Buy, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipQuicklyBuy));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int, bool>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, int, bool>(this.OnActorGoldCoinInBattleChanged));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ToggleStatView, new CUIEventManager.OnUIEventHandler(this.OnToggleStatView));
        }

        public override void UnInit()
        {
        }

        private void UnregisterEvents()
        {
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SpawnBuffEventParam>(GameSkillEventDef.Event_SpawnBuff, new GameSkillEvent<SpawnBuffEventParam>(this.OnPlayerSpawnBuff));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnFloatTextAnimEnd, new CUIEventManager.OnUIEventHandler(this.OnFloatTextAnimEnd));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_Form_Open, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipFormOpen));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_Form_Close, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipFormClose));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_TypeList_Select, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipTypeListSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_Item_Select, new CUIEventManager.OnUIEventHandler(this.OnBattleEuipItemSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_BagItem_Select, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBagItemSelect));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_BuyBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipBuy));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_SaleBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipSale));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleEquip_RecommendEquip_Buy, new CUIEventManager.OnUIEventHandler(this.OnBattleEquipQuicklyBuy));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int, bool>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, int, bool>(this.OnActorGoldCoinInBattleChanged));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ToggleStatView, new CUIEventManager.OnUIEventHandler(this.OnToggleStatView));
        }

        public void Update()
        {
            if (this.IsFormOpen && (this.m_battleEquipSystem != null))
            {
                this.m_battleEquipSystem.Update();
            }
            if (this._battleForm != null)
            {
                this._battleForm.Update();
                if (GameFramework.m_fFps > this.m_MaxBattleFPS)
                {
                    this.m_MaxBattleFPS = GameFramework.m_fFps;
                }
                if (this.m_MinBattleFPS > GameFramework.m_fFps)
                {
                    this.m_MinBattleFPS = GameFramework.m_fFps;
                }
                this.m_frameCount++;
                if (this.m_frameCount >= 50)
                {
                    this.m_BattleFPSCount++;
                    this.m_AveBattleFPS += GameFramework.m_fFps;
                    this.m_frameCount = 0;
                }
            }
        }

        public void UpdateLogic(int delta)
        {
            if (this.IsFormOpen && (this.m_battleEquipSystem != null))
            {
                this.m_battleEquipSystem.UpdateLogic(delta);
            }
            if (this._battleForm != null)
            {
                this._battleForm.UpdateLogic(delta);
            }
        }

        public Assets.Scripts.GameSystem.BattleStatView BattleStatView
        {
            get
            {
                return this._battleStatView;
            }
        }

        public Assets.Scripts.GameSystem.FightForm FightForm
        {
            get
            {
                return (this._battleForm as Assets.Scripts.GameSystem.FightForm);
            }
        }

        public CUIFormScript FightFormScript
        {
            get
            {
                return ((this.FightForm == null) ? null : this.FightForm.FormScript);
            }
        }

        public CUIFormScript FormScript
        {
            get
            {
                return ((this._battleForm == null) ? null : this._battleForm.FormScript);
            }
        }

        public bool IsFormOpen
        {
            get
            {
                return (null != this._battleForm);
            }
        }

        public CUIContainerScript TextHudContainer
        {
            get
            {
                return ((this.FightForm == null) ? null : this.FightForm.TextHudContainer);
            }
        }

        public KillNotify TheKillNotify
        {
            get
            {
                return this._killNotify;
            }
        }

        public MinimapSys TheMinimapSys
        {
            get
            {
                return this._miniMapSys;
            }
        }

        public SignalPanel TheSignalPanel
        {
            get
            {
                return ((this.FightForm == null) ? null : this.FightForm.GetSignalPanel());
            }
        }

        public Assets.Scripts.GameSystem.TowerHitMgr TowerHitMgr
        {
            get
            {
                return this.m_towerHitMgr;
            }
        }

        public Assets.Scripts.GameSystem.WatchForm WatchForm
        {
            get
            {
                return (this._battleForm as Assets.Scripts.GameSystem.WatchForm);
            }
        }

        public CUIFormScript WatchFormScript
        {
            get
            {
                return ((this.WatchForm == null) ? null : this.WatchForm.FormScript);
            }
        }

        public enum FormType
        {
            None,
            Fight,
            Watch
        }
    }
}

