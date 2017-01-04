namespace Pathfinding
{
    using System;
    using UnityEngine;

    [AddComponentMenu("Pathfinding/Navmesh/RelevantGraphSurface")]
    public class RelevantGraphSurface : MonoBehaviour
    {
        public float maxRange = 1f;
        private RelevantGraphSurface next;
        private Vector3 position;
        private RelevantGraphSurface prev;
        private static RelevantGraphSurface root;

        public static void FindAllGraphSurfaces()
        {
            RelevantGraphSurface[] surfaceArray = UnityEngine.Object.FindObjectsOfType(typeof(RelevantGraphSurface)) as RelevantGraphSurface[];
            for (int i = 0; i < surfaceArray.Length; i++)
            {
                surfaceArray[i].OnDisable();
                surfaceArray[i].OnEnable();
            }
        }

        private void OnDisable()
        {
            if (root == this)
            {
                root = this.next;
                if (root != null)
                {
                    root.prev = null;
                }
            }
            else
            {
                if (this.prev != null)
                {
                    this.prev.next = this.next;
                }
                if (this.next != null)
                {
                    this.next.prev = this.prev;
                }
            }
            this.prev = null;
            this.next = null;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2235294f, 0.827451f, 0.1803922f, 0.4f);
            Gizmos.DrawLine(base.transform.position - ((Vector3) (Vector3.up * this.maxRange)), base.transform.position + ((Vector3) (Vector3.up * this.maxRange)));
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.2235294f, 0.827451f, 0.1803922f);
            Gizmos.DrawLine(base.transform.position - ((Vector3) (Vector3.up * this.maxRange)), base.transform.position + ((Vector3) (Vector3.up * this.maxRange)));
        }

        private void OnEnable()
        {
            this.UpdatePosition();
            if (root == null)
            {
                root = this;
            }
            else
            {
                this.next = root;
                root.prev = this;
                root = this;
            }
        }

        public static void UpdateAllPositions()
        {
            for (RelevantGraphSurface surface = root; surface != null; surface = surface.Next)
            {
                surface.UpdatePosition();
            }
        }

        public void UpdatePosition()
        {
            this.position = base.transform.position;
        }

        public RelevantGraphSurface Next
        {
            get
            {
                return this.next;
            }
        }

        public Vector3 Position
        {
            get
            {
                return this.position;
            }
        }

        public RelevantGraphSurface Prev
        {
            get
            {
                return this.prev;
            }
        }

        public static RelevantGraphSurface Root
        {
            get
            {
                return root;
            }
        }
    }
}

