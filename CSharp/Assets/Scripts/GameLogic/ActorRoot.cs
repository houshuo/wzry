namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.DataCenter;
    using Assets.Scripts.GameLogic.GameKernal;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class ActorRoot : PooledClassObject, IUpdateLogic
    {
        private bool _bInCamera;
        private byte _bInitVisibleDelay;
        private bool _bVisible = true;
        [NonSerialized, HideInInspector]
        private VInt3 _forward = VInt3.forward;
        [NonSerialized, HideInInspector]
        private VInt3 _location;
        [NonSerialized, HideInInspector]
        private Quaternion _rotation = Quaternion.identity;
        [NonSerialized, HideInInspector]
        public ObjAgent ActorAgent;
        [NonSerialized, HideInInspector]
        public ObjWrapper ActorControl;
        [NonSerialized, HideInInspector]
        public AnimPlayComponent AnimControl;
        [NonSerialized, HideInInspector]
        public bool AttackOrderReady = true;
        [NonSerialized, HideInInspector]
        private bool bChildUpdate;
        public bool bOneKiller;
        [NonSerialized, HideInInspector]
        public BuffHolderComponent BuffHolderComp;
        public CActorInfo CharInfo;
        [NonSerialized, HideInInspector]
        public DefaultAttackMode DefaultAttackModeControl;
        [NonSerialized, HideInInspector]
        public EffectPlayComponent EffectControl;
        [NonSerialized, HideInInspector]
        public Assets.Scripts.GameLogic.EquipComponent EquipComponent;
        [NonSerialized, HideInInspector]
        public VInt groundY = 0;
        [NonSerialized, HideInInspector]
        public bool hasReachedNavEdge;
        [NonSerialized, HideInInspector]
        public HorizonMarkerBase HorizonMarker;
        [NonSerialized, HideInInspector]
        public HudComponent3D HudControl;
        [NonSerialized, HideInInspector]
        public HurtComponent HurtControl;
        public bool isMovable = true;
        [NonSerialized, HideInInspector]
        public bool isRecycled;
        public bool isRotatable = true;
        [NonSerialized, HideInInspector]
        public LockTargetAttackMode LockTargetAttackModeControl;
        [NonSerialized, HideInInspector]
        public MaterialHurtEffect MatHurtEffect;
        [NonSerialized, HideInInspector]
        public Movement MovementComponent;
        public string name = string.Empty;
        public uint ObjID;
        public ActorConfig ObjLinker;
        private GameObject OriginalActorMesh;
        private Animation OriginalMeshAnim;
        [NonSerialized, HideInInspector]
        public VInt pickFlyY = 0;
        [NonSerialized, HideInInspector]
        public PoolObjHandle<ActorRoot> SelfPtr = new PoolObjHandle<ActorRoot>();
        [NonSerialized, HideInInspector]
        public UpdateShadowPlane ShadowEffect;
        [NonSerialized, HideInInspector]
        public VCollisionShape shape;
        [NonSerialized, HideInInspector]
        public SkillComponent SkillControl;
        [NonSerialized, HideInInspector]
        private List<object> slotList = new List<object>();
        [NonSerialized, HideInInspector]
        public SceneManagement.Node SMNode;
        public ActorMeta TheActorMeta = new ActorMeta();
        public ActorStaticData TheStaticData;
        [NonSerialized, HideInInspector]
        public ValueProperty ValueComponent;

        public MonsterWrapper AsMonster()
        {
            if (this.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
            {
                return (MonsterWrapper) this.ActorControl;
            }
            return null;
        }

        public OrganWrapper AsOrgan()
        {
            if (this.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
            {
                return (OrganWrapper) this.ActorControl;
            }
            return null;
        }

        private void AttachSMNode()
        {
            if (this.TheActorMeta.ActorType != ActorTypeDef.Invalid)
            {
                DebugHelper.Assert(this.SMNode == null);
                this.SMNode = ClassObjPool<SceneManagement.Node>.Get();
                this.SMNode.owner = this.SelfPtr;
                this.SMNode.Attach();
            }
        }

        public bool CanAttack(ActorRoot target)
        {
            return (((!target.ObjLinker.Invincible && !target.ActorControl.IsDeadState) && (target.AttackOrderReady && !this.IsSelfCamp(target))) && !target.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneNegative));
        }

        public T CreateActorComponent<T>(ActorRoot actor) where T: MonoBehaviour, IActorComponent
        {
            T component = this.gameObject.GetComponent<T>();
            if (component == null)
            {
                component = this.gameObject.AddComponent<T>();
            }
            component.Born(actor);
            return component;
        }

        public ActorRootSlot CreateActorRootSlot(PoolObjHandle<ActorRoot> _child, VInt3 _trans)
        {
            ActorRootSlot item = new ActorRootSlot(_child, _trans);
            this.slotList.Add(item);
            this.bChildUpdate = true;
            return item;
        }

        public T CreateLogicComponent<T>(ActorRoot actor) where T: LogicComponent, new()
        {
            T local = ClassObjPool<T>.Get();
            local.Born(actor);
            return local;
        }

        public void CreateLogicWrapper()
        {
            switch (this.TheActorMeta.ActorType)
            {
                case ActorTypeDef.Actor_Type_Hero:
                    this.ActorControl = ClassObjPool<HeroWrapper>.Get();
                    break;

                case ActorTypeDef.Actor_Type_Monster:
                    this.ActorControl = ClassObjPool<MonsterWrapper>.Get();
                    break;

                case ActorTypeDef.Actor_Type_Organ:
                    this.ActorControl = ClassObjPool<OrganWrapper>.Get();
                    break;

                case ActorTypeDef.Actor_Type_EYE:
                    this.ActorControl = ClassObjPool<EyeWrapper>.Get();
                    break;

                case ActorTypeDef.Actor_Type_Bullet:
                    this.ActorControl = ClassObjPool<BulletWrapper>.Get();
                    break;
            }
            if (this.ActorControl != null)
            {
                this.ActorControl.Born(this);
            }
        }

        public void DeactiveActor()
        {
            this.gameObject.SetActive(false);
            this.SMNode.Detach();
            if (this.HurtControl != null)
            {
                this.HurtControl.Deactive();
            }
            if (this.MovementComponent != null)
            {
                this.MovementComponent.Deactive();
            }
            if (this.SkillControl != null)
            {
                this.SkillControl.Deactive();
            }
            if (this.HudControl != null)
            {
                this.HudControl.Deactive();
            }
            if (this.AnimControl != null)
            {
                this.AnimControl.Deactive();
            }
            if (this.BuffHolderComp != null)
            {
                this.BuffHolderComp.Deactive();
            }
            if (this.EffectControl != null)
            {
                this.EffectControl.Deactive();
            }
            if (this.ActorControl != null)
            {
                this.ActorControl.Deactive();
            }
            if (this.HorizonMarker != null)
            {
                this.HorizonMarker.Deactive();
            }
            if (this.ValueComponent != null)
            {
                this.ValueComponent.Deactive();
            }
            base.usingSeq = ClassObjPool<ActorRoot>.NewSeq();
        }

        public void FightOver()
        {
            if (this.ValueComponent != null)
            {
                this.ValueComponent.FightOver();
            }
            if (this.HurtControl != null)
            {
                this.HurtControl.FightOver();
            }
            if (this.MovementComponent != null)
            {
                this.MovementComponent.FightOver();
            }
            if (this.SkillControl != null)
            {
                this.SkillControl.FightOver();
            }
            if (this.HudControl != null)
            {
                this.HudControl.FightOver();
            }
            if (this.ActorControl != null)
            {
                this.ActorControl.FightOver();
            }
            if (this.AnimControl != null)
            {
                this.AnimControl.FightOver();
            }
            if (this.BuffHolderComp != null)
            {
                this.BuffHolderComp.FightOver();
            }
            if (this.EffectControl != null)
            {
                this.EffectControl.FightOver();
            }
            if (this.SkillControl != null)
            {
                this.SkillControl.FightOver();
            }
        }

        public T GetComponent<T>() where T: Component
        {
            if (this.gameObject != null)
            {
                return this.gameObject.GetComponent<T>();
            }
            return null;
        }

        public COM_PLAYERCAMP GiveMyEnemyCamp()
        {
            return ((this.TheActorMeta.ActorCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? COM_PLAYERCAMP.COM_PLAYERCAMP_1 : COM_PLAYERCAMP.COM_PLAYERCAMP_2);
        }

        public void InitActor()
        {
            if (!GameObjMgr.isPreSpawnActors)
            {
                this.ObjID = Singleton<GameObjMgr>.GetInstance().NewActorID;
            }
            if (this.ValueComponent != null)
            {
                this.ValueComponent.Init();
            }
            if (this.HurtControl != null)
            {
                this.HurtControl.Init();
            }
            if (this.MovementComponent != null)
            {
                this.MovementComponent.Init();
            }
            if (this.SkillControl != null)
            {
                this.SkillControl.Init();
            }
            if (this.HudControl != null)
            {
                this.HudControl.Init();
            }
            if (this.ActorControl != null)
            {
                this.ActorControl.Init();
            }
            if (this.AnimControl != null)
            {
                this.AnimControl.Init();
            }
            if (this.BuffHolderComp != null)
            {
                this.BuffHolderComp.Init();
            }
            if (this.EffectControl != null)
            {
                this.EffectControl.Init();
            }
            if (this.HorizonMarker != null)
            {
                this.HorizonMarker.Init();
            }
            if (this.EquipComponent != null)
            {
                this.EquipComponent.Init();
            }
            PoolObjHandle<ActorRoot> selfPtr = this.SelfPtr;
            Singleton<GameEventSys>.instance.SendEvent<PoolObjHandle<ActorRoot>>(GameEventDef.Event_ActorInit, ref selfPtr);
        }

        public void InitVisible()
        {
            if (Singleton<WatchController>.GetInstance().IsWatching)
            {
                this.Visible = true;
            }
            else
            {
                SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
                if ((((curLvelContext != null) && (curLvelContext.m_horizonEnableMethod != Horizon.EnableMethod.DisableAll)) && (this.TheActorMeta.ActorCamp != Singleton<GamePlayerCenter>.instance.hostPlayerCamp)) && (((this.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) || (this.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)) || (this.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)))
                {
                    this.Visible = false;
                    this._bInitVisibleDelay = 8;
                }
            }
        }

        public bool IsAtHostPlayerSameTeam()
        {
            return Singleton<GamePlayerCenter>.instance.GetHostPlayer().IsAtMyTeam(ref this.TheActorMeta);
        }

        public bool IsEnemyCamp(ActorRoot actor)
        {
            return ((actor != null) && (this.TheActorMeta.ActorCamp != actor.TheActorMeta.ActorCamp));
        }

        public bool IsHostCamp()
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if (hostPlayer == null)
            {
                DebugHelper.Assert(false, "Host Player is Empty!");
                return false;
            }
            return (hostPlayer.PlayerCamp == this.TheActorMeta.ActorCamp);
        }

        public bool IsSelfCamp(ActorRoot actor)
        {
            return ((actor != null) && (this.TheActorMeta.ActorCamp == actor.TheActorMeta.ActorCamp));
        }

        public bool IsSightExploree(COM_PLAYERCAMP myCampType)
        {
            return (myCampType != this.TheActorMeta.ActorCamp);
        }

        public bool IsSightExplorer(COM_PLAYERCAMP myCampType)
        {
            return ((myCampType == this.TheActorMeta.ActorCamp) && !this.ActorControl.IsDeadState);
        }

        public void LateUpdate()
        {
            if (this.ObjLinker != null)
            {
                this.ObjLinker.CustumLateUpdate();
            }
            int delta = (int) (Time.deltaTime * 1000f);
            if (this.HudControl != null)
            {
                this.HudControl.LateUpdate(delta);
            }
            if (this.SkillControl != null)
            {
                this.SkillControl.LateUpdate(delta);
            }
            if (this.ActorControl != null)
            {
                this.ActorControl.LateUpdate(delta);
            }
            if (this.EffectControl != null)
            {
                this.EffectControl.LateUpdate(delta);
            }
        }

        public override void OnRelease()
        {
            this.CharInfo = null;
            this.ObjLinker = null;
            this.ActorMesh = null;
            this.ActorMeshAnimation = null;
            this.isRecycled = true;
            this.ShadowEffect = null;
            base.OnRelease();
        }

        public override void OnUse()
        {
            base.OnUse();
            this.name = string.Empty;
            this.isMovable = true;
            this.isRotatable = true;
            this.ActorMesh = null;
            this.ActorMeshAnimation = null;
            this._bVisible = true;
            this._bInitVisibleDelay = 0;
            this._bInCamera = false;
            this.ObjID = 0;
            this.TheActorMeta = new ActorMeta();
            this.TheStaticData = new ActorStaticData();
            this.SelfPtr.Release();
            this.ObjLinker = null;
            this.ActorControl = null;
            this.ActorAgent = null;
            this.MovementComponent = null;
            this.SkillControl = null;
            this.ValueComponent = null;
            this.HurtControl = null;
            this.HudControl = null;
            this.AnimControl = null;
            this.BuffHolderComp = null;
            this.MatHurtEffect = null;
            this.ShadowEffect = null;
            this.EquipComponent = null;
            this.DefaultAttackModeControl = null;
            this.LockTargetAttackModeControl = null;
            this.OriginalActorMesh = null;
            this.OriginalMeshAnim = null;
            this.shape = null;
            this.slotList.Clear();
            this.bChildUpdate = false;
            this._location = VInt3.zero;
            this._forward = VInt3.forward;
            this._rotation = Quaternion.identity;
            this.groundY = 0;
            this.hasReachedNavEdge = false;
            this.pickFlyY = 0;
            this.AttackOrderReady = true;
            this.bOneKiller = false;
            this.CharInfo = null;
            this.HorizonMarker = null;
            this.isRecycled = false;
        }

        public void PrepareFight()
        {
            if (this.ValueComponent != null)
            {
                this.ValueComponent.Prepare();
            }
            if (this.HurtControl != null)
            {
                this.HurtControl.Prepare();
            }
            if (this.MovementComponent != null)
            {
                this.MovementComponent.Prepare();
            }
            if (this.SkillControl != null)
            {
                this.SkillControl.Prepare();
            }
            if (this.HudControl != null)
            {
                this.HudControl.Prepare();
            }
            if (this.ActorControl != null)
            {
                this.ActorControl.Prepare();
            }
            if (this.AnimControl != null)
            {
                this.AnimControl.Prepare();
            }
            if (this.BuffHolderComp != null)
            {
                this.BuffHolderComp.Prepare();
            }
            if (this.EffectControl != null)
            {
                this.EffectControl.Prepare();
            }
            PoolObjHandle<ActorRoot> selfPtr = this.SelfPtr;
            Singleton<GameEventSys>.instance.SendEvent<PoolObjHandle<ActorRoot>>(GameEventDef.Event_ActorPreFight, ref selfPtr);
        }

        public void ReactiveActor(VInt3 bornPos, VInt3 bornDir)
        {
            this.gameObject.SetActive(true);
            this.ObjID = Singleton<GameObjMgr>.GetInstance().NewActorID;
            this.SelfPtr.Validate();
            this.ObjLinker.ReattachActor();
            this.Teleport(bornPos, bornDir);
            this.InitVisible();
            if (this.shape != null)
            {
                this.shape.owner.Validate();
            }
            if (this.ValueComponent != null)
            {
                this.ValueComponent.Reactive();
            }
            if (this.HurtControl != null)
            {
                this.HurtControl.Reactive();
            }
            if (this.MovementComponent != null)
            {
                this.MovementComponent.Reactive();
            }
            if (this.SkillControl != null)
            {
                this.SkillControl.Reactive();
            }
            if (this.HudControl != null)
            {
                this.HudControl.Reactive();
            }
            if (this.ActorControl != null)
            {
                this.ActorControl.Reactive();
            }
            if (this.AnimControl != null)
            {
                this.AnimControl.Reactive();
            }
            if (this.BuffHolderComp != null)
            {
                this.BuffHolderComp.Reactive();
            }
            if (this.EffectControl != null)
            {
                this.EffectControl.Reactive();
            }
            if (this.HorizonMarker != null)
            {
                this.HorizonMarker.Reactive();
            }
            if (this.EquipComponent != null)
            {
                this.EquipComponent.Reactive();
            }
            PoolObjHandle<ActorRoot> selfPtr = this.SelfPtr;
            Singleton<GameEventSys>.instance.SendEvent<PoolObjHandle<ActorRoot>>(GameEventDef.Event_ActorInit, ref selfPtr);
            Singleton<GameEventSys>.instance.SendEvent<PoolObjHandle<ActorRoot>>(GameEventDef.Event_ActorPreFight, ref selfPtr);
            Singleton<GameObjMgr>.instance.AddActor(selfPtr);
            this.StartFight();
            this.SMNode.owner.Validate();
            this.SMNode.Attach();
        }

        public void RecordOriginalActorMesh()
        {
            this.OriginalActorMesh = this.ActorMesh;
            this.OriginalMeshAnim = this.ActorMeshAnimation;
        }

        public void RecoverOriginalActorMesh()
        {
            this.SetActorMesh(this.OriginalActorMesh);
            if (this.OriginalActorMesh != null)
            {
                this.OriginalActorMesh.SetActive(true);
            }
        }

        public bool RemoveActorRootSlot(ActorRootSlot _slot)
        {
            bool flag = this.slotList.Remove(_slot);
            if (this.slotList.Count == 0)
            {
                this.bChildUpdate = false;
            }
            return flag;
        }

        public void SetActorMesh(GameObject InActorMesh)
        {
            if (InActorMesh != this.ActorMesh)
            {
                this.ObjLinker.OnActorMeshChanged(InActorMesh);
                this.ActorMesh = InActorMesh;
                this.ActorMeshAnimation = (this.ActorMesh == null) ? null : this.ActorMesh.GetComponent<Animation>();
                if (this.ActorMesh != null)
                {
                    if (this._bVisible)
                    {
                        this.ActorMesh.SetLayer("Actor", "Particles", true);
                    }
                    else
                    {
                        this.ActorMesh.SetLayer("Hide", true);
                    }
                }
                if (this.ActorControl != null)
                {
                    this.ActorControl.UpdateAnimPlaySpeed();
                }
                if (this.MatHurtEffect != null)
                {
                    this.MatHurtEffect.OnMeshChanged();
                }
            }
        }

        public void Spawned()
        {
            this.SelfPtr = new PoolObjHandle<ActorRoot>(this);
            this.CreateLogicWrapper();
            this.AttachSMNode();
            this.InitVisible();
            if (this.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
            {
                Singleton<GamePlayerCenter>.instance.ConnectActorRootAndPlayer(ref this.SelfPtr);
            }
        }

        public void StartFight()
        {
            if (this.ValueComponent != null)
            {
                this.ValueComponent.Fight();
            }
            if (this.HurtControl != null)
            {
                this.HurtControl.Fight();
            }
            if (this.MovementComponent != null)
            {
                this.MovementComponent.Fight();
            }
            if (this.SkillControl != null)
            {
                this.SkillControl.Fight();
            }
            if (this.HudControl != null)
            {
                this.HudControl.Fight();
            }
            if (this.ActorControl != null)
            {
                this.ActorControl.Fight();
            }
            if (this.AnimControl != null)
            {
                this.AnimControl.Fight();
            }
            if (this.BuffHolderComp != null)
            {
                this.BuffHolderComp.Fight();
            }
            if (this.EffectControl != null)
            {
                this.EffectControl.Fight();
            }
            if (this.ObjLinker != null)
            {
                this.ObjLinker.ActorStart();
            }
            PoolObjHandle<ActorRoot> selfPtr = this.SelfPtr;
            Singleton<GameEventSys>.instance.SendEvent<PoolObjHandle<ActorRoot>>(GameEventDef.Event_ActorStartFight, ref selfPtr);
        }

        public void Suicide()
        {
            if (this.ValueComponent != null)
            {
                this.ValueComponent.actorHp = 0;
            }
        }

        public void Teleport(VInt3 pos, VInt3 dir)
        {
            VInt num;
            Transform transform = this.gameObject.transform;
            transform.position = (Vector3) pos;
            transform.rotation = Quaternion.LookRotation((Vector3) dir);
            transform.parent = MonoSingleton<SceneMgr>.GetInstance().GetRoot((SceneObjType) this.TheActorMeta.ActorType).transform;
            this.location = pos;
            this.forward = dir;
            this.rotation = transform.rotation;
            this.ObjLinker.SetForward(dir, -1);
            if ((this.TheActorMeta.ActorType < ActorTypeDef.Actor_Type_Bullet) && PathfindingUtility.GetGroundY(this, out num))
            {
                this.groundY = num;
                VInt3 location = this.location;
                location.y = this.groundY.i;
                this.location = location;
            }
        }

        public void UninitActor()
        {
            if (this.HurtControl != null)
            {
                this.HurtControl.Uninit();
            }
            if (this.MovementComponent != null)
            {
                this.MovementComponent.Uninit();
            }
            if (this.SkillControl != null)
            {
                this.SkillControl.Uninit();
            }
            if (this.HudControl != null)
            {
                this.HudControl.Uninit();
            }
            if (this.AnimControl != null)
            {
                this.AnimControl.Uninit();
            }
            if (this.BuffHolderComp != null)
            {
                this.BuffHolderComp.Uninit();
            }
            if (this.EffectControl != null)
            {
                this.EffectControl.Uninit();
            }
            if (this.ActorControl != null)
            {
                this.ActorControl.Uninit();
            }
            if (this.HorizonMarker != null)
            {
                this.HorizonMarker.Uninit();
            }
            if (this.ValueComponent != null)
            {
                this.ValueComponent.Uninit();
            }
            if (this.EquipComponent != null)
            {
                this.EquipComponent.Uninit();
            }
            if (this.HurtControl != null)
            {
                this.HurtControl.Release();
                this.HurtControl = null;
            }
            if (this.MovementComponent != null)
            {
                this.MovementComponent.Release();
                this.MovementComponent = null;
            }
            if (this.SkillControl != null)
            {
                this.SkillControl.Release();
                this.SkillControl = null;
            }
            if (this.HudControl != null)
            {
                this.HudControl.Release();
                this.HudControl = null;
            }
            if (this.AnimControl != null)
            {
                this.AnimControl.Release();
                this.AnimControl = null;
            }
            if (this.BuffHolderComp != null)
            {
                this.BuffHolderComp.Release();
                this.BuffHolderComp = null;
            }
            if (this.EffectControl != null)
            {
                this.EffectControl.Release();
                this.EffectControl = null;
            }
            if (this.HorizonMarker != null)
            {
                this.HorizonMarker.Release();
                this.HorizonMarker = null;
            }
            if (this.ActorControl != null)
            {
                this.ActorControl.Release();
                this.ActorControl = null;
            }
            if (this.ValueComponent != null)
            {
                this.ValueComponent.Release();
                this.ValueComponent = null;
            }
            if (this.EquipComponent != null)
            {
                this.EquipComponent.Release();
                this.EquipComponent = null;
            }
            if (this.DefaultAttackModeControl != null)
            {
                this.DefaultAttackModeControl.Release();
                this.DefaultAttackModeControl = null;
            }
            if (this.LockTargetAttackModeControl != null)
            {
                this.LockTargetAttackModeControl.Release();
                this.LockTargetAttackModeControl = null;
            }
        }

        private void UpdateActorRootSlot()
        {
            for (int i = 0; i < this.slotList.Count; i++)
            {
                ((ActorRootSlot) this.slotList[i]).Update(this);
            }
        }

        public void UpdateLogic(int delta)
        {
            if (this.bActive)
            {
                if ((this._bInitVisibleDelay > 0) && ((this._bInitVisibleDelay = (byte) (this._bInitVisibleDelay - 1)) == 0))
                {
                    this.Visible = true;
                }
                if (this.ActorControl != null)
                {
                    this.ActorControl.UpdateLogic(delta);
                }
                if (this.ValueComponent != null)
                {
                    this.ValueComponent.UpdateLogic(delta);
                }
                if (this.EquipComponent != null)
                {
                    this.EquipComponent.UpdateLogic(delta);
                }
                if (this.HurtControl != null)
                {
                    this.HurtControl.UpdateLogic(delta);
                }
                if (this.MovementComponent != null)
                {
                    this.MovementComponent.UpdateLogic(delta);
                }
                if (this.SkillControl != null)
                {
                    this.SkillControl.UpdateLogic(delta);
                }
                if (this.AnimControl != null)
                {
                    this.AnimControl.UpdateLogic(delta);
                }
                if (this.BuffHolderComp != null)
                {
                    this.BuffHolderComp.UpdateLogic(delta);
                }
                if (this.EffectControl != null)
                {
                    this.EffectControl.UpdateLogic(delta);
                }
                if (this.HorizonMarker != null)
                {
                    this.HorizonMarker.UpdateLogic(delta);
                }
                if (this.ActorControl != null)
                {
                    BaseAttackMode currentAttackMode = this.ActorControl.GetCurrentAttackMode();
                    if (currentAttackMode != null)
                    {
                        currentAttackMode.UpdateLogic(delta);
                    }
                }
            }
        }

        public GameObject ActorMesh { get; protected set; }

        public Animation ActorMeshAnimation { get; protected set; }

        private bool bActive
        {
            get
            {
                return this.gameObject.activeSelf;
            }
        }

        public VInt3 forward
        {
            get
            {
                return this._forward;
            }
            set
            {
                this._forward = value;
                if (this.shape != null)
                {
                    this.shape.dirty = true;
                }
                if (this.bChildUpdate)
                {
                    this.UpdateActorRootSlot();
                }
            }
        }

        public GameObject gameObject
        {
            get
            {
                return ((this.ObjLinker == null) ? null : this.ObjLinker.gameObject);
            }
        }

        public bool hasCollidedWithAgents
        {
            get
            {
                return (((this.MovementComponent != null) && (this.MovementComponent.Pathfinding != null)) && this.MovementComponent.Pathfinding.hasCollidedWithAgents);
            }
        }

        public bool InCamera
        {
            get
            {
                return this._bInCamera;
            }
            set
            {
                if (this._bInCamera != value)
                {
                    this._bInCamera = value;
                    if ((this._bInCamera && this._bVisible) && (this.AnimControl != null))
                    {
                        this.AnimControl.UpdateCurAnimState();
                    }
                    if (this.HudControl != null)
                    {
                        this.HudControl.SetComVisible(value && this._bVisible);
                    }
                }
            }
        }

        public VInt3 location
        {
            get
            {
                return this._location;
            }
            set
            {
                bool flag = (this._location.x != value.x) || (this._location.z != value.z);
                this._location = value;
                if (this.shape != null)
                {
                    this.shape.dirty = true;
                }
                if (flag && (this.SMNode != null))
                {
                    this.SMNode.makeDirty();
                }
                if (this.bChildUpdate)
                {
                    this.UpdateActorRootSlot();
                }
            }
        }

        public Quaternion rotation
        {
            get
            {
                return this._rotation;
            }
            set
            {
                if (this.isRotatable)
                {
                    this._rotation = value;
                }
            }
        }

        public bool Visible
        {
            get
            {
                return this._bVisible;
            }
            set
            {
                this._bInitVisibleDelay = 0;
                if (this._bVisible != value)
                {
                    this._bVisible = value;
                    if (this._bVisible)
                    {
                        if (this.AnimControl != null)
                        {
                            this.AnimControl.UpdateCurAnimState();
                        }
                        this.gameObject.SetLayer("Actor", "Particles", true);
                    }
                    else
                    {
                        this.gameObject.SetLayer("Hide", true);
                    }
                    if (this.HudControl != null)
                    {
                        this.HudControl.SetComVisible(value && this._bInCamera);
                    }
                    Singleton<EventRouter>.GetInstance().BroadCastEvent<uint, bool>("ActorVisibleToHostPlayerChnage", this.ObjID, this._bVisible);
                }
            }
        }

        public bool VisibleIniting
        {
            get
            {
                return (this._bInitVisibleDelay > 0);
            }
        }
    }
}

