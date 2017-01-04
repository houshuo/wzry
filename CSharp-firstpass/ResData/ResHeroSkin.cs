namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResHeroSkin : IUnpackable, tsf4g_csharp_interface
    {
        public ResDT_FuncEft_Obj[] astAttr = new ResDT_FuncEft_Obj[15];
        public ResDT_SkinFeature[] astFeature;
        public static readonly uint BASEVERSION = 1;
        public static readonly uint CURRVERSION = 1;
        public uint dwCombatAbility;
        public uint dwHeroID;
        public uint dwID;
        public uint dwPresentHeadImg;
        public uint dwSkinID;
        public static readonly uint LENGTH_szHeroName = 0x20;
        public static readonly uint LENGTH_szShareSkinUrl = 0x40;
        public static readonly uint LENGTH_szSkinName = 0x20;
        public static readonly uint LENGTH_szSkinPicID = 0x80;
        public static readonly uint LENGTH_szSkinSoundResPack = 0x80;
        public static readonly uint LENGTH_szSoundSwitchEvent = 0x80;
        public string szHeroName;
        public byte[] szHeroName_ByteArray = new byte[1];
        public string szShareSkinUrl;
        public byte[] szShareSkinUrl_ByteArray;
        public string szSkinName;
        public byte[] szSkinName_ByteArray = new byte[1];
        public string szSkinPicID;
        public byte[] szSkinPicID_ByteArray = new byte[1];
        public string szSkinSoundResPack;
        public byte[] szSkinSoundResPack_ByteArray = new byte[1];
        public string szSoundSwitchEvent;
        public byte[] szSoundSwitchEvent_ByteArray = new byte[1];

        public ResHeroSkin()
        {
            for (int i = 0; i < 15; i++)
            {
                this.astAttr[i] = new ResDT_FuncEft_Obj();
            }
            this.szShareSkinUrl_ByteArray = new byte[1];
            this.astFeature = new ResDT_SkinFeature[10];
            for (int j = 0; j < 10; j++)
            {
                this.astFeature[j] = new ResDT_SkinFeature();
            }
            this.szHeroName = string.Empty;
            this.szSkinName = string.Empty;
            this.szSkinPicID = string.Empty;
            this.szSkinSoundResPack = string.Empty;
            this.szSoundSwitchEvent = string.Empty;
            this.szShareSkinUrl = string.Empty;
        }

        public TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType load(ref TdrReadBuf srcBuf, uint cutVer)
        {
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
            type = srcBuf.readUInt32(ref this.dwID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwHeroID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int count = 0x20;
                if (this.szHeroName_ByteArray.GetLength(0) < count)
                {
                    this.szHeroName_ByteArray = new byte[LENGTH_szHeroName];
                }
                type = srcBuf.readCString(ref this.szHeroName_ByteArray, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwSkinID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num2 = 0x20;
                if (this.szSkinName_ByteArray.GetLength(0) < num2)
                {
                    this.szSkinName_ByteArray = new byte[LENGTH_szSkinName];
                }
                type = srcBuf.readCString(ref this.szSkinName_ByteArray, num2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num3 = 0x80;
                if (this.szSkinPicID_ByteArray.GetLength(0) < num3)
                {
                    this.szSkinPicID_ByteArray = new byte[LENGTH_szSkinPicID];
                }
                type = srcBuf.readCString(ref this.szSkinPicID_ByteArray, num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num4 = 0x80;
                if (this.szSkinSoundResPack_ByteArray.GetLength(0) < num4)
                {
                    this.szSkinSoundResPack_ByteArray = new byte[LENGTH_szSkinSoundResPack];
                }
                type = srcBuf.readCString(ref this.szSkinSoundResPack_ByteArray, num4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num5 = 0x80;
                if (this.szSoundSwitchEvent_ByteArray.GetLength(0) < num5)
                {
                    this.szSoundSwitchEvent_ByteArray = new byte[LENGTH_szSoundSwitchEvent];
                }
                type = srcBuf.readCString(ref this.szSoundSwitchEvent_ByteArray, num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwCombatAbility);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 15; i++)
                {
                    type = this.astAttr[i].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwPresentHeadImg);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num7 = 0x40;
                if (this.szShareSkinUrl_ByteArray.GetLength(0) < num7)
                {
                    this.szShareSkinUrl_ByteArray = new byte[LENGTH_szShareSkinUrl];
                }
                type = srcBuf.readCString(ref this.szShareSkinUrl_ByteArray, num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int j = 0; j < 10; j++)
                {
                    type = this.astFeature[j].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
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
            this.szHeroName = StringHelper.UTF8BytesToString(ref this.szHeroName_ByteArray);
            this.szHeroName_ByteArray = null;
            this.szSkinName = StringHelper.UTF8BytesToString(ref this.szSkinName_ByteArray);
            this.szSkinName_ByteArray = null;
            this.szSkinPicID = StringHelper.UTF8BytesToString(ref this.szSkinPicID_ByteArray);
            this.szSkinPicID_ByteArray = null;
            this.szSkinSoundResPack = StringHelper.UTF8BytesToString(ref this.szSkinSoundResPack_ByteArray);
            this.szSkinSoundResPack_ByteArray = null;
            this.szSoundSwitchEvent = StringHelper.UTF8BytesToString(ref this.szSoundSwitchEvent_ByteArray);
            this.szSoundSwitchEvent_ByteArray = null;
            this.szShareSkinUrl = StringHelper.UTF8BytesToString(ref this.szShareSkinUrl_ByteArray);
            this.szShareSkinUrl_ByteArray = null;
        }

        public TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
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
            type = srcBuf.readUInt32(ref this.dwID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = srcBuf.readUInt32(ref this.dwHeroID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint dest = 0;
                type = srcBuf.readUInt32(ref dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (dest > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (dest > this.szHeroName_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szHeroName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szHeroName_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szHeroName_ByteArray, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szHeroName_ByteArray[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szHeroName_ByteArray) + 1;
                if (dest != num2)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt32(ref this.dwSkinID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
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
                if (num3 > this.szSkinName_ByteArray.GetLength(0))
                {
                    if (num3 > LENGTH_szSkinName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSkinName_ByteArray = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSkinName_ByteArray, (int) num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szSkinName_ByteArray[((int) num3) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num4 = TdrTypeUtil.cstrlen(this.szSkinName_ByteArray) + 1;
                if (num3 != num4)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
                if (num5 > this.szSkinPicID_ByteArray.GetLength(0))
                {
                    if (num5 > LENGTH_szSkinPicID)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSkinPicID_ByteArray = new byte[num5];
                }
                if (1 > num5)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSkinPicID_ByteArray, (int) num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szSkinPicID_ByteArray[((int) num5) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num6 = TdrTypeUtil.cstrlen(this.szSkinPicID_ByteArray) + 1;
                if (num5 != num6)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num7 = 0;
                type = srcBuf.readUInt32(ref num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num7 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num7 > this.szSkinSoundResPack_ByteArray.GetLength(0))
                {
                    if (num7 > LENGTH_szSkinSoundResPack)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSkinSoundResPack_ByteArray = new byte[num7];
                }
                if (1 > num7)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSkinSoundResPack_ByteArray, (int) num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szSkinSoundResPack_ByteArray[((int) num7) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num8 = TdrTypeUtil.cstrlen(this.szSkinSoundResPack_ByteArray) + 1;
                if (num7 != num8)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num9 = 0;
                type = srcBuf.readUInt32(ref num9);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num9 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num9 > this.szSoundSwitchEvent_ByteArray.GetLength(0))
                {
                    if (num9 > LENGTH_szSoundSwitchEvent)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szSoundSwitchEvent_ByteArray = new byte[num9];
                }
                if (1 > num9)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szSoundSwitchEvent_ByteArray, (int) num9);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szSoundSwitchEvent_ByteArray[((int) num9) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num10 = TdrTypeUtil.cstrlen(this.szSoundSwitchEvent_ByteArray) + 1;
                if (num9 != num10)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt32(ref this.dwCombatAbility);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 15; i++)
                {
                    type = this.astAttr[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwPresentHeadImg);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
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
                if (num12 > this.szShareSkinUrl_ByteArray.GetLength(0))
                {
                    if (num12 > LENGTH_szShareSkinUrl)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szShareSkinUrl_ByteArray = new byte[num12];
                }
                if (1 > num12)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szShareSkinUrl_ByteArray, (int) num12);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szShareSkinUrl_ByteArray[((int) num12) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num13 = TdrTypeUtil.cstrlen(this.szShareSkinUrl_ByteArray) + 1;
                    if (num12 != num13)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    for (int j = 0; j < 10; j++)
                    {
                        type = this.astFeature[j].unpack(ref srcBuf, cutVer);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
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

