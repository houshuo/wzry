namespace Assets.Scripts.GameLogic
{
    using AGE;
    using Assets.Scripts.Common;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [AddComponentMenu("MMGameTrigger/AreaTrigger_General")]
    public class AreaEventTrigger : FuncRegion, ITrigger
    {
        private PoolObjHandle<ActorRoot>[] _actorTestCache;
        private uint[] _actorTormvCache;
        private bool _coolingDown;
        private int _curCoolDownCount;
        private int _curCoolingTime;
        [NonSerialized, HideInInspector]
        public Dictionary<uint, STriggerContext> _inActors = new Dictionary<uint, STriggerContext>();
        private Dictionary<uint, int> _inActorsCache;
        private int _targetActorCampMask;
        private int _targetActorSubTypeMask;
        private int _targetActorTypeMask;
        private int _testToken;
        private PoolObjHandle<ActorRoot> _thisActor;
        public ActionWhenFull actionWhenFull;
        [FriendlyName("存活时间【毫秒】")]
        public int AliveTicks;
        [FriendlyName("触发完毕后失效")]
        public bool bDeactivateSelf = true;
        [NonSerialized, HideInInspector]
        public bool bDoDeactivating;
        [FriendlyName("只有玩家队长触发")]
        public bool bPlayerCaptain;
        [FriendlyName("轮询式探测")]
        public bool bSimpleUpdate;
        [FriendlyName("玩家团队全进触发")]
        public bool bTriggerByTeam;
        [FriendlyName("容量")]
        public int Capacity = 10;
        [FriendlyName("冷却次数")]
        public int CoolDownCount;
        [FriendlyName("冷却时长")]
        public int CoolDownTime;
        [FriendlyName("启用开始时冷却")]
        public bool CoolingAtStart;
        [FriendlyName("冷却模式")]
        public bool CoolMode;
        [FriendlyName("难度筛选")]
        public int Difficulty;
        [FriendlyName("进入时音效")]
        public string EnterSound;
        [FriendlyName("开始冷却时间")]
        public int FisrtCoolDownTime;
        [FriendlyName("ID")]
        public int ID;
        [FriendlyName("是否忽略死亡单位")]
        public bool IgnoreDeathActor = true;
        [FriendlyName("离开时音效")]
        public string LeaveSound;
        private bool m_bShaped;
        private int m_collidedCnt;
        private GeoPolygon m_collidePolygon;
        private SceneManagement.Process_Bool m_coordHandler;
        private TriggerActionWrapper[] m_internalActList = new TriggerActionWrapper[0];
        private VCollisionShape m_shape;
        private SceneManagement.Coordinate m_shapeCoord = new SceneManagement.Coordinate();
        private int m_triggeredCount;
        private int m_updateTimer;
        [FriendlyName("标记")]
        public int Mark;
        public GameObject[] NextTriggerList = new GameObject[0];
        [FriendlyName("只有非满血或非满蓝才能触发")]
        public bool OnlyEffectNotFullHpOrMpActor;
        [NonSerialized, HideInInspector]
        public TriggerActionWrapper PresetActWrapper;
        [FriendlyName("探测半径")]
        public int Radius = 0x1388;
        public GameObject sourceActor;
        public COM_PLAYERCAMP[] TargetActorCamps;
        [FriendlyName("非战斗状态过滤")]
        public bool TargetActorOutBattle;
        public int[] TargetActorSubTypes;
        public ActorTypeDef[] TargetActorTypes;
        public TriggerActionWrapper[] TriggerActions = new TriggerActionWrapper[0];
        [FriendlyName("触发次数")]
        public int TriggerTimes;
        [FriendlyName("探测频率【毫秒】")]
        public int UpdateInterval;

        private void ActivateNext()
        {
            foreach (GameObject obj2 in this.NextTriggerList)
            {
                if (obj2 != null)
                {
                    obj2.SetActive(true);
                }
            }
        }

        public bool ActorFilter(ref PoolObjHandle<ActorRoot> act)
        {
            if (this.bPlayerCaptain && !this.bTriggerByTeam)
            {
                return (act == Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain);
            }
            ActorRoot handle = act.handle;
            if (((this.OnlyEffectNotFullHpOrMpActor && (handle.ValueComponent != null)) && (handle.ValueComponent.actorHp >= handle.ValueComponent.actorHpTotal)) && ((handle.ValueComponent.IsEnergyType(ENERGY_TYPE.Magic) && (handle.ValueComponent.actorEp >= handle.ValueComponent.actorEpTotal)) || !handle.ValueComponent.IsEnergyType(ENERGY_TYPE.Magic)))
            {
                return false;
            }
            if (this.TargetActorOutBattle)
            {
                ObjWrapper actorControl = handle.ActorControl;
                if ((actorControl != null) && actorControl.IsInBattle)
                {
                    return false;
                }
            }
            return ((((this._targetActorTypeMask == 0) || ((this._targetActorTypeMask & (((int) 1) << handle.TheActorMeta.ActorType)) != 0)) && ((this._targetActorCampMask == 0) || ((this._targetActorCampMask & (((int) 1) << handle.TheActorMeta.ActorCamp)) != 0))) && ((this._targetActorSubTypeMask == 0) || ((this._targetActorSubTypeMask & (((int) 1) << handle.ActorControl.GetActorSubType())) != 0)));
        }

        public static T[] AddElement<T>(T[] elements, T element)
        {
            ListView<T> inList = new ListView<T>(elements);
            inList.Add(element);
            return LinqS.ToArray<T>(inList);
        }

        public static T[] AppendElements<T>(T[] elements, T[] appendElements)
        {
            ListView<T> inList = new ListView<T>(elements);
            if (appendElements != null)
            {
                inList.AddRange(appendElements);
            }
            return LinqS.ToArray<T>(inList);
        }

        private void Awake()
        {
            this._actorTestCache = new PoolObjHandle<ActorRoot>[this.Capacity];
            this._actorTormvCache = new uint[this.Capacity];
            this.m_updateTimer = this.UpdateInterval;
            this._targetActorTypeMask = 0;
            if (this.TargetActorTypes != null)
            {
                for (int i = 0; i < this.TargetActorTypes.Length; i++)
                {
                    this._targetActorTypeMask |= ((int) 1) << this.TargetActorTypes[i];
                }
            }
            this._targetActorCampMask = 0;
            if (this.TargetActorCamps != null)
            {
                for (int j = 0; j < this.TargetActorCamps.Length; j++)
                {
                    this._targetActorCampMask |= ((int) 1) << this.TargetActorCamps[j];
                }
            }
            this._targetActorSubTypeMask = 0;
            if (this.TargetActorSubTypes != null)
            {
                for (int k = 0; k < this.TargetActorSubTypes.Length; k++)
                {
                    this._targetActorSubTypeMask |= ((int) 1) << this.TargetActorSubTypes[k];
                }
            }
            this.BuildTriggerWrapper();
            this.BuildInternalWrappers();
            this._curCoolingTime = !this.CoolingAtStart ? this.CoolDownTime : this.FisrtCoolDownTime;
            this._coolingDown = this.CoolingAtStart;
            this._curCoolDownCount = this.CoolDownCount;
            this.m_coordHandler = new SceneManagement.Process_Bool(this.FilterCoordActor);
        }

        private void BuildInternalWrappers()
        {
            if (this.PresetActWrapper != null)
            {
                this.m_internalActList = AddElement<TriggerActionWrapper>(this.m_internalActList, this.PresetActWrapper);
            }
            if (this.TriggerActions.Length > 0)
            {
                this.m_internalActList = AppendElements<TriggerActionWrapper>(this.m_internalActList, this.TriggerActions);
            }
            foreach (TriggerActionWrapper wrapper in this.m_internalActList)
            {
                if ((wrapper != null) && (wrapper.GetActionInternal() == null))
                {
                    wrapper.Init();
                }
            }
        }

        protected virtual void BuildTriggerWrapper()
        {
        }

        private bool CheckDifficulty()
        {
            return ((this.Difficulty == 0) || (Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_levelDifficulty >= this.Difficulty));
        }

        private void DeactivateSelf()
        {
            base.gameObject.SetActive(false);
        }

        protected virtual DictionaryView<TriggerActionBase, RefParamOperator> DoActorEnter(ref PoolObjHandle<ActorRoot> inActor)
        {
            if (!string.IsNullOrEmpty(this.EnterSound) && (this.bTriggerByTeam || (Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain == inActor)))
            {
                Singleton<CSoundManager>.GetInstance().PostEvent(this.EnterSound, null);
            }
            DebugHelper.Assert((bool) inActor);
            Singleton<TriggerEventSys>.instance.SendEvent(TriggerEventDef.ActorEnter, this, inActor.handle);
            return this.DoActorEnterShared(ref inActor);
        }

        protected DictionaryView<TriggerActionBase, RefParamOperator> DoActorEnterShared(ref PoolObjHandle<ActorRoot> inActor)
        {
            DictionaryView<TriggerActionBase, RefParamOperator> view = new DictionaryView<TriggerActionBase, RefParamOperator>();
            foreach (TriggerActionWrapper wrapper in this.m_internalActList)
            {
                if (wrapper != null)
                {
                    RefParamOperator @operator = wrapper.TriggerEnter(inActor, this.thisActor, this, null);
                    if (@operator != null)
                    {
                        view.Add(wrapper.GetActionInternal(), @operator);
                    }
                }
            }
            return view;
        }

        protected virtual void DoActorLeave(ref PoolObjHandle<ActorRoot> inActor)
        {
            this.DoActorLeaveShared(ref inActor);
            DebugHelper.Assert((bool) inActor, "Actor can't be null");
            Singleton<TriggerEventSys>.instance.SendEvent(TriggerEventDef.ActorLeave, this, inActor.handle);
            if (!string.IsNullOrEmpty(this.LeaveSound) && (Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain == inActor))
            {
                Singleton<CSoundManager>.GetInstance().PostEvent(this.LeaveSound, null);
            }
        }

        protected void DoActorLeaveShared(ref PoolObjHandle<ActorRoot> inActor)
        {
            foreach (TriggerActionWrapper wrapper in this.m_internalActList)
            {
                if (wrapper != null)
                {
                    wrapper.TriggerLeave(inActor, this);
                }
            }
        }

        protected virtual void DoActorUpdate(ref PoolObjHandle<ActorRoot> inActor)
        {
            this.DoActorUpdateShared(ref inActor);
        }

        protected void DoActorUpdateShared(ref PoolObjHandle<ActorRoot> inActor)
        {
            foreach (TriggerActionWrapper wrapper in this.m_internalActList)
            {
                if (wrapper != null)
                {
                    wrapper.TriggerUpdate(inActor, this.thisActor, this);
                }
            }
        }

        public void DoSelfDeactivating()
        {
            this.bDoDeactivating = false;
            int num = 0;
            Dictionary<uint, STriggerContext>.Enumerator enumerator = this._inActors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, STriggerContext> current = enumerator.Current;
                this._actorTormvCache[num++] = current.Key;
            }
            for (int i = 0; i < num; i++)
            {
                uint key = this._actorTormvCache[i];
                STriggerContext context = this._inActors[key];
                PoolObjHandle<ActorRoot> actor = context.actor;
                this.DoActorLeave(ref actor);
                this._inActors.Remove(key);
            }
            this.DeactivateSelf();
            this.ActivateNext();
        }

        private bool FilterCoordActor(ref PoolObjHandle<ActorRoot> actorPtr)
        {
            ActorRoot handle = actorPtr.handle;
            if (((handle.shape != null) && handle.shape.Intersects(this.m_shape)) && this.ActorFilter(ref actorPtr))
            {
                this._actorTestCache[this.m_collidedCnt++] = actorPtr;
                if (this.m_collidedCnt >= this.Capacity)
                {
                    return false;
                }
            }
            return true;
        }

        public List<PoolObjHandle<ActorRoot>> GetActors(Func<PoolObjHandle<ActorRoot>, bool> predicate)
        {
            List<PoolObjHandle<ActorRoot>> list = new List<PoolObjHandle<ActorRoot>>();
            Dictionary<uint, STriggerContext>.Enumerator enumerator = this._inActors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, STriggerContext> current = enumerator.Current;
                PoolObjHandle<ActorRoot> actor = current.Value.actor;
                if ((actor != 0) && ((predicate == null) || predicate(actor)))
                {
                    KeyValuePair<uint, STriggerContext> pair2 = enumerator.Current;
                    list.Add(pair2.Value.actor);
                }
            }
            return list;
        }

        private int GetCollidingActors()
        {
            int collidedCnt = 0;
            this.UpdateCollisionShape();
            if (this.m_shape != null)
            {
                this.m_collidedCnt = 0;
                Singleton<SceneManagement>.GetInstance().ForeachActorsBreak(this.m_shapeCoord, this.m_coordHandler);
                collidedCnt = this.m_collidedCnt;
                this.m_collidedCnt = 0;
                return collidedCnt;
            }
            if (this.m_collidePolygon != null)
            {
                List<PoolObjHandle<ActorRoot>> actorsInPolygon = Singleton<TargetSearcher>.GetInstance().GetActorsInPolygon(this.m_collidePolygon, this);
                if (actorsInPolygon != null)
                {
                    collidedCnt = actorsInPolygon.Count;
                    for (int i = 0; (i < collidedCnt) && (i < this.Capacity); i++)
                    {
                        this._actorTestCache[i] = actorsInPolygon[i];
                    }
                }
                return collidedCnt;
            }
            VInt3 position = (VInt3) base.transform.position;
            return Singleton<TargetSearcher>.GetInstance().GetActorsInCircle(position, this.Radius, this._actorTestCache, this);
        }

        private VCollisionShape GetCollisionShape()
        {
            SCollisionComponent component = base.gameObject.GetComponent<SCollisionComponent>();
            if (component != null)
            {
                return component.CreateShape();
            }
            return VCollisionShape.createFromCollider(base.gameObject);
        }

        public GameObject GetTriggerObj()
        {
            return base.gameObject;
        }

        public bool HasActorInside(PoolObjHandle<ActorRoot> target)
        {
            Dictionary<uint, STriggerContext>.Enumerator enumerator = this._inActors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, STriggerContext> current = enumerator.Current;
                if (current.Value.actor == target)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasActorInside(Func<PoolObjHandle<ActorRoot>, bool> predicate)
        {
            Dictionary<uint, STriggerContext>.Enumerator enumerator = this._inActors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, STriggerContext> current = enumerator.Current;
                PoolObjHandle<ActorRoot> actor = current.Value.actor;
                if ((actor != 0) && predicate(actor))
                {
                    return true;
                }
            }
            return false;
        }

        public void onActorDestroy(ref DefaultGameEventParam prm)
        {
            if (prm.src != 0)
            {
                uint objID = prm.src.handle.ObjID;
                if ((this._inActors != null) && this._inActors.ContainsKey(objID))
                {
                    this.DoActorLeave(ref prm.src);
                    this._inActors.Remove(objID);
                }
                if ((this._inActorsCache != null) && this._inActorsCache.ContainsKey(objID))
                {
                    this._inActorsCache.Remove(objID);
                }
            }
        }

        protected void OnCoolingDown()
        {
            foreach (TriggerActionWrapper wrapper in this.m_internalActList)
            {
                if (wrapper != null)
                {
                    wrapper.OnCoolDown(this);
                }
            }
        }

        private void OnDestroy()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorDestroy, new RefAction<DefaultGameEventParam>(this.onActorDestroy));
            this._inActorsCache = null;
            foreach (TriggerActionWrapper wrapper in this.m_internalActList)
            {
                if (wrapper != null)
                {
                    wrapper.Destroy();
                }
            }
        }

        protected void OnTriggerStart()
        {
            foreach (TriggerActionWrapper wrapper in this.m_internalActList)
            {
                if (wrapper != null)
                {
                    wrapper.OnTriggerStart(this);
                }
            }
        }

        public override void Startup()
        {
            base.Startup();
            this._thisActor = ActorHelper.GetActorRoot(this.sourceActor);
            this.m_collidePolygon = base.gameObject.GetComponent<GeoPolygon>();
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorDestroy, new RefAction<DefaultGameEventParam>(this.onActorDestroy));
            Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorDestroy, new RefAction<DefaultGameEventParam>(this.onActorDestroy));
            if (!this.CoolMode || !this._coolingDown)
            {
                this.OnTriggerStart();
            }
        }

        public override void Stop()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorDestroy, new RefAction<DefaultGameEventParam>(this.onActorDestroy));
            base.Stop();
        }

        private void UpdateCollisionShape()
        {
            if ((this.m_shape == null) && !this.m_bShaped)
            {
                this.m_shape = this.GetCollisionShape();
                if (this.m_shape != null)
                {
                    VInt3 forward = (VInt3) base.transform.forward;
                    this.m_shape.UpdateShape((VInt3) base.transform.position, forward.NormalizeTo(0x3e8));
                    Singleton<SceneManagement>.GetInstance().GetCoord(ref this.m_shapeCoord, this.m_shape);
                }
                this.m_bShaped = true;
            }
        }

        public override void UpdateLogic(int delta)
        {
            if (this.CheckDifficulty())
            {
                if (this.CoolMode)
                {
                    if (this._coolingDown)
                    {
                        if (this._curCoolingTime > 0)
                        {
                            this._curCoolingTime -= delta;
                            if (this._curCoolingTime <= 0)
                            {
                                this._curCoolingTime = this.CoolDownTime;
                                this._coolingDown = false;
                                this.OnTriggerStart();
                                if (this._curCoolDownCount > 0)
                                {
                                    this._curCoolDownCount--;
                                    if (this._curCoolDownCount <= 0)
                                    {
                                        this.DoSelfDeactivating();
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.DoSelfDeactivating();
                        }
                    }
                    else
                    {
                        if (this.bSimpleUpdate)
                        {
                            this.UpdateLogicSimple(delta);
                        }
                        if (this.InActorCount > 0)
                        {
                            Singleton<TriggerEventSys>.instance.SendEvent(TriggerEventDef.ActorInside, this, delta);
                        }
                    }
                }
                else
                {
                    if (this.bSimpleUpdate)
                    {
                        this.UpdateLogicSimple(delta);
                    }
                    else
                    {
                        this.UpdateLogicEnterLeave(delta);
                    }
                    if (this.InActorCount > 0)
                    {
                        Singleton<TriggerEventSys>.instance.SendEvent(TriggerEventDef.ActorInside, this, delta);
                    }
                    if (this.AliveTicks > 0)
                    {
                        this.AliveTicks -= delta;
                        if (this.AliveTicks <= 0)
                        {
                            this.AliveTicks = 0;
                            this.DoSelfDeactivating();
                        }
                    }
                }
            }
        }

        private void UpdateLogicEnterLeave(int delta)
        {
            this._testToken++;
            int collidingActors = this.GetCollidingActors();
            for (int i = 0; (i < collidingActors) && (i < this.Capacity); i++)
            {
                PoolObjHandle<ActorRoot> inActor = this._actorTestCache[i];
                uint objID = inActor.handle.ObjID;
                if (this._inActors.ContainsKey(objID))
                {
                    STriggerContext context = this._inActors[objID];
                    context.token = this._testToken;
                    if (this.UpdateInterval > 0)
                    {
                        context.updateTimer -= delta;
                        if (context.updateTimer <= 0)
                        {
                            context.updateTimer = this.UpdateInterval;
                            this.DoActorUpdate(ref inActor);
                        }
                    }
                    this._inActors[objID] = context;
                    continue;
                }
                if ((this.TriggerTimes <= 0) || (this.m_triggeredCount < this.TriggerTimes))
                {
                    if (((this.InActorCount + 1) >= this.Capacity) && (this.actionWhenFull == ActionWhenFull.Destroy))
                    {
                        this.DoSelfDeactivating();
                        return;
                    }
                    bool flag = false;
                    if (this.bTriggerByTeam)
                    {
                        if (this._inActorsCache == null)
                        {
                            this._inActorsCache = new Dictionary<uint, int>();
                        }
                        if (this._inActorsCache.ContainsKey(objID))
                        {
                            this._inActorsCache[objID] = this._testToken;
                        }
                        else
                        {
                            this._inActorsCache.Add(objID, this._testToken);
                            ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().GetAllHeroes();
                            bool flag2 = true;
                            for (int k = 0; k < allHeroes.Count; k++)
                            {
                                PoolObjHandle<ActorRoot> handle3 = allHeroes[k];
                                if (!this._inActorsCache.ContainsKey(handle3.handle.ObjID))
                                {
                                    flag2 = false;
                                    break;
                                }
                            }
                            if (flag2)
                            {
                                flag = true;
                            }
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        DictionaryView<TriggerActionBase, RefParamOperator> inRefParams = this.DoActorEnter(ref inActor);
                        this._inActors.Add(inActor.handle.ObjID, new STriggerContext(inActor, this._testToken, inRefParams, this.UpdateInterval));
                        if (this.UpdateInterval > 0)
                        {
                            this.DoActorUpdate(ref inActor);
                        }
                        if ((++this.m_triggeredCount >= this.TriggerTimes) && (this.TriggerTimes > 0))
                        {
                            this.bDoDeactivating = this.bDeactivateSelf;
                        }
                    }
                }
            }
            int num5 = 0;
            Dictionary<uint, STriggerContext>.Enumerator enumerator = this._inActors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<uint, STriggerContext> current = enumerator.Current;
                if (current.Value.token != this._testToken)
                {
                    KeyValuePair<uint, STriggerContext> pair2 = enumerator.Current;
                    this._actorTormvCache[num5++] = pair2.Key;
                }
            }
            for (int j = 0; j < num5; j++)
            {
                uint key = this._actorTormvCache[j];
                STriggerContext context4 = this._inActors[key];
                PoolObjHandle<ActorRoot> actor = context4.actor;
                this.DoActorLeave(ref actor);
                this._inActors.Remove(key);
            }
            if (this._inActorsCache != null)
            {
                num5 = 0;
                Dictionary<uint, int>.Enumerator enumerator2 = this._inActorsCache.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    KeyValuePair<uint, int> pair3 = enumerator2.Current;
                    if (pair3.Value != this._testToken)
                    {
                        KeyValuePair<uint, int> pair4 = enumerator2.Current;
                        this._actorTormvCache[num5++] = pair4.Key;
                    }
                }
                for (int m = 0; m < num5; m++)
                {
                    this._inActorsCache.Remove(this._actorTormvCache[m]);
                }
            }
        }

        private void UpdateLogicSimple(int delta)
        {
            if ((this.TriggerTimes <= 0) || (this.m_triggeredCount < this.TriggerTimes))
            {
                this.m_updateTimer -= delta;
                if (this.m_updateTimer <= 0)
                {
                    this.m_updateTimer = this.UpdateInterval;
                    int collidingActors = this.GetCollidingActors();
                    for (int i = 0; (i < collidingActors) && (i < this.Capacity); i++)
                    {
                        PoolObjHandle<ActorRoot> inActor = this._actorTestCache[i];
                        if (!this.IgnoreDeathActor || !inActor.handle.ActorControl.IsDeadState)
                        {
                            this.DoActorUpdate(ref inActor);
                            if (this.CoolMode)
                            {
                                this._coolingDown = true;
                                this.OnCoolingDown();
                                break;
                            }
                        }
                    }
                    if ((++this.m_triggeredCount >= this.TriggerTimes) && (this.TriggerTimes > 0))
                    {
                        this.bDoDeactivating = this.bDeactivateSelf;
                    }
                }
            }
        }

        public int InActorCount
        {
            get
            {
                return this._inActors.Count;
            }
        }

        public PoolObjHandle<ActorRoot> thisActor
        {
            get
            {
                return this._thisActor;
            }
        }

        public enum ActionWhenFull
        {
            DoNothing,
            Destroy
        }

        public enum EActTiming
        {
            Init,
            Enter,
            Leave,
            Update,
            EnterDura
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct STimingAction
        {
            public AreaEventTrigger.EActTiming Timing;
            public string HelperName;
            public int HelperIndex;
            public string ActionName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STriggerContext
        {
            public PoolObjHandle<ActorRoot> actor;
            public int token;
            public DictionaryView<TriggerActionBase, RefParamOperator> refParams;
            public int updateTimer;
            public STriggerContext(PoolObjHandle<ActorRoot> actor, int token, DictionaryView<TriggerActionBase, RefParamOperator> inRefParams, int inUpdateInterval)
            {
                this.actor = actor;
                this.token = token;
                this.refParams = inRefParams;
                this.updateTimer = inUpdateInterval;
            }
        }
    }
}

