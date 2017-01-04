using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct VLine
{
    public VInt2 point;
    public VInt2 direction;
}

