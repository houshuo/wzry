namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSPKG_MULTGAME_GAMEOVER : ProtocolObject
    {
        public CSDT_MULTIGAMEOVER_INFO[] astAcntInfo = new CSDT_MULTIGAMEOVER_INFO[10];
        public byte bAcntNum;
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x277;
        public static readonly uint CURRVERSION = 1;

        public CSPKG_MULTGAME_GAMEOVER()
        {
            for (int i = 0; i < 10; i++)
            {
                this.astAcntInfo[i] = (CSDT_MULTIGAMEOVER_INFO) ProtocolObjectPool.Get(CSDT_MULTIGAMEOVER_INFO.CLASS_ID);
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
            this.bAcntNum = 0;
            if (this.astAcntInfo != null)
            {
                for (int i = 0; i < this.astAcntInfo.Length; i++)
                {
                    if (this.astAcntInfo[i] != null)
                    {
                        this.astAcntInfo[i].Release();
                        this.astAcntInfo[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astAcntInfo != null)
            {
                for (int i = 0; i < this.astAcntInfo.Length; i++)
                {
                    this.astAcntInfo[i] = (CSDT_MULTIGAMEOVER_INFO) ProtocolObjectPool.Get(CSDT_MULTIGAMEOVER_INFO.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bAcntNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (10 < this.bAcntNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astAcntInfo.Length < this.bAcntNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bAcntNum; i++)
                {
                    type = this.astAcntInfo[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bAcntNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (10 < this.bAcntNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bAcntNum; i++)
                {
                    type = this.astAcntInfo[i].unpack(ref srcBuf, cutVer);
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

