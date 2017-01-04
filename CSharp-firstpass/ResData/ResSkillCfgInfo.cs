namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResSkillCfgInfo : IUnpackable, tsf4g_csharp_interface
    {
        public byte bAgeImmeExcute;
        public static readonly uint BASEVERSION = 1;
        public byte bBIngnoreDisable;
        public byte bImmediateUse;
        public byte bIndicatorType;
        public byte bIsCheckAntiCheat;
        public byte bIsInterruptImmediateUseSkill;
        public byte bIsStunSkill;
        public byte bNoInfluenceAnim;
        public byte bSkillType;
        public byte bWheelType;
        public static readonly uint CURRVERSION = 1;
        public uint dwAutoEnergyCost;
        public uint dwEnergyCostType;
        public uint dwIsBullet;
        public uint dwRangeAppointType;
        public uint dwSkillTargetFilter;
        public uint dwSkillTargetRule;
        public uint dwSkillUseRule;
        public uint dwTgtIncludeEnemy;
        public uint dwTgtIncludeSelf;
        public CrypticInteger iBaseDamage;
        public int iBulletID;
        public int iCfgID;
        public CrypticInteger iCoolDown;
        public CrypticInteger iCoolDownGrowth;
        public CrypticInteger iEnergyCost;
        public int iEnergyCostCalcType;
        public CrypticInteger iEnergyCostGrowth;
        public CrypticInteger iFixedDistance;
        public int iGreaterAttackDistance;
        public CrypticInteger iGuideDistance;
        public CrypticInteger iMaxAttackDistance;
        public int iMaxSearchDistance;
        public CrypticInteger iRangeRadius;
        public int iSelfSkillCombine;
        public int iSkillCombatType;
        public int iTargetSkillCombine;
        public static readonly uint LENGTH_szEffectPrefab = 0x80;
        public static readonly uint LENGTH_szEffectWarnPrefab = 0x80;
        public static readonly uint LENGTH_szFixedPrefab = 0x80;
        public static readonly uint LENGTH_szFixedWarnPrefab = 0x80;
        public static readonly uint LENGTH_szGuidePrefab = 0x80;
        public static readonly uint LENGTH_szGuideWarnPrefab = 0x80;
        public static readonly uint LENGTH_szIconPath = 0x80;
        public static readonly uint LENGTH_szPrefab = 0x80;
        public static readonly uint LENGTH_szSkillDesc = 0x400;
        public static readonly uint LENGTH_szSkillName = 0x80;
        public static readonly uint LENGTH_szSkillUpTip = 0x100;
        public static readonly uint LENGTH_szSkillValueDesc = 0x400;
        public uint[] SkillEffectType = new uint[2];
        public string szEffectPrefab = string.Empty;
        public byte[] szEffectPrefab_ByteArray = new byte[1];
        public string szEffectWarnPrefab = string.Empty;
        public byte[] szEffectWarnPrefab_ByteArray = new byte[1];
        public string szFixedPrefab = string.Empty;
        public byte[] szFixedPrefab_ByteArray = new byte[1];
        public string szFixedWarnPrefab = string.Empty;
        public byte[] szFixedWarnPrefab_ByteArray = new byte[1];
        public string szGuidePrefab = string.Empty;
        public byte[] szGuidePrefab_ByteArray = new byte[1];
        public string szGuideWarnPrefab = string.Empty;
        public byte[] szGuideWarnPrefab_ByteArray = new byte[1];
        public string szIconPath = string.Empty;
        public byte[] szIconPath_ByteArray = new byte[1];
        public string szPrefab = string.Empty;
        public byte[] szPrefab_ByteArray = new byte[1];
        public string szSkillDesc = string.Empty;
        public byte[] szSkillDesc_ByteArray = new byte[1];
        public string szSkillName = string.Empty;
        public byte[] szSkillName_ByteArray = new byte[1];
        public string szSkillUpTip = string.Empty;
        public byte[] szSkillUpTip_ByteArray = new byte[1];
        public string szSkillValueDesc = string.Empty;
        public byte[] szSkillValueDesc_ByteArray = new byte[1];

        public TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType load(ref TdrReadBuf srcBuf, uint cutVer)
        {
            int dest = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            int num7 = 0;
            int num8 = 0;
            int num9 = 0;
            srcBuf.disableEndian();
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            type = srcBuf.readInt32(ref this.iCfgID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int count = 0x80;
                if (this.szSkillName_ByteArray.GetLength(0) < count)
                {
                    this.szSkillName_ByteArray = new byte[LENGTH_szSkillName];
                }
                type = srcBuf.readCString(ref this.szSkillName_ByteArray, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num11 = 0x400;
                if (this.szSkillDesc_ByteArray.GetLength(0) < num11)
                {
                    this.szSkillDesc_ByteArray = new byte[LENGTH_szSkillDesc];
                }
                type = srcBuf.readCString(ref this.szSkillDesc_ByteArray, num11);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bSkillType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bWheelType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num12 = 0x400;
                if (this.szSkillValueDesc_ByteArray.GetLength(0) < num12)
                {
                    this.szSkillValueDesc_ByteArray = new byte[LENGTH_szSkillValueDesc];
                }
                type = srcBuf.readCString(ref this.szSkillValueDesc_ByteArray, num12);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num13 = 0x100;
                if (this.szSkillUpTip_ByteArray.GetLength(0) < num13)
                {
                    this.szSkillUpTip_ByteArray = new byte[LENGTH_szSkillUpTip];
                }
                type = srcBuf.readCString(ref this.szSkillUpTip_ByteArray, num13);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num14 = 0x80;
                if (this.szIconPath_ByteArray.GetLength(0) < num14)
                {
                    this.szIconPath_ByteArray = new byte[LENGTH_szIconPath];
                }
                type = srcBuf.readCString(ref this.szIconPath_ByteArray, num14);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num15 = 0x80;
                if (this.szGuidePrefab_ByteArray.GetLength(0) < num15)
                {
                    this.szGuidePrefab_ByteArray = new byte[LENGTH_szGuidePrefab];
                }
                type = srcBuf.readCString(ref this.szGuidePrefab_ByteArray, num15);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num16 = 0x80;
                if (this.szEffectPrefab_ByteArray.GetLength(0) < num16)
                {
                    this.szEffectPrefab_ByteArray = new byte[LENGTH_szEffectPrefab];
                }
                type = srcBuf.readCString(ref this.szEffectPrefab_ByteArray, num16);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num17 = 0x80;
                if (this.szGuideWarnPrefab_ByteArray.GetLength(0) < num17)
                {
                    this.szGuideWarnPrefab_ByteArray = new byte[LENGTH_szGuideWarnPrefab];
                }
                type = srcBuf.readCString(ref this.szGuideWarnPrefab_ByteArray, num17);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num18 = 0x80;
                if (this.szEffectWarnPrefab_ByteArray.GetLength(0) < num18)
                {
                    this.szEffectWarnPrefab_ByteArray = new byte[LENGTH_szEffectWarnPrefab];
                }
                type = srcBuf.readCString(ref this.szEffectWarnPrefab_ByteArray, num18);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num19 = 0x80;
                if (this.szFixedPrefab_ByteArray.GetLength(0) < num19)
                {
                    this.szFixedPrefab_ByteArray = new byte[LENGTH_szFixedPrefab];
                }
                type = srcBuf.readCString(ref this.szFixedPrefab_ByteArray, num19);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num20 = 0x80;
                if (this.szFixedWarnPrefab_ByteArray.GetLength(0) < num20)
                {
                    this.szFixedWarnPrefab_ByteArray = new byte[LENGTH_szFixedWarnPrefab];
                }
                type = srcBuf.readCString(ref this.szFixedWarnPrefab_ByteArray, num20);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref dest);
                this.iCoolDown = dest;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bImmediateUse);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsInterruptImmediateUseSkill);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bBIngnoreDisable);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num21 = 0x80;
                if (this.szPrefab_ByteArray.GetLength(0) < num21)
                {
                    this.szPrefab_ByteArray = new byte[LENGTH_szPrefab];
                }
                type = srcBuf.readCString(ref this.szPrefab_ByteArray, num21);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSelfSkillCombine);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iTargetSkillCombine);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwRangeAppointType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIndicatorType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num2);
                this.iRangeRadius = num2;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwTgtIncludeSelf);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwTgtIncludeEnemy);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num3);
                this.iBaseDamage = num3;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num4);
                this.iFixedDistance = num4;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num5);
                this.iGuideDistance = num5;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num6);
                this.iMaxAttackDistance = num6;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iGreaterAttackDistance);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iMaxSearchDistance);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwIsBullet);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBulletID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSkillUseRule);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSkillTargetRule);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSkillTargetFilter);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iSkillCombatType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsCheckAntiCheat);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 2; i++)
                {
                    type = srcBuf.readUInt32(ref this.SkillEffectType[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwAutoEnergyCost);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwEnergyCostType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iEnergyCostCalcType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num7);
                this.iEnergyCost = num7;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num8);
                this.iEnergyCostGrowth = num8;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref num9);
                this.iCoolDownGrowth = num9;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsStunSkill);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bNoInfluenceAnim);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bAgeImmeExcute);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                this.TransferData();
            }
            return type;
        }

        public TdrError.ErrorType load(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
        {
            if (((buffer == null) || (buffer.GetLength(0) == 0)) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrReadBuf srcBuf = new TdrReadBuf(ref buffer, size);
            TdrError.ErrorType type = this.load(ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            return type;
        }

        private void TransferData()
        {
            this.szSkillName = StringHelper.UTF8BytesToString(ref this.szSkillName_ByteArray);
            this.szSkillName_ByteArray = null;
            this.szSkillDesc = StringHelper.UTF8BytesToString(ref this.szSkillDesc_ByteArray);
            this.szSkillDesc_ByteArray = null;
            this.szSkillValueDesc = StringHelper.UTF8BytesToString(ref this.szSkillValueDesc_ByteArray);
            this.szSkillValueDesc_ByteArray = null;
            this.szSkillUpTip = StringHelper.UTF8BytesToString(ref this.szSkillUpTip_ByteArray);
            this.szSkillUpTip_ByteArray = null;
            this.szIconPath = StringHelper.UTF8BytesToString(ref this.szIconPath_ByteArray);
            this.szIconPath_ByteArray = null;
            this.szGuidePrefab = StringHelper.UTF8BytesToString(ref this.szGuidePrefab_ByteArray);
            this.szGuidePrefab_ByteArray = null;
            this.szEffectPrefab = StringHelper.UTF8BytesToString(ref this.szEffectPrefab_ByteArray);
            this.szEffectPrefab_ByteArray = null;
            this.szGuideWarnPrefab = StringHelper.UTF8BytesToString(ref this.szGuideWarnPrefab_ByteArray);
            this.szGuideWarnPrefab_ByteArray = null;
            this.szEffectWarnPrefab = StringHelper.UTF8BytesToString(ref this.szEffectWarnPrefab_ByteArray);
            this.szEffectWarnPrefab_ByteArray = null;
            this.szFixedPrefab = StringHelper.UTF8BytesToString(ref this.szFixedPrefab_ByteArray);
            this.szFixedPrefab_ByteArray = null;
            this.szFixedWarnPrefab = StringHelper.UTF8BytesToString(ref this.szFixedWarnPrefab_ByteArray);
            this.szFixedWarnPrefab_ByteArray = null;
            this.szPrefab = StringHelper.UTF8BytesToString(ref this.szPrefab_ByteArray);
            this.szPrefab_ByteArray = null;
        }

        public TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
        {
            int dest = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            int num7 = 0;
            int num8 = 0;
            int num9 = 0;
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            type = srcBuf.readInt32(ref this.iCfgID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                uint num10 = 0;
                type = srcBuf.readUInt32(ref num10);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num10 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num10 > this.szSkillName_ByteArray.GetLength(0))
                {
                    if (num10 > LENGTH_szSkillName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSkillName_ByteArray = new byte[num10];
                }
                if (1 > num10)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSkillName_ByteArray, (int) num10);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szSkillName_ByteArray[((int) num10) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num11 = TdrTypeUtil.cstrlen(this.szSkillName_ByteArray) + 1;
                if (num10 != num11)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num12 = 0;
                type = srcBuf.readUInt32(ref num12);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num12 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num12 > this.szSkillDesc_ByteArray.GetLength(0))
                {
                    if (num12 > LENGTH_szSkillDesc)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSkillDesc_ByteArray = new byte[num12];
                }
                if (1 > num12)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSkillDesc_ByteArray, (int) num12);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szSkillDesc_ByteArray[((int) num12) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num13 = TdrTypeUtil.cstrlen(this.szSkillDesc_ByteArray) + 1;
                if (num12 != num13)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt8(ref this.bSkillType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bWheelType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num14 = 0;
                type = srcBuf.readUInt32(ref num14);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num14 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num14 > this.szSkillValueDesc_ByteArray.GetLength(0))
                {
                    if (num14 > LENGTH_szSkillValueDesc)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSkillValueDesc_ByteArray = new byte[num14];
                }
                if (1 > num14)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSkillValueDesc_ByteArray, (int) num14);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szSkillValueDesc_ByteArray[((int) num14) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num15 = TdrTypeUtil.cstrlen(this.szSkillValueDesc_ByteArray) + 1;
                if (num14 != num15)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num16 = 0;
                type = srcBuf.readUInt32(ref num16);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num16 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num16 > this.szSkillUpTip_ByteArray.GetLength(0))
                {
                    if (num16 > LENGTH_szSkillUpTip)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSkillUpTip_ByteArray = new byte[num16];
                }
                if (1 > num16)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSkillUpTip_ByteArray, (int) num16);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szSkillUpTip_ByteArray[((int) num16) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num17 = TdrTypeUtil.cstrlen(this.szSkillUpTip_ByteArray) + 1;
                if (num16 != num17)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num18 = 0;
                type = srcBuf.readUInt32(ref num18);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num18 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num18 > this.szIconPath_ByteArray.GetLength(0))
                {
                    if (num18 > LENGTH_szIconPath)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szIconPath_ByteArray = new byte[num18];
                }
                if (1 > num18)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szIconPath_ByteArray, (int) num18);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szIconPath_ByteArray[((int) num18) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num19 = TdrTypeUtil.cstrlen(this.szIconPath_ByteArray) + 1;
                if (num18 != num19)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num20 = 0;
                type = srcBuf.readUInt32(ref num20);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num20 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num20 > this.szGuidePrefab_ByteArray.GetLength(0))
                {
                    if (num20 > LENGTH_szGuidePrefab)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szGuidePrefab_ByteArray = new byte[num20];
                }
                if (1 > num20)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szGuidePrefab_ByteArray, (int) num20);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szGuidePrefab_ByteArray[((int) num20) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num21 = TdrTypeUtil.cstrlen(this.szGuidePrefab_ByteArray) + 1;
                if (num20 != num21)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num22 = 0;
                type = srcBuf.readUInt32(ref num22);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num22 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num22 > this.szEffectPrefab_ByteArray.GetLength(0))
                {
                    if (num22 > LENGTH_szEffectPrefab)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szEffectPrefab_ByteArray = new byte[num22];
                }
                if (1 > num22)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szEffectPrefab_ByteArray, (int) num22);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szEffectPrefab_ByteArray[((int) num22) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num23 = TdrTypeUtil.cstrlen(this.szEffectPrefab_ByteArray) + 1;
                if (num22 != num23)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num24 = 0;
                type = srcBuf.readUInt32(ref num24);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num24 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num24 > this.szGuideWarnPrefab_ByteArray.GetLength(0))
                {
                    if (num24 > LENGTH_szGuideWarnPrefab)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szGuideWarnPrefab_ByteArray = new byte[num24];
                }
                if (1 > num24)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szGuideWarnPrefab_ByteArray, (int) num24);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szGuideWarnPrefab_ByteArray[((int) num24) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num25 = TdrTypeUtil.cstrlen(this.szGuideWarnPrefab_ByteArray) + 1;
                if (num24 != num25)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num26 = 0;
                type = srcBuf.readUInt32(ref num26);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num26 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num26 > this.szEffectWarnPrefab_ByteArray.GetLength(0))
                {
                    if (num26 > LENGTH_szEffectWarnPrefab)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szEffectWarnPrefab_ByteArray = new byte[num26];
                }
                if (1 > num26)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szEffectWarnPrefab_ByteArray, (int) num26);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szEffectWarnPrefab_ByteArray[((int) num26) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num27 = TdrTypeUtil.cstrlen(this.szEffectWarnPrefab_ByteArray) + 1;
                if (num26 != num27)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num28 = 0;
                type = srcBuf.readUInt32(ref num28);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num28 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num28 > this.szFixedPrefab_ByteArray.GetLength(0))
                {
                    if (num28 > LENGTH_szFixedPrefab)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szFixedPrefab_ByteArray = new byte[num28];
                }
                if (1 > num28)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szFixedPrefab_ByteArray, (int) num28);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szFixedPrefab_ByteArray[((int) num28) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num29 = TdrTypeUtil.cstrlen(this.szFixedPrefab_ByteArray) + 1;
                if (num28 != num29)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num30 = 0;
                type = srcBuf.readUInt32(ref num30);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num30 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num30 > this.szFixedWarnPrefab_ByteArray.GetLength(0))
                {
                    if (num30 > LENGTH_szFixedWarnPrefab)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szFixedWarnPrefab_ByteArray = new byte[num30];
                }
                if (1 > num30)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szFixedWarnPrefab_ByteArray, (int) num30);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szFixedWarnPrefab_ByteArray[((int) num30) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num31 = TdrTypeUtil.cstrlen(this.szFixedWarnPrefab_ByteArray) + 1;
                if (num30 != num31)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readInt32(ref dest);
                this.iCoolDown = dest;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bImmediateUse);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bIsInterruptImmediateUseSkill);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bBIngnoreDisable);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num32 = 0;
                type = srcBuf.readUInt32(ref num32);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num32 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num32 > this.szPrefab_ByteArray.GetLength(0))
                {
                    if (num32 > LENGTH_szPrefab)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szPrefab_ByteArray = new byte[num32];
                }
                if (1 > num32)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szPrefab_ByteArray, (int) num32);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szPrefab_ByteArray[((int) num32) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num33 = TdrTypeUtil.cstrlen(this.szPrefab_ByteArray) + 1;
                    if (num32 != num33)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    type = srcBuf.readInt32(ref this.iSelfSkillCombine);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref this.iTargetSkillCombine);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt32(ref this.dwRangeAppointType);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bIndicatorType);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref num2);
                    this.iRangeRadius = num2;
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt32(ref this.dwTgtIncludeSelf);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt32(ref this.dwTgtIncludeEnemy);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref num3);
                    this.iBaseDamage = num3;
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref num4);
                    this.iFixedDistance = num4;
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref num5);
                    this.iGuideDistance = num5;
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref num6);
                    this.iMaxAttackDistance = num6;
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref this.iGreaterAttackDistance);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref this.iMaxSearchDistance);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt32(ref this.dwIsBullet);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref this.iBulletID);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt32(ref this.dwSkillUseRule);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt32(ref this.dwSkillTargetRule);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt32(ref this.dwSkillTargetFilter);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref this.iSkillCombatType);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bIsCheckAntiCheat);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        type = srcBuf.readUInt32(ref this.SkillEffectType[i]);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                    }
                    type = srcBuf.readUInt32(ref this.dwAutoEnergyCost);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt32(ref this.dwEnergyCostType);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref this.iEnergyCostCalcType);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref num7);
                    this.iEnergyCost = num7;
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref num8);
                    this.iEnergyCostGrowth = num8;
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readInt32(ref num9);
                    this.iCoolDownGrowth = num9;
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bIsStunSkill);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bNoInfluenceAnim);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bAgeImmeExcute);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    this.TransferData();
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
            TdrReadBuf srcBuf = new TdrReadBuf(ref buffer, size);
            TdrError.ErrorType type = this.unpack(ref srcBuf, cutVer);
            usedSize = srcBuf.getUsedSize();
            return type;
        }
    }
}

