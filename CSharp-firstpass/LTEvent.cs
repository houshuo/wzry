using System;

public class LTEvent
{
    public object data;
    public int id;

    public LTEvent(int id, object data)
    {
        this.id = id;
        this.data = data;
    }
}

