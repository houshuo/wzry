namespace Assets.Scripts.Framework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class MultiMap<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            List<TValue> list = null;
            if (!this.TryGetValue(key, out list))
            {
                list = new List<TValue>();
                base.Add(key, list);
            }
            list.Add(value);
        }

        public int GetCountAll()
        {
            int num = 0;
            Dictionary<TKey, List<TValue>>.Enumerator enumerator = base.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<TKey, List<TValue>> current = enumerator.Current;
                List<TValue> list = current.Value;
                if (list != null)
                {
                    num += list.Count;
                }
            }
            return num;
        }

        public Iterator<TKey, TValue> GetIterator()
        {
            return new Iterator<TKey, TValue>((MultiMap<TKey, TValue>) this);
        }

        public List<TValue> GetValues(TKey key, bool returnEmptySet = true)
        {
            List<TValue> list = null;
            if (!base.TryGetValue(key, out list) && returnEmptySet)
            {
                list = new List<TValue>();
            }
            return list;
        }

        public TValue[] GetValuesAll()
        {
            List<TValue> list = new List<TValue>();
            Dictionary<TKey, List<TValue>>.Enumerator enumerator = base.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<TKey, List<TValue>> current = enumerator.Current;
                List<TValue> list2 = current.Value;
                if (list2 != null)
                {
                    IEnumerator enumerator2 = list2.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        TValue item = (TValue) enumerator2.Current;
                        list.Add(item);
                    }
                }
            }
            return list.ToArray();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Iterator
        {
            private Dictionary<TKey, List<TValue>>.Enumerator m_outerEnumerator;
            private int m_valIndex;
            private MultiMap<TKey, TValue> m_multiMap;
            private bool m_bEnd;
            public Iterator(MultiMap<TKey, TValue> inMultiMap)
            {
                this.m_outerEnumerator = inMultiMap.GetEnumerator();
                this.m_valIndex = -1;
                this.m_multiMap = inMultiMap;
                this.m_bEnd = false;
            }

            public bool MoveNext()
            {
                if (this.m_bEnd)
                {
                    return false;
                }
                if ((this.m_valIndex < 0) && !this.m_outerEnumerator.MoveNext())
                {
                    this.m_bEnd = true;
                    return false;
                }
                this.m_valIndex++;
                if (this.m_valIndex >= this.m_outerEnumerator.Current.Value.Count)
                {
                    if (!this.m_outerEnumerator.MoveNext())
                    {
                        this.m_bEnd = true;
                        return false;
                    }
                    this.m_valIndex = 0;
                }
                return true;
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    TKey key = this.m_outerEnumerator.Current.Key;
                    List<TValue> list = this.m_outerEnumerator.Current.Value;
                    if ((this.m_valIndex < 0) || (this.m_valIndex >= list.Count))
                    {
                        DebugHelper.Assert(false);
                    }
                    return new KeyValuePair<TKey, TValue>(key, list[this.m_valIndex]);
                }
            }
            public void Reset()
            {
                this.m_outerEnumerator = this.m_multiMap.GetEnumerator();
                this.m_valIndex = -1;
                this.m_bEnd = false;
            }

            public bool IsEnd()
            {
                return this.m_bEnd;
            }
        }
    }
}

