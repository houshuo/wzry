using System;
using System.Threading;

public class LockFreeQueue<T>
{
    private int capacity;
    public int head;
    public T[] items;
    public int tail;

    public LockFreeQueue() : this(0x40)
    {
    }

    public LockFreeQueue(int count)
    {
        this.items = new T[count];
        this.tail = this.head = 0;
        this.capacity = count;
    }

    public void Clear()
    {
        this.head = this.tail = 0;
    }

    public T Dequeue()
    {
        if (this.IsEmpty())
        {
            return default(T);
        }
        int index = this.head % this.capacity;
        T local = this.items[index];
        this.head++;
        return local;
    }

    public void Enqueue(T item)
    {
        while (this.IsFull())
        {
            Thread.Sleep(1);
        }
        int index = this.tail % this.capacity;
        this.items[index] = item;
        this.tail++;
    }

    public bool IsEmpty()
    {
        return (this.head == this.tail);
    }

    private bool IsFull()
    {
        return ((this.tail - this.head) >= this.capacity);
    }

    public int Count
    {
        get
        {
            return (this.tail - this.head);
        }
    }
}

