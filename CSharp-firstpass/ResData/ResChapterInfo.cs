namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResChapterInfo : IUnpackable, tsf4g_csharp_interface
    {
        public ResDT_ChapterRewardInfo[] astAbyssRewardDetail;
        public ResDT_ChapterRewardInfo[] astEliteRewardDetail;
        public ResDT_ChapterRewardInfo[] astMasterRewardDetail;
        public ResDT_ChapterRewardInfo[] astNormalRewardDetail = new ResDT_ChapterRewardInfo[5];
        public static readonly uint BASEVERSION = 1;
        public static readonly uint CURRVERSION = 1;
        public uint dwChapterId;
        public uint dwUnlockLevel;
        public static readonly uint LENGTH_szChapterDesc = 150;
        public static readonly uint LENGTH_szChapterIcon = 0x80;
        public static readonly uint LENGTH_szChapterName = 0x40;
        public static readonly uint LENGTH_szLockedTip = 0x80;
        public static readonly uint LENGTH_szUnlockTip = 0x80;
        public string szChapterDesc;
        public byte[] szChapterDesc_ByteArray;
        public string szChapterIcon;
        public byte[] szChapterIcon_ByteArray;
        public string szChapterName;
        public byte[] szChapterName_ByteArray = new byte[1];
        public string szLockedTip;
        public byte[] szLockedTip_ByteArray;
        public string szUnlockTip;
        public byte[] szUnlockTip_ByteArray;

        public ResChapterInfo()
        {
            for (int i = 0; i < 5; i++)
            {
                this.astNormalRewardDetail[i] = new ResDT_ChapterRewardInfo();
            }
            this.astEliteRewardDetail = new ResDT_ChapterRewardInfo[5];
            for (int j = 0; j < 5; j++)
            {
                this.astEliteRewardDetail[j] = new ResDT_ChapterRewardInfo();
            }
            this.astMasterRewardDetail = new ResDT_ChapterRewardInfo[5];
            for (int k = 0; k < 5; k++)
            {
                this.astMasterRewardDetail[k] = new ResDT_ChapterRewardInfo();
            }
            this.astAbyssRewardDetail = new ResDT_ChapterRewardInfo[5];
            for (int m = 0; m < 5; m++)
            {
                this.astAbyssRewardDetail[m] = new ResDT_ChapterRewardInfo();
            }
            this.szChapterIcon_ByteArray = new byte[1];
            this.szLockedTip_ByteArray = new byte[1];
            this.szUnlockTip_ByteArray = new byte[1];
            this.szChapterDesc_ByteArray = new byte[1];
            this.szChapterName = string.Empty;
            this.szChapterIcon = string.Empty;
            this.szLockedTip = string.Empty;
            this.szUnlockTip = string.Empty;
            this.szChapterDesc = string.Empty;
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
            type = srcBuf.readUInt32(ref this.dwChapterId);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int count = 0x40;
                if (this.szChapterName_ByteArray.GetLength(0) < count)
                {
                    this.szChapterName_ByteArray = new byte[LENGTH_szChapterName];
                }
                type = srcBuf.readCString(ref this.szChapterName_ByteArray, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwUnlockLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 5; i++)
                {
                    type = this.astNormalRewardDetail[i].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int j = 0; j < 5; j++)
                {
                    type = this.astEliteRewardDetail[j].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int k = 0; k < 5; k++)
                {
                    type = this.astMasterRewardDetail[k].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int m = 0; m < 5; m++)
                {
                    type = this.astAbyssRewardDetail[m].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                int num6 = 0x80;
                if (this.szChapterIcon_ByteArray.GetLength(0) < num6)
                {
                    this.szChapterIcon_ByteArray = new byte[LENGTH_szChapterIcon];
                }
                type = srcBuf.readCString(ref this.szChapterIcon_ByteArray, num6);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num7 = 0x80;
                if (this.szLockedTip_ByteArray.GetLength(0) < num7)
                {
                    this.szLockedTip_ByteArray = new byte[LENGTH_szLockedTip];
                }
                type = srcBuf.readCString(ref this.szLockedTip_ByteArray, num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num8 = 0x80;
                if (this.szUnlockTip_ByteArray.GetLength(0) < num8)
                {
                    this.szUnlockTip_ByteArray = new byte[LENGTH_szUnlockTip];
                }
                type = srcBuf.readCString(ref this.szUnlockTip_ByteArray, num8);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num9 = 150;
                if (this.szChapterDesc_ByteArray.GetLength(0) < num9)
                {
                    this.szChapterDesc_ByteArray = new byte[LENGTH_szChapterDesc];
                }
                type = srcBuf.readCString(ref this.szChapterDesc_ByteArray, num9);
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
            this.szChapterName = StringHelper.UTF8BytesToString(ref this.szChapterName_ByteArray);
            this.szChapterName_ByteArray = null;
            this.szChapterIcon = StringHelper.UTF8BytesToString(ref this.szChapterIcon_ByteArray);
            this.szChapterIcon_ByteArray = null;
            this.szLockedTip = StringHelper.UTF8BytesToString(ref this.szLockedTip_ByteArray);
            this.szLockedTip_ByteArray = null;
            this.szUnlockTip = StringHelper.UTF8BytesToString(ref this.szUnlockTip_ByteArray);
            this.szUnlockTip_ByteArray = null;
            this.szChapterDesc = StringHelper.UTF8BytesToString(ref this.szChapterDesc_ByteArray);
            this.szChapterDesc_ByteArray = null;
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
            type = srcBuf.readUInt32(ref this.dwChapterId);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
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
                if (dest > this.szChapterName_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szChapterName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szChapterName_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szChapterName_ByteArray, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szChapterName_ByteArray[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szChapterName_ByteArray) + 1;
                if (dest != num2)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt32(ref this.dwUnlockLevel);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 5; i++)
                {
                    type = this.astNormalRewardDetail[i].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int j = 0; j < 5; j++)
                {
                    type = this.astEliteRewardDetail[j].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int k = 0; k < 5; k++)
                {
                    type = this.astMasterRewardDetail[k].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int m = 0; m < 5; m++)
                {
                    type = this.astAbyssRewardDetail[m].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
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
                if (num7 > this.szChapterIcon_ByteArray.GetLength(0))
                {
                    if (num7 > LENGTH_szChapterIcon)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szChapterIcon_ByteArray = new byte[num7];
                }
                if (1 > num7)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szChapterIcon_ByteArray, (int) num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szChapterIcon_ByteArray[((int) num7) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num8 = TdrTypeUtil.cstrlen(this.szChapterIcon_ByteArray) + 1;
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
                if (num9 > this.szLockedTip_ByteArray.GetLength(0))
                {
                    if (num9 > LENGTH_szLockedTip)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szLockedTip_ByteArray = new byte[num9];
                }
                if (1 > num9)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szLockedTip_ByteArray, (int) num9);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szLockedTip_ByteArray[((int) num9) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num10 = TdrTypeUtil.cstrlen(this.szLockedTip_ByteArray) + 1;
                if (num9 != num10)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num11 = 0;
                type = srcBuf.readUInt32(ref num11);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num11 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num11 > this.szUnlockTip_ByteArray.GetLength(0))
                {
                    if (num11 > LENGTH_szUnlockTip)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szUnlockTip_ByteArray = new byte[num11];
                }
                if (1 > num11)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szUnlockTip_ByteArray, (int) num11);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szUnlockTip_ByteArray[((int) num11) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num12 = TdrTypeUtil.cstrlen(this.szUnlockTip_ByteArray) + 1;
                if (num11 != num12)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                uint num13 = 0;
                type = srcBuf.readUInt32(ref num13);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num13 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num13 > this.szChapterDesc_ByteArray.GetLength(0))
                {
                    if (num13 > LENGTH_szChapterDesc)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szChapterDesc_ByteArray = new byte[num13];
                }
                if (1 > num13)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szChapterDesc_ByteArray, (int) num13);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szChapterDesc_ByteArray[((int) num13) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num14 = TdrTypeUtil.cstrlen(this.szChapterDesc_ByteArray) + 1;
                    if (num13 != num14)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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

