namespace Pathfinding
{
    using Pathfinding.Util;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable, AddComponentMenu("Pathfinding/Modifiers/Funnel")]
    public class FunnelModifier : MonoModifier, IPooledMonoBehaviour
    {
        public override void Apply(Path p, ModifierData source)
        {
            List<GraphNode> path = p.path;
            List<VInt3> vectorPath = p.vectorPath;
            if (((path != null) && (path.Count != 0)) && ((vectorPath != null) && (vectorPath.Count == path.Count)))
            {
                List<VInt3> funnelPath = ListPool<VInt3>.Claim();
                List<VInt3> left = ListPool<VInt3>.Claim(path.Count + 1);
                List<VInt3> right = ListPool<VInt3>.Claim(path.Count + 1);
                left.Add(vectorPath[0]);
                right.Add(vectorPath[0]);
                for (int i = 0; i < (path.Count - 1); i++)
                {
                    bool flag = path[i].GetPortal(path[i + 1], left, right, false);
                    bool flag2 = false;
                    if (!flag && !flag2)
                    {
                        left.Add(path[i].position);
                        right.Add(path[i].position);
                        left.Add(path[i + 1].position);
                        right.Add(path[i + 1].position);
                    }
                }
                left.Add(vectorPath[vectorPath.Count - 1]);
                right.Add(vectorPath[vectorPath.Count - 1]);
                if (!this.RunFunnel(left, right, funnelPath))
                {
                    funnelPath.Add(vectorPath[0]);
                    funnelPath.Add(vectorPath[vectorPath.Count - 1]);
                }
                ListPool<VInt3>.Release(p.vectorPath);
                p.vectorPath = funnelPath;
                ListPool<VInt3>.Release(left);
                ListPool<VInt3>.Release(right);
            }
        }

        public void OnCreate()
        {
        }

        public void OnGet()
        {
            base.seeker = null;
            base.priority = 0;
            base.Awake();
        }

        public void OnRecycle()
        {
        }

        public bool RunFunnel(List<VInt3> left, List<VInt3> right, List<VInt3> funnelPath)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            if (funnelPath == null)
            {
                throw new ArgumentNullException("funnelPath");
            }
            if (left.Count != right.Count)
            {
                throw new ArgumentException("left and right lists must have equal length");
            }
            if (left.Count <= 3)
            {
                return false;
            }
            while ((left[1] == left[2]) && (right[1] == right[2]))
            {
                left.RemoveAt(1);
                right.RemoveAt(1);
                if (left.Count <= 3)
                {
                    return false;
                }
            }
            VInt3 c = left[2];
            if (c == left[1])
            {
                c = right[2];
            }
            while (Polygon.IsColinear(left[0], left[1], right[1]) || (Polygon.Left(left[1], right[1], c) == Polygon.Left(left[1], right[1], left[0])))
            {
                left.RemoveAt(1);
                right.RemoveAt(1);
                if (left.Count <= 3)
                {
                    return false;
                }
                c = left[2];
                if (c == left[1])
                {
                    c = right[2];
                }
            }
            if (!Polygon.IsClockwise(left[0], left[1], right[1]) && !Polygon.IsColinear(left[0], left[1], right[1]))
            {
                List<VInt3> list = left;
                left = right;
                right = list;
            }
            funnelPath.Add(left[0]);
            VInt3 a = left[0];
            VInt3 b = left[1];
            VInt3 num4 = right[1];
            int num5 = 0;
            int num6 = 1;
            int num7 = 1;
            for (int i = 2; i < left.Count; i++)
            {
                if (funnelPath.Count > 0x7d0)
                {
                    Debug.LogWarning("Avoiding infinite loop. Remove this check if you have this long paths.");
                    break;
                }
                VInt3 num9 = left[i];
                VInt3 num10 = right[i];
                if (Polygon.TriangleArea2(a, num4, num10) >= 0L)
                {
                    if ((a == num4) || (Polygon.TriangleArea2(a, b, num10) <= 0L))
                    {
                        num4 = num10;
                        num6 = i;
                    }
                    else
                    {
                        funnelPath.Add(b);
                        a = b;
                        num5 = num7;
                        b = a;
                        num4 = a;
                        num7 = num5;
                        num6 = num5;
                        i = num5;
                        continue;
                    }
                }
                if (Polygon.TriangleArea2(a, b, num9) <= 0L)
                {
                    if ((a == b) || (Polygon.TriangleArea2(a, num4, num9) >= 0L))
                    {
                        b = num9;
                        num7 = i;
                    }
                    else
                    {
                        funnelPath.Add(num4);
                        a = num4;
                        num5 = num6;
                        b = a;
                        num4 = a;
                        num7 = num5;
                        num6 = num5;
                        i = num5;
                    }
                }
            }
            funnelPath.Add(left[left.Count - 1]);
            return true;
        }

        public override ModifierData input
        {
            get
            {
                return ModifierData.StrictVectorPath;
            }
        }

        public override ModifierData output
        {
            get
            {
                return ModifierData.VectorPath;
            }
        }
    }
}

