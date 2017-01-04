namespace Assets.Scripts.Common
{
    using System;

    public interface IObjPoolCtrl
    {
        void Release(PooledClassObject obj);
    }
}

