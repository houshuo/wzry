namespace Apollo
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class ApolloBufferWriter
    {
        private MemoryStream stream;
        private BinaryWriter writer;

        public ApolloBufferWriter()
        {
            this.stream = new MemoryStream(0x80);
            this.writer = new BinaryWriter(this.stream, Encoding.BigEndianUnicode);
        }

        public ApolloBufferWriter(int capacity)
        {
            this.stream = new MemoryStream(capacity);
            this.writer = new BinaryWriter(this.stream, Encoding.BigEndianUnicode);
        }

        public ApolloBufferWriter(MemoryStream ms)
        {
            this.stream = ms;
            this.writer = new BinaryWriter(this.stream, Encoding.BigEndianUnicode);
        }

        public byte[] GetBufferData()
        {
            byte[] destinationArray = new byte[this.stream.Position];
            Array.Copy(this.stream.GetBuffer(), 0L, destinationArray, 0L, this.stream.Position);
            return destinationArray;
        }

        private void Reserve(int len)
        {
            int num = this.stream.Capacity - ((int) this.stream.Length);
            if (num < len)
            {
                this.stream.Capacity = (this.stream.Capacity + len) << 1;
            }
        }

        public void Write(ApolloBufferBase ab)
        {
            if (ab != null)
            {
                ab.WriteTo(this);
            }
        }

        public void Write(bool b)
        {
            this.Write(!b ? ((byte) 0) : ((byte) 1));
        }

        public void Write(byte c)
        {
            this.Reserve(1);
            this.writer.Write(c);
        }

        public void Write(short s)
        {
            this.Reserve(2);
            this.writer.Write(ByteConverter.ReverseEndian(s));
        }

        public void Write<K, V>(Dictionary<K, V> d)
        {
            if (d != null)
            {
                int count = d.Count;
                this.Write(count);
                foreach (KeyValuePair<K, V> pair in d)
                {
                    this.Write(pair.Key);
                    this.Write(pair.Value);
                }
            }
            else
            {
                this.Write(0);
            }
        }

        public void Write(int i)
        {
            this.Reserve(4);
            this.writer.Write(ByteConverter.ReverseEndian(i));
        }

        public void Write<T>(List<T> v)
        {
            int i = (v == null) ? 0 : v.Count;
            this.Write(i);
            if (v != null)
            {
                for (int j = 0; j < v.Count; j++)
                {
                    this.Write(v[j]);
                }
            }
        }

        public void Write(long l)
        {
            this.Reserve(8);
            this.writer.Write(ByteConverter.ReverseEndian(l));
        }

        public void Write(byte[] buffer)
        {
            if (buffer != null)
            {
                this.Write(buffer.Length);
                this.writer.Write(buffer);
            }
            else
            {
                this.Write(0);
            }
        }

        public void Write(object o)
        {
            if (o is byte)
            {
                this.Write((byte) o);
            }
            else if (o is bool)
            {
                this.Write((bool) o);
            }
            else if (o is short)
            {
                this.Write((short) o);
            }
            else if (o is ushort)
            {
                this.Write((int) ((ushort) o));
            }
            else if (o is int)
            {
                this.Write((int) o);
            }
            else if (o is uint)
            {
                this.Write((long) ((uint) o));
            }
            else if (o is long)
            {
                this.Write((long) o);
            }
            else if (o is ulong)
            {
                this.Write((long) ((ulong) o));
            }
            else if (o is float)
            {
                this.Write((float) o);
            }
            else if (o is double)
            {
                this.Write((double) o);
            }
            else if (o is string)
            {
                string s = o as string;
                this.Write(s);
            }
            else if (o is ApolloBufferBase)
            {
                this.Write((ApolloBufferBase) o);
            }
            else if (o is byte[])
            {
                this.Write((byte[]) o);
            }
            else if (o is bool[])
            {
                this.Write((bool[]) o);
            }
            else if (o is short[])
            {
                this.Write((short[]) o);
            }
            else if (o is int[])
            {
                this.Write((int[]) o);
            }
            else if (o is long[])
            {
                this.Write((long[]) o);
            }
            else if (o is float[])
            {
                this.Write((float[]) o);
            }
            else if (o is double[])
            {
                this.Write((double[]) o);
            }
            else if (o.GetType().IsArray)
            {
                this.Write((object[]) o);
            }
            else if (o is IList)
            {
                this.Write((IList) o);
            }
            else if (o is IDictionary)
            {
                this.Write((IDictionary) o);
            }
            else
            {
                if (!(o is IEnumerable))
                {
                    throw new Exception("write object error: unsupport type. " + o.ToString() + "\n");
                }
                this.Write((int) o);
            }
        }

        public void Write(string s)
        {
            byte[] buffer = ByteConverter.String2Bytes(s);
            if (buffer == null)
            {
                buffer = new byte[0];
            }
            int length = buffer.Length;
            this.Reserve(length + 4);
            this.Write(length);
            if (buffer.Length > 0)
            {
                this.writer.Write(buffer);
            }
        }

        public void Write(ushort s)
        {
            this.Reserve(2);
            this.writer.Write(ByteConverter.ReverseEndian((short) s));
        }

        public void Write(uint i)
        {
            this.Reserve(4);
            this.writer.Write(ByteConverter.ReverseEndian(i));
        }

        public void Write(ulong l)
        {
            this.Reserve(8);
            this.writer.Write(ByteConverter.ReverseEndian(l));
        }
    }
}

