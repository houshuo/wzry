namespace Assets.Scripts.Common
{
    using System;

    public class ClassObjPool<T> : ClassObjPoolBase where T: PooledClassObject, new()
    {
        private static ClassObjPool<T> instance;

        public static T Get()
        {
            if (ClassObjPool<T>.instance == null)
            {
                ClassObjPool<T>.instance = new ClassObjPool<T>();
            }
            if (ClassObjPool<T>.instance.pool.Count > 0)
            {
                T local = ClassObjPool<T>.instance.pool[ClassObjPool<T>.instance.pool.Count - 1];
                ClassObjPool<T>.instance.pool.RemoveAt(ClassObjPool<T>.instance.pool.Count - 1);
                ClassObjPool<T>.instance.reqSeq++;
                local.usingSeq = ClassObjPool<T>.instance.reqSeq;
                local.holder = ClassObjPool<T>.instance;
                local.OnUse();
                return local;
            }
            T local2 = Activator.CreateInstance<T>();
            ClassObjPool<T>.instance.reqSeq++;
            local2.usingSeq = ClassObjPool<T>.instance.reqSeq;
            local2.holder = ClassObjPool<T>.instance;
            local2.OnUse();
            return local2;
        }

        public static uint NewSeq()
        {
            if (ClassObjPool<T>.instance == null)
            {
                ClassObjPool<T>.instance = new ClassObjPool<T>();
            }
            ClassObjPool<T>.instance.reqSeq++;
            return ClassObjPool<T>.instance.reqSeq;
        }

        public override void Release(PooledClassObject obj)
        {
            T item = obj as T;
            obj.usingSeq = 0;
            obj.holder = null;
            base.pool.Add(item);
        }
    }
}

