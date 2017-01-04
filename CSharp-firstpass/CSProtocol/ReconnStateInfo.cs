namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class ReconnStateInfo : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x41f;
        public static readonly uint CURRVERSION = 0x8a;
        public ProtocolObject dataObject;

        public TdrError.ErrorType construct(long selector)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            ProtocolObject obj2 = this.select(selector);
            if (obj2 != null)
            {
                return obj2.construct();
            }
            return type;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
        }

        public TdrError.ErrorType pack(long selector, ref TdrWriteBuf destBuf, uint cutVer)
        {
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            ProtocolObject obj2 = this.select(selector);
            if (obj2 != null)
            {
                return obj2.pack(ref destBuf, cutVer);
            }
            return type;
        }

        public TdrError.ErrorType pack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if ((buffer.GetLength(0) == 0) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrWriteBuf destBuf = ClassObjPool<TdrWriteBuf>.Get();
            destBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.pack(selector, ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                buffer = destBuf.getBeginPtr();
                usedSize = destBuf.getUsedSize();
            }
            destBuf.Release();
            return type;
        }

        public ProtocolObject select(long selector)
        {
            if (selector <= 5L)
            {
                this.select_1_5(selector);
            }
            else if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
            return this.dataObject;
        }

        private void select_1_5(long selector)
        {
            long num = selector;
            if ((num >= 1L) && (num <= 5L))
            {
                switch (((int) (num - 1L)))
                {
                    case 0:
                        if (!(this.dataObject is CSDT_RECONN_BANINFO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_RECONN_BANINFO) ProtocolObjectPool.Get(CSDT_RECONN_BANINFO.CLASS_ID);
                        }
                        return;

                    case 1:
                        if (!(this.dataObject is CSDT_RECONN_PICKINFO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_RECONN_PICKINFO) ProtocolObjectPool.Get(CSDT_RECONN_PICKINFO.CLASS_ID);
                        }
                        return;

                    case 2:
                        if (!(this.dataObject is CSDT_RECONN_ADJUSTINFO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_RECONN_ADJUSTINFO) ProtocolObjectPool.Get(CSDT_RECONN_ADJUSTINFO.CLASS_ID);
                        }
                        return;

                    case 3:
                        if (!(this.dataObject is CSDT_RECONN_LOADINGINFO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_RECONN_LOADINGINFO) ProtocolObjectPool.Get(CSDT_RECONN_LOADINGINFO.CLASS_ID);
                        }
                        return;

                    case 4:
                        if (!(this.dataObject is CSDT_RECONN_GAMEINGINFO))
                        {
                            if (this.dataObject != null)
                            {
                                this.dataObject.Release();
                            }
                            this.dataObject = (CSDT_RECONN_GAMEINGINFO) ProtocolObjectPool.Get(CSDT_RECONN_GAMEINGINFO.CLASS_ID);
                        }
                        return;
                }
            }
            if (this.dataObject != null)
            {
                this.dataObject.Release();
                this.dataObject = null;
            }
        }

        public TdrError.ErrorType unpack(long selector, ref TdrReadBuf srcBuf, uint cutVer)
        {
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            ProtocolObject obj2 = this.select(selector);
            if (obj2 != null)
            {
                return obj2.unpack(ref srcBuf, cutVer);
            }
            return type;
        }

        public TdrError.ErrorType unpack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if ((buffer.GetLength(0) == 0) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrReadBuf srcBuf = ClassObjPool<TdrReadBuf>.Get();
            srcBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.unpack(selector, ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            srcBuf.Release();
            return type;
        }

        public CSDT_RECONN_ADJUSTINFO stAdjustInfo
        {
            get
            {
                return (this.dataObject as CSDT_RECONN_ADJUSTINFO);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_RECONN_BANINFO stBanInfo
        {
            get
            {
                return (this.dataObject as CSDT_RECONN_BANINFO);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_RECONN_GAMEINGINFO stGamingInfo
        {
            get
            {
                return (this.dataObject as CSDT_RECONN_GAMEINGINFO);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_RECONN_LOADINGINFO stLoadingInfo
        {
            get
            {
                return (this.dataObject as CSDT_RECONN_LOADINGINFO);
            }
            set
            {
                this.dataObject = value;
            }
        }

        public CSDT_RECONN_PICKINFO stPickInfo
        {
            get
            {
                return (this.dataObject as CSDT_RECONN_PICKINFO);
            }
            set
            {
                this.dataObject = value;
            }
        }
    }
}

