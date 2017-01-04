namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.DataCenter;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class GameObjMgr : Singleton<GameObjMgr>, IUpdateLogic
    {
        private uint _newActorID;
        public Dictionary<int, List<ActorRoot>> CachedActors = new Dictionary<int, List<ActorRoot>>();
        public GameObject cachedRoot;
        private List<PoolObjHandle<ActorRoot>>[] CampsActors = new List<PoolObjHandle<ActorRoot>>[3];
        private List<PoolObjHandle<ActorRoot>>[] CampsBullet = new List<PoolObjHandle<ActorRoot>>[3];
        private List<KeyValuePair<uint, int>> DelayRecycle = new List<KeyValuePair<uint, int>>(10);
        public List<PoolObjHandle<ActorRoot>> DynamicActors = new List<PoolObjHandle<ActorRoot>>(10);
        private DefaultGameEventParam eventParamCache;
        public List<PoolObjHandle<ActorRoot>> FakeTrueEyes = new List<PoolObjHandle<ActorRoot>>(5);
        public List<PoolObjHandle<ActorRoot>> GameActors = new List<PoolObjHandle<ActorRoot>>(100);
        public List<PoolObjHandle<ActorRoot>> HeroActors = new List<PoolObjHandle<ActorRoot>>(10);
        public static bool isPreSpawnActors;
        public List<PoolObjHandle<ActorRoot>> OrganActors = new List<PoolObjHandle<ActorRoot>>(20);
        public List<PoolObjHandle<ActorRoot>> SoldierActors = new List<PoolObjHandle<ActorRoot>>(40);
        public List<PoolObjHandle<ActorRoot>> StaticActors = new List<PoolObjHandle<ActorRoot>>(20);
        public List<PoolObjHandle<ActorRoot>> TowerActors = new List<PoolObjHandle<ActorRoot>>(0x10);

        public void AddActor(PoolObjHandle<ActorRoot> actor)
        {
            StringHelper.ClearFormater();
            StringHelper.Formater.Append(actor.handle.ActorControl.GetTypeName());
            StringHelper.Formater.Append(actor.handle.ObjID);
            StringHelper.Formater.Append("(");
            StringHelper.Formater.Append(actor.handle.TheStaticData.TheResInfo.Name);
            StringHelper.Formater.Append(")");
            actor.handle.name = StringHelper.Formater.ToString();
            actor.handle.gameObject.name = actor.handle.name;
            this.GameActors.Add(actor);
            if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
            {
                this.HeroActors.Add(actor);
            }
            else if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
            {
                this.OrganActors.Add(actor);
                if ((actor.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 1) || (actor.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 4))
                {
                    this.TowerActors.Add(actor);
                }
            }
            else if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
            {
                MonsterWrapper wrapper = actor.handle.AsMonster();
                if (((wrapper != null) && (wrapper.cfgInfo != null)) && (wrapper.cfgInfo.bMonsterType == 1))
                {
                    this.SoldierActors.Add(actor);
                }
            }
            else if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE)
            {
                this.FakeTrueEyes.Add(actor);
            }
            actor.handle.isRecycled = false;
            this.CampsActors[(int) actor.handle.TheActorMeta.ActorCamp].Add(actor);
        }

        public void AddBullet(ref PoolObjHandle<ActorRoot> actor)
        {
            this.CampsBullet[(int) actor.handle.TheActorMeta.ActorCamp].Add(actor);
        }

        public bool AddToCache(PoolObjHandle<ActorRoot> actor)
        {
            ActorRoot handle = actor.handle;
            if (((actor == 0) || (handle.TheActorMeta.ConfigId == 0)) || (handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Monster))
            {
                return false;
            }
            int configId = handle.TheActorMeta.ConfigId;
            List<ActorRoot> list = null;
            if (!this.CachedActors.TryGetValue(configId, out list))
            {
                list = new List<ActorRoot>();
                this.CachedActors.Add(configId, list);
            }
            handle.DeactiveActor();
            list.Add(handle);
            if (this.cachedRoot == null)
            {
                this.cachedRoot = new GameObject();
                this.cachedRoot.name = "CachedActorRoot";
            }
            handle.isRecycled = true;
            handle.gameObject.transform.parent = this.cachedRoot.gameObject.transform;
            return true;
        }

        public void ClearActor()
        {
            Dictionary<int, List<ActorRoot>>.Enumerator enumerator = this.CachedActors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<int, List<ActorRoot>> current = enumerator.Current;
                List<ActorRoot> list = current.Value;
                for (int num = 0; num < list.Count; num++)
                {
                    ActorRoot root = list[num];
                    GameObject gameObject = root.gameObject;
                    root.ObjLinker.DetachActorRoot();
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(gameObject);
                }
                list.Clear();
            }
            this.CachedActors.Clear();
            int count = this.GameActors.Count;
            for (int i = 0; i < count; i++)
            {
                DebugHelper.Assert(this.GameActors[i]);
                PoolObjHandle<ActorRoot> handle = this.GameActors[i];
                if (!handle.handle.ObjLinker.isStatic)
                {
                    PoolObjHandle<ActorRoot> handle2 = this.GameActors[i];
                    GameObject pooledGameObject = handle2.handle.gameObject;
                    PoolObjHandle<ActorRoot> handle3 = this.GameActors[i];
                    handle3.handle.ObjLinker.DetachActorRoot();
                    Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(pooledGameObject);
                }
                else
                {
                    PoolObjHandle<ActorRoot> handle4 = this.GameActors[i];
                    GameObject obj4 = handle4.handle.gameObject;
                    PoolObjHandle<ActorRoot> handle5 = this.GameActors[i];
                    handle5.handle.ObjLinker.DetachActorRoot();
                    UnityEngine.Object.DestroyObject(obj4);
                }
            }
            this.GameActors.Clear();
            for (int j = 0; j < this.StaticActors.Count; j++)
            {
                DebugHelper.Assert(count == 0);
                PoolObjHandle<ActorRoot> handle6 = this.StaticActors[j];
                GameObject obj5 = handle6.handle.gameObject;
                PoolObjHandle<ActorRoot> handle7 = this.StaticActors[j];
                handle7.handle.ObjLinker.DetachActorRoot();
                UnityEngine.Object.DestroyObject(obj5);
            }
            this.StaticActors.Clear();
            for (int k = 0; k < this.DynamicActors.Count; k++)
            {
                DebugHelper.Assert(count == 0);
                PoolObjHandle<ActorRoot> handle8 = this.DynamicActors[k];
                GameObject obj6 = handle8.handle.gameObject;
                PoolObjHandle<ActorRoot> handle9 = this.DynamicActors[k];
                handle9.handle.ObjLinker.DetachActorRoot();
                Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(obj6);
            }
            this.DynamicActors.Clear();
            this.HeroActors.Clear();
            this.OrganActors.Clear();
            this.TowerActors.Clear();
            this.SoldierActors.Clear();
            this.FakeTrueEyes.Clear();
            for (int m = 0; m < 3; m++)
            {
                this.CampsActors[m].Clear();
            }
            this.DelayRecycle.Clear();
            this.NewActorID = 0;
            for (int n = 0; n < 3; n++)
            {
                this.CampsBullet[n].Clear();
            }
        }

        public void DestroyActor(uint ObjID)
        {
            <DestroyActor>c__AnonStorey63 storey = new <DestroyActor>c__AnonStorey63 {
                ObjID = ObjID
            };
            int index = 0;
            while (index < this.GameActors.Count)
            {
                PoolObjHandle<ActorRoot> actor = this.GameActors[index];
                ActorRoot handle = actor.handle;
                if (handle.ObjID == storey.ObjID)
                {
                    this.eventParamCache.src = actor;
                    Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorDestroy, ref this.eventParamCache);
                    int num2 = this.HeroActors.FindIndex(new Predicate<PoolObjHandle<ActorRoot>>(storey.<>m__39));
                    if (num2 >= 0)
                    {
                        this.HeroActors.RemoveAt(num2);
                    }
                    num2 = this.OrganActors.FindIndex(new Predicate<PoolObjHandle<ActorRoot>>(storey.<>m__3A));
                    if (num2 >= 0)
                    {
                        this.OrganActors.RemoveAt(num2);
                    }
                    num2 = this.TowerActors.FindIndex(new Predicate<PoolObjHandle<ActorRoot>>(storey.<>m__3B));
                    if (num2 >= 0)
                    {
                        this.TowerActors.RemoveAt(num2);
                    }
                    num2 = this.SoldierActors.FindIndex(new Predicate<PoolObjHandle<ActorRoot>>(storey.<>m__3C));
                    if (num2 >= 0)
                    {
                        this.SoldierActors.RemoveAt(num2);
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        num2 = this.CampsActors[i].FindIndex(new Predicate<PoolObjHandle<ActorRoot>>(storey.<>m__3D));
                        if (num2 >= 0)
                        {
                            this.CampsActors[i].RemoveAt(num2);
                        }
                    }
                    num2 = this.FakeTrueEyes.FindIndex(new Predicate<PoolObjHandle<ActorRoot>>(storey.<>m__3E));
                    if (num2 >= 0)
                    {
                        this.FakeTrueEyes.RemoveAt(num2);
                    }
                    if (!this.AddToCache(actor))
                    {
                        GameObject gameObject = handle.gameObject;
                        if (handle.ObjLinker != null)
                        {
                            handle.ObjLinker.DetachActorRoot();
                        }
                        Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(gameObject);
                    }
                    this.GameActors.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }

        public bool DestroyEye(PoolObjHandle<ActorRoot> inEye)
        {
            if (inEye == 0)
            {
                return false;
            }
            int count = this.DelayRecycle.Count;
            int index = -1;
            for (int i = 0; i < count; i++)
            {
                KeyValuePair<uint, int> pair = this.DelayRecycle[i];
                if (pair.Key == inEye.handle.ObjID)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                this.DelayRecycle.RemoveAt(index);
            }
            this.RecycleActor(inEye, 0);
            return true;
        }

        public void FightOver()
        {
            int count = this.GameActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = this.GameActors[i];
                handle.handle.FightOver();
            }
        }

        public PoolObjHandle<ActorRoot> GetActor(ActorFilterDelegate predicate)
        {
            for (int i = 0; i < this.GameActors.Count; i++)
            {
                PoolObjHandle<ActorRoot> actor = this.GameActors[i];
                if ((predicate == null) || predicate(ref actor))
                {
                    return actor;
                }
            }
            return new PoolObjHandle<ActorRoot>(null);
        }

        public PoolObjHandle<ActorRoot> GetActor(uint ObjID)
        {
            for (int i = 0; i < this.GameActors.Count; i++)
            {
                PoolObjHandle<ActorRoot> handle = this.GameActors[i];
                if (handle.handle.ObjID == ObjID)
                {
                    return handle;
                }
            }
            return new PoolObjHandle<ActorRoot>();
        }

        public List<PoolObjHandle<ActorRoot>> GetCampActors(COM_PLAYERCAMP cmp)
        {
            return this.CampsActors[(int) cmp];
        }

        public List<PoolObjHandle<ActorRoot>> GetCampBullet(COM_PLAYERCAMP camp)
        {
            return this.CampsBullet[(int) camp];
        }

        public int GetHeroMaxLevel()
        {
            int actorSoulLevel = 1;
            for (int i = 0; i < this.HeroActors.Count; i++)
            {
                if (this.HeroActors[i] != 0)
                {
                    PoolObjHandle<ActorRoot> handle = this.HeroActors[i];
                    if (handle.handle.ValueComponent != null)
                    {
                        PoolObjHandle<ActorRoot> handle2 = this.HeroActors[i];
                        if (actorSoulLevel < handle2.handle.ValueComponent.actorSoulLevel)
                        {
                            PoolObjHandle<ActorRoot> handle3 = this.HeroActors[i];
                            actorSoulLevel = handle3.handle.ValueComponent.actorSoulLevel;
                        }
                    }
                }
            }
            return actorSoulLevel;
        }

        public void HoldDynamicActor(PoolObjHandle<ActorRoot> actor)
        {
            DebugHelper.Assert(!actor.handle.ObjLinker.isStatic);
            this.DynamicActors.Add(actor);
        }

        public void HoldStaticActor(PoolObjHandle<ActorRoot> actor)
        {
            DebugHelper.Assert(actor.handle.ObjLinker.isStatic);
            this.StaticActors.Add(actor);
        }

        public override void Init()
        {
            this.NewActorID = 0;
            for (int i = 0; i < 3; i++)
            {
                this.CampsActors[i] = new List<PoolObjHandle<ActorRoot>>(50);
            }
            for (int j = 0; j < 3; j++)
            {
                this.CampsBullet[j] = new List<PoolObjHandle<ActorRoot>>(50);
            }
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.onActorDamage));
            this.eventParamCache = new DefaultGameEventParam(new PoolObjHandle<ActorRoot>(null), new PoolObjHandle<ActorRoot>(null));
        }

        public void KillSoldiers()
        {
            List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = this.SoldierActors.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != 0)
                {
                    PoolObjHandle<ActorRoot> current = enumerator.Current;
                    current.handle.ValueComponent.actorHp = 0;
                }
            }
        }

        public void LateUpdate()
        {
            int count = this.GameActors.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.GameActors[i] != 0)
                {
                    PoolObjHandle<ActorRoot> handle = this.GameActors[i];
                    handle.handle.LateUpdate();
                }
            }
        }

        public void onActorDamage(ref HurtEventResultInfo info)
        {
            if (((info.hurtInfo.hurtType != HurtTypeDef.Therapic) && (info.hurtInfo.hurtCount == 0)) && (ActorHelper.IsHostCtrlActor(ref info.atker) && (info.src.handle.MatHurtEffect != null)))
            {
                info.src.handle.MatHurtEffect.PlayHurtEffect();
            }
        }

        public void onActorDead(ref GameDeadEventParam prm)
        {
        }

        public void PrepareFight()
        {
            int count = this.StaticActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = this.StaticActors[i];
                handle.handle.InitActor();
                PoolObjHandle<ActorRoot> handle2 = this.StaticActors[i];
                handle2.handle.PrepareFight();
            }
            int num3 = this.DynamicActors.Count;
            for (int j = 0; j < num3; j++)
            {
                PoolObjHandle<ActorRoot> handle3 = this.DynamicActors[j];
                handle3.handle.InitActor();
                PoolObjHandle<ActorRoot> handle4 = this.DynamicActors[j];
                handle4.handle.PrepareFight();
            }
            if (Singleton<BattleLogic>.GetInstance().m_GameInfo.gameContext.levelContext.IsGameTypeBurning())
            {
                BurnExpeditionUT.ApplyHP2Game(this.DynamicActors);
            }
        }

        public int QueryEyeLeftTime(PoolObjHandle<ActorRoot> inEye)
        {
            if (inEye != 0)
            {
                int count = this.DelayRecycle.Count;
                for (int i = 0; i < count; i++)
                {
                    KeyValuePair<uint, int> pair = this.DelayRecycle[i];
                    if (pair.Key == inEye.handle.ObjID)
                    {
                        KeyValuePair<uint, int> pair2 = this.DelayRecycle[i];
                        return pair2.Value;
                    }
                }
            }
            return -1;
        }

        public void RecycleActor(PoolObjHandle<ActorRoot> actor, int delay = 0)
        {
            this.DelayRecycle.Add(new KeyValuePair<uint, int>(actor.handle.ObjID, delay));
        }

        public void RmvBullet(ref PoolObjHandle<ActorRoot> actor)
        {
            this.CampsBullet[(int) actor.handle.TheActorMeta.ActorCamp].Remove(actor);
        }

        public PoolObjHandle<ActorRoot> SpawnActorEx(GameObject rootObj, ref ActorMeta actorMeta, VInt3 pos, VInt3 dir, bool useLobbyModel, bool addComponent)
        {
            if (actorMeta.Difficuty == 0)
            {
                actorMeta.Difficuty = (byte) Singleton<BattleLogic>.instance.GetCurLvelContext().m_levelDifficulty;
            }
            ActorStaticData actorData = new ActorStaticData();
            Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticBattleDataProvider).GetActorStaticData(ref actorMeta, ref actorData);
            ActorServerData data2 = new ActorServerData();
            Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider).GetActorServerData(ref actorMeta, ref data2);
            if (actorMeta.SkinID == 0)
            {
                actorMeta.SkinID = data2.SkinId;
            }
            ActorConfig component = null;
            GameObject gameObject = null;
            if (rootObj == null)
            {
                rootObj = MonoSingleton<SceneMgr>.GetInstance().Spawn(typeof(ActorRoot).Name, (SceneObjType) actorData.TheActorMeta.ActorType, pos, dir);
                component = rootObj.GetComponent<ActorConfig>();
            }
            else
            {
                component = rootObj.GetComponent<ActorConfig>();
                Animation componentInChildren = rootObj.GetComponentInChildren<Animation>();
                if (componentInChildren != null)
                {
                    gameObject = componentInChildren.gameObject;
                }
                rootObj.transform.parent = MonoSingleton<SceneMgr>.GetInstance().GetRoot((SceneObjType) actorData.TheActorMeta.ActorType).transform;
            }
            component.ConfigID = actorMeta.ConfigId;
            CActorInfo actorInfo = null;
            if (!string.IsNullOrEmpty(actorData.TheResInfo.ResPath))
            {
                CActorInfo original = CActorInfo.GetActorInfo(actorData.TheResInfo.ResPath, enResourceType.BattleScene);
                if (original != null)
                {
                    actorInfo = (CActorInfo) UnityEngine.Object.Instantiate(original);
                    string str = !useLobbyModel ? actorInfo.GetArtPrefabName((int) actorMeta.SkinID, -1) : actorInfo.GetArtPrefabNameLobby((int) actorMeta.SkinID);
                    if ((gameObject == null) && !string.IsNullOrEmpty(str))
                    {
                        bool isInit = false;
                        gameObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(str, false, SceneObjType.ActionRes, Vector3.zero, Quaternion.identity, out isInit);
                        if (gameObject != null)
                        {
                            Transform transform = gameObject.GetComponent<Transform>();
                            transform.SetParent(rootObj.transform);
                            transform.localPosition = Vector3.zero;
                            transform.localRotation = Quaternion.identity;
                            TransformConfig transformConfigIfHaveOne = actorInfo.GetTransformConfigIfHaveOne(ETransformConfigUsage.CharacterInGame);
                            if (transformConfigIfHaveOne != null)
                            {
                                transform.localPosition += transformConfigIfHaveOne.Offset;
                                transform.localScale = (Vector3) (transform.localScale * transformConfigIfHaveOne.Scale);
                            }
                        }
                    }
                }
            }
            PoolObjHandle<ActorRoot> handle = component.AttachActorRoot(rootObj, ref actorMeta, actorInfo);
            handle.handle.TheStaticData = actorData;
            if (addComponent)
            {
                handle.handle.Spawned();
            }
            return handle;
        }

        public void StartFight()
        {
            int count = this.StaticActors.Count;
            for (int i = 0; i < count; i++)
            {
                this.AddActor(this.StaticActors[i]);
            }
            int num3 = this.DynamicActors.Count;
            for (int j = 0; j < num3; j++)
            {
                this.AddActor(this.DynamicActors[j]);
            }
            int num5 = this.GameActors.Count;
            for (int k = 0; k < num5; k++)
            {
                PoolObjHandle<ActorRoot> handle = this.GameActors[k];
                handle.handle.StartFight();
            }
            if (Singleton<BattleLogic>.GetInstance().m_GameInfo.gameContext.levelContext.IsGameTypeBurning())
            {
                int buffid = (int) Singleton<BurnExpeditionController>.GetInstance().model.Get_CurSelected_BuffId();
                BurnExpeditionUT.ApplyBuff(this.DynamicActors, buffid);
                if (Singleton<CBattleSystem>.instance.FightForm != null)
                {
                    Singleton<CBattleSystem>.instance.FightForm.GetBattleMisc().Show_BuffCD(0, BurnExpeditionUT.Get_Buff_CDTime(buffid));
                }
            }
            this.StaticActors.Clear();
            this.DynamicActors.Clear();
        }

        public bool TryGetFromCache(ref PoolObjHandle<ActorRoot> actor, ref ActorMeta actorMeta)
        {
            List<ActorRoot> list = null;
            if (!this.CachedActors.TryGetValue(actorMeta.ConfigId, out list))
            {
                return false;
            }
            if (list.Count == 0)
            {
                return false;
            }
            int index = list.Count - 1;
            actor = new PoolObjHandle<ActorRoot>(list[index]);
            list.RemoveAt(index);
            return true;
        }

        public override void UnInit()
        {
            this.ClearActor();
            base.UnInit();
        }

        public void UpdateLogic(int delta)
        {
            int count = this.GameActors.Count;
            for (int i = 0; i < count; i++)
            {
                PoolObjHandle<ActorRoot> handle = this.GameActors[i];
                handle.handle.UpdateLogic(delta);
            }
            int index = 0;
            while (index < this.DelayRecycle.Count)
            {
                KeyValuePair<uint, int> pair = this.DelayRecycle[index];
                uint key = pair.Key;
                KeyValuePair<uint, int> pair2 = this.DelayRecycle[index];
                int num5 = pair2.Value - delta;
                if (num5 <= 0)
                {
                    this.DestroyActor(key);
                    this.DelayRecycle.RemoveAt(index);
                }
                else
                {
                    this.DelayRecycle[index] = new KeyValuePair<uint, int>(key, num5);
                    index++;
                }
            }
        }

        public uint NewActorID
        {
            get
            {
                this._newActorID++;
                return this._newActorID;
            }
            private set
            {
                this._newActorID = value;
            }
        }

        [CompilerGenerated]
        private sealed class <DestroyActor>c__AnonStorey63
        {
            internal uint ObjID;

            internal bool <>m__39(PoolObjHandle<ActorRoot> item)
            {
                return (item.handle.ObjID == this.ObjID);
            }

            internal bool <>m__3A(PoolObjHandle<ActorRoot> item)
            {
                return (item.handle.ObjID == this.ObjID);
            }

            internal bool <>m__3B(PoolObjHandle<ActorRoot> item)
            {
                return (item.handle.ObjID == this.ObjID);
            }

            internal bool <>m__3C(PoolObjHandle<ActorRoot> item)
            {
                return (item.handle.ObjID == this.ObjID);
            }

            internal bool <>m__3D(PoolObjHandle<ActorRoot> item)
            {
                return (item.handle.ObjID == this.ObjID);
            }

            internal bool <>m__3E(PoolObjHandle<ActorRoot> item)
            {
                return (item.handle.ObjID == this.ObjID);
            }
        }
    }
}

