namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResEquipInBattle : IUnpackable, tsf4g_csharp_interface
    {
        public ResEffectCombine[] astEffectCombine;
        public ResPassiveSkill[] astPassiveSkill = new ResPassiveSkill[2];
        public static readonly uint BASEVERSION = 1;
        public byte bInvalid;
        public byte bLevel;
        public byte bNeedPunish;
        public byte bType;
        public byte bUsage;
        public static readonly uint CURRVERSION = 1;
        public uint dwActiveSkillID;
        public uint dwAttackSpeed;
        public CrypticInteger dwBuyPrice;
        public uint dwCDReduce;
        public uint dwCriticalHit;
        public uint dwHealthPoint;
        public uint dwHealthRecover;
        public uint dwHealthSteal;
        public uint dwMagicAttack;
        public uint dwMagicDefence;
        public uint dwMagicPoint;
        public uint dwMagicRecover;
        public uint dwMoveSpeed;
        public uint dwPhyAttack;
        public uint dwPhyDefence;
        public CrypticInteger dwSalePrice;
        public static readonly uint LENGTH_szActiveSkillDes = 0x80;
        public static readonly uint LENGTH_szDesc = 0x80;
        public static readonly uint LENGTH_szIcon = 0x80;
        public static readonly uint LENGTH_szName = 0x40;
        public static readonly uint LENGTH_szRequireEquip = 0x40;
        public ushort[] PreEquipID = new ushort[3];
        public string szActiveSkillDes;
        public byte[] szActiveSkillDes_ByteArray = new byte[1];
        public string szDesc;
        public byte[] szDesc_ByteArray;
        public string szIcon;
        public byte[] szIcon_ByteArray = new byte[1];
        public string szName;
        public byte[] szName_ByteArray = new byte[1];
        public string szRequireEquip;
        public byte[] szRequireEquip_ByteArray = new byte[1];
        public ushort wGroup;
        public ushort wID;

        public ResEquipInBattle()
        {
            for (int i = 0; i < 2; i++)
            {
                this.astPassiveSkill[i] = new ResPassiveSkill();
            }
            this.astEffectCombine = new ResEffectCombine[3];
            for (int j = 0; j < 3; j++)
            {
                this.astEffectCombine[j] = new ResEffectCombine();
            }
            this.szDesc_ByteArray = new byte[1];
            this.szName = string.Empty;
            this.szIcon = string.Empty;
            this.szRequireEquip = string.Empty;
            this.szActiveSkillDes = string.Empty;
            this.szDesc = string.Empty;
        }

        public TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType load(ref TdrReadBuf srcBuf, uint cutVer)
        {
            uint dest = 0;
            uint num2 = 0;
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
            type = srcBuf.readUInt16(ref this.wID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int count = 0x40;
                if (this.szName_ByteArray.GetLength(0) < count)
                {
                    this.szName_ByteArray = new byte[LENGTH_szName];
                }
                type = srcBuf.readCString(ref this.szName_ByteArray, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bUsage);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wGroup);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bNeedPunish);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num4 = 0x80;
                if (this.szIcon_ByteArray.GetLength(0) < num4)
                {
                    this.szIcon_ByteArray = new byte[LENGTH_szIcon];
                }
                type = srcBuf.readCString(ref this.szIcon_ByteArray, num4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref dest);
                this.dwBuyPrice = dest;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref num2);
                this.dwSalePrice = num2;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 3; i++)
                {
                    type = srcBuf.readUInt16(ref this.PreEquipID[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                int num6 = 0x40;
                if (this.szRequireEquip_ByteArray.GetLength(0) < num6)
                {
                    this.szRequireEquip_ByteArray = new byte[LENGTH_szRequireEquip];
                }
                type = srcBuf.readCString(ref this.szRequireEquip_ByteArray, num6);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwPhyAttack);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwAttackSpeed);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCriticalHit);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwHealthSteal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMagicAttack);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCDReduce);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMagicPoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMagicRecover);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwPhyDefence);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMagicDefence);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwHealthPoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwHealthRecover);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMoveSpeed);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwActiveSkillID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num7 = 0x80;
                if (this.szActiveSkillDes_ByteArray.GetLength(0) < num7)
                {
                    this.szActiveSkillDes_ByteArray = new byte[LENGTH_szActiveSkillDes];
                }
                type = srcBuf.readCString(ref this.szActiveSkillDes_ByteArray, num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int j = 0; j < 2; j++)
                {
                    type = this.astPassiveSkill[j].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int k = 0; k < 3; k++)
                {
                    type = this.astEffectCombine[k].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                int num10 = 0x80;
                if (this.szDesc_ByteArray.GetLength(0) < num10)
                {
                    this.szDesc_ByteArray = new byte[LENGTH_szDesc];
                }
                type = srcBuf.readCString(ref this.szDesc_ByteArray, num10);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bInvalid);
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
            this.szName = StringHelper.UTF8BytesToString(ref this.szName_ByteArray);
            this.szName_ByteArray = null;
            this.szIcon = StringHelper.UTF8BytesToString(ref this.szIcon_ByteArray);
            this.szIcon_ByteArray = null;
            this.szRequireEquip = StringHelper.UTF8BytesToString(ref this.szRequireEquip_ByteArray);
            this.szRequireEquip_ByteArray = null;
            this.szActiveSkillDes = StringHelper.UTF8BytesToString(ref this.szActiveSkillDes_ByteArray);
            this.szActiveSkillDes_ByteArray = null;
            this.szDesc = StringHelper.UTF8BytesToString(ref this.szDesc_ByteArray);
            this.szDesc_ByteArray = null;
        }

        public TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
        {
            uint dest = 0;
            uint num2 = 0;
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if ((cutVer == 0) || (CURRVERSION < cutVer))
            {
                cutVer = CURRVERSION;
            }
            if (BASEVERSION > cutVer)
            {
                return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
            }
            type = srcBuf.readUInt16(ref this.wID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                uint num3 = 0;
                type = srcBuf.readUInt32(ref num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num3 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num3 > this.szName_ByteArray.GetLength(0))
                {
                    if (num3 > LENGTH_szName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szName_ByteArray = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szName_ByteArray, (int) num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szName_ByteArray[((int) num3) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num4 = TdrTypeUtil.cstrlen(this.szName_ByteArray) + 1;
                if (num3 != num4)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt8(ref this.bUsage);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt16(ref this.wGroup);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bNeedPunish);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num5 = 0;
                type = srcBuf.readUInt32(ref num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num5 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num5 > this.szIcon_ByteArray.GetLength(0))
                {
                    if (num5 > LENGTH_szIcon)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szIcon_ByteArray = new byte[num5];
                }
                if (1 > num5)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szIcon_ByteArray, (int) num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szIcon_ByteArray[((int) num5) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num6 = TdrTypeUtil.cstrlen(this.szIcon_ByteArray) + 1;
                if (num5 != num6)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt32(ref dest);
                this.dwBuyPrice = dest;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref num2);
                this.dwSalePrice = num2;
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 3; i++)
                {
                    type = srcBuf.readUInt16(ref this.PreEquipID[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                uint num8 = 0;
                type = srcBuf.readUInt32(ref num8);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num8 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num8 > this.szRequireEquip_ByteArray.GetLength(0))
                {
                    if (num8 > LENGTH_szRequireEquip)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szRequireEquip_ByteArray = new byte[num8];
                }
                if (1 > num8)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szRequireEquip_ByteArray, (int) num8);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szRequireEquip_ByteArray[((int) num8) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num9 = TdrTypeUtil.cstrlen(this.szRequireEquip_ByteArray) + 1;
                if (num8 != num9)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt32(ref this.dwPhyAttack);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwAttackSpeed);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCriticalHit);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwHealthSteal);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMagicAttack);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCDReduce);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMagicPoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMagicRecover);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwPhyDefence);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMagicDefence);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwHealthPoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwHealthRecover);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMoveSpeed);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwActiveSkillID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
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
                if (num10 > this.szActiveSkillDes_ByteArray.GetLength(0))
                {
                    if (num10 > LENGTH_szActiveSkillDes)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szActiveSkillDes_ByteArray = new byte[num10];
                }
                if (1 > num10)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szActiveSkillDes_ByteArray, (int) num10);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szActiveSkillDes_ByteArray[((int) num10) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num11 = TdrTypeUtil.cstrlen(this.szActiveSkillDes_ByteArray) + 1;
                if (num10 != num11)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                for (int j = 0; j < 2; j++)
                {
                    type = this.astPassiveSkill[j].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int k = 0; k < 3; k++)
                {
                    type = this.astEffectCombine[k].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
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
                if (num14 > this.szDesc_ByteArray.GetLength(0))
                {
                    if (num14 > LENGTH_szDesc)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szDesc_ByteArray = new byte[num14];
                }
                if (1 > num14)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szDesc_ByteArray, (int) num14);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szDesc_ByteArray[((int) num14) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num15 = TdrTypeUtil.cstrlen(this.szDesc_ByteArray) + 1;
                    if (num14 != num15)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    type = srcBuf.readUInt8(ref this.bInvalid);
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

