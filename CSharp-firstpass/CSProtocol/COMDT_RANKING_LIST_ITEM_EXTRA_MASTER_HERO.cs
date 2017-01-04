namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_RANKING_LIST_ITEM_EXTRA_MASTER_HERO : ProtocolObject
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 470;
        public static readonly uint CURRVERSION = 0x6b;
        public uint dwGameCnt;
        public uint dwWinCnt;
        public COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER stAcntInfo = ((COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID));
        public COMDT_EQUIP_LIST stEquipList = ((COMDT_EQUIP_LIST) ProtocolObjectPool.Get(COMDT_EQUIP_LIST.CLASS_ID));
        public COMDT_SYMBOLPAGE_INFO stSymbolPageInfo = ((COMDT_SYMBOLPAGE_INFO) ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_INFO.CLASS_ID));

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
            this.dwWinCnt = 0;
            this.dwGameCnt = 0;
            if (this.stAcntInfo != null)
            {
                this.stAcntInfo.Release();
                this.stAcntInfo = null;
            }
            if (this.stEquipList != null)
            {
                this.stEquipList.Release();
                this.stEquipList = null;
            }
            if (this.stSymbolPageInfo != null)
            {
                this.stSymbolPageInfo.Release();
                this.stSymbolPageInfo = null;
            }
        }

        public override void OnUse()
        {
            this.stAcntInfo = (COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER) ProtocolObjectPool.Get(COMDT_RANKING_LIST_ITEM_EXTRA_PLAYER.CLASS_ID);
            this.stEquipList = (COMDT_EQUIP_LIST) ProtocolObjectPool.Get(COMDT_EQUIP_LIST.CLASS_ID);
            this.stSymbolPageInfo = (COMDT_SYMBOLPAGE_INFO) ProtocolObjectPool.Get(COMDT_SYMBOLPAGE_INFO.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwWinCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt32(this.dwGameCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stAcntInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stEquipList.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSymbolPageInfo.pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwWinCnt);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwGameCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stAcntInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stEquipList.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stSymbolPageInfo.unpack(ref srcBuf, cutVer);
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

