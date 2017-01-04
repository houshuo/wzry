namespace ApolloTdr
{
    using System;

    public class TdrTLV
    {
        public static readonly int TLV_MSG_MAGIC_SIZE = 1;
        public static readonly int TLV_MSG_MIN_SIZE = 5;

        public static uint getFieldId(uint tagid)
        {
            return (tagid >> 4);
        }

        public static int getMsgSize(ref byte[] buffer, int size)
        {
            if ((buffer == null) || (size < TLV_MSG_MIN_SIZE))
            {
                return -1;
            }
            int dest = 0;
            new TdrReadBuf(ref buffer, size).readInt32(ref dest, TLV_MSG_MAGIC_SIZE);
            return dest;
        }

        public static uint getTypeId(uint tagid)
        {
            return (tagid & 15);
        }

        public static uint makeTag(int id, TdrTLVTypeId type)
        {
            return (uint) ((id << 4) | type);
        }

        public static TdrError.ErrorType skipUnknownFields(ref TdrReadBuf srcBuf, TdrTLVTypeId type_id)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            switch (type_id)
            {
                case TdrTLVTypeId.TDR_TYPE_ID_VARINT:
                {
                    long dest = 0L;
                    return srcBuf.readVarInt64(ref dest);
                }
                case TdrTLVTypeId.TDR_TYPE_ID_1_BYTE:
                    return srcBuf.skipForward(1);

                case TdrTLVTypeId.TDR_TYPE_ID_2_BYTE:
                    return srcBuf.skipForward(2);

                case TdrTLVTypeId.TDR_TYPE_ID_4_BYTE:
                    return srcBuf.skipForward(4);

                case TdrTLVTypeId.TDR_TYPE_ID_8_BYTE:
                    return srcBuf.skipForward(8);

                case TdrTLVTypeId.TDR_TYPE_ID_LENGTH_DELIMITED:
                {
                    int num2 = 0;
                    type = srcBuf.readInt32(ref num2);
                    if (type == TdrError.ErrorType.TDR_NO_ERROR)
                    {
                        return srcBuf.skipForward(num2);
                    }
                    return type;
                }
            }
            return TdrError.ErrorType.TDR_ERR_UNKNOWN_TYPE_ID;
        }

        public enum TdrTLVTypeId
        {
            TDR_TYPE_ID_VARINT,
            TDR_TYPE_ID_1_BYTE,
            TDR_TYPE_ID_2_BYTE,
            TDR_TYPE_ID_4_BYTE,
            TDR_TYPE_ID_8_BYTE,
            TDR_TYPE_ID_LENGTH_DELIMITED
        }

        public enum TLV_MAGIC
        {
            TLV_MAGIC_NOVARINT = 0x99,
            TLV_MAGIC_VARINT = 170
        }
    }
}

