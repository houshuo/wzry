using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct TimeStamp
{
    public ulong startTime;
    public ulong endTime;
    public TimeStamp(ulong startTime, ulong endTime)
    {
        this.startTime = startTime;
        this.endTime = endTime;
    }
}

