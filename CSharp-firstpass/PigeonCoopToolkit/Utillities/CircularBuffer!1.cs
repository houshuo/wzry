namespace PigeonCoopToolkit.Utillities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class CircularBuffer<T> : IEnumerable, IList<T>, ICollection<T>, IEnumerable<T>
    {
        private T[] _buffer;
        private int _position;
        private long _version;

        public CircularBuffer(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Must be greater than zero", "capacity");
            }
            this.Capacity = capacity;
            this._buffer = new T[capacity];
        }

        public void Add(T item)
        {
            this._buffer[this._position++ % this.Capacity] = item;
            if (this.Count < this.Capacity)
            {
                this.Count++;
            }
            this._version += 1L;
        }

        public void Clear()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this._buffer[i] = default(T);
            }
            this._position = 0;
            this.Count = 0;
            this._version += 1L;
        }

        public bool Contains(T item)
        {
            return (this.IndexOf(item) != -1);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < this.Count; i++)
            {
                array[i + arrayIndex] = this._buffer[((this._position - this.Count) + i) % this.Capacity];
            }
        }

        [DebuggerHidden]
        public IEnumerator<T> GetEnumerator()
        {
            return new <GetEnumerator>c__IteratorF<T> { <>f__this = (CircularBuffer<T>) this };
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < this.Count; i++)
            {
                T local = this._buffer[((this._position - this.Count) + i) % this.Capacity];
                if ((item == null) && (local == null))
                {
                    return i;
                }
                if ((item != null) && item.Equals(local))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            if ((index < 0) || (index > this.Count))
            {
                throw new IndexOutOfRangeException();
            }
            if (index == this.Count)
            {
                this.Add(item);
            }
            else
            {
                int num = Math.Min(this.Count, this.Capacity - 1) - index;
                int num2 = ((this._position - this.Count) + index) % this.Capacity;
                for (int i = num2 + num; i > num2; i--)
                {
                    int num4 = i % this.Capacity;
                    int num5 = (i - 1) % this.Capacity;
                    this._buffer[num4] = this._buffer[num5];
                }
                this._buffer[num2] = item;
                if (this.Count < this.Capacity)
                {
                    this.Count++;
                    this._position++;
                }
                this._version += 1L;
            }
        }

        public bool Remove(T item)
        {
            int index = this.IndexOf(item);
            if (index == -1)
            {
                return false;
            }
            this.RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            if ((index < 0) || (index >= this.Count))
            {
                throw new IndexOutOfRangeException();
            }
            for (int i = index; i < (this.Count - 1); i++)
            {
                int num2 = ((this._position - this.Count) + i) % this.Capacity;
                int num3 = (((this._position - this.Count) + i) + 1) % this.Capacity;
                this._buffer[num2] = this._buffer[num3];
            }
            int num4 = (this._position - 1) % this.Capacity;
            this._buffer[num4] = default(T);
            this._position--;
            this.Count--;
            this._version += 1L;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Capacity { get; private set; }

        public int Count { get; private set; }

        public T this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.Count))
                {
                    throw new IndexOutOfRangeException();
                }
                int num = ((this._position - this.Count) + index) % this.Capacity;
                return this._buffer[num];
            }
            set
            {
                this.Insert(index, value);
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        [CompilerGenerated]
        private sealed class <GetEnumerator>c__IteratorF : IDisposable, IEnumerator, IEnumerator<T>
        {
            internal T $current;
            internal int $PC;
            internal CircularBuffer<T> <>f__this;
            internal int <i>__1;
            internal long <version>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<version>__0 = this.<>f__this._version;
                        this.<i>__1 = 0;
                        break;

                    case 1:
                        this.<i>__1++;
                        break;

                    default:
                        goto Label_00AD;
                }
                if (this.<i>__1 < this.<>f__this.Count)
                {
                    if (this.<version>__0 != this.<>f__this._version)
                    {
                        throw new InvalidOperationException("Collection changed");
                    }
                    this.$current = this.<>f__this[this.<i>__1];
                    this.$PC = 1;
                    return true;
                }
                this.$PC = -1;
            Label_00AD:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            T IEnumerator<T>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

