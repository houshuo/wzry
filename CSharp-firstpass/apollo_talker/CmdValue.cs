namespace apollo_talker
{
    using ApolloTdr;
    using System;

    public class CmdValue
    {
        public int iIntCmd;
        public int iNilCmd;
        public static readonly uint LENGTH_szStrCmd = 0x40;
        public byte[] szStrCmd;

        public TdrError.ErrorType construct(long selector)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            tsf4g_csharp_interface _interface = this.select(selector);
            if (_interface != null)
            {
                return _interface.construct();
            }
            if (selector == 0)
            {
                this.iNilCmd = 0;
                return type;
            }
            if (selector == 1L)
            {
                if (this.szStrCmd == null)
                {
                    this.szStrCmd = new byte[0x40];
                }
                return type;
            }
            if (selector == 2L)
            {
                this.iIntCmd = 0;
            }
            return type;
        }

        public TdrError.ErrorType packTLV(long selector, ref TdrWriteBuf destBuf, bool useVarInt)
        {
            if (destBuf == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            long num9 = selector;
            if ((num9 >= 0L) && (num9 <= 2L))
            {
                switch (((int) num9))
                {
                    case 0:
                    {
                        if (!useVarInt)
                        {
                            uint num2 = TdrTLV.makeTag(0, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_4_BYTE);
                            type = destBuf.writeVarUInt32(num2);
                            if (type == TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                type = destBuf.writeInt32(this.iNilCmd);
                                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                                {
                                    return type;
                                }
                            }
                            return type;
                        }
                        uint src = TdrTLV.makeTag(0, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_VARINT);
                        type = destBuf.writeVarUInt32(src);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            type = destBuf.writeVarInt32(this.iNilCmd);
                            if (type == TdrError.ErrorType.TDR_NO_ERROR)
                            {
                            }
                            return type;
                        }
                        return type;
                    }
                    case 1:
                        if (this.szStrCmd != null)
                        {
                            uint num3 = TdrTLV.makeTag(1, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED);
                            type = destBuf.writeVarUInt32(num3);
                            if (type == TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                int pos = destBuf.getUsedSize();
                                type = destBuf.reserve(4);
                                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                                {
                                    return type;
                                }
                                int count = TdrTypeUtil.cstrlen(this.szStrCmd);
                                if (count >= 0x40)
                                {
                                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                                }
                                type = destBuf.writeCString(this.szStrCmd, count);
                                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                                {
                                    return type;
                                }
                                int num6 = (destBuf.getUsedSize() - pos) - 4;
                                type = destBuf.writeInt32(num6, pos);
                                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                                {
                                    return type;
                                }
                            }
                            return type;
                        }
                        return TdrError.ErrorType.TDR_ERR_UNION_SELECTE_FIELD_IS_NULL;

                    case 2:
                    {
                        if (!useVarInt)
                        {
                            uint num8 = TdrTLV.makeTag(2, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_4_BYTE);
                            type = destBuf.writeVarUInt32(num8);
                            if (type == TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                type = destBuf.writeInt32(this.iIntCmd);
                                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                                {
                                    return type;
                                }
                            }
                            return type;
                        }
                        uint num7 = TdrTLV.makeTag(2, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_VARINT);
                        type = destBuf.writeVarUInt32(num7);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            type = destBuf.writeVarInt32(this.iIntCmd);
                            if (type == TdrError.ErrorType.TDR_NO_ERROR)
                            {
                            }
                            return type;
                        }
                        return type;
                    }
                }
            }
            return TdrError.ErrorType.TDR_ERR_SUSPICIOUS_SELECTOR;
        }

        public TdrError.ErrorType packTLV(long selector, ref byte[] buffer, int size, ref int used, bool useVarInt)
        {
            if (((buffer == null) || (buffer.GetLength(0) == 0)) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrWriteBuf destBuf = new TdrWriteBuf(ref buffer, size);
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if (useVarInt)
            {
                type = destBuf.writeUInt8(170);
            }
            else
            {
                type = destBuf.writeUInt8(0x99);
            }
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int pos = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.packTLV(selector, ref destBuf, useVarInt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                used = destBuf.getUsedSize();
                destBuf.writeInt32(used, pos);
            }
            return type;
        }

        public tsf4g_csharp_interface select(long selector)
        {
            return null;
        }

        public TdrError.ErrorType unpackTLV(ref long selector, ref byte[] buffer, int size, ref int used)
        {
            if (((buffer == null) || (buffer.GetLength(0) == 0)) || (size > buffer.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            if (size < TdrTLV.TLV_MSG_MIN_SIZE)
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
            }
            TdrReadBuf srcBuf = new TdrReadBuf(ref buffer, size);
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            byte dest = 0;
            bool useVarInt = true;
            type = srcBuf.readUInt8(ref dest);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                switch (dest)
                {
                    case 170:
                        useVarInt = true;
                        break;

                    case 0x99:
                        useVarInt = false;
                        break;

                    default:
                        return TdrError.ErrorType.TDR_ERR_BAD_TLV_MAGIC;
                }
                int num2 = 0;
                srcBuf.readInt32(ref num2);
                if (size < num2)
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                type = this.unpackTLV(ref selector, ref srcBuf, num2 - TdrTLV.TLV_MSG_MIN_SIZE, useVarInt);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    used = srcBuf.getUsedSize();
                }
            }
            return type;
        }

        public TdrError.ErrorType unpackTLV(ref long selector, ref TdrReadBuf srcBuf, int length, bool useVarInt)
        {
            if ((srcBuf == null) || (length == 0))
            {
                return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
            }
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            int num = srcBuf.getUsedSize();
            uint dest = 0;
            type = srcBuf.readVarUInt32(ref dest);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                uint num3 = TdrTLV.getFieldId(dest);
                switch (num3)
                {
                    case 0:
                        if (!useVarInt)
                        {
                            type = srcBuf.readInt32(ref this.iNilCmd);
                            if (type == TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                break;
                            }
                            return type;
                        }
                        type = srcBuf.readVarInt32(ref this.iNilCmd);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            break;
                        }
                        return type;

                    case 1:
                    {
                        if (this.szStrCmd == null)
                        {
                            this.szStrCmd = new byte[0x40];
                        }
                        int num4 = 0;
                        type = srcBuf.readInt32(ref num4);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            if (num4 >= 0x40)
                            {
                                return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                            }
                            type = srcBuf.readCString(ref this.szStrCmd, num4);
                            if (type == TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                break;
                            }
                        }
                        return type;
                    }
                    case 2:
                        if (!useVarInt)
                        {
                            type = srcBuf.readInt32(ref this.iIntCmd);
                            if (type == TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                break;
                            }
                            return type;
                        }
                        type = srcBuf.readVarInt32(ref this.iIntCmd);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            break;
                        }
                        return type;

                    default:
                        type = TdrError.ErrorType.TDR_ERR_SUSPICIOUS_SELECTOR;
                        break;
                }
                if (srcBuf.getUsedSize() > (num + length))
                {
                    return TdrError.ErrorType.TDR_ERR_UNMATCHED_LENGTH;
                }
                selector = num3;
            }
            return type;
        }
    }
}

