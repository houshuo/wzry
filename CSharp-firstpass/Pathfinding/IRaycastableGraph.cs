namespace Pathfinding
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public interface IRaycastableGraph
    {
        bool Linecast(VInt3 start, VInt3 end);
        bool Linecast(VInt3 start, VInt3 end, GraphNode hint);
        bool Linecast(VInt3 start, VInt3 end, GraphNode hint, out GraphHitInfo hit);
        bool Linecast(VInt3 start, VInt3 end, GraphNode hint, out GraphHitInfo hit, List<GraphNode> trace);
    }
}

