namespace Apollo
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    public class ApolloBufferReader
    {
        private byte[] buffer;
        private int position;

        public ApolloBufferReader()
        {
        }

        public ApolloBufferReader(byte[] bs)
        {
            this.buffer = bs;
        }

        public bool Read(ref bool b)
        {
            byte c = !b ? ((byte) 0) : ((byte) 1);
            return (this.Read(ref c) != 0);
        }

        public byte Read(ref byte c)
        {
            if ((this.buffer == null) || (this.position >= this.buffer.Length))
            {
                return 0;
            }
            c = this.buffer[this.position];
            this.position++;
            return c;
        }

        public byte[] Read(ref byte[] buf)
        {
            if ((this.buffer != null) && (this.position < this.buffer.Length))
            {
                int v = 0;
                this.Read(ref v);
                if (v > 0)
                {
                    buf = new byte[v];
                    Array.Copy(this.buffer, this.position, buf, 0, v);
                    this.position += v;
                    return buf;
                }
            }
            return null;
        }

        public ApolloBufferBase Read(ref ApolloBufferBase ab)
        {
            if (ab != null)
            {
                ab.ReadFrom(this);
            }
            return ab;
        }

        public IDictionary<K, V> Read<K, V>(ref IDictionary<K, V> map)
        {
            return (this.ReadMap<IDictionary<K, V>>(ref map) as IDictionary<K, V>);
        }

        public short Read(ref short v)
        {
            if ((this.buffer == null) || (this.position >= this.buffer.Length))
            {
                return 0;
            }
            v = BitConverter.ToInt16(this.buffer, this.position);
            this.position += 2;
            v = ByteConverter.ReverseEndian(v);
            return v;
        }

        public int Read(ref int v)
        {
            if ((this.buffer == null) || (this.position >= this.buffer.Length))
            {
                return 0;
            }
            v = BitConverter.ToInt32(this.buffer, this.position);
            this.position += 4;
            v = ByteConverter.ReverseEndian(v);
            return v;
        }

        public IList<T> Read<T>(ref IList<T> v)
        {
            return this.Read<T>(ref v);
        }

        public long Read(ref long v)
        {
            if ((this.buffer == null) || (this.position >= this.buffer.Length))
            {
                return 0L;
            }
            v = BitConverter.ToInt64(this.buffer, this.position);
            this.position += 8;
            v = ByteConverter.ReverseEndian(v);
            return v;
        }

        public object Read<T>(ref T o)
        {
            if (((T) o) == null)
            {
                o = (T) BasicClassTypeUtil.CreateObject<T>();
            }
            if ((((T) o) is byte) || (((T) o) is char))
            {
                byte c = 0;
                o = (T) this.Read(ref c);
            }
            else if (((T) o) is char)
            {
                byte num2 = 0;
                o = (T) this.Read(ref num2);
            }
            else if (((T) o) is bool)
            {
                bool b = false;
                o = (T) this.Read(ref b);
            }
            else if (((T) o) is short)
            {
                short v = 0;
                o = (T) this.Read(ref v);
            }
            else if (((T) o) is ushort)
            {
                ushort num4 = 0;
                o = (T) this.Read(ref num4);
            }
            else if (((T) o) is int)
            {
                int num5 = 0;
                o = (T) this.Read(ref num5);
            }
            else
            {
                if (((T) o) is uint)
                {
                    uint num6 = 0;
                    o = (T) this.Read(ref num6);
                    return (T) o;
                }
                if (((T) o) is long)
                {
                    long num7 = 0L;
                    o = (T) this.Read(ref num7);
                    return (T) o;
                }
                if (((T) o) is Enum)
                {
                    int num8 = 0;
                    o = (T) this.Read(ref num8);
                    return (T) o;
                }
                if (((T) o) is ulong)
                {
                    ulong num9 = 0L;
                    object obj2 = this.Read(ref num9);
                    o = (T) obj2;
                    return obj2;
                }
                if (((T) o) is string)
                {
                    string s = string.Empty;
                    o = (T) this.Read(ref s);
                }
                else if (((T) o) is ApolloBufferBase)
                {
                    ApolloBufferBase ab = ((T) o) as ApolloBufferBase;
                    o = (T) this.Read(ref ab);
                }
                else if ((((T) o) == null) || !o.GetType().IsArray)
                {
                    if (((T) o) is IList)
                    {
                        return this.ReadList<T>(ref o);
                    }
                    if (((T) o) is IDictionary)
                    {
                        return this.ReadMap<T>(ref o);
                    }
                    object[] objArray1 = new object[] { "read object error: unsupport type:", o.GetType(), " value:", o.ToString() };
                    throw new Exception(string.Concat(objArray1));
                }
            }
            return (T) o;
        }

        public string Read(ref string s)
        {
            if ((this.buffer != null) && (this.position < this.buffer.Length))
            {
                byte[] buf = null;
                buf = this.Read(ref buf);
                if (buf != null)
                {
                    s = Encoding.UTF8.GetString(buf);
                    return s;
                }
            }
            return null;
        }

        public ushort Read(ref ushort v)
        {
            if ((this.buffer == null) || (this.position >= this.buffer.Length))
            {
                return 0;
            }
            v = BitConverter.ToUInt16(this.buffer, this.position);
            this.position += 2;
            v = ByteConverter.ReverseEndian(v);
            return v;
        }

        public uint Read(ref uint v)
        {
            if ((this.buffer == null) || (this.position >= this.buffer.Length))
            {
                return 0;
            }
            v = BitConverter.ToUInt32(this.buffer, this.position);
            this.position += 4;
            v = ByteConverter.ReverseEndian(v);
            return v;
        }

        public ulong Read(ref ulong v)
        {
            if ((this.buffer == null) || (this.position >= this.buffer.Length))
            {
                return 0L;
            }
            v = BitConverter.ToUInt64(this.buffer, this.position);
            this.position += 8;
            v = ByteConverter.ReverseEndian(v);
            return v;
        }

        public IList ReadList<T>(ref T l)
        {
            int v = 0;
            this.Read(ref v);
            IList list = ((T) l) as IList;
            if (list == null)
            {
                ADebug.LogError("ReadList list == null");
                return null;
            }
            list.Clear();
            for (int i = 0; i < v; i++)
            {
                object o = BasicClassTypeUtil.CreateListItem(list.GetType());
                this.Read<object>(ref o);
                list.Add(o);
            }
            return list;
        }

        public IDictionary ReadMap<T>(ref T map)
        {
            IDictionary dictionary = BasicClassTypeUtil.CreateObject(map.GetType()) as IDictionary;
            if (dictionary == null)
            {
                return null;
            }
            dictionary.Clear();
            int v = 0;
            this.Read(ref v);
            if (v <= 0)
            {
                return null;
            }
            Type[] genericArguments = dictionary.GetType().GetGenericArguments();
            if ((genericArguments == null) || (genericArguments.Length < 2))
            {
                return null;
            }
            for (int i = 0; i < v; i++)
            {
                object o = BasicClassTypeUtil.CreateObject(genericArguments[0]);
                object obj3 = BasicClassTypeUtil.CreateObject(genericArguments[1]);
                o = this.Read<object>(ref o);
                obj3 = this.Read<object>(ref obj3);
                dictionary.Add(o, obj3);
            }
            map = (T) dictionary;
            return dictionary;
        }

        public void Reset()
        {
            this.buffer = null;
            this.position = 0;
        }
    }
}

