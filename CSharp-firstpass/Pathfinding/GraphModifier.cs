namespace Pathfinding
{
    using System;
    using UnityEngine;

    public abstract class GraphModifier : MonoBehaviour
    {
        private GraphModifier next;
        private GraphModifier prev;
        private static GraphModifier root;

        protected GraphModifier()
        {
        }

        public static void FindAllModifiers()
        {
            GraphModifier[] modifierArray = UnityEngine.Object.FindObjectsOfType(typeof(GraphModifier)) as GraphModifier[];
            for (int i = 0; i < modifierArray.Length; i++)
            {
                modifierArray[i].OnEnable();
            }
        }

        protected virtual void OnDisable()
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

        protected virtual void OnEnable()
        {
            this.OnDisable();
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

        public virtual void OnGraphsPostUpdate()
        {
        }

        public virtual void OnGraphsPreUpdate()
        {
        }

        public virtual void OnLatePostScan()
        {
        }

        public virtual void OnPostCacheLoad()
        {
        }

        public virtual void OnPostScan()
        {
        }

        public virtual void OnPreScan()
        {
        }

        public static void TriggerEvent(EventType type)
        {
            if (!Application.isPlaying)
            {
                FindAllModifiers();
            }
            GraphModifier root = GraphModifier.root;
            switch (type)
            {
                case EventType.PostScan:
                    while (root != null)
                    {
                        root.OnPostScan();
                        root = root.next;
                    }
                    return;

                case EventType.PreScan:
                    while (root != null)
                    {
                        root.OnPreScan();
                        root = root.next;
                    }
                    return;

                case EventType.LatePostScan:
                    while (root != null)
                    {
                        root.OnLatePostScan();
                        root = root.next;
                    }
                    return;

                case EventType.PreUpdate:
                    while (root != null)
                    {
                        root.OnGraphsPreUpdate();
                        root = root.next;
                    }
                    return;

                case EventType.PostUpdate:
                    while (root != null)
                    {
                        root.OnGraphsPostUpdate();
                        root = root.next;
                    }
                    break;

                case EventType.PostCacheLoad:
                    while (root != null)
                    {
                        root.OnPostCacheLoad();
                        root = root.next;
                    }
                    break;
            }
        }

        public enum EventType
        {
            LatePostScan = 4,
            PostCacheLoad = 0x20,
            PostScan = 1,
            PostUpdate = 0x10,
            PreScan = 2,
            PreUpdate = 8
        }
    }
}

