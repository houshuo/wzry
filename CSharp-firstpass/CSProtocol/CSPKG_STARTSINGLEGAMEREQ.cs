namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class CSPKG_STARTSINGLEGAMEREQ : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 620;
        public static readonly uint CURRVERSION = 0x8a;
        public COMDT_BATTLELIST stBattleList = ((COMDT_BATTLELIST) ProtocolObjectPool.Get(COMDT_BATTLELIST.CLASS_ID));
        public CSDT_START_SINGLE_GAME_PARAM stBattleParam = ((CSDT_START_SINGLE_GAME_PARAM) ProtocolObjectPool.Get(CSDT_START_SINGLE_GAME_PARAM.CLASS_ID));
        public CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer = ((CSDT_BATTLE_PLAYER_BRIEF) ProtocolObjectPool.Get(CSDT_BATTLE_PLAYER_BRIEF.CLASS_ID));

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = this.stBattlePlayer.construct();
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stBattleParam.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stBattleList.construct();
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
            if (this.stBattlePlayer != null)
            {
                this.stBattlePlayer.Release();
                this.stBattlePlayer = null;
            }
            if (this.stBattleParam != null)
            {
                this.stBattleParam.Release();
                this.stBattleParam = null;
            }
            if (this.stBattleList != null)
            {
                this.stBattleList.Release();
                this.stBattleList = null;
            }
        }

        public override void OnUse()
        {
            this.stBattlePlayer = (CSDT_BATTLE_PLAYER_BRIEF) ProtocolObjectPool.Get(CSDT_BATTLE_PLAYER_BRIEF.CLASS_ID);
            this.stBattleParam = (CSDT_START_SINGLE_GAME_PARAM) ProtocolObjectPool.Get(CSDT_START_SINGLE_GAME_PARAM.CLASS_ID);
            this.stBattleList = (COMDT_BATTLELIST) ProtocolObjectPool.Get(COMDT_BATTLELIST.CLASS_ID);
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
            type = this.stBattlePlayer.pack(ref destBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stBattleParam.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stBattleList.pack(ref destBuf, cutVer);
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
            type = this.stBattlePlayer.unpack(ref srcBuf, cutVer);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stBattleParam.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stBattleList.unpack(ref srcBuf, cutVer);
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

