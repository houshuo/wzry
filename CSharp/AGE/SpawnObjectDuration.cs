namespace AGE
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameLogic.DataCenter;
    using CSProtocol;
    using System;
    using UnityEngine;

    [EventCategory("MMGame/Skill")]
    public class SpawnObjectDuration : DurationEvent
    {
        private PoolObjHandle<ActorRoot> actorRoot;
        private ActorRootSlot actorSlot;
        public bool applyActionSpeedToAnimation = true;
        public bool applyActionSpeedToParticle = true;
        public bool bEyeObj;
        public bool bTargetPosition;
        public bool bUseSkin;
        public bool bUseSkinAdvance;
        public VInt3 direction = VInt3.forward;
        public bool enableLayer;
        public bool enableTag;
        [AssetReference(AssetRefType.MonsterConfigId)]
        public int EyeCfgIdByMonster;
        public int EyeLifeTime;
        public int layer;
        private GameObject m_particleObj;
        public bool modifyDirection;
        public bool modifyScaling;
        public bool modifyTranslation = true;
        [ObjectTemplate(new System.Type[] {  })]
        public int objectSpaceId = -1;
        [ObjectTemplate(new System.Type[] {  })]
        public int parentId = -1;
        [AssetReference(AssetRefType.Particle)]
        public string prefabName = string.Empty;
        public bool recreateExisting = true;
        public Vector3 scaling = Vector3.one;
        public int sightRadius;
        public bool superTranslation;
        public string tag = string.Empty;
        [ObjectTemplate(true)]
        public int targetId = -1;
        public VInt3 targetPosition = VInt3.zero;
        public VInt3 translation = VInt3.zero;

        public override BaseEvent Clone()
        {
            SpawnObjectDuration duration = ClassObjPool<SpawnObjectDuration>.Get();
            duration.CopyData(this);
            return duration;
        }

        protected override void CopyData(BaseEvent src)
        {
            base.CopyData(src);
            SpawnObjectDuration duration = src as SpawnObjectDuration;
            this.targetId = duration.targetId;
            this.parentId = duration.parentId;
            this.objectSpaceId = duration.objectSpaceId;
            this.prefabName = duration.prefabName;
            this.recreateExisting = duration.recreateExisting;
            this.modifyTranslation = duration.modifyTranslation;
            this.superTranslation = duration.superTranslation;
            this.translation = duration.translation;
            this.bTargetPosition = duration.bTargetPosition;
            this.targetPosition = duration.targetPosition;
            this.modifyDirection = duration.modifyDirection;
            this.direction = duration.direction;
            this.modifyScaling = duration.modifyScaling;
            this.scaling = duration.scaling;
            this.enableLayer = duration.enableLayer;
            this.layer = duration.layer;
            this.enableTag = duration.enableTag;
            this.tag = duration.tag;
            this.applyActionSpeedToAnimation = duration.applyActionSpeedToAnimation;
            this.applyActionSpeedToParticle = duration.applyActionSpeedToParticle;
            this.bUseSkin = duration.bUseSkin;
            this.bUseSkinAdvance = duration.bUseSkinAdvance;
            this.sightRadius = duration.sightRadius;
            this.bEyeObj = duration.bEyeObj;
            this.EyeLifeTime = duration.EyeLifeTime;
            this.EyeCfgIdByMonster = duration.EyeCfgIdByMonster;
        }

        private void CreateBullet()
        {
            if (this.actorRoot != 0)
            {
                Singleton<GameObjMgr>.instance.AddBullet(ref this.actorRoot);
            }
        }

        private void CreateEye()
        {
            if ((this.actorRoot != 0) && this.bEyeObj)
            {
                this.actorRoot.handle.ObjLinker.CanMovable = false;
                Singleton<GameObjMgr>.instance.AddActor(this.actorRoot);
                EyeWrapper actorControl = this.actorRoot.handle.ActorControl as EyeWrapper;
                if (actorControl != null)
                {
                    actorControl.LifeTime = this.EyeLifeTime;
                }
                if (this.actorRoot.handle.HorizonMarker != null)
                {
                    this.actorRoot.handle.HorizonMarker.SightRadius = this.sightRadius;
                }
                if (this.actorRoot.handle.SMNode != null)
                {
                    VCollisionSphere sphere = new VCollisionSphere();
                    sphere.Born((ActorRoot) this.actorRoot);
                    sphere.Pos = VInt3.zero;
                    sphere.Radius = 500;
                    sphere.dirty = true;
                    sphere.ConditionalUpdateShape();
                    this.actorRoot.handle.SMNode.Attach();
                }
            }
        }

        public override void Enter(AGE.Action _action, Track _track)
        {
            base.Enter(_action, _track);
            if (this.bEyeObj)
            {
                this.EnterSpawnEye(_action, _track);
            }
            else
            {
                this.EnterSpawnBullet(_action, _track);
            }
        }

        private void EnterSpawnBullet(AGE.Action _action, Track _track)
        {
            string prefabName;
            if (this.bUseSkin)
            {
                prefabName = SkinResourceHelper.GetResourceName(_action, this.prefabName, this.bUseSkinAdvance);
            }
            else
            {
                prefabName = this.prefabName;
            }
            VInt3 zero = VInt3.zero;
            VInt3 forward = VInt3.forward;
            SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
            COM_PLAYERCAMP camp = ((refParamObject == null) || (refParamObject.Originator == 0)) ? COM_PLAYERCAMP.COM_PLAYERCAMP_MID : refParamObject.Originator.handle.TheActorMeta.ActorCamp;
            GameObject gameObject = _action.GetGameObject(this.parentId);
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.parentId);
            PoolObjHandle<ActorRoot> handle2 = _action.GetActorHandle(this.objectSpaceId);
            if (handle2 != 0)
            {
                ActorRoot handle = handle2.handle;
                if (this.superTranslation)
                {
                    VInt3 num3 = VInt3.zero;
                    _action.refParams.GetRefParam("_BulletPos", ref num3);
                    zero = IntMath.Transform(num3, handle.forward, handle.location);
                }
                else if (this.modifyTranslation)
                {
                    zero = IntMath.Transform(this.translation, handle.forward, handle.location);
                }
                if (this.modifyDirection)
                {
                    forward = handle2.handle.forward;
                }
            }
            else if (this.bTargetPosition)
            {
                zero = this.translation + this.targetPosition;
                if ((this.modifyDirection && (refParamObject != null)) && (refParamObject.Originator != 0))
                {
                    forward = refParamObject.Originator.handle.forward;
                }
            }
            else
            {
                if (this.modifyTranslation)
                {
                    zero = this.translation;
                }
                if ((this.modifyDirection && (this.direction.x != 0)) && (this.direction.y != 0))
                {
                    forward = this.direction;
                    forward.NormalizeTo(0x3e8);
                }
            }
            if (this.targetId >= 0)
            {
                _action.ExpandGameObject(this.targetId);
                GameObject obj3 = _action.GetGameObject(this.targetId);
                if (this.recreateExisting && (obj3 != null))
                {
                    if (this.applyActionSpeedToAnimation)
                    {
                        _action.RemoveTempObject(AGE.Action.PlaySpeedAffectedType.ePSAT_Anim, obj3);
                    }
                    if (this.applyActionSpeedToParticle)
                    {
                        _action.RemoveTempObject(AGE.Action.PlaySpeedAffectedType.ePSAT_Fx, obj3);
                    }
                    ActorHelper.DetachActorRoot(obj3);
                    ActionManager.DestroyGameObject(obj3);
                    _action.SetGameObject(this.targetId, null);
                }
                GameObject go = null;
                bool isInit = true;
                if (obj3 == null)
                {
                    go = MonoSingleton<SceneMgr>.GetInstance().Spawn("TempObject", SceneObjType.Bullet, zero, forward);
                    if (go == null)
                    {
                        throw new Exception("Age:SpawnObjectDuration Spawn Exception");
                    }
                    go.transform.localScale = Vector3.one;
                    bool flag2 = true;
                    int particleLOD = GameSettings.ParticleLOD;
                    if (GameSettings.DynamicParticleLOD)
                    {
                        if (((refParamObject != null) && (refParamObject.Originator != 0)) && (refParamObject.Originator.handle.TheActorMeta.PlayerId == Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().PlayerId))
                        {
                            flag2 = false;
                        }
                        if (!flag2 && (particleLOD > 1))
                        {
                            GameSettings.ParticleLOD = 1;
                        }
                        MonoSingleton<SceneMgr>.GetInstance().m_dynamicLOD = flag2;
                    }
                    this.m_particleObj = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(prefabName, true, SceneObjType.ActionRes, go.transform.position, go.transform.rotation, out isInit);
                    if (GameSettings.DynamicParticleLOD)
                    {
                        MonoSingleton<SceneMgr>.GetInstance().m_dynamicLOD = false;
                    }
                    if (this.m_particleObj == null)
                    {
                        if (GameSettings.DynamicParticleLOD)
                        {
                            MonoSingleton<SceneMgr>.GetInstance().m_dynamicLOD = flag2;
                        }
                        this.m_particleObj = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(this.prefabName, true, SceneObjType.ActionRes, go.transform.position, go.transform.rotation, out isInit);
                        if (GameSettings.DynamicParticleLOD)
                        {
                            MonoSingleton<SceneMgr>.GetInstance().m_dynamicLOD = false;
                        }
                    }
                    if (GameSettings.DynamicParticleLOD)
                    {
                        GameSettings.ParticleLOD = particleLOD;
                    }
                    if (this.m_particleObj != null)
                    {
                        this.m_particleObj.transform.SetParent(go.transform);
                        this.m_particleObj.transform.localPosition = Vector3.zero;
                        this.m_particleObj.transform.localRotation = Quaternion.identity;
                    }
                    this.actorRoot = ActorHelper.AttachActorRoot(go, ActorTypeDef.Actor_Type_Bullet, camp, null);
                    _action.SetGameObject(this.targetId, go);
                    this.actorRoot.handle.location = zero;
                    this.actorRoot.handle.forward = forward;
                    VCollisionShape.InitActorCollision((ActorRoot) this.actorRoot, this.m_particleObj, _action.actionName);
                    if (this.actorRoot.handle.shape != null)
                    {
                        this.actorRoot.handle.shape.ConditionalUpdateShape();
                    }
                    this.actorRoot.handle.InitActor();
                    if (refParamObject != null)
                    {
                        refParamObject.EffectPos = this.actorRoot.handle.location;
                        if (this.actorRoot.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_EYE)
                        {
                            this.CreateBullet();
                        }
                    }
                    if (this.applyActionSpeedToAnimation)
                    {
                        _action.AddTempObject(AGE.Action.PlaySpeedAffectedType.ePSAT_Anim, go);
                    }
                    if (this.applyActionSpeedToParticle)
                    {
                        _action.AddTempObject(AGE.Action.PlaySpeedAffectedType.ePSAT_Fx, go);
                    }
                    this.actorRoot.handle.StartFight();
                    if (this.enableLayer || this.enableTag)
                    {
                        if (this.enableLayer)
                        {
                            go.layer = this.layer;
                        }
                        if (this.enableTag)
                        {
                            go.tag = this.tag;
                        }
                        Transform[] componentsInChildren = go.GetComponentsInChildren<Transform>();
                        for (int i = 0; i < componentsInChildren.Length; i++)
                        {
                            if (this.enableLayer)
                            {
                                componentsInChildren[i].gameObject.layer = this.layer;
                            }
                            if (this.enableTag)
                            {
                                componentsInChildren[i].gameObject.tag = this.tag;
                            }
                        }
                    }
                    if (isInit)
                    {
                        ParticleHelper.Init(go, this.scaling);
                    }
                    PoolObjHandle<ActorRoot> newActor = _action.GetActorHandle(this.targetId);
                    this.SetParent(ref actorHandle, ref newActor, this.translation);
                    if (this.modifyScaling)
                    {
                        go.transform.localScale = this.scaling;
                    }
                }
            }
            else
            {
                GameObject obj5;
                if (this.modifyDirection)
                {
                    obj5 = MonoSingleton<SceneMgr>.GetInstance().InstantiateLOD(this.prefabName, true, SceneObjType.ActionRes, (Vector3) zero, Quaternion.LookRotation((Vector3) forward));
                }
                else
                {
                    obj5 = MonoSingleton<SceneMgr>.GetInstance().InstantiateLOD(this.prefabName, true, SceneObjType.ActionRes, (Vector3) zero);
                }
                if (obj5 != null)
                {
                    if (this.applyActionSpeedToAnimation)
                    {
                        _action.AddTempObject(AGE.Action.PlaySpeedAffectedType.ePSAT_Anim, obj5);
                    }
                    if (this.applyActionSpeedToParticle)
                    {
                        _action.AddTempObject(AGE.Action.PlaySpeedAffectedType.ePSAT_Fx, obj5);
                    }
                    if (this.enableLayer)
                    {
                        obj5.layer = this.layer;
                        Transform[] transformArray2 = obj5.GetComponentsInChildren<Transform>();
                        for (int j = 0; j < transformArray2.Length; j++)
                        {
                            transformArray2[j].gameObject.layer = this.layer;
                        }
                    }
                    if (this.enableTag)
                    {
                        obj5.tag = this.tag;
                        Transform[] transformArray3 = obj5.GetComponentsInChildren<Transform>();
                        for (int k = 0; k < transformArray3.Length; k++)
                        {
                            transformArray3[k].gameObject.tag = this.tag;
                        }
                    }
                    if ((obj5.GetComponent<ParticleSystem>() != null) && this.modifyScaling)
                    {
                        ParticleSystem[] systemArray = obj5.GetComponentsInChildren<ParticleSystem>();
                        for (int m = 0; m < systemArray.Length; m++)
                        {
                            ParticleSystem system1 = systemArray[m];
                            system1.startSize *= this.scaling.x;
                            ParticleSystem system2 = systemArray[m];
                            system2.startLifetime *= this.scaling.y;
                            ParticleSystem system3 = systemArray[m];
                            system3.startSpeed *= this.scaling.z;
                            Transform transform = systemArray[m].transform;
                            transform.localScale = (Vector3) (transform.localScale * this.scaling.x);
                        }
                    }
                    PoolObjHandle<ActorRoot> actorRoot = ActorHelper.GetActorRoot(obj5);
                    this.SetParent(ref actorHandle, ref actorRoot, this.translation);
                    if (this.modifyScaling)
                    {
                        obj5.transform.localScale = this.scaling;
                    }
                }
            }
        }

        private void EnterSpawnEye(AGE.Action _action, Track _track)
        {
            string prefabName;
            if (this.bUseSkin)
            {
                prefabName = SkinResourceHelper.GetResourceName(_action, this.prefabName, this.bUseSkinAdvance);
            }
            else
            {
                prefabName = this.prefabName;
            }
            VInt3 zero = VInt3.zero;
            VInt3 forward = VInt3.forward;
            SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
            COM_PLAYERCAMP com_playercamp = ((refParamObject == null) || (refParamObject.Originator == 0)) ? COM_PLAYERCAMP.COM_PLAYERCAMP_MID : refParamObject.Originator.handle.TheActorMeta.ActorCamp;
            GameObject gameObject = _action.GetGameObject(this.parentId);
            PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.parentId);
            PoolObjHandle<ActorRoot> handle2 = _action.GetActorHandle(this.objectSpaceId);
            if (handle2 != 0)
            {
                ActorRoot handle = handle2.handle;
                if (this.superTranslation)
                {
                    VInt3 num3 = VInt3.zero;
                    _action.refParams.GetRefParam("_BulletPos", ref num3);
                    zero = IntMath.Transform(num3, handle.forward, handle.location);
                }
                else if (this.modifyTranslation)
                {
                    zero = IntMath.Transform(this.translation, handle.forward, handle.location);
                }
                if (this.modifyDirection)
                {
                    forward = handle2.handle.forward;
                }
            }
            else if (this.bTargetPosition)
            {
                zero = this.translation + this.targetPosition;
                if ((this.modifyDirection && (refParamObject != null)) && (refParamObject.Originator != 0))
                {
                    forward = refParamObject.Originator.handle.forward;
                }
            }
            else
            {
                if (this.modifyTranslation)
                {
                    zero = this.translation;
                }
                if ((this.modifyDirection && (this.direction.x != 0)) && (this.direction.y != 0))
                {
                    forward = this.direction;
                    forward.NormalizeTo(0x3e8);
                }
            }
            if (this.targetId >= 0)
            {
                _action.ExpandGameObject(this.targetId);
                GameObject obj3 = _action.GetGameObject(this.targetId);
                if (this.recreateExisting && (obj3 != null))
                {
                    if (this.applyActionSpeedToAnimation)
                    {
                        _action.RemoveTempObject(AGE.Action.PlaySpeedAffectedType.ePSAT_Anim, obj3);
                    }
                    if (this.applyActionSpeedToParticle)
                    {
                        _action.RemoveTempObject(AGE.Action.PlaySpeedAffectedType.ePSAT_Fx, obj3);
                    }
                    ActorHelper.DetachActorRoot(obj3);
                    ActionManager.DestroyGameObject(obj3);
                    _action.SetGameObject(this.targetId, null);
                }
                GameObject obj4 = null;
                if (obj3 == null)
                {
                    ActorStaticData actorData = new ActorStaticData();
                    IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider);
                    ActorMeta actorMeta = new ActorMeta {
                        ActorType = ActorTypeDef.Actor_Type_EYE,
                        ActorCamp = com_playercamp,
                        ConfigId = this.EyeCfgIdByMonster,
                        EnCId = this.EyeCfgIdByMonster
                    };
                    actorDataProvider.GetActorStaticData(ref actorMeta, ref actorData);
                    CActorInfo info = null;
                    if (!string.IsNullOrEmpty(actorData.TheResInfo.ResPath))
                    {
                        CActorInfo actorInfo = CActorInfo.GetActorInfo(actorData.TheResInfo.ResPath, enResourceType.BattleScene);
                        if (actorInfo != null)
                        {
                            info = (CActorInfo) UnityEngine.Object.Instantiate(actorInfo);
                        }
                    }
                    PoolObjHandle<ActorRoot> handle3 = new PoolObjHandle<ActorRoot>();
                    if (info != null)
                    {
                        if ((refParamObject.Originator != 0) && !PathfindingUtility.IsValidTarget(refParamObject.Originator.handle, zero))
                        {
                            zero = refParamObject.Originator.handle.location;
                        }
                        handle3 = Singleton<GameObjMgr>.instance.SpawnActorEx(null, ref actorMeta, zero, forward, false, true);
                        if (handle3 != 0)
                        {
                            this.actorRoot = handle3;
                            obj4 = handle3.handle.gameObject;
                            handle3.handle.InitActor();
                            this.CreateEye();
                            handle3.handle.PrepareFight();
                            handle3.handle.StartFight();
                        }
                    }
                    if (handle3 != 0)
                    {
                        if (obj4 == null)
                        {
                            throw new Exception("Age:SpawnObjectDuration Spawn Exception");
                        }
                        obj4.transform.localScale = Vector3.one;
                        if (GameSettings.DynamicParticleLOD)
                        {
                            bool flag = true;
                            int particleLOD = GameSettings.ParticleLOD;
                            if (((refParamObject != null) && (refParamObject.Originator != 0)) && (refParamObject.Originator.handle.TheActorMeta.PlayerId == Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().PlayerId))
                            {
                                flag = false;
                            }
                            if (!flag && (particleLOD > 1))
                            {
                                GameSettings.ParticleLOD = 1;
                            }
                            MonoSingleton<SceneMgr>.GetInstance().m_dynamicLOD = flag;
                        }
                        this.actorRoot.handle.location = zero;
                        this.actorRoot.handle.forward = forward;
                        if (this.actorRoot.handle.shape != null)
                        {
                            this.actorRoot.handle.shape.ConditionalUpdateShape();
                        }
                        if (this.actorRoot.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE)
                        {
                            this.actorRoot.handle.TheActorMeta.ConfigId = this.EyeCfgIdByMonster;
                        }
                        if (refParamObject != null)
                        {
                            refParamObject.EffectPos = this.actorRoot.handle.location;
                            if (this.actorRoot.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE)
                            {
                                DebugHelper.Assert(this.actorRoot.handle.TheActorMeta.ActorCamp == com_playercamp);
                                this.actorRoot.handle.TheActorMeta.ActorCamp = com_playercamp;
                            }
                        }
                    }
                }
            }
        }

        public override void Leave(AGE.Action _action, Track _track)
        {
            base.Leave(_action, _track);
            if (!this.bEyeObj)
            {
                if (this.m_particleObj != null)
                {
                    this.m_particleObj.transform.parent = null;
                    ActionManager.DestroyGameObject(this.m_particleObj);
                }
                GameObject gameObject = _action.GetGameObject(this.targetId);
                if ((this.targetId >= 0) && (gameObject != null))
                {
                    if (this.applyActionSpeedToAnimation)
                    {
                        _action.RemoveTempObject(AGE.Action.PlaySpeedAffectedType.ePSAT_Anim, gameObject);
                    }
                    if (this.applyActionSpeedToParticle)
                    {
                        _action.RemoveTempObject(AGE.Action.PlaySpeedAffectedType.ePSAT_Fx, gameObject);
                    }
                    this.RemoveBullet();
                    ActorHelper.DetachActorRoot(gameObject);
                    ActionManager.DestroyGameObjectFromAction(_action, gameObject);
                }
            }
            if (this.actorSlot != null)
            {
                PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.parentId);
                if (actorHandle != 0)
                {
                    actorHandle.handle.RemoveActorRootSlot(this.actorSlot);
                }
                this.actorSlot = null;
            }
        }

        public override void OnUse()
        {
            base.OnUse();
            this.actorSlot = null;
            this.sightRadius = 0;
            this.m_particleObj = null;
            this.actorRoot.Release();
            this.sightRadius = 0;
            this.bEyeObj = false;
            this.EyeLifeTime = 0;
            this.EyeCfgIdByMonster = 0;
        }

        private void RemoveBullet()
        {
            if (this.actorRoot != 0)
            {
                Singleton<GameObjMgr>.instance.RmvBullet(ref this.actorRoot);
            }
        }

        private void SetParent(ref PoolObjHandle<ActorRoot> parentActor, ref PoolObjHandle<ActorRoot> newActor, VInt3 trans)
        {
            if ((parentActor != 0) && (newActor != 0))
            {
                this.actorSlot = parentActor.handle.CreateActorRootSlot(newActor, trans);
            }
        }

        public override bool SupportEditMode()
        {
            return true;
        }
    }
}

