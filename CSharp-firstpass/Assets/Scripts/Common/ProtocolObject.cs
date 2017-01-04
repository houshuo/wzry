namespace Assets.Scripts.Common
{
    using System;
    using tsf4g_tdr_csharp;

    public abstract class ProtocolObject
    {
        protected ProtocolObject()
        {
        }

        public virtual TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public abstract int GetClassID();
        public virtual void OnRelease()
        {
        }

        public virtual void OnUse()
        {
        }

        public virtual TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public void Release()
        {
            this.OnRelease();
            ProtocolObjectPool.Release(this);
        }

        public virtual TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }
    }
}

