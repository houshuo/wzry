using System;

public class GameTypeAttr : Attribute
{
    public readonly ushort key;
    public readonly Type type;

    public GameTypeAttr(ushort key, Type type)
    {
        this.key = key;
        this.type = type;
    }
}

