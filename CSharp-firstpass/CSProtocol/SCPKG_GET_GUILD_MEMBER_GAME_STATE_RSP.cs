namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP : ProtocolObject
    {
        public COMDT_GUILD_MEMBER_GAME_STATE[] astMemberInfo = new COMDT_GUILD_MEMBER_GAME_STATE[150];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x4f2;
        public static readonly uint CURRVERSION = 1;
        public int iMemberCnt;

        public SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP()
        {
            for (int i = 0; i < 150; i++)
            {
                this.astMemberInfo[i] = (COMDT_GUILD_MEMBER_GAME_STATE) ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_GAME_STATE.CLASS_ID);
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
            this.iMemberCnt = 0;
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
            if (this.astMemberInfo != null)
            {
                for (int i = 0; i < this.astMemberInfo.Length; i++)
                {
                    this.astMemberInfo[i] = (COMDT_GUILD_MEMBER_GAME_STATE) ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_GAME_STATE.CLASS_ID);
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
            type = destBuf.writeInt32(this.iMemberCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0 > this.iMemberCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
                }
                if (150 < this.iMemberCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astMemberInfo.Length < this.iMemberCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.iMemberCnt; i++)
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
            type = srcBuf.readInt32(ref this.iMemberCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0 > this.iMemberCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
                }
                if (150 < this.iMemberCnt)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.iMemberCnt; i++)
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

