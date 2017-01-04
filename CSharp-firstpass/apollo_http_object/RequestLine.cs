namespace apollo_http_object
{
    using ApolloTdr;
    using System;
    using System.Text;

    public class RequestLine : tsf4g_csharp_interface, IPackable, IUnpackable
    {
        private uint[] has_bits_ = new uint[1];
        public static readonly uint LENGTH_szHttpVersion = 0x20;
        public static readonly uint LENGTH_szRequestMethod = 0x20;
        public static readonly uint LENGTH_szRequestUri = 0x400;
        public byte[] szHttpVersion = new byte[0x20];
        public byte[] szRequestMethod = new byte[0x20];
        public byte[] szRequestUri = new byte[0x400];

        private void clear_has_HttpVersion()
        {
            this.has_bits_[0] &= (uint) 18446744073709551611L;
        }

        private void clear_has_RequestMethod()
        {
            this.has_bits_[0] &= (uint) 18446744073709551614L;
        }

        private void clear_has_RequestUri()
        {
            this.has_bits_[0] &= (uint) 18446744073709551613L;
        }

        public TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            string s = "HTTP/1.1";
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            if ((bytes.GetLength(0) + 1) > this.szHttpVersion.GetLength(0))
            {
                if (bytes.GetLength(0) >= LENGTH_szHttpVersion)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                this.szHttpVersion = new byte[bytes.GetLength(0) + 1];
            }
            for (int i = 0; i < bytes.GetLength(0); i++)
            {
                this.szHttpVersion[i] = bytes[i];
            }
            this.szHttpVersion[bytes.GetLength(0)] = 0;
            return type;
        }

        public string getLastLostRequiredFields()
        {
            return string.Empty;
        }

        public bool has_HttpVersion()
        {
            return ((this.has_bits_[0] & 4) != 0);
        }

        public bool has_RequestMethod()
        {
            return ((this.has_bits_[0] & 1) != 0);
        }

        public bool has_RequestUri()
        {
            return ((this.has_bits_[0] & 2) != 0);
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
                int count = TdrTypeUtil.cstrlen(this.szRequestMethod);
                if (count >= 0x20)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szRequestMethod, count);
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
                int num7 = TdrTypeUtil.cstrlen(this.szRequestUri);
                if (num7 >= 0x400)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szRequestUri, num7);
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
                uint num9 = TdrTLV.makeTag(3, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED);
                type = destBuf.writeVarUInt32(num9);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num10 = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num11 = TdrTypeUtil.cstrlen(this.szHttpVersion);
                if (num11 >= 0x20)
                {
                    return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                }
                type = destBuf.writeCString(this.szHttpVersion, num11);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num12 = (destBuf.getUsedSize() - num10) - 4;
                type = destBuf.writeInt32(num12, num10);
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

        public void set_has_HttpVersion()
        {
            this.has_bits_[0] |= 4;
        }

        public void set_has_RequestMethod()
        {
            this.has_bits_[0] |= 1;
        }

        public void set_has_RequestUri()
        {
            this.has_bits_[0] |= 2;
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
                        if (!this.has_RequestMethod())
                        {
                            this.set_has_RequestMethod();
                        }
                        int num5 = 0;
                        type = srcBuf.readInt32(ref num5);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            if (num5 >= 0x20)
                            {
                                return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                            }
                            type = srcBuf.readCString(ref this.szRequestMethod, num5);
                            if (type == TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                continue;
                            }
                        }
                        return type;
                    }
                    case 2:
                    {
                        if (!this.has_RequestUri())
                        {
                            this.set_has_RequestUri();
                        }
                        int num6 = 0;
                        type = srcBuf.readInt32(ref num6);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            if (num6 >= 0x400)
                            {
                                return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                            }
                            type = srcBuf.readCString(ref this.szRequestUri, num6);
                            if (type == TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                continue;
                            }
                        }
                        return type;
                    }
                    case 3:
                    {
                        if (!this.has_HttpVersion())
                        {
                            this.set_has_HttpVersion();
                        }
                        int num7 = 0;
                        type = srcBuf.readInt32(ref num7);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            if (num7 >= 0x20)
                            {
                                return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
                            }
                            type = srcBuf.readCString(ref this.szHttpVersion, num7);
                            if (type == TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                continue;
                            }
                        }
                        return type;
                    }
                }
                uint num8 = TdrTLV.getTypeId(dest);
                type = TdrTLV.skipUnknownFields(ref srcBuf, (TdrTLV.TdrTLVTypeId) num8);
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
            type = TdrBufUtil.printString(ref destBuf, indent, separator, "[szRequestMethod]", this.szRequestMethod);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = TdrBufUtil.printString(ref destBuf, indent, separator, "[szRequestUri]", this.szRequestUri);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = TdrBufUtil.printString(ref destBuf, indent, separator, "[szHttpVersion]", this.szHttpVersion);
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

