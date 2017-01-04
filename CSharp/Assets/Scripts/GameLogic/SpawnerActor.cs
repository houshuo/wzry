namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.DataCenter;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class SpawnerActor : SpawnerBase
    {
        public bool bSequentialMeta;
        public int[] InitBuffDemand;
        public int InitRandPassSkillRule;
        public GameObject m_rangeDeadPoint;
        public GeoPolygon m_rangePolygon;
        public ActorMeta TheActorMeta;

        public SpawnerActor(SpawnerWrapper inWrapper) : base(inWrapper)
        {
        }

        public override object DoSpawn(VInt3 inWorldPos, VInt3 inDir, GameObject inSpawnPoint)
        {
            List<PoolObjHandle<ActorRoot>> inSpawnedList = new List<PoolObjHandle<ActorRoot>>();
            ActorMeta[] inActorMetaList = new ActorMeta[] { this.TheActorMeta };
            SpawnPoint.DoSpawn(inSpawnPoint, inWorldPos, inDir, this.bSequentialMeta, inActorMetaList, this.m_rangePolygon, this.m_rangeDeadPoint, this.InitBuffDemand, this.InitRandPassSkillRule, ref inSpawnedList);
            if ((inSpawnedList.Count > 0) && (inSpawnedList[0] != 0))
            {
                PoolObjHandle<ActorRoot> handle = inSpawnedList[0];
                return handle.handle;
            }
            return null;
        }
    }
}

