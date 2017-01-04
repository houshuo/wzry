namespace Pathfinding
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct GraphHitInfo
    {
        public VInt3 origin;
        public VInt3 point;
        public GraphNode node;
        public VInt3 tangentOrigin;
        public VInt3 tangent;
        public GraphHitInfo(VInt3 point)
        {
            this.tangentOrigin = VInt3.zero;
            this.origin = VInt3.zero;
            this.point = point;
            this.node = null;
            this.tangent = VInt3.zero;
        }

        public float distance
        {
            get
            {
                VInt3 num = this.point - this.origin;
                return (float) num.magnitude;
            }
        }
    }
}

