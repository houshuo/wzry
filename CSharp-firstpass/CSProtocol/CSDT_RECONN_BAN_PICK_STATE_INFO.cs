namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSDT_RECONN_BAN_PICK_STATE_INFO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bBanPosNum;
        public byte bCamp1BanNum;
        public byte bCamp2BanNum;
        public uint[] Camp1BanList = new uint[3];
        public uint[] Camp2BanList = new uint[3];
        public static readonly int CLASS_ID = 0x414;
        public static readonly uint CURRVERSION = 1;
        public CSDT_BAN_PICK_STATE_INFO stCurState = ((CSDT_BAN_PICK_STATE_INFO) ProtocolObjectPool.Get(CSDT_BAN_PICK_STATE_INFO.CLASS_ID));
        public CSDT_BAN_PICK_STATE_INFO stNextState = ((CSDT_BAN_PICK_STATE_INFO) ProtocolObjectPool.Get(CSDT_BAN_PICK_STATE_INFO.CLASS_ID));

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
            this.bCamp1BanNum = 0;
            this.bCamp2BanNum = 0;
            if (this.stCurState != null)
            {
                this.stCurState.Release();
                this.stCurState = null;
            }
            if (this.stNextState != null)
            {
                this.stNextState.Release();
                this.stNextState = null;
            }
            this.bBanPosNum = 0;
        }

        public override void OnUse()
        {
            this.stCurState = (CSDT_BAN_PICK_STATE_INFO) ProtocolObjectPool.Get(CSDT_BAN_PICK_STATE_INFO.CLASS_ID);
            this.stNextState = (CSDT_BAN_PICK_STATE_INFO) ProtocolObjectPool.Get(CSDT_BAN_PICK_STATE_INFO.CLASS_ID);
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
            type = destBuf.writeUInt8(this.bCamp1BanNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (3 < this.bCamp1BanNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.Camp1BanList.Length < this.bCamp1BanNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.bCamp1BanNum; i++)
                {
                    type = destBuf.writeUInt32(this.Camp1BanList[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt8(this.bCamp2BanNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (3 < this.bCamp2BanNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.Camp2BanList.Length < this.bCamp2BanNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int j = 0; j < this.bCamp2BanNum; j++)
                {
                    type = destBuf.writeUInt32(this.Camp2BanList[j]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stCurState.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stNextState.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bBanPosNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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
            type = srcBuf.readUInt8(ref this.bCamp1BanNum);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (3 < this.bCamp1BanNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                this.Camp1BanList = new uint[this.bCamp1BanNum];
                for (int i = 0; i < this.bCamp1BanNum; i++)
                {
                    type = srcBuf.readUInt32(ref this.Camp1BanList[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt8(ref this.bCamp2BanNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (3 < this.bCamp2BanNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                this.Camp2BanList = new uint[this.bCamp2BanNum];
                for (int j = 0; j < this.bCamp2BanNum; j++)
                {
                    type = srcBuf.readUInt32(ref this.Camp2BanList[j]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stCurState.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stNextState.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bBanPosNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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

