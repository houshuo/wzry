namespace Pathfinding.Util
{
    using System;
    using System.Collections.Generic;

    public static class ListPool<T>
    {
        private const int MaxCapacitySearchLength = 8;
        private static List<object> pool;

        static ListPool()
        {
            ListPool<T>.pool = new List<object>();
        }

        public static List<T> Claim()
        {
            if (ListPool<T>.pool.Count > 0)
            {
                List<T> list = (List<T>) ListPool<T>.pool[ListPool<T>.pool.Count - 1];
                ListPool<T>.pool.RemoveAt(ListPool<T>.pool.Count - 1);
                return list;
            }
            return new List<T>();
        }

        public static List<T> Claim(int capacity)
        {
            if (ListPool<T>.pool.Count <= 0)
            {
                return new List<T>(capacity);
            }
            List<T> list = null;
            int num = 0;
            while ((num < ListPool<T>.pool.Count) && (num < 8))
            {
                list = (List<T>) ListPool<T>.pool[(ListPool<T>.pool.Count - 1) - num];
                DebugHelper.Assert(list != null);
                if (list.Capacity >= capacity)
                {
                    ListPool<T>.pool.RemoveAt((ListPool<T>.pool.Count - 1) - num);
                    return list;
                }
                num++;
            }
            if (list == null)
            {
                return new List<T>(capacity);
            }
            list.Capacity = capacity;
            ListPool<T>.pool[ListPool<T>.pool.Count - num] = ListPool<T>.pool[ListPool<T>.pool.Count - 1];
            ListPool<T>.pool.RemoveAt(ListPool<T>.pool.Count - 1);
            return list;
        }

        public static void Clear()
        {
            ListPool<T>.pool.Clear();
        }

        public static int GetSize()
        {
            return ListPool<T>.pool.Count;
        }

        public static void Release(List<T> list)
        {
            list.Clear();
            ListPool<T>.pool.Add(list);
        }

        public static void Warmup(int count, int size)
        {
            List<T>[] listArray = new List<T>[count];
            for (int i = 0; i < count; i++)
            {
                listArray[i] = ListPool<T>.Claim(size);
            }
            for (int j = 0; j < count; j++)
            {
                ListPool<T>.Release(listArray[j]);
            }
        }
    }
}

