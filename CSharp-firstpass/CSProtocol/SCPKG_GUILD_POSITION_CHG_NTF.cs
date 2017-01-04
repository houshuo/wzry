namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class SCPKG_GUILD_POSITION_CHG_NTF : ProtocolObject
    {
        public COMDT_MEMBER_POSITION[] astChgInfo = new COMDT_MEMBER_POSITION[8];
        public static readonly uint BASEVERSION = 1;
        public byte bCount;
        public static readonly int CLASS_ID = 0x461;
        public static readonly uint CURRVERSION = 1;

        public SCPKG_GUILD_POSITION_CHG_NTF()
        {
            for (int i = 0; i < 8; i++)
            {
                this.astChgInfo[i] = (COMDT_MEMBER_POSITION) ProtocolObjectPool.Get(COMDT_MEMBER_POSITION.CLASS_ID);
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
            this.bCount = 0;
            if (this.astChgInfo != null)
            {
                for (int i = 0; i < this.astChgInfo.Length; i++)
                {
                    if (this.astChgInfo[i] != null)
                    {
                        this.astChgInfo[i].Release();
                        this.astChgInfo[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astChgInfo != null)
            {
                for (int i = 0; i < this.astChgInfo.Length; i++)
                {
                    this.astChgInfo[i] = (COMDT_MEMBER_POSITION) ProtocolObjectPool.Get(COMDT_MEMBER_POSITION.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bCount);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (8 < this.bCount)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astChgInfo.Length < this.bCount)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bCount; i++)
                {
                    type = this.astChgInfo[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt8(ref this.bCount);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (8 < this.bCount)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.bCount; i++)
                {
                    type = this.astChgInfo[i].unpack(ref srcBuf, cutVer);
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

