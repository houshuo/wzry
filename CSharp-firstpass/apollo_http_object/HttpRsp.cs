namespace apollo_http_object
{
    using ApolloTdr;
    using System;

    public class HttpRsp : tsf4g_csharp_interface, IPackable, IUnpackable
    {
        private uint[] has_bits_ = new uint[1];
        public HttpHeaders stHttpHeaders = new HttpHeaders();
        public ResponseContent stResponseContent = new ResponseContent();
        public ResponseStatus stResponseStatus = new ResponseStatus();

        private void clear_has_HttpHeaders()
        {
            this.has_bits_[0] &= (uint) 18446744073709551613L;
        }

        private void clear_has_ResponseContent()
        {
            this.has_bits_[0] &= (uint) 18446744073709551611L;
        }

        private void clear_has_ResponseStatus()
        {
            this.has_bits_[0] &= (uint) 18446744073709551614L;
        }

        public TdrError.ErrorType construct()
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            type = this.stResponseStatus.construct();
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = this.stHttpHeaders.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stResponseContent.construct();
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            return type;
        }

        public string getLastLostRequiredFields()
        {
            return string.Empty;
        }

        public bool has_HttpHeaders()
        {
            return ((this.has_bits_[0] & 2) != 0);
        }

        public bool has_ResponseContent()
        {
            return ((this.has_bits_[0] & 4) != 0);
        }

        public bool has_ResponseStatus()
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
                type = this.stResponseStatus.packTLV(ref destBuf, useVarInt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num3 = (destBuf.getUsedSize() - pos) - 4;
                type = destBuf.writeInt32(num3, pos);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num4 = TdrTLV.makeTag(2, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED);
                type = destBuf.writeVarUInt32(num4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num5 = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stHttpHeaders.packTLV(ref destBuf, useVarInt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num6 = (destBuf.getUsedSize() - num5) - 4;
                type = destBuf.writeInt32(num6, num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num7 = TdrTLV.makeTag(3, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED);
                type = destBuf.writeVarUInt32(num7);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num8 = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = this.stResponseContent.packTLV(ref destBuf, useVarInt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num9 = (destBuf.getUsedSize() - num8) - 4;
                type = destBuf.writeInt32(num9, num8);
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

        public void set_has_HttpHeaders()
        {
            this.has_bits_[0] |= 2;
        }

        public void set_has_ResponseContent()
        {
            this.has_bits_[0] |= 4;
        }

        public void set_has_ResponseStatus()
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
                        if (!this.has_ResponseStatus())
                        {
                            this.set_has_ResponseStatus();
                        }
                        int num5 = 0;
                        type = srcBuf.readInt32(ref num5);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            type = this.stResponseStatus.unpackTLV(ref srcBuf, num5, useVarInt);
                            if (type == TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                continue;
                            }
                        }
                        return type;
                    }
                    case 2:
                    {
                        if (!this.has_HttpHeaders())
                        {
                            this.set_has_HttpHeaders();
                        }
                        int num6 = 0;
                        type = srcBuf.readInt32(ref num6);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            type = this.stHttpHeaders.unpackTLV(ref srcBuf, num6, useVarInt);
                            if (type == TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                continue;
                            }
                        }
                        return type;
                    }
                    case 3:
                    {
                        if (!this.has_ResponseContent())
                        {
                            this.set_has_ResponseContent();
                        }
                        int num7 = 0;
                        type = srcBuf.readInt32(ref num7);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            type = this.stResponseContent.unpackTLV(ref srcBuf, num7, useVarInt);
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
            type = TdrBufUtil.printVariable(ref destBuf, indent, separator, "[stResponseStatus]", true);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0 > indent)
                {
                    type = this.stResponseStatus.visualize(ref destBuf, indent, separator);
                }
                else
                {
                    type = this.stResponseStatus.visualize(ref destBuf, indent + 1, separator);
                }
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = TdrBufUtil.printVariable(ref destBuf, indent, separator, "[stHttpHeaders]", true);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (0 > indent)
                {
                    type = this.stHttpHeaders.visualize(ref destBuf, indent, separator);
                }
                else
                {
                    type = this.stHttpHeaders.visualize(ref destBuf, indent + 1, separator);
                }
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = TdrBufUtil.printVariable(ref destBuf, indent, separator, "[stResponseContent]", true);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                if (0 > indent)
                {
                    type = this.stResponseContent.visualize(ref destBuf, indent, separator);
                }
                else
                {
                    type = this.stResponseContent.visualize(ref destBuf, indent + 1, separator);
                }
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

