namespace Pathfinding.RVO
{
    using System;

    public class ObstacleVertex
    {
        public bool convex;
        public VInt2 dir;
        public VInt height;
        public bool ignore;
        public RVOLayer layer;
        public ObstacleVertex next;
        public VInt3 position;
        public ObstacleVertex prev;
        public bool split;
    }
}

