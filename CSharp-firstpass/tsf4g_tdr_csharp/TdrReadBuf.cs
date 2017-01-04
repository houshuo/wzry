namespace tsf4g_tdr_csharp
{
    using System;
    using System.Net;
    using System.Runtime.InteropServices;

    public class TdrReadBuf : TdrBufBase
    {
        private byte[] beginPtr;
        private bool IsNetEndian;
        private int length;
        private int position;

        public TdrReadBuf()
        {
            this.length = 0;
            this.position = 0;
            this.beginPtr = null;
            this.IsNetEndian = true;
        }

        public TdrReadBuf(ref TdrWriteBuf writeBuf)
        {
            byte[] ptr = writeBuf.getBeginPtr();
            this.set(ref ptr, writeBuf.getUsedSize());
        }

        public TdrReadBuf(ref byte[] ptr, int len)
        {
            this.set(ref ptr, len);
        }

        public void disableEndian()
        {
            this.IsNetEndian = false;
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
            this.reset();
        }

        public TdrError.ErrorType readCString(ref byte[] dest, int count)
        {
            if ((this.beginPtr == null) || (count > dest.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if ((dest == null) || (dest.GetLength(0) == 0))
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (count > (this.length - this.position))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
            }
            for (int i = 0; i < count; i++)
            {
                dest[i] = this.beginPtr[this.position++];
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readCString(ref byte[] dest, int count, int pos)
        {
            if ((this.beginPtr == null) || (count > dest.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if ((dest == null) || (dest.GetLength(0) == 0))
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (count > (this.length - pos))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
            }
            for (int i = 0; i < count; i++)
            {
                dest[i] = this.beginPtr[pos + count];
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readDouble(ref double dest)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            long num = 0L;
            type = this.readInt64(ref num);
            dest = BitConverter.Int64BitsToDouble(num);
            return type;
        }

        public TdrError.ErrorType readDouble(ref double dest, int pos)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            long num = 0L;
            type = this.readInt64(ref num, pos);
            dest = BitConverter.Int64BitsToDouble(num);
            return type;
        }

        public TdrError.ErrorType readFloat(ref float dest)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            int num = 0;
            type = this.readInt32(ref num);
            dest = BitConverter.ToSingle(BitConverter.GetBytes(num), 0);
            return type;
        }

        public TdrError.ErrorType readFloat(ref float dest, int pos)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            int num = 0;
            type = this.readInt32(ref num, pos);
            dest = BitConverter.ToSingle(BitConverter.GetBytes(num), 0);
            return type;
        }

        public TdrError.ErrorType readInt16(ref short dest)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (2 > (this.length - this.position))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
            }
            dest = BitConverter.ToInt16(this.beginPtr, this.position);
            this.position += 2;
            if (this.IsNetEndian && BitConverter.IsLittleEndian)
            {
                dest = IPAddress.NetworkToHostOrder(dest);
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readInt16(ref short dest, int pos)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (2 > (this.length - pos))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
            }
            dest = BitConverter.ToInt16(this.beginPtr, pos);
            if (this.IsNetEndian && BitConverter.IsLittleEndian)
            {
                dest = IPAddress.NetworkToHostOrder(dest);
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readInt32(ref int dest)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (4 > (this.length - this.position))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
            }
            dest = BitConverter.ToInt32(this.beginPtr, this.position);
            this.position += 4;
            if (this.IsNetEndian && BitConverter.IsLittleEndian)
            {
                dest = IPAddress.NetworkToHostOrder(dest);
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readInt32(ref int dest, int pos)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (4 > (this.length - pos))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
            }
            dest = BitConverter.ToInt32(this.beginPtr, pos);
            if (this.IsNetEndian && BitConverter.IsLittleEndian)
            {
                dest = IPAddress.NetworkToHostOrder(dest);
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readInt64(ref long dest)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (8 > (this.length - this.position))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
            }
            dest = BitConverter.ToInt64(this.beginPtr, this.position);
            this.position += 8;
            if (this.IsNetEndian && BitConverter.IsLittleEndian)
            {
                dest = IPAddress.NetworkToHostOrder(dest);
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readInt64(ref long dest, int pos)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (8 > (this.length - pos))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
            }
            dest = BitConverter.ToInt64(this.beginPtr, pos);
            if (this.IsNetEndian && BitConverter.IsLittleEndian)
            {
                dest = IPAddress.NetworkToHostOrder(dest);
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readInt8(ref sbyte dest)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            byte num = 0;
            type = this.readUInt8(ref num);
            dest = (sbyte) num;
            return type;
        }

        public TdrError.ErrorType readInt8(ref sbyte dest, int pos)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            byte num = 0;
            type = this.readUInt8(ref num, pos);
            dest = (sbyte) num;
            return type;
        }

        public T readStruct<T>()
        {
            T local;
            int cb = Marshal.SizeOf(typeof(T));
            if ((this.length - this.position) < cb)
            {
                throw new Exception();
            }
            IntPtr destination = Marshal.AllocHGlobal(cb);
            try
            {
                Marshal.Copy(this.beginPtr, this.position, destination, cb);
                local = (T) Marshal.PtrToStructure(destination, typeof(T));
            }
            finally
            {
                Marshal.FreeHGlobal(destination);
            }
            return local;
        }

        public TdrError.ErrorType readUInt16(ref ushort dest)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            short num = 0;
            type = this.readInt16(ref num);
            dest = (ushort) num;
            return type;
        }

        public TdrError.ErrorType readUInt16(ref ushort dest, int pos)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            short num = 0;
            type = this.readInt16(ref num, pos);
            dest = (ushort) num;
            return type;
        }

        public TdrError.ErrorType readUInt32(ref uint dest)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            int num = 0;
            type = this.readInt32(ref num);
            dest = (uint) num;
            return type;
        }

        public TdrError.ErrorType readUInt32(ref uint dest, int pos)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            int num = 0;
            type = this.readInt32(ref num, pos);
            dest = (uint) num;
            return type;
        }

        public TdrError.ErrorType readUInt64(ref ulong dest)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            long num = 0L;
            type = this.readInt64(ref num);
            dest = (ulong) num;
            return type;
        }

        public TdrError.ErrorType readUInt64(ref ulong dest, int pos)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            long num = 0L;
            type = this.readInt64(ref num, pos);
            dest = (ulong) num;
            return type;
        }

        public TdrError.ErrorType readUInt8(ref byte dest)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (1 > (this.length - this.position))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
            }
            dest = this.beginPtr[this.position++];
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readUInt8(ref byte dest, int pos)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if (1 > (this.length - pos))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
            }
            dest = this.beginPtr[pos];
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readVarInt16(ref short dest)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            dest = 0;
            int num = 0;
            byte num3 = 0;
            do
            {
                if ((this.length - this.position) <= 0)
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                num3 = this.beginPtr[this.position++];
                dest = (short) (dest | ((short) ((num3 & 0x7fL) << (7 * num))));
                num++;
            }
            while ((num3 & 0x80) != 0);
            if ((dest & 1) != 0)
            {
                dest = (short) ((((dest ^ 0xffff) >> 1) & -32769) | ((dest & 1) << 15));
            }
            else
            {
                dest = (short) (((dest >> 1) & -32769) | ((dest & 1) << 15));
            }
            if (!BitConverter.IsLittleEndian)
            {
                dest = IPAddress.NetworkToHostOrder(dest);
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readVarInt32(ref int dest)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            dest = 0;
            int num = 0;
            byte num3 = 0;
            do
            {
                if ((this.length - this.position) <= 0)
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                num3 = this.beginPtr[this.position++];
                dest |= (int) ((num3 & 0x7fL) << (7 * num));
                num++;
            }
            while ((num3 & 0x80) != 0);
            if ((dest & 1) != 0)
            {
                uint num4 = (uint) ((dest ^ -1) >> 1);
                num4 &= 0x7fffffff;
                dest = (int) (num4 | ((dest & 1) << 0x1f));
            }
            else
            {
                uint num5 = (uint) ((dest >> 1) & 0x7fffffff);
                dest = (int) (num5 | ((dest & 1) << 0x1f));
            }
            if (!BitConverter.IsLittleEndian)
            {
                dest = IPAddress.NetworkToHostOrder(dest);
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readVarInt64(ref long dest)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            dest = 0L;
            int num = 0;
            byte num3 = 0;
            do
            {
                if ((this.length - this.position) <= 0)
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                num3 = this.beginPtr[this.position++];
                dest |= (num3 & 0x7fL) << (7 * num);
                num++;
            }
            while ((num3 & 0x80) != 0);
            if ((dest & 1L) != 0)
            {
                ulong num4 = (ulong) ((dest ^ -1L) >> 1);
                num4 &= (ulong) 0x7fffffffffffffffL;
                dest = (long) (num4 | ((ulong) ((dest & 1L) << 0x3f)));
            }
            else
            {
                ulong num5 = (ulong) ((dest >> 1) & 0x7fffffffffffffffL);
                dest = (long) (num5 | ((ulong) ((dest & 1L) << 0x3f)));
            }
            if (!BitConverter.IsLittleEndian)
            {
                dest = IPAddress.NetworkToHostOrder(dest);
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readVarUInt16(ref ushort dest)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            dest = 0;
            int num = 0;
            byte num3 = 0;
            do
            {
                if ((this.length - this.position) <= 0)
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                num3 = this.beginPtr[this.position++];
                dest = (ushort) (dest | ((ushort) ((num3 & 0x7fL) << (7 * num))));
                num++;
            }
            while ((num3 & 0x80) != 0);
            if (!BitConverter.IsLittleEndian)
            {
                dest = (ushort) IPAddress.NetworkToHostOrder((short) dest);
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readVarUInt32(ref uint dest)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            dest = 0;
            int num = 0;
            byte num3 = 0;
            do
            {
                if ((this.length - this.position) <= 0)
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                num3 = this.beginPtr[this.position++];
                dest |= (uint) ((num3 & 0x7fL) << (7 * num));
                num++;
            }
            while ((num3 & 0x80) != 0);
            if (!BitConverter.IsLittleEndian)
            {
                dest = (uint) IPAddress.NetworkToHostOrder((int) dest);
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readVarUInt64(ref ulong dest)
        {
            if (this.beginPtr == null)
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            dest = 0L;
            int num = 0;
            byte num3 = 0;
            do
            {
                if ((this.length - this.position) <= 0)
                {
                    return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
                }
                num3 = this.beginPtr[this.position++];
                dest = (ulong) (dest | ((num3 & 0x7fL) << (7 * num)));
                num++;
            }
            while ((num3 & 0x80) != 0);
            if (!BitConverter.IsLittleEndian)
            {
                dest = (ulong) IPAddress.NetworkToHostOrder((long) dest);
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readWString(ref short[] dest, int count)
        {
            if ((this.beginPtr == null) || (count > dest.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if ((dest == null) || (dest.GetLength(0) == 0))
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if ((2 * count) > (this.length - this.position))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
            }
            for (int i = 0; i < count; i++)
            {
                dest[i] = BitConverter.ToInt16(this.beginPtr, this.position);
                this.position += 2;
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType readWString(ref short[] dest, int count, int pos)
        {
            if ((this.beginPtr == null) || (count > dest.GetLength(0)))
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if ((dest == null) || (dest.GetLength(0) == 0))
            {
                return TdrError.ErrorType.TDR_ERR_ARG_IS_NULL;
            }
            if ((2 * count) > (this.length - pos))
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
            }
            for (int i = 0; i < count; i++)
            {
                dest[i] = BitConverter.ToInt16(this.beginPtr, pos + (2 * count));
            }
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public void reset()
        {
            this.length = 0;
            this.position = 0;
            this.beginPtr = null;
            this.IsNetEndian = true;
        }

        public void set(ref byte[] ptr, int len)
        {
            this.beginPtr = ptr;
            this.position = 0;
            this.length = 0;
            this.IsNetEndian = true;
            if (this.beginPtr != null)
            {
                this.length = len;
            }
        }

        public TdrError.ErrorType skipForward(int step)
        {
            if ((this.length - this.position) < step)
            {
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
            }
            this.position += step;
            return TdrError.ErrorType.TDR_NO_ERROR;
        }

        public TdrError.ErrorType toHexStr(ref string buffer)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            byte[] buffer2 = new byte[this.length - this.position];
            for (int i = 0; i < (this.length - this.position); i++)
            {
                type = this.readUInt8(ref buffer2[i], this.position + i);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    return type;
                }
                buffer = buffer + string.Format("{0:x2}", buffer2[i]);
            }
            buffer = buffer + string.Format("{0:x}", 0);
            return type;
        }

        public TdrError.ErrorType toHexStr(ref char[] buffer, out int usedsize)
        {
            TdrError.ErrorType type = TdrError.ErrorType.TDR_NO_ERROR;
            int num = this.length - this.position;
            int num2 = (num * 2) + 1;
            if (buffer.GetLength(0) < num2)
            {
                usedsize = 0;
                return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_WRITE;
            }
            string str = string.Empty;
            byte[] buffer2 = new byte[this.length - this.position];
            for (int i = 0; i < (this.length - this.position); i++)
            {
                type = this.readUInt8(ref buffer2[i], this.position + i);
                if (type != TdrError.ErrorType.TDR_NO_ERROR)
                {
                    usedsize = 0;
                    return type;
                }
                str = str + string.Format("{0:x2}", buffer2[i]);
            }
            buffer = (str + string.Format("{0:x}", 0)).ToCharArray();
            usedsize = num2;
            return type;
        }
    }
}

