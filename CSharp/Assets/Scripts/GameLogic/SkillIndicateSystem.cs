namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using UnityEngine;

    internal class SkillIndicateSystem : Singleton<SkillIndicateSystem>
    {
        public const string CommonAttackIndicatePrefabName = "Prefab_Skill_Effects/tongyong_effects/Indicator/select_02";
        private GameObject commonAttackPrefab;
        public const string DeadIndicatePrefabName = "Prefab_Skill_Effects/tongyong_effects/Siwang_tongyong/siwang_tongyong_01";
        private GameObject indicatePrefab;
        private GameObject lockHeroPrefab;
        public const string LockHeroPrefabName = "Prefab_Skill_Effects/tongyong_effects/Indicator/select_b_01";
        private GameObject lockTargetPrefab;
        public const string LockTargetPrefabName = "Prefab_Skill_Effects/tongyong_effects/Indicator/lockt_01";
        public const string TargetIndicatePrefabName = "Prefab_Skill_Effects/tongyong_effects/Indicator/lockt_01";
        private PoolObjHandle<ActorRoot> targetObj;

        private void HidePrefab(GameObject _prefab)
        {
            if (_prefab != null)
            {
                _prefab.SetLayer("Hide", false);
            }
        }

        public override void Init()
        {
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_SelectTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnSelectTarget));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_ClearTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnClearTarget));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_LockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnLockTarget));
            Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<LockTargetEventParam>(GameSkillEventDef.Event_ClearLockTarget, new GameSkillEvent<LockTargetEventParam>(this.OnClearLockTarget));
            Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.OnFightPrepare));
            Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.OnFightOver));
            Singleton<GameEventSys>.GetInstance().AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
        }

        private void OnActorDead(ref GameDeadEventParam prm)
        {
            if (((this.targetObj != 0) && (prm.src.handle.ObjID == this.targetObj.handle.ObjID)) && (this.indicatePrefab != null))
            {
                this.HidePrefab(this.indicatePrefab);
                MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.indicatePrefab, SceneObjType.ActionRes);
            }
            if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
            {
                GameObject pooledGameObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD("Prefab_Skill_Effects/tongyong_effects/Siwang_tongyong/siwang_tongyong_01", true, SceneObjType.ActionRes, prm.src.handle.gameObject.transform.position);
                if (pooledGameObject != null)
                {
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(pooledGameObject, 0x1388, null);
                }
            }
        }

        private void OnClearLockTarget(ref LockTargetEventParam _param)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(_param.lockTargetID);
            if ((actor != 0) && (this.lockTargetPrefab != null))
            {
                this.targetObj = new PoolObjHandle<ActorRoot>(null);
                this.HidePrefab(this.lockTargetPrefab);
                MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.lockTargetPrefab, SceneObjType.ActionRes);
                if ((actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (this.lockHeroPrefab != null))
                {
                    this.HidePrefab(this.lockHeroPrefab);
                    MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.lockHeroPrefab, SceneObjType.ActionRes);
                }
            }
        }

        private void OnClearTarget(ref SelectTargetEventParam _param)
        {
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(_param.commonAttackTargetID);
            if ((actor != 0) && (this.indicatePrefab != null))
            {
                this.targetObj = new PoolObjHandle<ActorRoot>(null);
                this.HidePrefab(this.indicatePrefab);
                MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.indicatePrefab, SceneObjType.ActionRes);
                if ((actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) && (this.lockHeroPrefab != null))
                {
                    this.HidePrefab(this.lockHeroPrefab);
                    MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.lockHeroPrefab, SceneObjType.ActionRes);
                }
            }
        }

        private void OnFightOver(ref DefaultGameEventParam prm)
        {
            if (this.indicatePrefab != null)
            {
                UnityEngine.Object.Destroy(this.indicatePrefab);
            }
            if (this.commonAttackPrefab != null)
            {
                UnityEngine.Object.Destroy(this.commonAttackPrefab);
            }
            if (this.lockTargetPrefab != null)
            {
                UnityEngine.Object.Destroy(this.lockTargetPrefab);
            }
        }

        private void OnFightPrepare(ref DefaultGameEventParam prm)
        {
            GameObject content = Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Skill_Effects/tongyong_effects/Indicator/lockt_01", typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject;
            if (content != null)
            {
                this.indicatePrefab = (GameObject) UnityEngine.Object.Instantiate(content);
                this.HidePrefab(this.indicatePrefab);
                MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.indicatePrefab, SceneObjType.ActionRes);
            }
            content = Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Skill_Effects/tongyong_effects/Indicator/select_02", typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject;
            if (content != null)
            {
                this.commonAttackPrefab = (GameObject) UnityEngine.Object.Instantiate(content);
                this.HidePrefab(this.commonAttackPrefab);
                MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.commonAttackPrefab, SceneObjType.ActionRes);
            }
            Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Skill_Effects/tongyong_effects/Siwang_tongyong/siwang_tongyong_01", typeof(GameObject), enResourceType.BattleScene, true, false);
            content = Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Skill_Effects/tongyong_effects/Indicator/lockt_01", typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject;
            if (content != null)
            {
                this.lockTargetPrefab = (GameObject) UnityEngine.Object.Instantiate(content);
                this.HidePrefab(this.lockTargetPrefab);
                MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.lockTargetPrefab, SceneObjType.ActionRes);
            }
            content = Singleton<CResourceManager>.GetInstance().GetResource("Prefab_Skill_Effects/tongyong_effects/Indicator/select_b_01", typeof(GameObject), enResourceType.BattleScene, false, false).m_content as GameObject;
            if (content != null)
            {
                this.lockHeroPrefab = (GameObject) UnityEngine.Object.Instantiate(content);
                this.HidePrefab(this.lockHeroPrefab);
                MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.lockHeroPrefab, SceneObjType.ActionRes);
            }
        }

        private void OnLockTarget(ref LockTargetEventParam _param)
        {
            Vector3 zero = Vector3.zero;
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(_param.lockTargetID);
            if (((actor != 0) && (this.lockTargetPrefab != null)) && !ActorHelper.IsHostActor(ref actor))
            {
                this.targetObj = actor;
                this.ShowPrefab(this.lockTargetPrefab);
                if (actor.handle.CharInfo != null)
                {
                    this.SetPrefabScaler(this.lockTargetPrefab, actor.handle.CharInfo.iCollisionSize.x);
                }
                else
                {
                    this.SetPrefabScaler(this.lockTargetPrefab, 400);
                }
                zero = actor.handle.gameObject.transform.position;
                zero.y += 0.2f;
                this.lockTargetPrefab.transform.position = zero;
                this.lockTargetPrefab.transform.SetParent(actor.handle.gameObject.transform);
                if (this.lockHeroPrefab != null)
                {
                    if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        float num = 0f;
                        if (actor.handle.CharInfo != null)
                        {
                            num = ((float) actor.handle.CharInfo.iBulletHeight) / 1000f;
                        }
                        this.ShowPrefab(this.lockHeroPrefab);
                        zero.y += num;
                        this.lockHeroPrefab.transform.position = zero;
                        this.lockHeroPrefab.transform.SetParent(actor.handle.gameObject.transform);
                    }
                    else
                    {
                        this.HidePrefab(this.lockHeroPrefab);
                        MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.lockHeroPrefab, SceneObjType.ActionRes);
                    }
                }
            }
        }

        private void OnSelectTarget(ref SelectTargetEventParam _param)
        {
            Vector3 zero = Vector3.zero;
            PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(_param.commonAttackTargetID);
            if (((actor != 0) && (this.indicatePrefab != null)) && !ActorHelper.IsHostActor(ref actor))
            {
                this.targetObj = actor;
                this.ShowPrefab(this.indicatePrefab);
                if (this.targetObj.handle.CharInfo != null)
                {
                    this.SetPrefabScaler(this.indicatePrefab, this.targetObj.handle.CharInfo.iCollisionSize.x);
                }
                else
                {
                    this.SetPrefabScaler(this.indicatePrefab, 400);
                }
                zero = actor.handle.gameObject.transform.position;
                zero.y += 0.2f;
                this.indicatePrefab.transform.position = zero;
                this.indicatePrefab.transform.SetParent(actor.handle.gameObject.transform);
                if (this.lockHeroPrefab != null)
                {
                    if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                    {
                        float num = 0f;
                        if (actor.handle.CharInfo != null)
                        {
                            num = ((float) actor.handle.CharInfo.iBulletHeight) / 1000f;
                        }
                        zero.y += num;
                        this.lockHeroPrefab.transform.position = zero;
                        this.lockHeroPrefab.transform.SetParent(actor.handle.gameObject.transform);
                        this.ShowPrefab(this.lockHeroPrefab);
                    }
                    else
                    {
                        this.HidePrefab(this.lockHeroPrefab);
                        MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.lockHeroPrefab, SceneObjType.ActionRes);
                    }
                }
            }
        }

        public void PlayCommonAttackTargetEffect(ActorRoot _target)
        {
            if ((_target != null) && (this.commonAttackPrefab != null))
            {
                this.ShowPrefab(this.commonAttackPrefab);
                this.commonAttackPrefab.transform.position = _target.gameObject.transform.position;
                this.commonAttackPrefab.transform.SetParent(_target.gameObject.transform);
            }
        }

        public static void Preload(ref ActorPreloadTab preloadTab)
        {
            preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Indicator/select_02");
            preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Indicator/lockt_01");
            preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Siwang_tongyong/siwang_tongyong_01");
            preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Indicator/lockt_01");
            preloadTab.AddParticle("Prefab_Skill_Effects/tongyong_effects/Indicator/select_b_01");
        }

        public void SetPrefabScaler(GameObject _obj, int _distance)
        {
            float x = ((float) (_distance + 400)) / 800f;
            _obj.transform.localScale = new Vector3(x, x, x);
        }

        private void ShowPrefab(GameObject _prefab)
        {
            if (_prefab != null)
            {
                _prefab.SetLayer("Actor", "Particles", false);
            }
        }

        public void StopCommonAttackTargetEffect()
        {
            if (this.commonAttackPrefab != null)
            {
                this.HidePrefab(this.commonAttackPrefab);
            }
            MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.commonAttackPrefab, SceneObjType.ActionRes);
        }

        public override void UnInit()
        {
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_SelectTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnSelectTarget));
            Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<SelectTargetEventParam>(GameSkillEventDef.Event_ClearTarget, new GameSkillEvent<SelectTargetEventParam>(this.OnClearTarget));
            Singleton<GameEventSys>.GetInstance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.OnFightPrepare));
            Singleton<GameEventSys>.GetInstance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.OnFightOver));
            Singleton<GameEventSys>.GetInstance().RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
        }
    }
}

