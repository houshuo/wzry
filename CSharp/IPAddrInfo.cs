using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct IPAddrInfo
{
    public string ip;
    public string port;
}

