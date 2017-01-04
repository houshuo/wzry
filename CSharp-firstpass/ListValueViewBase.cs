using System;

public abstract class ListValueViewBase
{
    protected int _size;
    protected int _version;
    protected const int DefaultCapacity = 4;

    protected ListValueViewBase()
    {
    }

    public int Count
    {
        get
        {
            return this._size;
        }
    }
}

