namespace apollo_http_object
{
    using ApolloTdr;
    using System;

    public class RequestContent : tsf4g_csharp_interface, IPackable, IUnpackable
    {
        public uint dwDataLen;
        private uint[] has_bits_ = new uint[1];
        public byte[] szData = new byte[0x1fa0];

        private void clear_has_Data()
        {
            this.has_bits_[0] &= (uint) 18446744073709551613L;
        }

        private void clear_has_DataLen()
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

        public bool has_Data()
        {
            return ((this.has_bits_[0] & 2) != 0);
        }

        public bool has_DataLen()
        {
            return ((this.has_bits_[0] & 1) != 0);
        }

        public TdrError.ErrorType packTLV(ref TdrWriteBuf destBuf, bool useVarInt)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if (useVarInt)
            {
                uint src = TdrTLV.makeTag(1, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_VARINT);
                type = destBuf.writeVarUInt32(src);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeVarUInt32(this.dwDataLen);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            else
            {
                uint num2 = TdrTLV.makeTag(1, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_4_BYTE);
                type = destBuf.writeVarUInt32(num2);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt32(this.dwDataLen);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            if (this.dwDataLen > 0x1fa0)
            {
                return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
            }
            if (this.dwDataLen > 0)
            {
                uint num3 = TdrTLV.makeTag(2, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED);
                type = destBuf.writeVarUInt32(num3);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int pos = destBuf.getUsedSize();
                type = destBuf.reserve(4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < this.dwDataLen; i++)
                {
                    type = destBuf.writeUInt8(this.szData[i]);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                int num6 = (destBuf.getUsedSize() - pos) - 4;
                type = destBuf.writeInt32(num6, pos);
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

        public void set_has_Data()
        {
            this.has_bits_[0] |= 2;
        }

        public void set_has_DataLen()
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
                        if (!this.has_DataLen())
                        {
                            this.set_has_DataLen();
                        }
                        if (useVarInt)
                        {
                            type = srcBuf.readVarUInt32(ref this.dwDataLen);
                            if (type != TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                return type;
                            }
                            continue;
                        }
                        type = srcBuf.readUInt32(ref this.dwDataLen);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            continue;
                        }
                        return type;

                    case 2:
                    {
                        if (!this.has_Data())
                        {
                            this.set_has_Data();
                        }
                        int num5 = 0;
                        type = srcBuf.readInt32(ref num5);
                        if (type != TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            return type;
                        }
                        if (num5 == 0)
                        {
                            return TdrError.ErrorType.TDR_ERR_NULL_ARRAY;
                        }
                        int num6 = srcBuf.getUsedSize();
                        for (int i = 0; i < 0x1fa0; i++)
                        {
                            type = srcBuf.readUInt8(ref this.szData[i]);
                            if (type != TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                return type;
                            }
                            if (srcBuf.getUsedSize() > (num6 + num5))
                            {
                                return TdrError.ErrorType.TDR_ERR_UNMATCHED_LENGTH;
                            }
                            if (srcBuf.getUsedSize() == (num6 + num5))
                            {
                                this.dwDataLen = (uint) (i + 1);
                                break;
                            }
                        }
                        continue;
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
            object[] args = new object[] { this.dwDataLen };
            type = TdrBufUtil.printVariable(ref destBuf, indent, separator, "[dwDataLen]", "{0:d}", args);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                if (0x1fa0 < this.dwDataLen)
                {
                    return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
                }
                type = TdrBufUtil.printArray(ref destBuf, indent, separator, "[szData]", (long) this.dwDataLen);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                for (int i = 0; i < this.dwDataLen; i++)
                {
                    object[] objArray2 = new object[] { this.szData[i] };
                    type = destBuf.sprintf("0x{0:x2}", objArray2);
                    if (type != TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return type;
                    }
                }
                object[] objArray3 = new object[] { separator };
                type = destBuf.sprintf("{0}", objArray3);
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

