namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_PVPSPECITEM_OUTPUT : ProtocolObject
    {
        public COMDT_PVPSPEC_OUTPUTINFO[] astOutputInfo = new COMDT_PVPSPEC_OUTPUTINFO[3];
        public static readonly uint BASEVERSION = 1;
        public byte bOutputCnt;
        public static readonly int CLASS_ID = 0xd0;
        public static readonly uint CURRVERSION = 1;

        public COMDT_PVPSPECITEM_OUTPUT()
        {
            for (int i = 0; i < 3; i++)
            {
                this.astOutputInfo[i] = (COMDT_PVPSPEC_OUTPUTINFO) ProtocolObjectPool.Get(COMDT_PVPSPEC_OUTPUTINFO.CLASS_ID);
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
            this.bOutputCnt = 0;
            if (this.astOutputInfo != null)
            {
                for (int i = 0; i < this.astOutputInfo.Length; i++)
                {
                    if (this.astOutputInfo[i] != null)
                    {
                        this.astOutputInfo[i].Release();
                        this.astOutputInfo[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astOutputInfo != null)
            {
                for (int i = 0; i < this.astOutputInfo.Length; i++)
                {
                    this.astOutputInfo[i] = (COMDT_PVPSPEC_OUTPUTINFO) ProtocolObjectPool.Get(COMDT_PVPSPEC_OUTPUTINFO.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bOutputCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (3 < this.bOutputCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astOutputInfo.Length < this.bOutputCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bOutputCnt; i++)
                {
                    type = this.astOutputInfo[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bOutputCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (3 < this.bOutputCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bOutputCnt; i++)
                {
                    type = this.astOutputInfo[i].unpack(ref srcBuf, cutVer);
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

