namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_BURNING_ENEMY_HERO_DETAIL : ProtocolObject
    {
        public COMDT_BURNING_HERO_INFO[] astHeroList = new COMDT_BURNING_HERO_INFO[3];
        public static readonly uint BASEVERSION = 1;
        public byte bHeroNum;
        public static readonly int CLASS_ID = 0x9b;
        public static readonly uint CURRVERSION = 1;

        public COMDT_BURNING_ENEMY_HERO_DETAIL()
        {
            for (int i = 0; i < 3; i++)
            {
                this.astHeroList[i] = (COMDT_BURNING_HERO_INFO) ProtocolObjectPool.Get(COMDT_BURNING_HERO_INFO.CLASS_ID);
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
            this.bHeroNum = 0;
            if (this.astHeroList != null)
            {
                for (int i = 0; i < this.astHeroList.Length; i++)
                {
                    if (this.astHeroList[i] != null)
                    {
                        this.astHeroList[i].Release();
                        this.astHeroList[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astHeroList != null)
            {
                for (int i = 0; i < this.astHeroList.Length; i++)
                {
                    this.astHeroList[i] = (COMDT_BURNING_HERO_INFO) ProtocolObjectPool.Get(COMDT_BURNING_HERO_INFO.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bHeroNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (3 < this.bHeroNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astHeroList.Length < this.bHeroNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bHeroNum; i++)
                {
                    type = this.astHeroList[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bHeroNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (3 < this.bHeroNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bHeroNum; i++)
                {
                    type = this.astHeroList[i].unpack(ref srcBuf, cutVer);
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

