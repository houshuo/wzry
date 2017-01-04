namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class WatchForm : IBattleForm
    {
        private CUIListScript _camp1BaseList;
        private CUIListScript _camp1EquipList;
        private GameObject _camp1List;
        private CUIListScript _camp2BaseList;
        private CUIListScript _camp2EquipList;
        private GameObject _camp2List;
        private uint _clickPickHeroID;
        private CUIFormScript _form;
        private HeroInfoHud _heroInfoHud;
        private DictionaryView<uint, HeroInfoItem> _heroWrapDict;
        private GameObject _hideBtn;
        private bool _isBottomFold;
        private bool _isCampFold_1;
        private bool _isCampFold_2;
        private bool _isViewHide;
        private bool _kdaChanged;
        private PoolObjHandle<ActorRoot> _pickedHero;
        private Plane _pickPlane = new Plane(new Vector3(0f, 1f, 0f), 0f);
        private ReplayControl _replayControl;
        private WatchScoreHud _scoreHud;
        private GameObject _showBtn;
        public static string s_watchUIForm = "UGUI/Form/Battle/Form_Watch.prefab";

        public void BattleStart()
        {
            if (this._heroWrapDict == null)
            {
                this._heroWrapDict = new DictionaryView<uint, HeroInfoItem>();
                List<HeroKDA> heroList = new List<HeroKDA>();
                List<HeroKDA> list2 = new List<HeroKDA>();
                DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
                    PlayerKDA rkda = current.Value;
                    if (rkda.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                    {
                        ListView<HeroKDA>.Enumerator enumerator2 = rkda.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            heroList.Add(enumerator2.Current);
                        }
                    }
                    else if (rkda.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                    {
                        ListView<HeroKDA>.Enumerator enumerator3 = rkda.GetEnumerator();
                        while (enumerator3.MoveNext())
                        {
                            list2.Add(enumerator3.Current);
                        }
                    }
                }
                this.InitCampInfoUIList(COM_PLAYERCAMP.COM_PLAYERCAMP_1, heroList, this._camp1BaseList, this._camp1EquipList);
                this.InitCampInfoUIList(COM_PLAYERCAMP.COM_PLAYERCAMP_2, list2, this._camp2BaseList, this._camp2EquipList);
                Player playerByUid = Singleton<GamePlayerCenter>.GetInstance().GetPlayerByUid(Singleton<WatchController>.GetInstance().TargetUID);
                this.PickHero((playerByUid == null) ? Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain.handle.ObjID : playerByUid.Captain.handle.ObjID);
                this.ValidateCampMoney();
                this.RegisterEvents();
                this._isBottomFold = false;
                this._isCampFold_1 = false;
                this.OnClickCampFold_1(null);
                this._isCampFold_2 = false;
                this.OnClickCampFold_2(null);
                this._isViewHide = false;
            }
        }

        public void CloseForm()
        {
            if (null != this._form)
            {
                Singleton<CUIManager>.GetInstance().CloseForm(this._form);
                this._form = null;
            }
        }

        private void DisableAnimator(GameObject node)
        {
            if (node != null)
            {
                Animator component = node.GetComponent<Animator>();
                if (component != null)
                {
                    component.enabled = false;
                }
            }
        }

        private void FocusHeroPicked(COM_PLAYERCAMP listCamp, int listIndex)
        {
            if (this._heroWrapDict != null)
            {
                HeroInfoItem item = null;
                DictionaryView<uint, HeroInfoItem>.Enumerator enumerator = this._heroWrapDict.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<uint, HeroInfoItem> current = enumerator.Current;
                    HeroInfoItem item2 = current.Value;
                    if ((item2.listCamp == listCamp) && (item2.listIndex == listIndex))
                    {
                        item = item2;
                        break;
                    }
                }
                if (item != null)
                {
                    MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(false);
                    MonoSingleton<CameraSystem>.instance.SetFocusActor(item.HeroInfo.actorHero);
                    this._pickedHero = item.HeroInfo.actorHero;
                    this._heroInfoHud.SetPickHero(item.HeroInfo);
                    if (listCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                    {
                        this._camp1BaseList.SelectElement(listIndex, false);
                        this._camp2BaseList.SelectElement(-1, false);
                    }
                    else if (listCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                    {
                        this._camp2BaseList.SelectElement(listIndex, false);
                        this._camp1BaseList.SelectElement(-1, false);
                    }
                    else
                    {
                        this._camp1BaseList.SelectElement(-1, false);
                        this._camp2BaseList.SelectElement(-1, false);
                    }
                }
                else
                {
                    this._pickedHero.Release();
                    this._camp1BaseList.SelectElement(-1, false);
                    this._camp2BaseList.SelectElement(-1, false);
                }
            }
        }

        private void InitCampInfoUIList(COM_PLAYERCAMP listCamp, List<HeroKDA> heroList, CUIListScript baseInfoUIList, CUIListScript equipInfoUIList)
        {
            if (((null != baseInfoUIList) && (heroList != null)) && (heroList.Count != 0))
            {
                baseInfoUIList.SetElementAmount(5);
                equipInfoUIList.SetElementAmount(5);
                for (int i = 0; i < 5; i++)
                {
                    GameObject gameObject = baseInfoUIList.GetElemenet(i).gameObject;
                    GameObject equipInfoItem = equipInfoUIList.GetElemenet(i).gameObject;
                    if (i < heroList.Count)
                    {
                        HeroKDA okda = heroList[i];
                        HeroInfoItem item = new HeroInfoItem(listCamp, i, okda, gameObject, equipInfoItem);
                        this._heroWrapDict.Add(okda.actorHero.handle.ObjID, item);
                    }
                    else
                    {
                        HeroInfoItem.MakeEmpty(gameObject, equipInfoItem);
                    }
                }
            }
        }

        public void LateUpdate()
        {
            if (this._heroWrapDict != null)
            {
                if (!Singleton<WatchController>.GetInstance().IsWatching)
                {
                    this.CloseForm();
                    Singleton<SettlementSystem>.GetInstance().ShowSettlementPanel(true);
                    Singleton<GameBuilder>.GetInstance().EndGame();
                }
                else if ((Singleton<FrameSynchr>.GetInstance().CurFrameNum % 5) == 0)
                {
                    DictionaryView<uint, HeroInfoItem>.Enumerator enumerator = this._heroWrapDict.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<uint, HeroInfoItem> current = enumerator.Current;
                        current.Value.LateUpdate();
                        if (this._kdaChanged)
                        {
                            KeyValuePair<uint, HeroInfoItem> pair2 = enumerator.Current;
                            pair2.Value.ValidateKDA();
                        }
                    }
                    if (this._heroInfoHud != null)
                    {
                        this._heroInfoHud.LateUpdate();
                        if (this._kdaChanged)
                        {
                            this._heroInfoHud.ValidateKDA();
                        }
                    }
                    if (this._scoreHud != null)
                    {
                        this._scoreHud.LateUpdate();
                    }
                    if (this._replayControl != null)
                    {
                        this._replayControl.LateUpdate();
                    }
                    this._kdaChanged = false;
                }
            }
        }

        private void OnBattleEquipChange(uint actorObjectID, stEquipInfo[] equips)
        {
            if (this._heroWrapDict.ContainsKey(actorObjectID))
            {
                this._heroWrapDict[actorObjectID].ValidateEquip();
            }
            if (actorObjectID == this._heroInfoHud.PickedHeroID)
            {
                this._heroInfoHud.ValidateEquip();
            }
        }

        private void OnBattleKDAChange()
        {
            this._kdaChanged = true;
        }

        private void OnBattleMoneyChange(PoolObjHandle<ActorRoot> actor, int changeValue, int currentValue, bool isIncome)
        {
            if (this._heroWrapDict.ContainsKey(actor.handle.ObjID))
            {
                this._heroWrapDict[actor.handle.ObjID].ValidateMoney();
            }
            this.ValidateCampMoney();
            if (actor.handle.ObjID == this._heroInfoHud.PickedHeroID)
            {
                this._heroInfoHud.ValidateMoney();
            }
        }

        public void OnCameraDraging(CUIEvent uiEvent)
        {
            if (MonoSingleton<CameraSystem>.GetInstance().enableLockedCamera)
            {
                MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(true);
                if (this._pickedHero != 0)
                {
                    MonoSingleton<CameraSystem>.instance.MobaCamera.SetAbsoluteLockLocation((Vector3) this._pickedHero.handle.ActorControl.actorLocation);
                }
                this.OnCamerFreed();
            }
            MonoSingleton<CameraSystem>.GetInstance().MoveCamera(-uiEvent.m_pointerEventData.delta.x, -uiEvent.m_pointerEventData.delta.y);
        }

        public void OnCamerFreed()
        {
            this._camp1BaseList.SelectElement(-1, false);
            this._camp2BaseList.SelectElement(-1, false);
        }

        private void OnClickBattleScene(CUIEvent uievent)
        {
            float num;
            Ray ray = Camera.main.ScreenPointToRay((Vector3) uievent.m_pointerEventData.position);
            this._clickPickHeroID = 0;
            if (this._pickPlane.Raycast(ray, out num))
            {
                Vector3 point = ray.GetPoint(num);
                SceneManagement instance = Singleton<SceneManagement>.GetInstance();
                SceneManagement.Coordinate coord = new SceneManagement.Coordinate();
                VInt3 num2 = (VInt3) point;
                instance.GetCoord_Center(ref coord, num2.xz, 0xbb8);
                instance.UpdateDirtyNodes();
                instance.ForeachActorsBreak(coord, new SceneManagement.Process_Bool(this.TrySearchHero));
                if (this._clickPickHeroID > 0)
                {
                    this.PickHero(this._clickPickHeroID);
                }
            }
        }

        private void OnClickBottomFold(CUIEvent evt)
        {
            CUIAnimatorScript componetInChild = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.gameObject, "HeroInfoHud");
            CUIAnimatorScript script2 = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.gameObject, "ReplayControl");
            if ((componetInChild != null) && (script2 != null))
            {
                this._isBottomFold = !this._isBottomFold;
                componetInChild.PlayAnimator(!this._isBottomFold ? "Open" : "Close");
                script2.PlayAnimator(!this._isBottomFold ? "Open" : "Close");
            }
        }

        private void OnClickCampFold_1(CUIEvent evt)
        {
            CUIAnimatorScript componetInChild = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.gameObject, "CampInfoList_1/EquipInfoList");
            if (componetInChild != null)
            {
                this._isCampFold_1 = !this._isCampFold_1;
                componetInChild.PlayAnimator(!this._isCampFold_1 ? "Open" : "Close");
            }
        }

        private void OnClickCampFold_1_End(CUIEvent evt)
        {
            this._form.m_sgameGraphicRaycaster.RefreshGameObject(Utility.FindChild(this._form.gameObject, "CampInfoList_1/EquipInfoList/FoldButton"));
        }

        private void OnClickCampFold_2(CUIEvent evt)
        {
            CUIAnimatorScript componetInChild = Utility.GetComponetInChild<CUIAnimatorScript>(this._form.gameObject, "CampInfoList_2/EquipInfoList");
            if (componetInChild != null)
            {
                this._isCampFold_2 = !this._isCampFold_2;
                componetInChild.PlayAnimator(!this._isCampFold_2 ? "Open" : "Close");
            }
        }

        private void OnClickCampFold_2_End(CUIEvent evt)
        {
            this._form.m_sgameGraphicRaycaster.RefreshGameObject(Utility.FindChild(this._form.gameObject, "CampInfoList_2/EquipInfoList/FoldButton"));
        }

        private void OnClickReplayTalk(CUIEvent evt)
        {
            Singleton<CUIManager>.GetInstance().OpenTips("barrageNotReady", true, 1.5f, null, new object[0]);
        }

        private void OnFormClosed(CUIEvent uiEvt)
        {
            Singleton<CBattleSystem>.GetInstance().OnFormClosed();
            this.UnRegisterEvents();
            if (this._heroWrapDict != null)
            {
                this._heroWrapDict.Clear();
                this._heroWrapDict = null;
            }
            if (this._scoreHud != null)
            {
                this._scoreHud.Clear();
                this._scoreHud = null;
            }
            if (this._heroInfoHud != null)
            {
                this._heroInfoHud.Clear();
                this._heroInfoHud = null;
            }
            if (this._replayControl != null)
            {
                this._replayControl.Clear();
                this._replayControl = null;
            }
            this._camp1List = null;
            this._camp2List = null;
            this._camp1BaseList = null;
            this._camp1EquipList = null;
            this._camp2BaseList = null;
            this._camp2EquipList = null;
            this._hideBtn = null;
            this._showBtn = null;
            this._form = null;
        }

        private void OnHeroEpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
        {
            if (hero.handle.ObjID == this._heroInfoHud.PickedHeroID)
            {
                this._heroInfoHud.ValidateEp();
            }
        }

        private void OnHeroHpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
        {
            if (hero.handle.ObjID == this._heroInfoHud.PickedHeroID)
            {
                this._heroInfoHud.ValidateHp();
            }
        }

        private void OnHeroLevelChange(PoolObjHandle<ActorRoot> hero, int level)
        {
            if (this._heroWrapDict.ContainsKey(hero.handle.ObjID))
            {
                this._heroWrapDict[hero.handle.ObjID].ValidateLevel();
            }
            if (hero.handle.ObjID == this._heroInfoHud.PickedHeroID)
            {
                this._heroInfoHud.ValidateLevel();
            }
        }

        public void OnPickCampList_1(CUIEvent uiEvent)
        {
            this.FocusHeroPicked(COM_PLAYERCAMP.COM_PLAYERCAMP_1, uiEvent.m_srcWidgetIndexInBelongedList);
        }

        public void OnPickCampList_2(CUIEvent uiEvent)
        {
            this.FocusHeroPicked(COM_PLAYERCAMP.COM_PLAYERCAMP_2, uiEvent.m_srcWidgetIndexInBelongedList);
        }

        private void OnQuitConfirm(CUIEvent uiEvent)
        {
            Singleton<WatchController>.GetInstance().Quit();
        }

        private void OnQuitWatch(CUIEvent uiEvent)
        {
            Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("confirmQuitWatch"), enUIEventID.Watch_QuitConfirm, enUIEventID.Watch_QuitCancel, false);
        }

        private void OnSkillCDChanged(ref DefaultSkillEventParam _param)
        {
            if ((this._heroInfoHud != null) && (_param.actor.handle.ObjID == this._heroInfoHud.PickedHeroID))
            {
                this._heroInfoHud.TheSkillHud.ValidateCD(_param.slot);
            }
        }

        private void OnSkillLevelUp(PoolObjHandle<ActorRoot> hero, byte bSlotType, byte bSkillLevel)
        {
            if ((this._heroInfoHud != null) && (hero.handle.ObjID == this._heroInfoHud.PickedHeroID))
            {
                this._heroInfoHud.TheSkillHud.ValidateLevel((SkillSlotType) bSlotType);
            }
        }

        private void OnToggleHide(CUIEvent uiEvent)
        {
            if ((this._form != null) && (this._form.gameObject != null))
            {
                this._isViewHide = !this._isViewHide;
                this._hideBtn.CustomSetActive(!this._isViewHide);
                this._showBtn.CustomSetActive(this._isViewHide);
                this.DisableAnimator(Utility.FindChild(this._camp1List, "EquipInfoList"));
                this._camp1List.CustomSetActive(!this._isViewHide);
                this.DisableAnimator(Utility.FindChild(this._camp2List, "EquipInfoList"));
                this._camp2List.CustomSetActive(!this._isViewHide);
                this.DisableAnimator(this._heroInfoHud.Root);
                this._heroInfoHud.Root.CustomSetActive(!this._isViewHide);
                this.DisableAnimator(this._replayControl.Root);
                this._replayControl.Root.CustomSetActive(!this._isViewHide);
            }
        }

        public bool OpenForm()
        {
            this._form = Singleton<CUIManager>.GetInstance().OpenForm(s_watchUIForm, false, true);
            if (null == this._form)
            {
                return false;
            }
            this._scoreHud = new WatchScoreHud(Utility.FindChild(this._form.gameObject, "ScoreBoard"));
            this._camp1List = Utility.FindChild(this._form.gameObject, "CampInfoList_1");
            this._camp1BaseList = Utility.GetComponetInChild<CUIListScript>(this._camp1List, "BaseInfoList");
            this._camp1EquipList = Utility.GetComponetInChild<CUIListScript>(this._camp1List, "EquipInfoList");
            this._camp2List = Utility.FindChild(this._form.gameObject, "CampInfoList_2");
            this._camp2BaseList = Utility.GetComponetInChild<CUIListScript>(this._camp2List, "BaseInfoList");
            this._camp2EquipList = Utility.GetComponetInChild<CUIListScript>(this._camp2List, "EquipInfoList");
            this._hideBtn = Utility.FindChild(this._form.gameObject, "PanelBtn/HideBtn");
            this._showBtn = Utility.FindChild(this._form.gameObject, "PanelBtn/ShowBtn");
            this._heroInfoHud = new HeroInfoHud(Utility.FindChild(this._form.gameObject, "HeroInfoHud"));
            this._replayControl = new ReplayControl(Utility.FindChild(this._form.gameObject, "ReplayControl"));
            return true;
        }

        private void PickHero(uint heroObjId)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(heroObjId);
            if (actor != 0)
            {
                this.FocusHeroPicked(actor.handle.TheActorMeta.ActorCamp, this._heroWrapDict[heroObjId].listIndex);
            }
        }

        private void RegisterEvents()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_CameraDraging, new CUIEventManager.OnUIEventHandler(this.OnCameraDraging));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_PickCampList_1, new CUIEventManager.OnUIEventHandler(this.OnPickCampList_1));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_PickCampList_2, new CUIEventManager.OnUIEventHandler(this.OnPickCampList_2));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_Quit, new CUIEventManager.OnUIEventHandler(this.OnQuitWatch));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickCampFold_1, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_1));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickCampFold_1_End, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_1_End));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickCampFold_2, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_2));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickCampFold_2_End, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_2_End));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickBottomFold, new CUIEventManager.OnUIEventHandler(this.OnClickBottomFold));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickReplayTalk, new CUIEventManager.OnUIEventHandler(this.OnClickReplayTalk));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_HideView, new CUIEventManager.OnUIEventHandler(this.OnToggleHide));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_QuitConfirm, new CUIEventManager.OnUIEventHandler(this.OnQuitConfirm));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_FormClosed, new CUIEventManager.OnUIEventHandler(this.OnFormClosed));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Click_Scene, new CUIEventManager.OnUIEventHandler(this.OnClickBattleScene));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroLevelChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int, bool>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, int, bool>(this.OnBattleMoneyChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<uint, stEquipInfo[]>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[]>(this.OnBattleEquipChange));
            Singleton<EventRouter>.instance.AddEventHandler(EventID.BATTLE_KDA_CHANGED_BY_ACTOR_DEAD, new System.Action(this.OnBattleKDAChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroEpChange));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, new GameSkillEvent<DefaultSkillEventParam>(this.OnSkillCDChanged));
            Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this.OnSkillLevelUp));
        }

        private bool TrySearchHero(ref PoolObjHandle<ActorRoot> actor)
        {
            if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
            {
                this._clickPickHeroID = actor.handle.ObjID;
                return false;
            }
            return true;
        }

        private void UnRegisterEvents()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_CameraDraging, new CUIEventManager.OnUIEventHandler(this.OnCameraDraging));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_PickCampList_1, new CUIEventManager.OnUIEventHandler(this.OnPickCampList_1));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_PickCampList_2, new CUIEventManager.OnUIEventHandler(this.OnPickCampList_2));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_Quit, new CUIEventManager.OnUIEventHandler(this.OnQuitWatch));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickCampFold_1, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_1));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickCampFold_1_End, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_1_End));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickCampFold_2, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_2));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickCampFold_2_End, new CUIEventManager.OnUIEventHandler(this.OnClickCampFold_2_End));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickBottomFold, new CUIEventManager.OnUIEventHandler(this.OnClickBottomFold));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickReplayTalk, new CUIEventManager.OnUIEventHandler(this.OnClickReplayTalk));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_HideView, new CUIEventManager.OnUIEventHandler(this.OnToggleHide));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_QuitConfirm, new CUIEventManager.OnUIEventHandler(this.OnQuitConfirm));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_FormClosed, new CUIEventManager.OnUIEventHandler(this.OnFormClosed));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Click_Scene, new CUIEventManager.OnUIEventHandler(this.OnClickBattleScene));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroLevelChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int, bool>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, int, bool>(this.OnBattleMoneyChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint, stEquipInfo[]>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[]>(this.OnBattleEquipChange));
            Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BATTLE_KDA_CHANGED_BY_ACTOR_DEAD, new System.Action(this.OnBattleKDAChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroEnergyChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroEpChange));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<DefaultSkillEventParam>(GameSkillEventDef.AllEvent_ChangeSkillCD, new GameSkillEvent<DefaultSkillEventParam>(this.OnSkillCDChanged));
            Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, byte, byte>("HeroSkillLevelUp", new Action<PoolObjHandle<ActorRoot>, byte, byte>(this.OnSkillLevelUp));
        }

        public void Update()
        {
        }

        public void UpdateLogic(int delta)
        {
            if (this._heroInfoHud != null)
            {
                this._heroInfoHud.UpdateLogic(delta);
            }
        }

        private void ValidateCampMoney()
        {
            int num = 0;
            int num2 = 0;
            DictionaryView<uint, HeroInfoItem>.Enumerator enumerator = this._heroWrapDict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, HeroInfoItem> current = enumerator.Current;
                if (current.Value.listCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
                {
                    KeyValuePair<uint, HeroInfoItem> pair2 = enumerator.Current;
                    num += pair2.Value.HeroInfo.TotalCoin;
                }
                else
                {
                    KeyValuePair<uint, HeroInfoItem> pair3 = enumerator.Current;
                    if (pair3.Value.listCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
                    {
                        KeyValuePair<uint, HeroInfoItem> pair4 = enumerator.Current;
                        num2 += pair4.Value.HeroInfo.TotalCoin;
                    }
                }
            }
            this._scoreHud.ValidateMoney(num, num2);
        }

        public CUIFormScript FormScript
        {
            get
            {
                return this._form;
            }
        }

        public bool IsFormShow
        {
            get
            {
                return (null != this._form);
            }
        }
    }
}

