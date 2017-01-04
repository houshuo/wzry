using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FriendlyName : PropertyAttribute
{
    public FriendlyName(string InDisplayName)
    {
        this.friendlyName = InDisplayName;
    }

    public string friendlyName { get; protected set; }
}

