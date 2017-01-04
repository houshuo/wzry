using System;
using System.Runtime.CompilerServices;

public class ArgumentAttribute : AutoRegisterAttribute
{
    public ArgumentAttribute(int InOrder)
    {
        this.order = InOrder;
    }

    public int order { get; protected set; }
}

