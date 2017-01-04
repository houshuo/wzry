namespace Assets.Scripts.Framework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class MultiValueHashDictionary<TKey, TValue> : DictionaryView<TKey, HashSet<TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            HashSet<TValue> set = null;
            if (!base.TryGetValue(key, out set))
            {
                set = new HashSet<TValue>();
                base.Add(key, set);
            }
            set.Add(value);
        }

        public TValue[] GetAllValueArray()
        {
            ListLinqView<TValue> view = new ListLinqView<TValue>();
            DictionaryView<TKey, HashSet<TValue>>.Enumerator enumerator = base.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<TKey, HashSet<TValue>> current = enumerator.Current;
                HashSet<TValue> set = current.Value;
                if (set != null)
                {
                    IEnumerator enumerator2 = set.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        TValue item = (TValue) enumerator2.Current;
                        view.Add(item);
                    }
                }
            }
            return view.ToArray();
        }

        public HashSet<TValue> GetValues(TKey key, bool returnEmptySet = true)
        {
            HashSet<TValue> set = null;
            if (!base.TryGetValue(key, out set) && returnEmptySet)
            {
                set = new HashSet<TValue>();
            }
            return set;
        }
    }
}

