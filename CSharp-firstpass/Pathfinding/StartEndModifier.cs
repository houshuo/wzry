namespace Pathfinding
{
    using System;
    using UnityEngine;

    [Serializable]
    public class StartEndModifier : PathModifier
    {
        public bool addPoints;
        public Exactness exactEndPoint = Exactness.ClosestOnNode;
        public Exactness exactStartPoint = Exactness.ClosestOnNode;
        public LayerMask mask = -1;
        public bool useGraphRaycasting;

        public override void Apply(Path _p, ModifierData source)
        {
            ABPath path = _p as ABPath;
            if ((path != null) && (path.vectorPath.Count != 0))
            {
                if ((path.vectorPath.Count < 2) && !this.addPoints)
                {
                    path.vectorPath.Add(path.vectorPath[0]);
                }
                VInt3 zero = VInt3.zero;
                VInt3 point = VInt3.zero;
                if (this.exactStartPoint == Exactness.Original)
                {
                    zero = this.GetClampedPoint(path.path[0].position, path.originalStartPoint, path.path[0]);
                }
                else if (this.exactStartPoint == Exactness.ClosestOnNode)
                {
                    zero = this.GetClampedPoint(path.path[0].position, path.startPoint, path.path[0]);
                }
                else if (this.exactStartPoint == Exactness.Interpolate)
                {
                    zero = this.GetClampedPoint(path.path[0].position, path.originalStartPoint, path.path[0]);
                    VInt3 position = path.path[0].position;
                    VInt3 lineEnd = path.path[(1 < path.path.Count) ? 1 : 0].position;
                    zero = AstarMath.NearestPointStrict(ref position, ref lineEnd, ref zero);
                }
                else
                {
                    zero = path.path[0].position;
                }
                if (this.exactEndPoint == Exactness.Original)
                {
                    point = this.GetClampedPoint(path.path[path.path.Count - 1].position, path.originalEndPoint, path.path[path.path.Count - 1]);
                }
                else if (this.exactEndPoint == Exactness.ClosestOnNode)
                {
                    point = this.GetClampedPoint(path.path[path.path.Count - 1].position, path.endPoint, path.path[path.path.Count - 1]);
                }
                else if (this.exactEndPoint == Exactness.Interpolate)
                {
                    point = this.GetClampedPoint(path.path[path.path.Count - 1].position, path.originalEndPoint, path.path[path.path.Count - 1]);
                    VInt3 lineStart = path.path[path.path.Count - 1].position;
                    VInt3 num6 = path.path[((path.path.Count - 2) >= 0) ? (path.path.Count - 2) : 0].position;
                    point = AstarMath.NearestPointStrict(ref lineStart, ref num6, ref point);
                }
                else
                {
                    point = path.path[path.path.Count - 1].position;
                }
                if (!this.addPoints)
                {
                    path.vectorPath[0] = zero;
                    path.vectorPath[path.vectorPath.Count - 1] = point;
                }
                else
                {
                    if (this.exactStartPoint != Exactness.SnapToNode)
                    {
                        path.vectorPath.Insert(0, zero);
                    }
                    if (this.exactEndPoint != Exactness.SnapToNode)
                    {
                        path.vectorPath.Add(point);
                    }
                }
            }
        }

        public VInt3 GetClampedPoint(VInt3 from, VInt3 to, GraphNode hint)
        {
            VInt3 end = to;
            if (this.useGraphRaycasting && (hint != null))
            {
                NavGraph graph = AstarData.GetGraph(hint);
                if (graph != null)
                {
                    GraphHitInfo info;
                    IRaycastableGraph graph2 = graph as IRaycastableGraph;
                    if ((graph2 != null) && graph2.Linecast(from, end, hint, out info))
                    {
                        end = info.point;
                    }
                }
            }
            return end;
        }

        public override ModifierData input
        {
            get
            {
                return ModifierData.Vector;
            }
        }

        public override ModifierData output
        {
            get
            {
                return ((!this.addPoints ? ModifierData.StrictVectorPath : ModifierData.None) | ModifierData.VectorPath);
            }
        }

        public enum Exactness
        {
            SnapToNode,
            Original,
            Interpolate,
            ClosestOnNode
        }
    }
}

