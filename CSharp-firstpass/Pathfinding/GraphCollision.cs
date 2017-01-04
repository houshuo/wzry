namespace Pathfinding
{
    using System;
    using UnityEngine;

    [Serializable]
    public class GraphCollision
    {
        public bool collisionCheck = true;
        public float collisionOffset;
        public float diameter = 1f;
        private float finalRadius;
        private float finalRaycastRadius;
        public float fromHeight = 100f;
        public float height = 2f;
        public bool heightCheck = true;
        public LayerMask heightMask = -1;
        public LayerMask mask;
        public const float RaycastErrorMargin = 0.005f;
        public RayDirection rayDirection = RayDirection.Both;
        public bool thickRaycast;
        public float thickRaycastDiameter = 1f;
        public ColliderType type = ColliderType.Capsule;
        public bool unwalkableWhenNoGround = true;
        public Vector3 up;
        private Vector3 upheight;
        public bool use2D;
    }
}

