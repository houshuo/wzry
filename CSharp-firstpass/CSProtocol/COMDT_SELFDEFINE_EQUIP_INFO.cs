namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_SELFDEFINE_EQUIP_INFO : ProtocolObject
    {
        public COMDT_HERO_EQUIPLIST[] astEquipInfoList = new COMDT_HERO_EQUIPLIST[200];
        public static readonly uint BASEVERSION = 1;
        public static readonly int CLASS_ID = 0x6d;
        public static readonly uint CURRVERSION = 1;
        public uint dwLastChgHeroId;
        public ushort wHeroNum;

        public COMDT_SELFDEFINE_EQUIP_INFO()
        {
            for (int i = 0; i < 200; i++)
            {
                this.astEquipInfoList[i] = (COMDT_HERO_EQUIPLIST) ProtocolObjectPool.Get(COMDT_HERO_EQUIPLIST.CLASS_ID);
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
            this.dwLastChgHeroId = 0;
            this.wHeroNum = 0;
            if (this.astEquipInfoList != null)
            {
                for (int i = 0; i < this.astEquipInfoList.Length; i++)
                {
                    if (this.astEquipInfoList[i] != null)
                    {
                        this.astEquipInfoList[i].Release();
                        this.astEquipInfoList[i] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astEquipInfoList != null)
            {
                for (int i = 0; i < this.astEquipInfoList.Length; i++)
                {
                    this.astEquipInfoList[i] = (COMDT_HERO_EQUIPLIST) ProtocolObjectPool.Get(COMDT_HERO_EQUIPLIST.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwLastChgHeroId);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt16(this.wHeroNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (200 < this.wHeroNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astEquipInfoList.Length < this.wHeroNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.wHeroNum; i++)
                {
                    type = this.astEquipInfoList[i].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwLastChgHeroId);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt16(ref this.wHeroNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (200 < this.wHeroNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int i = 0; i < this.wHeroNum; i++)
                {
                    type = this.astEquipInfoList[i].unpack(ref srcBuf, cutVer);
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

