using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

public class LuaObjectMap
{
    private List<object> list = new List<object>(0x400);
    private Queue<int> pool = new Queue<int>(0x400);

    public int Add(object obj)
    {
        int num = -1;
        if (this.pool.Count > 0)
        {
            num = this.pool.Dequeue();
            this.list[num] = obj;
            return num;
        }
        this.list.Add(obj);
        return (this.list.Count - 1);
    }

    public object Remove(int index)
    {
        if ((index < 0) || (index >= this.list.Count))
        {
            return null;
        }
        object obj2 = this.list[index];
        if (obj2 != null)
        {
            this.pool.Enqueue(index);
        }
        this.list[index] = null;
        return obj2;
    }

    public bool TryGetValue(int index, out object obj)
    {
        if ((index >= 0) && (index < this.list.Count))
        {
            obj = this.list[index];
            return (obj != null);
        }
        obj = null;
        return false;
    }

    public object this[int i]
    {
        get
        {
            return this.list[i];
        }
    }
}

