namespace Assets.Scripts.Framework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class MultiValueListDictionary<TKey, TValue> : DictionaryView<TKey, ListView<TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            ListView<TValue> view = null;
            if (!base.TryGetValue(key, out view))
            {
                view = new ListView<TValue>();
                base.Add(key, view);
            }
            view.Add(value);
        }

        public ListView<TValue> GetValues(TKey key, bool returnEmptySet = true)
        {
            ListView<TValue> view = null;
            if (!base.TryGetValue(key, out view) && returnEmptySet)
            {
                view = new ListView<TValue>();
            }
            return view;
        }

        public TValue[] GetValuesAll()
        {
            ListLinqView<TValue> view = new ListLinqView<TValue>();
            DictionaryView<TKey, ListView<TValue>>.Enumerator enumerator = base.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<TKey, ListView<TValue>> current = enumerator.Current;
                ListView<TValue> view2 = current.Value;
                if (view2 != null)
                {
                    IEnumerator enumerator2 = view2.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        TValue item = (TValue) enumerator2.Current;
                        view.Add(item);
                    }
                }
            }
            return view.ToArray();
        }
    }
}

