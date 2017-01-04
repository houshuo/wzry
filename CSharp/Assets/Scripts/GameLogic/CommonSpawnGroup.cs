namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    public class CommonSpawnGroup : CommonSpawnPoint
    {
        public int AlertPreroll;
        [FriendlyName("需要触发器")]
        public bool bTriggerSpawn;
        private CommonSpawnPoint[] drawPoints;
        protected Color GroupColor = new Color(0.2f, 0.8f, 0.2f);
        public EIntervalRule IntervalRule;
        private bool m_bCountingSpawn;
        private bool m_bStarted;
        private int m_spawnCounter;
        private int m_spawnTimer;
        [FriendlyName("生成间隔")]
        public int SpawnInternval = 0x2710;
        [FriendlyName("生成总次数")]
        public int SpawnTimes;
        [FriendlyName("第一次生成延迟")]
        public int StartUpDelay = 0x1388;

        protected override void DecSpawnPointOver()
        {
            base.DecSpawnPointOver();
            if (base.m_spawnPointOver == 0)
            {
                if (this.IntervalRule == EIntervalRule.UsedUp)
                {
                    this.bCountingSpawn = true;
                }
                SGroupDeadEventParam prm = new SGroupDeadEventParam {
                    csg = this
                };
                Singleton<GameEventSys>.instance.SendEvent<SGroupDeadEventParam>(GameEventDef.Event_SpawnGroupDead, ref prm);
                if ((this.m_spawnCounter == 0) && (this.SpawnTimes > 0))
                {
                    this.GoNextGroup();
                }
            }
        }

        private CommonSpawnPoint[] FindChildrenPoints()
        {
            return base.GetComponentsInChildren<CommonSpawnPoint>();
        }

        public int GetSpawnCounter()
        {
            return this.m_spawnCounter;
        }

        public int GetSpawnTimer()
        {
            return this.m_spawnTimer;
        }

        protected virtual void GoNextGroup()
        {
            this.m_bStarted = false;
        }

        protected override void OnDrawGizmos()
        {
            Gizmos.color = this.GroupColor;
            Gizmos.DrawSphere(base.transform.position, 0.3f);
            this.drawPoints = this.FindChildrenPoints();
            if ((this.drawPoints != null) && (this.drawPoints.Length > 0))
            {
                Gizmos.color = this.GroupColor;
                for (int i = 0; i < (this.drawPoints.Length - 1); i++)
                {
                    Vector3 position = this.drawPoints[0].gameObject.transform.position;
                    Vector3 a = this.drawPoints[i + 1].gameObject.transform.position;
                    Vector3 vector4 = a - position;
                    Vector3 normalized = vector4.normalized;
                    float num2 = (Vector3.Distance(a, position) - this.drawPoints[i + 1].radius) - this.drawPoints[0].radius;
                    position += (Vector3) (normalized * this.drawPoints[0].radius);
                    a = position + ((Vector3) (normalized * num2));
                    Gizmos.DrawLine(position, a);
                    this.drawPoints[i + 1].PointColor = this.GroupColor;
                }
                Gizmos.DrawIcon(new Vector3(this.drawPoints[0].transform.position.x, this.drawPoints[0].transform.position.y + (this.drawPoints[0].radius * 3f), this.drawPoints[0].transform.position.z), "EndPoint", true);
            }
        }

        protected virtual SpawnerWrapper.ESpawnObjectType QuerySpawnObjType()
        {
            SpawnerWrapper.ESpawnObjectType invalid = SpawnerWrapper.ESpawnObjectType.Invalid;
            if (base.SpawnerList.Length > 0)
            {
                invalid = base.SpawnerList[0].SpawnType;
            }
            return invalid;
        }

        protected override void Start()
        {
            base.Start();
            for (CommonSpawnPoint point = base.NextPoint; point != null; point = point.NextPoint)
            {
                base.m_spawnPointList.Add(point);
                point.onAllDeadEvt += new CommonSpawnPointAllDead(this.onSpawnPointAllDead);
            }
        }

        public override void Startup()
        {
            if (!this.bTriggerSpawn && !this.m_bStarted)
            {
                this.m_spawnTimer = this.StartUpDelay;
                this.m_spawnCounter = this.SpawnTimes;
                this.bCountingSpawn = true;
                this.m_bStarted = true;
            }
            base.Startup();
        }

        public void TriggerStartUp()
        {
            if (!this.m_bStarted)
            {
                this.m_spawnTimer = this.StartUpDelay;
                this.m_spawnCounter = this.SpawnTimes;
                this.bCountingSpawn = true;
                this.m_bStarted = true;
            }
        }

        public override void UpdateLogic(int delta)
        {
            if ((this.m_bStarted && this.bCountingSpawn) && ((this.SpawnTimes <= 0) || (this.m_spawnCounter > 0)))
            {
                this.m_spawnTimer -= delta;
                if (this.m_spawnTimer <= 0)
                {
                    this.m_spawnTimer = this.SpawnInternval;
                    this.DoSpawn();
                    SCommonSpawnEventParam prm = new SCommonSpawnEventParam((VInt3) base.gameObject.transform.position, this.m_spawnTimer, this.AlertPreroll, this.QuerySpawnObjType());
                    Singleton<GameEventSys>.instance.SendEvent<SCommonSpawnEventParam>(GameEventDef.Event_SpawnGroupSpawn, ref prm);
                    if (this.IntervalRule == EIntervalRule.UsedUp)
                    {
                        this.bCountingSpawn = false;
                    }
                    this.m_spawnCounter--;
                }
            }
        }

        public bool bCountingSpawn
        {
            get
            {
                return this.m_bCountingSpawn;
            }
            private set
            {
                bool bCountingSpawn = this.m_bCountingSpawn;
                this.m_bCountingSpawn = value;
                if (!bCountingSpawn && this.m_bCountingSpawn)
                {
                    SCommonSpawnEventParam prm = new SCommonSpawnEventParam((VInt3) base.gameObject.transform.position, this.m_spawnTimer, this.AlertPreroll, this.QuerySpawnObjType());
                    Singleton<GameEventSys>.instance.SendEvent<SCommonSpawnEventParam>(GameEventDef.Event_SpawnGroupStartCount, ref prm);
                }
            }
        }

        public enum EIntervalRule
        {
            UsedUp,
            FixedInterval
        }
    }
}

