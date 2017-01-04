namespace Pathfinding
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("Pathfinding/Navmesh/RecastMeshObj")]
    public class RecastMeshObj : MonoBehaviour
    {
        private bool _dynamic;
        public int area;
        [HideInInspector]
        public Bounds bounds;
        public bool dynamic;
        protected static List<RecastMeshObj> dynamicMeshObjs = new List<RecastMeshObj>();
        private bool registered;
        protected static RecastBBTree tree = new RecastBBTree();

        public static void GetAllInBounds(List<RecastMeshObj> buffer, Bounds bounds)
        {
            if (!Application.isPlaying)
            {
                RecastMeshObj[] objArray = UnityEngine.Object.FindObjectsOfType(typeof(RecastMeshObj)) as RecastMeshObj[];
                for (int i = 0; i < objArray.Length; i++)
                {
                    objArray[i].RecalculateBounds();
                    if (objArray[i].GetBounds().Intersects(bounds))
                    {
                        buffer.Add(objArray[i]);
                    }
                }
            }
            else
            {
                if (Time.timeSinceLevelLoad == 0f)
                {
                    RecastMeshObj[] objArray2 = UnityEngine.Object.FindObjectsOfType(typeof(RecastMeshObj)) as RecastMeshObj[];
                    for (int k = 0; k < objArray2.Length; k++)
                    {
                        objArray2[k].Register();
                    }
                }
                for (int j = 0; j < dynamicMeshObjs.Count; j++)
                {
                    if (dynamicMeshObjs[j].GetBounds().Intersects(bounds))
                    {
                        buffer.Add(dynamicMeshObjs[j]);
                    }
                }
                Rect rect = Rect.MinMaxRect(bounds.min.x, bounds.min.z, bounds.max.x, bounds.max.z);
                tree.QueryInBounds(rect, buffer);
            }
        }

        public Bounds GetBounds()
        {
            if (this._dynamic)
            {
                this.RecalculateBounds();
            }
            return this.bounds;
        }

        public Collider GetCollider()
        {
            return base.GetComponent<Collider>();
        }

        public MeshFilter GetMeshFilter()
        {
            return base.GetComponent<MeshFilter>();
        }

        private void OnDisable()
        {
            this.registered = false;
            if (this._dynamic)
            {
                dynamicMeshObjs.Remove(this);
            }
            else if (!tree.Remove(this))
            {
                throw new Exception("Could not remove RecastMeshObj from tree even though it should exist in it. Has the object moved without being marked as dynamic?");
            }
            this._dynamic = this.dynamic;
        }

        private void OnEnable()
        {
            this.Register();
        }

        private void RecalculateBounds()
        {
            Renderer component = base.GetComponent<Renderer>();
            Collider collider = this.GetCollider();
            if ((component == null) && (collider == null))
            {
                throw new Exception("A renderer or a collider should be attached to the GameObject");
            }
            MeshFilter filter = base.GetComponent<MeshFilter>();
            if ((component != null) && (filter == null))
            {
                throw new Exception("A renderer was attached but no mesh filter");
            }
            if (component != null)
            {
                this.bounds = component.bounds;
            }
            else
            {
                this.bounds = collider.bounds;
            }
        }

        private void Register()
        {
            if (!this.registered)
            {
                this.registered = true;
                this.area = Mathf.Clamp(this.area, -1, 0x2000000);
                Renderer component = base.GetComponent<Renderer>();
                Collider collider = base.GetComponent<Collider>();
                if ((component == null) && (collider == null))
                {
                    throw new Exception("A renderer or a collider should be attached to the GameObject");
                }
                MeshFilter filter = base.GetComponent<MeshFilter>();
                if ((component != null) && (filter == null))
                {
                    throw new Exception("A renderer was attached but no mesh filter");
                }
                if (component != null)
                {
                    this.bounds = component.bounds;
                }
                else
                {
                    this.bounds = collider.bounds;
                }
                this._dynamic = this.dynamic;
                if (this._dynamic)
                {
                    dynamicMeshObjs.Add(this);
                }
                else
                {
                    tree.Insert(this);
                }
            }
        }
    }
}

