namespace CSProtocol
{
    using Assets.Scripts.Common;
    using System;
    using tsf4g_tdr_csharp;

    public class COMDT_SETTLE_HERO_INFO : ProtocolObject
    {
        public COMDT_SETTLE_INBATTLE_EQUIP_INFO[] astInBattleEquipInfo;
        public COMDT_SKILL_STATISTIC_INFO[] astSkillStatisticInfo;
        public COMDT_SETTLE_TALENT_INFO[] astTalentDetail = new COMDT_SETTLE_TALENT_INFO[5];
        public static readonly uint BASEVERSION = 1;
        public byte bInBattleEquipNum;
        public static readonly int CLASS_ID = 0x8d;
        public static readonly uint CURRVERSION = 1;
        public uint dwBloodTTH;
        public uint dwGhostLevel;
        public uint dwHeroConfID;
        public COMDT_HERO_BATTLE_STATISTIC_INFO stHeroBattleInfo;
        public COMDT_HERO_BASE_INFO stHeroDetailInfo;

        public COMDT_SETTLE_HERO_INFO()
        {
            for (int i = 0; i < 5; i++)
            {
                this.astTalentDetail[i] = (COMDT_SETTLE_TALENT_INFO) ProtocolObjectPool.Get(COMDT_SETTLE_TALENT_INFO.CLASS_ID);
            }
            this.stHeroDetailInfo = (COMDT_HERO_BASE_INFO) ProtocolObjectPool.Get(COMDT_HERO_BASE_INFO.CLASS_ID);
            this.stHeroBattleInfo = (COMDT_HERO_BATTLE_STATISTIC_INFO) ProtocolObjectPool.Get(COMDT_HERO_BATTLE_STATISTIC_INFO.CLASS_ID);
            this.astSkillStatisticInfo = new COMDT_SKILL_STATISTIC_INFO[5];
            for (int j = 0; j < 5; j++)
            {
                this.astSkillStatisticInfo[j] = (COMDT_SKILL_STATISTIC_INFO) ProtocolObjectPool.Get(COMDT_SKILL_STATISTIC_INFO.CLASS_ID);
            }
            this.astInBattleEquipInfo = new COMDT_SETTLE_INBATTLE_EQUIP_INFO[30];
            for (int k = 0; k < 30; k++)
            {
                this.astInBattleEquipInfo[k] = (COMDT_SETTLE_INBATTLE_EQUIP_INFO) ProtocolObjectPool.Get(COMDT_SETTLE_INBATTLE_EQUIP_INFO.CLASS_ID);
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
            this.dwHeroConfID = 0;
            this.dwBloodTTH = 0;
            this.dwGhostLevel = 0;
            if (this.astTalentDetail != null)
            {
                for (int i = 0; i < this.astTalentDetail.Length; i++)
                {
                    if (this.astTalentDetail[i] != null)
                    {
                        this.astTalentDetail[i].Release();
                        this.astTalentDetail[i] = null;
                    }
                }
            }
            if (this.stHeroDetailInfo != null)
            {
                this.stHeroDetailInfo.Release();
                this.stHeroDetailInfo = null;
            }
            if (this.stHeroBattleInfo != null)
            {
                this.stHeroBattleInfo.Release();
                this.stHeroBattleInfo = null;
            }
            if (this.astSkillStatisticInfo != null)
            {
                for (int j = 0; j < this.astSkillStatisticInfo.Length; j++)
                {
                    if (this.astSkillStatisticInfo[j] != null)
                    {
                        this.astSkillStatisticInfo[j].Release();
                        this.astSkillStatisticInfo[j] = null;
                    }
                }
            }
            this.bInBattleEquipNum = 0;
            if (this.astInBattleEquipInfo != null)
            {
                for (int k = 0; k < this.astInBattleEquipInfo.Length; k++)
                {
                    if (this.astInBattleEquipInfo[k] != null)
                    {
                        this.astInBattleEquipInfo[k].Release();
                        this.astInBattleEquipInfo[k] = null;
                    }
                }
            }
        }

        public override void OnUse()
        {
            if (this.astTalentDetail != null)
            {
                for (int i = 0; i < this.astTalentDetail.Length; i++)
                {
                    this.astTalentDetail[i] = (COMDT_SETTLE_TALENT_INFO) ProtocolObjectPool.Get(COMDT_SETTLE_TALENT_INFO.CLASS_ID);
                }
            }
            this.stHeroDetailInfo = (COMDT_HERO_BASE_INFO) ProtocolObjectPool.Get(COMDT_HERO_BASE_INFO.CLASS_ID);
            this.stHeroBattleInfo = (COMDT_HERO_BATTLE_STATISTIC_INFO) ProtocolObjectPool.Get(COMDT_HERO_BATTLE_STATISTIC_INFO.CLASS_ID);
            if (this.astSkillStatisticInfo != null)
            {
                for (int j = 0; j < this.astSkillStatisticInfo.Length; j++)
                {
                    this.astSkillStatisticInfo[j] = (COMDT_SKILL_STATISTIC_INFO) ProtocolObjectPool.Get(COMDT_SKILL_STATISTIC_INFO.CLASS_ID);
                }
            }
            if (this.astInBattleEquipInfo != null)
            {
                for (int k = 0; k < this.astInBattleEquipInfo.Length; k++)
                {
                    this.astInBattleEquipInfo[k] = (COMDT_SETTLE_INBATTLE_EQUIP_INFO) ProtocolObjectPool.Get(COMDT_SETTLE_INBATTLE_EQUIP_INFO.CLASS_ID);
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
            type = destBuf.writeUInt32(this.dwHeroConfID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt32(this.dwBloodTTH);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwGhostLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 5; i++)
                {
                    type = this.astTalentDetail[i].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stHeroDetailInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeroBattleInfo.pack(ref destBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int j = 0; j < 5; j++)
                {
                    type = this.astSkillStatisticInfo[j].pack(ref destBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = destBuf.writeUInt8(this.bInBattleEquipNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (30 < this.bInBattleEquipNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                if (this.astInBattleEquipInfo.Length < this.bInBattleEquipNum)
                {
                    return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
                }
                for (int k = 0; k < this.bInBattleEquipNum; k++)
                {
                    type = this.astInBattleEquipInfo[k].pack(ref destBuf, cutVer);
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
            type = srcBuf.readUInt32(ref this.dwHeroConfID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwBloodTTH);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwGhostLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 5; i++)
                {
                    type = this.astTalentDetail[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = this.stHeroDetailInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHeroBattleInfo.unpack(ref srcBuf, cutVer);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int j = 0; j < 5; j++)
                {
                    type = this.astSkillStatisticInfo[j].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt8(ref this.bInBattleEquipNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (30 < this.bInBattleEquipNum)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                for (int k = 0; k < this.bInBattleEquipNum; k++)
                {
                    type = this.astInBattleEquipInfo[k].unpack(ref srcBuf, cutVer);
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

