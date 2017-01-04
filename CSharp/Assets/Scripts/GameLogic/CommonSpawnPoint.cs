namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class CommonSpawnPoint : FuncRegion
    {
        protected ArrayList m_spawnedList = new ArrayList();
        protected ArrayList m_spawnPointList = new ArrayList();
        protected int m_spawnPointOver;
        public CommonSpawnPoint NextPoint;
        [HideInInspector]
        public Color PointColor = new Color(1f, 1f, 1f);
        [HideInInspector]
        public float radius = 0.25f;
        [FriendlyName("生成位置随机")]
        public bool RandomPos;
        [FriendlyName("随机位置内半径")]
        public int RandomPosRadiusInner;
        [FriendlyName("随机位置外半径")]
        public int RandomPosRadiusOuter;
        [SerializeField]
        public SpawnerWrapper[] SpawnerList = new SpawnerWrapper[0];
        public ESpawnStyle SpawnStyle;

        public event CommonSpawnPointAllDead onAllDeadEvt;

        public event OnCommonPointAllSpawned onAllSpawned;

        private void Awake()
        {
            foreach (SpawnerWrapper wrapper in this.SpawnerList)
            {
                if (wrapper != null)
                {
                    wrapper.Init();
                }
            }
        }

        protected void CalcPosDir(out VInt3 outPos, out VInt3 outDir)
        {
            outPos = (VInt3) base.gameObject.transform.position;
            outDir = (VInt3) base.gameObject.transform.forward;
            if (this.RandomPos)
            {
                int num = 0;
                int num2 = FrameRandom.Random(0x7d1);
                if (num2 > 0x3e8)
                {
                    num2 -= 0x3e8;
                    num2 = -num2;
                }
                int num3 = FrameRandom.Random(0x7d1);
                if (num3 > 0x3e8)
                {
                    num3 -= 0x3e8;
                    num3 = -num3;
                }
                VInt3 num4 = new VInt3(num2, num, num3);
                Vector3 vector = (Vector3) num4;
                vector.Normalize();
                int num5 = 0;
                if ((this.RandomPosRadiusOuter - this.RandomPosRadiusInner) > 0)
                {
                    num5 = FrameRandom.Random((uint) (this.RandomPosRadiusOuter - this.RandomPosRadiusInner)) + this.RandomPosRadiusInner;
                }
                float num6 = num5 * 0.001f;
                Vector3 vector2 = ((Vector3) outPos) + ((Vector3) (vector * num6));
                outPos = (VInt3) vector2;
                outDir = num4.NormalizeTo(0x3e8);
            }
        }

        protected virtual void DecSpawnPointOver()
        {
            if ((--this.m_spawnPointOver == 0) && (this.onAllDeadEvt != null))
            {
                this.onAllDeadEvt(this);
            }
        }

        public virtual void DoSpawn()
        {
            switch (this.SpawnStyle)
            {
                case ESpawnStyle.AllPoints:
                    this.DoSpawnSelf();
                    this.DoSpawnSubPointsAll();
                    this.m_spawnPointOver += this.m_spawnPointList.Count + 1;
                    goto Label_00B7;

                case ESpawnStyle.RandomSubPoint:
                {
                    int num = FrameRandom.Random((uint) (this.m_spawnPointList.Count + 1));
                    if (num != 0)
                    {
                        this.DoSpawnSubPoint(this.m_spawnPointList[num - 1] as CommonSpawnPoint);
                        break;
                    }
                    this.DoSpawnSelf();
                    break;
                }
                case ESpawnStyle.Self:
                    this.DoSpawnSelf();
                    this.m_spawnPointOver++;
                    goto Label_00B7;

                default:
                    goto Label_00B7;
            }
            this.m_spawnPointOver++;
        Label_00B7:
            if (this.onAllSpawned != null)
            {
                this.onAllSpawned(this);
            }
        }

        protected virtual void DoSpawnSelf()
        {
            VInt3 zero = VInt3.zero;
            VInt3 forward = VInt3.forward;
            this.CalcPosDir(out zero, out forward);
            foreach (SpawnerWrapper wrapper in this.SpawnerList)
            {
                if (wrapper != null)
                {
                    this.m_spawnedList.Add(wrapper.DoSpawn(zero, forward, base.gameObject));
                }
            }
        }

        private void DoSpawnSubPoint(CommonSpawnPoint subsp)
        {
            if (subsp != null)
            {
                subsp.DoSpawn();
            }
        }

        private void DoSpawnSubPointsAll()
        {
            IEnumerator enumerator = this.m_spawnPointList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CommonSpawnPoint current = enumerator.Current as CommonSpawnPoint;
                this.DoSpawnSubPoint(current);
            }
        }

        public ArrayList GetSpawnedList()
        {
            return this.m_spawnedList;
        }

        protected virtual void onActorDead(ref GameDeadEventParam prm)
        {
            int count = this.m_spawnedList.Count;
            int index = 0;
            while (index < this.m_spawnedList.Count)
            {
                if (this.m_spawnedList[index] == null)
                {
                    this.m_spawnedList.RemoveAt(index);
                }
                else
                {
                    if (this.m_spawnedList[index].Equals(prm.src.handle))
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
            }
        }

        protected void OnDestroy()
        {
            Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameEventSys>.instance.RmvEventHandler<STailsmanEventParam>(GameEventDef.Event_TailsmanUsed, new RefAction<STailsmanEventParam>(this.onTailsmanUsed));
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = this.PointColor;
            Gizmos.DrawSphere(base.transform.position, 0.15f);
        }

        protected void onMyselfAllDead()
        {
            this.DecSpawnPointOver();
        }

        protected void onSpawnPointAllDead(CommonSpawnPoint inSpawnPoint)
        {
            if (this.m_spawnPointList.Contains(inSpawnPoint))
            {
                this.DecSpawnPointOver();
            }
        }

        private void onTailsmanUsed(ref STailsmanEventParam prm)
        {
            int count = this.m_spawnedList.Count;
            this.m_spawnedList.Remove(prm.tailsman.handle);
            int num2 = this.m_spawnedList.Count;
            if ((num2 == 0) && (num2 < count))
            {
                this.onMyselfAllDead();
            }
        }

        public void PreLoadResource(ref ActorPreloadTab loadInfo, LoaderHelper loadHelper)
        {
            foreach (SpawnerWrapper wrapper in this.SpawnerList)
            {
                if (wrapper != null)
                {
                    wrapper.PreLoadResource(ref loadInfo, loadHelper);
                }
            }
        }

        public void PreLoadResource(ref List<ActorPreloadTab> list, LoaderHelper loadHelper)
        {
            foreach (SpawnerWrapper wrapper in this.SpawnerList)
            {
                if (wrapper != null)
                {
                    wrapper.PreLoadResource(ref list, loadHelper);
                }
            }
        }

        protected virtual void Start()
        {
            Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
            Singleton<GameEventSys>.instance.AddEventHandler<STailsmanEventParam>(GameEventDef.Event_TailsmanUsed, new RefAction<STailsmanEventParam>(this.onTailsmanUsed));
        }

        public override void UpdateLogic(int delta)
        {
        }

        public enum ESpawnStyle
        {
            AllPoints,
            RandomSubPoint,
            Self
        }
    }
}

