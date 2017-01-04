namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic.DataCenter;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class SpawnPoint : FuncRegion
    {
        [FriendlyName("Meta信息非随机")]
        public bool bSequentialMeta;
        public int[] InitBuffDemand = new int[0];
        [FriendlyName("随机被动技能规则")]
        public int InitRandPassSkillRule;
        public GameObject m_rangeDeadPoint;
        public GeoPolygon m_rangePolygon;
        private List<PoolObjHandle<ActorRoot>> m_spawnedList = new List<PoolObjHandle<ActorRoot>>();
        protected List<SpawnPoint> m_spawnPointList = new List<SpawnPoint>();
        protected int m_spawnPointOver;
        public SpawnPoint NextPoint;
        [HideInInspector]
        public Color PointColor = new Color(1f, 0f, 0f);
        [HideInInspector]
        public float radius = 0.25f;
        public ActorMeta[] TheActorsMeta = new ActorMeta[1];

        public event SpawnPointAllDeadEvent onAllDeadEvt;

        public event OnAllSpawned onAllSpawned;

        protected virtual void DecSpawnPointOver()
        {
            if ((--this.m_spawnPointOver == 0) && (this.onAllDeadEvt != null))
            {
                this.onAllDeadEvt(this);
            }
        }

        public void DoSpawn()
        {
            DoSpawn(base.gameObject, (VInt3) base.gameObject.transform.position, (VInt3) base.gameObject.transform.forward, this.bSequentialMeta, this.TheActorsMeta, this.m_rangePolygon, this.m_rangeDeadPoint, this.InitBuffDemand, this.InitRandPassSkillRule, ref this.m_spawnedList);
            IEnumerator enumerator = this.m_spawnPointList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SpawnPoint current = enumerator.Current as SpawnPoint;
                if (current != null)
                {
                    current.DoSpawn();
                }
            }
            this.m_spawnPointOver = this.m_spawnPointList.Count + 1;
            if (this.onAllSpawned != null)
            {
                this.onAllSpawned(this);
            }
        }

        public static void DoSpawn(GameObject inGameObj, VInt3 bornPos, VInt3 bornDir, bool bInSeqMeta, ActorMeta[] inActorMetaList, GeoPolygon inPolygon, GameObject inDeadPoint, int[] inBuffDemand, int inRandPassSkillRule, ref List<PoolObjHandle<ActorRoot>> inSpawnedList)
        {
            if (((inGameObj != null) && (inActorMetaList != null)) && ((inActorMetaList.Length != 0) && (inSpawnedList != null)))
            {
                List<ActorMeta> list = new List<ActorMeta>();
                if (bInSeqMeta)
                {
                    list.AddRange(inActorMetaList);
                }
                else if (inActorMetaList.Length > 0)
                {
                    int length = inActorMetaList.Length;
                    int index = FrameRandom.Random((uint) length);
                    list.Add(inActorMetaList[index]);
                }
                List<ActorMeta>.Enumerator enumerator = list.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ActorMeta current = enumerator.Current;
                    if ((current.ConfigId > 0) && (current.ActorType != ActorTypeDef.Invalid))
                    {
                        PoolObjHandle<ActorRoot> actor = new PoolObjHandle<ActorRoot>();
                        if (!Singleton<GameObjMgr>.GetInstance().TryGetFromCache(ref actor, ref current))
                        {
                            actor = Singleton<GameObjMgr>.GetInstance().SpawnActorEx(null, ref current, bornPos, bornDir, false, true);
                            if (actor != 0)
                            {
                                actor.handle.InitActor();
                                actor.handle.PrepareFight();
                                Singleton<GameObjMgr>.instance.AddActor(actor);
                                actor.handle.StartFight();
                            }
                        }
                        else
                        {
                            actor.handle.ReactiveActor(bornPos, bornDir);
                        }
                        if (actor != 0)
                        {
                            actor.handle.ObjLinker.Invincible = current.Invincible;
                            actor.handle.ObjLinker.CanMovable = !current.NotMovable;
                            if ((inPolygon != null) && (inDeadPoint != null))
                            {
                                actor.handle.ActorControl.m_rangePolygon = inPolygon;
                                actor.handle.ActorControl.m_deadPointGo = inDeadPoint;
                            }
                            inSpawnedList.Add(actor);
                            if (inBuffDemand != null)
                            {
                                foreach (int num3 in inBuffDemand)
                                {
                                    new BufConsumer(num3, actor, actor).Use();
                                }
                            }
                            if ((inRandPassSkillRule > 0) && (actor.handle.SkillControl != null))
                            {
                                actor.handle.SkillControl.InitRandomSkill(inRandPassSkillRule);
                            }
                        }
                    }
                }
            }
        }

        public List<PoolObjHandle<ActorRoot>> GetSpawnedList()
        {
            return this.m_spawnedList;
        }

        protected void onActorDead(ref GameDeadEventParam prm)
        {
            int count = this.m_spawnedList.Count;
            int index = 0;
            while (index < this.m_spawnedList.Count)
            {
                if (this.m_spawnedList[index] == 0)
                {
                    this.m_spawnedList.RemoveAt(index);
                }
                else
                {
                    PoolObjHandle<ActorRoot> handle = this.m_spawnedList[index];
                    if (handle.Equals(prm.src))
                    {
                        this.m_spawnedList.RemoveAt(index);
                        break;
                    }
                    index++;
                }
            }
            int num3 = this.m_spawnedList.Count;
            if ((num3 == 0) && (num3 < count))
            {
                this.onMyselfAllDead();
                Singleton<GameEventSys>.instance.SendEvent<GameDeadEventParam>(GameEventDef.Event_MonsterGroupDead, ref prm);
            }
        }

        protected void OnDestroy()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = this.PointColor;
            Gizmos.DrawSphere(base.transform.position, 0.15f);
        }

        private void onMyselfAllDead()
        {
            this.DecSpawnPointOver();
        }

        protected void onSpawnPointAllDead(SpawnPoint inSpawnPoint)
        {
            if (this.m_spawnPointList.Contains(inSpawnPoint))
            {
                this.DecSpawnPointOver();
            }
        }

        public void PreLoadResource(ref List<ActorPreloadTab> list, LoaderHelper loadHelper)
        {
            if (this.TheActorsMeta.Length > 0)
            {
                for (int i = 0; i < this.TheActorsMeta.Length; i++)
                {
                    ActorMeta actorMeta = this.TheActorsMeta[i];
                    if (actorMeta.ConfigId > 0)
                    {
                        loadHelper.AddPreloadActor(ref list, ref actorMeta, 1f, 0);
                    }
                }
            }
        }

        protected virtual void Start()
        {
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
        }

        public override void Startup()
        {
            base.Startup();
        }

        public override void UpdateLogic(int delta)
        {
        }
    }
}

