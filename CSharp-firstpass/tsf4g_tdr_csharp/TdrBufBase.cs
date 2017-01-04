namespace tsf4g_tdr_csharp
{
    using Assets.Scripts.Common;
    using System;

    public class TdrBufBase : PooledClassObject
    {
        public TdrBufBase()
        {
            base.bChkReset = false;
        }
    }
}

