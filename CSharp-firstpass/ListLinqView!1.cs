using System;

public class ListLinqView<T> : ListView<T>
{
    public ListLinqView()
    {
    }

    public ListLinqView(int capacity) : base(capacity)
    {
    }

    public T[] ToArray()
    {
        T[] localArray = new T[base.Count];
        if (localArray.Length < base.Context.Count)
        {
            throw new ArgumentException("Input array has not enough size.");
        }
        for (int i = 0; i < base.Context.Count; i++)
        {
            localArray[i] = base.Context[i];
        }
        return localArray;
    }
}

