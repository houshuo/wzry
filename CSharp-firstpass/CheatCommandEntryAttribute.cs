using System;
using System.Runtime.CompilerServices;

public class CheatCommandEntryAttribute : AutoRegisterAttribute
{
    public CheatCommandEntryAttribute(string InGroup)
    {
        this.group = InGroup;
    }

    public string group { get; protected set; }
}

