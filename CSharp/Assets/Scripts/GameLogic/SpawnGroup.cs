namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    public class SpawnGroup : SpawnPoint
    {
        public bool bTriggerSpawn;
        private SpawnPoint[] drawPoints;
        private Color GroupColor = new Color(0.8f, 0.1f, 0.1f, 0.8f);
        [HideInInspector]
        public int GroupId;
        private bool m_bCountingSpawn;
        private bool m_bStarted;
        private int m_spawnCounter;
        private int m_spawnTimer;
        public SpawnGroup[] NextGroups = new SpawnGroup[0];
        public int SpawnInternval = 0x2710;
        public int SpawnTimes;
        public int StartUpDelay = 0x1388;

        protected override void DecSpawnPointOver()
        {
            base.DecSpawnPointOver();
            if (base.m_spawnPointOver == 0)
            {
                this.m_bCountingSpawn = true;
                SGroupDeadEventParam prm = new SGroupDeadEventParam {
                    sg = this
                };
                Singleton<GameEventSys>.instance.SendEvent<SGroupDeadEventParam>(GameEventDef.Event_SpawnGroupDead, ref prm);
                if ((this.m_spawnCounter == 0) && (this.SpawnTimes > 0))
                {
                    this.m_bStarted = false;
                    foreach (SpawnGroup group in this.NextGroups)
                    {
                        if (group != null)
                        {
                            group.TriggerStartUp();
                        }
                    }
                }
            }
        }

        private SpawnPoint[] FindChildrenPoints()
        {
            return base.GetComponentsInChildren<SpawnPoint>();
        }

        public int GetSpawnCounter()
        {
            return this.m_spawnCounter;
        }

        public int GetSpawnTimer()
        {
            return this.m_spawnTimer;
        }

        public bool IsCountingDown()
        {
            return this.m_bCountingSpawn;
        }

        private void OnDrawGizmos()
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

        protected override void Start()
        {
            for (SpawnPoint point = base.NextPoint; point != null; point = point.NextPoint)
            {
                base.m_spawnPointList.Add(point);
                point.onAllDeadEvt += new SpawnPointAllDeadEvent(this.onSpawnPointAllDead);
            }
            base.Start();
        }

        public override void Startup()
        {
            if (!this.bTriggerSpawn && !this.m_bStarted)
            {
                this.m_spawnTimer = this.StartUpDelay;
                this.m_spawnCounter = this.SpawnTimes;
                this.m_bCountingSpawn = true;
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
                this.m_bCountingSpawn = true;
                this.m_bStarted = true;
            }
        }

        public override void UpdateLogic(int delta)
        {
            if ((this.m_bStarted && this.m_bCountingSpawn) && ((this.SpawnTimes <= 0) || (this.m_spawnCounter > 0)))
            {
                this.m_spawnTimer -= delta;
                if (this.m_spawnTimer <= 0)
                {
                    this.m_spawnTimer = this.SpawnInternval;
                    base.DoSpawn();
                    this.m_bCountingSpawn = false;
                    this.m_spawnCounter--;
                }
            }
        }
    }
}

