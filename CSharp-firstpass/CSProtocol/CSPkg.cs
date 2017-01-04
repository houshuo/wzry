namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSPkg : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x531;
        public static readonly uint CURRVERSION = 0x8a;
        public CSPkgBody stPkgData = ((CSPkgBody) ProtocolObjectPool.Get(CSPkgBody.CLASS_ID));
        public CSPkgHead stPkgHead = ((CSPkgHead) ProtocolObjectPool.Get(CSPkgHead.CLASS_ID));

        private void ClearVariables()
        {
            if (this.stPkgHead != null)
            {
                this.stPkgHead.Release();
                this.stPkgHead = null;
            }
            if (this.stPkgData != null)
            {
                this.stPkgData.Release();
                this.stPkgData = null;
            }
        }

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = this.stPkgHead.construct();
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                long dwMsgID = this.stPkgHead.dwMsgID;
                type = this.stPkgData.construct(dwMsgID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            return type;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public static CSPkg New()
        {
            return (CSPkg) ProtocolObjectPool.Get(CLASS_ID);
        }

        public override void OnRelease()
        {
            this.ClearVariables();
        }

        public override void OnUse()
        {
            this.ClearVariables();
            this.stPkgHead = (CSPkgHead) ProtocolObjectPool.Get(CSPkgHead.CLASS_ID);
            this.stPkgData = (CSPkgBody) ProtocolObjectPool.Get(CSPkgBody.CLASS_ID);
        }

        public override TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            type = this.stPkgHead.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                long dwMsgID = this.stPkgHead.dwMsgID;
                type = this.stPkgData.pack(dwMsgID, ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            return type;
        }

        public TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if (((buffer == null) || (buffer.GetLength(0) == 0)) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrWriteBuf destBuf = ClassObjPool<TdrWriteBuf>.Get();
            destBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                buffer = destBuf.getBeginPtr();
                usedSize = destBuf.getUsedSize();
            }
            destBuf.Release();
            return type;
        }

        public override TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            type = this.stPkgHead.unpack(ref srcBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                long dwMsgID = this.stPkgHead.dwMsgID;
                type = this.stPkgData.unpack(dwMsgID, ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            return type;
        }

        public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if (((buffer == null) || (buffer.GetLength(0) == 0)) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrReadBuf srcBuf = ClassObjPool<TdrReadBuf>.Get();
            srcBuf.set(ref buffer, size);
            TdrError.ErrorType type = this.unpack(ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            srcBuf.Release();
            return type;
        }
    }
}

