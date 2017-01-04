using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct TdirSvrGroup
{
    public List<TdirUrl> tdirUrls;
    public string name;
    public string appAttr;
    public int nodeID;
}

