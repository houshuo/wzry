namespace Pathfinding
{
    using Pathfinding.ClipperLib;
    using Pathfinding.Util;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [AddComponentMenu("Pathfinding/Navmesh/Navmesh Cut")]
    public class NavmeshCut : MonoBehaviour
    {
        private static ListView<NavmeshCut> allCuts = new ListView<NavmeshCut>();
        private Bounds bounds;
        public int campIndex = -1;
        public Vector3 center;
        public float circleRadius = 1f;
        public int circleResolution = 6;
        private Vector3[][] contours;
        [NonSerialized, HideInInspector]
        public int cutIndex = -1;
        public bool cutsAddedGeom = true;
        private static readonly Dictionary<VInt2, int> edges = new Dictionary<VInt2, int>();
        public static readonly Color GizmoColor = new Color(0.145098f, 0.7215686f, 0.9372549f);
        public float height = 1f;
        public bool isDual;
        private Bounds lastBounds;
        private Mesh lastMesh;
        private Vector3 lastPosition;
        private Quaternion lastRotation;
        public Mesh mesh;
        public float meshScale = 1f;
        private static readonly Dictionary<int, int> pointers = new Dictionary<int, int>();
        public Vector2 rectangleSize = new Vector2(1f, 1f);
        protected Transform tr;
        public MeshType type = MeshType.Circle;
        public float updateDistance = 0.4f;
        public float updateRotationDistance = 10f;
        public bool useRotation;
        private bool wasEnabled;

        public static  event Action<NavmeshCut> OnDestroyCallback;

        private static void AddCut(NavmeshCut obj)
        {
            allCuts.Add(obj);
        }

        public void Awake()
        {
            AddCut(this);
        }

        private void CalculateMeshContour()
        {
            if (this.mesh != null)
            {
                edges.Clear();
                pointers.Clear();
                Vector3[] vertices = this.mesh.vertices;
                int[] triangles = this.mesh.triangles;
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    if (Polygon.IsClockwise(vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]]))
                    {
                        int num2 = triangles[i];
                        triangles[i] = triangles[i + 2];
                        triangles[i + 2] = num2;
                    }
                    edges[new VInt2(triangles[i], triangles[i + 1])] = i;
                    edges[new VInt2(triangles[i + 1], triangles[i + 2])] = i;
                    edges[new VInt2(triangles[i + 2], triangles[i])] = i;
                }
                for (int j = 0; j < triangles.Length; j += 3)
                {
                    for (int m = 0; m < 3; m++)
                    {
                        if (!edges.ContainsKey(new VInt2(triangles[j + ((m + 1) % 3)], triangles[j + (m % 3)])))
                        {
                            pointers[triangles[j + (m % 3)]] = triangles[j + ((m + 1) % 3)];
                        }
                    }
                }
                ListLinqView<Vector3[]> view = new ListLinqView<Vector3[]>();
                List<Vector3> list = ListPool<Vector3>.Claim();
                for (int k = 0; k < vertices.Length; k++)
                {
                    if (!pointers.ContainsKey(k))
                    {
                        continue;
                    }
                    list.Clear();
                    int index = k;
                    do
                    {
                        int num7 = pointers[index];
                        if (num7 == -1)
                        {
                            break;
                        }
                        pointers[index] = -1;
                        list.Add(vertices[index]);
                        switch (num7)
                        {
                            case -1:
                                Debug.LogError("Invalid Mesh '" + this.mesh.name + " in " + base.gameObject.name);
                                break;
                        }
                    }
                    while (index != k);
                    if (list.Count > 0)
                    {
                        view.Add(list.ToArray());
                    }
                }
                ListPool<Vector3>.Release(list);
                this.contours = view.ToArray();
            }
        }

        public void Check()
        {
            if (this.tr == null)
            {
                this.tr = base.transform;
                this.lastPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
                this.lastRotation = this.tr.rotation;
            }
        }

        public void ForceUpdate()
        {
            this.lastPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        }

        public static ListView<NavmeshCut> GetAll()
        {
            return allCuts;
        }

        public static ListView<NavmeshCut> GetAllInRange(Bounds b)
        {
            ListView<NavmeshCut> view = new ListView<NavmeshCut>();
            for (int i = 0; i < allCuts.Count; i++)
            {
                if (allCuts[i].enabled && Intersects(b, allCuts[i].GetBounds()))
                {
                    view.Add(allCuts[i]);
                }
            }
            return view;
        }

        public Bounds GetBounds()
        {
            switch (this.type)
            {
                case MeshType.Rectangle:
                    this.bounds = GetBounds_Rectangle(this.tr, this.center, this.rectangleSize, this.height, this.useRotation);
                    break;

                case MeshType.Circle:
                    this.bounds = GetBounds_Circle(this.tr, this.center, this.circleRadius, this.height, this.useRotation);
                    break;

                case MeshType.CustomMesh:
                    if (this.mesh != null)
                    {
                        Bounds bounds = this.mesh.bounds;
                        if (this.useRotation)
                        {
                            Matrix4x4 localToWorldMatrix = this.tr.localToWorldMatrix;
                            bounds.center = (Vector3) (bounds.center * this.meshScale);
                            bounds.size = (Vector3) (bounds.size * this.meshScale);
                            this.bounds = new Bounds(localToWorldMatrix.MultiplyPoint3x4(this.center + bounds.center), Vector3.zero);
                            Vector3 max = bounds.max;
                            Vector3 min = bounds.min;
                            this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(max.x, max.y, max.z)));
                            this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(min.x, max.y, max.z)));
                            this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(min.x, max.y, min.z)));
                            this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(max.x, max.y, min.z)));
                            this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(max.x, min.y, max.z)));
                            this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(min.x, min.y, max.z)));
                            this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(min.x, min.y, min.z)));
                            this.bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(max.x, min.y, min.z)));
                            Vector3 size = this.bounds.size;
                            size.y = Mathf.Max(size.y, this.height * this.tr.lossyScale.y);
                            this.bounds.size = size;
                        }
                        else
                        {
                            Vector3 vector4 = (Vector3) (bounds.size * this.meshScale);
                            vector4.y = Mathf.Max(vector4.y, this.height);
                            this.bounds = new Bounds((base.transform.position + this.center) + ((Vector3) (bounds.center * this.meshScale)), vector4);
                        }
                        break;
                    }
                    break;
            }
            return this.bounds;
        }

        public static Bounds GetBounds_Circle(Transform tr, Vector3 center, float circleRadius, float height, bool useRotation)
        {
            if (useRotation)
            {
                Matrix4x4 localToWorldMatrix = tr.localToWorldMatrix;
                return new Bounds(localToWorldMatrix.MultiplyPoint3x4(center), tr.lossyScale.Mul(new Vector3(circleRadius * 2f, height, circleRadius * 2f)));
            }
            return new Bounds(tr.position + tr.lossyScale.Mul(center), tr.lossyScale.Mul(new Vector3(circleRadius * 2f, height, circleRadius * 2f)));
        }

        public static Bounds GetBounds_Rectangle(Transform tr, Vector3 center, Vector2 rectangleSize, float height, bool useRotation)
        {
            if (useRotation)
            {
                Matrix4x4 localToWorldMatrix = tr.localToWorldMatrix;
                Bounds bounds = new Bounds(localToWorldMatrix.MultiplyPoint3x4(center + ((Vector3) (new Vector3(-rectangleSize.x, -height, -rectangleSize.y) * 0.5f))), Vector3.zero);
                bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(center + ((Vector3) (new Vector3(rectangleSize.x, -height, -rectangleSize.y) * 0.5f))));
                bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(center + ((Vector3) (new Vector3(rectangleSize.x, -height, rectangleSize.y) * 0.5f))));
                bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(center + ((Vector3) (new Vector3(-rectangleSize.x, -height, rectangleSize.y) * 0.5f))));
                bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(center + ((Vector3) (new Vector3(-rectangleSize.x, height, -rectangleSize.y) * 0.5f))));
                bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(center + ((Vector3) (new Vector3(rectangleSize.x, height, -rectangleSize.y) * 0.5f))));
                bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(center + ((Vector3) (new Vector3(rectangleSize.x, height, rectangleSize.y) * 0.5f))));
                bounds.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(center + ((Vector3) (new Vector3(-rectangleSize.x, height, rectangleSize.y) * 0.5f))));
                return bounds;
            }
            return new Bounds(tr.position + tr.lossyScale.Mul(center), tr.lossyScale.Mul(new Vector3(rectangleSize.x, height, rectangleSize.y)));
        }

        public void GetContour(List<List<Pathfinding.ClipperLib.IntPoint>> buffer)
        {
            List<Pathfinding.ClipperLib.IntPoint> list;
            if (this.circleResolution < 3)
            {
                this.circleResolution = 3;
            }
            Vector3 position = this.tr.position;
            switch (this.type)
            {
                case MeshType.Rectangle:
                    list = ListPool<Pathfinding.ClipperLib.IntPoint>.Claim();
                    GetContour_Rectangle(list, this.tr, this.rectangleSize, this.center, this.useRotation);
                    buffer.Add(list);
                    break;

                case MeshType.Circle:
                    list = ListPool<Pathfinding.ClipperLib.IntPoint>.Claim(this.circleResolution);
                    GetContour_Circle(list, this.tr, this.circleResolution, this.circleRadius, this.center, this.useRotation);
                    buffer.Add(list);
                    break;

                case MeshType.CustomMesh:
                    if ((this.mesh != this.lastMesh) || (this.contours == null))
                    {
                        this.CalculateMeshContour();
                        this.lastMesh = this.mesh;
                    }
                    if (this.contours != null)
                    {
                        position += this.center;
                        bool flag = Vector3.Dot(this.tr.up, Vector3.up) < 0f;
                        for (int i = 0; i < this.contours.Length; i++)
                        {
                            Vector3[] vectorArray = this.contours[i];
                            list = ListPool<Pathfinding.ClipperLib.IntPoint>.Claim(vectorArray.Length);
                            if (this.useRotation)
                            {
                                Matrix4x4 localToWorldMatrix = this.tr.localToWorldMatrix;
                                for (int j = 0; j < vectorArray.Length; j++)
                                {
                                    list.Add(V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(this.center + ((Vector3) (vectorArray[j] * this.meshScale)))));
                                }
                            }
                            else
                            {
                                for (int k = 0; k < vectorArray.Length; k++)
                                {
                                    list.Add(V3ToIntPoint(position + ((Vector3) (vectorArray[k] * this.meshScale))));
                                }
                            }
                            if (flag)
                            {
                                list.Reverse();
                            }
                            buffer.Add(list);
                        }
                    }
                    break;
            }
        }

        public static void GetContour_Circle(List<Pathfinding.ClipperLib.IntPoint> buffer, Transform tr, int circleResolution, float circleRadius, Vector3 center, bool useRotation)
        {
            Vector3 position = tr.position;
            if (useRotation)
            {
                Matrix4x4 localToWorldMatrix = tr.localToWorldMatrix;
                for (int i = 0; i < circleResolution; i++)
                {
                    buffer.Add(V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(center + ((Vector3) (new Vector3(Mathf.Cos(((i * 2) * 3.141593f) / ((float) circleResolution)), 0f, Mathf.Sin(((i * 2) * 3.141593f) / ((float) circleResolution))) * circleRadius)))));
                }
            }
            else
            {
                Vector3 zero = Vector3.zero;
                position += center.Mul(tr.lossyScale);
                for (int j = 0; j < circleResolution; j++)
                {
                    zero.x = (Mathf.Cos(((j * 2) * 3.141593f) / ((float) circleResolution)) * circleRadius) * tr.lossyScale.x;
                    zero.z = (Mathf.Sin(((j * 2) * 3.141593f) / ((float) circleResolution)) * circleRadius) * tr.lossyScale.z;
                    buffer.Add(V3ToIntPoint(position + zero));
                }
            }
        }

        public static void GetContour_Rectangle(List<Pathfinding.ClipperLib.IntPoint> buffer, Transform tr, Vector2 rectangleSize, Vector3 center, bool useRotation)
        {
            Vector3 position = tr.position;
            if (useRotation)
            {
                Matrix4x4 localToWorldMatrix = tr.localToWorldMatrix;
                buffer.Add(V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(center + ((Vector3) (new Vector3(-rectangleSize.x, 0f, -rectangleSize.y) * 0.5f)))));
                buffer.Add(V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(center + ((Vector3) (new Vector3(rectangleSize.x, 0f, -rectangleSize.y) * 0.5f)))));
                buffer.Add(V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(center + ((Vector3) (new Vector3(rectangleSize.x, 0f, rectangleSize.y) * 0.5f)))));
                buffer.Add(V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(center + ((Vector3) (new Vector3(-rectangleSize.x, 0f, rectangleSize.y) * 0.5f)))));
            }
            else
            {
                float x = rectangleSize.x * tr.lossyScale.x;
                float z = rectangleSize.y * tr.lossyScale.y;
                position += center.Mul(tr.lossyScale);
                buffer.Add(V3ToIntPoint(position + ((Vector3) (new Vector3(-x, 0f, -z) * 0.5f))));
                buffer.Add(V3ToIntPoint(position + ((Vector3) (new Vector3(x, 0f, -z) * 0.5f))));
                buffer.Add(V3ToIntPoint(position + ((Vector3) (new Vector3(x, 0f, z) * 0.5f))));
                buffer.Add(V3ToIntPoint(position + ((Vector3) (new Vector3(-x, 0f, z) * 0.5f))));
            }
        }

        private static bool Intersects(Bounds b1, Bounds b2)
        {
            Vector3 min = b1.min;
            Vector3 max = b1.max;
            Vector3 vector3 = b2.min;
            Vector3 vector4 = b2.max;
            return (((((min.x <= vector4.x) && (max.x >= vector3.x)) && ((min.y <= vector4.y) && (max.y >= vector3.y))) && (min.z <= vector4.z)) && (max.z >= vector3.z));
        }

        public static Vector3 IntPointToV3(Pathfinding.ClipperLib.IntPoint p)
        {
            VInt3 num = new VInt3((int) p.X, 0, (int) p.Y);
            return (Vector3) num;
        }

        public void NotifyUpdated()
        {
            this.wasEnabled = base.enabled;
            if (this.wasEnabled)
            {
                this.lastPosition = this.tr.position;
                this.lastBounds = this.GetBounds();
                if (this.useRotation)
                {
                    this.lastRotation = this.tr.rotation;
                }
            }
        }

        public void OnDestroy()
        {
            if (OnDestroyCallback != null)
            {
                OnDestroyCallback(this);
            }
            RemoveCut(this);
        }

        public void OnDrawGizmos()
        {
            if (this.tr == null)
            {
                this.tr = base.transform;
            }
            List<List<Pathfinding.ClipperLib.IntPoint>> buffer = ListPool<List<Pathfinding.ClipperLib.IntPoint>>.Claim();
            this.GetContour(buffer);
            Gizmos.color = GizmoColor;
            Bounds bounds = this.GetBounds();
            float y = bounds.min.y;
            Vector3 vector = (Vector3) (Vector3.up * (bounds.max.y - y));
            for (int i = 0; i < buffer.Count; i++)
            {
                List<Pathfinding.ClipperLib.IntPoint> list2 = buffer[i];
                for (int j = 0; j < list2.Count; j++)
                {
                    Vector3 from = IntPointToV3(list2[j]);
                    from.y = y;
                    Vector3 to = IntPointToV3(list2[(j + 1) % list2.Count]);
                    to.y = y;
                    Gizmos.DrawLine(from, to);
                    Gizmos.DrawLine(from + vector, to + vector);
                    Gizmos.DrawLine(from, from + vector);
                    Gizmos.DrawLine(to, to + vector);
                }
            }
            ListPool<List<Pathfinding.ClipperLib.IntPoint>>.Release(buffer);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.Lerp(GizmoColor, new Color(1f, 1f, 1f, 0.2f), 0.9f);
            Bounds bounds = this.GetBounds();
            Gizmos.DrawCube(bounds.center, bounds.size);
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        public void OnEnable()
        {
            this.tr = base.transform;
            this.lastPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            this.lastRotation = this.tr.rotation;
        }

        private static void RemoveCut(NavmeshCut obj)
        {
            allCuts.Remove(obj);
        }

        public bool RequiresUpdate()
        {
            return ((this.wasEnabled != base.enabled) || (this.wasEnabled && (((this.tr.position - this.lastPosition).sqrMagnitude > (this.updateDistance * this.updateDistance)) || (this.useRotation && (Quaternion.Angle(this.lastRotation, this.tr.rotation) > this.updateRotationDistance)))));
        }

        public virtual void UsedForCut()
        {
        }

        public static Pathfinding.ClipperLib.IntPoint V3ToIntPoint(Vector3 p)
        {
            VInt3 num = (VInt3) p;
            return new Pathfinding.ClipperLib.IntPoint((long) num.x, (long) num.z);
        }

        public Bounds LastBounds
        {
            get
            {
                return this.lastBounds;
            }
        }

        public class DrawGizmos
        {
            public Vector3 center;
            public float circleRadius;
            public int circleResolution;
            public float height;
            public Vector2 rectangleSize;
            public NavmeshCut.MeshType type;
            public bool useRotation;

            public void OnDrawGismos(Transform tr)
            {
                Bounds bounds;
                List<Pathfinding.ClipperLib.IntPoint> buffer = ListPool<Pathfinding.ClipperLib.IntPoint>.Claim();
                if (this.type == NavmeshCut.MeshType.Circle)
                {
                    NavmeshCut.GetContour_Circle(buffer, tr, this.circleResolution, this.circleRadius, this.center, this.useRotation);
                    bounds = NavmeshCut.GetBounds_Circle(tr, this.center, this.circleRadius, this.height, this.useRotation);
                }
                else
                {
                    NavmeshCut.GetContour_Rectangle(buffer, tr, this.rectangleSize, this.center, this.useRotation);
                    bounds = NavmeshCut.GetBounds_Rectangle(tr, this.center, this.rectangleSize, this.height, this.useRotation);
                }
                float y = bounds.min.y;
                Vector3 vector = (Vector3) (Vector3.up * (bounds.max.y - y));
                for (int i = 0; i < buffer.Count; i++)
                {
                    Vector3 from = NavmeshCut.IntPointToV3(buffer[i]);
                    from.y = y;
                    Vector3 to = NavmeshCut.IntPointToV3(buffer[(i + 1) % buffer.Count]);
                    to.y = y;
                    Gizmos.DrawLine(from, to);
                    Gizmos.DrawLine(from + vector, to + vector);
                    Gizmos.DrawLine(from, from + vector);
                    Gizmos.DrawLine(to, to + vector);
                }
                ListPool<Pathfinding.ClipperLib.IntPoint>.Release(buffer);
            }

            public void OnDrawGizmosSelected(Transform tr)
            {
                Bounds bounds;
                Gizmos.color = Color.Lerp(NavmeshCut.GizmoColor, new Color(1f, 1f, 1f, 0.2f), 0.9f);
                if (this.type == NavmeshCut.MeshType.Circle)
                {
                    bounds = NavmeshCut.GetBounds_Circle(tr, this.center, this.circleRadius, this.height, this.useRotation);
                }
                else
                {
                    bounds = NavmeshCut.GetBounds_Rectangle(tr, this.center, this.rectangleSize, this.height, this.useRotation);
                }
                Gizmos.DrawCube(bounds.center, bounds.size);
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }

        public enum MeshType
        {
            Rectangle,
            Circle,
            CustomMesh
        }
    }
}

