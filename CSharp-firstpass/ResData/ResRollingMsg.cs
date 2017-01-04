namespace ResData
{
    using System;
    using tsf4g_tdr_csharp;

    public class ResRollingMsg : IUnpackable, tsf4g_csharp_interface
    {
        public static readonly uint BASEVERSION = 1;
        public static readonly uint CURRVERSION = 1;
        public uint dwRollingMsgID;
        public static readonly uint LENGTH_szRollingMsgText = 0x100;
        public uint[] RollingMsgParam = new uint[2];
        public string szRollingMsgText = string.Empty;
        public byte[] szRollingMsgText_ByteArray = new byte[1];
        public uint[] TriggerCondition = new uint[15];

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
            type = srcBuf.readUInt32(ref this.dwRollingMsgID);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int count = 0x100;
                if (this.szRollingMsgText_ByteArray.GetLength(0) < count)
                {
                    this.szRollingMsgText_ByteArray = new byte[LENGTH_szRollingMsgText];
                }
                type = srcBuf.readCString(ref this.szRollingMsgText_ByteArray, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < 15; i++)
                {
                    type = srcBuf.readUInt32(ref this.TriggerCondition[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                for (int j = 0; j < 2; j++)
                {
                    type = srcBuf.readUInt32(ref this.RollingMsgParam[j]);
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
            this.szRollingMsgText = StringHelper.UTF8BytesToString(ref this.szRollingMsgText_ByteArray);
            this.szRollingMsgText_ByteArray = null;
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
            type = srcBuf.readUInt32(ref this.dwRollingMsgID);
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
                if (dest > this.szRollingMsgText_ByteArray.GetLength(0))
                {
                    if (dest > LENGTH_szRollingMsgText)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                    }
                    this.szRollingMsgText_ByteArray = new byte[dest];
                }
                if (1 > dest)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
                }
                type = srcBuf.readCString(ref this.szRollingMsgText_ByteArray, (int) dest);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    if (this.szRollingMsgText_ByteArray[((int) dest) - 1] != 0)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    int num2 = TdrTypeUtil.cstrlen(this.szRollingMsgText_ByteArray) + 1;
                    if (dest != num2)
                    {
                        return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
                    }
                    for (int i = 0; i < 15; i++)
                    {
                        type = srcBuf.readUInt32(ref this.TriggerCondition[i]);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                    }
                    for (int j = 0; j < 2; j++)
                    {
                        type = srcBuf.readUInt32(ref this.RollingMsgParam[j]);
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

