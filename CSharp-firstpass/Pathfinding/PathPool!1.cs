namespace Pathfinding
{
    using Pathfinding.Util;
    using System;
    using System.Collections.Generic;

    public static class PathPool<T> where T: Path, new()
    {
        private static Stack<object> pool;
        private static int totalCreated;

        static PathPool()
        {
            PathPool<T>.pool = new Stack<object>();
        }

        public static T GetPath()
        {
            T local;
            if (PathPool<T>.pool.Count > 0)
            {
                local = PathPool<T>.pool.Pop();
            }
            else
            {
                local = Activator.CreateInstance<T>();
                PathPool<T>.totalCreated++;
            }
            local.recycled = false;
            local.Reset();
            return local;
        }

        public static int GetSize()
        {
            return PathPool<T>.pool.Count;
        }

        public static int GetTotalCreated()
        {
            return PathPool<T>.totalCreated;
        }

        public static void Recycle(T path)
        {
            path.recycled = true;
            path.OnEnterPool();
            PathPool<T>.pool.Push(path);
        }

        public static void Warmup(int count, int length)
        {
            ListPool<GraphNode>.Warmup(count, length);
            ListPool<Vector3>.Warmup(count, length);
            Path[] o = new Path[count];
            for (int i = 0; i < count; i++)
            {
                o[i] = PathPool<T>.GetPath();
                o[i].Claim(o);
            }
            for (int j = 0; j < count; j++)
            {
                o[j].Release(o);
            }
        }
    }
}

