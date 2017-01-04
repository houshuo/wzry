namespace tsf4g_tdr_csharp
{
    using System;
    using System.Net;

    public class TdrWriteBuf : TdrBufBase
    {
        private byte[] beginPtr;
        private int length;
        private int position;

        public TdrWriteBuf()
        {
            this.beginPtr = null;
            this.position = 0;
            this.length = 0;
        }

        public TdrWriteBuf(int len)
        {
            this.beginPtr = new byte[len];
            this.position = 0;
            this.length = 0;
            if (this.beginPtr != null)
            {
                this.length = len;
            }
        }

        public TdrWriteBuf(ref byte[] ptr, int len)
        {
            this._set(ref ptr, len);
        }

        private void _reset()
        {
            this.position = 0;
            this.length = 0;
            this.beginPtr = null;
        }

        private void _set(ref byte[] ptr, int len)
        {
            this.beginPtr = ptr;
            this.position = 0;
            this.length = 0;
            if (this.beginPtr != null)
            {
                this.length = len;
            }
        }

        public byte[] getBeginPtr()
        {
            return this.beginPtr;
        }

        public int getLeftSize()
        {
            return (this.length - this.position);
        }

        public int getTotalSize()
        {
            return this.length;
        }

        public int getUsedSize()
        {
            return this.position;
        }

        public override void OnRelease()
        {
            this._reset();
        }

        public TdrError.ErrorType reserve(int gap)
        {
            if (this.position > this.length)
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
            }
            if (gap > (this.length - this.position))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
            }
            this.position += gap;
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public void reset()
        {
            this._reset();
        }

        public void set(ref byte[] ptr, int len)
        {
            this._set(ref ptr, len);
        }

        public TdrError.ErrorType writeCString(byte[] src, int count)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (count > (this.length - this.position))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
            }
            for (int i = 0; i < count; i++)
            {
                this.beginPtr[this.position++] = src[i];
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeCString(byte[] src, int count, int pos)
        {
            if ((this.beginPtr == null) || (count > src.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (count > (this.length - pos))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
            }
            for (int i = 0; i < count; i++)
            {
                this.beginPtr[pos + i] = src[i];
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeDouble(double src)
        {
            long num = BitConverter.DoubleToInt64Bits(src);
            return this.writeInt64(num);
        }

        public TdrError.ErrorType writeDouble(double src, int pos)
        {
            long num = BitConverter.DoubleToInt64Bits(src);
            return this.writeInt64(num, pos);
        }

        public TdrError.ErrorType writeFloat(float src)
        {
            int num = BitConverter.ToInt32(BitConverter.GetBytes(src), 0);
            return this.writeInt32(num);
        }

        public TdrError.ErrorType writeFloat(float src, int pos)
        {
            int num = BitConverter.ToInt32(BitConverter.GetBytes(src), 0);
            return this.writeInt32(num, pos);
        }

        public TdrError.ErrorType writeInt16(short src)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (2 > (this.length - this.position))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
            }
            if (BitConverter.IsLittleEndian)
            {
                src = IPAddress.HostToNetworkOrder(src);
            }
            byte[] bytes = BitConverter.GetBytes(src);
            for (int i = 0; i < bytes.GetLength(0); i++)
            {
                this.beginPtr[this.position++] = bytes[i];
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeInt16(short src, int pos)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (2 > (this.length - pos))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
            }
            if (BitConverter.IsLittleEndian)
            {
                src = IPAddress.HostToNetworkOrder(src);
            }
            byte[] bytes = BitConverter.GetBytes(src);
            for (int i = 0; i < bytes.GetLength(0); i++)
            {
                this.beginPtr[pos + i] = bytes[i];
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeInt32(int src)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (4 > (this.length - this.position))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
            }
            if (BitConverter.IsLittleEndian)
            {
                src = IPAddress.HostToNetworkOrder(src);
            }
            byte[] bytes = BitConverter.GetBytes(src);
            for (int i = 0; i < bytes.GetLength(0); i++)
            {
                this.beginPtr[this.position++] = bytes[i];
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeInt32(int src, int pos)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (4 > (this.length - pos))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
            }
            if (BitConverter.IsLittleEndian)
            {
                src = IPAddress.HostToNetworkOrder(src);
            }
            byte[] bytes = BitConverter.GetBytes(src);
            for (int i = 0; i < bytes.GetLength(0); i++)
            {
                this.beginPtr[pos + i] = bytes[i];
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeInt64(long src)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (8 > (this.length - this.position))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
            }
            if (BitConverter.IsLittleEndian)
            {
                src = IPAddress.HostToNetworkOrder(src);
            }
            byte[] bytes = BitConverter.GetBytes(src);
            for (int i = 0; i < bytes.GetLength(0); i++)
            {
                this.beginPtr[this.position++] = bytes[i];
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeInt64(long src, int pos)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (8 > (this.length - pos))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
            }
            if (BitConverter.IsLittleEndian)
            {
                src = IPAddress.HostToNetworkOrder(src);
            }
            byte[] bytes = BitConverter.GetBytes(src);
            for (int i = 0; i < bytes.GetLength(0); i++)
            {
                this.beginPtr[pos + i] = bytes[i];
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeInt8(sbyte src)
        {
            return this.writeUInt8((byte) src);
        }

        public TdrError.ErrorType writeInt8(sbyte src, int pos)
        {
            return this.writeUInt8((byte) src, pos);
        }

        public TdrError.ErrorType writeUInt16(ushort src)
        {
            return this.writeInt16((short) src);
        }

        public TdrError.ErrorType writeUInt16(ushort src, int pos)
        {
            return this.writeInt16((short) src, pos);
        }

        public TdrError.ErrorType writeUInt32(uint src)
        {
            return this.writeInt32((int) src);
        }

        public TdrError.ErrorType writeUInt32(uint src, int pos)
        {
            return this.writeInt32((int) src, pos);
        }

        public TdrError.ErrorType writeUInt64(ulong src)
        {
            return this.writeInt64((long) src);
        }

        public TdrError.ErrorType writeUInt64(ulong src, int pos)
        {
            return this.writeInt64((long) src, pos);
        }

        public TdrError.ErrorType writeUInt8(byte src)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (1 > (this.length - this.position))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
            }
            this.beginPtr[this.position++] = src;
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeUInt8(byte src, int pos)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (1 > (this.length - pos))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
            }
            this.beginPtr[pos] = src;
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeVarInt16(short src)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (!BitConverter.IsLittleEndian)
            {
                src = IPAddress.HostToNetworkOrder(src);
            }
            src = (short) ((src << 1) ^ (src >> 15));
            ushort num = (ushort) src;
            byte num2 = 0;
            do
            {
                if ((this.length - this.position) <= 0)
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
                }
                num2 = (byte) (BitConverter.GetBytes(num)[0] & 0x7f);
                num = (ushort) (num >> 7);
                if (num != 0)
                {
                    num2 = (byte) (num2 | 0x80);
                }
                this.beginPtr[this.position++] = num2;
            }
            while (num != 0);
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeVarInt32(int src)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (!BitConverter.IsLittleEndian)
            {
                src = IPAddress.HostToNetworkOrder(src);
            }
            src = (src << 1) ^ (src >> 0x1f);
            uint num = (uint) src;
            byte num2 = 0;
            do
            {
                if ((this.length - this.position) <= 0)
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
                }
                num2 = (byte) (BitConverter.GetBytes(num)[0] & 0x7f);
                num = num >> 7;
                if (num != 0)
                {
                    num2 = (byte) (num2 | 0x80);
                }
                this.beginPtr[this.position++] = num2;
            }
            while (num != 0);
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeVarInt64(long src)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (!BitConverter.IsLittleEndian)
            {
                src = IPAddress.HostToNetworkOrder(src);
            }
            src = (src << 1) ^ (src >> 0x3f);
            ulong num = (ulong) src;
            byte num2 = 0;
            do
            {
                if ((this.length - this.position) <= 0)
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
                }
                num2 = (byte) (BitConverter.GetBytes(num)[0] & 0x7f);
                num = num >> 7;
                if (num != 0)
                {
                    num2 = (byte) (num2 | 0x80);
                }
                this.beginPtr[this.position++] = num2;
            }
            while (num != 0);
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeVarUInt16(ushort src)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (!BitConverter.IsLittleEndian)
            {
                src = (ushort) IPAddress.HostToNetworkOrder((short) src);
            }
            byte num = 0;
            do
            {
                if ((this.length - this.position) <= 0)
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
                }
                num = (byte) (BitConverter.GetBytes(src)[0] & 0x7f);
                src = (ushort) (src >> 7);
                if (src != 0)
                {
                    num = (byte) (num | 0x80);
                }
                this.beginPtr[this.position++] = num;
            }
            while (src != 0);
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeVarUInt32(uint src)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (!BitConverter.IsLittleEndian)
            {
                src = (uint) IPAddress.HostToNetworkOrder((int) src);
            }
            byte num = 0;
            do
            {
                if ((this.length - this.position) <= 0)
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
                }
                num = (byte) (BitConverter.GetBytes(src)[0] & 0x7f);
                src = src >> 7;
                if (src != 0)
                {
                    num = (byte) (num | 0x80);
                }
                this.beginPtr[this.position++] = num;
            }
            while (src != 0);
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeVarUInt64(ulong src)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (!BitConverter.IsLittleEndian)
            {
                src = (ulong) IPAddress.HostToNetworkOrder((long) src);
            }
            byte num = 0;
            do
            {
                if ((this.length - this.position) <= 0)
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
                }
                num = (byte) (BitConverter.GetBytes(src)[0] & 0x7f);
                src = src >> 7;
                if (src != 0)
                {
                    num = (byte) (num | 0x80);
                }
                this.beginPtr[this.position++] = num;
            }
            while (src != 0);
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeWString(short[] src, int count)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if ((2 * count) > (this.length - this.position))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
            }
            for (int i = 0; i < count; i++)
            {
                byte[] bytes = BitConverter.GetBytes(src[i]);
                for (int j = 0; j < bytes.GetLength(0); j++)
                {
                    this.beginPtr[this.position++] = bytes[j];
                }
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType writeWString(short[] src, int count, int pos)
        {
            if ((this.beginPtr == null) || (count > src.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if ((2 * count) > (this.length - pos))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
            }
            for (int i = 0; i < count; i++)
            {
                byte[] bytes = BitConverter.GetBytes(src[i]);
                for (int j = 0; j < bytes.GetLength(0); j++)
                {
                    this.beginPtr[pos + ((2 * i) + j)] = bytes[j];
                }
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }
    }
}

