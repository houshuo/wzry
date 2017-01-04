namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSPKG_SHARE_TLOG_REQ : ProtocolObject
    {
        public CSDT_SHARE_TLOG_INFO[] astShareDetail = new CSDT_SHARE_TLOG_INFO[0x40];
        public CSDT_TRANK_TLOG_INFO[] astTrankDetail;
        public static readonly uint BASEVERSION = 1;
        public byte bNum;
        public static readonly int CLASS_ID = 0x4a4;
        public static readonly uint CURRVERSION = 1;
        public uint dwSecretaryNum;
        public uint dwTrankNum;
        public uint[] SecretaryDetail;

        public CSPKG_SHARE_TLOG_REQ()
        {
            for (int i = 0; i < 0x40; i++)
            {
                this.astShareDetail[i] = (CSDT_SHARE_TLOG_INFO) ProtocolObjectPool.Get(CSDT_SHARE_TLOG_INFO.CLASS_ID);
            }
            this.SecretaryDetail = new uint[0x40];
            this.astTrankDetail = new CSDT_TRANK_TLOG_INFO[0x42];
            for (int j = 0; j < 0x42; j++)
            {
                this.astTrankDetail[j] = (CSDT_TRANK_TLOG_INFO) ProtocolObjectPool.Get(CSDT_TRANK_TLOG_INFO.CLASS_ID);
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
            this.bNum = 0;
            if (this.astShareDetail != null)
            {
                for (int i = 0; i < this.astShareDetail.Length; i++)
                {
                    if (this.astShareDetail[i] != null)
                    {
                        this.astShareDetail[i].Release();
                        this.astShareDetail[i] = null;
                    }
                }
            }
            this.dwSecretaryNum = 0;
            this.dwTrankNum = 0;
            if (this.astTrankDetail != null)
            {
                for (int j = 0; j < this.astTrankDetail.Length; j++)
                {
                    if (this.astTrankDetail[j] != null)
                    {
                        this.astTrankDetail[j].Release();
                        this.astTrankDetail[j] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astShareDetail != null)
            {
                for (int i = 0; i < this.astShareDetail.Length; i++)
                {
                    this.astShareDetail[i] = (CSDT_SHARE_TLOG_INFO) ProtocolObjectPool.Get(CSDT_SHARE_TLOG_INFO.CLASS_ID);
                }
            }
            if (this.astTrankDetail != null)
            {
                for (int j = 0; j < this.astTrankDetail.Length; j++)
                {
                    this.astTrankDetail[j] = (CSDT_TRANK_TLOG_INFO) ProtocolObjectPool.Get(CSDT_TRANK_TLOG_INFO.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0x40 < this.bNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astShareDetail.Length < this.bNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bNum; i++)
                {
                    type = this.astShareDetail[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt32(this.dwSecretaryNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (0x40 < this.dwSecretaryNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.SecretaryDetail.Length < this.dwSecretaryNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int j = 0; j < this.dwSecretaryNum; j++)
                {
                    type = destBuf.writeUInt32(this.SecretaryDetail[j]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt32(this.dwTrankNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (0x42 < this.dwTrankNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astTrankDetail.Length < this.dwTrankNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int k = 0; k < this.dwTrankNum; k++)
                {
                    type = this.astTrankDetail[k].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0x40 < this.bNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bNum; i++)
                {
                    type = this.astShareDetail[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwSecretaryNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (0x40 < this.dwSecretaryNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                this.SecretaryDetail = new uint[this.dwSecretaryNum];
                for (int j = 0; j < this.dwSecretaryNum; j++)
                {
                    type = srcBuf.readUInt32(ref this.SecretaryDetail[j]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwTrankNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (0x42 < this.dwTrankNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int k = 0; k < this.dwTrankNum; k++)
                {
                    type = this.astTrankDetail[k].unpack(ref srcBuf, cutVer);
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

