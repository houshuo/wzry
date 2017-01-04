namespace Pathfinding
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class BBTree
    {
        private BBTreeBox[] arr = new BBTreeBox[6];
        private int count;
        public INavmeshHolder graph;

        public BBTree(INavmeshHolder graph)
        {
            this.graph = graph;
        }

        public void Clear()
        {
            this.count = 0;
        }

        private void EnsureCapacity(int c)
        {
            if (this.arr.Length < c)
            {
                BBTreeBox[] boxArray = new BBTreeBox[Math.Max(c, (int) (this.arr.Length * 1.5f))];
                for (int i = 0; i < this.count; i++)
                {
                    boxArray[i] = this.arr[i];
                }
                this.arr = boxArray;
            }
        }

        private static Rect ExpandToContain(Rect r, Rect r2)
        {
            float left = Math.Min(r.xMin, r2.xMin);
            float right = Math.Max(r.xMax, r2.xMax);
            float top = Math.Min(r.yMin, r2.yMin);
            float bottom = Math.Max(r.yMax, r2.yMax);
            return Rect.MinMaxRect(left, top, right, bottom);
        }

        private static float ExpansionRequired(Rect r, Rect r2)
        {
            float num = Math.Min(r.xMin, r2.xMin);
            float num2 = Math.Max(r.xMax, r2.xMax);
            float num3 = Math.Min(r.yMin, r2.yMin);
            float num4 = Math.Max(r.yMax, r2.yMax);
            return (((num2 - num) * (num4 - num3)) - RectArea(r));
        }

        private int GetBox(MeshNode node)
        {
            if (this.count >= this.arr.Length)
            {
                this.EnsureCapacity(this.count + 1);
            }
            this.arr[this.count] = new BBTreeBox(this, node);
            this.count++;
            return (this.count - 1);
        }

        public void Insert(MeshNode node)
        {
            BBTreeBox box2;
            int index = this.GetBox(node);
            if (index == 0)
            {
                return;
            }
            BBTreeBox box = this.arr[index];
            int left = 0;
        Label_0023:
            box2 = this.arr[left];
            box2.rect = ExpandToContain(box2.rect, box.rect);
            if (box2.node != null)
            {
                box2.left = index;
                int num3 = this.GetBox(box2.node);
                box2.right = num3;
                box2.node = null;
                this.arr[left] = box2;
            }
            else
            {
                this.arr[left] = box2;
                float num4 = ExpansionRequired(this.arr[box2.left].rect, box.rect);
                float num5 = ExpansionRequired(this.arr[box2.right].rect, box.rect);
                if (num4 < num5)
                {
                    left = box2.left;
                }
                else if (num5 < num4)
                {
                    left = box2.right;
                }
                else
                {
                    left = (RectArea(this.arr[box2.left].rect) >= RectArea(this.arr[box2.right].rect)) ? box2.right : box2.left;
                }
                goto Label_0023;
            }
        }

        private static bool NodeIntersectsCircle(MeshNode node, Vector3 p, float radius)
        {
            if (float.IsPositiveInfinity(radius))
            {
                return true;
            }
            Vector3 vector = p - node.ClosestPointOnNode(p);
            return (vector.sqrMagnitude < (radius * radius));
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
            if (this.count == 0)
            {
            }
        }

        private void OnDrawGizmos(int boxi, int depth)
        {
            BBTreeBox box = this.arr[boxi];
            Vector3 vector = new Vector3(box.rect.xMin, 0f, box.rect.yMin);
            Vector3 vector2 = new Vector3(box.rect.xMax, 0f, box.rect.yMax);
            Vector3 center = (Vector3) ((vector + vector2) * 0.5f);
            Vector3 size = (Vector3) ((vector2 - center) * 2f);
            center.y += depth * 0.2f;
            Gizmos.color = AstarMath.IntToColor(depth, 0.05f);
            Gizmos.DrawCube(center, size);
            if (box.node == null)
            {
                this.OnDrawGizmos(box.left, depth + 1);
                this.OnDrawGizmos(box.right, depth + 1);
            }
        }

        public NNInfo Query(Vector3 p, NNConstraint constraint)
        {
            if (this.count == 0)
            {
                return new NNInfo(null);
            }
            NNInfo nnInfo = new NNInfo();
            this.SearchBox(0, p, constraint, ref nnInfo);
            nnInfo.UpdateInfo();
            return nnInfo;
        }

        public NNInfo QueryCircle(Vector3 p, float radius, NNConstraint constraint)
        {
            if (this.count == 0)
            {
                return new NNInfo(null);
            }
            NNInfo nnInfo = new NNInfo(null);
            this.SearchBoxCircle(0, p, radius, constraint, ref nnInfo);
            nnInfo.UpdateInfo();
            return nnInfo;
        }

        public NNInfo QueryClosest(Vector3 p, NNConstraint constraint, out float distance)
        {
            distance = float.PositiveInfinity;
            return this.QueryClosest(p, constraint, ref distance, new NNInfo(null));
        }

        public NNInfo QueryClosest(Vector3 p, NNConstraint constraint, ref float distance, NNInfo previous)
        {
            if (this.count != 0)
            {
                this.SearchBoxClosest(0, p, ref distance, constraint, ref previous);
            }
            return previous;
        }

        public NNInfo QueryClosestXZ(Vector3 p, NNConstraint constraint, ref float distance, NNInfo previous)
        {
            if (this.count != 0)
            {
                this.SearchBoxClosestXZ(0, p, ref distance, constraint, ref previous);
            }
            return previous;
        }

        public MeshNode QueryInside(Vector3 p, NNConstraint constraint)
        {
            if (this.count == 0)
            {
                return null;
            }
            return this.SearchBoxInside(0, p, constraint);
        }

        private static float RectArea(Rect r)
        {
            return (r.width * r.height);
        }

        private static bool RectContains(Rect r, Vector3 p)
        {
            return ((((p.x >= r.xMin) && (p.x <= r.xMax)) && (p.z >= r.yMin)) && (p.z <= r.yMax));
        }

        private static bool RectIntersectsCircle(Rect r, Vector3 p, float radius)
        {
            if (float.IsPositiveInfinity(radius))
            {
                return true;
            }
            Vector3 vector = p;
            p.x = Math.Max(p.x, r.xMin);
            p.x = Math.Min(p.x, r.xMax);
            p.z = Math.Max(p.z, r.yMin);
            p.z = Math.Min(p.z, r.yMax);
            return ((((p.x - vector.x) * (p.x - vector.x)) + ((p.z - vector.z) * (p.z - vector.z))) < (radius * radius));
        }

        private void SearchBox(int boxi, Vector3 p, NNConstraint constraint, ref NNInfo nnInfo)
        {
            BBTreeBox box = this.arr[boxi];
            if (box.node != null)
            {
                if (box.node.ContainsPoint((VInt3) p))
                {
                    if (nnInfo.node == null)
                    {
                        nnInfo.node = box.node;
                    }
                    else
                    {
                        Vector3 position = (Vector3) box.node.position;
                        Vector3 vector2 = (Vector3) nnInfo.node.position;
                        if (Mathf.Abs((float) (position.y - p.y)) < Mathf.Abs((float) (vector2.y - p.y)))
                        {
                            nnInfo.node = box.node;
                        }
                    }
                    if (constraint.Suitable(box.node))
                    {
                        if (nnInfo.constrainedNode == null)
                        {
                            nnInfo.constrainedNode = box.node;
                        }
                        else
                        {
                            float introduced3 = Mathf.Abs((float) (box.node.position.y - p.y));
                            if (introduced3 < Mathf.Abs((float) (nnInfo.constrainedNode.position.y - p.y)))
                            {
                                nnInfo.constrainedNode = box.node;
                            }
                        }
                    }
                }
            }
            else
            {
                if (RectContains(this.arr[box.left].rect, p))
                {
                    this.SearchBox(box.left, p, constraint, ref nnInfo);
                }
                if (RectContains(this.arr[box.right].rect, p))
                {
                    this.SearchBox(box.right, p, constraint, ref nnInfo);
                }
            }
        }

        private void SearchBoxCircle(int boxi, Vector3 p, float radius, NNConstraint constraint, ref NNInfo nnInfo)
        {
            BBTreeBox box = this.arr[boxi];
            if (box.node != null)
            {
                if (NodeIntersectsCircle(box.node, p, radius))
                {
                    Vector3 vector = box.node.ClosestPointOnNode(p);
                    Vector3 vector2 = vector - p;
                    float sqrMagnitude = vector2.sqrMagnitude;
                    if (nnInfo.node == null)
                    {
                        nnInfo.node = box.node;
                        nnInfo.clampedPosition = vector;
                    }
                    else
                    {
                        Vector3 vector3 = nnInfo.clampedPosition - p;
                        if (sqrMagnitude < vector3.sqrMagnitude)
                        {
                            nnInfo.node = box.node;
                            nnInfo.clampedPosition = vector;
                        }
                    }
                    if ((constraint == null) || constraint.Suitable(box.node))
                    {
                        if (nnInfo.constrainedNode == null)
                        {
                            nnInfo.constrainedNode = box.node;
                            nnInfo.constClampedPosition = vector;
                        }
                        else
                        {
                            Vector3 vector4 = nnInfo.constClampedPosition - p;
                            if (sqrMagnitude < vector4.sqrMagnitude)
                            {
                                nnInfo.constrainedNode = box.node;
                                nnInfo.constClampedPosition = vector;
                            }
                        }
                    }
                }
            }
            else
            {
                if (RectIntersectsCircle(this.arr[box.left].rect, p, radius))
                {
                    this.SearchBoxCircle(box.left, p, radius, constraint, ref nnInfo);
                }
                if (RectIntersectsCircle(this.arr[box.right].rect, p, radius))
                {
                    this.SearchBoxCircle(box.right, p, radius, constraint, ref nnInfo);
                }
            }
        }

        private void SearchBoxClosest(int boxi, Vector3 p, ref float closestDist, NNConstraint constraint, ref NNInfo nnInfo)
        {
            BBTreeBox box = this.arr[boxi];
            if (box.node != null)
            {
                if (NodeIntersectsCircle(box.node, p, closestDist))
                {
                    Vector3 vector = box.node.ClosestPointOnNode(p);
                    Vector3 vector2 = vector - p;
                    float sqrMagnitude = vector2.sqrMagnitude;
                    if ((constraint == null) || constraint.Suitable(box.node))
                    {
                        if (nnInfo.constrainedNode == null)
                        {
                            nnInfo.constrainedNode = box.node;
                            nnInfo.constClampedPosition = vector;
                            closestDist = (float) Math.Sqrt((double) sqrMagnitude);
                        }
                        else if (sqrMagnitude < (closestDist * closestDist))
                        {
                            nnInfo.constrainedNode = box.node;
                            nnInfo.constClampedPosition = vector;
                            closestDist = (float) Math.Sqrt((double) sqrMagnitude);
                        }
                    }
                }
            }
            else
            {
                if (RectIntersectsCircle(this.arr[box.left].rect, p, closestDist))
                {
                    this.SearchBoxClosest(box.left, p, ref closestDist, constraint, ref nnInfo);
                }
                if (RectIntersectsCircle(this.arr[box.right].rect, p, closestDist))
                {
                    this.SearchBoxClosest(box.right, p, ref closestDist, constraint, ref nnInfo);
                }
            }
        }

        private void SearchBoxClosestXZ(int boxi, Vector3 p, ref float closestDist, NNConstraint constraint, ref NNInfo nnInfo)
        {
            BBTreeBox box = this.arr[boxi];
            if (box.node != null)
            {
                Vector3 vector = box.node.ClosestPointOnNodeXZ(p);
                float num = ((vector.x - p.x) * (vector.x - p.x)) + ((vector.z - p.z) * (vector.z - p.z));
                if ((constraint == null) || constraint.Suitable(box.node))
                {
                    if (nnInfo.constrainedNode == null)
                    {
                        nnInfo.constrainedNode = box.node;
                        nnInfo.constClampedPosition = vector;
                        closestDist = (float) Math.Sqrt((double) num);
                    }
                    else if (num < (closestDist * closestDist))
                    {
                        nnInfo.constrainedNode = box.node;
                        nnInfo.constClampedPosition = vector;
                        closestDist = (float) Math.Sqrt((double) num);
                    }
                }
            }
            else
            {
                if (RectIntersectsCircle(this.arr[box.left].rect, p, closestDist))
                {
                    this.SearchBoxClosestXZ(box.left, p, ref closestDist, constraint, ref nnInfo);
                }
                if (RectIntersectsCircle(this.arr[box.right].rect, p, closestDist))
                {
                    this.SearchBoxClosestXZ(box.right, p, ref closestDist, constraint, ref nnInfo);
                }
            }
        }

        private MeshNode SearchBoxInside(int boxi, Vector3 p, NNConstraint constraint)
        {
            BBTreeBox box = this.arr[boxi];
            if (box.node != null)
            {
                if (box.node.ContainsPoint((VInt3) p) && ((constraint == null) || constraint.Suitable(box.node)))
                {
                    return box.node;
                }
            }
            else
            {
                MeshNode node;
                if (this.arr[box.left].rect.Contains(new Vector2(p.x, p.z)))
                {
                    node = this.SearchBoxInside(box.left, p, constraint);
                    if (node != null)
                    {
                        return node;
                    }
                }
                if (this.arr[box.right].rect.Contains(new Vector2(p.x, p.z)))
                {
                    node = this.SearchBoxInside(box.right, p, constraint);
                    if (node != null)
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        public Rect Size
        {
            get
            {
                return ((this.count == 0) ? new Rect(0f, 0f, 0f, 0f) : this.arr[0].rect);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BBTreeBox
        {
            public Rect rect;
            public MeshNode node;
            public int left;
            public int right;
            public BBTreeBox(BBTree tree, MeshNode node)
            {
                this.node = node;
                Vector3 vertex = (Vector3) node.GetVertex(0);
                Vector2 vector2 = new Vector2(vertex.x, vertex.z);
                Vector2 vector3 = vector2;
                for (int i = 1; i < node.GetVertexCount(); i++)
                {
                    Vector3 vector4 = (Vector3) node.GetVertex(i);
                    vector2.x = Math.Min(vector2.x, vector4.x);
                    vector2.y = Math.Min(vector2.y, vector4.z);
                    vector3.x = Math.Max(vector3.x, vector4.x);
                    vector3.y = Math.Max(vector3.y, vector4.z);
                }
                this.rect = Rect.MinMaxRect(vector2.x, vector2.y, vector3.x, vector3.y);
                this.left = this.right = -1;
            }

            public bool IsLeaf
            {
                get
                {
                    return (this.node != null);
                }
            }
            public bool Contains(Vector3 p)
            {
                return this.rect.Contains(new Vector2(p.x, p.z));
            }
        }
    }
}

