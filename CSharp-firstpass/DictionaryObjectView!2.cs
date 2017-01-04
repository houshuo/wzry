using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

public class DictionaryObjectView<TKey, TValue> : IEnumerable, IEnumerable<KeyValuePair<TKey, TValue>>
{
    protected Dictionary<object, object> Context;

    public DictionaryObjectView()
    {
        this.Context = new Dictionary<object, object>();
    }

    public DictionaryObjectView(int capacity)
    {
        this.Context = new Dictionary<object, object>(capacity);
    }

    public void Add(TKey key, TValue value)
    {
        this.Context.Add(key, value);
    }

    public void Clear()
    {
        this.Context.Clear();
    }

    public bool ContainsKey(TKey key)
    {
        return this.Context.ContainsKey(key);
    }

    public Enumerator<TKey, TValue> GetEnumerator()
    {
        return new Enumerator<TKey, TValue>(this.Context);
    }

    public bool Remove(TKey key)
    {
        return this.Context.Remove(key);
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        object obj2 = null;
        bool flag = this.Context.TryGetValue(key, out obj2);
        value = (obj2 == null) ? default(TValue) : ((TValue) obj2);
        return flag;
    }

    public int Count
    {
        get
        {
            return this.Context.Count;
        }
    }

    public TValue this[TKey key]
    {
        get
        {
            object obj2 = this.Context[key];
            return ((obj2 == null) ? default(TValue) : ((TValue) obj2));
        }
        set
        {
            this.Context[key] = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Enumerator : IDisposable, IEnumerator, IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private Dictionary<object, object> Reference;
        private Dictionary<object, object>.Enumerator Iter;
        public Enumerator(Dictionary<object, object> InReference)
        {
            this.Reference = InReference;
            this.Iter = this.Reference.GetEnumerator();
        }

        object IEnumerator.Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public KeyValuePair<TKey, TValue> Current
        {
            get
            {
                return new KeyValuePair<TKey, TValue>((this.Iter.Current.Key == null) ? default(TKey) : this.Iter.Current.Key, (this.Iter.Current.Value == null) ? default(TValue) : this.Iter.Current.Value);
            }
        }
        public void Reset()
        {
            this.Iter = this.Reference.GetEnumerator();
        }

        public void Dispose()
        {
            this.Iter.Dispose();
            this.Reference = null;
        }

        public bool MoveNext()
        {
            return this.Iter.MoveNext();
        }
    }
}

