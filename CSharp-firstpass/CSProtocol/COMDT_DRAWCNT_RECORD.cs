namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_DRAWCNT_RECORD : ProtocolObject
    {
        public COMDT_DRAWCNT_SUBINFO[] astSubDrawInfo = new COMDT_DRAWCNT_SUBINFO[5];
        public static readonly uint BASEVERSION = 1;
        public byte bSubDrawCnt;
        public static readonly int CLASS_ID = 0x10f;
        public static readonly uint CURRVERSION = 0x47;
        public int iFreeDrawTime;
        public int iFreeDrawTotalCnt;
        public static readonly uint VERSION_iFreeDrawTotalCnt = 0x47;

        public COMDT_DRAWCNT_RECORD()
        {
            for (int i = 0; i < 5; i++)
            {
                this.astSubDrawInfo[i] = (COMDT_DRAWCNT_SUBINFO) ProtocolObjectPool.Get(COMDT_DRAWCNT_SUBINFO.CLASS_ID);
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
            this.iFreeDrawTime = 0;
            this.iFreeDrawTotalCnt = 0;
            this.bSubDrawCnt = 0;
            if (this.astSubDrawInfo != null)
            {
                for (int i = 0; i < this.astSubDrawInfo.Length; i++)
                {
                    if (this.astSubDrawInfo[i] != null)
                    {
                        this.astSubDrawInfo[i].Release();
                        this.astSubDrawInfo[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astSubDrawInfo != null)
            {
                for (int i = 0; i < this.astSubDrawInfo.Length; i++)
                {
                    this.astSubDrawInfo[i] = (COMDT_DRAWCNT_SUBINFO) ProtocolObjectPool.Get(COMDT_DRAWCNT_SUBINFO.CLASS_ID);
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
            type = destBuf.writeInt32(this.iFreeDrawTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (VERSION_iFreeDrawTotalCnt <= cutVer)
                {
                    type = destBuf.writeInt32(this.iFreeDrawTotalCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt8(this.bSubDrawCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (5 < this.bSubDrawCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astSubDrawInfo.Length < this.bSubDrawCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bSubDrawCnt; i++)
                {
                    type = this.astSubDrawInfo[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readInt32(ref this.iFreeDrawTime);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (VERSION_iFreeDrawTotalCnt <= cutVer)
                {
                    type = srcBuf.readInt32(ref this.iFreeDrawTotalCnt);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.iFreeDrawTotalCnt = 0;
                }
                type = srcBuf.readUInt8(ref this.bSubDrawCnt);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (5 < this.bSubDrawCnt)
                    {
                        return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                    }
                    for (int i = 0; i < this.bSubDrawCnt; i++)
                    {
                        type = this.astSubDrawInfo[i].unpack(ref srcBuf, cutVer);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
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

