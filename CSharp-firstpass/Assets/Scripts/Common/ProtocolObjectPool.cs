namespace Assets.Scripts.Common
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public class ProtocolObjectPool
    {
        public int ClassID;
        public Type ClassType;
        private static List<ProtocolObjectPool> poolList = new List<ProtocolObjectPool>(0x400);
        public List<ProtocolObject> unusedObjs = new List<ProtocolObject>(0x80);

        static ProtocolObjectPool()
        {
            Init();
        }

        private static void AddPool(ProtocolObjectPool pool)
        {
            if (poolList.Count <= pool.ClassID)
            {
                for (int i = poolList.Count; i <= pool.ClassID; i++)
                {
                    poolList.Add(null);
                }
            }
            poolList[pool.ClassID] = pool;
        }

        public static void Clear(int nReserve = 0)
        {
            for (int i = 0; i < poolList.Count; i++)
            {
                ProtocolObjectPool pool = poolList[i];
                if (nReserve == 0)
                {
                    pool.unusedObjs.Clear();
                }
                else
                {
                    int count = pool.unusedObjs.Count;
                    int num3 = count - nReserve;
                    if (num3 > 0)
                    {
                        pool.unusedObjs.RemoveRange(count - num3, num3);
                    }
                }
            }
        }

        public static ProtocolObject Get(int ClassID)
        {
            ProtocolObjectPool pool = poolList[ClassID];
            if (pool.unusedObjs.Count > 0)
            {
                int index = pool.unusedObjs.Count - 1;
                ProtocolObject obj2 = pool.unusedObjs[index];
                pool.unusedObjs.RemoveAt(index);
                obj2.OnUse();
                return obj2;
            }
            return (ProtocolObject) Activator.CreateInstance(pool.ClassType);
        }

        public static ProtocolObjectPool GetPool(int index)
        {
            return poolList[index];
        }

        public static void Init()
        {
            if (poolList.Count <= 0)
            {
                Type c = typeof(ProtocolObject);
                foreach (Type type2 in c.Assembly.GetTypes())
                {
                    if (!type2.IsAbstract && type2.IsSubclassOf(c))
                    {
                        int num2 = (int) type2.GetField("CLASS_ID", BindingFlags.Public | BindingFlags.Static).GetValue(null);
                        ProtocolObjectPool pool = new ProtocolObjectPool {
                            ClassType = type2,
                            ClassID = num2
                        };
                        AddPool(pool);
                    }
                }
            }
        }

        public static void Release(ProtocolObject obj)
        {
            int classID = obj.GetClassID();
            poolList[classID].unusedObjs.Add(obj);
        }

        public static int PoolCount
        {
            get
            {
                return poolList.Count;
            }
        }
    }
}

