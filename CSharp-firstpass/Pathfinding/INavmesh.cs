namespace Pathfinding
{
    using System;

    public interface INavmesh
    {
        void GetNodes(GraphNodeDelegateCancelable del);
    }
}

