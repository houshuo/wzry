using System;
using System.Runtime.CompilerServices;

public class ObjectTypeSerializerAttribute : Attribute
{
    public ObjectTypeSerializerAttribute(System.Type serializeType)
    {
        this.type = serializeType;
    }

    public System.Type type { get; private set; }
}

