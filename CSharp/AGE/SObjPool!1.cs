namespace AGE
{
    using System;

    public class SObjPool<T> where T: new()
    {
        private static int allocNum;
        public static ListView<T> list;

        static SObjPool()
        {
            SObjPool<T>.list = new ListView<T>();
            SObjPool<T>.allocNum = 0;
        }

        public static void Alloc(int num)
        {
            int num2 = num - SObjPool<T>.list.Count;
            for (int i = 0; i < num2; i++)
            {
                SObjPool<T>.allocNum++;
                T local = default(T);
                SObjPool<T>.list.Add((local == null) ? Activator.CreateInstance<T>() : (local = default(T)));
            }
        }

        public static void Delete(T v)
        {
            SObjPool<T>.list.Add(v);
        }

        public static T New()
        {
            if (SObjPool<T>.list.Count > 0)
            {
                int index = SObjPool<T>.list.Count - 1;
                T local = SObjPool<T>.list[index];
                SObjPool<T>.list.RemoveAt(index);
                return local;
            }
            SObjPool<T>.allocNum++;
            T local2 = default(T);
            return ((local2 == null) ? Activator.CreateInstance<T>() : default(T));
        }
    }
}

