namespace Pathfinding
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class RecastBBTree
    {
        public RecastBBTreeBox root;

        public Rect ExpandToContain(Rect r, Rect r2)
        {
            float left = Mathf.Min(r.xMin, r2.xMin);
            float right = Mathf.Max(r.xMax, r2.xMax);
            float top = Mathf.Min(r.yMin, r2.yMin);
            float bottom = Mathf.Max(r.yMax, r2.yMax);
            return Rect.MinMaxRect(left, top, right, bottom);
        }

        public float ExpansionRequired(Rect r, Rect r2)
        {
            float num = Mathf.Min(r.xMin, r2.xMin);
            float num2 = Mathf.Max(r.xMax, r2.xMax);
            float num3 = Mathf.Min(r.yMin, r2.yMin);
            float num4 = Mathf.Max(r.yMax, r2.yMax);
            return (((num2 - num) * (num4 - num3)) - this.RectArea(r));
        }

        public void Insert(RecastMeshObj mesh)
        {
            RecastBBTreeBox box = new RecastBBTreeBox(this, mesh);
            if (this.root == null)
            {
                this.root = box;
                return;
            }
            RecastBBTreeBox root = this.root;
        Label_0022:
            root.rect = this.ExpandToContain(root.rect, box.rect);
            if (root.mesh != null)
            {
                root.c1 = box;
                RecastBBTreeBox box3 = new RecastBBTreeBox(this, root.mesh);
                root.c2 = box3;
                root.mesh = null;
            }
            else
            {
                float num = this.ExpansionRequired(root.c1.rect, box.rect);
                float num2 = this.ExpansionRequired(root.c2.rect, box.rect);
                if (num < num2)
                {
                    root = root.c1;
                }
                else if (num2 < num)
                {
                    root = root.c2;
                }
                else
                {
                    root = (this.RectArea(root.c1.rect) >= this.RectArea(root.c2.rect)) ? root.c2 : root.c1;
                }
                goto Label_0022;
            }
        }

        public void OnDrawGizmos()
        {
        }

        public void OnDrawGizmos(RecastBBTreeBox box)
        {
            if (box != null)
            {
                Vector3 vector = new Vector3(box.rect.xMin, 0f, box.rect.yMin);
                Vector3 vector2 = new Vector3(box.rect.xMax, 0f, box.rect.yMax);
                Vector3 center = (Vector3) ((vector + vector2) * 0.5f);
                Vector3 size = (Vector3) ((vector2 - center) * 2f);
                Gizmos.DrawCube(center, size);
                this.OnDrawGizmos(box.c1);
                this.OnDrawGizmos(box.c2);
            }
        }

        public void QueryBoxInBounds(RecastBBTreeBox box, Rect bounds, List<RecastMeshObj> boxes)
        {
            if (box.mesh != null)
            {
                if (this.RectIntersectsRect(box.rect, bounds))
                {
                    boxes.Add(box.mesh);
                }
            }
            else
            {
                if (this.RectIntersectsRect(box.c1.rect, bounds))
                {
                    this.QueryBoxInBounds(box.c1, bounds, boxes);
                }
                if (this.RectIntersectsRect(box.c2.rect, bounds))
                {
                    this.QueryBoxInBounds(box.c2, bounds, boxes);
                }
            }
        }

        public void QueryInBounds(Rect bounds, List<RecastMeshObj> buffer)
        {
            RecastBBTreeBox root = this.root;
            if (root != null)
            {
                this.QueryBoxInBounds(root, bounds, buffer);
            }
        }

        public float RectArea(Rect r)
        {
            return (r.width * r.height);
        }

        public bool RectContains(Rect r, Vector3 p)
        {
            return ((((p.x >= r.xMin) && (p.x <= r.xMax)) && (p.z >= r.yMin)) && (p.z <= r.yMax));
        }

        public bool RectIntersectsCircle(Rect r, Vector3 p, float radius)
        {
            return (float.IsPositiveInfinity(radius) || (this.RectContains(r, p) || (((this.XIntersectsCircle(r.xMin, r.xMax, r.yMin, p, radius) || this.XIntersectsCircle(r.xMin, r.xMax, r.yMax, p, radius)) || this.ZIntersectsCircle(r.yMin, r.yMax, r.xMin, p, radius)) || this.ZIntersectsCircle(r.yMin, r.yMax, r.xMax, p, radius))));
        }

        public bool RectIntersectsRect(Rect r, Rect r2)
        {
            return ((((r.xMax > r2.xMin) && (r.yMax > r2.yMin)) && (r2.xMax > r.xMin)) && (r2.yMax > r.yMin));
        }

        public bool Remove(RecastMeshObj mesh)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException("mesh");
            }
            if (this.root == null)
            {
                return false;
            }
            bool found = false;
            Bounds bounds = mesh.GetBounds();
            Rect rect = Rect.MinMaxRect(bounds.min.x, bounds.min.z, bounds.max.x, bounds.max.z);
            this.root = this.RemoveBox(this.root, mesh, rect, ref found);
            return found;
        }

        private RecastBBTreeBox RemoveBox(RecastBBTreeBox c, RecastMeshObj mesh, Rect bounds, ref bool found)
        {
            if (this.RectIntersectsRect(c.rect, bounds))
            {
                if (c.mesh == mesh)
                {
                    found = true;
                    return null;
                }
                if ((c.mesh != null) || found)
                {
                    return c;
                }
                c.c1 = this.RemoveBox(c.c1, mesh, bounds, ref found);
                if (c.c1 == null)
                {
                    return c.c2;
                }
                if (!found)
                {
                    c.c2 = this.RemoveBox(c.c2, mesh, bounds, ref found);
                    if (c.c2 == null)
                    {
                        return c.c1;
                    }
                }
                if (found)
                {
                    c.rect = this.ExpandToContain(c.c1.rect, c.c2.rect);
                }
            }
            return c;
        }

        public void TestIntersections(Vector3 p, float radius)
        {
            RecastBBTreeBox root = this.root;
            this.TestIntersections(root, p, radius);
        }

        public void TestIntersections(RecastBBTreeBox box, Vector3 p, float radius)
        {
            if (box != null)
            {
                this.RectIntersectsCircle(box.rect, p, radius);
                this.TestIntersections(box.c1, p, radius);
                this.TestIntersections(box.c2, p, radius);
            }
        }

        public void ToString()
        {
            RecastBBTreeBox root = this.root;
            new Stack<RecastBBTreeBox>().Push(root);
            root.WriteChildren(0);
        }

        public bool XIntersectsCircle(float x1, float x2, float zpos, Vector3 circle, float radius)
        {
            double num = Math.Abs((float) (zpos - circle.z)) / radius;
            if ((num > 1.0) || (num < -1.0))
            {
                return false;
            }
            float num2 = ((float) Math.Sqrt(1.0 - (num * num))) * radius;
            float num3 = circle.x - num2;
            num2 += circle.x;
            float b = Math.Min(num2, num3);
            float num5 = Math.Max(num2, num3);
            b = Mathf.Max(x1, b);
            return (Mathf.Min(x2, num5) > b);
        }

        public bool ZIntersectsCircle(float z1, float z2, float xpos, Vector3 circle, float radius)
        {
            double num = Math.Abs((float) (xpos - circle.x)) / radius;
            if ((num > 1.0) || (num < -1.0))
            {
                return false;
            }
            float num2 = ((float) Math.Sqrt(1.0 - (num * num))) * radius;
            float num3 = circle.z - num2;
            num2 += circle.z;
            float b = Math.Min(num2, num3);
            float num5 = Math.Max(num2, num3);
            b = Mathf.Max(z1, b);
            return (Mathf.Min(z2, num5) > b);
        }
    }
}

