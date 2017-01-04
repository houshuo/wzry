namespace Pathfinding
{
    using System;
    using UnityEngine;

    [AddComponentMenu("Pathfinding/GraphUpdateScene")]
    public class GraphUpdateScene : GraphModifier
    {
        [HideInInspector]
        public bool applyOnScan = true;
        [HideInInspector]
        public bool applyOnStart = true;
        [HideInInspector]
        public bool convex = true;
        private Vector3[] convexPoints;
        private bool firstApplied;
        [HideInInspector]
        public bool lockToY;
        [HideInInspector]
        public float lockToYValue;
        [HideInInspector]
        public float minBoundsHeight = 1f;
        [HideInInspector]
        public bool modifyTag;
        [HideInInspector]
        public bool modifyWalkability;
        [HideInInspector]
        public int penaltyDelta;
        public Vector3[] points;
        [HideInInspector]
        public bool resetPenaltyOnPhysics = true;
        [HideInInspector]
        public int setTag;
        private int setTagInvert;
        [HideInInspector]
        public bool setWalkability;
        [HideInInspector]
        public bool updateErosion = true;
        [HideInInspector]
        public bool updatePhysics;
        [HideInInspector]
        public bool useWorldSpace;

        public void Apply()
        {
            if (AstarPath.active == null)
            {
                Debug.LogError("There is no AstarPath object in the scene");
            }
            else
            {
                GraphUpdateObject obj2;
                if ((this.points == null) || (this.points.Length == 0))
                {
                    Bounds bounds;
                    Collider component = base.GetComponent<Collider>();
                    Renderer renderer = base.GetComponent<Renderer>();
                    if (component != null)
                    {
                        bounds = component.bounds;
                    }
                    else if (renderer != null)
                    {
                        bounds = renderer.bounds;
                    }
                    else
                    {
                        Debug.LogWarning("Cannot apply GraphUpdateScene, no points defined and no renderer or collider attached");
                        return;
                    }
                    if (bounds.size.y < this.minBoundsHeight)
                    {
                        bounds.size = new Vector3(bounds.size.x, this.minBoundsHeight, bounds.size.z);
                    }
                    obj2 = new GraphUpdateObject(bounds);
                }
                else
                {
                    GraphUpdateShape shape = new GraphUpdateShape {
                        convex = this.convex
                    };
                    Vector3[] points = this.points;
                    if (!this.useWorldSpace)
                    {
                        points = new Vector3[this.points.Length];
                        Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
                        for (int i = 0; i < points.Length; i++)
                        {
                            points[i] = localToWorldMatrix.MultiplyPoint3x4(this.points[i]);
                        }
                    }
                    shape.points = points;
                    Bounds b = shape.GetBounds();
                    if (b.size.y < this.minBoundsHeight)
                    {
                        b.size = new Vector3(b.size.x, this.minBoundsHeight, b.size.z);
                    }
                    obj2 = new GraphUpdateObject(b) {
                        shape = shape
                    };
                }
                this.firstApplied = true;
                obj2.modifyWalkability = this.modifyWalkability;
                obj2.setWalkability = this.setWalkability;
                obj2.addPenalty = this.penaltyDelta;
                obj2.updatePhysics = this.updatePhysics;
                obj2.updateErosion = this.updateErosion;
                obj2.resetPenaltyOnPhysics = this.resetPenaltyOnPhysics;
                obj2.modifyTag = this.modifyTag;
                obj2.setTag = this.setTag;
                AstarPath.active.UpdateGraphs(obj2);
            }
        }

        public void Apply(AstarPath active)
        {
            if (this.applyOnScan)
            {
                this.Apply();
            }
        }

        public Bounds GetBounds()
        {
            Bounds bounds;
            if ((this.points == null) || (this.points.Length == 0))
            {
                Collider component = base.GetComponent<Collider>();
                Renderer renderer = base.GetComponent<Renderer>();
                if (component == null)
                {
                    if (renderer == null)
                    {
                        return new Bounds(Vector3.zero, Vector3.zero);
                    }
                    bounds = renderer.bounds;
                }
                else
                {
                    bounds = component.bounds;
                }
            }
            else
            {
                Matrix4x4 identity = Matrix4x4.identity;
                if (!this.useWorldSpace)
                {
                    identity = base.transform.localToWorldMatrix;
                }
                Vector3 lhs = identity.MultiplyPoint3x4(this.points[0]);
                Vector3 vector2 = identity.MultiplyPoint3x4(this.points[0]);
                for (int i = 0; i < this.points.Length; i++)
                {
                    Vector3 rhs = identity.MultiplyPoint3x4(this.points[i]);
                    lhs = Vector3.Min(lhs, rhs);
                    vector2 = Vector3.Max(vector2, rhs);
                }
                bounds = new Bounds((Vector3) ((lhs + vector2) * 0.5f), vector2 - lhs);
            }
            if (bounds.size.y < this.minBoundsHeight)
            {
                bounds.size = new Vector3(bounds.size.x, this.minBoundsHeight, bounds.size.z);
            }
            return bounds;
        }

        public virtual void InvertSettings()
        {
            this.setWalkability = !this.setWalkability;
            this.penaltyDelta = -this.penaltyDelta;
            if (this.setTagInvert == 0)
            {
                this.setTagInvert = this.setTag;
                this.setTag = 0;
            }
            else
            {
                this.setTag = this.setTagInvert;
                this.setTagInvert = 0;
            }
        }

        public void LockToY()
        {
            if (this.points != null)
            {
                for (int i = 0; i < this.points.Length; i++)
                {
                    this.points[i].y = this.lockToYValue;
                }
            }
        }

        public void OnDrawGizmos()
        {
            this.OnDrawGizmos(false);
        }

        public void OnDrawGizmos(bool selected)
        {
            Color a = !selected ? new Color(0.8901961f, 0.2392157f, 0.08627451f, 0.9f) : new Color(0.8901961f, 0.2392157f, 0.08627451f, 1f);
            if (selected)
            {
                Gizmos.color = Color.Lerp(a, new Color(1f, 1f, 1f, 0.2f), 0.9f);
                Bounds bounds = this.GetBounds();
                Gizmos.DrawCube(bounds.center, bounds.size);
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
            if (this.points != null)
            {
                if (this.convex)
                {
                    a.a *= 0.5f;
                }
                Gizmos.color = a;
                Matrix4x4 matrixx = !this.useWorldSpace ? base.transform.localToWorldMatrix : Matrix4x4.identity;
                if (this.convex)
                {
                    a.r -= 0.1f;
                    a.g -= 0.2f;
                    a.b -= 0.1f;
                    Gizmos.color = a;
                }
                if (selected || !this.convex)
                {
                    for (int i = 0; i < this.points.Length; i++)
                    {
                        Vector3 from = matrixx.MultiplyPoint3x4(this.points[i]);
                        Gizmos.DrawLine(from, matrixx.MultiplyPoint3x4(this.points[(i + 1) % this.points.Length]));
                    }
                }
                if (this.convex)
                {
                    if (this.convexPoints == null)
                    {
                        this.RecalcConvex();
                    }
                    Gizmos.color = !selected ? new Color(0.8901961f, 0.2392157f, 0.08627451f, 0.9f) : new Color(0.8901961f, 0.2392157f, 0.08627451f, 1f);
                    if (this.convexPoints != null)
                    {
                        for (int j = 0; j < this.convexPoints.Length; j++)
                        {
                            Vector3 introduced6 = matrixx.MultiplyPoint3x4(this.convexPoints[j]);
                            Gizmos.DrawLine(introduced6, matrixx.MultiplyPoint3x4(this.convexPoints[(j + 1) % this.convexPoints.Length]));
                        }
                    }
                }
            }
        }

        public void OnDrawGizmosSelected()
        {
            this.OnDrawGizmos(true);
        }

        public override void OnPostScan()
        {
            if (this.applyOnScan)
            {
                this.Apply();
            }
        }

        public void RecalcConvex()
        {
            if (this.convex)
            {
                this.convexPoints = Polygon.ConvexHull(this.points);
            }
            else
            {
                this.convexPoints = null;
            }
        }

        public void Start()
        {
            if (!this.firstApplied && this.applyOnStart)
            {
                this.Apply();
            }
        }

        public void ToggleUseWorldSpace()
        {
            this.useWorldSpace = !this.useWorldSpace;
            if (this.points != null)
            {
                this.convexPoints = null;
                Matrix4x4 matrixx = !this.useWorldSpace ? base.transform.worldToLocalMatrix : base.transform.localToWorldMatrix;
                for (int i = 0; i < this.points.Length; i++)
                {
                    this.points[i] = matrixx.MultiplyPoint3x4(this.points[i]);
                }
            }
        }
    }
}

