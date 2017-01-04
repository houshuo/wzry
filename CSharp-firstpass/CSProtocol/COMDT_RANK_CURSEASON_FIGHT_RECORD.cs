namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_RANK_CURSEASON_FIGHT_RECORD : ProtocolObject
    {
        public COMDT_INGAME_EQUIP_INFO[] astEquipDetail = new COMDT_INGAME_EQUIP_INFO[6];
        public static readonly uint BASEVERSION = 1;
        public byte bEquipNum;
        public byte bIsBanPick;
        public byte bTeamerNum;
        public static readonly int CLASS_ID = 0x16a;
        public static readonly uint CURRVERSION = 0x89;
        public uint dwAssistNum;
        public uint dwDeadNum;
        public uint dwFightTime;
        public uint dwGameResult;
        public uint dwHeroId;
        public uint dwKillNum;
        public uint dwSeasonId;
        public uint dwTalentNum;
        public uint[] Talent = new uint[5];
        public static readonly uint VERSION_astEquipDetail = 0x49;
        public static readonly uint VERSION_bEquipNum = 0x49;
        public static readonly uint VERSION_bIsBanPick = 0x89;
        public static readonly uint VERSION_bTeamerNum = 0x62;

        public COMDT_RANK_CURSEASON_FIGHT_RECORD()
        {
            for (int i = 0; i < 6; i++)
            {
                this.astEquipDetail[i] = (COMDT_INGAME_EQUIP_INFO) ProtocolObjectPool.Get(COMDT_INGAME_EQUIP_INFO.CLASS_ID);
            }
        }

        public override TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            this.dwSeasonId = 0;
            this.dwHeroId = 0;
            this.dwGameResult = 0;
            this.dwFightTime = 0;
            this.dwKillNum = 0;
            this.dwDeadNum = 0;
            this.dwAssistNum = 0;
            this.dwTalentNum = 0;
            this.bEquipNum = 0;
            for (int i = 0; i < 6; i++)
            {
                type = this.astEquipDetail[i].construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            this.bTeamerNum = 1;
            this.bIsBanPick = 0;
            return type;
        }

        public override int GetClassID()
        {
            return CLASS_ID;
        }

        public override void OnRelease()
        {
            this.dwSeasonId = 0;
            this.dwHeroId = 0;
            this.dwGameResult = 0;
            this.dwFightTime = 0;
            this.dwKillNum = 0;
            this.dwDeadNum = 0;
            this.dwAssistNum = 0;
            this.dwTalentNum = 0;
            this.bEquipNum = 0;
            if (this.astEquipDetail != null)
            {
                for (int i = 0; i < this.astEquipDetail.Length; i++)
                {
                    if (this.astEquipDetail[i] != null)
                    {
                        this.astEquipDetail[i].Release();
                        this.astEquipDetail[i] = null;
                    }
                }
            }
            this.bTeamerNum = 0;
            this.bIsBanPick = 0;
        }

        public override void OnUse()
        {
            if (this.astEquipDetail != null)
            {
                for (int i = 0; i < this.astEquipDetail.Length; i++)
                {
                    this.astEquipDetail[i] = (COMDT_INGAME_EQUIP_INFO) ProtocolObjectPool.Get(COMDT_INGAME_EQUIP_INFO.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwSeasonId);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt32(this.dwHeroId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwGameResult);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwFightTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwKillNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwDeadNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwAssistNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwTalentNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (5 < this.dwTalentNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.Talent.Length < this.dwTalentNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int i = 0; i < this.dwTalentNum; i++)
                {
                    type = destBuf.writeUInt32(this.Talent[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_bEquipNum <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bEquipNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_astEquipDetail <= cutVer)
                {
                    if (6 < this.bEquipNum)
                    {
                        return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                    }
                    if (this.astEquipDetail.Length < this.bEquipNum)
                    {
                        return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                    }
                    for (int j = 0; j < this.bEquipNum; j++)
                    {
                        type = this.astEquipDetail[j].pack(ref destBuf, cutVer);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                    }
                }
                if (VERSION_bTeamerNum <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bTeamerNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_bIsBanPick <= cutVer)
                {
                    type = destBuf.writeUInt8(this.bIsBanPick);
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
            type = srcBuf.readUInt32(ref this.dwSeasonId);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwHeroId);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwGameResult);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwFightTime);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwKillNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwDeadNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwAssistNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwTalentNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (5 < this.dwTalentNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                this.Talent = new uint[this.dwTalentNum];
                for (int i = 0; i < this.dwTalentNum; i++)
                {
                    type = srcBuf.readUInt32(ref this.Talent[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                if (VERSION_bEquipNum <= cutVer)
                {
                    type = srcBuf.readUInt8(ref this.bEquipNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.bEquipNum = 0;
                }
                if (VERSION_astEquipDetail <= cutVer)
                {
                    if (6 < this.bEquipNum)
                    {
                        return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                    }
                    for (int j = 0; j < this.bEquipNum; j++)
                    {
                        type = this.astEquipDetail[j].unpack(ref srcBuf, cutVer);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                    }
                }
                else
                {
                    if (6 < this.bEquipNum)
                    {
                        return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                    }
                    for (int k = 0; k < this.bEquipNum; k++)
                    {
                        type = this.astEquipDetail[k].construct();
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                    }
                }
                if (VERSION_bTeamerNum <= cutVer)
                {
                    type = srcBuf.readUInt8(ref this.bTeamerNum);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                else
                {
                    this.bTeamerNum = 1;
                }
                if (VERSION_bIsBanPick <= cutVer)
                {
                    type = srcBuf.readUInt8(ref this.bIsBanPick);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    return type;
                }
                this.bIsBanPick = 0;
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

