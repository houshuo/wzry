namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_NTF_ACNT_INFO_UPD : ProtocolObject
    {
        public SCDT_NTF_ACNT_INFO_UPD[] astAcntUpdInfo = new SCDT_NTF_ACNT_INFO_UPD[3];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x28c;
        public static readonly uint CURRVERSION = 1;
        public int iAcntUpdNum;

        public SCPKG_NTF_ACNT_INFO_UPD()
        {
            for (int i = 0; i < 3; i++)
            {
                this.astAcntUpdInfo[i] = (SCDT_NTF_ACNT_INFO_UPD) ProtocolObjectPool.Get(SCDT_NTF_ACNT_INFO_UPD.CLASS_ID);
            }
        }

        public override TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            this.iAcntUpdNum = 0;
            if (this.astAcntUpdInfo != null)
            {
                for (int i = 0; i < this.astAcntUpdInfo.Length; i++)
                {
                    if (this.astAcntUpdInfo[i] != null)
                    {
                        this.astAcntUpdInfo[i].Release();
                        this.astAcntUpdInfo[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astAcntUpdInfo != null)
            {
                for (int i = 0; i < this.astAcntUpdInfo.Length; i++)
                {
                    this.astAcntUpdInfo[i] = (SCDT_NTF_ACNT_INFO_UPD) ProtocolObjectPool.Get(SCDT_NTF_ACNT_INFO_UPD.CLASS_ID);
                }
            }
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
            type = destBuf.writeInt32(this.iAcntUpdNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0 > this.iAcntUpdNum)
                {
                    return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
                }
                if (3 < this.iAcntUpdNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astAcntUpdInfo.Length < this.iAcntUpdNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.iAcntUpdNum; i++)
                {
                    type = this.astAcntUpdInfo[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
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
            type = srcBuf.readInt32(ref this.iAcntUpdNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0 > this.iAcntUpdNum)
                {
                    return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
                }
                if (3 < this.iAcntUpdNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.iAcntUpdNum; i++)
                {
                    type = this.astAcntUpdInfo[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
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

