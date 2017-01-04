namespace apollo_http_object
{
    using ApolloTdr;
    using System;

    public class HeaderUnit : tsf4g_csharp_interface, IPackable, IUnpackable
    {
        private uint[] has_bits_ = new uint[1];
        public static readonly uint LENGTH_szHeaderContent = 260;
        public static readonly uint LENGTH_szHeaderName = 80;
        public byte[] szHeaderContent = new byte[260];
        public byte[] szHeaderName = new byte[80];

        private void clear_has_HeaderContent()
        {
            this.has_bits_[0] &= (uint) 18446744073709551613L;
        }

        private void clear_has_HeaderName()
        {
            this.has_bits_[0] &= (uint) 18446744073709551614L;
        }

        public TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public string getLastLostRequiredFields()
        {
            return string.Empty;
        }

        public bool has_HeaderContent()
        {
            return ((this.has_bits_[0] & 2) != 0);
        }

        public bool has_HeaderName()
        {
            return ((this.has_bits_[0] & 1) != 0);
        }

        public TdrError.ErrorType packTLV(ref TdrWriteBuf destBuf, bool useVarInt)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            uint src = TdrTLV.makeTag(1, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED);
            type = destBuf.writeVarUInt32(src);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                int pos = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int count = TdrTypeUtil.cstrlen(this.szHeaderName);
                if (count >= 80)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szHeaderName, count);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num4 = (destBuf.getUsedSize() - pos) - 4;
                type = destBuf.writeInt32(num4, pos);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num5 = TdrTLV.makeTag(2, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED);
                type = destBuf.writeVarUInt32(num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num6 = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num7 = TdrTypeUtil.cstrlen(this.szHeaderContent);
                if (num7 >= 260)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szHeaderContent, num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num8 = (destBuf.getUsedSize() - num6) - 4;
                type = destBuf.writeInt32(num8, num6);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            return type;
        }

        public TdrError.ErrorType packTLV(ref byte[] buffer, int size, ref int used, bool useVarInt)
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
                type = this.packTLV(ref destBuf, useVarInt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                used = destBuf.getUsedSize();
                destBuf.writeInt32(used, pos);
            }
            return type;
        }

        private int requiredFieldNum()
        {
            return 0;
        }

        public void set_has_HeaderContent()
        {
            this.has_bits_[0] |= 2;
        }

        public void set_has_HeaderName()
        {
            this.has_bits_[0] |= 1;
        }

        public TdrError.ErrorType unpackTLV(ref byte[] buffer, int size, ref int used)
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
                type = this.unpackTLV(ref srcBuf, num2 - TdrTLV.TLV_MSG_MIN_SIZE, useVarInt);
                if (type == TdrError.ErrorType.TDR_NO_ERROR)
                {
                    used = srcBuf.getUsedSize();
                }
            }
            return type;
        }

        public TdrError.ErrorType unpackTLV(ref TdrReadBuf srcBuf, int length, bool useVarInt)
        {
            if ((srcBuf == null) || (length <= 0))
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            uint dest = 0;
            int num3 = 0;
            int num4 = srcBuf.getUsedSize();
            while (srcBuf.getUsedSize() < (num4 + length))
            {
                type = srcBuf.readVarUInt32(ref dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                switch (TdrTLV.getFieldId(dest))
                {
                    case 1:
                    {
                        if (!this.has_HeaderName())
                        {
                            this.set_has_HeaderName();
                        }
                        int num5 = 0;
                        type = srcBuf.readInt32(ref num5);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            if (num5 >= 80)
                            {
                                return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                            }
                            type = srcBuf.readCString(ref this.szHeaderName, num5);
                            if (type == TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                continue;
                            }
                        }
                        return type;
                    }
                    case 2:
                    {
                        if (!this.has_HeaderContent())
                        {
                            this.set_has_HeaderContent();
                        }
                        int num6 = 0;
                        type = srcBuf.readInt32(ref num6);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            if (num6 >= 260)
                            {
                                return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                            }
                            type = srcBuf.readCString(ref this.szHeaderContent, num6);
                            if (type == TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                continue;
                            }
                        }
                        return type;
                    }
                }
                uint num7 = TdrTLV.getTypeId(dest);
                type = TdrTLV.skipUnknownFields(ref srcBuf, (TdrTLV.TdrTLVTypeId) num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            if (srcBuf.getUsedSize() > (num4 + length))
            {
                return TdrError.ErrorType.TDR_ERR_UNMATCHED_LENGTH;
            }
            if (num3 < this.requiredFieldNum())
            {
                return TdrError.ErrorType.TDR_ERR_LOST_REQUIRED_FIELD;
            }
            return type;
        }

        public TdrError.ErrorType visualize(ref TdrVisualBuf destBuf, int indent, char separator)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = TdrBufUtil.printString(ref destBuf, indent, separator, "[szHeaderName]", this.szHeaderName);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = TdrBufUtil.printString(ref destBuf, indent, separator, "[szHeaderContent]", this.szHeaderContent);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            return type;
        }

        public TdrError.ErrorType visualize(ref string buffer, int indent, char separator)
        {
            TdrVisualBuf destBuf = new TdrVisualBuf();
            TdrError.ErrorType type = this.visualize(ref destBuf, indent, separator);
            buffer = destBuf.getVisualBuf();
            return type;
        }
    }
}

