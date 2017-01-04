namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_PREPARE_GUILD_INFO : ProtocolObject
    {
        public COMDT_GUILD_MEMBER_BRIEF_INFO[] astMemberInfo = new COMDT_GUILD_MEMBER_BRIEF_INFO[20];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x141;
        public static readonly uint CURRVERSION = 0x84;
        public COMDT_PREPARE_GUILD_BRIEF_INFO stBriefInfo = ((COMDT_PREPARE_GUILD_BRIEF_INFO) ProtocolObjectPool.Get(COMDT_PREPARE_GUILD_BRIEF_INFO.CLASS_ID));

        public COMDT_PREPARE_GUILD_INFO()
        {
            for (int i = 0; i < 20; i++)
            {
                this.astMemberInfo[i] = (COMDT_GUILD_MEMBER_BRIEF_INFO) ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_BRIEF_INFO.CLASS_ID);
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
            if (this.stBriefInfo != null)
            {
                this.stBriefInfo.Release();
                this.stBriefInfo = null;
            }
            if (this.astMemberInfo != null)
            {
                for (int i = 0; i < this.astMemberInfo.Length; i++)
                {
                    if (this.astMemberInfo[i] != null)
                    {
                        this.astMemberInfo[i].Release();
                        this.astMemberInfo[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            this.stBriefInfo = (COMDT_PREPARE_GUILD_BRIEF_INFO) ProtocolObjectPool.Get(COMDT_PREPARE_GUILD_BRIEF_INFO.CLASS_ID);
            if (this.astMemberInfo != null)
            {
                for (int i = 0; i < this.astMemberInfo.Length; i++)
                {
                    this.astMemberInfo[i] = (COMDT_GUILD_MEMBER_BRIEF_INFO) ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_BRIEF_INFO.CLASS_ID);
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
            type = this.stBriefInfo.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (20 < this.stBriefInfo.bMemberNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astMemberInfo.Length < this.stBriefInfo.bMemberNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.stBriefInfo.bMemberNum; i++)
                {
                    type = this.astMemberInfo[i].pack(ref destBuf, cutVer);
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
            type = this.stBriefInfo.unpack(ref srcBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (20 < this.stBriefInfo.bMemberNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.stBriefInfo.bMemberNum; i++)
                {
                    type = this.astMemberInfo[i].unpack(ref srcBuf, cutVer);
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

