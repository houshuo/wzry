namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_ACNT_HEADIMG_LIST : ProtocolObject
    {
        public COMDT_ACNT_HEADIMG_INFO[] astHeadImgInfo = new COMDT_ACNT_HEADIMG_INFO[300];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 540;
        public static readonly uint CURRVERSION = 1;
        public ushort wHeadImgCnt;

        public COMDT_ACNT_HEADIMG_LIST()
        {
            for (int i = 0; i < 300; i++)
            {
                this.astHeadImgInfo[i] = (COMDT_ACNT_HEADIMG_INFO) ProtocolObjectPool.Get(COMDT_ACNT_HEADIMG_INFO.CLASS_ID);
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
            this.wHeadImgCnt = 0;
            if (this.astHeadImgInfo != null)
            {
                for (int i = 0; i < this.astHeadImgInfo.Length; i++)
                {
                    if (this.astHeadImgInfo[i] != null)
                    {
                        this.astHeadImgInfo[i].Release();
                        this.astHeadImgInfo[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astHeadImgInfo != null)
            {
                for (int i = 0; i < this.astHeadImgInfo.Length; i++)
                {
                    this.astHeadImgInfo[i] = (COMDT_ACNT_HEADIMG_INFO) ProtocolObjectPool.Get(COMDT_ACNT_HEADIMG_INFO.CLASS_ID);
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
            type = destBuf.writeUInt16(this.wHeadImgCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (300 < this.wHeadImgCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astHeadImgInfo.Length < this.wHeadImgCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.wHeadImgCnt; i++)
                {
                    type = this.astHeadImgInfo[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt16(ref this.wHeadImgCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (300 < this.wHeadImgCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.wHeadImgCnt; i++)
                {
                    type = this.astHeadImgInfo[i].unpack(ref srcBuf, cutVer);
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

