namespace apollo_talker
{
    using ApolloTdr;
    using System;

    public class TalkerHead : tsf4g_csharp_interface, IPackable, IUnpackable
    {
        public byte bCmdFmt;
        public byte bDomain;
        public byte bFlag;
        public uint dwAsync;
        private uint[] has_bits_ = new uint[1];
        public CmdValue stCommand = new CmdValue();

        private void clear_has_Async()
        {
            this.has_bits_[0] &= (uint) 18446744073709551614L;
        }

        private void clear_has_CmdFmt()
        {
            this.has_bits_[0] &= (uint) 18446744073709551607L;
        }

        private void clear_has_Command()
        {
            this.has_bits_[0] &= (uint) 18446744073709551599L;
        }

        private void clear_has_Domain()
        {
            this.has_bits_[0] &= (uint) 18446744073709551611L;
        }

        private void clear_has_Flag()
        {
            this.has_bits_[0] &= (uint) 18446744073709551613L;
        }

        public TdrError.ErrorType construct()
        {
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public string getLastLostRequiredFields()
        {
            return string.Empty;
        }

        public bool has_Async()
        {
            return ((this.has_bits_[0] & 1) != 0);
        }

        public bool has_CmdFmt()
        {
            return ((this.has_bits_[0] & 8) != 0);
        }

        public bool has_Command()
        {
            return ((this.has_bits_[0] & 0x10) != 0);
        }

        public bool has_Domain()
        {
            return ((this.has_bits_[0] & 4) != 0);
        }

        public bool has_Flag()
        {
            return ((this.has_bits_[0] & 2) != 0);
        }

        public TdrError.ErrorType packTLV(ref TdrWriteBuf destBuf, bool useVarInt)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            if (useVarInt)
            {
                uint num = TdrTLV.makeTag(1, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_VARINT);
                type = destBuf.writeVarUInt32(num);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeVarUInt32(this.dwAsync);
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
                type = destBuf.writeUInt32(this.dwAsync);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
            }
            uint src = TdrTLV.makeTag(2, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_1_BYTE);
            type = destBuf.writeVarUInt32(src);
            if (type == TdrError.ErrorType.TDR_NO_ERROR)
            {
                type = destBuf.writeUInt8(this.bFlag);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num4 = TdrTLV.makeTag(3, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_1_BYTE);
                type = destBuf.writeVarUInt32(num4);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bDomain);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num5 = TdrTLV.makeTag(4, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_1_BYTE);
                type = destBuf.writeVarUInt32(num5);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                type = destBuf.writeUInt8(this.bCmdFmt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                uint num6 = TdrTLV.makeTag(5, TdrTLV.TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED);
                type = destBuf.writeVarUInt32(num6);
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
                type = this.stCommand.packTLV((long) this.bCmdFmt, ref destBuf, useVarInt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                int num8 = (destBuf.getUsedSize() - pos) - 4;
                type = destBuf.writeInt32(num8, pos);
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

        public void set_has_Async()
        {
            this.has_bits_[0] |= 1;
        }

        public void set_has_CmdFmt()
        {
            this.has_bits_[0] |= 8;
        }

        public void set_has_Command()
        {
            this.has_bits_[0] |= 0x10;
        }

        public void set_has_Domain()
        {
            this.has_bits_[0] |= 4;
        }

        public void set_has_Flag()
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
                int num5;
                uint num7;
                type = srcBuf.readVarUInt32(ref dest);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                switch (TdrTLV.getFieldId(dest))
                {
                    case 1:
                        if (!this.has_Async())
                        {
                            this.set_has_Async();
                        }
                        if (useVarInt)
                        {
                            type = srcBuf.readVarUInt32(ref this.dwAsync);
                            if (type != TdrError.ErrorType.TDR_NO_ERROR)
                            {
                                return type;
                            }
                            continue;
                        }
                        type = srcBuf.readUInt32(ref this.dwAsync);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            continue;
                        }
                        return type;

                    case 2:
                        if (!this.has_Flag())
                        {
                            this.set_has_Flag();
                        }
                        type = srcBuf.readUInt8(ref this.bFlag);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            continue;
                        }
                        return type;

                    case 3:
                        if (!this.has_Domain())
                        {
                            this.set_has_Domain();
                        }
                        type = srcBuf.readUInt8(ref this.bDomain);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            continue;
                        }
                        return type;

                    case 4:
                        if (!this.has_CmdFmt())
                        {
                            this.set_has_CmdFmt();
                        }
                        type = srcBuf.readUInt8(ref this.bCmdFmt);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            continue;
                        }
                        return type;

                    case 5:
                        num5 = 0;
                        type = srcBuf.readInt32(ref num5);
                        if (type == TdrError.ErrorType.TDR_NO_ERROR)
                        {
                            break;
                        }
                        return type;

                    default:
                        goto Label_0177;
                }
                long selector = 0L;
                type = this.stCommand.unpackTLV(ref selector, ref srcBuf, num5, useVarInt);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                this.bCmdFmt = (byte) selector;
                continue;
            Label_0177:
                num7 = TdrTLV.getTypeId(dest);
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
    }
}

