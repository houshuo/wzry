using System;
using System.Runtime.CompilerServices;

public class ComponentTypeSerializerAttribute : Attribute
{
    public ComponentTypeSerializerAttribute(System.Type serializeType)
    {
        this.type = serializeType;
    }

    public System.Type type { get; private set; }
}

