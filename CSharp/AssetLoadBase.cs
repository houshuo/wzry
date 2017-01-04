using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct AssetLoadBase
{
    public string assetPath;
    public int nInstantiate;
}

