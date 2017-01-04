namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_SVRHERO_LIST : ProtocolObject
    {
        public CSDT_HERO_DETAIL[] astHeroInfoList = new CSDT_HERO_DETAIL[200];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x2bb;
        public static readonly uint CURRVERSION = 0x8a;
        public uint dwHeroNum;

        public CSDT_SVRHERO_LIST()
        {
            for (int i = 0; i < 200; i++)
            {
                this.astHeroInfoList[i] = (CSDT_HERO_DETAIL) ProtocolObjectPool.Get(CSDT_HERO_DETAIL.CLASS_ID);
            }
        }

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            this.dwHeroNum = 0;
            for (int i = 0; i < 200; i++)
            {
                type = this.astHeroInfoList[i].construct();
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
            this.dwHeroNum = 0;
            if (this.astHeroInfoList != null)
            {
                for (int i = 0; i < this.astHeroInfoList.Length; i++)
                {
                    if (this.astHeroInfoList[i] != null)
                    {
                        this.astHeroInfoList[i].Release();
                        this.astHeroInfoList[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astHeroInfoList != null)
            {
                for (int i = 0; i < this.astHeroInfoList.Length; i++)
                {
                    this.astHeroInfoList[i] = (CSDT_HERO_DETAIL) ProtocolObjectPool.Get(CSDT_HERO_DETAIL.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwHeroNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (200 < this.dwHeroNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astHeroInfoList.Length < this.dwHeroNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.dwHeroNum; i++)
                {
                    type = this.astHeroInfoList[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwHeroNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (200 < this.dwHeroNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.dwHeroNum; i++)
                {
                    type = this.astHeroInfoList[i].unpack(ref srcBuf, cutVer);
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

