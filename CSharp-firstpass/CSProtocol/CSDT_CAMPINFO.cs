namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_CAMPINFO : ProtocolObject
    {
        public CSDT_CAMPPLAYERINFO[] astCampPlayerInfo = new CSDT_CAMPPLAYERINFO[5];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x25d;
        public static readonly uint CURRVERSION = 0x8a;
        public uint dwPlayerNum;

        public CSDT_CAMPINFO()
        {
            for (int i = 0; i < 5; i++)
            {
                this.astCampPlayerInfo[i] = (CSDT_CAMPPLAYERINFO) ProtocolObjectPool.Get(CSDT_CAMPPLAYERINFO.CLASS_ID);
            }
        }

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            this.dwPlayerNum = 0;
            for (int i = 0; i < 5; i++)
            {
                type = this.astCampPlayerInfo[i].construct();
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

        public override void OnRelease()
        {
            this.dwPlayerNum = 0;
            if (this.astCampPlayerInfo != null)
            {
                for (int i = 0; i < this.astCampPlayerInfo.Length; i++)
                {
                    if (this.astCampPlayerInfo[i] != null)
                    {
                        this.astCampPlayerInfo[i].Release();
                        this.astCampPlayerInfo[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astCampPlayerInfo != null)
            {
                for (int i = 0; i < this.astCampPlayerInfo.Length; i++)
                {
                    this.astCampPlayerInfo[i] = (CSDT_CAMPPLAYERINFO) ProtocolObjectPool.Get(CSDT_CAMPPLAYERINFO.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwPlayerNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (5 < this.dwPlayerNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astCampPlayerInfo.Length < this.dwPlayerNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.dwPlayerNum; i++)
                {
                    type = this.astCampPlayerInfo[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwPlayerNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (5 < this.dwPlayerNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.dwPlayerNum; i++)
                {
                    type = this.astCampPlayerInfo[i].unpack(ref srcBuf, cutVer);
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

