using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

public class ListValueView<T> : ListValueViewBase, IEnumerable, IEnumerable<T>
{
    private T[] _items;
    private static readonly T[] EmptyArray;

    static ListValueView()
    {
        ListValueView<T>.EmptyArray = new T[0];
    }

    public ListValueView()
    {
        this._items = ListValueView<T>.EmptyArray;
    }

    public ListValueView(int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException("capacity");
        }
        this._items = new T[capacity];
    }

    public void Add(T item)
    {
        if (base._size == this._items.Length)
        {
            this.GrowIfNeeded(1);
        }
        this._items[base._size++] = item;
        base._version++;
    }

    private void CheckRange(int idx, int count)
    {
        if (idx < 0)
        {
            throw new ArgumentOutOfRangeException("index");
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException("count");
        }
        if ((idx + count) > base._size)
        {
            throw new ArgumentException("index and count exceed length of list");
        }
    }

    public void Clear()
    {
        Array.Clear(this._items, 0, this._items.Length);
        base._size = 0;
        base._version++;
    }

    public Enumerator<T> GetEnumerator()
    {
        return new Enumerator<T>((ListValueView<T>) this);
    }

    private void GrowIfNeeded(int newCount)
    {
        int num = base._size + newCount;
        if (num > this._items.Length)
        {
            this.Capacity = Math.Max(Math.Max(this.Capacity * 2, 4), num);
        }
    }

    public int IndexOf(T item)
    {
        return Array.IndexOf<T>(this._items, item, 0, base._size);
    }

    public int IndexOf(T item, int index)
    {
        if ((index < 0) || (index > base._size))
        {
            throw new ArgumentOutOfRangeException("index");
        }
        return Array.IndexOf<T>(this._items, item, index, base._size - index);
    }

    public int IndexOf(T item, int index, int count)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException("index");
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException("count");
        }
        if ((index + count) > base._size)
        {
            throw new ArgumentOutOfRangeException("index and count exceed length of list");
        }
        return Array.IndexOf<T>(this._items, item, index, count);
    }

    public void RemoveAt(int index)
    {
        if ((index < 0) || (index >= base._size))
        {
            throw new ArgumentOutOfRangeException("index");
        }
        this.Shift(index, -1);
        Array.Clear(this._items, base._size, 1);
        base._version++;
    }

    public void RemoveRange(int index, int count)
    {
        this.CheckRange(index, count);
        if (count > 0)
        {
            this.Shift(index, -count);
            Array.Clear(this._items, base._size, count);
            base._version++;
        }
    }

    private void Shift(int start, int delta)
    {
        if (delta < 0)
        {
            start -= delta;
        }
        if (start < base._size)
        {
            Array.Copy(this._items, start, this._items, start + delta, base._size - start);
        }
        base._size += delta;
        if (delta < 0)
        {
            Array.Clear(this._items, base._size, -delta);
        }
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public int Capacity
    {
        get
        {
            return this._items.Length;
        }
        set
        {
            if (value < base._size)
            {
                throw new ArgumentOutOfRangeException();
            }
            Array.Resize<T>(ref this._items, value);
        }
    }

    public T this[int index]
    {
        get
        {
            if (index >= base._size)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            return this._items[index];
        }
        set
        {
            if ((index < 0) || (index > base._size))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (index == base._size)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            this._items[index] = value;
        }
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct Enumerator : IDisposable, IEnumerator, IEnumerator<T>
    {
        private ListValueView<T> l;
        private int next;
        private int ver;
        private T current;
        internal Enumerator(ListValueView<T> l)
        {
            this.l = l;
            this.ver = l._version;
        }

        void IEnumerator.Reset()
        {
            this.VerifyState();
            this.next = 0;
        }

        object IEnumerator.Current
        {
            get
            {
                this.VerifyState();
                if (this.next <= 0)
                {
                    throw new InvalidOperationException();
                }
                return this.current;
            }
        }
        public void Dispose()
        {
            this.l = null;
        }

        private void VerifyState()
        {
            if (this.l == null)
            {
                throw new ObjectDisposedException(base.GetType().FullName);
            }
            if (this.ver != this.l._version)
            {
                throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
            }
        }

        public bool MoveNext()
        {
            this.VerifyState();
            if (this.next >= 0)
            {
                if (this.next < this.l._size)
                {
                    this.current = this.l._items[this.next++];
                    return true;
                }
                this.next = -1;
            }
            return false;
        }

        public T Current
        {
            get
            {
                return this.current;
            }
        }
    }
}

