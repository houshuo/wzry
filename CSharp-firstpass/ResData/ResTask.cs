namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResTask : IUnpackable, tsf4g_csharp_interface
    {
        public ResDT_IntParamArrayNode[] astOpenTaskParam = new ResDT_IntParamArrayNode[3];
        public ResDT_PrerequisiteInTask[] astPrerequisiteArray;
        public static readonly uint BASEVERSION = 1;
        public byte bNextTaskNum;
        public byte bPrerequisiteNum;
        public byte bTaskAddOnBorn;
        public byte bTaskIconShowType;
        public byte bTaskSubType;
        public static readonly uint CURRVERSION = 1;
        public uint dwMiShuIndex;
        public uint dwOpenType;
        public uint dwPreTaskID;
        public uint dwRewardID;
        public uint dwTaskID;
        public uint dwTaskType;
        public static readonly uint LENGTH_szMiShuDesc = 0x80;
        public static readonly uint LENGTH_szTaskBgIcon = 0x20;
        public static readonly uint LENGTH_szTaskDesc = 0x80;
        public static readonly uint LENGTH_szTaskIcon = 0x20;
        public static readonly uint LENGTH_szTaskName = 0x80;
        public uint[] NextTaskID = new uint[2];
        public string szMiShuDesc;
        public byte[] szMiShuDesc_ByteArray = new byte[1];
        public string szTaskBgIcon;
        public byte[] szTaskBgIcon_ByteArray;
        public string szTaskDesc;
        public byte[] szTaskDesc_ByteArray = new byte[1];
        public string szTaskIcon;
        public byte[] szTaskIcon_ByteArray;
        public string szTaskName;
        public byte[] szTaskName_ByteArray = new byte[1];

        public ResTask()
        {
            for (int i = 0; i < 3; i++)
            {
                this.astOpenTaskParam[i] = new ResDT_IntParamArrayNode();
            }
            this.astPrerequisiteArray = new ResDT_PrerequisiteInTask[2];
            for (int j = 0; j < 2; j++)
            {
                this.astPrerequisiteArray[j] = new ResDT_PrerequisiteInTask();
            }
            this.szTaskBgIcon_ByteArray = new byte[1];
            this.szTaskIcon_ByteArray = new byte[1];
            this.szTaskName = string.Empty;
            this.szTaskDesc = string.Empty;
            this.szMiShuDesc = string.Empty;
            this.szTaskBgIcon = string.Empty;
            this.szTaskIcon = string.Empty;
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
            type = srcBuf.readUInt32(ref this.dwTaskID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int count = 0x80;
                if (this.szTaskName_ByteArray.GetLength(0) < count)
                {
                    this.szTaskName_ByteArray = new byte[LENGTH_szTaskName];
                }
                type = srcBuf.readCString(ref this.szTaskName_ByteArray, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num2 = 0x80;
                if (this.szTaskDesc_ByteArray.GetLength(0) < num2)
                {
                    this.szTaskDesc_ByteArray = new byte[LENGTH_szTaskDesc];
                }
                type = srcBuf.readCString(ref this.szTaskDesc_ByteArray, num2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num3 = 0x80;
                if (this.szMiShuDesc_ByteArray.GetLength(0) < num3)
                {
                    this.szMiShuDesc_ByteArray = new byte[LENGTH_szMiShuDesc];
                }
                type = srcBuf.readCString(ref this.szMiShuDesc_ByteArray, num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwMiShuIndex);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwTaskType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bTaskSubType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwPreTaskID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bNextTaskNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 2; i++)
                {
                    type = srcBuf.readUInt32(ref this.NextTaskID[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwOpenType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int j = 0; j < 3; j++)
                {
                    type = this.astOpenTaskParam[j].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt8(ref this.bPrerequisiteNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int k = 0; k < 2; k++)
                {
                    type = this.astPrerequisiteArray[k].load(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwRewardID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num7 = 0x20;
                if (this.szTaskBgIcon_ByteArray.GetLength(0) < num7)
                {
                    this.szTaskBgIcon_ByteArray = new byte[LENGTH_szTaskBgIcon];
                }
                type = srcBuf.readCString(ref this.szTaskBgIcon_ByteArray, num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num8 = 0x20;
                if (this.szTaskIcon_ByteArray.GetLength(0) < num8)
                {
                    this.szTaskIcon_ByteArray = new byte[LENGTH_szTaskIcon];
                }
                type = srcBuf.readCString(ref this.szTaskIcon_ByteArray, num8);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bTaskIconShowType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bTaskAddOnBorn);
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
            this.szTaskName = StringHelper.UTF8BytesToString(ref this.szTaskName_ByteArray);
            this.szTaskName_ByteArray = null;
            this.szTaskDesc = StringHelper.UTF8BytesToString(ref this.szTaskDesc_ByteArray);
            this.szTaskDesc_ByteArray = null;
            this.szMiShuDesc = StringHelper.UTF8BytesToString(ref this.szMiShuDesc_ByteArray);
            this.szMiShuDesc_ByteArray = null;
            this.szTaskBgIcon = StringHelper.UTF8BytesToString(ref this.szTaskBgIcon_ByteArray);
            this.szTaskBgIcon_ByteArray = null;
            this.szTaskIcon = StringHelper.UTF8BytesToString(ref this.szTaskIcon_ByteArray);
            this.szTaskIcon_ByteArray = null;
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
            type = srcBuf.readUInt32(ref this.dwTaskID);
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
                if (dest > this.szTaskName_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szTaskName)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szTaskName_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szTaskName_ByteArray, (int) dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szTaskName_ByteArray[((int) dest) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num2 = TdrTypeUtil.cstrlen(this.szTaskName_ByteArray) + 1;
                if (dest != num2)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
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
                if (num3 > this.szTaskDesc_ByteArray.GetLength(0))
                {
                    if (num3 > LENGTH_szTaskDesc)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szTaskDesc_ByteArray = new byte[num3];
                }
                if (1 > num3)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szTaskDesc_ByteArray, (int) num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szTaskDesc_ByteArray[((int) num3) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num4 = TdrTypeUtil.cstrlen(this.szTaskDesc_ByteArray) + 1;
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
                if (num5 > this.szMiShuDesc_ByteArray.GetLength(0))
                {
                    if (num5 > LENGTH_szMiShuDesc)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szMiShuDesc_ByteArray = new byte[num5];
                }
                if (1 > num5)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szMiShuDesc_ByteArray, (int) num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szMiShuDesc_ByteArray[((int) num5) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num6 = TdrTypeUtil.cstrlen(this.szMiShuDesc_ByteArray) + 1;
                if (num5 != num6)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                type = srcBuf.readUInt32(ref this.dwMiShuIndex);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwTaskType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bTaskSubType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt32(ref this.dwPreTaskID);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = srcBuf.readUInt8(ref this.bNextTaskNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 2; i++)
                {
                    type = srcBuf.readUInt32(ref this.NextTaskID[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwOpenType);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int j = 0; j < 3; j++)
                {
                    type = this.astOpenTaskParam[j].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt8(ref this.bPrerequisiteNum);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int k = 0; k < 2; k++)
                {
                    type = this.astPrerequisiteArray[k].unpack(ref srcBuf, cutVer);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                type = srcBuf.readUInt32(ref this.dwRewardID);
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
                if (num10 > this.szTaskBgIcon_ByteArray.GetLength(0))
                {
                    if (num10 > LENGTH_szTaskBgIcon)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szTaskBgIcon_ByteArray = new byte[num10];
                }
                if (1 > num10)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szTaskBgIcon_ByteArray, (int) num10);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (this.szTaskBgIcon_ByteArray[((int) num10) - 1] != 0)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                }
                int num11 = TdrTypeUtil.cstrlen(this.szTaskBgIcon_ByteArray) + 1;
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
                if (num12 > this.szTaskIcon_ByteArray.GetLength(0))
                {
                    if (num12 > LENGTH_szTaskIcon)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szTaskIcon_ByteArray = new byte[num12];
                }
                if (1 > num12)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szTaskIcon_ByteArray, (int) num12);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szTaskIcon_ByteArray[((int) num12) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num13 = TdrTypeUtil.cstrlen(this.szTaskIcon_ByteArray) + 1;
                    if (num12 != num13)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    type = srcBuf.readUInt8(ref this.bTaskIconShowType);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                    type = srcBuf.readUInt8(ref this.bTaskAddOnBorn);
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

