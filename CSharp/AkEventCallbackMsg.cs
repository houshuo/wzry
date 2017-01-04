using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct AkEventCallbackMsg
{
    public AkCallbackType type;
    public GameObject sender;
    public object info;
}

