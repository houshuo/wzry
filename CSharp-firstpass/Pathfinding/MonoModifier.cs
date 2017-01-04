namespace Pathfinding
{
    using System;
    using UnityEngine;

    [Serializable]
    public abstract class MonoModifier : MonoBehaviour, IPathModifier
    {
        public int priority;
        [NonSerialized]
        public Seeker seeker;

        protected MonoModifier()
        {
        }

        public abstract void Apply(Path p, ModifierData source);
        [Obsolete]
        public virtual Vector3[] Apply(Vector3[] path, Vector3 start, Vector3 end)
        {
            return path;
        }

        [Obsolete]
        public virtual Vector3[] Apply(GraphNode[] path, Vector3 start, Vector3 end, int startIndex, int endIndex, NavGraph graph)
        {
            Vector3[] vectorArray = new Vector3[endIndex - startIndex];
            for (int i = startIndex; i < endIndex; i++)
            {
                vectorArray[i - startIndex] = (Vector3) path[i].position;
            }
            return vectorArray;
        }

        [Obsolete]
        public virtual void ApplyOriginal(Path p)
        {
        }

        public void Awake()
        {
            this.seeker = base.GetComponent<Seeker>();
            if (this.seeker != null)
            {
                this.seeker.RegisterModifier(this);
            }
        }

        public void OnDestroy()
        {
            if (this.seeker != null)
            {
                this.seeker.DeregisterModifier(this);
            }
        }

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
        }

        [Obsolete]
        public virtual void PreProcess(Path p)
        {
        }

        public abstract ModifierData input { get; }

        public abstract ModifierData output { get; }

        public int Priority
        {
            get
            {
                return this.priority;
            }
            set
            {
                this.priority = value;
            }
        }
    }
}

