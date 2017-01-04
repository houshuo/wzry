namespace Pathfinding
{
    using Pathfinding.Serialization.JsonFx;
    using System;
    using UnityEngine;

    public class UserConnection
    {
        [JsonName("doOverCost")]
        public bool doOverrideCost;
        [JsonName("doOverCost")]
        public bool doOverridePenalty;
        [JsonName("doOverWalkable")]
        public bool doOverrideWalkability = true;
        public bool enable = true;
        public bool oneWay;
        [JsonName("overCost")]
        public int overrideCost;
        [JsonName("overPenalty")]
        public uint overridePenalty;
        public Vector3 p1;
        public Vector3 p2;
        public ConnectionType type;
        public float width;
    }
}

