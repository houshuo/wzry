namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResRankGradeConf : IUnpackable, tsf4g_csharp_interface
    {
        public static readonly uint BASEVERSION = 1;
        public byte bBelongBigGrade;
        public byte bGrade;
        public static readonly uint CURRVERSION = 1;
        public uint dwConWinCnt;
        public uint dwGradeUpNeedScore;
        public uint dwGuildSignInPoint;
        public int iBaseMMR;
        public int iGradeUpNeedFightCnt;
        public int iGradeUpNeedWinCnt;
        public int iMultiMatchMMRAdjustValue;
        public int iTRankAdjustMMR;
        public static readonly uint LENGTH_szBigGradeName = 0x20;
        public static readonly uint LENGTH_szGradeDesc = 0x20;
        public static readonly uint LENGTH_szGradeFramePicPath = 0x80;
        public static readonly uint LENGTH_szGradeFramePicPathSuperMaster = 0x80;
        public static readonly uint LENGTH_szGradePicturePath = 0x80;
        public static readonly uint LENGTH_szGradePicturePathSuperMaster = 0x80;
        public static readonly uint LENGTH_szGradeSmallPicPath = 0x80;
        public static readonly uint LENGTH_szGradeSmallPicPathSuperMaster = 0x80;
        public string szBigGradeName = string.Empty;
        public byte[] szBigGradeName_ByteArray = new byte[1];
        public string szGradeDesc = string.Empty;
        public byte[] szGradeDesc_ByteArray = new byte[1];
        public string szGradeFramePicPath = string.Empty;
        public byte[] szGradeFramePicPath_ByteArray = new byte[1];
        public string szGradeFramePicPathSuperMaster = string.Empty;
        public byte[] szGradeFramePicPathSuperMaster_ByteArray = new byte[1];
        public string szGradePicturePath = string.Empty;
        public byte[] szGradePicturePath_ByteArray = new byte[1];
        public string szGradePicturePathSuperMaster = string.Empty;
        public byte[] szGradePicturePathSuperMaster_ByteArray = new byte[1];
        public string szGradeSmallPicPath = string.Empty;
        public byte[] szGradeSmallPicPath_ByteArray = new byte[1];
        public string szGradeSmallPicPathSuperMaster = string.Empty;
        public byte[] szGradeSmallPicPathSuperMaster_ByteArray = new byte[1];

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
            type = srcBuf.readUInt8(ref this.bGrade);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int count = 0x20;
                if (this.szGradeDesc_ByteArray.GetLength(0) < count)
                {
                    this.szGradeDesc_ByteArray = new byte[LENGTH_szGradeDesc];
                }
                type = srcBuf.readCString(ref this.szGradeDesc_ByteArray, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iGradeUpNeedFightCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iGradeUpNeedWinCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwGradeUpNeedScore);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num2 = 0x80;
                if (this.szGradePicturePath_ByteArray.GetLength(0) < num2)
                {
                    this.szGradePicturePath_ByteArray = new byte[LENGTH_szGradePicturePath];
                }
                type = srcBuf.readCString(ref this.szGradePicturePath_ByteArray, num2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num3 = 0x80;
                if (this.szGradePicturePathSuperMaster_ByteArray.GetLength(0) < num3)
                {
                    this.szGradePicturePathSuperMaster_ByteArray = new byte[LENGTH_szGradePicturePathSuperMaster];
                }
                type = srcBuf.readCString(ref this.szGradePicturePathSuperMaster_ByteArray, num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num4 = 0x80;
                if (this.szGradeSmallPicPath_ByteArray.GetLength(0) < num4)
                {
                    this.szGradeSmallPicPath_ByteArray = new byte[LENGTH_szGradeSmallPicPath];
                }
                type = srcBuf.readCString(ref this.szGradeSmallPicPath_ByteArray, num4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num5 = 0x80;
                if (this.szGradeSmallPicPathSuperMaster_ByteArray.GetLength(0) < num5)
                {
                    this.szGradeSmallPicPathSuperMaster_ByteArray = new byte[LENGTH_szGradeSmallPicPathSuperMaster];
                }
                type = srcBuf.readCString(ref this.szGradeSmallPicPathSuperMaster_ByteArray, num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num6 = 0x80;
                if (this.szGradeFramePicPath_ByteArray.GetLength(0) < num6)
                {
                    this.szGradeFramePicPath_ByteArray = new byte[LENGTH_szGradeFramePicPath];
                }
                type = srcBuf.readCString(ref this.szGradeFramePicPath_ByteArray, num6);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num7 = 0x80;
                if (this.szGradeFramePicPathSuperMaster_ByteArray.GetLength(0) < num7)
                {
                    this.szGradeFramePicPathSuperMaster_ByteArray = new byte[LENGTH_szGradeFramePicPathSuperMaster];
                }
                type = srcBuf.readCString(ref this.szGradeFramePicPathSuperMaster_ByteArray, num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iTRankAdjustMMR);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwGuildSignInPoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iMultiMatchMMRAdjustValue);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBaseMMR);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwConWinCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bBelongBigGrade);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num8 = 0x20;
                if (this.szBigGradeName_ByteArray.GetLength(0) < num8)
                {
                    this.szBigGradeName_ByteArray = new byte[LENGTH_szBigGradeName];
                }
                type = srcBuf.readCString(ref this.szBigGradeName_ByteArray, num8);
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
            this.szGradeDesc = StringHelper.UTF8BytesToString(ref this.szGradeDesc_ByteArray);
            this.szGradeDesc_ByteArray = null;
            this.szGradePicturePath = StringHelper.UTF8BytesToString(ref this.szGradePicturePath_ByteArray);
            this.szGradePicturePath_ByteArray = null;
            this.szGradePicturePathSuperMaster = StringHelper.UTF8BytesToString(ref this.szGradePicturePathSuperMaster_ByteArray);
            this.szGradePicturePathSuperMaster_ByteArray = null;
            this.szGradeSmallPicPath = StringHelper.UTF8BytesToString(ref this.szGradeSmallPicPath_ByteArray);
            this.szGradeSmallPicPath_ByteArray = null;
            this.szGradeSmallPicPathSuperMaster = StringHelper.UTF8BytesToString(ref this.szGradeSmallPicPathSuperMaster_ByteArray);
            this.szGradeSmallPicPathSuperMaster_ByteArray = null;
            this.szGradeFramePicPath = StringHelper.UTF8BytesToString(ref this.szGradeFramePicPath_ByteArray);
            this.szGradeFramePicPath_ByteArray = null;
            this.szGradeFramePicPathSuperMaster = StringHelper.UTF8BytesToString(ref this.szGradeFramePicPathSuperMaster_ByteArray);
            this.szGradeFramePicPathSuperMaster_ByteArray = null;
            this.szBigGradeName = StringHelper.UTF8BytesToString(ref this.szBigGradeName_ByteArray);
            this.szBigGradeName_ByteArray = null;
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
            type = srcBuf.readUInt8(ref this.bGrade);
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
                if (dest > this.szGradeDesc_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szGradeDesc)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szGradeDesc_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szGradeDesc_ByteArray, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szGradeDesc_ByteArray[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szGradeDesc_ByteArray) + 1;
                if (dest != num2)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readInt32(ref this.iGradeUpNeedFightCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iGradeUpNeedWinCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwGradeUpNeedScore);
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
                if (num3 > this.szGradePicturePath_ByteArray.GetLength(0))
                {
                    if (num3 > LENGTH_szGradePicturePath)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szGradePicturePath_ByteArray = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szGradePicturePath_ByteArray, (int) num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szGradePicturePath_ByteArray[((int) num3) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num4 = TdrTypeUtil.cstrlen(this.szGradePicturePath_ByteArray) + 1;
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
                if (num5 > this.szGradePicturePathSuperMaster_ByteArray.GetLength(0))
                {
                    if (num5 > LENGTH_szGradePicturePathSuperMaster)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szGradePicturePathSuperMaster_ByteArray = new byte[num5];
                }
                if (1 > num5)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szGradePicturePathSuperMaster_ByteArray, (int) num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szGradePicturePathSuperMaster_ByteArray[((int) num5) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num6 = TdrTypeUtil.cstrlen(this.szGradePicturePathSuperMaster_ByteArray) + 1;
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
                if (num7 > this.szGradeSmallPicPath_ByteArray.GetLength(0))
                {
                    if (num7 > LENGTH_szGradeSmallPicPath)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szGradeSmallPicPath_ByteArray = new byte[num7];
                }
                if (1 > num7)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szGradeSmallPicPath_ByteArray, (int) num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szGradeSmallPicPath_ByteArray[((int) num7) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num8 = TdrTypeUtil.cstrlen(this.szGradeSmallPicPath_ByteArray) + 1;
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
                if (num9 > this.szGradeSmallPicPathSuperMaster_ByteArray.GetLength(0))
                {
                    if (num9 > LENGTH_szGradeSmallPicPathSuperMaster)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szGradeSmallPicPathSuperMaster_ByteArray = new byte[num9];
                }
                if (1 > num9)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szGradeSmallPicPathSuperMaster_ByteArray, (int) num9);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szGradeSmallPicPathSuperMaster_ByteArray[((int) num9) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num10 = TdrTypeUtil.cstrlen(this.szGradeSmallPicPathSuperMaster_ByteArray) + 1;
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
                if (num11 > this.szGradeFramePicPath_ByteArray.GetLength(0))
                {
                    if (num11 > LENGTH_szGradeFramePicPath)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szGradeFramePicPath_ByteArray = new byte[num11];
                }
                if (1 > num11)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szGradeFramePicPath_ByteArray, (int) num11);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szGradeFramePicPath_ByteArray[((int) num11) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num12 = TdrTypeUtil.cstrlen(this.szGradeFramePicPath_ByteArray) + 1;
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
                if (num13 > this.szGradeFramePicPathSuperMaster_ByteArray.GetLength(0))
                {
                    if (num13 > LENGTH_szGradeFramePicPathSuperMaster)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szGradeFramePicPathSuperMaster_ByteArray = new byte[num13];
                }
                if (1 > num13)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szGradeFramePicPathSuperMaster_ByteArray, (int) num13);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szGradeFramePicPathSuperMaster_ByteArray[((int) num13) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num14 = TdrTypeUtil.cstrlen(this.szGradeFramePicPathSuperMaster_ByteArray) + 1;
                if (num13 != num14)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readInt32(ref this.iTRankAdjustMMR);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwGuildSignInPoint);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iMultiMatchMMRAdjustValue);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readInt32(ref this.iBaseMMR);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwConWinCnt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bBelongBigGrade);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num15 = 0;
                type = srcBuf.readUInt32(ref num15);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (num15 > srcBuf.getLeftSize())
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                if (num15 > this.szBigGradeName_ByteArray.GetLength(0))
                {
                    if (num15 > LENGTH_szBigGradeName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szBigGradeName_ByteArray = new byte[num15];
                }
                if (1 > num15)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szBigGradeName_ByteArray, (int) num15);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szBigGradeName_ByteArray[((int) num15) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num16 = TdrTypeUtil.cstrlen(this.szBigGradeName_ByteArray) + 1;
                    if (num15 != num16)
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

