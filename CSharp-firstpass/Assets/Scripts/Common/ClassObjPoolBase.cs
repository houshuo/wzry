namespace Assets.Scripts.Common
{
    using System;
    using System.Collections.Generic;

    public abstract class ClassObjPoolBase : IObjPoolCtrl
    {
        protected List<object> pool = new List<object>(0x80);
        protected uint reqSeq;

        protected ClassObjPoolBase()
        {
        }

        public abstract void Release(PooledClassObject obj);

        public int capacity
        {
            get
            {
                return this.pool.Capacity;
            }
            set
            {
                this.pool.Capacity = value;
            }
        }
    }
}

