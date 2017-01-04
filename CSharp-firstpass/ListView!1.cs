using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

public class ListView<T> : ListViewBase, IEnumerable, IEnumerable<T>
{
    public ListView()
    {
        base.Context = new List<object>();
    }

    public ListView(int capacity)
    {
        base.Context = new List<object>(capacity);
    }

    public ListView(IEnumerable<T> collection)
    {
        base.Context = new List<object>();
        IEnumerator<T> enumerator = collection.GetEnumerator();
        while (enumerator.MoveNext())
        {
            base.Context.Add(enumerator.Current);
        }
    }

    public void Add(T item)
    {
        base.Context.Add(item);
    }

    public void AddRange(IEnumerable<T> collection)
    {
        if (collection != null)
        {
            IEnumerator<T> enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                base.Context.Add(enumerator.Current);
            }
        }
    }

    public int BinarySearch(T item, IComparer<T> comparer)
    {
        return base.Context.BinarySearch(item, new ComparerConverter<T>(comparer));
    }

    public bool Contains(T item)
    {
        return base.Context.Contains(item);
    }

    public Enumerator<T> GetEnumerator()
    {
        return new Enumerator<T>(base.Context);
    }

    public int IndexOf(T item)
    {
        return base.Context.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        base.Context.Insert(index, item);
    }

    public int LastIndexOf(T item)
    {
        return base.Context.LastIndexOf(item);
    }

    public bool Remove(T item)
    {
        return base.Context.Remove(item);
    }

    public void Sort(IComparer<T> comparer)
    {
        base.Context.Sort(new ComparerConverter<T>(comparer));
    }

    public void Sort(Comparison<T> comparison)
    {
        base.Context.Sort(new ComparisonConverter<T>(comparison));
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public T this[int index]
    {
        get
        {
            return base.Context[index];
        }
        set
        {
            base.Context[index] = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ComparerConverter : IComparer<object>
    {
        private IComparer<T> ComparerRef;
        public ComparerConverter(IComparer<T> comparer)
        {
            this.ComparerRef = comparer;
        }

        public int Compare(object x, object y)
        {
            return this.ComparerRef.Compare((T) x, (T) y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ComparisonConverter : IComparer<object>
    {
        private Comparison<T> ComparerRef;
        public ComparisonConverter(Comparison<T> comparer)
        {
            this.ComparerRef = comparer;
        }

        public int Compare(object x, object y)
        {
            return this.ComparerRef((T) x, (T) y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Enumerator : IDisposable, IEnumerator, IEnumerator<T>
    {
        private List<object> Reference;
        private List<object>.Enumerator Iter;
        public Enumerator(List<object> InReference)
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
        public T Current
        {
            get
            {
                return this.Iter.Current;
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

