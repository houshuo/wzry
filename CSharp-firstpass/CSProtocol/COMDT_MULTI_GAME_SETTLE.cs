namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_MULTI_GAME_SETTLE : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public byte bFinishType;
        public static readonly int CLASS_ID = 0xaf;
        public static readonly uint CURRVERSION = 1;
        public int iSettleType;
        public COMDT_MULTI_GAME_PARAM stBattleParam = ((COMDT_MULTI_GAME_PARAM) ProtocolObjectPool.Get(COMDT_MULTI_GAME_PARAM.CLASS_ID));
        public COMDT_INGAME_CHEAT_DETAIL stCheatDetail = ((COMDT_INGAME_CHEAT_DETAIL) ProtocolObjectPool.Get(COMDT_INGAME_CHEAT_DETAIL.CLASS_ID));
        public COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL stMemberDetail = ((COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL) ProtocolObjectPool.Get(COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL.CLASS_ID));
        public COMDT_MULTI_GAME_SERVER_PARAM stServerParam = ((COMDT_MULTI_GAME_SERVER_PARAM) ProtocolObjectPool.Get(COMDT_MULTI_GAME_SERVER_PARAM.CLASS_ID));
        public COMDT_MULTIGAME_SETTLE_UNION stSettleDetail = ((COMDT_MULTIGAME_SETTLE_UNION) ProtocolObjectPool.Get(COMDT_MULTIGAME_SETTLE_UNION.CLASS_ID));
        public ulong ullUserQQ;

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
            this.ullUserQQ = 0L;
            this.iSettleType = 0;
            if (this.stCheatDetail != null)
            {
                this.stCheatDetail.Release();
                this.stCheatDetail = null;
            }
            if (this.stBattleParam != null)
            {
                this.stBattleParam.Release();
                this.stBattleParam = null;
            }
            if (this.stServerParam != null)
            {
                this.stServerParam.Release();
                this.stServerParam = null;
            }
            if (this.stMemberDetail != null)
            {
                this.stMemberDetail.Release();
                this.stMemberDetail = null;
            }
            this.bFinishType = 0;
            if (this.stSettleDetail != null)
            {
                this.stSettleDetail.Release();
                this.stSettleDetail = null;
            }
        }

        public override void OnUse()
        {
            this.stCheatDetail = (COMDT_INGAME_CHEAT_DETAIL) ProtocolObjectPool.Get(COMDT_INGAME_CHEAT_DETAIL.CLASS_ID);
            this.stBattleParam = (COMDT_MULTI_GAME_PARAM) ProtocolObjectPool.Get(COMDT_MULTI_GAME_PARAM.CLASS_ID);
            this.stServerParam = (COMDT_MULTI_GAME_SERVER_PARAM) ProtocolObjectPool.Get(COMDT_MULTI_GAME_SERVER_PARAM.CLASS_ID);
            this.stMemberDetail = (COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL) ProtocolObjectPool.Get(COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL.CLASS_ID);
            this.stSettleDetail = (COMDT_MULTIGAME_SETTLE_UNION) ProtocolObjectPool.Get(COMDT_MULTIGAME_SETTLE_UNION.CLASS_ID);
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
            type = destBuf.writeUInt64(this.ullUserQQ);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeInt32(this.iSettleType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stCheatDetail.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stBattleParam.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stServerParam.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stMemberDetail.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bFinishType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                long bFinishType = this.bFinishType;
                type = this.stSettleDetail.pack(bFinishType, ref destBuf, cutVer);
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
            type = srcBuf.readUInt64(ref this.ullUserQQ);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readInt32(ref this.iSettleType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stCheatDetail.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stBattleParam.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stServerParam.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stMemberDetail.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bFinishType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                long bFinishType = this.bFinishType;
                type = this.stSettleDetail.unpack(bFinishType, ref srcBuf, cutVer);
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

