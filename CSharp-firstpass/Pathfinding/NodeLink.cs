namespace Pathfinding
{
    using System;
    using UnityEngine;

    [AddComponentMenu("Pathfinding/Link")]
    public class NodeLink : GraphModifier
    {
        public float costFactor = 1f;
        public bool deleteConnection;
        public Transform end;
        public bool oneWay;

        public virtual void Apply()
        {
            if (((this.Start != null) && (this.End != null)) && (AstarPath.active != null))
            {
                GraphNode node = AstarPath.active.GetNearest(this.Start.position).node;
                GraphNode node2 = AstarPath.active.GetNearest(this.End.position).node;
                if ((node != null) && (node2 != null))
                {
                    if (this.deleteConnection)
                    {
                        node.RemoveConnection(node2);
                        if (!this.oneWay)
                        {
                            node2.RemoveConnection(node);
                        }
                    }
                    else
                    {
                        VInt3 num2 = node.position - node2.position;
                        uint cost = (uint) Math.Round((double) (num2.costMagnitude * this.costFactor));
                        node.AddConnection(node2, cost);
                        if (!this.oneWay)
                        {
                            node2.AddConnection(node, cost);
                        }
                    }
                }
            }
        }

        private void DrawGizmoBezier(Vector3 p1, Vector3 p2)
        {
            Vector3 rhs = p2 - p1;
            if (rhs != Vector3.zero)
            {
                Vector3 vector2 = Vector3.Cross(Vector3.up, rhs);
                Vector3 vector3 = (Vector3) (Vector3.Cross(rhs, vector2).normalized * (rhs.magnitude * 0.1f));
                Vector3 vector4 = p1 + vector3;
                Vector3 vector5 = p2 + vector3;
                Vector3 from = p1;
                for (int i = 1; i <= 20; i++)
                {
                    float t = ((float) i) / 20f;
                    Vector3 to = AstarMath.CubicBezier(p1, vector4, vector5, p2, t);
                    Gizmos.DrawLine(from, to);
                    from = to;
                }
            }
        }

        public void InternalOnPostScan()
        {
            this.Apply();
        }

        public void OnDrawGizmos()
        {
            if ((this.Start != null) && (this.End != null))
            {
                Vector3 position = this.Start.position;
                Vector3 vector2 = this.End.position;
                Gizmos.color = !this.deleteConnection ? Color.green : Color.red;
                this.DrawGizmoBezier(position, vector2);
            }
        }

        public override void OnGraphsPostUpdate()
        {
            if (!AstarPath.active.isScanning)
            {
                AstarPath.active.AddWorkItem(new AstarPath.AstarWorkItem(delegate (bool force) {
                    this.InternalOnPostScan();
                    return true;
                }));
            }
        }

        public override void OnPostScan()
        {
            if (AstarPath.active.isScanning)
            {
                this.InternalOnPostScan();
            }
            else
            {
                AstarPath.active.AddWorkItem(new AstarPath.AstarWorkItem(delegate (bool force) {
                    this.InternalOnPostScan();
                    return true;
                }));
            }
        }

        public Transform End
        {
            get
            {
                return this.end;
            }
        }

        public Transform Start
        {
            get
            {
                return base.transform;
            }
        }
    }
}

